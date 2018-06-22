-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocSaveCurrentDiagnosis','P') IS NOT NULL
   DROP PROCEDURE SprocSaveCurrentDiagnosis
GO

CREATE PROCEDURE SprocSaveCurrentDiagnosis
(
	@pId bigint,
	@pDiagnosisType int,
	@pCId bigint,
	@pFId bigint,
	@pPatientId bigint,
	@pEncounterId bigint,
	@pMedicalRecordNumber nvarchar(20),
	@pDCodeId int,
	@pDCode nvarchar(100),
	@pDCodeDesc nvarchar(1000),
	@pNotes nvarchar(500),
	@pEnteredBy bigint,
	@pCoderReviewedBy bigint,
	@pPhysicianReviewedBy bigint,
	@pDrgCodeId bigint,
	@pMajorCPTCode nvarchar(100),
	@pUserId bigint,
	@pDiagnosisTN nvarchar(20)=NULL,
	@pDRGTN nvarchar(20)=NULL,
	@pCPTTN nvarchar(20)=NULL
)
As
Begin
	Declare @IsAuthenticated bit=0,@CurrentDateTime datetime,@ExecutedStatus int=0
	,@IsPrimaryExists bit=0,@MajorCPTExists bit=0,@MajorDRGExists bit=0

	IF Exists (Select 1 From Users Where UserId=@pUserId And IsActive=1 ANd ISNULL(IsDeleted,0)=0)
		SET @IsAuthenticated=1
	Else
		SET @ExecutedStatus=-1	--Not Authorized


	IF @IsAuthenticated=1
	Begin
		--Check If Primary Already Done
		If Exists (Select 1 From Diagnosis Where DiagnosisID != @pId And ISNULL(IsDeleted,0)=0 
		And EncounterID=@pEncounterId And DiagnosisType=@pDiagnosisType And @pDiagnosisType=1)
			SET @ExecutedStatus=-2 --Primary Diagnosis Already there against the current Encounter


		--Check If Major CPT Already Done
		If Exists (Select 1 From Diagnosis Where DiagnosisID != @pId And ISNULL(IsDeleted,0)=0 
		And EncounterID=@pEncounterId And DiagnosisType=@pDiagnosisType And @pDiagnosisType=4)
			SET @ExecutedStatus=-3 --Major CPT Already there against the current Encounter

		--Check If Major DRG Already Done
		If Exists (Select 1 From Diagnosis Where DiagnosisID != @pId And ISNULL(IsDeleted,0)=0 
		And EncounterID=@pEncounterId And DiagnosisType=@pDiagnosisType And @pDiagnosisType=3)
			SET @ExecutedStatus=-4 --Major DRG Already there against the current Encounter

		--Check If there is any Duplicate Entry
		If Exists (Select 1 From Diagnosis Where DiagnosisID != @pId And ISNULL(IsDeleted,0)=0 
		And EncounterID=@pEncounterId And DiagnosisCode1=@pDCode)
			SET @ExecutedStatus=-5 --Current Diagnosis Code Already there against the current Encounter

		If @ExecutedStatus=0
		Begin
			--If The Diagnosis Type is Primary or Secondary
			If @pDiagnosisType IN (1,2)
			Begin
				If @pId > 0
				Begin
					Update Diagnosis Set DiagnosisCode1=@pDCode, Notes = @pNotes
					,ModifiedBy=@pUserId,ModifiedDate=@CurrentDateTime
					Where [DiagnosisID]=@pId

					SET @ExecutedStatus=1
				End
				Else
				Begin
					INSERT INTO Diagnosis ([DiagnosisType],[CorporateID],[FacilityID],[PatientID]
					,[EncounterID],[MedicalRecordNumber],[DiagnosisCodeId],[DiagnosisCode]
					,[DiagnosisCodeDescription],[Notes],[InitiallyEnteredByPhysicianId],[ReviewedByCoderID]
					,[ReviewedByPhysicianID],[CreatedBy],[CreatedDate],IsDeleted)
					VALUES (@pDiagnosisType,@pCId,@pFId,@pPatientId,@pEncounterId,@pMedicalRecordNumber
					,@pDCodeId,@pDCode,@pDCodeDesc,@pNotes,@pEnteredBy,@pCoderReviewedBy,@pPhysicianReviewedBy
					,@pUserId,@CurrentDateTime,0)
					SET @pId=SCOPE_IDENTITY()
					SET @ExecutedStatus=1
				End
			End
			Else
				SET @ExecutedStatus=1
				
				
			If @pDrgCodeId > 0 AND @ExecutedStatus=1
			Begin
				SET @ExecutedStatus=0

				IF Exists (Select 1 From Diagnosis Where DrgCodeId=@pDrgCodeId And IsActive=1 
				And ISNULL(IsDeleted,0)=0)
				Begin
					Update Diagnosis Set DrgCodesId=@pDrgCodeId, Notes = @pNotes
					,ModifiedBy=@pUserId,ModifiedDate=@CurrentDateTime
					Where [DiagnosisID]=@pId
					
					SET @ExecutedStatus=1

				End
				Else
				Begin
					INSERT INTO Diagnosis ([DiagnosisType],[CorporateID],[FacilityID],[PatientID]
					,[EncounterID],[MedicalRecordNumber],[DiagnosisCodeId],[DiagnosisCode]
					,[DiagnosisCodeDescription],[Notes],[InitiallyEnteredByPhysicianId],[ReviewedByCoderID]
					,[ReviewedByPhysicianID],[CreatedBy],[CreatedDate],IsDeleted)
					VALUES (@pDiagnosisType,@pCId,@pFId,@pPatientId,@pEncounterId,@pMedicalRecordNumber
					,@pDCodeId,@pDCode,@pDCodeDesc,@pNotes,@pEnteredBy,@pCoderReviewedBy,@pPhysicianReviewedBy
					,@pUserId,@CurrentDateTime,0)
					SET @pId=SCOPE_IDENTITY()
					SET @ExecutedStatus=1
				End
			End						
		End
	End

	SET @CurrentDateTime=[dbo].[GetCurrentDatetimeByEntity](@FId)

End