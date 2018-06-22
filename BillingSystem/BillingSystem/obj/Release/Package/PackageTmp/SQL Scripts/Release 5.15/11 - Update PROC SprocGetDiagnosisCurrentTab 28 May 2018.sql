	-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetDiagnosisCurrentTab','P') IS NOT NULL
   DROP PROCEDURE SprocGetDiagnosisCurrentTab
GO

/****** Object:  StoredProcedure [dbo].[SprocGetDiagnosisTabData]    Script Date: 5/26/2018 4:15:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetDiagnosisCurrentTab] 
(
@pPId bigint,
@pEId bigint=0,
@pEncounterNumber nvarchar(100)=NULL
)
AS
BEGIN
If ISNULL(@pEncounterNumber,'') = ''
		Select @pEncounterNumber = EncounterNumber From Encounter Where EncounterID = @pEId

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
		D1.[DiagnosisCodeDescription]
	END) As DiagnosisCodeDescription,


	(Case 
	When @pEId > 0 
		THEN @pEncounterNumber
	ELSE 
		(Select EncounterNumber From Encounter Where EncounterID = D1.EncounterID)
	End) As EncounterNumber,

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
			From Diagnosis D Where D.PatientID = @pPId
			AND D.EncounterID = @pEId
		 ) D1
	LEFT OUTER JOIN DRGCodes DR ON D1.DRGCodeID = DR.DRGCodesId
	Order by D1.DiagnosisType ASC, D1.CreatedDate DESC
	FOR JSON Path,Root('Diagnosis'),INCLUDE_NULL_VALUES
END