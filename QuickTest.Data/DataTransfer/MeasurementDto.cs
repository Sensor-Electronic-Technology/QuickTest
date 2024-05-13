using System.ComponentModel.DataAnnotations;

namespace QuickTest.Data.DataTransfer;

public class MeasurementDto {
    public string? WaferId { get; set; }
    public string? QuickTestResultId { get; set; }
    public string? Pad { get; set; }
    public int MeasurementType { get; set; }
    public string? Current { get; set; }
    public double Wl { get; set; }
    public double Power { get; set; }
    public double Voltage { get; set; }
    public double Knee { get; set; }
    public double Ir { get; set; }
}