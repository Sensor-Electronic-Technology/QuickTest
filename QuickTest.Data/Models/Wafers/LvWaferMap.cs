using MongoDB.Bson;

namespace QuickTest.Data.Models.Wafers;

public class LvWaferMap  {
    public ObjectId _id { get; set; }
    public int Size { get; set; }
    public List<LvMapPad> Pads { get; set; } = new();
}