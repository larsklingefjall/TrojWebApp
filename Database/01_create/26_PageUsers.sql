SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PageUsers](
	[PageUserId] [int] IDENTITY(1,1) NOT NULL,
	[PageId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_PageUsers] PRIMARY KEY CLUSTERED 
(
	[PageUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PageUsers]  WITH CHECK ADD  CONSTRAINT [FK_PageUsers_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[PageUsers] CHECK CONSTRAINT [FK_PageUsers_Employees]
GO

ALTER TABLE [dbo].[PageUsers]  WITH CHECK ADD  CONSTRAINT [FK_PageUsers_Pages] FOREIGN KEY([PageId])
REFERENCES [dbo].[Pages] ([PageId])
GO

ALTER TABLE [dbo].[PageUsers] CHECK CONSTRAINT [FK_PageUsers_Pages]
GO


