namespace PS.Domain.Interfaces
{
    public interface IExchangeWebSocketService
    {
        Task ConnectAndSubscribe();
        Task Disconnect();
    }
}
