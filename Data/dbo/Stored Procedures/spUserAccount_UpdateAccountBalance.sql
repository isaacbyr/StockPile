CREATE PROCEDURE [dbo].[spUserAccount_UpdateAccountBalance]
	@UserId NVARCHAR(128),
	@CashAmount MONEY
AS
BEGIN
	UPDATE [dbo].[UserAccount]
	SET AccountBalance = AccountBalance - @CashAmount
	WHERE UserId = @UserId
END
