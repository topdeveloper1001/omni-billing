IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[GetOrderCategoryByOrderCodeAndFacility]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].GetOrderCategoryByOrderCodeAndFacility

Go

CREATE FUNCTION [dbo].[GetOrderCategoryByOrderCodeAndFacility]
(
	@pOrderCode nvarchar(50), 
	@pOrderType nvarchar(10),
	@pFId bigint=null
)
RETURNS varchar(50)
AS
BEGIN
	
Declare @Ordercategory nvarchar(50);

If(@pOrderType =3) --- For CPT
BEGIN
	--Declare @TempTable1 Table (GCCV nvarchar(20),GCV nvarchar(20),EV1 nvarchar(2),EV2 nvarchar(50),EV3 nvarchar(50),UserValue nvarchar(50))
	--Insert into @TempTable1	
	--Select GlobalCodeCategoryValue,GlobalCodeValue,ExternalValue1,ExternalValue2,ExternalValue3,@pOrderCode from GlobalCodes where  
	--ExternalValue1 = '3'  
	--and (CAST(@pOrderCode as Nvarchar(50)) Between CAST(ExternalValue2 as Nvarchar(50)) and Cast(ExternalValue3 as Nvarchar(50)))
	--And (ISNULL(@pFId,0)=0 OR FacilityNumber=@pFId) 
	
	--Set @Ordercategory = (Select TOP 1 GCCV from @TempTable1 Where Cast(@pOrderCode as Int) Between Cast(EV2 as Int) and Cast(EV3 as Int))

	Select TOP 1 @Ordercategory=GlobalCodeCategoryValue from GlobalCodes where  
	ExternalValue1 = '3'  
	and (CAST(@pOrderCode as Nvarchar(50)) Between CAST(ExternalValue2 as Nvarchar) and Cast(ExternalValue3 as Nvarchar))
	And (ISNULL(@pFId,0)=0 OR FacilityNumber=@pFId)

END
ELSE If(@pOrderType = 5 ) -- FOR DRUG
BEGIN
	Declare @BrandCode nvarchar	(10) = (Select TOP 1 BrandCode from Drug Where DrugCode like @pOrderCode)

	SET @Ordercategory =(Select TOP  1 GlobalCodeCategoryValue from GlobalCodes where  
	GlobalCodeValue = @BrandCode and GlobalCodeCategoryValue = 11100 And (ISNULL(@pFId,0)=0 OR FacilityNumber=@pFId))
	
END

RETURN @Ordercategory

END
