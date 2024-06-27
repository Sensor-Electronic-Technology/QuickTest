using EpiData.Data.Models.Epi.Enums;
using MongoDB.Bson;
using QuickTest.Data.DataTransfer;
namespace QuickTest.Data.Models.Wafers;

public class WaferMap {
    public ObjectId _id { get; set; }
    public WaferSize? WaferSize { get; set; }
    public string? ViewBoxString { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public int SvgWidth { get; set; }
    public int SvgHeight { get; set; }
    public string? WaferMapPath { get; set; }
    public List<ObjectId> PadIds { get; set; } = [];

    public WaferMapDto WaferMapDto() {
        return new WaferMapDto() {
            ImageHeight = this.ImageHeight,
            ImageWidth = this.ImageWidth,
            SvgHeight = this.SvgHeight,
            SvgWidth = this.SvgWidth,
            ViewBoxString = this.ViewBoxString,
            WaferMapPath = this.WaferMapPath,
            WaferSize = this.WaferSize!.Value
        };
    }
}