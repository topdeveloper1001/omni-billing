IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_Get_REP_ClaimTransSummary')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_Get_REP_ClaimTransSummary
GO

/****** Object:  StoredProcedure [dbo].[SPROC_Get_REP_ClaimTransSummary]    Script Date: 20-03-2018 18:13:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

 
CREATE PROCEDURE [dbo].[SPROC_Get_REP_ClaimTransSummary]    
(      
@pCorporateID int,  
@pFacilityID int,
@pFromDate datetime, ---- From Which Date --- If NULL then will Use Todays' Date
@pTillDate datetime, ---- Till Which Date ---- If NULL then it will be FromDate + 1 Day 
@pDisplayBy int   ---- Sorted/Group by (DEFAULT) 1 = Transaction Date --> 2 = By Patient Name --> 3 = By Payer 
)      
AS
BEGIN

Declare @CurrentDate datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

Set @pFromDate = isnull(@pFromDate, @CurrentDate)
Set @pTillDate = isnull(@pTillDate, DATEADD(DAY,1,@pFromDate))
Set @pDisplayBy = isnull(@pDisplayBy,1)

If @pDisplayBy = 1
Begin
Select Cast(XF.TransactionDate as Date) 'TransmittedDate',sum(Cast(XC.Gross as Numeric (18,2))) 'Gross' ,
						sum(Cast(XC.PatientShare as Numeric (18,2))) 'PatientShare' ,sum(Cast(XC.Net as Numeric (18,2))) 'PayerNet'  from XClaim XC
inner join PatientInfo P on P.PatientID = XC.PatientID
inner join XFileHeader XF on  XF.FileID = XC.FileID
Where XF.TransactionDate between @pFromDate AND @pTillDate
Group by Cast(XF.TransactionDate as Date)
order by Cast(XF.TransactionDate as Date)

End

---- PATIENT WISE
If @pDisplayBy = 2
Begin

Select P.PersonFirstName,P.PersonLastName,P.PersonEmiratesIDNumber,sum(Cast(XC.Gross as Numeric (18,2))) 'Gross' ,
						sum(Cast(XC.PatientShare as Numeric (18,2))) 'PatientShare' ,sum(Cast(XC.Net as Numeric (18,2))) 'PayerNet' ,XC.PatientID from XClaim XC
inner join PatientInfo P on P.PatientID = XC.PatientID
inner join XFileHeader XF on  XF.FileID = XC.FileID
Where XF.TransactionDate between @pFromDate AND @pTillDate
Group by XC.PatientID,P.PersonFirstName,P.PersonLastName,P.PersonEmiratesIDNumber 
order by P.PersonFirstName,P.PersonLastName

End

----- PAYER WISE
If @pDisplayBy = 3
Begin

Select I.InsuranceCompanyName,sum(Cast(XC.Gross as Numeric (18,2))) 'Gross' ,
						sum(Cast(XC.PatientShare as Numeric (18,2))) 'PatientShare' ,sum(Cast(XC.Net as Numeric (18,2))) 'PayerNet' ,I.InsuranceCompanyId from XClaim XC
inner join PatientInfo P on P.PatientID = XC.PatientID
inner join InsuranceCompany I on I.InsuranceCompanyId = P.PersonInsuranceCompany
inner join XFileHeader XF on  XF.FileID = XC.FileID
Where XF.TransactionDate between @pFromDate AND @pTillDate
Group by I.InsuranceCompanyName,I.InsuranceCompanyId
order by I.InsuranceCompanyName,I.InsuranceCompanyId


End


END













GO


