using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Put;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.PutEndpoint;

public class MarkTestedEndpoint:Endpoint<MarkTestedRequest,EmptyResponse>  {
    private readonly QuickTestDataService _qtDataService;
    
    public MarkTestedEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Put(QtApiPaths.MarkTestedPath);
        AllowAnonymous();
    }

    public override async Task HandleAsync(MarkTestedRequest request, CancellationToken cancellationToken) {
        
    }
}