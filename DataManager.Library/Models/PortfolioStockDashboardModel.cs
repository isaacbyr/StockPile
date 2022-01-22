using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models
{
    public class PortfolioStockDashboardModel
    {
        public string Ticker { get; set; }
        public int Shares { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
