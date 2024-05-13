using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get.Results;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Data.Models.Wafers.Enums;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get.Excel.Many;

public class GetManyInitialExcelEndpoint:Endpoint<GetManyInitialExcelResultsRequest,GetManyInitialExcelResultsResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetManyInitialExcelEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetManyInitialExcelResultsPath+"{waferIds}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetManyInitialExcelResultsRequest excelResultsRequest, CancellationToken cancellationToken) {
        var rows = await this._qtDataService.GetResults(excelResultsRequest.WaferIds,MeasurementType.Initial);
        await SendAsync(new GetManyInitialExcelResultsResponse(){Rows = rows}, cancellation: cancellationToken);
    }
    
}