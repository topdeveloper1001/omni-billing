IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_Get_REP_ClaimTransDetails')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_Get_REP_ClaimTransDetails
GO

/****** Object:  StoredProcedure [dbo].[SPROC_Get_REP_ClaimTransDetails]    Script Date: 20-03-2018 18:14:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

 
CREATE PROCEDURE [dbo].[SPROC_Get_REP_ClaimTransDetails]    
(      
@pCorporateID int,  
@pFacilityID int,
@pFromDate datetime, ---- From Which Date --- If NULL then will Use Todays' Date
@pTillDate datetime, ---- Till Which Date ---- If NULL then it will be FromDate + 1 Day 
@pDisplayBy int   ---- Sorted by (DEFAULT) 1 =  By Transaction Date and then Patient Name --> 2 = By Patient Name and then Transaction Date --> 3 = By Payer and then Transaction Date
)      
AS
BEGIN

Declare @CurrentDate datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

Declare @SqlToExecute nvarchar(max), @SQLWhereOrder nvarchar(500)

Set @pFromDate = isnull(@pFromDate, @CurrentDate)
Set @pTillDate = isnull(@pTillDate, DATEADD(DAY,1,@pFromDate))
Set @pDisplayBy = isnull(@pDisplayBy,1)

Set @SqlToExecute  = 'Select distinct XC.FileID,XF.TransactionDate,P.PersonFirstName,P.PersonLastName,P.PersonEmiratesIDNumber,E.EncounterNumber,
I.InsuranceCompanyName,BH.BillNumber,XC.Gross,XC.PatientShare,XC.Net,XC.EncounterID,XC.PatientID,XC.PayerID,XC.ClaimID from XClaim XC
inner join XFileHeader XF on  XF.FileID = XC.FileID
inner join PatientInfo P on P.PatientID = XC.PatientID
inner join Encounter E on E.EncounterID = XC.EncounterID
inner join BillHeader BH on BH.BillHeaderID = XC.ClaimID
inner join InsuranceCompany I on I.InsuranceCompanyId = P.PersonInsuranceCompany
Where XF.TransactionDate between ''' + Cast(@pFromDate as nvarchar(20)) + ''' AND ''' + Cast(@pTillDate as nvarchar(20)) + ''''

 
Set @SQLWhereOrder = CASE 
		WHEN @pDisplayBy = 1 THEN ' order by XF.TransactionDate,P.PersonFirstName,P.PersonLastName '  --- By Transaction Date and then Patient Name
		WHEN @pDisplayBy = 2 THEN ' order by P.PersonFirstName,P.PersonLastName,XF.TransactionDate '  --- By Patient Name and then Transaction Date
		WHEN @pDisplayBy = 3 THEN ' order by I.InsuranceCompanyName,XF.TransactionDate '  --- By Payer and then Transaction Date
	END

Set @SqlToExecute = @SqlToExecute + @SQLWhereOrder

Execute (@SqlToExecute);


END













GO


