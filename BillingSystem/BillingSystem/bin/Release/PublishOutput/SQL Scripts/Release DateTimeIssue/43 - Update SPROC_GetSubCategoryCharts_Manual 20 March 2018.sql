IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetSubCategoryCharts_Manual')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetSubCategoryCharts_Manual
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetSubCategoryCharts_Manual]    Script Date: 21-03-2018 10:31:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetSubCategoryCharts_Manual]
(
	@pFacilityId int =17,
	@pCorporateId int =12, 
	@pIndicatorNumber nvarchar(50) ='159',
	@pFiscalYear nvarchar(50)='2015',
	@pfacilityType int =0,
	@psegment int =0,
	@pDepartment int =0
)
AS
BEGIN
Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))
/*Select  dbo.GetSubCategory1Name(SubCategory1,SubCategory2) Name, * From ManualDashboard Where 
Indicators = @pIndicatorNumber ANd FacilityId = @pFacilityId And CorporateId = @pCorporateId And BudgetType = 2 And [Year] = @pFiscalYear And ExternalValue3 = 1*/
	--- Delcare Cursur Variables
Declare @CUR_CategoryName nvarchar(50),@CUR_IndicatorId int,@CUR_IndicatorNumber nvarchar(50),@CUR_SubCategory1 nvarchar(50),@CUR_SubCategory2 nvarchar(50),@CUR_FacilityId int
,@CUR_CorporateId int,@CUR_CreatedBy int,@CUR_ExternalValue1 nvarchar(50),@CUR_ExternalValue2 nvarchar(50),@CUR_ExternalValue3 nvarchar(50),@CUR_IsActive bit
,@CUR_DepartmentNumber nvarchar(20),@CUR_M1 nvarchar(50),@CUR_M2  nvarchar(50),@CUR_M3  nvarchar(50),@CUR_M4  nvarchar(50),@CUR_M5  nvarchar(50),@CUR_M6  nvarchar(50)
,@CUR_M7  nvarchar(50),@CUR_M8 nvarchar(50),@CUR_M9 nvarchar(50),@CUR_M10 nvarchar(50),@CUR_M11 nvarchar(50),@CUR_M12 nvarchar(50),@CUR_Year int

Declare @TA1Value table(CategoryName nvarchar(50),ID int,IndicatorId int,IndicatorNumber nvarchar(50),SubCategory1 nvarchar(50),SubCategory2 nvarchar(50),StatisticData nvarchar(50),[Month] varchar(5)
,[Year] nvarchar(5),FacilityId int,CorporateId int,CreatedBy int,CreatedDate Datetime,ExternalValue1 nvarchar(50),ExternalValue2 nvarchar(50),ExternalValue3 nvarchar(50)
,IsActive bit,DepartmentNumber nvarchar(20))

-- Declare Local Table To retrun
Declare @TA1 table(Name nvarchar(50),BudgetType int,DashboardType int,KPICategory int,Indicators int,SubCategory1 nvarchar(50),SubCategory2 nvarchar(50),
			Frequency int,Defination nvarchar(50),DataType int,CompanyTotal int,OwnerShip nvarchar(50),[Year] INT
           ,M1 nvarchar(50),M2 nvarchar(50),M3 nvarchar(50),M4 nvarchar(50),M5 nvarchar(50),M6 nvarchar(50),M7 nvarchar(50),M8 nvarchar(50),M9 nvarchar(50)
		   ,M10 nvarchar(50),M11 nvarchar(50),M12 nvarchar(50)
           ,OtherDescription nvarchar(500),FacilityId int,CorporateId int,CreatedBy int,CreatedDate Datetime,ExternalValue1  nvarchar(50),ExternalValue2  nvarchar(50),
		   ExternalValue3  nvarchar(50),IsActive bit)

--- Custom table to add data for months
Declare @TA1Custom table(CategoryName nvarchar(50),ID int,IndicatorId int,IndicatorNumber nvarchar(50),SubCategory1 nvarchar(50),SubCategory2 nvarchar(50),[Year] nvarchar(5),FacilityId int
,CorporateId int,CreatedBy int,CreatedDate Datetime,ExternalValue1 nvarchar(50),ExternalValue2 nvarchar(50),ExternalValue3 nvarchar(50),IsActive bit
,DepartmentNumber nvarchar(20),M1 nvarchar(50),M2  nvarchar(50),M3  nvarchar(50),M4  nvarchar(50),M5  nvarchar(50),M6  nvarchar(50),M7  nvarchar(50),M8 nvarchar(50),M9 nvarchar(50),M10 nvarchar(50),M11 nvarchar(50),M12 nvarchar(50))

--- To get Distinct values from the Custom table
Declare @TA1CustomDistinct table(CategoryName nvarchar(50),IndicatorId int,IndicatorNumber nvarchar(50),SubCategory1 nvarchar(50),SubCategory2 nvarchar(50),FacilityId int
,CorporateId int,CreatedBy int,ExternalValue1 nvarchar(50),ExternalValue2 nvarchar(50),ExternalValue3 nvarchar(50),IsActive bit
,DepartmentNumber nvarchar(20),M1 nvarchar(50),M2  nvarchar(50),M3  nvarchar(50),M4  nvarchar(50),M5  nvarchar(50),M6  nvarchar(50),M7 nvarchar(50),M8 nvarchar(50),M9 nvarchar(50),M10 nvarchar(50)
,M11 nvarchar(50),M12 nvarchar(50),[Year] nvarchar(5))

Declare @DashboardType int,@KPICategory int,@Frequency int,@Defination nvarchar(50),@DataType int,@OwnerShip nvarchar(50),@year int

DECLARE @CurrentYear int= Year(Cast(@pFiscalYear as Datetime)),@CurrentMonth int= Month(Cast(@pFiscalYear as Datetime));
Declare @priorFiscalYear int;
Set @priorFiscalYear = Year(Cast(@pFiscalYear as Datetime)) -1;

Declare @SelT table (FID int, DepartmentNumber nvarchar(20))

if @pFacilityId > 0 
	Begin
		Insert into @SelT Select FacilityId,'' from Facility Where FacilityId = @pFacilityId and CorporateID = @pCorporateId;
	End
ELSE
Begin
	If @pfacilityType <> 0  and @psegment  = 0
		Insert into @SelT Select FacilityId,'' from Facility Where FacilityRelated = @pfacilityType and CorporateID = @pCorporateId;
	If @psegment <> 0 and @pfacilityType = 0
		Insert into @SelT Select FacilityId,'' from Facility Where RegionId = @psegment and CorporateID = @pCorporateId;
	If @pfacilityType <> 0 and @psegment <> 0
		Insert into @SelT Select FacilityId,'' from Facility Where FacilityRelated = @pfacilityType and RegionId = @psegment and CorporateID = @pCorporateId;
	If @pfacilityType = 0 and @psegment = 0 and @pFacilityId = 0
		Insert into @SelT Select FacilityId,'' from Facility Where CorporateID = @pCorporateId;
End

--- Insert in to TA1Value
Insert Into @TA1Value
Select dbo.[GetSubCategoryName](DID.SubCategory1,DID.SubCategory2), DID.* from DashboardIndicatorData DID
			where  DID.IndicatorNumber = @pIndicatorNumber
			and FacilityId in (Select FID from @SelT)
			and  DID.ExternalValue1 in ('2') and  DID.Corporateid =@pCorporateId and DID.[Year] =@pFiscalYear 
--- Insert in to TA1Value

----- Insert in to TA1Value
--Insert Into @TA1Value
--Select  DID.* from DashboardIndicatorData DID
--			where  DID.IndicatorNumber = @pIndicatorNumber
--			and FacilityId in (Select FID from @SelT)
--			and  DID.ExternalValue1 in ('1','2') and  DID.Corporateid =@pCorporateId and DID.[Year] =@pFiscalYear 

--Insert Into @TA1Value
--Select  DID.* from DashboardIndicatorData DID
--			where  DID.IndicatorNumber = @pIndicatorNumber
--			and FacilityId in (Select FID from @SelT)
--			and  DID.ExternalValue1 in ('2') and  DID.Corporateid =@pCorporateId and DID.[Year] =@priorFiscalYear 

Update @TA1Value
Set [Month] = 'M'+[Month]


;With Report     
AS    
(   
select * from  
(
Select * from @TA1Value
) src    
pivot    
(    
MAX(StatisticData)    
for [Month] in (M1,M2,M3,M4,M5,M6,M7,M8,M9,M10,M11,M12)    
)piv   
)


--- Add values in custom table for Months wise data
insert into @TA1Custom select * from report;

insert into @TA1CustomDistinct
Select CategoryName,MAX(IndicatorId) ,IndicatorNumber ,SubCategory1 ,SubCategory2 ,MAX(FacilityId)
,CorporateId,MAX(CreatedBy) ,ExternalValue1 ,ExternalValue2 ,ExternalValue3 ,IsActive
,DepartmentNumber,ISNULL(Max(ISNULL(Cast(M1 as Numeric(18,2)),0)),0) ,ISNULL(SUM(ISNULL( Cast(M2 as Numeric(18,2)),0)),0)  
,ISNULL(SUM(ISNULL(Cast(M3 as numeric(18,2)),0)),0),ISNULL(SUM(ISNULL(CAST(M4 as Numeric(18,2)),0)),0) ,ISNULL(SUM(ISNULL(CAST(M5 as Numeric(18,2)),0)),0) ,
ISNULL(SUM(ISNULL(CAST(M6 as Numeric(18,2)),0)),0),ISNULL(SUM(ISNULL(CAST(M7 as Numeric(18,2)),0)),0) ,ISNULL(SUM(ISNULL(CAST(M8 as Numeric(18,2)),0)),0) ,
ISNULL(SUM(ISNULL(CAST(M9 as Numeric(18,2)),0)),0),ISNULL(SUM(ISNULL(CAST(M10 as Numeric(18,2)),0)),0) ,ISNULL(SUM(ISNULL(CAST(M11 as Numeric(18,2)),0)),0) ,
ISNULL(SUM(ISNULL(CAST(M12 as Numeric(18,2)),0)),0),[Year]
 from @TA1Custom 
 Group by IndicatorNumber ,SubCategory1 ,SubCategory2
,CorporateId, ExternalValue1 ,ExternalValue2 ,ExternalValue3 ,IsActive
,DepartmentNumber,[YEAR],CategoryName

DECLARE CountersData1 CURSOR FOR
			(Select * from @TA1CustomDistinct)

OPEN CountersData1;  

FETCH NEXT FROM  CountersData1 INTO  @CUR_CategoryName,@CUR_IndicatorId ,@CUR_IndicatorNumber ,@CUR_SubCategory1 ,@CUR_SubCategory2 ,@CUR_FacilityId 
,@CUR_CorporateId ,@CUR_CreatedBy ,@CUR_ExternalValue1 ,@CUR_ExternalValue2 ,@CUR_ExternalValue3 ,@CUR_IsActive
,@CUR_DepartmentNumber ,@CUR_M1 ,@CUR_M2  ,@CUR_M3  ,@CUR_M4  ,@CUR_M5  ,@CUR_M6  
,@CUR_M7  ,@CUR_M8 ,@CUR_M9 ,@CUR_M10 ,@CUR_M11 ,@CUR_M12,@CUR_Year;

WHILE @@FETCH_STATUS = 0  
		BEGIN 
		Select  @DashboardType = 1, @KPICategory=1,@Frequency=FerquencyType,@Defination = Defination,@DataType= FormatType,@OwnerShip = [OwnerShip]  
		From DashboardIndicators
		where IndicatorNumber =@pIndicatorNumber  and Corporateid =@pCorporateId


		Set @year =@CUR_Year;
		---Insert Command
		INSERT INTO @TA1(Name,BudgetType ,DashboardType ,KPICategory ,Indicators ,SubCategory1 ,SubCategory2 ,
			Frequency ,Defination ,DataType ,CompanyTotal ,[OwnerShip] ,[Year] 
           ,M1 ,M2 ,M3 ,M4 ,M5 ,M6 ,M7 ,M8 ,M9 
		   ,M10 ,M11 ,M12 
           ,OtherDescription ,FacilityId ,CorporateId ,CreatedBy ,CreatedDate,ExternalValue1  ,ExternalValue2  ,
		   ExternalValue3  ,IsActive )
		 VALUES
		       (@CUR_CategoryName,Cast(@CUR_ExternalValue1 as int),@DashboardType,@KPICategory,@CUR_IndicatorId,@CUR_SubCategory1,@CUR_SubCategory2,
			   @Frequency,@Defination,@DataType,1,@OwnerShip,@year,@CUR_M1,@CUR_M2,@CUR_M3,@CUR_M4,@CUR_M5,@CUR_M6,@CUR_M7,@CUR_M8,@CUR_M9,@CUR_M10,@CUR_M11,@CUR_M12,null,
			   @CUR_FacilityId,@CUR_CorporateId,@CUR_CreatedBy,@LocalDateTime,@CUR_DepartmentNumber,null,null,@CUR_IsActive);

FETCH NEXT FROM  CountersData1 INTO  @CUR_CategoryName,@CUR_IndicatorId ,@CUR_IndicatorNumber ,@CUR_SubCategory1 ,@CUR_SubCategory2 ,@CUR_FacilityId 
,@CUR_CorporateId ,@CUR_CreatedBy ,@CUR_ExternalValue1 ,@CUR_ExternalValue2 ,@CUR_ExternalValue3 ,@CUR_IsActive
,@CUR_DepartmentNumber ,@CUR_M1 ,@CUR_M2  ,@CUR_M3  ,@CUR_M4  ,@CUR_M5  ,@CUR_M6  
,@CUR_M7  ,@CUR_M8 ,@CUR_M9 ,@CUR_M10 ,@CUR_M11 ,@CUR_M12,@CUR_Year;

END  --END OF @@FETCH_STATUS = 0  
CLOSE CountersData1;  
DEALLOCATE CountersData1; 


Select * from @TA1 order by [Year]
END













GO


