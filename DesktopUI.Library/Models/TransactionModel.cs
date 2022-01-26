using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models
{
    public class TransactionModel
    {
        public string Ticker { get; set; }
        public int Shares { get; set; }
        public bool Buy { get; set; }
        public bool Sell { get; set; }
        public decimal Price { get; set; }
    }
}
