using Ardalis.SmartEnum;

namespace QuickTest.Data.Models.Wafers.Enums;

public class PadLocation : SmartEnum<PadLocation, string> {
    public static PadLocation PadLocationA = new PadLocation(nameof(PadLocationA), "A");
    public static PadLocation PadLocationB = new PadLocation(nameof(PadLocationB), "B");
    public static PadLocation PadLocationC = new PadLocation(nameof(PadLocationC), "C");
    public static PadLocation PadLocationD = new PadLocation(nameof(PadLocationD), "D");
    public static PadLocation PadLocationL = new PadLocation(nameof(PadLocationL), "L");
    public static PadLocation PadLocationR = new PadLocation(nameof(PadLocationR), "R");
    public static PadLocation PadLocationT = new PadLocation(nameof(PadLocationT), "T");
    public static PadLocation PadLocationG = new PadLocation(nameof(PadLocationG), "G");
    public PadLocation(String name, String value) : base(name, value) { }
}