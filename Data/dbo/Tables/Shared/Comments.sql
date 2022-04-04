﻿CREATE TABLE [dbo].[Comments]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UserId] NVARCHAR(128) NOT NULL,
	[Ticker] NVARCHAR(10) NOT NULL,
	[PostedAt] DATETIME2 NOT NULL DEFAULT getutcdate(),
	[Comment] NVARCHAR(256) NOT NULL
)