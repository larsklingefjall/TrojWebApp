USE [d000540]
GO

INSERT INTO Pages3 ([Title],[Controller],[Action],[Tip],[Position],[HasChild],[Changed],[ChangedBy])
SELECT [Title],[Controller],[Action],[Tip],[Position],[HasChild],[Changed],[ChangedBy] FROM Pages;

