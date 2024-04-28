using Ardalis.SmartEnum;

namespace QuickTest.Data.Models.Wafers.Enums;

public class WaferArea : SmartEnum<WaferArea, string> {
    public static WaferArea Center = new WaferArea(nameof(Center), "C");
    public static WaferArea Middle = new WaferArea(nameof(Middle), "M");
    public static WaferArea Edge = new WaferArea(nameof(Edge), "E");
    public WaferArea(String name, String value) : base(name, value) { }
}