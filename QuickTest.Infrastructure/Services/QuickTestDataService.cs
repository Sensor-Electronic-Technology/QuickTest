using System.Globalization;
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
    
    public async Task<List<List<string>>> GetResults(List<string> waferIds,MeasurementType type) {
        List<List<string>> rows = new List<List<string>>();
        foreach (var waferId in waferIds) {
            var row=await this.GetResult(waferId,type);
            rows.Add(row);
        }
        return rows;
    }

    public async Task<List<string>> GetResultAll(string waferId) {
        List<string> row= new List<string>();
        var initial = await this.GetResult(waferId,MeasurementType.Initial);
        var final = await this.GetResult(waferId,MeasurementType.Final);
        row.AddRange(initial);
        row.AddRange(final);
        return row;
    }

    public async Task<List<List<string>>> GetAllResults(List<string> waferIds) {
        List<List<string>> rows = new List<List<string>>();
        foreach (var waferId in waferIds) {
            var row=await this.GetResultAll(waferId);
            rows.Add(row);
        }
        return rows;
    }
    
    public async Task<List<string>> GetResult(string waferId,MeasurementType type) {
        var qt = await this._qtCollection.Find(e => e.WaferId == waferId)
            .FirstOrDefaultAsync();
        List<string> values = new List<string>();
        if(qt==null) {
            double zero = 0.00;
            values.Add("");
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                values.AddRange([zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture)]);
            }
            Console.WriteLine($"WaferId {waferId} not found in database. Returning empty.");
            return values;
        }
        List<Measurement> measurements;
        StringBuilder builder = new StringBuilder();
        if (type == MeasurementType.Initial) {
            measurements=await this._initMeasureCollection.Find(e => e.QuickTestResultId==qt._id).ToListAsync();
            builder.AppendFormat($"{qt.InitialTimeStamp.ToString(CultureInfo.InvariantCulture)}");
        } else {
            measurements = await this._finalMeasureCollection.Find(e => e.QuickTestResultId==qt._id).ToListAsync();
            builder.AppendFormat($"{qt.FinalTimeStamp.ToString(CultureInfo.InvariantCulture)}");
        }
        if (measurements.Any()) {
            values.Add(qt.InitialTimeStamp.ToString(CultureInfo.InvariantCulture));
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                var padMeasurements = measurements.Where(e => e.Pad != null && e.Pad.Contains(pad)).ToArray();
                Measurement? measurement=null;
                if (padMeasurements.Any()) {
                    if(padMeasurements.Count()>1) {
                        measurement=padMeasurements.OrderBy(e => e._id).First();
                    } else {
                        measurement=padMeasurements.First();
                    }
                } else {
                    measurement = null;
                }
                if (measurement != null) {
                    values.AddRange([Math.Round(measurement.Wl, 2).ToString(CultureInfo.InvariantCulture),
                                     Math.Round(measurement.Power, 2).ToString(CultureInfo.InvariantCulture),
                                     Math.Round(measurement.Voltage, 2).ToString(CultureInfo.InvariantCulture),
                                     measurement.Knee.ToString(CultureInfo.InvariantCulture),
                                     measurement.Ir.ToString(CultureInfo.InvariantCulture)]);
                } else {
                    double zero = 0.00;
                    values.AddRange([zero.ToString(CultureInfo.InvariantCulture),
                                     zero.ToString(CultureInfo.InvariantCulture),
                                     zero.ToString(CultureInfo.InvariantCulture),
                                     zero.ToString(CultureInfo.InvariantCulture),
                                     zero.ToString(CultureInfo.InvariantCulture)]);
                }
            }
            return values;
        } else {
            double zero = 0.00;
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                values.AddRange([zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture)]);
            }
            return values;
        }
    }
    
    public Task<List<string>> GetWaferList() {
        return this._qtCollection.Find(e=>e.WaferId!=null).Project(e=>e.WaferId).ToListAsync();
    }


}