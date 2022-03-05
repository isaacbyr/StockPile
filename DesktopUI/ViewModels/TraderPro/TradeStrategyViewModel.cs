using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels.TraderPro
{
    public class TradeStrategyViewModel: Screen
    {
        public string Ticker { get; set; }
        public int BuyShares { get; set; }
        public int SellShares { get; set; }
        public string MA1 { get; set; }
        public string MA2 { get; set; }
        public string Indicator { get; set; }
        public string Interval { get; set; }
        public string Range { get; set; }

        public TradeStrategyViewModel(string ticker, int buyShares, int sellShares, string ma1, string ma2, string indicator, string interval, string range)
        {
            Ticker = ticker;
            BuyShares = buyShares;
            SellShares = sellShares;
            MA1 = ma1;
            MA2 = ma2;
            Indicator = indicator;
            Interval = interval;
            Range = range;
        }
    }
}
