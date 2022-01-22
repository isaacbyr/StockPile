using DesktopUI.Library.Models;
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
    public class WatchlistItemColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StockDashboardDataModel stock = (StockDashboardDataModel)value; 
            decimal temp = 1;
            decimal.TryParse(stock.PercentChanged, out temp);

            if (temp < 0)
            {
                return "Red";
            }
            else
            {
                return "#03AC13";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
