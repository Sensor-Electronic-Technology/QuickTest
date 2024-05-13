using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Requests.Get.Results;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.WaferPads.Get.Excel;

public class GetInitialExcelResultsEndpoint:Endpoint<GetInitialRequest,GetInitialExcelResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetInitialExcelResultsEndpoint(QuickTestDataService qtDataService,
        ILogger<GetInitialExcelResultsEndpoint> logger) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetInitialExcelResultsPath+"{waferId}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetInitialRequest request, CancellationToken cancellationToken) {
        Console.WriteLine($"Recieved: {request}");
        var initial = await this._qtDataService.GetInitialResultsV2(request.WaferId);
        
        await SendAsync(new GetInitialExcelResponse(){Row=initial}, cancellation: cancellationToken);
    }
    
}