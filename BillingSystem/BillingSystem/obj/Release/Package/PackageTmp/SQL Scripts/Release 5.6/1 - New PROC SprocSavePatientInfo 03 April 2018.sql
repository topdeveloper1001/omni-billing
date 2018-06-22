IF EXISTS (
		SELECT *
		FROM sysobjects
		WHERE id = object_id(N'SprocSavePatientInfo') AND OBJECTPROPERTY(id, N'IsProcedure') = 1
		)
BEGIN
	DROP PROCEDURE SprocSavePatientInfo
END
GO

CREATE PROCEDURE dbo.SprocSavePatientInfo 
(
	@pPatientId BIGINT,
	@pFId BIGINT,
	@pUserId BIGINT,
	@pCurrentDate DATETIME,
	@pCId BIGINT,
	@pInsCompanyId BIGINT, 
	@pInsurancePlanId BIGINT, 
	@pMemberId VARCHAR(100), 
	@pEmail VARCHAR(100), 
	@pSSN VARCHAR(100), 
	@pMRN VARCHAR(100), 
	@pFinanceNo VARCHAR(100), 
	@pMasterPatientNo VARCHAR(100), 
	@pPassportNo VARCHAR(100), 
	@pPassportExpiry datetime, 
	@pLastName VARCHAR(100), 
	@pFirstName VARCHAR(100), 
	@pNationality VARCHAR(100), 
	@pVipStatus VARCHAR(100), 
	@pGender VARCHAR(100), 
	@pContactNo varchar(100), 
	@pDOB DATETIME, 
	@pAgeInYears INT, 
	@pMaritalStatus VARCHAR(100), 
	@pInsuranceStart DATETIME, 
	@pInsuranceEnd DATETIME, 
	@pInsurancePolicyId BIGINT, 
	@pInsuranceCompanyId2 BIGINT, 
	@pInsurancePlanId2 BIGINT, 
	@pMemberId2 VARCHAR(100), 
	@pInsuranceStart2 varchar(50), 
	@pInsuranceEnd2 varchar(50), 
	@pInsurancePolicyId2 BIGINT, 
	@pDataArray ValuesArrayT ReadOnly,
	@pStreetAddress1 VARCHAR(200), 
	@pStreetAddress2 VARCHAR(200), 
	@pPOBox VARCHAR(100), 
	@pCityId BIGINT, 
	@pStateId BIGINT, 
	@pCountryId BIGINT, 
	@pAddressFirstName VARCHAR(100), 
	@pAddressLastName VARCHAR(100), 
	@ZipCode VARCHAR(100), 
	@pRelationType VARCHAR(100), 
	@pToken VARCHAR(100), 
	@pCode VARCHAR(100),
	@pEmiratesIDExpiry datetime
)
AS
BEGIN
	
	
	-----------$$$$$$$$$$$$$$$$$$ DECLARATIONS $$$$$$$$$$$$$$$$$$$-------------------
	DECLARE @ErrorMessage nvarchar(500), @DefaultSSN nvarchar(15) = '111-11-1111'
	,@ExecutionStatus int=0,@NewPatient bit=0,@OtherDoc int=5,@ProfileImageType smallint=1
	,@DocAssociatedTypeId int=1,@NewPatientLoginId int=0
	DECLARE @TempInsertedValues TABLE (Id BIGINT, TableValue VARCHAR(50))
	DECLARE @MaxPatientInsuranceId bigint=0,@MaxPatientLoginDetailId bigint=0,@MaxPatientPhoneId bigint=0
	,@MaxPatientId bigint=0,@MaxPatientAddressRelationId bigint=0
	-----------$$$$$$$$$$$$$$$$$$ DECLARATIONS $$$$$$$$$$$$$$$$$$$-------------------

	--SET @ExecutionStatus=1 
	--SET @ErrorMessage='Success'
	--SELECT @ExecutionStatus as [Status], @ErrorMessage as [Message]
	--Return;


	IF Cast(@pEmiratesIDExpiry as date) <= CAST(GETDATE() as date)
		SET @pEmiratesIDExpiry=NULL


	IF Cast(@pPassportExpiry as date) <= CAST(GETDATE() as date)
		SET @pPassportExpiry=NULL

	-----------$$$$$$$$$$$$$$$$$$ QUERY $$$$$$$$$$$$$$$$$$$-------------------
	
	
	-----------------########## Check for Duplicate EmiratedId Number
	IF (ISNULL(@pSSN,'') != '' AND @pSSN!=@DefaultSSN)
	BEGIN
		IF EXISTS (SELECT 1 FROM PatientInfo WHERE PersonEmiratesIDNumber = @pSSN AND PatientID != @pPatientId AND ISNULL(IsDeleted,0)=0)
		BEGIN
			SET @ErrorMessage = 'Duplicate SSN'
			SET @ExecutionStatus=-1
			GOTO ExecutionStatus
		END
	END
	ELSE
		SET @pSSN = ISNULL(@pSSN,@DefaultSSN) --- Define Default Value If Number is blank

	-----------------########## Check for Duplicate EmiratedId Number


	-----------------########## Check for Duplicate Health Care Number (MEMBER ID)
	IF EXISTS (
				SELECT 1 FROM PatientInsurance
				WHERE PersonHealthCareNumber= @pMemberId AND PatientID != @pPatientId 
				AND InsuranceCompanyId = @pInsCompanyId AND InsurancePlanId = @pInsurancePlanId
				AND IsActive=1 AND IsDeleted=0
			  )
	BEGIN
		SET @ErrorMessage = 'Duplicate Member ID (Health Care Number)'
		SET @ExecutionStatus=-1

		GOTO ExecutionStatus
	END
	-----------------########## Check for Duplicate Health Care Number


	-----------------########## Check for Duplicate Health Care Number 2
	IF EXISTS (
				SELECT 1 FROM PatientInsurance P
				WHERE P.PersonHealthCareNumber = @pMemberId2 AND P.PatientID != @pPatientId 
				AND P.InsuranceCompanyId = @pInsuranceCompanyId2 AND P.InsurancePlanId = @pInsurancePlanId2
				AND P.IsActive=1 AND P.IsDeleted=0
			 )
	BEGIN
		SET @ErrorMessage = 'Duplicate Member ID 2 (Health Care Number 2)'
		SET @ExecutionStatus=-1

		GOTO ExecutionStatus
	END
	-----------------########## Check for Duplicate Health Care Number 2


	-----------------########## Check for Duplicate Email
	IF EXISTS (SELECT 1 FROM PatientLoginDetail WHERE Email=@pEmail AND PatientID != @pPatientId AND ISNULL(IsDeleted,0)=0)
	BEGIN
		SET @ErrorMessage = 'Duplicate User (The patient with this Email already exists in the System)'
		SET @ExecutionStatus=-1

		GOTO ExecutionStatus
	END

	-----------------########## Check for Duplicate Email
	-----------------########## Check for Duplicate User
	IF EXISTS (SELECT 1 FROM PatientInfo
			WHERE PersonFirstName=@pFirstName AND PersonLastName=@pLastName AND PersonBirthDate=@pDOB AND PatientID != @pPatientId 
			AND ISNULL(IsDeleted,0)=0
			)
	BEGIN
		SET @ErrorMessage = 'Duplicate User (The patient with this FirstName, LastName or Date Of Birth already exists in the System)'
		SET @ExecutionStatus=-1

		GOTO ExecutionStatus
	END

	-----------------########## Check for Duplicate User
	
	-----------------########## Check for Duplicate User
	IF EXISTS (SELECT 1 FROM PatientInfo WHERE PersonPassportNumber=@pPassportNo AND PatientID != @pPatientId AND ISNULL(IsDeleted,0)=0)
	BEGIN
		SET @ErrorMessage = 'Duplicate Passport (The patient with this Passport Number already exists in the System)'
		SET @ExecutionStatus=-1

		GOTO ExecutionStatus
	END
	-----------------########## Check for Duplicate User




	-----------------########## Save PATIENT INFORMATION SECTION ##########-------------------------------

	BEGIN TRY;
		-----------------########## Save Patient Info in the table 'PatientInfo' Table #################----------------------
		IF (@pPatientId > 0)
		BEGIN
			UPDATE PatientInfo
			SET PersonEmiratesIDNumber= @pSSN, PersonMedicalRecordNumber = @pMRN, PersonFinancialNumber = @pFinanceNo
			,PersonMasterPatientNumber = @pMasterPatientNo, PersonPassportNumber = @pPassportNo
			,PersonPassportExpirtyDate = ISNULL(@pPassportExpiry,''), PersonLastName = @pLastName, PersonFirstName = @pFirstName
			,PersonVIP = @pVipStatus, PersonGender = @pGender, PersonNationality = @pNationality
			,PersonContactNumber = @pContactNo, PersonEmailAddress = @pEmail,PersonBirthDate = @pDOB
			,PersonAge = @pAgeInYears,PersonMaritalStatus = @pMaritalStatus, PersonContactMobileNumber = @pContactNo
			,ModifiedBy = @pUserId, ModifiedDate = @pCurrentDate,PersonEmiratesIDExpiration=@pEmiratesIDExpiry
			WHERE PatientID = @pPatientId

			SET @ExecutionStatus=1
		END
		ELSE
		BEGIN
			INSERT INTO PatientInfo (PersonEmiratesIDNumber, PersonMedicalRecordNumber, PersonFinancialNumber
			,PersonMasterPatientNumber,PersonPassportNumber, PersonPassportExpirtyDate, PersonLastName, PersonFirstName, PersonVIP, PersonGender
			,PersonNationality, PersonContactNumber, PersonEmailAddress, PersonBirthDate, PersonAge, PersonMaritalStatus, FacilityId
			,PersonContactMobileNumber, CorporateId, CreatedBy, CreatedDate, IsDeleted,PersonEmiratesIDExpiration)
			VALUES (@pSSN, @pMRN, @pFinanceNo, @pMasterPatientNo, @pPassportNo, @pPassportExpiry, 
			@pLastName, @pFirstName, @pVipStatus, @pGender, @pNationality, @pContactNo, @pEmail, @pDOB, 
			@pAgeInYears, @pMaritalStatus, @pFId, @pContactNo, @pCId, @pUserId, @pCurrentDate, 0,@pEmiratesIDExpiry)

			SET @pPatientId = SCOPE_IDENTITY()
			SET @ExecutionStatus=1
			SET @NewPatient=1
		END
		-----------------########## Save Patient Info in the table 'PatientInfo' Table #################----------------------


		-----------------########## Save Patient Insurance ##########------------------------
		IF (@ExecutionStatus=1 AND @pPatientId > 0)
		BEGIN
			SET @ExecutionStatus=0

			--SET @ErrorMessage='Error in Patient Insurance'
			IF EXISTS (SELECT 1 FROM PatientInsurance WHERE PatientID=@pPatientId AND IsPrimary = 1 AND IsActive=1 AND IsDeleted=0)
			BEGIN
				UPDATE PatientInsurance
				SET PersonHealthCareNumber = ISNULL(@pMemberId,PersonHealthCareNumber)
				,InsuranceCompanyId = @pInsCompanyId, InsurancePlanId = @pInsurancePlanId
				,InsurancePolicyId = @pInsurancePolicyId, Startdate = @pInsuranceStart, Expirydate = @pInsuranceEnd
				,ModifiedBy = @pUserId,ModifiedDate =@pCurrentDate
				WHERE PatientID=@pPatientId AND IsPrimary = 1

				SET @ExecutionStatus=1
			END
			ELSE
			BEGIN
				INSERT INTO PatientInsurance (PatientID, PersonHealthCareNumber, InsuranceCompanyId, InsurancePlanId, InsurancePolicyId, Startdate, 
				Expirydate, CreatedBy, CreatedDate, IsDeleted, IsActive, IsPrimary)
				--OUTPUT Inserted.PatientInsuraceID, 'Insurance' INTO @TempInsertedValues
				VALUES (@pPatientId, @pMemberId, @pInsCompanyId, @pInsurancePlanId, @pInsurancePolicyId, @pInsuranceStart, 
				@pInsuranceEnd, @pUserId, @pCurrentDate, 0, 1, 1)
					
				SET @ExecutionStatus=1
			END

			IF (ISNULL(@pMemberId2,'') != '')
			BEGIN
				INSERT INTO PatientInsurance (PatientID, PersonHealthCareNumber, InsuranceCompanyId, InsurancePlanId, InsurancePolicyId, Startdate, Expirydate,
				CreatedBy, CreatedDate, IsDeleted, IsActive, IsPrimary)
				--OUTPUT Inserted.PatientInsuraceID, 'Insurance' INTO @TempInsertedValues
				VALUES (@pPatientId, @pMemberId2, @pInsuranceCompanyId2, @pInsurancePlanId2, @pInsurancePolicyId2, @pInsuranceStart2, @pInsuranceEnd2, 
				@pUserId, @pCurrentDate, 0, 1, 0)

				SET @ExecutionStatus=1
			END
		END
		-----------------########## Save Patient Insurance ##########------------------------



		-----------------########## Save Patient Phone ##########------------------------
		IF (@ExecutionStatus=1 AND @pPatientId > 0)
		BEGIN
			SET @ExecutionStatus=0

			--SET @ErrorMessage='Error in Patient Phone'

			IF EXISTS (SELECT 1 FROM PatientPhone WHERE PatientID = @pPatientId AND IsPrimary = 1 And IsDeleted=0)
			BEGIN
				UPDATE PatientPhone
				SET PhoneNo = @pContactNo, ModifiedBy = @pUserId, ModifiedDate = @pCurrentDate,IsDeleted=0
				WHERE PatientID = @pPatientId AND IsPrimary = 1

				SET @ExecutionStatus=1
			END
			ELSE
			BEGIN
				INSERT INTO PatientPhone (PatientID, PhoneType, PhoneNo, IsPrimary, IsdontContact, CreatedBy, CreatedDate, IsDeleted)
				--OUTPUT Inserted.PatientPhoneId, 'Phone' INTO @TempInsertedValues
				VALUES (@pPatientId, 2, @pContactNo, 1, 0, @pUserId, @pCurrentDate, 0)

				SET @ExecutionStatus=1
			END
		END
		-----------------########## Save Patient Phone ##########------------------------



		-----------------########## Save Patient's Profile Image #########---------------
		IF (@ExecutionStatus=1 AND @pPatientId > 0 AND Exists (Select 1 From @pDataArray Where Value1=@ProfileImageType))
		BEGIN
			SET @ExecutionStatus=0

			--SET @ErrorMessage='Error in Patient Profile Image'

			IF EXISTS (SELECT 1 FROM DocumentsTemplates D WHERE D.PatientID=@pPatientId 
			And ISNULL(D.IsDeleted,0)=0 And D.DocumentTypeID=@ProfileImageType)
			BEGIN
				UPDATE D SET D.[FileName]=T.Value3
				,D.FilePath=T.Value5,D.ModifiedBy=@pUserId, D.ModifiedDate = @pCurrentDate
				,D.IsDeleted=0
				From DocumentsTemplates D
				INNER JOIN @pDataArray T ON D.DocumentTypeID=@ProfileImageType AND D.PatientID = @pPatientId 
				And ISNULL(D.IsDeleted,0)=0 And D.DocumentTypeID=@ProfileImageType
				AND D.AssociatedType=@DocAssociatedTypeId

				SET @ExecutionStatus=1
			END
			ELSE
			BEGIN
				INSERT INTO DocumentsTemplates (DocumentTypeID, DocumentName, AssociatedID
				,AssociatedType,[FileName], FilePath,IsTemplate
				,IsRequired,CreatedBy,CreatedDate,IsDeleted,CorporateID, FacilityID
				,PatientID)
				Select TOP 1 @ProfileImageType,'profilepicture', @pPatientId
				,@DocAssociatedTypeId,Value3,Value5,1,0,@pUserId,@pCurrentDate,0,@pCId,@pFId
				,@pPatientId
				From @pDataArray Where Value1=@ProfileImageType

				SET @ExecutionStatus=1
			END
		END
		-----------------########## Save Patient's Profile Image #########---------------



		-----------------########## Save Patient's Other Attachments #########---------------
		IF (@ExecutionStatus=1 AND @pPatientId > 0 AND Exists (Select 1 From @pDataArray Where Value1!=@ProfileImageType))
		BEGIN
			SET @ExecutionStatus=0

			IF EXISTS (
						SELECT 1 FROM DocumentsTemplates WHERE PatientID = @pPatientId And ISNULL(IsDeleted,0)=0 
						And DocumentTypeID IN (Select Value1 From @pDataArray Where Value1!=@ProfileImageType)
					  )
			BEGIN
				UPDATE D
				SET D.DocumentName=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeCategoryValue='1103' And G.GlobalCodeValue=Value1)
				,D.[FileName]=T.Value3,D.FilePath=T.Value5,D.ModifiedBy=@pUserId, D.ModifiedDate = @pCurrentDate
				,D.IsDeleted=0
				From DocumentsTemplates D 
				INNER JOIN @pDataArray T ON D.DocumentTypeID=T.Value1 AND D.PatientID = @pPatientId
				And ISNULL(D.IsDeleted,0)=0

				SET @ExecutionStatus=1
			END
			ELSE
			BEGIN
				INSERT INTO DocumentsTemplates (DocumentTypeID, DocumentName, AssociatedID
				,AssociatedType,[FileName], FilePath,IsTemplate,IsRequired,CreatedBy,CreatedDate,IsDeleted,CorporateID,FacilityID
				,PatientID)
				Select TOP 1 Value1
				,(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeCategoryValue='1103' And G.GlobalCodeValue=Value1)
				,@pPatientId,@DocAssociatedTypeId,Value3,Value5,1,0,@pUserId,@pCurrentDate,0,@pCId,@pFId
				,@pPatientId
				From @pDataArray Where Value1!=@ProfileImageType

				SET @ExecutionStatus=1
			END
		END
		-----------------########## Save Patient's Other Attachments #########---------------




		-----------------########## Save Patient Login Detail ##########------------------------
		IF (@ExecutionStatus=1 AND @pPatientId > 0 AND ISNULL(@pToken, '') != '' AND ISNULL(@pCode, '') != '')
		BEGIN
			SET @ExecutionStatus=0

			IF EXISTS (SELECT 1 FROM PatientLoginDetail WHERE PatientID = @pPatientId AND Email = @pEmail AND ISNULL(IsDeleted,0)=0)
			BEGIN
				UPDATE PatientLoginDetail SET ModifiedBy = @pUserId, ModifiedDate = @pCurrentDate, IsDeleted=0 WHERE PatientID=@pPatientId
				--SET @ErrorMessage='Error in Patient Update Login Detail'
				SET @ExecutionStatus=1
			END
			ELSE
			BEGIN
				--SET @ErrorMessage='Error Before Patient Add Login Detail'
				INSERT INTO PatientLoginDetail (PatientId, Email, TokenId, PatientPortalAccess, CodeValue, CreatedBy, CreatedDate, IsDeleted)
				VALUES (@pPatientId, @pEmail, @pToken, 1, @pCode, @pUserId, @pCurrentDate, 0)

				--SET @ErrorMessage='Error in Patient Add Login Detail'

				SET @ExecutionStatus=1
			END

			IF @ExecutionStatus=1
				Select TOP 1 @NewPatientLoginId=Id From PatientLoginDetail WHERE PatientID=@pPatientId
		END
		-----------------########## Save Patient Login Detail ##########------------------------
		


		-----------------########## Save Patient Address ##########------------------------
		IF (@ExecutionStatus=1 AND @pPatientId > 0 AND ISNULL(@pAddressFirstName, '') != '' AND ISNULL(@pAddressLastName	, '') != '')
		BEGIN
			SET @ExecutionStatus=0

			--SET @ErrorMessage='Error in Patient Address Relation'


			IF EXISTS (SELECT 1 FROM PatientAddressRelation WHERE PatientID=@pPatientId AND IsDeleted=0)
			BEGIN
				UPDATE PatientAddressRelation
				SET PatientAddressRelationType=@pRelationType,FirstName=@pAddressFirstName,LastName=@pAddressLastName
				,StreetAddress1=@pStreetAddress1,StreetAddress2=@pStreetAddress2,CityID=@pCityId,StateID=@pStateId
				,CountryID=@pCountryId,ZipCode=@ZipCode,POBox=@pPOBox,ModifiedBy = @pUserId
				,ModifiedDate = @pCurrentDate,IsDeleted=0
				WHERE PatientID = @pPatientId

				SET @ExecutionStatus=1
			END
			ELSE
			BEGIN
				INSERT INTO PatientAddressRelation (PatientID,PatientAddressRelationType,FirstName,LastName
				,StreetAddress1,StreetAddress2,CityID,StateID,CountryID,ZipCode
				,POBox,IsPrimary,CreatedBy,CreatedDate,IsDeleted)
				VALUES (@pPatientId,@pRelationType,@pAddressFirstName,@pAddressLastName,@pStreetAddress1
				,@pStreetAddress2,@pCityId,@pStateId,@pCountryId,@ZipCode,
				@pPOBox,1,@pUserId,@pCurrentDate, 0)

				SET @ExecutionStatus=1
			END
		END
		-----------------########## Save Patient Address ##########------------------------


	END TRY
	BEGIN CATCH
		SET @ErrorMessage = ERROR_MESSAGE()
		SET @ExecutionStatus=0
	END CATCH

	--In case of Exceptions Occurred in any query above, then delete the entries from Patient Related Tables.
	IF (@ExecutionStatus=0 AND @NewPatient=1)
	Begin
		DELETE FROM PatientInsurance where PatientID=@pPatientId
		DELETE FROM PatientLoginDetail where PatientId=@pPatientId
		DELETE FROM PatientPhone where PatientID=@pPatientId
		DELETE FROM PatientInfo where PatientID=@pPatientId
		DELETE FROM PatientAddressRelation where PatientID=@pPatientId
		
		Update MaxValues SET MaxValue=MaxValue-1 Where AssociatedKey=1

		Select @MaxPatientInsuranceId=Max(PatientInsuraceID) From PatientInsurance;
		Select @MaxPatientLoginDetailId=Max(Id) From PatientLoginDetail;
		Select @MaxPatientPhoneId=Max(PatientPhoneId) From PatientPhone;
		Select @MaxPatientId=Max(PatientId) From PatientInfo;
		Select @MaxPatientAddressRelationId=Max(PatientAddressRelationID) From PatientAddressRelation;

		--This resets the Auto-Identity in the Patient Related Tables tables
		DBCC CHECKIDENT('PatientInsurance', RESEED, @MaxPatientInsuranceId)
		DBCC CHECKIDENT('PatientLoginDetail', RESEED, @MaxPatientLoginDetailId)
		DBCC CHECKIDENT('PatientPhone', RESEED, @MaxPatientPhoneId)
		DBCC CHECKIDENT('PatientInfo', RESEED, @MaxPatientId)
		DBCC CHECKIDENT('PatientAddressRelation', RESEED, @MaxPatientAddressRelationId)

		SET @ExecutionStatus=9999
		SET @ErrorMessage='Unexpected Errors Occurred. Please try again sometime later!!'
	End
	Else IF @ExecutionStatus=1
	Begin
		SET @ExecutionStatus = @pPatientId
		Update MaxValues SET MaxValue=MaxValue+1 Where AssociatedKey=1
	End
	
	ExecutionStatus:
		SELECT @ExecutionStatus as [Status], ISNULL(@ErrorMessage,'') as [Message]
		,Value1=(P.PersonFirstName + ' ' + P.PersonLastName)
		,Value2=(Select TOP 1 FacilityName  From Facility Where FacilityId=P.FacilityId)
		,Value3=@NewPatientLoginId
		From PatientInfo P Where PatientID=@pPatientId

	-----------$$$$$$$$$$$$$$$$$$ QUERY $$$$$$$$$$$$$$$$$$$-------------------
END