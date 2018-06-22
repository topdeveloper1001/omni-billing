IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetAgeingDepartmentWise')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetAgeingDepartmentWise
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetAgeingDepartmentWise]    Script Date: 20-03-2018 17:59:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SPROC_GetAgeingDepartmentWise]
(  
@pCorporateID int,  
@pFacilityID int,
@pAsOnDate Datetime  
)  
AS  
BEGIN

Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

Set @pAsOnDate = isnull(@pAsOnDate,@LocalDateTime)
--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[#Temp1]') AND type in (N'U'))
--Drop Table #temp1
 Create Table #Temp1     
 (    
 ID Nvarchar(20),
 Name Nvarchar(50),     
 Delayed int,    
 TBRAmount numeric(9,2)    
 )

INSERT INTO #Temp1 Select  OO.CategoryId 'DepartmentNumber',GCC.GlobalCodeCategoryName 'DepartmentName',
DATEDIFF(DAY,BA.ActivityStartDate,@pAsOnDate),isnull(sum((BA.PatientShare + BA.PayerShareNet)-isnull(BA.PaymentAmount,0)-isnull(BA.PatientPayAmount,0)),0) 'Charges' from BillActivity BA
inner join OpenOrder OO on OO.OpenOrderID = BA.ActivityID
inner join GlobalCodeCategory GCC on GCC.GlobalCodeCategoryValue = OO.CategoryId
Where BA.ActivityStartDate is not null and BA.CorporateID = @pCorporateID and BA.FacilityID = @pFacilityID
Group by OO.CategoryId,GCC.GlobalCodeCategoryName,BA.ActivityStartDate
Having isnull(sum(BA.Gross-isnull(BA.PaymentAmount,0)-isnull(BA.PatientPayAmount,0)),0)>0

;With Report     
AS    
(    
select * from     
(    
select ID, Name,(CASE  WHEN (Delayed>0 and Delayed<=30) then'B1' WHEN (Delayed>30 and Delayed<=60) then'B2' 
WHEN (Delayed>60 and Delayed<=90) then'B3' WHEN (Delayed>90 and Delayed<=120) then'B4' WHEN (Delayed>120 and Delayed<=150) then'B5' 
WHEN (Delayed>150 and Delayed<=180) then'B6' WHEN (Delayed>180) then'B7' ELSE 'OnTime' END)Delayed, TBRAmount    
  from #Temp1    
) src    
pivot    
(    
  MAX(TBRAmount)    
  for Delayed in (Ontime,B1,B2,B3,B4,B5,B6,B7)    
)piv    
)


Select ID as 'DepartmentNumber', Name as 'DepartmentName', Isnull(Ontime,0.00) Ontime,
Isnull(B1,0.00) 'Days1To30', Isnull(B2,0.00) 'Days31To60',Isnull(B3,0.00) 'Days61To90',Isnull(B4,0.00) 'Days91To120', Isnull(B5,0.00) 'Days121To150',
Isnull(B6,0.00) 'Days151To180',Isnull(B7,0.00) 'Days181More'
from Report order by ID

END













GO


