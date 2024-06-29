using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Put;
using QuickTest.Data.Contracts.Responses.Put;
using QuickTest.Data.Models.Measurements;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.PutEndpoint;


public class InsertMeasurementEndpoint:Endpoint<InsertMeasurementRequest, InsertMeasurementResponse> {
    private readonly QuickTestDataService _qtDataService;

    public InsertMeasurementEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }

    public override void Configure() {
        Put(QtApiPaths.InsertMeasurementPath);
        AllowAnonymous();
    }

    public override async Task HandleAsync(InsertMeasurementRequest req, CancellationToken ct) {
        if(req.Measurement == null) {
            ThrowError("Measurement cannot be null");
        }
        if (string.IsNullOrEmpty(req.Measurement.WaferId)) {
            ThrowError("WaferId cannot be null or empty");
        }
        /*var result = await this._qtDataService.InsertMeasurement(new QtMeasurement() {
            MeasurementType = (MeasurementType)req.Measurement.MeasurementType,
            Current= req.Measurement.Current,
            Voltage= req.Measurement.Voltage,
            WaferId = req.Measurement.WaferId,
            Power= req.Measurement.Power,
            Knee=req.Measurement.Knee,
            Ir = req.Measurement.Ir,
            Wl= req.Measurement.Wl
        });*/
        var result = await this._qtDataService.InsertMeasurement(req.Measurement);
        await SendAsync(new InsertMeasurementResponse() { Success = result },cancellation:ct);
    }
}