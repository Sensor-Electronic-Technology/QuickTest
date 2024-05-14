namespace QuickTest.Data.Contracts.Requests.Get;

public class GetResultRequest {
    public string? WaferId { get; set; }
}

public class GetManyResultsRequest {
    public List<string>? WaferIds { get; set; }
}