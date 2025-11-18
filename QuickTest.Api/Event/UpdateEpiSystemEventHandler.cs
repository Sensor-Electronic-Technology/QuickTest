using FastEndpoints;
using QuickTest.Data.Contracts.Events;
using QuickTest.Data.Models.Measurements;

namespace QuickTest.Api.Event;

public class MarkExternalProcessCompleteRequest {
    public required string WaferId { get; set; }
    public required string ExternalReferenceId { get; set; }
}

public class UpdateEpiSystemEventHandler:IEventHandler<UpdateEpiSystemEvent> {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UpdateEpiSystemEventHandler> _logger;
    
    public UpdateEpiSystemEventHandler(IHttpClientFactory httpClientFactory,ILogger<UpdateEpiSystemEventHandler> logger) {
        this._httpClientFactory=httpClientFactory;
        this._logger = logger;
    }
    
    public async Task HandleAsync(UpdateEpiSystemEvent eventModel, CancellationToken ct) {
        var client=this._httpClientFactory.CreateClient("QuickTestApi");
        client.BaseAddress=new Uri("http://localhost:34000/");
        var req = new MarkExternalProcessCompleteRequest() {
            WaferId = eventModel.WaferId,
            ExternalReferenceId = eventModel.ReferenceId
        };
        /*var response = await client.GetAsync($"api/process/quick-test/" +
                                             $"{(eventModel.MeasurementType == MeasurementType.Initial ? "initial" : "final")}" +
                                             $"/{eventModel.WaferId}?externalReferenceId={eventModel.ReferenceId}",ct);*/
        var response=await client.PutAsJsonAsync($"api/process/quick-test/" +
                                                 $"{(eventModel.MeasurementType == MeasurementType.Initial ? "initial" : "final")}/",req,ct);
        if (response.IsSuccessStatusCode) {
            this._logger.LogInformation("Sent ReferenceId for WaferId:{WaferId} RefId: {RefId}",
                eventModel.WaferId,eventModel.ReferenceId);
        } else {
            this._logger.LogWarning("Failed to send ReferenceId for WaferId:{WaferId} RefId: {RefId}",
                eventModel.WaferId,eventModel.ReferenceId);
        }
    }
}