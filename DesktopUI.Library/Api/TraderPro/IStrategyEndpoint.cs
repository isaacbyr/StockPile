﻿using DesktopUI.Library.Models;
using DesktopUI.Library.Models.TraderPro;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public interface IStrategyEndpoint
    {
        Task<int> PostStrategy(StrategyModel strategy);
        Task<ResponseModel> PostStrategyStock(StrategyItemModel strategyItem);
        Task<List<StrategyModel>> LoadStrategies();
        Task<List<StrategyItemModel>> GetStrategyStocks(int id);
    }
}