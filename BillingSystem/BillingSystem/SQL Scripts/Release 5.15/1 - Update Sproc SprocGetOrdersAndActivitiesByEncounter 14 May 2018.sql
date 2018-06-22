IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetOrdersAndActivitiesByEncounter')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetOrdersAndActivitiesByEncounter
GO

/****** Object:  StoredProcedure [dbo].[SprocGetOrdersAndActivitiesByEncounter]    Script Date: 5/14/2018 7:10:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



Create PROCEDURE [dbo].[SprocGetOrdersAndActivitiesByEncounter]
(
@pEncounterId bigint
)
AS
BEGIN
	--Get All Orders by Encounter ID
	Declare @PharmacyCategoryid nvarchar(20)= '11100',@pOrderStatusGCC nvarchar(50) = '3102',@pFrequencyCodeGCC nvarchar(50) = '1024'
	,@pOrderTypeGCC nvarchar(50) = '1201',@pLabTestResultSpecimenGCC nvarchar(50) ='3105'
	,@FId bigint = (Select TOP 1 EncounterFacility From Encounter Where EncounterID=@pEncounterId)
	,@OrderActivityStatusGCC nvarchar(50)='3103'
	,@LabMeasurementValueGCC nvarchar(10)='3108'
	,@PathologyandLaboratoryGCC nvarchar(10)='11080'
	
	Declare @Table Table
	(
	[OrderActivityID] [int],
	[OrderType] [int],
	[OrderCode] [nvarchar](50),
	[OrderCategoryID] [int],
	[OrderSubCategoryID] [int],
	[OrderActivityStatus] [int],
	[CorporateID] [int],
	[FacilityID] [int],
	[PatientID] [int],
	[EncounterID] [int],
	[MedicalRecordNumber] [nvarchar](20),
	[OrderID] [int],
	[OrderBy] [int],
	[OrderActivityQuantity] [numeric](18, 2),
	[OrderScheduleDate] [datetime],
	[PlannedBy] [int],
	[PlannedDate] [datetime],
	[PlannedFor] [int],
	[ExecutedBy] [int],
	[ExecutedDate] [datetime],
	[ExecutedQuantity] [numeric](18, 2),
	[ResultValueMin] [numeric](18, 4),
	[ResultValueMax] [numeric](18, 2),
	[ResultUOM] [int],
	[Comments] [nvarchar](250),
	[IsActive] [bit],
	[ModifiedBy] [int],
	[ModifiedDate] [datetime],
	[CreatedBy] [int],
	[CreatedDate] [datetime],
	[CategoryName] [nvarchar](max),
	[SubCategoryName] [nvarchar](max),
	[OrderDescription] [nvarchar](max),
	[Status] [nvarchar](max),
	[OrderTypeName] [nvarchar](max),
	[ShowEditAction] [bit],
	[ResultUOMStr] [nvarchar](max),
	[LabResultTypeStr] [nvarchar](max),
	[SpecimenTypeStr] [nvarchar](max),
	[ShowSpecimanEditAction] [bit],
	[BarCodeValue] [nvarchar](500)
	)

	Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@FId))

	Select OpenOrderID,OpenOrderPrescribedDate,PhysicianID,PatientID,EncounterID,DiagnosisCode,OrderType,CategoryId,SubCategoryId,
	OrderCode,Quantity,FrequencyCode,PeriodDays,OrderNotes,OrderStatus,IsActivitySchecduled,ItemName,ItemStrength,
	ItemDosage,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,DeletedBy,DeletedDate,StartDate,EndDate,IsApproved
	--,dbo.[GetOrderDescription](OrderType,OrderCode) 'OrderDescription',
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
	,[Status]=(CASE WHEN CategoryId = @PharmacyCategoryid AND OrderStatus not in ('3','4','9') AND IsApproved = 0 
					THEN 'Waiting For Approval'
					ELSE
							(Select TOP 1 GlobalCodeName From GlobalCodes 
									Where GlobalCodeCategoryValue = @pOrderStatusGCC and isdeleted=0 And GlobalCodeValue = OrderStatus)
					--dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pOrderStatusGCC,OrderStatus) 
					END)
	--,dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pFrequencyCodeGCC,FrequencyCode) 'FrequencyText',
	,FrequencyText=(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = @pFrequencyCodeGCC and isdeleted=0 And GlobalCodeValue = FrequencyCode)
	,CategoryName=(Select TOP 1 GlobalCodeCategoryName from GlobalCodeCategory where GlobalCodeCategoryValue = CategoryId AND IsDeleted=0 And FacilityNumber=@FId)
	,SubCategoryName=(Select TOP 1 GlobalCodeName from GlobalCodes where GlobalCodeValue = CAST(SubCategoryId as nvarchar) AND IsDeleted=0
				And GlobalCodeCategoryValue = CategoryId And FacilityNumber=@FId)
	,OrderTypeName=(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = @pOrderTypeGCC and isdeleted=0 And GlobalCodeValue = OrderType)
	--dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pOrderTypeGCC,OrderType) 'OrderTypeName',
	,SpecimenTypeStr=(Select TOP 1 GlobalCodeName from GlobalCodes 
			where GlobalCodeCategoryValue=@pLabTestResultSpecimenGCC  and isdeleted=0
			and GlobalCodeValue = (Select TOP 1 LabTestResultSpecimen From [dbo].[LabTestResult] 
									Where Cast(LabTestResultCPTCode As Varchar) = OrderCode )) 
	FROM OpenOrder where EncounterId=@pEncounterId Order by EndDate DESC
	FOR JSON PATH,Root('OpenOrders'),INCLUDE_NULL_VALUES

	--Get All Order Activities by Encounter ID And Insert int Temp Table
	Insert InTo @Table
	Select OrderActivityID, OrderType, OrderCode, OrderCategoryID, OrderSubCategoryID, OrderActivityStatus, CorporateID, FacilityID, PatientID, EncounterID,
	MedicalRecordNumber, OrderID, OrderBy, OrderActivityQuantity, OrderScheduleDate, PlannedBy, PlannedDate, PlannedFor, ExecutedBy, ExecutedDate, ExecutedQuantity,
	ResultValueMin, ResultValueMax, 
	(Case When (ResultUOM Is Null OR ResultUOM = '') Then 
	(Select Case When (Select  Count(*) From [dbo].[LabTestResult] Where Cast(LabTestResultCPTCode As NVarchar)  = OrderCode) = 0 Then 0 Else 
					  (Select  TOP 1 Case When (LabTestResultMeasurementValue Is Null OR LabTestResultMeasurementValue = '') Then 0 Else LabTestResultMeasurementValue End
					   From [dbo].[LabTestResult] where Cast(LabTestResultCPTCode As NVarchar) = OrderCode) 
			End)
	Else ResultUOM END) ResultUOM, Comments, IsActive, ModifiedBy, ModifiedDate, CreatedBy, CreatedDate, 
	(Select TOP 1 GlobalCodeCategoryName From GlobalCodeCategory Where GlobalCodeCategoryValue = Cast(OrderCategoryID As Varchar) AND IsDeleted=0 
			AND (@FId=0 OR FacilityNumber=CAST(@FId as nvarchar))) CategoryName,
	(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = CAST(OrderSubCategoryID as nvarchar) AND IsDeleted=0 AND (@FId=0 OR FacilityNumber=CAST(@FId as nvarchar))) SubCategoryName
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
	,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = Cast(OrderActivityStatus As Varchar) and isdeleted=0 And GlobalCodeCategoryValue = @OrderActivityStatusGCC) [Status],
	(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = Cast(OrderType As Varchar) And GlobalCodeCategoryValue = @pOrderTypeGCC and isdeleted=0
	AND (@FId=0 OR FacilityNumber=CAST(@FId as nvarchar))) OrderTypeName
	,ShowEditAction=(Case When 
						Exists (Select 1 From OpenOrder OO Where OO.OpenOrderID=OrderID And OO.FrequencyCode IN ('10','1','23') And IsApproved=1
								And DATEDIFF(Day,OrderScheduleDate,@LocalDateTime)>=0)
						OR Exists (Select 1 From OpenOrder OO Where OO.OpenOrderID=OrderID And IsApproved=1
								And DATEDIFF(SECOND,OrderScheduleDate,@LocalDateTime) > 0)
						THEN 1 Else 0 End)
	,ResultUOMStr=(Case 
						When ResultUOM Is Not Null 
						Then (Select GlobalCodeName From GlobalCodes 
									Where GlobalCodeValue = Cast(ResultUOM As Varchar) and isdeleted=0 And GlobalCodeCategoryValue = @LabMeasurementValueGCC) 
						Else '' End)
	,'' As LabResultTypeStr,
	(Select TOP 1 GlobalCodeName from GlobalCodes where GlobalCodeCategoryValue='3105' and isdeleted=0 and GlobalCodeValue = (Select TOP 1 LabTestResultSpecimen From [dbo].[LabTestResult] Where Cast(LabTestResultCPTCode As Varchar) = OrderCode )) SpecimenTypeStr,
	(Case When (Cast(OrderActivityStatus As Varchar) = '0' OR Cast(OrderActivityStatus As Varchar) = '1' OR Cast(OrderActivityStatus As Varchar) = '20') Then 1 Else 0 End) ShowSpecimanEditAction, BarCodeValue
	From OrderActivity 
	Where EncounterID = @pEncounterId AND ISNULL(IsActive,1)=1 AND OrderID IN (Select O.OpenOrderID From OpenOrder O Where O.IsApproved=1)
	Order By OrderScheduleDate Desc

	--Update Lab Result Values in the Order Activities
	UPDATE @Table
	Set LabResultTypeStr = 
		(Case When T.ResultValueMin Is Not Null And T.OrderCategoryID = @PathologyandLaboratoryGCC
		Then
			(CASE WHEN T.ResultValueMin BETWEEN L.LabTestResultGoodFrom and L.LabTestResultGoodTo THEN 'Good'
						WHEN T.ResultValueMin between L.LabTestResultCautionFrom and L.LabTestResultCautionTo THEN 'Caution'
						WHEN T.ResultValueMin between L.LabTestResultBadFrom and L.LabTestResultBadTo THEN 'Bad'
						ELSE 'UnKnown' END
			)
		Else '' End) 
	From @Table T
	Inner Join [dbo].[LabTestResult] L On Cast(L.LabTestResultCPTCode As NVarchar) = T.OrderCode

	--Return the list of Temp Table.
	Select * From @Table
	FOR JSON PATH,Root('OrderActivities'),INCLUDE_NULL_VALUES

END
GO


