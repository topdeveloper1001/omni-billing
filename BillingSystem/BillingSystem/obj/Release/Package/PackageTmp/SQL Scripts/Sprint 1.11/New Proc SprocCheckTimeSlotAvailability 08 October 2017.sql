IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocCheckTimeSlotAvailability') 
  DROP PROCEDURE SprocCheckTimeSlotAvailability;
GO
/****** Object:  StoredProcedure [dbo].[SPROC_GetTimeSlotAvailablity]    Script Date: 10/5/2017 4:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocCheckTimeSlotAvailability] 
(
	@pSchedulingId int,
	@pSchedulingType nvarchar(10),
	@pSelectedDate Datetime,
	@TimeFrom nvarchar(10),
	@TimeTo nvarchar(10),
	@pUserid int,
	@pPhysicianid Int,
	@pFacilityid int
)
AS
BEGIN

Declare @TimeSlotAvialable int
	
IF(@pSchedulingType = '2')
BEGIN
	SET @TimeSlotAvialable = (Select Count(SchedulingID) From Scheduling S Where ((@TimeFrom >= Cast(ScheduleFrom as time) AND
	@TimeFrom <= Cast(ScheduleTo as time)) or (@TimeTo >=  Cast(ScheduleFrom as time) AND @TimeTo <= Cast(ScheduleTo as time)) 
	OR Cast(ScheduleFrom as time) between @TimeFrom AND @TimeTo)
	AND Cast(ScheduleFrom as Date) = Cast(@pSelectedDate as Date)
	AND S.FacilityId = @pFacilityid and S.SchedulingId <>  @pSchedulingId
	AND (S.PhysicianId = @pPhysicianid OR (S.AssociatedId = @pPhysicianid and S.AssociatedType = '2') AND S.AssociatedType <> '3'))
END
ELSE
BEGIN
	SET @TimeSlotAvialable = (Select Count(SchedulingID) From Scheduling S Where ((@TimeFrom >= Cast(ScheduleFrom as time) AND
	@TimeFrom <= Cast(ScheduleTo as time)) or (@TimeTo >=  Cast(ScheduleFrom as time) AND @TimeTo <= Cast(ScheduleTo as time)) 
	OR Cast(ScheduleFrom as time) between @TimeFrom AND @TimeTo)
	AND Cast(ScheduleFrom as Date) = Cast(@pSelectedDate as Date)
	AND S.FacilityId = @pFacilityid and S.SchedulingId <>  @pSchedulingId
	AND (S.PhysicianId = @pPhysicianid OR (S.AssociatedId = @pPhysicianid and S.AssociatedType = '2')))
END

Select @TimeSlotAvialable as 'TimeSlotAvailable'

END





