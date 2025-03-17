USE [d000540]
GO

/****** Object:  Table [dbo].[Cases]    Script Date: 2025-03-17 18:29:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Cases](
	[CaseId] [int] IDENTITY(1,1) NOT NULL,
	[CaseTypeId] [int] NOT NULL,
	[Title] [nvarchar](255) NULL,
	[TitleCry] [varbinary](512) NULL,
	[CaseDate] [datetime] NOT NULL,
	[Responsible] [nvarchar](255) NULL,
	[FilePath] [nvarchar](255) NULL,
	[FilePathCry] [varbinary](512) NULL,
	[Active] [bit] NULL,
	[Secrecy] [bit] NULL,
	[FinishedDate] [datetime] NULL,
	[Comment] [nvarchar](max) NULL,
	[CommentCry] [varbinary](max) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_Cases] PRIMARY KEY CLUSTERED 
(
	[CaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Cases]  WITH CHECK ADD  CONSTRAINT [FK_Cases_CaseTypes] FOREIGN KEY([CaseTypeId])
REFERENCES [dbo].[CaseTypes] ([CaseTypeId])
GO

ALTER TABLE [dbo].[Cases] CHECK CONSTRAINT [FK_Cases_CaseTypes]
GO


