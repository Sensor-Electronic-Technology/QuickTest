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
    private ILogger<WaferDataService> _logger;
    
    public WaferDataService(ILogger<WaferDataService> logger,IMongoClient mongoClient) {
        this._logger = logger;
        var database = mongoClient.GetDatabase("quick_test_db");
        this._waferPadCollection = database.GetCollection<WaferPad>("wafer_pads");
    }

    public Task<List<WaferPad>?> GetWaferPads(WaferSize waferSize) {
        return this._waferPadCollection.Find(e=>e.WaferSize==waferSize).ToListAsync();
    }

    public async Task<IEnumerable<WaferPad>> GetWaferPads(List<string> pads) {
        var p = await this._waferPadCollection.Find(e => pads.Contains(e.Identifier!)).ToListAsync();
        return p ?? Enumerable.Empty<WaferPad>().ToList();
    }
    
    /*public Task<List<WaferPad>?> GetAvailableBurnInPads(WaferSize waferSize) {
        return this._waferPadCollection.Find(e=>e.WaferSize==WaferSize.TwoInch).ToListAsync();
    }*/
    
    public Task<List<Pad>?> GetMap(WaferSize waferSize) {
        return this._waferPadCollection.Find(e => e.WaferSize == waferSize && e.SvgObject!=null).Project(e => 
                new Pad() {
                    Identifier = e.Identifier, X = e.SvgObject!.X, Y = e.SvgObject!.Y, Radius = e.SvgObject!.Radius
                })
            .ToListAsync();
    }

    public async Task<ErrorOr<WaferPad>> CreateWaferPad(WaferPad pad) {
        pad._id = ObjectId.GenerateNewId();
        await this._waferPadCollection.InsertOneAsync(pad);
        var check = await this.Exists(id: pad._id);
        return check ? pad : Error.Failure(description: "Failed to create wafer pad");
    }
    
    public async Task<bool> Exists(string? identifier=default, ObjectId? id=default) {
        if (!string.IsNullOrEmpty(identifier)) {
            var check=await this._waferPadCollection.Find(e=>e.Identifier==identifier).FirstOrDefaultAsync();
            return check != null;
        }
        if (id != null) {
            var check=await this._waferPadCollection.Find(e=>e._id==id).FirstOrDefaultAsync();
            return check != null;
        }
        return false;
    }
}