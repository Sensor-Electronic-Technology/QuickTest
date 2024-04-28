using MongoDB.Bson;

namespace QuickTest.Data.Models.Wafers;

public class Wafer {
    public ObjectId _id { get; set; }
    public string WaferId { get; set; }
    public string? SystemId { get; set; }
    public string? RunNumber { get; set; }
    public string? Pocket { get; set; }
    public string? VersionId { get; set; }
}