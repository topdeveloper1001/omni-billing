IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetOrderActivitiesByEncounterId')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetOrderActivitiesByEncounterId

/****** Object:  StoredProcedure [dbo].[SPROC_GetOrderActivitiesByEncounterId]    Script Date: 3/22/2018 7:29:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[SPROC_GetOrderActivitiesByEncounterId] -- SPROC_GetOrderActivitiesByEncounterId 2228
(
@pEncounterId INT
)
AS
Begin
	DECLARE  @Facility_Id int =(select EncounterFacility from dbo.Encounter where EncounterID=@pEncounterId)
	Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))
	Declare @GlobalCodeCategoryValueActivityStatus Varchar(50) = '3103'
	Declare @GlobalCodeCategoryValueCodeTypes Varchar(50) = '1201'
	Declare @GlobalCodeCategoryValueLabMeasurementValue Varchar(50) = '3108'
	Declare @OrderTypeCategoryPathologyandLaboratory Int = 11080
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
	(Select TOP 1 GlobalCodeCategoryName From GlobalCodeCategory Where GlobalCodeCategoryValue = Cast(OrderCategoryID As Varchar) AND (@Facility_Id=0 OR FacilityNumber=CAST(@Facility_Id as nvarchar))) CategoryName,
	(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = CAST(OrderSubCategoryID as nvarchar) AND (@Facility_Id=0 OR FacilityNumber=CAST(@Facility_Id as nvarchar))) SubCategoryName,
	dbo.GetOrderDescription(OrderType, OrderCode) OrderDescription,
	(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = Cast(OrderActivityStatus As Varchar) And GlobalCodeCategoryValue = @GlobalCodeCategoryValueActivityStatus) [Status],
	(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = Cast(OrderType As Varchar) And GlobalCodeCategoryValue = @GlobalCodeCategoryValueCodeTypes 
	AND (@Facility_Id=0 OR FacilityNumber=CAST(@Facility_Id as nvarchar))) OrderTypeName
	,
	(
	Case When 
	Exists (Select 1 From OpenOrder OO Where OO.OpenOrderID=OrderID And OO.FrequencyCode IN ('10','1','23') And IsApproved=1
			And DATEDIFF(Day,OrderScheduleDate,@LocalDateTime)>=0)
	OR Exists (Select 1 From OpenOrder OO Where OO.OpenOrderID=OrderID And IsApproved=1
			And DATEDIFF(SECOND,OrderScheduleDate,@LocalDateTime)> 0)
	THEN
		1
	Else
		0
	End
	)
	As ShowEditAction,
	(Case When ResultUOM Is Not Null Then (Select GlobalCodeName From GlobalCodes Where GlobalCodeValue = Cast(ResultUOM As Varchar) And GlobalCodeCategoryValue = @GlobalCodeCategoryValueLabMeasurementValue) Else '' End) ResultUOMStr
	--,(Case When ((ResultValueMin Is Not Null) And OrderCategoryID = @OrderTypeCategoryPathologyandLaboratory) Then 
	--(Select Case When (Select  Count(*) From [dbo].[LabTestResult] Where Cast(LabTestResultCPTCode As NVarchar)  = OrderCode) = 0 Then '' Else 
	--                  (Select CASE WHEN ResultValueMin between LabTestResultGoodFrom and LabTestResultGoodTo THEN 'Good'
	--							   WHEN ResultValueMin between LabTestResultCautionFrom and LabTestResultCautionTo THEN 'Caution'
	--							   WHEN ResultValueMin between LabTestResultBadFrom and LabTestResultBadTo THEN 'Bad'
	--			                   ELSE 'UnKnown' END
	--                   From [dbo].[LabTestResult] where Cast(LabTestResultCPTCode As NVarchar) = OrderCode) 
	--        End) 
	--		Else '' End ) As LabResultTypeStr
	,'' As LabResultTypeStr,
	(Select TOP 1 GlobalCodeName from GlobalCodes where GlobalCodeCategoryValue='3105' and GlobalCodeValue = (Select TOP 1 LabTestResultSpecimen From [dbo].[LabTestResult] Where Cast(LabTestResultCPTCode As Varchar) = OrderCode )) SpecimenTypeStr,
	(Case When (Cast(OrderActivityStatus As Varchar) = '0' OR Cast(OrderActivityStatus As Varchar) = '1' OR Cast(OrderActivityStatus As Varchar) = '20') Then 1 Else 0 End) ShowSpecimanEditAction, BarCodeValue
	From OrderActivity Where (IsActive = null OR IsActive = 1) AND EncounterID = @pEncounterId Order By OrderScheduleDate Desc
	update @Table Set LabResultTypeStr = Case When ((T.ResultValueMin Is Not Null) And T.OrderCategoryID = @OrderTypeCategoryPathologyandLaboratory) Then
					  (CASE WHEN T.ResultValueMin between L.LabTestResultGoodFrom and L.LabTestResultGoodTo THEN 'Good'
								   WHEN T.ResultValueMin between L.LabTestResultCautionFrom and L.LabTestResultCautionTo THEN 'Caution'
								   WHEN T.ResultValueMin between L.LabTestResultBadFrom and L.LabTestResultBadTo THEN 'Bad'
								   ELSE 'UnKnown' END
					   )Else '' End 
						From @Table T Inner Join [dbo].[LabTestResult] L On Cast(L.LabTestResultCPTCode As NVarchar) = T.OrderCode

	Select * From @Table Where OrderID IN (Select OpenOrderID From OpenOrder Where IsApproved=1) --Inner Join [dbo].[LabTestResult] L On Cast(L.LabTestResultCPTCode As NVarchar) = T.OrderCode
End
GO


