USE [d002883]
GO

/****** Object:  Table [dbo].[InvoicePrintingFields]    Script Date: 2017-09-07 19:49:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoicePrintingFields](
	[InvoicePrintingField] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceUnderlayId] [int] NOT NULL,
	[FieldName] [nvarchar](255) NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_InvoicePrintingFields] PRIMARY KEY CLUSTERED 
(
	[InvoicePrintingField] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[InvoicePrintingFields]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePrintingFields_InvoiceUnderlays] FOREIGN KEY([InvoiceUnderlayId])
REFERENCES [dbo].[InvoiceUnderlays] ([InvoiceUnderlayId])
GO

ALTER TABLE [dbo].[InvoicePrintingFields] CHECK CONSTRAINT [FK_InvoicePrintingFields_InvoiceUnderlays]
GO


