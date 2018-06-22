IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SendEClaimByPayerIDs')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SendEClaimByPayerIDs

/****** Object:  StoredProcedure [dbo].[SendEClaimByPayerIDs]    Script Date: 4/9/2018 12:50:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<BB>
-- Create date: <Nov-2014>
-- Description:	<Sending E Claims>
-- =============================================
CREATE PROCEDURE [dbo].[SendEClaimByPayerIDs]   -- [SendEClaimByPayerIDs] 8,'test',3,'0'
	(
	@SenderID int, ---- FacilityID
	@DispositionFlag nvarchar(100),
	@PayerID nvarchar(1000),
	@BillHeaderIds nvarchar(1000)
	)
AS
BEGIN
	SET @DispositionFlag = 'PRODUCTION'
	Declare @FetchEID int,@ID int,@FacilityNumber nvarchar(50),@BillHeaderStatus int, @NextStatus int, @FacilityType nvarchar(20)=0

--------- Get FacilityNumber based on @SenderID (which is FacilityID been sent)
    Select @FacilityNumber=FacilityNumber,@FacilityType=isnull(cast(Facilityrelated as nvarchar(20)),'0') FROM Facility Where FacilityID = @SenderID
	
	IF ISNULL(@BillHeaderIds,'')=''
		SET @BillHeaderIds=''

	IF(@BillHeaderIds !='')
		SET @PayerID='0'


---- Get all Encounters ready for Claim Submission
	Declare QD1 Cursor for
	(
	   Select distinct(BillheaderID),Status from [dbo].[BillHeader] where [Status] IN (55,105,155) 
	   AND FacilityID = @SenderID
	   --And PayerId IN (Select IDValue From dbo.Split(',',@PayerIDs))
	   --And ((BillHeaderID IN (Select IDValue From dbo.Split(',',@BillHeaderIds))) or PayerId=@PayerID)
	   --And ((BillHeaderID=0 or BillHeaderID IN (Select IDValue From dbo.Split(',',@BillHeaderIds)) OR ISNULL(@BillHeaderIds,'') = '') 
	   --And PayerID = @PayerID)
	   AND ((@PayerID='0' AND @BillHeaderIds !='') OR PayerId IN (Select IDValue From dbo.Split(',',@PayerID)))
	   And ((@BillHeaderIds='' AND @PayerID!='0') OR BillHeaderID IN (Select IDValue From dbo.Split(',',@BillHeaderIds)))
	)
	
	Open QD1;
	Fetch Next from QD1 into @FetchEID,@BillHeaderStatus;

While @@FETCH_STATUS = 0
BEGIN
		
-- Prepare XML under Claim parts - STARTS
		--Print 'XClaim Info Start'
-- Prepare XClaim Info
		insert into XClaim ([ClaimId],[EncounterID],[IDPayer],[MemberID],[PayerID] ,[ProviderID],[EmiratesIDNumber],[Gross],[PatientShare],[Net],[FacilityID],[FType]
								,[PatientID],[EligibilityIDPayer],[StartDate],[EndDate],[StartType],[EndType],[TransferSource],[TransferDestination],MCDiscount)
					(Select  B.BillHeaderID,B.EncounterID,B.PayerID,B.MemberID,B.PayerID,@FacilityNumber,P.PersonEmiratesIDNumber ,(B.PatientShare+B.PayerShareNet),B.PatientShare,B.PayerShareNet,@FacilityNumber,'0',
					B.PatientID,A.AuthorizationCode,E.EncounterStartTime,ISNULL(E.EncounterEndTime, cast(E.EncounterDischargeLocation as Datetime)) 'EncounterEndTime',E.EncounterStartType ,ISNULL(E.EncounterEndType,6) 'EncounterEndType' ,E.EncounterTransferSource,E.EncounterTransferDestination,MCDiscount 
					from Billheader B
					inner join Patientinfo P on P.PatientID = B.PatientID
					inner join Encounter E on E.EncounterID = B.EncounterID
					LEFT OUTER join [Authorization] A on A.EncounterID = B.EncounterID
					Where B.BillHeaderID = @FetchEID and B.Status = @BillHeaderStatus)
		--Print 'XClaim Info End'
	-- Prepare XEncounter Info
		--- TMS ISSUE NUMBER: 21959
		--- Bugs fixes Before XMl billing Demo 
		--What -- Changed the @Facilitytype which is being stored AS Ftype NOw it is will store the EncounterPatientType from encounter Table, 
		--Why -- Because in the front end it will always show as Inpatient if we import or export the XML file, 
		--When -- 3rd March 2016,
		--Who -- Shashank Awasthy
		--Print 'XEncounter Info Start'
		insert into XEncounter ([EncounterID],[IDPayer],[MemberID],[PayerID] ,[ProviderID],[EmiratesIDNumber],[Gross],[PatientShare],[Net],[FacilityID],[FType]
								,[PatientID],[EligibilityIDPayer],[StartDate],[EndDate],[StartType],[EndType],[TransferSource],[TransferDestination],[ClaimID])
					(Select  B.EncounterID,B.PayerID,B.MemberID,B.PayerID,@FacilityNumber,P.PersonEmiratesIDNumber ,(B.PatientShare+B.PayerShareNet),B.PatientShare,B.PayerShareNet,@FacilityNumber,EncounterPatientType,
					B.PatientID,A.AuthorizationCode,E.EncounterStartTime,ISNULL(E.EncounterEndTime, cast(E.EncounterDischargeLocation as Datetime)) 'EncounterEndTime',E.EncounterStartType ,ISNULL(E.EncounterEndType,6) 'EncounterEndType' ,E.EncounterTransferSource,E.EncounterTransferDestination,@FetchEID 
					from Billheader B
					inner join Patientinfo P on P.PatientID = B.PatientID
					inner join Encounter E on E.EncounterID = B.EncounterID
					LEFT OUTER join [Authorization] A on A.EncounterID = B.EncounterID
					Where B.BillHeaderID = @FetchEID and B.Status = @BillHeaderStatus)

		--Print 'XEncounter Info End'

		--Print 'XActivity Info Start'
		insert into XActivity ([EncounterID],[ActivityID],[DType],[DCode],[StartDate],[AType],[ACode],[Quantity],[OrderingClinician]
								,[OrderDate],[Clinician],[OrderCloseDate],[PriorAuthorizationID],[Gross],[PatientShare],[Net],[ClaimID],[MCDiscount],[XActivityParsedId])
						(Select BA.EncounterID,BA.BillActivityID,BA.ActivityType,BA.DiagnosisCode,BA.ActivityStartDate,BA.ActivityType, BA.ActivityCode, BA.QuantityOrdered, (Select TOP 1 ISNULL(PhysicianLicenseNumber,'') From Physician Where UserId = BA.OrderingClinicianID)
								,BA.OrderedDate,(Select TOP 1 ISNULL(PhysicianLicenseNumber,'') From Physician Where UserId = BA.AdminstratingClinicianID),BA.OrderCloseDate,BA.AuthorizationCode,BA.Gross,BA.PatientShare,BA.PayerShareNet,@FetchEID,MCDiscount,XActivityParsedId
						from BillActivity BA
						Where BA.BillHeaderID = @FetchEID)
		--Print 'XActivity Info End'
   -- Prepare XActivity Info

   ----- Update the status so it is not picked up again
   Set @NextStatus = [dbo].[GetBillNextStatus] (@BillHeaderStatus,1)

   Update BillHeader Set Status = @NextStatus where BillheaderID = @FetchEID

	
-- Prepare XML under Claim parts - ENDS

Fetch Next from QD1 into @FetchEID,@BillHeaderStatus;

END -- While Ends

--- CleanUP - STARTS
	Close QD1;
	Deallocate QD1;
--- CleanUP - ENDS

---XXXXXXXXXXXXXXXXXX----
-- CALL for XML generation - STARTS
Declare @X1 xml

Exec [dbo].[GenerateClaimFileByPayerIDs]  @BillHeaderIds,@SenderID,@PayerID,@DispositionFlag,@X1 output

Select @X1 as 'XMLOUT'
-- CALL for XML generation - ENDS
	
END --- Procedure ENDS





GO


