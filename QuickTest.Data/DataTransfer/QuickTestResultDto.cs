using QuickTest.Data.Models.Measurements;

namespace QuickTest.Data.DataTransfer;

public class QuickTestResultDto {
    public string? WaferId { get; set; }
    public DateTime InitialTimeStamp { get; set; }
    public DateTime FinalTimeStamp { get; set; }
    public List<Measurement>? InitialMeasurements { get; set; }
    public List<Measurement>? FinalMeasurements { get; set; }
}