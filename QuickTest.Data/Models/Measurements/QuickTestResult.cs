using MongoDB.Bson;

namespace QuickTest.Data.Models.Measurements;

public class QuickTestResult {
    public ObjectId _id { get; set; }
    public string? WaferId { get; set; }
    public int WaferSize { get; set; }
    public int ProbeStationId { get; set; }
    public bool InitialTested { get; set; }
    public DateTime InitialTimeStamp { get; set; }
    public bool FinalTested { get; set; }
    public DateTime FinalTimeStamp { get; set; }
    
    public ObjectId Initial20MeasurementId { get; set; }
    public ObjectId Initial50MeasurementId { get; set; }
    public ObjectId Final20MeasurementId { get; set; }
    public ObjectId Final50MeasurementId { get; set; }
    
    public ObjectId Initial20SpectMeasurementId { get; set; }
    public ObjectId Initial50SpectMeasurementId { get; set; }
    public ObjectId FinalSpect20MeasurementId { get; set; }
    public ObjectId FinalSpect50MeasurementId { get; set; }
}