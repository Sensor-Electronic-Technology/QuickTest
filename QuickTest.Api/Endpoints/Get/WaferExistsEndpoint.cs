using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get;

public class WaferExistsEndpoint:Endpoint<QtWaferExistsRequest,QtWaferExistsResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public WaferExistsEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }

    public override void Configure() {
        Get(QtApiPaths.GetQuickTestExistsPath+"{waferId}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(QtWaferExistsRequest req, CancellationToken ct) {
        if (string.IsNullOrEmpty(req.WaferId)) {
            ThrowError("WaferId cannot be null or empty");
        }
        var exists = await this._qtDataService.QtWaferExists(req.WaferId);
        await SendAsync(new QtWaferExistsResponse() { Exists = exists },cancellation:ct);
    }
}