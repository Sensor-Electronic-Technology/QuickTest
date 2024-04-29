using FastEndpoints;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.WaferPads;

public class GetInitialResultsEndpoint:Endpoint<string,string> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetInitialResultsEndpoint(QuickTestDataService qtDataService,
        ILogger<GetInitialResultsEndpoint> logger) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get("/api/results/initial/result");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(string request, CancellationToken cancellationToken) {
        Console.WriteLine($"Recieved: {request}");
        var result = await this._qtDataService.GetInitialResults(request);
        await SendAsync(result, cancellation: cancellationToken);
    }
    
}