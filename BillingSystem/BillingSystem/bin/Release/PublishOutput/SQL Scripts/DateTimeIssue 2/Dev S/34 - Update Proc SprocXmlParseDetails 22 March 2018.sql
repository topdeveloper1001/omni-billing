IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocXmlParseDetails')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocXmlParseDetails
GO

/****** Object:  StoredProcedure [dbo].[SprocXmlParseDetails]    Script Date: 22-03-2018 19:46:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--use XMLParse
--use USE [BillingSystem6OCT2014]
-- =============================================
-- Author:		<BB - Spadez>
-- Create date: <DEc- 2014>
-- Description:	<For Parsing INcoming Files from Third Parties - Format Claim Submission
-- =============================================
    
CREATE Proc [dbo].[SprocXmlParseDetails]
(          
	@pCID bigint,
	@pFID bigint,
	@pFileHeaderId bigint
)
AS
BEGIN

	Declare @PatientID bigint
	Declare @PIDTable table (EmirateID nvarchar(100), ExistStatus bit)
	Declare @FacilityName varchar(30)  = (Select FacilityName from Facility where FacilityId = @pFID)


	Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFID))

	---- Setting Date Format to Standard format --- Starts
	Update TPXMLParsedData SET EStart = Replace(Estart,'T',' '), EEnd = Replace(EEnd,'T',' '),AStart = Replace(Astart,'T',' ') 
	where PStatus is null And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)
	---- Setting Date Format to Standard format --- Ends

	---- Check incoming UserID (Physicians/Nurses) exists based on License number Exist --- STARTS
	Declare @UserTable table (LicenseNumber nvarchar(50), ExistStatus bit,UserID int)

		insert into @UserTable
			Select AOrderingClinician,0,0 from TPXMLParsedData where PStatus is null And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId) group by AOrderingClinician
		insert into @UserTable
			Select AExecutingClinician,0,0 from TPXMLParsedData where PStatus is null And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId) group by AExecutingClinician
			 
		--insert into users(CountryID, StateID, CityID, UserName, FirstName, LastName, [Password], AdminUser, IsActive,
		--					CreatedBy, CreatedDate,IsDeleted,UserType, CorporateId, FacilityId)
		--Select 45,3,5,LicenseNumber,LicenseNumber,LicenseNumber,'GyHl3AQLBV4=',0,1,1111,@CurrentDate,0,4,@pCID,@pFID
		--From @UserTable Where LicenseNumber not in (Select UserName from users where CorporateId = @pCID and FacilityId = @pFID)
		--Group by LicenseNumber

		insert into users(CountryID, StateID, CityID, UserName, FirstName, LastName, [Password], AdminUser, IsActive,
							CreatedBy, CreatedDate,IsDeleted, CorporateId, FacilityId)
		Select 45,3,5,LicenseNumber,LicenseNumber,LicenseNumber,'GyHl3AQLBV4=',0,1,1111,@CurrentDate,0,@pCID,@pFID
		From @UserTable Where LicenseNumber not in (Select UserName from users where CorporateId = @pCID and FacilityId = @pFID)
		Group by LicenseNumber
		 
		Update A  Set A.UserID = U.UserID from @UserTable A inner join Users U on U.UserName = A.LicenseNumber and U.CorporateId = @PCID and U.FacilityId = @pFID

		insert into Physician (PhysicianEmployeeNumber, PhysicianName, PhysicianLicenseNumber, PhysicianLicenseType, PhysicianLicenseEffectiveStartDate, 
                         PhysicianLicenseEffectiveEndDate, PhysicianPrimaryFacility, CreatedBy, CreatedDate,  IsDeleted, IsActive,UserType,UserId,FacilityId,CorporateId)
       	Select 1111,LicenseNumber,LicenseNumber,1,'2015-01-01','2020-01-01',@pFID, 1111, @CurrentDate,0,1,4,max(UserID),@pFID,@PCID
			From @UserTable Where LicenseNumber not in (Select PhysicianName from Physician where PhysicianPrimaryFacility = @pFID)
			Group by LicenseNumber

		---- Update Ordering Clinicians
	  Update A  Set A.OMOrderingClincialID = U.UserID from TPXMLParsedData A 
	  inner join Users U on U.UserName = A.AOrderingClinician and U.CorporateId = @pCID and U.FacilityId = @pFID
	  And (@pFileHeaderId=0 OR A.TPFileID=@pFileHeaderId)

	  ---- Update Executing/Adminstering Clinicians
	  Update A  Set A.OMExecutingClincialID = U.UserID from TPXMLParsedData A 
	  inner join Users U on U.UserName = A.AExecutingClinician and U.CorporateId = @pCID and U.FacilityId = @pFID
	  And (@pFileHeaderId=0 OR A.TPFileID=@pFileHeaderId)
	---- Check incoming UserID (Physicians/Nurses) exists based on License number Exist --- ENDS

	---- First Check incoming Patients Exist --- STARTS
		insert into @PIDTable
		Select CEmiratesIDNumber,0 from TPXMLParsedData where PStatus is null group by CEmiratesIDNumber 

		Update @PIDTable Set ExistStatus = 1 
		Where EmirateID IN (Select PersonEmiratesIDNumber from PatientInfo where CorporateId = @pCID and FacilityId = @pFID)

		---- MISSING KEY INFO (We need this from SEHA - Get it later) --->>> PersonBirthDate, PersonBirthTime, PersonAge,PersonEstimatedAge, PersonMaritalStatus, PersonType,
		insert into PatientInfo(PersonEmiratesIDNumber, PersonMedicalRecordNumber,PersonMasterPatientNumber,PersonPayerID, PersonInsuranceCompany, 
                          PersonLastName, PersonFirstName, PersonSecondName, PersonNationality,  FacilityId, CorporateId,CreatedBy, CreatedDate,  IsDeleted,PersonBirthDate,PersonGender,PersonType)
			Select  CEmiratesIDNumber,max(CMemberID), max(EPatientID), max(CPayerID),max(CPayerID), 
					CEmiratesIDNumber As LastName,CEmiratesIDNumber As FirstName,CEmiratesIDNumber As SecondName,45,@pFID,@pCID,1111,@CurrentDate,0,'01/01/1900','Male','XML'
			From TPXMLParsedData Where CEmiratesIDNumber in (Select EmirateID from @PIDTable where ExistStatus = 0)
			AND PStatus is null 
			And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)
			group by CEmiratesIDNumber

		Update A SET A.OMPatientID = PIM.PatientID from TPXMLParsedData A
		inner join PatientInfo PIM
		on PIM.PersonEmiratesIDNumber = A.CEmiratesIDNumber and PIM.CorporateId = @pCID and PIM.FacilityId = @pFID
		where A.PStatus is null
		And (@pFileHeaderId=0 OR A.TPFileID=@pFileHeaderId)

		Update PatientInfo Set PersonEmiratesIDNumber = REPLACE(PersonEmiratesIDNumber,'XML','') Where PersonEmiratesIDNumber LIKE 'XML%'
		Update TPXMLParsedData Set CEmiratesIDNumber = REPLACE(CEmiratesIDNumber,'XML','') Where CEmiratesIDNumber LIKE 'XML%'
		And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)

	---- First Check incoming Patients Exist --- ENDS

	---- Insert Encounter Information --- STARTS

	insert into Encounter (EncounterNumber, EncounterRegistrationDate, EncounterStartTime, EncounterInpatientAdmitDate,PersonEmiratesIDNumber, PatientID, 
							  EncounterFacilityID, EncounterFacility, EncounterStartType, EncounterModeofArrival, EncounterPatientType,EncounterServiceCategory, EncounterAdmitType,
							  EncounterType, EncounterEndType, EncounterEligibilityIDPayer, Charges,WorkingDiagnosis,EncounterPhysicianType,EncounterAttendingPhysician,
							  CorporateID, IsAutoClosed,  IsMCContractBaseRateApplied,EncounterEndTime)
		 --Select 
		 ----(EFacilityID+EPatientID+CClaimID+ max(TPFileID))
		 --(Cast(LTRIM(RTRIM(EFacilityID)) as nvarchar)+Cast(LTRIM(RTRIM(EPatientID)) as nvarchar)+Cast(LTRIM(RTRIM(CClaimID)) as nvarchar)+ Cast(max(TPFileID) as nvarchar))
		 --,CONVERT(VARCHAR(16),MAX(EStart), 121),CONVERT(VARCHAR(16),MAX(EStart), 121),CONVERT(VARCHAR(16),MAX(EStart), 121),max(CEmiratesIDNumber),max(OMPatientID),
			--		@pFID,@pFID,max(EStartType),1,max(EType),1,1,
			--		max(EType),max(EEndType),max(EligibilityIDPayer),max(CGross),Case when max(DCode) ='' OR max(DCode) IS NULL then NULL ELSE max(DCode) END,1,1,@pCID,0,0,Cast(MAX(EEnd) as Datetime)
		 --from TPXMLParsedData Where PStatus is null group by CClaimID,EFacilityID,EPatientID,EligibilityIDPayer
		  Select
		(Cast(LTRIM(RTRIM(EFacilityID)) as nvarchar)+Cast(LTRIM(RTRIM(EPatientID)) as nvarchar)+Cast(LTRIM(RTRIM(CClaimID)) as nvarchar)+ Cast(max(TPFileID) as nvarchar))
		 ,CONVERT(VARCHAR(24), CONVERT(DATETIME, max(EStart), 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, max(EStart), 103), 108),
		 CONVERT(VARCHAR(24), CONVERT(DATETIME, max(EStart), 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, max(EStart), 103), 108),
		 CONVERT(VARCHAR(24), CONVERT(DATETIME, max(EStart), 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, max(EStart), 103), 108),
		 max(CEmiratesIDNumber),max(OMPatientID),@pFID,@pFID,max(EStartType),1,CASE WHEN max(EType) IN (3,4,13) THEN 2 WHEN max(EType) IN (1,5,6,7,8,9,12,15,41,42) THEN 3 ELSE 1 END,1,1,
		max(EType),max(EEndType),max(EligibilityIDPayer),max(CGross),Case when max(DCode) ='' OR max(DCode) IS NULL then NULL ELSE max(DCode) END,1,1,@pCID,0,0,
			CONVERT(VARCHAR(24), CONVERT(DATETIME, max(EEnd), 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, max(EEnd), 103), 108)
		 from TPXMLParsedData Where PStatus is null 
		 And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)
		 group by CClaimID,EFacilityID,EPatientID,EligibilityIDPayer
		 
	Update A SET A.OMEncounterID = PIM.EncounterID from TPXMLParsedData A inner join Encounter PIM 
					on PIM.EncounterNumber = (A.EFacilityID+A.EPatientID+A.CClaimID+CONVERT(VARCHAR(10),TPFileID)) and PIM.CorporateId = @pCID and PIM.EncounterFacility = @pFID
	where A.PStatus is null
	And (@pFileHeaderId=0 OR A.TPFileID=@pFileHeaderId)

  ----TPFileID, CClaimID, CMemberID, CPayerID, CProviderID, CEmiratesIDNumber, CGross, CPatientShare, CNet, EFacilityID, EType, 
  ----                       EPatientID, EligibilityIDPayer, EStart, EEnd, EStartType, EEndType, DType, DCode, AStart, AType, ACode, AQuantity, ANet, AOrderingClinician, AExecutingClinician, 
  ----                       APriorAuthorizationID, CNPackageName, ModifiedBy, ModifiedDate, PStatus, OMCorporateID, OMFacilityID, OMPatientID, OMEncounterID, OMBillID, 
  ----                       OMInsuranceID
	
	INSERT INTO [dbo].[Authorization]
           ([CorporateID],[FacilityID],[PatientID],[EncounterID],[AuthorizationDateOrdered],[AuthorizationStart],[AuthorizationEnd]
           ,[AuthorizationCode],[AuthorizationType],[AuthorizationComments],[AuthorizationDenialCode],[AuthorizationIDPayer],[AuthorizationLimit]
           ,[AuthorizationMemberID],[AuthorizationResult],[CreatedBy],[CreatedDate])
	Select @pCID,@pFID,max(OMPatientID),max(OMEncounterID),
	CONVERT(VARCHAR(24), CONVERT(DATETIME, max(EStart), 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, max(EStart), 103), 108),
	CONVERT(VARCHAR(24), CONVERT(DATETIME, max(EStart), 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, max(EStart), 103), 108),
	CONVERT(VARCHAR(24), CONVERT(DATETIME, max(EEnd), 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, max(EEnd), 103), 108),
	MAX(ACode),'3','TP XML Uploaded',NULL,MAX(CPayerId),0,0,null,99911,@CurrentDate 
	from TPXMLParsedData Where  PStatus is null 
	And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)
	group by CClaimID,EFacilityID,EPatientID,EligibilityIDPayer

	---- Insert Encounter Information --- ENDS
		

	---- Insert Bill Header Information --- STARTS
	--- Bugs fixes Before XMl billing Demo -- TMS Issue Number :	21961
	--What --Added the BillDate as Encounter start Date from XML file
	--Why -- Because one of the rule fails for Activity Start time should be greater then the  BillHeader date
	--When -- 11 March 2016,
	--Who -- Shashank Awasthy
	--insert into billheader (EncounterID, BillNumber, PatientID, FacilityID, PayerID, MemberID, Gross, PatientShare, PayerShareNet, BillDate, Status, 
	--						CreatedBy, CreatedDate,IsDeleted,CorporateID,DueDate,AuthCode)
	--   Select  max(OMEncounterID),('TPXML/'+Cast(max(TPFileID) as nvarchar(8))+'-'+Cast(@pFID as nvarchar(8))+'-'+Cast(max(OMPatientID) as nvarchar(8))+'-'+CClaimID),max(OMPatientID),@pFID,max(CPayerID),max(CMemberID),
	--		max(CGross),max(CPatientShare),max(CNet),max(EStart),40,CClaimID,@CurrentDate,0,@pCID,@CurrentDate,EligibilityIDPayer -- DateADD(Minute,1,CAST(max(EStart) as Datetime))
	-- from TPXMLParsedData Where  PStatus is null group by CClaimID,EFacilityID,EPatientID,EligibilityIDPayer

	insert into billheader (EncounterID, BillNumber, PatientID, FacilityID, PayerID, MemberID, Gross, PatientShare, PayerShareNet, BillDate, Status, 
							CreatedBy, CreatedDate,IsDeleted,CorporateID,DueDate,AuthCode)
    Select  max(OMEncounterID),('TPXML/'+Cast(max(TPFileID) as nvarchar(8))+'-'+Cast(@pFID as nvarchar(8))+'-'+Cast(max(OMPatientID) as nvarchar(8))+'-'+CClaimID),max(OMPatientID),@pFID,max(CPayerID),max(CMemberID),
			max(CGross),max(CPatientShare),max(CNet),
			CONVERT(VARCHAR(24), CONVERT(DATETIME, max(EStart), 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, max(EStart), 103), 108),
			40,/*CClaimID*/999999,@CurrentDate,0,@pCID,@CurrentDate,EligibilityIDPayer
	 from TPXMLParsedData Where  PStatus is null 
	And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)
	 group by CClaimID,EFacilityID,EPatientID,EligibilityIDPayer
	 
	Update A SET A.OMBillID = PIM.BillHeaderID from TPXMLParsedData A inner join billheader PIM 
					on PIM.BillNumber = ('TPXML/'+Cast((TPFileID) as nvarchar(8))+'-'+Cast(@pFID as nvarchar(8))+'-'+Cast((OMPatientID) as nvarchar(8))+'-'+CClaimID) and PIM.CorporateId = @pCID and PIM.FacilityID = @pFID
		where A.PStatus is null
	And (@pFileHeaderId=0 OR A.TPFileID=@pFileHeaderId)

    ---- Insert Bill Header Information --- ENDS

	---- Insert Bill Activities Information --- STARTS
	--insert into billactivity (BillHeaderID, CorporateID, FacilityID, PatientID, EncounterID, DiagnosisID, DiagnosisType, DiagnosisCode, ActivityID, 
 --                        ActivityType, ActivityCode, ActivityStartDate, ActivityEndDate, QuantityOrdered, OrderingClinicianID, OrderedDate, AdminstratingClinicianID, OrderCloseDate, 
 --                        AuthorizationID, AuthorizationCode, Gross, PatientShare, PayerShareNet, Status, CreatedBy, CreatedDate,IsDeleted,  MCDiscount, IsBaserateCalulationDone)
 --   Select OMBillID,OMCorporateID,OMFacilityID,OMPatientID,OMEncounterID,0,DType, DCode,0, 
	--		AType, ACode,AStart,AStart,AQuantity,OMOrderingClincialID,AStart,OMExecutingClincialID,AStart,
 --           0,APriorAuthorizationID,0,0,ANet,0,CClaimID,@CurrentDate,0,0,0
	-- from TPXMLParsedData Where OMBillID in (Select SC.OMBillID from TPXMLParsedData SC where SC.PStatus is null )
	
		DECLARE CursorBillactivitySum CURSOR fast_forward FOR  
		(Select  MAX(OMBillID) from TPXMLParsedData Where  PStatus is null and OMBillID in (Select SC.OMBillID from TPXMLParsedData SC where SC.PStatus is null )
		  group by CClaimID,EFacilityID,EPatientID,EligibilityIDPayer)
		
		DECLARE @OMBillIDVal INT
		OPEN CursorBillactivitySum   
		FETCH NEXT FROM CursorBillactivitySum INTO @OMBillIDVal
		
		WHILE @@FETCH_STATUS = 0   
		BEGIN   
		--- insert the First row with Patient Share and Actviity Net to as Gross , And patient share from Claim file
			insert into billactivity (BillHeaderID, CorporateID, FacilityID, PatientID, EncounterID, DiagnosisID, DiagnosisType, DiagnosisCode, ActivityID, 
				ActivityType, ActivityCode, ActivityStartDate, ActivityEndDate, QuantityOrdered, OrderingClinicianID, OrderedDate, AdminstratingClinicianID, OrderCloseDate, 
				AuthorizationID, AuthorizationCode, Gross, PatientShare, PayerShareNet, [Status], CreatedBy, CreatedDate,IsDeleted,  MCDiscount, IsBaserateCalulationDone,ActivityCost)
			Select Top 1 OMBillID,OMCorporateID,OMFacilityID,OMPatientID,OMEncounterID,0,DType, Case when (DCode) ='' OR (DCode) IS NULL then NULL ELSE (DCode) END,0, 
				AType, ACode,CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),
				CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),
				AQuantity,OMOrderingClincialID,CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),
				OMExecutingClincialID,
				CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),
				0,APriorAuthorizationID,Cast(ANet as Numeric(18,2))+ Cast(CPatientShare as Numeric(18,2)),CPatientShare,ANet,0,/*CClaimID*/999999,@CurrentDate,0,0,0,Cast(ANet as Numeric(18,2))+ Cast(CPatientShare as Numeric(18,2))
				from TPXMLParsedData Where OMBillID = @OMBillIDVal
				And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)
		
		--- insert the Other rows with Patient Share (as 0) + Actviity Net to as Gross , And patient share as 0 for other actviites from Claim file
			insert into billactivity (BillHeaderID, CorporateID, FacilityID, PatientID, EncounterID, DiagnosisID, DiagnosisType, DiagnosisCode, ActivityID, 
				ActivityType, ActivityCode, ActivityStartDate, ActivityEndDate, QuantityOrdered, OrderingClinicianID, OrderedDate, AdminstratingClinicianID, OrderCloseDate, 
				AuthorizationID, AuthorizationCode, Gross, PatientShare, PayerShareNet, [Status], CreatedBy, CreatedDate,IsDeleted,  MCDiscount, IsBaserateCalulationDone,ActivityCost)
			select OMBillID,OMCorporateID,OMFacilityID,OMPatientID,OMEncounterID,0,DType,  Case when (DCode) ='' OR (DCode) IS NULL then NULL ELSE (DCode) END,0, 
			AType, ACode,CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),AQuantity,OMOrderingClincialID,
			CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),OMExecutingClincialID,CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),
		    0,APriorAuthorizationID,Cast(ANet as Numeric(18,2)),0,ANet,0,/*CClaimID*/999999,@CurrentDate,0,0,0,ANet from
			(
			select *,
			ROW_NUMBER() OVER (ORDER BY OMBillID) AS ROW_NUM
			from TPXMLParsedData
			Where OMBillID = @OMBillIDVal
			And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)
			) x
			where ROW_NUM > 1
		
		       FETCH NEXT FROM CursorBillactivitySum INTO @OMBillIDVal
		END   
		
		CLOSE CursorBillactivitySum   
		DEALLOCATE CursorBillactivitySum  
		

	 --Declare @Dcode nvarchar(10) = (Select TOP(1)CAST(DCode as nvarchar(10)) from  TPXMLParsedData Where OMBillID in (Select SC.OMBillID from TPXMLParsedData SC where SC.PStatus is null ))
	 --Set @Dcode = Isnull(@Dcode,'')
	 --If @Dcode <> ''
	 --Begin
		insert into Diagnosis (DiagnosisType, CorporateID, FacilityID, PatientID, EncounterID, MedicalRecordNumber, DiagnosisCodeId, DiagnosisCode, DiagnosisCodeDescription, 
		                    Notes, InitiallyEnteredByPhysicianId, ReviewedByCoderID, ReviewedByPhysicianID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsDeleted, 
		                    DeletedBy, DeletedDate, DRGCodeID)
		Select '1',OMCorporateID,OMFacilityID,OMPatientID,OMEncounterID,null,0,Max([DiagnosisCode]), (select TOP(1)DiagnosisFullDescription from DiagnosisCode where DiagnosisCode1 =Max([DiagnosisCode])),
				'XML Uploader', 0,0,0,/*100*/999999,DateADD(Minute,1,CONVERT(VARCHAR(24), CONVERT(DATETIME, max(EStart), 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, max(EStart), 103), 108)),null,null,0,null,null,null
		from TPXMLParsedData P inner join [TPXMLDiagnosis] D on P.[TPFileID] = D.[TPFileID]
		Where  D.[ClaimID] = P.[CClaimID] And D.[MemberID] = P.[CMemberID]
				And D.[PayerID] = P.[CPayerID] 
				And D.[ProviderID] = P.[CProviderID] And
		OMBillID in (Select SC.OMBillID from TPXMLParsedData SC where SC.PStatus is null 
				And (@pFileHeaderId=0 OR SC.TPFileID=@pFileHeaderId))
		Group by OMCorporateID,OMFacilityID,OMPatientID,OMEncounterID

	 --END

	 --- Insert in to OpenOrder Table __Starts
	INSERT INTO [dbo].[OpenOrder]
           ([OpenOrderPrescribedDate],[PhysicianID],[PatientID],[EncounterID],[DiagnosisCode],[StartDate],[EndDate]
           ,[CategoryId],[SubCategoryId],[OrderType],[OrderCode],[Quantity],[FrequencyCode],[PeriodDays],[OrderNotes]
           ,[OrderStatus],[IsActivitySchecduled],[ActivitySchecduledOn],[IsActive],[CreatedBy],[CreatedDate],[CorporateID],[FacilityID],[IsApproved],EV1)

	Select CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),OMExecutingClincialID,OMPatientID,OMEncounterID,Case when (DCode) ='' OR (DCode) IS NULL then NULL ELSE (DCode) END,CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),
	CONVERT(VARCHAR(24), CONVERT(DATETIME, AStart, 103), 101)+' '+ CONVERT(VARCHAR(5), CONVERT(DATETIME, AStart, 103), 108),
		Case when AType= '3' THEN dbo.GetOrderCategoryByOrderCodeAndType(Cast(ACode as int),AType) ELSE dbo.GetOrderCategoryByOrderCodeAndType(ACode,AType) END,
		Case when AType= '3' THEN dbo.GetOrderSubCategoryByOrderCodeAndType(Cast(ACode as int),AType) ELSE dbo.GetOrderSubCategoryByOrderCodeAndType(ACode,AType) END
	,AType,ACode,AQuantity,'10',1,'XML order','4',NULL,NULL,1,OMExecutingClincialID,@CurrentDate,OMCorporateID,OMFacilityID,1,'99901'
	 from TPXMLParsedData Where OMBillID in (Select SC.OMBillID from TPXMLParsedData SC where SC.PStatus is null And (@pFileHeaderId=0 OR SC.TPFileID=@pFileHeaderId)) 
	 and Atype <> '8'
	 And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)


	 	-----Need to discuss what we have to do in case of diagnosis----

	 ---- Triger will not work here as the order start date is less then cruuent date for the External XML Data addition so have to create the Order actvities here by ourself
	 ---- UPdate the Order activties set the Order Activity Status = '4' that is on BILL
	 Update OrderActivity Set OrderActivityStatus = '4' 
	 where EncounterId in (Select OMEncounterID from TPXMLParsedData SC where SC.PStatus is null
	 And (@pFileHeaderId=0 OR SC.TPFileID=@pFileHeaderId)) 

	 ---- Below Update command is used to Update the Bill Activity Table for the Activity Id which will be OrderActivityId From the Order activity table
	 Update BA
	 SET BA.ActivityId = OA.OrderActivityID
	 From BillActivity BA 
	 INNER JOIN OrderActivity OA ON OA.EncounterID = BA.ENCOUNTERID
	 WHERE BA.ActivityType = OA.ORDERTYPE and BA.ActivityCode = OA.OrderCode
	 and BA.EncounterId in (Select OMEncounterID from TPXMLParsedData SC where SC.PStatus is null 
	 And (@pFileHeaderId=0 OR SC.TPFileID=@pFileHeaderId)
	 ) 
	 --AND FacilityId = OMFacilityID and 
	---- Insert intoo OPenOrder table End

	 --Declare @EncounterIDVal int = (Select  max(OMEncounterID) from TPXMLParsedData Where  PStatus is null group by CClaimID,EFacilityID,EPatientID,EligibilityIDPayer)
		DECLARE cursorPIFix CURSOR fast_forward FOR  
		(Select max(OMEncounterID) from TPXMLParsedData Where  PStatus is null 
		And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)
		group by CClaimID,EFacilityID,EPatientID,EligibilityIDPayer)
		
		DECLARE @EncounterIDVal INT
		OPEN cursorPIFix   
		FETCH NEXT FROM cursorPIFix INTO @EncounterIDVal
		
		WHILE @@FETCH_STATUS = 0   
		BEGIN   
			   Exec [dbo].[SPROC_EncounterEndCheckBillEdit] @EncounterIDVal
		       FETCH NEXT FROM cursorPIFix INTO @EncounterIDVal
		END   
		
		CLOSE cursorPIFix   
		DEALLOCATE cursorPIFix  
	 ---- Insert Bill Activities Information --- ENDS
	 
	 ---- Final Update to loaded Data 
	 Update TPXMLParsedData Set PStatus = 1 where PStatus is null
	 And (@pFileHeaderId=0 OR TPFileID=@pFileHeaderId)
END
GO


