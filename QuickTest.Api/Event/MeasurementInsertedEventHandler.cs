

using FastEndpoints;
using QuickTest.Data.Events;
using QuickTest.Data.Models.Measurements;
using QuickTest.Infrastructure.Services;
using ILogger = Amazon.Runtime.Internal.Util.ILogger;

namespace QuickTest.Api.Event;

public class MeasurementInsertedEventHandler:IEventHandler<MeasurementInsertedEvent> {
    private readonly QuickTestDataService _qtDataService;
    private readonly ILogger<MeasurementInsertedEventHandler> _logger;
    public MeasurementInsertedEventHandler(QuickTestDataService qtDataService,ILogger<MeasurementInsertedEventHandler> logger) {
        this._qtDataService = qtDataService;
        this._logger = logger;
    }
    public async Task HandleAsync(MeasurementInsertedEvent eventModel, CancellationToken ct) {
        foreach (var measurement in eventModel.SpectrumMeasurements) {
            /*await this._qtDataService.InsertSpectrumMeasurement(measurement,(MeasurementType)eventModel.MeasurementType,
                eventModel.WaferId, eventModel.PadLocation, eventModel.ActualPad,measurement.Current);*/
            this._logger.LogInformation("Inserted Spectrum Measurement for {WaferId}",eventModel.WaferId);
        }
    }
}