using DesktopUI.Library.Models;
using DesktopUI.Library.Models.TraderPro;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public interface IStrategyEndpoint
    {
        Task<ResponseModel> PostStrategy(StrategyModel strategy);
    }
}