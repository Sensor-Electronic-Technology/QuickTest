using FastEndpoints;
using QuickTest.Data.Contracts.Requests.Push;
using QuickTest.Data.Contracts.Responses.Push;
using QuickTest.Data.DataTransfer;
using QuickTest.Data.Models.Measurements;
using QuickTest.Data.Models.Wafers.Enums;

namespace QuickTest.Data.Mappers;

public class MeasurementMapper:Mapper<InsertMeasurementRequest,InsertMeasurementResponse,QtMeasurement?> {
    public override InsertMeasurementResponse FromEntity(QtMeasurement? e) {
        if (e == null) {
            return new InsertMeasurementResponse() {
                Success = false
            };
        }
        return new InsertMeasurementResponse() {
            Success = true
        };
    }
    
    public override QtMeasurement? ToEntity(InsertMeasurementRequest r) {
        if (r.Measurement == null) {
            return null;
        }
        var measurement= new QtMeasurement() {
            Pad = r.Measurement.Pad,
            Current = r.Measurement.Current,
            Wl = r.Measurement.Wl,
            Power = r.Measurement.Power,
            Voltage = r.Measurement.Voltage,
            Knee = r.Measurement.Knee,
            Ir = r.Measurement.Ir
        };
        if (string.IsNullOrEmpty(r.Measurement.WaferId)) {
            return null;
        }
        measurement.WaferId = r.Measurement.WaferId;
        measurement.MeasurementType = (MeasurementType)r.Measurement.MeasurementType;
        return measurement;
        /*if (MeasurementType.TryFromValue(r.Measurement.MeasurementType,out var type)) {
            measurement.MeasurementType = type;
            return measurement;
        } else {
            return null;
        }*/
    }

    public override QtMeasurement? UpdateEntity(InsertMeasurementRequest r, QtMeasurement? e) {
        if (e == null) {
            return null;
        }
        if (r.Measurement == null) {
            return null;
        }
        e.Pad = r.Measurement.Pad;
        e.Current = r.Measurement.Current;
        e.Wl = r.Measurement.Wl;
        e.Power = r.Measurement.Power;
        e.Voltage = r.Measurement.Voltage;
        e.Knee = r.Measurement.Knee;
        e.Ir = r.Measurement.Ir;
        if (string.IsNullOrEmpty(r.Measurement.WaferId)) {
            return null;
        }
        e.WaferId = r.Measurement.WaferId;
        e.MeasurementType= (MeasurementType)r.Measurement.MeasurementType;
        return e;
        /*if (MeasurementType.TryFromValue(r.Measurement.MeasurementType,out var type)) {
            e.MeasurementType = type;
            return e;
        } else {
            return null;
        }*/
    }
}