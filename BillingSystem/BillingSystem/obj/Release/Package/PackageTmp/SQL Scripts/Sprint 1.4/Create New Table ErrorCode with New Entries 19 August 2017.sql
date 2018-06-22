IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[ErrorCode]') AND type in (N'U'))

BEGIN
	CREATE TABLE [dbo].[ErrorCode](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[ErrorCode] [int] NOT NULL,
		[ErrorDetail] [nvarchar](100) NOT NULL,
		[IsActive] [bit] NOT NULL,
	CONSTRAINT [PK_ErrorCode] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END


IF NOT Exists (Select 1 From ErrorCode Where ErrorDetail = 'Duplicate')
Begin
	INSERT INTO ErrorCode
	Select -1 As ErrorCode, 'Duplicate' As ErrorDetail,1 As IsActive
	UNION
	Select -2 As ErrorCode,	'Query Not Executed' As ErrorDetail,1 As IsActive
End
