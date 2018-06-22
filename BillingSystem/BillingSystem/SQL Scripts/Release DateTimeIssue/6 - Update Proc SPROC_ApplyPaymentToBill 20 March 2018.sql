IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplyPaymentManualToBill')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplyPaymentManualToBill

GO

/****** Object:  StoredProcedure [dbo].[SPROC_ApplyPaymentManualToBill]    Script Date: 20-03-2018 16:04:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_ApplyPaymentManualToBill]
(  
@pCorporateID int,
@pFacilityID int
)
AS
BEGIN
	DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))	

    --BEGIN TRY
    --    SET NOCOUNT ON
    --    SET XACT_ABORT ON
 
    --    --- 
		 
    --    BEGIN TRANSACTION
          
			--- Declare Cursor Fetch Variables
		    DECLARE @Cur_PaymentID INT, @Cur_PayDate Datetime,@Cur_PayAmount decimal (18,2),@Cur_PayReference nvarchar(50), @Cur_PayFor int,@Cur_PayBy int,
					@Cur_PayBillIDID int,@Cur_PayActivityID int,@Cur_PayEncounterID int,@Cur_PayFacilityID int,@Cur_PayCorporateID int,@Cur_PayXADenialCode nvarchar(50),
					@Cur_ARFileID bigint,@Cur_PayNETAmount decimal (18,2),
					@Cur2_PatientShareAmount numeric (18,2),@Cur2_PatientPAIDAmount numeric (18,2),@Cur2_BillActivityID bigint;
					
			
			--- Declare Other Variables
			DECLARE @CurrentDate DATETIME = @LocalDateTime, @BillNumber nvarchar(50),@BillHeaderID_Previous INT = 0,@TotalAmountPaid numeric (18,2) = 0, @PaymentStatus int,
					@TotalPaymentReference nvarchar(50), @TotalPaymentDate datetime,@TotalDenialCode nvarchar(50), @TotalPayFor int,@TotalEncounterID int, 
					@TotalARFileID bigint,@IsApplied bit = 0,@PayAppliedStatus int,@AppliedAmount numeric (18,2),@AlreadyPaymentAmount numeric (18,2),
					@PayerNETAmount numeric (18,2), @PresentBillHeaderStatus int;
			

			-- Declare Cursor with New Payments Uploaded
			
			DECLARE PaymentManualToApply CURSOR FOR
			Select PaymentID, PayDate, (isnull(PayAmount,0) - isnull(PayAppliedAmount,0)) as 'PayAmount',PayReference,PayFor,PayBy,
			PayBillID, PayActivityID,PayEncounterID,PayFacilityID,PayCorporateID,PayXADenialCode,PayXAFileHeaderID,PayNETAmount
			from Payment Where (isnull(PayAmount,0) - isnull(PayAppliedAmount,0)) > 0 and  isnull(PayStatus,0) < 900 and PayAppliedStatus is NULL and PayType = 500
			Order by PayBillID ;
			
				OPEN PaymentManualToApply;  
								
				FETCH NEXT FROM PaymentManualToApply INTO @Cur_PaymentID, @Cur_PayDate,@Cur_PayAmount,@Cur_PayReference, @Cur_PayFor,@Cur_PayBy,@Cur_PayBillIDID,
				@Cur_PayActivityID,@Cur_PayEncounterID,@Cur_PayFacilityID,@Cur_PayCorporateID,@Cur_PayXADenialCode,@Cur_ARFileID,@Cur_PayNETAmount
							
				
				WHILE @@FETCH_STATUS = 0  
				BEGIN  

				---- Reset following for Each Row
				Set @PayAppliedStatus = 0 --- Unknown
				Set @AppliedAmount = 0 
				Set @AlreadyPaymentAmount = 0
				Set @PayerNETAmount = 0 
			
				Select @AlreadyPaymentAmount = isnull(PatientPayAmount,0),@PayerNETAmount = isnull(PatientShare,0) from BillHeader
				Where BillHeaderID = @Cur_PayBillIDID and EncounterID = @Cur_PayEncounterID and PatientID = @Cur_PayFor 

			
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
									

					Update Payment Set PayAppliedAmount = (Isnull(PayAppliedAmount,0)+@AppliedAmount),
					PayUnAppliedAmount = (CASE WHEN @Cur_PayAmount>@AppliedAmount Then(@Cur_PayAmount - @AppliedAmount) ELSE 0 END),
					PayAppliedStatus = @PayAppliedStatus
					Where PaymentID = @Cur_PaymentID;
					
				--End
					
					--- Apply Payment Summary to Bill Header - STARTS
			
						If @AppliedAmount > 0
						Begin
							Update BillHeader Set PatientPayAmount = (isnull(PatientPayAmount,0) + @AppliedAmount), PatientPayReference = @Cur_PayReference, 
								PatientDateSettlement = @Cur_PayDate 
							 Where BillHeaderID = @Cur_PayBillIDID and EncounterID = @Cur_PayEncounterID and PatientID = @Cur_PayFor;

							 ---- Now Applying to Bill Activity 
							 DECLARE PaymentManualToDETAIL CURSOR FOR
								Select isnull(PatientShare,0),isnull(PatientPayAmount,0),BillActivityID from BillActivity 
								Where BillHeaderID = @Cur_PayBillIDID and EncounterID = @Cur_PayEncounterID and PatientID = @Cur_PayFor and PatientShare>PatientPayAmount;

							OPEN PaymentManualToDETAIL;
								---- @Cur2_PatientShareAmount numeric (18,2),@Cur2_PatientPAIDAmount numeric (18,2);
								Fetch Next from PaymentManualToDETAIL into @Cur2_PatientShareAmount,@Cur2_PatientPAIDAmount,@Cur2_BillActivityID;
								WHILE @@FETCH_STATUS = 0  
								BEGIN
									
									If (@Cur2_PatientShareAmount - @Cur2_PatientPAIDAmount)  >= @AppliedAmount
										Set @TotalAmountPaid = @AppliedAmount
									ELSE
										Set @TotalAmountPaid = (@Cur2_PatientShareAmount - @Cur2_PatientPAIDAmount)   --- Balance to be Paid

										----- Update Bill Activity with Paid Amount per Line
										UpDate BillActivity Set PatientPayAmount =(isnull(PatientPayAmount,0)+@TotalAmountPaid),PatientDateSettlement = @Cur_PayDate,PatientPayReference = @Cur_PayReference
										Where BillHeaderID = @Cur_PayBillIDID and EncounterID = @Cur_PayEncounterID and PatientID = @Cur_PayFor and BillActivityID = @Cur2_BillActivityID; 

										---- Set Applied Amount with Balance Reduced as Applied
										Set @AppliedAmount = @AppliedAmount  - @TotalAmountPaid

									Fetch Next from PaymentManualToDETAIL into @Cur2_PatientShareAmount,@Cur2_PatientPAIDAmount,@Cur2_BillActivityID;
								END
							CLOSE PaymentManualToDETAIL;
							DEALLOCATE PaymentManualToDETAIL;


						End
					--	End



							
				FETCH NEXT FROM PaymentManualToApply INTO @Cur_PaymentID, @Cur_PayDate,@Cur_PayAmount,@Cur_PayReference, @Cur_PayFor,@Cur_PayBy,@Cur_PayBillIDID,
				@Cur_PayActivityID,@Cur_PayEncounterID,@Cur_PayFacilityID,@Cur_PayCorporateID,@Cur_PayXADenialCode,@Cur_ARFileID,@Cur_PayNETAmount				
							
				END  --END OF @@FETCH_STATUS = 0  
				
		
			CLOSE PaymentManualToApply;  
			DEALLOCATE PaymentManualToApply; 
						
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


