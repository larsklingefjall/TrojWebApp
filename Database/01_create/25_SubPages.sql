SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SubPages](
	[SubPageId] [int] IDENTITY(1,1) NOT NULL,
	[PageId] [int] NOT NULL,
	[Controller] [nvarchar](255) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[Tip] [nvarchar](255) NULL,
	[Position] [int] NOT NULL,
	[Parameter] [nvarchar](255) NULL,
	[IsVisible] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_SubPages] PRIMARY KEY CLUSTERED 
(
	[SubPageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 
GO

ALTER TABLE [dbo].[SubPages]  WITH CHECK ADD  CONSTRAINT [FK_SubPages_Pages] FOREIGN KEY([PageId])
REFERENCES [dbo].[Pages] ([PageId])
GO

ALTER TABLE [dbo].[SubPages] CHECK CONSTRAINT [FK_SubPages_Pages]
GO


