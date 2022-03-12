CREATE TABLE [dbo].[TWSTrading]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Ticker] NVARCHAR(10) NOT NULL,
	[BuyShares] INT NOT NULL,
	[SellShares] INT NOT NULL,
	[BuyPrice] MONEY,
	[SellPrice] MONEY,
	[ProfitLoss] MONEY,
)
