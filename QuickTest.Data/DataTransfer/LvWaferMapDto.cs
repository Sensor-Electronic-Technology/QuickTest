using QuickTest.Data.Models.Wafers;

namespace QuickTest.Data.DataTransfer;

public class LvWaferMapDto {
    public int Size { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public int ContainerWidth { get; set; }
    public int ContainerHeight { get; set; }
    public double ZoomFactor { get; set; }
    public List<LvMapPad> Pads { get; set; } = new();
}