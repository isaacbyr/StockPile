CREATE PROCEDURE [dbo].[spStrategyItem_Post]
	@Id INT,
	@Ticker NVARCHAR(10),
	@BuyShares INT,
	@SellShares INT,
	@ProfitLoss MONEY
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[StrategyItem] (Id, Ticker, BuyShares, SellShares, ProfitLoss)
	VALUES (@Id, @Ticker, @BuyShares, @SellShares, @ProfitLoss)
END