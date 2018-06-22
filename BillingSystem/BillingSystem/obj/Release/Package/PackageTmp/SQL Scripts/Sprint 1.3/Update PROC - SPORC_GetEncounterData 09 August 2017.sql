IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SPORC_GetEncounterData') 
  DROP PROCEDURE SPORC_GetEncounterData;

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[SPORC_GetEncounterData]							---- SPORC_GetEncounterData 8
(
@FacilityId int=8
)
AS
BEGIN
Declare @EncounterTempTable table(
BedName nvarchar(1000),Room nvarchar(1000), DepartmentName nvarchar(1000),FloorName nvarchar(1000), BedAssignedOn [datetime] NULL, patientBedService nvarchar(1000),
BedRateApplicable nvarchar(1000),
EncounterID int null, [EncounterNumber] [nvarchar](1000) NULL, [EncounterStartTime] [datetime] NULL,[EncounterPatientType] [int] NULL,
[PatientID] [int] NULL, [EncounterFacility] [varchar](1000) NULL,[EncounterEndTime] [datetime] NULL,[EncounterAdmitReason][nvarchar](100),[WaitingTime][nvarchar](1000)NULL,[FirstName] [nvarchar](1000)NULL,
[LastName] [nvarchar](1000)NULL, [BirthDate] [datetime] NULL,[PersonEmiratesIDNumber] [nvarchar](1000) NULL,[PatientIsVIP] [nvarchar](20)NULL,[Age] int NULL,
[PhysicianName] [nvarchar](1000)NULL, [TriageValue] [nvarchar](1000)NULL, [PatientStageName][nvarchar](50)NULL,[PatientState][nvarchar](20),
[Triage][nvarchar](20),[IsPrimaryDiagnosisDone] [bit] NULL,
[PrimaryDiagnosisDescription] [nvarchar](1000)NULL,[IsEncounterAuthorized] [bit], AverageLengthofStay nvarchar(50), ExpectedLengthofStay nvarchar(50), IsDRGExist [bit]
)
Declare @LocalTime datetime, @TimeZone nvarchar(50)
set @TimeZone=(Select TimeZ from Facility Where Facilityid=@FacilityId)

Set @LocalTime=(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,GETDATE()))

/*
Calculcate Waiting Time from the Encounters and insert in temporary table variable.
*/
Declare @TempWaitingTimes Table(EId bigint, WaitingTime nvarchar(100), ALS nvarchar(50), ELOS nvarchar(50))

INSERT INTO @TempWaitingTimes
Select EncounterId,
(CONVERT(VARCHAR(1500), DATEDIFF(MINUTE, EncounterStartTime,@LocalTime) / 1440 ) +' Days'
       + ' ' + REPLICATE('0', 2 - DATALENGTH(CONVERT(VARCHAR(2), (DATEDIFF(MINUTE, EncounterStartTime,@LocalTime) % 1440) / 60 ))) 
       + CONVERT(VARCHAR(2), (DATEDIFF(MINUTE, EncounterStartTime,@LocalTime) % 1440) / 60 ) + ' Hours '
       + ' ' + REPLICATE('0', 2 - DATALENGTH(CONVERT(VARCHAR(2), (DATEDIFF(MINUTE, EncounterStartTime,@LocalTime) % 60)))) 
       + CONVERT(VARCHAR(2), (DATEDIFF(MINUTE, EncounterStartTime,@LocalTime) % 60)) + ' Minutes ') As WaitingTime
,CONVERT(VARCHAR(1500), DATEDIFF(MINUTE, EncounterStartTime,@LocalTime) / 1440 ) As AverageLengthofStay
,(Case ISNULL((Select TOP 1 D.DRGCodeID From Diagnosis D Where EncounterID = D.EncounterID AND D.DiagnosisType = '3'),0) 
	When 0
		Then 'NA'
	ELSE (Select TOP 1 (Case [Alos]
							WHEN NULL
								THEN '' 
							ELSE CAST(Cast([Alos] As NUMERIC(18,2)) as NVARCHAR(10)) 
						END) 
		From DRGCodes Where [DRGCodesId] = (Select TOP 1 D.DRGCodeID From Diagnosis D Where EncounterID = D.EncounterID AND D.DiagnosisType = '3')
		)
 END) As ELOS
From Encounter Where EncounterFacility=@FacilityId



INSERT INTO @EncounterTempTable
Select '' As BedName,'' As Room,'' As DepartmentName,'' As FloorName, '' As BedAssignedOn, '' As patientBedService,
'' As BedRateApplicable,E.EncounterID, E.EncounterNumber, E.[EncounterStartTime], E.EncounterPatientType, E.PatientId, E.EncounterFacility, E.EncounterEndTime,
E.EncounterAdmitReason
--,Cast( DATEDIFF(hour,EncounterStartTime,@LocalTime) as nvarchar)+' '+'Hours'+' '+Cast( DATEDIFF(MINUTE,EncounterStartTime,@LocalTime)%60 as nvarchar)+' '+'Minutes' AS WaitingTime
,Temp.WaitingTime
,P.PersonFirstName, P.PersonLastName, P.PersonBirthDate, P.PersonEmiratesIDNumber,ISNULL(P.PersonVIP,'') As PatientIsVIP,P.PersonAge As Age,
(Select TOP 1 PhysicianName From Physician Where Id = E.EncounterAttendingPhysician) PhysicianName,
(Select GlobalCodeName From GlobalCodes Where GlobalCodeValue=E.Triage AND GlobalCodeCategoryValue='4952') TriageValue,
(Select GlobalCodeName From GlobalCodes Where GlobalCodeValue=E.PatientState AND GlobalCodeCategoryValue='4951') PatientStageName,E.PatientState,E.Triage,
Case When (Select TOP 1 D.DiagnosisID From Diagnosis D Where E.EncounterID = D.EncounterID AND D.DiagnosisType = '1')!='' Then 1 Else 0 END IsPrimaryDiagnosisDone, 
(Select TOP 1 ISNULL(D.DiagnosisCodeDescription,'') From Diagnosis D Where E.EncounterID = D.EncounterID AND D.DiagnosisType = '1') AS PrimaryDiagnosisDescription,
Case When (Select TOP 1 A.AuthorizationId From [Authorization] A Where E.EncounterID = A.EncounterID)!='' THEN 1 Else 0 END IsEncounterAuthorized
--,DATEDIFF(day,EncounterStartTime,@LocalTime) As AverageLengthofStay,
--(Case ISNULL((Select TOP 1 D.DRGCodeID From Diagnosis D Where E.EncounterID = D.EncounterID AND D.DiagnosisType = '3'),0) 
--	When 0 
--		Then 'NA' ELSE (Select TOP 1 (Case [Alos] 
--											WHEN NULL
--												THEN '' 
--											ELSE CAST(Cast([Alos] As NUMERIC(18,2)) as NVARCHAR(10)) 
--									  END) 
--					    From DRGCodes Where [DRGCodesId] = (Select TOP 1 D.DRGCodeID From Diagnosis D Where E.EncounterID = D.EncounterID AND D.DiagnosisType = '3')
--					    )
-- END) As ELOS
,Temp.ALS As AverageLengthofStay
,Temp.ELOS
,Case When (Select TOP 1 D.DiagnosisID From Diagnosis D Where E.EncounterID = D.EncounterID AND D.DiagnosisType = '3')!='' Then 1 Else 0 END IsDRGExist
From Encounter E
INNER JOIN PatientInfo P ON E.PatientID = P.PatientID
INNER JOIN @TempWaitingTimes Temp ON E.EncounterID = Temp.EId
--LEFT JOIN Diagnosis D ON E.EncounterID = D.EncounterID AND D.DiagnosisType = '1'
Where E.EncounterEndTime IS NULL And EncounterFacility = @FacilityId

--------------------------------------------------------------------------------------------
/*
INPATIENT ENCOUNTERS
*/
Declare @IPCount int

Select @IPCount = Count(1) From @EncounterTempTable Where [EncounterPatientType] = 2

If @IPCOunt > 0
Begin
	Declare @BedInfo table(FSName nvarchar(100), Room nvarchar(100),Department nvarchar(200), FloorName nvarchar(100),AssignedSince Date,
	AssignedSCV nvarchar(20), AssignedRate numeric(9,2),PatientId int)

	
	;With MPB (ParentId,FacilityStructureName,AssignedBedType,AssignedSince,PatientID)
	As
	(
	Select ISNULL(FS.ParentId,0), ISNULL(FS.FacilityStructureName,''),
	(Case ISNULL(M.OverrideBedType,0) WHEN 0 THEN M.BedType ELSE M.OverrideBedType END) AssignedBedType,
	Cast(StartDate as Date) AssignedSince, M.PatientID
	from MappingPatientBed M
	INNER JOIN FacilityStructure FS ON M.FacilityStructureID = FS.FacilityStructureID
	Where M.PatientID IN (Select PatientID From @EncounterTempTable Where [EncounterPatientType] = 2) and EndDate is NULL
	)

	INSERT INTO @BedInfo
	Select M.FacilityStructureName As BedName, (Select TOP 1 FacilityStructureName from FacilityStructure Where FacilityStructureId= M.ParentId) As Room,
	(Select FacilityStructureName from FacilityStructure Where FacilityStructureId= (Select ParentID from FacilityStructure Where FacilityStructureId= M.ParentId)) As DepartmentName,
	(Select FacilityStructureName from FacilityStructure Where FacilityStructureId= (Select ParentID from FacilityStructure Where FacilityStructureId= (Select ParentID from FacilityStructure Where FacilityStructureId= M.ParentId))) As FloorName,
	M.AssignedSince,
	(Select Top 1 ServiceCodeValue From BedRateCard B Where M.AssignedBedType = B.BedTypes And IsNull(B.IsDeleted,'0') = '0' And IsNull(B.IsActive,'1') = '1') AssignedSCV,
	(Select Top 1 Rates From BedRateCard B Where M.AssignedBedType = B.BedTypes And IsNull(B.IsDeleted,'0') = '0' And IsNull(B.IsActive,'1') = '1') AssignedRate,
	M.PatientID
	FROM MPB M


	Update @EncounterTempTable
	Set BedName = B.FSName, Room = B.Room,DepartmentName=B.Department,FloorName=B.FloorName,BedAssignedOn=B.AssignedSince,patientBedService=B.AssignedSCV,
	BedRateApplicable=B.AssignedRate
	From @BedInfo B 
	INNER JOIN @EncounterTempTable E ON B.PatientID = E.PatientID
	Where E.[EncounterPatientType] = 2
End


Select * From @EncounterTempTable


END







