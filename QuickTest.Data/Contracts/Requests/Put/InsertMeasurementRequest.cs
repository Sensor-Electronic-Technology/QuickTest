using QuickTest.Data.DataTransfer;

namespace QuickTest.Data.Contracts.Requests.Put;

/*public class InsertMeasurementRequest {
    public string? WaferId { get; set; }
    public string? PadLocation { get; set; }
    public string? ActualPad { get; set; }
    public int MeasurementType { get; set; }
    public int Current { get; set; }
    public double Wl { get; set; }
    public double Power { get; set; }
    public double Voltage { get; set; }
    public double Knee { get; set; }
    public double Ir { get; set; }
}*/

public class InsertMeasurementRequest {
    public string? WaferId { get; set; }
    public string? PadLocation { get; set; }
    public string? ActualPad { get; set; }
    public int MeasurementType { get; set; }
    public List<CurrentMeasurementDto> Measurements { get; set; } = new();
    public List<SpectrumMeasureDto> SpectrumMeasurements { get; set; } = new();

}