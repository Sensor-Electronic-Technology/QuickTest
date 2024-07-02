using QuickTest.Data.Models.Wafers;

namespace QuickTest.Data.DataTransfer;

public class LvWaferMapDto {
    public int Size { get; set; }
    public List<LvMapPad> Pads { get; set; } = new();
}