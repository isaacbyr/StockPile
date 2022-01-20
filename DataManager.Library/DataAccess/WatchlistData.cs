using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class WatchlistData
    {
        public List<WatchlistModel> LoadWatchlistStocksById(string id)
        {
            var sql = new SqlDataAccess();

            // TODO: Change hardcode of user id
            var p = new {UserId = "34b965a6-ba23-4a13-b834-1e456f21d86c" };

            try
            {
                var output = sql.LoadData<WatchlistModel, dynamic>("dbo.spWatchlist_LoadAllById", p, "StockPileData");
                return output;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
