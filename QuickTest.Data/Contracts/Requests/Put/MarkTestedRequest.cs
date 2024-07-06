namespace QuickTest.Data.Contracts.Requests.Put;

public class MarkTestedRequest {
    public string WaferId { get; set; }
    public int MeasurementType { get; set; }
    public bool Tested { get; set; }
}