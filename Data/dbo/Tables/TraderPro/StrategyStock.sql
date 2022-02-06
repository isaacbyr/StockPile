CREATE TABLE [dbo].[StrategyStock]
(
	[Id] INT NOT NULL,
	[Ticker] NVARCHAR(10) NOT NULL,
	[BuyShares] INT NOT NULL,
	[SellShares] INT NOT NULL,
	[ProfitLoss] MONEY NOT NULL
)
