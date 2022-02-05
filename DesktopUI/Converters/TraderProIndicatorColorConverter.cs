using DesktopUI.Library.Models.TraderPro;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DesktopUI.Converters
{
    public class TraderProIndicatorColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var indicator = (IndicatorDisplayModel)value;

            //convert color form string to SolidColorBrush
            Color newColor = (Color)ColorConverter.ConvertFromString(indicator.Color);
            SolidColorBrush brush = new SolidColorBrush(newColor);

            if (indicator.Color == "")
            {
                return "White";
            }
            else
            {
                return brush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
