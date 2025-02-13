IF OBJECT_ID('SprocGetDiagnosisCodes','P') IS NOT NULL
	DROP PROCEDURE SprocGetDiagnosisCodes

GO
/****** Object:  StoredProcedure [dbo].[SprocGetDiagnosisCodes]    Script Date: 6/1/2018 1:02:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SprocGetDiagnosisCodes]
(
	@pUserId bigint,
	@pKeyword nvarchar(500),
	@pTableNumber nvarchar(20),
	@pFId bigint=0
)
As
Begin
	Declare @IsAuthenticated bit=0,@CurrentDate datetime = (Select dbo.GetCurrentDatetimeByEntity(@pFId))

	SET @CurrentDate = Cast(ISNULL(@CurrentDate,GETDATE()) as date)

	If Exists (Select 1 From Users Where UserId=@pUserId)
		SET @IsAuthenticated=1

	Select DiagnosisCode1,DiagnosisFullDescription,DiagnosisEffectiveStartDate,DiagnosisEffectiveEndDate 
	from DiagnosisCode
	Where DiagnosisTableNumber=@pTableNumber And ISNULL(IsDeleted,0)=0
	AND (
			(DiagnosisCode1 like '%'+ @pKeyword  +'%')
			OR 
			(DiagnosisFullDescription like '%'+ @pKeyword  +'%')
			OR
			(DiagnosisMediumDescription like '%'+ @pKeyword  +'%')
			OR
			(ShortDescription like '%'+ @pKeyword  +'%')
			OR
			ISNULL(@pKeyword,'')=''
		)
	And @IsAuthenticated=1
	And CAST(ISNULL(DiagnosisEffectiveStartDate,@CurrentDate) as date) <= @CurrentDate
	And CAST(ISNULL(DiagnosisEffectiveEndDate,@CurrentDate) as date) >= @CurrentDate
	For Json Path,Root('DiagnosisCodes'),Include_Null_Values
End