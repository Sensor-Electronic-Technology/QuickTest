using EpiData.Data.Models.Epi.Enums;
using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.WaferPads;

public class GetMapEndpoint:Endpoint<GetMapRequest, GetMapResponse>{
    private readonly WaferDataService _waferDataService;
    
    public GetMapEndpoint(WaferDataService waferDataService){
        this._waferDataService=waferDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetMapPath+"{waferSize:int}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetMapRequest request, CancellationToken cancellationToken) {
        if (WaferSize.TryFromValue(request.WaferSize, out var waferSize)) {
            var pads = await this._waferDataService.GetMap(waferSize);
            if (pads != null) {
                await SendAsync(new GetMapResponse(){Pads=pads},cancellation: cancellationToken);
            }else {
                await SendNotFoundAsync(cancellationToken);
            }
        } else {
            AddError("Integer was out of range of WaferSize enum");
            ThrowIfAnyErrors();
        }
    }
}