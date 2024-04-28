namespace QuickTest.Data.Models.Measurements;

public class Spectrum {
    public string? Pad { get; set; }
    public string? Current { get; set; }
    public List<double>? Wl { get; set; }
    public List<double>? Intensity { get; set; }
}