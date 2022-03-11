CREATE TABLE [dbo].[StrategyItem]
(
	
	--[StrategyId] INT NOT NULL
	--[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1)
	[Id] INT NOT NULL,
	[Ticker] NVARCHAR(10) NOT NULL,
	[BuyShares] INT NOT NULL,
	[SellShares] INT NOT NULL,
	[ProfitLoss] MONEY NOT NULL
)
