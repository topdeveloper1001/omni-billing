IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetDBBudgetActualManualBB')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetDBBudgetActualManualBB

/****** Object:  StoredProcedure [dbo].[SPROC_GetDBBudgetActualManualBB]    Script Date: 3/22/2018 7:24:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SPROC_GetDBBudgetActualManualBB]
(  
	@CID int = 12,  --- CorporateID
	@FID int =17,  --- FacilityID
	@TillDate date = '2015-06-06', --- Pass in the current date till which date you want data to be displayed
	@FacilityRelated nvarchar(40)=null, ---- FacilityType
	@FacilityRegion nvarchar(10) =null  ----- regionID
)
AS
BEGIN
Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@FID))
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
--IF((Select Count(FID) from @SelT) > 1)
--	Begin
--		--- Delete Old data for the Facility and Corporate
--		Delete From [DashboardIndicatorData] Where FacilityId=9999 and CorporateId=@CID

--		--- insert data with sum for the Selected Facilities
--		Insert into [DashboardIndicatorData] 
--		Select 1,DID.IndicatorNumber,DID.SubCategory1,DID.SubCategory2,SUM(CAST(DID.StatisticData as DECIMAL(18,4))), DID.[Month],DID.[Year],9999,@CID,1,@CurrentDate,DID.ExternalValue1 
--			,DID.ExternalValue2,DID.ExternalValue3,1,Null 	from  [dbo].[DashboardIndicatorData] DID
--			Where DID.CorporateId = @CID and FacilityId in (Select FID from @SelT) GROUP BY DID.IndicatorNumber,DID.SubCategory1,DID.SubCategory2,DID.[Month],
--			DID.[Year],DID.ExternalValue1,DID.ExternalValue2,DID.ExternalValue3;

		
--		--- Get the Calculations first with Following PROC
--		--Exec [dbo].[SPROC_SetDBCalculatedIndicators]  @CID,9999,@TillDate
		
--		------ JUGAAD need to be fixed with proper sequence column ------XXXX
--		------ NOTE: Called SECOND TIME for a purpose - Due to missing Sequence and some calculations are based on others - So in first round it will fix and this call will correctly do rest calulations
--		----- Get the Calculations first with Following PROC

--		--Exec [dbo].[SPROC_SetDBCalculatedIndicators]  @CID,9999,@TillDate
--		---- NOTE: Called SECOND TIME for a purpose - Due to missing Sequence and some calculations are based on others - So in first round it will fix and this call will correctly do rest calulations
--		---- JUGAAD need to be fixed with proper sequence column ------XXXX
--		--Declare CustomCursor Cursor for Select FID from @SelT
		
--		--Open CustomCursor;
		
--		--Fetch Next from CustomCursor into @CUR_FacID;
--		--While @@FETCH_STATUS = 0
--		--Begin
--		--	Exec [dbo].[SPROC_SetDBCalculatedIndicators]  @CID,@CUR_FacID,@TillDate
--		-- Fetch Next from CustomCursor into @CUR_FacID;
--		--End

--		--- Delete From the @SelT
--		Delete From @SelT
--		-- Add Custom Value in the @SelT for the Below New Facility
--		Insert into @SelT (FID,DepartmentNumber) Values (9999,'')
--	END
----ELSE
----BEGIN
----	--- Get the Calculations first with Following PROC
----	--Exec [dbo].[SPROC_SetDBCalculatedIndicators]  @CID,@FID,@TillDate,'144,103,109,156,110,120,122,277,247'
	
----	------ JUGAAD need to be fixed with proper sequence column ------XXXX
----	------ NOTE: Called SECOND TIME for a purpose - Due to missing Sequence and some calculations are based on others - So in first round it will fix and this call will correctly do rest calulations
----	----- Get the Calculations first with Following PROC
----	--Exec [dbo].[SPROC_SetDBCalculatedIndicators]  @CID,@FID,@TillDate,'144,103,109,156,110,120,122,277,247'
----	---- NOTE: Called SECOND TIME for a purpose - Due to missing Sequence and some calculations are based on others - So in first round it will fix and this call will correctly do rest calulations
----	---- JUGAAD need to be fixed with proper sequence column ------XXXX
----END

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
 --and DID.IndicatorNumber in  ('144','103','109','156','110','120','122','277','247');
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
Set @N1 = (DATEDIFF(d, DATEADD(yy, DATEDIFF(yy,0,@CurrentDate), 0), DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0,@CurrentDate) + 1, 1))));

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

---- Returning Resullt in desired format

--Select * from @FormatResults
---- Updating DashBoard Name, FormatType and ExternalValue2 (Taken out from Inner join as it was giving issues in SUM) --- STARTS
---- NOTE: Commented inner joins in first queries, getting simple data as it is - ALSO changed IndicatorNumber column to INum due to below query update for Temp Table
	Update @FormatResults Set DashBoard =  DI.DashBoard, FormatType = DI.FormatType, EV2 = DI.ExternalValue2, [OwnerShip] = DI.[OwnerShip],TypeOfData =  DI.ExternalValue3
		from [dbo].[DashboardIndicators] DI 
		Where DI.IndicatorNumber = INum and DI.CorporateId = @CID 
		and (DI.FacilityId in (Select FID from @SelT) OR DI.FacilityId = 0)
---- Updating DashBoard Name, FormatType and ExternalValue2 (Taken out from Inner join as it was giving issues in SUM) --- ENDS

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


