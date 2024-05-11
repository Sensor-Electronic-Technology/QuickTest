using QuickTest.Data.Models.Wafers;

namespace QuickTest.Data.Contracts.Responses;

public class GetWaferPadsResponse {
    public IEnumerable<PadMapDefinition> Pads { get; set; }
}