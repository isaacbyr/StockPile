using DesktopUI.Library.Models.TraderPro;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public interface ITWSTradingEndpoint
    {
        Task PostTWSStategy(TWSTradeModel trade);
        Task<List<TWSTradeModel>> LoadStrategies();
    }
}