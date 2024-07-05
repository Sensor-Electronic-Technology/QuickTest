using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Data.Models.Measurements;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get;

public class CheckQuickTestEndpoint:Endpoint<CheckQuickTestRequest, CheckQuickTestResponse> {
    private readonly QuickTestDataService _qtDataService;

    public CheckQuickTestEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }

    public override void Configure() {
        Get(QtApiPaths.CheckQuickTestPath+"{waferId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CheckQuickTestRequest req, CancellationToken ct) {
        if (string.IsNullOrEmpty(req.WaferId)) {
            ThrowError("WaferId cannot be null or empty");
        }
        Console.WriteLine(req.WaferId);
        var result = await this._qtDataService.CheckQuickTest(req.WaferId, (MeasurementType)req.MeasurementType);
        await SendAsync(new CheckQuickTestResponse() {
            Exists = result.Exisits, 
            StationId = result.StationId, 
            Tested = result.Tested
        }, cancellation: ct);
    }
}