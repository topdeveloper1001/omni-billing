IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CopyToManualDashboard_SA')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CopyToManualDashboard_SA
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CopyToManualDashboard_SA]    Script Date: 22-03-2018 18:23:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_CopyToManualDashboard_SA] -- [SPROC_CopyToManualDashboard_SA] '270','12','9999','10',0,0
(
@IndicatorNumber NVARCHAR(100),
@CorporateId NVARCHAR(100),
@FacilityId NVARCHAR(100),
@loggedInUserId nvarchar(100),
@SubCategory1 nvarchar(50),
@SubCategory2 nvarchar(50)
)
AS
Begin
	Declare @Count int
	Declare @CurrentMonth int
	Declare @MonthlyType int = 1
	Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@FacilityId))

	Select @Count = Count(1) From ManualDashboard Where Indicators = @IndicatorNumber And CorporateId = @CorporateId And FacilityId = @FacilityId and SubCategory1 =@SubCategory1  and SubCategory2= @SubCategory2

	--- Begin Tran

		DELETE From ManualDashboard Where Indicators = @IndicatorNumber And CorporateId = @CorporateId And FacilityId = @FacilityId 
		And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
	

	
		/* Here, ExternalValue3 is considered as Type of Monthly Data i.e. Monthly and Year to Date. 
	Monthly  -> 1,
	Yearly  -> 2,
	*/
		INSERT INTO ManualDashboard ([BudgetType],[DashboardType],[KPICategory],[Indicators],[SubCategory1],[SubCategory2],[Frequency],[Defination],[DataType],
		[CompanyTotal],[OwnerShip],[Year],[OtherDescription],[FacilityId],[CorporateId],[CreatedBy],[CreatedDate],[IsActive],[ExternalValue2],[ExternalValue3],
		[M1],[M2],[M3],[M4],[M5],[M6],[M7],[M8],[M9],[M10],[M11],[M12])
		SELECT *
		FROM (
			SELECT DID.ExternalValue1, '' as DashboardType,'' as KPICategory, DID.IndicatorNumber, DID.SubCategory1, DID.SubCategory2, D.FerquencyTYpe 'Frequency',
			Left(D.Defination,50) as Defination, D.FormatType as DataType,'' CompanyTotal,D.[OwnerShip], DID.[year], CAST(DID.[month] as nvarchar) as 'month1', 
			CAST(ISNULL(DID.StatisticData,'0.0000') as numeric(18,4)) AS DATA,Left(D.[Description],50) as [Description], @FacilityId FacilityId, @CorporateId CorporateId,
			@loggedInUserId CreatedBy,@CurrentDate CreatedDate,1 as IsActive, D.ExternalValue2,DID.ExternalValue3 as MonthlyType
			FROM DashboardIndicatorData DID
			INNER JOIN DashboardIndicators D ON D.IndicatorNumber = DID.IndicatorNumber AND D.CorporateId = DID.CorporateId --AND D.FacilityId = DID.FacilityId
			AND D.SubCategory1 =DID.SubCategory1 AND D.SubCategory2 =  DID.SubCategory2 
			where DID.IndicatorNumber = @IndicatorNumber AND DID.CorporateId = @CorporateId AND DID.FacilityId = @FacilityId
			AND DID.SubCategory1 = @SubCategory1 AND DID.SubCategory2 = @SubCategory2 
			And DID.ExternalValue3 = '1'
		) as s
		PIVOT
		(
			MAX(DATA)
			FOR month1 IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
		) AS FINAL

	--Print 'Stay1'
    SET @MonthlyType = 2
	Select @Count = Count(1) From ManualDashboard Where Indicators = @IndicatorNumber And CorporateId = @CorporateId And FacilityId = @FacilityId 
	And ExternalValue3 = @MonthlyType
	--AND SubCategory1 = @SubCategory1 AND SubCategory2 = @SubCategory2

	--Print 'Stay3'
	
		INSERT INTO ManualDashboard ([BudgetType],[DashboardType],[KPICategory],[Indicators],[SubCategory1],[SubCategory2],[Frequency],[Defination],[DataType],
		[CompanyTotal],[OwnerShip],[Year],[OtherDescription],[FacilityId],[CorporateId],[CreatedBy],[CreatedDate],[IsActive],[ExternalValue2],[ExternalValue3],
		[M1],[M2],[M3],[M4],[M5],[M6],[M7],[M8],[M9],[M10],[M11],[M12])
		SELECT *
		FROM (
			SELECT DID.ExternalValue1, '' as DashboardType,'' as KPICategory, DID.IndicatorNumber, DID.SubCategory1, DID.SubCategory2, D.FerquencyTYpe 'Frequency',
			Left(D.Defination,50) as Defination, D.FormatType as DataType,'' CompanyTotal,D.[OwnerShip], DID.[year], CAST(DID.[month] as nvarchar) as 'month1', 
			CAST(ISNULL(DID.StatisticData,'0.0000') as numeric(18,4)) AS DATA,Left(D.[Description],50) as [Description], @FacilityId FacilityId, @CorporateId CorporateId,
			@loggedInUserId CreatedBy,@CurrentDate CreatedDate,1 as IsActive, D.ExternalValue2,DID.ExternalValue3 as MonthlyType
			FROM DashboardIndicatorData DID
			INNER JOIN DashboardIndicators D ON D.IndicatorNumber = DID.IndicatorNumber AND D.CorporateId = DID.CorporateId --AND D.FacilityId = DID.FacilityId
			AND D.SubCategory1 =DID.SubCategory1 AND D.SubCategory2 =  DID.SubCategory2 
			where DID.IndicatorNumber = @IndicatorNumber AND DID.CorporateId = @CorporateId AND DID.FacilityId = @FacilityId
			AND DID.SubCategory1 = @SubCategory1 AND DID.SubCategory2 = @SubCategory2 
			And DID.ExternalValue3 = '2'
		) as S
		PIVOT
		(
			MAX(DATA)
			FOR month1 IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
		) AS FINAL
	
	--Print 'Stay4'


	--IF @@ERROR=0
	--	Commit Tran
	--Else
	--	RollBack Tran
End







--TRUNCATE TABLE ManualDashboard

--Delete From ManualDashboard Where FacilityId = 9999
--INSERT INTO ManualDashboard (DashboardType,KPICategory,FacilityId,CorporateId, Indicators, SubCategory1, 
--SubCategory2, BudgetType, [Year], ExternalValue3,Frequency,DataType,CompanyTotal,[Ownership],OtherDescription,CreatedBy,CreatedDate
--,IsActive,ExternalValue1,ExternalValue2
--,M1
--,M2
--,M3
--,M4
--,M5
--,M6
--,M7
--,M8
--,M9
--,M10
--,M11
--,M12
--)
--Select '' DashboardType,'' KPICategory,9999 AS FacilityId,CorporateId, Indicators, SubCategory1, SubCategory2, 
--BudgetType,[Year],ExternalValue3,1 AS Frequency,DataType,0 As CompanyTotal,[Ownership],OtherDescription,CreatedBy,@CurrentDate,IsActive,
--ExternalValue1,ExternalValue2
--,Sum(CAST(M1 as numeric(18,4))) M1
--,Sum(CAST(M2 as numeric(18,4))) M2
--,Sum(CAST(M3 as numeric(18,4))) M3
--,Sum(CAST(M4 as numeric(18,4))) M4
--,Sum(CAST(M5 as numeric(18,4))) M5
--,Sum(CAST(M6 as numeric(18,4))) M6
--,Sum(CAST(M7 as numeric(18,4))) M7
--,Sum(CAST(M8 as numeric(18,4))) M8
--,Sum(CAST(M9 as numeric(18,4))) M9
--,Sum(CAST(M10 as numeric(18,4))) M10
--,Sum(CAST(M11 as numeric(18,4))) M11
--,Sum(CAST(M12 as numeric(18,4))) M12
--From ManualDashboard Group By CorporateId, Indicators, SubCategory1, SubCategory2, 
--BudgetType,[Year],ExternalValue3,DataType,[Ownership],OtherDescription,CreatedBy,IsActive,
--ExternalValue1,ExternalValue2




--Declare @IndicatorNumber NVARCHAR(100),
--@CorporateId NVARCHAR(100) = 12,
--@FacilityId NVARCHAR(100),
--@loggedInUserId nvarchar(100) = '100',
--@SubCategory1 nvarchar(50), 
--@SubCategory2 nvarchar(50)

--Declare FCursorTest Cursor FOR
--Select FacilityId From Facility 
--Where CorporateId = @CorporateId

--Open FCursorTest
--Fetch Next from FCursorTest into @FacilityId
--While @@FETCH_STATUS = 0
--Begin
--	Declare IndicatorCursorTest Cursor FOR
--	Select IndicatorNumber,SubCategory1,SubCategory2 From DashboardIndicators Where CorporateId = @CorporateId
--	Open IndicatorCursorTest

--	Fetch Next from IndicatorCursorTest into @IndicatorNumber,@SubCategory1,@SubCategory2

--	While @@FETCH_STATUS = 0
--	Begin
--		EXEC SPROC_CopyToManualDashboard @IndicatorNumber,12, @FacilityId,@loggedInUserId,@SubCategory1,@SubCategory2;
--		Fetch Next from IndicatorCursorTest into @IndicatorNumber,@SubCategory1,@SubCategory2
--	End

--	Close IndicatorCursorTest;
--	Deallocate IndicatorCursorTest;
	
--	Fetch Next from FCursorTest into @FacilityId	
--End

--Close FCursorTest;
--Deallocate FCursorTest;

--Update ManualDashboard Set ManualDashboard.Defination = D.Dashboard
--From ManualDashboard
--INNER JOIN DashboardIndicators D ON ManualDashboard.Indicators = D.IndicatorNumber
--Where ManualDashboard.Defination IS NULL





GO


