using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models.TraderPro
{
    public class StrategyItemModel
    {
        public int Id { get; set; }
        public string Ticker { get; set; }
        public int BuyShares { get; set; }
        public int SellShares { get; set; }
        public decimal ProfitLoss { get; set; }
    }
}
