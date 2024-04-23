using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace WPFLiveStockPlotting.Converters
{
    [ValueConversion(typeof(double), typeof(String))]
    internal class StockPriceDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            double doubleValue = (double)value;
            return $"{doubleValue:N2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = (string)value;
            double resultValue;
            if (double.TryParse(strValue, out resultValue))
                return resultValue;
            return DependencyProperty.UnsetValue;
        }
    }
}
