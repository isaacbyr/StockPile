using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models
{
    public class LeaderboardDisplayModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public decimal WinningPercentage { get; set; }
        public decimal ProfitLoss { get; set; }
    }
}
