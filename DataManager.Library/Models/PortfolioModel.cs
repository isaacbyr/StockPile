using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models
{
    public class PortfolioModel
    {
        public string UserId { get; set; }
        public string Ticker { get; set; }
        public decimal Price { get; set; }
        public int Shares { get; set; }
    }
}
