using EpiData.Data.Models.Epi.Enums;
using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get;

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
            var waferMap = await this._waferDataService.GetMap(waferSize);
            if (waferMap != null) {
                await SendAsync(new GetMapResponse(){WaferMap=waferMap.WaferMapDto()},cancellation: cancellationToken);
            }else {
                await SendNotFoundAsync(cancellationToken);
            }
        } else {
            AddError("Integer was out of range of WaferSize enum");
            ThrowIfAnyErrors();
        }
    }
}