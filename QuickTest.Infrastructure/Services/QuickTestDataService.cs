using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuickTest.Data.AppSettings;
using QuickTest.Data.Models.Measurements;
using ErrorOr;
using QuickTest.Data.Models.Wafers.Enums;

namespace QuickTest.Infrastructure.Services;

public class QuickTestDataService {
    private readonly IMongoCollection<QuickTestResult> _qtCollection;
    private readonly IMongoCollection<Measurement> _initMeasureCollection;
    private readonly IMongoCollection<Measurement> _finalMeasureCollection;
    private readonly IMongoCollection<Spectrum> _initSpectrumCollection;
    private readonly IMongoCollection<Spectrum> _finalSpectrumCollection;
    private ILogger<QuickTestDataService> _logger;

    public QuickTestDataService(ILogger<QuickTestDataService> logger, IMongoClient mongoClient,IOptions<DatabaseSettings> options) {
        this._logger = logger;
        var database = mongoClient.GetDatabase(options.Value.DatabaseName);
        this._qtCollection = database.GetCollection<QuickTestResult>("quick_test");
        this._initMeasureCollection=database.GetCollection<Measurement>("init_measurements");
        this._finalMeasureCollection=database.GetCollection<Measurement>("final_measurements");
        this._initSpectrumCollection=database.GetCollection<Spectrum>("init_spectrum");
        this._finalSpectrumCollection=database.GetCollection<Spectrum>("final_spectrum");
    }
    
    public QuickTestDataService(IMongoClient mongoClient,string dbName) {
        var database = mongoClient.GetDatabase(dbName);
        this._qtCollection = database.GetCollection<QuickTestResult>("quick_test");
    }
    
    public Task<bool> WaferExists(string waferId) {
        return this._qtCollection.Find(e=>e.WaferId==waferId).AnyAsync();
    }

    public async Task<string> GetInitialResults(string waferId) {
        var qt = await this._qtCollection.Find(e => e.WaferId == waferId)
            .FirstOrDefaultAsync();
        if(qt==null) {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat($"TimeStamp");
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                builder.AppendFormat($"\t{pad}_Voltage\t{pad}_Power\t{pad}_Wl\t{pad}_Ir\t{pad}_Knee");
            }
            double zero = 0.00;
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                builder.AppendFormat($"\t{zero}\t{zero}\t{zero}\t{zero}\t{zero}");
            }
            Console.WriteLine($"WaferId {waferId} not found in database. Returning empty string.");
            return builder.ToString();
        }
        var initMeasurements = await this._initMeasureCollection.Find(e => e.QuickTestResultId==qt._id).ToListAsync();
        if (initMeasurements.Any()) {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat($"TimeStamp");
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                builder.AppendFormat($"\t{pad}_Voltage\t{pad}_Power\t{pad}_Wl\t{pad}_Ir\t{pad}_Knee");
            }

            builder.AppendLine();
            builder.AppendFormat($"{qt.InitialTimeStamp}");
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                var measurement = initMeasurements.FirstOrDefault(e => e.Pad != null && e.Pad.Contains(pad));
                if (measurement != null) {
                    builder.AppendFormat($"\t{Math.Round(measurement.Voltage, 2)}\t" +
                                         $"{Math.Round(measurement.Power, 2)}\t" +
                                         $"{Math.Round(measurement.Wl, 2)}\t" +
                                         $"{Math.Round(measurement.Ir, 2)}\t" +
                                         $"{Math.Round(measurement.Knee, 2)}");
                } else {
                    double zero = 0.00;
                    builder.AppendFormat($"\t{zero}\t{zero}\t{zero}\t{zero}\t{zero}");
                }
            }
            return builder.ToString();
        } else {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat($"TimeStamp");
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                builder.AppendFormat($"\t{pad}_Voltage\t{pad}_Power\t{pad}_Wl\t{pad}_Ir\t{pad}_Knee");
            }
            double zero = 0.00;
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                builder.AppendFormat($"\t{zero}\t{zero}\t{zero}\t{zero}\t{zero}");
            }
            return builder.ToString();
        }
    }
        

    /*public async Task<ErrorOr<IEnumerable<string>>> AvailableBurnInPads(string waferId) {
        var result = await this._qtCollection.Find(e => e.WaferId == waferId && e.InitialMeasurements != null)
            .Project(e => e.InitialMeasurements.Where(e => e.Pad != null)
                .Select(p => p.Pad)).FirstOrDefaultAsync();
        return result != null ? result.ToList() : Error.Failure(description: "Either WaferId is not valid or initial measurements are empty");
    }*/

    public Task<List<string>> GetWaferList() {
        return this._qtCollection.Find(e=>e.WaferId!=null).Project(e=>e.WaferId).ToListAsync();
    }


}