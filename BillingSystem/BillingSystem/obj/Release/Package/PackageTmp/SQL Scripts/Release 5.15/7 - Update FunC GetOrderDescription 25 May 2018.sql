USE [OmniDevDB]
GO

/****** Object:  UserDefinedFunction [dbo].[GetOrderDescription]    Script Date: 5/25/2018 4:45:40 PM ******/
DROP FUNCTION [dbo].[GetOrderDescription]
GO

/****** Object:  UserDefinedFunction [dbo].[GetOrderDescription]    Script Date: 5/25/2018 4:45:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
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
   
If @pOrderType = '16' or @pOrderType = 'Diagnosis'  
 Set @RETDescription = (Select Top 1 DiagnosisFullDescription from DiagnosisCode Where DiagnosisCode1 = @pOrderCode)  
  
Set @RETDescription = isnull(@RETDescription,'NOT a Valid Code')  
   
return @RETDescription  
  
  
  
END  
GO


