using MongoDB.Bson;
using QuickTest.Data.Models.Wafers.Enums;

namespace QuickTest.Data.Models.Measurements;

/*public class Spectrum {
    public string? Pad { get; set; }
    public string? Current { get; set; }
    public List<double>? Wl { get; set; }
    public List<double>? Intensity { get; set; }
}*/

public class Spectrum {
    public ObjectId _id { get; set; }
    public string WaferId { get; set; }
    public ObjectId QuickTestResultId { get; set; }
    public MeasurementType MeasurementType { get; set; }
    public string? Pad { get; set; }
    public string? Current { get; set; }
    public List<double>? Wl { get; set; }
    public List<double>? Intensity { get; set; }
}