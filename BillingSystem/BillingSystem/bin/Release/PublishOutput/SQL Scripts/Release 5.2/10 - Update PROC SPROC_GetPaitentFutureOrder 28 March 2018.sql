IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetPaitentFutureOrder')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetPaitentFutureOrder
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetPaitentFutureOrder]    Script Date: 3/28/2018 10:22:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetPaitentFutureOrder]  
(
	@pPId int =1086
)
AS
BEGIN
	Declare @PharmacyCategoryid nvarchar(20)= '11100'
	,@pOrderStatusGCC nvarchar(50) = '3102'
	,@pFrequencyCodeGCC nvarchar(50) = '1024'
	,@pOrderTypeGCC nvarchar(50) = '1201'
	,@pLabTestResultSpecimenGCC nvarchar(50) ='3105'

	Select FutureOpenOrderID,OpenOrderPrescribedDate,PhysicianID,PatientID,EncounterID,DiagnosisCode,OrderType,
	OrderCode,Quantity,FrequencyCode,PeriodDays,OrderNotes,OrderStatus,IsActivitySchecduled,ItemName,ItemStrength,
	ItemDosage,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,DeletedBy,DeletedDate,StartDate,EndDate
	--dbo.[GetOrderDescription](OrderType,OrderCode) 'OrderDescription',
	,OrderDescription= (Case WHEN OrderType='3' OR OrderType='CPT' THEN 
									(Select TOP 1 CodeDescription from CPTCodes Where CodeNumbering = OrderCode)
								WHEN OrderType='4' OR OrderType='HCPCS' THEN
									(Select TOP 1 CodeDescription from HCPCSCodes Where CodeNumbering = OrderCode)
								WHEN OrderType='5' OR OrderType='DRUG' THEN 
									(Select TOP 1 DrugPackageName from Drug Where DrugCode = OrderCode)
								WHEN OrderType='8' OR OrderType='ServiceCode' THEN 
									(Select TOP 1 ServiceCodeDescription from ServiceCode Where ServiceCodeValue = OrderCode)
								WHEN OrderType='9' OR OrderType='DRG' THEN 
									(Select TOP 1 CodeDescription from DRGCodes Where CodeNumbering = OrderCode)
								ELSE '' END)
	--,dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pOrderStatusGCC,OrderStatus) 'Status'
	,[Status]=(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = @pOrderStatusGCC And GlobalCodeValue = OrderStatus)
	--,dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pFrequencyCodeGCC,FrequencyCode) 'FrequencyText'
	,FrequencyText=(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = @pFrequencyCodeGCC And GlobalCodeValue = FrequencyCode)
	,(Select GlobalCodeCategoryName from GlobalCodeCategory where GlobalCodeCategoryValue = CategoryId) 'CategoryName',
	(Select GlobalCodeName from GlobalCodes where GlobalCodeID = SubCategoryId)'SubCategoryName'
	--,dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pOrderTypeGCC,OrderType) 'OrderTypeName'
	,OrderTypeName=(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = @pOrderTypeGCC And GlobalCodeValue = OrderType)
	,(Select TOP 1 GlobalCodeName from GlobalCodes where GlobalCodeCategoryValue=@pLabTestResultSpecimenGCC
	and GlobalCodeValue = (Select TOP 1 LabTestResultSpecimen From [dbo].[LabTestResult] Where Cast(LabTestResultCPTCode As Varchar) = OrderCode )) 
	'SpecimenTypeStr'
	from FutureOpenOrder where PatientID = @pPId order by 1 desc
END
GO


