CREATE TABLE [dbo].[Portfolio]
(
	[UserId] NVARCHAR(128) NOT NULL,
	[Ticker] NVARCHAR(10) NOT NULL,
	[Shares] INT NOT NULL,
	[AveragePrice] MONEY NOT NULL, 
)
