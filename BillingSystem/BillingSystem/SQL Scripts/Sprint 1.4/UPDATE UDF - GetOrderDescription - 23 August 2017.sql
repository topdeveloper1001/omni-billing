IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[GetOrderDescription]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].GetOrderDescription

Go

CREATE FUNCTION [dbo].[GetOrderDescription]
(
	-- Add the parameters for the function here
 @pOrderType nvarchar(50),
 @pOrderCode nvarchar(50)
)
RETURNS varchar(500)
AS

BEGIN

Declare @RETDescription nvarchar(500)

Set @RETDescription = 'CODE Category NOT FOUND'

If @pOrderType = '3' or @pOrderType = 'CPT'
Begin
	Declare @T Table (Code bigint, CodeDesc nvarchar(max))

	INSERT INTO @T
	Select  CAST(CodeNumbering as bigint),CodeDescription from CPTCodes Where ISNUMERIC(CodeNumbering)=1

	Select TOP 1 @RETDescription = CodeDesc From @T Where Code=@pOrderCode
	--Set @RETDescription = (Select TOP 1 CodeDescription from CPTCodes Where CodeNumbering = @pOrderCode)
End

If @pOrderType = '4' or @pOrderType = 'HCPCS'
	Set @RETDescription = (Select TOP 1 CodeDescription from HCPCSCodes Where CodeNumbering = @pOrderCode)

If @pOrderType = '5' or @pOrderType = 'DRUG'
	Set @RETDescription = (Select TOP 1 DrugPackageName from Drug Where DrugCode = @pOrderCode)

If @pOrderType = '8' or @pOrderType = 'ServiceCode'
	Set @RETDescription = (Select TOP 1 ServiceCodeDescription from ServiceCode Where ServiceCodeValue = @pOrderCode)
	
If @pOrderType = '9' or @pOrderType = 'DRG'
	Set @RETDescription = (Select TOP 1 CodeDescription from DRGCodes Where CodeNumbering = @pOrderCode)

Set @RETDescription = isnull(@RETDescription,'NOT a Valid Code')
	
return @RETDescription



END
