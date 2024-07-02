using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get.Excel.Single;

public class GetQuickTestExcelResultsEndpoint:Endpoint<GetResultRequest,GetResultExcelResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetQuickTestExcelResultsEndpoint(QuickTestDataService qtDataService) {

        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetQuickTestExcelResultsPath+"{waferId}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetResultRequest resultsRequest, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(resultsRequest.WaferId)) {
            ThrowError("WaferId cannot be null");
        }
        ThrowIfAnyErrors();
        var initial = await this._qtDataService.GetResultAll(resultsRequest.WaferId);
        await SendAsync(new GetResultExcelResponse(){Row=initial}, cancellation: cancellationToken);
    }
}