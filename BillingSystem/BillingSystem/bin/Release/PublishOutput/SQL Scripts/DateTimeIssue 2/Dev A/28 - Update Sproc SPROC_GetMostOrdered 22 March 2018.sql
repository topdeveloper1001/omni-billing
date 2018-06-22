
IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetMostOrdered')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetMostOrdered
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetMostOrdered]    Script Date: 3/22/2018 7:58:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_GetMostOrdered]
(  
@pPhysicianID int,  
@pNumberOfDaysBack int  
)  
AS  
BEGIN  
Declare  @Facility_Id int =(SELECT FacilityId  from dbo.Physician where Id=@pPhysicianID)
 		DECLARE @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))
 Declare @DateFrom datetime  
   
 --- SET the SELECTION Date from - STARTS  
 -- Set the Date for selection as per Number of Days passed in -- Default to 365 (1 Year)  
  
 Set @pNumberOfDaysBack = isnull(@pNumberOfDaysBack,0)  
  
 If @pNumberOfDaysBack = 0  
  Set @pNumberOfDaysBack = 365  
  
 Set @pNumberOfDaysBack = (@pNumberOfDaysBack * -1)  
 SET @DateFrom = DATEADD(day,@pNumberOfDaysBack,@LocalDateTime)  
   
 --- SET the SELECTION Date from - ENDS  
         
    --- Query to Return a List as requested  
  
      
Select DISTINCT OO.PhysicianID,Count (OO.OpenOrderID) OrderCount,OO.OrderType, MAX(GC.GlobalCodeName) OrderTypeName,OO.OrderCode, Max([dbo].[GetOrderDescription](OO.OrderType,OO.OrderCode)) OrderDescription  
from [dbo].[OpenOrder] OO  
inner join GlobalCodes GC on GC.GlobalCodeValue = OO.OrderType and GC.GlobalCodeCategoryValue = '1201'  
Where PhysicianID = @pPhysicianID and OpenOrderPrescribedDate >= @DateFrom  
Group by OO.PhysicianID,OO.OrderType,OO.OrderCode  
ORDER by OO.PhysicianID,OrderCount desc, OO.OrderType,OO.OrderCode  
            
END  












GO


