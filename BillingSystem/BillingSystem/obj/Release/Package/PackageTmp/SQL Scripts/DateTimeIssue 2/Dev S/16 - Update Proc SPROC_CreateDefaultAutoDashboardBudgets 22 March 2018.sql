IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CreateDefaultAutoDashboardBudgets')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CreateDefaultAutoDashboardBudgets
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CreateDefaultAutoDashboardBudgets]    Script Date: 22-03-2018 18:51:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,SHASHANK AWASTHY>
-- Create date: <Create Date,,03 MAy 2016>
-- Description:	<To add the 0 as the data in the monthly budget for the new Corporate and new facility,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_CreateDefaultAutoDashboardBudgets]
(
	@CID int, 
	@Fid int
)
AS
BEGIN
	
Declare @DashBoardBudgetTable Table
(
Budgettype int,BudgetDescription nvarchar(100),DepartNumber nvarchar(20),FiscalYear int,M1 numeric(15,2),M2 numeric(15,2),M3 numeric(15,2),
M4 numeric(15,2),M5 numeric(15,2),M6 numeric(15,2),M7 numeric(15,2),M8 numeric(15,2),M9 numeric(15,2),M10 numeric(15,2),M11 numeric(15,2),M12 numeric(15,2),
CID int, FID int, CreatedBy int, CreatedDate datetime,MOdifyBy int, modifyDate datetime,IsActive bit,BudgetFor nvarchar(20)
)

Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@Fid))

Declare @CurrentYear int = DatePart(YEAR,@CurrentDate)

DECLARE cursorPIFix CURSOR fast_forward FOR  
Select GlobalcodeValue from GlobalCodes where GLobalCodeCategoryValue = 3119 order by Cast(GlobalCodeValue as int) desc

DECLARE @cCursorGlobalCodeValue nvarchar(10), @cClientLoanID INT
OPEN cursorPIFix   
FETCH NEXT FROM cursorPIFix INTO @cCursorGlobalCodeValue

WHILE @@FETCH_STATUS = 0   
BEGIN   
	   Insert into @DashBoardBudgetTable
	   Select 1,'Monthly-Budget',1000,@CurrentYear,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00
	   ,@CID,@FID,1,@CurrentDate,null,null,1,@cCursorGlobalCodeValue

       FETCH NEXT FROM cursorPIFix INTO @cCursorGlobalCodeValue   
END   

CLOSE cursorPIFix   
DEALLOCATE cursorPIFix 


insert into DashboardBudget
Select * from  @DashBoardBudgetTable


END





GO


