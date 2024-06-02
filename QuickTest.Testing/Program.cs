// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using EpiData.Data.Models.Epi.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Data.Models.Wafers;
using QuickTest.Data.Models.Wafers.Enums;
using QuickTest.Data.Contracts.Requests.Post;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Data.Models;
using QuickTest.Data.Models.Measurements;
using QuickTest.Infrastructure.Services;


/*await CreateWaferPads();
await CreateFourInchWaferPads();

await CreateWaferMaps();*/
//await GetWaferPadsTest();
//await GetNewMap();

await CloneDatabase();


async Task CloneDatabase(){
    var mongoClient = new MongoClient("mongodb://172.20.3.41:27017/");
    var database = mongoClient.GetDatabase("quick_test_db");
    
    var pimongoClient = new MongoClient("mongodb://192.168.68.111:27017/");
    var pidatabase = pimongoClient.GetDatabase("quick_test_db");
    
    var qtCollection=database.GetCollection<QuickTestResult>("quick_test");
    var initMeasureCollection=database.GetCollection<QtMeasurement>("init_measurements");
    var finalMeasureCollection=database.GetCollection<QtMeasurement>("final_measurements");
    var initSpectrumCollection=database.GetCollection<Spectrum>("init_spectrum");
    var finalSpectrumCollection=database.GetCollection<Spectrum>("final_spectrum");
    var probeStationCollection=database.GetCollection<ProbeStation>("probe_stations");
    var waferPadCollection = database.GetCollection<WaferPad>("wafer_pads");
    var waferMapCollection = database.GetCollection<WaferMap>("wafer_maps");
    
    var piQtCollection=pidatabase.GetCollection<QuickTestResult>("quick_test");
    var piInitMeasureCollection=pidatabase.GetCollection<QtMeasurement>("init_measurements");
    var piFinalMeasureCollection=pidatabase.GetCollection<QtMeasurement>("final_measurements");
    var piInitSpectrumCollection=pidatabase.GetCollection<Spectrum>("init_spectrum");
    var piFinalSpectrumCollection=pidatabase.GetCollection<Spectrum>("final_spectrum");
    var piProbeStationCollection=pidatabase.GetCollection<ProbeStation>("probe_stations");
    var piWaferPadCollection = pidatabase.GetCollection<WaferPad>("wafer_pads");
    var piWaferMapCollection = pidatabase.GetCollection<WaferMap>("wafer_maps");

    Console.WriteLine("Collecting Data...");

    var qtList=qtCollection.AsQueryable().Take(1000).ToList();
    var qtListIds=qtList.Select(e => e._id);
    var initMeasureList = await initMeasureCollection.Find(e => qtListIds.Contains(e.QuickTestResultId)).ToListAsync();
    var finalMeasureList = await finalMeasureCollection.Find(e => qtListIds.Contains(e.QuickTestResultId)).ToListAsync();
    var initSpectMeasureList = await initSpectrumCollection.Find(e => qtListIds.Contains(e.QuickTestResultId)).ToListAsync();
    var finalSpectMeasureList = await finalSpectrumCollection.Find(e => qtListIds.Contains(e.QuickTestResultId)).ToListAsync();
    var probeStationList = await probeStationCollection.Find(_=>true).ToListAsync();
    var waferPadList = await waferPadCollection.Find(_=>true).ToListAsync();
    var waferMapList = await waferMapCollection.Find(_=>true).ToListAsync();

    Console.WriteLine("Cloning Data...");
    
    await piQtCollection.InsertManyAsync(qtList);
    await piInitMeasureCollection.InsertManyAsync(initMeasureList);
    await piFinalMeasureCollection.InsertManyAsync(finalMeasureList);
    await piInitSpectrumCollection.InsertManyAsync(initSpectMeasureList);
    await piFinalSpectrumCollection.InsertManyAsync(finalSpectMeasureList);
    await piProbeStationCollection.InsertManyAsync(probeStationList);
    await piWaferPadCollection.InsertManyAsync(waferPadList);
    await piWaferMapCollection.InsertManyAsync(waferMapList);

    Console.WriteLine("Check Pi Database");


}

async Task GetNewMap() {
    var mongoClient = new MongoClient("mongodb://172.20.3.41:27017/");
    WaferDataService waferDataService = new WaferDataService(mongoClient);
    var waferMap=await waferDataService.GetMap(WaferSize.TwoInch);
    foreach (var pad in waferMap.MapPads) {
        Console.WriteLine($"Id: {pad.Identifier}");
    }
}

async Task GetWaferPadsTest() {
    var mongoClient = new MongoClient("mongodb://172.20.3.41:27017/");
    WaferDataService waferDataService = new WaferDataService(mongoClient);
    var waferPads=await waferDataService.GetPadsOther(WaferSize.TwoInch);
    foreach (var pad in waferPads) {
        Console.WriteLine($"Id: {pad.Identifier} Size: {pad.WaferSize.Name}");
    }
}

async Task CreateWaferMaps() {
    var mongoClient = new MongoClient("mongodb://172.20.3.41:27017/");
    WaferDataService waferDataService = new WaferDataService(mongoClient);

    var twoInPads = await waferDataService.GetPadsOther(WaferSize.TwoInch);
    var fourInPads = await waferDataService.GetPadsOther(WaferSize.FourInch);
    
    WaferMap twoInMap = new WaferMap() {
        _id = ObjectId.GenerateNewId(),
        WaferSize = WaferSize.TwoInch,
        SvgWidth = 826,
        SvgHeight = 810,
        ImageWidth = 426,
        ImageHeight = 410,
        WaferMapPath = "images/WaferMask2in.png",
        PadIds = twoInPads.Select(e=>e._id).ToList()
    };
    twoInMap.ViewBoxString = $"0 0 {twoInMap.SvgWidth} {twoInMap.SvgHeight}";

    WaferMap fourInMap = new WaferMap() {
        _id = ObjectId.GenerateNewId(),
        WaferSize = WaferSize.FourInch,
        SvgWidth = 851,
        SvgHeight = 852,
        ImageWidth = 451,
        ImageHeight = 452,
        WaferMapPath = "images/Wafer4Inch.PNG",
        PadIds = fourInPads.Select(e=>e._id).ToList()
    };
    fourInMap.ViewBoxString = $"0 0 {fourInMap.SvgWidth} {fourInMap.SvgHeight}";
    await waferDataService.CreateWaferMap(twoInMap);
    await waferDataService.CreateWaferMap(fourInMap);
    Console.WriteLine("Check Database");
}

async Task GetWaferList() {
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("http://172.20.4.206");
    var start = new DateTime(2023, 1, 1);
    Console.WriteLine("Sending request...");
    var waferListResponse=await client.GetFromJsonAsync<GetQuickTestListResponse>($"{QtApiPaths.GetQuickTestListSincePath}{start.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture)}");
    if (waferListResponse != null) {
        Console.WriteLine("Found Wafers");
        foreach (var waferId in waferListResponse.WaferList) {
            Console.WriteLine(waferId);
        }
    } else {
        Console.WriteLine("Error: Response was null");
    }
}

async Task TestCheck() {
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("http://localhost:5260");
    var checkRequest = new CheckQuickTestRequest() { WaferId = "B01-3482-10", MeasurementType = 0 };
    //client.PostAsJsonAsync($"{QtApiPaths.CheckQuickTestPath}{checkRequest}", checkRequest);
    var response=await client.GetAsync($"{QtApiPaths.CheckQuickTestPath}{checkRequest.WaferId}/{checkRequest.MeasurementType}");
    Console.WriteLine(response.ToString());
}

async Task GetWaferPad() {
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("http://localhost:5260");
    var waferPads = await client.GetFromJsonAsync<IEnumerable<WaferPadDto>>($"api/wafer_pads/{WaferSize.TwoInch.Value}");
    if(waferPads == null) {
        Console.WriteLine("No wafer pads found");
        return;
    }
    foreach (var waferPad in waferPads) {
        Console.WriteLine($"Pad: {waferPad.Identifier}");
    }
}

async Task DeleteWaferPads() {
    var client = new MongoClient("mongodb://172.20.3.41:27017/");
    var database = client.GetDatabase("quick_test_db");
    var collection=database.GetCollection<WaferPad>("wafer_pads");
}

async Task CreateFourInchWaferPads() {
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("http://172.20.4.206/");
    List<(PadLocation, WaferArea, int, int, int, int)> _coord = 
    [
        (PadLocation.PadLocationL, WaferArea.Edge, 1, 121, 369, 7),
        (PadLocation.PadLocationL, WaferArea.Edge, 2, 136, 368, 7),
        (PadLocation.PadLocationL, WaferArea.Edge, 3, 153, 370, 6),
        (PadLocation.PadLocationL, WaferArea.Edge, 4, 122, 433, 5),
        (PadLocation.PadLocationL, WaferArea.Edge, 5, 136, 433, 5),
        (PadLocation.PadLocationL, WaferArea.Edge, 6, 152, 433, 5),

        (PadLocation.PadLocationL, WaferArea.Middle, 1, 242, 372, 6),
        (PadLocation.PadLocationL, WaferArea.Middle, 2, 259, 372, 6),
        (PadLocation.PadLocationL, WaferArea.Middle, 3, 273, 373, 6),
        (PadLocation.PadLocationL, WaferArea.Middle, 4, 243, 437, 6),
        (PadLocation.PadLocationL, WaferArea.Middle, 5, 257, 436, 6),
        (PadLocation.PadLocationL, WaferArea.Middle, 6, 274, 437, 6),

        (PadLocation.PadLocationG, WaferArea.Edge, 1, 399, 708, 6),
        (PadLocation.PadLocationG, WaferArea.Edge, 2, 399, 724, 6),
        (PadLocation.PadLocationG, WaferArea.Edge, 3, 398, 740, 6),
        (PadLocation.PadLocationG, WaferArea.Edge, 4, 467, 709, 7),
        (PadLocation.PadLocationG, WaferArea.Edge, 5, 465, 722, 5),
        (PadLocation.PadLocationG, WaferArea.Edge, 6, 465, 738, 5),

        (PadLocation.PadLocationG, WaferArea.Middle, 1, 401, 581, 5),
        (PadLocation.PadLocationG, WaferArea.Middle, 2, 400, 596, 7),
        (PadLocation.PadLocationG, WaferArea.Middle, 3, 400, 612, 7),
        (PadLocation.PadLocationG, WaferArea.Middle, 4, 469, 580, 7),
        (PadLocation.PadLocationG, WaferArea.Middle, 5, 469, 595, 6),
        (PadLocation.PadLocationG, WaferArea.Middle, 6, 470, 612, 7),

        (PadLocation.PadLocationR, WaferArea.Middle, 1, 583, 385, 6),
        (PadLocation.PadLocationR, WaferArea.Middle, 2, 600, 385, 7),
        (PadLocation.PadLocationR, WaferArea.Middle, 3, 616, 385, 6),
        (PadLocation.PadLocationR, WaferArea.Middle, 4, 584, 444, 7),
        (PadLocation.PadLocationR, WaferArea.Middle, 5, 601, 446, 6),
        (PadLocation.PadLocationR, WaferArea.Middle, 6, 617, 445, 6),

        (PadLocation.PadLocationR, WaferArea.Edge, 1, 700, 383, 5),
        (PadLocation.PadLocationR, WaferArea.Edge, 2, 717, 383, 6),
        (PadLocation.PadLocationR, WaferArea.Edge, 3, 732, 383, 6),
        (PadLocation.PadLocationR, WaferArea.Edge, 4, 704, 444, 7),
        (PadLocation.PadLocationR, WaferArea.Edge, 5, 719, 445, 5),
        (PadLocation.PadLocationR, WaferArea.Edge, 6, 734, 444, 6),

        (PadLocation.PadLocationT, WaferArea.Edge, 1, 397, 135, 7),
        (PadLocation.PadLocationT, WaferArea.Edge, 2, 396, 121, 6),
        (PadLocation.PadLocationT, WaferArea.Edge, 3, 397, 105, 6),
        (PadLocation.PadLocationT, WaferArea.Edge, 4, 462, 136, 6),
        (PadLocation.PadLocationT, WaferArea.Edge, 5, 461, 122, 6),
        (PadLocation.PadLocationT, WaferArea.Edge, 6, 462, 106, 6),

        (PadLocation.PadLocationT, WaferArea.Middle, 1, 397, 262, 6),
        (PadLocation.PadLocationT, WaferArea.Middle, 2, 396, 246, 6),
        (PadLocation.PadLocationT, WaferArea.Middle, 3, 397, 231, 6),
        (PadLocation.PadLocationT, WaferArea.Middle, 4, 461, 263, 4),
        (PadLocation.PadLocationT, WaferArea.Middle, 5, 462, 248, 6),
        (PadLocation.PadLocationT, WaferArea.Middle, 6, 463, 232, 6),

        (PadLocation.PadLocationA, WaferArea.Center, 0, 398, 376, 6),
        (PadLocation.PadLocationB, WaferArea.Center, 0, 398, 439, 7),
        (PadLocation.PadLocationC, WaferArea.Center, 0, 463, 440, 7),
        (PadLocation.PadLocationD, WaferArea.Center, 0, 462, 375, 7),
    ];
    
    foreach(var pad in _coord) {
        CreateWaferPadRequest request = new CreateWaferPadRequest() {
            PadLocation = pad.Item1,
            WaferArea = pad.Item2,
            PadNumber = pad.Item3,
            PadMapDefinition = new PadMapDefinition() {
                X = pad.Item4,
                Y = pad.Item5,
                Radius = pad.Item6
            },
            WaferSize = WaferSize.FourInch
        };
        HttpResponseMessage response = await client.PostAsJsonAsync("api/pads/create/{waferPad}", request);
        Console.WriteLine(response.ToString());
    }
}

async Task CreateWaferPads() { 
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("http://172.20.4.206");
    
    List<CreateWaferPadRequest> centerPads = new List<CreateWaferPadRequest>() {
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationA,
            PadNumber = 0,
            PadMapDefinition = new PadMapDefinition() {
                X=368,
                Y=365,
                Radius=11,
            },
            WaferArea = WaferArea.Center,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationB,
            PadNumber = 0,
            PadMapDefinition = new PadMapDefinition() {
                X=368,
                Y=441,
                Radius=11,
            },
            WaferArea = WaferArea.Center,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationC,
            PadNumber = 0,
            PadMapDefinition = new PadMapDefinition() {
                X=445,
                Y=442,
                Radius=11,
            },
            WaferArea = WaferArea.Center,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationD,
            PadNumber = 0,
            PadMapDefinition = new PadMapDefinition() {
                X=445,
                Y=366,
                Radius=11,
            },
            WaferArea = WaferArea.Center,
            WaferSize = WaferSize.TwoInch
        },
    };

    List<CreateWaferPadRequest> leftPads = new List<CreateWaferPadRequest>() {
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationL,
            PadNumber = 1,
            PadMapDefinition = new PadMapDefinition() {
                X=143,
                Y=365,
                Radius=7,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationL,
            PadNumber = 2,
            PadMapDefinition = new PadMapDefinition() {
                X=160,
                Y=365,
                Radius=7,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationL,
            PadNumber = 3,
            PadMapDefinition = new PadMapDefinition() {
                X=178,
                Y=365,
                Radius=7,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationL,
            PadNumber = 4,
            PadMapDefinition = new PadMapDefinition() {
                X=141,
                Y=441,
                Radius=7,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationL,
            PadNumber = 5,
            PadMapDefinition = new PadMapDefinition() {
                X=160,
                Y=442,
                Radius=7,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationL,
            PadNumber = 6,
            PadMapDefinition = new PadMapDefinition() {
                X=179,
                Y=442,
                Radius=7,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
    };
    
    List<CreateWaferPadRequest> rightPads = new List<CreateWaferPadRequest>() {
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 1,
            PadMapDefinition = new PadMapDefinition() {
                X=632,
                Y=373,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 2,
            PadMapDefinition = new PadMapDefinition() {
                X=651,
                Y=373,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 3,
            PadMapDefinition = new PadMapDefinition() {
                X=669,
                Y=373,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 4,
            PadMapDefinition = new PadMapDefinition() {
                X=634,
                Y=445,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 5,
            PadMapDefinition = new PadMapDefinition() {
                X=653,
                Y=445,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 6,
            PadMapDefinition = new PadMapDefinition() {
                X=672,
                Y=445,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
    };
    
    List<CreateWaferPadRequest> bottomPads = new List<CreateWaferPadRequest>() {
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationG,
            PadNumber = 1,
            PadMapDefinition = new PadMapDefinition() {
                X=364,
                Y=649,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationG,
            PadNumber = 2,
            PadMapDefinition = new PadMapDefinition() {
                X=364,
                Y=668,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationG,
            PadNumber = 3,
            PadMapDefinition = new PadMapDefinition() {
                X=364,
                Y=687,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationG,
            PadNumber = 4,
            PadMapDefinition = new PadMapDefinition() {
                X=443,
                Y=652,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationG,
            PadNumber = 5,
            PadMapDefinition = new PadMapDefinition() {
                X=443,
                Y=670,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationG,
            PadNumber = 6,
            PadMapDefinition = new PadMapDefinition() {
                X=442,
                Y=688,
                Radius=7
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
    };
    
    List<CreateWaferPadRequest> topPads = new List<CreateWaferPadRequest>() {
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationT,
            PadNumber = 1,
            PadMapDefinition = new PadMapDefinition() {
                X=365,
                Y=164,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationT,
            PadNumber = 2,
            PadMapDefinition = new PadMapDefinition() {
                X=365,
                Y=146,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationT,
            PadNumber = 3,
            PadMapDefinition = new PadMapDefinition() {
                X=365,
                Y=127,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationT,
            PadNumber = 4,
            PadMapDefinition = new PadMapDefinition() {
                X=445,
                Y=163,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationT,
            PadNumber = 5,
            PadMapDefinition = new PadMapDefinition() {
                X=445,
                Y=144,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationT,
            PadNumber = 6,
            PadMapDefinition = new PadMapDefinition() {
                X=445,
                Y=126,
                Radius=8
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
    };
    
    Console.WriteLine("Press any key to create a wafer pad...");
    Console.ReadKey();
    foreach (var pad in centerPads) {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/pads/create/{waferPad}", pad);
        Console.WriteLine(response.ToString());
    }

    foreach (var pad in rightPads) {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/pads/create/{waferPad}", pad);
        Console.WriteLine(response.ToString());
    }
    
    foreach (var pad in bottomPads) {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/pads/create/{waferPad}", pad);
        Console.WriteLine(response.ToString());
    }
    
    foreach (var pad in topPads) {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/pads/create/{waferPad}", pad);
        
        Console.WriteLine(response.ToString());
    }
    
    foreach (var pad in leftPads) {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/pads/create/{waferPad}", pad);
        Console.WriteLine(response.ToString());
    }


    Console.WriteLine("Hello, World!");
}