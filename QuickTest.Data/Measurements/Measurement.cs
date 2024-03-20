using MongoDB.Bson;
using QuickTest.Data.Wafer;
namespace QuickTest.Data.Measurements;

public class Measurement {
    public string Pad { get; set; }
    public MeasurementType MeasurementType { get; set; }
    public string Current { get; set; }
    public double Wl { get; set; }
    public double Power { get; set; }
    public double Voltage { get; set; }
    public double Knee { get; set; }
    public double Ir { get; set; }
}

