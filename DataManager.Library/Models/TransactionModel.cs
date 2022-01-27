using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models
{
    public class TransactionModel
    {
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public int Shares { get; set; }
        public string Ticker { get; set; }
        public bool Buy { get; set; }
        public bool Sell { get; set; }
        public decimal Price { get; set; }
    }
}
