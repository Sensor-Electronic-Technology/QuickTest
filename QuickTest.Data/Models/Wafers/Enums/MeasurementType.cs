using Ardalis.SmartEnum;
namespace QuickTest.Data.Models.Wafers.Enums;


/*public enum MeasurementType:int {
    Final=1,
    Initial=0
}*/


public class MeasurementType : SmartEnum<MeasurementType, int> {
    public static MeasurementType Initial  = new MeasurementType(nameof(Initial), 0);
    public static MeasurementType Final  = new MeasurementType(nameof(Final), 0);
    public MeasurementType(String name, int value) : base(name, value) { }
}
