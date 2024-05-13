using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.WaferPads;

public class GetAvailableBurnInPadsEndpoint:Endpoint<GetAvailableBurnInPadsRequest,GetAvailableBurnInPadsResponse> {
    private readonly WaferDataService _waferDataService;
    public GetAvailableBurnInPadsEndpoint(WaferDataService waferDataService) {
        this._waferDataService = waferDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetAvailableBurnInPadsPath+"{waferId}");
        AllowAnonymous();
    }
    public override Task HandleAsync(GetAvailableBurnInPadsRequest req, CancellationToken ct) {
        //var pads = _waferDataService.GetAvailableBurnInPads();
        return base.HandleAsync(req, ct);
    }
}