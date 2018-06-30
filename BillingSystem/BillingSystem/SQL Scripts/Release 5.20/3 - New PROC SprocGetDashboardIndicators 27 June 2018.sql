IF OBJECT_ID('SprocGetDashboardIndicators','P') IS NOT NULL
	DROP PROCEDURE SprocGetDashboardIndicators
GO

/****** Object:  StoredProcedure [dbo].[SPROC_SortDashboardIndicatorCols]    Script Date: 6/27/2018 2:46:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Exec SprocGetDashboardIndicators 'IndicatorNumber','ASC',1016,0,1
CREATE Procedure [dbo].[SprocGetDashboardIndicators]
(
	@pSortBy nvarchar(50),
	@pSortDirection nvarchar(50),
	@pCID nvarchar(50),
	@pFID nvarchar(10),
	@pStatus bit=1
)
As
Begin
	Declare @FName nvarchar(100)=(Select TOP 1 FacilityName From Facility Where FacilityId=@pFID)
	Declare @KpiSubCategories1 nvarchar(5)='4347',@KpiSubCategories2 nvarchar(5)='4351'

	DECLARE @DashBoardTempTable table (
	[IndicatorID] [int]  NULL,[IndicatorNumber] [nvarchar](50) NULL,[Dashboard] [nvarchar](500) NULL,[Description] [nvarchar](500) NULL,
	[Defination] [nvarchar](100) NULL,[SubCategoryFirst] [nvarchar](500) NULL,[SubCategorySecond] [nvarchar](500) NULL,[FormatTypeStr] [nvarchar](100) NULL,
	[DecimalNumbers] [nvarchar](10) NULL,[FerquencyTypeStr] [nvarchar](100) NULL,[OwnerShip] [nvarchar](50) NULL,[FacilityId] [int] NULL,
	[CorporateId] [int] NULL,[CreatedBy] [int] NULL,[CreatedDate] [datetime] NULL,[IsActive] [int] NULL,[ExternalValue1] [nvarchar](50) NULL,
	[ExternalValue2] [nvarchar](50) NULL,[ExternalValue3] [nvarchar](50) NULL,[ReferencedIndicators] [nvarchar](1000) NULL,[ExternalValue4] [nvarchar](50) NULL,
	[ExternalValue5] [nvarchar](50) NULL,[ExternalValue6] [nvarchar](50) NULL,[SortOrder] [int] NULL,[ExpressionText] [nvarchar](500) NULL,
	[ExpressionValue] [nvarchar](500) NULL,[SpecialCase] [int]  NULL,[TypeOFData] [nvarchar](100) NULL
	,FacilityNameStr nvarchar(100),UsernameStr nvarchar(100)
	)


	INSERT INTO @DashBoardTempTable
	select DI.IndicatorId, DI.IndicatorNumber, DI.Dashboard, DI.[Description],DI.Defination
	,SubCategoryFirst =
	(
		Case When ISNULL(DI.SubCategory1,'') !='' AND ISNULL(DI.SubCategory1,'0') !='0'
		THEN (Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeValue=DI.SubCategory1 And G.GlobalCodeCategoryValue=@KpiSubCategories1) 
		ELSE '' END
	)
	,SubCategorySecond =
	(
		Case When ISNULL(DI.SubCategory1,'') !='' AND IsNumeric(ISNULL(DI.SubCategory1,'a'))=1 AND ISNULL(DI.SubCategory2,'') !=''
		THEN 
		(
			Select TOP 1 G.GlobalCodeName From GlobalCodes G 
			Where G.GlobalCodeValue=DI.SubCategory2 And G.GlobalCodeCategoryValue=@KpiSubCategories2
		)
		ELSE '' END
	)
	,FormatTypeStr=(Select TOP 1 G1.GlobalCodeName From GlobalCodes G1 Where G1.GlobalCodeCategoryValue='4343' And G1.GlobalCodeValue=DI.FormatType)
	,DI.DecimalNumbers
	,FerquencyTypeStr=(Select TOP 1 G1.GlobalCodeName From GlobalCodes G1 Where G1.GlobalCodeCategoryValue='4344' And G1.GlobalCodeValue=DI.FerquencyType)
	,DI.[OwnerShip], DI.[FacilityId],DI.[CorporateId],DI.CreatedBy,DI.CreatedDate,DI.[IsActive],DI.[ExternalValue1],DI.[ExternalValue2]
	,DI.[ExternalValue3], DI.[ReferencedIndicators], DI.[ExternalValue4], DI.[ExternalValue5], DI.[ExternalValue6],DI.[SortOrder]
	,DI.[ExpressionText], DI.[ExpressionValue], DI.[SpecialCase]
	,TypeOFData=
	(
		Case WHEN ISNULL(DI.ExternalValue3,'') !='' THEN 
		(Select TOP 1 G1.GlobalCodeName From GlobalCodes G1 Where G1.GlobalCodeCategoryValue='4407' And G1.GlobalCodeValue=DI.ExternalValue3)
		ELSE '' 
		END
	)
	,FacilityNameStr=@FName
	,UsernameStr=(Select TOP 1 U.UserName From Users U Where U.UserID=DI.CreatedBy)
	FROM DashboardIndicators AS DI
	Where DI.CorporateId=@pCID
	And IsActive=@pStatus
	
  
	If(@pSortBy = 'IndicatorNumber')
	Begin
		select *
		,DashboardTypeStr=''
		From @DashBoardTempTable D Order by CASE 
		When @pSortDirection <> 'ASC' then 0
		When 1=1 THEN Cast(IndicatorNumber as INT)
		end ASC
		, case
		When @pSortDirection <> 'DESC' then 0
		When 1=1 THEN Cast(IndicatorNumber as INT)
		end DESC
		FOR JSON Path,Root('DashboardResult'),Include_Null_Values
	End
	Else
	Begin
		Select * From @DashBoardTempTable Order by
		CASE WHEN @pSortBy = 'Dashboard' AND @pSortDirection = 'ASC'
		THEN Description END ASC, 
		CASE WHEN @pSortBy = 'Dashboard' AND @pSortDirection = 'DESC' 
		THEN Dashboard END DESC,

		CASE WHEN @pSortBy = 'Description' AND @pSortDirection = 'ASC' 
		THEN Description END ASC, 
		CASE WHEN @pSortBy = 'Description' AND @pSortDirection = 'DESC' 
		THEN Description END DESC,

		CASE WHEN @pSortBy = 'Defination' AND @pSortDirection = 'ASC' 
		THEN Defination END ASC, 
		CASE WHEN @pSortBy = 'Defination' AND @pSortDirection = 'DESC' 
		THEN Defination END DESC,

		CASE WHEN @pSortBy = 'SubCategoryFirst' AND @pSortDirection = 'ASC' 
		THEN SubCategoryFirst END ASC, 
		CASE WHEN @pSortBy = 'SubCategoryFirst' AND @pSortDirection = 'DESC' 
		THEN SubCategoryFirst END DESC,

		CASE WHEN @pSortBy = 'SubCategorySecond' AND @pSortDirection = 'ASC' 
		THEN SubCategorySecond END ASC, 
		CASE WHEN @pSortBy = 'SubCategorySecond' AND @pSortDirection = 'DESC' 
		THEN SubCategorySecond END DESC,

		CASE WHEN @pSortBy = 'FormatTypeStr' AND @pSortDirection = 'ASC' 
		THEN FormatTypeStr END ASC, 
		CASE WHEN @pSortBy = 'FormatTypeStr' AND @pSortDirection = 'DESC' 
		THEN FormatTypeStr END DESC,

		CASE WHEN @pSortBy = 'DecimalNumbers' AND @pSortDirection = 'ASC' 
		THEN DecimalNumbers END ASC, 
		CASE WHEN @pSortBy = 'DecimalNumbers' AND @pSortDirection = 'DESC' 
		THEN DecimalNumbers END DESC,

		CASE WHEN @pSortBy = 'FerquencyTypeStr' AND @pSortDirection = 'ASC' 
		THEN DecimalNumbers END ASC, 
		CASE WHEN @pSortBy = 'FerquencyTypeStr' AND @pSortDirection = 'DESC' 
		THEN FerquencyTypeStr END DESC,

		CASE WHEN @pSortBy = 'OwnerShip' AND @pSortDirection = 'ASC' 
		THEN DecimalNumbers END ASC, 
		CASE WHEN @pSortBy = 'OwnerShip' AND @pSortDirection = 'DESC' 
		THEN OwnerShip END DESC,

		CASE WHEN @pSortBy = 'SortOrder' AND @pSortDirection = 'ASC' 
		THEN SortOrder END ASC,
		CASE WHEN @pSortBy = 'SortOrder' AND @pSortDirection = 'DESC' 
		THEN SortOrder END DESC
		FOR JSON Path,Root('DashboardResult'),Include_Null_Values
	End
End


