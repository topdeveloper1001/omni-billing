 
CREATE PROCEDURE [dbo].[SPROC_GetOrderActivityDetailsByOrderActivityID]
(  
@pOrderActivityId int
)  
AS  
BEGIN  
 Select O.*,E.EncounterStartTime EncounterStartDate,E.EncounterEndTime EncounterEndDate
 From OrderActivity O  
 Inner join Encounter E  ON O.EncounterID = E.EncounterId  
 Where O.OrderActivityId = @pOrderActivityId Order by 1 desc  
END  
  
  
  
  
  