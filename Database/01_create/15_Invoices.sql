SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[Invoices](
	[InvoiceId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceUnderlayId] [int] NOT NULL,
	[PersonId] [int] NULL,
	[PersonAddressId] [int] NULL,
	[EmployeeId] [int] NULL,
	[EmployeeTitle] [nvarchar](255) NULL,
	[SignatureLink] [nvarchar](255) NULL,
	[InvoiceNumber] [nvarchar](255) NULL,
	[InvoiceDate] [datetime] NULL,
	[InvoicePlace] [nvarchar](255) NULL,
	[ExpirationDate] [datetime] NULL,
	[ReceiverName] [nvarchar](255) NULL,
	[ReceiverNameCry] [varbinary](512) NULL,
	[ContactName] [nvarchar](255) NULL,
	[CareOf] [nvarchar](255) NULL,
	[CareOfCry] [varbinary](512) NULL,
	[StreetName] [nvarchar](255) NULL,
	[StreetNameCry] [varbinary](256) NULL,
	[StreetNumber] [nvarchar](100) NULL,
	[PostalCode] [nvarchar](20) NULL,
	[PostalAddress] [nvarchar](255) NULL,
	[Country] [nvarchar](255) NULL,
	[Headline1] [nvarchar](255) NULL,
	[Headline2] [nvarchar](255) NULL,
	[Vat] [int] NULL,
	[Sum] [float] NULL,
	[Division] [int] NULL,
	[Locked] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
	[HideClientFunding] [bit] NULL,
	[Text1] [nvarchar](255) NULL,	
 CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED 
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_InvoiceUnderlays] FOREIGN KEY([InvoiceUnderlayId])
REFERENCES [dbo].[InvoiceUnderlays] ([InvoiceUnderlayId])
GO

ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_InvoiceUnderlays]
GO


