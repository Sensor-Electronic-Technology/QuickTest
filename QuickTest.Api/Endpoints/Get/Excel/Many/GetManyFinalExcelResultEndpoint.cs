using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Data.Models.Wafers.Enums;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get.Excel.Many;

public class GetManyFinalExcelResultEndpoint:Endpoint<GetManyResultsRequest,GetManyResultsExcelResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetManyFinalExcelResultEndpoint(QuickTestDataService qtDataService) {

        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetManyFinalExcelResultsPath+"{waferIds}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetManyResultsRequest request, CancellationToken cancellationToken) {
        if (request.WaferIds == null) {
            ThrowError("Wafer list cannot be null");
        }

        if (!request.WaferIds.Any()) {
            ThrowError("Wafer list cannot be empty");
        }
        var rows = await this._qtDataService.GetResults(request.WaferIds, MeasurementType.Final);
        await SendAsync(new GetManyResultsExcelResponse() { Rows = rows }, cancellation: cancellationToken);
    }
}