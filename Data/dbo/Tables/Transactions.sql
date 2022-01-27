﻿CREATE TABLE [dbo].[Transactions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Date] DATETIME2 NOT NULL DEFAULT getutcdate(),
	[UserId] NVARCHAR(128) NOT NULL,
	[Ticker] NVARCHAR(10) NOT NULL,
	[Shares] INT NOT NULL,
	[Buy] BIT NOT NULL,
	[Sell] BIT NOT NULL,
	[Price] MONEY,
)
