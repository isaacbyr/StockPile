CREATE TABLE [dbo].[UserAccount]
(
	[UserId] NVARCHAR(128) NOT NULL PRIMARY KEY,
	[StartAmount] MONEY NOT NULL,
	[PortfolioAccountBalance] MONEY NOT NULL, 
	[TradesAccountBalance] MONEY NOT NULL,
)
