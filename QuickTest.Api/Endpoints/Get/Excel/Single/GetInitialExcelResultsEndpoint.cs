using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get.Results;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Data.Models.Wafers.Enums;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get.Excel.Single;

public class GetInitialExcelResultsEndpoint:Endpoint<GetInitialExcelResultsRequest,GetInitialExcelResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetInitialExcelResultsEndpoint(QuickTestDataService qtDataService,
        ILogger<GetInitialExcelResultsEndpoint> logger) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetInitialExcelResultsPath+"{waferId}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetInitialExcelResultsRequest excelResultsRequest, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(excelResultsRequest.WaferId)) {
            ThrowError("WaferId cannot be null");
        }
        ThrowIfAnyErrors();
        
        var initial = await this._qtDataService.GetResult(excelResultsRequest.WaferId,MeasurementType.Initial);
        await SendAsync(new GetInitialExcelResponse(){Row=initial}, cancellation: cancellationToken);
    }
    
}