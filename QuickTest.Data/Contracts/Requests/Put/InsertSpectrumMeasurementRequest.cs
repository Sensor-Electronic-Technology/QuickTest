namespace QuickTest.Data.Contracts.Requests.Put;

public class InsertSpectrumMeasurementRequest {
    public string WaferId { get; set; }
    public string? PadLocation { get; set; }
    public string? ActualPad { get; set; }
    public int MeasurementType { get; set; }
    public int Current { get; set; }
    public List<double>? Wl { get; set; }
    public List<double>? Intensity { get; set; }
}

