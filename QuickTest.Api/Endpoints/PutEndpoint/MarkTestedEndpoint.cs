using System.Text.Json;
using FastEndpoints;
using QuickTest.Api.Processors;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Events;
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
        //PostProcessor<MarkCompletedPostProcessor>();
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
            Console.WriteLine(JsonSerializer.Serialize(request,new JsonSerializerOptions(){WriteIndented = true}));
            var result=await this._qtDataService.MarkTestedV2(request.WaferId,request.Tested,(MeasurementType)request.MeasurementType);
            if(result.IsError) {
                await SendAsync(new MarkTestedResponse() {
                    Success = false,
                    Errors = result.FirstError.Description
                },cancellation:cancellationToken);
            }
            await this.PublishAsync(result.Value,Mode.WaitForNone,cancellationToken);
            
            await SendAsync(new MarkTestedResponse(){Success = true,Errors = ""},cancellation:cancellationToken);
        }
    }
}