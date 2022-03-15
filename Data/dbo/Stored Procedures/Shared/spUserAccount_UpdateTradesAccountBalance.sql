CREATE PROCEDURE [dbo].[spUserAccount_UpdateTradesAccountBalance]
	@UserId NVARCHAR(128),
	@CashAmount MONEY
AS
BEGIN
	UPDATE [dbo].[UserAccount]
	SET TradesAccountBalance = TradesAccountBalance - @CashAmount
	WHERE UserId = @UserId
END
