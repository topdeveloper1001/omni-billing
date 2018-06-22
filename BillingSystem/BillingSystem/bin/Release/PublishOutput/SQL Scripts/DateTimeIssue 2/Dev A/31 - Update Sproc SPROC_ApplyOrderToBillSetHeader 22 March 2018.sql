IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplyOrderToBillSetHeader')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplyOrderToBillSetHeader
		 
GO

/****** Object:  StoredProcedure [dbo].[SPROC_ApplyOrderToBillSetHeader]    Script Date: 3/22/2018 8:03:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_ApplyOrderToBillSetHeader]
(  
@pEncounterID int,  --- EncounterID for whom the Order need to be processed  --- Input Param 
@BillHeaderID INT Output, 
@BillDetailLineNumber INT Output,
@BillNumber nvarchar(50) Output,
@AuthID INT Output, 
@AuthType INT Output, 
@AuthCode nvarchar(50) Output,
@SelfPayFlag bit Output,--- 1 Means Self Pay (NOTE Multiplier will be 1 if Self Payed) and Gross will be all in Patient Share, AuthCode = BLank)
@pReClaimFlag varchar(2),
@pClaimId bigint
)
AS
BEGIN
    DECLARE @CurrentDate DATETIME = (Select dbo.GetCurrentDatetimeByEntity(0)),@BillFormat nvarchar(20), @PayerID nvarchar(20), @MemberID nvarchar(20),
			@CorporateID INT,@FacilityID INT, @PatientID INT,@INSCompanyId INT,@INSPlanId INT,@INSPolicyId INT, @MCID INT,@BillHeaderStatus int = 0,
			@InsuranceCompanyLicenseNumber nvarchar(10) =''
			
			
		Select @CorporateID = corporateID,@FacilityID = EncounterFacility, @PatientID = PatientID from Encounter Where EncounterID = @pEncounterID

		-------- To comment after testing
			--Print 'Reclaim flag check'

		--	Select BillHeaderID,BillNumber,AuthID,AuthCode,
		--					EncounterSelfPayFlag,Status,@pEncounterID,@pClaimId
		--					from BillHeader 
		--					where EncounterID = @pEncounterID and BillHeaderID = @pClaimId;
		-------- To comment after testing
					---
						---- XXXXXXXXXXXXXXXXXXXXXXXXXXX----
						---- Check for any existing Bill which is yet not reached approved stage -- If present Use that one to Add Activities 
						if(@pReClaimFlag = '1')
							Select @BillHeaderID= BillHeaderID,@BillNumber=BillNumber,@AuthID= AuthID,@AuthCode= AuthCode,
							@SelfPayFlag = EncounterSelfPayFlag, @BillHeaderStatus = Status
							from BillHeader 
							where EncounterID = @pEncounterID and BillHeaderID = @pClaimId;
						ELSE
							Select @BillHeaderID= BillHeaderID,@BillNumber=BillNumber,@AuthID= AuthID,@AuthCode= AuthCode,
							@SelfPayFlag = EncounterSelfPayFlag, @BillHeaderStatus = Status
							from BillHeader 
							where EncounterID = @pEncounterID and Status <= 40;

						---- >>>>>> New Logic to Check AuthorizationCode All time --- Because it may Be Updated on Later Stage - STARTS

						----- XXXXXXXXXXXXXXXXXXXXXX ---- SET All DEFAULT VALUES as per Authorization, Insurance---- STARTS

							--- Get The Authorization Info eg: AuthCode,AuthType,PayerID and MemberID from EncounterID
							Set @AuthID = NULL

							Select @AuthID = A.AuthorizationID, @AuthType = A.AuthorizationType,@AuthCode = A.AuthorizationCode,
									@PayerID = A.AuthorizationIDPayer,@MemberID =A.AuthorizationMemberID  
							from [Authorization] A
							where EncounterID = @pEncounterID;

							-------- To comment after testing
							--Select A.AuthorizationID,A.AuthorizationType,A.AuthorizationCode,
							--		A.AuthorizationIDPayer,A.AuthorizationMemberID,@pEncounterID
							--from [Authorization] A
							--where EncounterID = @pEncounterID;
							-------- To comment after testing

							If @AuthID is NULL
								Set @BillHeaderStatus = 5  -- Means Authorization is Not Obtained so Cannot proceed to before premlimnary Bill  Status = 40

							---- Get the Patient Share if any set as per Contract/Manged Care -- STARTS
							--- First Get Insurance Details for a Patient

							Select @INSCompanyId = PIN.InsuranceCompanyId, @INSPlanId = PIN.InsurancePlanID, @INSPolicyId = PIN.InsurancePolicyID
							from PatientInsurance PIN
							Where PIN.PatientID = @PatientID and PIN.IsActive = 1

							---- New changes done--- as of the 1 march 2016

							if(@INSCompanyId is null)
								Set @INSCompanyId = @PayerID --- for the XML rule Check as Insureance Id is not provided by the user

							----- If it Self Pay then CompanyID = 999 - STARTS
							-- Changes Now the Self pay flag will be Considered for company having InsuranceCompanyLicenseNumber = '0000'
							If @INSCompanyId <> 0 
							Begin
							-- Added By Shashank for Slef Pay check on June 17 2015 Starts
								Select @InsuranceCompanyLicenseNumber = InsuranceCompanyLicenseNumber From  InsuranceCompany WHere InsuranceCompanyId = @INSCompanyId
							-- Added By Shashank for Slef Pay check on June 17 2015 END
								IF @InsuranceCompanyLicenseNumber <> '0000'
								BEGIn
									--- There is Insurance for their patient
										Set @SelfPayFlag = 0 --- Not Self Paid Status

									If @AuthID is NOT NULL and @BillHeaderStatus < 40
									Begin
									 ---- Check to See if it was below 40 Status than we Set Status
									 	 Set @BillHeaderStatus = 40  --- Means All OK to proceed for Prelimnary Bill AnyTime
									End
								END	
								ELSE
								-- Added By Shashank for Slef Pay check on June 17 2015 Starts
								Begin
									---- Meaning it is a Self Pay by Patient
									Set @SelfPayFlag = 1
									Set @BillHeaderStatus = 40  --- Means All OK to proceed for Prelimnary Bill AnyTime
								End
								-- Added By Shashank for Slef Pay check on June 17 2015 END
							End
							ELSE
							Begin
								---- Meaning it is a Self Pay by Patient
								Set @SelfPayFlag = 1
								Set @BillHeaderStatus = 40  --- Means All OK to proceed for Prelimnary Bill AnyTime
							End
		
							----- If it Self Pay then CompanyID = 999 - ENDS

							----- XXXXXXXXXXXXXXXXXXXXXX ---- SET All DEFAULT VALUES as per Authorization, Insurance, Managed Care ---- ENDS


						---- >>>>>> New Logic to Check Multiplier and AuthorizationCode All time --- Because it may Be Updated on Later Stage - STARTS
						

						Set @BillHeaderID= isnull(@BillHeaderID,0)
						If @BillHeaderID > 0
						BEGIN
							--- Bill Exists with Status yet Open - So use this to Add more lines/Actiities
							Select @BillDetailLineNumber = max(linenumber) from BillActivity where BillheaderID = @BillHeaderID;
							--Update BillHeader Set  Status = @BillHeaderStatus, AuthID=@AuthID,PayerID =@PayerID,MemberID= @MemberID  Where BillheaderID = @BillHeaderID;
							--- Change the AuthId to Policy Id so that we can view the Policy name in front end

							--Select @AuthID 'AuthID',@INSCompanyId 'PayerID',@BillHeaderID 'BillHeaderID'



							Update BillHeader Set  Status = @BillHeaderStatus, AuthID=@AuthID,PayerID =@INSCompanyId,MemberID= @MemberID,AuthCode=@AuthCode  Where BillheaderID = @BillHeaderID;
						END
						ELSE
						BEGIN
						-- Bill date must be gretaer then encounter start date time
						Declare @EncStartDate Datetime = (Select (EncounterStartTime) from Encounter where EncounterID = @pEncounterID);
						Set @EncStartDate = ISNULL(@EncStartDate,@CurrentDate)
						Set @EncStartDate = DateAdd(Minute,1,@EncStartDate)
						-- So we will add the 1 minute to the bill start date
							----- Create a new Bill Entry
							Insert into BillHeader (BillDate,CorporateID,FacilityID,PatientID,EncounterID,PayerID,MemberID,
													Gross,PatientShare,PayerShareNET,AuthID,AuthCode,EncounterSelfPayFlag,MCID,Status,CreatedBy,CreatedDate)
							Values(@EncStartDate,@CorporateID,@FacilityID,@PatientID,@pEncounterID,@INSCompanyId,@MemberID,
									0,0,0,@AuthID,@AuthCode,@SelfPayFlag,@MCID, @BillHeaderStatus,1,@CurrentDate)
						
							----- Get the Latest Bill Header ID
							Set @BillHeaderID = (Select max(BillHeaderID) from BillHeader where EncounterID = @pEncounterID);
							
							--- Prepare a New Bill Number as desired Format -- CorporateNumber,FacilityName(First4 Digit),YEAR,Month,EncounterID and random number - STARTS
							Set @BillFormat = (Select CorporateNumber from Corporate where CorporateID = @CorporateID);
							Set @BillNumber = isnull(@BillFormat,'-')
							Set @BillFormat = (Select FacilityName from Facility where FacilityId = @FacilityID);
							Set @BillNumber = @BillNumber+'/'+Substring(isnull(@BillFormat,'--'),0,5)
							Set @BillNumber = @BillNumber+'/'+ Cast(YEAR(@CurrentDate) as Nvarchar(10)) +'/'++ Cast(Month(@CurrentDate) as Nvarchar(10)) +'/'+Cast(@pEncounterID as Nvarchar(10))++'-'+Cast(Isnull(@BillHeaderID,0) as Nvarchar(10)) 
							
							
							Update BillHeader Set  BillNumber = @BillNumber Where BillheaderID = @BillHeaderID;

							--- Because it is a new Bill Set Line number to ZERO
							Set @BillDetailLineNumber = 0
							--- Prepare a New Bill Number as desired Format -- CorporateNumber,FacilityName(First4 Digit),YEAR,Month,EncounterID and random number - ENDS
						END  --- Else Part Ends	  -- Generate NEW BIll Header - ENDS
					  		
END





GO


