-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocDeleteDiagnosisById','P') IS NOT NULL
   DROP PROCEDURE SprocDeleteDiagnosisById
GO

CREATE PROCEDURE SprocDeleteDiagnosisById
(
	@pUserId bigint,
	@pId bigint,
	@pDRGTN nvarchar(20)
)
As
Begin
	-- ############ Declarations ########################
	Declare @IsAuthenticated bit=0,@PatientId bigint=0,@EncounterId bigint=0,@DrgCode nvarchar(100)
	,@MajorCPTExists bit=0,@MajorDRGExists bit=0,@PrimaryExists bit=0,
	@BillActivityId bigint=0,@EncounterNumber nvarchar(100)

	Declare @ExecutedStatus int=0

	-- ############ Declarations ########################

	--Check If User exists in the Database
	If Exists (Select 1 From Users WHere UserId=@pUserId and IsActive=1 And ISNULL(IsDeleted,0)=0)
		SET @IsAuthenticated=1
	Else 
		SET @ExecutedStatus=-1	--Not Authorized

	If @IsAuthenticated=1 AND @ExecutedStatus=0
	Begin
		Select @PatientId=D.PatientID,@EncounterId=D.EncounterID
		,@DrgCode=
		(
			Case When ISNULL(D.DRGCodeID,0)=0 THEN '' ELSE 
			 (Select TOP 1 D1.CodeNumbering From DRGCodes D1 Where D1.DRGCodesId=D.DRGCodeID)
			END
		)
		,@PrimaryExists=(Case When D.DiagnosisType='1' THEN Cast(1 as bit) ELSE Cast(0 as bit) END)
		From Diagnosis D Where D.DiagnosisID=@pId And ISNULL(D.IsDeleted,0)=0

		
		--Check If Current Diagnosis that is to be deleted, is a primary one
		If @PrimaryExists=1
			Set @ExecutedStatus=-2	--Cannot delete Primary diagnosis.
		Else
			SET @ExecutedStatus=0

		If @ExecutedStatus=0
			Select @EncounterNumber=EncounterNumber From Encounter Where EncounterId=@EncounterId

		--Check if Current Diagnosis Contains the Bill Activities, DRG Code etc.
		If ISNULL(@DrgCode,'') !='' AND @ExecutedStatus=0
		Begin			
			Select @BillActivityId=BillActivityID From BillActivity 
			Where EncounterID=@EncounterId
			And PatientID=@PatientId And ISNULL(IsDeleted,0)=0 And 
			ActivityCode = @DrgCode

			--Delete the Current Diagnosis from Other Tables first such as OrderActivity and BillActivity
			IF @BillActivityId > 0
				Exec SPROC_DeleteBillActivites @BillActivityId,@pUserId

			Set @ExecutedStatus=1
		End
		Else
		Begin
			If ISNULL(@DrgCode,'')=''
				Set @ExecutedStatus=1
			Else
				Set @ExecutedStatus=0
		End

		--Delete the Current Diagnosis
		IF @ExecutedStatus=1
		Begin
			SET @ExecutedStatus=0

			Update Diagnosis SET IsDeleted=1,DeletedBy=@pUserId,DeletedDate=GETDATE() 
			Where DiagnosisID=@pId

			SET @ExecutedStatus=1
		End
		
		--Check If Primary Diagnosis is Done
		IF Exists (Select 1 From Diagnosis Where PatientID=@PatientId And EncounterID=@EncounterId 
		And ISNULL(IsDeleted,0)=0 And DiagnosisType='1')
			SET @PrimaryExists=1
		
		--Check If Major CPT is Done.
		IF Exists (Select 1 From Diagnosis Where PatientID=@PatientId And EncounterID=@EncounterId 
		And ISNULL(IsDeleted,0)=0 And DiagnosisType='4')
			SET @MajorCPTExists=1

		--Check If Major DRG is Done.
		IF Exists (Select 1 From Diagnosis Where PatientID=@PatientId And EncounterID=@EncounterId 
		And ISNULL(IsDeleted,0)=0 And DiagnosisType='3')
			SET @MajorDRGExists=1
	End

	Select @ExecutedStatus

	If @ExecutedStatus=1
	Begin
		Select @PrimaryExists
		Select @MajorCPTExists
		Select @MajorDRGExists
		
		/* ########-----------GETTING the Current Diagnosis LIST-------------######### */
		Exec SprocGetCurrentDiagnosisData @PatientId,@EncounterId,@EncounterNumber,@pDRGTN
	End
End