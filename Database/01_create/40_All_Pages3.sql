USE [d002883]
GO

DROP TABLE [dbo].[SubPageUsers3]
DROP TABLE [dbo].[PageUsers3]
DROP TABLE [dbo].[SubPages3]
DROP TABLE [dbo].[Pages3]






SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Pages3](
	[PageId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Controller] [nvarchar](255) NOT NULL,
	[Action] [nvarchar](255) NOT NULL,
	[Tip] [nvarchar](255) NULL,
	[Position] [int] NOT NULL,
	[HasChild] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_Pages3] PRIMARY KEY CLUSTERED 
(
	[PageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO




SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SubPages3](
	[SubPageId] [int] IDENTITY(1,1) NOT NULL,
	[PageId] [int] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Controller] [nvarchar](255) NOT NULL,
	[Action] [nvarchar](255) NOT NULL,
	[Tip] [nvarchar](255) NULL,
	[Position] [int] NOT NULL,
	[IsVisible] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_SubPages3] PRIMARY KEY CLUSTERED 
(
	[SubPageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SubPages3]  WITH CHECK ADD  CONSTRAINT [FK_SubPages3_Pages3] FOREIGN KEY([PageId])
REFERENCES [dbo].[Pages3] ([PageId])
GO

ALTER TABLE [dbo].[SubPages3] CHECK CONSTRAINT [FK_SubPages3_Pages3]
GO





SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PageUsers3](
	[PageUserId] [int] IDENTITY(1,1) NOT NULL,
	[PageId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_PageUsers3] PRIMARY KEY CLUSTERED 
(
	[PageUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PageUsers3]  WITH CHECK ADD  CONSTRAINT [FK_PageUsers3_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[PageUsers3] CHECK CONSTRAINT [FK_PageUsers3_Employees]
GO

ALTER TABLE [dbo].[PageUsers3]  WITH CHECK ADD  CONSTRAINT [FK_PageUsers3_Pages3] FOREIGN KEY([PageId])
REFERENCES [dbo].[Pages3] ([PageId])
GO

ALTER TABLE [dbo].[PageUsers3] CHECK CONSTRAINT [FK_PageUsers3_Pages3]
GO




SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SubPageUsers3](
	[SubPageUserId] [int] IDENTITY(1,1) NOT NULL,
	[SubPageId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_SubPageUsers3] PRIMARY KEY CLUSTERED 
(
	[SubPageUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SubPageUsers3]  WITH CHECK ADD  CONSTRAINT [FK_SubPageUsers3_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[SubPageUsers3] CHECK CONSTRAINT [FK_SubPageUsers3_Employees]
GO

ALTER TABLE [dbo].[SubPageUsers3]  WITH CHECK ADD  CONSTRAINT [FK_SubPageUsers3_SubPages3] FOREIGN KEY([SubPageId])
REFERENCES [dbo].[SubPages3] ([SubPageId])
GO

ALTER TABLE [dbo].[SubPageUsers3] CHECK CONSTRAINT [FK_SubPageUsers3_SubPages3]
GO





