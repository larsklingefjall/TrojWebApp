USE [d000540]
GO

INSERT INTO SubPageUsers3 ([SubPageId],[EmployeeId],[Changed],[ChangedBy])
SELECT [SubPageId],3 AS [EmployeeId],[Changed],[ChangedBy] FROM [d000540].[dbo].[SubPages3]

 