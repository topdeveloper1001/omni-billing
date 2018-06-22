IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_DBDMain_IndicatorUpdate')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_DBDMain_IndicatorUpdate
GO

/****** Object:  StoredProcedure [dbo].[SPROC_DBDMain_IndicatorUpdate]    Script Date: 22-03-2018 19:18:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_DBDMain_IndicatorUpdate] ---- Exec [SPROC_DBDMain_IndicatorUpdate] 12,17,'108','1',2015,'0','0','1252','90.0000','90.0000','90.0000','90.0000','90.0000','90.0000','90.0000','90.0000','90.0000','90.0000','90.0000' 
(  
	@CID int = 12,					--- CorporateID
	@FID int = 18,					--- FacilityID
	@IndicatorNumber nvarchar(50) ='1308',	--- Current IndicatorNumber --- 1308
	@BudgetType varchar(10) ='2',		--- Budget (1)/Actual (2)
	@CurrentYear int = 2015,				---	Current Year / Previous Year,
	@SubCategory1 nvarchar(50) ='0',
	@SubCategory2 nvarchar(50) ='0',
	@INVal1 nvarchar(50)= '1500.0000',@INVal2 nvarchar(50)= '1600.0000',@INVal3 nvarchar(50)= '1700.0000',@INVal4 nvarchar(50)= '0.0000',
	@INVal5 nvarchar(50)= '0.0000',@INVal6 nvarchar(50)= '0.0000',@INVal7 nvarchar(50)= '0.0000',@INVal8 nvarchar(50)= '0.0000',
	@INVal9 nvarchar(50)= '0.0000',@INVal10 nvarchar(50)= '0.0000',@INVal11 nvarchar(50)= '0.0000',@INVal12 nvarchar(50)= '0.0000'
)
AS
BEGIN	
	DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@FID))

Declare @V1 numeric(18,4),@V2 numeric(18,4),@V3 numeric(18,4),@V4 numeric(18,4),@V5 numeric(18,4),@V6 numeric(18,4),
	@V7 numeric(18,4),@V8 numeric(18,4),@V9 numeric(18,4),@V10 numeric(18,4),@V11 numeric(18,4),@V12 numeric(18,4), @Cur_Indicator int, @SpecialCase int, @LoopFlag int

Declare @CalcIndicators Table (CalcIndicator int)

--Handled the special case where not to sum the YTD values in calculations. 
Select Top(1) @SpecialCase = SpecialCase  from DashboardIndicators where CorporateID = @CID and IndicatorNumber = @IndicatorNumber;

---- Taking Care of NULLs first
	Select  @INVal1 = isnull(@INVal1,'0.0000'), @INVal2 = isnull(@INVal2,'0.0000'), @INVal3 = isnull(@INVal3,'0.0000'), @INVal4 = isnull(@INVal4,'0.0000'),
			@INVal5 = isnull(@INVal5,'0.0000'), @INVal6 = isnull(@INVal6,'0.0000'), @INVal7 = isnull(@INVal7,'0.0000'), @INVal8 = isnull(@INVal8,'0.0000'),
			@INVal9 = isnull(@INVal9,'0.0000'), @INVal10 = isnull(@INVal10,'0.0000'), @INVal11 = isnull(@INVal11,'0.0000'), @INVal12 = isnull(@INVal12,'0.0000')

	Select @V1=@INVal1, @V2=@INVal2, @V3=@INVal3, @V4=@INVal4, @V5=@INVal5,@V6=@INVal6, @V7=@INVal7, @V8=@INVal8, @V9=@INVal9, @V10=@INVal10, @V11=@INVal11, @V12=@INVal12

--- Update MONTH for the Main Updated Indicator for passed in Facility
Update DashboardData SET M1 = @INVal1, M2 = @INVal2, M3 = @INVal3, M4 = @INVal4, M5 = @INVal5, M6 = @INVal6,
					M7 = @INVal7, M8 = @INVal8, M9 = @INVal9, M10 = @INVal10, M11 = @INVal11, M12 = @INVal12,CreatedDate= @LocalDateTime
			Where CorporateId = @CID  and FacilityId = @FID  and Indicators = @IndicatorNumber and SubCategory1 = @SubCategory1 and SubCategory2 = @SubCategory2 
			and [Year] = @CurrentYear and BudgetType =@BudgetType  and ExternalValue3 = 1

--Handled the special case where not to sum the YTD values in calculations. 
If @SpecialCase = 0 
Begin
		--- Update YTD for the Main Updated Indicator for passed in Facility
		Update DashboardData SET M1 = @V1, M2 = (@V1+@V2), M3 = (@V1+@V2+@V3), M4 = (@V1+@V2+@V3+@V4), M5 =(@V1+@V2+@V3+@V4+@V5), M6 =(@V1+@V2+@V3+@V4+@V5+@V6),
		M7 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7),M8 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7+@V8), M9 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7+@V8+@V9),M10 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7+@V8+@V9+@V10),
		M11 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7+@V8+@V9+@V10+@V11),M12 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7+@V8+@V9+@V10+@V11+@V12), CreatedDate= @LocalDateTime
		Where CorporateId = @CID  and FacilityId = @FID  and Indicators = @IndicatorNumber and SubCategory1 = @SubCategory1 and SubCategory2 = @SubCategory2 
		and [Year] = @CurrentYear and BudgetType =@BudgetType  and ExternalValue3 = 2
End
Else
Begin
		Update DashboardData SET M1=@V1, M2=@V2, M3=@V3, M4=@V4, M5=@V5, M6=@V6,M7=@V7, M8=@V8, M9=@V9, M10=@V10, M11=@V11, M12=@V12,CreatedDate= @LocalDateTime
		Where CorporateId = @CID  and FacilityId = @FID  and Indicators = @IndicatorNumber and SubCategory1 = @SubCategory1 and SubCategory2 = @SubCategory2 
		and [Year] = @CurrentYear and BudgetType =@BudgetType  and ExternalValue3 = 2
End
---  >>>>>>> 9999 --- > Update MONTH for the ALL case 9999 for passed in Facility  --- STARTS 

Select  @V1 = sum(Cast(M1 as Numeric(18,4))), @V2 = sum(Cast(M2 as Numeric(18,4))), @V3 = sum(Cast(M3 as Numeric(18,4))), 
			@V4 = sum(Cast(M4 as Numeric(18,4))), @V5 = sum(Cast(M5 as Numeric(18,4))), @V6 = sum(Cast(M6 as Numeric(18,4))),
			@V7 = sum(Cast(M7 as Numeric(18,4))), @V8 = sum(Cast(M8 as Numeric(18,4))), @V9 = sum(Cast(M9 as Numeric(18,4))), 
			@V10 = sum(Cast(M10 as Numeric(18,4))), @V11 = sum(Cast(M11 as Numeric(18,4))), @V12 = sum(Cast(M12 as Numeric(18,4))) from DashBoardData
			Where CorporateId = @CID  and FacilityId in (Select FacilityID from Corporate Where CorporateID=@CID and FacilityID <> 9999)  
			and Indicators = @IndicatorNumber and @SubCategory1 = @SubCategory1 and @SubCategory2 = @SubCategory2 
			and [Year] = @CurrentYear and BudgetType =@BudgetType  and ExternalValue3 = 1

Update DashboardData SET M1=@V1, M2=@V2, M3=@V3, M4=@V4, M5=@V5, M6=@V6,M7=@V7, M8=@V8, M9=@V9, M10=@V10, M11=@V11, M12=@V12, CreatedDate= @LocalDateTime
			Where CorporateId = @CID  and FacilityId = 9999  and Indicators = @IndicatorNumber and SubCategory1 = @SubCategory1 and SubCategory2 = @SubCategory2 
			and [Year] = @CurrentYear and BudgetType =@BudgetType  and ExternalValue3 = 1

--Handled the special case where not to sum the YTD values in calculations. 
If @SpecialCase = 0 
Begin		
		Update DashboardData SET M1 = @V1, M2 = (@V1+@V2), M3 = (@V1+@V2+@V3), M4 = (@V1+@V2+@V3+@V4), M5 =(@V1+@V2+@V3+@V4+@V5), M6 =(@V1+@V2+@V3+@V4+@V5+@V6),
		M7 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7),M8 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7+@V8), M9 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7+@V8+@V9),M10 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7+@V8+@V9+@V10),
		M11 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7+@V8+@V9+@V10+@V11),M12 =(@V1+@V2+@V3+@V4+@V5+@V6+@V7+@V8+@V9+@V10+@V11+@V12), CreatedDate= @LocalDateTime
		Where CorporateId = @CID  and FacilityId = 9999  and Indicators = @IndicatorNumber and SubCategory1 = @SubCategory1 and SubCategory2 = @SubCategory2 
		and [Year] = @CurrentYear and BudgetType =@BudgetType  and ExternalValue3 = 2
End
Else
Begin
		Update DashboardData SET M1=@V1, M2=@V2, M3=@V3, M4=@V4, M5=@V5, M6=@V6,M7=@V7, M8=@V8, M9=@V9, M10=@V10, M11=@V11, M12=@V12, CreatedDate= @LocalDateTime
		Where CorporateId = @CID  and FacilityId = 9999  and Indicators = @IndicatorNumber and SubCategory1 = @SubCategory1 and SubCategory2 = @SubCategory2 
		and [Year] = @CurrentYear and BudgetType =@BudgetType  and ExternalValue3 = 2
End
---  >>>>>>> 9999 --- > Update MONTH for the ALL case 9999 for passed in Facility  --- ENDS

---- >>>> XXXXXXXXXXXX >>>>>>>>>>>>>>>>>>>>>>>>>>NOW CALL for All effected Calculation - Here --- STARTS



---- Get List of Effected Indicators
	;WITH IndicatorEffected
	AS ( SELECT Indicator,EffectedBy
	FROM DashBoardIndicatorEffects
	WHERE EffectedBy = @IndicatorNumber  and Indicator <> EffectedBy	and CorporateID =@CID
	UNION ALL
	SELECT a.Indicator,a.EffectedBy
	FROM DashBoardIndicatorEffects a
	INNER JOIN IndicatorEffected ON IndicatorEffected.Indicator = a.EffectedBy
	WHERE  a.EffectedBy <> @IndicatorNumber and a.CorporateID =@CID)

		insert into @CalcIndicators Select distinct(Indicator) from IndicatorEffected;

		Set @LoopFlag = 1

		While @LoopFlag <=2
		Begin
				Declare Cursor_Calc Cursor For
				Select * from @CalcIndicators;

				Open Cursor_Calc;
				Fetch Next From Cursor_Calc into @Cur_Indicator;

				WHILE @@FETCH_STATUS = 0  
				BEGIN
						Exec [SPROC_DBDCalc_IndicatorUpdate] @CID,@FID,@Cur_Indicator,@BudgetType,@CurrentYear,@SubCategory1,@SubCategory2
						Fetch Next From Cursor_Calc into @Cur_Indicator;
				END

				-- Clean Up
				CLOSE Cursor_Calc;  
				DEALLOCATE Cursor_Calc;

				SET @LoopFlag += 1
		End



---- >>>> XXXXXXXXXXXX >>>>>>>>>>>>>>>>>>>>>>>>>>NOW CALL for All effected Calculation - Here --- ENDS




---  >>>>>>> SPECIAL CASES 9999 --- > if Any Place/Handle SPECIAL CASES BELOW for ALL 9999   --- STARTS 

---  >>>>>>> SPECIAL CASES 9999 --- > if Any Place/Handle SPECIAL CASES ABOVE for ALL 9999   --- ENDS



END  ---- Proc Ends







GO


