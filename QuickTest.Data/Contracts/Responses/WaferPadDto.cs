using QuickTest.Data.Wafer;
using QuickTest.Data.Wafer.Enums;

namespace QuickTest.Data.Contracts.Responses;

public class WaferPadDto {
    public string? Identifier { get; set; }
    public WaferSize? WaferSize { get; set; }
    public PadLocation? PadLocation { get; set; }
    public WaferArea? WaferArea { get; set; }
    public SvgObject? SvgObject { get; set; }
    public int PadNumber { get; set; }
}