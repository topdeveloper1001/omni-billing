-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetMedicalHistoryAndAllergyData','P') IS NOT NULL
   DROP PROCEDURE SprocGetMedicalHistoryAndAllergyData
GO

CREATE PROCEDURE SprocGetMedicalHistoryAndAllergyData
(
@pPatientId bigint,
@pEncounterId bigint=null,
@pUserId bigint
)
As
Begin
	Select M.*
	,Drug=D.DrugGenericName
	,DrugDuration=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=M.Duration And G.GlobalCodeCategoryValue='4801')
	,DrugVolume=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=M.Volume And G.GlobalCodeCategoryValue='4802')
	,DrugDosage=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=M.Dosage And G.GlobalCodeCategoryValue='4803')
	,DrugFrequency=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=M.Frequency And G.GlobalCodeCategoryValue='1024')
	,DrugDecription=(D.DrugGenericName + ' - ' + D.DrugCode)
	From MedicalHistory M
	INNER JOIN DRUG D ON M.DrugName=D.DrugCode
	Where M.PatientId=@pPatientId And ISNULL(M.IsDeleted,0)=0
	FOR JSON Path,Root('MedicalHistory'),INCLUDE_NULL_VALUES


	--Global Codes Category List
	Select * From GlobalCodeCategory Where Cast(GlobalCodeCategoryValue as bigint) Between 8101 and 8999
	FOR JSON Path,Root('GlobalCategory'),INCLUDE_NULL_VALUES


	--Global Codes List
	Select G.* From GlobalCodes G Where G.GlobalCodeCategoryValue IN ( Select GC.GlobalCodeCategoryValue From GlobalCodeCategory GC
								Where Cast(GC.GlobalCodeCategoryValue as bigint) Between 8101 and 8999)
	Order by G.SortOrder
	FOR JSON Path,Root('GlobalCode'),INCLUDE_NULL_VALUES

	--Get Medical Reoords (Allergies)
	Select R.MedicalRecordID As 'MedicalRecord.MedicalRecordID'
	,R.MedicalRecordType As 'MedicalRecord.MedicalRecordType'
	,R.CorporateID As 'MedicalRecord.CorporateID'
	,R.FacilityID As 'MedicalRecord.FacilityID'
	,R.PatientID As 'MedicalRecord.PatientID'
	,R.EncounterID As 'MedicalRecord.EncounterID'
	,R.MedicalRecordNumber As 'MedicalRecord.MedicalRecordNumber'
	,R.GlobalCodeCategoryID As 'MedicalRecord.GlobalCodeCategoryID'
	,R.GlobalCode As 'MedicalRecord.GlobalCode'
	,R.ShortAnswer As 'MedicalRecord.ShortAnswer'
	,R.DetailAnswer As 'MedicalRecord.DetailAnswer'
	,R.Comments As 'MedicalRecord.Comments'
	,R.CommentBy As 'MedicalRecord.CommentBy'
	,R.CommentDate As 'MedicalRecord.CommentDate'
	,R.CreatedBy As 'MedicalRecord.CreatedBy'
	,R.CreatedDate As 'MedicalRecord.CreatedDate'
	,R.ModifiedBy As 'MedicalRecord.ModifiedBy'
	,R.ModifiedDate As 'MedicalRecord.ModifiedDate'
	,R.IsDeleted As 'MedicalRecord.IsDeleted'
	,AlergyName=(Case WHEN ISNULL(R.GlobalCode,0)=0 AND ISNULL(R.GlobalCodeCategoryID,0) > 0 THEN 'Other' 
				ELSE (Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeID=R.GlobalCode)
				END)
	,AlergyType=(Select TOP 1 GlobalCodeCategoryName From GlobalCodeCategory Where GlobalCodeCategoryID=R.GlobalCodeCategoryID)
	,AddedBy=(Select TOP 1 (U.FirstName + ' ' + U.LastName) From Users U Where U.UserID=R.CreatedBy)
	From MedicalRecord R Where PatientID=@pPatientId And ISNULL(IsDeleted,0)=0 And MedicalRecordType=3
	FOR JSON Path,Root('Allergy'),INCLUDE_NULL_VALUES
End