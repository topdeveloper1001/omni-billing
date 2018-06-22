IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetPatientSummaryTabData')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetPatientSummaryTabData
GO

/****** Object:  StoredProcedure [dbo].[SprocGetPatientSummaryTabData]    Script Date: 3/27/2018 7:42:53 PM ******/
DROP PROCEDURE [dbo].[SprocGetPatientSummaryTabData]
GO

/****** Object:  StoredProcedure [dbo].[SprocGetPatientSummaryTabData]    Script Date: 3/27/2018 7:42:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetPatientSummaryTabData]
(
@PId bigint,
@EId bigint=0
)
AS
BEGIN
	Declare @EncounterNumber nvarchar(100)=''

	--Getting the Encounter Number from the table ENCOUNTER, STARTs here
	If @EId = 0
		Select TOP 1 @EId= EncounterID, @EncounterNumber = EncounterNumber From Encounter 
		Where PatientID=@PId Order by EncounterEndTime

	If ISNULL(@EncounterNumber,'') = ''
		Select @EncounterNumber = EncounterNumber From Encounter Where EncounterID = @EId

	/*Get Patient Details*/
	Exec SprocGetPatientDetails @PId=@PId, @EId=@EId

	--/*Get Active Encounter Details*/
	--Select * From Encounter Where PatientId = @PId And (EncounterEndTime IS NULL OR (@EId=0 OR EncounterID=@EId))


	/*Get All Encounter Details, of Current Patient */
	Select E.*, 
	(Select GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = '1501' And GlobalCodeValue=E.EncounterType) As EncounterTypeName,
	(Select GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = '1107' And GlobalCodeValue=E.EncounterPatientType) As EncounterPatientTypeName
	From Encounter E Where E.PatientId = @PId
	Order by E.EncounterStartTime Desc


	/* Get Current Encounter's Medical Record */
	Select * From MedicalRecord Where ISNULL(IsDeleted,0)=0 ANd PatientId = @PId And (EncounterID=@EID OR @EId=0)



	/*-------------------Get Patient's Medical Vitals based on Current Encounter (MedicalVital), starts here-----------------------------*/
	Select M.* From MedicalVital M Where ISNULL(M.IsDeleted,0)=0 And MedicalVitalType = 1 And PatientID = @PId
	Order by M.CreatedDate Desc


	Select Cast(Cast(M.AnswerValueMin as numeric(9,2)) as nvarchar) As PressureCustom
	,(Select TOP 1 (U.FirstName + ' ' + U.LastName) From Users U Where U.UserID=M.CommentBy) As VitalAddedBy,
	M.CreatedDate As VitalAddedOn
	,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = M.GlobalCode And GlobalCodeCategoryValue = '1901') As MedicalVitalName
	,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = M.AnswerUOM And GlobalCodeCategoryValue = '3101') As UnitOfMeasureName
	From MedicalVital M 
	Where ISNULL(M.IsDeleted,0)=0 And MedicalVitalType = 1 And PatientID = @PId
	Order by M.CreatedDate Desc
	/*-------------------Get Patient's Medical Vitals based on Current Encounter (MedicalVital), ends here-----------------------------*/



	/*-------------------Get MedicalNotes and Other related Details, starts here-----------------------------*/
	Select DISTINCT M.* From MedicalNotes M
	Where ISNULL(M.IsDeleted,0)=0 And M.PatientID = @PId
	Order by M.CreatedDate Desc

	Select 
	(Select U.FirstName + ' ' + U.LastName From Users U Where U.UserID=M.NotesBy) As NotesAddedBy,
	(Case M.NotesUserType 
	WHEN 1 THEN 'Physician' ELSE 'Nurse' END) As NotesUserTypeName,
	(Select TOP 1 G.GlobalCodeName From GlobalCodes G 
	Where G.GlobalCodeCategoryValue = '963' And G.GlobalCodeValue = M.MedicalNotesType) As NotesTypeName
	From MedicalNotes M	
	Where ISNULL(M.IsDeleted,0)=0 And M.PatientID = @PId
	Order by M.CreatedDate Desc
	/*-------------------Get MedicalNotes and Other related Details, ends here-----------------------------*/


	/*-------------------Get Medical Records and Other related Details, starts here-----------------------------*/
	Select DISTINCT M.* From MedicalRecord M
	Where ISNULL(M.IsDeleted,0)=0 AND M.MedicalRecordType = 3 And M.PatientID = @PId
	Order by M.CreatedDate Desc

	Select (Select U.FirstName + ' ' + U.LastName From Users U Where U.UserID=M.CreatedBy) As AddedBy,
	(Case 
	When ISNULL(M.GlobalCode,0)=0 AND ISNULL(GlobalCodeCategoryID,0) > 0
	THEN 'Other'
	ELSE
		ISNULL((Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeID = M.GlobalCode),'Other')
	End) As AlergyName,
	(Select TOP 1 G.GlobalCodeCategoryName From GlobalCodeCategory G Where G.GlobalCodeCategoryValue = M.GlobalCodeCategoryID) As AlergyType
	From MedicalRecord M	
	Where ISNULL(M.IsDeleted,0)=0 AND M.MedicalRecordType = 3 And M.PatientID = @PId
	Order by M.CreatedDate Desc
	/*-------------------Get Medical Records and Other related Details, ends here-----------------------------*/

	/* Get Physician Orders Only */
	Exec SprocGetPhysicianOrdersAndActivities @pEncounterId=@EId


	/* Get Diagnosis List */
	Select D1.*
	,DR.CodeDescription as DrgCodeDescription
	,DR.CodeNumbering As DrgCodeValue
	,(CASE D1.DiagnosisType
	When '3'
		THEN DR.CodeNumbering
	ELSE 
		'0'
	END) As DiagnosisCode,

	(CASE D1.DiagnosisType
	When '3' 
		THEN DR.CodeDescription
	ELSE 
		'0' 
	END) As DiagnosisCodeDescription,


	(Case 
	When @EId > 0 
		THEN @EncounterNumber
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
	From Diagnosis D Where D.PatientID = @PId
	AND (@EId = 0 OR D.EncounterID = @EId)) D1	
	LEFT OUTER JOIN DRGCodes DR ON D1.DRGCodeID = DR.DRGCodesId
	Order by D1.DiagnosisType ASC, D1.CreatedDate DESC
	/* ########-----------GETTING the Current Diagnosis LIST-------------######### */



	/*-----Get Medical Vitals / Risks*/
	Exec SPROC_GetRiskFactors @pPatientId=@PId
	/*-----Get Medical Vitals / Risks*/


	/*Get Encounter ID*/
	Select Cast(@EId as bigint) As CurrentEncounterId
END


GO


