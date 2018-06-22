IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocSaveAndApplyPayments') 
  DROP PROCEDURE SprocSaveAndApplyPayments;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE SprocSaveAndApplyPayments
(
@pCId bigint,
@pFId bigint,
@pDateTime datetime,
@pPaymentId bigint=0,
@pPReference nvarchar(100),
@pPayDate datetime,
@pPTypeId nvarchar(20),
@pPAmount nvarchar(100),
@pCardNo nvarchar(100),
@pExpiryMonth smallint,
@pExpiryYear smallint,
@pCardHolderName nvarchar(100),
@pSecurityNo nvarchar(10),
@pBillNumber nvarchar(100),
@pBillHeaderId bigint,
@pUserId bigint=0,
@pPType nvarchar(100),
@pPayFor_Id bigint,
@pPayBy_Id bigint,
@pEId bigint=0
)
AS
BEGIN
	Declare @LocalDateTime datetime, @TimeZone nvarchar(50)
	SET @TimeZone=(Select TimeZ from Facility Where Facilityid=@pFId)
	SET @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,GETDATE()))

	Declare @ExeResult bit=0


	/* Save Payment Details Section */
	If @pPaymentId > 0
	Begin
		Update Payment Set PayType=@pPType,PayAmount=@pPAmount,PayReference=@pPReference,PayBillNumber=@pBillNumber
		,PayBillID=@pBillHeaderId,PaymentTypeId=@pPTypeId,PayIsActive=1,PayModifiedBy=@pUserId,PayModifiedDate=@LocalDateTime
		,PayDate=@pPayDate,PayFor=@pPayFor_Id,PayBy=@pPayBy_Id
		Where PaymentID=@pPaymentId
		SET @ExeResult=1
	End
	Else
	Begin
		INSERT INTO Payment (PayType,PayAmount,PayReference,PayBillNumber,PayBillID,PaymentTypeId
		,PayIsActive,PayCreatedBy,PayCreatedDate,PayDate,PayFor,PayBy,PayEncounterID,PayCorporateID,PayFacilityID)
		Select @pPType,@pPAmount,@pPReference,@pBillNumber,@pBillHeaderId,@pPTypeId
		,1 as PayIsActive,@pUserId,@LocalDateTime,@pPayDate,@pPayFor_Id,@pPayBy_Id,@pEId,@pCId,@pFId

		Set @pPaymentId = SCOPE_IDENTITY()
		
		SET @ExeResult=1
	End
	/* Save Payment Details Section */


	/* Save Payment Type Details Section */
	IF @ExeResult =1
	Begin
		SET @ExeResult=0
		
		Declare @PaymentTypeDetailId bigint=0
		Select TOP 1 @PaymentTypeDetailId=Id From PaymentTypeDetail Where Id=@pPTypeId
		
		IF @PaymentTypeDetailId>0
		Begin
			Update PaymentTypeDetail Set PaymentId=@pPaymentId
			,PaymentType=@pPTypeId,CardHolderName=@pCardHolderName,CardNumber=@pCardNo,ExpiryMonth=@pExpiryMonth
			,ExpiryYear=@pExpiryYear,CreatedBy=@pUserId,CreatedDate=@LocalDateTime,ExtValue1=@pSecurityNo
			Where Id=@PaymentTypeDetailId
			SET @ExeResult=1
		End
		Else
		Begin
			INSERT INTO PaymentTypeDetail (PaymentId,PaymentType,CardHolderName,CardNumber,ExpiryMonth,ExpiryYear
			,CreatedBy,CreatedDate,ExtValue1)
			Select @pPaymentId,@pPTypeId,@pCardHolderName,@pCardNo,@pExpiryMonth,@pExpiryYear
			,@pUserId,@LocalDateTime,@pSecurityNo
			SET @ExeResult=1
		End
	End	
	/* Save Payment Type Details Section */


	/* Apply Payments Section */
	IF @ExeResult =1
	Begin
		SET @ExeResult=0
		
		Exec SPROC_ApplyPaymentManualToBill @pCorporateID=@pCId,@pFacilityID=@pFId

		SET @ExeResult=1
	End
	/* Apply Payments Section */

	/* The below Stored Proc will return the following:
	1. Payment List 
	2. Payment Details Header 
	3. List of Patients for Dropdown
	*/
	Exec SprocGetPaymentsList @pFId,@pBillHeaderId,@pUserId,@pPayFor_Id,@pEId,@pBillNumber,@pCId,@pPaymentId	
END
GO
