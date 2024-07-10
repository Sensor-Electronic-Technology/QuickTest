using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Post;
using QuickTest.Data.Contracts.Responses.Post;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.PostEndpoints;

public class CreateQuickTestEndpoint:Endpoint<CreateQuickTestRequest, CreateQuickTestResponse> {
    private readonly QuickTestDataService _qtDataService;

    public CreateQuickTestEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }

    public override void Configure() {
        Post(QtApiPaths.CreateQuickTestPath+"{createRequest}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateQuickTestRequest req, CancellationToken ct) {
        if (string.IsNullOrEmpty(req.WaferId)) {
            ThrowError("WaferId cannot be null or empty");
        }

        var result = await this._qtDataService.CreateQuickTest(req.WaferId,req.ProbeStationId,req.WaferSize);
        if (result.IsError) {
            await SendAsync(new CreateQuickTestResponse() {
                Success = false,
                Message = result.FirstError.Description
            }, cancellation: ct);
        } else {
            await SendAsync(new CreateQuickTestResponse() { Success = result.Value }, cancellation: ct);
        }
    }
}