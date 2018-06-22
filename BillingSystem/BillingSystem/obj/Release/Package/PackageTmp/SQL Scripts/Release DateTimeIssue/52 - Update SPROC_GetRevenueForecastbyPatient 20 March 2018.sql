IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetRevenueForecastbyPatient')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetRevenueForecastbyPatient
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetRevenueForecastbyPatient]    Script Date: 21-03-2018 11:07:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  

CREATE PROCEDURE [dbo].[SPROC_GetRevenueForecastbyPatient]
(  
@pPatientID int  
)  
AS  
BEGIN

-- Declare @pEncounterID int,@ComputedTill datetime


--Set @ComputedTill = '2014-12-19'

--Set @pEncounterID = 303				------- AmanKaur 261 and Gurdip Singh = 255

----Select OrderActivityStatus, sum(OrderActivityQuantity) from OrderActivity 
----Where EncounterID = 261
----Group by OrderActivityStatus
--Select * from OrderActivity Where EncounterID = 261 and Executedby is not null order by 1 desc
-- Select * from PatientInfo where PersonFirstname like 'Gur%' --- PID = 258 and EID = 261   ---- Gurdip Singh PID= 245 - EID = 255
-- Select * from Encounter where PatientID = 245

-- Select * from Transactions order by 1 desc
------ Exec [dbo].[SPROC_GetMainIDSEncounterBased] @pEncounterID,@CorporateID Output,@FacilityID Output,@PatientID Output,@InsuranceCompanyID Output,@InsurancePayerID output,@InsurancePlanID Output,@InsurancePolicyID Output,@AuthID Output,@Multiplier Output 
------Set @CurrentDate = @LocalDateTime

-- Drop table #T2


Create table #T2
( PatientID int,EncounterID INT, TRDate date, TRType nvarchar(20),OrderType int, OrderCode nvarchar(20),OrderCategoryID int,OrderSubCategoryID int,
Quantity int, UnitPrice decimal(18,2), Gross decimal(18,2),OrderStatus int)

Declare @CorporateId int, @FacilityId  int

DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@FacilityId))

Select @FacilityId = CAST(FacilityId as INT), @CorporateId = CorporateID  From PatientInfo where PatientId = @pPatientID



----- ORDERS -----STARTS
insert into #T2
Select PatientID, EncounterID,Cast(PlannedDate as Date), 
(CASE  WHEN  OrderActivityStatus in (0,1) Then 'Planned' WHEN OrderActivityStatus in (2,3) Then 'Closed' WHEN OrderActivityStatus = 9 Then 'Cancelled' ELSE 'OnBill' END) ,
OrderType,OrderCode,max(OrderCategoryID),max(OrderSubCategoryID), 
sum(OrderActivityQuantity),dbo.GetUnitPrice(Cast (OrderType as nvarchar(20)),OrderCode,@CorporateId,@FacilityId,@LocalDateTime),
(sum(OrderActivityQuantity) * dbo.GetUnitPrice(Cast (OrderType as nvarchar(20)),OrderCode,@CorporateId,@FacilityId,@LocalDateTime)),
OrderActivityStatus
from [dbo].[OrderActivity]
--Where CorporateID = @pCorporateID and FacilityID = @pFacilityID
Where PatientID = @pPatientID
Group by PatientID, EncounterID, Cast(PlannedDate as Date),OrderActivityStatus ,OrderType,OrderCode
order by Cast(PlannedDate as Date) Desc

---- Now Insert Total per day
insert into #T2 (PatientID,EncounterID, TRDate,TRType,Quantity,Gross)
Select PatientID, EncounterID,TRDate,'EXPECTED',sum(Quantity),sum(Gross) from #T2 
group by PatientID, EncounterID,TRDate;

----- ORDERS -----ENDS

--- BillActivity -- STARTS
-- Select * from BillActivity
-- Select * from BillHeader

insert into #T2 
Select PatientID,EncounterID,Cast(BillDate as Date),
(CASE  WHEN  [Status] = '0' Then 'BillConsolidation' WHEN [Status] = '1' Then 'BillApproved' WHEN [Status] = '2' Then 'BillSentforClaim' WHEN [Status] = '1170' Then 'BillPrelimnary' ELSE 'BillConsolidation' END),
'','','','',0,0,max(Gross),[Status]
from BillHeader
--Where CorporateID = @pCorporateID and FacilityID = @pFacilityID
Where PatientID = @pPatientID
Group by PatientID, EncounterID, Cast(BillDate as Date),[Status]
order by Cast(BillDate as Date) Desc
--- BillActivity -- ENDS

;With Report     
AS    
( 
Select * from ( 
select PatientID, EncounterID,TRType,TRDate,Gross from  #T2
) src
pivot    
(    
  Sum(Gross)    
  for TRType in (EXPECTED,Planned,Closed,Cancelled,OnBill,BillConsolidation,BillPrelimnary,BillApproved,BillSentforClaim,PaymentReceived)    
)piv    
)


Select R.PatientID,P.PersonFirstName, P.PersonLastName,R.EncounterID,E.EncounterNumber, R.TRDate,EXPECTED,Planned,Closed,Cancelled,OnBill,BillConsolidation,BillPrelimnary,BillApproved,BillSentforClaim,PaymentReceived from Report R
inner join patientinfo P on P.PatientID = R.PatientID
inner join Encounter E on E.EncounterID = R.EncounterID
order by R.PatientID,R.EncounterID,R.TRDate;

END
 --- Select * from #T2 order by OrderStatus;

------ ORDER STATUS --- ENDS












GO


