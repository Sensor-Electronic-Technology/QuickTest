namespace QuickTest.Data.DataTransfer;

public class SpectrumMeasureDto {
    public int Current { get; set; }
    public List<double>? Wl { get; set; }
    public List<double>? Intensity { get; set; }
}