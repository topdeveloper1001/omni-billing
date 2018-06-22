IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetDBBudgetActualManual_Acute')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetDBBudgetActualManual_Acute

/****** Object:  StoredProcedure [dbo].[SPROC_GetDBBudgetActualManual_Acute]    Script Date: 3/22/2018 7:13:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SPROC_GetDBBudgetActualManual_Acute]
(  
	@CID int = 12,  --- CorporateID
	@FID int = 17,  --- FacilityID
	@TillDate date = '2015-01-06', --- Pass in the current date till which date you want data to be displayed
	@FacilityRelated nvarchar(40)=null, ---- FacilityType
	@FacilityRegion nvarchar(10) =null  ----- regionID
)
AS
BEGIN
Declare @LocalDateTime datetime  = (Select dbo.GetCurrentDatetimeByEntity(@FID))


Declare @SelT table (FID int, DepartmentNumber nvarchar(20))

Declare @Month int, @FiscalYear int, @strStartDate nvarchar(20), @StartDate date, @DenominatorYTD numeric(18,4)
Declare @N1 numeric(18,2)
Declare @N2 numeric(18,2)

Declare @Results table (CID int, FID int, IndicatorNumber nvarchar(50), DashBoard nvarchar(500), ResultType nvarchar(50),Result decimal(18,4), Tag nvarchar(10), FormatType int, EV2 nvarchar(50))

Declare @FormatResults table (CID int, FID int, INum nvarchar(50), DashBoard nvarchar(500), ResultType nvarchar(50), FormatType int, EV2 nvarchar(50),
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


---- Get Selection Information - ENDS

--- Get the Calculations first with Following PROC
Exec [dbo].[SPROC_SetDBCalculatedIndicators_Acute]  @CID,@FID,@TillDate

---- JUGAAD need to be fixed with proper sequence column ------XXXX
---- NOTE: Called SECOND TIME for a purpose - Due to missing Sequence and some calculations are based on others - So in first round it will fix and this call will correctly do rest calulations
--- Get the Calculations first with Following PROC
Exec [dbo].[SPROC_SetDBCalculatedIndicators_Acute]  @CID,@FID,@TillDate
---- NOTE: Called SECOND TIME for a purpose - Due to missing Sequence and some calculations are based on others - So in first round it will fix and this call will correctly do rest calulations
---- JUGAAD need to be fixed with proper sequence column ------XXXX


Set @Month = month(@TillDate)
Set @FiscalYear = year(@TillDate)
Set @N1 = (datepart(dd,@TillDate))
Set @N2 = (datepart(dd,EOMONTH(@TillDate)))
Set @DenominatorYTD = (@Month - 1)+ (@N1/@N2)


---- Current YEAR - Starts
Set @strStartDate = cast(@FiscalYear as nvarchar(4)) + '-01-01'
Set @StartDate = @strStartDate

----- max(DI.Dashboard) DashBoard, --- max(DI.FormatType), MAX(CAST(DI.ExternalValue2 as int)) 
---- Year to Date
insert into @Results
Select DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,'',DID.ExternalValue1 ResultType, cast(DID.StatisticData as decimal(18,4)) Result,
Case When DID.ExternalValue1 = 1 Then 'CYTB' Else 'CYTA' END Tag,0,'' from [dbo].[DashboardIndicatorData] DID
-- inner join [dbo].[DashboardIndicators] DI on DI.IndicatorNumber = DID.IndicatorNumber and DI.CorporateId = @CID and DI.FacilityId  in (Select FID from @SelT)
Where DID.CorporateId = @CID and DID.FacilityId in (Select FID from @SelT) and DID.[Year] =@FiscalYear and DID.[Month] <= @Month
--Group by DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,DID.ExternalValue1

---- Monthly
insert into @Results
Select DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,'',DID.ExternalValue1 ResultType, cast(DID.StatisticData as decimal(18,4)) Result,
Case When DID.ExternalValue1 = 1 Then 'CMB' Else 'CMA' END Tag,0,'' from [dbo].[DashboardIndicatorData] DID
-- inner join [dbo].[DashboardIndicators] DI on DI.IndicatorNumber = DID.IndicatorNumber and DI.CorporateId = @CID and DI.FacilityId  in (Select FID from @SelT)
Where DID.CorporateId = @CID and DID.FacilityId in (Select FID from @SelT) and DID.[Year] =@FiscalYear and DID.[Month] = @Month
-- Group by DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,DID.ExternalValue1

---- Current YEAR - Ends


---- Previous YEAR - Starts

Set @StartDate = DATEADD(YEAR,-1,@Startdate)
Set @TillDate = DATEADD(YEAR,-1,@TillDate)
Set @FiscalYear = year(@TillDate)

---- Year to Date
insert into @Results
Select DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,'',DID.ExternalValue1 ResultType, cast(DID.StatisticData as decimal(18,4)) Result,
Case When DID.ExternalValue1 = 1 Then 'PYTB' Else 'PYTA' END Tag,0,'' from [dbo].[DashboardIndicatorData] DID
-- inner join [dbo].[DashboardIndicators] DI on DI.IndicatorNumber = DID.IndicatorNumber and DI.CorporateId = @CID and DI.FacilityId  in (Select FID from @SelT)
Where DID.CorporateId = @CID and DID.FacilityId in (Select FID from @SelT) and DID.[Year] =@FiscalYear and DID.[Month] <= @Month
-- Group by DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,DID.ExternalValue1

---- Monthly
insert into @Results
Select DID.CorporateId, DID.FacilityId,DID.IndicatorNumber,'',DID.ExternalValue1 ResultType, cast(DID.StatisticData as decimal(18,4)) Result,
Case When DID.ExternalValue1 = 1 Then 'PMB' Else 'PMA' END Tag,0,'' from [dbo].[DashboardIndicatorData] DID
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

---- Updating DashBoard Name, FormatType and ExternalValue2 (Taken out from Inner join as it was giving issues in SUM) --- STARTS
---- NOTE: Commented inner joins in first queries, getting simple data as it is - ALSO changed IndicatorNumber column to INum due to below query update for Temp Table
	Update @FormatResults Set DashBoard =  DI.DashBoard, FormatType = DI.FormatType, EV2 = DI.ExternalValue2
		from [dbo].[DashboardIndicators] DI 
		Where DI.IndicatorNumber = INum and DI.CorporateId = @CID 
		and (DI.FacilityId in (Select FID from @SelT) OR DI.FacilityId = 0)
---- Updating DashBoard Name, FormatType and ExternalValue2 (Taken out from Inner join as it was giving issues in SUM) --- ENDS

	--Print 'Section1'
--->>> Special case where no sum/Average it has to be Number as it is of present Month (eg:Total Operating Beds)
	Update @FormatResults Set CYTA = CMA, CYTB = CMB, PYTA = PMA, PYTB = PMB Where INum = '108'

	Declare @A1 numeric(18,4)=0, @A2 numeric(18,4)=0, @A3 numeric(18,4)=0
	Declare @M1 numeric(18,4)=0, @M2 numeric(18,4)=0, @M3 numeric(18,4)=0

	Select @A1 = CYTB From @FormatResults Where INum = '103' And ResultType = 1
	Select @A2 = CYTB From @FormatResults Where INum = '101' And ResultType = 1

	Set @A2 = ISNULL(@A2,0.0000)

	If @A2 = 0.0000 or @A2= 0
		Set @A2 = 1
	-->>> Special Case for calculating the Avrage Length of Stay.
	Update @FormatResults Set CYTB = (@A1 / @A2) Where INum = '106' And ResultType = 1

	Select @A1 = CYTA From @FormatResults Where INum = '103' And ResultType = 2
	Select @A2 = CYTA From @FormatResults Where INum = '101' And ResultType = 2
	--Print 'Section2'
	Set @A2 = ISNULL(@A2,0.0000)

	If @A2 = 0.0000 or @A2= 0
		Set @A2 = 1
	-->>> Special Case for calculating the Avrage Length of Stay.
	Update @FormatResults Set CYTA = (@A1 / @A2) Where INum = '106' And ResultType = 2

	Select @A1 = PYTA From @FormatResults Where INum = '103' And ResultType = 2
	Select @A2 = PYTA From @FormatResults Where INum = '101' And ResultType = 2

	Set @A2 = ISNULL(@A2,0.0000)

	If @A2 = 0.0000 or @A2= 0
		Set @A2 = 1
	-->>> Special Case for calculating the Avrage Length of Stay.
	Update @FormatResults Set PYTA = (@A1 / @A2) Where INum = '106' And ResultType = 2


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

	Set @M1 = isnull(@M1,'1')
	If @M1 = '0'
		Set @M1 = '1'

	Set @M2 = isnull(@M2,'1')
			If @M2 = '0'
				Set @M2 = '1'

	Set @M3 = isnull(@M3,'0')

	Set @A1 = isnull(@A1,'1')
	If @A1 = '0'
		Set @A1 = '1'

	Set @A2 = isnull(@A2,'1')
			If @A2 = '0'
				Set @A2 = '1'

	Set @A3 = isnull(@A3,'0')

    Update @FormatResults Set CYTA = @A1 + (@A3 / (@A2 / @A1)), CMA = @M1 + (@M3 / (@M2 / @M1)) Where INum = '105' And ResultType = 2

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

	Set @M1 = isnull(@M1,'1')
				If @M1 = '0'
					Set @M1 = '1'

	Set @M2 = isnull(@M2,'1')
			If @M2 = '0'
				Set @M2 = '1'

	Set @M3 = isnull(@M3,'0')

	Set @A1 = isnull(@A1,'1')
	If @A1 = '0'
		Set @A1 = '1'

	Set @A2 = isnull(@A2,'1')
			If @A2 = '0'
				Set @A2 = '1'

	Set @A3 = isnull(@A3,'0')

    Update @FormatResults Set CYTB = @A1 + (@A3 / (@A2 / @A1)), CMB = @M1 + (@M3 / (@M2 / @M1))  Where INum = '105' And ResultType = 1

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

	Set @M1 = isnull(@M1,'1')
				If @M1 = '0'
					Set @M1 = '1'

	Set @M2 = isnull(@M2,'1')
			If @M2 = '0'
				Set @M2 = '1'

	Set @M3 = isnull(@M3,'0')

	Set @A1 = isnull(@A1,'1')
	If @A1 = '0'
		Set @A1 = '1'

	Set @A2 = isnull(@A2,'1')
			If @A2 = '0'
				Set @A2 = '1'

	Set @A3 = isnull(@A3,'0')

    Update @FormatResults Set PYTA = @A1 + (@A3 / (@A2 / @A1)), PMA = @M1 + (@M3 / (@M2 / @M1))  Where INum = '105' And ResultType = 2
	----> Special Case for Adjusted Patietn Days (i.e. Indicator Number '105') Ends Here

	--Print 'Section6'
	------ New MARGIN CALCULATIONS - STARTS  ---- XXXXXXXXXXXXXXXXXXXXx

	----> Special Case for SWB%/Net Revnue (i.e. Indicator Number '120') STARTS
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	--> THis is for Actuals
	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '111' And ResultType = 2			---- Get SWB Direct
	Select @A3 = CYTA, @M3 = CMA From @FormatResults Where INum = '155' And ResultType = 2			---- Get SWB Indirect Costs
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	
    Update @FormatResults Set CYTA =  (((isnull(@A3,0) +isnull(@A2,0)) / isnull(@A1,1)) *100), CMA = (((isnull(@M3,0) +isnull(@M2,0)) / isnull(@M1,1))*100) Where INum = '120' And ResultType = 2
	--Print 'Section7'

	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTB, @M1 = CMB From @FormatResults Where INum = '110' And ResultType = 1			---- Get Net Revenue
	Select @A2 = CYTB, @M2 = CMB From @FormatResults Where INum = '111' And ResultType = 1			---- Get SWB Direct
	Select @A3 = CYTB, @M3 = CMB From @FormatResults Where INum = '155' And ResultType = 1			---- Get SWB Indirect Costs
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	 Update @FormatResults Set CYTB =  (((isnull(@A3,0) +isnull(@A2,0)) / isnull(@A1,1))*100), CMB = (((isnull(@M3,0) +isnull(@M2,0)) / isnull(@M1,1))*100) Where INum = '120' And ResultType = 1

	--> This is for Prior Year
	--Print 'Section8'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = PYTA, @M1 = PMA  From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = PYTA, @M2 = PMA From @FormatResults Where INum = '111' And ResultType = 2			---- Get SWB Direct
	Select @A3 = PYTA, @M3 = PMA From @FormatResults Where INum = '155' And ResultType = 2			---- Get SWB Indirect Costs

	--Select @A1,@A2,@A3,@M1,@M2,@M3
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Update @FormatResults Set PYTA =  (((isnull(@A3,0) +isnull(@A2,0)) / isnull(@A1,1))*100), PMA = (((isnull(@M3,0) +isnull(@M2,0)) / isnull(@M1,1))*100) Where INum = '120' And ResultType = 2

	----> Special Case for SWB%/Net Revnue (i.e. Indicator Number '120') ---- XXXXX ENDS

	----- Next Margin is
	
	----> Special Case for SWB INDIRECT% / Net Revnue (i.e. Indicator Number '121') STARTS
	--> THis is for Actuals
	--Print 'Section9'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '115' And ResultType = 2			---- Get SWB Indirect Costs
	Select @A3 = CYTA, @M3 = CMA From @FormatResults Where INum = '155' And ResultType = 2			---- Get Other Indirect Costs
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Update @FormatResults Set CYTA =  ((isnull(@A2,0) + ISNULL(@A3,0)) / isnull(@A1,1) * 100), CMA = ((isnull(@M2,0) + isnull(@M3,0)) / isnull(@M1,1)*100) Where INum = '121' And ResultType = 2


	--> This is for Budgets
	--Print 'Section10'
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTB, @M1 = CMB From @FormatResults Where INum = '110' And ResultType = 1			---- Get Net Revenue
	Select @A2 = CYTB, @M2 = CMB From @FormatResults Where INum = '155' And ResultType = 1			---- Get SWB Indirect Costs
	Select @A3 = CYTB, @M3 = CMB From @FormatResults Where INum = '115' And ResultType = 1			---- Get Other Indirect Costs
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	 Update @FormatResults Set CYTB =  ((isnull(@A2,0) + ISNULL(@A3,0)) / isnull(@A1,1) * 100), CMB = ((isnull(@M2,0) + isnull(@M3,0)) / isnull(@M1,1)*100) Where INum = '121' And ResultType = 1

	--> This is for Prior Year

	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = PYTA, @M1 = PMA  From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = PYTA, @M2 = PMA From @FormatResults Where INum = '155' And ResultType = 2			---- Get SWB Indirect Costs
	Select @A3 = PYTA, @M3 = PMA From @FormatResults Where INum = '115' And ResultType = 2			---- Get Other Indirect Costs
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Update @FormatResults Set PYTA =  ((isnull(@A2,0) + ISNULL(@A3,0)) / isnull(@A1,1) * 100), PMA = ((isnull(@M2,0) + isnull(@M3,0)) / isnull(@M1,1)*100) Where INum = '121' And ResultType = 2

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

	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Update @FormatResults Set CYTA =  ((isnull(@A2,0)) / isnull(@A1,1)*100), CMA = ((isnull(@M2,0)) / isnull(@M1,1)*100) Where INum = '122' And ResultType = 2

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
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	 Update @FormatResults Set CYTB =  ((isnull(@A2,0)) / isnull(@A1,1)*100), CMB = ((isnull(@M2,0)) / isnull(@M1,1)*100) Where INum = '122' And ResultType = 1

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
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Update @FormatResults Set PYTA =  ((isnull(@A2,0)) / isnull(@A1,1)*100), PMA = ((isnull(@M2,0) / isnull(@M1,1))*100) Where INum = '122' And ResultType = 2

	----> Special Case for EBIDTA %/Net Revnue (i.e. Indicator Number '122') ---- XXXXX ENDS

	--Print 'Section14'
	----> Special Case for (NET Income Margin) Net Income/Loss  /Net Revnue (i.e. Indicator Number '123') STARTS
	--> THis is for Actuals
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTA, @M1 = CMA From @FormatResults Where INum = '110' And ResultType = 2			---- Get Net Revenue
	Select @A2 = CYTA, @M2 = CMA From @FormatResults Where INum = '119' And ResultType = 2			---- Get Income/Loss
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Update @FormatResults Set CYTA =  ((isnull(@A2,0)) / isnull(@A1,1)*100), CMA = ((isnull(@M2,0)) / isnull(@M1,1)*100) Where INum = '162' And ResultType = 2

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
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	 Update @FormatResults Set CYTB =  ((isnull(@A2,0)) / isnull(@A1,1)*100), CMB = ((isnull(@M2,0)) / isnull(@M1,1)*100) Where INum = '162' And ResultType = 1
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
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Update @FormatResults Set PYTA =  ((isnull(@A2,0)) / isnull(@A1,1)*100), PMA = ((isnull(@M2,0) / isnull(@M1,1))*100) Where INum = '162' And ResultType = 2
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
	Update @FormatResults Set CYTA =  ((isnull(@A2,0)) / isnull(@A1,1)*100), CMA = ((isnull(@M2,0)) / isnull(@M1,1)*100) Where INum = '145' And ResultType = 2

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
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	 Update @FormatResults Set CYTB =  ((isnull(@A2,0)) / isnull(@A1,1)*100), CMB = ((isnull(@M2,0)) / isnull(@M1,1)*100) Where INum = '145' And ResultType = 1
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
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Update @FormatResults Set PYTA =  ((isnull(@A2,0)) / isnull(@A1,1)*100), PMA = ((isnull(@M2,0) / isnull(@M1,1))*100) Where INum = '145' And ResultType = 2

	----> Special Case for (Operating Margin) (Net Operating /Net Revenue)*100 (i.e. Indicator Number '145') - XXXX ENDS
	--Print 'Section20'

	----> Special Case for (Average Daily Census) (i.e. Indicator Number '144') STARTS
	----> THis is for Actuals and Prior Year
	--Set @A1 = 0
	--Set @A2 = 0
	--Set @N1 = 0

	--Set @N1 = (datepart(dd,EOMONTH(@TillDate)))

	--Select @A1 = CMA, @A2 = PMA From @FormatResults Where INum = '103' And ResultType = 2		---- Get In Patient Days
	--Update @FormatResults Set CMA =  ISNULL(@A1,0) / @N1, PMA = ISNULL(@A2,0) / @N1 Where INum = '144' And ResultType = 2

	----Print 'Section21'
	----> This is for Budgets
	--Set @A1 = 0
	--Set @A2 = 0
	--Select @A1 = CMB From @FormatResults Where INum = '103' And ResultType = 1	    ---- Get In Patient Days
	--Update @FormatResults Set CMB =  ISNULL(@A1,0) / @N1 Where INum = '144' And ResultType = 1

	----> Special Case for (Average Daily Census) (i.e. Indicator Number '144') - XXXX ENDS

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

	Set @N1 = (DATEDIFF(d, DATEADD(yy, DATEDIFF(yy,0,@LocalDateTime), 0), DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0,@LocalDateTime) + 1, 1))))

	Select @A1 = CYTA, @A2 = PYTA From @FormatResults Where INum = '103' And ResultType = 2		---- Get In Patient Days
	Select @A3 = CYTA, @M1 = PYTA From @FormatResults Where INum = '108' And ResultType = 2		---- Get Total Operating Beds

	if(@A3 = 0.0000)
		SET @A3 =1
	if(@M1 = 0.0000)
		SET @M1 =1
	Set @M2 = (Cast(@A1 as numeric(18,4))/(Cast(@A3 as numeric(18,4)) * @N1))*100
	Set @M3 = (Cast(@A2 as numeric(18,4))/(Cast(@M1 as numeric(18,4)) * @N1))*100
	
	Update @FormatResults Set CYTA =  @M2, PYTA = @M3 Where INum = '109' And ResultType = 2


	--> This is for Budgets
	Set @A1 = 0
	Set @A2 = 0
	Set @A3 = 0
	Set @M1 = 0
	Set @M2 = 0
	Set @M3 = 0

	Select @A1 = CYTB From @FormatResults Where INum = '103' And ResultType = 1		---- Get In Patient Days
	Select @A3 = CYTB From @FormatResults Where INum = '108' And ResultType = 1		---- Get Total Operating Beds
	if(@A1 = 0.0000)
		SET @A1 =1
	if(@A3 = 0.0000)
		SET @A3 =1
	Set @M2 = (Cast(@A1 as numeric(18,4))/(Cast(@A3 as numeric(18,4)) * @N1))*100
	
	Update @FormatResults Set CYTB =  @M2 Where INum = '109' And ResultType = 1

	----> Special Case for (Occupany Rate) (i.e. Indicator Number '109') - XXXX ENDS

	------ New MARGIN CALCULATIONS - ENDS ---- XXXXXXXXXXXXXXXXXXXXx

	--print 'Before Select'
---- Returning Resullt in desired format

Select INum IndicatorNumber,isnull(DashBoard,'') DashBoard,MAX(FormatType) FormatType, MAX(ISNULL(CAST(EV2 as int), 0)) EV2,
		isnull(sum(CMB),0.0) CMB,
		isnull(sum(CMA),0.0) CMA,
		(isnull(sum(CMA),0.0) - isnull(sum(CMB),0.0)) CMVar,
		case WHEN isnull(sum(CMB),0.0) = 0.0 
		Then 0 		
		WHEN sum(CMB) = 0.0000
		THEN ((isnull(sum(CMA),0.0) - isnull(sum(CMB),0.0))/1)
		Else ((isnull(sum(CMA),0.0) - isnull(sum(CMB),0.0))/isnull(sum(CMB),1.0)) END CMVarPercentage ,
		isnull(sum(PMB),0.0) PMB,
		isnull(sum(PMA),0.0) PMA,
		(isnull(sum(CMA),0.0) - isnull(sum(PMA),0.0)) PMVar,
		case WHEN isnull(sum(PMA),0.0) = 0.0 
		Then 0 		
		WHEN sum(PMA) = 0.0000
		THEN ((isnull(sum(PMA),0.0) - isnull(sum(CMA),0.0))/1)
		Else		((isnull(sum(CMA),0.0) - isnull(sum(PMA),0.0))/isnull(sum(PMA),1.0)) END PMVarPercentage,
		isnull(sum(CYTB),0.0) CYTB,
		isnull(sum(CYTA),0.0) CYTA,
		(isnull(sum(CYTA),0.0) - isnull(sum(CYTB),0.0)) CYTBVar,
		case WHEN isnull(sum(CYTB),0.0) = 0.0 
		Then 0 	
		WHEN sum(CYTB) = 0.0000
		THEN ((isnull(sum(CYTA),0.0) - isnull(sum(CYTB),0.0))/1)
		Else		((isnull(sum(CYTA),0.0) - isnull(sum(CYTB),0.0))/isnull(sum(CYTB),1.0)) END CYTBVarPercentage,
		isnull(sum(PYTB),0.0) PYTB,
		isnull(sum(PYTA),0.0) PYTA,
		(isnull(sum(CYTA),0.0) - isnull(sum(PYTA),0.0)) PYTBVar,
		case WHEN isnull(sum(PYTA),0.0) = 0.0 
		Then 0 		
		WHEN sum(PYTA) = 0.0000
		THEN ((isnull(sum(PYTA),0.0) - isnull(sum(CYTA),0.0))/1)
		Else		((isnull(sum(CYTA),0.0) - isnull(sum(PYTA),0.0))/isnull(sum(PYTA),1.0)) END PYTBVarPercentage
from @FormatResults group by INum,DashBoard order by INum


END --- Procedure Ends












GO


