SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClientFundingMoves](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FromId] [int] NOT NULL,
	[ToId] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_ClientFundingMoves] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ClientFundingMoves]  WITH CHECK ADD  CONSTRAINT [FK_ClientFundingMoves_ClientFundings_From] FOREIGN KEY([FromId])
REFERENCES [dbo].[ClientFundings] ([ClientFundId])
GO

ALTER TABLE [dbo].[ClientFundingMoves] CHECK CONSTRAINT [FK_ClientFundingMoves_ClientFundings_From]
GO

ALTER TABLE [dbo].[ClientFundingMoves]  WITH CHECK ADD  CONSTRAINT [FK_ClientFundingMoves_ClientFundings_To] FOREIGN KEY([ToId])
REFERENCES [dbo].[ClientFundings] ([ClientFundId])
GO

ALTER TABLE [dbo].[ClientFundingMoves] CHECK CONSTRAINT [FK_ClientFundingMoves_ClientFundings_To]
GO