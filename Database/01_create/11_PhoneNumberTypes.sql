USE [d002883]
GO

/****** Object:  Table [dbo].[PhoneNumberTypes]    Script Date: 2017-09-07 19:38:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PhoneNumberTypes](
	[PhoneNumberTypeId] [int] IDENTITY(1,1) NOT NULL,
	[PhoneNumberType] [nvarchar](255) NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_PhoneNumberTypes] PRIMARY KEY CLUSTERED 
(
	[PhoneNumberTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


