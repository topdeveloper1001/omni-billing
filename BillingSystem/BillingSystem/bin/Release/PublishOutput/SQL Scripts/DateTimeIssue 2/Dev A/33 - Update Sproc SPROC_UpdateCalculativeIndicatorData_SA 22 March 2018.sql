IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_UpdateCalculativeIndicatorData_SA')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_UpdateCalculativeIndicatorData_SA
GO

/****** Object:  StoredProcedure [dbo].[SPROC_UpdateCalculativeIndicatorData_SA]    Script Date: 3/22/2018 8:07:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_UpdateCalculativeIndicatorData_SA]  --[SPROC_UpdateCalculativeIndicatorData_SA] '155',12,17,2015,2,28,0,0
(
@IndicatorNumbers NVARCHAR(100),
@CorporateId NVARCHAR(100),
@FacilityId NVARCHAR(100),
@Year NVARCHAR(100),
@BudgetType NVARCHAR(100),
@LoggedInUserId NVARCHAR(100),
@SubCategory1 nvarchar(50),
@SubCategory2 nvarchar(50)
)
AS
BEGIN
		DECLARE @LocalDateTime datetime = (Select dbo.GetCurrentDatetimeByEntity(@FacilityId))
	DECLARE @ReturnValue NVARCHAR(MAX)
	DECLARE @ExternalValue3 NVARCHAR(1000)
	DECLARE @TillDate NVARCHAR(100)
	DECLARE @SplitIndicatorNumber INT
	Declare @INumbers nvarchar(1000), @YTDVal1 nvarchar(50)

	Declare @T2 Table(indicatorNumber int)

	-- get Referneced Indicators
	;WITH MyCTE
	AS ( SELECT IndicatorNumberRef,MainIndicatorNumbe
	FROM ReferencedIndicators
	WHERE MainIndicatorNumbe = @IndicatorNumbers  and IndicatorNumberRef <> MainIndicatorNumbe
	and corporateid =@CorporateId
	UNION ALL
	SELECT a.IndicatorNumberRef,a.MainIndicatorNumbe
	FROM ReferencedIndicators a
	INNER JOIN MyCTE ON MyCTE.IndicatorNumberRef = a.MainIndicatorNumbe
	WHERE a.MainIndicatorNumbe <> a.IndicatorNumberRef and a.MainIndicatorNumbe <> @IndicatorNumbers
	and corporateid =@CorporateId)

	INSERT INTO @T2 Select IndicatorNumberRef From MyCTE
	
	--- Execute the Sproc for Calculations (STep 1)
	------>>>>>>>>>>>>>>>>>>>>>> BB Feb-2016 For performance and to Calculate YTD for Passed in Indicator  ------ STARTS
	------ There is NO need to Call Second PROD (Caclulative for Passed in Indicator -- It should be only call for referenceed/Calculative Indicators
	------ COMMENTING BELOW PROC
		--		EXEC SPROC_CalculatedIndicatorsValueByIndicator_SA  @CorporateId, @FacilityId, @IndicatorNumbers,@BudgetType,@Year,@SubCategory1,@SubCategory2
	------>>>>>>>>>>>>>>>>>>>>>> BB Feb-2016 For performance and to Calculate YTD for Passed in Indicator  ------ STARTS
	
	Declare @UpdateLoop int =1;
	Declare @Month int = 1,@Sum numeric(18,4),@YTD int =1

	--- Special Case for the Indicators (For actual save average only for Actuals and only for facilities have data)
	Declare @SpecialCase bit = CASE WHEN (Select TOP (1) ExternalValue4 from DashboardIndicators where IndicatorNumber = @IndicatorNumbers and CorporateId= @CorporateId) = 'True'
	THEN 1 ELSE 0 END 
	Declare @AvgFacilityCount int =0;
	----9999 Case Starts here-------------------------
		Set @Month  = 1;Set @Sum =0.00; Set @YTD =1;
		WHILE @YTD <= 2
		BEGIN
		While @Month <=12
		Begin
			Set @AvgFacilityCount = 0

			--Select @Month,@YTD
			------>>>>>>>>>>>>>>>>>>>>>> BB Feb-2016 For performance and to Calculate YTD for Passed in Indicator  ------ STARTS
			------ YTD for Same Facility is Brought Below (Taken out from Calculatie PROC for Indicator being Updated (passed IN this PROC)
			------ There is NO need to Call Second PROD (Caclulative for Passed in Indicator -- It should be only call for referenceed/Calculative Indicators
			------- This will Improve Performance as well
			If @YTD = 2
			Begin
				Select @YTDVal1 =  SUM( CAST(ISNULL(StatisticData,'0.0000') as Numeric(18,4))) from [dbo].[DashboardIndicatorData] 
				Where IndicatorNumber = @IndicatorNumbers and CorporateId = @CorporateId and FacilityId = @FacilityId and [Year] = @Year and [Month] <=  @Month 
					and ExternalValue1 = @BudgetType And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
					and ExternalValue3 <> '2';

				Set @YTDVal1 = isnull(@YTDVal1,'0.0000')
			
				Update [dbo].[DashboardIndicatorData]  Set StatisticData = Cast(@YTDVal1 as Nvarchar(50)) 
				Where IndicatorNumber = @IndicatorNumbers and CorporateId = @CorporateId and FacilityId = @FacilityId and 
				[Year] = @Year and [Month] = @Month and ExternalValue1 = @BudgetType And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
				and ExternalValue3 ='2'
			End

			------>>>>>>>>>>>>>>>>>>>>>> BB Feb-2016 For performance and to Calculate YTD for Passed in Indicator  ------ ENDS

			
			Select @Sum = Sum(CAST(ISNULL(StatisticData,'0.0000') as numeric(18,4))) From DashboardIndicatorData 
			Where IndicatorNumber = @IndicatorNumbers and CorporateId = @CorporateId 
			and FacilityId  IN (Select FacilityId From Facility Where CorporateId = @CorporateId And FacilityId != 9999)
			and [Year] = @Year and [Month] = @Month and ExternalValue1 = @BudgetType And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
			and ExternalValue3 =@YTD

			--Print @Sum

			If(@SpecialCase= 1)
			BEGIN
			  Select @AvgFacilityCount = Count(FacilityId) from DashboardIndicatorData Where IndicatorNumber = @IndicatorNumbers and CorporateId = @CorporateId 
			  And (FacilityId != 9999 and FacilityId <> 0)
				and [Year] = @Year and [Month] = @Month and ExternalValue1 = @BudgetType And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
				and ExternalValue3 =@YTD and (StatisticData <> '0' AND StatisticData <> '0.00' AND StatisticData <> '0.0000' AND StatisticData <> '0.0')

			  Set @Sum = CASE WHEN @AvgFacilityCount <> 0 THEN  @Sum/@AvgFacilityCount ELSE @Sum END;
			END

			--Print @AvgFacilityCount
			
			--Print @Sum

			SET @Sum = ISNULL(@Sum,'0.0000')

			If EXISTS (Select 1 from DashboardIndicatorData Where IndicatorNumber = @IndicatorNumbers and CorporateId = @CorporateId 
			and FacilityId  = 9999
			and [Year] = @Year and [Month] = @Month and ExternalValue1 = @BudgetType And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
			and ExternalValue3 =@YTD )
			Begin
				Update DashboardIndicatorData SET StatisticData = @Sum Where IndicatorNumber = @IndicatorNumbers and CorporateId = @CorporateId 
				and FacilityId  =9999
				and [Year] = @Year and [Month] = @Month and ExternalValue1 = @BudgetType And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
				and ExternalValue3 =@YTD 
			END
			ELSE
			BEGIN
				Insert into DashboardIndicatorData
				  values (@IndicatorNumbers,@IndicatorNumbers,@SubCategory1,@SubCategory2,@Sum,@Month,@Year,9999,@CorporateId,@LoggedInUserId,@LocalDateTime,@BudgetType,'',@YTD,1,'0')
			END
			Set @Month +=1
		End
		Set @Month  = 1;
		Set @YTD +=1
		END

		--- Copy the Data to the Manaual Dashboard (STep 2)
	EXEC SPROC_CopyToManualDashboard_SA @IndicatorNumbers,@CorporateId, @FacilityId, @LoggedInUserId,@SubCategory1,@SubCategory2


	----- Below is for all Calculative Indicators 

	While @UpdateLoop<=2
	BEGIN
	DECLARE Indicator_Number CURSOR FOR
		SELECT * From @T2
	--- Use cursur for referneced Indicators calculations  (STep 3)
	OPEN Indicator_Number;
	
	FETCH NEXT FROM Indicator_Number INTO @SplitIndicatorNumber;
	WHILE @@FETCH_STATUS = 0
	BEGIN
	--print 'Counter'
	--print @SplitIndicatorNumber
	 --Special cases for the 106 with subcategories 
		If( @SplitIndicatorNumber = 106)
		BEGIN
		--- Change the Subcategories as these are not mapped from the front end and causes some problem in coding/calculations
			If (@SubCategory1 = '7')
				Set @SubCategory1 ='116'
			else If (@SubCategory1 = '8')
				Set @SubCategory1 ='115'
			else If (@SubCategory1 = '117')
				Set @SubCategory1 ='117'
		END	

		---Perform calculations  (STep 4)
		EXEC SPROC_CalculatedIndicatorsValueByIndicator_SA @CorporateId, @FacilityId, @SplitIndicatorNumber,@BudgetType,@Year,@SubCategory1,@SubCategory2
		---Copy Calculations (STep 5)
		EXEC SPROC_CopyToManualDashboard_SA @SplitIndicatorNumber,@CorporateId, @FacilityId, @LoggedInUserId,@SubCategory1,@SubCategory2;
		---Sum for The Facility 9999 (STep 6)
		Set @Month  = 1;Set @Sum =0.00; Set @YTD =1;
		WHILE @YTD <= 2
		BEGIN
		While @Month <=12
		Begin
			Select @Sum = Sum(CAST(StatisticData as numeric(18,4))) From DashboardIndicatorData 
			Where IndicatorNumber = @SplitIndicatorNumber and CorporateId = @CorporateId 
			and FacilityId  IN (Select FacilityId From Facility Where CorporateId = @CorporateId And FacilityId != 9999)
			and [Year] = @Year and [Month] = @Month and ExternalValue1 = @BudgetType And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
			and ExternalValue3 =@YTD
			
			Set @AvgFacilityCount = 0

			If(@SpecialCase= 1 and @BudgetType = '2')
			BEGIN
			  Select @AvgFacilityCount = Count(FacilityId) from DashboardIndicatorData Where IndicatorNumber = @SplitIndicatorNumber and CorporateId = @CorporateId 
			  And (FacilityId != 9999 and FacilityId <> 0)
				and [Year] = @Year and [Month] = @Month and ExternalValue1 = @BudgetType And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
				and ExternalValue3 =@YTD and (StatisticData <> '0' AND StatisticData <> '0.00' AND StatisticData <> '0.0000' AND StatisticData <> '0.0')

			  Set @Sum = CASE WHEN @AvgFacilityCount <> 0 THEN  @Sum/@AvgFacilityCount ELSE @Sum END;
			END

			If EXISTS (Select 1 from DashboardIndicatorData Where IndicatorNumber = @SplitIndicatorNumber and CorporateId = @CorporateId 
			and FacilityId  =9999
			and [Year] = @Year and [Month] = @Month and ExternalValue1 = @BudgetType And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
			and ExternalValue3 =@YTD)
			Begin
				SET @Sum = ISNULL(@Sum,'0.0000')


				Update DashboardIndicatorData SET StatisticData = @Sum Where IndicatorNumber = @SplitIndicatorNumber and CorporateId = @CorporateId 
					and FacilityId  =9999
					and [Year] = @Year and [Month] = @Month and ExternalValue1 = @BudgetType And SubCategory1 = @SubCategory1 And SubCategory2 = @SubCategory2
					and ExternalValue3 =@YTD 
			END
			ELSE
			BEGIN
				Insert into DashboardIndicatorData
				  values (@SplitIndicatorNumber,@SplitIndicatorNumber,@SubCategory1,@SubCategory2,@Sum,@Month,@Year,9999,@CorporateId,@LoggedInUserId,@LocalDateTime,@BudgetType,'',@YTD,1,'0')
			END

			Set @Month +=1
		End
		Set @YTD +=1
		END

		-- Need to call copy To Manual Dashboard Here To update the Data for 9999 Facility for referneced indicator
		--EXEC SPROC_CopyToManualDashboard_SA @SplitIndicatorNumber,@CorporateId, 9999, @LoggedInUserId,@SubCategory1,@SubCategory2;

		--if (@SplitIndicatorNumber  in ('105','106','109','114','116','119','120','121','122','123','124','128','130','131','144','145','162','255','270','131','277'))
		--BEGIN
		--	EXEC SPROC_CalculatedIndicatorsValueByIndicator_SA @CorporateId, 9999, @SplitIndicatorNumber,@BudgetType,@Year,@SubCategory1,@SubCategory2;
		--	EXEC SPROC_CopyToManualDashboard_SA @SplitIndicatorNumber,@CorporateId, 9999, @LoggedInUserId,@SubCategory1,@SubCategory2;		
		----For 9999 Case Ends here-------------------------
		--END
		--ELSE
		--	EXEC SPROC_CopyToManualDashboard_SA @SplitIndicatorNumber,@CorporateId, 9999, @LoggedInUserId,@SubCategory1,@SubCategory2;	
		
		EXEC SPROC_CalculatedIndicatorsValueByIndicator_SA @CorporateId, 9999, @SplitIndicatorNumber,@BudgetType,@Year,@SubCategory1,@SubCategory2;
		EXEC SPROC_CopyToManualDashboard_SA @SplitIndicatorNumber,@CorporateId, 9999, @LoggedInUserId,@SubCategory1,@SubCategory2;
		If( @SplitIndicatorNumber = 106)
		BEGIN
			EXEC SPROC_CopyToManualDashboard_SA @SplitIndicatorNumber,@CorporateId, @FacilityId, @LoggedInUserId,'27',@SubCategory2;
			EXEC SPROC_CopyToManualDashboard_SA @SplitIndicatorNumber,@CorporateId, 9999, @LoggedInUserId,'27',@SubCategory2;
		END	

		--SELECT @SplitIndicatorNumber AS SPIN FOR XML PATH('IndNumber')
		FETCH NEXT FROM Indicator_Number INTO @SplitIndicatorNumber;
	END;

	CLOSE Indicator_Number;
	DEALLOCATE Indicator_Number;
	Set @UpdateLoop +=1;
	--INSERT INTO @T2 Values (@IndicatorNumbers)
	END
	
	Set @UpdateLoop =1;

	---Sum for The Facility 9999 (STep 6)
	--if (@IndicatorNumbers  in ('105','106','109','114','116','119','120','121','122','123','124','128','130','131','144','145','162','255','270','131','277'))
	--BEGIN
	--	EXEC SPROC_CalculatedIndicatorsValueByIndicator_SA  @CorporateId, 9999, @IndicatorNumbers,@BudgetType,@Year,@SubCategory1,@SubCategory2
	--	EXEC SPROC_CopyToManualDashboard_SA @IndicatorNumbers ,@CorporateId, 9999, @LoggedInUserId,@SubCategory1,@SubCategory2
	--END
	--ELSE
	--	EXEC SPROC_CopyToManualDashboard_SA @IndicatorNumbers ,@CorporateId, 9999, @LoggedInUserId,@SubCategory1,@SubCategory2
	
	------>>>>>>>>>>>>>>>>>>>>>> BB Feb-2016 For performance and to Calculate YTD for Passed in Indicator  ------ STARTS
	------ There is NO need to Call Second PROD (Caclulative for Passed in Indicator -- It should be only call for referenceed/Calculative Indicators
	------ COMMENTING BELOW PROC
		---- EXEC SPROC_CalculatedIndicatorsValueByIndicator_SA  @CorporateId, 9999, @IndicatorNumbers,@BudgetType,@Year,@SubCategory1,@SubCategory2
	------>>>>>>>>>>>>>>>>>>>>>> BB Feb-2016 For performance and to Calculate YTD for Passed in Indicator  ------ ENDS
	EXEC SPROC_CopyToManualDashboard_SA @IndicatorNumbers ,@CorporateId, 9999, @LoggedInUserId,@SubCategory1,@SubCategory2
End





GO


