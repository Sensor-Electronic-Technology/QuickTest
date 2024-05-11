using System.Net.Http.Json;
using EpiData.Data.Constants;
using EpiData.Data.Contracts.Requests;
using EpiData.Data.Contracts.Responses;
using EpiData.Data.Dtos;
using Microsoft.Extensions.Configuration;
using QuickTest.Data.Models.Wafers;

namespace QuickTest.Infrastructure.Services;

public class EpiDataService {
    private readonly HttpClient _epiDataClient;
    
    public EpiDataService(IConfiguration configuration) {
        var baseUrl=configuration[""] ?? "http://172.20.4.204";
        this._epiDataClient = new HttpClient();
        this._epiDataClient.BaseAddress= new Uri(baseUrl);
    }
    
    /*public EpiDataService() {
        //var baseUrl=configuration[""] ?? "http://172.20.4.204";
        this._epiDataClient = new HttpClient();
        this._epiDataClient.BaseAddress= new Uri("http://172.20.4.204");
    }*/
    
    public async Task<LedWaferDto?> GetWaferById(string waferId) {
        var response=await this._epiDataClient.GetFromJsonAsync<GetLedWaferResponse>($"{ApiPaths.GetLedWaferEndpoint}{waferId}");
        return response?.LedWafer ?? null;
    }

    public async Task<List<string>?> GetLedWafersSince(DateTime growthDate) {
        var requestDateTime = new GetLedWafersSinceRequest() { GrowthDate = growthDate };
        var response = await this._epiDataClient.GetFromJsonAsync<GetLedWafersSinceResponse>(
                $"{ApiPaths.GetLedWafersSinceEndpoint}{requestDateTime}");
        /*var response = await this._epiDataClient.GetFromJsonAsync<GetLedWafersSinceResponse>(
            $"{ApiPaths.GetLedWafersSinceEndpoint}{requestDateTime.GrowthDate:yyyy-MM-ddTHH:mm:ss.fffZ}");*/
        return response?.LedWafers.ToList();
    }
    
    
    
    
}