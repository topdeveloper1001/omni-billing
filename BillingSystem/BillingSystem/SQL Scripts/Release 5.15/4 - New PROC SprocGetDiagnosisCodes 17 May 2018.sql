-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetDiagnosisCodes','P') IS NOT NULL
   DROP PROCEDURE SprocGetDiagnosisCodes
GO

CREATE PROCEDURE SprocGetDiagnosisCodes
(
	@pUserId bigint,
	@pKeyword nvarchar(500),
	@pTableNumber nvarchar(20),
	@pFId bigint=0
)
As
Begin
	Declare @IsAuthenticated bit=0

	If Exists (Select 1 From Users Where UserId=@pUserId)
		SET @IsAuthenticated=1

	Select DiagnosisCode1,DiagnosisFullDescription from DiagnosisCode 
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
	For Json Path,Root('DiagnosisCodes'),Include_Null_Values
End