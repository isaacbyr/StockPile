using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DesktopUI.Converters
{
    public class PortfolioItemColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stock = (PortfolioStockDisplayModel)value;

            if(stock.ProfitLoss < 0)
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
