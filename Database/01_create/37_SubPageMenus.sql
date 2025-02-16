USE [d000540]
GO

/****** Object:  Table [dbo].[SubPageMenus]    Script Date: 2025-02-17 00:23:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SubPageMenus](
	[SubPageMenuId] [int] IDENTITY(1,1) NOT NULL,
	[ParentPageId] [int] NOT NULL,
	[ChildPageId] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
	[Position] [int] NULL,
 CONSTRAINT [PK_SubPageMenus] PRIMARY KEY CLUSTERED 
(
	[SubPageMenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SubPageMenus]  WITH CHECK ADD  CONSTRAINT [FK_SubPageMenu_ChildSubPages3] FOREIGN KEY([ChildPageId])
REFERENCES [dbo].[SubPages3] ([SubPageId])
GO

ALTER TABLE [dbo].[SubPageMenus] CHECK CONSTRAINT [FK_SubPageMenu_ChildSubPages3]
GO

ALTER TABLE [dbo].[SubPageMenus]  WITH CHECK ADD  CONSTRAINT [FK_SubPageMenu_ParentSubPages3] FOREIGN KEY([ParentPageId])
REFERENCES [dbo].[SubPages3] ([SubPageId])
GO

ALTER TABLE [dbo].[SubPageMenus] CHECK CONSTRAINT [FK_SubPageMenu_ParentSubPages3]
GO


