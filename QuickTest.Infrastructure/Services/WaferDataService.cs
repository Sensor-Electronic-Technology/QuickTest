using Amazon.Runtime.Internal.Util;
using EpiData.Data.Models.Epi.Enums;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ErrorOr;
using MongoDB.Bson;
using QuickTest.Data.DataTransfer;
using QuickTest.Data.Models.Measurements;
using QuickTest.Data.Models.Wafers;
using QuickTest.Data.Models.Wafers.Enums;
using ILogger = Amazon.Runtime.Internal.Util.ILogger;

namespace QuickTest.Infrastructure.Services;

public class WaferDataService {
    private readonly IMongoCollection<WaferPad> _waferPadCollection;
    private readonly IMongoCollection<WaferMap> _waferMapCollection;
    private readonly IMongoCollection<LvWaferMap> _labviewWaferMapCollection;
    private ILogger<WaferDataService> _logger;

    public WaferDataService(ILogger<WaferDataService> logger, IMongoClient mongoClient) {
        this._logger = logger;
        var database = mongoClient.GetDatabase("quick_test_db");
        this._waferPadCollection = database.GetCollection<WaferPad>("wafer_pads");
        this._waferMapCollection = database.GetCollection<WaferMap>("wafer_maps");
        this._labviewWaferMapCollection = database.GetCollection<LvWaferMap>("lv_wafer_maps");
    }

    public WaferDataService(IMongoClient mongoClient) {
        var database = mongoClient.GetDatabase("quick_test_db");
        this._waferPadCollection = database.GetCollection<WaferPad>("wafer_pads");
        this._waferMapCollection = database.GetCollection<WaferMap>("wafer_maps");
        this._labviewWaferMapCollection = database.GetCollection<LvWaferMap>("lv_wafer_maps");
    }

    public Task<List<WaferPad>?> GetWaferPads(WaferSize waferSize) {
        return this._waferPadCollection.Find(e => e.WaferSize == waferSize).ToListAsync();
    }

    public Task CreateWaferMap(WaferMap map) {
        return this._waferMapCollection.InsertOneAsync(map);
    }

    public async Task<IEnumerable<WaferPad>> GetWaferPads(List<string> pads) {
        var p = await this._waferPadCollection.Find(e => pads.Contains(e.Identifier!)).ToListAsync();
        return p ?? Enumerable.Empty<WaferPad>().ToList();
    }

    /*public Task<List<WaferPad>?> GetAvailableBurnInPads(WaferSize waferSize) {
        return this._waferPadCollection.Find(e=>e.WaferSize==WaferSize.TwoInch).ToListAsync();
    }*/
    
    public async Task<LvWaferMap?> GetLabviewWaferMap(WaferSize waferSize) {
        return await this._labviewWaferMapCollection.Find(e => e.Size == waferSize.Value).FirstOrDefaultAsync();
    }

    public async Task<WaferMapDto?> GetMap(WaferSize waferSize) { 
        var waferMap=await this._waferMapCollection.Find(e => e.WaferSize == waferSize).FirstOrDefaultAsync();
        if (waferMap != null) {
            var pads=await this._waferPadCollection.Find(e => waferMap.PadIds.Contains(e._id)).Project(e=>new Pad() {
                Identifier = e.Identifier,
                X=e.SvgObject.X,
                Y=e.SvgObject.Y,
                Radius = e.SvgObject.Radius
            }).ToListAsync();
            var waferMapDto=waferMap.WaferMapDto();
            waferMapDto.MapPads = pads;
            return waferMapDto;
        } else {
            return null;
        }
    }

    public Task<List<Pad>> GetPads(WaferSize waferSize) {
        return this._waferPadCollection.Find(e => e.WaferSize == waferSize && e.SvgObject!=null).Project(e =>
        new Pad() {
            Identifier = e.Identifier, X = e.SvgObject!.X, Y = e.SvgObject!.Y, Radius = e.SvgObject!.Radius
        })
        .ToListAsync();
    }
    
    public Task<List<WaferPad>> GetPadsOther(WaferSize waferSize) {
        return this._waferPadCollection.Find(e => e.WaferSize == waferSize && e.SvgObject!=null)
            .ToListAsync();
    }

    public async Task<ErrorOr<WaferPad>> CreateWaferPad(WaferPad pad) {
        pad._id = ObjectId.GenerateNewId();
        await this._waferPadCollection.InsertOneAsync(pad);
        var check = await this.Exists(pad.WaferSize,id: pad._id);
        return check ? pad : Error.Failure(description: "Failed to create wafer pad");
    }
    
    public async Task<bool> Exists(WaferSize size,string? identifier=default, ObjectId? id=default) {
        if (!string.IsNullOrEmpty(identifier)) {
            var check=await this._waferPadCollection.Find(e=>e.Identifier==identifier && e.WaferSize==size).FirstOrDefaultAsync();
            return check != null;
        }
        if (id != null) {
            var check=await this._waferPadCollection.Find(e=>e._id==id && e.WaferSize==size).FirstOrDefaultAsync();
            return check != null;
        }
        return false;
    }
}