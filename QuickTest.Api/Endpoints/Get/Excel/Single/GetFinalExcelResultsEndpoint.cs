using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Data.Models.Wafers.Enums;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get.Excel.Single;

public class GetFinalExcelResultsEndpoint:Endpoint<GetResultRequest,GetResultExcelResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetFinalExcelResultsEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetFinalExcelResultsPath+"{waferId}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetResultRequest request, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(request.WaferId)) {
            ThrowError("WaferId cannot be null or empty");
        }
        var rows = await this._qtDataService.GetResult(request.WaferId, MeasurementType.Final);
        await SendAsync(new GetResultExcelResponse() { Row = rows }, cancellation: cancellationToken);
    }
}