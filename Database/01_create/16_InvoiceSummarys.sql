SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoiceSummarys](
	[InvoiceSummaryId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[TariffTypeId] [int] NOT NULL,
	[UnitCost] [float] NULL,
	[UnitCounts] [float] NULL,
	[Sum] [float] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_InvoiceSummarys] PRIMARY KEY CLUSTERED 
(
	[InvoiceSummaryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[InvoiceSummarys]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceSummarys_Invoices] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoices] ([InvoiceId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[InvoiceSummarys] CHECK CONSTRAINT [FK_InvoiceSummarys_Invoices]
GO

ALTER TABLE [dbo].[InvoiceSummarys]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceSummarys_TariffTypes] FOREIGN KEY([TariffTypeId])
REFERENCES [dbo].[TariffTypes] ([TariffTypeId])
GO

ALTER TABLE [dbo].[InvoiceSummarys] CHECK CONSTRAINT [FK_InvoiceSummarys_TariffTypes]
GO


