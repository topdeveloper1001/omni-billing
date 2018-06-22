IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_Get_REP_LoginTimeDayNightWise')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_Get_REP_LoginTimeDayNightWise
GO

/****** Object:  StoredProcedure [dbo].[SPROC_Get_REP_LoginTimeDayNightWise]    Script Date: 20-03-2018 18:04:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_Get_REP_LoginTimeDayNightWise]     --[SPROC_Get_REP_LoginTimeDayNightWise] 0,4,null,null,1120
(  
@pCorporateID int,      

@pFacilityID int,    

@pFromDate Datetime,

@pTillDate Datetime,

@pUserId int
)      
AS      
BEGIN  
		DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))
		 
Declare @LoginData table (UserID int, UserName Nvarchar(100), LoginDate date,ShiftType nvarchar(20),LoggedinMinutes int)

Set @pFromDate = isnull(@pFromDate, @LocalDateTime)

Set @pTillDate = isnull(@pTillDate, DATEADD(DAY,1,@pFromDate))


insert into @LoginData
	Select ID, max(FirstName+' '+LastName),Cast(Logintime as Date),CASE WHEN Cast(Logintime as Time) between '06:00:00' and '18:00:00' THEN 'DayShift' ELSE 'NightShift' END, 
	sum(isnull(DATEDIFF(Minute,Logintime,LogoutTime),0)) from loginTracking
	inner join Users on UserID = ID ----- and CorporateID = @pCorporateID and FacilityId = @pFacilityID
	Where Logintime between @pFromDate and @pTillDate 
	And (@pCorporateID = 0 or Users.CorporateId = @pCorporateID)
	And ( @pUserId = 0 OR Users.UserID = @pUserId )
	Group by ID,Logintime,LogoutTime


;With Report
AS        
(        
select * from         
(        

Select * from @LoginData

) src  
pivot        
(        
Sum(LoggedinMinutes)        

for ShiftType in (DayShift,NightShift)        

)piv        

)	


Select UserID,UserName,LoginDate,isnull(DayShift,0) 'DayShiftMinutes',isnull(NightShift,0) 'NightShiftMinutes' from Report order by UserName ,LoginDate desc;

END












GO


