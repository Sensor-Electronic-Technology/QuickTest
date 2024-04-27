using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using QuickTest.Data.Wafer;
using ErrorOr;
using MongoDB.Bson;
using ILogger = Amazon.Runtime.Internal.Util.ILogger;

namespace QuickTest.Infrastructure.Services;

public class WaferDataService {
    private readonly IMongoCollection<WaferPad> _waferPadCollection;
    private ILogger<WaferDataService> _logger;
    
    public WaferDataService(ILogger<WaferDataService> logger,IMongoClient mongoClient) {
        this._logger = logger;
        var database = mongoClient.GetDatabase("quick_test_db");
        _waferPadCollection = database.GetCollection<WaferPad>("wafer_pads");
    }

    public Task<List<WaferPad>?> GetWaferPads(WaferSize waferSize) {
        return this._waferPadCollection.Find(e=>e.WaferSize==waferSize).ToListAsync();
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