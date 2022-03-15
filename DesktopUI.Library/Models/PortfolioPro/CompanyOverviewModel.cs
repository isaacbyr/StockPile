using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models
{
    public class CompanyOverviewModel
    {
        public string Symbol { get; set; }
        public string Sector { get; set; }
        public string MarketCapitalization { get; set; }
        public string EBITDA { get; set; }
        public string PERatio { get; set; }
        public string PEGRatio { get; set; }
        public string DividendYield { get; set; }
        public string EPS { get; set; }
        public string Beta { get; set; }
        public string TrailingPE { get; set; }
        public string ForwardPE { get; set; }
        public string SharesOutstanding { get; set; }

    }
}
