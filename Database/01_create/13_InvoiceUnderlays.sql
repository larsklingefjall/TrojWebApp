/****** Object:  Table [dbo].[InvoiceUnderlays]    Script Date: 2020-03-07 10:33:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoiceUnderlays](
	[InvoiceUnderlayId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NULL,
	[UnderlayNumber] [nvarchar](50) NULL,
	[PersonId] [int] NOT NULL,
	[CaseId] [int] NOT NULL,
	[PersonAddressId] [int] NULL,
	[EmployeeId] [int] NOT NULL,
	[EmployeeTitle] [nvarchar](255) NULL,
	[SignatureLink] [nvarchar](255) NULL,
	[UnderlayDate] [datetime] NULL,
	[UnderlayPlace] [nvarchar](255) NULL,
	[ReceiverName] [nvarchar](255) NULL,
	[ReceiverNameCry] [varbinary](512) NULL,
	[CareOf] [nvarchar](255) NULL,
	[CareOfCry] [varbinary](512) NULL,
	[StreetName] [nvarchar](255) NULL,
	[StreetNameCry] [varbinary](256) NULL,
	[StreetNumber] [nvarchar](255) NULL,
	[PostalCode] [nvarchar](255) NULL,
	[PostalAddress] [nvarchar](255) NULL,
	[Country] [nvarchar](255) NULL,
	[Headline1] [nvarchar](255) NULL,
	[Headline2] [nvarchar](255) NULL,
	[WorkingReport] [nvarchar](255) NULL,
	[Vat] [int] NULL,
	[Sum] [float] NULL,
	[Locked] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_InvoiceUnderlays] PRIMARY KEY CLUSTERED 
(
	[InvoiceUnderlayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[InvoiceUnderlays]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceUnderlays_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([CaseId])
GO

ALTER TABLE [dbo].[InvoiceUnderlays] CHECK CONSTRAINT [FK_InvoiceUnderlays_Cases]
GO

ALTER TABLE [dbo].[InvoiceUnderlays]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceUnderlays_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[InvoiceUnderlays] CHECK CONSTRAINT [FK_InvoiceUnderlays_Employees]
GO

ALTER TABLE [dbo].[InvoiceUnderlays]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceUnderlays_Persons] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Persons] ([PersonId])
GO

ALTER TABLE [dbo].[InvoiceUnderlays] CHECK CONSTRAINT [FK_InvoiceUnderlays_Persons]
GO