namespace QuickTest.Data.Contracts.Requests.Get;

public class GetManyResultsRequest {
    public List<string>? WaferIds { get; set; }
    public int MeasurementType { get; set; }
}