using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints;

public class DeleteQuickTestEndpoint:Endpoint<DeleteQuickTestRequest,DeleteQuickTestResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public DeleteQuickTestEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }

    public override void Configure() {
        Delete(QtApiPaths.DeleteQuickTestPath+"{waferId}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(DeleteQuickTestRequest req, CancellationToken ct) {
        if (string.IsNullOrEmpty(req.WaferId)) {
            await SendAsync(new DeleteQuickTestResponse() { Success = false,Message = "Error: WaferId was null or empty"},cancellation:ct);
            return;
        }

        await this._qtDataService.DeleteQuickTest(req.WaferId);
        await SendAsync(new DeleteQuickTestResponse() { Success = true,Message = "QuickTest deleted successfully"},cancellation:ct);

    }
}