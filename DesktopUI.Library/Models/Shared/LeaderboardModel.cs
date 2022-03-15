using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models
{
    public class LeaderboardModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public decimal ProfitLoss { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalRealized { get; set; }
    }
}
