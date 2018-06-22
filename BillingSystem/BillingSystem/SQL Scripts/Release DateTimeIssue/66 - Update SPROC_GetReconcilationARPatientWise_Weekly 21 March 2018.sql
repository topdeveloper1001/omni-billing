IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetReconcilationARPatientWise_Weekly')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetReconcilationARPatientWise_Weekly
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetReconcilationARPatientWise_Weekly]    Script Date: 21-03-2018 13:32:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_GetReconcilationARPatientWise_Weekly]
(  
@pCorporateID int = 9,  
@pFacilityID int =8,
@pAsOnDate Datetime =null,
@pViewType nvarchar(2) ='M' 
)  
AS  
BEGIN
DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

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



-- DATEPART(dw,BA.ActivityStartDate)
---- Weekly 
If @pViewType = 'W'
	Begin
		Set @StartDate = DATEADD(Week, DATEDIFF(Week, 0, @pAsOnDate), 0)
		Set @EndDate = DATEADD(Week, 1, @StartDate)

		INSERT INTO @Temp Select  BA.PatientID 'PatientID',max(PInfo.PersonLastName +'-'+ PInfo.PersonFirstName) 'PatientName', 
	'D'+ Cast(DATEPART(dw,BA.ActivityStartDate) as nvarchar(2)) 'WorkDay',sum(isnull(BH.PatientShare,0)) 'TBRAmount' from BillActivity BA
	INNER JOIN PatientInfo PInfo on PInfo.PatientID = BA.PatientID
	INNER JOIN BillHeader BH on BH.PatientID = BA.PatientID
	Where BA.CorporateID = @pCorporateID and BA.FacilityID = @pFacilityID and BA.ActivityStartDate between @StartDate and @EndDate
	Group by BA.PatientID,BA.ActivityStartDate
	Having sum(isnull(BA.Gross,0))>0

	End

----- DATE & QUERY SETTING - ENDS



--Select * from #Temp




 


If @pViewType = 'W'

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
  for [WorkDay] in (D1,D2,D3,D4,D5,D6,D7)    
)piv    
)    
    
 Select ID,Name,  ISNULL(D1,0.0) D1, ISNULL(D2,0.0) D2,   ISNULL(D3,0.0) D3, ISNULL(D4,0.0) D4, ISNULL(D5,0.0) D5, 
 ISNULL(D6,0.0) D6, ISNULL(D7,0.0) D7
 From Report Order By ID; 

 END --- WEEKLY Ends

 END --- Procedure Ends




GO


