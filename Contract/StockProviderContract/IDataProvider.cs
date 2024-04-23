using System.Net.Sockets;

namespace StockProviderContract
{
    public interface IDataProvider
    {
        public void Subscribe(Socket client, string stock);
        public void Unsubscribe(Socket client, string stock);
    }
}
