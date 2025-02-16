USE [d000540]
GO

INSERT INTO SubPages3 ([PageId],[Title],[Controller],[Action],[Tip],[Position],[IsVisible],[Changed],[ChangedBy])
SELECT [PageId],[Title],[Controller],[FileName],[Tip],[Position],[IsVisible],[Changed],[ChangedBy] FROM [d000540].[dbo].[SubPages]
 WHERE Version = 3 AND PageId < 13
 ORDER BY PageId, Position
 
INSERT INTO SubPages3 ([PageId],[Title],[Controller],[Action],[Tip],[Position],[IsVisible],[Changed],[ChangedBy])
SELECT 11 AS [PageId],[Title],[Controller],[FileName],[Tip],[Position],[IsVisible],[Changed],[ChangedBy] FROM [d000540].[dbo].[SubPages]
 WHERE Version = 3 AND PageId >= 13
 ORDER BY PageId, Position
 
 