using Xunit;
using CryptoPriceTracker.Api.Validators;
using CryptoPriceTracker.Api.Models;
using System;
using System.Collections.Generic;

namespace CryptoPriceTracker.Tests
{
    public class PriceValidatorTests
    {
        private readonly PriceValidator _validator;

        public PriceValidatorTests()
        {
            _validator = new PriceValidator();
        }

        [Fact]
        public void ShouldSavePrice_ReturnsFalse_WhenPriceIsZero()
        {
            // Arrange
            decimal price = 0;
            var date = DateTime.UtcNow;
            var history = new List<CryptoPriceHistory>();

            // Act
            var result = _validator.ShouldSavePrice(price, date, history);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldSavePrice_ReturnsFalse_WhenPriceIsNegative()
        {
            // Arrange
            decimal price = -100;
            var date = DateTime.UtcNow;
            var history = new List<CryptoPriceHistory>();

            // Act
            var result = _validator.ShouldSavePrice(price, date, history);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldSavePrice_ReturnsFalse_WhenSamePriceExistsForSameDay()
        {
            // Arrange
            var date = DateTime.UtcNow;
            decimal price = 50000;
            var history = new List<CryptoPriceHistory>
            {
                new CryptoPriceHistory { Date = date, Price = price }
            };

            // Act
            var result = _validator.ShouldSavePrice(price, date, history);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldSavePrice_ReturnsTrue_WhenPriceIsDifferent()
        {
            // Arrange
            var date = DateTime.UtcNow;
            decimal oldPrice = 50000;
            decimal newPrice = 51000;
            var history = new List<CryptoPriceHistory>
            {
                new CryptoPriceHistory { Date = date, Price = oldPrice }
            };

            // Act
            var result = _validator.ShouldSavePrice(newPrice, date, history);

            // Assert
            Assert.True(result);
        }
    }
}
