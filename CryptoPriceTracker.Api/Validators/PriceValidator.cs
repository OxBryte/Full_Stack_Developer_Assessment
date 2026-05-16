using CryptoPriceTracker.Api.Models;

namespace CryptoPriceTracker.Api.Validators;

public class PriceValidator
{
    /// <summary>
    /// Decision: We prevent duplicates by checking if the exact same price has already been recorded for the same calendar day.
    /// This avoids cluttering the history with redundant data points if the sync is triggered multiple times without price movement.
    /// </summary>
    public bool ShouldSavePrice(decimal newPrice, DateTime date, List<CryptoPriceHistory> history)
    {
        if (newPrice <= 0) return false;
        return !history.Any(h => h.Date.Date == date.Date && h.Price == newPrice);
    }
}