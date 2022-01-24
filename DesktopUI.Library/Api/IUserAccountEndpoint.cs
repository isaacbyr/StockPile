﻿using DesktopUI.Library.Models;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IUserAccountEndpoint
    {
        Task<UserPortfolioOverviewModel> GetPortfolioOverview();
    }
}