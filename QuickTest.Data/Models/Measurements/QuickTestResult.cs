using MongoDB.Bson;

namespace QuickTest.Data.Models.Measurements;

/*public class QuickTestResult {
    public ObjectId _id { get; set; }
    public string? WaferId { get; set; }
    public DateTime InitialTimeStamp { get; set; }
    public DateTime FinalTimeStamp { get; set; }
    public List<Measurement>? InitialMeasurements { get; set; }
    public List<Measurement>? FinalMeasurements { get; set; }
    /*public List<Spectrum>? InitialSpectrum { get; set; }
    public List<Spectrum>? FinalSpectrum { get; set; }#1#
}*/

public class QuickTestResult {
    public ObjectId _id { get; set; }
    public string? WaferId { get; set; }
    public DateTime InitialTimeStamp { get; set; }
    public DateTime FinalTimeStamp { get; set; }
}