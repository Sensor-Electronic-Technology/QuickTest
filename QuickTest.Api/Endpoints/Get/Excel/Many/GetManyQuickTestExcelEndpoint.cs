using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get.Excel.Many;

public class GetManyQuickTestExcelEndpoint:Endpoint<GetManyResultsRequest,GetManyResultsExcelResponse>  {
    private readonly QuickTestDataService _qtDataService;
    
    public GetManyQuickTestExcelEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }

    public override void Configure() {
        Get(QtApiPaths.GetManyQuickTestExcelResultsPath+"{waferIds}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetManyResultsRequest request, CancellationToken ct) {
        if (request.WaferIds == null) {
            ThrowError("Wafer list cannot be null");
        }

        if (!request.WaferIds.Any()) {
            ThrowError("Wafer list cannot be empty");
        }
        var response=await this._qtDataService.GetAllResults(request.WaferIds);
        await SendAsync(new GetManyResultsExcelResponse(){Rows = response}, cancellation: ct);
    }
}