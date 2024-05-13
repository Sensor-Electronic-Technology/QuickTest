using EpiData.Data.Models.Epi.Enums;
using QuickTest.Data.Models.Wafers;
using QuickTest.Data.Models.Wafers.Enums;

namespace QuickTest.Data.Contracts.Requests.Post;

public class CreateWaferPadRequest {
    public required WaferSize WaferSize { get; set; }
    public required PadLocation PadLocation { get; set; }
    public required WaferArea WaferArea { get; set; }
    public required PadMapDefinition PadMapDefinition { get; set; }
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