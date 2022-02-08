using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models.TraderPro
{
    public class StrategyModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public decimal ProfitLoss { get; set; }
    }
}
