using EpiData.Data.Models.Epi.Enums;
using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get;

public class GetLabviewWaferMapEndpoint:Endpoint<GetLvWaferMapRequest, GetLabviewWaferMapResponse> {
    private readonly WaferDataService _waferDataService;
    
    public GetLabviewWaferMapEndpoint(WaferDataService waferDataService){
        this._waferDataService=waferDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetLabviewWaferMap+"{waferSize:int}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetLvWaferMapRequest request, CancellationToken cancellationToken) {
        if (WaferSize.TryFromValue(request.WaferSize, out var waferSize)) {
            var waferMap = await this._waferDataService.GetLabviewWaferMap(waferSize);
            if (waferMap != null) {
                await SendAsync(new GetLabviewWaferMapResponse(){WaferMap=waferMap},cancellation: cancellationToken);
            }else {
                await SendNotFoundAsync(cancellationToken);
            }
        } else {
            AddError("Integer was out of range of WaferSize enum");
            ThrowIfAnyErrors();
        }
    }
}