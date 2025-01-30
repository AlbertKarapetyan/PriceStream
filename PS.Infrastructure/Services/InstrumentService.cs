using PS.Domain.Interfaces;

namespace PS.Infrastructure.Services
{
    /// <summary>
    /// Provides instrument-related data, including available instruments and their prices.
    /// </summary>
    public class InstrumentService : IInstrumentService
    {
        // Predefined list of supported trading instruments
        private static readonly List<string> Instruments = new() { "EURUSDT", "USDTJPY", "BTCUSDT" };

        /// <summary>
        /// Retrieves the list of available instruments.
        /// </summary>
        /// <returns>List of instrument symbols.</returns>
        public List<string> GetInstruments()
        {
            return Instruments;
        }

        /// <summary>
        /// Retrieves the latest price of the specified instrument.
        /// </summary>
        /// <param name="instrument">The instrument symbol.</param>
        /// <returns>The latest price of the instrument.</returns>
        public decimal GetPrice(string instrument)
        {
            return PriceCache.GetPrice(instrument);
        }
    }
}
