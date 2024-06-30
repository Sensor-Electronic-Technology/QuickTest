using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Data.DataTransfer;
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
        var result = await this._qtDataService.GetAvailableBurnInPads(req.WaferId);
        if (result.testedPads.Any()) {
            var pads = await this._waferDataService.GetWaferPads(result.testedPads,result.waferSize);
                var results = pads.Select(e => new Pad() {
                    Identifier = e.Identifier, X = e.SvgObject!.X, Y = e.SvgObject!.Y, Radius = e.SvgObject!.Radius
                }).ToList();
                await SendAsync(new GetAvailableBurnInPadsResponse(){Pads=results}, cancellation: ct);
        }
        
        //return base.HandleAsync(req, ct);
    }
}