using System.Net.Sockets;
using System.Net;
using System.Text;
using StockProviderContract.DataContract;

namespace SampleServiceProvider
{
    public class BaseClient : IDisposable
    {
        #region Client Runtime
        public BaseClient()
        {
            // Not handling customizable endpoints
        }
        public void Initialize()
        {
            IPHostEntry entry = Dns.GetHostEntry("localhost");
            IPEndPoint endpoint = new IPEndPoint(entry.AddressList[0], 12900);
            _clientStock = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _clientStock.Connect(endpoint);

            // Handle server pushing events
            new Thread(() => ClientReceiveMessage(_clientStock)).Start();

            void ClientReceiveMessage(Socket socket)
            {
                while (true)
                {
                    byte[] buffer = new byte[2014]; // Remark: Similar to client size message throttling, there might be segmentation issue that need to be handled
                    int size = socket.Receive(buffer);
                    StockPrice price = StockPrice.Deserialize(buffer);
                    StockPriceReceived?.Invoke(price);
                }
            }
        }
        #endregion

        #region Properties
        private Socket? _clientStock;
        #endregion

        #region Events
        public Action<StockPrice> StockPriceReceived;
        #endregion

        #region Methods
        public void Subscribe(string stock)
        {
            string commandString = $"Subscribe {stock}\n"; // Not handling advanced formatting
            byte[] data = Encoding.UTF8.GetBytes(commandString);
            _clientStock.Send(data);
        }
        public void Unsubscribe(string stock)
        {
            string commandString = $"Unsubscribe {stock}\n"; // Not handling advanced formatting
            byte[] data = Encoding.UTF8.GetBytes(commandString);
            _clientStock.Send(data);
        }
        #endregion

        #region Disposable Interface
        public void Dispose()
        {
            _clientStock.Shutdown(SocketShutdown.Both);
        }
        #endregion
    }
}
