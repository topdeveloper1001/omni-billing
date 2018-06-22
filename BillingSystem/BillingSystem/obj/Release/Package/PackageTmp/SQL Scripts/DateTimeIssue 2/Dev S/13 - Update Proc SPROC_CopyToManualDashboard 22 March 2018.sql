IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CopyToManualDashboard')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CopyToManualDashboard
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CopyToManualDashboard]    Script Date: 22-03-2018 17:50:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_CopyToManualDashboard] -- [SPROC_CopyToManualDashboard] '120','12','17','10',0,0
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
	Declare @LocalDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@FacilityId))

	Select @Count = Count(1) From ManualDashboard Where Indicators = @IndicatorNumber And CorporateId = @CorporateId And FacilityId = @FacilityId and SubCategory1 =@SubCategory1  and SubCategory2= @SubCategory2

	Begin Tran
	IF @Count > 0 --And @FacilityId != 9999
	Begin
		DELETE From ManualDashboard Where Indicators = @IndicatorNumber And CorporateId = @CorporateId And FacilityId = @FacilityId 
		And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
	End

	Print 'Stay'
	IF (@Count > 0)
	BEGIN
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
			D.Defination, D.FormatType as DataType,'' CompanyTotal,D.[OwnerShip], DID.[year], CAST(DID.[month] as nvarchar) as 'month1', 
			CAST(DID.StatisticData as numeric(18,4)) AS DATA,D.[Description], @FacilityId FacilityId, @CorporateId CorporateId,
			@loggedInUserId CreatedBy,@LocalDate CreatedDate,1 as IsActive, D.ExternalValue2,1 as MonthlyType
			FROM DashboardIndicatorData DID
			INNER JOIN DashboardIndicators D ON D.IndicatorNumber = DID.IndicatorNumber AND D.CorporateId = DID.CorporateId --AND D.FacilityId = DID.FacilityId
			AND D.SubCategory1 =DID.SubCategory1 AND D.SubCategory2 =  DID.SubCategory2 
			where DID.IndicatorNumber = @IndicatorNumber AND DID.CorporateId = @CorporateId AND DID.FacilityId = @FacilityId
			AND DID.SubCategory1 = @SubCategory1 AND DID.SubCategory2 = @SubCategory2
		) as s
		PIVOT
		(
			SUM(DATA)
			FOR month1 IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
		) AS FINAL
	END

	Print 'Stay1'
    SET @MonthlyType = 2
	Select @Count = Count(1) From ManualDashboard Where Indicators = @IndicatorNumber And CorporateId = @CorporateId And FacilityId = @FacilityId 
	And ExternalValue3 = @MonthlyType
	AND SubCategory1 = @SubCategory1 AND SubCategory2 = @SubCategory2

	Print 'Stay3'
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
			@loggedInUserId CreatedBy,@LocalDate CreatedDate,1 as IsActive, D.ExternalValue2,2 as MonthlyType
			FROM DashboardIndicatorData DID
			INNER JOIN DashboardIndicators D ON D.IndicatorNumber = DID.IndicatorNumber AND D.CorporateId = DID.CorporateId --AND D.FacilityId = DID.FacilityId
			AND D.SubCategory1 =DID.SubCategory1 AND D.SubCategory2 =  DID.SubCategory2 
			where DID.IndicatorNumber = @IndicatorNumber AND DID.CorporateId = @CorporateId AND DID.FacilityId = @FacilityId
			AND DID.SubCategory1 = @SubCategory1 AND DID.SubCategory2 = @SubCategory2
		) as S
		PIVOT
		(
			SUM(DATA)
			FOR month1 IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
		) AS FINAL
	End
	Print 'Stay4'

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
	Print 'Stay5'
	IF @IndicatorNumber IN (108,123,131,144)
	BEGIN
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

			if @febDays = 29
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
				Select @M1v1 = CAST(M1 as numeric(18,4)), @M2v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4))), @M3v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4))), @M4v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))), @M5v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4))), 
				@M6v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))), @M5v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4))), 
				@M7v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))), @M5v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))), @M8v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))), @M5v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) +CAST(M6 as numeric(18,4)) +CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4))), 
				@M9v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))), @M5v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) +CAST(M6 as numeric(18,4)) +CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))), @M10v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))), @M5v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) +CAST(M6 as numeric(18,4)) +CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) +CAST(M9 as numeric(18,4))+CAST(M10 as numeric(18,4))), 
				@M11v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) +CAST(M6 as numeric(18,4)) +CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) +CAST(M9 as numeric(18,4))+CAST(M10 as numeric(18,4))+CAST(M11 as numeric(18,4))), @M12v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) +CAST(M6 as numeric(18,4)) +CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) +CAST(M9 as numeric(18,4))+CAST(M10 as numeric(18,4))+CAST(M11 as numeric(18,4)))
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
				Select @M1v2 = CAST(M1 as numeric(18,4)), @M2v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4))), @M3v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4))), @M4v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))), @M5v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4))), @M4v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4))), 
				@M6v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4))), 
				@M7v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))), @M8v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4))), 
				@M9v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))), @M8v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))), @M10v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))), @M8v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+CAST(M10 as numeric(18,4))), 
				@M11v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))), @M8v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+CAST(M10 as numeric(18,4))+CAST(M11 as numeric(18,4))), @M12v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))), @M8v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+CAST(M10 as numeric(18,4))+CAST(M12 as numeric(18,4)))
				From ManualDashboard Where Indicators = 264 And CorporateId = @CorporateId And 
				FacilityId = @FacilityId And ExternalValue3 = 2 And [Year] = @Year AND BudgetType = @BudgetType

				--Get the Values of Indicator 'Net Revenue'
				Select @M1v1 = CAST(M1 as numeric(18,4)), @M2v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4))), @M3v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4))), @M4v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))), @M5v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4))), 
				@M6v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))), @M8v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4))), 
				@M7v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))), @M8v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))), @M8v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))), @M8v2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4))), 
				@M9v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))), @M10v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+CAST(M10 as numeric(18,4))), 
				@M11v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+CAST(M10 as numeric(18,4))+CAST(M11 as numeric(18,4))), @M12v1 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))+ CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))+ CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+CAST(M10 as numeric(18,4))+CAST(M12 as numeric(18,4)))
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
		END

			Fetch Next from IndicatorCursor into @M1,@M2,@M3,@M4,@M5,@M6,@M7,@M8,@M9,@M10,@M11,@M12,@BudgetType,@Year
		End

		Close IndicatorCursor;
		Deallocate IndicatorCursor;
	END
	ELSE
	BEGIN
		IF @IndicatorNumber IN (109,120,121,145,122,162) --(120,121,145,122,162) 
		Begin
			Update ManualDashboard SET M1 = M1
			,M2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4))) / 2
			,M3 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4))) / 3
			,M4 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))) / 4
			,M5 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4))) / 5
			,M6 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4))) / 6
			,M7 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))) / 7
			,M8 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4)) + CAST(M8 as numeric(18,4)))/8
			,M9 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4)) + CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4)))/9
			,M10 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4)) + CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+ CAST(M10 as numeric(18,4))) / 10
			,M11 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4)) + CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+ CAST(M10 as numeric(18,4)) + CAST(M11 as numeric(18,4))) / 11
			,M12 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4)) + CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+ CAST(M10 as numeric(18,4)) + CAST(M11 as numeric(18,4)) + CAST(M12 as numeric(18,4)))/12
			Where Indicators = @IndicatorNumber And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2 
			AND CorporateId = @CorporateId AND FacilityID = @FacilityId AND ExternalValue3 = 2
		End
	    Else
		Begin
		Print 'Final'
			Update ManualDashboard SET M1 = M1
			,M2 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4))) 
			,M3 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4))) 
			,M4 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4))) 
			,M5 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4))) 
			,M6 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4))) 
			,M7 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4))) 
			,M8 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4)) + CAST(M8 as numeric(18,4))) 
			,M9 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4)) + CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4)))
			,M10 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4)) + CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+ CAST(M10 as numeric(18,4))) 
			,M11 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4)) + CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+ CAST(M10 as numeric(18,4)) + CAST(M11 as numeric(18,4))) 
			,M12 = (CAST(M1 as numeric(18,4)) + CAST(M2 as numeric(18,4)) + CAST(M3 as numeric(18,4)) + CAST(M4 as numeric(18,4)) + CAST(M5 as numeric(18,4)) + CAST(M6 as numeric(18,4)) + CAST(M7 as numeric(18,4)) + CAST(M8 as numeric(18,4)) + CAST(M9 as numeric(18,4))+ CAST(M10 as numeric(18,4)) + CAST(M11 as numeric(18,4)) + CAST(M12 as numeric(18,4)))
			Where Indicators = @IndicatorNumber And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2 
			AND CorporateId = @CorporateId AND FacilityID = @FacilityId AND ExternalValue3 = 2
		End

	END

	IF @@ERROR=0
		Commit Tran
	Else
		RollBack Tran
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
--BudgetType,[Year],ExternalValue3,1 AS Frequency,DataType,0 As CompanyTotal,[Ownership],OtherDescription,CreatedBy,@LocalDate,IsActive,
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


