IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetCMODashboardData')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetCMODashboardData

/****** Object:  StoredProcedure [dbo].[SPROC_GetCMODashboardData]    Script Date: 3/22/2018 6:21:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetCMODashboardData]
(
@CID int=9, @FID int=8
)
AS
BEGIN
		
		Declare @LocalDateTime    datetime= (Select dbo.GetCurrentDatetimeByEntity(@FID))

Declare @ENCId int, @Pid Int,@PatientType int,@EncounterStartTime datetime, @OrderCode nvarchar(20),@LabTestResultVal numeric(18,4),@LabTestResultStatus nvarchar(10);

Declare @FST table(PatientID int,PatientName nvarchar(100), BedName nvarchar(20),Room nvarchar(20),DepartmentName nvarchar(20),FloorName nvarchar(20),BedAssignedOn nvarchar(20),patientBedService nvarchar(20),BedRateApplicable nvarchar(20))

Declare @FormatResults table 
(PatientIsVIP nvarchar(10),CID int, FID int, EncounterId int, PatientId int, 
PersonFirstName nvarchar(50), PersonLastName nvarchar(50), PersonBirthDate Datetime,
EncounterNumber nvarchar(50),EncounterStartTime Datetime,EncounterType nvarchar(50),EncounterPatientType int,
PrimaryDiagnosis nvarchar(250),BedAssigned nvarchar(50),BedServiceCode  nvarchar(50),ALOS int,ELOS nvarchar(10),
DiagnosisStatus nvarchar(10),LabResultStatus nvarchar(10),MedicationStatus nvarchar(10),VitalStatus nvarchar(10),NurseTaskStatus nvarchar(50),IsDRGExist bit)


insert into @FormatResults
select 
PIN.PersonVIP,
EN.CorporateId,PIN.FacilityId,EN.EncounterID,Pin.PatientID,
PIN.personFirstName,PIN.personLastName,PIN.PersonBirthDate,EN.EncounterNumber,EN.EncounterStartTime,
EncounterType = (Case WHEN EN.EncounterPatientType = 1 THEN 'ERPatient' 
ELSE CASE WHEN EN.EncounterPatientType = 2 THEN 'InPatient' ELSE 
CASE WHEN EN.EncounterPatientType = 3 THEN 'OutPatient' ELSE ' ' END END END),EN.EncounterPatientType,
PrimaryDiagnosis =(CASE WHEN  (Select Count(1) from Diagnosis where Diagnosis.EncounterId = EN.EncounterId and Diagnosis.Patientid = EN.PatientID) > 0
THEN (Select DiagnosisCodeDescription from Diagnosis where Diagnosis.EncounterId = EN.EncounterId and Diagnosis.Patientid = EN.PatientID and  Diagnosis.DiagnosisType = 1)
ELSE 'NO' END),
'','',0,
'','','','','','',0
from Encounter EN 
Inner join PatientInfo PIN on PIN.Patientid = EN.PatientID
where EN.CorporateId =@CID and EN.EncounterFacility= @FID and EN.EncounterEndTime is null
Order by EN.EncounterStartTime DESC

Declare @ALOS nvarchar(10) = '',@CurrentDate Datetime = @LocalDateTime, @AveragelengthOfStay int,@MedicationStatus int,@NurseTaskStatus int,@LabTaskStatus int,@VitalStatus int
,@LabTestBADStatus nvarchar(10),@LabTestGoodStatus nvarchar(10),@LabTestCautionStatus nvarchar(10),@VitalStatusBad nvarchar(10) ='0';

Declare ActiveEnc Cursor for 
	Select CID,FID,EncounterId,PatientId,EncounterPatientType,EncounterStartTime from @FormatResults

Open ActiveEnc


FETCH NEXT FROM ActiveEnc INTO @CID,@FID,@ENCId,@Pid,@PatientType,@EncounterStartTime

WHILE @@FETCH_STATUS = 0  
BEGIN  
	IF(@PatientType = 2) --- inpatient bed data
	BEGIN
		Delete from @FST
		Insert into @FST
			Exec [dbo].[SPROC_GetPatientBedInformation] @Pid

		Declare @BedServiceCode nvarchar(20) = ISNULL((Select TOP(1)patientBedService from @FST where PatientId = @Pid),'')
		Declare @DRGCodeId int = (Select DrgCodeId from Diagnosis where Diagnosis.EncounterId = @ENCId and Diagnosis.Patientid = @Pid and Diagnosis.DiagnosisType = 3)
		Set @DRGCodeId = ISNULL(@DRGCodeId,0);
		Set @ALOS ='';

		--print @DRGCodeId

		If(@DRGCodeId <> 0)
		BEGIN
			SET @ALOS =	(Select  TOP(1)ISNULL(Alos,'0') from DRGCodes where DRGCodesId = @DRGCodeId)
		END
		
		Declare @ELOS nvarchar(10) =  
		Case when (@BedServiceCode  = '17-13' OR @BedServiceCode  = '17-14' OR @BedServiceCode  = '17-14' OR
		@BedServiceCode  = '17-15')
		THEN
		'NA'
		ELSE @ALOS END
		-- Average Length of STAY
		Set @AveragelengthOfStay = DATEDIFF(dd,@EncounterStartTime,@CurrentDate)
		
		Set @DRGCodeId =
		Case when (@BedServiceCode  = '17-13' OR @BedServiceCode  = '17-14' OR @BedServiceCode  = '17-14' OR
		@BedServiceCode  = '17-15')
		THEN
		1
		ELSE @DRGCodeId END

		-- Update the Return table with the results
		Update @FormatResults Set 
		BedServiceCode = @BedServiceCode,
		BedAssigned = ISNULL((Select BedName from @FST where PatientId = @Pid),''),
		ELOS = Case WHEN @ELOS <> '0.00000' AND @ELOS <> '' AND @ELOS <> 'NA'  THEN CAST(CAST(@ELOS as Numeric(8,2)) as Nvarchar(10)) ELSE 'NA' END,
		ALOS = Cast(@AveragelengthOfStay as int),
		IsDRGExist = CASE WHEN @DRGCodeId <> 0 THEN 1 else 0 END
		where PatientId = @Pid
		---
	END
	
	---Get Medication Status 
	Set @MedicationStatus = 0;
	Set @MedicationStatus= ISNULL((Select COUNT(1) from OrderActivity 
		where PatientId =@Pid and EncounterId =@ENCId and OrderActivityStatus in (1,2) and OrderCategoryId = 11090
		and DateDiff(mi,OrderScheduleDate,@LocalDateTime) > 30),0)

	---Get Lab Result Status 
	Set @LabTaskStatus = 0;
	Set @LabTestResultStatus = '';
	Set @LabTestBADStatus = '0';Set @LabTestGoodStatus = '0';Set @LabTestCautionStatus = '0';
	Declare LabtestCursor Cursor for 
		Select OrderCode,ResultValueMin from OrderActivity Where EncounterId=@ENCId and patientid =@Pid and OrderActivityStatus=4 and OrderCategoryId =11080
	
	OPEN LabtestCursor

	Fetch NEXT From LabtestCursor INTO @OrderCode,@LabTestResultVal
	WHILE @@FETCH_STATUS = 0  
	BEGIN 
		 Set @LabTestResultStatus =  (Select  dbo.[GetLabTestResultStatus](@LabTestResultVal,@OrderCode,@Pid))
		 if(@LabTestResultStatus = 'Bad')
		 BEGIN
			Set @LabTestBADStatus = '1'
		 END
		 ELSE IF (@LabTestResultStatus = 'Caution')
		 BEGIN
			Set @LabTestCautionStatus = '1'
		 END
		 ELSE IF (@LabTestResultStatus = 'Good')
		 BEGIN
			Set @LabTestGoodStatus = '1'
		 END
	Fetch NEXT From LabtestCursor INTO @OrderCode,@LabTestResultVal
	END
	CLOSE LabtestCursor;  
	DEALLOCATE LabtestCursor;

	---Get Vitals Status _________________________________________________________________________________________________________________________________________
	Set @VitalStatus = 0;Set @VitalStatusBad = '0';
	Declare VitalStatusCursur Cursor for 
		Select MV.GlobalCode,MAX(MV.AnswerValueMin) AnswerValueMin from MedicalVital MV where MV.EncounterId=@ENCId and MV.PatientId= @Pid Group by MV.GlobalCode

	OPEN VitalStatusCursur

	Fetch NEXT From VitalStatusCursur INTO @OrderCode,@LabTestResultVal
	WHILE @@FETCH_STATUS = 0  
	BEGIN 
		 Set @LabTestResultStatus =  (Select  dbo.GetVitalStatus(@LabTestResultVal,@OrderCode))
		 if(@LabTestResultStatus = 'Bad')
		 BEGIN
			Set @VitalStatusBad = '1'
		 END
	Fetch NEXT From VitalStatusCursur INTO @OrderCode,@LabTestResultVal
	END
	CLOSE VitalStatusCursur;  
	DEALLOCATE VitalStatusCursur;
	--________________________________________________________________________________________________________________________________________________________________

	---Get Nurse Tasks Status 
	Set @NurseTaskStatus = 0;
	Set @NurseTaskStatus= ISNULL((Select COUNT(1) from OrderActivity 
		where PatientId =@Pid and EncounterId =@ENCId and OrderActivityStatus in (1,2) 
		and DateDiff(mi,OrderScheduleDate,@LocalDateTime) > 30),0)

	Update @FormatResults
	Set DiagnosisStatus =
	CASE WHEN (Select Count(1) from Diagnosis where Diagnosis.EncounterId = @ENCId and Diagnosis.Patientid = @Pid and Diagnosis.DiagnosisType = 1) <> 0
	THEN
	'True' ELSE 'False' END,
	MedicationStatus = CASE WHEN @MedicationStatus > 0 THEN 'True' ELSE 'False' END,
	NurseTaskStatus = CASE WHEN @NurseTaskStatus > 0 THEN 'True' ELSE 'False' END,
	LabResultStatus = CASE WHEN @LabTestBADStatus = '1' THEN 'BAD' ELSE 
					  CASE WHEN @LabTestCautionStatus ='1' THEN 'Caution' ELSE
					  CASE WHEN @LabTestGoodStatus ='1' THEN 'Good' ELSE 'Good' 
					  END END END ,
	VitalStatus = case WHEN @VitalStatusBad = '1' THEN 'True' ELSE 'False' END
	where PatientId = @Pid

	FETCH NEXT FROM ActiveEnc INTO @CID,@FID,@ENCId,@Pid,@PatientType,@EncounterStartTime
END

CLOSE ActiveEnc;  
DEALLOCATE ActiveEnc;


Select * from @FormatResults

END












GO


