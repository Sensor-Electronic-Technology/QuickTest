using System.Text;
using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Put;
using QuickTest.Data.Contracts.Responses.Put;
using QuickTest.Data.Events;
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
        StringBuilder builder = new StringBuilder();
        bool error = false;
        if(string.IsNullOrWhiteSpace(req.WaferId)) {
            error = true;
            builder.AppendLine("WaferId cannot be null or empty");
        }
        if (string.IsNullOrEmpty(req.PadLocation)) {
            error = true;
            builder.AppendLine("PadLocation cannot be null or empty");
        }
        if (string.IsNullOrWhiteSpace(req.ActualPad)) {
            error = true;
            builder.AppendLine("ActualPad cannot be null or empty");
        }
        
        if(req.Measurements.Count==0 || req.SpectrumMeasurements.Count==0) {
            error = true;
            builder.AppendLine("Measurements or SpectrumMeasurements must have at least one item.");
        }
        
        if (error) {
            await SendAsync(new InsertMeasurementResponse() {
                Success = false,
                Errors=builder.ToString()
            },cancellation:ct);
        } else {
            var result = await this._qtDataService.InsertAllMeasurements(req);
            if (result.IsError) {
                await SendAsync(new InsertMeasurementResponse() {
                    Success = false,
                    Errors =result.FirstError.Description
                },cancellation:ct);
            } else {
                await SendAsync(new InsertMeasurementResponse() {
                    Success = true,
                    Errors = ""
                },cancellation:ct);
                await PublishAsync(
                    new MeasurementInsertedEvent() {
                        SpectrumMeasurements = req.SpectrumMeasurements,
                        WaferId = req.WaferId,
                        MeasurementType = req.MeasurementType,
                        ActualPad = req.ActualPad,
                        PadLocation = req.PadLocation
                    },Mode.WaitForAll, cancellation: ct);
            }
        }
    }
}