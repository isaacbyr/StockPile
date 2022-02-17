using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models.TraderPro
{
    public class PaperTradeModel
    {
        public string BuyOrSell { get; set; }
        public double Price { get; set; }
        public int Shares { get; set; }
    }
}
