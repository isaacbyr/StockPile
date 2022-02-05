CREATE PROCEDURE [dbo].[spStrategy_PostStrategy]
	@UserId NVARCHAR(128),
	@Name NVARCHAR(100),
	@Ticker NVARCHAR(10),
	@ProfitLoss MONEY
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].Strategies (UserId, [Name], Ticker,  ProfitLoss)
	VALUES (@UserId, @Name, @Ticker, @ProfitLoss)
END