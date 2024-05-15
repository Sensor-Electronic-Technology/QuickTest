using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Post;
using QuickTest.Data.Contracts.Responses.Post;
using QuickTest.Data.Mappers;
using QuickTest.Data.Models.Wafers;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.PostEndpoints;

public class CreateWaferPadEndpoint:Endpoint<CreateWaferPadRequest,CreateWaferPadResponse,WaferPadMapper>{
    private readonly WaferDataService _waferDataService;
    private readonly ILogger<CreateWaferPadEndpoint> _logger;
    
    public CreateWaferPadEndpoint(WaferDataService waferDataService,
        ILogger<CreateWaferPadEndpoint> logger) {
        this._waferDataService = waferDataService;
        this._logger = logger;
    }
    
    public override void Configure() {
        Post(QtApiPaths.CreateWaferPadPath+"{waferPad}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CreateWaferPadRequest request, CancellationToken cancellationToken) {
        WaferPad entity = Map.ToEntity(request);
        var exists = await this._waferDataService.Exists(identifier: entity.Identifier);
        if (exists) {
            this._logger.LogError("Attempted to create WaferPad with an Identifier that already exists");
            AddError("Identifier already exists");
        }
        ThrowIfAnyErrors();
        var result = await this._waferDataService.CreateWaferPad(entity);
        if (result.IsError) {
            this._logger.LogError("Failed to create WaferPad: {Description}", result.FirstError.Description);
            AddError(result.FirstError.Description);
        }
        await SendAsync(Map.FromEntity(result.Value), cancellation: cancellationToken);
    }
}