IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_Get_REP_DenialCode')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_Get_REP_DenialCode
GO

/****** Object:  StoredProcedure [dbo].[SPROC_Get_REP_DenialCode]    Script Date: 20-03-2018 18:11:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

      
  
CREATE PROCEDURE [dbo].[SPROC_Get_REP_DenialCode]     -- [dbo].[SPROC_Get_REP_DenialCode]  6,4,'01/04/2016','30/04/2016',1
(      
@pCorporateID int,      
@pFacilityID int,    
@pFromDate Datetime,   ---- From Which Date --- If NULL then will Use Todays' Date
@pTillDate Datetime,   ---- Till Which Date ---- If NULL then it will be FromDate + 1 Day
@pDisplayBy int       ----  Denial Counts --> DisplayBy--- 1 = PayDate,2 = PayDate and DenialCodes, 3= DenialCodes, 4= Payer, 5= By Payer and Denial Codes
)      
AS      
BEGIN 
		DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

Set @pFromDate = isnull(@pFromDate, @LocalDateTime)
Set @pTillDate = isnull(@pTillDate, DATEADD(DAY,1,@pFromDate))
Set @pDisplayBy = isnull(@pDisplayBy,1)

-- Select * from Payment

----- By PayDate Wise
If @pDisplayBy = 1
Begin
Select Cast(PayDate as Date) 'PaymentDate',count(1) 'DenialCount' from Payment
Where PayXADenialCode is not null and len(PayXADenialCode)> 0 and PayDate between @pFromDate and @pTillDate
Group by Cast(PayDate as Date)
Order by Cast(PayDate as Date)
End

----- By PayDateWise and Denial Codes
If @pDisplayBy = 2
Begin
Select Cast(PayDate as Date) 'PaymentDate',PayXADenialCode,count(1) 'DenialCount' from Payment
Where PayXADenialCode is not null and len(PayXADenialCode)> 0 and PayDate between @pFromDate and @pTillDate
Group by Cast(PayDate as Date),PayXADenialCode
Order by Cast(PayDate as Date),PayXADenialCode
End


----- By Denial Codes
If @pDisplayBy = 3 
Begin
Select PayXADenialCode,count(1) 'DenialCount' from Payment
Where PayXADenialCode is not null and len(PayXADenialCode)> 0 and PayDate between @pFromDate and @pTillDate
Group by PayXADenialCode 
Order by Count(1) Desc,PayXADenialCode
End

---- By Payer
If @pDisplayBy = 4
Begin
Select PayBy,InsuranceCompanyName,count(1) 'DenialCount' from Payment
inner join InsuranceCompany on InsuranceCompanyId = PayBy
Where PayXADenialCode is not null and len(PayXADenialCode)> 0 and PayDate between @pFromDate and @pTillDate
Group by PayBy,InsuranceCompanyName
Order by PayBy,InsuranceCompanyName,Count(1) Desc
End

---- By Payer and Denial Codes
If @pDisplayBy = 5
Begin
Select PayBy,InsuranceCompanyName,PayXADenialCode,count(1) 'DenialCount' from Payment
inner join InsuranceCompany on InsuranceCompanyId = PayBy
Where PayXADenialCode is not null and len(PayXADenialCode)> 0 and PayDate between @pFromDate and @pTillDate
Group by PayBy,InsuranceCompanyName,PayXADenialCode 
Order by PayBy,InsuranceCompanyName,Count(1) Desc,PayXADenialCode
End

END












GO


