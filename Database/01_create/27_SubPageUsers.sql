SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SubPageUsers](
	[SubPageUserId] [int] IDENTITY(1,1) NOT NULL,
	[SubPageId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
 CONSTRAINT [PK_SubPageUsers] PRIMARY KEY CLUSTERED 
(
	[SubPageUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SubPageUsers]  WITH CHECK ADD  CONSTRAINT [FK_SubPageUsers_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[SubPageUsers] CHECK CONSTRAINT [FK_SubPageUsers_Employees]
GO

ALTER TABLE [dbo].[SubPageUsers]  WITH CHECK ADD  CONSTRAINT [FK_SubPageUsers_SubPages] FOREIGN KEY([SubPageId])
REFERENCES [dbo].[SubPages] ([SubPageId])
GO

ALTER TABLE [dbo].[SubPageUsers] CHECK CONSTRAINT [FK_SubPageUsers_SubPages]
GO


