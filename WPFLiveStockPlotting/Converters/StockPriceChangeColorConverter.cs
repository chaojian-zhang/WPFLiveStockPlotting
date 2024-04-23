using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace WPFLiveStockPlotting.Converters
{
    [ValueConversion(typeof(double), typeof(SolidColorBrush))]
    internal class StockPriceChangeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Colors.White);

            double doubleValue = (double)value;
            if (doubleValue == 0)
                return new SolidColorBrush(Colors.White);
            else if (doubleValue > 1)
                return new SolidColorBrush(Colors.Green);
            else
                return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
