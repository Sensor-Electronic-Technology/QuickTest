// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using System.Text.Json;
using EpiData.Data.Models.Epi.Enums;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Data.Models.Wafers;
using QuickTest.Data.Models.Wafers.Enums;
using QuickTest.Data.Contracts.Requests.Post;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Infrastructure.Services;

//await GetWaferPad();

//await CreateWaferPads();

//await TestClientGetWafer();

/*await TestClientGetManyWafers();*/

//TestFormatString();

//await TestQuickTest();

/*void TestFormatString() {
    string format = "api/wafer/{waferId}";
    
    Console.WriteLine(format.Replace("waferIds","B01-4520-01"));
}*/

/*async Task TestQuickTest() {
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("http://172.20.4.206");
    GetInitialExcelResultsRequest excelResultsRequest = new GetInitialExcelResultsRequest() {
        WaferId = "B03-2615-07"
    };
    
    var response=await client.GetAsync($"/api/results/initial/B03-2615-07");
    Console.WriteLine(response.ToString());
    Console.Write(response.Content.ToString());
    Console.WriteLine(response.Headers.ToString());
    Console.WriteLine(response.RequestMessage?.ToString());
    
}*/

/*async Task TestClientGetWafer() {
    var service = new EpiDataService();
    var waferId = "B01-4520-01";
    Console.WriteLine("Press any key to check if wafer exists...");
    Console.ReadLine();
    var wafer =await service.GetWaferById(waferId);
    if(wafer == null) {
        Console.WriteLine("Wafer does not exist");
    } else {
        Console.WriteLine($"Wafer exists: {wafer.TemplateId}");
    }
}

async Task TestClientGetManyWafers() {
    var service = new EpiDataService();
    DateTime growthDate = new DateTime(2024, 1, 1);
    
    Console.WriteLine($"Press any key to get wafers since {growthDate.ToShortDateString()}...");
    Console.ReadLine();
    var waferList =await service.GetLedWafersSince(growthDate);
    if(waferList == null) {
        Console.WriteLine("Wafer does not exist");
    } else {
        foreach (var wafer in waferList) {
            Console.WriteLine(wafer);
        }
    }
}*/

/*async Task TestClient() {
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("http://localhost:5142/");
    var clientAdapter=new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(),httpClient:httpClient);
    EpiDataClient client = new EpiDataClient(clientAdapter);
    Console.WriteLine("Press any key to check if wafer exists...");
    Console.ReadLine();
    //var response=await client.Api.Wafer.Led.Exists["B01-4520-01"].GetAsync();
    var response= await client.Api.Wafer.Led["B01-4520-01"].GetAsync();
    if(response.LedWafer == null) {
        Console.WriteLine("Wafer does not exist");
    } else {
        Console.WriteLine($"Wafer exists: {response.LedWafer.TemplateId}");
    }
}*/

await TestCheck();


async Task TestCheck() {
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("http://localhost:5260");
    var checkRequest = new CheckQuickTestRequest() { WaferId = "B01-3482-10", MeasurementType = 0 };
    //client.PostAsJsonAsync($"{QtApiPaths.CheckQuickTestPath}{checkRequest}", checkRequest);
    var response=await client.GetAsync($"{QtApiPaths.CheckQuickTestPath}/{checkRequest.WaferId}/{checkRequest.MeasurementType}");
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

    List<CreateWaferPadRequest> rightPads = new List<CreateWaferPadRequest>() {
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 1,
            PadMapDefinition = new PadMapDefinition() {
                X=143,
                Y=365,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 2,
            PadMapDefinition = new PadMapDefinition() {
                X=160,
                Y=365,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 3,
            PadMapDefinition = new PadMapDefinition() {
                X=180,
                Y=366,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 4,
            PadMapDefinition = new PadMapDefinition() {
                X=142,
                Y=441,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 5,
            PadMapDefinition = new PadMapDefinition() {
                X=161,
                Y=441,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationR,
            PadNumber = 6,
            PadMapDefinition = new PadMapDefinition() {
                X=179,
                Y=442,
                Radius=8,
            },
            WaferArea = WaferArea.Edge,
            WaferSize = WaferSize.TwoInch
        },
    };
    
    List<CreateWaferPadRequest> leftPads = new List<CreateWaferPadRequest>() {
        new CreateWaferPadRequest() {
            PadLocation = PadLocation.PadLocationL,
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
            PadLocation = PadLocation.PadLocationL,
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
            PadLocation = PadLocation.PadLocationL,
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
            PadLocation = PadLocation.PadLocationL,
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
            PadLocation = PadLocation.PadLocationL,
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
            PadLocation = PadLocation.PadLocationL,
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
                X=688,
                Y=445,
                Radius=8
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