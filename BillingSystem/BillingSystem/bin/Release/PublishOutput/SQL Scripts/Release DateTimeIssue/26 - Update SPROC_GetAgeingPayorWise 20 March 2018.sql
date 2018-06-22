IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetAgeingPayorWise')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetAgeingPayorWise
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetAgeingPayorWise]    Script Date: 20-03-2018 17:37:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_GetAgeingPayorWise]    
(      
@pCorporateID int= 9,        
@pFacilityID int =8,    
@pAsOnDate Datetime   =null   
)      

AS      
  
BEGIN    

	Declare @CurrentDate datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

	Set @pAsOnDate = isnull(@pAsOnDate,@CurrentDate)    
 
 Declare @FinalResults Table(ID int,Name Nvarchar(50), OnTime numeric(9,2), Days1To30 numeric(9,2),Days31To60 numeric(9,2), 
Days61To90 numeric(9,2),Days91To120 numeric(9,2) ,Days121To150 numeric(9,2),Days151To180 numeric(9,2), Days181More numeric(9,2))

  
 Declare  @Temp3 Table          
 (ID int, Name Nvarchar(50), Delayed int, TBRAmount numeric(9,2) )    
  
--INSERT INTO #Temp3 Select  INS.InsuranceCompanyId 'PayorID',max(INS.InsuranceCompanyName) 'PayorName',    
--Case when MAX(EN.EncounterEndTime) IS NULL AND MAX(BA.DueDate) IS NULL THEN -1 ELSE  DATEDIFF(DAY,Cast(BA.DueDate as Date),@pAsOnDate) END,isnull(sum((BA.PatientShare + BA.PayerShareNet) -(isnull(BA.PaymentAmount,0)+isnull(BA.PatientPayAmount,0))),0) 'Charges' from BillHeader BA    
--inner join PatientInfo PInfo on PInfo.PatientID = BA.PatientID    
----inner join InsuranceCompany Ins on Ins.InsuranceCompanyId = PInfo.PersonInsuranceCompany    
--INNER JOIN PatientInsurance PINS on PINS.PatientID = BA.PatientID  
--INNER join InsuranceCompany Ins on Ins.InsuranceCompanyId = PINS.InsuranceCompanyId
--INNER JOIN Encounter EN on EN.EncounterID = BA.EncounterID
--Where BA.BillDate is not null and BA.CorporateID = @pCorporateID and BA.FacilityID = @pFacilityID    
--Group by INS.InsuranceCompanyId,Cast(BA.DueDate as Date) 
--Having isnull(sum((BA.PatientShare + BA.PayerShareNet)-isnull(BA.PaymentAmount,0)-isnull(BA.PatientPayAmount,0)),0)>0


INSERT INTO @Temp3 Select  INS.InsuranceCompanyId 'PayorID',max(INS.InsuranceCompanyName) 'PayorName',    
Case when MAX(EN.EncounterEndTime) IS NULL AND MAX(BA.DueDate) IS NULL THEN -1 ELSE  DATEDIFF(DAY,Cast(BA.DueDate as Date),@pAsOnDate) END,isnull(sum((BA.PayerShareNet) -(isnull(BA.PaymentAmount,0))),0) 'Charges' from BillHeader BA    
inner join PatientInfo PInfo on PInfo.PatientID = BA.PatientID    
INNER JOIN PatientInsurance PINS on PINS.PatientID = BA.PatientID  
INNER join InsuranceCompany Ins on Ins.InsuranceCompanyId = PINS.InsuranceCompanyId
INNER JOIN Encounter EN on EN.EncounterID = BA.EncounterID
Where BA.BillDate is not null and BA.CorporateID = @pCorporateID and BA.FacilityID = @pFacilityID  
AND ((EN.EncounterEndTime is not null and BA.DueDate is not null) OR (EN.EncounterEndTime is null and BA.DueDate is null))  
Group by INS.InsuranceCompanyId,Cast(BA.DueDate as Date) 
Having isnull(sum((BA.PayerShareNet)-isnull(BA.PaymentAmount,0)),0)>0
   
  
--Select * from #Temp3    
;With Report         
AS        
(        
select * from         
(        
select ID, Name,(CASE  WHEN (Delayed>=0 and Delayed<=30) then'B1' WHEN (Delayed>31 and Delayed<=60) then'B2' WHEN (Delayed>61 and Delayed<=90) then'B3'  
WHEN (Delayed>91 and Delayed<=120) then'B4' WHEN (Delayed>121 and Delayed<=150) then'B5' WHEN (Delayed>151 and Delayed<=180) then'B6' WHEN (Delayed>181) then'B7' ELSE 'OnTime' END)Delayed, TBRAmount        
from @Temp3      
) src        
 
pivot        
(        
 SUM(TBRAmount)        
 for Delayed in (Ontime,B1,B2,B3,B4,B5,B6,B7)        
)piv        
)		
  
insert into @FinalResults
Select ID as 'PayorID', Name as 'PayorName', Isnull(SUM(Ontime),0.00) 'Ontime',Isnull(SUM(B1),0.00) AS 'Days1To30',
Isnull(SUM(B2),0.00) 'Days31To60',Isnull(SUM(B3),0.00) 'Days61To90',Isnull(SUM(B4),0.00) 'Days91To120', Isnull(SUM(B5),0.00) 'Days121To150',  
Isnull(SUM(B6),0.00) 'Days151To180',Isnull(SUM(B7),0.00) 'Days181More' from Report 
Group by ID,Name
order by ID  


IF EXISTS (Select 1 from @FinalResults)
BEGIN
------- Now Insert Totals
insert into @FinalResults
Select 99999999, 'ZZZZZ',sum(OnTime),sum(Days1To30),sum(Days31To60),sum(Days61To90),sum(Days91To120),sum(Days121To150),sum(Days151To180),sum(Days181More)
from @FinalResults
END

--- Display Results
Select *,(OnTime+Days1To30+Days31To60+Days61To90+Days91To120+Days121To150+Days151To180+Days181More) 'Total' from @FinalResults

  
END










GO


