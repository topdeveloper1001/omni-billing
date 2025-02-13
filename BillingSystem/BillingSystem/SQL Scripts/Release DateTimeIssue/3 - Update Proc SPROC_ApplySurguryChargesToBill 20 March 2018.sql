IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplySurguryChargesToBill')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplySurguryChargesToBill
GO
/****** Object:  StoredProcedure [dbo].[SPROC_ApplySurguryChargesToBill]    Script Date: 20-03-2018 15:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_ApplySurguryChargesToBill]-- [SPROC_ApplySurguryChargesToBill] 9,8,1096,'',0
(	
	@pCorporateID int,
	@pFacilityID int,
	@pEncounterID int,  --- EncounterID for whom the Order need to be processed 
	@pReClaimFlag varchar(2),
	@pClaimId bigint
)
AS
BEGIN
	--- Declare Cursor Fetch Variables
DECLARE @Cur_OpenOrderID INT, @Cur_PhysicianID INT, @Cur_CorporateID INT,@Cur_FacilityID INT, @Cur_PatientID INT,@Cur_EncounterID INT,
		@Cur_PrescribedDate datetime, @Cur_OrderStartDate datetime, @Cur_OrderEndDate datetime,@Cur_Quantity numeric(18,2),@Cur_OrderedQuantity numeric(18,2),
		@Cur_OrderType nvarchar(20),@Cur_OrderCode nvarchar(20), @Cur_OrderActivityID INT, @Cur_StartTime nvarchar(10), @Cur_EndTime nvarchar(10), @Cur_CalculatedHours int,
		@Cur_OperatingType nvarchar(20),@CreatedBy int;

DECLARE @ExecuteQuantity int, @ExeutedSection int;

--- Declare Other Variables
DECLARE @CurrentDate DATETIME, @UnitPrice numeric(18,2), @TotalPrice numeric(18,2) =0, @HeaderTotalPrice numeric(18,2) =0, @BillNumber nvarchar(50),
		@BillFormat nvarchar(20),@UpriceString nvarchar(20), @EncounterFlag bit = 0, @BillHeaderID INT, @BillDetailLineNumber INT = 0,
		@AuthID INT, @AuthType INT, @AuthCode nvarchar(50), @PayerID nvarchar(20), @MCMultiplier numeric (18,2),@MCPatientShare numeric (18,2),
		@SelfPayFlag bit = 0,@EncounterPatientShareApplied numeric (18,2), @BalancePatientShare numeric (18,2), @PayerShareNet numeric (18,2),
		@poPayerNETShare  numeric (18,2),@ClosedActivitiesCount int,@poMCDiscount  numeric (18,2)=0,@poHeaderMCDiscount  numeric (18,2)=0, @poIsMCContractBaseRateApplied bit 
		,@poDRGCode nvarchar(20) = '';

	SET @CurrentDate = (Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))
-- Declare Cursor with Closed Order but not on any Bill
			
	DECLARE CursurSurguryToBill CURSOR FOR
	Select [Id],[PatientId],[EncounterId],[OperatingType],[StartDay],[EndDay],[StartTime],[EndTime],[CalculatedHours],[CodeValue],[CodeValueType],[CreatedBy]
	from [dbo].[OperatingRoom] 
	Where CorporateID = @pCorporateID and FacilityID = @pFacilityID and EncounterID = @pEncounterID and Status in ('1') AND OperatingType = '1'
	OPEN CursurSurguryToBill;  
	
	FETCH NEXT FROM CursurSurguryToBill INTO @Cur_OpenOrderID,@Cur_PatientID,@Cur_EncounterID,@Cur_OperatingType,@Cur_OrderStartDate,@Cur_OrderEndDate,
	@Cur_StartTime,@Cur_EndTime,@Cur_CalculatedHours,@Cur_OrderCode,@Cur_OrderType,@CreatedBy;
					
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
	Print @Cur_OrderType;
	IF(@Cur_CalculatedHours is not null)
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

		--- Get unit price from the ServiceCode/CPT tables

		--Set @UnitPrice = [dbo].[GetUnitPrice] (@Cur_OrderType,@Cur_OrderCode,@pCorporateID,@pFacilityID)
		/*
		Here, in function [GetUnitPrice], added one more input parameter that will get the Code Price based on that activity Date.
		By Amit Jain on 26 Jan, 2016
		*/
		Set @UnitPrice = [dbo].[GetUnitPrice] (@Cur_OrderType,@Cur_OrderCode,@pCorporateID,@pFacilityID,@Cur_OrderEndDate)

		Set @TotalPrice = (@UnitPrice * 1)
		Set @HeaderTotalPrice = @HeaderTotalPrice + @TotalPrice


		--If @SelfPayFlag <> 1
		--Begin
		--print(@SelfPayFlag)
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

				Set @BalancePatientShare = @MCPatientShare
				Set @PayerShareNet = @poPayerNETShare
		--End --- Check of Self Pay Ends Here
		--ELSE
		--Begin  
		--		--- This a Self Pay Flag so Set Shares Accrodingly
		--		Set @BalancePatientShare = @TotalPrice
		--		Set @PayerShareNet = 0
		--End


		----- XXXXXXXXXXXXXXXXXXXXXXXX----
        -- Generate BillDetails/Activity - STARTS
		Set @BillDetailLineNumber = @BillDetailLineNumber + 1;

		Insert into BillActivity (BillHeaderID,LineNumber,CorporateID,FacilityID,PatientID,EncounterID,DiagnosisID,DiagnosisType,DiagnosisCode,
						ActivityID,ActivityType,ActivityCode,ActivityStartDate,ActivityEndDate,QuantityOrdered,orderingClinicianID,OrderedDate,
						AdminstratingClinicianID,OrderCloseDate,AuthorizationID,AuthorizationCode,Gross,PatientShare,PayerShareNet ,Status,CreatedBy,CreatedDate,
						MCDiscount)
			Values(@BillHeaderID,@BillDetailLineNumber,@pCorporateID,@pFacilityID,@Cur_PatientID,@Cur_EncounterID,0,
			Case WHEN @Cur_OrderType = '3' THEN 'CPT' ELSE 'Service Code' END,'',
						@Cur_OpenOrderID, @Cur_OrderType,@Cur_OrderCode,cast(@Cur_OrderStartDate as datetime),
						cast(@Cur_OrderEndDate as datetime),1,@CreatedBy,cast(@Cur_OrderStartDate as datetime),
						1,cast(@Cur_OrderStartDate as datetime) ,@AuthID,@AuthCode,@TotalPrice,@BalancePatientShare,@PayerShareNet,0,1,@CurrentDate,@poMCDiscount)


		SET @ExecuteQuantity = ((@Cur_CalculatedHours - 1) /0.3);
		IF (@ExecuteQuantity > 0)
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

			--- Get unit price from the ServiceCode/CPT tables
			--Set @UnitPrice = [dbo].[GetUnitPrice] (@Cur_OrderType,@Cur_OrderCode,@pCorporateID,@pFacilityID)
			/*
			Here, in function [GetUnitPrice], added one more input parameter that will get the Code Price based on that activity Date.
			By Amit Jain on 26 Jan, 2016
			*/
			Set @UnitPrice = [dbo].[GetUnitPrice] (@Cur_OrderType,@Cur_OrderCode,@pCorporateID,@pFacilityID,@Cur_OrderEndDate)
			Set @TotalPrice = (@UnitPrice * @ExecuteQuantity)
			Set @HeaderTotalPrice = @HeaderTotalPrice + @TotalPrice

			If @SelfPayFlag <> 1
		Begin
		--print(@SelfPayFlag)
			------ XXXXXXXXXXXXXXx ---- New MC Contract APPPLIED
			EXEC [dbo].[SPROC_ApplyMCContractToBillActivity]	@Cur_PatientID,@Cur_EncounterID,@Cur_OrderType,'20-03', @TotalPrice output,
																	@MCPatientShare output, @poPayerNETShare output, @EncounterPatientShareApplied output,
																	@poMCDiscount output, @poIsMCContractBaseRateApplied output, @SelfPayFlag, ''
			
			---- NULL are creating issues with not taking right effect for PatientShare and PayerShareNET So folloiwng commands to take care
				Set @EncounterPatientShareApplied = isnull(@EncounterPatientShareApplied,0)
				Set @MCPatientShare = isnull(@MCPatientShare,0)
				Set @PayerShareNet = isnull(@PayerShareNet,0)
				Set @TotalPrice = isnull(@TotalPrice,0)
				Set @BalancePatientShare = isnull(@BalancePatientShare,0)
				Set @HeaderTotalPrice =  isnull(@HeaderTotalPrice,0)

				Set @BalancePatientShare = @MCPatientShare
				Set @PayerShareNet = @poPayerNETShare
		End --- Check of Self Pay Ends Here
		ELSE
		Begin  
				--- This a Self Pay Flag so Set Shares Accrodingly
				Set @BalancePatientShare = @TotalPrice
				Set @PayerShareNet = 0
		End

		----- XXXXXXXXXXXXXXXXXXXXXXXX----
        -- Generate BillDetails/Activity - STARTS
		Set @BillDetailLineNumber = @BillDetailLineNumber + 1;

		Insert into BillActivity (BillHeaderID,LineNumber,CorporateID,FacilityID,PatientID,EncounterID,DiagnosisID,DiagnosisType,DiagnosisCode,
						ActivityID,ActivityType,ActivityCode,ActivityStartDate,ActivityEndDate,QuantityOrdered,orderingClinicianID,OrderedDate,
						AdminstratingClinicianID,OrderCloseDate,AuthorizationID,AuthorizationCode,Gross,PatientShare,PayerShareNet ,Status,CreatedBy,CreatedDate,
						MCDiscount)
			Values(@BillHeaderID,@BillDetailLineNumber,@pCorporateID,@pFacilityID,@Cur_PatientID,@Cur_EncounterID,0,
			Case WHEN @Cur_OrderType = '3' THEN 'CPT' ELSE 'Service Code' END,'',
						@Cur_OpenOrderID, @Cur_OrderType,'20-03',cast(@Cur_OrderStartDate as datetime) ,
						cast(@Cur_OrderEndDate as datetime),@ExecuteQuantity,@CreatedBy,cast(@Cur_OrderStartDate as datetime),
						1,cast(@Cur_OrderStartDate as datetime) ,@AuthID,@AuthCode,@TotalPrice,@BalancePatientShare,@PayerShareNet,0,1,@CurrentDate,@poMCDiscount)

		--SET @ExecuteQuantity =@ExecuteQuantity - 1;
		END

		-- Generate BillDetails/Activity - STARTS
					  		 
         ----- XXXXXXXXXXXXXXXXXXXXXXXX----
          -- Update OpenOrder to state that it is on Bill now - Meaning Status Update to 4 - On Bill - STARTS
          Update OperatingRoom Set Status = 4 Where ID = @Cur_OpenOrderID;
          ---- BB - 30-Jan-2015 -- New Logic Needed if All Scheduled Activities are on Bill Set related OpenOrder also on Bill - STARTS
        FETCH NEXT FROM CursurSurguryToBill INTO @Cur_OpenOrderID,@Cur_PatientID,@Cur_EncounterID,@Cur_OperatingType,@Cur_OrderStartDate,@Cur_OrderEndDate,
		@Cur_StartTime,@Cur_EndTime,@Cur_CalculatedHours,@Cur_OrderCode,@Cur_OrderType,@CreatedBy;	

	END
	END
	CLOSE CursurSurguryToBill;  
    DEALLOCATE CursurSurguryToBill; 

	DECLARE @AnthesiaBaseUnit int =0;
	Declare @StartTimeUnit DATETIME, @EndTimeUnit Datetime, @MinsDiffernece numeric(18,2), @MinsRoundOf int,@CPTQunatity int;

	DECLARE CursurAnthesiaSurguryToBill CURSOR FOR
	Select [Id],[PatientId],[EncounterId],[OperatingType],[StartDay],[EndDay],[StartTime],[EndTime],[CalculatedHours],[CodeValue],[CodeValueType],[CreatedBy]
	from [dbo].[OperatingRoom] 
	Where CorporateID = @pCorporateID and FacilityID = @pFacilityID and EncounterID = @pEncounterID and Status in ('1') AND OperatingType = '2'
	OPEN CursurAnthesiaSurguryToBill;  
	
	FETCH NEXT FROM CursurAnthesiaSurguryToBill INTO @Cur_OpenOrderID,@Cur_PatientID,@Cur_EncounterID,@Cur_OperatingType,@Cur_OrderStartDate,@Cur_OrderEndDate,
	@Cur_StartTime,@Cur_EndTime,@Cur_CalculatedHours,@Cur_OrderCode,@Cur_OrderType,@CreatedBy;
					
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
	Print @Cur_OrderType;
	IF(@Cur_CalculatedHours is not null)
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

		--- Get unit price from the ServiceCode/CPT tables
		
		--Set @UnitPrice = [dbo].[GetUnitPrice] (@Cur_OrderType,@Cur_OrderCode,@pCorporateID,@pFacilityID)

		/*
		Here, in function [GetUnitPrice], added one more input parameter that will get the Code Price based on that activity Date.
		By Amit Jain on 26 Jan, 2016
		*/
		Set @UnitPrice = [dbo].[GetUnitPrice] (@Cur_OrderType,@Cur_OrderCode,@pCorporateID,@pFacilityID,@Cur_OrderEndDate)

		--- Get Anthensia Base unit
		Select @AnthesiaBaseUnit = CodeAnesthesiaBaseUnit from CPTCOdes where CodeNumbering = @Cur_OrderCode
		SET @AnthesiaBaseUnit = ISNULL(@AnthesiaBaseUnit,0)

		
		SET @StartTimeUnit  =  CAST( cast(Cast(@Cur_OrderStartDate as date) as nVarchar(50)) + ' '+cast(@Cur_StartTime as nvarchar(10)) as datetime); --cast(@Cur_OrderStartDate as datetime) + cast(@Cur_StartTime as datetime);
		SET @EndTimeUnit =CAST( cast(Cast(@Cur_OrderEndDate as date) as nVarchar(50)) + ' '+cast(@Cur_EndTime as nvarchar(10)) as datetime);-- cast(@Cur_OrderEndDate as datetime) + cast(@Cur_EndTime as datetime);
		
		SET @MinsDiffernece = (ISNULL((SELECT DATEDIFF(minute, @StartTimeUnit, @EndTimeUnit)),0)/15);
		SET @MinsRoundOf =  Cast(ROUND(@MinsDiffernece, 0) as int);

		SET @CPTQunatity = @AnthesiaBaseUnit + @MinsRoundOf;


		Set @TotalPrice = (@UnitPrice * @CPTQunatity)
		Set @HeaderTotalPrice = @HeaderTotalPrice + @TotalPrice


		If @SelfPayFlag <> 1
		Begin
		--print(@SelfPayFlag)
			------ XXXXXXXXXXXXXXx ---- New MC Contract APPPLIED
			EXEC [dbo].[SPROC_ApplyMCContractToBillActivity]	@Cur_PatientID,@Cur_EncounterID,@Cur_OrderType,@Cur_OrderCode, @TotalPrice output,
																@MCPatientShare output, @poPayerNETShare output, @EncounterPatientShareApplied output,
																@poMCDiscount output, @poIsMCContractBaseRateApplied output, @SelfPayFlag, ''
			
			---- NULL are creating issues with not taking right effect for PatientShare and PayerShareNET So folloiwng commands to take care
				Set @EncounterPatientShareApplied = isnull(@EncounterPatientShareApplied,0)
				Set @MCPatientShare = isnull(@MCPatientShare,0)
				Set @PayerShareNet = isnull(@PayerShareNet,0)
				Set @TotalPrice = isnull(@TotalPrice,0)
				Set @BalancePatientShare = isnull(@BalancePatientShare,0)
				Set @HeaderTotalPrice =  isnull(@HeaderTotalPrice,0)

				Set @BalancePatientShare = @MCPatientShare
				Set @PayerShareNet = @poPayerNETShare
		End --- Check of Self Pay Ends Here
		ELSE
		Begin  
				--- This a Self Pay Flag so Set Shares Accrodingly
				Set @BalancePatientShare = @TotalPrice
				Set @PayerShareNet = 0
		End

		----- XXXXXXXXXXXXXXXXXXXXXXXX----
        -- Generate BillDetails/Activity - STARTS
		Set @BillDetailLineNumber = @BillDetailLineNumber + 1;

		Insert into BillActivity (BillHeaderID,LineNumber,CorporateID,FacilityID,PatientID,EncounterID,DiagnosisID,DiagnosisType,DiagnosisCode,
						ActivityID,ActivityType,ActivityCode,ActivityStartDate,ActivityEndDate,QuantityOrdered,orderingClinicianID,OrderedDate,
						AdminstratingClinicianID,OrderCloseDate,AuthorizationID,AuthorizationCode,Gross,PatientShare,PayerShareNet ,Status,CreatedBy,CreatedDate,
						MCDiscount)
			Values(@BillHeaderID,@BillDetailLineNumber,@pCorporateID,@pFacilityID,@Cur_PatientID,@Cur_EncounterID,0,
			Case WHEN @Cur_OrderType = '3' THEN 'CPT' ELSE 'Service Code' END,'',
						@Cur_OpenOrderID, @Cur_OrderType,@Cur_OrderCode,cast(@Cur_OrderStartDate as datetime),
						cast(@Cur_OrderEndDate as datetime),@CPTQunatity,@CreatedBy,cast(@Cur_OrderStartDate as datetime),
						1,cast(@Cur_OrderStartDate as datetime) ,@AuthID,@AuthCode,@TotalPrice,@BalancePatientShare,@PayerShareNet,0,1,@CurrentDate,@poMCDiscount)


         ----- XXXXXXXXXXXXXXXXXXXXXXXX----
          -- Update OpenOrder to state that it is on Bill now - Meaning Status Update to 4 - On Bill - STARTS
          Update OperatingRoom Set Status = 4 Where ID = @Cur_OpenOrderID;
          ---- BB - 30-Jan-2015 -- New Logic Needed if All Scheduled Activities are on Bill Set related OpenOrder also on Bill - STARTS
        FETCH NEXT FROM CursurAnthesiaSurguryToBill INTO @Cur_OpenOrderID,@Cur_PatientID,@Cur_EncounterID,@Cur_OperatingType,@Cur_OrderStartDate,@Cur_OrderEndDate,
		@Cur_StartTime,@Cur_EndTime,@Cur_CalculatedHours,@Cur_OrderCode,@Cur_OrderType,@CreatedBy;	

	END
	END
	CLOSE CursurAnthesiaSurguryToBill;  
    DEALLOCATE CursurAnthesiaSurguryToBill; 
					  
	-- Needed Following Check because if there are no rows Selected in Cursor then there is No point doing updates/calulcations on Header and Encounter
	If @EncounterFlag = 1
	BEGIN
			
		---- Update Bill Header and Encounter
		declare @TotalPatientShare numeric (18,2), @TotalPayerNETShare numeric (18,2)

		Select @HeaderTotalPrice = sum(gross),@TotalPatientShare = sum(PatientShare),@TotalPayerNETShare = sum(PayerShareNet), @poHeaderMCDiscount = sum(MCDiscount)  
		from BillActivity Where BillHeaderID = @BillHeaderID;
		
		---- Now Update Total Header Price
			Update BillHeader Set BillNumber = @BillNumber,Gross = Isnull(@HeaderTotalPrice,0),PatientShare = isnull(@TotalPatientShare,0),
			MCDiscount = isnull(@poHeaderMCDiscount,0),PayerShareNet = isnull(@TotalPayerNETShare,0) Where BillheaderID = @BillHeaderID;

		--- Update New Charges to Encounter 
		Update Encounter Set Charges = (isnull(Charges,0)+@HeaderTotalPrice), IsMCContractBaseRateApplied =@poIsMCContractBaseRateApplied  Where EncounterID = @pEncounterID;
	END ---- Ch
END





