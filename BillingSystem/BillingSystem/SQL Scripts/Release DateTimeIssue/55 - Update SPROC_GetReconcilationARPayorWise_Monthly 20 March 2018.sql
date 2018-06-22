IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetReconcilationARPayorWise_Monthly')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetReconcilationARPayorWise_Monthly
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetReconcilationARPayorWise_Monthly]    Script Date: 21-03-2018 11:15:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_GetReconcilationARPayorWise_Monthly]
(  
@pCorporateID int = 9,  
@pFacilityID int =8,
@pAsOnDate Datetime =null,
@pViewType nvarchar(2) ='M' 
)  
AS  
BEGIN
DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))

Declare @StartDate datetime, @EndDate datetime

--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[#Temp]') AND type in (N'U'))
--Drop Table #temp
             
 Declare @Temp  Table     
 (    
 ID Nvarchar(20),
 Name Nvarchar(50), 
 WorkDay nvarchar(20),    
 TBRAmount numeric(18,2)    
 )



----- DATE and Query SETTING - STARTS

Set @pAsOnDate = isnull(@pAsOnDate,@LocalDateTime)
---- Following line takes care of Todays entered data as well because it will pick till Mid Night
  Set @pAsOnDate =  DATEADD(DAY,1,@pAsOnDate)


--- Set default for View Type ---> by Default Weekly
	If @pViewType not in ('Y','M','W')
		Set @pViewType = 'M'



	
---- Monthly
If @pViewType = 'M'
	Begin
		Set @StartDate = DATEADD(Month, DATEDIFF(Month, 0, @pAsOnDate), 0)
		Set @EndDate = DATEADD(Month, 1, @StartDate)
		INSERT INTO @Temp Select  INS.InsuranceCompanyId 'PayorID',max(INS.InsuranceCompanyName) 'PayorName', 
	'D'+ Cast(DAY(BA.ActivityStartDate) as nvarchar(2)) 'WorkDay',sum(isnull(BA.PayerShareNet,0)) 'TBRAmount' from BillActivity BA
	inner join PatientInfo PInfo on PInfo.PatientID = BA.PatientID
	inner join InsuranceCompany Ins on Ins.InsuranceCompanyId = PInfo.PersonInsuranceCompany
	Where BA.CorporateID = @pCorporateID and BA.FacilityID = @pFacilityID and BA.ActivityStartDate between @StartDate and @EndDate
	Group by INS.InsuranceCompanyId,BA.ActivityStartDate
	Having sum(isnull(BA.Gross,0))>0

	End


----- DATE & QUERY SETTING - ENDS



--Select * from #Temp




 

----- MONTHLY BreakDown - STARTS

If @pViewType = 'M'

BEGIN

;With Report     
AS    
(    
select * from     
(    
Select  * from @Temp
) src    
pivot    
(    
  MAX(TBRAmount)    
  for [WorkDay] in (D1,D2,D3,D4,D5,D6,D7,D8,D9,D10,D11,D12,D13,D14,D15,
  D16,D17,D18,D19,D20,D21,D22,D23,D24,D25,D26,D27,D28,D29,D30,D31)    
)piv    
)    
    
 Select ID,Name,  ISNULL(D1,0.0) D1, ISNULL(D2,0.0) D2,   ISNULL(D3,0.0) D3, ISNULL(D4,0.0) D4, ISNULL(D5,0.0) D5, 
 ISNULL(D6,0.0) D6, ISNULL(D7,0.0) D7, ISNULL(D8,0.0) D8, ISNULL(D9,0.0) D9,ISNULL(D10,0.0) D10, ISNULL(D11,0.0) D11, ISNULL(D12,0.0) D12,
 ISNULL(D13,0.0) D13, ISNULL(D14,0.0) D14, ISNULL(D15,0.0) D15,ISNULL(D16,0.0) D16, ISNULL(D17,0.0) D17, ISNULL(D18,0.0) D18, ISNULL(D19,0.0) D19,
 ISNULL(D20,0.0) D20,ISNULL(D21,0.0) D21, ISNULL(D22,0.0) D22,ISNULL(D23,0.0) D23, ISNULL(D24,0.0) D24, ISNULL(D25,0.0) D25,
 ISNULL(D26,0.0) D26, ISNULL(D27,0.0) D27,ISNULL(D28,0.0) D28, ISNULL(D29,0.0) D29, ISNULL(D30,0.0) D30,ISNULL(D31,0.0) D31     
 From Report Order By ID; 

 END --- Monthly Ends
 END --- Procedure Ends




GO


