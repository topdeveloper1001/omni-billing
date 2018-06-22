IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CopyToManualDashboardForAllFacilities')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CopyToManualDashboardForAllFacilities
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CopyToManualDashboardForAllFacilities]    Script Date: 22-03-2018 18:27:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_CopyToManualDashboardForAllFacilities] -- [SPROC_CopyToManualDashboard] '144','12','17','10'
(
@IndicatorNumber NVARCHAR(100),
@CorporateId NVARCHAR(100),
@loggedInUserId nvarchar(100)
)
AS
Begin
	Declare @FacilityId int = 9999

	Declare @Count int
	Declare @CurrentMonth int
	Declare @MonthlyType int = 1
	Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))
	Select @Count = Count(1) From ManualDashboard Where Indicators = @IndicatorNumber And CorporateId = @CorporateId And FacilityId = @FacilityId

	Begin Tran
	IF @Count > 0
	Begin
		DELETE From ManualDashboard Where Indicators = @IndicatorNumber And CorporateId = @CorporateId And FacilityId = @FacilityId
	End

	/* Here, ExternalValue3 is considered as Type of Monthly Data i.e. Monthly and Year to Date. 
	Monthly  -> 1,
	Yearly  -> 2,
	*/
	INSERT INTO ManualDashboard ([BudgetType],[DashboardType],[KPICategory],[Indicators],[SubCategory1],[SubCategory2],[Frequency],[Defination],[DataType],
		[CompanyTotal],[OwnerShip],[Year],[OtherDescription],[FacilityId],[CorporateId],[CreatedBy],[CreatedDate],[IsActive],[ExternalValue2],[ExternalValue3],
		[M1],[M2],[M3],[M4],[M5],[M6],[M7],[M8],[M9],[M10],[M11],[M12])
		SELECT *
		FROM (
			SELECT DID.ExternalValue1, '' as DashboardType,'' as KPICategory, DID.IndicatorNumber, DID.SubCategory1, DID.SubCategory2, D.FerquencyType 'Frequency',
			D.Defination, D.FormatType as DataType,'' CompanyTotal,D.[OwnerShip], DID.[year], CAST(DID.[month] as nvarchar) as 'month1', 
			CAST(DID.StatisticData as numeric(18,4)) AS DATA,D.[Description], @FacilityId FacilityId, @CorporateId CorporateId,
			@loggedInUserId CreatedBy,@CurrentDate CreatedDate,1 as IsActive, D.ExternalValue2,1 as MonthlyType
			FROM DashboardIndicatorData DID
			INNER JOIN DashboardIndicators D ON DID.IndicatorNumber = D.IndicatorNumber AND DID.CorporateId = D.CorporateId
			where DID.IndicatorNumber = @IndicatorNumber AND DID.CorporateId = @CorporateId 
			AND DID.FacilityId IN (Select FacilityId From Facility Where CorporateId = @CorporateId And FacilityId != @FacilityId)
		) as s
		PIVOT
		(
			SUM(DATA)
			FOR month1 IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
		) AS FINAL

    SET @MonthlyType = 2
	Select @Count = Count(1) From ManualDashboard Where Indicators = @IndicatorNumber And CorporateId = @CorporateId 
	And FacilityId = @FacilityId And ExternalValue3 = @MonthlyType

	IF @Count=0
	Begin
		INSERT INTO ManualDashboard ([BudgetType],[DashboardType],[KPICategory],[Indicators],[SubCategory1],[SubCategory2],[Frequency],[Defination],[DataType],
		[CompanyTotal],[OwnerShip],[Year],[OtherDescription],[FacilityId],[CorporateId],[CreatedBy],[CreatedDate],[IsActive],[ExternalValue2],[ExternalValue3],
		[M1],[M2],[M3],[M4],[M5],[M6],[M7],[M8],[M9],[M10],[M11],[M12])
		SELECT *
		FROM (
			SELECT DID.ExternalValue1, '' as DashboardType,'' as KPICategory, DID.IndicatorNumber, DID.SubCategory1, DID.SubCategory2, D.FerquencyType 'Frequency',
			D.Defination, D.FormatType as DataType,'' CompanyTotal,D.[OwnerShip], DID.[year], CAST(DID.[month] as nvarchar) as 'month1', 
			CAST(DID.StatisticData as numeric(18,4)) AS DATA,D.[Description], @FacilityId FacilityId, @CorporateId CorporateId,
			@loggedInUserId CreatedBy,@CurrentDate CreatedDate,1 as IsActive, D.ExternalValue2,2 as MonthlyType
			FROM DashboardIndicatorData DID
			INNER JOIN DashboardIndicators D ON DID.IndicatorNumber = D.IndicatorNumber AND DID.CorporateId = D.CorporateId
			where DID.IndicatorNumber = @IndicatorNumber AND DID.CorporateId = @CorporateId 
			AND DID.FacilityId IN (Select FacilityId From Facility Where CorporateId = @CorporateId And FacilityId != @FacilityId)
		) as S
		PIVOT
		(
			SUM(DATA)
			FOR month1 IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
		) AS FINAL
	End

	--Update Operations
	Declare @M1 numeric(18,4),
	@M2 numeric(18,4),
	@M3 numeric(18,4),
	@M4 numeric(18,4),
	@M5 numeric(18,4),
	@M6 numeric(18,4),
	@M7 numeric(18,4),
	@M8 numeric(18,4),
	@M9 numeric(18,4),
	@M10 numeric(18,4),
	@M11 numeric(18,4),
	@M12 numeric(18,4),
	@BudgetType int,
	@Year int


	SET @MonthlyType = 1

	Declare IndicatorCursor Cursor FOR
	Select M1,M2,M3,M4,M5,M6,M7,M8,M9,M10,M11,M12,BudgetType,[Year] From ManualDashboard 
	Where Indicators = @IndicatorNumber And CorporateId = @CorporateId And FacilityId = @FacilityId And ExternalValue3 = @MonthlyType
	And Indicators != 108		


	Open IndicatorCursor
	Fetch Next from IndicatorCursor into @M1,@M2,@M3,@M4,@M5,@M6,@M7,@M8,@M9,@M10,@M11,@M12,@BudgetType,@Year
	While @@FETCH_STATUS = 0
	Begin
		--->>> Special case where no sum/Average it has to be Number as it is of present Month (e.g: Total Operating Beds and Available Bed Days)	
		IF @IndicatorNumber != 108 AND @IndicatorNumber != 123
		BEGIN

		Declare @ManualDashboardId int
		 
		Select @ManualDashboardId = ID, @BudgetType = BudgetType, @Year = [Year] From ManualDashboard Where Indicators = @IndicatorNumber 
		And CorporateId = @CorporateId And 
		FacilityId = @FacilityId And [Year] = @Year AND BudgetType = @BudgetType And ExternalValue3 = 2
		
		--> Special Case for Indicator Numbers 144 (Average Daily Census), 123 (Available Bed Days) and 131 (A/R Net Days) for Year To Date Starts
		IF @IndicatorNumber = 123 OR @IndicatorNumber = 144 OR @IndicatorNumber = 131
		Begin
			Declare @M1v numeric(18,4) = 31,
					@M2v numeric(18,4) = 59,
					@M3v numeric(18,4) = 90,
					@M4v numeric(18,4) = 120,
					@M5v numeric(18,4) = 151,
					@M6v numeric(18,4)= 181,
					@M7v numeric(18,4)= 212,
					@M8v numeric(18,4)= 243,
					@M9v numeric(18,4)= 273,
					@M10v numeric(18,4)=304,
					@M11v numeric(18,4)=334,
					@M12v numeric(18,4)=365, @StrDate as datetime,
					@febDays int,
					@M1v1 numeric(18,4)=0,
					@M2v1 numeric(18,4)=0,
					@M3v1 numeric(18,4)=0,
					@M4v1 numeric(18,4)=0,
					@M5v1 numeric(18,4)=0,
					@M6v1 numeric(18,4)=0,
					@M7v1 numeric(18,4)=0,
					@M8v1 numeric(18,4)=0,
					@M9v1 numeric(18,4)=0,
				   @M10v1 numeric(18,4)=0,
				   @M11v1 numeric(18,4)=0,
				   @M12v1 numeric(18,4)=0,
				    @M1v2 numeric(18,4)=0,
					@M2v2 numeric(18,4)=0,
					@M3v2 numeric(18,4)=0,
					@M4v2 numeric(18,4)=0,
					@M5v2 numeric(18,4)=0,
					@M6v2 numeric(18,4)=0,
					@M7v2 numeric(18,4)=0,
					@M8v2 numeric(18,4)=0,
					@M9v2 numeric(18,4)=0,
				   @M10v2 numeric(18,4)=0,
				   @M11v2 numeric(18,4)=0,
				   @M12v2 numeric(18,4)=0
				
			SET @StrDate = CAST(@Year as nvarchar) +'-' + Cast(2 as nvarchar(2)) +'-01'
			Select @febDays = dbo.GetDaysInMonth(@StrDate)

			IF @febDays = 29
			begin
				Set @M2v +=@M2v
				Set @M3v +=@M3v
				Set	@M4v +=@M4v
				Set	@M5v +=@M5v
				Set	@M6v +=@M6v
				Set	@M7v +=@M7v
				Set	@M8v +=@M8v
				Set	@M9v +=@M9v
				Set	@M10v +=@M10v
				Set	@M11v +=@M11v
				Set	@M12v +=@M12v
			end
			IF @IndicatorNumber = 144			--Average Daily Census
			Begin
				Select @M1v1 = M1, @M2v1 = (M1 + M2), @M3v1 = (M1 + M2 + M3), @M4v1 = (M1 + M2 + M3 + M4), @M5v1 = (M1 + M2 + M3 + M4 + M5), 
				@M6v1 = (M1 + M2 + M3 + M4 + M5 + M6), 
				@M7v1 = (M1 + M2 + M3 + M4 + M5 + M6 + M7), @M8v1 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8), 
				@M9v1 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9), @M10v1 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9+M10), 
				@M11v1 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9+M10+M11), @M12v1 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9+M10+M12)
				From ManualDashboard Where Indicators = 103 And CorporateId = @CorporateId And 
				FacilityId = @FacilityId And ExternalValue3 = 2 And [Year] = @Year AND BudgetType = @BudgetType

				Update ManualDashboard SET M1 = @M1v1 / @M1v
				,M2 = @M2v1 / @M2v
				,M3 = @M3v1 / @M3v
				,M4 = @M4v1 /@M4v
				,M5 = @M5v1 /@M5v
				,M6 = @M6v1 /@M6v
				,M7 = @M7v1 /@M7v
				,M8 = @M8v1 /@M8v
				,M9 = @M9v1 /@M9v
				,M10 = @M10v1 /@M10v
				,M11 = @M11v1 /@M11v
				,M12 = @M12v1 /@M12v
				Where ID = @ManualDashboardId
			End
			IF @IndicatorNumber = 131			--A/R Net Days
			Begin
				--Get the Values of Indicator 'Net Accounts Receivable Balance'
				Select @M1v2 = M1, @M2v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4))), @M3v2 = (M1 + M2 + M3), @M4v2 = (M1 + M2 + M3 + M4), @M5v2 = (M1 + M2 + M3 + M4 + M5), 
				@M6v2 = (M1 + M2 + M3 + M4 + M5 + M6), 
				@M7v2 = (M1 + M2 + M3 + M4 + M5 + M6 + M7), @M8v2 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8), 
				@M9v2 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9), @M10v2 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9+M10), 
				@M11v2 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9+M10+M11), @M12v2 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9+M10+M12)
				From ManualDashboard Where Indicators = 264 And CorporateId = @CorporateId And 
				FacilityId = @FacilityId And ExternalValue3 = 2 And [Year] = @Year AND BudgetType = @BudgetType

				--Get the Values of Indicator 'Net Revenue'
				Select @M1v1 = M1, @M2v1 = (M1 + M2), @M3v1 = (M1 + M2 + M3), @M4v1 = (M1 + M2 + M3 + M4), @M5v1 = (M1 + M2 + M3 + M4 + M5), 
				@M6v1 = (M1 + M2 + M3 + M4 + M5 + M6), 
				@M7v1 = (M1 + M2 + M3 + M4 + M5 + M6 + M7), @M8v1 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8), 
				@M9v1 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9), @M10v1 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9+M10), 
				@M11v1 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9+M10+M11), @M12v1 = (M1 + M2 + M3 + M4 + M5 + M6 +M7+ M8 + M9+M10+M12)
				From ManualDashboard Where Indicators = 110 And CorporateId = @CorporateId And 
				FacilityId = @FacilityId And ExternalValue3 = 2 And [Year] = @Year AND BudgetType = @BudgetType
				

				Update ManualDashboard 
				SET  M1  =	@M1v2 / (@M1v1 / @M1v)
					,M2  =	@M2v2 / (@M2v1 / @M2v)
					,M3  =	@M3v2 / (@M3v1 / @M3v)
					,M4  =	@M4v2 / (@M4v1 /@M4v )
					,M5  =	@M5v2 / (@M5v1 /@M5v )
					,M6  =	@M6v2 / (@M6v1 /@M6v )
					,M7  =	@M7v2 / (@M7v1 /@M7v )
					,M8  =	@M8v2 / (@M8v1 /@M8v )
					,M9  =	@M9v2 / (@M9v1 /@M9v )
					,M10 = @M10v2/  (@M10v1/@M10v)
					,M11 = @M11v2/  (@M11v1/@M11v)
					,M12 = @M12v2/  (@M12v1/@M12v)
				Where ID = @ManualDashboardId
			End
		End
		--> Special Case for Indicator Numbers 144 (Average Daily Census), 123 () and 131 () for Year To Date Ends
		Else
		Begin
			Update ManualDashboard SET M1 = @M1
			,M2 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0)) / 2
			,M3 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0)) / 3
			,M4 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0)) / 4
			,M5 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0)) / 5
			,M6 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0)) / 6
			,M7 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0)) / 7
			,M8 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0) + ISNULL(@M8,0.0))/8
			,M9 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0) + ISNULL(@M8,0.0) + ISNULL(@M9,0.0))/9
			,M10 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0) + ISNULL(@M8,0.0) + ISNULL(@M9,0.0)+ ISNULL(@M10,0.0)) / 10
			,M11 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0) + ISNULL(@M8,0.0) + ISNULL(@M9,0.0)+ ISNULL(@M10,0.0) + ISNULL(@M11,0.0)) / 11
			,M12 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0) + ISNULL(@M8,0.0) + ISNULL(@M9,0.0)+ ISNULL(@M10,0.0) + ISNULL(@M11,0.0) + ISNULL(@M12,0.0))/12
			Where ID = @ManualDashboardId
		End
		--IF @IndicatorNumber = 107
		--Begin
		--	Update ManualDashboard SET M1 = @M1
		--	,M2 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0)) / 2
 	--		,M3 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0)) / 3
		--	,M4 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0)) / 4
		--	,M5 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0)) / 5
		--	,M6 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0)) / 6
		--	,M7 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0)) / 7
		--	,M8 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0) + ISNULL(@M8,0.0))/8
		--	,M9 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0) + ISNULL(@M8,0.0) + ISNULL(@M9,0.0))/9
		--	,M10 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0) + ISNULL(@M8,0.0) + ISNULL(@M9,0.0)+ ISNULL(@M10,0.0)) / 10
		--	,M11 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0) + ISNULL(@M8,0.0) + ISNULL(@M9,0.0)+ ISNULL(@M10,0.0) + ISNULL(@M11,0.0)) / 11
		--	,M12 = (ISNULL(@M1,0.0) + ISNULL(@M2,0.0) + ISNULL(@M3,0.0) + ISNULL(@M4,0.0) + ISNULL(@M5,0.0) + ISNULL(@M6,0.0) + ISNULL(@M7,0.0) + ISNULL(@M8,0.0) + ISNULL(@M9,0.0)+ ISNULL(@M10,0.0) + ISNULL(@M11,0.0) + ISNULL(@M12,0.0))/12
		--	Where ID = @ManualDashboardId
		--End
		--Else
		--Begin
		--	Update ManualDashboard SET M1 = @M1
		--	,M2 = @M1 + @M2
		--	,M3 = @M1 + @M2 + @M3
		--	,M4 = @M1 + @M2 + @M3 + @M4
		--	,M5 = @M1 + @M2 + @M3 + @M4 + @M5
		--	,M6 = @M1 + @M2 + @M3 + @M4 + @M5 + @M6
		--	,M7 = @M1 + @M2 + @M3 + @M4 + @M5 + @M6 + @M7
		--	,M8 = @M1 + @M2 + @M3 + @M4 + @M5 + @M6 + @M7 + @M8
		--	,M9 = @M1 + @M2 + @M3 + @M4 + @M5 + @M6 + @M7 + @M8 + @M9
		--	,M10 = @M1 + @M2 + @M3 + @M4 + @M5 + @M6 + @M7 + @M8 + @M9+ @M10
		--	,M11 = @M1 + @M2 + @M3 + @M4 + @M5 + @M6 + @M7 + @M8 + @M9+ @M10 + @M11
		--	,M12 = @M1 + @M2 + @M3 + @M4 + @M5 + @M6 + @M7 + @M8 + @M9+ @M10 + @M11 + @M12
		--	Where ID = @ManualDashboardId
		--End
		END

		Fetch Next from IndicatorCursor into @M1,@M2,@M3,@M4,@M5,@M6,@M7,@M8,@M9,@M10,@M11,@M12,@BudgetType,@Year
	End

	Close IndicatorCursor;
	Deallocate IndicatorCursor;
	
	IF @@ERROR=0
		Commit Tran
	Else
		RollBack Tran
End















GO


