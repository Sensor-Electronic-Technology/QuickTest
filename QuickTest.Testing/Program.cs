// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using QuickTest.Data.Dtos;
using QuickTest.Data.Wafer;
using QuickTest.Data.Wafer.Enums;

HttpClient client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5260");


var request = new CreateWaferPadRequest() {
    PadLocation = PadLocation.PadLocationA,
    PadNumber = 0,
    SvgObject = new SvgObject(),
    WaferArea = WaferArea.Center,
    WaferSize = WaferSize.TwoInch
};

List<CreateWaferPadRequest> centerPads = new List<CreateWaferPadRequest>() {
    new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationA,
        PadNumber = 0,
        SvgObject = new SvgObject() {
            X=368,
            Y=365,
            Radius=11,
            FillColor = "grey",
            Opacity = 0,
        },
        WaferArea = WaferArea.Center,
        WaferSize = WaferSize.TwoInch
    },
    new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationB,
        PadNumber = 0,
        SvgObject = new SvgObject() {
            X=368,
            Y=441,
            Radius=11,
            FillColor = "grey",
            Opacity = 0,
        },
        WaferArea = WaferArea.Center,
        WaferSize = WaferSize.TwoInch
    },
    new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationC,
        PadNumber = 0,
        SvgObject = new SvgObject() {
            X=445,
            Y=442,
            Radius=11,
            FillColor = "grey",
            Opacity = 0,
        },
        WaferArea = WaferArea.Center,
        WaferSize = WaferSize.TwoInch
    },
    new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationD,
        PadNumber = 0,
        SvgObject = new SvgObject() {
            X=445,
            Y=366,
            Radius=11,
            FillColor = "grey",
            Opacity = 0,
        },
        WaferArea = WaferArea.Center,
        WaferSize = WaferSize.TwoInch
    },
};
List<CreateWaferPadRequest> rightPads = new List<CreateWaferPadRequest>() {
    new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationR,
        PadNumber = 1,
        SvgObject = new SvgObject() {
            X=143,
            Y=365,
            Radius=8,
            FillColor = "grey",
            Opacity = 0,
        },
        WaferArea = WaferArea.Edge,
        WaferSize = WaferSize.TwoInch
    },
    new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationR,
        PadNumber = 2,
        SvgObject = new SvgObject() {
            X=160,
            Y=365,
            Radius=8,
            FillColor = "grey",
            Opacity = 0,
        },
        WaferArea = WaferArea.Edge,
        WaferSize = WaferSize.TwoInch
    },
    new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationR,
        PadNumber = 3,
        SvgObject = new SvgObject() {
            X=180,
            Y=366,
            Radius=8,
            FillColor = "grey",
            Opacity = 0,
        },
        WaferArea = WaferArea.Edge,
        WaferSize = WaferSize.TwoInch
    },
    new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationR,
        PadNumber = 4,
        SvgObject = new SvgObject() {
            X=142,
            Y=441,
            Radius=8,
            FillColor = "grey",
            Opacity = 0,
        },
        WaferArea = WaferArea.Edge,
        WaferSize = WaferSize.TwoInch
    },
    new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationR,
        PadNumber = 5,
        SvgObject = new SvgObject() {
            X=161,
            Y=441,
            Radius=8,
            FillColor = "grey",
            Opacity = 0,
        },
        WaferArea = WaferArea.Edge,
        WaferSize = WaferSize.TwoInch
    },
    new CreateWaferPadRequest() {
        PadLocation = PadLocation.PadLocationR,
        PadNumber = 6,
        SvgObject = new SvgObject() {
            X=179,
            Y=442,
            Radius=8,
            FillColor = "grey",
            Opacity = 0,
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


Console.WriteLine("Hello, World!");