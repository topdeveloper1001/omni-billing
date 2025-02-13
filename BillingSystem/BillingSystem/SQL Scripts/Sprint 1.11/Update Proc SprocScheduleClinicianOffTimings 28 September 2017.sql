IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocScheduleClinicianOffTimings') 
  DROP PROCEDURE SprocScheduleClinicianOffTimings;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Exec [SprocGetOrderCodesToExport] '','ATC',9,8,'','',10
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocScheduleClinicianOffTimings]
(
@pTempNewEntries dbo.SchedulerArrayTT Readonly,
@pUserId bigint,
@pLocalDateTime datetime,
@pClinicianRosterId bigint,
@pFacilityName nvarchar(100)
)
AS
BEGIN
	--Select @pUserId As UserId, @pLocalDateTime As CurrentDate
	Declare @pTempNewEntries2 dbo.SchedulerArrayTT

	INSERT INTO @pTempNewEntries2
	Select * From @pTempNewEntries Order by ScheduleFrom

	Update T Set T.SchedulingId=S.SchedulingId
	From Scheduling S
	INNER JOIN @pTempNewEntries2 T ON S.StatusType=T.[StatusType] And S.ScheduleFrom=T.ScheduleFrom And S.ScheduleTo=T.ScheduleTo 
	And S.AssociatedId=T.AssociatedId


	Update Scheduling Set ModifiedBy=@pUserId, ModifiedDate=@pLocalDateTime,ExtValue5=@pClinicianRosterId Where SchedulingId IN (Select SchedulingId From @pTempNewEntries2)
	
	Delete From Scheduling Where StatusType=@pClinicianRosterId And ExtValue5 IS NULL	

	--Scheduling Insertion
	Insert into [Scheduling] ([AssociatedId],[AssociatedType],[SchedulingType],[Status],[StatusType],[ScheduleFrom],[ScheduleTo],[TypeOfVisit],[PhysicianSpeciality]
							,[PhysicianId],[TypeOfProcedure],[FacilityId],[CorporateId],[Comments],[Location],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate]
							,[IsActive],[DeletedBy],[DeletedDate],[ExtValue1],[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5]
							,[IsRecurring],[RecType],[RecPattern],[RecEventlength],[RecEventPId],[RecurringDateFrom],[RecurringDateTill]
							,[EventId],[WeekDay],[EventParentId],[RoomAssigned],[EquipmentAssigned])
	Select S.AssociatedId,S.AssociatedType,S.SchedulingType,[Status],@pClinicianRosterId,S.ScheduleFrom,S.ScheduleTo,S.TypeOfVisit,S.PhysicianSpeciality
	,S.AssociatedId As PhysicianId,NULL As [TypeOfProcedure],S.FacilityId,S.CorporateId,S.TypeOfVisit As Comments
	,@pFacilityName As [Location],S.CreatedBy,@pLocalDateTime As CurrentDateTime,NULL As ModifiedBy,NULL As ModifiedDate
	,1 As IsActive,NULL As DeletedBy,NULL As DeletedDate,CAST('' as Nvarchar(10)) As DepartmentId,NULL As ExtValue2,'True' As ExtValue3
	,NULL As ExtValue4,NULL As ExtValue5
	,NULL As IsRecurring,NULL As RecType,NULL As RecPattern,NULL As RecEventLength,NULL As RecEventId,NULL AS [RecurringDateFrom],NULL As [RecurringDateTill]
	,EventId,NULL As [WeekDay],EventParentId,NULL As [RoomAssigned],NULL As [EquipmentAssigned]
	From @pTempNewEntries2 S
	Where S.SchedulingId=0

	Update Scheduling Set ExtValue5=NULL Where StatusType=@pClinicianRosterId And ExtValue5=@pClinicianRosterId
END



