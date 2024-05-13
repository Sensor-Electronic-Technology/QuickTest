using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get.Results;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get.Excel.Many;

public class GetManyQuickTestExcelEndpoint:Endpoint<GetManyQuickTestExcelRequest,GetManyQuickTestExcelResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetManyQuickTestExcelEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }

    public override void Configure() {
        Get(QtApiPaths.GetManyQuickTestExcelResultsPath+"{waferIds}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetManyQuickTestExcelRequest req, CancellationToken ct) {
        var response=await this._qtDataService.GetAllResults(req.WaferIds);
        await SendAsync(new GetManyQuickTestExcelResponse(){Rows = response}, cancellation: ct);
    }
}