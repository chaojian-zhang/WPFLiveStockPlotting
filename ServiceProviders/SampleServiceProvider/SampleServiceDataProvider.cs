using StockProviderContract.DataContract;

namespace SampleServiceProvider
{
    public sealed class SampleServiceDataProvider : BaseServer
    {
        #region Custom Implementations
        public override StockPrice FetchStockPrice(string stock)
        {
            switch (stock)
            {
                case "Stock 1":
                    return new StockPrice(stock, DateTime.Now, GenerateRandomPrice(240, 270));
                case "Stock 2":
                    return new StockPrice(stock, DateTime.Now, GenerateRandomPrice(180, 210));
                default:
                    throw new ArgumentException();
            }
        }
        #endregion

        #region Generator
        private double GenerateRandomPrice(double min, double max)
        {
            return new Random().NextDouble() * (max - min) + min;
        }
        #endregion
    }
}
