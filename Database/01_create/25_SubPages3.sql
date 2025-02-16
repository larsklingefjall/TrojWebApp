USE [d000540]
GO

/****** Object:  Table [dbo].[SubPages3]    Script Date: 2025-02-16 18:29:36 ******/
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



