IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplyAdvicePayments')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplyAdvicePayments
GO

/****** Object:  StoredProcedure [dbo].[SPROC_ApplyAdvicePayments]    Script Date: 20-03-2018 16:56:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_ApplyAdvicePayments]
(  
@pCorporateID int,
@pFacilityID int
)
AS
BEGIN
Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))
    --BEGIN TRY
    --    SET NOCOUNT ON
    --    SET XACT_ABORT ON
 
    --    --- 
		 
    --    BEGIN TRANSACTION
          
			--- Declare Cursor Fetch Variables
		    DECLARE @Cur_XAdviceXMLParsedDataID BIGINT, @Cur_XAFileHeaderFileID BIGINT, @Cur_XACClaimID int,@Cur_XACIDPayer nvarchar(50),@Cur_XACProviderID nvarchar(50),
					@Cur_XACDenialCode nvarchar(50),@Cur_XACPaymentReference nvarchar(50),@Cur_XACDateSettlement datetime,
					@Cur_XAAActivityID int,@Cur_XAAPaymentAmount decimal (18,2),@Cur_XAADenialCode nvarchar(50),@Cur_XAClaimDenialCode nvarchar(50),@Cur_XAANET decimal (18,2)
			
			--- Declare Other Variables
			DECLARE @CurrentDate DATETIME = @LocalDateTime, @BillNumber nvarchar(50),@PatientID INT, @EncounterID INT, @PayerID nvarchar(20),@BillCorprateID int, @BillFacilityID int,
							@AmountPaid numeric (18,2), @PaymentStatus int, @XAFileHeaderID_Previous bigint = 0;
			

			-- Declare Cursor with Closed Order but not on any Bill
			Update [XAdviceXMLParsedData] SET [XACDateSettlement] = Replace([XACDateSettlement],'T',' ') 
			where XAAdviceStatus is NULL and CorporateID =@pCorporateID and FacilityID = @pFacilityID ;


			DECLARE AdvicePayment CURSOR FOR
			Select Cast([XAdviceXMLParsedDataID] as Bigint),Cast([XAFileHeaderFileID] as bigint),Cast([XACClaimID] as int),[XACIDPayer],[XACProviderID],[XACDenialCode],[XACPaymentReference],
			Cast([XACDateSettlement] as datetime),Cast([XAAActivityID] as int),Cast([XAAPaymentAmount] as decimal(18,2)),[XAADenialCode],[XACDenialCode],Cast([XAANet] as Decimal(18,2))
			 from [dbo].[XAdviceXMLParsedData] where XAAdviceStatus is NULL and CorporateID = @pCorporateID and FacilityID = @pFacilityID ;
			

				OPEN AdvicePayment;  

				
				FETCH NEXT FROM AdvicePayment INTO @Cur_XAdviceXMLParsedDataID, @Cur_XAFileHeaderFileID, @Cur_XACClaimID,@Cur_XACIDPayer,@Cur_XACProviderID,
					@Cur_XACDenialCode,@Cur_XACPaymentReference,@Cur_XACDateSettlement,@Cur_XAAActivityID,@Cur_XAAPaymentAmount,@Cur_XAADenialCode,@Cur_XAClaimDenialCode,@Cur_XAANET;
							
				
				WHILE @@FETCH_STATUS = 0  
				BEGIN  

				--- Reset for each line
				Set @PaymentStatus = 0 
				Set @BillNumber = NULL
				
				----- Match the Cliam with Bill									
				Select @BillNumber=BillNumber,@PatientID=PatientID,@EncounterID=EncounterID,@PayerID=PayerID,@BillFacilityID=FacilityID,@BillCorprateID=CorporateID 
				from  BillHeader Where BillheaderID = @Cur_XACClaimID

								----- Set Payment Status based on incoming file XML Data - STARTS
				If @BillNumber is NULL
					Set @PaymentStatus = 1000 --- Not Matched in our System
				
				If @PaymentStatus <> 1000
				
				BEGIN

					--- Claim Level Denied
					if @Cur_XAClaimDenialCode is NULL OR @Cur_XAClaimDenialCode = '' OR @Cur_XAClaimDenialCode = ' '
					
					Begin
					if @Cur_XAADenialCode is not NULL OR @Cur_XAADenialCode <> '' OR @Cur_XAADenialCode <> ' '
					
					BEGIN
						---- Fully Paid for this Line(Activity)
						If isnull(@Cur_XAAPaymentAmount,0) > 0
						Begin
							--- Fully Paid or Partial Paid
							If isnull(@Cur_XAANET,0) = isnull(@Cur_XAAPaymentAmount,0)
								Set @PaymentStatus = 100  --- Fully Paid
							Else 
								Set @PaymentStatus = 50  --- Partial Paid
						End	
					END --- Activity Level NO Denial Check Ends
					ELSE  --- There was Activity Level Denial
					Begin
						Set @PaymentStatus = 20  ---- Denial at Line/Activity Level
					End  --- Activity Level NO Denial Check Ends
				End --- Claim Level Check Ends
				ELSE --- There was Full Claim Denied
				Begin
					Set @PaymentStatus = 900  -- Claim Level Denied
					Set @Cur_XAADenialCode = @Cur_XAClaimDenialCode
				End

				END -- Not Matched Ends

				----- Set Payment Status based on incoming file XML Data - ENDS

				------ Insert Record into Payment File
				Insert into [dbo].[Payment]([PayType],[PayDate],[PayAmount],[PayReference],[PayBillNumber],[PayFor],[PayBy],[PayBillID],[PayActivityID],
							[PayEncounterID],[PayFacilityID],[PayCorporateID],[PayXAFileHeaderID],[PayXAAdviceID],[PayNETAmount],[PayXADenialCode],[PayStatus],[PayCreatedBy],[PayCreatedDate],[PayIsActive])
					Select '100',@Cur_XACDateSettlement,@Cur_XAAPaymentAmount,@Cur_XACPaymentReference,@BillNumber,@PatientID,@PayerID,@Cur_XACClaimID,@Cur_XAAActivityID,
							@EncounterID,@BillFacilityID,@BillCorprateID,@Cur_XAFileHeaderFileID,@Cur_XAdviceXMLParsedDataID,@Cur_XAANET,@Cur_XAADenialCode,@PaymentStatus,1,@CurrentDate,1
				
				---- Mark it as Processed 
				If @XAFileHeaderID_Previous <> @Cur_XAFileHeaderFileID and  @XAFileHeaderID_Previous >0
					Update  [dbo].[XAdviceXMLParsedData] Set XAAdviceStatus = 1	Where XAFileHeaderFileID = @XAFileHeaderID_Previous				
			 
				Set @XAFileHeaderID_Previous = @Cur_XAFileHeaderFileID
									
				FETCH NEXT FROM AdvicePayment INTO @Cur_XAdviceXMLParsedDataID, @Cur_XAFileHeaderFileID, @Cur_XACClaimID,@Cur_XACIDPayer,@Cur_XACProviderID,
					@Cur_XACDenialCode,@Cur_XACPaymentReference,@Cur_XACDateSettlement,@Cur_XAAActivityID,@Cur_XAAPaymentAmount,@Cur_XAADenialCode,@Cur_XAClaimDenialCode,@Cur_XAANET;
							
							
				END  --END OF @@FETCH_STATUS = 0  


				---- Mark it as Processed (NOTE: To Handle Last Line processed)
				If  @XAFileHeaderID_Previous >0
					Update  [dbo].[XAdviceXMLParsedData] Set XAAdviceStatus = 1	Where XAFileHeaderFileID = @XAFileHeaderID_Previous	

			CLOSE AdvicePayment;  
			DEALLOCATE AdvicePayment; 


			--- Now Apply Payments to Bill

			Exec [dbo].[SPROC_ApplyPaymentToBill] @pCorporateID,@pFacilityID
			
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


