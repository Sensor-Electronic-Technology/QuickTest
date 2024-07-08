using QuickTest.Data.DataTransfer;

namespace QuickTest.Data.Events;

public class MeasurementInsertedEvent {
    public string? WaferId { get; set; }
    public string? PadLocation { get; set; }
    public string? ActualPad { get; set; }
    public int MeasurementType { get; set; }
    public List<SpectrumMeasureDto> SpectrumMeasurements { get; set; } = new();

}