CREATE PROCEDURE [dbo].[spStrategy_GetStrategyId]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT TOP 1 Id
	FROM [dbo].Strategies
	Order By Id DESC
END
