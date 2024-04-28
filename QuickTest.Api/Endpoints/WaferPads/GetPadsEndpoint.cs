using FastEndpoints;
using QuickTest.Data.Contracts.Requests;
using QuickTest.Data.Models.Wafers;
using QuickTest.Data.Models.Wafers.Enums;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.WaferPads;

public class GetPadsEndpoint : Endpoint<GetWaferPadsRequest, IEnumerable<WaferPad>> {
    private readonly WaferDataService _waferDataService;
    private readonly ILogger<GetPadsEndpoint> _logger;
    
    public GetPadsEndpoint(WaferDataService waferDataService,ILogger<GetPadsEndpoint> logger) {
        this._waferDataService = waferDataService;
        this._logger = logger;
    }
    
    public override void Configure() {
        Get("/api/wafer_pads/{waferSize:int}");
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
