namespace PS.Domain.Interfaces
{
    public interface IInstrumentService
    {
        List<string> GetInstruments();
        decimal GetPrice(string instrument);
    }
}
