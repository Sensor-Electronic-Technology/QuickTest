using MongoDB.Bson;

namespace QuickTest.Data.Models.Measurements;

public class MeasurementCurrent {
    public ObjectId _id { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
}