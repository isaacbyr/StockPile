using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models.TraderPro
{
    public class StrategyStockModel
    {
        public int Id { get; set; }
        public string Ticker { get; set; }
        public int BuyShares { get; set; }
        public int SellShares { get; set; }
        public decimal ProfitLoss { get; set; }
    }
}
