using System.Text.Json.Serialization;

namespace PS.Domain.Models
{
    public class PriceUpdate
    {
        // Helper property to get the instrument name (e.g., "BTCUSDT")
        public string Instrument => Symbol;

        [JsonPropertyName("e")]
        public string EventType { get; set; }

        [JsonPropertyName("E")]
        public long EventTime { get; set; }

        [JsonPropertyName("s")]
        public string Symbol { get; set; }

        [JsonPropertyName("a")]
        public long AggregateTradeId { get; set; }

        [JsonPropertyName("p")]
        public string Price { get; set; }

        [JsonPropertyName("q")]
        public string Quantity { get; set; }

        [JsonPropertyName("f")]
        public long FirstTradeId { get; set; }

        [JsonPropertyName("l")]
        public long LastTradeId { get; set; }

        [JsonPropertyName("T")]
        public long TradeTime { get; set; }

        [JsonPropertyName("m")]
        public bool IsMarketMaker { get; set; }

        [JsonPropertyName("M")]
        public bool Ignore { get; set; }
    }
}
