using System.Collections.Concurrent;

namespace PS.Infrastructure.Services
{
    /// <summary>
    /// Provides a thread-safe cache for storing and retrieving instrument prices.
    /// </summary>
    public static class PriceCache
    {
        // Thread-safe dictionary to store instrument prices
        private static readonly ConcurrentDictionary<string, decimal> _prices = new();

        /// <summary>
        /// Updates the price of a given instrument in the cache.
        /// </summary>
        /// <param name="instrument">The instrument symbol (e.g., "EURUSD").</param>
        /// <param name="price">The latest price of the instrument.</param>
        public static void UpdatePrice(string instrument, decimal price)
        {
            _prices.AddOrUpdate(instrument, price, (key, oldValue) => price);
        }

        /// <summary>
        /// Retrieves the latest price of the specified instrument.
        /// </summary>
        /// <param name="instrument">The instrument symbol.</param>
        /// <returns>The latest price if available; otherwise, returns 0.</returns>
        public static decimal GetPrice(string instrument)
        {
            return _prices.TryGetValue(instrument, out var price) ? price : 0;
        }
    }
}
