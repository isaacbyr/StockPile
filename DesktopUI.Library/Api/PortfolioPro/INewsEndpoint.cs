using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface INewsEndpoint
    {
        Task<List<NewsArticleModel>> GetMarketNews(string query);
    }
}