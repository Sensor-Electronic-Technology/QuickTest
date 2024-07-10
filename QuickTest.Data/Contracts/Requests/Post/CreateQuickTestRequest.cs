namespace QuickTest.Data.Contracts.Requests.Post;

public class CreateQuickTestRequest {
    public string WaferId { get; set; }
    public int WaferSize { get; set; }
    public int ProbeStationId { get; set; }
}