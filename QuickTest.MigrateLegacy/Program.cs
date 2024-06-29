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

//await TestGetWaferPads();

/*async Task TestGetWaferPads() {
    var client=new MongoClient("mongodb://172.20.3.41:27017");
    var database=client.GetDatabase("quick_test_db");
    var initialCollection=database.GetCollection<QtMeasurement>("init_measurements");
    var waferPadCollection=database.GetCollection<WaferPad>("wafer_pads");
    var tested = await initialCollection.Find(e => e.WaferId == "B01-3482-10").Project(e => e.Pad).ToListAsync();
    /*List<string?> pads = ["A","B","G6-E"];#1#
    var result=await waferPadCollection.Find(e=>tested.Contains(e.Identifier)).ToListAsync();
    foreach (var pad in result) {
        Console.WriteLine($"Pad: {pad.Identifier} SvgX: {pad.SvgObject.X} SvgY: {pad.SvgObject.Y} SvgR: {pad.SvgObject.Radius}" );
    }
}*/



async Task Migrate() {
    var context = new EpiContext();
    var clientWork=new MongoClient("mongodb://172.20.3.41:27017");
    //var clientPi=new MongoClient("mongodb://192.168.68.112:27017");
    var databaseWork=clientWork.GetDatabase("quick_test_db_v2");
    //var databasePi=clientPi.GetDatabase("quick_test_db");
    var qtCollectionWork=databaseWork.GetCollection<QuickTestResult>("quick_test");
    var initMeasureCollection=databaseWork.GetCollection<QtMeasurement>("init_measurements");
    var finalMeasureCollection=databaseWork.GetCollection<QtMeasurement>("final_measurements");
    var initSpectCollection=databaseWork.GetCollection<Spectrum>("init_spectrum");
    var finalSpectCollection=databaseWork.GetCollection<Spectrum>("final_spectrum");
    await qtCollectionWork.Indexes.CreateOneAsync(new CreateIndexModel<QuickTestResult>(Builders<QuickTestResult>.IndexKeys.Ascending(e => e.WaferId)));
    
    Console.WriteLine("Starting Migration...");
    var start=new DateTime(2024,1,1);
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
        /*var result=await CreateQuickTestResult(context,wafer);*/
        var qtId = ObjectId.GenerateNewId();
        var initMeasure = await CreateInitialMeasurements(context, wafer,qtId);
        var finalMeasure = await CreateFinalMeasurements(context, wafer,qtId);
        var qt=new QuickTestResult() {
            _id = qtId,
            WaferId = wafer,
            ProbeStationId = 1,
            InitialTimeStamp = initMeasure.timeStamp,
            FinalTimeStamp = initMeasure.timeStamp,
        };
        var initSpectrum = await CreateInitialSpectrum(context, wafer, qt._id);
        var finalSpectrum=await CreateFinalSpectrum(context,wafer, qt._id);
        
        if(initSpectrum!=null) {
            initSpectrumResults.Add(initSpectrum.Value._20mA);
            initSpectrumResults.Add(initSpectrum.Value._50mA);
        }
        if(finalSpectrum!=null) {
            finalSpectResults.Add(finalSpectrum.Value._20mA);
            finalSpectResults.Add(finalSpectrum.Value._50mA);
        }
 
        initMeasureResults.Add(initMeasure._20mA);
        initMeasureResults.Add(initMeasure._50mA);
        finalMeasureResults.Add(finalMeasure._20mA);
        finalMeasureResults.Add(finalMeasure._50mA);
        results.Add(qt);
        saveCounter++;
        count++;
        Console.WriteLine($"Created QuickTestResult for wafer: {wafer} Count:{count}");
        if(saveCounter>=2) {
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
    var database=client.GetDatabase("quick_test_db_v2");
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

async Task<(QtMeasurement _20mA,QtMeasurement _50mA,DateTime timeStamp)> CreateInitialMeasurements(EpiContext context, string waferId, ObjectId qtId) {
    var initialData = await context.EpiDataInitials.FirstOrDefaultAsync(e => e.WaferId == waferId);
    var initial50mA= await context.EpiDataInitial50mas.FirstOrDefaultAsync(e => e.WaferId == waferId);
    QtMeasurement qt20mA = new() {
        _id = ObjectId.GenerateNewId(),
        WaferId = waferId,
        QuickTestResultId = qtId,
        MeasurementType = MeasurementType.Initial,
        Current = 20,
        Measurements = new Dictionary<string, PadMeasurement>()
    };
    QtMeasurement qt50mA = new(){
        _id = ObjectId.GenerateNewId(),
        WaferId = waferId,
        QuickTestResultId = qtId,
        MeasurementType = MeasurementType.Initial,
        Current = 50,
        Measurements = new Dictionary<string, PadMeasurement>()
    };
    if (initialData != null) {
        if (initialData.CenterAPower > 0 || initialData.CenterAWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationA.Value,initialData.CenterAPower,initialData.CenterAVolt,
                initialData.CenterAWl,initialData.CenterAReverse ?? 0,initialData.CenterAKnee);
        }

        
        if(initialData.CenterBPower > 0 || initialData.CenterBWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationB.Value,initialData.CenterBPower,initialData.CenterBVolt,
                initialData.CenterBWl,initialData.CenterBReverse ?? 0,initialData.CenterBKnee);
        }
        
        if(initialData.CenterCPower > 0 || initialData.CenterCWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationC.Value,initialData.CenterCPower,initialData.CenterCVolt,
                initialData.CenterCWl,initialData.CenterCReverse ?? 0,initialData.CenterCKnee);
        }
        
        if(initialData.CenterDPower > 0 || initialData.CenterDWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationD.Value,initialData.CenterDPower,initialData.CenterDVolt,
                initialData.CenterDWl,initialData.CenterDReverse ?? 0,initialData.CenterDKnee);
        }
        
        if(initialData.TopPower > 0 || initialData.TopWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationT.Value,initialData.TopPower,initialData.TopVolt,
                initialData.TopWl,initialData.TopReverse ?? 0,initialData.TopKnee,"T2-E");
        }
        
        if(initialData.LeftPower > 0 || initialData.LeftWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationL.Value,initialData.LeftPower,initialData.LeftVolt,
                initialData.LeftWl,initialData.LeftReverse ?? 0,initialData.LeftKnee,"L2-E");
        }
        
        if(initialData.BottomPower > 0 || initialData.BottomWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationG.Value,initialData.BottomPower,initialData.BottomVolt,
                initialData.BottomWl,initialData.BottomReverse ?? 0,initialData.BottomKnee,"G2-E");
        }
        
        if(initialData.RightPower > 0 || initialData.RightWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationR,initialData.RightPower,initialData.RightVolt,
                initialData.RightWl,initialData.RightReverse ?? 0,initialData.RightKnee,"R2-E");
        }
    }
    
    if (initial50mA != null) {
        if (initial50mA.CenterAPower > 0 || initial50mA.CenterAWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationA.Value,initial50mA.CenterAPower,initial50mA.CenterAVolt,
                initial50mA.CenterAWl,initial50mA.CenterAReverse ?? 0,initial50mA.CenterAKnee,swap:true);
        }
        
        if(initial50mA.CenterBPower > 0 || initial50mA.CenterBWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationB.Value,initial50mA.CenterBPower,initial50mA.CenterBVolt,
                initial50mA.CenterBWl,initial50mA.CenterBReverse ?? 0,initial50mA.CenterBKnee,swap:true);
        }
        
        if(initial50mA.CenterCPower > 0 || initial50mA.CenterCWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationC.Value,initial50mA.CenterCPower,initial50mA.CenterCVolt,
                initial50mA.CenterCWl,initial50mA.CenterCReverse ?? 0,initial50mA.CenterCKnee,swap:true);
        }
        
        if(initial50mA.CenterDPower > 0 || initial50mA.CenterDWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationD.Value,initial50mA.CenterDPower,initial50mA.CenterDVolt,
                initial50mA.CenterDWl,initial50mA.CenterDReverse ?? 0,initial50mA.CenterDKnee,swap:true);
        }
        
        if(initial50mA.TopPower > 0 || initial50mA.TopWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationT.Value,initial50mA.TopPower,initial50mA.TopVolt,
                initial50mA.TopWl,initial50mA.TopReverse ?? 0,initial50mA.TopKnee,"T2-E",swap:true);
        }
        
        if(initial50mA.LeftPower > 0 || initial50mA.LeftWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationL.Value,initial50mA.LeftPower,initial50mA.LeftVolt,
                initial50mA.LeftWl,initial50mA.LeftReverse ?? 0,initial50mA.LeftKnee,"L2-E",swap:true);
        }
        
        if(initial50mA.BottomPower > 0 || initial50mA.BottomWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationG.Value,initial50mA.BottomPower,initial50mA.BottomVolt,
                initial50mA.BottomWl,initial50mA.BottomReverse ?? 0,initial50mA.BottomKnee,"G2-E",swap:true);
        }
        
        if(initial50mA.RightPower > 0 || initial50mA.RightWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationR,initial50mA.RightPower,initial50mA.RightVolt,
                initial50mA.RightWl,initial50mA.RightReverse ?? 0,initial50mA.RightKnee,"R2-E",swap:true);
        }
    }

    return (qt20mA,qt50mA,initialData?.DateTime ?? DateTime.MinValue);
}

async Task<(QtMeasurement _20mA,QtMeasurement _50mA,DateTime timeStamp)> CreateFinalMeasurements(EpiContext context,string waferId,ObjectId qtId) {
    var finalData = await context.EpiDataAfters.FirstOrDefaultAsync(e => e.WaferId == waferId);
    var final50mAData= await context.EpiDataAfter50mas.FirstOrDefaultAsync(e => e.WaferId == waferId);
    QtMeasurement qt20mA = new() {
        _id = ObjectId.GenerateNewId(),
        WaferId = waferId,
        QuickTestResultId = qtId,
        MeasurementType = MeasurementType.Final,
        Current = 20,
        Measurements = new Dictionary<string, PadMeasurement>()
    };
    QtMeasurement qt50mA = new() {
        _id = ObjectId.GenerateNewId(),
        WaferId = waferId,
        QuickTestResultId = qtId,
        MeasurementType = MeasurementType.Final,
        Current = 50,
        Measurements = new Dictionary<string, PadMeasurement>()
    };
    if (finalData != null) {
        if (finalData.CenterAPower > 0 || finalData.CenterAWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationA.Value,finalData.CenterAPower,finalData.CenterAVolt,
                finalData.CenterAWl,finalData.CenterAReverse ?? 0,finalData.CenterAKnee);
        }

        
        if(finalData.CenterBPower > 0 || finalData.CenterBWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationB.Value,finalData.CenterBPower,finalData.CenterBVolt,
                finalData.CenterBWl,finalData.CenterBReverse ?? 0,finalData.CenterBKnee);
        }
        
        if(finalData.CenterCPower > 0 || finalData.CenterCWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationC.Value,finalData.CenterCPower,finalData.CenterCVolt,
                finalData.CenterCWl,finalData.CenterCReverse ?? 0,finalData.CenterCKnee);
        }
        
        if(finalData.CenterDPower > 0 || finalData.CenterDWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationD.Value,finalData.CenterDPower,finalData.CenterDVolt,
                finalData.CenterDWl,finalData.CenterDReverse ?? 0,finalData.CenterDKnee);
        }
        
        if(finalData.TopPower > 0 || finalData.TopWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationT.Value,finalData.TopPower,finalData.TopVolt,
                finalData.TopWl,finalData.TopReverse ?? 0,finalData.TopKnee,"T2-E");
        }
        
        if(finalData.LeftPower > 0 || finalData.LeftWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationL.Value,finalData.LeftPower,finalData.LeftVolt,
                finalData.LeftWl,finalData.LeftReverse ?? 0,finalData.LeftKnee,"L2-E");
        }
        
        if(finalData.BottomPower > 0 || finalData.BottomWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationG.Value,finalData.BottomPower,finalData.BottomVolt,
                finalData.BottomWl,finalData.BottomReverse ?? 0,finalData.BottomKnee,"G2-E");
        }
        
        if(finalData.RightPower > 0 || finalData.RightWl>0) {
            UpdateMeasurement(qt20mA,PadLocation.PadLocationR,finalData.RightPower,finalData.RightVolt,
                finalData.RightWl,finalData.RightReverse ?? 0,finalData.RightKnee,"R2-E");
        }
    }
    
    if (final50mAData != null) {
        if (final50mAData.CenterAPower > 0 || final50mAData.CenterAWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationA.Value,final50mAData.CenterAPower,final50mAData.CenterAVolt,
                final50mAData.CenterAWl,final50mAData.CenterAReverse ?? 0,final50mAData.CenterAKnee,swap:true);
        }
        
        if(final50mAData.CenterBPower > 0 || final50mAData.CenterBWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationB.Value,final50mAData.CenterBPower,final50mAData.CenterBVolt,
                final50mAData.CenterBWl,final50mAData.CenterBReverse ?? 0,final50mAData.CenterBKnee,swap:true);
        }
        
        if(final50mAData.CenterCPower > 0 || final50mAData.CenterCWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationC.Value,final50mAData.CenterCPower,final50mAData.CenterCVolt,
                final50mAData.CenterCWl,final50mAData.CenterCReverse ?? 0,final50mAData.CenterCKnee,swap:true);
        }
        
        if(final50mAData.CenterDPower > 0 || final50mAData.CenterDWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationD.Value,final50mAData.CenterDPower,final50mAData.CenterDVolt,
                final50mAData.CenterDWl,final50mAData.CenterDReverse ?? 0,final50mAData.CenterDKnee,swap:true);
        }
        
        if(final50mAData.TopPower > 0 || final50mAData.TopWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationT.Value,final50mAData.TopPower,final50mAData.TopVolt,
                final50mAData.TopWl,final50mAData.TopReverse ?? 0,final50mAData.TopKnee,"T2-E",swap:true);
        }
        
        if(final50mAData.LeftPower > 0 || final50mAData.LeftWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationL.Value,final50mAData.LeftPower,final50mAData.LeftVolt,
                final50mAData.LeftWl,final50mAData.LeftReverse ?? 0,final50mAData.LeftKnee,"L2-E",swap:true);
        }
        
        if(final50mAData.BottomPower > 0 || final50mAData.BottomWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationG.Value,final50mAData.BottomPower,final50mAData.BottomVolt,
                final50mAData.BottomWl,final50mAData.BottomReverse ?? 0,final50mAData.BottomKnee,"G2-E",swap:true);
        }
        
        if(final50mAData.RightPower > 0 || final50mAData.RightWl>0) {
            UpdateMeasurement(qt50mA,PadLocation.PadLocationR,final50mAData.RightPower,final50mAData.RightVolt,
                final50mAData.RightWl,final50mAData.RightReverse ?? 0,final50mAData.RightKnee,"R2-E",swap:true);
        }
    }

    return (qt20mA,qt50mA,finalData?.DateTime ?? DateTime.MinValue);
}

void UpdateMeasurement(QtMeasurement measurement,string pad, double power, double voltage, double wl, double ir, double knee,string? actualPad=null,bool swap=false) {
    measurement.Measurements[pad]=new PadMeasurement(){
        ActualPad = string.IsNullOrEmpty(actualPad) ? pad:actualPad,
        Power = swap ? wl:power,
        Voltage = voltage,
        Wl = swap ? power:wl,
        Ir = ir,
        Knee = knee
    };
}

void UpdateSpectrum(Spectrum spectrum,string pad, string wl, string intensity,string? actualPad=null) {
    spectrum.Measurements[pad]=new PadSpectrumMeasurement(){
        Wl = JsonSerializer.Deserialize<List<double>>(wl),
        Intensity = JsonSerializer.Deserialize<List<double>>(intensity)
    };
    spectrum.Measurements[pad].ActualPad=string.IsNullOrEmpty(actualPad) ? pad:actualPad;
}

async Task<(Spectrum _20mA,Spectrum _50mA)?> CreateInitialSpectrum(EpiContext context, string waferId,ObjectId qtId) {
    var spectrum= await context.EpiSpectrumInitials.FirstOrDefaultAsync(e => e.WaferId == waferId);
    var spectrumData=new List<Spectrum>();
    if (spectrum != null) {
        var spectrum20mA = new Spectrum() { 
            _id = ObjectId.GenerateNewId(),
            WaferId = waferId, 
            QuickTestResultId = qtId,Current = 20,
            MeasurementType = MeasurementType.Initial
        };
        var spectrum50mA = new Spectrum(){
            _id = ObjectId.GenerateNewId(),
            WaferId = waferId, 
            QuickTestResultId = qtId,Current = 50,
            MeasurementType = MeasurementType.Initial
        };
        if (!string.IsNullOrEmpty(spectrum.CenterAWl) && !string.IsNullOrEmpty(spectrum.CenterASpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationA.Value,spectrum.CenterAWl,spectrum.CenterASpect);
        }
        if (!string.IsNullOrEmpty(spectrum.CenterAWl50mA) && !string.IsNullOrEmpty(spectrum.CenterASpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationA.Value,spectrum.CenterAWl50mA,spectrum.CenterASpect50mA);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterBWl) && !string.IsNullOrEmpty(spectrum.CenterBSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationB.Value,spectrum.CenterBWl,spectrum.CenterBSpect);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterBWl50mA) && !string.IsNullOrEmpty(spectrum.CenterBSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationB.Value,spectrum.CenterBWl50mA,spectrum.CenterBSpect50mA);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterCWl) && !string.IsNullOrEmpty(spectrum.CenterCSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationC.Value,spectrum.CenterCWl,spectrum.CenterCSpect);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterCWl50mA) && !string.IsNullOrEmpty(spectrum.CenterCSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationC.Value,spectrum.CenterCWl50mA,spectrum.CenterCSpect50mA);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterDWl) && !string.IsNullOrEmpty(spectrum.CenterDSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationD.Value,spectrum.CenterDWl,spectrum.CenterDSpect);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterDWl50mA) && !string.IsNullOrEmpty(spectrum.CenterDSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationD.Value,spectrum.CenterDWl50mA,spectrum.CenterDSpect50mA);
        }
        
        if (!string.IsNullOrEmpty(spectrum.RightWl) && !string.IsNullOrEmpty(spectrum.RightSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationR,spectrum.RightWl,spectrum.RightSpect,"R2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.RightWl50mA) && !string.IsNullOrEmpty(spectrum.RightSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationR,spectrum.RightWl50mA,spectrum.RightSpect50mA,"R2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.TopWl) && !string.IsNullOrEmpty(spectrum.TopSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationT,spectrum.TopWl,spectrum.TopSpect,"T2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.TopWl50mA) && !string.IsNullOrEmpty(spectrum.TopSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationT,spectrum.TopWl50mA,spectrum.TopSpect50mA,"T2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.LeftWl) && !string.IsNullOrEmpty(spectrum.LeftSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationL,spectrum.LeftWl,spectrum.LeftSpect,"L2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.LeftWl50mA) && !string.IsNullOrEmpty(spectrum.LeftSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationL,spectrum.LeftWl50mA,spectrum.LeftSpect50mA,"L2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.BottomWl) && !string.IsNullOrEmpty(spectrum.BottomSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationG,spectrum.BottomWl,spectrum.BottomSpect,"G2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.BottomWl50mA) && !string.IsNullOrEmpty(spectrum.BottomSpect50mA)) {
            UpdateSpectrum(spectrum50mA,"G2-E",spectrum.BottomWl50mA,spectrum.BottomSpect50mA);
        }
        return new(spectrum20mA,spectrum50mA);
    }
    return null;
}

async Task<(Spectrum _20mA,Spectrum _50mA)?> CreateFinalSpectrum(EpiContext context, string waferId,ObjectId qtId) {
    var spectrum= await context.EpiSpectrumAfters.FirstOrDefaultAsync(e => e.WaferId == waferId);
        if (spectrum != null) {
        var spectrum20mA = new Spectrum() { 
            WaferId = waferId, 
            QuickTestResultId = qtId,Current = 20,
            MeasurementType = MeasurementType.Initial
        };
        var spectrum50mA = new Spectrum(){
            WaferId = waferId, 
            QuickTestResultId = qtId,Current = 50,
            MeasurementType = MeasurementType.Initial
        };
        if (!string.IsNullOrEmpty(spectrum.CenterAWl) && !string.IsNullOrEmpty(spectrum.CenterASpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationA.Value,spectrum.CenterAWl,spectrum.CenterASpect);
        }
        if (!string.IsNullOrEmpty(spectrum.CenterAWl50mA) && !string.IsNullOrEmpty(spectrum.CenterASpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationA.Value,spectrum.CenterAWl50mA,spectrum.CenterASpect50mA);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterBWl) && !string.IsNullOrEmpty(spectrum.CenterBSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationB.Value,spectrum.CenterBWl,spectrum.CenterBSpect);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterBWl50mA) && !string.IsNullOrEmpty(spectrum.CenterBSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationB.Value,spectrum.CenterBWl50mA,spectrum.CenterBSpect50mA);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterCWl) && !string.IsNullOrEmpty(spectrum.CenterCSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationC.Value,spectrum.CenterCWl,spectrum.CenterCSpect);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterCWl50mA) && !string.IsNullOrEmpty(spectrum.CenterCSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationC.Value,spectrum.CenterCWl50mA,spectrum.CenterCSpect50mA);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterDWl) && !string.IsNullOrEmpty(spectrum.CenterDSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationD.Value,spectrum.CenterDWl,spectrum.CenterDSpect);
        }
        
        if (!string.IsNullOrEmpty(spectrum.CenterDWl50mA) && !string.IsNullOrEmpty(spectrum.CenterDSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationD.Value,spectrum.CenterDWl50mA,spectrum.CenterDSpect50mA);
        }
        
        if (!string.IsNullOrEmpty(spectrum.RightWl) && !string.IsNullOrEmpty(spectrum.RightSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationR,spectrum.RightWl,spectrum.RightSpect,"R2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.RightWl50mA) && !string.IsNullOrEmpty(spectrum.RightSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationR,spectrum.RightWl50mA,spectrum.RightSpect50mA,"R2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.TopWl) && !string.IsNullOrEmpty(spectrum.TopSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationR,spectrum.TopWl,spectrum.TopSpect,"T2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.TopWl50mA) && !string.IsNullOrEmpty(spectrum.TopSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationR,spectrum.TopWl50mA,spectrum.TopSpect50mA,"T2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.LeftWl) && !string.IsNullOrEmpty(spectrum.LeftSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationL,spectrum.LeftWl,spectrum.LeftSpect,"L2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.LeftWl50mA) && !string.IsNullOrEmpty(spectrum.LeftSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationL,spectrum.LeftWl50mA,spectrum.LeftSpect50mA,"L2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.BottomWl) && !string.IsNullOrEmpty(spectrum.BottomSpect)) {
            UpdateSpectrum(spectrum20mA,PadLocation.PadLocationG,spectrum.BottomWl,spectrum.BottomSpect,"G2-E");
        }
        
        if (!string.IsNullOrEmpty(spectrum.BottomWl50mA) && !string.IsNullOrEmpty(spectrum.BottomSpect50mA)) {
            UpdateSpectrum(spectrum50mA,PadLocation.PadLocationG,spectrum.BottomWl50mA,spectrum.BottomSpect50mA,"G2-E");
        }
        return new(spectrum20mA,spectrum50mA);
    }
    return null;
}