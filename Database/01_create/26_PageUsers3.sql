USE [d000540]
GO

/****** Object:  Table [dbo].[PageUsers3]    Script Date: 2025-02-16 15:47:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PageUsers3](
	[PageUserId] [int] IDENTITY(1,1) NOT NULL,
	[PageId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_PageUsers3] PRIMARY KEY CLUSTERED 
(
	[PageUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PageUsers3]  WITH CHECK ADD  CONSTRAINT [FK_PageUsers3_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[PageUsers3] CHECK CONSTRAINT [FK_PageUsers3_Employees]
GO

ALTER TABLE [dbo].[PageUsers3]  WITH CHECK ADD  CONSTRAINT [FK_PageUsers3_Pages3] FOREIGN KEY([PageId])
REFERENCES [dbo].[Pages3] ([PageId])
GO

ALTER TABLE [dbo].[PageUsers3] CHECK CONSTRAINT [FK_PageUsers3_Pages3]
GO


