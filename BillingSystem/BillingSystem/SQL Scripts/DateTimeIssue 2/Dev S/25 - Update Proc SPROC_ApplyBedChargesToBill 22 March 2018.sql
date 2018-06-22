IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplyBedChargesToBill')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplyBedChargesToBill
GO

/****** Object:  StoredProcedure [dbo].[SPROC_ApplyBedChargesToBill]    Script Date: 22-03-2018 19:20:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_ApplyBedChargesToBill]
(  
@pEncounterID int  --- EncounterID for whom the Order need to be processed 
)
AS
BEGIN
			--- Declare Cursor Fetch Variables
		    DECLARE @Cur_OpenOrderID INT, @Cur_PhysicianID INT, @Cur_CorporateID INT,@Cur_FacilityID INT, @Cur_PatientID INT,@Cur_EncounterID INT,
					@Cur_PrescribedDate datetime, @Cur_OrderStartDate datetime, @Cur_OrderEndDate datetime,@Cur_Quantity numeric(18,2),
					@Cur_OrderType nvarchar(20),@Cur_OrderCode nvarchar(20),@Cur_DiagnosisCode nvarchar(20), @Cur_Gross numeric(18,2);

			--- Declare Other Variables
			DECLARE @UnitPrice numeric(18,2), @TotalPrice numeric(18,2) =0, @HeaderTotalPrice numeric(18,2) =0, @BillNumber nvarchar(50),
					@BillFormat nvarchar(20),@UpriceString nvarchar(20), @EncounterFlag bit = 0, @BillHeaderID INT, @BillDetailLineNumber INT = 0,
					@AuthID INT, @AuthType INT, @AuthCode nvarchar(50), @PayerID nvarchar(20), @MCMultiplier numeric (18,2),@MCPatientShare numeric (18,2),
					@SelfPayFlag bit, @BillActivityID INT,@EncounterPatientShareApplied numeric (18,2), @BalancePatientShare numeric (18,2), @PayerShareNet numeric (18,2),
					@pReClaimFlag varchar(2),@pClaimId bigint , @poPayerNETShare  numeric (18,2),@ClosedActivitiesCount int,
					@poMCDiscount  numeric (18,2)=0,@poHeaderMCDiscount  numeric (18,2)=0, @poIsMCContractBaseRateApplied bit,@poDRGCode nvarchar(20)
					,@ActivityCost numeric (18,2),@TotalActivityCost numeric (18,2);
			

			--Select @TimeZone=TimeZ from Facility Where FacilityId=(Select TOP 1 EncounterFacility From Encounter WHere EncounterID=@pEncounterID)

			DECLARE @CurrentDate datetime=(Select dbo.GetCurrentDatetimeByEntity(@Cur_FacilityID))

			Declare @IsInpatientBedrateApplied int;---Addded by Shashank on March 16 2015
			Declare @DRGToExecute int = 0 ;---Addded by Shashank on March 08 2016
			
			Set @pClaimId = (Select TOP(1) BillHeaderId from BillHeader where EncounterId = @pEncounterID order by BillHeaderId Desc )
			Set @pClaimId = ISNULL(@pClaimId,0);
			
			IF @pClaimId <> 0 
			BEGIN
			   Set @pReClaimFlag = '1';
			   SET @EncounterFlag = 1
			END
			-- Declare Cursor with Closed Order but not on any Bill
			
			DECLARE BTForBill CURSOR FOR
				Select BedChargesID,BCTransactionDate,1,BCCorporateID, BCFacilityID, BCPatientID, BCEncounterID, 8,ServiceCodeValue,BCRangeEffectiveDays,
						BCActivityStartDate, BCActivityEndDate,'8',BCGross  from BedCharges Where BCEncounterID = @pEncounterID and BCStatus = 0;

			--SELECT BT.BedTransactionID,MP.StartDate,1,BT.CorporateID,BT.FacilityID,BT.PatientID,BT.EncounterID,6,6,BT.EffectiveDays,MP.StartDate,MP.Enddate,'6',BT.BedCharges 
			--FROM BedTransaction BT
			--inner join MappingPatientBed MP on MP.MappingPatientBedID = BT.MappingPatientBedID
			--WHERE BT.EncounterID = @pEncounuterID and BT.ProcessStatus = 0 ; 
			
			
				OPEN BTForBill;  

				
				FETCH NEXT FROM BTForBill INTO @Cur_OpenOrderID,@Cur_PrescribedDate,@Cur_PhysicianID,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,
				@Cur_EncounterID,@Cur_OrderType,@Cur_OrderCode,@Cur_Quantity,@Cur_OrderStartDate,@Cur_OrderEndDate,@Cur_DiagnosisCode,@Cur_Gross;
							
				
				WHILE @@FETCH_STATUS = 0  
				BEGIN  
																				  
					If @EncounterFlag = 0 --- Meaning First time for Encounter - Insert into Header and get the newly generated BillHeaderID
					Begin
						----- XXXXXXXXXXXXXXXXXXXXXXXX----
					  -- Generate BIll Header - STARTS
						Set @EncounterFlag = 1
						----- GETSET - Bill Header Common PROC
						Exec [dbo].[SPROC_ApplyOrderToBillSetHeader] @pEncounterID,@BillHeaderID output,@BillDetailLineNumber output,@BillNumber output,
						@AuthID output,@AuthType output,@AuthCode output,@SelfPayFlag Output, @pReClaimFlag, @pClaimId
						
					End 
					ELSE
					BEGIN
						Select @BillHeaderID = BillHeaderId, @BillNumber= BillNumber from BillHeader where EncounterID = @pEncounterID and PatientID = @Cur_PatientID and FacilityID =  @Cur_FacilityID
						Exec [dbo].[SPROC_ApplyOrderToBillSetHeader] @pEncounterID,@BillHeaderID output,@BillDetailLineNumber output,@BillNumber output,
							@AuthID output,@AuthType output,@AuthCode output,@SelfPayFlag Output, @pReClaimFlag, @pClaimId
					END
						----- For Bed Charges to Apply and seen even if Multiplier is not set
						---- Set @MCMultiplier = isnull(@MCMultiplier,1)

					  ----- XXXXXXXXXXXXXXXXXXXXXXXX----
					  -- Calulate Amounts - - STARTS
						--- Based on OrderType Get Amount to be Charged
						---- 3=HCPCS, 4 = CPT, 5= DRIG , 9= DRGCodes -----> 8 = ServiceCodes
						    --Set @UnitPrice = [dbo].[GetUnitPrice] (@Cur_OrderType,@Cur_OrderCode,@pCorporateID,@pFacilityID)
							Set @UnitPrice = isnull(@Cur_Gross,0)
							Set @TotalPrice = @UnitPrice
							Set @HeaderTotalPrice = @HeaderTotalPrice + @TotalPrice
							----- Changed BB ---- 8Mar2016 -- New Column addded To Table and have to keep Same as Original Cost (ActivityCost) --- STARTS
							---- Declare New Variable and used that in Updating BillActivity and BillHeader Table 
							---- Also Updating new COlumn Unit Price in BillActivity
							Set @ActivityCost = (@UnitPrice * @Cur_Quantity)
							Set @ActivityCost = isnull(@ActivityCost,0)
							----- Changed BB ---- 8Mar2016 -- New Column addded To Table and have to keep Same as Original Cost (ActivityCost) --- ENDS

		
						 
	  ---- To Take Care of Balance Patient Share in Case MCPatientShare is more than Total Charges - STARTS

		--If @SelfPayFlag <> 1
		--Begin
			------ XXXXXXXXXXXXXXx ---- New MC Contract APPPLIED
			EXEC [dbo].[SPROC_ApplyMCContractToBillActivity]	@Cur_PatientID,@Cur_EncounterID,@Cur_OrderType,@Cur_OrderCode, @TotalPrice output,
																	@MCPatientShare output, @poPayerNETShare output, @EncounterPatientShareApplied output,
																	@poMCDiscount output, @poIsMCContractBaseRateApplied output,@SelfPayFlag,@poDRGCode output
			---- NULL are creating issues with not taking right effect for PatientShare and PayerShareNET So folloiwng commands to take care
					   Set @EncounterPatientShareApplied = isnull(@EncounterPatientShareApplied,0)
					  Set @MCPatientShare = isnull(@MCPatientShare,0)
					  Set @PayerShareNet = isnull(@PayerShareNet,0)
					  Set @TotalPrice = isnull(@TotalPrice,0)
					  Set @BalancePatientShare = isnull(@BalancePatientShare,0)
					  Set @HeaderTotalPrice =  isnull(@HeaderTotalPrice,0)

					  Set @poIsMCContractBaseRateApplied = Isnull(@poIsMCContractBaseRateApplied,0);

					  Set @BalancePatientShare = @MCPatientShare
					  Set @PayerShareNet = @poPayerNETShare
			--IF @poDRGCode <> '0'
			--BEGIN
			-- IF ((Select COUNT(1) From BillActivity Where ActivityType = 9 and ActivityCode = @poDRGCode and EncounterID = @Cur_EncounterID) = 0)
			-- BEGIN
			--	Insert into BillActivity (BillHeaderID,LineNumber,CorporateID,FacilityID,PatientID,EncounterID,DiagnosisID,DiagnosisType,DiagnosisCode,
			--								ActivityID,ActivityType,ActivityCode,ActivityStartDate,ActivityEndDate,QuantityOrdered,orderingClinicianID,OrderedDate,
			--								AdminstratingClinicianID,OrderCloseDate,AuthorizationID,AuthorizationCode,Gross,PatientShare,PayerShareNet ,Status,CreatedBy,CreatedDate,
			--								MCDiscount)
			--	Values(@BillHeaderID,@BillDetailLineNumber,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,0,'DRG TYPE',@Cur_DiagnosisCode,
			--								@Cur_OpenOrderID, '9',@poDRGCode,@Cur_OrderStartDate,@Cur_OrderEndDate,@Cur_Quantity,@Cur_PhysicianID,@Cur_PrescribedDate,
			--								1,@Cur_OrderEndDate,@AuthID,@AuthCode,@TotalPrice,@BalancePatientShare,@PayerShareNet,0,1,@CurrentDate,@poMCDiscount)
				
			--	Update BillActivity Set PatientShare = 0, PayerShareNet = 0 where ActivityType <> '9'  and BillHeaderID = @BillHeaderID
			--	Set @BalancePatientShare = 0
			--	Set @PayerShareNet = 0
			-- END
			-- ELSE
			-- BEGIN
			--	Update BillActivity Set Gross=@TotalPrice,PatientShare =@BalancePatientShare, PayerShareNet = @PayerShareNet where ActivityType ='9' and BillHeaderID = @BillHeaderID
			--	Update BillActivity Set PatientShare = 0, PayerShareNet = 0 where ActivityType <> '9'  and BillHeaderID = @BillHeaderID
			--	Set @BalancePatientShare = 0
			--	Set @PayerShareNet = 0
			-- END
			--END
			/*
			if (@TotalPrice + @EncounterPatientShareApplied) < @MCPatientShare
			Begin
				Set @BalancePatientShare = @TotalPrice
				Set @PayerShareNet = 0
			End
			ELSE
			Begin

				if @EncounterPatientShareApplied < @MCPatientShare
				Begin
					Set @BalancePatientShare = @MCPatientShare - @EncounterPatientShareApplied
					Set @PayerShareNet = @TotalPrice - @BalancePatientShare
				End
				ELSE 
				Begin
					Set @BalancePatientShare = 0
					Set @PayerShareNet = @TotalPrice
				End
				-- Set @BalancePatientShare = isnull(@MCPatientShare,0) - (isnull(@EncounterPatientShareApplied,0)+isnull(@HeaderTotalPrice,0))
				--Set @BalancePatientShare = 0
			End

			If @BalancePatientShare <= 0
				Set @BalancePatientShare = 0
			*/
		--End --- Check of Self Pay Ends Here
		--ELSE
		--Begin  
		--		--- This a Self Pay Flag so Set Shares Accrodingly
		--		Set @BalancePatientShare = @TotalPrice
		--		Set @PayerShareNet = 0
		--End

			 ---- To Take Care of Balance Patient Share in Case MCPatientShare is more than Total Charges - ENDS 
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
			-- Commentedby Shashank To Show tha acitivites with 0 price on bill
			--If @TotalPrice > 0 
			--Begin
					  ----- XXXXXXXXXXXXXXXXXXXXXXXX----
					  -- Generate BillDetails/Activity - STARTS
						Set @BillDetailLineNumber = @BillDetailLineNumber + 1;
						-- CUstom value to fix the Rule Actviity start date should be greater than the Bill Header Transaction date.
						-- Added by Shashank on 29 March 2016
						Declare @BillHeaderTransactionDate datetime = (Select BillDate from Billheader where BillheaderId = @BillHeaderID)
						Set @BillHeaderTransactionDate = DATEADD(MINUTE,1,@BillHeaderTransactionDate)
						-- Old Code Commented
						--Insert into BillActivity (BillHeaderID,LineNumber,CorporateID,FacilityID,PatientID,EncounterID,DiagnosisID,DiagnosisType,DiagnosisCode,
						--				ActivityID,ActivityType,ActivityCode,ActivityStartDate,ActivityEndDate,QuantityOrdered,orderingClinicianID,OrderedDate,
						--				AdminstratingClinicianID,OrderCloseDate,AuthorizationID,AuthorizationCode,Gross,PatientShare,PayerShareNet ,[Status],CreatedBy,CreatedDate,
						--				MCDiscount,ActivityCost,UnitCost)
						--	Values(@BillHeaderID,@BillDetailLineNumber,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,0,'BEDDiagType',@Cur_DiagnosisCode,
						--				@Cur_OpenOrderID, @Cur_OrderType,@Cur_OrderCode,@Cur_OrderStartDate,@Cur_OrderEndDate,@Cur_Quantity,@Cur_PhysicianID,@Cur_PrescribedDate,
						--				1,@Cur_OrderEndDate,@AuthID,@AuthCode,@TotalPrice,@BalancePatientShare,@PayerShareNet,0,1,@CurrentDate,@poMCDiscount,
						--				@ActivityCost,@UnitPrice)
						--- Removed the line --- ISNULL(@BillHeaderTransactionDate,@Cur_OrderEndDate)
						Insert into BillActivity (BillHeaderID,LineNumber,CorporateID,FacilityID,PatientID,EncounterID,DiagnosisID,DiagnosisType,DiagnosisCode,
										ActivityID,ActivityType,ActivityCode,ActivityStartDate,ActivityEndDate,QuantityOrdered,orderingClinicianID,OrderedDate,
										AdminstratingClinicianID,OrderCloseDate,AuthorizationID,AuthorizationCode,Gross,PatientShare,PayerShareNet ,[Status],CreatedBy,CreatedDate,
										MCDiscount,ActivityCost,UnitCost,XActivityParsedId)
							Values(@BillHeaderID,@BillDetailLineNumber,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,0,'BEDDiagType',@Cur_DiagnosisCode,
										@Cur_OpenOrderID, @Cur_OrderType,@Cur_OrderCode,@Cur_OrderEndDate,@Cur_OrderEndDate,@Cur_Quantity,@Cur_PhysicianID,@Cur_PrescribedDate,
										1,@Cur_OrderEndDate,@AuthID,@AuthCode,@TotalPrice,@BalancePatientShare,@PayerShareNet,0,1,@CurrentDate,@poMCDiscount,
										@ActivityCost,@UnitPrice,@ActivityCountUnderClaimCustom)

						if @Cur_OrderType = 9
							SET @DRGToExecute = 1
					  -- Generate BillDetails/Activity - STARTS
				--END --- Not on BillActivity Table - So Insert - ENDS					  		 
					 ----- XXXXXXXXXXXXXXXXXXXXXXXX----
					  -- Update BedCharges to state that it is on Bill now - Meaning Status Update to 4 - On Bill - STARTS
					    Update BedCharges Set BCStatus = 4 Where BedChargesID = @Cur_OpenOrderID;
			--End		
				--- Fetch Next Record/Order from Cursor
				FETCH NEXT FROM BTForBill INTO @Cur_OpenOrderID,@Cur_PrescribedDate,@Cur_PhysicianID,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,
				@Cur_EncounterID,@Cur_OrderType,@Cur_OrderCode,@Cur_Quantity,@Cur_OrderStartDate,@Cur_OrderEndDate,@Cur_DiagnosisCode,@Cur_Gross;

			END  --END OF @@FETCH_STATUS = 0  


			CLOSE BTForBill;  
			DEALLOCATE BTForBill; 

			----XXXXXX---- Busines Logic
			--- Patient Share should be Applied per Encounter Level - NOTE: There can be many bills for same Encounter
			---- So Sum all Patient Share for current Encounter and then Apply Patien Share accordingly 
		
		--Select @poIsMCContractBaseRateApplied;
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
			
			SET @DRGToExecute = ISNULL(@DRGToExecute,0)
			---This is to perform the DRG calcualtion right as these were wrong (Patient share was not being deducted on Drg amount)
			IF @poDRGCode <> '0' and @SelfPayFlag = 0 
			BEGIN
				IF @DRGToExecute = 1
					SET @TotalPayerNETShare = @TotalPayerNETShare - @TotalPatientShare

				Update BillActivity Set PayerShareNet = @TotalPayerNETShare ,PatientShare =@TotalPatientShare,
				Gross = (ISNULL(@TotalPayerNETShare,0.00) +ISNULL(@TotalPatientShare,0.00))  where ActivityType ='9' and BillHeaderID = @BillHeaderID
				Update BillActivity Set PatientShare = 0,Gross = 0  where ActivityType <> '9' and EncounterID = @pEncounterID  and BillHeaderID = @BillHeaderID
			END

			---- Now Update Total Header Price
			---- BB 8Mar2016 ActivityCost Added new Column and Updated the same
				Update BillHeader Set BillNumber = @BillNumber,Gross = Isnull(@HeaderTotalPrice,0),PatientShare = isnull(@TotalPatientShare,0),MCDiscount = @poHeaderMCDiscount,
				PayerShareNet = isnull(@TotalPayerNETShare,0), ActivityCost = @TotalActivityCost Where BillheaderID = @BillHeaderID;

				Select @poIsMCContractBaseRateApplied;
			--- Update New Charges to Encounter 
			--Update Encounter Set Charges = (isnull(Charges,0)+isnull(@HeaderTotalPrice,0)), IsMCContractBaseRateApplied =@poIsMCContractBaseRateApplied  Where EncounterID = @pEncounterID;
			Update Encounter Set Charges = isnull(@TotalPatientShare,0) + isnull(@TotalPayerNETShare,0), IsMCContractBaseRateApplied =@poIsMCContractBaseRateApplied  Where EncounterID = @pEncounterID;
		
		END --- Check for Header Updates ENDS
END













GO


