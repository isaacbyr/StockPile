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
        public decimal MarketPrice { get; set; }
        public decimal PercentChanged { get; set; }
    }
}
