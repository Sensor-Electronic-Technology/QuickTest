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
using QuickTest.Data.DataTransfer;
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
        //var database=mongoClient.GetDatabase("quick_test_db_v2");
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

    public async Task MarkTested(string waferId,bool tested, MeasurementType measurementType) {
        var filter=Builders<QuickTestResult>.Filter.Eq(e=>e.WaferId,waferId);
        if (measurementType == MeasurementType.Initial) {
            var update = Builders<QuickTestResult>.Update
                .Set(e => e.InitialTested, tested)
                .Set(e => e.InitialTimeStamp, tested ? DateTime.Now : DateTime.MinValue);
            this._logger.LogInformation("Updated {Wafer} InitialTested to {Tested}",waferId,tested);
            await this._qtCollection.UpdateOneAsync(filter,update);
        } else {
            var update = Builders<QuickTestResult>.Update
                .Set(e => e.FinalTested, tested)
                .Set(e=>e.FinalTimeStamp,tested ? DateTime.Now:DateTime.MinValue);
            this._logger.LogInformation("Updated {Wafer} FinalTested to {Tested}",waferId,tested);
            await this._qtCollection.UpdateOneAsync(filter,update);
        }
    }

    public async Task<ErrorOr<bool>> CreateQuickTest(string waferId,int stationId) {
        if (await this.QtWaferExists(waferId)) {
            return true;
        }
        QuickTestResult qt = new QuickTestResult() { _id = ObjectId.GenerateNewId(), WaferId = waferId };
        await this._qtCollection.InsertOneAsync(qt);
        return await this.QtWaferExists(waferId);
    }
    
    public async Task<bool> InsertMeasurement(MeasurementDto measurement) {
        QtMeasurement? qtMeasurement;
        if (measurement.MeasurementType == (int)MeasurementType.Initial) {
            qtMeasurement = await this._initMeasureCollection
                .Find(e => e.WaferId == measurement.WaferId && e.Current == measurement.Current)
                .FirstOrDefaultAsync();
        } else {
            qtMeasurement = await this._finalMeasureCollection
                .Find(e => e.WaferId == measurement.WaferId && e.Current == measurement.Current)
                .FirstOrDefaultAsync();
        }
        if (qtMeasurement != null) {
            var pad = PadLocation.List.FirstOrDefault(e => measurement.Pad!.Contains(e));
            if(pad!=null) {
                qtMeasurement.Measurements[pad.Value]=new PadMeasurement() {
                    ActualPad = measurement.Pad!,
                    Wl = measurement.Wl,
                    Power = measurement.Power,
                    Voltage = measurement.Voltage,
                    Knee = measurement.Knee,
                    Ir = measurement.Ir
                };
                var update=Builders<QtMeasurement>.Update
                    .Set(e=>e.Measurements,qtMeasurement.Measurements);
                if (measurement.MeasurementType == (int)MeasurementType.Initial) {
                    await this._initMeasureCollection.UpdateOneAsync(e=>e._id==qtMeasurement._id,update);
                    return true;
                } else {
                    await this._finalMeasureCollection.UpdateOneAsync(e=>e._id==qtMeasurement._id,update);
                    return true;
                }
            }
            return false;
        } else {
            qtMeasurement=new QtMeasurement() {
                _id = ObjectId.GenerateNewId(),
                WaferId = measurement.WaferId!,
                Current = measurement.Current,
                Measurements = new Dictionary<string, PadMeasurement>()
            };
            var pad = PadLocation.List.FirstOrDefault(e => measurement.Pad!.Contains(e));
            if(pad!=null) {
                qtMeasurement.Measurements.Add(pad.Value,new PadMeasurement() {
                    ActualPad = measurement.Pad!,
                    Wl = measurement.Wl,
                    Power = measurement.Power,
                    Voltage = measurement.Voltage,
                    Knee = measurement.Knee,
                    Ir = measurement.Ir
                });
            }
            var update=Builders<QtMeasurement>.Update
                .Set(e=>e.Measurements,qtMeasurement.Measurements);
            if (measurement.MeasurementType == (int)MeasurementType.Initial) {
                await this._initMeasureCollection.UpdateOneAsync(e=>e._id==qtMeasurement._id,update);
                return true;
            } else {
                await this._finalMeasureCollection.UpdateOneAsync(e=>e._id==qtMeasurement._id,update);
                return true;
            }
        }
        return false;
    }
    
    public Task<bool> QtWaferExists(string waferId) {
        return this._qtCollection.Find(e=>e.WaferId==waferId).AnyAsync();
    }

    public async Task<ErrorOr<bool>> WaferExists(string waferId) {
        var result =
            await this._epiDataClient.GetFromJsonAsync<CheckWaferExistsResponse>(
                $"{ApiPaths.CheckWaferExistsPath}{waferId}");
        if (result == null) {
            this._logger.LogError("Error checking wafer exists, Null return value");
            return Error.Failure(description:"Failed to check EpiDatabase for wafer existence. Result was null");
        }
        return result.Exists;
    }
    
    public async Task<List<List<string>>> GetResults(List<string> waferIds,MeasurementType type) {
        List<List<string>> rows = new List<List<string>>();
        foreach (var waferId in waferIds) {
            var row=await this.GetResultV2(waferId,type);
            rows.Add(row);
        }
        return rows;
    }

    public async Task<List<string>> GetResultAll(string waferId) {
        List<string> row= new List<string>();
        var initial = await this.GetResultV2(waferId,MeasurementType.Initial);
        var final = await this.GetResultV2(waferId,MeasurementType.Final);
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

    public async Task<List<string>> GetResultV2(string waferId, MeasurementType type) {
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
        //List<QtMeasurement> measurements;
        Dictionary<string,PadMeasurement> measurements;
        StringBuilder builder = new StringBuilder();
        if (type == MeasurementType.Initial) {
            measurements=await this._initMeasureCollection
                .Find(e => e.QuickTestResultId==qt._id && e.Current==20)
                .Project(e=>e.Measurements)
                .FirstOrDefaultAsync();
            builder.AppendFormat($"{qt.InitialTimeStamp.ToString(CultureInfo.InvariantCulture)}");
        } else {
            measurements=await this._finalMeasureCollection
                .Find(e => e.QuickTestResultId==qt._id && e.Current==20)
                .Project(e=>e.Measurements)
                .FirstOrDefaultAsync();
            builder.AppendFormat($"{qt.FinalTimeStamp.ToString(CultureInfo.InvariantCulture)}");
        }
        if (measurements.Any()) {
            values.Add(qt.InitialTimeStamp.ToString(CultureInfo.InvariantCulture));
            GetPadMeasurement(measurements,values);
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
    
    private static void GetPadMeasurement(Dictionary<string,PadMeasurement> measurements, List<string> values) {
        foreach (var pad in PadLocation.List) {
            if (measurements.ContainsKey(pad)) {
                values.AddRange([Math.Round(measurements[pad].Wl, 2).ToString(CultureInfo.InvariantCulture),
                    Math.Round(measurements[pad].Power, 2).ToString(CultureInfo.InvariantCulture),
                    Math.Round(measurements[pad].Voltage, 2).ToString(CultureInfo.InvariantCulture),
                    measurements[pad].Knee.ToString(CultureInfo.InvariantCulture),
                    measurements[pad].Ir.ToString(CultureInfo.InvariantCulture)]);
            } else {
                double zero = 0.00;
                values.AddRange([zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture),
                    zero.ToString(CultureInfo.InvariantCulture)]);
            }
        }
    }
    
    public async Task<List<string>> GetAvailableBurnInPads(string waferId) {
        var measurements = await this._initMeasureCollection.Find(e => e.WaferId == waferId && e.Current == 20)
            .Project(e => e.Measurements)
            .FirstOrDefaultAsync();
        return measurements.Keys.ToList() ?? [];
    }

    public async Task<List<string>> GetQuickTestList(DateTime start) {
        var waferList=await this._qtCollection.Find(e=>e.InitialTimeStamp>start).Project(e=>e.WaferId!).ToListAsync();
        return waferList ?? [];
    }
}