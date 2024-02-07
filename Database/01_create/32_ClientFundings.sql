SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClientFundings](
	[ClientFundId] [int] IDENTITY(1,1) NOT NULL,
	[CaseId] [int] NOT NULL,
	[ClientSum] [float] NULL,
	[Comment] [nvarchar](512) NULL,
	[ClientFundDate] [datetime] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_ClientFundings] PRIMARY KEY CLUSTERED 
(
	[ClientFundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ClientFundings]  WITH CHECK ADD  CONSTRAINT [FK_ClientFundings_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([CaseId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ClientFundings] CHECK CONSTRAINT [FK_ClientFundings_Cases]
GO

