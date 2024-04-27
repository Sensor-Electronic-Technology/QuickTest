using MongoDB.Bson;
using QuickTest.Data.Dtos;
using QuickTest.Data.Wafer.Enums;

namespace QuickTest.Data.Wafer;

public class WaferPad {
    public ObjectId _id { get; set; }
    public string? Identifier { get; set; }
    public int PadNumber { get; set; }
    public WaferSize? WaferSize { get; set; }
    public PadLocation? PadLocation { get; set; }
    public WaferArea? WaferArea { get; set; }
    public SvgObject? SvgObject { get; set; }

    public WaferPad() {
        
    }
    
    public WaferPad(WaferSize size,PadLocation location, WaferArea area,int padNumber) {
        this.WaferSize = size;
        this.PadLocation = location;
        this.WaferArea = area;
        this.PadNumber = padNumber;
        this.Identifier = $"{location.Value}-{area.Value}-{padNumber}";
    }
}

public class SvgObject {
     public int X { get; set; }
     public int Y { get; set; }
     public int Radius { get; set; }
     public string? FillColor { get; set; }
     public double Opacity { get; set; }
}

