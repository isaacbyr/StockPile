using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class UserData
    {
        public List<LoggedInUserModel> GetUserById(string id)
        {
            var sql = new SqlDataAccess();

            var p = new { Id = id };

            try
            {
                var output = sql.LoadData<LoggedInUserModel, dynamic>("dbo.spUser_GetUserById", p, "StockPileData");

                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public List<FriendModel> FriendSearch(string keyword)
        {
            var sql = new SqlDataAccess();

            var p = new { Keyword = keyword };

            try
            {
                var output = sql.LoadData<FriendModel, dynamic>("dbo.spUser_FriendSearch", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
