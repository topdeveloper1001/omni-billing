
/****** Object:  StoredProcedure [dbo].[SPROC_GetPatientAgeingPayorWise]    Script Date: 21-03-2018 13:19:30 ******/
IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetPatientAgeingPayorWise')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[SPROC_GetPatientAgeingPayorWise]
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetPatientAgeingPayorWise]    Script Date: 21-03-2018 13:19:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SPROC_GetPatientAgeingPayorWise]
(  
@pCorporateID int,  
@pFacilityID int,
@pAsOnDate Datetime,
@PayorID varchar(5)
)  
AS  
BEGIN
		DECLARE @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))
		Set @pAsOnDate = isnull(@pAsOnDate,@LocalDateTime)


Declare @FinalResults Table(ID int,Name Nvarchar(50),EncounterEnd datetime, DueDate Datetime, EncounterNumber Nvarchar(100),PayorId int, OnTime numeric(9,2), Days1To30 numeric(9,2),Days31To60 numeric(9,2), 
		Days61To90 numeric(9,2),Days91To120 numeric(9,2) ,Days121To150 numeric(9,2),Days151To180 numeric(9,2), Days181More numeric(9,2))

 Create Table #Temp2     
 (    
 ID int, Name Nvarchar(50),  EncounterNumber Nvarchar(100), Delayed int,TBRAmount numeric(9,2) ,EncounterEnd datetime, DueDate datetime,PayorId int)

--INSERT INTO #Temp2 Select  BA.PatientID 'PatientID',max(PInfo.PersonLastName +'-'+ PInfo.PersonFirstName) 'PatientName',max(EN.EncounterNumber) 'EncounterNumber',
--DATEDIFF(DAY,Cast(BA.DueDate as Date),@pAsOnDate),isnull(sum((BA.PatientShare + BA.PayerShareNet)-isnull(BA.PaymentAmount,0)-isnull(BA.PatientPayAmount,0)),0) 'Charges',
--MAX(EncounterEndTime) 'EncounterEnd', MAX(BA.DueDate) 'DueDate',MAX(INS.InsuranceCompanyId) 'PayorID'
--from BillHeader BA
--inner join PatientInfo PInfo on PInfo.PatientID = BA.PatientID
--INNER JOIN Encounter EN on EN.EncounterID = BA.EncounterID
--INNER JOIN PatientInsurance PINS on PINS.PatientID = BA.PatientID  
--INNER join InsuranceCompany Ins on Ins.InsuranceCompanyId = PINS.InsuranceCompanyId
----inner join InsuranceCompany Ins on Ins.InsuranceCompanyId = PInfo.PersonInsuranceCompany  
--Where BA.BillDate is not null and BA.CorporateID = @pCorporateID and BA.FacilityID = @pFacilityID
--Group by BA.PatientID,Cast(BA.DueDate as Date)
--Having isnull(sum(BA.Gross-isnull(BA.PaymentAmount,0)-isnull(BA.PatientPayAmount,0)),0)>0

INSERT INTO #Temp2 Select  BA.PatientID 'PatientID',max(PInfo.PersonLastName +'-'+ PInfo.PersonFirstName) 'PatientName',max(EN.EncounterNumber) 'EncounterNumber',
DATEDIFF(DAY,Cast(BA.DueDate as Date),@pAsOnDate),isnull(sum((BA.PayerShareNet)-isnull(BA.PaymentAmount,0)),0) 'Charges',
MAX(EncounterEndTime) 'EncounterEnd', MAX(BA.DueDate) 'DueDate',MAX(INS.InsuranceCompanyId) 'PayorID'
from BillHeader BA
inner join PatientInfo PInfo on PInfo.PatientID = BA.PatientID
INNER JOIN Encounter EN on EN.EncounterID = BA.EncounterID
INNER JOIN PatientInsurance PINS on PINS.PatientID = BA.PatientID  
INNER join InsuranceCompany Ins on Ins.InsuranceCompanyId = PINS.InsuranceCompanyId
--inner join InsuranceCompany Ins on Ins.InsuranceCompanyId = PInfo.PersonInsuranceCompany  
Where BA.BillDate is not null and BA.CorporateID = @pCorporateID and BA.FacilityID = @pFacilityID
AND ((EN.EncounterEndTime is not null and BA.DueDate is not null) OR (EN.EncounterEndTime is null and BA.DueDate is null))
Group by BA.PatientID,Cast(BA.DueDate as Date)
Having isnull(sum(BA.Gross-isnull(BA.PaymentAmount,0)),0)>0

;With Report     
AS    
(    
select * from     
(    
select ID, Name,EncounterEnd,DueDate,EncounterNumber,PayorID,(CASE  WHEN (Delayed>=0 and Delayed<=30) then'B1' WHEN (Delayed>30 and Delayed<=60) then'B2'
 WHEN (Delayed>60 and Delayed<=90) then'B3' WHEN (Delayed>90 and Delayed<=120) then'B4' WHEN (Delayed>120 and Delayed<=150) then'B5' 
 WHEN (Delayed>150 and Delayed<=180) then'B6' WHEN (Delayed>180) then'B7' ELSE 'OnTime' END)Delayed, TBRAmount    
from #Temp2    
) src    
pivot    
(    
MAX(TBRAmount)    
for Delayed in (Ontime,B1,B2,B3,B4,B5,B6,B7)    
)piv    
)


insert into @FinalResults
Select ID as 'PatientID', Name as 'PatientName',EncounterEnd as 'EncounterEnd',DueDate as 'DueDate', EncounterNumber 'EncounterNumber',PayorID 'PayorID', Isnull(Ontime,0.00) 'Ontime',
Isnull(B1,0.00) 'Days1To30', Isnull(B2,0.00) 'Days31To60',Isnull(B3,0.00) 'Days61To90',Isnull(B4,0.00) 'Days91To120', Isnull(B5,0.00) 'Days121To150',
Isnull(B6,0.00) 'Days151To180',Isnull(B7,0.00) 'Days181More'
from Report order by ID


if(CONVERT(int,ISnull(@PayorID,0)) <> 0)
BEGIN 
----- Now Insert Totals
insert into @FinalResults
Select 99999999, 'ZZZZZ',null,NULL,'TOTAL',MAX(PayorID),sum(OnTime),sum(Days1To30),sum(Days31To60),sum(Days61To90),sum(Days91To120),sum(Days121To150),sum(Days151To180),sum(Days181More)
from @FinalResults 
where PayorID = CONVERT(int,ISnull(@PayorID,0))

--- Display Results
Select *,(OnTime+Days1To30+Days31To60+Days61To90+Days91To120+Days121To150+Days151To180+Days181More) 'Total' from @FinalResults where PayorID = CONVERT(int,ISnull(@PayorID,0))
and ((EncounterEnd is not null and DueDate is not null) OR (EncounterEnd is null and DueDate is null))
END
ELSE
BEGIN
----- Now Insert Totals
insert into @FinalResults
Select 99999999, 'ZZZZZ',null,NULL,'TOTAL',null,sum(OnTime),sum(Days1To30),sum(Days31To60),sum(Days61To90),sum(Days91To120),sum(Days121To150),sum(Days151To180),sum(Days181More)
from @FinalResults

--- Display Results
Select *,(OnTime+Days1To30+Days31To60+Days61To90+Days91To120+Days121To150+Days151To180+Days181More) 'Total' from @FinalResults
Where ((EncounterEnd is not null and DueDate is not null) OR (EncounterEnd is null and DueDate is null))
END



Drop table #Temp2;


END









GO


