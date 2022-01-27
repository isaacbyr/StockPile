CREATE TABLE [dbo].[RealizedProfitLoss]
(
	[UserId] NVARCHAR(128) NOT NULL,
	[ProfitLoss] MONEY NOT NULL,
	[Date] DATETIME2 NOT NULL DEFAULT getutcdate(),
	[TotalRealized] MONEY NOT NULL
)