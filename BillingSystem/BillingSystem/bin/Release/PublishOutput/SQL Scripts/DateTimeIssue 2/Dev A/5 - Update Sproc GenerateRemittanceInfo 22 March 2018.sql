IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'GenerateRemittanceInfo')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE GenerateRemittanceInfo

/****** Object:  StoredProcedure [dbo].[GenerateRemittanceInfo]    Script Date: 3/22/2018 6:26:44 PM ******/
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
CREATE PROCEDURE [dbo].[GenerateRemittanceInfo]
	(
	@PassedInClaim int,
	@CorporateID int, 
	@FacilityID int=8
	)
AS
BEGIN
		Declare @LocalDateTime datetime  = (Select dbo.GetCurrentDatetimeByEntity(@FacilityID))

Declare  @CurrentDate date = @LocalDateTime, @XClaimID int

--- To Get Top/Most recent file sent for Claims (Same/Repeated/Resent BillID)
	Select @XClaimID = max(XClaimID)from  XClaim Where ClaimID = @PassedInClaim

insert into [XPaymentReturn] 
			([SenderID],[ReceiverID],[TransactionDate],[RecordCount],[DispositionFlag],[ID],[IDPayer],[ProviderID],[DenialCode],[PaymentReference],[DateSettlement],[FacilityID],
			[AActivityID],[AStart],[AType],[ACode],[AQuantity],[AANet],[AAOrderingClinician],[AAPriorAuthorizationID],[AAGross],[AAPatientShare],[AAPaymentAmount],[AADenialCode],
			[XModifiedBy],[XModifiedDate],[AdviceStatus],[XCorporateID],[XFacilityID])

	Select XF.SenderID, XF.ReceiverID,@CurrentDate,1,XF.DispositionFlag,XC.ClaimID,XC.IDPayer,XC.ProviderID,'','Paid by NEFT-TESTBank',@CurrentDate,XC.FacilityID,
			XA.ActivityID,XA.StartDate,XA.AType,XA.ACode,XA.Quantity,XA.Net,XA.OrderingClinician,XA.PriorAuthorizationID,XA.Gross,XA.PatientShare,XA.Net,'',
			1,@CurrentDate,0,@CorporateID,@FacilityID
	from XClaim XC
	inner join XActivity XA on XA.EncounterID = XC.EncounterID and XA.FileID = XC.FileID and XA.ClaimID = XC.ClaimID
	inner Join XFileHeader XF on XF.FileID = XC.FileID
	Where XC.ClaimID = @PassedInClaim and XC.XClaimID = @XClaimID
END













GO


