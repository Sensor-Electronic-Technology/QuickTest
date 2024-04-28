using MongoDB.Bson;

namespace QuickTest.Data.Models.Wafers;

public class EpiVersion {
    public ObjectId Id { get; set; }
    public string? Identifier { get; set; }
    public string? Name { get; set; }
}