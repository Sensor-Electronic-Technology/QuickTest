using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Requests.Get.Results;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.WaferPads.Get.Excel;

public class GetManyInitialExcelEndpoint:Endpoint<GetManyInitialRequest,GetManyInitialExcelResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetManyInitialExcelEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetManyInitialExcelResults+"{waferIds}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetManyInitialRequest request, CancellationToken cancellationToken) {
        var rows = await this._qtDataService.GetInitialResults(request.WaferIds);
        await SendAsync(new GetManyInitialExcelResponse(){Rows = rows});
    }

}