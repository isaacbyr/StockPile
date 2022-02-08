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
        public string MA1 { get; set; }
        public string MA2 { get; set; }
        public string Indicator { get; set; }
        public string Interval { get; set; }
        public string Range { get; set; }
    }
}
