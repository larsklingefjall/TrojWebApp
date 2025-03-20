USE [d002883]
GO

UPDATE [dbo].[PageUsers3]
   SET [EmployeeId] = 6
 WHERE [EmployeeId] = 3
GO


UPDATE [dbo].[SubPageUsers3]
   SET [EmployeeId] = 6
 WHERE [EmployeeId] = 3
GO


UPDATE [dbo].[PageUsers3]
   SET [EmployeeId] = 5
 WHERE [EmployeeId] = 4
GO

UPDATE [dbo].[SubPageUsers3]
   SET [EmployeeId] = 5
 WHERE [EmployeeId] = 4
GO

