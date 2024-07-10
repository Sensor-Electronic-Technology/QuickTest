using QuickTest.Data.DataTransfer;

namespace QuickTest.Data.Contracts.Responses.Get;

public class GetLvQuickTestResultResponse {
    public int WaferSize { get; set; }
    public Dictionary<string,PadMeasurementDto> Measurements { get; set; } = new Dictionary<string, PadMeasurementDto>();
}