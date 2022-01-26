CREATE PROCEDURE [dbo].[spTransaction_PostTransaction]
	@UserId NVARCHAR(128),
	@Shares INT,
	@Buy BIT,
	@Sell BIT,
	@Price MONEY,
	@Ticker NVARCHAR(10)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[Transactions] (UserId, Shares, Buy, Sell, Price, Ticker)
	VALUES (@UserId, @Shares, @Buy, @Sell, @Price, @Ticker)
END
