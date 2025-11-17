using FastEndpoints;
using QuickTest.Data.Contracts.Requests.Put;
using QuickTest.Data.Contracts.Responses.Put;
using QuickTest.Data.Models.Measurements;

namespace QuickTest.Api.Processors;

/*public class MarkCompletedPostProcessor:IPostProcessor<MarkTestedRequest,MarkTestedResponse> {
    private readonly ILogger<MarkCompletedPostProcessor> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    
    public MarkCompletedPostProcessor(ILogger<MarkCompletedPostProcessor> logger, IHttpClientFactory httpClientFactory) {
        this._logger = logger;
        this._httpClientFactory = httpClientFactory;
    }
    
    public async Task PostProcessAsync(IPostProcessorContext<MarkTestedRequest, MarkTestedResponse> context, CancellationToken ct) {
        var client=this._httpClientFactory.CreateClient("QuickTestApi");
        client.BaseAddress=new Uri("http://172.20.4.208:34000/");
        
        if ((MeasurementType)context.Request.MeasurementType == MeasurementType.Initial) {
            if (context.Request.Tested) {
                var response = await client.GetAsync($"api/process/quick-test/initial/{context.Request.WaferId}",ct);
            
                if (response.IsSuccessStatusCode) {
                    this._logger.LogInformation("Sent Initial Power Measurement process complete for WaferId:{WaferId}",context.Request.WaferId);
                } else {
                    this._logger.LogWarning("Failed to send Initial Power Measurement process complete for WaferId:{WaferId}",context.Request.WaferId);
                }
            }
        } else {
            if (context.Request.Tested) {
                var response = await client.GetAsync($"api/process/quick-test/final/{context.Request.WaferId}",ct);
                if (response.IsSuccessStatusCode) {
                    this._logger.LogInformation("Sent Final Power Measurement process complete for WaferId:{WaferId}",context.Request.WaferId);
                } else {
                    this._logger.LogWarning("Failed to send Final Power Measurement process complete for WaferId:{WaferId}",context.Request.WaferId);
                }
            }
        }
    }
}*/