CREATE TABLE [dbo].[Portfolio]
(
	[UserId] NVARCHAR(128) NOT NULL PRIMARY KEY,
	[StockName] NVARCHAR(50) NOT NULL,
	[Ticker] NVARCHAR(10) NOT NULL,
	[AmountInvesteed] MONEY NOT NULL,
	[AveragePrice] MONEY NOT NULL, 
)
