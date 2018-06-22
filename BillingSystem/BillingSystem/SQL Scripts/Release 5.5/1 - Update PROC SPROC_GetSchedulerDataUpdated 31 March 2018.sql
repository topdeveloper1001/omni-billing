IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetSchedulerDataUpdated')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[SPROC_GetSchedulerDataUpdated]
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetSchedulerDataUpdated]    Script Date: 3/31/2018 11:06:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetSchedulerDataUpdated]
(
@pVT int=3,
@pDate datetime='2016-08-01',
@pPhyIds nvarchar(1000)='4,1004,1005,1031,1032,1033,1050,1051,1052,1053,1054,1055,1056',
@pFId int=8,
@pDIds nvarchar(1000)='',
@pRoomIds nvarchar(4000)='',
@pStatusIds nvarchar(100)='',
@pSectionType nvarchar(20)='others',
@pPatientId int=0
)
AS
BEGIN
	Declare @TableToReturn Table([SchedulingId] int, [AssociatedId] int,[AssociatedType] nvarchar(50),[SchedulingType] nvarchar(50),[Status] nvarchar(50),[StatusType] nvarchar(50)
	,[ScheduleFrom] datetime,[ScheduleTo] datetime,[TypeOfVisit] nvarchar(100),[PhysicianSpeciality] nvarchar(100),[PhysicianId1] nvarchar(100)
	,[TypeOfProcedure] nvarchar(100),[FacilityId] nvarchar(50) /*int*/,[CorporateId] int,[Comments] nvarchar(500),[Location] nvarchar(500),[CreatedBy] int,[CreatedDate] datetime,[ModifiedBy] int,[ModifiedDate] datetime,[IsActive] bit,[DeletedBy] int
	,[DeletedDate] Datetime,[ExtValue1] nvarchar(200),[ExtValue2] nvarchar(200),[ExtValue3] bit /*nvarchar(200)*/,[ExtValue4] nvarchar(200),[ExtValue5] nvarchar(200),[IsRecurring] bit,[RecType] nvarchar(50),[RecPattern] nvarchar(50),[RecEventlength] bigint,[RecEventPId] bigint
	,[RecurringDateFrom] datetime,[RecurringDateTill] datetime,[EventId]  nvarchar(50),[WeekDay]  nvarchar(10),[EventParentId]  nvarchar(50),[RoomAssigned] int,[EquipmentAssigned] int, DepartmentName nvarchar(200)
	,PhysicianSPL nvarchar(200),PatientId int,PatientName nvarchar(200)
	,PatientDOB datetime,PatientEmailId nvarchar(200),PatientPhoneNumber nvarchar(200)
	,PhysicianName nvarchar(200),PatientEmirateIdNumber nvarchar(200), PhysicianId int,  
	MultipleProcedures bit,RoomAssignedSTR nvarchar(200),EquipmentAssignedSTR nvarchar(200),PhysicianReferredBy bigint)

	/*the following query is to get the list of column names declared in the Table Variable declared*/
--	select TN.N.value('local-name(.)', 'sysname') as ColumnName
--from 
--  (
--  select TV.*
--  from (select 1) as D(N)
--    outer apply (
--                select top(0) *
--                from @TableToReturn
--                ) as TV
--  for xml path(''), elements xsinil, type
--  ) as TX(X)
--cross apply TX.X.nodes('*') as TN(N)
	
	Declare @TableToReturn2 Table([SchedulingId] int, [AssociatedId] int,[AssociatedType] nvarchar(50),[SchedulingType] nvarchar(50),[Status] nvarchar(50),[StatusType] nvarchar(50)
	,[ScheduleFrom] datetime,[ScheduleTo] datetime,[TypeOfVisit] nvarchar(100),[PhysicianSpeciality] nvarchar(100),[PhysicianId1] nvarchar(100)
	,[TypeOfProcedure] nvarchar(100),[FacilityId] nvarchar(50) /*int*/,[CorporateId] int,[Comments] nvarchar(500),[Location] nvarchar(500),[CreatedBy] int,[CreatedDate] datetime,[ModifiedBy] int,[ModifiedDate] datetime,[IsActive] bit,[DeletedBy] int
	,[DeletedDate] Datetime,[ExtValue1] nvarchar(200),[ExtValue2] nvarchar(200),[ExtValue3] bit /*nvarchar(200)*/,[ExtValue4] nvarchar(200),[ExtValue5] nvarchar(200),[IsRecurring] bit,[RecType] nvarchar(50),[RecPattern] nvarchar(50),[RecEventlength] bigint,[RecEventPId] bigint
	,[RecurringDateFrom] datetime,[RecurringDateTill] datetime,[EventId] nvarchar(50),[WeekDay]  nvarchar(10),[EventParentId]  nvarchar(50),[RoomAssigned] int,[EquipmentAssigned] int, DepartmentName nvarchar(200)
	,PhysicianSPL nvarchar(200),PatientId int,PatientName nvarchar(200)
	,PatientDOB datetime,PatientEmailId nvarchar(200),PatientPhoneNumber nvarchar(200)
	,PhysicianName nvarchar(200),PatientEmirateIdNumber nvarchar(200), PhysicianId int,  
	MultipleProcedures bit,RoomAssignedSTR nvarchar(200),EquipmentAssignedSTR nvarchar(200), PhysicianReferredBy bigint)

	IF @pPatientId > 0
	Begin
		INSERT INTO @TableToReturn
		Exec SPROC_GetPatientSchedulingEvents @pVT,@pDate,@pPatientId
	End
	Else
	Begin
		INSERT INTO @TableToReturn
		Exec SprocGetScheduledEvents @pVT,@pDate, @pPhyIds, @pFId
	End

	INSERT INTO @TableToReturn2
	Select * From @TableToReturn
	Where
	(@pDIds='' OR ExtValue1 IN (Select IDValue From dbo.Split(',',@pDIds)))
	AND (@pRoomIds='' OR RoomAssigned IN (Select IDVALUE From dbo.Split(',',@pRoomIds)))
	AND (@pStatusIds='' OR [Status] IN (Select IDVALUE From dbo.Split(',',@pStatusIds)))
	Order by [EventParentId]
	
	Declare @MainResults Table(TimeSlotId int, [AssociatedId] int,[AssociatedType] nvarchar(50),[SchedulingType] nvarchar(50),[Availability] nvarchar(50),[StatusType] nvarchar(50)
	,[start_date] nvarchar(100),[end_date] nvarchar(100)
	,[TypeOfVisit] nvarchar(100),[PhysicianSpeciality] nvarchar(100),[TypeOfProcedure] int,location nvarchar(50)
	,[CorporateId] int,[text] nvarchar(500),[CreatedBy] int,[CreatedDate] datetime,Department nvarchar(200),[ExtValue2] nvarchar(200)
	,MultipleProcedure bit
	,[ExtValue4] nvarchar(200),PhysicianComments nvarchar(200),_timed bit, IsRecurrance bit,rec_type nvarchar(50),rec_type_type nvarchar(50)
	,rec_pattern nvarchar(50),rec_pattern_type nvarchar(50),event_length bigint
	,event_pid bigint
	,Rec_Start_date nvarchar(50), Rec_end_date nvarchar(50)
	,Rec_Start_date_type nvarchar(50),Rec_end_date_type nvarchar(50),
	id  bigint,[WeekDay] nvarchar(50),[EventParentId] nvarchar(50),[RoomAssigned] int,[EquipmentAssigned] int,DepartmentName nvarchar(200),PhysicianSpecialityStr nvarchar(200)
	,PatientId int,PatientName nvarchar(200),PatientDOB nvarchar(50)
	,PatientEmailId nvarchar(200),PatientPhoneNumber nvarchar(200)
	,PhysicianName nvarchar(200),EmirateIdnumber nvarchar(200), PhysicianId int,  
	section_id nvarchar(200)
	,TimeSlotTimeInterval nvarchar(50),VacationType nvarchar(100),RoomAssignedSTR nvarchar(200),EquipmentAssignedSTR nvarchar(200)
	,PhysicianReferredBy bigint,ExtValue5 nvarchar(100))


	INSERT INTO @MainResults
	Select [SchedulingId] As TimeSlotId,ISNULL([AssociatedId],'0') As [AssociatedId],[AssociatedType],[SchedulingType],[Status] As [Availability],[StatusType]
	,CAST(FORMAT([ScheduleFrom], 'MM/dd/yyyy HH:mm:ss:mmm') as nvarchar) As [start_date],CAST(FORMAT([ScheduleTo], 'MM/dd/yyyy HH:mm:ss:mmm') as nvarchar) As [end_date]
	,[TypeOfVisit],[PhysicianSpeciality],[TypeOfProcedure],[FacilityId] As location
	,ISNULL([CorporateId],0) As [CorporateId],[Comments] As [text],ISNULL([CreatedBy],'0') As [CreatedBy],[CreatedDate],ISNULL([ExtValue1],'') As Department,[ExtValue2]
	,ISNULL([ExtValue3],0) As MultipleProcedure
	,[ExtValue4],ISNULL([ExtValue5],'') As PhysicianComments,ISNULL([IsRecurring],'0') As _timed,ISNULL([IsRecurring],'0') As IsRecurrance,[RecType] as rec_type,[RecType] as rec_type_type
	,ISNULL([RecPattern],'') as rec_pattern, ISNULL([RecPattern],'') as rec_pattern_type,ISNULL([RecEventlength],'0') As event_length
	,(Case ISNULL([IsRecurring],0) WHEN 1 THEN ISNULL(CAST(EventId as bigint),0) ELSE 0 END) As event_pid,
	Convert(nvarchar(10),[RecurringDateFrom],101) As Rec_Start_date,Convert(nvarchar(10),[RecurringDateTill],101) As Rec_end_date
	,Convert(nvarchar(10),[RecurringDateFrom],101) Rec_Start_date_type,Convert(nvarchar(10),[RecurringDateTill],101) Rec_end_date_type,
	ISNULL(CAST(EventId as bigint),0) As id,
	[WeekDay],[EventParentId],ISNULL([RoomAssigned],'0') As [RoomAssigned],ISNULL([EquipmentAssigned],'0') As [EquipmentAssigned],DepartmentName,PhysicianSPL As PhysicianSpecialityStr
	,ISNULL(patientid,'0') As patientid,PatientName,(CASE ISNULL(PatientDOB,'') WHEN '' THEN '' ELSE CONVERT(nvarchar(10),PatientDOB,101) END) As PatientDOB
	,PatientEmailId,PatientPhoneNumber 
	,PhysicianName ,PatientEmirateIdNumber As EmirateIdnumber,ISNULL(physicianid,'0') As physicianid,  
	(Case WHEN @pSectionType='dep' OR @pSectionType='room' THEN [RoomAssigned] WHEN ISNULL([PhysicianId1],'')='' THEN [AssociatedId] ELSE PhysicianId1 END) As section_id,
	'' As TimeSlotTimeInterval, [TypeOfProcedure] As VacationType,RoomAssignedSTR,EquipmentAssignedSTR
	,ISNULL(PhysicianReferredBy,0),ExtValue5
	From @TableToReturn2
	


	Declare @TempEventsByParent Table (DateSelected nvarchar(100), TimeFrom nvarchar(10)
	,TimeTo nvarchar(10)
	,TypeOfProcedureId nvarchar(100), EventParentId nvarchar(100),MainId int, TypeOfProcedureName nvarchar(100)
	,TimeSlotTimeInterval nvarchar(100)
	,ProcedureAvailablityStatus nvarchar(100), IsRecurrance bit, Rec_Pattern nvarchar(100)
	,Rec_Type nvarchar(100), end_By nvarchar(100), Rec_Start_date nvarchar(100)
	,Rec_end_date nvarchar(100)
	,DeptOpeningDays nvarchar(100)
	,PhysicianId nvarchar(100), ExtValue5 nvarchar(10))


	--Select EventParentId From @TableToReturn2 Group By EventParentId HAVING COUNT(EventParentId) > 1

	INSERT INTO @TempEventsByParent
	--Select CAST(FORMAT(T1.[ScheduleFrom], 'MM/dd/yyyy HH:mm:ss:mmm') as nvarchar) As DateSelected
	Select CAST(FORMAT(T1.[ScheduleFrom], 'MM/dd/yyyy') as nvarchar) As DateSelected
	, CAST(FORMAT(T1.[ScheduleFrom], 'HH:mm') as nvarchar) As TimeFrom
	,(Case ISNULL(T1.IsRecurring,0) When 1 THEN dbo.GetTimeWithTicks(T1.[RecurringDateFrom], T1.[RecEventlength]) ELSE CAST(FORMAT(T1.[ScheduleTo], 'HH:mm') as nvarchar) END) As TimeTo
	,T1.[TypeOfProcedure],T1.[EventParentId],ISNULL(T1.[SchedulingId],'0') As MainId, Ap.Name As TypeOfProcedureName
	,ISNULL(AP.DefaultTime,'0') As TimeSlotTimeInterval
	,T1.Status As ProcedureAvailablityStatus, ISNULL(T1.IsRecurring,'0') As IsRecurrance, T1.RecPattern As Rec_Pattern
	,T1.RecType As Rec_Type, Convert(nvarchar(10),T1.[RecurringDateTill],101) As end_By, Convert(nvarchar(10),T1.[RecurringDateFrom],101) As Rec_Start_date
	,Convert(nvarchar(10),T1.[RecurringDateTill],101) As Rec_end_date
	,(Case ISNULL(T1.ExtValue1,'') WHEN '' THEN '' ELSE dbo.CombineDeptOpeningDays(T1.ExtValue1) END) As DeptOpeningDays
	,T1.PhysicianId, T1.ExtValue5
	From  @TableToReturn2 T1
	LEFT JOIN AppointmentTypes AP ON T1.TypeOfProcedure = AP.Id
	Order by [EventParentId]

	Select * From @MainResults

	Select * From @TempEventsByParent
END


GO


