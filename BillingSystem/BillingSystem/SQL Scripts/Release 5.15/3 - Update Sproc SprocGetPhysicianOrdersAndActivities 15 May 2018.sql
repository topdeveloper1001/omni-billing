IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetPhysicianOrdersAndActivities')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetPhysicianOrdersAndActivities
GO

/****** Object:  StoredProcedure [dbo].[SprocGetPhysicianOrdersAndActivities]    Script Date: 5/15/2018 7:07:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[SprocGetPhysicianOrdersAndActivities]		-- [SPROC_GetPhysicianOrders] 2523,1
(
@pEncounterId Int,
@pOrderStatus NVarchar(10)='',
@pCategoryId bigint=0,
@pWithActivities bit=0
)
As
Declare @OpenOrderActivityStatusOnBill NVarchar(10) = '4'
Declare @OpenOrderActivityStatusCancel NVarchar(10) = '9'
Declare @OpenOrderActivityStatusClosed NVarchar(10) = '3'
Declare @GlobalCodeCategoryValueOrderStatus NVarchar(10) = '3102'
Declare @GlobalCodeCategoryValueOrderFrequencyType NVarchar(10) = '1024'
Declare @GlobalCodeCategoryValue NVarchar(10) = '3105'
Declare @FacilityId int = (Select TOP 1 EncounterFacility From Encounter Where EncounterID=@pEncounterId)

Declare @tblOpenOrders Table
(
	[OpenOrderID] [int],
	[OpenOrderPrescribedDate] [datetime],
	[PhysicianID] [int],
	[PatientID] [int],
	[EncounterID] [int],
	[DiagnosisCode] [nvarchar](300),
	[OrderType] [nvarchar](100),
	[OrderCode] [nvarchar](500),
	[Quantity] [decimal](18, 2),
	[FrequencyCode] [nvarchar](100),
	[PeriodDays] [nvarchar](20),
	[OrderNotes] [nvarchar](500),
	[OrderStatus] [nvarchar](500),
	[IsActivitySchecduled] [bit],
	[ItemName] [varchar](100),
	[ItemStrength] [varchar](50),
	[ItemDosage] [varchar](50),
	[IsActive] [bit],
	[CreatedBy] [int],
	[CreatedDate] [datetime],
	[ModifiedBy] [int],
	[ModifiedDate] [datetime],
	[IsDeleted] [bit],
	[DeletedBy] [int],
	[DeletedDate] [datetime],
	[CategoryId] [int],
	[SubCategoryId] [int],
	[StartDate] [datetime],
    [EndDate] [datetime],
	[DiagnosisDescription] [nvarchar](max),
	[CategoryName] [nvarchar](max),
	[SubCategoryName] [nvarchar](max),
	[OrderDescription] [nvarchar](max),
	[Status] [nvarchar](max),
	[FrequencyText] [nvarchar](max),
	[SpecimenTypeStr] [nvarchar](max),
	[IsApproved] [bit]
)
	
	Insert InTo @tblOpenOrders
	Select OpenOrderID, OpenOrderPrescribedDate, PhysicianID, PatientID, EncounterID, DiagnosisCode, OrderType, OrderCode, Quantity, FrequencyCode, PeriodDays,
	OrderNotes, OrderStatus, IsActivitySchecduled, ItemName, ItemStrength, ItemDosage, IsActive, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsDeleted,
	DeletedBy, DeletedDate, CategoryId, SubCategoryId, StartDate, EndDate
	, (Select TOP 1 DiagnosisCodeDescription  From Diagnosis Where Cast(DiagnosisID As NVarchar) = DiagnosisCode) DiagnosisDescription,
	(Select TOP 1 GlobalCodeCategoryName from GlobalCodeCategory where GlobalCodeCategoryValue = CategoryId AND IsDeleted=0 And FacilityNumber=@FacilityId) CategoryName,
	(Select TOP 1 GlobalCodeName from GlobalCodes where GlobalCodeValue = CAST(SubCategoryId as nvarchar) And IsDeleted=0 And GlobalCodeCategoryValue = CategoryId And FacilityNumber=@FacilityId) SubCategoryName, 
	 dbo.GetOrderDescription(OrderType, OrderCode) OrderDescription,
	(Case 
	When (CategoryId = 11100 And IsApproved = 0 And 
		OrderStatus != @OpenOrderActivityStatusOnBill And OrderStatus != @OpenOrderActivityStatusCancel And OrderStatus != @OpenOrderActivityStatusClosed) 
		Then 'Waiting For Approval'
	Else 
		(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = OrderStatus 
			And GlobalCodeCategoryValue = @GlobalCodeCategoryValueOrderStatus) 
	End
	) [Status],
	--(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = OrderStatus And GlobalCodeCategoryValue = @GlobalCodeCategoryValueOrderFrequencyType) FrequencyText,
	(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = FrequencyCode And GlobalCodeCategoryValue = @GlobalCodeCategoryValueOrderFrequencyType) FrequencyText,
	(
		Select GlobalCodeName from GlobalCodes where GlobalCodeCategoryValue='3105' and 
		GlobalCodeValue = (Select TOP 1 LabTestResultSpecimen From [dbo].[LabTestResult] Where Cast(LabTestResultCPTCode As Varchar) = OrderCode)
	) SpecimenTypeStr,
	IsApproved
	From OpenOrder
	Where EncounterID = @pEncounterId And (ISNULL(@pOrderStatus,'')='' OR OrderStatus = @pOrderStatus) 
	And (ISNULL(@pCategoryId,0)=0 OR CategoryId = @pCategoryId)
	Order By OpenOrderID Desc

	Select * From @tblOpenOrders

If @pWithActivities=1
Begin
	Declare @ActivitiesTable Table 
	([OrderActivityID] [int],[OrderType] [int],[OrderCode] [nvarchar](50),[OrderCategoryID] [int],[OrderSubCategoryID] [int]
	,[OrderActivityStatus] [int],[CorporateID] [int],[FacilityID] [int],[PatientID] [int],[EncounterID] [int]
	,[MedicalRecordNumber] [nvarchar](20),[OrderID] [int],[OrderBy] [int],[OrderActivityQuantity] [numeric](18, 2)
	,[OrderScheduleDate] [datetime],[PlannedBy] [int],[PlannedDate] [datetime],[PlannedFor] [int],[ExecutedBy] [int]
	,[ExecutedDate] [datetime],[ExecutedQuantity] [numeric](18, 2),[ResultValueMin] [numeric](18, 4)
	,[ResultValueMax] [numeric](18, 2),[ResultUOM] [int],[Comments] [nvarchar](250),[IsActive] [bit],[ModifiedBy] [int]
	,[ModifiedDate] [datetime],[CreatedBy] [int],[CreatedDate] [datetime],[CategoryName] [nvarchar](max)
	,[SubCategoryName] [nvarchar](max),[OrderDescription] [nvarchar](max),[Status] [nvarchar](max),[OrderTypeName] [nvarchar](max)
	,[ShowEditAction] [bit],[ResultUOMStr] [nvarchar](max),[LabResultTypeStr] [nvarchar](max),[SpecimenTypeStr] [nvarchar](max)
	,[ShowSpecimanEditAction] [bit],[BarCodeValue] [nvarchar](500)
	)

	INSERT INTO @ActivitiesTable
	Exec SPROC_GetOrderActivitiesByEncounterId @pEncounterId

	Select * FROM @ActivitiesTable Where OrderID IN (Select OpenOrderID From @tblOpenOrders Where ISNULL(@pCategoryId,0)=0 OR CategoryId = @pCategoryId)
End
GO


