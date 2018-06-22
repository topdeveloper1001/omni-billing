-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetFavoriteDiagnosisData','P') IS NOT NULL
   DROP PROCEDURE SprocGetFavoriteDiagnosisData
GO

CREATE PROCEDURE SprocGetFavoriteDiagnosisData
(
	@pUserId bigint,
	@pDiagnosisTN nvarchar(20),
	@pDRGTN nvarchar(20)
)
As
Begin
	Select U.UserDefinedDescriptionID,U.CategoryId,U.CodeId,U.RoleID,U.UserID,U.UserDefineDescription,U.CreatedBy,U.CreatedDate
	,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = '1201' And GlobalCodeValue = U.CategoryId) As CategoryName
	,CodeDesc=
	(
		Case U.CategoryId WHEN '9' THEN (Select Top 1 CodeDescription FROM DRGCodes Where CodeNumbering=U.CodeId)
						WHEN '16' THEN (Select Top 1 DiagnosisFullDescription FROM DiagnosisCode Where DiagnosisCode1=U.CodeId)
						ELSE '' END
	)
	From UserDefinedDescriptions U
	Where U.UserID=@pUserId
	And U.CategoryId IN (9,16)
	And ISNULL(U.IsDeleted,0)=0
	For Json Path, Root('FavoriteDiagnosis'),Include_Null_Values
End


