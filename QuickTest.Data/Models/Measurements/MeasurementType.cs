using Ardalis.SmartEnum;

namespace QuickTest.Data.Models.Measurements;


public enum MeasurementType:int {
    Final=1,
    Initial=0
}


public class MeasurementTypeSmart : SmartEnum<MeasurementTypeSmart, int> {
    public static MeasurementTypeSmart Initial  = new MeasurementTypeSmart(nameof(Initial), 0);
    public static MeasurementTypeSmart Final  = new MeasurementTypeSmart(nameof(Final), 0);
    public MeasurementTypeSmart(String name, int value) : base(name, value) { }
}
