using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models
{
    public class UserPortfolioOverviewModel
    {
        public decimal StartAmount { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal RealizedProfitLoss { get; set; }
    }
}
