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
using QuickTest.Data.Contracts.Requests.Put;
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
            var result=await this._qtCollection.UpdateOneAsync(filter,update);
        }
    }
    
    public async Task DeleteQuickTest(string waferId) {
        await this._qtCollection.DeleteOneAsync(e => e.WaferId == waferId);
        await this._initMeasureCollection.DeleteManyAsync(e => e.WaferId == waferId);
        await this._finalMeasureCollection.DeleteManyAsync(e => e.WaferId == waferId);
        await this._initSpectrumCollection.DeleteManyAsync(e => e.WaferId == waferId);
        await this._finalSpectrumCollection.DeleteManyAsync(e => e.WaferId == waferId);
    }

    public async Task<ErrorOr<bool>> CreateQuickTest(string waferId,int stationId) {
        if (await this.QtWaferExists(waferId)) {
            return true;
        }
        QuickTestResult qt = new QuickTestResult() { _id = ObjectId.GenerateNewId(), WaferId = waferId,ProbeStationId = stationId};
        await this._qtCollection.InsertOneAsync(qt);
        return await this.QtWaferExists(waferId);
    }

    public async Task<ErrorOr<Success>> InsertAllMeasurements(InsertMeasurementRequest request) {
        List<Error> errors = [];
        foreach (var measurement in request.Measurements) {
            var result=await this.InsertMeasurement(measurement,(MeasurementType)request.MeasurementType,request.WaferId!,request.PadLocation!,request.ActualPad!);
            if (result.IsError) {
                errors.Add(result.FirstError);
            }
        }
        
        foreach (var measurement in request.SpectrumMeasurements) {
            var result=await this.InsertSpectrumMeasurement(measurement,(MeasurementType)request.MeasurementType,request.WaferId!,request.PadLocation!,request.ActualPad!);
            if (result.IsError) {
                errors.Add(result.FirstError);
            }
        }
        return errors.Any() ? errors : Result.Success;
    }
    
    public async Task<ErrorOr<Success>>InsertMeasurement(CurrentMeasurementDto measurement,MeasurementType type,string waferId,string padLocation,string actualPad) {
        QtMeasurement? qtMeasurement;
        if (type == MeasurementType.Initial) {
            qtMeasurement = await this._initMeasureCollection
                .Find(e => e.WaferId == waferId && e.Current == measurement.Current)
                .FirstOrDefaultAsync();
        } else {
            qtMeasurement = await this._finalMeasureCollection
                .Find(e => e.WaferId == waferId && e.Current == measurement.Current)
                .FirstOrDefaultAsync();
        }
        if (qtMeasurement != null) {
            var pad = PadLocation.List.FirstOrDefault(e => padLocation!.Contains(e));
            if(pad!=null) {
                qtMeasurement.Measurements[pad.Value]=new PadMeasurement() {
                    ActualPad = actualPad!,
                    Wl = measurement.Wl,
                    Power = measurement.Power,
                    Voltage = measurement.Voltage,
                    Knee = measurement.Knee,
                    Ir = measurement.Ir
                };
                var update=Builders<QtMeasurement>.Update
                    .Set(e=>e.Measurements,qtMeasurement.Measurements);
                if (type == MeasurementType.Initial) {
                    await this._initMeasureCollection.UpdateOneAsync(e=>e._id==qtMeasurement._id,update);
                    return Result.Success;
                } else {
                    await this._finalMeasureCollection.UpdateOneAsync(e=>e._id==qtMeasurement._id,update);
                    return Result.Success;
                }
            }
            return Error.NotFound(description: "Failed to find pad in database");
        } else {
            var qtResult=await this._qtCollection
                .Find(e=>e.WaferId==waferId)
                .FirstOrDefaultAsync();
            if(qtResult==null) {
                return Error.NotFound(description: $"QuickTestResult for {waferId} not found in database");
            }
            qtMeasurement=new QtMeasurement() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId!,
                Current = measurement.Current,
                QuickTestResultId = qtResult._id,
                Measurements = new Dictionary<string, PadMeasurement>()
            };
            var pad = PadLocation.List.FirstOrDefault(e => padLocation!.Contains(e));
            if(pad!=null) {
                qtMeasurement.Measurements.Add(pad.Value,new PadMeasurement() {
                    ActualPad = actualPad!,
                    Wl = measurement.Wl,
                    Power = measurement.Power,
                    Voltage = measurement.Voltage,
                    Knee = measurement.Knee,
                    Ir = measurement.Ir
                });
                if (type == (int)MeasurementType.Initial) {
                    await this._initMeasureCollection.InsertOneAsync(qtMeasurement);
                    return Result.Success;
                } else {
                    await this._finalMeasureCollection.InsertOneAsync(qtMeasurement);
                    return Result.Success;
                }
            }
            return Error.NotFound(description: "Failed to find pad in database");

        }
    }
    
    public async Task<ErrorOr<Success>> InsertSpectrumMeasurement(SpectrumMeasureDto measurement,MeasurementType type,string waferId,string padLocation,string actualPad) {
        Spectrum? spectrumMeasurement;
        if (type == (int)MeasurementType.Initial) {
            spectrumMeasurement = await this._initSpectrumCollection
                .Find(e=>e.WaferId==waferId && e.Current==measurement.Current)
                .FirstOrDefaultAsync();
        } else {
            spectrumMeasurement = await this._finalSpectrumCollection
                .Find(e=>e.WaferId==waferId && e.Current==measurement.Current)
                .FirstOrDefaultAsync();
        }
        if (spectrumMeasurement != null) {
            var pad = PadLocation.List.FirstOrDefault(e => padLocation!.Contains(e));
            if(pad!=null) {
                spectrumMeasurement.Measurements[pad.Value] = new PadSpectrumMeasurement() {
                    ActualPad = actualPad ?? "", 
                    Wl = measurement.Wl, 
                    Intensity = measurement.Intensity
                };
                var update=Builders<Spectrum>.Update
                    .Set(e=>e.Measurements,spectrumMeasurement.Measurements);
                if (type == MeasurementType.Initial) {
                    await this._initSpectrumCollection.UpdateOneAsync(e=>e._id==spectrumMeasurement._id,update);
                    return Result.Success;
                } else {
                    await this._finalSpectrumCollection.UpdateOneAsync(e=>e._id==spectrumMeasurement._id,update);
                    return Result.Success;
                }
            }
            return Error.NotFound(description: "Failed to find pad in database");
        } else {
            var qtResult=await this._qtCollection
                .Find(e=>e.WaferId==waferId)
                .FirstOrDefaultAsync();
            if(qtResult==null) {
                return Error.NotFound(description: $"QuickTestResult for {waferId} not found in database");
            }
            spectrumMeasurement=new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = waferId!,
                Current = measurement.Current,
                QuickTestResultId = qtResult._id,
                Measurements = new Dictionary<string, PadSpectrumMeasurement>()
            };
            var pad = PadLocation.List.FirstOrDefault(e => padLocation!.Contains(e));
            if(pad!=null) {
                spectrumMeasurement.Measurements.Add(pad.Value,new PadSpectrumMeasurement() {
                    ActualPad = actualPad ?? "", 
                    Wl = measurement.Wl, 
                    Intensity = measurement.Intensity
                });
                if (type == (int)MeasurementType.Initial) {
                    await this._initSpectrumCollection.InsertOneAsync(spectrumMeasurement);
                    return Result.Success;
                } else {
                    await this._finalSpectrumCollection.InsertOneAsync(spectrumMeasurement);
                    return Result.Success;
                }
            }
            return Error.NotFound(description: "Failed to find pad in database");
        }
    }
    
    /*public async Task<ErrorOr<Success>>InsertMeasurement(InsertMeasurementRequest measurement) {
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
            var pad = PadLocation.List.FirstOrDefault(e => measurement.PadLocation!.Contains(e));
            if(pad!=null) {
                qtMeasurement.Measurements[pad.Value]=new PadMeasurement() {
                    ActualPad = measurement.PadLocation!,
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
                    return Result.Success;
                } else {
                    await this._finalMeasureCollection.UpdateOneAsync(e=>e._id==qtMeasurement._id,update);
                    return Result.Success;
                }
            }
            return Error.NotFound(description: "Failed to find pad in database");
        } else {
            var qtResult=await this._qtCollection
                .Find(e=>e.WaferId==measurement.WaferId)
                .FirstOrDefaultAsync();
            if(qtResult==null) {
                return Error.NotFound(description: $"QuickTestResult for {measurement.WaferId} not found in database");
            }
            qtMeasurement=new QtMeasurement() {
                _id = ObjectId.GenerateNewId(),
                WaferId = measurement.WaferId!,
                Current = measurement.Current,
                QuickTestResultId = qtResult._id,
                Measurements = new Dictionary<string, PadMeasurement>()
            };
            var pad = PadLocation.List.FirstOrDefault(e => measurement.PadLocation!.Contains(e));
            if(pad!=null) {
                qtMeasurement.Measurements.Add(pad.Value,new PadMeasurement() {
                    ActualPad = measurement.PadLocation!,
                    Wl = measurement.Wl,
                    Power = measurement.Power,
                    Voltage = measurement.Voltage,
                    Knee = measurement.Knee,
                    Ir = measurement.Ir
                });
                if (measurement.MeasurementType == (int)MeasurementType.Initial) {
                    await this._initMeasureCollection.InsertOneAsync(qtMeasurement);
                    return Result.Success;
                } else {
                    await this._finalMeasureCollection.InsertOneAsync(qtMeasurement);
                    return Result.Success;
                }
            }
            return Error.NotFound(description: "Failed to find pad in database");

        }
    }*/
    
    /*public async Task<ErrorOr<Success>> InsertSpectrumMeasurement(InsertSpectrumMeasurementRequest measurement) {
        Spectrum? spectrumMeasurement;
        if (measurement.MeasurementType == (int)MeasurementType.Initial) {
            spectrumMeasurement = await this._initSpectrumCollection
                .Find(e=>e.WaferId==measurement.WaferId && e.Current==measurement.Current)
                .FirstOrDefaultAsync();
        } else {
            spectrumMeasurement = await this._finalSpectrumCollection
                .Find(e=>e.WaferId==measurement.WaferId && e.Current==measurement.Current)
                .FirstOrDefaultAsync();
        }
        if (spectrumMeasurement != null) {
            var pad = PadLocation.List.FirstOrDefault(e => measurement.PadLocation!.Contains(e));
            if(pad!=null) {
                spectrumMeasurement.Measurements[pad.Value] = new PadSpectrumMeasurement() {
                    ActualPad = measurement.ActualPad ?? "", 
                    Wl = measurement.Wl, 
                    Intensity = measurement.Intensity
                };
                var update=Builders<Spectrum>.Update
                    .Set(e=>e.Measurements,spectrumMeasurement.Measurements);
                if (measurement.MeasurementType == (int)MeasurementType.Initial) {
                    await this._initSpectrumCollection.UpdateOneAsync(e=>e._id==spectrumMeasurement._id,update);
                    return Result.Success;
                } else {
                    await this._finalSpectrumCollection.UpdateOneAsync(e=>e._id==spectrumMeasurement._id,update);
                    return Result.Success;
                }
            }
            return Error.NotFound(description: "Failed to find pad in database");
        } else {
            var qtResult=await this._qtCollection
                .Find(e=>e.WaferId==measurement.WaferId)
                .FirstOrDefaultAsync();
            if(qtResult==null) {
                return Error.NotFound(description: $"QuickTestResult for {measurement.WaferId} not found in database");
            }
            spectrumMeasurement=new Spectrum() {
                _id = ObjectId.GenerateNewId(),
                WaferId = measurement.WaferId!,
                Current = measurement.Current,
                QuickTestResultId = qtResult._id,
                Measurements = new Dictionary<string, PadSpectrumMeasurement>()
            };
            var pad = PadLocation.List.FirstOrDefault(e => measurement.PadLocation!.Contains(e));
            if(pad!=null) {
                spectrumMeasurement.Measurements.Add(pad.Value,new PadSpectrumMeasurement() {
                    ActualPad = measurement.ActualPad ?? "", 
                    Wl = measurement.Wl, 
                    Intensity = measurement.Intensity
                });
                if (measurement.MeasurementType == (int)MeasurementType.Initial) {
                    await this._initSpectrumCollection.InsertOneAsync(spectrumMeasurement);
                    return Result.Success;
                } else {
                    await this._finalSpectrumCollection.InsertOneAsync(spectrumMeasurement);
                    return Result.Success;
                }
            }
            return Error.NotFound(description: "Failed to find pad in database");
        }
    }*/
    
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

    public async Task<List<string>> GetResult(string waferId, MeasurementType type) {
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
    
    public async Task<Dictionary<string,PadMeasurementDto>> GetLabViewResult(string waferId, MeasurementType type) {
        var qt = await this._qtCollection.Find(e => e.WaferId == waferId)
            .FirstOrDefaultAsync();
        if(qt==null) {
            Dictionary<string, PadMeasurementDto> lvMeasurements = new Dictionary<string, PadMeasurementDto>();
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                lvMeasurements[pad] = new PadMeasurementDto() {
                    PadLocation = pad,
                    ActualPad = "",
                    Wl = 0.00,
                    Power = 0.00,
                    Voltage = 0.00,
                    Knee = 0.00,
                    Ir = 0.00
                };
            }
           
            return lvMeasurements;
        }
        Dictionary<string,PadMeasurement> measurements;
        if (type == MeasurementType.Initial) {
            measurements=await this._initMeasureCollection
                .Find(e => e.QuickTestResultId==qt._id && e.Current==20)
                .Project(e=>e.Measurements)
                .FirstOrDefaultAsync();

        } else {
            measurements=await this._finalMeasureCollection
                .Find(e => e.QuickTestResultId==qt._id && e.Current==20)
                .Project(e=>e.Measurements)
                .FirstOrDefaultAsync();

        }
        if (measurements.Any()) {
            Dictionary<string,PadMeasurementDto> lvMeasurements = new Dictionary<string, PadMeasurementDto>();
            foreach (var pad in PadLocation.List.Select(e=>e.Value)) {
                if (measurements.ContainsKey(pad)) {
                    lvMeasurements[pad]=new PadMeasurementDto() {
                        PadLocation = pad,
                        ActualPad = measurements[pad].ActualPad,
                        Wl = Math.Round(measurements[pad].Wl, 2),
                        Power = Math.Round(measurements[pad].Power, 2),
                        Voltage = Math.Round(measurements[pad].Voltage, 2),
                        Knee = measurements[pad].Knee,
                        Ir = measurements[pad].Ir
                    };
                } else {
                    double zero = 0.00;
                    lvMeasurements[pad] = new PadMeasurementDto() {
                        PadLocation = pad,
                        ActualPad = "",
                        Wl = zero,
                        Power = zero,
                        Voltage = zero,
                        Knee = zero,
                        Ir = zero
                    };
                }
            }
            return lvMeasurements;
        } else {
            Dictionary<string,PadMeasurementDto> lvMeasurements = new Dictionary<string, PadMeasurementDto>();
            double zero = 0.00;
            foreach (var pad in PadLocation.List.Select(e => e.Value)) {
                lvMeasurements[pad] = new PadMeasurementDto() {
                    PadLocation = pad,
                    ActualPad = "",
                    Wl = zero,
                    Power = zero,
                    Voltage = zero,
                    Knee = zero,
                    Ir = zero
                };
            }
            return lvMeasurements;
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
    
    public async Task<(List<string> testedPads,int waferSize)> GetAvailableBurnInPads(string waferId) {
        var waferSize=await this._qtCollection.Find(e=>e.WaferId==waferId)
            .Project(e=>e.WaferSize)
            .FirstOrDefaultAsync();
        waferSize=waferSize==0 ? 2 : waferSize;
        var measurements = await this._initMeasureCollection.Find(e => e.WaferId == waferId && e.Current == 20)
            .Project(e => e.Measurements)
            .FirstOrDefaultAsync();
        return (measurements.Values.Select(e=>e.ActualPad).ToList() ?? [],waferSize);
    }

    public async Task<List<string>> GetQuickTestList(DateTime start) {
        var waferList=await this._qtCollection.Find(e=>e.InitialTimeStamp>start).Project(e=>e.WaferId!).ToListAsync();
        return waferList ?? [];
    }
}