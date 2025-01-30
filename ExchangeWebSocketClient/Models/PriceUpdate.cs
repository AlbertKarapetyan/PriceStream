namespace ExchangeWebSocketClient.Models
{
    public class PriceUpdate
    {
        public required string Instrument { get; set; } // "BTCUSDT"
        public required string Price { get; set; } // Price of the instrument
    }
}
