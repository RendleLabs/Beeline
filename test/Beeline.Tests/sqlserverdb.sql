USE [Beeline]
GO
/****** Object:  Table [dbo].[Test]    Script Date: 04/01/2018 17:35:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Test](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](128) NULL,
	[ByteValue] [tinyint] NULL,
	[ShortValue] [smallint] NULL,
	[LongValue] [bigint] NULL,
	[DecimalValue] [decimal](18, 0) NULL,
	[FloatValue] [float] NULL,
	[DateTimeValue] [datetime] NULL,
	[DateTime2Value] [datetime2](7) NULL,
	[DateTimeOffsetValue] [datetimeoffset](7) NULL,
	[UnicodeName] [nvarchar](128) NULL,
	[LongStringValue] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Test] ON 
GO
INSERT [dbo].[Test] ([Id], [Name], [ByteValue], [ShortValue], [LongValue], [DecimalValue], [FloatValue], [DateTimeValue], [DateTime2Value], [DateTimeOffsetValue], [UnicodeName], [LongStringValue])
VALUES (1, N'Bob', 255, 12345, 123456789, CAST(12345678 AS Decimal(18, 0)), 12345678.876543211, CAST(N'2018-01-04T00:00:00.000' AS DateTime), CAST(N'2018-01-04T00:00:00.0000000' AS DateTime2), CAST(N'2018-01-04T00:00:00.0000000+00:00' AS DateTimeOffset), N'Bob', N'We love Bob')
GO
SET IDENTITY_INSERT [dbo].[Test] OFF
GO
