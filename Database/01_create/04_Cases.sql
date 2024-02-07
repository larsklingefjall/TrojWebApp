USE [d002883]
GO

/****** Object:  Table [dbo].[Cases]    Script Date: 2017-11-15 19:06:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
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
	[FinishedDate] [datetime] NULL,
    [Comment] [nvarchar](max) NULL,
    [CommentCry] [varbinary](max) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_Cases] PRIMARY KEY CLUSTERED 
(
	[CaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Cases]  WITH CHECK ADD  CONSTRAINT [FK_Cases_CaseTypes] FOREIGN KEY([CaseTypeId])
REFERENCES [dbo].[CaseTypes] ([CaseTypeId])
GO

ALTER TABLE [dbo].[Cases] CHECK CONSTRAINT [FK_Cases_CaseTypes]
GO


