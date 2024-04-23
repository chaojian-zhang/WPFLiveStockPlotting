using SampleServiceProvider;
using StockProviderContract.DataContract;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WPFLiveStockPlotting
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            // Start client
            var client = new BaseClient();
            client.StockPriceReceived += StockPriceReceived;
            client.Initialize();
            client.Subscribe("Stock 1");
            client.Subscribe("Stock 2");
            StockPrices["Stock 1"] = [];
            StockPrices["Stock 2"] = [];
        }
        #endregion

        #region Public View Properties
        private Dictionary<string, ObservableCollection<StockPrice>>? _StockPrices = [];
        public Dictionary<string, ObservableCollection<StockPrice>>? StockPrices { get => _StockPrices; set => SetField(ref _StockPrices, value); }

        private double? _stock1Price;
        public double? Stock1Price { get => _stock1Price; set => SetField(ref _stock1Price, value); }
        private double? _stock2Price;
        public double? Stock2Price { get => _stock2Price; set => SetField(ref _stock2Price, value); }
        private double? _stock1PriceChange;
        public double? Stock1PriceChange { get => _stock1PriceChange; set => SetField(ref _stock1PriceChange, value); }
        private double? _stock2PriceChange;
        public double? Stock2PriceChange { get => _stock2PriceChange; set => SetField(ref _stock2PriceChange, value); }

        private ObservableCollection<StockPrice>? _currentSelectedHistory = null;
        public ObservableCollection<StockPrice>? CurrentSelectedHistory { get => _currentSelectedHistory; set => SetField(ref _currentSelectedHistory, value); }
        #endregion

        #region Data Binding
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<TType>(ref TType field, TType value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<TType>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region Events
        public void StockPriceReceived(StockPrice price)
        {
            Dispatcher.BeginInvoke(() =>
            {
                StockPrices![price.StockName].Add(price);
                switch (price.StockName)
                {
                    case "Stock 1":
                        Stock1PriceChange = price.Price - (Stock1Price == null ? price.Price : Stock1Price.Value);
                        Stock1Price = price.Price;
                        break;
                    case "Stock 2":
                        Stock2PriceChange = price.Price - (Stock2Price == null ? price.Price : Stock2Price.Value);
                        Stock2Price = price.Price;
                        break;
                    default:
                        break;
                }
            });
        }
        private void Border1_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Handle double-click
            if (e.ClickCount == 2)
            {
                DisplayHistory("Stock 1");
                e.Handled = true;
            }
        }
        private void Border2_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Handle double-click
            if (e.ClickCount == 2)
            {
                DisplayHistory("Stock 2");
                e.Handled = true;
            }
        }
        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Handle double-click
            if (e.ClickCount == 2)
            {
                HideHistory();
                e.Handled = true;
            }
        }
        #endregion

        #region Routines
        private void DisplayHistory(string name)
        {
            HistoryDataGrid.Visibility = Visibility.Visible;
            Panels.Visibility = Visibility.Collapsed;

            CurrentSelectedHistory = StockPrices[name];
        }
        private void HideHistory()
        {
            HistoryDataGrid.Visibility = Visibility.Collapsed;
            Panels.Visibility = Visibility.Visible;
        }
        #endregion
    }
}