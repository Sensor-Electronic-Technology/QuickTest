using EpiData.Data.Models.Epi.Enums;
using FastEndpoints;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Models.Wafers;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get;

public class GetWaferPadsEndpoint : Endpoint<GetWaferPadsRequest, IEnumerable<WaferPad>> {
    private readonly WaferDataService _waferDataService;
    private readonly ILogger<GetWaferPadsEndpoint> _logger;
    
    public GetWaferPadsEndpoint(WaferDataService waferDataService,ILogger<GetWaferPadsEndpoint> logger) {
        this._waferDataService = waferDataService;
        this._logger = logger;
    }
    
    public override void Configure() {
        Get("/api/pads/many/{waferSize:int}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetWaferPadsRequest request, CancellationToken cancellationToken) {
        if (WaferSize.TryFromValue(request.WaferSize, out var waferSize)) {
            var pads = await this._waferDataService.GetWaferPads(waferSize);
            if (pads != null) {
                await SendAsync(pads,cancellation: cancellationToken);
            }else {
                await SendNotFoundAsync(cancellationToken);
            }
        } else {
            AddError("Integer was out of range of WaferSize enum");
            ThrowIfAnyErrors();
        }
    }
}
