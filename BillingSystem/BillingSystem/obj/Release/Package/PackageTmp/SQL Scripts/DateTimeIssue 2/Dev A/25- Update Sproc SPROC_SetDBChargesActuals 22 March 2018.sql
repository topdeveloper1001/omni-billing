IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_SetDBChargesActuals')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_SetDBChargesActuals
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SPROC_SetDBChargesActuals]    Script Date: 3/22/2018 7:53:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_SetDBChargesActuals]  -- [SPROC_SetDBChargesActuals] 9,8,'0','2016'
(  
	@CID int,  --- CorporateID
	@FID int,  --- FacilityID
	@BudgetFor nvarchar(20), --- BudgetFor is Which Budget figures to be seen eg: 1 = OutPatient Bills
	@FiscalYear nvarchar(5)  --- Fiscal Year we are asking eg:'@FiscalYear'
)
AS
BEGIN
Declare @LocalDateTime datetime =(Select dbo.GetCurrentDatetimeByEntity(@FID))
--Select * from GlobalCodes where GLobalCodeCategoryValue = 3119 order by Cast(GlobalCodeValue as int) desc
Declare @lFiscalYear nvarchar(5) =@FiscalYear,@pCId int =@CID, @pFid int =@FID

Declare @TA1 table( BudgetType int,BudgetFor int,BudgetDescription nvarchar(60),Department numeric(15,0),FiscalYear nvarchar(5),M1 numeric(15,2),M2 numeric(15,2),M3 numeric(15,2),
M4 numeric(15,2),M5 numeric(15,2),M6 numeric(15,2),M7 numeric(15,2),M8 numeric(15,2),M9 numeric(15,2),M10 numeric(15,2),M11 numeric(15,2),M12 numeric(15,2))

Declare @TAFirst table (BudgetType int,BudgetFor int,BudgetDescription Nvarchar(100), Department numeric(15,0), FiscalYear nvarchar(5), Gross numeric (15,2), MonthType nvarchar(3))

DECLARE cursorPIFix CURSOR fast_forward FOR  
Select GlobalcodeValue from GlobalCodes where GLobalCodeCategoryValue = 3119 order by Cast(GlobalCodeValue as int) desc

DECLARE @cCursorGlobalCodeValue nvarchar(10), @cClientLoanID INT
OPEN cursorPIFix   
FETCH NEXT FROM cursorPIFix INTO @cCursorGlobalCodeValue

WHILE @@FETCH_STATUS = 0   
BEGIN   
	   --Select @cCursorGlobalCodeValue
	   if( @cCursorGlobalCodeValue not in ('14','15','16'))
	   BEGIN
		insert into @TAFirst
		Select 2,@cCursorGlobalCodeValue,'Monthly-Actuals',MAX(DepartmentNumber),@lFiscalYear,Sum(ActivityTotal),Max('M'+Cast( DatePart(mm,ActivityDay) as nvarchar(2))) 'MonthType'
		from DashboardTransactionCounter where [StatisticDescription] =@cCursorGlobalCodeValue  and CorporateId = @pCId and FacilityId = @pFid and DatePart(Year,ActivityDay) = @lFiscalYear
		Group by DatePart(mm,ActivityDay)
	   END
	   ELSE
	   BEGIN
			insert into @TA1
			EXEC [dbo].[SPROC_SetDBChargesActuals_Calc] @pCId,@pFid,@cCursorGlobalCodeValue,@lFiscalYear
	   END
       FETCH NEXT FROM cursorPIFix INTO @cCursorGlobalCodeValue   
END   

CLOSE cursorPIFix   
DEALLOCATE cursorPIFix  

;With Report     
AS    
(    
select * from  
(
Select * from @TAFirst
) src    
pivot    
(    
SUM(GROSS)    
for MonthType in (M1,M2,M3,M4,M5,M6,M7,M8,M9,M10,M11,M12)    
)piv    
)

insert into @TA1 select * from report;

Delete from [dbo].[DashboardBudget] where CorporateId = @pCId and FacilityId = @pFid and BudgetType = 2 and FiscalYear = @lFiscalYear
and BudgetFor in (Select BudgetFor from @TA1)

insert into [dbo].[DashboardBudget]
	(BudgetType, BudgetDescription, DepartmentNumber, FiscalYear, JanuaryBudget, FebruaryBudget, MarchBudget, AprilBudget, MayBudget, JuneBudget, JulyBudget, 
	AugustBudget, SeptemberBudget, OctoberBudget, NovemberBudget, DecemberBudget, CorporateId, FacilityId, CreatedBy, CreatedDate,ModifiedBy, ModifiedDate, IsActive, BudgetFor)
	Select MAX(BudgetType),MAX(BudgetDescription),MAX(Department),@lFiscalYear,SUM(isnull(M1,0)) 'M1',SUM(isnull(M2,0)) 'M2',SUM(isnull(M3,0)) 'M3',
	SUM(isnull(M4,0)) 'M4',SUM(isnull(M5,0)) 'M5',SUM(isnull(M6,0)) 'M6',
	SUM(isnull(M7,0)) 'M7',SUM(isnull(M8,0)) 'M8',SUM(isnull(M9,0)) 'M9',SUM(isnull(M10,0)) 'M10',SUM(isnull(M11,0)) 'M11',SUM(isnull(M12,0)) 'M12',
	@pCId 'CID',@pFid 'FID',1 'CBy',@LocalDateTime 'CDate',null 'MBy',null 'MDate',1 'IsActive', BudgetFor 'BudgetFor' from @TA1
	Group by BudgetFor


END











GO


