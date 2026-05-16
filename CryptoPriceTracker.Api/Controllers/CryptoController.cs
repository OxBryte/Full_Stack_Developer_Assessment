using Microsoft.AspNetCore.Mvc;
using CryptoPriceTracker.Api.Services;
using CryptoPriceTracker.Api.Data;
using Microsoft.EntityFrameworkCore;
using CryptoPriceTracker.Api.Models;

namespace CryptoPriceTracker.Api.Controllers
{
    [ApiController]
    [Route("api/crypto")]
    public class CryptoController : ControllerBase
    {
        private readonly CryptoPriceService _service;
        private readonly ApplicationDbContext _db;

        public CryptoController(CryptoPriceService service, ApplicationDbContext db)
        {
            _service = service;
            _db = db;
        }

        [HttpPost("update-prices")]
        public async Task<IActionResult> UpdatePrices()
        {
            try
            {
                await _service.UpdatePricesAsync();
                return Ok(new { message = "Prices updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating prices.", error = ex.Message });
            }
        }

        [HttpGet("latest-prices")]
        public async Task<IActionResult> GetLatestPrices()
        {
            var assets = await _db.CryptoAssets
                .Include(a => a.PriceHistory)
                .ToListAsync();

            var result = assets.Select(asset => {
                var history = asset.PriceHistory.OrderByDescending(p => p.Date).ToList();
                var latest = history.FirstOrDefault();
                var previous = history.Skip(1).FirstOrDefault();

                decimal? trend = null;
                if (latest != null && previous != null && previous.Price != 0)
                {
                    trend = (latest.Price - previous.Price) / previous.Price * 100;
                }

                return new {
                    asset.Name,
                    asset.Symbol,
                    asset.ExternalId,
                    asset.IconUrl,
                    CurrentPrice = latest?.Price ?? 0,
                    Currency = "USD",
                    LastUpdated = latest?.Date,
                    TrendPercentage = trend,
                    TrendIndicator = trend > 0 ? "up" : (trend < 0 ? "down" : "stable")
                };
            });

            return Ok(result);
        }
    }
}