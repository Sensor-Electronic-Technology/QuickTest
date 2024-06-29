using FastEndpoints;
using QuickTest.Data.Contracts.Requests.Put;
using QuickTest.Data.Contracts.Responses.Put;
using QuickTest.Data.Models.Measurements;
using QuickTest.Infrastructure.Services;

namespace QuickTest.Api.Processors;

/*public class InsertMeasurementPostProcessor:IPostProcessor<InsertMeasurementRequest, InsertMeasurementResponse>{
    public async Task PostProcessAsync(IPostProcessorContext<InsertMeasurementRequest, InsertMeasurementResponse> context, 
        CancellationToken ct) {
        var dataService = context.HttpContext.Resolve<QuickTestDataService>();
        if(context.Request.Measurement != null && context.Response!=null && context.Response.Success) {
            await dataService.PostInsertMeasurement(new QtMeasurement() {
                MeasurementType = (MeasurementType)context.Request.Measurement.MeasurementType,
                Current = context.Request.Measurement.Current,
                Voltage = context.Request.Measurement.Voltage,
                WaferId = context.Request.Measurement.WaferId!,
                Power = context.Request.Measurement.Power,
                Knee = context.Request.Measurement.Knee,
                Ir = context.Request.Measurement.Ir,
                Wl = context.Request.Measurement.Wl
            });
        }
    }
}*/