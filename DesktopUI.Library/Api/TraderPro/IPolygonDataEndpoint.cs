using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public interface IPolygonDataEndpoint
    {
        Task<(List<double>, List<double>, List<double>, List<double>)> LoadTradeData(string ticker, string timestamp);    }
}