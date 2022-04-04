using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models.Shared
{
    public class NewCommentModel
    {
        public string Ticker { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
    }
}
