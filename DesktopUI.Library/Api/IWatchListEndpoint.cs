using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IWatchListEndpoint
    {
        Task<List<WatchlistModel>> LoadWatchList();
        Task<ResponseModel> PostWatchlistStock(string ticker);
    }
}