IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetDBRegistrationProductivity')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetDBRegistrationProductivity
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetDBRegistrationProductivity]    Script Date: 20-03-2018 17:11:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SPROC_GetDBRegistrationProductivity]  
(  
@pFacilityID int,  --- FacilityID for whom the Data is requested  
@pDisplayTypeID int, --- Display Type to be Seen VALID Entries --> 0 = Year To Date, 1= Month To Date  -- If NULL Default is Year To Date
@pTillDate datetime  --- If NULL Then Default to Today --- If Passed then(will be treated as TILL Date) 
)  
AS  
BEGIN  
    Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

Declare @FromDate Datetime, @NumberOfMonths int
--- Check and SET the SELECTION Date  

---- Set Defaults if Params sent as NULL
Set @pDisplayTypeID = isnull(@pDisplayTypeID,0)
Set @pTillDate = isnull(@pTillDate,@LocalDateTime)


Set @pTillDate =  DATEADD(DAY,1,Cast(@pTillDate as Date))
---- Following line takes care of Todays entered data as well because it will pick till Mid Night
if MONTH(@pTillDate) = 1 AND DAY(@pTillDate) = 1
	Set @pTillDate =  DATEADD(minute,-1,@pTillDate)
 
 --- Selection Based on Display Type  

If @pDisplayTypeID = 0
Begin
------- YEAR to DATE 
Set @FromDate = DATEADD(Year, DATEDIFF(Year, 0, @pTillDate), 0) 
Set @NumberOfMonths = MONTH(@pTillDate)

Select (Select isnull(max(UserName),'SYS') from Users Where UserID = Patientinfo.CreatedBy) 'UserName', Count(1) 'Registered',([dbo].[GetUserTarget](FacilityID,CreatedBy,'910')*@NumberOfMonths) 'Target' ,CreatedBy,FacilityID  
from PatientInfo Where FacilityID = @pFacilityID and CreatedDate between @FromDate and @pTillDate
Group by FacilityID, CreatedBy 
Order by FacilityId, CreatedBy

End

If @pDisplayTypeID = 1
Begin
------- MONTH to DATE 
Set @FromDate = DATEADD(MONTH, DATEDIFF(MONTH, 0, @pTillDate), 0) 

Select (Select isnull(max(UserName),'SYS') from Users Where UserID = Patientinfo.CreatedBy) 'UserName', Count(1) 'Registered',[dbo].[GetUserTarget](FacilityID,CreatedBy,'910') 'Target' ,CreatedBy,FacilityID  
from PatientInfo Where FacilityID = @pFacilityID and CreatedDate between @FromDate and @pTillDate
Group by FacilityID, CreatedBy 
Order by FacilityId, CreatedBy

End


END ---- Procedure Ends













GO


