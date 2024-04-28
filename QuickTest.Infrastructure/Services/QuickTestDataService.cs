using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuickTest.Data.AppSettings;
using QuickTest.Data.Models.Measurements;
using ErrorOr;

namespace QuickTest.Infrastructure.Services;

public class QuickTestDataService {
    private readonly IMongoCollection<QuickTestResult> _qtCollection;
    private ILogger<QuickTestDataService> _logger;

    public QuickTestDataService(ILogger<QuickTestDataService> logger, IMongoClient mongoClient,IOptions<DatabaseSettings> options) {
        this._logger = logger;
        var database = mongoClient.GetDatabase(options.Value.DatabaseName);
        this._qtCollection = database.GetCollection<QuickTestResult>(options.Value.QuickTestCollectionName ?? "quick_test");
    }
    
    public QuickTestDataService(IMongoClient mongoClient,string dbName) {
        var database = mongoClient.GetDatabase(dbName);
        this._qtCollection = database.GetCollection<QuickTestResult>("quick_test");
    }
    
    public Task<bool> WaferExists(string waferId) {
        return this._qtCollection.Find(e=>e.WaferId==waferId).AnyAsync();
    }

    public async Task<ErrorOr<IEnumerable<string>>> AvailableBurnInPads(string waferId) {
        var result = await this._qtCollection.Find(e => e.WaferId == waferId && e.InitialMeasurements != null)
            .Project(e => e.InitialMeasurements.Where(e => e.Pad != null)
                .Select(p => p.Pad)).FirstOrDefaultAsync();
        return result != null ? result.ToList() : Error.Failure(description: "Either WaferId is not valid or initial measurements are empty");
    }

    public Task<List<string>> GetWaferList() {
        return this._qtCollection.Find(e=>e.WaferId!=null).Project(e=>e.WaferId).ToListAsync();
    }


}