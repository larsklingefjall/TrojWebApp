SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoiceClientFunds](
	[InvoiceClientFundId] [int] IDENTITY(1,1) NOT NULL,
	[ClientFundId] [int] NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_InvoiceClientFunds] PRIMARY KEY CLUSTERED 
(
	[InvoiceClientFundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[InvoiceClientFunds]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceClientFunds_ClientFundings] FOREIGN KEY([ClientFundId])
REFERENCES [dbo].[ClientFundings] ([ClientFundId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[InvoiceClientFunds] CHECK CONSTRAINT [FK_InvoiceClientFunds_ClientFundings]
GO


ALTER TABLE [dbo].[InvoiceClientFunds]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceClientFunds_Invoices] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoices] ([InvoiceId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[InvoiceClientFunds] CHECK CONSTRAINT [FK_InvoiceClientFunds_Invoices]
GO



