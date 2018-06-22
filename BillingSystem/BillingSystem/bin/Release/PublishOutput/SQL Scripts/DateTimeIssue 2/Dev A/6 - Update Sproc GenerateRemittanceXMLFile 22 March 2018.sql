
IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'GenerateRemittanceXMLFile')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE GenerateRemittanceXMLFile

/****** Object:  StoredProcedure [dbo].[GenerateRemittanceXMLFile]    Script Date: 3/22/2018 6:28:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<BB>
-- Create date: <Dec-2014>
-- Description:	<Generating XML file for Claim Sent--> Payment Remittance File>
-- New Table to Handle this is [dbo].[XPaymentReturn]
-- =============================================
create  PROCEDURE [dbo].[GenerateRemittanceXMLFile]  -- [GenerateRemittanceXMLFile] 1022,1056
	(
	@CorporateID int, 
	@FacilityID int
	)
AS
BEGIN

		Declare @LocalDateTime datetime, @TimeZone nvarchar(50)
  		SET @LocalDateTime = (Select dbo.GetCurrentDatetimeByEntity(@FacilityID))
Declare  @CurrentDate datetime = @LocalDateTime,@XH xml,	@XC xml,@XE xml,@XA xml,@MainXML xml,@ClaimID int, @SuccessFlag bit


Declare Pay1 Cursor for
	(
	   Select distinct(ID) from [dbo].[XPaymentReturn] Where [AdviceStatus] = 0
	)
	
	Open Pay1;
	Fetch Next from Pay1 into @ClaimID;

While @@FETCH_STATUS = 0
BEGIN
	
	-- Select * from  XpaymentReturn
	Set @XH = NULL
	Set @XC = NULL
	Set @XE = NULL
	Set @XA = NULL
	Set @MainXML = NULL

	Set @XH = (Select top(1) [SenderID],[ReceiverID],[TransactionDate],[RecordCount],[DispositionFlag] from [XPaymentReturn] Where ID = @ClaimID  and [AdviceStatus] = 0 for XML PATH ('Header'))
	Set @XE = (Select top(1) [FacilityID] from [XPaymentReturn] Where ID = @ClaimID and [AdviceStatus] = 0 for XML PATH ('Encounter'))
	Set @XA = (Select [AActivityID] 'ID',[AStart] 'Start',[AType] 'Type',[ACode] 'Code',[AQuantity] 'Quantity',[AANet] 'Net',[AAOrderingClinician] 'OrderingClinician',
				[AAPriorAuthorizationID] 'PriorAuthorizationID',[AAGross] 'Gross',[AAPatientShare] 'PatientShare',[AAPaymentAmount] 'PaymentAmount',[AADenialCode] 'DenialCode'
				from [XPaymentReturn] Where ID = @ClaimID and [AdviceStatus] = 0 for XML PATH ('Activity'))
	Set @XC = (Select top(1) [ID],[IDPayer],[ProviderID],[DenialCode],[PaymentReference],[DateSettlement],@XE,@XA from [XPaymentReturn] Where ID = @ClaimID and [AdviceStatus] = 0 for XML PATH ('Claim'))

	Set @MainXML = ( Select @MainXML,@XC for XML PATH (''))
	Set @MainXML = ( Select @XH,@MainXML for XML PATH ('Remittance.Advice'))
	--Set @MainXML = ( Select @XH,@MainXML for XML PATH ('Remittance.Advice'))
	insert into [dbo].[XPaymentFileXML]([XClaimID],[XPaymentFileXML],[XModifiedDate],[XModifiedBy],[XStatus])
				Select @ClaimID,@MainXML,@CurrentDate,1,0;

If @MainXML is NOT NULL
Begin
	--- Now call the Main Proc to handle incoming XML files and Parse it
	Exec [dbo].[XMLRemittanceAdviceParser] @MainXML,'TEST - FILE - Upload',@CorporateID,@FacilityID,@SuccessFlag output 

	--- Update the XpaymentTable not to Pick this Again
	Update [XPaymentReturn] set [AdviceStatus] = 1 Where ID = @ClaimID;
End



Fetch Next from Pay1 into @ClaimID;
END
	---- Insert into XML Table


--- CleanUP - STARTS
	Close Pay1;
	Deallocate Pay1;
--- CleanUP - ENDS

--- Now Apply the Payment
--Exec [dbo].[SPROC_ApplyAdvicePayments] @CorporateID,@FacilityID



END














GO


