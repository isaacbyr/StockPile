using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models
{
    public class PortfolioStockDisplayModel
    {
        public string Ticker { get; set; }
        public decimal Price { get; set; }
        public decimal ProfitLoss { get; set; }
        public int Shares { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
