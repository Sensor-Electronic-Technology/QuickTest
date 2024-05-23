using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get;

public class GetQuickTestListEndpoint:Endpoint<GetQuickTestsListRequest,GetQuickTestListResponse> {
    private readonly QuickTestDataService _qtDataService;

    public GetQuickTestListEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }

    public override void Configure() {
        Get(QtApiPaths.GetQuickTestListSincePath+"{StartDate}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetQuickTestsListRequest req, CancellationToken ct) {
        var waferList = await this._qtDataService.GetQuickTestList(req.StartDate);
        await SendAsync(new GetQuickTestListResponse() { WaferList = waferList },cancellation:ct);
    }
}
