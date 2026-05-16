using CryptoPriceTracker.Api.Data;
using CryptoPriceTracker.Api.Models;
using CryptoPriceTracker.Api.Validators;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CryptoPriceTracker.Api.Services;

public class CryptoPriceService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly PriceValidator _validator;

    public CryptoPriceService(ApplicationDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _validator = new PriceValidator();
        
        // CoinGecko API requires a user-agent
        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "CryptoPriceTrackerApi/1.0");
        }
    }

    public async Task UpdatePricesAsync()
    {
        var cryptoAssets = await _dbContext.CryptoAssets.ToListAsync();
        Console.WriteLine($"[Service] Found {cryptoAssets.Count} assets in DB.");
        if (!cryptoAssets.Any()) return;

        var ids = string.Join(",", cryptoAssets.Select(a => a.ExternalId));
        var url = $"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids={ids}";
        Console.WriteLine($"[Service] Fetching from: {url}");

        try 
        {
            var response = await _httpClient.GetAsync(url);
            Console.WriteLine($"[Service] Response status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var coinDataList = JsonSerializer.Deserialize<List<CoinGeckoMarketData>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (coinDataList == null || !coinDataList.Any()) 
                {
                    Console.WriteLine("[Service] Deserialization failed or returned empty list.");
                    return;
                }

                foreach (var asset in cryptoAssets)
                {
                    var coinData = coinDataList.FirstOrDefault(c => c.Id == asset.ExternalId);
                    if (coinData != null)
                    {
                        asset.IconUrl = coinData.Image;

                        var history = await _dbContext.CryptoPriceHistories
                            .Where(p => p.CryptoAssetId == asset.Id)
                            .OrderByDescending(p => p.Date)
                            .Take(10) 
                            .ToListAsync();

                        if (_validator.ShouldSavePrice(coinData.CurrentPrice, DateTime.UtcNow, history))
                        {
                            var priceHistory = new CryptoPriceHistory
                            {
                                CryptoAssetId = asset.Id,
                                Price = coinData.CurrentPrice,
                                Date = DateTime.UtcNow
                            };

                            _dbContext.CryptoPriceHistories.Add(priceHistory);
                            Console.WriteLine($"[Service] Saved new price for {asset.Name}: {coinData.CurrentPrice}");
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[Service] Error response: {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Service] Exception: {ex.Message}");
            throw;
        }
    }

    private class CoinGeckoMarketData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("current_price")]
        public decimal CurrentPrice { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; } = string.Empty;
    }
}