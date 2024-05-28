// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using QuickTest.MigrateLegacy.epi;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Kiota.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using QuickTest.Data.Models;
using QuickTest.Data.Models.Measurements;
using QuickTest.Data.Models.Wafers;
using QuickTest.Data.Models.Wafers.Enums;
using QuickTest.Infrastructure.Services;


//await PrintProperties();

//await Migrate();

/*QuickTestDataService service=new(clientWork,"quick_test_db");
var waferList=await service.GetWaferList();*/

//Console.WriteLine($"Wafer Count: {waferList.Count}");
//await CreateWaferList();
/*await GetWaferListV1();*/
//await GetWaferListV2();
//await GetInitial();
//await QuickTestV2Migrate();
//await TestGenerated();
//await TestQtV2();
//await GetInitialResults();

//await Migrate();
//await CreateHelperCollections();

await TestGetWaferPads();

async Task TestGetWaferPads() {
    var client=new MongoClient("mongodb://172.20.3.41:27017");
    var database=client.GetDatabase("quick_test_db");
    var initialCollection=database.GetCollection<QtMeasurement>("init_measurements");
    var waferPadCollection=database.GetCollection<WaferPad>("wafer_pads");
    var tested = await initialCollection.Find(e => e.WaferId == "B01-3482-10").Project(e => e.Pad).ToListAsync();
    /*List<string?> pads = ["A","B","G6-E"];*/
    var result=await waferPadCollection.Find(e=>tested.Contains(e.Identifier)).ToListAsync();
    foreach (var pad in result) {
        Console.WriteLine($"Pad: {pad.Identifier} SvgX: {pad.SvgObject.X} SvgY: {pad.SvgObject.Y} SvgR: {pad.SvgObject.Radius}" );
    }
}

async Task Migrate() {
    var context = new EpiContext();
    var clientWork=new MongoClient("mongodb://172.20.3.41:27017");
    var clientPi=new MongoClient("mongodb://192.168.68.112:27017");
    var databaseWork=clientWork.GetDatabase("quick_test_db");
    var databasePi=clientPi.GetDatabase("quick_test_db");
    var qtCollectionWork=databaseWork.GetCollection<QuickTestResult>("quick_test");
    var initMeasureCollection=databaseWork.GetCollection<QtMeasurement>("init_measurements");
    var finalMeasureCollection=databaseWork.GetCollection<QtMeasurement>("final_measurements");
    var initSpectCollection=databaseWork.GetCollection<Spectrum>("init_spectrum");
    var finalSpectCollection=databaseWork.GetCollection<Spectrum>("final_spectrum");
    await qtCollectionWork.Indexes.CreateOneAsync(new CreateIndexModel<QuickTestResult>(Builders<QuickTestResult>.IndexKeys.Ascending(e => e.WaferId)));
    
    Console.WriteLine("Starting Migration...");
    var start=new DateTime(2023,1,1);
    var wafers=await context.EpiDataInitials.Where(e=>e.DateTime>=start).Select(e=>e.WaferId)
        .Distinct().ToListAsync();
    Console.WriteLine($"Wafer Count:{wafers.Count()}");
    int saveCounter=0;
    int count = 0;
    List<QuickTestResult> results=new();
    List<Spectrum> initSpectrumResults=new();
    List<QtMeasurement> initMeasureResults=new();
    List<Spectrum> finalSpectResults=new();
    List<QtMeasurement> finalMeasureResults=new();
    foreach(var wafer in wafers) {
        var result=await CreateQuickTestResult(context,wafer);
        if (result.qt != null) {
            if(result.initSpect!=null) {
                initSpectrumResults.AddRange(result.initSpect);
            }
            if(result.finalSpect!=null) {
                finalSpectResults.AddRange(result.finalSpect);
            }
            if(result.initMeas!=null) {
                initMeasureResults.AddRange(result.initMeas);
            }
            if(result.finalMeas!=null) {
                finalMeasureResults.AddRange(result.finalMeas);
            }
            results.Add(result.qt);
            saveCounter++;
            count++;
            Console.WriteLine($"Created QuickTestResult for wafer: {wafer} Count:{count}");
        }
        if(saveCounter>=100) {
            Console.WriteLine("Saving 100 records...");
            await qtCollectionWork.InsertManyAsync(results);
            
            if(initMeasureResults.Any()) {
                await initMeasureCollection.InsertManyAsync(initMeasureResults);
            }
            if(finalMeasureResults.Any()) {
                await finalMeasureCollection.InsertManyAsync(finalMeasureResults);
            }
            if (initSpectrumResults.Any()) {
                await initSpectCollection.InsertManyAsync(initSpectrumResults);
            }
            if (finalSpectResults.Any()) {
                await finalSpectCollection.InsertManyAsync(finalSpectResults);
            }
            initSpectrumResults.Clear();
            finalSpectResults.Clear();
            initMeasureResults.Clear();
            finalMeasureResults.Clear();
            results.Clear();
            saveCounter=0;
            Console.WriteLine("Collecting next 100");
        }
    }

    if (results.Any()) {
        Console.WriteLine("Saving leftover records...");
        await qtCollectionWork.InsertManyAsync(results);
        results.Clear();
        Console.WriteLine("Completed");
    }

    Console.WriteLine("Migration Completed, check database...");
}

async Task CreateHelperCollections() {
    var client=new MongoClient("mongodb://172.20.3.41:27017");
    var database=client.GetDatabase("quick_test_db");
    var currentCollection = database.GetCollection<MeasurementCurrent>("measure_current");
    await currentCollection.Indexes.CreateOneAsync(new CreateIndexModel<MeasurementCurrent>(Builders<MeasurementCurrent>.IndexKeys.Ascending(e => e.Name)));
    var stationCollection = database.GetCollection<ProbeStation>("probe_stations");
    await stationCollection.Indexes.CreateOneAsync(new CreateIndexModel<ProbeStation>(Builders<ProbeStation>.IndexKeys.Ascending(e => e.StationNumber)));
    
    ProbeStation station1 = new ProbeStation() {
        _id = ObjectId.GenerateNewId(),
        Name = "Qt Main Station",
        StationNumber = 1
    };
    ProbeStation station2 = new ProbeStation() {
        _id = ObjectId.GenerateNewId(),
        Name = "Qt Secondary Station",
        StationNumber = 2
    };

    MeasurementCurrent _20mA = new MeasurementCurrent() { _id = ObjectId.GenerateNewId(), Name = "20mA", Value = 20 };
    MeasurementCurrent _50mA = new MeasurementCurrent() { _id = ObjectId.GenerateNewId(), Name = "50mA", Value = 50 };

    await currentCollection.InsertOneAsync(_20mA);
    await currentCollection.InsertOneAsync(_50mA);
    await stationCollection.InsertOneAsync(station1);
    await stationCollection.InsertOneAsync(station2);
    Console.WriteLine("Current and Station Collection Created");

}

async Task GetInitialResults() {
    var clientWork=new MongoClient("mongodb://172.20.3.41:27017");
    var database=clientWork.GetDatabase("quick_test_db");
    var qtCollection=database.GetCollection<QuickTestResult>("quick_test");
    var qtCollectionWork=database.GetCollection<QuickTestResult>("quick_test");
    var initMeasureCollection=database.GetCollection<QtMeasurement>("init_measurements");
    var finalMeasureCollection=database.GetCollection<QtMeasurement>("final_measurements");
    var initSpectCollection=database.GetCollection<Spectrum>("init_spectrum");
    var finalSpectCollection=database.GetCollection<Spectrum>("final_spectrum");

    var qt = await qtCollection.Find(e => e.WaferId == "13-2487")
        .FirstOrDefaultAsync();

    if (qt != null) {
        var initMeasurements = await initMeasureCollection.Find(e => e.QuickTestResultId == qt._id).ToListAsync();
        if (initMeasurements.Any()) {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat($"TimeStamp");
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                builder.AppendFormat($"\t{pad}_Voltage\t{pad}_Power\t{pad}_Wl\t{pad}_Ir\t{pad}_Knee");
            }
            builder.AppendLine();
            builder.AppendFormat($"{qt.InitialTimeStamp}");
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                var measurement = initMeasurements.FirstOrDefault(e => e.Pad != null && e.Pad.Contains(pad));
                if(measurement!=null) {
                    builder.AppendFormat($"\t{measurement.Voltage}\t{measurement.Power}\t{measurement.Wl}\t{measurement.Ir}\t{measurement.Knee}");
                } else {
                    int zero = 0;
                    builder.AppendFormat($"\t{zero}\t{zero}\t{zero}\t{zero}\t{zero}");
                }
            }

            Console.WriteLine("Check file");
            
            await File.WriteAllTextAsync("C:\\Users\\aelmendo\\Documents\\QtQueryTests\\initTests.txt",builder.ToString());
        }
    } else {
        Console.WriteLine("Failed to find wafer");
    }
}

/*async Task GetWaferListV2() {
    Stopwatch stopwatch = new();
    var clientWork=new MongoClient("mongodb://172.20.3.41:27017");
    var database=clientWork.GetDatabase("quick_test_db");
    var waferCollection=database.GetCollection<Wafer>("wafers");
    stopwatch.Start();
    var waferList = await waferCollection.Find(_ => true).ToListAsync();
    var count = 0;
    foreach(var wafer in waferList) {
        var split=wafer.WaferId.Split('-');
        if (split.Length == 3) {
            var update=Builders<Wafer>.Update.Set(e => e.SystemId, split[0])
                .Set(e => e.RunNumber, split[1])
                .Set(e => e.Pocket, split[2])
                .Set(e=>e.VersionId,"unknown");
            await waferCollection.UpdateOneAsync(e=>e.WaferId==wafer.WaferId,update);
        } else if (split.Length == 2){
            var update=Builders<Wafer>.Update.Set(e => e.SystemId, split[0])
                .Set(e => e.RunNumber, split[1])
                .Set(e => e.Pocket, "0")
                .Set(e=>e.VersionId,"unknown");
            await waferCollection.UpdateOneAsync(e=>e.WaferId==wafer.WaferId,update);
        } else if (split.Length == 1){
            var update=Builders<Wafer>.Update.Set(e => e.SystemId, split[0])
                .Set(e => e.RunNumber, "0")
                .Set(e => e.Pocket, "0")
                .Set(e=>e.VersionId,"unknown");
            await waferCollection.UpdateOneAsync(e=>e.WaferId==wafer.WaferId,update);
        } else {
            var update=Builders<Wafer>.Update.Set(e => e.SystemId,wafer.WaferId)
                .Set(e => e.RunNumber, "0")
                .Set(e => e.Pocket, "0")
                .Set(e=>e.VersionId,"unknown");
            await waferCollection.UpdateOneAsync(e=>e.WaferId==wafer.WaferId,update);
        }
        count++;
        Console.WriteLine($"Updated Wafer: {wafer.WaferId} Count: {count}");
    }
    /*stopwatch.Stop();
    Console.WriteLine($"Wafer Count: {waferList.Count()} Elapsed: {(double)stopwatch.ElapsedMilliseconds/1000}");#1#
}*/

async Task GetWaferListV1() {
    Stopwatch stopwatch = new();
    var clientWork=new MongoClient("mongodb://172.20.3.41:27017");
    var database=clientWork.GetDatabase("quick_test_db");
    var qtCollection = database.GetCollection<QuickTestResult>("quick_test");
    stopwatch.Start();
    var waferList=await qtCollection.Find(e=>e.WaferId!=null).Project(e=>e.WaferId).ToListAsync();
    stopwatch.Stop();
    Console.WriteLine($"Wafer Count: {waferList.Count()} Elapsed: {(double)stopwatch.ElapsedMilliseconds/1000}");
}

/*async Task CreateWaferList() {
    var clientWork=new MongoClient("mongodb://172.20.3.41:27017");
    var database=clientWork.GetDatabase("quick_test_db");
    var qtCollection=database.GetCollection<QuickTestResult>("quick_test");
    var waferCollection=database.GetCollection<Wafer>("wafers");
    var waferList=await qtCollection.Find(e=>e.WaferId!=null).Project(e=>e.WaferId).ToListAsync();
    await waferCollection.InsertManyAsync(waferList.Select(e=>new Wafer(){WaferId = e ?? "not set"}));
}*/

async Task<(QuickTestResult? qt,List<QtMeasurement>? initMeas,List<QtMeasurement>? finalMeas,List<Spectrum>? initSpect,List<Spectrum>? finalSpect)> CreateQuickTestResult(EpiContext context, string waferId) {
    var qtId = ObjectId.GenerateNewId();
    var initialMeasurements = await CreateInitialMeasurements(context, waferId,qtId);
    var finalMeasurements = await CreateFinalMeasurements(context, waferId,qtId);
    if(initialMeasurements.measurements==null && finalMeasurements.measurements==null) {
        return (null,null,null,null,null);
    }
    
    var qt=new QuickTestResult() {
        _id = qtId,
        WaferId = waferId,
        ProbeStationId = 1,
        InitialTimeStamp = initialMeasurements.timeStamp,
        FinalTimeStamp = finalMeasurements.timeStamp,
    };
    var initSpectrum = await CreateInitialSpectrum(context, waferId, qt._id);
    var finalSpectrum=await CreateFinalSpectrum(context,waferId, qt._id);
    return (qt,initialMeasurements.measurements,finalMeasurements.measurements,initSpectrum,finalSpectrum);
}

async Task<(List<QtMeasurement>? measurements,DateTime timeStamp)> CreateInitialMeasurements(EpiContext context, string waferId, ObjectId qtId) {
        var initialData = await context.EpiDataInitials.FirstOrDefaultAsync(e => e.WaferId == waferId);
    var initial50mA= await context.EpiDataInitial50mas.FirstOrDefaultAsync(e => e.WaferId == waferId);
    var initialMeasurements = new List<QtMeasurement>();
    if (initialData != null) {
        if (initialData.CenterAPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,initialData.CenterAPower,initialData.CenterAVolt,
                initialData.CenterAWl,initialData.CenterAReverse ?? 0,initialData.CenterAKnee,PadLocation.PadLocationA.Value));
        }

        
        if(initialData.CenterBPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,initialData.CenterBPower,initialData.CenterBVolt,
                initialData.CenterBWl,initialData.CenterBReverse ?? 0,initialData.CenterBKnee,PadLocation.PadLocationB.Value));
        }
        
        if(initialData.CenterCPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,initialData.CenterCPower,initialData.CenterCVolt,
                initialData.CenterCWl,initialData.CenterCReverse ?? 0,initialData.CenterCKnee,PadLocation.PadLocationC.Value));
        }
        
        if(initialData.CenterDPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,initialData.CenterDPower,initialData.CenterDVolt,
                initialData.CenterDWl,initialData.CenterDReverse ?? 0,initialData.CenterDKnee,PadLocation.PadLocationD.Value));
        }
        
        if(initialData.TopPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,initialData.TopPower,initialData.TopVolt,
                initialData.TopWl,initialData.TopReverse ?? 0,initialData.TopKnee,"T2-E"));
        }
        
        if(initialData.LeftPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,initialData.LeftPower,initialData.LeftVolt,
                initialData.LeftWl,initialData.LeftReverse ?? 0,initialData.LeftKnee,"L2-E"));
        }
        
        if(initialData.BottomPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,initialData.BottomPower,initialData.BottomVolt,
                initialData.BottomWl,initialData.BottomReverse ?? 0,initialData.BottomKnee,"G2-E"));
        }
        
        if(initialData.RightPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,initialData.RightPower,initialData.RightVolt,
                initialData.RightWl,initialData.RightReverse ?? 0,initialData.RightKnee,"R2-E"));
        }
    }
    
    if (initial50mA != null) {
        if (initial50mA.CenterAPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,initial50mA.CenterAPower,initial50mA.CenterAVolt,
                initial50mA.CenterAWl,initial50mA.CenterAReverse ?? 0,initial50mA.CenterAKnee,PadLocation.PadLocationA.Value));
        }
        
        if(initial50mA.CenterBPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,initial50mA.CenterBPower,initial50mA.CenterBVolt,
                initial50mA.CenterBWl,initial50mA.CenterBReverse ?? 0,initial50mA.CenterBKnee,PadLocation.PadLocationB.Value));
        }
        
        if(initial50mA.CenterCPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,initial50mA.CenterCPower,initial50mA.CenterCVolt,
                initial50mA.CenterCWl,initial50mA.CenterCReverse ?? 0,initial50mA.CenterCKnee,PadLocation.PadLocationC.Value));
        }
        
        if(initial50mA.CenterDPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,initial50mA.CenterDPower,initial50mA.CenterDVolt,
                initial50mA.CenterDWl,initial50mA.CenterDReverse ?? 0,initial50mA.CenterDKnee,PadLocation.PadLocationD.Value));
        }
        
        if(initial50mA.TopPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,initial50mA.TopPower,initial50mA.TopVolt,
                initial50mA.TopWl,initial50mA.TopReverse ?? 0,initial50mA.TopKnee,"T2-E"));
        }
        
        if(initial50mA.LeftPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,initial50mA.LeftPower,initial50mA.LeftVolt,
                initial50mA.LeftWl,initial50mA.LeftReverse ?? 0,initial50mA.LeftKnee,"L2-E"));
        }
        
        if(initial50mA.BottomPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,initial50mA.BottomPower,initial50mA.BottomVolt,
                initial50mA.BottomWl,initial50mA.BottomReverse ?? 0,initial50mA.BottomKnee,"G2-E"));
        }
        
        if(initial50mA.RightPower > 0) {
            initialMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,initial50mA.RightPower,initial50mA.RightVolt,
                initial50mA.RightWl,initial50mA.RightReverse ?? 0,initial50mA.RightKnee,"R2-E"));
        }
    }

    return (initialMeasurements,initialData?.DateTime ?? DateTime.MinValue);
}

async Task<(List<QtMeasurement>? measurements,DateTime timeStamp)> CreateFinalMeasurements(EpiContext context, string waferId,ObjectId qtId) {
    var finalData = await context.EpiDataAfters.FirstOrDefaultAsync(e => e.WaferId == waferId);
    var final50mAData= await context.EpiDataAfter50mas.FirstOrDefaultAsync(e => e.WaferId == waferId);
    var finalMeasurements = new List<QtMeasurement>();
    if (finalData != null) {
        if (finalData.CenterAPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,finalData.CenterAPower,finalData.CenterAVolt,
                finalData.CenterAWl,finalData.CenterAReverse ?? 0,finalData.CenterAKnee,PadLocation.PadLocationA.Value));
        }

        
        if(finalData.CenterBPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,finalData.CenterBPower,finalData.CenterBVolt,
                finalData.CenterBWl,finalData.CenterBReverse ?? 0,finalData.CenterBKnee,PadLocation.PadLocationB.Value));
        }
        
        if(finalData.CenterCPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
               20,finalData.CenterCPower,finalData.CenterCVolt,
                finalData.CenterCWl,finalData.CenterCReverse ?? 0,finalData.CenterCKnee,PadLocation.PadLocationC.Value));
        }
        
        if(finalData.CenterDPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,finalData.CenterDPower,finalData.CenterDVolt,
                finalData.CenterDWl,finalData.CenterDReverse ?? 0,finalData.CenterDKnee,PadLocation.PadLocationD.Value));
        }
        
        if(finalData.TopPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,finalData.TopPower,finalData.TopVolt,
                finalData.TopWl,finalData.TopReverse ?? 0,finalData.TopKnee,"T2-E"));
        }
        
        if(finalData.LeftPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,finalData.LeftPower,finalData.LeftVolt,
                finalData.LeftWl,finalData.LeftReverse ?? 0,finalData.LeftKnee,"L2-E"));
        }
        
        if(finalData.BottomPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,finalData.BottomPower,finalData.BottomVolt,
                finalData.BottomWl,finalData.BottomReverse ?? 0,finalData.BottomKnee,"G2-E"));
        }
        
        if(finalData.RightPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                20,finalData.RightPower,finalData.RightVolt,
                finalData.RightWl,finalData.RightReverse ?? 0,finalData.RightKnee,"R2-E"));
        }
    }
    
    if (final50mAData != null) {
        if (final50mAData.CenterAPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,final50mAData.CenterAPower,final50mAData.CenterAVolt,
                final50mAData.CenterAWl,final50mAData.CenterAReverse ?? 0,final50mAData.CenterAKnee,PadLocation.PadLocationA.Value));
        }
        
        if(final50mAData.CenterBPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,final50mAData.CenterBPower,final50mAData.CenterBVolt,
                final50mAData.CenterBWl,final50mAData.CenterBReverse ?? 0,final50mAData.CenterBKnee,PadLocation.PadLocationB.Value));
        }
        
        if(final50mAData.CenterCPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,final50mAData.CenterCPower,final50mAData.CenterCVolt,
                final50mAData.CenterCWl,final50mAData.CenterCReverse ?? 0,final50mAData.CenterCKnee,PadLocation.PadLocationC.Value));
        }
        
        if(final50mAData.CenterDPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,final50mAData.CenterDPower,final50mAData.CenterDVolt,
                final50mAData.CenterDWl,final50mAData.CenterDReverse ?? 0,final50mAData.CenterDKnee,PadLocation.PadLocationD.Value));
        }
        
        if(final50mAData.TopPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,final50mAData.TopPower,final50mAData.TopVolt,
                final50mAData.TopWl,final50mAData.TopReverse ?? 0,final50mAData.TopKnee,"T2-E"));
        }
        
        if(final50mAData.LeftPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,final50mAData.LeftPower,final50mAData.LeftVolt,
                final50mAData.LeftWl,final50mAData.LeftReverse ?? 0,final50mAData.LeftKnee,"L2-E"));
        }
        
        if(final50mAData.BottomPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,final50mAData.BottomPower,final50mAData.BottomVolt,
                final50mAData.BottomWl,final50mAData.BottomReverse ?? 0,final50mAData.BottomKnee,"G2-E"));
        }
        
        if(final50mAData.RightPower > 0) {
            finalMeasurements.Add(CreateMeasurement(MeasurementType.Initial,waferId,qtId,
                50,final50mAData.RightPower,final50mAData.RightVolt,
                final50mAData.RightWl,final50mAData.RightReverse ?? 0,final50mAData.RightKnee,"R2-E"));
        }
    }

    return (finalMeasurements,finalData?.DateTime ?? DateTime.MinValue);
}

QtMeasurement CreateMeasurement(MeasurementType type,string waferId,ObjectId qtId,int current, double power, double voltage, double wl, double ir, double knee,string pad) {
    return new QtMeasurement() {
        WaferId = waferId,
        QuickTestResultId = qtId,
        MeasurementType = type,
        Pad = pad,
        Current = current,
        Power = power,
        Voltage = voltage,
        Wl = wl,
        Ir = ir,
        Knee = knee
    };
}

async Task<List<Spectrum>?> CreateInitialSpectrum(EpiContext context, string waferId,ObjectId qtId) {
    var spectrum= await context.EpiSpectrumInitials.FirstOrDefaultAsync(e => e.WaferId == waferId);
    var spectrumData=new List<Spectrum>();
    if (spectrum != null) {
        if (!string.IsNullOrEmpty(spectrum.CenterAWl) && !string.IsNullOrEmpty(spectrum.CenterASpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationA.Value,
                Current=20,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterAWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterASpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterAWl50mA) && !string.IsNullOrEmpty(spectrum.CenterASpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationA.Name,
                Current=50,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterAWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterASpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterBWl) && !string.IsNullOrEmpty(spectrum.CenterBSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationB.Name,
                Current=20,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterBWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterBSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterBWl50mA) && !string.IsNullOrEmpty(spectrum.CenterBSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationB.Name,
                Current=50,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterBWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterBSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterCWl) && !string.IsNullOrEmpty(spectrum.CenterCSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationC.Name,
                Current=20,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterCWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterCSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterCWl50mA) && !string.IsNullOrEmpty(spectrum.CenterCSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationC.Name,
                Current=50,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterCWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterCSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterDWl) && !string.IsNullOrEmpty(spectrum.CenterDSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationD.Name,
                Current=20,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterDWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterDSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterDWl50mA) && !string.IsNullOrEmpty(spectrum.CenterDSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationD.Name,
                Current=50,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterDWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterDSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.RightWl) && !string.IsNullOrEmpty(spectrum.RightSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="R2-E",
                Current=20,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.RightWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.RightSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.RightWl50mA) && !string.IsNullOrEmpty(spectrum.RightSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="R2-E",
                Current=50,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.RightWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.RightSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.TopWl) && !string.IsNullOrEmpty(spectrum.TopSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="T2-E",
                Current=20,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.TopWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.TopSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.TopWl50mA) && !string.IsNullOrEmpty(spectrum.TopSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="T2-E",
                Current=50,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.TopWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.TopSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.LeftWl) && !string.IsNullOrEmpty(spectrum.LeftSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="L2-E",
                Current=20,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.LeftWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.LeftSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.LeftWl50mA) && !string.IsNullOrEmpty(spectrum.LeftSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="L2-E",
                Current=50,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.LeftWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.LeftSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.BottomWl) && !string.IsNullOrEmpty(spectrum.BottomSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="G2-E",
                Current=20,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.BottomWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.BottomSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.BottomWl50mA) && !string.IsNullOrEmpty(spectrum.BottomSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="G2-E",
                Current=50,
                MeasurementType = MeasurementType.Initial,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.BottomWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.BottomSpect50mA)
            });
        }
        return spectrumData;
    }
    return null;
}

async Task<List<Spectrum>?> CreateFinalSpectrum(EpiContext context, string waferId,ObjectId qtId) {
    var spectrum= await context.EpiSpectrumAfters.FirstOrDefaultAsync(e => e.WaferId == waferId);
    var spectrumData=new List<Spectrum>();
    if (spectrum != null) {
        if (!string.IsNullOrEmpty(spectrum.CenterAWl) && !string.IsNullOrEmpty(spectrum.CenterASpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationA.Value,
                Current=20,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterAWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterASpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterAWl50mA) && !string.IsNullOrEmpty(spectrum.CenterASpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationA.Name,
                Current=50,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterAWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterASpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterBWl) && !string.IsNullOrEmpty(spectrum.CenterBSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationB.Name,
                Current=20,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterBWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterBSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterBWl50mA) && !string.IsNullOrEmpty(spectrum.CenterBSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationB.Name,
                Current=50,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterBWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterBSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterCWl) && !string.IsNullOrEmpty(spectrum.CenterCSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationC.Name,
                Current=20,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterCWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterCSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterCWl50mA) && !string.IsNullOrEmpty(spectrum.CenterCSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationC.Name,
                Current=50,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterCWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterCSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterDWl) && !string.IsNullOrEmpty(spectrum.CenterDSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationD.Name,
                Current=20,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterDWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterDSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterDWl50mA) && !string.IsNullOrEmpty(spectrum.CenterDSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad=PadLocation.PadLocationD.Name,
                Current=50,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.CenterDWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.CenterDSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.RightWl) && !string.IsNullOrEmpty(spectrum.RightSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="R2-E",
                Current=20,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.RightWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.RightSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.RightWl50mA) && !string.IsNullOrEmpty(spectrum.RightSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="R2-E",
                Current=50,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.RightWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.RightSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.TopWl) && !string.IsNullOrEmpty(spectrum.TopSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="T2-E",
                Current=20,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.TopWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.TopSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.TopWl50mA) && !string.IsNullOrEmpty(spectrum.TopSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="T2-E",
                Current=50,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.TopWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.TopSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.LeftWl) && !string.IsNullOrEmpty(spectrum.LeftSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="L2-E",
                Current=20,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.LeftWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.LeftSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.LeftWl50mA) && !string.IsNullOrEmpty(spectrum.LeftSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="L2-E",
                Current=50,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.LeftWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.LeftSpect50mA)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.BottomWl) && !string.IsNullOrEmpty(spectrum.BottomSpect)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="G2-E",
                Current=20,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.BottomWl),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.BottomSpect)
            });
        }
        
        if (!string.IsNullOrEmpty(spectrum.BottomWl50mA) && !string.IsNullOrEmpty(spectrum.BottomSpect50mA)) {
            spectrumData.Add(new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId,
                QuickTestResultId = qtId,
                Pad="G2-E",
                Current=50,
                MeasurementType = MeasurementType.Final,
                Wl = JsonSerializer.Deserialize<List<double>>(spectrum.BottomWl50mA),
                Intensity = JsonSerializer.Deserialize<List<double>>(spectrum.BottomSpect50mA)
            });
        }
        return spectrumData;
    }
    return null;
}

async Task PrintProperties() {
    EpiContext context = new();
    var initialData = await context.EpiDataInitials.FirstOrDefaultAsync(e => e.WaferId == "B03-2542-05");
    var initial50mA= await context.EpiDataInitial50mas.FirstOrDefaultAsync(e => e.WaferId == "B03-2542-05");
    var finalData = await context.EpiDataAfters.FirstOrDefaultAsync(e => e.WaferId == "B03-2542-05");
    var final50mA= await context.EpiDataAfter50mas.FirstOrDefaultAsync(e => e.WaferId == "B03-2542-05");
    var initialSpectrum = await context.EpiSpectrumInitials.FirstOrDefaultAsync(e => e.WaferId == "B03-2542-05");
    var initialSpectrum50mA = await context.EpiSpectrumInitials.FirstOrDefaultAsync(e => e.WaferId == "B03-2542-05");
    
    var properties = TypeDescriptor.GetProperties(initialData);
    var properties50mA = TypeDescriptor.GetProperties(initial50mA);
    var propertiesFinal = TypeDescriptor.GetProperties(finalData);
    var propertiesFinal50mA = TypeDescriptor.GetProperties(final50mA);
    var propertiesSpectrum = TypeDescriptor.GetProperties(initialSpectrum);
    var propertiesSpectrum50mA = TypeDescriptor.GetProperties(initialSpectrum50mA);
    StringBuilder builder = new();
    foreach(PropertyDescriptor property in properties) {
        Console.WriteLine($"Column{property.Name}");
        builder.AppendLine(property.Name);
    }
    await File.WriteAllTextAsync("C:\\Users\\aelmendo\\Documents\\QuickTestDbProperties\\EpiDataInitials.txt",builder.ToString());
    
    foreach(PropertyDescriptor property in properties50mA) {
        Console.WriteLine($"Column{property.Name}");
        builder.AppendLine(property.Name);
    }
    await File.WriteAllTextAsync("C:\\Users\\aelmendo\\Documents\\QuickTestDbProperties\\EpiDataInitial50mA.txt",builder.ToString());
    
    foreach(PropertyDescriptor property in propertiesFinal) {
        Console.WriteLine($"Column: {property.Name}");
        builder.AppendLine(property.Name);
    }

    await File.WriteAllTextAsync("C:\\Users\\aelmendo\\Documents\\QuickTestDbProperties\\EpiDataFinal.txt",builder.ToString());
    
    foreach(PropertyDescriptor property in propertiesFinal50mA) {
        Console.WriteLine($"Column{property.Name}");
        builder.AppendLine(property.Name);
    }
    await File.WriteAllTextAsync("C:\\Users\\aelmendo\\Documents\\QuickTestDbProperties\\EpiDataFinal50mA.txt",builder.ToString());
    
    foreach(PropertyDescriptor property in propertiesSpectrum) {
        Console.WriteLine($"Column{property.Name}");
        builder.AppendLine(property.Name);
    }
    await File.WriteAllTextAsync("C:\\Users\\aelmendo\\Documents\\QuickTestDbProperties\\EpiDataSpectrum.txt",builder.ToString());
    
    foreach(PropertyDescriptor property in propertiesSpectrum50mA) {
        Console.WriteLine($"Column{property.Name}");
        builder.AppendLine(property.Name);
    }
    await File.WriteAllTextAsync("C:\\Users\\aelmendo\\Documents\\QuickTestDbProperties\\EpiDataFinal50mA.txt",builder.ToString());

}

/*async Task<List<Measurement>?> CreateFinalMeasurements(EpiContext context, string waferId) {
    var initialData = await context.EpiDataInitials.FirstOrDefaultAsync(e => e.WaferId == waferId);
    var initialMeasurements = new List<Measurement>();
    if (initialData != null) {
        var properties = TypeDescriptor.GetProperties(initialData);
        foreach (PropertyDescriptor property in properties) {
            if (property.Name.Contains("CenterA")) { } else if (property.Name.Contains("CenterAPower")) {
                if (initialData.CenterAPower != 0) {
                    initialMeasurements.Add(new Measurement() {
                        MeasurementType = MeasurementType.Initial,
                        Current = 20,
                        Power = initialData.CenterAPower,
                        Voltage = initialData.CenterAVolt,
                        Wl = initialData.CenterAWl,
                        Ir = initialData.CenterAReverse ?? 0,
                        Knee = initialData.CenterAKnee
                    });
                }

            } else if (property.Name.Contains("CenterBPower")) {
                if (initialData.CenterBPower != 0) {
                    initialMeasurements.Add(new Measurement() {
                        MeasurementType = MeasurementType.Initial,
                        Pad = PadLocation.PadLocationB.Name,
                        Current = 20,
                        Power = initialData.CenterBPower,
                        Voltage = initialData.CenterBVolt,
                        Wl = initialData.CenterBWl,
                        Ir = initialData.CenterBReverse ?? 0,
                        Knee = initialData.CenterBKnee
                    });
                }

            } else if (property.Name.Contains("CenterCPower")) {
                if (initialData.CenterCPower != 0) {
                    initialMeasurements.Add(new Measurement() {
                        MeasurementType = MeasurementType.Initial,
                        Pad = PadLocation.PadLocationC.Name,
                        Current = 20,
                        Power = initialData.CenterCPower,
                        Voltage = initialData.CenterCVolt,
                        Wl = initialData.CenterCWl,
                        Ir = initialData.CenterCReverse ?? 0,
                        Knee = initialData.CenterCKnee
                    });
                }

            } else if (property.Name.Contains("CenterDPower")) {
                
            } else if (property.Name.Contains("TopPower")) {
                
            } else if (property.Name.Contains("LeftPower")) {
                
            } else if (property.Name.Contains("BottomPower")) {
                
            } else if (property.Name.Contains("RightPower")) { }

            string propertyName = property.Name;
            object propertyValue = property.GetValue(initialData);
            Console.WriteLine($"{propertyName}: {propertyValue}");
            Console.WriteLine($"Wafer Count: {context.EpiDataInitials.Count()}");

            var json = JsonSerializer.Serialize(initialData, new JsonSerializerOptions() { WriteIndented = true });
            Console.WriteLine($"B03-2542-05 initial data:\n {json}");
        }

        return initialMeasurements;
    }
    return null;
}*/