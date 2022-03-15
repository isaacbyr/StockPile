using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models
{
    public class UpdateUserAccountModel
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal RealizedProfitLoss { get; set; }
    }
}
