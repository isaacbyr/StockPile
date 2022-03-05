CREATE PROCEDURE [dbo].[spStrategy_GetAllStrategyStocks]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM [dbo].[StrategyItem]
	WHERE Id = @Id
END

