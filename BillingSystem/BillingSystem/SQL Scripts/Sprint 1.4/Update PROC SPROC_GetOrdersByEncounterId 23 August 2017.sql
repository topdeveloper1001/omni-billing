IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SPROC_GetOrdersByEncounterId') 
  DROP PROCEDURE SPROC_GetOrdersByEncounterId;
 
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
CREATE PROCEDURE [dbo].[SPROC_GetOrdersByEncounterId] -- [SPROC_GetOrdersByEncounterId] 2848
(
	@pEncId int =3453
)
AS
BEGIN
	
	Declare @PharmacyCategoryid nvarchar(20)= '11100',@pOrderStatusGCC nvarchar(50) = '3102',@pFrequencyCodeGCC nvarchar(50) = '1024',
	@pOrderTypeGCC nvarchar(50) = '1201',@pLabTestResultSpecimenGCC nvarchar(50) ='3105',
	@FId bigint = (Select TOP 1 EncounterFacility From Encounter Where EncounterID=@pEncId)

	Select OpenOrderID,OpenOrderPrescribedDate,PhysicianID,PatientID,EncounterID,DiagnosisCode,OrderType,CategoryId,SubCategoryId,
	OrderCode,Quantity,FrequencyCode,PeriodDays,OrderNotes,OrderStatus,IsActivitySchecduled,ItemName,ItemStrength,
	ItemDosage,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,DeletedBy,DeletedDate,StartDate,EndDate,IsApproved,
	dbo.[GetOrderDescription](OrderType,OrderCode) 'OrderDescription',
	CASE WHEN CategoryId = @PharmacyCategoryid AND OrderStatus not in ('3','4','9') AND IsApproved = 0 THEN 'Waiting For Approval' ELSE
	dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pOrderStatusGCC,OrderStatus) END'Status',
	dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pFrequencyCodeGCC,FrequencyCode) 'FrequencyText',
	(Select TOP 1 GlobalCodeCategoryName from GlobalCodeCategory where GlobalCodeCategoryValue = CategoryId And FacilityNumber=@FId) 'CategoryName',
	(Select TOP 1 GlobalCodeName from GlobalCodes where GlobalCodeValue = CAST(SubCategoryId as nvarchar) And GlobalCodeCategoryValue = CategoryId And FacilityNumber=@FId) 'SubCategoryName',
	dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pOrderTypeGCC,OrderType) 'OrderTypeName',
	(Select TOP 1 GlobalCodeName from GlobalCodes where GlobalCodeCategoryValue=@pLabTestResultSpecimenGCC 
	and GlobalCodeValue = (Select TOP 1 LabTestResultSpecimen From [dbo].[LabTestResult] Where Cast(LabTestResultCPTCode As Varchar) = OrderCode )) 
	'SpecimenTypeStr'
	from OpenOrder where EncounterId =@pEncId order by 1 desc

END





