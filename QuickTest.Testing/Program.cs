// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using FastEndpoints;
using QuickTest.Data.Contracts.Requests;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Data.Dtos;
using QuickTest.Data.Models.Wafers;
using QuickTest.Data.Models.Wafers.Enums;

await GetWaferPad();

//await CreateWaferPads();

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
    client.BaseAddress = new Uri("http://localhost:5260");
    var request = new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationA,
        PadNumber = 0,
        PadMapDefinition = new PadMapDefinition(),
        WaferArea = WaferArea.Center,
        WaferSize = WaferSize.TwoInch
    };

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
        HttpResponseMessage response = await client.PostAsJsonAsync("api/wafer_pads/create", pad);
        Console.WriteLine(response.ToString());
    }

    foreach (var pad in rightPads) {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/wafer_pads/create", pad);
        Console.WriteLine(response.ToString());
    }
    
    foreach (var pad in bottomPads) {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/wafer_pads/create", pad);
        Console.WriteLine(response.ToString());
    }
    
    foreach (var pad in topPads) {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/wafer_pads/create", pad);
        Console.WriteLine(response.ToString());
    }
    
    foreach (var pad in leftPads) {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/wafer_pads/create", pad);
        Console.WriteLine(response.ToString());
    }


    Console.WriteLine("Hello, World!");
}