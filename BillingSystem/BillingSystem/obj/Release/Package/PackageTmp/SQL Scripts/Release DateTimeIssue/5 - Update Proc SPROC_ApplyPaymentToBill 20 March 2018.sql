IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplyPaymentToBill')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplyPaymentToBill

GO

/****** Object:  StoredProcedure [dbo].[SPROC_ApplyPaymentToBill]    Script Date: 20-03-2018 15:59:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_ApplyPaymentToBill]
(  
@pCorporateID int,
@pFacilityID int
)
AS
BEGIN
    --BEGIN TRY
    --    SET NOCOUNT ON
    --    SET XACT_ABORT ON
 
    --    --- 
		 
    --    BEGIN TRANSACTION
          
			--- Declare Cursor Fetch Variables
		    DECLARE @Cur_PaymentID INT, @Cur_PayDate Datetime,@Cur_PayAmount decimal (18,2),@Cur_PayReference nvarchar(50), @Cur_PayFor int,@Cur_PayBy int,
					@Cur_PayBillIDID int,@Cur_PayActivityID int,@Cur_PayEncounterID int,@Cur_PayFacilityID int,@Cur_PayCorporateID int,@Cur_PayXADenialCode nvarchar(50),
					@Cur_ARFileID bigint,@Cur_PayNETAmount decimal (18,2)
					
			
			--- Declare Other Variables
			DECLARE @CurrentDate DATETIME, @BillNumber nvarchar(50),@BillHeaderID_Previous INT = 0,@TotalAmountPaid numeric (18,2) = 0, @PaymentStatus int,
					@TotalPaymentReference nvarchar(50), @TotalPaymentDate datetime,@TotalDenialCode nvarchar(50), @TotalPayFor int,@TotalEncounterID int, 
					@TotalARFileID bigint,@IsApplied bit = 0,@PayAppliedStatus int,@AppliedAmount numeric (18,2),@AlreadyPaymentAmount numeric (18,2),
					@PayerNETAmount numeric (18,2), @PresentBillHeaderStatus int;
			
			SET @CurrentDate = (Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

			-- Declare Cursor with New Payments Uploaded
			
			DECLARE PaymentToApply CURSOR FOR
			Select PaymentID, PayDate, (isnull(PayAmount,0) - isnull(PayAppliedAmount,0)) as 'PayAmount',PayReference,PayFor,PayBy,
			PayBillID, PayActivityID,PayEncounterID,PayFacilityID,PayCorporateID,PayXADenialCode,PayXAFileHeaderID,PayNETAmount
			from Payment Where (isnull(PayAmount,0) - isnull(PayAppliedAmount,0)) > 0 and  PayStatus < 900 and PayAppliedStatus is NULL and PayType < 200
			Order by PayBillID ;
			
				OPEN PaymentToApply;  
								
				FETCH NEXT FROM PaymentToApply INTO @Cur_PaymentID, @Cur_PayDate,@Cur_PayAmount,@Cur_PayReference, @Cur_PayFor,@Cur_PayBy,@Cur_PayBillIDID,
				@Cur_PayActivityID,@Cur_PayEncounterID,@Cur_PayFacilityID,@Cur_PayCorporateID,@Cur_PayXADenialCode,@Cur_ARFileID,@Cur_PayNETAmount
							
				
				WHILE @@FETCH_STATUS = 0  
				BEGIN  

				---- Reset following for Each Row
				Set @PayAppliedStatus = 0 --- Unknown
				Set @AppliedAmount = 0 
				Set @AlreadyPaymentAmount = 0
				Set @PayerNETAmount = 0 
				---- First Select Orginal Amount Paid for this Activity (If Any) - To Handle Multiple Payments for one Activity
				Select @AlreadyPaymentAmount = isnull(PaymentAmount,0),@PayerNETAmount = isnull(PayerShareNet,0) from BillActivity
				Where BillHeaderID = @Cur_PayBillIDID and EncounterID = @Cur_PayEncounterID and PatientID = @Cur_PayFor and BillActivityID = @Cur_PayActivityID

				------ >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>  -- TEST Different Scenarios - STARTS
					----If @PayerNETAmount <> @AlreadyPaymentAmount
					----Begin
					----	--- Under Paid
					----	If @Cur_PayAmount > 10000
					----		Set @Cur_PayAmount = @Cur_PayAmount - 2000
					----	Else --- Over Paid
					----		Set @Cur_PayAmount = @Cur_PayAmount + 200

						--- Different Scenario of Denials
						----If @Cur_PayAmount > 10000
						----Begin
						----	Set @Cur_PayAmount = 0
						----	Set @Cur_PayXADenialCode = 'BBDenied'
						----End
					----End
				------ >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>  -- TEST Different Scenarios - ENDS

				--- Set the amount to be applied after making sure Already paid is handled
					If (@PayerNETAmount - @AlreadyPaymentAmount) >= @Cur_PayAmount  ----- If Balance to be Applied is Greater or Equal to incoming Amount then Use the Whole Amount
						Set @AppliedAmount = @Cur_PayAmount
					Else
						Set @AppliedAmount = (@PayerNETAmount - @AlreadyPaymentAmount)  ---- Balance to be Paid

				---- Set the Payment Status for Payment Table based on how much is applied
					Select @PayAppliedStatus = (CASE
					WHEN @Cur_PayAmount > @AppliedAmount Then 150 -- Over Paid (UnApplied)
					WHEN @Cur_PayAmount = @AppliedAmount Then 100 -- Fully Paid
					WHEN @Cur_PayAmount < @AppliedAmount Then 50 -- Partial Paid
					ELSE 0 -- Un-Known
					END)
					
					------ Set The Applied/UnApplied Amounts in incoming Payments									
					Update Payment Set PayAppliedAmount = (Isnull(PayAppliedAmount,0)+@AppliedAmount),
					PayUnAppliedAmount = (CASE WHEN @Cur_PayAmount>@AppliedAmount Then(@Cur_PayAmount - @AppliedAmount) ELSE 0 END),
					PayAppliedStatus = @PayAppliedStatus
					Where PaymentID = @Cur_PaymentID;
				
				--- Update the Applied Amount if There was any Payment Applied to BillActivity
				If @AppliedAmount > 0 OR (Len(@Cur_PayXADenialCode) > 0)
				Begin
					--- Apply Payments to Order Activity
					Update BillActivity Set PaymentAmount = (isnull(PaymentAmount,0) + @AppliedAmount), PaymentReference = @Cur_PayReference, DateSettlement = @Cur_PayDate ,
							DenialCode = @Cur_PayXADenialCode, ARFileID = @Cur_PaymentID
					Where BillHeaderID = @Cur_PayBillIDID and EncounterID = @Cur_PayEncounterID and PatientID = @Cur_PayFor and BillActivityID = @Cur_PayActivityID;
					
					--- Set @TotalAmountPaid = isnull(@TotalAmountPaid,0) + isnull(@AppliedAmount,0)

					--- For Summary to Excute on Bill Header set following IsApplied
					Set @IsApplied = 1
					
				End
					
					--- Apply Payment Summary to Bill Header - STARTS
					If @Cur_PayBillIDID <> @BillHeaderID_Previous and @BillHeaderID_Previous <> 0 and @IsApplied = 1
					Begin

						Set @TotalAmountPaid = 0
						Select @TotalAmountPaid = Sum(isnull(PaymentAmount,0)) from BillActivity 
						Where BillHeaderID = @BillHeaderID_Previous and EncounterID = @TotalEncounterID and PatientID = @TotalPayFor; 

						Select @PresentBillHeaderStatus = Status from BillHeader 
						Where BillHeaderID = @BillHeaderID_Previous and EncounterID = @TotalEncounterID and PatientID = @TotalPayFor;

						if @PresentBillHeaderStatus in (60,110,160)
						Begin
							Update BillHeader Set PaymentAmount = isnull(@TotalAmountPaid,0), PaymentReference = @TotalPaymentReference, 
								DateSettlement = @TotalPaymentDate , DenialCode = @TotalDenialCode, ARFileID = @TotalARFileID,
								 Status =   CASE WHEN @TotalDenialCode is NULL OR @TotalDenialCode='' OR @TotalDenialCode=' ' 
									THEN [dbo].[GetBillNextStatus] (@PresentBillHeaderStatus,1) ELSE [dbo].[GetBillNextStatus] (@PresentBillHeaderStatus,2)  END
							Where BillHeaderID = @BillHeaderID_Previous and EncounterID = @TotalEncounterID and PatientID = @TotalPayFor;
						End
						ELSE
						Begin
							Update BillHeader Set PaymentAmount = isnull(@TotalAmountPaid,0), PaymentReference = @TotalPaymentReference, 
								DateSettlement = @TotalPaymentDate , DenialCode = @TotalDenialCode, ARFileID = @TotalARFileID
							 Where BillHeaderID = @BillHeaderID_Previous and EncounterID = @TotalEncounterID and PatientID = @TotalPayFor;
						End

						---- Reset Variables for Next Bill/Claim
						Set @TotalAmountPaid = 0
						Set @TotalEncounterID = 0
						Set @TotalARFileID = 0
						Set @TotalDenialCode = NULL
						Set @TotalPaymentReference = NULL
						Set @TotalPaymentDate = NULL
						Set @IsApplied = 0
					

					End
					--- Apply Payment Summary to Bill Header - ENDS
										

					----- Set Variables for Summary
					 Set @BillHeaderID_Previous = @Cur_PayBillIDID
					 Set @TotalEncounterID = @Cur_PayEncounterID
					 Set @TotalARFileID = @Cur_ARFileID
					 Set @TotalPaymentReference = @Cur_PayReference
					 Set @TotalPaymentDate = @Cur_PayDate
					 Set @TotalPayFor = @Cur_PayFor
					 If @Cur_PayXADenialCode is not NULL AND (Len(@Cur_PayXADenialCode) > 0)
						Set @TotalDenialCode = @Cur_PayXADenialCode

					---- PRINT @TotalDenialCode
				FETCH NEXT FROM PaymentToApply INTO @Cur_PaymentID, @Cur_PayDate,@Cur_PayAmount,@Cur_PayReference, @Cur_PayFor,@Cur_PayBy,@Cur_PayBillIDID,
				@Cur_PayActivityID,@Cur_PayEncounterID,@Cur_PayFacilityID,@Cur_PayCorporateID,@Cur_PayXADenialCode,@Cur_ARFileID,@Cur_PayNETAmount				
							
				END  --END OF @@FETCH_STATUS = 0  



			--- For the Summary to Bill Header ( This is needed to handle Last line selected from Payments)
			If  @BillHeaderID_Previous <> 0 and @IsApplied = 1
			Begin
						Set @TotalAmountPaid = 0
						Select @TotalAmountPaid = Sum(isnull(PaymentAmount,0)) from BillActivity 
						Where BillHeaderID = @BillHeaderID_Previous and EncounterID = @TotalEncounterID and PatientID = @TotalPayFor;

					Select @PresentBillHeaderStatus = Status from BillHeader 
						Where BillHeaderID = @BillHeaderID_Previous and EncounterID = @TotalEncounterID and PatientID = @TotalPayFor;

						if @PresentBillHeaderStatus in (60,110,160)
						Begin
							Update BillHeader Set PaymentAmount = isnull(@TotalAmountPaid,0), PaymentReference = @TotalPaymentReference, 
								DateSettlement = @TotalPaymentDate , DenialCode = @TotalDenialCode, ARFileID = @TotalARFileID,
								 Status =   CASE WHEN @TotalDenialCode is NULL OR @TotalDenialCode='' OR @TotalDenialCode=' ' 
											THEN [dbo].[GetBillNextStatus] (@PresentBillHeaderStatus,1) ELSE [dbo].[GetBillNextStatus] (@PresentBillHeaderStatus,2)  END
							Where BillHeaderID = @BillHeaderID_Previous and EncounterID = @TotalEncounterID and PatientID = @TotalPayFor;
						End
						ELSE
						Begin
							Update BillHeader Set PaymentAmount = isnull(@TotalAmountPaid,0), PaymentReference = @TotalPaymentReference, 
								DateSettlement = @TotalPaymentDate , DenialCode = @TotalDenialCode, ARFileID = @TotalARFileID
							 Where BillHeaderID = @BillHeaderID_Previous and EncounterID = @TotalEncounterID and PatientID = @TotalPayFor;
						End

			End
			

		
			CLOSE PaymentToApply;  
			DEALLOCATE PaymentToApply; 




			
    --    commit TRANSACTION
    --END TRY
    --BEGIN CATCH
    --    IF @@TRANCOUNT > 0 AND XACT_STATE() <> 0 
    --        ROLLBACK TRAN
    --    -- Do the Necessary Error logging if required
    --    -- Take Corrective Action if Required
    --END CATCH
END














GO


