
IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetDBBudgetActualManual')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetDBBudgetActualManual

/****** Object:  StoredProcedure [dbo].[SPROC_GetDBBudgetActualManual]    Script Date: 3/22/2018 7:07:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SPROC_GetDBBudgetActualManual]
(  
	@CID int = 12,  --- CorporateID
	@FID int =17,  --- FacilityID
	@TillDate date = '2015-06-06', --- Pass in the current date till which date you want data to be displayed
	@FacilityRelated nvarchar(40)=null, ---- FacilityType
	@FacilityRegion nvarchar(10) =null  ----- regionID
)
AS
BEGIN
Declare @LocalDateTime datetime  = (Select dbo.GetCurrentDatetimeByEntity(@FID))
 
 Declare @SelT table (FID int, DepartmentNumber nvarchar(20))

Declare @Month int, @FiscalYear int, @strStartDate nvarchar(20), @StartDate date, @DenominatorYTD numeric(18,4), @previousYear int
Declare @N1 numeric(18,2)
Declare @N2 numeric(18,2)
Declare @NumberOfDays int, @CUR_FacID int, @FacilityCount int;
Declare @Results table (CID int, FID int, IndicatorNumber nvarchar(50), DashBoard nvarchar(500), ResultType nvarchar(50),Result decimal(18,4), Tag nvarchar(10), FormatType int, EV2 nvarchar(50),Subcategory1 nvarchar(10),Subcategory2 nvarchar(10),[OwnerShip] nvarchar(50),TypeOfData nvarchar(20))

Declare @FormatResults table (CID int, FID int, INum nvarchar(50), DashBoard nvarchar(500), ResultType nvarchar(50), FormatType int, EV2 nvarchar(50),Subcategory1 nvarchar(10),Subcategory2 nvarchar(10),[OwnerShip] nvarchar(50),TypeOfData nvarchar(20),
								CMB decimal(18,4),CMA decimal(18,4),PMB decimal(18,4),PMA decimal(18,4),CYTB decimal(18,4),CYTA decimal(18,4),PYTB decimal(18,4),PYTA decimal(18,4))

---- Get Selection Information - STARTS
	if @FID > 0 
	Begin
		Insert into @SelT Select FacilityId,'' from Facility Where FacilityId = @FID and CorporateID = @CID;
	End
	ELSE
	Begin
		If @FacilityRelated is not NULL and @FacilityRegion is NULL
			Insert into @SelT Select FacilityId,'' from Facility Where FacilityRelated = @FacilityRelated and CorporateID = @CID;
		If @FacilityRegion is not NULL and @FacilityRelated is NULL
			Insert into @SelT Select FacilityId,'' from Facility Where RegionId = @FacilityRegion and CorporateID = @CID;
		If @FacilityRelated is not NULL and @FacilityRegion is not NULL
			Insert into @SelT Select FacilityId,'' from Facility Where FacilityRelated = @FacilityRelated and RegionId = @FacilityRegion and CorporateID = @CID;
		If @FacilityRelated is NULL and @FacilityRegion is NULL and @FID =0
			Insert into @SelT Select FacilityId,'' from Facility Where CorporateID = @CID;
	End
--- If their is multiple facilities selected
Select @FacilityCount =  Count(FID) from @SelT;
IF((Select Count(FID) from @SelT) > 1)
	Begin
		--- Delete Old data for the Facility and Corporate
		Delete From [DashboardIndicatorData] Where FacilityId=9999 and CorporateId=@CID

		--- insert data with sum for the Selected Facilities
		Insert into [DashboardIndicatorData] 
		Select 1,DID.IndicatorNumber,DID.SubCategory1,DID.SubCategory2,SUM(CAST(DID.StatisticData as DECIMAL(18,4))), DID.[Month],DID.[Year],9999,@CID,1,@LocalDateTime,DID.ExternalValue1 
			,DID.ExternalValue2,DID.ExternalValue3,1,Null 	from  [dbo].[DashboardIndicatorData] DID
			Where DID.CorporateId = @CID and FacilityId in (Select FID from @SelT) GROUP BY DID.IndicatorNumber,DID.SubCategory1,DID.SubCategory2,DID.[Month],
			DID.[Year],DID.ExternalValue1,DID.ExternalValue2,DID.ExternalValue3;

		
		--- Get the Calculations first with Following PROC
		Exec [dbo].[SPROC_SetDBCalculatedIndicators]  @CID,9999,@TillDate
		
		---- JUGAAD need to be fixed with proper sequence column ------XXXX
		---- NOTE: Called SECOND TIME for a purpose - Due to missing Sequence and some calculations are based on others - So in first round it will fix and this call will correctly do rest calulations
		--- Get the Calculations first with Following PROC

		Exec [dbo].[SPROC_SetDBCalculatedIndicators]  @CID,9999,@TillDate
		---- NOTE: Called SECOND TIME for a purpose - Due to missing Sequence and some calculations are based on others - So in first round it will fix and this call will correctly do rest calulations
		---- JUGAAD need to be fixed with proper sequence column ------XXXX
		--Declare CustomCursor Cursor for Select FID from @SelT
		
		--Open CustomCursor;
		
		--Fetch Next from CustomCursor into @CUR_FacID;
		--While @@FETCH_STATUS = 0
		--Begin
		--	Exec [dbo].[SPROC_SetDBCalculatedIndicators]  @CID,@CUR_FacID,@TillDate
		-- Fetch Next from CustomCursor into @CUR_FacID;
		--End

		--- Delete From the @SelT
		Delete From @SelT
		-- Add Custom Value in the @SelT for the Below New Facility
		Insert into @SelT (FID,DepartmentNumber) Values (9999,'')
	END
ELSE
BEGIN
	--- Get the Calculations first with Following PROC
	Exec [dbo].[SPROC_SetDBCalculatedIndicators]  @CID,@FID,@TillDate
	
	---- JUGAAD need to be fixed with proper sequence column ------XXXX
	---- NOTE: Called SECOND TIME for a purpose - Due to missing Sequence and some calculations are based on others - So in first round it will fix and this call will correctly do rest calulations
	--- Get the Calculations first with Following PROC
	Exec [dbo].[SPROC_SetDBCalculatedIndicators]  @CID,@FID,@TillDate
	---- NOTE: Called SECOND TIME for a purpose - Due to missing Sequence and some calculations are based on others - So in first round it will fix and this call will correctly do rest calulations
	---- JUGAAD need to be fixed with proper sequence column ------XXXX
END

Set @Month = month(@TillDate)
Set @FiscalYear = year(@TillDate)
Set @N1 = (datepart(dd,@TillDate))
Set @N2 = (datepart(dd,EOMONTH(@TillDate)))
Set @DenominatorYTD = (@Month - 1)+ (@N1/@N2)
Set @previousYear = year(DATEADD(yy,-1,@TillDate))

---- Current YEAR - Starts
Set @strStartDate = cast(@FiscalYear as nvarchar(4)) + '-01-01'
Set @StartDate = @strStartDate

----- max(DI.Dashboard) DashBoard, --- max(DI.FormatType), MAX(CAST(DI.ExternalValue2 as int)) 
---- Year to Date
insert into @Results
Select DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,'',DID.ExternalValue1 ResultType, cast(DID.StatisticData as decimal(18,4)) Result,
Case When DID.ExternalValue1 = 1 Then 'CYTB' Else 'CYTA' END Tag,0,'',DID.SubCategory1,DID.SubCategory2,null,null from [dbo].[DashboardIndicatorData] DID
-- inner join [dbo].[DashboardIndicators] DI on DI.IndicatorNumber = DID.IndicatorNumber and DI.CorporateId = @CID and DI.FacilityId  in (Select FID from @SelT)
Where DID.CorporateId = @CID and DID.FacilityId in (Select FID from @SelT) and DID.[Year] =@FiscalYear and DID.[Month] <= @Month
--Group by DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,DID.ExternalValue1

---- Monthly
insert into @Results
Select DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,'',DID.ExternalValue1 ResultType, cast(DID.StatisticData as decimal(18,4)) Result,
Case When DID.ExternalValue1 = 1 Then 'CMB' Else 'CMA' END Tag,0,'',DID.SubCategory1,DID.SubCategory2,null,null from [dbo].[DashboardIndicatorData] DID
-- inner join [dbo].[DashboardIndicators] DI on DI.IndicatorNumber = DID.IndicatorNumber and DI.CorporateId = @CID and DI.FacilityId  in (Select FID from @SelT)
Where DID.CorporateId = @CID and DID.FacilityId in (Select FID from @SelT) and DID.[Year] =@FiscalYear and DID.[Month] = @Month
-- Group by DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,DID.ExternalValue1

---- Current YEAR - Ends


---- Previous Y;;EAR - Starts

Set @StartDate = DATEADD(YEAR,-1,@Startdate)
Set @TillDate = DATEADD(YEAR,-1,@TillDate)
Set @FiscalYear = year(@TillDate)

---- Year to Date
insert into @Results
Select DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,'',DID.ExternalValue1 ResultType, cast(DID.StatisticData as decimal(18,4)) Result,
Case When DID.ExternalValue1 = 1 Then 'PYTB' Else 'PYTA' END Tag,0,'',DID.SubCategory1,DID.SubCategory2,null,null from [dbo].[DashboardIndicatorData] DID
-- inner join [dbo].[DashboardIndicators] DI on DI.IndicatorNumber = DID.IndicatorNumber and DI.CorporateId = @CID and DI.FacilityId  in (Select FID from @SelT)
Where DID.CorporateId = @CID and DID.FacilityId in (Select FID from @SelT) and DID.[Year] =@FiscalYear and DID.[Month] <= @Month
-- Group by DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,DID.ExternalValue1

---- Monthly
insert into @Results
Select DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,'',DID.ExternalValue1 ResultType, cast(DID.StatisticData as decimal(18,4)) Result,
Case When DID.ExternalValue1 = 1 Then 'PMB' Else 'PMA' END Tag,0,'',DID.SubCategory1,DID.SubCategory2,null,null from [dbo].[DashboardIndicatorData] DID
-- inner join [dbo].[DashboardIndicators] DI on DI.IndicatorNumber = DID.IndicatorNumber and DI.CorporateId = @CID and DI.FacilityId  in (Select FID from @SelT)
Where DID.CorporateId = @CID and DID.FacilityId in (Select FID from @SelT) and DID.[Year] =@FiscalYear and DID.[Month] = @Month
-- Group by DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,DID.ExternalValue1



---- Previous YEAR - Ends



---- Fix Avergaes for YTD Data --- STARTS
SET @N1 = 0
Set @N1 = (DATEDIFF(d, DATEADD(yy, DATEDIFF(yy,0,@LocalDateTime), 0), DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0,@LocalDateTime) + 1, 1))));

	Update @Results Set Result = isnull(Result,0.0)/isnull(@Month,1.0) Where IndicatorNumber in ('107') and Tag in ('CYTA','CYTB','PYTA','PYTB')
	--Update @Results Set Result = isnull(Result,0.0)/isnull(@Month,1.0) Where IndicatorNumber in ('144','109','107') and Tag in ('CYTA','CYTB','PYTA','PYTB')

---- Fix Avergaes for YTD Data --- ENDS
;With Report     
AS    
(    
select * from  
(
Select * from @Results
) src    
pivot    
(    
sum(Result)    
for Tag in (CMB,CMA,PMB,PMA,CYTB,CYTA,PYTB,PYTA)    
)piv    
)

insert into @FormatResults Select * from Report;


--Select * from @FormatResults
---- Updating DashBoard Name, FormatType and ExternalValue2 (Taken out from Inner join as it was giving issues in SUM) --- STARTS
---- NOTE: Commented inner joins in first queries, getting simple data as it is - ALSO changed IndicatorNumber column to INum due to below query update for Temp Table
	Update @FormatResults Set DashBoard =  DI.DashBoard, FormatType = DI.FormatType, EV2 = DI.ExternalValue2, [OwnerShip] = DI.[OwnerShip],TypeOfData =  DI.ExternalValue3
		from [dbo].[DashboardIndicators] DI 
		Where DI.IndicatorNumber = INum and DI.CorporateId = @CID 
		and (DI.FacilityId in (Select FID from @SelT) OR DI.FacilityId = 0)
---- Updating DashBoard Name, FormatType and ExternalValue2 (Taken out from Inner join as it was giving issues in SUM) --- ENDS

	--Print 'Section1'
--->>> Special case where no sum/Average it has to be Number as it is of present Month (eg:Total Operating Beds)
	Update @FormatResults Set CYTA = CMA, CYTB = CMB, PYTA = PMA, PYTB = PMB Where INum = '108'

	Declare @A1 numeric(18,4)=0, @A2 numeric(18,4)=0, @A3 numeric(18,4)=0, @A4 numeric(18,4)=0
	Declare @M1 numeric(18,4)=0, @M2 numeric(18,4)=0, @M3 numeric(18,4)=0, @M4 numeric(18,4)=0

	Select @A1 = CYTB From @FormatResults Where INum = '103' And ResultType = 1
	Select @A2 = CYTB From @FormatResults Where INum = '101' And ResultType = 1

	Set @A2 = ISNULL(@A2,0.0000)

	--If @A2 = 0.0000 or @A2= 0
	--	Set @A2 = 1
	-->>> Special Case for calculating the Avrage Length of Stay.
	Update @FormatResults Set CYTB = CASE WHEN  @A2 = 0.0000 or @A2= 0 THEN 0.0000 ELSE (@A1 / @A2) END Where INum = '106' And ResultType = 1

	Select @A1 = CYTA From @FormatResults Where INum = '103' And ResultType = 2
	Select @A2 = CYTA From @FormatResults Where INum = '101' And ResultType = 2
	--Print 'Section2'
	Set @A2 = ISNULL(@A2,0.0000)

	--If @A2 = 0.0000 or @A2= 0
	--	Set @A2 = 1
	-->>> Special Case for calculating the Avrage Length of Stay.
	Update @FormatResults Set CYTA = CASE WHEN  @A2 = 0.0000 or @A2= 0 THEN 0.0000 ELSE  (@A1 / @A2) END Where INum = '106' And ResultType = 2

	Select @A1 = PYTA From @FormatResults Where INum = '103' And ResultType = 2
	Select @A2 = PYTA From @FormatResults Where INum = '101' And ResultType = 2

	Set @A2 = ISNULL(@A2,0.0000)

	--If @A2 = 0.0000 or @A2= 0
	--	Set @A2 = 1
	-->>> Special Case for calculating the Avrage Length of Stay.
	Update @FormatResults Set PYTA =  CASE WHEN  @A2 = 0.0000 or @A2= 0 THEN 0.0000 ELSE  (@A1 / @A2) END Where INum = '106' And ResultType = 2


	----> Special Case for Adjusted Patietn Days (i.e. Indicator Number '105') starts here
	--Print 'Section3'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	
	--> THis is for Actuals
	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '103' And ResultType = 2			--In Patient Days
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '160' And ResultType = 2			--InPatient Revenue
	Select @A3 = CYTA, @M3 = CMA From @FormatResults Where INum = '161' And ResultType = 2			--OutPatient Revenue

	Set @M1 = isnull(@M1,'0')
	--If @M1 = '0'
	--	Set @M1 = '1'

	Set @M2 = isnull(@M2,'0')
			--If @M2 = '0'
			--	Set @M2 = '1'

	Set @M3 = isnull(@M3,'0')

	Set @A1 = isnull(@A1,'0')
	--If @A1 = '0'
	--	Set @A1 = '1'

	Set @A2 = isnull(@A2,'0')
			--If @A2 = '0'
			--	Set @A2 = '1'

	Set @A3 = isnull(@A3,'0')

    Update @FormatResults Set 
	--CYTA = CASE WHEN  @A2 = 0.0000 or @A2= 0 OR @A1 = 0.0000 or @A1= 0  THEN 0.0000 ELSE @A1 + (@A3 / (@A2 / @A1)) END, 
	--CMA =  CASE WHEN  @M2 = 0.0000 or @M2= 0 OR @M1 = 0.0000 or @M1= 0  THEN 0.0000 ELSE @M1 + (@M3 / (@M2 / @M1))  END
	CYTA = @A1 + CASE WHEN  @A2 = 0.0000 or @A2= 0 OR @A1 = 0.0000 or @A1= 0  THEN 0.0000 ELSE  (@A3 / (@A2 / @A1)) END, 
	CMA =  @M1 + CASE WHEN  @M2 = 0.0000 or @M2= 0 OR @M1 = 0.0000 or @M1= 0  THEN 0.0000 ELSE  (@M3 / (@M2 / @M1))  END
	Where INum = '105' And ResultType = 2

	--Print 'Section4'
	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTB, @M1 = CMB From @FormatResults Where INum = '103' And ResultType = 1			--In Patient Days
	Select @A2 = CYTB, @M2 = CMB From @FormatResults Where INum = '160' And ResultType = 1			--InPatient Revenue
	Select @A3 = CYTB, @M3 = CMB From @FormatResults Where INum = '161' And ResultType = 1			--OutPatient Revenue

	Set @M1 = isnull(@M1,'0')
				--If @M1 = '0'
				--	Set @M1 = '1'

	Set @M2 = isnull(@M2,'0')
			--If @M2 = '0'
			--	Set @M2 = '1'

	Set @M3 = isnull(@M3,'0')

	Set @A1 = isnull(@A1,'0')
	--If @A1 = '0'
	--	Set @A1 = '1'

	Set @A2 = isnull(@A2,'0')
			--If @A2 = '0'
			--	Set @A2 = '1'

	Set @A3 = isnull(@A3,'0')

	--	CYTB = @A1 + (@A3 / (@A2 / @A1)), 
	--CMB =  @M1 + (@M3 / (@M2 / @M1))  
    Update @FormatResults Set 
	CYTB = @A1 + CASE WHEN  @A2 = 0.0000 or @A2= 0 OR @A1 = 0.0000 or @A1= 0  THEN 0.0000 ELSE  (@A3 / (@A2 / @A1)) END,
	CMB =  @M1 + CASE WHEN  @M2 = 0.0000 or @M2= 0 OR @M1 = 0.0000 or @M1= 0  THEN 0.0000 ELSE  (@M3 / (@M2 / @M1))  END
	Where INum = '105' And ResultType = 1

	--> This is for Prior Year
	--Print 'Section5'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = PYTA, @M1 = PMA  From @FormatResults Where INum = '103' And ResultType = 2			--In Patient Days
	Select @A2 = PYTA, @M2 = PMA From @FormatResults Where INum = '160' And ResultType = 2			--InPatient Revenue
	Select @A3 = PYTA, @M3 = PMA From @FormatResults Where INum = '161' And ResultType = 2			--OutPatient Revenue

	Set @M1 = isnull(@M1,'0')
				--If @M1 = '0'
				--	Set @M1 = '1'

	Set @M2 = isnull(@M2,'0')
			--If @M2 = '0'
			--	Set @M2 = '1'

	Set @M3 = isnull(@M3,'0')

	Set @A1 = isnull(@A1,'0')
	--If @A1 = '0'
	--	Set @A1 = '1'

	Set @A2 = isnull(@A2,'0')
			--If @A2 = '0'
			--	Set @A2 = '1'

	Set @A3 = isnull(@A3,'0')

    Update @FormatResults Set 
	PYTA =  @A1 + CASE WHEN  @A2 = 0.0000 or @A2= 0 OR @A1 = 0.0000 or @A1= 0  THEN 0.0000 ELSE  (@A3 / (@A2 / @A1)) END,
	PMA =   @M1 + CASE WHEN  @M2 = 0.0000 or @M2= 0 OR @M1 = 0.0000 or @M1= 0  THEN 0.0000 ELSE  (@M3 / (@M2 / @M1))  END
	Where INum = '105' And ResultType = 2
	----> Special Case for Adjusted Patietn Days (i.e. Indicator Number '105') Ends Here

	--Print 'Section6'
	------ New MARGIN CALCULATIONS - STARTS  ---- XXXXXXXXXXXXXXXXXXXXx

	----> Special Case for All SWB  /Net Revnue (i.e. Indicator Number '120') STARTS
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @A4 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Set @M4 = 0

	--> THis is for Actuals
	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '111' And ResultType = 2			---- Get SWB Direct
	Select @A3 = CYTA, @M3 = CMA From @FormatResults Where INum = '155' And ResultType = 2			---- Get SWB Indirect Costs
	Select @A4 = CYTA, @M4 = CMA From @FormatResults Where INum = '260' And ResultType = 2			---- Get New Market Development SWB 
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1
	
	
	--Select (isnull(@A2,0) + isnull(@A3,0) + isnull(@A4,0)),isnull(@A1,1)
    Update @FormatResults 
	Set CYTA = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE  (((isnull(@A2,0) + isnull(@A3,0) + isnull(@A4,0)) / isnull(@A1,1)) *100) END,
	CMA = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE (((isnull(@M3,0) +isnull(@M2,0)+ isnull(@M4,0)) / isnull(@M1,1))*100) END Where INum = '120' And ResultType = 2
	--Print 'Section7'

	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @A4 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Set @M4 = 0

	Select @A1 = CYTB, @M1 = CMB From @FormatResults Where INum = '110' And ResultType = 1			---- Get Net Revenue
	Select @A2 = CYTB, @M2 = CMB From @FormatResults Where INum = '111' And ResultType = 1			---- Get SWB Direct
	Select @A3 = CYTB, @M3 = CMB From @FormatResults Where INum = '155' And ResultType = 1			---- Get SWB Indirect Costs
	Select @A4 = CYTB, @M4 = CMB From @FormatResults Where INum = '260' And ResultType = 1			---- Get SWB Indirect Costs
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1

	--Select (isnull(@A2,0) + isnull(@A3,0) + isnull(@A4,0)),isnull(@A1,1)
	 Update @FormatResults 
	 Set CYTB =   CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE  (((isnull(@A3,0) + isnull(@A2,0) + isnull(@A4,0)) / isnull(@A1,1)) *100) END,
	  CMB = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE (((isnull(@M3,0) +isnull(@M2,0)+ isnull(@M4,0)) / isnull(@M1,1))*100) END Where INum = '120' And ResultType = 1

	--> This is for Prior Year
	--Print 'Section8'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @A4 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Set @M4 = 0

	Select @A1 = PYTA, @M1 = PMA  From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = PYTA, @M2 = PMA From @FormatResults Where INum = '111' And ResultType = 2			---- Get SWB Direct
	Select @A3 = PYTA, @M3 = PMA From @FormatResults Where INum = '155' And ResultType = 2			---- Get SWB Indirect Costs
	Select @A4 = PYTA, @M4 = PMA From @FormatResults Where INum = '260' And ResultType = 2	
	--Select @A1,@A2,@A3,@M1,@M2,@M3
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1
	--Select @A1,@A2,@A3 ,@M1,@M2,@M3
	Update @FormatResults 
	Set PYTA = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE (((isnull(@A3,0) + isnull(@A2,0) + isnull(@A4,0)) / isnull(@A1,1)) *100) END, 
	PMA =  CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE (((isnull(@M3,0) +isnull(@M2,0)+ isnull(@A4,0)) / isnull(@M1,1))*100) END Where INum = '120' And ResultType = 2

	----> Special Case for SWB%/Net Revnue (i.e. Indicator Number '120') ---- XXXXX ENDS

	----- Next Margin is
	
	----> Special Case for Indirect % of Net Revenue (i.e. Indicator Number '121') STARTS
	--> THis is for Actuals
	--Print 'Section9'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @A4 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Set @M4 = 0

	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '115' And ResultType = 2			---- Get SWB Indirect Costs
	Select @A3 = CYTA, @M3 = CMA From @FormatResults Where INum = '155' And ResultType = 2			---- Get Other Indirect Costs
	Select @A4 = CYTA, @M4 = CMA From @FormatResults Where INum = '281' And ResultType = 2			---- Get Marketing & BD Costs 
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1

	Update @FormatResults 
	Set CYTA = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0) + ISNULL(@A3,0) + ISNULL(@A4,0)) / isnull(@A1,1) * 100) END, 
	CMA = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE  ((isnull(@M2,0) + isnull(@M3,0) + ISNULL(@M4,0)) / isnull(@M1,1)*100) END
	Where INum = '121' And ResultType = 2


	--> This is for Budgets
	--Print 'Section10'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @A4 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Set @M4 = 0

	Select @A1 = CYTB, @M1 = CMB From @FormatResults Where INum = '110' And ResultType = 1			---- Get Net Revenue
	Select @A2 = CYTB, @M2 = CMB From @FormatResults Where INum = '155' And ResultType = 1			---- Get SWB Indirect Costs
	Select @A3 = CYTB, @M3 = CMB From @FormatResults Where INum = '115' And ResultType = 1			---- Get Other Indirect Costs
	Select @A4 = CYTA, @M4 = CMA From @FormatResults Where INum = '281' And ResultType = 1			---- Get Marketing & BD Costs 
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1

	 Update @FormatResults 
	 Set CYTB = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0) + ISNULL(@A3,0)  + ISNULL(@A4,0)) / isnull(@A1,1) * 100) END, 
	 CMB = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE ((isnull(@M2,0) + isnull(@M3,0)+ ISNULL(@M4,0)) / isnull(@M1,1)*100) END
	 Where INum = '121' And ResultType = 1

	--> This is for Prior Year

	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @A4 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Set @M4 = 0

	Select @A1 = PYTA, @M1 = PMA  From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = PYTA, @M2 = PMA From @FormatResults Where INum = '155' And ResultType = 2			---- Get SWB Indirect Costs
	Select @A3 = PYTA, @M3 = PMA From @FormatResults Where INum = '115' And ResultType = 2			---- Get Other Indirect Costs
	Select @A4 = PYTA, @M4 = PMA From @FormatResults Where INum = '281' And ResultType = 2			---- Get Marketing & BD Costs 
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1

	Update @FormatResults 
	Set PYTA = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0) + ISNULL(@A3,0)  + ISNULL(@A4,0)) / isnull(@A1,1) * 100) END, 
	PMA = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE ((isnull(@M2,0) + isnull(@M3,0)+ ISNULL(@M4,0)) / isnull(@M1,1)*100)  END
	Where INum = '121' And ResultType = 2

	----> Special Case for SWB INDIRECT %/Net Revnue (i.e. Indicator Number '121') ---- XXXXX ENDS

	--Print 'Section11'
	----> Special Case for EBIDTA % /Net Revnue (i.e. Indicator Number '122') STARTS
	--> THis is for Actuals

	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '116' And ResultType = 2			---- Get EBITDA

	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1

	Update @FormatResults 
	Set CYTA = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0)) / isnull(@A1,1)*100) END, 
	CMA = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE ((isnull(@M2,0)) / isnull(@M1,1)*100)  END
	Where INum = '122' And ResultType = 2

	--Print 'Section12'
	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTB, @M1 = CMB From @FormatResults Where INum = '110' And ResultType = 1			---- Get Net Revenue
	Select @A2 = CYTB, @M2 = CMB From @FormatResults Where INum = '116' And ResultType = 1			---- Get EBITDA

	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1

	 Update @FormatResults 
	 Set CYTB = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0)) / isnull(@A1,1)*100) END, 
	 CMB = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE ((isnull(@M2,0)) / isnull(@M1,1)*100)  END
	 Where INum = '122' And ResultType = 1

	--> This is for Prior Year
	--Print 'Section13'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = PYTA, @M1 = PMA  From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = PYTA, @M2 = PMA From @FormatResults Where INum = '116' And ResultType = 2			---- Get EBITDA
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1

	Update @FormatResults 
	Set PYTA = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE  ((isnull(@A2,0)) / isnull(@A1,1)*100) END,
	PMA = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE  ((isnull(@M2,0) / isnull(@M1,1))*100) END
	Where INum = '122' And ResultType = 2

	----> Special Case for EBIDTA %/Net Revnue (i.e. Indicator Number '122') ---- XXXXX ENDS

	--Print 'Section14'
	----> Special Case for (NET Income Margin) Net Income/Loss  /Net Revnue (i.e. Indicator Number '162') STARTS
	--> THis is for Actuals
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '119' And ResultType = 2			---- Get Income/Loss
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1

	Update @FormatResults 
	Set CYTA = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0)) / isnull(@A1,1)*100) END,
	CMA = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE ((isnull(@M2,0)) / isnull(@M1,1)*100) END
	 Where INum = '162' And ResultType = 2

	--Print 'Section15'
	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTB, @M1 = CMB From @FormatResults Where INum = '110' And ResultType = 1			---- Get Net Revenue
	Select @A2 = CYTB, @M2 = CMB From @FormatResults Where INum = '119' And ResultType = 1			---- Get Income/Loss
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1
	 Update @FormatResults 
	 Set CYTB = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0)) / isnull(@A1,1)*100) END, 
	 CMB = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE ((isnull(@M2,0)) / isnull(@M1,1)*100)  END
	 Where INum = '162' And ResultType = 1
	 --Print 'Section16'
	--> This is for Prior Year
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = PYTA, @M1 = PMA  From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = PYTA, @M2 = PMA From @FormatResults Where INum = '119' And ResultType = 2			---- Get Income/Loss
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1
	Update @FormatResults 
	Set PYTA = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0)) / isnull(@A1,1)*100) END, 
	PMA =CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE ((isnull(@M2,0) / isnull(@M1,1))*100)  END
	Where INum = '162' And ResultType = 2

	--Print 'Section17'
	----> Special Case for (NET Income Margin) Net Income/Loss   %/Net Revnue (i.e. Indicator Number '123') ---- XXXXX ENDS

	----> Special Case for (Operating Margin) (Net Operating /Net Revenue)*100 (i.e. Indicator Number '145') STARTS
	--> THis is for Actuals
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '114' And ResultType = 2			---- Get Income/Loss
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Update @FormatResults 
	Set CYTA = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0)) / isnull(@A1,1)*100) END, 
	CMA = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE ((isnull(@M2,0)) / isnull(@M1,1)*100) END
	 Where INum = '145' And ResultType = 2

	--Print 'Section18'
	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTB, @M1 = CMB From @FormatResults Where INum = '110' And ResultType = 1			---- Get Net Revenue
	Select @A2 = CYTB, @M2 = CMB From @FormatResults Where INum = '114' And ResultType = 1			---- Get Income/Loss
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1
	 Update @FormatResults 
	 Set CYTB = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0)) / isnull(@A1,1)*100) END, 
	 CMB = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE ((isnull(@M2,0)) / isnull(@M1,1)*100) END
	 Where INum = '145' And ResultType = 1
	 --Print 'Section19'
	--> This is for Prior Year
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = PYTA, @M1 = PMA  From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = PYTA, @M2 = PMA From @FormatResults Where INum = '114' And ResultType = 2			---- Get Income/Loss
	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1

	Update @FormatResults 
	Set PYTA = CASE WHEN @A1 = 0.0000 OR @A1 = 0 THEN 0.0000 ELSE ((isnull(@A2,0)) / isnull(@A1,1)*100) END, 
	PMA = CASE WHEN @M1 = 0.0000 OR @M1 = 0 THEN 0.0000 ELSE ((isnull(@M2,0) / isnull(@M1,1))*100) END
	Where INum = '145' And ResultType = 2

	----> Special Case for (Operating Margin) (Net Operating /Net Revenue)*100 (i.e. Indicator Number '145') - XXXX ENDS
	--Print 'Section20'

	----> Special Case for (Average Daily Census) (i.e. Indicator Number '144') STARTS
	--> THis is for Actuals and Prior Year
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @A4 = 0
	Set @N1 = 0

	Set @N1 = (datepart(dd,EOMONTH(@TillDate)))
	Set @NumberOfDays = (DATEDIFF(d, DATEADD(yy, DATEDIFF(yy,0,@TillDate), 0), DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0, @TillDate) + 1, 1))))

	Select @A1 = CMA, @A2 = PMA,@A3 = CYTA From @FormatResults Where INum = '103' And ResultType = 2		---- Get In Patient Days
	Update @FormatResults Set CMA =  ISNULL(@A1,0) / @N1, PMA = ISNULL(@A2,0) / @N1 ,
	 CYTA = @A3 / @NumberOfDays Where INum = '144' And ResultType = 2

	--Print 'Section21'
	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Select @A1 = CMB, @A3 = CYTB From @FormatResults Where INum = '103' And ResultType = 1	    ---- Get In Patient Days
	Update @FormatResults Set CMB =  ISNULL(@A1,0) / @N1, CYTB = @A3/@NumberOfDays Where INum = '144' And ResultType = 1

	----> Special Case for (Average Daily Census) (i.e. Indicator Number '144') - XXXX ENDS

	--> Special Case for (Available Bed Days) (i.e. Indicator Number '123') STARTS
	--> THis is for Actuals and Prior Year
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Set @N1 = 0

	--Set @N1 = (DATEDIFF(d, DATEADD(yy, DATEDIFF(yy,0,GETDATE()), 0), DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0,GETDATE()) + 1, 1))))
	Set @N1 = (DATEDIFF(d, DATEADD(yy, DATEDIFF(yy,0,@TillDate), 0), DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0,@TillDate) + 1, 1))))

	--Select @A1 = CYTA, @A2 = PYTA From @FormatResults Where INum = '279' And ResultType = 2		---- Get Days in Month
	Select @A3 = CYTA, @M1 = PYTA From @FormatResults Where INum = '108' And ResultType = 2		----  Get Total Operating Beds
	--Print @N1;
	--Print '123';
	if(@A3 = 0.0000)
		SET @A3 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Set @M2 = (Cast(@N1 as numeric(18,4))*(Cast(@A3 as numeric(18,4)) )) 
	Set @M3 = (Cast(@N1 as numeric(18,4))*(Cast(@M1 as numeric(18,4)) ))

	Update @FormatResults Set CYTA =  @M2, PYTA = @M3 Where INum = '123' And ResultType = 2


	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Set @N1 = 0

	Set @N1 = (DATEDIFF(d, DATEADD(yy, DATEDIFF(yy,0,@TillDate), 0), DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0,@TillDate) + 1, 1))))
	--Select @A1 = CYTB From @FormatResults Where INum = '279' And ResultType = 1		---- Get Days in Month
	Select @A3 = CYTB From @FormatResults Where INum = '108' And ResultType = 1		----  Get Total Operating Beds

	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@A3 = 0.0000)
	--	SET @A3 =1

	Set @M2 =(Cast(@N1 as numeric(18,4))*(Cast(@A3 as numeric(18,4)))) 
	
	Update @FormatResults Set CYTB =  @M2 Where INum = '123' And ResultType = 1

	----> Special Case for (Available Bed Days) (i.e. Indicator Number '123') - XXXX ENDS

	--Print 'Section22'
	----> Special Case for (Occupany Rate) (i.e. Indicator Number '109') STARTS
	--> THis is for Actuals and Prior Year
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0
	Set @N1 = 0

	--Set @N1 = (DATEDIFF(d, DATEADD(yy, DATEDIFF(yy,0,GETDATE()), 0), DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0,GETDATE()) + 1, 1))))

	Select @A1 = CYTA, @A2 = PYTA From @FormatResults Where INum = '103' And ResultType = 2		---- Get In Patient Days
	--Select @A3 = CYTA, @M1 = PYTA From @FormatResults Where INum = '108' And ResultType = 2	---- Get Total Operating Beds
	Select @A3 = CYTA, @M1 = PYTA From @FormatResults Where INum = '123' And ResultType = 2		---- Get Available Bed Days
	
	--if(@A3 = 0.0000)
	--	SET @A3 =1
	--if(@M1 = 0.0000)
	--	SET @M1 =1
	Set @M2 = CASE WHEN (@A3 = 0.0000) OR (@A3 = 0) THEN 0.0000 ELSE  (Cast(@A1 as numeric(18,4))/(Cast(@A3 as numeric(18,4)) ))*100 END
	Set @M3 = CASE WHEN (@M1 = 0.0000) OR (@M1 = 0) THEN 0.0000 ELSE (Cast(@A2 as numeric(18,4))/(Cast(@M1 as numeric(18,4)) ))*100 END
	
	Update @FormatResults Set CYTA =  @M2, PYTA = @M3 Where INum = '109' And ResultType = 2


	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTB From @FormatResults Where INum = '103' And ResultType = 1		---- Get In Patient Days
	--Select @A3 = CYTB From @FormatResults Where INum = '108' And ResultType = 1		---- Get Total Operating Beds
	Select @A3 = CYTB From @FormatResults Where INum = '123' And ResultType = 1		---- Get Available Bed Days

	--if(@A1 = 0.0000)
	--	SET @A1 =1
	--if(@A3 = 0.0000)
	--	SET @A3 =1

	Set @M2 =CASE WHEN (@A3 = 0.0000) OR (@A3 = 0) THEN 0.0000 ELSE (Cast(@A1 as numeric(18,4))/(Cast(@A3 as numeric(18,4))))*100 END
	
	Update @FormatResults Set CYTB =  @M2 Where INum = '109' And ResultType = 1

	----> Special Case for (Occupany Rate) (i.e. Indicator Number '109') - XXXX ENDS

	------ New MARGIN CALCULATIONS - ENDS ---- XXXXXXXXXXXXXXXXXXXXx
	---- ****************** ADDED BY SHASHANK**********************
	----> Special Case for Total Dollar Claims Resubmitted   (i.e. Indicator Number '270') STARTS
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	--> THis is for Actuals
	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '231' And ResultType = 2			---- Total Dollar Claims Resubmitted  
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '269' And ResultType = 2			---- Total Dollar Submitted Claims  
	
	SET @A1 =ISNULL(@A1,0.0000)
	SET @M1 =ISNULL(@M1,0.0000)
	--Print 'Total Dollar Submitted Claims ';
	--Print @A2;
	--Print @A1;
    Update @FormatResults Set CYTA = CASE WHEN (@A1 = 0.0000) THEN 0.0000 ELSE  ((isnull(@A2,0) / isnull(@A1,1))) * 100 END, 
							   CMA = CASE WHEN (@M1 = 0.0000) THEN 0.0000 ELSE (((isnull(@M2,0)) / isnull(@M1,1))) * 100 END Where INum = '270' And ResultType = 2

	--Print 'Section7'

	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTB, @M1 = CMB From @FormatResults Where INum = '231' And ResultType = 1			---- Total Dollar Claims Resubmitted
	Select @A2 = CYTB, @M2 = CMB From @FormatResults Where INum = '269' And ResultType = 1			---- Total Dollar Submitted Claims  
	SET @A1 =ISNULL(@A1,0.0000)
	SET @M1 =ISNULL(@M1,0.0000)
	 Update @FormatResults Set CYTB =CASE WHEN (@A1 = 0.0000) THEN 0.0000 ELSE   (((isnull(@A2,0)) / isnull(@A1,1))) * 100 END,
							 CMB = CASE WHEN (@M1 = 0.0000) THEN 0.0000 ELSE  (((isnull(@M2,0)) / isnull(@M1,1))) * 100 END Where INum = '270' And ResultType = 1

	--> This is for Prior Year
	--Print 'Section8'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = PYTA, @M1 = PMA  From @FormatResults Where INum = '231' And ResultType = 2			---- Total Dollar Claims Resubmitted
	Select @A2 = PYTA, @M2 = PMA From @FormatResults Where INum = '269' And ResultType = 2			---- Total Dollar Submitted Claims  

	--Select @A1,@A2,@A3,@M1,@M2,@M3
	SET @A1 =ISNULL(@A1,0.0000)
	SET @M1 =ISNULL(@M1,0.0000)
	Update @FormatResults Set PYTA = CASE WHEN (@A1 = 0.0000) THEN 0.0000 ELSE ((isnull(@A2,0)) / isnull(@A1,1)) * 100 END, 
							PMA =CASE WHEN (@M1 = 0.0000) THEN 0.0000 ELSE  ((isnull(@M2,0)) / isnull(@M1,1)) * 100 END Where INum = '270' And ResultType = 2

	----> Special Case for Total Dollar Claims Resubmitted   (i.e. Indicator Number '270') Ends

	---- ****************** ADDED BY SHASHANK**********************
	----> Special Case for A/R Net Days   (i.e. Indicator Number '131') STARTS
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	--> THis is for Actuals
	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '264' And ResultType = 2			---- Net Accounts Receivable Balance  
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '110' And ResultType = 2			---- Net Revenue
	SET @NumberOfDays = (datepart(dd,EOMONTH(@TillDate))); 

	if(@A2 = 0.0000)
		SET @A2 =1
	if(@M2 = 0.0000)
		SET @M2 =1
	
    Update @FormatResults Set CYTA =  ((isnull(@A1,0) / ((isnull(@A2,1))/@NumberOfDays))), CMA = (((isnull(@M1,0)) / ((isnull(@M2,1))/@NumberOfDays))) Where INum = '131' And ResultType = 2
	--Print 'Section7'

	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTB, @M1 = CMB From @FormatResults Where INum = '264' And ResultType = 1			---- Net Accounts Receivable Balance  
	Select @A2 = CYTB, @M2 = CMB From @FormatResults Where INum = '110' And ResultType = 1			---- Net Revenue
	if(@A2 = 0.0000)
		SET @A2 =1
	if(@M2 = 0.0000)
		SET @M2 =1
	 --Update @FormatResults Set CYTB =  (((isnull(@A1,0)) / isnull(@A2,1))*100), CMB = (((isnull(@M2,0)) / isnull(@M1,1))*100) Where INum = '270' And ResultType = 1
	 Update @FormatResults Set CYTB =  ((isnull(@A1,0) / ((isnull(@A2,1))/@NumberOfDays))), CMB = (((isnull(@M1,0)) / ((isnull(@M2,1))/@NumberOfDays))) Where INum = '131' And ResultType = 1

	--> This is for Prior Year
	--Print 'Section8'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = PYTA, @M1 = PMA  From @FormatResults Where INum = '264' And ResultType = 2			---- Net Accounts Receivable Balance
	Select @A2 = PYTA, @M2 = PMA From @FormatResults Where INum = '110' And ResultType = 2			---- Net Revenue

	if(@A2 = 0.0000)
		SET @A2 =1
	if(@M2 = 0.0000)
		SET @M2 =1
	--Update @FormatResults Set PYTA =  ((isnull(@A2,0)) / isnull(@A1,1)), PMA = ((isnull(@M2,0)) / isnull(@M1,1)) Where INum = '270' And ResultType = 2
	Update @FormatResults Set PYTA =  ((isnull(@A1,0) / ((isnull(@A2,1))/@NumberOfDays))), PMA = (((isnull(@M1,0)) / ((isnull(@M2,1))/@NumberOfDays))) Where INum = '131' And ResultType = 2

	--> This is for Prior Year
	--Print 'Section8'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = PYTB, @M1 = PMB  From @FormatResults Where INum = '264' And ResultType = 1			---- Net Accounts Receivable Balance
	Select @A2 = PYTB, @M2 = PMB From @FormatResults Where INum = '110' And ResultType = 1			---- Net Revenue

	if(@A2 = 0.0000)
		SET @A2 =1
	if(@M2 = 0.0000)
		SET @M2 =1
	--Update @FormatResults Set PYTA =  ((isnull(@A2,0)) / isnull(@A1,1)), PMA = ((isnull(@M2,0)) / isnull(@M1,1)) Where INum = '270' And ResultType = 2
	Update @FormatResults Set PYTB =  ((isnull(@A1,0) / ((isnull(@A2,1))/@NumberOfDays))), PMB = (((isnull(@M1,0)) / ((isnull(@M2,1))/@NumberOfDays))) Where INum = '131' And ResultType = 1

	----> Special Case for Total Dollar Claims Resubmitted   (i.e. Indicator Number '270') Ends

	---- ****************** ADDED BY SHASHANK**********************
	----> Special Case for Opening Cash (i.e. Indicator Number ' 288') STARTS
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	--> THis is for Actuals
	Select @A1 = Cast(StatisticData as Numeric(18,4)) From DashboardIndicatorData Where IndicatorNumber = '288' 
				And ExternalValue1 = 2 and Month =12 and Year = @previousYear and FacilityId = @FID and CorporateId =@CID
	Update @FormatResults SET CYTA = @A1 Where INum = '288' and ResultType = 2	

	--> THis is for BUdget
	Set @A1 = 0
	Select @A1 = Cast(StatisticData as Numeric(18,4)) From DashboardIndicatorData Where IndicatorNumber = '288' 
				And ExternalValue1 = 1 and Month =12 and Year = @previousYear and FacilityId = @FID and CorporateId =@CID
	Update @FormatResults SET CYTB = ISNULL(@A1,0) Where INum = '288' and ResultType = 1	
	--Print 'Section7'

	---- ****************** ADDED BY SHASHANK**********************
	----> Special Case for Opening Cash (i.e. Indicator Number ' 288') STARTS
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	--> THis is for Actuals
	Select @A1 = CYTA  From @FormatResults Where INum = '288' And ResultType = 2			---- Net Accounts Receivable Balance
	Select @A2 = CYTA  From @FormatResults Where INum = '128' And ResultType = 2			
	Update @FormatResults SET CYTA = ISNULL(@A1,0)  + ISNULL(@A2,0) Where INum = '129' And ResultType = 2

	--> THis is for BUdget
	Set @A1 = 0
	Set @A2 = 0
	Select @A1 = CYTB  From @FormatResults Where INum = '288' And ResultType = 1			---- Net Accounts Receivable Balance
	Select @A2 = CYTB  From @FormatResults Where INum = '128' And ResultType = 1			
	Update @FormatResults SET CYTB = ISNULL(@A1,0)  + ISNULL(@A2,0) Where INum = '129' And ResultType = 1
	--Print 'Section7'
	
	--@FacilityCount
	--Select * from @FormatResults
	Update @FormatResults SET CMB = ISNULL(CMB,0)/@FacilityCount,CMA=ISNULL(CMA,0)/@FacilityCount,PMB =ISNULL(PMB,0)/@FacilityCount,PMA=ISNULL(PMA,0)/@FacilityCount  Where  FormatType = 2 and EV2=1
	--print 'Before Select'
	--Select * from @FormatResults
---- Returning Resullt in desired format
--Select * from @FormatResults
Select INum IndicatorNumber,
(isnull(DashBoard,'') +' ' +dbo.GetSubCategory1Name(Subcategory1,0) +' '+ dbo.GetSubCategory2Name(Subcategory1,Subcategory2)) DashBoard,MAX(FormatType) FormatType, MAX(ISNULL(CAST(EV2 as int), 0)) EV2,
		isnull(sum(CMB),0.0) CMB, --Current Month Budget
		isnull(sum(CMA),0.0) CMA, --Current Month Actual
		(isnull(sum(CMA),0.0) - (isnull(sum(CMB),0.0))) CMVar,--Current Month Varinace
		case WHEN isnull(sum(CMB),0.0) = 0.0 
		Then 0 		
		WHEN sum(CMB) = 0.0000
		THEN ((isnull(sum(CMA),0.0) - (isnull(sum(CMB),0.0)))/1)
		Else ((isnull(sum(CMA),0.0) - (isnull(sum(CMB),0.0)))/ABS(isnull(sum(CMB),1.0))) END CMVarPercentage ,--Current Month Varinace Percentage
		isnull(sum(PMB),0.0) PMB, --Previous Year Month Budget
		isnull(sum(PMA),0.0) PMA, --Previous Year Month Actual
		(isnull(sum(CMA),0.0) - (isnull(sum(PMA),0.0))) PMVar,--Previous Year Varinace
		case WHEN isnull(sum(PMA),0.0) = 0.0 
		Then 0 		
		WHEN sum(PMA) = 0.0000 or sum(PMA) = 0
		THEN ((isnull(sum(PMA),0.0) - (isnull(sum(CMA),0.0)))/1)
		Else ((isnull(sum(CMA),0.0) - (isnull(sum(PMA),0.0)))/ABS(isnull(sum(PMA),1.0))) END PMVarPercentage,--Previous Year Varinace Percentage
		isnull(sum(CYTB),0.0) CYTB, --Year TO DATE Month Budget
		isnull(sum(CYTA),0.0) CYTA,	--Year TO DATE Month Actual
		(isnull(sum(CYTA),0.0) - (isnull(sum(CYTB),0.0))) CYTBVar,--Year TO DATE Variance
		case WHEN isnull(sum(CYTB),0.0) = 0.0 
		Then 0 	
		WHEN sum(CYTB) = 0.0000
		THEN ((isnull(sum(CYTA),0.0) - (isnull(sum(CYTB),0.0)))/1)
		Else ((isnull(sum(CYTA),0.0) - (isnull(sum(CYTB),0.0)))/ABS(isnull(sum(CYTB),1.0))) END CYTBVarPercentage,--Year TO DATE Variance Percentage
		isnull(sum(PYTB),0.0) PYTB, -- Previous Year TO DATE Month Budget
		isnull(sum(PYTA),0.0) PYTA,	-- Previous Year TO DATE Month Actual
		(isnull(sum(CYTA),0.0) - (isnull(sum(PYTA),0.0))) PYTBVar,-- Previous Year TO DATE Variance
		case WHEN isnull(sum(PYTA),0.0) = 0.0 
		Then 0 		
		WHEN sum(PYTA) = 0.0000
		THEN ((isnull(sum(PYTA),0.0) - (isnull(sum(CYTA),0.0)))/1)
		Else ((isnull(sum(CYTA),0.0) - (isnull(sum(PYTA),0.0)))/ABS(isnull(sum(PYTA),1.0))) END PYTBVarPercentage-- Previous Year TO DATE Varinace Percenatge
		,MAX([OwnerShip]) AS 'OwnerShip',
		MAX(TypeOfData) 'TypeOfData',
		(Select [dbo].[GetColorCodeByVarRange] ((isnull(sum(CMA),0.0) - (isnull(sum(CMB),0.0))),1,MAX(TypeOfData))) CMVarColor,
		--(Select TOP(1)ISNULL(ColorCode,'1') From DashboardParameters WHERE ((isnull(sum(CMA),0.0) - (isnull(sum(CMB),0.0))) >=  Cast(ISNULL(RangeTo,0) as Numeric(18,4)) 
		--and ((isnull(sum(CMA),0.0) - (isnull(sum(CMB),0.0))) <= Cast(ISNULL(RangeFrom,0) as Numeric(18,4))) 
		--and ValueType =1 and IndicatorCategory=Max(TypeOfData)))  CMVarColor,

		(Select [dbo].[GetColorCodeByVarRange] (CASE WHEN sum(CMB) = 0.0000 THEN 0.0000 ELSE ((isnull(sum(CMA),0.0) - (isnull(sum(CMB),0.0)))/ABS(isnull(sum(CMB),1.0))) *100 END,2,MAX(TypeOfData))) CMVarPercentColor,
		--(Select TOP(1) ISNULL(ColorCode,'1') From DashboardParameters WHERE CASE WHEN sum(CMB) = 0.0000 THEN 0.0000 ELSE ((isnull(sum(CMA),0.0) - (isnull(sum(CMB),0.0)))/ABS(isnull(sum(CMB),1.0))) END
		--Between  Cast(ISNULL(RangeTo,0) as Numeric(18,4)) 
		--AND Cast(ISNULL(RangeFrom,0) as Numeric(18,4)) 
		--and ValueType =2 and IndicatorCategory=Max(TypeOfData)) CMVarPercentColor,

		(Select [dbo].[GetColorCodeByVarRange] ( (isnull(sum(CMA),0.0) - (isnull(sum(PMA),0.0))),1,MAX(TypeOfData))) PMAColor,
		--(Select TOP(1)ISNULL(ColorCode,'1') From DashboardParameters WHERE (isnull(sum(CMA),0.0) - (isnull(sum(PMA),0.0))) Between  Cast(ISNULL(RangeTo,0) as Numeric(18,4)) 
		--AND Cast(ISNULL(RangeFrom,0) as Numeric(18,4)) 
		--and ValueType =1 and IndicatorCategory=Max(TypeOfData))  PMAColor,

		(Select [dbo].[GetColorCodeByVarRange] (CASE WHEN sum(PMA) = 0.0000 THEN 0.0000 ELSE ((isnull(sum(CMA),0.0) - (isnull(sum(PMA),0.0)))/ABS(isnull(sum(PMA),1.0))) *100 END,2,MAX(TypeOfData))) PMAPercentColor,
		--(Select TOP(1)ISNULL(ColorCode,'1') From DashboardParameters WHERE CASE WHEN sum(PMA) = 0.0000 THEN 0.0000 ELSE ((isnull(sum(CMA),0.0) - (isnull(sum(PMA),0.0)))/ABS(isnull(sum(PMA),1.0))) END
		--Between  Cast(ISNULL(RangeTo,0) as Numeric(18,4)) 
		--AND Cast(ISNULL(RangeFrom,0) as Numeric(18,4)) 
		--and ValueType =2 and IndicatorCategory=Max(TypeOfData))  PMAPercentColor,

		(Select [dbo].[GetColorCodeByVarRange] ((isnull(sum(CYTA),0.0) - (isnull(sum(CYTB),0.0))),1,MAX(TypeOfData))) CYTAVarColor,
		--(Select TOP(1)ISNULL(ColorCode,'1') From DashboardParameters WHERE (isnull(sum(CYTA),0.0) - (isnull(sum(CYTB),0.0))) Between  Cast(ISNULL(RangeTo,0) as Numeric(18,4)) 
		--AND Cast(ISNULL(RangeFrom,0) as Numeric(18,4))
		--and ValueType =1 and IndicatorCategory=Max(TypeOfData))  CYTAVarColor,

		(Select [dbo].[GetColorCodeByVarRange] (CASE WHEN sum(CYTB) = 0.0000 THEN 0.0000 ELSE ((isnull(sum(CYTA),0.0) - (isnull(sum(CYTB),0.0)))/ABS(isnull(sum(CYTB),1.0))) *100 END,2,MAX(TypeOfData))) CYTBPercentColor,
		--(Select TOP(1)ISNULL(ColorCode,'1') From DashboardParameters WHERE CASE WHEN sum(CYTB) = 0.0000 THEN 0.0000 ELSE ((isnull(sum(CYTA),0.0) - (isnull(sum(CYTB),0.0)))/ABS(isnull(sum(CYTB),1.0))) END
		--Between  Cast(ISNULL(RangeTo,0) as Numeric(18,4)) 
		--AND Cast(ISNULL(RangeFrom,0) as Numeric(18,4)) 
		--and ValueType =2 and IndicatorCategory=Max(TypeOfData))  CYTBPercentColor,

		(Select [dbo].[GetColorCodeByVarRange] ((isnull(sum(CYTA),0.0) - (isnull(sum(PYTA),0.0))),1,MAX(TypeOfData))) PYTAColor,
		--(Select TOP(1)ISNULL(ColorCode,'1') From DashboardParameters WHERE (isnull(sum(CYTA),0.0) - (isnull(sum(PYTA),0.0))) Between  Cast(ISNULL(RangeTo,0) as Numeric(18,4)) 
		--AND Cast(ISNULL(RangeFrom,0) as Numeric(18,4)) 
		--and ValueType =1 and IndicatorCategory=Max(TypeOfData))  PYTAColor,

		(Select [dbo].[GetColorCodeByVarRange] (CASE WHEN sum(PYTA) = 0.0000 THEN 0.0000 ELSE ((isnull(sum(CYTA),0.0) - (isnull(sum(PYTA),0.0)))/ABS(isnull(sum(PYTA),1.0))) *100 END,2,MAX(TypeOfData))) PYTAPercentColor
		--(Select TOP(1)ISNULL(ColorCode,'1') From DashboardParameters WHERE CASE WHEN sum(PYTA) = 0.0000 THEN 0.0000 ELSE ((isnull(sum(CYTA),0.0) - (isnull(sum(PYTA),0.0)))/ABS(isnull(sum(PYTA),1.0))) END
		--Between  Cast(ISNULL(RangeTo,0) as Numeric(18,4)) 
		--AND Cast(ISNULL(RangeFrom,0) as Numeric(18,4)) 
		--and ValueType =2 and IndicatorCategory=Max(TypeOfData))  PYTAPercentColor

from @FormatResults group by INum,DashBoard,Subcategory1 ,Subcategory2 order by INum


END --- Procedure Ends












GO


