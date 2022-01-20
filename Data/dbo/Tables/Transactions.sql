CREATE TABLE [dbo].[Transactions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UserId] NVARCHAR(128) NOT NULL,
	[Ticker] NVARCHAR(10) NOT NULL,
	[StockName] NVARCHAR(50) NOT NULL,
	[Buy] BIT NOT NULL,
	[Sell] BIT NOT NULL,
	[BuyPrice] MONEY,
	[SellPrice] MONEY, 
    CONSTRAINT [FK_Transactions_UserAccount] FOREIGN KEY ([UserId]) REFERENCES [UserAccount]([UserId]),
)
