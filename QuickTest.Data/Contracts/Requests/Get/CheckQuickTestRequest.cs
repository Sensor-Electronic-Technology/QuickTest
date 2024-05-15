using QuickTest.Data.Models.Measurements;

namespace QuickTest.Data.Contracts.Requests.Get;

public class CheckQuickTestRequest {
    public string? WaferId { get; set; }
    public int MeasurementType { get; set; }
}