IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetPhyVacationsEvents')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetPhyVacationsEvents
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetPhyVacationsEvents]    Script Date: 22-03-2018 19:03:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetPhyVacationsEvents]
(
@facilityid int =8,@physicianId int =4
)
AS
BEGIN
		Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@facilityid))


--Declare @physicianId int =4,@facilityid int =8
Declare @TableToReturn Table(
          [SchedulingId] int, [AssociatedId] int,[AssociatedType] nvarchar(50),[SchedulingType] nvarchar(50),[Status] nvarchar(50),[StatusType] nvarchar(50),[ScheduleFrom] datetime,[ScheduleTo] datetime,[TypeOfVisit] nvarchar(100),[PhysicianSpeciality] nvarchar(100),[PhysicianId] nvarchar(100)
           ,[TypeOfProcedure] nvarchar(100),[FacilityId] int,[CorporateId] int,[Comments] nvarchar(500),[Location] nvarchar(500),[CreatedBy] int,[CreatedDate] datetime,[ModifiedBy] int,[ModifiedDate] datetime,[IsActive] bit,[DeletedBy] int
           ,[DeletedDate] Datetime,[ExtValue1] nvarchar(200),[ExtValue2] nvarchar(200),[ExtValue3] nvarchar(200),[ExtValue4] nvarchar(200),[ExtValue5] nvarchar(200),[IsRecurring] bit,[RecType] nvarchar(50),[RecPattern] nvarchar(50),[RecEventlength] bigint,[RecEventPId] int
           ,[RecurringDateFrom] datetime,[RecurringDateTill] datetime,[EventId]  nvarchar(50),[WeekDay]  nvarchar(10),[EventParentId]  nvarchar(50),[RoomAssigned] int,[EquipmentAssigned] INT ,[PhysicianReferredBy] INT)

Insert into @TableToReturn
Select * from Scheduling where AssociatedId = @physicianId and SchedulingType = 2 and Cast(ScheduleFrom  as Date) >= Cast(@LocalDateTime as Date)

Select TR.*,FS.FacilityStructureName 'DepartmentName',GCPHY.GlobalCodeName 'PhysicianSPL'
,PHY.PhysicianName 'PhysicianName',TR.AssociatedId 'PhysicianId'
from @TableToReturn TR 
LEFT JOIN  FacilityStructure FS on FS.FacilityStructureId = CAST(TR.Extvalue1 as INT) and TR.Extvalue1 is not null
LEFT JOIN Physician PHY on PHY.ID = TR.AssociatedId 
LEFT JOIN Globalcodes GCPHY on GCPHY.GlobalcodeValue =  PHY.FacultySpeciality and GCPHY.GlobalcodeCategoryValue = 1121


END





GO


