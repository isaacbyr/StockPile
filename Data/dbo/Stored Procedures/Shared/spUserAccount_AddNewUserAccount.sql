CREATE PROCEDURE [dbo].[spUserAccount_AddNewUserAccount]
	@UserId NVARCHAR(128),
	@StartAmount MONEY,
	@AccountBalance MONEY
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[UserAccount] (UserId, StartAmount, PortfolioAccountBalance)
	Values (@UserId, @StartAmount, @AccountBalance)
END
