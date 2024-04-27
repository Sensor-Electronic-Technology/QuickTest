using Ardalis.SmartEnum;

namespace QuickTest.Data.Wafer;



public class WaferSize : SmartEnum<WaferSize, int> {
    public static WaferSize TwoInch = new WaferSize("2Inch",2);
    public static WaferSize FourInch = new WaferSize("4Inch",4);
    public static WaferSize SixInch = new WaferSize("6Inch",6);
    public static WaferSize EightInch = new WaferSize("8Inch",8);
    public static WaferSize TenInch = new WaferSize("10Inch",10);
    public static WaferSize TwelveInch = new WaferSize("12Inch",12);

    public WaferSize(String name, int value) : base(name, value) { }
}

