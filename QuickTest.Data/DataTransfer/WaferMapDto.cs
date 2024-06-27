namespace QuickTest.Data.DataTransfer;

public class WaferMapDto {
    public int WaferSize { get; set; }
    public string? ViewBoxString { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public int SvgWidth { get; set; }
    public int SvgHeight { get; set; }
    public string? WaferMapPath { get; set; }
    public List<Pad> MapPads { get; set; } = [];
}