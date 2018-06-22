IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_FORM1_FeedDatainPDF')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_FORM1_FeedDatainPDF
GO

/****** Object:  StoredProcedure [dbo].[SPROC_FORM1_FeedDatainPDF]    Script Date: 22-03-2018 19:57:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[SPROC_FORM1_FeedDatainPDF] -- [SPROC_FORM1_FeedDatainPDF] 1095, 1112
(
       @PID int = 1099 , @EID int = 1113
)
AS
 BEGIN

------ Local Variables --- STARTS
Declare @PName nvarchar(500),@PAddress nvarchar(500), @FormNumber nvarchar(500), @FacilityName nvarchar(500), @FID int, @Location nvarchar(500), @EIDStartDate nvarchar(500),
@EIDReason nvarchar(500), @NursingIntials nvarchar(500), @UserID int,@Unknown nvarchar(500) = ' ',@PreparedByVPNSNMOPS nvarchar(500) = 'VPNS', @RowNumber int = 1,
@PFName nvarchar(500), @PLName nvarchar(500), @EncounterDate nvarchar(500), @PatientDOB nvarchar(500), @PatientAge nvarchar(500), @AssessDateTime nvarchar(500),
@NurseUserID nvarchar(500), @NurseName nvarchar(500)

Declare @Vitals table (MVID int, GCID int)
Declare @VitalSelected table (GCID int, GCName nvarchar(100),MinValue numeric(18,2),MaxValue numeric(18,2))
Declare @Results table (KeyName nvarchar(100), KeyValue nvarchar(500))
------ Local Variables --- ENDS

Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))

--GET Information in regards to PatientInfo, Encounter and Facility
Select @PName = Isnull(P.PersonFirstName,' ')+' '+isnull(P.PersonLastName,' '), @FormNumber = P.PersonEmiratesIDNumber, @PAddress = dbo.GetPatientAddressLabel (P.PatientID),
@FID = E.EncounterFacility,@EIDStartDate = Cast(E.EncounterStartTime as nvarchar(50)),@EIDReason= E.EncounterAdmitReason, @UserID = E.CreatedBy,
@FacilityName = F.FacilityName, @Location = Isnull(F.FacilityStreetAddress,' ')+' '+isnull(F.FacilityStreetAddress2,' ')+' '+isnull(F.FacilityPOBox,' '),
@PFName = Isnull(P.PersonFirstName,' '), @PLName = isnull(P.PersonLastName,' '), @EncounterDate = Cast(E.EncounterStartTime as nvarchar(50)),
@PatientDOB = Cast(P.PersonBirthDate As Date), @PatientAge = P.PersonAge, @AssessDateTime = Cast(@CurrentDate As Date), @NurseUserID = @UserID, @NurseName = 
(Select PhysicianName From Physician Where Id = 2035)
from PatientInfo P inner join Encounter E on E.EncounterID = @EID
inner join Facility F on F.FacilityID = E.EncounterFacility
Where P.PatientID = @PID 

----- For Person Who started Encounter Get Intials
Select @NursingIntials = UserName from Users Where UserID = @UserID

---- Get results in Desired Format --- STARTS
insert into @Results
SELECT  KeyName, KeyValue FROM (
Select  @PName 'PatientName',@PAddress 'PatientAddress', @FormNumber 'FormNoNSF30', @FacilityName 'Nursing', @PreparedByVPNSNMOPS as 'PreparedByVPNSNMOPS', @Unknown as 'ReviewedByAssytDONMidwifery',
			@FacilityName 'ApprovedByChiefExecutiveOfficer', @EIDStartDate 'Issuedate', @Unknown as 'Reviewdate', @Unknown as 'Nextreviewdate', @EIDStartDate 'DateTimeKey',
			 @EIDReason 'ReasonforVisit',@Location 'Location', @Unknown as 'Charactercode', @Unknown as 'Frequencyhowoftendoyouexperiencepain', @Unknown as 'Durationhowlonghaveyouhadthispain',
			 @Unknown as 'RadiationNoYesspecifywhere', @NursingIntials 'NurseInitialID',Cast(@EID As NVarchar(500)) 'EncounterID', @PFName 'PatientFirstName',
			 @PLName 'PatientLastName', @EncounterDate 'EncounterDate', @PatientDOB 'PatientDateofBirth', @PatientAge 'PatientAge', @AssessDateTime 'AssessDateTime',
			 Cast(@NurseUserID As NVarchar(500)) 'NurseUserID', Cast(@NurseName As NVarchar(500)) 'NurseName') VW
 unpivot
   (  KeyValue for KeyName in (PatientName,PatientAddress,FormNoNSF30, Nursing,PreparedByVPNSNMOPS,ReviewedByAssytDONMidwifery,ApprovedByChiefExecutiveOfficer,
				IssueDate,Reviewdate,Nextreviewdate,DateTimeKey,ReasonforVisit,Location,Charactercode,Frequencyhowoftendoyouexperiencepain,Durationhowlonghaveyouhadthispain,
				RadiationNoYesspecifywhere,NurseInitialID,EncounterID,PatientFirstName,PatientLastName,EncounterDate,PatientDateofBirth,PatientAge,AssessDateTime,NurseUserID,NurseName)
	) as FormValues;
 ---- Get results in Desired Format --- ENDS

------ Get Latest Vitals for passed in Patient and Encounter --- STARTS
insert into @Vitals
Select max(MedicalVitalID),GlobalCode from MedicalVital Where PatientID = @PID and EncounterID = @EID Group by GlobalCode 

insert into @VitalSelected
Select Distinct MV.GlobalCode,GC.GlobalCodeName,MV.AnswerValueMin ,MV.AnswerValueMax
 from MedicalVital MV inner join GlobalCodes GC on GC.GlobalCodeCategoryValue = '1901' and GC.GlobalCodeValue = MV.GlobalCode
Where PatientID = @PID and EncounterID = @EID  and MedicalVitalID in  ( Select MVID from @Vitals) order by MV.GlobalCode

insert into @Results
 Select CASE WHEN GCID = 3 Then 'TEMP' WHEN GCID = 2 Then 'WEIGHTKey' WHEN GCID = 4 Then 'PULSE' WHEN GCID = 1 Then 'BP' ELSE GCNAME END as KeyName
 ,' Min=' + Cast(isnull(MinValue,0) as Nvarchar(50)) + ' Max=' + Cast(isnull(Maxvalue,0) as Nvarchar(50)) as KeyValue
  from @VitalSelected
------ Get Latest Vitals for passed in Patient and Encounter --- ENDS



 ------ Notes --- Starts
 insert into @Results
 Select top(9) 'NursingNotesRow'+(Cast((Row_Number() Over (order by MedicalNotesID)) as nvarchar(10)))  as KeyName ,(Cast(Cast(NotesDate as Date) as Nvarchar(20)) + ' - '+ Notes) as KeyValue 
--from MedicalNotes where PatientID = 2194 and EncounterID = 2173 order by MedicalNotesID desc
from MedicalNotes where PatientID = @PID and EncounterID = @EID order by MedicalNotesID desc
------ Notes --- Ends

---- Return Results
Select * from @Results

END ---- End of PROC





GO


