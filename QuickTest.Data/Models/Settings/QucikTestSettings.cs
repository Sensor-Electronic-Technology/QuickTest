using MongoDB.Bson;

namespace QuickTest.Data.Models.Settings;

public abstract class Settings {
    public ObjectId _id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class WaferMapSettings:Settings {
    public string? FillColor { get; set; }
    public string? StrokeColor { get; set; }
}

public class PassFailCriteria:Settings {
    public string? EpiVersion { get; set; }
    public int MinPower { get; set; }
    public int MinVoltage { get; set; }
    public int MaxVoltage { get; set; }
    public int MinWl { get; set; }
    public int MaxWl { get; set; }
}