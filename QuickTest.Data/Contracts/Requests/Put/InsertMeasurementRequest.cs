using QuickTest.Data.DataTransfer;

namespace QuickTest.Data.Contracts.Requests.Put;

public class InsertMeasurementRequest {
    public MeasurementDto? Measurement { get; set; }
}