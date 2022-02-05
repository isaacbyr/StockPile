using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DesktopUI.Converters
{
    public class TraderProPLColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pl = value.ToString();
            pl = pl.Substring(1);

            decimal convPL;
            decimal.TryParse(pl, out convPL);

            if(convPL > 0)
            {
                return "#03AC13";
            }
            else
            {
                return "Red";
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
