using QuickTest.Data.Wafer;
using QuickTest.Data.Wafer.Enums;

namespace QuickTest.Data.Dtos;

public class CreateWaferPadRequest {
    public required WaferSize WaferSize { get; set; }
    public required PadLocation PadLocation { get; set; }
    public required WaferArea WaferArea { get; set; }
    public required SvgObject SvgObject { get; set; }
    public required int PadNumber { get; set; }

    public CreateWaferPadRequest() {
        
    }
    
    public CreateWaferPadRequest(WaferSize size,PadLocation location, WaferArea area,int padNumber) {
        this.WaferSize = size;
        this.PadLocation = location;
        this.WaferArea = area;
        this.PadNumber = padNumber;
    }
}