namespace QuickTest.Data.Contracts.Responses.Get;

public class CheckQuickTestResponse {
    public bool Exists { get; set; }
    public bool Tested { get; set; }
    public int StationId { get; set; }
}