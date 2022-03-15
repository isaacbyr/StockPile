CREATE PROCEDURE [dbo].[spPortfolio_GetAllByUserId]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Ticker, Shares, AveragePrice
	FROM [dbo].[Portfolio]
	WHERE UserId = @UserId
END
