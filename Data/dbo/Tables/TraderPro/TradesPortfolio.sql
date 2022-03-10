CREATE TABLE [dbo].[TradesPortfolio]
(
	[UserId] NVARCHAR(128) NOT NULL,
	[Ticker] NVARCHAR(10) NOT NULL,
	[Shares] INT NOT NULL,
	[AveragePrice] MONEY NOT NULL
)
