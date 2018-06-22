IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetScheduledEvents')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE SprocGetScheduledEvents
END
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetScheduledEvents]  --  [SPROC_GetSchedulingEvents_Back] '3','2015-1130 00:00:00.000','2036,3044',6
(
	 @ViewType nvarchar(50),
	 @selectedDate datetime,
	 @PhysicianIdlist nvarchar(500),
	 @pfacilityId int
)
AS
BEGIN

DECLARE @IDsTable table(Id nvarchar(20))

Declare @TableToReturn Table(
          [SchedulingId] int, [AssociatedId] int,[AssociatedType] nvarchar(50),[SchedulingType] nvarchar(50),[Status] nvarchar(50),[StatusType] nvarchar(50),[ScheduleFrom] datetime,[ScheduleTo] datetime,[TypeOfVisit] nvarchar(100),[PhysicianSpeciality] nvarchar(100),[PhysicianId] nvarchar(100)
           ,[TypeOfProcedure] nvarchar(100),[FacilityId] int,[CorporateId] int,[Comments] nvarchar(500),[Location] nvarchar(500),[CreatedBy] int,[CreatedDate] datetime,[ModifiedBy] int,[ModifiedDate] datetime,[IsActive] bit,[DeletedBy] int
           ,[DeletedDate] Datetime,[ExtValue1] nvarchar(200),[ExtValue2] nvarchar(200),[ExtValue3] nvarchar(200),[ExtValue4] nvarchar(200),[ExtValue5] nvarchar(200),[IsRecurring] bit,[RecType] nvarchar(50),[RecPattern] nvarchar(50),[RecEventlength] bigint,[RecEventPId] int
           ,[RecurringDateFrom] datetime,[RecurringDateTill] datetime,[EventId]  nvarchar(50),[WeekDay]  nvarchar(10),[EventParentId]  nvarchar(50),[RoomAssigned] int,[EquipmentAssigned] int, PhysicianReferredBy bigint)

Insert into @IDsTable
Select IDValue from dbo.split(',',@PhysicianIdlist)

If(@ViewType = '1')
BEGIN
--- No recerrance Events

IF EXISTS (Select 1 from Scheduling  
Where ((PhysicianId IN  (Select Id From @IDsTable)) OR (AssociatedId IN (Select Id From @IDsTable))) AND [Status] not in (4)
AND (CAST(@selectedDate as Date) BETWEEN CAST(ScheduleFrom As Date) and CAST(ScheduleTo As Date)))
BEGIN
	Insert into @TableToReturn
	(
		[SchedulingId],[AssociatedId],[AssociatedType],[SchedulingType],[Status],[StatusType],[ScheduleFrom],[ScheduleTo],[TypeOfVisit],[PhysicianSpeciality],[PhysicianId],[TypeOfProcedure]
	           ,[FacilityId],[CorporateId],[Comments],[Location],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive],[DeletedBy],[DeletedDate],[ExtValue1]
	           ,[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5],[IsRecurring],[RecType],[RecPattern],[RecEventlength],[RecEventPId],[RecurringDateFrom],[RecurringDateTill]
	           ,[EventId],[WeekDay],[EventParentId],[RoomAssigned],[EquipmentAssigned], PhysicianReferredBy
	)
	Select * from Scheduling  
	Where ((PhysicianId IN (Select Id From @IDsTable)) OR (CAST(AssociatedId as nvarchar(50)) in (Select Id From @IDsTable))) 
	AND (CAST(@selectedDate as Date) BETWEEN  CAST(ScheduleFrom As Date) AND CAST(ScheduleTo As Date)) AND [Status] not in (4)
END
ELSE
----- Case of Department
BEGIN
	Insert into @TableToReturn
	(
		[SchedulingId],[AssociatedId],[AssociatedType],[SchedulingType],[Status],[StatusType],[ScheduleFrom],[ScheduleTo],[TypeOfVisit],[PhysicianSpeciality],[PhysicianId],[TypeOfProcedure]
	           ,[FacilityId],[CorporateId],[Comments],[Location],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive],[DeletedBy],[DeletedDate],[ExtValue1]
	           ,[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5],[IsRecurring],[RecType],[RecPattern],[RecEventlength],[RecEventPId],[RecurringDateFrom],[RecurringDateTill]
	           ,[EventId],[WeekDay],[EventParentId],[RoomAssigned],[EquipmentAssigned],PhysicianReferredBy
	)
	select * from Scheduling  
	Where (CAST(ExtValue1 as nvarchar(50)) IN (Select Id From @IDsTable)) AND [Status] not in (4)
	AND (CAST(@selectedDate as Date) BETWEEN  CAST (ScheduleFrom As Date) and CAST(ScheduleTo As Date))
END
END
ELSE If(@ViewType = '2') 
BEGIN

Declare @WeekStart datetime, @WeekEnd Datetime
SELECT @WeekStart = DATEADD(DAY, 1 - DATEPART(WEEKDAY, @selectedDate), CAST(@selectedDate AS DATE)),
       @WeekEnd = DATEADD(DAY, 7 - DATEPART(WEEKDAY, @selectedDate), CAST(@selectedDate AS DATE))

--- No recerrance Events
IF EXISTS (select 1 from Scheduling  
Where ((CAST(PhysicianId as nvarchar(50)) IN  (Select Id From @IDsTable)) or (CAST(AssociatedId as nvarchar(50)) in (Select Id From @IDsTable))) AND [Status] not in (4)
and (CAST (ScheduleFrom As Date) BETWEEN CAST(@WeekStart as Date) AND  CAST(@WeekEnd as Date))
and (CAST (ScheduleTo As Date) BETWEEN CAST(@WeekStart as Date) AND  CAST(@WeekEnd as Date)))
BEGIN
	Insert into @TableToReturn
	(
		[SchedulingId],[AssociatedId],[AssociatedType],[SchedulingType],[Status],[StatusType],[ScheduleFrom],[ScheduleTo],[TypeOfVisit],[PhysicianSpeciality],[PhysicianId],[TypeOfProcedure]
	           ,[FacilityId],[CorporateId],[Comments],[Location],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive],[DeletedBy],[DeletedDate],[ExtValue1]
	           ,[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5],[IsRecurring],[RecType],[RecPattern],[RecEventlength],[RecEventPId],[RecurringDateFrom],[RecurringDateTill]
	           ,[EventId],[WeekDay],[EventParentId],[RoomAssigned],[EquipmentAssigned], PhysicianReferredBy
	)
	select * from Scheduling  
	Where ((PhysicianId IN (Select Id From @IDsTable)) OR (CAST(AssociatedId as nvarchar(50)) in (Select Id From @IDsTable))) 
	and (CAST (ScheduleFrom As Date) BETWEEN CAST(@WeekStart as Date) AND  CAST(@WeekEnd as Date))
	and (CAST (ScheduleTo As Date) BETWEEN CAST(@WeekStart as Date) AND  CAST(@WeekEnd as Date)) AND [Status] not in (4)
END
--- Case of Department
ELSE
BEGIN
	Insert into @TableToReturn
	(
				[SchedulingId],[AssociatedId],[AssociatedType],[SchedulingType],[Status],[StatusType],[ScheduleFrom],[ScheduleTo],[TypeOfVisit],[PhysicianSpeciality],[PhysicianId],[TypeOfProcedure]
	           ,[FacilityId],[CorporateId],[Comments],[Location],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive],[DeletedBy],[DeletedDate],[ExtValue1]
	           ,[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5],[IsRecurring],[RecType],[RecPattern],[RecEventlength],[RecEventPId],[RecurringDateFrom],[RecurringDateTill]
	           ,[EventId],[WeekDay],[EventParentId],[RoomAssigned],[EquipmentAssigned], [PhysicianReferredBy]
	)
	select * from Scheduling  
	Where ((CAST(ExtValue1 as nvarchar(50)) IN  (Select Id From @IDsTable))) 
	and (CAST (ScheduleFrom As Date) BETWEEN CAST(@WeekStart as Date) AND  CAST(@WeekEnd as Date))
	and (CAST (ScheduleTo As Date) BETWEEN CAST(@WeekStart as Date) AND  CAST(@WeekEnd as Date)) AND [Status] not in (4)
END

END
ELSE If(@ViewType = '3') 
BEGIN
Declare @CurrentMonth int = Month(@selectedDate);
Declare @MonthStartDate datetime = Cast(Cast( YEAR(@selectedDate) as nvarchar(10)) + ' - '+ cast(@CurrentMonth as nvarchar(10)) +' - ' + '01' as Datetime)
Declare @MonthEndDate datetime = DATEADD(mm,1,@MonthStartDate)

If EXISTS (select 1 from Scheduling  
Where ((PhysicianId IN (Select Id From @IDsTable)) OR (CAST(AssociatedId as nvarchar(50)) in (Select Id From @IDsTable)))
AND ((CAST(ScheduleFrom As Date) BETWEEN CAST(@MonthStartDate as Date) AND CAST(@MonthEndDate as Date)) AND [Status] not in (4)
OR  (CAST(ScheduleTo As Date) BETWEEN CAST(@MonthStartDate as Date) AND CAST(@MonthEndDate as Date))
OR  ((CAST(@MonthStartDate As Date) BETWEEN CAST(ScheduleFrom as Date) AND CAST(ScheduleTo as Date))
OR  (CAST(@MonthEndDate As Date) BETWEEN CAST(ScheduleFrom as Date) AND CAST(ScheduleTo as Date)))))
BEGIN
	Insert into @TableToReturn
	(
		[SchedulingId],[AssociatedId],[AssociatedType],[SchedulingType],[Status],[StatusType],[ScheduleFrom],[ScheduleTo],[TypeOfVisit],[PhysicianSpeciality],[PhysicianId],[TypeOfProcedure]
	           ,[FacilityId],[CorporateId],[Comments],[Location],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive],[DeletedBy],[DeletedDate],[ExtValue1]
	           ,[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5],[IsRecurring],[RecType],[RecPattern],[RecEventlength],[RecEventPId],[RecurringDateFrom],[RecurringDateTill]
	           ,[EventId],[WeekDay],[EventParentId],[RoomAssigned],[EquipmentAssigned], [PhysicianReferredBy]
	)
	select * from Scheduling  
	Where ((CAST(PhysicianId as nvarchar(50)) IN  (Select Id From @IDsTable)) or (CAST(AssociatedId as nvarchar(50)) in (Select Id From @IDsTable)))
	AND ((CAST(ScheduleFrom As Date) BETWEEN CAST(@MonthStartDate as Date) AND CAST(@MonthEndDate as Date))
	OR  (CAST(ScheduleTo As Date) BETWEEN CAST(@MonthStartDate as Date) AND CAST(@MonthEndDate as Date))
	OR  ((CAST(@MonthStartDate As Date) BETWEEN CAST(ScheduleFrom as Date) AND CAST(ScheduleTo as Date))
	OR  (CAST(@MonthEndDate As Date) BETWEEN CAST(ScheduleFrom as Date) AND CAST(ScheduleTo as Date)))) AND [Status] not in (4)
END
----- Case of Department
ELSE
BEGIN
	Insert into @TableToReturn
	(
				[SchedulingId],[AssociatedId],[AssociatedType],[SchedulingType],[Status],[StatusType],[ScheduleFrom],[ScheduleTo],[TypeOfVisit],[PhysicianSpeciality],[PhysicianId],[TypeOfProcedure]
	           ,[FacilityId],[CorporateId],[Comments],[Location],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive],[DeletedBy],[DeletedDate],[ExtValue1]
	           ,[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5],[IsRecurring],[RecType],[RecPattern],[RecEventlength],[RecEventPId],[RecurringDateFrom],[RecurringDateTill]
	           ,[EventId],[WeekDay],[EventParentId],[RoomAssigned],[EquipmentAssigned], [PhysicianReferredBy]
	)
	select * from Scheduling  
	Where (CAST(ExtValue1 as nvarchar(50)) IN  (Select Id From @IDsTable))
	AND ((CAST(ScheduleFrom As Date) BETWEEN CAST(@MonthStartDate as Date) AND CAST(@MonthEndDate as Date))
	OR  (CAST(ScheduleTo As Date) BETWEEN CAST(@MonthStartDate as Date) AND CAST(@MonthEndDate as Date))
	OR  ((CAST(@MonthStartDate As Date) BETWEEN CAST(ScheduleFrom as Date) AND CAST(ScheduleTo as Date))
	OR  (CAST(@MonthEndDate As Date) BETWEEN CAST(ScheduleFrom as Date) AND CAST(ScheduleTo as Date)))) AND [Status] not in (4)
END
END

Declare @PhysicianIdTable table (Id int)

Insert into @PhysicianIdTable
select Distinct Cast(Extvalue1 as int) from @TableToReturn where Extvalue1 is not null


Declare @PatientIdTable table (Id int)
Insert into @PatientIdTable
select Distinct Cast(AssociatedId as int) from @TableToReturn where [AssociatedType] = 1


Select DISTINCT TR.[SchedulingId],TR.[AssociatedId],TR.[AssociatedType],TR.[SchedulingType],TR.[Status],TR.[StatusType],TR.[ScheduleFrom],TR.[ScheduleTo],TR.[TypeOfVisit],TR.[PhysicianSpeciality]
,TR.[PhysicianId],TR.[TypeOfProcedure],TR.[FacilityId],TR.[CorporateId],TR.[Comments],TR.[Location],TR.[CreatedBy],TR.[CreatedDate],TR.[ModifiedBy],TR.[ModifiedDate]
,TR.[IsActive],TR.[DeletedBy],TR.[DeletedDate],TR.[ExtValue1],TR.[ExtValue2],TR.[ExtValue3],TR.[ExtValue4],TR.[ExtValue5],TR.[IsRecurring],TR.[RecType],TR.[RecPattern]
,TR.[RecEventlength],TR.[RecEventPId],TR.[RecurringDateFrom],TR.[RecurringDateTill],TR.[EventId],TR.[WeekDay],TR.[EventParentId],TR.[RoomAssigned],TR.[EquipmentAssigned]
,FS.FacilityStructureName 'DepartmentName',GCPHY.GlobalCodeName 'PhysicianSPL',Pinfo.PatientId 'PatientId',Pinfo.PersonFirstName + ' '+Pinfo.PersonLastName 'PatientName'
,Pinfo.PersonBirthDate 'PatientDOB',Pinfo.PersonEmailAddress 'PatientEmailId', (Select TOP(1) PhoneNo from PatientPhone where PatientId = AssociatedId and AssociatedType = 1 ) 'PatientPhoneNumber'
,CASE WHEN TR.AssociatedType =1 THEN PHY.PhysicianName ELSE PHY1.PhysicianName  END 'PhysicianName' ,Pinfo.PersonEmiratesIDNumber 'PatientEmirateIdNumber', case when TR.AssociatedType = 1 THEN  TR.PhysicianId ELSE TR.AssociatedId END 'PhysicianId',  
Case WHEN TR.ExtValue3 is null THEN CAST(0 as bit) ELSE CAST(TR.ExtValue3 as bit) END MultipleProcedures,FSR.FacilityStructureName 'RoomAssignedSTR',EM.Equipmentname 'EquipmentAssignedSTR'
,PhysicianReferredBy 
from @TableToReturn TR 
LEFT JOIN  FacilityStructure FS on FS.FacilityStructureId = CAST(TR.Extvalue1 as INT) and TR.Extvalue1 is not null
LEFT JOIN Physician PHY on PHY.ID = TR.PhysicianId and TR.AssociatedType = 1
LEFT JOIN Physician PHY1 on TR.AssociatedId = PHY1.Id and TR.AssociatedType in (2,3)
LEFT JOIN Globalcodes GCPHY on GCPHY.GlobalcodeValue =  PHY.FacultySpeciality and GCPHY.GlobalcodeCategoryValue = 1121
LEFT JOIN PatientInfo Pinfo on Pinfo.PatientId = TR.AssociatedId and TR.AssociatedType = 1
LEFT JOIN FacilityStructure FSR ON FSR.FacilityStructureId = TR.RoomAssigned AND TR.RoomAssigned is not NULL
LEFT JOIN EquipmentMaster EM  ON EM.EquipmentMasterId = TR.EquipmentAssigned AND TR.EquipmentAssigned is not NULL AND TR.EquipmentAssigned <> 0
END
GO


