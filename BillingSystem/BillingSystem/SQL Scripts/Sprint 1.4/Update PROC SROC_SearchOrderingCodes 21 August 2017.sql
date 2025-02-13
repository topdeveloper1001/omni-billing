IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SROC_SearchOrderingCodes') 
  DROP PROCEDURE SROC_SearchOrderingCodes;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SROC_SearchOrderingCodes]  --  SROC_SearchOrderingCodes 'test',11100,0,9,8
(
@pSearchText nvarchar(max)='',
@pCategoryid int=11100, 
@pSubcategoryId int = 0,
@pCID int = 6, 
@pFID int = 4
)
AS
BEGIN
	If ISNULL(@pCategoryid,0)=0
		SET @pCategoryid=11100

	SET @pSubcategoryId=ISNULL(@pSubcategoryId,0)
	---- Proc Calling Parmaters
--Declare @pSearchText nvarchar(max)='CYANIDE KIT AKRON',@pCategoryid int =11100, @pSubcategoryId int = 5036,@pCID int = 6, @pFID int = 4

---- Delcare the Table to return
Declare @GeneralCodesTable Table
(
Code nvarChar(50),[Description] nvarChar(1000),CodeDescription nvarChar(max),CodeType nvarChar(50),CodeTypeName nvarChar(100),ID nvarChar(50),ExternalCode nvarChar(50),
GlobalCodeId int,GlobalCodeCategoryId nvarChar(50),GlobalCodeName nvarChar(500),GlobalCodeCategoryName nvarChar(500),DrugPackageName nvarChar(500)
)

--- Declare the Local variables to be used in the PROC
declare @lMaxRange nvarchar(50),@lMinRange nvarchar(50),@lCodetype nvarchar(10), @lCPTCodeTableNumber nvarchar(20),@lDRUGCodeTableNumber nvarchar(20),
@lHCPCSCodeTableNumber nvarchar(20),@lDRGCodeTableNumber nvarchar(20),@lServiceCodeTableNumber nvarchar(20)

----- Get the Tbale number for the Facility 
Select TOP 1 @lCPTCodeTableNumber = ISNULL(CPTTableNumber,0),@lDRUGCodeTableNumber = ISNULL(DRUGTableNumber,0),
@lHCPCSCodeTableNumber = ISNULL(HCPCSTableNumber,0),@lDRGCodeTableNumber = ISNULL(DRGTableNumber,0),
@lServiceCodeTableNumber = ISNULL(ServiceCodeTableNumber,0) from BillingSystemParameters 
where FacilityNumber = (Select FacilityNumber from Facility where FacilityId = @pFID ) and CorporateId = @pCID

----- Get the Tbale number for the Corporate if not found for the Facility 
If(@lCPTCodeTableNumber = 0)
	Select @lCPTCodeTableNumber = ISNULL(DefaultCPTTableNumber,0) from Corporate where CorporateId = @pCID
If (@lDRUGCodeTableNumber = 0)
	Select @lDRUGCodeTableNumber = ISNULL(DefaultDRUGTableNumber,0) from Corporate where CorporateId = @pCID
If (@lHCPCSCodeTableNumber = 0)
	Select @lHCPCSCodeTableNumber = ISNULL(DefaultHCPCSTableNumber,0) from Corporate where CorporateId = @pCID
If (@lDRGCodeTableNumber = 0)
	Select @lDRGCodeTableNumber = ISNULL(DefaultDRGTableNumber,0) from Corporate where CorporateId = @pCID
If (@lServiceCodeTableNumber = 0)
	Select @lServiceCodeTableNumber = ISNULL(DefaultServiceCodeTableNumber,0) from Corporate where CorporateId = @pCID


--- Get the Code type, Min value and max value for the Code
if(@pCategoryid != 0)
Select @lMinRange= Min(ISNULL(ExternalValue2,0)), @lMaxRange =Max(ISNULL(ExternalValue3,0)),@lCodetype =Max(ISNULL(ExternalValue1,0)) from GlobalCodes 
where GlobalCodeCategoryValue = @pCategoryid
	--Select @lMinRange= Min(ExternalValue2), @lMaxRange =Max(ExternalValue3),@lCodetype =Max(ExternalValue1) from GlobalCodes 
	--where GlobalCodeCategoryValue = @pCategoryid


--- Get the Code type, Min value and max value for the Code From Subcategpry
if(@pSubcategoryId != 0)
	Select @lMinRange= Min(ISNULL(ExternalValue2,0)), @lMaxRange =Max(ISNULL(ExternalValue3,0)),@lCodetype =Max(ISNULL(ExternalValue1,0)) from GlobalCodes 
	where GlobalCodeValue = CAST(@pSubcategoryId as nvarchar) And (@pFID=0 OR FacilityNumber=@pFID)


Set @lCodetype = CASE when @pCategoryid = 11100 THEN '5' ELSE ISNULL(@lCodetype,'0') END


--- Now fetch the Code data from the tables
if @lCodetype = '3' 
BEGIN
	Insert into @GeneralCodesTable(Code,[Description],CodeDescription,CodeType,CodeTypeName,ID,ExternalCode
	,GlobalCodeId,GlobalCodeCategoryId,GlobalCodeName,
	GlobalCodeCategoryName,DrugPackageName)
	Select CodeNumbering,CodeDescription,ISNULL(CodeNumbering,'') +' - '+ISNULL(CodeDescription,''),'3','CPT',Cast(CPTCodesId as nvarchar(50)),Cast(CTPCodeRangeValue as nvarchar(50)),
	CAST(dbo.GetOrderSubCategoryByOrderCodeAndFacilityId(CTPCodeRangeValue,'3',@pFID) as int),dbo.GetOrderCategoryByOrderCodeAndFacility(CTPCodeRangeValue,'3',@pFID)
	,null,null,null
	 from CptCodes where (CodeNumbering like '%'+ @pSearchText  +'%' or CodeDescription like '%'+ @pSearchText  +'%')
	and CodeTableNumber = @lCPTCodeTableNumber and ISNULL(IsActive,1) = 1 and ISNULL(IsDeleted,0) = 0
	and (@pCategoryid = 0 or CTPCodeRangeValue between @lMinRange and @lMaxRange)
	and (@pSubcategoryId = 0 or CTPCodeRangeValue between @lMinRange and @lMaxRange)
END


ELSE if @lCodetype = '4'
BEGIN
	Insert into @GeneralCodesTable(Code,[Description],CodeDescription,CodeType,CodeTypeName,ID,ExternalCode,GlobalCodeId,GlobalCodeCategoryId,GlobalCodeName,
	GlobalCodeCategoryName,DrugPackageName)
	Select CodeNumbering,CodeDescription,ISNULL(CodeNumbering,'') +' - '+ISNULL(CodeDescription,''),'4','HCPCS',Cast(HCPCSCodesId as nvarchar(50)),'',NULL,NULL,NULL,null,null
	from HCPCSCodes where (CodeNumbering like '%'+ @pSearchText  +'%' or CodeDescription like '%'+ @pSearchText  +'%')
	and CodeTableNumber = @lHCPCSCodeTableNumber and ISNULL(IsActive,1) = 1 and ISNULL(IsDeleted,0) = 0
END

ELSE if @lCodetype = '5'
BEGIN
	--Declare @TGCodes Table (SubCategoryValue bigint,CategoryValue nvarchar(50),DrugId bigint)

	--INSERT INTO @TGCodes
	--Select CAST(dbo.GetOrderSubCategoryByOrderCodeAndType(DrugCode,'5') as int),dbo.GetOrderCategoryByOrderCodeAndType(DrugCode,'5'),Id
	--From DRUG Where DRUGTableNumber = @lDRUGCodeTableNumber and DrugStatus not like 'Deleted' and ISNULL(InStock,1)=1

	--Insert into @GeneralCodesTable(Code,[Description],CodeDescription,CodeType,CodeTypeName,ID,ExternalCode
	--,GlobalCodeName,GlobalCodeCategoryName,DrugPackageName,GlobalCodeId,GlobalCodeCategoryId)
	--Select D.DrugCode,D.DrugPackageName,D.CodeDescription,D.CodeType,D.CodeTypeName,D.ID,D.ExternalCode,D.GlobalCodeName,D.GlobalCodeCategoryName
	--,D.DrugPackageName, T.SubCategoryValue As GlobalCodeId,T.CategoryValue As GlobalCodeCategoryId 
	--From 
	--(
	--Select DrugCode,DrugPackageName As [Description],(ISNULL(DrugCode,'') +' - '+ISNULL(DrugPackageName,'') + ' - '+ ISNULL(DrugGenericName,'') +' - '+ISNULL(DrugStrength,'')+' - '+ISNULL(DrugDosage,'')) As CodeDescription
	--,'5' As CodeType,'DRUG' As CodeTypeName,ISNULL(BrandCode,'0') As ID,Cast(Id as nvarchar(50)) As ExternalCode
	--,Null As GlobalCodeName,Null As GlobalCodeCategoryName,ISNULL(DrugPackageName,'') As DrugPackageName
	--from DRUG where (DrugCode like '%'+ @pSearchText  +'%' or DrugGenericName like '%'+ @pSearchText  +'%' or DrugPackageName like '%'+ @pSearchText  +'%')
	--and DRUGTableNumber = @lDRUGCodeTableNumber and DrugStatus not like 'Deleted' and ISNULL(InStock,1) =1
	--) D
	--INNER JOIN @TGCodes T ON D.ID=T.DrugId

	Insert into @GeneralCodesTable(Code,[Description],CodeDescription,CodeType,CodeTypeName,ID,ExternalCode,GlobalCodeId,GlobalCodeCategoryId,GlobalCodeName,
	GlobalCodeCategoryName,DrugPackageName)
	Select DrugCode,DrugPackageName,ISNULL(DrugCode,'') +' - '+ISNULL(DrugPackageName,'') + ' - '+ ISNULL(DrugGenericName,'') +' - '+ISNULL(DrugStrength,'')+' - '+ISNULL(DrugDosage,''),'5','DRUG',ISNULL(BrandCode,'0'),Cast(ID as nvarchar(50)),
	CAST(dbo.GetOrderSubCategoryByOrderCodeAndFacilityId(DrugCode,'5',@pFID) as int),dbo.GetOrderCategoryByOrderCodeAndFacility(DrugCode,'5',@pFID),Null,Null,ISNULL(DrugPackageName,'')
	from DRUG where (DrugCode like '%'+ @pSearchText  +'%' or DrugGenericName like '%'+ @pSearchText  +'%' or DrugPackageName like '%'+ @pSearchText  +'%')
	and DRUGTableNumber = @lDRUGCodeTableNumber and DrugStatus not like 'Deleted' and ISNULL(InStock,1) =1
END
Else If @lCodetype = '0'
BEGIN
	Insert into @GeneralCodesTable(Code,[Description],CodeDescription,CodeType,CodeTypeName,ID,ExternalCode,GlobalCodeId,GlobalCodeCategoryId,GlobalCodeName,
	GlobalCodeCategoryName,DrugPackageName)
	Select CodeNumbering,CodeDescription,ISNULL(CodeNumbering,'') +' - '+ISNULL(CodeDescription,''),'3','CPT',Cast(CPTCodesId as nvarchar(50)),Cast(CTPCodeRangeValue as nvarchar(50)),
	CAST(dbo.GetOrderSubCategoryByOrderCodeAndFacilityId(CTPCodeRangeValue,'3',@pFID) as int),dbo.GetOrderCategoryByOrderCodeAndFacility(CTPCodeRangeValue,'3',@pFID)
	,null,null,null
	 from CptCodes where (CodeNumbering like '%'+ @pSearchText  +'%' or CodeDescription like '%'+ @pSearchText  +'%')
	and CodeTableNumber = @lCPTCodeTableNumber and ISNULL(IsActive,1) = 1 and ISNULL(IsDeleted,0) = 0
	UNION
	Select DrugCode,DrugPackageName,ISNULL(DrugCode,'') +' - '+ISNULL(DrugPackageName,'') + ' - '+ ISNULL(DrugGenericName,'') +' - '+ISNULL(DrugStrength,'')+' - '+ISNULL(DrugDosage,''),'5','DRUG',ISNULL(BrandCode,'0'),Cast(ID as nvarchar(50)),
	CAST(dbo.GetOrderSubCategoryByOrderCodeAndFacilityId(DrugCode,'3',@pFID) as int),dbo.GetOrderCategoryByOrderCodeAndFacility(DrugCode,'3',@pFID),Null,Null,ISNULL(DrugPackageName,'')
	 from DRUG where (DrugCode like '%'+ @pSearchText  +'%' or DrugGenericName like '%'+ @pSearchText  +'%' or DrugPackageName like '%'+ @pSearchText  +'%')
	and DRUGTableNumber = @lDRUGCodeTableNumber and DrugStatus not like 'Deleted' and ISNULL(InStock,1) =1
	UNION
	Select CodeNumbering,CodeDescription,ISNULL(CodeNumbering,'') +' - '+ISNULL(CodeDescription,''),'4','HCPCS',Cast(HCPCSCodesId as nvarchar(50)),'',HCPCSCodesId,NULL,NULL,null,null
	from HCPCSCodes where (CodeNumbering like '%'+ @pSearchText  +'%' or CodeDescription like '%'+ @pSearchText  +'%')
	and CodeTableNumber = @lHCPCSCodeTableNumber and ISNULL(IsActive,1) = 1 and ISNULL(IsDeleted,0) = 0
END

Select * from @GeneralCodesTable


END





