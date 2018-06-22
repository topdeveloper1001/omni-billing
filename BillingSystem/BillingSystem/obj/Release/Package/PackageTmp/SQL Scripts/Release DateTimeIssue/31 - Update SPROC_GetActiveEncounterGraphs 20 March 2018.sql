IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetActiveEncounterGraphs')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetActiveEncounterGraphs
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetActiveEncounterGraphs]    Script Date: 20-03-2018 18:00:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetActiveEncounterGraphs]
(
	@pFacilityID int,  --- PatientID for whom the Data is requested  
	@pDisplayTypeID int, --- Display Value on X Axis to be Seen VALID Entries --> 0 = OpenVsClosed, 1=EncounterPatientType, 2 = Mode Of Arrival, 3 = Encounter EndType, 4 = Attending Physician  
	@pFromDate datetime,  --- If NULL Then Default picked Start of Year --- If Passed then(will be treated as FROM Date) which will be the reference point from Data 
	@pTillDate datetime  --- If NULL Then Default to Today --- If Passed then(will be treated as TILL Date) 
)
AS
BEGIN

Declare @CurrentDate datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

--- Check and SET the SELECTION Date  
 If @pFromDate is NULL  
  Set @pFromDate = DATEADD(Year, DATEDIFF(Year, 0, @CurrentDate), 0)  

If @pTillDate is NULL  
  Set @pTillDate = @CurrentDate  

---- Following line takes care of Todays entered data as well because it will pick till Mid Night
  Set @pTillDate =  DATEADD(DAY,1,@pTillDate)

Declare   @GraphData Table(Descrip nvarchar(50),Value numeric(15,2), Total numeric(15,2))

---- Encounter type Graph Data
Insert Into @GraphData
Select max(GC.GlobalCodeName) 'XAxis',Count(E.EncounterPatientType) 'YAxis',1 from [dbo].[Encounter] E
	inner join GlobalCodes GC on GC.GlobalCodeValue = E.EncounterPatientType and GlobalCodeCategoryValue = '1107'
	Where E.EncounterFacility = @pFacilityID and E.EncounterEndTime is NULL 
	Group by E.EncounterPatientType

----- Attendening physician type graph data
Insert Into @GraphData
Select max(GC.PhysicianName) 'XAxis',Count(E.EncounterAttendingPhysician) 'YAxis',2 from [dbo].[Encounter] E
	inner join Physician GC on GC.ID = E.EncounterAttendingPhysician
	Where E.EncounterFacility = @pFacilityID and E.EncounterEndTime is NULL -- and E.EncounterRegistrationDate between @pFromDate and @pTillDate
	Group by E.EncounterAttendingPhysician


Select * from @GraphData order by Total

Delete from @GraphData

END












GO


