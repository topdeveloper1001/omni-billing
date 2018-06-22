IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplyOrderActivityToBill')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplyOrderActivityToBill
GO

/****** Object:  StoredProcedure [dbo].[SPROC_ApplyOrderActivityToBill]    Script Date: 20-03-2018 16:14:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_ApplyOrderActivityToBill] -- [SPROC_ApplyOrderActivityToBill] 6,4,2252,'',0
(  
@pCorporateID int=1019,
@pFacilityID int=1051,
@pEncounterID int=2152,  ---EncounterID for whom the Order need to be processed 
@pReClaimFlag varchar(2)='',
@pClaimId bigint = 0
)
AS
BEGIN
		DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

			--- Declare Cursor Fetch Variables
		    DECLARE @Cur_OpenOrderID INT, @Cur_PhysicianID INT, @Cur_CorporateID INT,@Cur_FacilityID INT, @Cur_PatientID INT,@Cur_EncounterID INT,
					@Cur_PrescribedDate datetime, @Cur_OrderStartDate datetime, @Cur_OrderEndDate datetime,@Cur_Quantity numeric(18,2),@Cur_OrderedQuantity numeric(18,2),
					@Cur_OrderType nvarchar(20),@Cur_OrderCode nvarchar(20),@Cur_DiagnosisCode nvarchar(20), @Cur_OrderActivityID INT;
			
			--- Declare Other Variables
			DECLARE @CurrentDate DATETIME = @LocalDateTime, @UnitPrice numeric(18,2), @TotalPrice numeric(18,2) =0, @HeaderTotalPrice numeric(18,2) =0, @BillNumber nvarchar(50),
					@BillFormat nvarchar(20),@UpriceString nvarchar(20), @EncounterFlag bit = 0, @BillHeaderID INT, @BillDetailLineNumber INT = 0,
					@AuthID INT, @AuthType INT, @AuthCode nvarchar(50), @PayerID nvarchar(20), @MCMultiplier numeric (18,2),@MCPatientShare numeric (18,2),
					@SelfPayFlag bit,@EncounterPatientShareApplied numeric (18,2), @BalancePatientShare numeric (18,2), @PayerShareNet numeric (18,2),
					@poPayerNETShare  numeric (18,2),@ClosedActivitiesCount int,@poMCDiscount  numeric (18,2)=0,@poHeaderMCDiscount  numeric (18,2)=0, @poIsMCContractBaseRateApplied bit 
					,@poDRGCode nvarchar(20),@ActivityCost numeric (18,2),@TotalActivityCost numeric (18,2);
			
			Declare @Cur_QuantityDiff numeric(18,2)=0.00,@Cur_OrderActivityID_1 int
			Declare @DRGToExecute int = 0 ;---Addded by Shashank on March 08 2016
			-- Declare Cursor with Closed Order but not on any Bill
			
			DECLARE OrdersForBill CURSOR FOR
			Select OrderID,OrderScheduleDate,Orderby,CorporateID,FacilityID,PatientID,EncounterID,OrderType,OrderCode,
			ExecutedQuantity,OrderScheduleDate,ExecutedDate,OrderActivityID,OrderActivityQuantity
			from [dbo].[OrderActivity]
			Where CorporateID = @pCorporateID and FacilityID = @pFacilityID and EncounterID = @pEncounterID and OrderActivityStatus in (2,3) and ExecutedDate is not null 
			
			
				OPEN OrdersForBill;  

				FETCH NEXT FROM OrdersForBill INTO @Cur_OpenOrderID,@Cur_PrescribedDate,@Cur_PhysicianID,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,
				@Cur_EncounterID,@Cur_OrderType,@Cur_OrderCode,@Cur_Quantity,@Cur_OrderStartDate,@Cur_OrderEndDate,@Cur_OrderActivityID,@Cur_OrderedQuantity;
				
								
				WHILE @@FETCH_STATUS = 0  
				BEGIN  
				
				Set @Cur_Quantity = isnull(@Cur_Quantity,@Cur_OrderedQuantity)
					--IF(@Cur_Quantity != @Cur_OrderedQuantity)
					--BEGIN
					--	Set  @Cur_QuantityDiff = @Cur_OrderedQuantity - @Cur_Quantity;
					--	INSERT INTO OrderActivity (OrderID, OrderCode,OrderScheduleDate,PlannedDate,OrderActivityStatus,IsActive, CreatedDate, CreatedBy )
					--	SELECT @Cur_OpenOrderID,@Cur_OrderCode,@Cur_PrescribedDate AS ActivitySchecduleOn,@Cur_PrescribedDate AS PlanDate,1 AS ActitvityStatus, 1 AS IsActive,@CurrentDate AS CreatedDate, 1 AS CreatedBy  

					--	Set @Cur_OrderActivityID_1= (SELECT SCOPE_IDENTITY())

					--	update OOAS	set	OOAS.OrderType = OO.OrderType,OOAS.OrderCategoryID = OO.CategoryID,OOAS.OrderSubCategoryID = OO.SubCategoryID
					--		,OOAS.OrderActivityQuantity = @Cur_QuantityDiff,OOAS.PatientID = OO.PatientID,OOAS.EncounterID = OO.EncounterID,OOAS.OrderBy = OO.PhysicianID
					--		,OOAS.PlannedBy = OO.PhysicianID,OOAS.PlannedDate = OOAS.OrderScheduleDate,OOAS.CorporateID = OO.CorporateID,OOAS.FacilityID = OO.FacilityID,
					--		OOAS.Comments = 'Partially Executed Order'
					--		from OpenOrder OO
					--	inner join OrderActivity OOAS on OO.OpenOrderID = OOAS.OrderID 
					--	Where  OOAS.OrderActivityID = @Cur_OrderActivityID_1 
					--END
					----- Get Patient Share Already Applied for this Encounter till Now
					-- Set @EncounterPatientShareApplied = (Select sum(isnull(PatientShare,0)) from BillActivity Where EncounterID = @pEncounterID);
										  
					If @EncounterFlag = 0 --- Meaning First time for Encounter - Insert into Header and get the newly generated BillHeaderID
					Begin
						----- XXXXXXXXXXXXXXXXXXXXXXXX----
					  -- Gernerate BIll Header - STARTS
						Set @EncounterFlag = 1
						--print 'Apply Bill Header Check'
							----- GETSET - Bill Header Common PROC
							Exec [dbo].[SPROC_ApplyOrderToBillSetHeader] @pEncounterID,@BillHeaderID output,@BillDetailLineNumber output,@BillNumber output,
							@AuthID output,@AuthType output,@AuthCode output,@SelfPayFlag Output, @pReClaimFlag, @pClaimId
						
					End 
					ELSE
					BEGIN
						--print 'Update Bill Header Check'
						Select @BillHeaderID = BillHeaderId, @BillNumber= BillNumber from BillHeader where EncounterID = @pEncounterID and PatientID = @Cur_PatientID and FacilityID =  @pFacilityID
						Exec [dbo].[SPROC_ApplyOrderToBillSetHeader] @pEncounterID,@BillHeaderID output,@BillDetailLineNumber output,@BillNumber output,
							@AuthID output,@AuthType output,@AuthCode output,@SelfPayFlag Output, @pReClaimFlag, @pClaimId
					END
					 ----- XXXXXXXXXXXXXXXXXXXXXXXX----
					  -- Calulate Amounts - - STARTS
						--- Based on OrderType Get Amount to be Charged
						---- 3=HCPCS, 4 = CPT, 5= DRIG , 9= DRGCodes -----> 8 = ServiceCodes
						    --Set @UnitPrice = [dbo].[GetUnitPrice] (@Cur_OrderType,@Cur_OrderCode)--- Commented By Shashank on Sept 5 2015
							
							/*
							Here, in function [GetUnitPrice], added one more input parameter that will get the Code Price based on that activity Date.
							By Amit Jain on 26 Jan, 2016
							*/
							Set @UnitPrice = [dbo].[GetUnitPrice] (@Cur_OrderType,@Cur_OrderCode,@pCorporateID,@pFacilityID,@Cur_OrderEndDate)
							Set @TotalPrice = (@UnitPrice * @Cur_Quantity)
							Set @HeaderTotalPrice = @HeaderTotalPrice + @TotalPrice
							----- Changed BB ---- 8Mar2016 -- New Column addded To Table and have to keep Same as Original Cost (ActivityCost) --- STARTS
							---- Declare New Variable and used that in Updating BillActivity and BillHeader Table 
							---- Also Updating new COlumn Unit Price in BillActivity
							Set @ActivityCost = (@UnitPrice * @Cur_Quantity)
							Set @ActivityCost = isnull(@ActivityCost,0)
							----- Changed BB ---- 8Mar2016 -- New Column addded To Table and have to keep Same as Original Cost (ActivityCost) --- ENDS

							---- Code is added to put the value of the X Activity ID
							/*
							WHO: Shashank Awasthy           
							WHEN: April 7 2016
							WHAT: Add the column X activity Id which is combination of the BillHeader id and Count +1 for the BillActivity Under that Bill
							WHY: to Fix the issue of the External XML remiitance upload
							*/
							Declare @ActivityCountUnderClaim int = (Select Count(BillActivityId) From  BillActivity where BillheaderId = @BillHeaderID)
							Set @ActivityCountUnderClaim = @ActivityCountUnderClaim + 1;
							Declare @ActivityCountUnderClaimCustom nvarchar(20) = CASE WHEN LEN(@ActivityCountUnderClaim) > 1 
										Then Cast(@BillHeaderID  as Nvarchar(20))+'-'+ cast(@ActivityCountUnderClaim as nvarchar(19)) ELSE
											Cast(@BillHeaderID  as Nvarchar(20)) +'-'+'0'+ CAST(@ActivityCountUnderClaim as nvarchar(19)) END
					  -- Calulate Amounts - - ENDS
					  
					  
					  ----- Calculate PatientShare - STARTS

					   ---- To Take Care of Balance Patient Share in Case MCPatientShare is more than Total Charges - STARTS
		--- Need to remove this check IF Selfpay flag is true then have to Get the (IP/ER/OP) mutiplier and Mutiply the HAAD Base rate with Multiplier
		--If @SelfPayFlag <> 1
		--Begin
			--Set @poDRGCode =0
			------ XXXXXXXXXXXXXXx ---- New MC Contract APPPLIED
			--print 'MC CONTRACT '
			--Select @Cur_PatientID,@Cur_EncounterID,@Cur_OrderType,@Cur_OrderCode
			EXEC [dbo].[SPROC_ApplyMCContractToBillActivity] @Cur_PatientID,@Cur_EncounterID,@Cur_OrderType,@Cur_OrderCode, @TotalPrice output,
																	@MCPatientShare output, @poPayerNETShare output, @EncounterPatientShareApplied output,
																	@poMCDiscount output, @poIsMCContractBaseRateApplied output,@SelfPayFlag,@poDRGCode output
			--print 'MC CONTRACT Apllied'
			---- NULL are creating issues with not taking right effect for PatientShare and PayerShareNET So folloiwng commands to take care
					  Set @EncounterPatientShareApplied = isnull(@EncounterPatientShareApplied,0)
					  Set @MCPatientShare = isnull(@MCPatientShare,0)
					  Set @PayerShareNet = isnull(@PayerShareNet,0)
					  Set @TotalPrice = isnull(@TotalPrice,0)
					  Set @BalancePatientShare = isnull(@BalancePatientShare,0)
					  Set @HeaderTotalPrice =  isnull(@HeaderTotalPrice,0)
		

				Set @BalancePatientShare = @MCPatientShare
				Set @PayerShareNet = @poPayerNETShare

			--IF @SelfPayFlag = 0
			--BEGIN
			--IF @poDRGCode <> '0'
			--BEGIN
			----- Check if this is first entry for DRG in the bill for Encounter 
			---- If yes Add the Activity to Bill and Update the Payer Share Net for the bill actviity to 0
			-- IF ((Select COUNT(*) From BillActivity Where ActivityType = 9 and EncounterID = @Cur_EncounterID) = 0)
			-- BEGIN
			--	Insert into BillActivity (BillHeaderID,LineNumber,CorporateID,FacilityID,PatientID,EncounterID,DiagnosisID,DiagnosisType,DiagnosisCode,
			--								ActivityID,ActivityType,ActivityCode,ActivityStartDate,ActivityEndDate,QuantityOrdered,orderingClinicianID,OrderedDate,
			--								AdminstratingClinicianID,OrderCloseDate,AuthorizationID,AuthorizationCode,Gross,PatientShare,PayerShareNet ,Status,CreatedBy,CreatedDate,
			--								MCDiscount)
			--	Values(@BillHeaderID,@BillDetailLineNumber,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,0,'DRG TYPE',@Cur_DiagnosisCode,
			--								@Cur_OpenOrderID, '9',@poDRGCode,@Cur_OrderStartDate,@Cur_OrderEndDate,@Cur_Quantity,@Cur_PhysicianID,@Cur_PrescribedDate,
			--								1,@Cur_OrderEndDate,@AuthID,@AuthCode,0,@MCPatientShare,@PayerShareNet,0,1,@CurrentDate,@poMCDiscount)
			--	-- Removing the Patient Share to check For DRG cases.
			--	--Update BillActivity Set PatientShare = 0, PayerShareNet = 0 where ActivityType <> '9'  and BillHeaderID = @BillHeaderID
			--	Update BillActivity Set PayerShareNet = 0 where ActivityType <> '9'  and BillHeaderID = @BillHeaderID
			--	Set @BalancePatientShare = 0
			--	Set @PayerShareNet = 0
			-- END
			-- ELSE
			-- BEGIN
			-- --- Check if the Order type is not 9 means not a DRG Then allow to Enter the Bill Activity
			-- if @Cur_OrderType <> '9'
			-- BEGIN
			-- -- Allow Activity to go to Bill activity and update the Activites from the 
			--	Insert into BillActivity (BillHeaderID,LineNumber,CorporateID,FacilityID,PatientID,EncounterID,DiagnosisID,DiagnosisType,DiagnosisCode,
			--							ActivityID,ActivityType,ActivityCode,ActivityStartDate,ActivityEndDate,QuantityOrdered,orderingClinicianID,OrderedDate,
			--							AdminstratingClinicianID,OrderCloseDate,AuthorizationID,AuthorizationCode,Gross,PatientShare,PayerShareNet ,Status,CreatedBy,CreatedDate,
			--							MCDiscount)
			--	Values(@BillHeaderID,@BillDetailLineNumber,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,0,'DiagType',@Cur_DiagnosisCode,
			--							@Cur_OpenOrderID, @Cur_OrderType,@Cur_OrderCode,@Cur_OrderStartDate,@Cur_OrderEndDate,@Cur_Quantity,@Cur_PhysicianID,@Cur_PrescribedDate,
			--							1,@Cur_OrderEndDate,@AuthID,@AuthCode,@TotalPrice,@MCPatientShare,@PayerShareNet,0,1,@CurrentDate,@poMCDiscount)
			--  END
			--	--Update BillActivity Set ActivityCode = @poDRGCode ,Gross=0,PatientShare =@BalancePatientShare, PayerShareNet = @PayerShareNet where ActivityType ='9' and BillHeaderID = @BillHeaderID
			--	Update BillActivity Set ActivityCode = @poDRGCode ,Gross=0, PayerShareNet = @PayerShareNet where ActivityType ='9' and BillHeaderID = @BillHeaderID
			--	--Update BillActivity Set PatientShare = 0, PayerShareNet = 0 where ActivityType <> '9' and BillHeaderID = @BillHeaderID 
			--	Update BillActivity Set PayerShareNet = 0 where ActivityType <> '9' and EncounterID = @Cur_EncounterID 
			--	Set @BalancePatientShare = 0
			--	Set @PayerShareNet = 0
			-- END
			--END
			--END

			-- *********** Commented If Statement by Shashank to Allow the Activity to go to bill if it has 0 tatal price: to keep track ************
			--If @TotalPrice > 0 
			--Begin
					  ----- XXXXXXXXXXXXXXXXXXXXXXXX----
					  -- Generate BillDetails/Activity - STARTS
					  --IF @poDRGCode = '0'
					  --BEGIN

					  ---- BB 8Mar2016 ActivityCost,UnitCost Added new Columns and Inserted the same
						Set @BillDetailLineNumber = @BillDetailLineNumber + 1;
						--Insert into BillActivity (BillHeaderID,LineNumber,CorporateID,FacilityID,PatientID,EncounterID,DiagnosisID,DiagnosisType,DiagnosisCode,
						--				ActivityID,ActivityType,ActivityCode,ActivityStartDate,ActivityEndDate,QuantityOrdered,orderingClinicianID,OrderedDate,
						--				AdminstratingClinicianID,OrderCloseDate,AuthorizationID,AuthorizationCode,Gross,PatientShare,PayerShareNet ,Status,CreatedBy,CreatedDate,
						--				MCDiscount,ActivityCost,UnitCost)
						--	Values(@BillHeaderID,@BillDetailLineNumber,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,0,'DiagType',@Cur_DiagnosisCode,
						--				@Cur_OrderActivityID, @Cur_OrderType,@Cur_OrderCode,@Cur_OrderStartDate,@Cur_OrderEndDate,@Cur_Quantity,@Cur_PhysicianID,@Cur_PrescribedDate,
						--				1,@Cur_OrderEndDate,@AuthID,@AuthCode,@TotalPrice,@MCPatientShare,@PayerShareNet,0,1,@CurrentDate,@poMCDiscount,
						--				@ActivityCost,@UnitPrice)

						--What: Changed the query of insert now changed the Activity start date from the OrderActivityStartdate to OrderactivityendDate
						--WHY: Changed is done to make the rule work ,, Rule states Billactivity start date >= BillHeaderStartDate.
						--WHO: Shashank
						--WHEN: 17th March 2016
						Insert into BillActivity (BillHeaderID,LineNumber,CorporateID,FacilityID,PatientID,EncounterID,DiagnosisID,DiagnosisType,DiagnosisCode,
										ActivityID,ActivityType,ActivityCode,ActivityStartDate,ActivityEndDate,QuantityOrdered,orderingClinicianID,OrderedDate,
										AdminstratingClinicianID,OrderCloseDate,AuthorizationID,AuthorizationCode,Gross,PatientShare,PayerShareNet ,Status,CreatedBy,CreatedDate,
										MCDiscount,ActivityCost,UnitCost,XActivityParsedId)
							Values(@BillHeaderID,@BillDetailLineNumber,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,0,'DiagType',@Cur_DiagnosisCode,
										@Cur_OrderActivityID, @Cur_OrderType,@Cur_OrderCode,@Cur_OrderEndDate,@Cur_OrderEndDate,@Cur_Quantity,@Cur_PhysicianID,@Cur_PrescribedDate,
										1,@Cur_OrderEndDate,@AuthID,@AuthCode,@TotalPrice,@MCPatientShare,@PayerShareNet,0,1,@CurrentDate,@poMCDiscount,
										@ActivityCost,@UnitPrice,@ActivityCountUnderClaimCustom)
					  
					  if @Cur_OrderType = '9'
							SET @DRGToExecute = 1
					  --END
					  -- Generate BillDetails/Activity - STARTS
					  		 
					 ----- XXXXXXXXXXXXXXXXXXXXXXXX----
					  -- Update OpenOrder to state that it is on Bill now - Meaning Status Update to 4 - On Bill - STARTS
					  Update OrderActivity Set OrderActivityStatus = 4,OrderActivityQuantity = @Cur_Quantity  Where OrderActivityID = @Cur_OrderActivityID;
					  --Update OrderActivity Set OrderActivityStatus = 4  Where OrderActivityID = @Cur_OrderActivityID;
					  ---- BB - 30-Jan-2015 -- New Logic Needed if All Scheduled Activities are on Bill Set related OpenOrder also on Bill - STARTS
						Set @ClosedActivitiesCount = 0;
						Select @ClosedActivitiesCount = Count(1) from OrderActivity Where OrderID = @Cur_OpenOrderID and (OrderActivityStatus < 4 and OrderActivityStatus <> 40);
						Set @ClosedActivitiesCount = isnull(@ClosedActivitiesCount,0);
						If @ClosedActivitiesCount = 0 
							Update OpenOrder Set OrderStatus = '4' Where OpenOrderID = @Cur_OpenOrderID;
					  ---- BB - 30-Jan-2015 -- New Logic Needed if All Scheduled Activities are on Bill Set related OpenOrder also on Bill - ENDS
					   
			--End
			-- *********** Commented If Statement by Shashank to Allow the Activity to go to bill if it has 0 tatal price: to keep track ************
				--- Fetch Next Record/Order from Cursor
				FETCH NEXT FROM OrdersForBill INTO @Cur_OpenOrderID,@Cur_PrescribedDate,@Cur_PhysicianID,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,
				@Cur_EncounterID,@Cur_OrderType,@Cur_OrderCode,@Cur_Quantity,@Cur_OrderStartDate,@Cur_OrderEndDate,@Cur_OrderActivityID,@Cur_OrderedQuantity;
							
				END  --END OF @@FETCH_STATUS = 0  


			CLOSE OrdersForBill;  
			DEALLOCATE OrdersForBill; 

			----XXXXXX---- Busines Logic
			--- Patient Share should be Applied per Encounter Level - NOTE: There can be many bills for same Encounter
			---- So Sum all Patient Share for current Encounter and then Apply Patien Share accordingly 
		
		-- Needed Following Check because if there are no rows Selected in Cursor then there is No point doing updates/calulcations on Header and Encounter
		If @EncounterFlag = 1
		BEGIN
				
			---- Update Bill Header and Encounter
			declare @TotalPatientShare numeric (18,2), @TotalPayerNETShare numeric (18,2)

			--- Check for the DRGCODE and Patient is inpatient Type
			IF @poDRGCode <> '0' and @SelfPayFlag = 0
			BEGIN

				---- BB 8Mar2016 -- Commented below Gross = 0 and instead because now ActivityCost will be the one Holding Data for the same
				--------   Update BillActivity Set Gross=0 where ActivityType ='9' and BillHeaderID = @BillHeaderID
				Update BillActivity Set ActivityCost=0 where ActivityType ='9' and BillHeaderID = @BillHeaderID
				Update BillActivity Set PayerShareNet = 0 where ActivityType <> '9' and EncounterID = @pEncounterID  and BillHeaderID = @BillHeaderID
			END

			---- BB 8Mar2016 -- New Column ActivityCost totaled below for Update in BillHeader
			Select @HeaderTotalPrice = sum(gross),@TotalPatientShare = sum(PatientShare),@TotalPayerNETShare = sum(PayerShareNet), @poHeaderMCDiscount = sum(MCDiscount),
			@TotalActivityCost = sum(ActivityCost)  
			from BillActivity Where BillHeaderID = @BillHeaderID;
			
			---This is to perform the DRG calcualtion right as these were wrong (Patient share was not being deducted on Drg amount)
			IF @poDRGCode <> '0' and @SelfPayFlag = 0
			BEGIN
				IF @DRGToExecute = 1
					SET @TotalPayerNETShare = @TotalPayerNETShare - @TotalPatientShare
				--SET @TotalPayerNETShare = @TotalPayerNETShare - @TotalPatientShare
				Update BillActivity Set PayerShareNet =@TotalPayerNETShare ,PatientShare =@TotalPatientShare,Gross = (ISNULL(@TotalPayerNETShare,0.00) +ISNULL(@TotalPatientShare,0.00))  where ActivityType ='9' and BillHeaderID = @BillHeaderID
				Update BillActivity Set PatientShare = 0,Gross = 0  where ActivityType <> '9' and EncounterID = @pEncounterID  and BillHeaderID = @BillHeaderID
			END

			---- Now Update Total Header Price
			---- BB 8Mar2016 ActivityCost Added new Column and Updated the same
				Update BillHeader Set BillNumber = @BillNumber,Gross = Isnull(@HeaderTotalPrice,0),PatientShare = isnull(@TotalPatientShare,0),
				MCDiscount = isnull(@poHeaderMCDiscount,0),PayerShareNet = isnull(@TotalPayerNETShare,0), ActivityCost = @TotalActivityCost  Where BillheaderID = @BillHeaderID;

			--- Update New Charges to Encounter 
			--Update Encounter Set Charges = (isnull(Charges,0)+@HeaderTotalPrice), IsMCContractBaseRateApplied =@poIsMCContractBaseRateApplied  Where EncounterID = @pEncounterID;
			-- commented this line to Change the Charges for the Encounter as it have to show the same data in All screens
			--Update Encounter Set Charges = (@HeaderTotalPrice), IsMCContractBaseRateApplied =@poIsMCContractBaseRateApplied  Where EncounterID = @pEncounterID;
			Update Encounter Set Charges = isnull(@TotalPatientShare,0) + isnull(@TotalPayerNETShare,0), IsMCContractBaseRateApplied =@poIsMCContractBaseRateApplied  Where EncounterID = @pEncounterID;

			 			
			
		END ---- Check for if HeaderUpdates needed ENDS
END





GO


