IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetDBEncounter')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetDBEncounter
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetDBEncounter]    Script Date: 20-03-2018 17:15:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_GetDBEncounter]  
(  
@pFacilityID int,  --- PatientID for whom the Data is requested  
@pDisplayTypeID int, --- Display Value on X Axis to be Seen VALID Entries --> 0 = OpenVsClosed, 1=EncounterPatientType, 2 = Mode Of Arrival, 3 = Encounter EndType, 4 = Attending Physician  
@pFromDate datetime,  --- If NULL Then Default picked Start of Year --- If Passed then(will be treated as FROM Date) which will be the reference point from Data 
@pTillDate datetime  --- If NULL Then Default to Today --- If Passed then(will be treated as TILL Date) 
)  
AS  
BEGIN  
     
    Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

 --- Check and SET the SELECTION Date  
 If @pFromDate is NULL  
  Set @pFromDate = DATEADD(Year, DATEDIFF(Year, 0, @LocalDateTime), 0)  

If @pTillDate is NULL  
  Set @pTillDate = @LocalDateTime 

  ---- Following line takes care of Todays entered data as well because it will pick till Mid Night
  Set @pTillDate =  DATEADD(DAY,1,@pTillDate)
  
 --- Selection Based on Display Type  



  -- EncounterPatientType 
 If @pDisplayTypeID = 1  
 BEGIN  
   
	Select 'EncounterType' as 'Title',E.EncounterPatientType 'Code', max(GC.GlobalCodeName) 'XAxis',Count(E.EncounterPatientType) 'YAxis' from [dbo].[Encounter] E
	inner join GlobalCodes GC on GC.GlobalCodeValue = E.EncounterPatientType and GlobalCodeCategoryValue = '1107'
	Where E.EncounterFacility = @pFacilityID and E.EncounterEndTime is NULL -- and E.EncounterRegistrationDate between @pFromDate and @pTillDate
	Group by E.EncounterPatientType

 END  
  
  -- ModeOfArrivals 
 If @pDisplayTypeID = 2  
 BEGIN  
   
	Select 'ModeOfArrivals' as 'Title',E.EncounterModeofArrival 'Code', max(GC.GlobalCodeName) 'XAxis',Count(E.EncounterModeofArrival) 'YAxis' from [dbo].[Encounter] E
	inner join GlobalCodes GC on GC.GlobalCodeValue = E.EncounterModeofArrival and GlobalCodeCategoryValue = '1106'
	Where E.EncounterFacility = @pFacilityID and E.EncounterEndTime is NULL -- and E.EncounterRegistrationDate between @pFromDate and @pTillDate
	Group by E.EncounterModeofArrival

 END  

  -- Encounter EndType
 If @pDisplayTypeID = 3  
 BEGIN  
   
	Select 'Encounter End Type' as 'Title',E.EncounterEndType 'Code', max(GC.GlobalCodeName) 'XAxis',Count(E.EncounterEndType) 'YAxis' from [dbo].[Encounter] E
	inner join GlobalCodes GC on GC.GlobalCodeValue = E.EncounterEndType and GlobalCodeCategoryValue = '1301'
	Where E.EncounterFacility = @pFacilityID and E.EncounterEndTime is NULL -- and E.EncounterRegistrationDate between @pFromDate and @pTillDate
	Group by E.EncounterEndType

 END  
  
 -- Attending Physician
 If @pDisplayTypeID = 4  
 BEGIN  
   
	Select 'Attending Physician' as 'Title',E.EncounterAttendingPhysician 'Code', max(GC.PhysicianName) 'XAxis',Count(E.EncounterAttendingPhysician) 'YAxis' from [dbo].[Encounter] E
	inner join Physician GC on GC.ID = E.EncounterAttendingPhysician
	Where E.EncounterFacility = @pFacilityID and E.EncounterEndTime is NULL -- and E.EncounterRegistrationDate between @pFromDate and @pTillDate
	Group by E.EncounterAttendingPhysician

 END  
 
            
END  













GO


