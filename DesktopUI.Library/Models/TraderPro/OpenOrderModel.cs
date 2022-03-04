using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models.TraderPro
{
    public class OpenOrderModel
    {
        public string Time { get; set; }
        public int Id { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public double Trigger { get; set; }
        public int Shares { get; set; }
        public string Side { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Fill { get; set; }

    }
}
