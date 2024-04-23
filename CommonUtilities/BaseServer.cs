using StockProviderContract;
using StockProviderContract.DataContract;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SampleServiceProvider
{
    public abstract class BaseServer : IDataProvider, IDisposable
    {
        #region Server Runtime
        public BaseServer()
        {
            // Not handling customizable endpoints
            // Not handling additional stock customization
            Stocks.Add("Stock 1");
            Stocks.Add("Stock 2");
        }
        public void Initialize()
        {
            IPHostEntry entry = Dns.GetHostEntry("localhost");
            IPEndPoint endpoint = new IPEndPoint(entry.AddressList[0], 12900);
            _serverSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(endpoint);
            _serverSocket.Listen(100);

            // Accept clients
            new Thread(() =>
            {
                while (true)
                {
                    var client = _serverSocket.Accept();
                    _clients.Add(client);
                    _clientSubscriptions[client] = [];
                    Console.WriteLine("New client is connected.");
                    new Thread(() => ServerHandleClient(client)).Start();
                }
            }).Start();

            void ServerHandleClient(Socket client)
            {
                while (true)
                {
                    byte[] buffer = new byte[2014];
                    var size = client.Receive(buffer);
                    // Could receive multiple commands in a single buffer
                    var commands = Encoding.UTF8.GetString(buffer, 0, size);

                    foreach (var commandString in commands.Split('\n'))
                    {
                        string command = commandString.Split(' ').First();
                        string stockName = commandString.Substring(command.Length).Trim();
                        switch (command)
                        {
                            case "Subscribe":
                                Subscribe(client, stockName); break;
                            case "Unsubscribe":
                                Unsubscribe(client, stockName); break;
                        }
                    }
                }
            }

            Console.WriteLine($"Server initialized at {endpoint}.");
        }
        #endregion

        #region Properties
        private Socket? _serverSocket;
        private readonly List<Socket> _clients = [];
        private readonly Dictionary<Socket, HashSet<string>> _clientSubscriptions = [];

        public HashSet<string> Stocks = [];
        #endregion

        #region Data Provider Interface
        public void Subscribe(Socket client, string stock)
        {
            _clientSubscriptions[client].Add(stock);
        }
        public void Unsubscribe(Socket client, string stock)
        {
            _clientSubscriptions[client].Remove(stock);
        }
        #endregion

        #region Routines
        public abstract StockPrice FetchStockPrice(string stock);
        #endregion

        #region Serverside Push
        protected virtual void NotifyAllClients(StockPrice newPrice)
        {
            // Loop through clients and push notification
            byte[] bytes = newPrice.Serialize();
            foreach (Socket client in _clients)
                if (_clientSubscriptions[client].Contains(newPrice.StockName))
                    client.Send(bytes);
        }
        #endregion

        #region Routine Activities
        public virtual void Start()
        {
            // Emulates periodic behavior
            while (true)
            {
                foreach (string stock in Stocks)
                {
                    // Might want to book-keep the time
                    StockPrice newPrice = FetchStockPrice(stock);
                    NotifyAllClients(newPrice);
                }
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region Disposable Interface
        public void Dispose() 
            => _serverSocket?.Shutdown(SocketShutdown.Both);
        #endregion
    }
}
