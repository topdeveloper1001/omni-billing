-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetPreviousDiagnosisData','P') IS NOT NULL
   DROP PROCEDURE SprocGetPreviousDiagnosisData
GO

CREATE PROCEDURE SprocGetPreviousDiagnosisData
(
	@pEncounterId bigint,
	@pPatientId bigint,
	@pDRGTN nvarchar(20),
	@pDiagnosisTN nvarchar(20),
	@pEncounterNumber nvarchar(100)
)
As
Begin
	IF @pEncounterId=0
		Select @pEncounterNumber=EncounterNumber From Encounter Where EncounterID = @pEncounterId

	IF ISNULL(@pEncounterNumber,'')=''
		Select TOP 1 @pEncounterId= EncounterID, @pEncounterNumber = EncounterNumber From Encounter Where PatientID=@pPatientId 

	Select D1.[DiagnosisID]
      ,D1.[DiagnosisType]
      ,D1.[CorporateID]
      ,D1.[FacilityID]
      ,D1.[PatientID]
      ,D1.[EncounterID]
      ,D1.[MedicalRecordNumber]
      ,D1.[DiagnosisCodeId]
      --,D1.[DiagnosisCodeDescription]
      ,D1.[Notes]
      ,D1.[InitiallyEnteredByPhysicianId]
      ,D1.[ReviewedByCoderID]
      ,D1.[ReviewedByPhysicianID]
      ,D1.[CreatedBy]
      ,D1.[CreatedDate]
      ,D1.[ModifiedBy]
      ,D1.[ModifiedDate]
      ,D1.[IsDeleted]
      ,D1.[DeletedBy]
      ,D1.[DeletedDate]
      ,D1.[DRGCodeID]
	,DR.CodeDescription as DrgCodeDescription
	,DR.CodeNumbering As DrgCodeValue
	,(CASE D1.DiagnosisType
	When '3'
		THEN DR.CodeNumbering
	ELSE 
		D1.DiagnosisCode
	END) As DiagnosisCode,

	(CASE D1.DiagnosisType
	When '3' 
		THEN DR.CodeDescription
	ELSE 
		D1.DiagnosisCodeDescription
	END) As DiagnosisCodeDescription,


	(
		Case 
			When @pEncounterId > 0 THEN @pEncounterNumber
			ELSE (Select EncounterNumber From Encounter Where EncounterID = D1.EncounterID)
		End
	) As EncounterNumber,

	(Select U.FirstName + ' ' + U.LastName From Users U Where U.UserID = D1.CreatedBy) As EnteredBy,

	(CASE D1.DiagnosisType
	WHEN 1
		THEN 'Primary Diagnosis'
	WHEN 2 
		THEN 'Secondary Diagnosis'
	WHEN 3 
		THEN 'DRG'
	WHEN 4
		THEN 'Major CPT'
	ELSE
		''
	END
	) As DiagnosisTypeName
	From ( 
			Select D.*
			From Diagnosis D Where D.PatientID = @pPatientId
			AND (@pEncounterId = 0 OR D.EncounterID != @pEncounterId)
		 ) D1	
	LEFT OUTER JOIN DRGCodes DR ON D1.DRGCodeID = DR.DRGCodesId And DR.CodeTableNumber = @pDRGTN
	Order by D1.DiagnosisType ASC, D1.CreatedDate DESC
	FOR JSON Path,Root('Diagnosis'),INCLUDE_NULL_VALUES
End