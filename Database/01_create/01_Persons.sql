USE [d002883]
GO

/****** Object:  Table [dbo].[Persons]    Script Date: 2017-09-07 18:54:00 ******/
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
	[MailAddress] [nvarchar](255) NULL,
	[MailAddressCry] [varbinary](512) NULL,
	[Active] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_Persons] PRIMARY KEY CLUSTERED 
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


