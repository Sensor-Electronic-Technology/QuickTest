using FastEndpoints;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using QuickTest.Data.Contracts.Responses.Get;
using QuickTest.Data.Contracts.Responses.Get.Excel;
using QuickTest.Data.Models.Measurements;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Endpoints.Get;

public class GetLvQuickTestResultEndpoint:Endpoint<GetResultRequest,GetLvQuickTestResultResponse> {
    private readonly QuickTestDataService _qtDataService;
    
    public GetLvQuickTestResultEndpoint(QuickTestDataService qtDataService) {
        this._qtDataService = qtDataService;
    }
    
    public override void Configure() {
        Get(QtApiPaths.GetLabviewResultsPath+"{waferId}");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetResultRequest request, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(request.WaferId)) {
            ThrowError("WaferId cannot be null or empty");
        }
        var result = await this._qtDataService.GetLabViewResult(request.WaferId,(MeasurementType)request.MeasurementType);
        await SendAsync(new GetLvQuickTestResultResponse() {
            Measurements = result.measurments,
            WaferSize = result.size==0 ? 2: result.size
        },cancellation:cancellationToken);

    }
}