CREATE TABLE [dbo].[UserAccount]
(
	[UserId] NVARCHAR(128) NOT NULL PRIMARY KEY,
	[StartAmount] MONEY NOT NULL,
	[AccountBalance] MONEY NOT NULL, 
    [RealizedProfitLoss] MONEY NOT NULL DEFAULT 0
)
