SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CaseLogs](
	[CaseLogId] [int] IDENTITY(1,1) NOT NULL,
	[CaseId] [int] NOT NULL,
	[WhenDate] [datetime] NULL,
	[Comment] [nvarchar](2048) NULL,
	[CommentCry] [varbinary](2048) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_CaseLogs] PRIMARY KEY CLUSTERED 
(
	[CaseLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CaseLogs]  WITH CHECK ADD  CONSTRAINT [FK_CaseLogs_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([CaseId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CaseLogs] CHECK CONSTRAINT [FK_CaseLogs_Cases]
GO