IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetPatientSummaryTabData')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetPatientSummaryTabData
GO

/****** Object:  StoredProcedure [dbo].[SprocGetPatientSummaryTabData]    Script Date: 5/3/2018 9:36:32 PM ******/
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

	--/*Get Patient Details*/
	--Exec SprocGetPatientDetails @PId=@PId, @EId=@EId

	/*Get All Encounter Details, of Current Patient */
	Select E.*,
	(Select GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = '1501' And GlobalCodeValue=E.EncounterType) As EncounterTypeName,
	(Select GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = '1107' And GlobalCodeValue=E.EncounterPatientType) As EncounterPatientTypeName
	From Encounter E Where E.PatientId = @PId
	Order by E.EncounterStartTime Desc


	/* Get Current Encounter's Medical Record */
	Select * From MedicalRecord Where ISNULL(IsDeleted,0)=0 ANd PatientId = @PId And (EncounterID=@EID OR @EId=0)
	FOR JSON PATH,ROOT('MedicalRecord'),INCLUDE_NULL_VALUES



	/*-------------------Get Patient's Medical Vitals based on Current Encounter (MedicalVital), starts here-----------------------------*/
	--Select M.* From MedicalVital M Where ISNULL(M.IsDeleted,0)=0 And MedicalVitalType = 1 And PatientID = @PId
	--Order by M.CreatedDate Desc


	Select M.MedicalVitalID As 'MedicalVital.MedicalVitalID'
	,M.MedicalVitalType As 'MedicalVital.MedicalVitalType'
	,M.CorporateID As 'MedicalVital.CorporateID'
	,M.FacilityID As 'MedicalVital.FacilityID'
	,M.PatientID As 'MedicalVital.PatientID'
	,M.EncounterID As 'MedicalVital.EncounterID'
	,M.MedicalRecordNumber As 'MedicalVital.MedicalRecordNumber'
	,M.GlobalCodeCategoryID As 'MedicalVital.GlobalCodeCategoryID'
	,M.GlobalCode As 'MedicalVital.GlobalCode'
	,M.AnswerValueMin As 'MedicalVital.AnswerValueMin'
	,M.AnswerValueMax As 'MedicalVital.AnswerValueMax'
	,M.AnswerUOM As 'MedicalVital.AnswerUOM'
	,M.Comments As 'MedicalVital.Comments'
	,M.CommentBy As 'MedicalVital.CommentBy'
	,M.CommentDate As 'MedicalVital.CommentDate'
	,M.CreatedBy As 'MedicalVital.CreatedBy'
	,M.CreatedDate As 'MedicalVital.CreatedDate'
	,M.ModifiedBy As 'MedicalVital.ModifiedBy'
	,M.ModifiedDate As 'MedicalVital.ModifiedDate'
	,Cast(Cast(M.AnswerValueMin as numeric(9,2)) as nvarchar) As PressureCustom
	,(Select TOP 1 (U.FirstName + ' ' + U.LastName) From Users U Where U.UserID=M.CommentBy) As VitalAddedBy,
	M.CreatedDate As VitalAddedOn
	,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = M.GlobalCode And GlobalCodeCategoryValue = '1901') As MedicalVitalName
	,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = M.AnswerUOM And GlobalCodeCategoryValue = '3101') As UnitOfMeasureName
	From MedicalVital M
	Where ISNULL(M.IsDeleted,0)=0 And MedicalVitalType = 1 And PatientID = @PId
	Order by M.CreatedDate Desc,M.EncounterID
	FOR JSON PATH,ROOT('Vitals'),INCLUDE_NULL_VALUES
	/*-------------------Get Patient's Medical Vitals based on Current Encounter (MedicalVital), ends here-----------------------------*/



	/*-------------------Get MedicalNotes and Other related Details, starts here-----------------------------*/
	--Select DISTINCT M.* From MedicalNotes M
	--Where ISNULL(M.IsDeleted,0)=0 And M.PatientID = @PId
	--Order by M.CreatedDate Desc

	Select M.MedicalNotesID As 'MedicalNotes.MedicalNotesID'
	,M.MedicalNotesType As 'MedicalNotes.MedicalNotesType'
	,M.NotesUserType As 'MedicalNotes.NotesUserType'
	,M.CorporateID As 'MedicalNotes.CorporateID'
	,M.FacilityID As 'MedicalNotes.FacilityID'
	,M.PatientID As 'MedicalNotes.PatientID'
	,M.EncounterID As 'MedicalNotes.EncounterID'
	,M.MedicalRecordNumber As 'MedicalNotes.MedicalRecordNumber'
	,M.Notes As 'MedicalNotes.Notes'
	,M.NotesBy As 'MedicalNotes.NotesBy'
	,M.NotesDate As 'MedicalNotes.NotesDate'
	,M.MarkedComplication As 'MedicalNotes.MarkedComplication'
	,M.CreatedBy As 'MedicalNotes.CreatedBy'
	,M.CreatedDate As 'MedicalNotes.CreatedDate'
	,M.ModifiedBy As 'MedicalNotes.ModifiedBy'
	,M.ModifiedDate As 'MedicalNotes.ModifiedDate'
	,M.IsDeleted As 'MedicalNotes.IsDeleted'
	,M.DeletedBy As 'MedicalNotes.DeletedBy'
	,M.DeletedDate As 'MedicalNotes.DeletedDate'
	,(Select U.FirstName + ' ' + U.LastName From Users U Where U.UserID=M.NotesBy) As NotesAddedBy,
	(Case M.NotesUserType 
	WHEN 1 THEN 'Physician' ELSE 'Nurse' END) As NotesUserTypeName,
	(Select TOP 1 G.GlobalCodeName From GlobalCodes G 
	Where G.GlobalCodeCategoryValue = '963' And G.GlobalCodeValue = M.MedicalNotesType) As NotesTypeName
	From MedicalNotes M	
	Where ISNULL(M.IsDeleted,0)=0 And M.PatientID = @PId
	Order by M.CreatedDate Desc
	FOR JSON PATH,ROOT('MedicalNotes'),INCLUDE_NULL_VALUES
	/*-------------------Get MedicalNotes and Other related Details, ends here-----------------------------*/


	/*-------------------Get Medical Records and Other related Details, starts here-----------------------------*/
	--Select DISTINCT M.* From MedicalRecord M
	--Where ISNULL(M.IsDeleted,0)=0 AND M.MedicalRecordType = 3 And M.PatientID = @PId
	--Order by M.CreatedDate Desc

	Select M.MedicalRecordID As 'CurrentAlergy.MedicalRecordID'
	,M.MedicalRecordType As 'CurrentAlergy.MedicalRecordType'
	,M.CorporateID As 'CurrentAlergy.CorporateID'
	,M.FacilityID As 'CurrentAlergy.FacilityID'
	,M.PatientID As 'CurrentAlergy.PatientID'
	,M.EncounterID As 'CurrentAlergy.EncounterID'
	,M.MedicalRecordNumber As 'CurrentAlergy.MedicalRecordNumber'
	,M.GlobalCodeCategoryID As 'CurrentAlergy.GlobalCodeCategoryID'
	,M.GlobalCode As 'CurrentAlergy.GlobalCode'
	,M.ShortAnswer As 'CurrentAlergy.ShortAnswer'
	,M.DetailAnswer As 'CurrentAlergy.DetailAnswer'
	,M.Comments As 'CurrentAlergy.Comments'
	,M.CommentBy As 'CurrentAlergy.CommentBy'
	,M.CommentDate As 'CurrentAlergy.CommentDate'
	,M.CreatedBy As 'CurrentAlergy.CreatedBy'
	,M.CreatedDate As 'CurrentAlergy.CreatedDate'
	,M.ModifiedBy As 'CurrentAlergy.ModifiedBy'
	,M.ModifiedDate As 'CurrentAlergy.ModifiedDate'
	,M.IsDeleted As 'CurrentAlergy.IsDeleted'
	,AddedBy=(Select U.FirstName + ' ' + U.LastName From Users U Where U.UserID=M.CreatedBy)
	,AlergyName=(Case 
				When ISNULL(M.GlobalCode,'0')='0' AND ISNULL(GlobalCodeCategoryID,'0') !='0'
				THEN 'Other'
				ELSE
					ISNULL((Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = Cast(M.GlobalCode as nvarchar) AND GlobalCodeCategoryValue=M.GlobalCodeCategoryID),'Other')
				End)
	,AlergyType=(Select TOP 1 G.GlobalCodeCategoryName From GlobalCodeCategory G 
					Where G.GlobalCodeCategoryValue = M.GlobalCodeCategoryID)
	From MedicalRecord M
	Where ISNULL(M.IsDeleted,0)=0 AND M.MedicalRecordType = 3 And M.PatientID = @PId
	Order by M.CreatedDate Desc
	FOR JSON PATH,ROOT('Allergy'),INCLUDE_NULL_VALUES
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


	/*Get Current Medications*/
	Select DISTINCT M.*
	,Drug=D.DrugGenericName
	,DrugDuration=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=M.Duration And G.GlobalCodeCategoryValue='4801')
	,DrugVolume=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=M.Volume And G.GlobalCodeCategoryValue='4802')
	,DrugDosage=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=M.Dosage And G.GlobalCodeCategoryValue='4803')
	,DrugFrequency=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=M.Frequency And G.GlobalCodeCategoryValue='1024')
	,DrugDecription=(D.DrugGenericName + ' - ' + D.DrugCode)
	,DrugCode=M.DrugName
	From MedicalHistory M
	INNER JOIN DRUG D ON M.DrugName=D.DrugCode
	Where M.PatientId=@PId And ISNULL(M.IsDeleted,0)=0
	FOR JSON Path,Root('MedicalHistory'),INCLUDE_NULL_VALUES
END
GO