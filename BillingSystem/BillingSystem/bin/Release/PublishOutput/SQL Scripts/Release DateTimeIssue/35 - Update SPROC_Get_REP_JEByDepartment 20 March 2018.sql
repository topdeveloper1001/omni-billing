IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_Get_REP_JEByDepartment')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_Get_REP_JEByDepartment
GO

/****** Object:  StoredProcedure [dbo].[SPROC_Get_REP_JEByDepartment]    Script Date: 20-03-2018 18:09:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

 
CREATE PROCEDURE [dbo].[SPROC_Get_REP_JEByDepartment]    
(      
@pCorporateID int = 9,  
@pFacilityID int =8 ,
@pFromDate datetime = '2015-01-01', ---- From Which Date --- If NULL then will Use Todays' Date
@pTillDate datetime = '2015-09-30', ---- Till Which Date ---- If NULL then it will be FromDate + 1 Day 
@pDisplayBy int=1   ---- Sorted by (DEFAULT) 1 =  By Transaction Date and then Patient Name --> 2 = By Patient Name and then Transaction Date --> 3 = By Payer and then Transaction Date
)      
AS
BEGIN
		DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

Declare @SqlToExecute nvarchar(max), @SQLWhereOrder nvarchar(500), @SQLGCColumn nvarchar(500)

Declare @TReturn table(ActivityDate date,ActivityType nvarchar(100),ActivityCode nvarchar(100),ActivityDescription nvarchar(500),EncounterNumber nvarchar(100),Gross numeric(18,2),DebitAccount nvarchar(100),CreditAccount nvarchar(100),GroupBy int)

----- BB- New Request/Fix changed Logic on 17-Feb (No matter which date dome always run fromDate as MidNight and Till Date last second (Midnight)of given Date - STARTS
Set @pFromDate = isnull(@pFromDate, @LocalDateTime)
Set @pTillDate = isnull(@pTillDate, @pFromDate)

Set @pFromDate = Cast(@pFromDate as Date)
Set @pTillDate = Cast(@pTillDate as Date)

Set @pFromDate = @pFromDate + '00:00'
Set @pTillDate = @pTillDate + '23:59:59'

----- BB- New Request/Fix changed Logic on 17-Feb (No matter which date dome always run fromDate as MidNight and Till Date last second (Midnight)of given Date - ENDS
Set @pDisplayBy = isnull(@pDisplayBy,1)
---- Get the Column Name based on Input DisplayFlagSent --- STARTS

Set @SQLGCColumn = CASE 
		WHEN @pDisplayBy = 1 THEN ' max(GC.ExternalValue1) as DebitAccount,max(GC.ExternalValue2) as CreditAccount,0 '  
		WHEN @pDisplayBy = 2 THEN ' max(GC.ExternalValue1) as DebitAccount,max(GC.ExternalValue3) as CreditAccount,0 '  
		WHEN @pDisplayBy = 3 THEN '  max(GC.ExternalValue1) as DebitAccount,max(GC.ExternalValue4) as CreditAccount,0 '  
		--WHEN @pDisplayBy = 4 THEN ' max(GC.ExternalValue4) as GeneralLedger '  
		--WHEN @pDisplayBy = 5 THEN ' max(GC.ExternalValue5) as GeneralLedger '  
		--WHEN @pDisplayBy = 6 THEN ' max(GC.ExternalValue6) as GeneralLedger '  
	END


---- Get the Column Name based on Input DisplayFlagSent --- ENDS

--- Make SQL to be Executed --- STARTS
Set @SqlToExecute  = 'Select Cast(BA.OrderedDate as Date) as ActivityDate,max(GC.GlobalCodeName) as ActivityType,BA.ActivityCode as ActivityCode,[dbo].[GetOrderDescription](BA.ActivityType,BA.ActivityCode) as ActivityDescription ,[dbo].[GetEncounterNumber](BA.EncounterID) EncounterNumber ,sum((BA.PatientShare + BA.PayerShareNet)) as Gross,'
Set @SqlToExecute  = @SqlToExecute  + @SQLGCColumn
Set @SqlToExecute  = @SqlToExecute  + ' From BillActivity BA inner join GlobalCodes GC on GC.GlobalCodeValue = BA.ActivityType and GC.GlobalCodeCategoryValue = 1201' 
Set @SqlToExecute  = @SqlToExecute  + ' Where (BA.OrderedDate between ''' + Cast(@pFromDate as nvarchar(100)) + ''' AND ''' + Cast(@pTillDate as nvarchar(100)) + ''')'
Set @SqlToExecute  = @SqlToExecute  +  ' and BA.CorporateID = ''' + Cast(@pCorporateID as nvarchar(100)) + ''''
Set @SqlToExecute  = @SqlToExecute  + ' group by Cast(BA.OrderedDate as Date) ,BA.ActivityType,BA.ActivityCode,BA.EncounterID  Order by Cast(BA.OrderedDate as Date) ,BA.ActivityType,BA.EncounterID'


--- Make SQL to be Executed --- STARTSr
print @SqlToExecute
--- Set @SqlToExecute = @SqlToExecute + @SQLWhereOrder
insert into @TReturn Execute (@SqlToExecute);

insert into @TReturn Select ActivityDate,' ',' ','Sub-TOTAL for --> '+max(isnull(CreditAccount,0)),' ',sum(Gross), max(DebitAccount),max(CreditAccount),1 
from @TReturn group by ActivityDate,CreditAccount 

If Exists (Select 1 From @TReturn)
Begin
	insert into @TReturn Select '2099-01-01',' ',' ','GRAND-TOTAL  --> ',' ',sum(Gross), ' ','9999999999',2 from @TReturn Where Groupby <> 1 
End

Select ActivityDate,ActivityType,ActivityCode,ActivityDescription,EncounterNumber,Gross,DebitAccount,CreditAccount from @TReturn order by ActivityDate,CreditAccount,GroupBy
END













GO


