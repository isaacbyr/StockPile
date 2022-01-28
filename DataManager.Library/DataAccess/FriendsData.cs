using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class FriendsData
    {
        public List<FriendModel> LoadFriends(string id)
        {
            var sql = new SqlDataAccess();

            var p = new { FolloweeId = id };

            try
            {
                var output = sql.LoadData<FriendModel, dynamic>("dbo.spFriends_LoadFriendsById", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void PostFriendship(string followeeId, string followerId)
        {
            var sql = new SqlDataAccess();

            var p = new { FolloweeId = followeeId, FollowerId = followerId };

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spFriends_PostFriendship", p);
                sql.CommitTransaction();
            }
            catch (Exception e)
            {
                sql.RollbackTransaction();
                throw new Exception(e.Message);
            }
        }
    }
}
