
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Employees](
	[EmployeeId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[Initials] [nvarchar](255) NULL,
	[MailAddress] [nvarchar](255) NULL,
	[EmployeeTitle] [nvarchar](255) NULL,
	[SignatureLink] [nvarchar](255) NULL,
	[Represent] [bit] NULL,
	[Active] [bit] NULL,
	[ReadOnly] [bit] NULL,
	[UserName] [varchar](255) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Persons](
	[PersonId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](255) NULL,
	[FirstNameCry] [varbinary](256) NULL,
	[LastName] [nvarchar](255) NULL,
	[LastNameCry] [varbinary](256) NULL,
	[MiddleName] [nvarchar](255) NULL,
	[MiddleNameCry] [varbinary](256) NULL,
	[PersonNumber] [nvarchar](255) NULL,
	[PersonNumberCry] [varbinary](256) NULL,
	[MailAddress] [nvarchar](512) NULL,
	[MailAddressCry] [varbinary](512) NULL,
	[Active] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_Persons] PRIMARY KEY CLUSTERED 
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PersonAddresses](
	[PersonAddressId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[CareOf] [nvarchar](512) NULL,
	[CareOfCry] [varbinary](512) NULL,
	[StreetName] [nvarchar](255) NULL,
	[StreetNameCry] [varbinary](256) NULL,
	[StreetNumber] [nvarchar](100) NULL,
	[PostalCode] [nvarchar](20) NULL,
	[PostalAddress] [nvarchar](255) NULL,
	[Country] [nvarchar](255) NULL,
	[Valid] [bit] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_PersonAddresses] PRIMARY KEY CLUSTERED 
(
	[PersonAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PersonAddresses]  WITH CHECK ADD  CONSTRAINT [FK_PersonAddresses_Persons] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Persons] ([PersonId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PersonAddresses] CHECK CONSTRAINT [FK_PersonAddresses_Persons]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CaseTypes](
	[CaseTypeId] [int] IDENTITY(1,1) NOT NULL,
	[CaseType] [nvarchar](255) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_CaseTypes] PRIMARY KEY CLUSTERED 
(
	[CaseTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

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
	[ChangedBy] [nvarchar](50) NULL,
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

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PersonTypes](
	[PersonTypeId] [int] IDENTITY(1,1) NOT NULL,
	[PersonType] [nvarchar](255) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_PersonTypes] PRIMARY KEY CLUSTERED 
(
	[PersonTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PersonCases](
	[PersonCaseId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[CaseId] [int] NOT NULL,
	[PersonTypeId] [int] NOT NULL,
	[Responsible] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_PersonCases] PRIMARY KEY CLUSTERED 
(
	[PersonCaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PersonCases]  WITH CHECK ADD  CONSTRAINT [FK_PersonCases_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([CaseId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PersonCases] CHECK CONSTRAINT [FK_PersonCases_Cases]
GO

ALTER TABLE [dbo].[PersonCases]  WITH CHECK ADD  CONSTRAINT [FK_PersonCases_Persons] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Persons] ([PersonId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PersonCases] CHECK CONSTRAINT [FK_PersonCases_Persons]
GO

ALTER TABLE [dbo].[PersonCases]  WITH CHECK ADD  CONSTRAINT [FK_PersonCases_PersonTypes] FOREIGN KEY([PersonTypeId])
REFERENCES [dbo].[PersonTypes] ([PersonTypeId])
GO

ALTER TABLE [dbo].[PersonCases] CHECK CONSTRAINT [FK_PersonCases_PersonTypes]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TariffTypes](
	[TariffTypeId] [int] IDENTITY(1,1) NOT NULL,
	[TariffType] [nvarchar](255) NOT NULL,
	[NoLevel] [bit] NULL,
	[Invisible] [bit] NULL,
	[BackgroundColor] [nvarchar](50) NULL,	
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_TariffTypes] PRIMARY KEY CLUSTERED 
(
	[TariffTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TariffLevels](
	[TariffLevelId] [int] IDENTITY(1,1) NOT NULL,
	[TariffTypeId] [int] NOT NULL,
	[TariffLevel] [float] NOT NULL,
	[ValidFrom] [datetime] NOT NULL,
	[ValidTo] [datetime] NOT NULL,
	[Valid] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_TariffLevels] PRIMARY KEY CLUSTERED 
(
	[TariffLevelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TariffLevels]  WITH CHECK ADD  CONSTRAINT [FK_TariffLevels_TariffTypes] FOREIGN KEY([TariffTypeId])
REFERENCES [dbo].[TariffTypes] ([TariffTypeId])
GO

ALTER TABLE [dbo].[TariffLevels] CHECK CONSTRAINT [FK_TariffLevels_TariffTypes]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WorkingTimes](
	[WorkingTimeId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[CaseId] [int] NOT NULL,
	[TariffTypeId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[WhenDate] [datetime] NULL,
	[TariffLevel] [float] NULL,
	[NumberOfHours] [float] NULL,
	[Cost] [float] NULL,
	[Sum] [float] NULL,
	[Comment] [nvarchar](1024) NULL,
	[CommentCry] [varbinary](1024) NULL,
	[Billed] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_WorkingTimes] PRIMARY KEY CLUSTERED 
(
	[WorkingTimeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[WorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_WorkingTimes_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([CaseId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[WorkingTimes] CHECK CONSTRAINT [FK_WorkingTimes_Cases]
GO

ALTER TABLE [dbo].[WorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_WorkingTimes_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[WorkingTimes] CHECK CONSTRAINT [FK_WorkingTimes_Employees]
GO

ALTER TABLE [dbo].[WorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_WorkingTimes_Persons] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Persons] ([PersonId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[WorkingTimes] CHECK CONSTRAINT [FK_WorkingTimes_Persons]
GO

ALTER TABLE [dbo].[WorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_WorkingTimes_TariffTypes] FOREIGN KEY([TariffTypeId])
REFERENCES [dbo].[TariffTypes] ([TariffTypeId])
GO

ALTER TABLE [dbo].[WorkingTimes] CHECK CONSTRAINT [FK_WorkingTimes_TariffTypes]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PhoneNumberTypes](
	[PhoneNumberTypeId] [int] IDENTITY(1,1) NOT NULL,
	[PhoneNumberType] [nvarchar](255) NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_PhoneNumberTypes] PRIMARY KEY CLUSTERED 
(
	[PhoneNumberTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[PhoneNumbers](
	[PhoneNumberId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[PhoneNumberTypeId] [int] NOT NULL,
	[PhoneNumber] [nvarchar](255) NULL,
	[PhoneNumberCry] [varbinary](255) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_PhoneNumbers] PRIMARY KEY CLUSTERED 
(
	[PhoneNumberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PhoneNumbers]  WITH CHECK ADD  CONSTRAINT [FK_PhoneNumbers_Persons] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Persons] ([PersonId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PhoneNumbers] CHECK CONSTRAINT [FK_PhoneNumbers_Persons]
GO

ALTER TABLE [dbo].[PhoneNumbers]  WITH CHECK ADD  CONSTRAINT [FK_PhoneNumbers_PhoneNumberTypes] FOREIGN KEY([PhoneNumberTypeId])
REFERENCES [dbo].[PhoneNumberTypes] ([PhoneNumberTypeId])
GO

ALTER TABLE [dbo].[PhoneNumbers] CHECK CONSTRAINT [FK_PhoneNumbers_PhoneNumberTypes]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
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
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_InvoiceUnderlays] PRIMARY KEY CLUSTERED 
(
	[InvoiceUnderlayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
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


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoiceWorkingTimes](
	[InvoiceWorkingTimeId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceUnderlayId] [int] NOT NULL,
	[WorkingTimeId] [int] NOT NULL,
	[UnitCost] [float] NULL,
	[NumberOfHours] [float] NULL,
	[Sum] [float] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_InvoiceWorkingTimes] PRIMARY KEY CLUSTERED 
(
	[InvoiceWorkingTimeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[InvoiceWorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceWorkingTimes_InvoiceUnderlays] FOREIGN KEY([InvoiceUnderlayId])
REFERENCES [dbo].[InvoiceUnderlays] ([InvoiceUnderlayId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[InvoiceWorkingTimes] CHECK CONSTRAINT [FK_InvoiceWorkingTimes_InvoiceUnderlays]
GO

ALTER TABLE [dbo].[InvoiceWorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceWorkingTimes_WorkingTimes] FOREIGN KEY([WorkingTimeId])
REFERENCES [dbo].[WorkingTimes] ([WorkingTimeId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[InvoiceWorkingTimes] CHECK CONSTRAINT [FK_InvoiceWorkingTimes_WorkingTimes]
GO


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
	[ChangedBy] [nvarchar](50) NULL,
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
	[ChangedBy] [nvarchar](50) NULL,
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


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoicePrintingFields](
	[InvoicePrintingField] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceUnderlayId] [int] NOT NULL,
	[FieldName] [nvarchar](255) NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
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


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WorkingTimesPrintingFields](
	[WorkingTimesPrintingFieldId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceUnderlayId] [int] NOT NULL,
	[FieldName] [nvarchar](255) NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_WorkingTimesPrintingFields] PRIMARY KEY CLUSTERED 
(
	[WorkingTimesPrintingFieldId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WorkingTimesPrintingFields]  WITH CHECK ADD  CONSTRAINT [FK_WorkingTimesPrintingFields_InvoiceUnderlays] FOREIGN KEY([InvoiceUnderlayId])
REFERENCES [dbo].[InvoiceUnderlays] ([InvoiceUnderlayId])
GO

ALTER TABLE [dbo].[WorkingTimesPrintingFields] CHECK CONSTRAINT [FK_WorkingTimesPrintingFields_InvoiceUnderlays]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Courts](
	[CourtId] [int] IDENTITY(1,1) NOT NULL,
	[CourtName] [nvarchar](255) NOT NULL,
	[StreetName] [nvarchar](255) NULL,
	[StreetNumber] [nvarchar](10) NULL,
	[PostalCode] [nvarchar](20) NULL,
	[PostalAddress] [nvarchar](255) NULL,
	[Country] [nvarchar](255) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_Courts] PRIMARY KEY CLUSTERED 
(
	[CourtId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[CaseNumbers](
	[CaseNumberId] [int] IDENTITY(1,1) NOT NULL,
	[CaseId] [int] NOT NULL,
	[CourtId] [int] NOT NULL,
	[CaseNumber] [nvarchar](255) NULL,
	[CaseNumberCry] [varbinary](256) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_CaseNumbers] PRIMARY KEY CLUSTERED 
(
	[CaseNumberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CaseNumbers]  WITH CHECK ADD  CONSTRAINT [FK_CaseNumbers_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([CaseId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CaseNumbers] CHECK CONSTRAINT [FK_CaseNumbers_Cases]
GO

ALTER TABLE [dbo].[CaseNumbers]  WITH CHECK ADD  CONSTRAINT [FK_CaseNumbers_Courts] FOREIGN KEY([CourtId])
REFERENCES [dbo].[Courts] ([CourtId])
GO

ALTER TABLE [dbo].[CaseNumbers] CHECK CONSTRAINT [FK_CaseNumbers_Courts]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DocumentTypes](
	[DocumentTypeId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentType] [nvarchar](255) NULL,
	[Extension] [nvarchar](10) NULL,
	[ContentType] [nvarchar](25) NULL,	
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_DocumentTypes] PRIMARY KEY CLUSTERED 
(
	[DocumentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


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
	[ChangedBy] [nvarchar](50) NULL,
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



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Configurations](
	[ConfigurationId] [int] IDENTITY(1,1) NOT NULL,
	[ConfigKey] [nvarchar](255) NULL,
	[ConfigValue] [nvarchar](MAX) NULL,
	[ChangedBy] [nvarchar](50) NULL,
	[Changed] [datetime] NULL,
 CONSTRAINT [PK_Configurations] PRIMARY KEY CLUSTERED 
(
	[ConfigurationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TariffTypePrintingFields](
	[TariffTypePrintingFieldId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceUnderlayId] [int] NOT NULL,
	[TariffTypeId] [int] NOT NULL,
	[Invisible] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_TariffTypePrintingFields] PRIMARY KEY CLUSTERED 
(
	[TariffTypePrintingFieldId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TariffTypePrintingFields]  WITH CHECK ADD  CONSTRAINT [FK_TariffTypePrintingFields_InvoiceUnderlays] FOREIGN KEY([InvoiceUnderlayId])
REFERENCES [dbo].[InvoiceUnderlays] ([InvoiceUnderlayId])
GO

ALTER TABLE [dbo].[TariffTypePrintingFields] CHECK CONSTRAINT [FK_TariffTypePrintingFields_InvoiceUnderlays]
GO

ALTER TABLE [dbo].[TariffTypePrintingFields]  WITH CHECK ADD  CONSTRAINT [FK_TariffTypePrintingFields_TariffTypes] FOREIGN KEY([TariffTypeId])
REFERENCES [dbo].[TariffTypes] ([TariffTypeId])
GO

ALTER TABLE [dbo].[TariffTypePrintingFields] CHECK CONSTRAINT [FK_TariffTypePrintingFields_TariffTypes]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WorkingTimesBu](
	[WorkingTimeBuId] [int] IDENTITY(1,1) NOT NULL,
	[WorkingTimeId] [int] NOT NULL,	
	[PersonId] [int] NOT NULL,
	[CaseId] [int] NOT NULL,
	[TariffTypeId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[WhenDate] [datetime] NULL,
	[TariffLevel] [float] NULL,
	[NumberOfHours] [float] NULL,
	[Cost] [float] NULL,
	[Sum] [float] NULL,
	[Comment] [nvarchar](255) NULL,
	[CommentCry] [varbinary](512) NULL,
	[Billed] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
	[Reason] [nvarchar](50) NULL,
	[ChangedDone] [datetime] NULL,
	[ChangedByDone] [nvarchar](50) NULL,
 CONSTRAINT [PK_WorkingTimesBu] PRIMARY KEY CLUSTERED 
(
	[WorkingTimeBuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TrojPages](
	[PageId] [int] IDENTITY(1,1) NOT NULL,
	[PageLink] [nvarchar](512) NULL,
	[AccessTime] [datetime] NULL,
	[UserName] [nvarchar](512) NULL,
 CONSTRAINT [PK_TrojPages] PRIMARY KEY CLUSTERED
(
	[PageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


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
	[ChangedBy] [nvarchar](50) NULL,
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


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClientFundingMoves](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FromId] [int] NOT NULL,
	[ToId] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
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


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoiceClientFunds](
	[InvoiceClientFundId] [int] IDENTITY(1,1) NOT NULL,
	[ClientFundId] [int] NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
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


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UnderlayTitle](
	[UnderlayTitleId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](512) NULL,
 CONSTRAINT [PK_UnderlayTitleId] PRIMARY KEY CLUSTERED 
(
	[UnderlayTitleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



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
	[ChangedBy] [nvarchar](50) NULL,
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