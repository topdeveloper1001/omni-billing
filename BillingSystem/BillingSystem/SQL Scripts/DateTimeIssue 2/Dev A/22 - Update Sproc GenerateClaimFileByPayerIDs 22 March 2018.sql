IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'GenerateClaimFileByPayerIDs')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE GenerateClaimFileByPayerIDs

/****** Object:  StoredProcedure [dbo].[GenerateClaimFileByPayerIDs]    Script Date: 3/22/2018 7:43:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GenerateClaimFileByPayerIDs]
	(
	@ClaimIDs nvarchar(1000),
	@SenderID int,			--This is the Facility ID from the Facility table (Not the actual SenderID)
	@PayerID nvarchar(100),
	@DispositionFlag nvarchar(100),
	@XMLFile XML output
	)
AS
BEGIN
	 
	/*Here @SenderID is referred to as Facility ID and 
	@FSenderID is the Sender ID from the table Facility.*/

	Declare @RecordCount int,@ID bigint,@FetchEID bigint,@AID bigint,@HeaderXMLDesc nvarchar(500),@XH xml,@XC xml,@XE xml,@XA xml,@XO xml,
			@CorporateID int,@FacilityNumber nvarchar(50), @FSenderID nvarchar(100), @ReceiverID nvarchar(100),@XD xml,@EID int,@PN xml,@PNC xml, @PName nvarchar(50)
	Declare @PatientId int= 0,@EncType nvarchar(20)='2',  @LocalDate Datetime;
		--- Get Time Zone From the Facility.
 	set @LocalDate = (Select dbo.GetCurrentDatetimeByEntity(@SenderID))
----- Get CorporateID and FacilityNumber based on @SenderID (which is FacilityID been sent)
		Select @CorporateID = CorporateID, @FacilityNumber=FacilityNumber, @FSenderID = ISNULL(SenderID,'') from Facility Where FacilityID = @SenderID
		
		--Added by Amit Jain on 26 June 2015
		Select @ReceiverID = ISNULL(ExternalValue2,'') From BillingSystemParameters Where FacilityNumber = @FacilityNumber

	 Select @RecordCount=Count(1) from [dbo].[XClaim] where  Status is NULL and ProviderID = @FacilityNumber
	 --And PayerID IN (Select IDValue From dbo.Split(',',@PayerIDs));
	 And ((ClaimID=0 or ClaimID IN (Select IDValue From dbo.Split(',',@ClaimIDs)) OR ISNULL(@ClaimIDs,'') = '') AND PayerID=@PayerID);

		
If @RecordCount > 0 
Begin
	 -- Handling next number for XFileHeader - STARTS

		/* Earlier, The Insert Query of XFIleHeader was executed just after the below cursor but now it will be executed before that.
		WHY: To Get the latest File ID From File Header after the actual record get inserted into this, unlike previous case.
		 */
		/* Below Changes by Amit Jain start here, 13 September, 2017 */
		Insert into [dbo].[XFileHeader] 
		values('REQ',@FSenderID,@ReceiverID,@LocalDate,@RecordCount,@DispositionFlag,'','1',@LocalDate,'',NULL,'UspGenerateClaimFile',@LocalDate,@CorporateID,@SenderID)
		
		SET @ID = SCOPE_IDENTITY();

		--SET @ID = (Select max(FileID) from [dbo].[XFileHeader])
		--If @ID is not Null
		--	Set @ID = @ID + 1
		--Else
		--	Set @ID = 1
		/* Below Changes by Amit Jain end here, 13 September, 2017 */


		-- Handling next number for XFileHeader - ENDS

--- Prepare XML Header part - STARTS
	
	
	/*Changes by Amit Jain on 26 June, 2015
	Purpose: Earlier Sender ID was considered as Facility Number and now, it is taken as separate column in the table Facility.
	Also, Receiver ID will be coming from BillingSystemParameter table 
	*/
--Set @XH = (Select @FacilityNumber as SenderID,@PayerID as ReceiverID,getdate() as TransactionDate,@RecordCount as RecordCount,@DispositionFlag as DispositionFlag for XML PATH ('Header'))
Set @XH = (Select @FSenderID as SenderID,@ReceiverID as ReceiverID,@LocalDate as TransactionDate,@RecordCount as RecordCount,
@DispositionFlag as DispositionFlag for XML PATH ('Header'))

--- Prepare XML Header part - ENDS

---- Get all Encounters ready for Claim Submission
Declare QD1 Cursor for
	(
	   Select ClaimID from [dbo].[XClaim] where Status is NULL and ProviderID = @FacilityNumber
	   --And PayerID IN (Select IDValue From dbo.Split(',',@PayerIDs))
	   --And ((ClaimID IN (Select IDValue From dbo.Split(',',@ClaimIDs)))OR PayerID=@PayerID)
	   And ((ClaimID=0 or ClaimID IN (Select IDValue From dbo.Split(',',@ClaimIDs)) OR ISNULL(@ClaimIDs,'') = '') AND PayerID=@PayerID)
	)
	
	Open QD1;
	Fetch Next from QD1 into @FetchEID;

While @@FETCH_STATUS = 0
BEGIN
		
	--Select @EID = EncounterID from [dbo].[XEncounter] where ClaimID = @FetchEID 
	--- Line commented above now we get thepatientId too from the XEncounter table to fetch the PatientPolicy name from patinetID
	Select @EID = EncounterID,@PatientId = PatientID from [dbo].[XEncounter] where ClaimID = @FetchEID
	
	-- Patient id is hard coded in the below line
	--Select @PName=PolicyName from InsurancePolices where InsurancePolicyId = (Select InsurancePolicyId from PatientInsurance where PatientId=1018)
	-- Now the Id will be fetched from the XEncounter table
	Select @PName=PolicyName from InsurancePolices where InsurancePolicyId = (Select InsurancePolicyId from PatientInsurance where PatientId=@PatientId)
	--- We fetch the EncType for the field Type in the XML  (Previously it is used as the Facility type now it will be ENcounter type)
	--Select @EncType = EncounterType from Encounter where EncounterId = @EID
-- Print @PName

-- Prepare XML under Claim parts - STARTS
	
	Set @XE = (Select FacilityID as FacilityID,FType as 'Type',PatientID as PatientID,EligibilityIDPayer as EligibilityIDPayer,StartDate as Start,ISNULL(CAST(Case EndDate When NULL Then '' Else EndDate End AS nvarchar),'') as 'End',StartType as StartType, EndType as EndType,TransferSource as TransaaferSource,TransferDestination as TransferDestination  from [dbo].[XEncounter] where ClaimID = @FetchEID and Status is NULL for XML PATH ('Encounter'))
	--Set @XA = (Select ActivityID as ID,StartDate as Start,AType as 'Type',ACode as Code,Quantity as Quantity,Net as Net,OrderingClinician as OrderingClinician,Clinician as Clinician,PriorAuthorizationID as PriorAuthorizationID from [dbo].[XActivity] where ClaimID = @FetchEID and Status is NULL for XML PATH ('Activity'))
	-- Above line is commented -- Incase of Service Code STartDate is always same
	--- Bugs fixes Before XMl billing Demo
	--What --Changed the Startdate to OrderDate 
	--Why -- Because in the case of the Service Code the Data is always coming for one date which is Start date of the Bill, 
	--When -- 3rd March 2016,
	--Who -- Shashank Awasthy
	--Set @XA = (Select Case when XActivityParsedId IS NULL Then CAST(ActivityID as Nvarchar(20)) ELSE  CAST(XActivityParsedId as Nvarchar(20)) END as ID,OrderDate as Start,AType as 'Type',ACode as Code,Quantity as Quantity,Net as Net,OrderingClinician as OrderingClinician,Clinician as Clinician,PriorAuthorizationID as PriorAuthorizationID from [dbo].[XActivity] where ClaimID = @FetchEID and Status is NULL for XML PATH ('Activity'))
	Set @XA = (Select ActivityID as ID,OrderDate as Start,AType as 'Type',ACode as Code,Quantity as Quantity,Net as Net,OrderingClinician as OrderingClinician,Clinician as Clinician,PriorAuthorizationID as PriorAuthorizationID from [dbo].[XActivity] where ClaimID = @FetchEID and Status is NULL for XML PATH ('Activity'))
	Set @XD = (Select (CASE WHEN DiagnosisType=1 THEN 'Principal' ELSE 'Secondary' END) as 'Type',DiagnosisCode as 'Code' from Diagnosis Where EncounterID = @EID for XML PATH ('Diagnosis'))
	Set @PN = (Select @PName for XML PATH ('PackageName'))
	Set @PNC =  (Select @PN for XML PATH ('Contract'))
	Set @XC = (Select ClaimID as ID,IDPayer as IDPayer, MemberID as MemberID,PayerID as PayerID,ProviderID as ProviderID,EmiratesIDNumber as EmiratesIDNumber,Gross as Gross, PatientShare as PatientShare,Net as Net, @XE,@XD,@XA,@PNC  from [dbo].[XEncounter] where ClaimID = @FetchEID and Status is NULL for XML PATH ('Claim'))
	
	
	Set @XMLFile = ( Select @XMLFile,@XC for XML PATH (''))

-- Prepare XML under Claim parts - ENDS
	
	Declare @BillHeaderStatus nvarchar(20)= '', @NextStatus nvarchar(20)


	Select @BillHeaderStatus=[Status] From BillHeader Where BillHeaderID = @FetchEID;

	Set @NextStatus = [dbo].[GetBillNextStatus] (@BillHeaderStatus,1)


	---- Update BillHeader to Note Which XML FileID it is on for future Tracking
	UpDate BillHeader Set ClaimID = @FetchEID, FileID = @ID, [Status] = @NextStatus	Where BillHeaderID = @FetchEID;

	--- Update FileID and Status on XTables to mean it is on XML file now
	Update XEncounter Set Status = 1, FileID = @ID Where ClaimID = @FetchEID
	Update XClaim Set Status = 1, FileID = @ID Where ClaimID = @FetchEID
	Update XActivity Set Status = 1, FileID = @ID Where ClaimID = @FetchEID

Fetch Next from QD1 into @FetchEID;

END -- While Ends

--- CleanUP - STARTS
	Close QD1;
	Deallocate QD1;
--- CleanUP - ENDS

--- Final Wrapper of Main XML - STARTS
		Set @XMLFile = ( Select @XH,@XMLFile for XML PATH ('Claim.Submission'))

--- Final Wrapper of Main XML - ENDS

-- SELECT @XMLFile

	/*Changes by Amit Jain on 26 June, 2015
	Purpose: Earlier Sender ID was considered as Facility Number and now, it is taken as separate column in the table Facility.
	*/
------- Finally Insert record into File Header to be sent
--Insert into [dbo].[XFileHeader] values(@ID,'REQ',@SenderID,@PayerID,GETDATE(),@RecordCount,@DispositionFlag,'','1',GETDATE(),'',NULL,'UspGenerateClaimFile',getdate(),@CorporateID,@SenderID)

--- Saving XML file in DB for Backup purpose if Network fails and to Check any issues on front end
Insert into [dbo].[XFileXML] values(@ID,@XMLFile,@LocalDate)


End -- Records If ENDS
	
END --- Procedure ENDS














GO


