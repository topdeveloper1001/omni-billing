IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetPhysicianTabData')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetPhysicianTabData
GO

/****** Object:  StoredProcedure [dbo].[SprocGetPhysicianTabData]    Script Date: 22-03-2018 16:15:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetPhysicianTabData]
(
@EId bigint,
@PId bigint,
@PhyId bigint,
@NotesUserType smallint
--@PCareCategoryId int,
--@PCareStatus nvarchar(20),
--@PCareFlag int,
--@GcCategories nvarchar(500)='963,1024,1011,2305,3103,3102'
)
AS
BEGIN	
	Declare @FacilityId int=(Select TOP 1 EncounterFacility From Encounter Where EncounterID=@EId)
	Declare @LocalTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@FacilityId))

	/*-------------------Get Nurse Assessment and Other Form Details (PatientEvaluationSet), starts here-----------------------------*/
	Select S.SetId, S.CreatedDate As ENMStartdate,
	(Select TOP 1 PhysicianName From Physician Where Id = E.EncounterAttendingPhysician) PhysicianName,
	E.EncounterNumber, 
	(Case ISNULL(S.FormType,'')
		When '' THEN 'Evaluation Management'
		Else S.FormType
	 End) As DocumentName,
	 S.ExtValue2, S.CreatedBy,
	 (Select UserName From Users Where UserID = S.CreatedBy) As CompletedBy,
	'View Assessment Form' As Title
	From 
	(
	Select * From PatientEvaluationSet 
	Where PatientId = @PId And (@EId=0 OR EncounterId=@EId) 
	) As S
	INNER JOIN Encounter E ON S.EncounterId = E.EncounterID And (@EId=0 OR S.EncounterId=@EId)
	/*-------------------Get Nurse Assessment and Other Form Details (PatientEvaluationSet), ends here-----------------------------*/

	/*-------------------Get MedicalNotes and Other related Details, starts here-----------------------------*/
	Select DISTINCT * From MedicalNotes M
	Where ISNULL(M.IsDeleted,0)=0 AND M.NotesUserType = @NotesUserType And M.PatientID = @PId
	Order by M.CreatedDate Desc

	Select 
	(Select U.FirstName + ' ' + U.LastName From Users U Where U.UserID=M.NotesBy) As NotesAddedBy,
	(Case M.NotesUserType 
	WHEN 1 THEN 'Physician' ELSE 'Nurse' END) As NotesUserTypeName,
	(Select TOP 1 G.GlobalCodeName From GlobalCodes G 
	Where G.GlobalCodeCategoryValue = '963' And G.GlobalCodeValue = M.MedicalNotesType) As NotesTypeName
	From MedicalNotes M	
	Where ISNULL(M.IsDeleted,0)=0 AND M.NotesUserType = @NotesUserType And M.PatientID = @PId
	Order by M.CreatedDate Desc
	/*-------------------Get MedicalNotes and Other related Details, ends here-----------------------------*/


	/*-------------------Get Physician Orders (OpenOrder), starts here-----------------------------*/
	--Exec SprocGetPhysicianOrdersAndActivities @pEncounterId = @EId
	Exec SPROC_GetOrdersByEncounterId @pEncId=@EId
	/*-------------------Get Physician Orders (OpenOrder), ends here-----------------------------*/


	/*-------------------Get Nurse Documents (DocumentsTemplates), starts here-----------------------------*/
	Select D.* From DocumentsTemplates D
	Where ISNULL(D.IsDeleted,0)=0 And D.PatientID = @PId And D.ExternalValue3 = '4950'
	Order by D.DocumentsTemplatesID Desc
	/*-------------------Get Nurse Documents (DocumentsTemplates), ends here-----------------------------*/


	/*-------------------Get Patient Care Activities (OrderActivity), starts here-----------------------------*/
	DECLARE @OrderActivityTempTable Table(
	[OrderActivityID][int] NULL,[OrderType][int] NULL,[OrderCode][nvarchar](50) NULL,[OrderCategoryID][int] NULL
	,[OrderSubCategoryID][int] NULL,[OrderActivityStatus][int] NULL,[CorporateID][int] NULL,[FacilityID][int] NULL
	,[PatientID][int] NULL,[EncounterID][int] NULL,[MedicalRecordNumber][nvarchar](50),[OrderID][int] NULL
	,[OrderBy][int] NULL,[OrderActivityQuantity][decimal](18,2),[OrderScheduleDate][datetime] NULL,[PlannedBy][int] NULL
	,[PlannedDate][datetime] NULL,[PlannedEndDate][datetime] NULL,[PlannedFor][int] NULL,[ExecutedBy][int] NULL
	,[ExecutedDate][datetime] null,[ExecutedQuantity][decimal](18,2) null,[ResultValueMin][decimal](18,4)
	,[ResultValueMax][decimal](18,2),[ResultUOM][int] NULL,[Comments][nvarchar](500),[PatientName][nvarchar](500),
	[CategoryName][nvarchar](500),[SubCategoryName][nvarchar](500),[OrderDescription][nvarchar](500)
	,[Status][nvarchar](500),[OrderTypeName][nvarchar](500))

	INSERT INTO @OrderActivityTempTable
	Exec SPROC_GetOrderTypeActivity @pEncounterId=@EId, @pCategoryId=0, @pStatus='1,2,3,4,9', @pPatientId=@PId, @pFlag=1

	Select O.*
	,CAST((Case When DATEDIFF(minute,O.OrderScheduleDate,@LocalTime) > 0 THEN 1 ELSE 0 END) As bit) ShowEditAction
	,(Case ISNULL(O.ResultUOM,'') 
		When '' THEN '' 
		ELSE (Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = O.ResultUOM And GlobalCodeCategoryValue = '3108')
	 END) As ResultUOMStr
	 ,(Case 
		When ISNULL(O.OrderCategoryID,'')='11080' AND ISNULL(O.ResultValueMin,'') !='' 
		THEN (Select TOP 1 (CASE WHEN O.ResultValueMin between L.LabTestResultGoodFrom and L.LabTestResultGoodTo THEN 'Good'
			WHEN O.ResultValueMin between L.LabTestResultCautionFrom and L.LabTestResultCautionTo THEN 'Caution'
			WHEN O.ResultValueMin between L.LabTestResultBadFrom and L.LabTestResultBadTo THEN 'Bad'
			ELSE 'UnKnown' END) From [dbo].[LabTestResult] L Where L.LabTestResultCPTCode=O.OrderCode)
		ELSE ''
	 END) As LabResultTypeStr
	 ,(Case
		When ISNULL(O.OrderCategoryID,'')='11080' AND ISNULL(O.ResultValueMin,'') !='' 
		THEN (
				Select TOP 1 GlobalCodeName From GlobalCodes G1 Where G1.GlobalCodeCategoryValue = '3105' 
				And G1.GlobalCodeValue = (Select TOP 1 L.LabTestResultSpecimen From [dbo].[LabTestResult] L 
											Where L.LabTestResultCPTCode=O.OrderCode)
				)
		ELSE ''
	 END) As SpecimenTypeStr

	 ,(CASE WHEN O.OrderActivityStatus IN ('0','1','20') THEN Cast(1 As bit) ELSE Cast(0 as bit) END) As ShowSpecimanEditAction
	From @OrderActivityTempTable O
	/*-------------------Get Patient Care Activities (OrderActivity), ends here-----------------------------*/


	/*-------------------Get Patient's Medical Vitals based on Current Encounter (MedicalVital), starts here-----------------------------*/
	Select M.* From MedicalVital M Where ISNULL(M.IsDeleted,0)=0 And MedicalVitalType = 1 And PatientID = @PId
	Order by M.CreatedDate Desc


	Select M.*
	,Cast(Cast(M.AnswerValueMin as numeric(9,2)) as nvarchar) As PressureCustom
	,(Select TOP 1 (U.FirstName + ' ' + U.LastName) From Users U Where U.UserID=M.CommentBy),
	M.CreatedDate As VitalAddedOn
	,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = M.GlobalCode And GlobalCodeCategoryValue = '1901') As MedicalVitalName
	,(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeValue = M.AnswerUOM And GlobalCodeCategoryValue = '3101') As UnitOfMeasureName
	From MedicalVital M 
	Where ISNULL(M.IsDeleted,0)=0 And MedicalVitalType = 1 And PatientID = @PId
	Order by M.CreatedDate Desc
	/*-------------------Get Patient's Medical Vitals based on Current Encounter (MedicalVital), ends here-----------------------------*/


	/*-------------------Get DropdownListData (GlobalCodes), starts here-----------------------------*/
	
	/*	
	Bind Activities Status List
	Frequency Codes
	Order Status
	Order Quantity List
	Executed Quantity
	Order Type Category
	
	Categories:
	1. 963: Note Types
	2. 1024: Frequency
	3. 3102: Order Status
	4. 1011: Order Quantity
	5. 2305: Document Type
	6. 3103: Order Activity Status
	*/
	Exec SPROC_GetGlobalCodesByCategories @pGCC = '963,1024,1011,2305,3103,3102'

	/*-------------------Get DropdownListData (GlobalCodes), ends here-----------------------------*/


	/*-------------------Get Order Type Category (GlobalCodeCategory: ExternalValue3 as ordercategory), starts here-----------------------------*/
	Select * From GlobalCodeCategory Where ExternalValue3 = 'ordercategory' And IsActive = 1 And ISNULL(IsDeleted,0)=0
	Order by GlobalCodeCategoryName
	/*-------------------Get Order Type Category (GlobalCodeCategory: ExternalValue3 as ordercategory), ends here-----------------------------*/



	

	/*-------------------Get Lab Order Activities, starts here-----------------------------*/
	
	IF @NotesUserType = 1
		Exec SPROC_GetActiveLabOrdersByPhysicianId @UserId=@PhyId,@OrderActivityStatus=1
		,@OrderCategory='11080',@IsActiveEncountersOnly=1,@EncounterId=@EId
		
	/*-------------------Get Lab Order Activities, ends here-----------------------------*/
END


GO
