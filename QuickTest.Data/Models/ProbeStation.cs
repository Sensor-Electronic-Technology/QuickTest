using MongoDB.Bson;

namespace QuickTest.Data.Models;

public class ProbeStation {
    public ObjectId _id { get; set; }
    public string Name { get; set; }
    public int StationNumber { get; set; }
    public string? MacAddress { get; set; }
}