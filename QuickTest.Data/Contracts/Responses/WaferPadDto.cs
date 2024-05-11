using EpiData.Data.Models.Epi.Enums;
using QuickTest.Data.Models.Wafers;
using QuickTest.Data.Models.Wafers.Enums;

namespace QuickTest.Data.Contracts.Responses;

public class WaferPadDto {
    public string? Identifier { get; set; }
    public WaferSize? WaferSize { get; set; }
    public PadLocation? PadLocation { get; set; }
    public WaferArea? WaferArea { get; set; }
    public PadMapDefinition? SvgObject { get; set; }
    public int PadNumber { get; set; }
}