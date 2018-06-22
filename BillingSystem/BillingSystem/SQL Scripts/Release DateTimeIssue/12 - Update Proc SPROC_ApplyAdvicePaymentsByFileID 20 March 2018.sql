IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplyAdvicePaymentsByFileID')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplyAdvicePaymentsByFileID
GO

/****** Object:  StoredProcedure [dbo].[SPROC_ApplyAdvicePaymentsByFileID]    Script Date: 20-03-2018 16:53:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_ApplyAdvicePaymentsByFileID]
(  
@pCorporateID int,
@pFacilityID int,
@pFileId int
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
					@Cur_XAAActivityID nvarchar(20),@Cur_XAAPaymentAmount decimal (18,2),@Cur_XAADenialCode nvarchar(50),@Cur_XAClaimDenialCode nvarchar(50),@Cur_XAANET decimal (18,2),
					@Cur_FacilityId int,@Cur_CorpoarteId int
			--- Declare Other Variables
			DECLARE @CurrentDate DATETIME =@LocalDateTime, @BillNumber nvarchar(50),@PatientID INT, @EncounterID INT, @PayerID nvarchar(20),@BillCorprateID int, @BillFacilityID int,
							@AmountPaid numeric (18,2), @PaymentStatus int, @XAFileHeaderID_Previous bigint = 0;
			
			--set language british
			-- Declare Cursor with Closed Order but not on any Bill
			Update [XAdviceXMLParsedData] SET [XACDateSettlement] = Replace([XACDateSettlement],'T',' ') 
			where XAAdviceStatus is NULL and CorporateID =@pCorporateID and FacilityID = @pFacilityID ;

			BEGIN TRY
				Select Cast([XACDateSettlement] as Datetime)
				from [XAdviceXMLParsedData]
				where XAAdviceStatus is NULL and CorporateID =@pCorporateID and FacilityID = @pFacilityID ;
				set language Us_english
			END TRY
			BEGIN CATCH
			set language british
			END CATCH


			DECLARE AdvicePayment CURSOR FOR
			Select Cast([XAdviceXMLParsedDataID] as Bigint),Cast([XAFileHeaderFileID] as bigint),Cast([XACClaimID] as int),[XACIDPayer],[XACProviderID],[XACDenialCode],[XACPaymentReference],
			Cast([XACDateSettlement] as datetime),[XAAActivityID] ,Cast([XAAPaymentAmount] as decimal(18,2)),[XAADenialCode],[XACDenialCode],Cast([XAANet] as Decimal(18,2))
			,CorporateID,FacilityID 
			from [dbo].[XAdviceXMLParsedData] where XAAdviceStatus is NULL and CorporateID = @pCorporateID and FacilityID = @pFacilityID And XAFileHeaderFileID = @pFileId;
			

				OPEN AdvicePayment;  

				
				FETCH NEXT FROM AdvicePayment INTO @Cur_XAdviceXMLParsedDataID, @Cur_XAFileHeaderFileID, @Cur_XACClaimID,@Cur_XACIDPayer,@Cur_XACProviderID,
					@Cur_XACDenialCode,@Cur_XACPaymentReference,@Cur_XACDateSettlement,@Cur_XAAActivityID,@Cur_XAAPaymentAmount,@Cur_XAADenialCode,@Cur_XAClaimDenialCode,@Cur_XAANET
					,@Cur_CorpoarteId,@Cur_FacilityId		
				
				WHILE @@FETCH_STATUS = 0  
				BEGIN  

				--- Reset for each line
				Set @PaymentStatus = 0 
				Set @BillNumber = NULL
				
				----- Match the Cliam with Bill									
				Select @BillNumber=BillNumber,@PatientID=PatientID,@EncounterID=EncounterID,@PayerID=PayerID,@BillFacilityID=FacilityID,@BillCorprateID=CorporateID 
				from  BillHeader Where BillheaderID = @Cur_XACClaimID and FacilityId =@Cur_FacilityId and CorporateID =@Cur_CorpoarteId

								----- Set Payment Status based on incoming file XML Data - STARTS
				If @BillNumber is NULL
					Set @PaymentStatus = 1000 --- Not Matched in our System
				
				If @PaymentStatus <> 1000
				BEGIN
				--- Claim Level Denied
					if @Cur_XAClaimDenialCode is NULL OR @Cur_XAClaimDenialCode = '' OR @Cur_XAClaimDenialCode = ' '
					Begin

						if @Cur_XAADenialCode is  NULL OR @Cur_XAADenialCode = '' OR @Cur_XAADenialCode = ' '
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
							ELSE
							BEGIN
								If isnull(@Cur_XAANET,0) = isnull(@Cur_XAAPaymentAmount,0)
									Set @PaymentStatus = 100  --- Fully Paid
							END
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
				If(CHARINDEX('-', @Cur_XAAActivityID) > 0) -- 3rd Party XMl file 
				BEGIn
					Insert into [dbo].[Payment]([PayType],[PayDate],[PayAmount],[PayReference],[PayBillNumber],[PayFor],[PayBy],[PayBillID],[PayActivityID],
							[PayEncounterID],[PayFacilityID],[PayCorporateID],[PayXAFileHeaderID],[PayXAAdviceID],[PayNETAmount],[PayXADenialCode],[PayStatus],[PayCreatedBy],[PayCreatedDate],[PayIsActive],[XActivityParsedId])
					Select '100',@Cur_XACDateSettlement,@Cur_XAAPaymentAmount,@Cur_XACPaymentReference,@BillNumber,@PatientID,@PayerID,@Cur_XACClaimID,NULL,
							@EncounterID,@BillFacilityID,@BillCorprateID,@Cur_XAFileHeaderFileID,@Cur_XAdviceXMLParsedDataID,@Cur_XAANET,@Cur_XAADenialCode,@PaymentStatus,1,@CurrentDate,1,@Cur_XAAActivityID
				END
				ELSE -- Internal XMl file 
				BEGIN 
					Insert into [dbo].[Payment]([PayType],[PayDate],[PayAmount],[PayReference],[PayBillNumber],[PayFor],[PayBy],[PayBillID],[PayActivityID],
							[PayEncounterID],[PayFacilityID],[PayCorporateID],[PayXAFileHeaderID],[PayXAAdviceID],[PayNETAmount],[PayXADenialCode],[PayStatus],[PayCreatedBy],[PayCreatedDate],[PayIsActive],[XActivityParsedId])
					Select '100',@Cur_XACDateSettlement,@Cur_XAAPaymentAmount,@Cur_XACPaymentReference,@BillNumber,@PatientID,@PayerID,@Cur_XACClaimID,@Cur_XAAActivityID,
							@EncounterID,@BillFacilityID,@BillCorprateID,@Cur_XAFileHeaderFileID,@Cur_XAdviceXMLParsedDataID,@Cur_XAANET,@Cur_XAADenialCode,@PaymentStatus,1,@CurrentDate,1,NUll
				END
				---- Mark it as Processed 
				If @XAFileHeaderID_Previous <> @Cur_XAFileHeaderFileID and  @XAFileHeaderID_Previous >0
					Update  [dbo].[XAdviceXMLParsedData] Set XAAdviceStatus = 1	Where XAFileHeaderFileID = @XAFileHeaderID_Previous				
			 
				Set @XAFileHeaderID_Previous = @Cur_XAFileHeaderFileID
									
				FETCH NEXT FROM AdvicePayment INTO @Cur_XAdviceXMLParsedDataID, @Cur_XAFileHeaderFileID, @Cur_XACClaimID,@Cur_XACIDPayer,@Cur_XACProviderID,
				@Cur_XACDenialCode,@Cur_XACPaymentReference,@Cur_XACDateSettlement,@Cur_XAAActivityID,@Cur_XAAPaymentAmount,@Cur_XAADenialCode,@Cur_XAClaimDenialCode,@Cur_XAANET
				,@Cur_CorpoarteId,@Cur_FacilityId	
							
				END  --END OF @@FETCH_STATUS = 0  


				---- Mark it as Processed (NOTE: To Handle Last Line processed)
				If  @XAFileHeaderID_Previous >0
					Update  [dbo].[XAdviceXMLParsedData] Set XAAdviceStatus = 1	Where XAFileHeaderFileID = @XAFileHeaderID_Previous	

			CLOSE AdvicePayment;  
			DEALLOCATE AdvicePayment; 


			--- Now Apply Payments to Bill
			set language Us_english
			-- Commented for now have to uncomment after one cycle test ***************************************************************
			Exec [dbo].[SPROC_ApplyPaymentToBillByFileId] @pCorporateID,@pFacilityID,@pFileId
			-- Commented for now have to uncomment after one cycle test ***************************************************************
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


