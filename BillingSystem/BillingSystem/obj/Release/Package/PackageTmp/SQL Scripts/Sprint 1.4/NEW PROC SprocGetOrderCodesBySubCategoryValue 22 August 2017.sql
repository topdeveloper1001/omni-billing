IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocGetOrderCodesBySubCategoryValue') 
  DROP PROCEDURE SprocGetOrderCodesBySubCategoryValue;
 
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
CREATE PROCEDURE SprocGetOrderCodesBySubCategoryValue 
(
@pTableNumber nvarchar(100),
@pCategoryValue nvarchar(50),
@pSubCategoryValue nvarchar(20),
@pOrderCodeType nvarchar(10),
@pStartRange bigint,
@pEndRange bigint,
@pFId bigint=0
)
AS
BEGIN
	Declare @TCodes Table([Text] nvarchar(max),[Value] nvarchar(200))

	--Get CodeTypeName and CodeTypeValue
	Select TOP 1 * From GlobalCodes Where GlobalCodeCategoryValue='1201' And GlobalCodeValue=@pOrderCodeType
	
	--Get Order Codes
	IF @pOrderCodeType='3'
	Begin
		Declare @CPTs Table (Code bigint, Descs nvarchar(max))

		INSERT INTO @CPTs
		Select CAST(CodeNumbering as bigint) As CodeNumbering,(CodeNumbering + ' - ' + CodeDescription) From CPTCodes Where CodeNumbering not like '%[^0-9]%' And CodeTableNumber='4010' And ISNULL(IsActive,1)=1
		And ISNULL(IsDeleted,0)=0


		INSERT INTO @TCodes ([Text],[Value])
		Select Descs,Code From @CPTs
		Where Code Between @pStartRange and @pEndRange
		Order by Descs
	End
	Else IF @pOrderCodeType='4'
		INSERT INTO @TCodes ([Text],[Value])
		Select (CodeNumbering + ' - ' + CodeDescription),CodeNumbering From HCPCSCodes Where CodeNumbering NOT IN ('T','F') 
		And CodeTableNumber=@pTableNumber And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0
		Order by CodeDescription
		
	Else IF @pOrderCodeType='5'
		INSERT INTO @TCodes ([Text],[Value])
		Select (DrugCode + ' - ' + DrugGenericName + ' - ' + DrugStrength + ' - ' + DrugDosage) As [Text],DrugCode As [Value] 
		From Drug Where DrugTableNumber=@pTableNumber
		And BrandCode=@pSubCategoryValue
		And ISNULL(DrugStatus,'') IN ('Active','Grace')
		And ISNULL(InStock,1)=1
		Order by DrugCode

	Select * From @TCodes
END
GO
