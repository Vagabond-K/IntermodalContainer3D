using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IntermodalContainers
{
    public class StripeViewportSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2
                && values[0] is double width
                && values[1] is double height
                && parameter is double pitch)
            {
                return new Rect(0, 0, width / Math.Ceiling(width / pitch), height);
            }
            return new Rect();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
