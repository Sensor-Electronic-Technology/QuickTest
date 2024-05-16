using QuickTest.Data.DataTransfer;

namespace QuickTest.Data.Contracts.Responses.Get;

public class GetAvailableBurnInPadsResponse {
    public IEnumerable<Pad> Pads { get; set; }
}