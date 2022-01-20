using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models
{
    public class StockDashboardDataModel
    {
        public string Ticker { get; set; }
        public string MarketPrice { get; set; }
        public string PercentChanged { get; set; }
    }
}
