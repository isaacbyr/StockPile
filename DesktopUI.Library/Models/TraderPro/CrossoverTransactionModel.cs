using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models.TraderPro
{
    public class CrossoverTransactionModel
    {
        public int Index { get; set; }
        public string Date { get; set; }
        public string BuyOrSell { get; set; }
        public decimal Price { get; set; }
        public int Shares { get; set; }
    }
}
