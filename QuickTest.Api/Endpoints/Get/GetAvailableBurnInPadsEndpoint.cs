using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Data.Models.Wafers;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get;

public class GetAvailableBurnInPadsEndpoint:Endpoint<GetAvailableBurnInPadsRequest,GetAvailableBurnInPadsResponse> {
    private readonly WaferDataService _waferDataService;
    private readonly QuickTestDataService _qtDataService;
    public GetAvailableBurnInPadsEndpoint(WaferDataService waferDataService, QuickTestDataService qtDataService) {
        this._waferDataService = waferDataService;
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetAvailableBurnInPadsPath+"{waferId}");
        AllowAnonymous();
    }
    public override async Task HandleAsync(GetAvailableBurnInPadsRequest req, CancellationToken ct) {
        if (string.IsNullOrEmpty(req.WaferId)) {
            ThrowError("WaferId cannot be null or empty");
        }

        var testedPads = await this._qtDataService.GetAvailableBurnInPads(req.WaferId);
        if (testedPads.Any()) {
            List<WaferPad> pads = await this._waferDataService.GetWaferPads(testedPads);
                var results = pads.Select(e => new WaferPadDto() {
                    Identifier = e.Identifier, WaferSize = e.WaferSize, SvgObject = e.SvgObject
                }).ToList();
                await SendAsync(new GetAvailableBurnInPadsResponse(){Pads=results}, cancellation: ct);
        }
        
        //return base.HandleAsync(req, ct);
    }
}