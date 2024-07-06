using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Put;
using QuickTest.Data.Contracts.Responses.Put;
using QuickTest.Data.Models.Measurements;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.PutEndpoint;

public class MarkTestedEndpoint:Endpoint<MarkTestedRequest,MarkTestedResponse>  {
    private readonly QuickTestDataService _qtDataService;
    
    public MarkTestedEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Put(QtApiPaths.MarkTestedPath);
        AllowAnonymous();
    }

    public override async Task HandleAsync(MarkTestedRequest request, CancellationToken cancellationToken) {
        string errors = "";
        bool error = false;
        if (string.IsNullOrEmpty(request.WaferId)) {
            errors="WaferId cannot be null or empty";
            error = true;
        }

        if (error) {
            await SendAsync(new MarkTestedResponse() {
                Success = false,
                Errors = errors
            },cancellation:cancellationToken);
        } else {
            await this._qtDataService.MarkTested(request.WaferId,request.Tested,(MeasurementType)request.MeasurementType);
            await SendAsync(new MarkTestedResponse(){Success = true,Errors = ""},cancellation:cancellationToken);
        }
        
    }
}