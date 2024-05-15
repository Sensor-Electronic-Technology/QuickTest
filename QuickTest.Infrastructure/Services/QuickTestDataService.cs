using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using EpiData.Data.Constants;
using EpiData.Data.Contracts.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuickTest.Data.AppSettings;
using QuickTest.Data.Models.Measurements;
using ErrorOr;
using MongoDB.Bson;
using QuickTest.Data.Models;
using QuickTest.Data.Models.Wafers.Enums;
namespace QuickTest.Infrastructure.Services;

public class CheckQuickTestResult {
    public bool Exisits { get; set; }
    public bool Tested { get; set; }
    public int StationId { get; set; }
}

public class QuickTestDataService {
    private readonly IMongoCollection<QuickTestResult> _qtCollection;
    private readonly IMongoCollection<QtMeasurement> _initMeasureCollection;
    private readonly IMongoCollection<QtMeasurement> _finalMeasureCollection;
    private readonly IMongoCollection<ProbeStation> _probeStationCollection;
    private readonly HttpClient _epiDataClient;
    private readonly IMongoCollection<Spectrum> _initSpectrumCollection;
    private readonly IMongoCollection<Spectrum> _finalSpectrumCollection;
    private ILogger<QuickTestDataService> _logger;

    public QuickTestDataService(ILogger<QuickTestDataService> logger, IMongoClient mongoClient,IOptions<DatabaseSettings> options) {
        this._logger = logger;
        var database = mongoClient.GetDatabase(options.Value.DatabaseName);
        this._qtCollection = database.GetCollection<QuickTestResult>(options.Value.QuickTestCollectionName ??"quick_test");
        this._initMeasureCollection=database.GetCollection<QtMeasurement>(options.Value.InitialMeasurementCollectionName ?? "init_measurements");
        this._finalMeasureCollection=database.GetCollection<QtMeasurement>(options.Value.FinalMeasurementCollectionName ??"final_measurements");
        this._initSpectrumCollection=database.GetCollection<Spectrum>(options.Value.InitialSpectrumCollectionName ??"init_spectrum");
        this._finalSpectrumCollection=database.GetCollection<Spectrum>(options.Value.FinalSpectrumCollectionName ??"final_spectrum");
        this._probeStationCollection=database.GetCollection<ProbeStation>(options.Value.ProbeStationCollectionName ??"probe_stations");
        this._epiDataClient = new HttpClient();
        this._epiDataClient.BaseAddress= new Uri(options.Value.EpiDataEndpoint);
    }
    
    public QuickTestDataService(IMongoClient mongoClient,string dbName) {
        var database = mongoClient.GetDatabase(dbName);
        this._qtCollection = database.GetCollection<QuickTestResult>("quick_test");
    }

    public async Task<CheckQuickTestResult> CheckQuickTest(string waferId,MeasurementType type) {
        var qt=await this._qtCollection.Find(e=>e.WaferId==waferId)
            .FirstOrDefaultAsync();
        if (qt == null) {
            return new CheckQuickTestResult() { Exisits = false, Tested = false ,StationId = 0};
        }
        
        if (type == MeasurementType.Initial) {
            return new CheckQuickTestResult() { Exisits = true, Tested = qt.InitialTested, StationId = qt.ProbeStationId };
        } else {
            return new CheckQuickTestResult() { Exisits = true, Tested = qt.FinalTested, StationId = qt.ProbeStationId };
        }
    }

    public async Task<ErrorOr<bool>> CreateQuickTest(string waferId,int stationId) {
        if (await this.QtWaferExists(waferId)) {
            return true;
        }
        var epiDataResult = await this.WaferExists(waferId);
        if (epiDataResult.IsError) {
            return epiDataResult.FirstError;
        }
        if (epiDataResult.Value) {
            QuickTestResult qt = new QuickTestResult() { _id = ObjectId.GenerateNewId(), WaferId = waferId };
            await this._qtCollection.InsertOneAsync(qt);
            return await this.QtWaferExists(waferId);
        } else {
            return false;
        }
    }
    
    public async Task<bool> InsertMeasurement(QtMeasurement measurement) {
        if (measurement.MeasurementType == (int)MeasurementType.Initial) {
            measurement._id = ObjectId.GenerateNewId();
            await this._initMeasureCollection.InsertOneAsync(measurement);
            return await this._initMeasureCollection.Find(e=>e._id==measurement._id).AnyAsync();
        } else {
            measurement._id = ObjectId.GenerateNewId();
            await this._finalMeasureCollection.InsertOneAsync(measurement);
            return await this._initMeasureCollection.Find(e=>e._id==measurement._id).AnyAsync();
        }
    }
    
    public Task<bool> QtWaferExists(string waferId) {
        return this._qtCollection.Find(e=>e.WaferId==waferId).AnyAsync();
    }

    public async Task<ErrorOr<bool>> WaferExists(string waferId) {
        var result =
            await this._epiDataClient.GetFromJsonAsync<CheckWaferExistsResponse>(
                $"{ApiPaths.CheckWaferExistsEndpoint}{waferId}");
        if (result == null) {
            this._logger.LogError("Error checking wafer exists, Null return value");
            return Error.Failure(description:"Failed to check EpiDatabase for wafer existence. Result was null");
        }
        return result.Exists;
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
        List<QtMeasurement> measurements;
        StringBuilder builder = new StringBuilder();
        if (type == MeasurementType.Initial) {
            measurements=await this._initMeasureCollection.Find(e => e.QuickTestResultId==qt._id && e.Current==20).ToListAsync();
            builder.AppendFormat($"{qt.InitialTimeStamp.ToString(CultureInfo.InvariantCulture)}");
        } else {
            measurements = await this._finalMeasureCollection.Find(e => e.QuickTestResultId==qt._id).ToListAsync();
            builder.AppendFormat($"{qt.FinalTimeStamp.ToString(CultureInfo.InvariantCulture)}");
        }
        if (measurements.Any()) {
            values.Add(qt.InitialTimeStamp.ToString(CultureInfo.InvariantCulture));
            GetPadMeasurement(measurements,PadLocation.PadLocationA.Value,values);
            GetPadMeasurement(measurements,PadLocation.PadLocationB.Value,values);
            GetPadMeasurement(measurements,PadLocation.PadLocationC.Value,values);
            GetPadMeasurement(measurements,PadLocation.PadLocationD.Value,values);
            GetPadMeasurement(measurements,PadLocation.PadLocationR.Value,values);
            GetPadMeasurement(measurements,PadLocation.PadLocationT.Value,values);
            GetPadMeasurement(measurements,PadLocation.PadLocationL.Value,values);
            GetPadMeasurement(measurements,PadLocation.PadLocationG.Value,values);
            /*foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                GetPadMeasurement(measurements, pad, values);
            }*/
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

    private static void GetPadMeasurement(List<QtMeasurement> measurements, string pad, List<string> values) {
        var padMeasurements = measurements.Where(e => e.Pad != null && e.Pad.Contains(pad)).ToArray();
        QtMeasurement? measurement=null;
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
    
    public async Task<List<string>> GetAvailableBurnInPads(string waferId) {
        var pads= await  this._initMeasureCollection.Find(e => e.WaferId == waferId && e.Current == 20 && !string.IsNullOrEmpty(e.Pad)).Project(e => e.Pad!)
            .ToListAsync();
        return pads ?? Enumerable.Empty<string>().ToList();
    }
}