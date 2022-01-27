using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models
{
    public class RealizedPLChartModel
    {
        public DateTime Date { get; set; }
        public decimal TotalRealized { get; set; }
        public decimal ProfitLoss { get; set; }

    }
}
