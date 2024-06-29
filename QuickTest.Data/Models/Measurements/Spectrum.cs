using MongoDB.Bson;
using QuickTest.Data.Models.Wafers.Enums;

namespace QuickTest.Data.Models.Measurements;

public class Spectrum:Measurement {
    public Dictionary<string, PadSpectrumMeasurement> Measurements { get; set; } = new();
}

public class PadSpectrumMeasurement {
    public string ActualPad { get; set; }
    public List<double>? Wl { get; set; }
    public List<double>? Intensity { get; set; }
}