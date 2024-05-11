using MongoDB.Bson;

namespace QuickTest.Data.Models.Wafers;

public class QuickTestWafer {
    public ObjectId _id { get; set; }
    public string WaferId { get; set; }
    public bool InitialTested { get; set; }
    public bool FinalTested { get; set; }
}