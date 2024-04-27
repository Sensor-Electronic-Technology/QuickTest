using QuickTest.Data.Wafer;

namespace QuickTest.Data.Contracts.Requests;

public class GetWaferPadsRequest {
    public WaferSize WaferSize { get; set; }
}