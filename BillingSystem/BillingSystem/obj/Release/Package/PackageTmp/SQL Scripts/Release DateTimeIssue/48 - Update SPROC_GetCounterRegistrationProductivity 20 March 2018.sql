IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetCounterRegistrationProductivity')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetCounterRegistrationProductivity
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetCounterRegistrationProductivity]    Script Date: 21-03-2018 10:41:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetCounterRegistrationProductivity]
	(
	@pCorporateID int,         ---- 9 for SEHA
	@pFacilityID int,          ---  8 For conrniche
	@pType nvarchar(5),       ---   2 for Registration
	@pFiscalYear nvarchar(5), ---   '2015'
	@pGraphType nvarchar(5)   ----  1013 for Registrations
	)
AS
BEGIN
Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))


Declare @MonthTarget int, @MonthActual int;

Declare @TA1 table(BudgetType int,BudgetDescription nvarchar(60),Department numeric(15,0),FiscalYear nvarchar(5)
, M1 NUMERIC(15,0),Sorting  int)

IF(@pGraphType = '1013')
Begin
Insert INTO @TA1
	Select 1 'BudgetType','Monthly - Target',1000,'2015', 
	Target = (Target * MAX(DAY(DATEADD(DD,-1,DATEADD(MM,DATEDIFF(MM,-1,@LocalDateTime),0))))),1
	from DashboardTargets where FacilityId = @pFacilityID and CorporateId = @pCorporateID and UnitOfMeasure = @pType --and RoleId = @pRoleType
	GROUP by Target

Insert INTO @TA1
	Select 3 'BudgetType','Yearly - Target',1000,'2015', 
	Target = (Target * 365),2
	from DashboardTargets where FacilityId = @pFacilityID and CorporateId = @pCorporateID and UnitOfMeasure = @pType --and RoleId = @pRoleType
	GROUP by Target

Insert INTO @TA1
Select  2 'BudgetType', 'Monthly - Actuals' 'BudgetDescription', 1000 'Department','2015' 'FiscalYear', ISNULL(sum(DTC.ActivityTotal),0) 'Actual',3
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = @pCorporateID and DTC.FacilityID = @pFacilityID and DTC.StatisticDescription = @pGraphType 
	 and DatePart(mm,DTC.ActivityDay) = DatePart(mm,@LocalDateTime)

Insert INTO @TA1
Select  4 'BudgetType', 'Yearly - Actuals' 'BudgetDescription', 1000 'Department','2015' 'FiscalYear', ISNULL(sum(DTC.ActivityTotal),0) 'Actual',4
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = 9 and DTC.FacilityID = 8 and DTC.StatisticDescription = @pGraphType
	 -- and DatePart(mm,DTC.ActivityDay) = DatePart(mm,@LocalDateTime)
END

IF(@pGraphType = '1027' AND @pType= '3')
Begin
Insert INTO @TA1
	Select 1 'BudgetType','Monthly - Target',1000,'2015', 
	Target = (Target * MAX(DAY(DATEADD(DD,-1,DATEADD(MM,DATEDIFF(MM,-1,@LocalDateTime),0))))),1
	from DashboardTargets where FacilityId = @pFacilityID and CorporateId = @pCorporateID and UnitOfMeasure = @pType --and RoleId = @pRoleType
	GROUP by Target

Insert INTO @TA1
	Select 3 'BudgetType','Yearly - Target',1000,'2015', 
	Target = (Target * 365),2
	from DashboardTargets where FacilityId = @pFacilityID and CorporateId = @pCorporateID and UnitOfMeasure = @pType --and RoleId = @pRoleType
	GROUP by Target

Insert INTO @TA1
Select  2 'BudgetType', 'Monthly - Actuals' 'BudgetDescription', 1000 'Department','2015' 'FiscalYear', ISNULL(sum(DTC.ActivityTotal),0) 'Actual',3
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = @pCorporateID and DTC.FacilityID = @pFacilityID --and DTC.StatisticDescription = @pGraphType 
	 and (DTC.StatisticDescription = '1029' OR DTC.StatisticDescription = '1028' OR DTC.StatisticDescription = '1027')
	 and DatePart(mm,DTC.ActivityDay) = DatePart(mm,@LocalDateTime)

Insert INTO @TA1
Select  4 'BudgetType', 'Yearly - Actuals' 'BudgetDescription', 1000 'Department','2015' 'FiscalYear', ISNULL(sum(DTC.ActivityTotal),0) 'Actual',4
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = 9 and DTC.FacilityID = 8 --and DTC.StatisticDescription = @pGraphType
	 and (DTC.StatisticDescription = '1029' OR DTC.StatisticDescription = '1028' OR DTC.StatisticDescription = '1027')
	 -- and DatePart(mm,DTC.ActivityDay) = DatePart(mm,@LocalDateTime)
END


IF(@pGraphType = '1028')
BEGIN 
Insert INTO @TA1
	 Select  1 'BudgetType', 'OP Encounters Monthly' 'BudgetDescription', 1000 'Department','2015' 'FiscalYear',Convert(Decimal(15,2) ,ISNULL(sum(DTC.ActivityTotal),0.00)) 'Actual',1
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = @pCorporateID and DTC.FacilityID = @pFacilityID and DTC.StatisticDescription = '1002' 
	 and DatePart(mm,DTC.ActivityDay) = DatePart(mm,@LocalDateTime)

Insert INTO @TA1
	 Select  2 'BudgetType', 'OP Encounters Billed Monthly' 'BudgetDescription', 1000 'Department','2015' 'FiscalYear', Convert(Decimal(15,2) ,ISNULL(sum(DTC.ActivityTotal),0.00)) 'Actual',2
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = @pCorporateID and DTC.FacilityID = @pFacilityID and DTC.StatisticDescription = '1028' 
	 and DatePart(mm,DTC.ActivityDay) = DatePart(mm,@LocalDateTime)

Insert INTO @TA1
	 Select  3 'BudgetType', 'OP Encounters Yearly', 1000 'Department','2015' 'FiscalYear',Convert(Decimal(15,2) ,ISNULL(sum(DTC.ActivityTotal),0.00)) 'Actual',3
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = 9 and DTC.FacilityID = 8 and DTC.StatisticDescription = '1002'

Insert INTO @TA1
	 Select  4 'BudgetType', 'OP Encounters Billed Yearly', 1000 'Department','2015' 'FiscalYear', Convert(Decimal(15,2) ,ISNULL(sum(DTC.ActivityTotal),0.00)) 'Actual',4
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = 9 and DTC.FacilityID = 8 and DTC.StatisticDescription = '1028' 
END

IF(@pGraphType = '1027' AND @pType <> '3')
BEGIN 
Insert INTO @TA1
	 Select  1 'BudgetType', 'IP Encounters Monthly' 'BudgetDescription', 1000 'Department','2015' 'FiscalYear', Convert(Decimal(15,2) ,ISNULL(sum(DTC.ActivityTotal),0.00)) 'Actual',1
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = @pCorporateID and DTC.FacilityID = @pFacilityID and DTC.StatisticDescription = '1015' 
	 and DatePart(mm,DTC.ActivityDay) = DatePart(mm,@LocalDateTime)

Insert INTO @TA1
	 Select  2 'BudgetType', 'IP Encounters Billed Monthly' 'BudgetDescription', 1000 'Department','2015' 'FiscalYear', Convert(Decimal(15,2) ,ISNULL(sum(DTC.ActivityTotal),0.00)) 'Actual',2
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = @pCorporateID and DTC.FacilityID = @pFacilityID and DTC.StatisticDescription = '1027' 
	 and DatePart(mm,DTC.ActivityDay) = DatePart(mm,@LocalDateTime)

Insert INTO @TA1
	 Select  3 'BudgetType', 'IP Encounters Yearly', 1000 'Department','2015' 'FiscalYear', Convert(Decimal(15,2) ,ISNULL(sum(DTC.ActivityTotal),0.00)) 'Actual',3
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = 9 and DTC.FacilityID = 8 and DTC.StatisticDescription = '1015'

Insert INTO @TA1
	 Select  4 'BudgetType', 'IP Encounters Billed Yearly', 1000 'Department','2015' 'FiscalYear', Convert(Decimal(15,2) ,ISNULL(sum(DTC.ActivityTotal),0.00)) 'Actual',4
	 from DashboardTransactionCounter DTC
	 where DTC.CorporateId = 9 and DTC.FacilityID = 8 and DTC.StatisticDescription = '1027' 
END

SELECT * FROM @TA1 Order by 1 

Delete From @TA1;
END












GO


