using System.Text;
using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Put;
using QuickTest.Data.Contracts.Responses.Put;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.PutEndpoint;

/*public class InsertSpectrumMeasurementEndpoint:Endpoint<InsertSpectrumMeasurementRequest, InsertSpectrumMeasurementResponse> {
    private readonly QuickTestDataService _qtDataService;

    public InsertSpectrumMeasurementEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }

    public override void Configure() {
        Put(QtApiPaths.InsertSpectrumMeasurementPath);
        AllowAnonymous();
    }

    public override async Task HandleAsync(InsertSpectrumMeasurementRequest req, CancellationToken ct) {
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
        if (error) {
            await SendAsync(new InsertSpectrumMeasurementResponse() {
                Success = false,
                Errors=builder.ToString()
            },cancellation:ct);
        } else {
            var result = await this._qtDataService.InsertSpectrumMeasurement(req);
            if (result.IsError) {
                await SendAsync(new InsertSpectrumMeasurementResponse() {
                    Success = false,
                    Errors =result.FirstError.Description
                },cancellation:ct);
            } else {
                await SendAsync(new InsertSpectrumMeasurementResponse() {
                    Success = true,
                    Errors = ""
                },cancellation:ct);
            }
        }
    }
}*/