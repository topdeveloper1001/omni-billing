IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocOrdersViewData')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE SprocOrdersViewData
END
GO
CREATE PROCEDURE [dbo].[SprocOrdersViewData] (@PhysicianId INT, @MostRecentDays INT, @CId INT, @FId INT, @EncId INT, @GCCategoryCodes NVARCHAR(max), @PatientId INT,
											  @OpCode NVARCHAR(50), @ExParam1 NVARCHAR(50))
AS
BEGIN
	IF @OpCode = '7'
	Begin
			------------$$$$$$$$$$$$$$$$$$$$$$$  DECLARATIONS    $$$$$$$$$$$$$$$$$$$$$$$---------------
				--------Open Order Table (Temp Variable)
					DECLARE @OpenOrder TABLE (OpenOrderID INT, OpenOrderPrescribedDate DATETIME, PhysicianID INT, PatientID INT, EncounterID INT, DiagnosisCode NVARCHAR(300), StartDate DATETIME,
					EndDate DATETIME, CategoryId INT, SubCategoryId INT, OrderType NVARCHAR(100), OrderCode NVARCHAR(500), Quantity DECIMAL(18, 2), FrequencyCode NVARCHAR(100),
					PeriodDays NVARCHAR(20), OrderNotes NVARCHAR(500), OrderStatus NVARCHAR(500), IsActivitySchecduled BIT, ActivitySchecduledOn DATETIME, ItemName VARCHAR(100),
					ItemStrength VARCHAR(50), ItemDosage VARCHAR(50), IsActive BIT, CreatedBy INT, CreatedDate DATETIME, ModifiedBy INT, ModifiedDate DATETIME, IsDeleted BIT, DeletedBy INT,
					DeletedDate DATETIME, CorporateID INT, FacilityID INT, IsApproved BIT, EV1 NVARCHAR(100), EV2 NVARCHAR(100), EV3 NVARCHAR(100), EV4 NVARCHAR(100),
					OrderDescription VARCHAR(500), OrderTypeName VARCHAR(100))
				--------Open Order Table (Temp Variable)

					DECLARE @Facility_Id INT = (SELECT FacilityId FROM dbo.Physician WHERE Id = @PhysicianID)
					DECLARE @LocalDateTime DATETIME = (SELECT dbo.GetCurrentDatetimeByEntity(@Facility_Id))
					DECLARE @DateFrom DATETIME

					SET @MostRecentDays = isnull(@MostRecentDays, 0)

					IF @MostRecentDays = 0
					SET @MostRecentDays = 365
					SET @MostRecentDays = (@MostRecentDays * - 1)
					SET @DateFrom = DATEADD(day, @MostRecentDays, @LocalDateTime)


				-------------------FOR PREVIOUS ORDER
					DECLARE @OpenOrderCustom TABLE (TempOpenOrderID INT IDENTITY(1, 1), OpenOrderID INT, ActivityCode VARCHAR(20), OrderCode VARCHAR(20), OrderType VARCHAR(20), 
													OrderDescription VARCHAR(500), UserDefinedDescriptionID INT, OrderTypeName VARCHAR(20), UserDefinedDescprition NVARCHAR(500))

				--------------------FOR PREVIOUS ORDER

				--------------------FOR OPEN ORDER BY ENCOUNTER ID

					DECLARE @PharmacyCategoryid NVARCHAR(20) = '11100', @pOrderStatusGCC NVARCHAR(50) = '3102', @pFrequencyCodeGCC NVARCHAR(50) = '1024', @pOrderTypeGCC NVARCHAR(50) = '1201',
						@pLabTestResultSpecimenGCC NVARCHAR(50) = '3105', @EFId BIGINT = (SELECT TOP 1 EncounterFacility FROM Encounter WHERE EncounterID = @EncId) --Encounter Facility ID
				--------------------FOR OPEN ORDER BY ENCOUNTER ID


			------------$$$$$$$$$$$$$$$$$$$$$$$  DECLARATIONS    $$$$$$$$$$$$$$$$$$$$$$$---------------
	
	
				------------$$$$$$$$$$$$$$$$$$$$$$$   FILL OPEN ORDER TEMP VARIABLE    $$$$$$$$$$-----------
					INSERT INTO @OpenOrder
					SELECT OO.OpenOrderID, OO.OpenOrderPrescribedDate, OO.PhysicianID, OO.PatientID, OO.EncounterID, OO.DiagnosisCode, OO.StartDate, OO.EndDate, OO.CategoryId,
					OO.SubCategoryId, OO.OrderType, OO.OrderCode, OO.Quantity, OO.FrequencyCode, OO.PeriodDays, OO.OrderNotes, OO.OrderStatus, OO.IsActivitySchecduled,
					OO.ActivitySchecduledOn, OO.ItemName, OO.ItemStrength, OO.ItemDosage, OO.IsActive, OO.CreatedBy, OO.CreatedDate, OO.ModifiedBy, OO.ModifiedDate, OO.IsDeleted,
					OO.DeletedBy, OO.DeletedDate, OO.CorporateID, OO.FacilityID, OO.IsApproved, OO.EV1, OO.EV2, OO.EV3, OO.EV4, [dbo].[GetOrderDescription](OO.OrderType, OO.OrderCode),
					GC.GlobalCodeName FROM [dbo].[OpenOrder] OO
					INNER JOIN GlobalCodes GC ON GC.GlobalCodeValue = OO.OrderType AND GC.GlobalCodeCategoryValue = '1201'
			
				------------$$$$$$$$$$$$$$$$$$$$$$$   FILL OPEN ORDER TEMP VARIABLE    $$$$$$$$$$-----------


				------------$$$$$$$$$$$$$$$$$$$$$$$   QUERIES    $$$$$$$$$$-----------
		
					---------------------------------------Get Open Orders
						SELECT O.PhysicianID, COUNT(O.OpenOrderID) AS OrderCount, O.OrderType, O.OrderTypeName, O.OrderCode, O.OrderDescription
						FROM @OpenOrder o
						WHERE o.OpenOrderPrescribedDate >= @DateFrom AND  PhysicianID = @PhysicianID
						GROUP BY O.PhysicianID, O.OrderType, O.OrderTypeName, O.OrderCode, O.OrderDescription
					-----------------------------END OPEN ORDERS

					-----------------------------END PREVIOUS ORDERS
						INSERT INTO @OpenOrderCustom (OpenOrderID, ActivityCode, OrderCode, OrderType, OrderDescription)
						SELECT MAX(OO.OpenOrderId), OO.OrderTypeName, OO.OrderCode, OO.OrderType, OO.OrderDescription FROM @OpenOrder OO
						WHERE OO.CorporateID = @CId AND OO.FacilityID = @FId AND  PhysicianID = @PhysicianID
						GROUP BY OO.OrderCode, OO.OrderTypeName, OO.OrderType, OO.OrderDescription

						SELECT * FROM @OpenOrderCustom
					-----------------------------END PREVIOUS ORDERS
						DELETE FROM @OpenOrderCustom ---- Reuse of Table Variable

					-----------------------------FAVOURITE ORDERS
						INSERT INTO @OpenOrderCustom (UserDefinedDescriptionID, OrderTypeName, OrderCode, OrderType, OrderDescription, UserDefinedDescprition)
						SELECT A.UserDefinedDescriptionID, (SELECT TOP 1 (GlobalCodeName) FROM GlobalCodes WHERE GlobalCodeCategoryValue = 1201 AND GlobalCodeValue = A.CategoryId), 
						A.CodeId, A.CategoryId, (SELECT [dbo].[GetOrderDescription](A.CategoryId, A.CodeId) OrderDescription), A.UserDefineDescription	FROM 
						(
						SELECT DISTINCT CodeId, CategoryId, UserID, UserDefineDescription, UserDefinedDescriptionID	FROM UserDefinedDescriptions
						WHERE UserID = @PhysicianID AND CategoryId IN (3, 4, 5, 8)
						) A

						SELECT UserDefinedDescriptionID, OrderTypeName, OrderCode, OrderType, OrderDescription, UserDefinedDescprition	FROM @OpenOrderCustom

					-----------------------------END FAVOURITE ORDERS

					------------------------------------GET ORDER FROM ENCOUNTER ID
						SELECT OpenOrderID, OpenOrderPrescribedDate, PhysicianID, PatientID, EncounterID, DiagnosisCode, OrderType, CategoryId, SubCategoryId, OrderCode, Quantity,
						FrequencyCode, PeriodDays, OrderNotes, OrderStatus, IsActivitySchecduled, ItemName, ItemStrength, ItemDosage, IsActive, CreatedBy, CreatedDate, ModifiedBy,
						ModifiedDate, IsDeleted, DeletedBy, DeletedDate, StartDate, EndDate, IsApproved, dbo.[GetOrderDescription](OrderType, OrderCode) 'OrderDescription', 
						CASE WHEN CategoryId = @PharmacyCategoryid AND OrderStatus NOT IN ('3', '4', '9') AND IsApproved = 0	THEN 'Waiting For Approval'
						ELSE dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pOrderStatusGCC, OrderStatus)	END 'Status', 
						dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pFrequencyCodeGCC, FrequencyCode) 'FrequencyText',
						(SELECT TOP 1 GlobalCodeCategoryName FROM GlobalCodeCategory WHERE GlobalCodeCategoryValue = CategoryId AND FacilityNumber = @EFId) 'CategoryName',
						(SELECT TOP 1 GlobalCodeName FROM GlobalCodes WHERE GlobalCodeValue = CAST(SubCategoryId AS NVARCHAR) AND GlobalCodeCategoryValue = CategoryId 
						AND FacilityNumber = @EFId) 'SubCategoryName',
						dbo.GetGlobalCodeNameByCategoryAndCodeValue(@pOrderTypeGCC, OrderType) 'OrderTypeName', 
						(SELECT TOP 1 GlobalCodeName FROM GlobalCodes WHERE GlobalCodeCategoryValue = @pLabTestResultSpecimenGCC AND GlobalCodeValue = (SELECT TOP 1 LabTestResultSpecimen
						FROM [dbo].[LabTestResult] WHERE Cast(LabTestResultCPTCode AS VARCHAR) = OrderCode)) 'SpecimenTypeStr'
						FROM @OpenOrder
						WHERE EncounterId = @EncId
						ORDER BY 1 DESC
					-----------------------------------END GET ORDER FROM ENCOUNTER ID
					-----------------------------------Get All Order Activities by Encounter ID
						Exec SPROC_GetOrderActivitiesByEncounterId @EncId
					-----------------------------------END Get All Order Activities by Encounter ID

					-----------------------------------Get Future Orders Only by Patient ID
						Select @PatientId = PatientId From Encounter Where EncounterID = @EncId
						Exec SPROC_GetPaitentFutureOrder @PatientId				--FutureOpenOrderCustomModel
					-----------------------------------END Get Future Orders Only by Patient ID

					-------------------------------------Get Global Codes by GC Category ID

						Exec SPROC_GetGlobalCodesByCategories @GCCategoryCodes--'1024,3102,1011,2305,963'		--GlobalCodes
					-----------------------------------END Get Global Codes by GC Category ID
				------------$$$$$$$$$$$$$$$$$$$$$$$   QUERIES    $$$$$$$$$$-----------
	END
END