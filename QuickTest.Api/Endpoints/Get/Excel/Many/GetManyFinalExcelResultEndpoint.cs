using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get.Results;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Data.Models.Wafers.Enums;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get.Excel.Many;

public class GetManyFinalExcelResultEndpoint:Endpoint<GetManyFinalExcelResultRequest,GetManyFinalExcelResultsResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetManyFinalExcelResultEndpoint(QuickTestDataService qtDataService) {

        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetManyFinalExcelResultsPath+"{waferIds}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetManyFinalExcelResultRequest request, CancellationToken cancellationToken) {
        var rows = await this._qtDataService.GetResults(request.WaferIds, MeasurementType.Final);
        await SendAsync(new GetManyFinalExcelResultsResponse() { Rows = rows }, cancellation: cancellationToken);
    }
}