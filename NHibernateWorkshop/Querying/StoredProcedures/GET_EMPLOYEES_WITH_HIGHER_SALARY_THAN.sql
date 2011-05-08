SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE GET_EMPLOYEES_WITH_HIGHER_SALARY_THAN
	@employeeId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Id]
		  ,[FirstName]
		  ,[LastName]
		  ,[Title]
		  ,[BirthDate]
		  ,[HireDate]
		  ,[Phone]
		  ,[Salary]
		  ,[Street]
		  ,[City]
		  ,[PostalCode]
		  ,[Country]
		  ,[ManagerId]
	FROM [Employee]
	WHERE [Salary] > (SELECT [Salary] FROM [Employee] WHERE [Id] = @employeeId);
END
GO