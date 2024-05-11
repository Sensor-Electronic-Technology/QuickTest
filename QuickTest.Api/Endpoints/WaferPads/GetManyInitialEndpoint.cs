using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.WaferPads;

public class GetManyInitialEndpoint:Endpoint<GetManyInitialRequest,GetManyInitialResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetManyInitialEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetManyInitialResults+"{waferIds}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetManyInitialRequest request, CancellationToken cancellationToken) {
        var rows = await this._qtDataService.GetInitialResults(request.WaferIds);
        var response=rows.Select(e => new GetInitialResponse() { Row = e }).ToList();
        await SendAsync(new GetManyInitialResponse(){Rows = response});
    }

}