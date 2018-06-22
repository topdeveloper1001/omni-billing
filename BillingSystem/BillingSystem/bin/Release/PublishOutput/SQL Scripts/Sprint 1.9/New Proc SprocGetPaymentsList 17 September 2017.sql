IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocGetPaymentsList') 
  DROP PROCEDURE SprocGetPaymentsList;
 
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
CREATE PROCEDURE SprocGetPaymentsList
(
@pFId bigint,
@pBillHeaderId bigint,
@pUserId bigint=0,
@pPatientId bigint,
@pEId bigint=0,
@pBillNumber nvarchar(100)=null,
@pCId bigint=null,
@pPaymentId bigint=null
)
AS
BEGIN
	Declare @LocalDateTime datetime, @TimeZone nvarchar(50)
	SET @TimeZone=(Select TimeZ from Facility Where Facilityid=@pFId)
	SET @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,GETDATE()))

	/*List of Payment Custom Model */
	Select P.*
	,(Case 
	WHEN ISNULL(P.PayFor,0)=0 THEN ''
	Else (Select TOP 1 PT.PersonFirstName + ' ' + PT.PersonLastName From PatientInfo PT Where PT.PatientID=P.PayFor)
	End) As PayForPatientName
	,(Case 
	WHEN ISNULL(P.PayBy,0)=0 THEN ''
	Else (Select TOP 1 PT.PersonFirstName + ' ' + PT.PersonLastName From PatientInfo PT Where PT.PatientID=P.PayBy)
	End) As PayByPatientName
	,(Select TOP 1 E.EncounterNumber From Encounter E Where E.EncounterID=P.PayEncounterID) As EncounterNumber
	From Payment P Where (@pEId=0 OR P.PayEncounterID=@pEId)
	AND
	(@pPatientId=0 OR P.PayFor=@pPatientId)
	And (@pBillHeaderId=0 OR P.PayBillID=@pBillHeaderId)
	/*List of Payment Custom Model */


	/* Single Record of Payment Details by Bill Header ID, of Type 'PaymentDetailsCustomModel' */
	Declare @BillHeaderTable Table(Id bigint,PatientSharePayable decimal(10,2), InsSharePayable decimal(10,2)
	,GrossSharePayable decimal(10,2),PatientSharePaid decimal(10,2),InsSharePaid decimal(10,2),GrossSharePaid decimal(10,2),PatientShareBalance decimal(10,2)
	,InsShareBalance decimal(10,2),GrossShareBalance decimal(10,2),InsPayment decimal(10,2),InsTotalPaid decimal(10,2), InsApplied decimal(10,2)
	,InsUnapplied decimal(10,2),PatientPayment decimal(10,2),PatientTotalPaid decimal(10,2), PatientApplied decimal(10,2), PatientUnApplied decimal(10,2))

	Declare @BillHeaderId int, @expectedPaymentAmount decimal(10,2),@TotalPaidAmount decimal(10,2),@TotalAppliedAmount decimal(10,2),@TotalUNAppliedAmount decimal(10,2)

	INSERT INTO @BillHeaderTable (Id,PatientSharePayable,InsSharePayable,GrossSharePayable,PatientSharePaid,InsSharePaid,GrossSharePaid
	,PatientShareBalance,InsShareBalance,GrossShareBalance)
	Select BillHeaderID, CAST(ISNULL(PatientShare,0) as decimal) AS PatientSharePayable, CAST(ISNULL(PayerShareNet,0) as decimal) As InsSharePayable
	,CAST(ISNULL(PatientShare,0) + ISNULL(PayerShareNet,0) as decimal) As GrossSharePayable 
	,CAST(ISNULL(PatientPayAmount,0) as decimal) As PatientSharePaid,CAST(ISNULL(PaymentAmount,0) as decimal) As InsSharePaid
	,CAST(ISNULL(PatientPayAmount,0) + ISNULL(PaymentAmount,0) as decimal) As GrossSharePaid 
	,CAST(ISNULL(PatientShare,0) - ISNULL(PatientPayAmount,0) as decimal) As PatientShareBalance
	,CAST(ISNULL(PayerShareNet,0) - ISNULL(PaymentAmount,0) as decimal) As InsShareBalance
	,CAST(((ISNULL(PatientShare,0) - ISNULL(PatientPayAmount,0)) + (ISNULL(PayerShareNet,0) - ISNULL(PaymentAmount,0))) as decimal) As GrossShareBalance
	From BillHeader Where BillHeaderID=@pBillHeaderId

	--Get Total of Insurance Payments from Payment Table
	Select @expectedPaymentAmount=SUM(PayNETAmount),@TotalPaidAmount=SUM(PayAmount),@TotalAppliedAmount=SUM(PayAppliedAmount)
	,@TotalUNAppliedAmount=SUM(PayUnAppliedAmount)
	From Payment Where PayBillID=@pBillHeaderId And PayType=100

	Update @BillHeaderTable Set InsPayment=ISNULL(@expectedPaymentAmount,0),InsTotalPaid=ISNULL(@TotalPaidAmount,0)
	,InsApplied=ISNULL(@TotalUNAppliedAmount,0),InsUnapplied=ISNULL(@TotalUNAppliedAmount,0)


	--Get Total of Patient Payments from Payment Table
	Select @expectedPaymentAmount=SUM(PayNETAmount),@TotalPaidAmount=SUM(PayAmount),@TotalAppliedAmount=SUM(PayAppliedAmount)
	,@TotalUNAppliedAmount=SUM(PayUnAppliedAmount)
	From Payment Where PayBillID=@pBillHeaderId And PayType=500

	Update @BillHeaderTable Set PatientPayment=ISNULL(@expectedPaymentAmount,0),PatientTotalPaid=ISNULL(@TotalPaidAmount,0)
	,PatientApplied=ISNULL(@TotalUNAppliedAmount,0),PatientUnApplied=ISNULL(@TotalUNAppliedAmount,0)

	Select TOP 1 * From @BillHeaderTable
	/* Single Record of Payment Details by Bill Header ID, of Type 'PaymentDetailsCustomModel' */


	/* Get the list of Patients by Facility ID and Corporate ID*/
	Select (PersonFirstName + ' ' + PersonLastName) As [Text], CAST(PatientID as nvarchar) As [Value]
	,PersonEmiratesIDNumber As ExternalValue1,PersonMedicalRecordNumber As ExternalValue2
	From PatientInfo Where FacilityId=@pFId
	/* Get the list of Patients by Facility ID and Corporate ID*/
END
GO
