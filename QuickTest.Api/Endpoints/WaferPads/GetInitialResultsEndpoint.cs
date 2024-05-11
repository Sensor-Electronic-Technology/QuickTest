using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.WaferPads;

public class GetInitialResultsEndpoint:Endpoint<GetInitialRequest,GetInitialResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetInitialResultsEndpoint(QuickTestDataService qtDataService,
        ILogger<GetInitialResultsEndpoint> logger) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetInitialResultsPath+"{waferId}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetInitialRequest request, CancellationToken cancellationToken) {
        Console.WriteLine($"Recieved: {request}");
        var initial = await this._qtDataService.GetInitialResultsV2(request.WaferId);
        
        await SendAsync(new GetInitialResponse(){Row=initial}, cancellation: cancellationToken);
    }
    
}