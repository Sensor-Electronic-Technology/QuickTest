using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get.Results;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Data.Models.Wafers.Enums;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get.Excel.Single;

public class GetFinalExcelResultsEndpoint:Endpoint<GetFinalExcelResultsRequest,GetFinalExcelResultsResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetFinalExcelResultsEndpoint(QuickTestDataService qtDataService) {

        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetFinalExcelResultsPath+"{waferId}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetFinalExcelResultsRequest request, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(request.WaferId)) {
            ThrowError("WaferId cannot be null or empty");
        }
        var rows = await this._qtDataService.GetResult(request.WaferId, MeasurementType.Final);
        await SendAsync(new GetFinalExcelResultsResponse() { Row = rows }, cancellation: cancellationToken);
    }
}