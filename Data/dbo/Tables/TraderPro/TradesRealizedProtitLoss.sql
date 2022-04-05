CREATE TABLE [dbo].[TradesRealizedProtitLoss]
(
	[UserId] NVARCHAR(128) NOT NULL,
	[ProfitLoss] MONEY NOT NULL,
	[Date] DATETIME2 DEFAULT getutcdate(),
	[TotalRealized] MONEY,

)
