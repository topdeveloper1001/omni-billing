IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocCheckRoomsAndOthersAvailibilty')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[SprocCheckRoomsAndOthersAvailibilty]
GO

/****** Object:  StoredProcedure [dbo].[SprocCheckRoomsAndOthersAvailibilty]    Script Date: 3/26/2018 6:23:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- [SPROC_CheckRoomsAndEquipmentsForScheduling_New] 8,1,'5/25/2016','12:00','12:30',0
-- =============================================
CREATE PROCEDURE [dbo].[SprocCheckRoomsAndOthersAvailibilty]
(
@FacilityId int,
@AppointmentType nvarchar(10),
@ScheduledDate datetime,
@TimeFrom nvarchar(10),
@TimeTo nvarchar(10),
@AvailableRoom int = 0,
@SchId int,
@pId int
)
AS
BEGIN
	Declare @RoomId int=0,@EquipmentId int=0,@RoomCount int=0, @DepId int
	Declare @TempScheduling table (SchedulingId int, RoomAssigned int, EquipmentAssigned int, SameSlotAssigned bit, AssociatedId int)
	Declare @IsEquipmentRequired bit--declared variable is used to store the value whether equipment is required or not
	Declare @IsAllreadyAppointed bit

	--Inserting data(ScheduleId, RoomAssigned,EquipmentAssigned) in to @TempScheduling from Scheduling table
	Insert into @TempScheduling
	Select S.SchedulingId, S.RoomAssigned, S.EquipmentAssigned, (Case SchedulingId When NULL THEN 0 ELSE 1 END) As SameSlotAssigned, S.AssociatedId From Scheduling S
	Where S.FacilityId = @FacilityId 
	And S.TypeOfProcedure = @AppointmentType 
	AND CAST(S.ScheduleFrom as date) = CAST(@ScheduledDate as date)
	AND CAST(S.ScheduleFrom as time) = @TimeFrom AND 
	CAST(S.ScheduleTo as time) = @TimeTo
	And (@SchId=0 OR (S.SchedulingId != @SchId And @SchId!=0))

	IF Exists (Select Count(1) From @TempScheduling)
		Select @IsAllreadyAppointed = SameSlotAssigned From @TempScheduling Where AssociatedId = @pId
	else
		set @IsAllreadyAppointed = 0

	--Retrieving equipment required for coming appointment type or not
	Select @IsEquipmentRequired = CAST(ISNULL(ExtValue1,0) as bit) From [dbo].[AppointmentTypes] Where FacilityId = @FacilityId And Id = @AppointmentType
	--PRINT '@@IsEquipmentRequired: ' + CAST(@IsEquipmentRequired as nvarchar)
	
	--PRINT '@@AvailableRoom: ' + CAST(@AvailableRoom as nvarchar)


	--Check room is passing from front end or not
	If(@AvailableRoom = 0)
	Begin	
		Select TOP 1 @RoomId=F1.FacilityStructureId, @DepId = F1.ParentId From FacilityStructure F1 Where FacilityStructureId NOT IN (Select RoomAssigned From @TempScheduling)
		AND F1.FacilityId = @FacilityId
		And  @AppointmentType in  (Select IDValue From dbo.Split(',',ExternalValue4))
		And (@IsEquipmentRequired=0 OR (@IsEquipmentRequired=1 AND Exists(Select 1 From EquipmentMaster E Where E.FacilityId = @FacilityId AND 
		E.FacilityStructureId IN (F1.FacilityStructureId))))		
	End
	Else
	Begin
		--Set room passing from front end to the room id parameter
		Set @RoomId = @AvailableRoom
	End

	--Check equipment is required or not
	If(@IsEquipmentRequired = 1 AND @RoomId > 0)
	Begin
		Select TOP 1 @EquipmentId = EquipmentMasterId From EquipmentMaster Where EquipmentMasterId NOT IN 
		(Select EquipmentAssigned From @TempScheduling)		 
		AND FacilityId = @FacilityId
		And FacilityStructureId = @RoomId
	End

	--Return room and equipment
	Select ISNULL(@RoomId,0) As RoomId,ISNULL(@EquipmentId,0) AS EquipmentId, ISNULL(@DepId,0) As DepartmentId, ISNULL(@IsAllreadyAppointed, 0) as IsAppointed,ISNULL(@IsEquipmentRequired,0) as IsEquipmentRequired
END





GO


