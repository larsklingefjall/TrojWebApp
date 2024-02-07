USE [d002883]
GO

/****** Object:  Table [dbo].[Documents]    Script Date: 2017-11-15 19:48:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Documents](
	[DocumentId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentTypeId] [int] NOT NULL,	
	[CaseId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[TitleCry] [varbinary](512) NULL,
	[Title] [nvarchar](255) NULL,
	[DocumentDate] [datetime] NULL,
	[FileLink] [nvarchar](1024) NULL,
	[FileLinkCry] [varbinary](1024) NULL,
	[TotalFileLink] [nvarchar](1024) NULL,
	[TotalFileLinkCry] [varbinary](1024) NULL,
	[FileName] [nvarchar](255) NULL,
	[FileContent] [varbinary](max) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_Documents] PRIMARY KEY CLUSTERED 
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [FK_Documents_DocumentTypes] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[DocumentTypes] ([DocumentTypeId])
GO

ALTER TABLE [dbo].[Documents] CHECK CONSTRAINT [FK_Documents_DocumentTypes]
GO

ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [FK_Documents_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([CaseId])
GO

ALTER TABLE [dbo].[Documents] CHECK CONSTRAINT [FK_Documents_Cases]
GO

ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [FK_Documents_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[Documents] CHECK CONSTRAINT [FK_Documents_Employees]
GO


