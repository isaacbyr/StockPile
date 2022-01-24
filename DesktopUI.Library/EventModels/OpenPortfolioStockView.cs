using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.EventModels
{
    public class OpenPortfolioStockView
    {
        public string UserId { get; set; }
        public string Ticker { get; set; }

        public OpenPortfolioStockView(string ticker)
        {
            Ticker = ticker;
        }

        public OpenPortfolioStockView(string id, string ticker)
        {
            UserId = id;
        }
    }
}
