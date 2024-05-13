using QuickTest.Data.DataTransfer;

namespace QuickTest.Data.Contracts.Requests.Push;

public class InsertMeasurementRequest {
    public MeasurementDto? Measurement { get; set; }
}