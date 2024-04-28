using MongoDB.Bson;
using QuickTest.Data.Models.Wafers.Enums;

namespace QuickTest.Data.Models.Wafers;

public class WaferPad {
    public ObjectId _id { get; set; }
    public string? Identifier { get; set; }
    public int PadNumber { get; set; }
    public WaferSize? WaferSize { get; set; }
    public PadLocation? PadLocation { get; set; }
    public WaferArea? WaferArea { get; set; }
    public PadMapDefinition? SvgObject { get; set; }

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

public class PadMapDefinition {
     public int X { get; set; }
     public int Y { get; set; }
     public int Radius { get; set; }
}

