using QuickTest.Data.Models.Measurements;

namespace QuickTest.Data.Contracts.Responses;

public class QuickTestResultDto {
    public string? WaferId { get; set; }
    public DateTime InitialTimeStamp { get; set; }
    public DateTime FinalTimeStamp { get; set; }
    public List<Measurement>? InitialMeasurements { get; set; }
    public List<Measurement>? FinalMeasurements { get; set; }
}