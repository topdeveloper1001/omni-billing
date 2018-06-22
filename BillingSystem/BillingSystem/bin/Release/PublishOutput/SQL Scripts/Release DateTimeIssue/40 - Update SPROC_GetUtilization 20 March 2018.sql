IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetUtilization')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetUtilization
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetUtilization]    Script Date: 20-03-2018 18:41:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_GetUtilization]   --- Exec  [SPROC_GetUtilization]
(      
@pCorporateID int =6,     ----- Corporate ID (MUST Pass)
@pFromDate nvarchar(20)   ='2016-05-01',  ---- Date From (MUST Pass)
@pTillDate nvarchar(20)   ='2016-05-30',  ---- Date Till (MUST Pass)
@pDisplayFlag int =1,   ----  1 = Physician Utilization, 2 = Department Utilization (MUST Pass)
@pFacilityID int =0,   ---- Facility ID (Optional)
@pPhysicianID int =0,   ---- Physician ID (Optional)
@pDepartmentID int =0   ---- Physician ID (Optional)    
)      

AS      
  
BEGIN    
	
 ----- Table to return results as final outcome
 Declare @FinalResults Table(ID int,Name Nvarchar(50), ScheduledDate date, TSTotal numeric(9,2),TSConfirmed numeric(9,2), TSDone numeric(9,2),TSCancelled numeric(9,2), 
		PerConfirmed numeric(9,2),PerDone numeric(9,2) ,PerCancelled numeric(9,2))

 --- Temp Table to manuplate for data as per outcome
 Declare @TempSchedules Table (ID int, SchDate date, SchStatus nvarchar(50), SchDuration numeric(9,2),EffectedSlots numeric(9,2) )  

 ------ LOCAL VARIABLES HERE --- STARTS
 ---- NOTE: Average Time Slot Variables below should be picked from GlobalCode Setup table - To be done later
 Declare @AvgTimeSlot numeric(9,2) = 15.00, @TotalSlotsPerDay numeric(9,2) = 25.00, @SQL1 as nvarchar(max);

------ LOCAL VARIABLES HERE --- ENDS    

---- Set Defaults below
Select @pFacilityID = isnull(@pfacilityID,0), @pDepartmentID = isnull(@pDepartmentID,0), @pPhysicianID = isnull(@pPhysicianID,0)

Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))
----- Prepare SQL as per passed in Params 
Set @SQL1 = 'Select PhysicianID, Cast(ScheduleFrom as Date), (CASE WHEN ([Status] = 1 and ScheduleFrom <= @LocalDateTime) Then ' +'99'+' ELSE [Status] END), 
datediff(minute,ScheduleFrom,ScheduleTo),0 
from Scheduling where CorporateID = '+ Cast(@pCorporateID as Nvarchar(10)) +' AND ScheduleFrom between ''' + @pFromDate + ''' and ''' + @pTillDate +''''

----- Conditonal Where Clauses -- STARTS
If @pFacilityID > 0 Set @SQL1 = @SQL1 + ' AND FacilityID = ' + Cast(@pFacilityID as Nvarchar(10))
If @pPhysicianID > 0 Set @SQL1 = @SQL1 + ' AND PhysicianID = ' + Cast(@pPhysicianID as Nvarchar(10))
---- NOTE:  Following Department ID field need to be decided (either to use Extval or add new) - Recommend that it is insterted at time of creating a schedule in Table Scheduling 
---If @pDepartmentID > 0 Set @SQL1 = @SQL1 + ' AND >>>XXXXX<<< = ' + Cast(@pDepartmentID as Nvarchar(10))
----- Conditonal Where Clauses -- ENDS

---- Getting Scheduled Records
INSERT INTO @TempSchedules Execute (@SQL1);

--- Set EffectedSlots
Update @TempSchedules Set EffectedSlots = SchDuration / @AvgTimeSlot
  
-- Select * from #TempSchedules 
 
  
;With Report         
AS        
(        
select * from         
(        
select ID, Schdate,(CASE  WHEN (SchStatus = 1) then 'Confirmed' WHEN (SchStatus = 3) then'Cancelled' ELSE 'Done' END) SchStatus, EffectedSlots --Case Need to be changed       
from @TempSchedules -- Group by ID,Schdate   
) src        
 
pivot        
(        
 SUM(EffectedSlots)        
 for SchStatus in (Confirmed,Cancelled,Done)        
)piv        
)		
--Select * from Report

insert into @FinalResults
Select Report.ID, P.PhysicianName, SchDate,@TotalSlotsPerDay,(Isnull(Confirmed,0.00)+Isnull(Done,0.00)),Isnull(Done,0.00),Isnull(Cancelled,0.00),
((Isnull(Confirmed,0.00)+Isnull(Done,0.00))/@TotalSlotsPerDay)*100.00,(Isnull(Done,0.00)/@TotalSlotsPerDay)*100.00,(Isnull(Cancelled,0.00)/@TotalSlotsPerDay)*100.00
from Report 
inner join Physician P on P.ID = Report.ID 
order by Report.ID

----->>>>> Handling TOTALS --- STARTS
IF EXISTS (Select 1 from @FinalResults)
BEGIN
------- Now Insert Totals
---- ONLY doing TOTAL of Whole report --- Breaks on Physician and Department Totals need to be handled in similar fashion 
insert into @FinalResults
Select 99999999, 'TOTAL','',sum(TSTotal),sum(TSConfirmed),sum(TSDone),sum(TSCancelled),
(sum(TSConfirmed)/sum(TSTotal))*100,(sum(TSDone)/sum(TSTotal))*100,(sum(TSCancelled)/sum(TSTotal))*100
from @FinalResults
 END

----->>>>> Handling TOTALS --- ENDS
insert into @FinalResults
Select 99999999, 'Total Of The Report','',sum(TSTotal),sum(TSConfirmed),sum(TSDone),sum(TSCancelled),
(sum(TSConfirmed)/sum(TSTotal))*100,(sum(TSDone)/sum(TSTotal))*100,(sum(TSCancelled)/sum(TSTotal))*100
from @FinalResults where Name='TOTAL'
  
--- Display Results
Select * from @FinalResults


  
END





GO


