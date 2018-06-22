-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetPatientSearchResultAndOtherData','P') IS NOT NULL
   DROP PROCEDURE SprocGetPatientSearchResultAndOtherData
GO

CREATE PROCEDURE SprocGetPatientSearchResultAndOtherData
(
@pUserId bigint,
@pLastName nvarchar(50)=null,
@pPassport nvarchar(50)=null,
@pMobilePhone nvarchar(100)=null,
@pBirthDate datetime=null,
@pSSN nvarchar(100)=null,
@pWithAccessedRoles bit,
@pFId bigint=null,
@pCId bigint=null,
@pRoleId bigint=null
)
As
Begin
	Declare @TPatients Table (PatientId bigint)
	Declare @PatientInfoAccessible bit=0,@EhrViewAccessible bit=0,@AuthorizationViewAccessible bit=0
	,@BillHeaderViewAccessible bit=0
	

	SET @pFId = ISNULL(@pFId,0)
	SET @pCId = ISNULL(@pCId,0)
	SET @pRoleId = ISNULL(@pRoleId,0)

	
	INSERT INTO @TPatients
	Select P.PatientID From PatientInfo P
	Where (ISNULL(@pLastName,'')='' OR P.PersonLastName like '%' + @pLastName + '%')
	AND (ISNULL(@pPassport,'')='' OR P.PersonPassportNumber like '%' + @pPassport + '%')
	AND (ISNULL(@pMobilePhone,'')='' OR P.PersonContactNumber like '%' + @pMobilePhone + '%')
	AND (@pBirthDate IS NULL OR CAST(P.PersonBirthDate as date)=@pBirthDate)
	AND (ISNULL(@pSSN,'')='' OR P.PersonEmiratesIDNumber like '%' + @pSSN + '%')
	And ISNULL(P.IsDeleted,0)=0
	AND (ISNULL(@pFId,0)=0 OR P.FacilityId IN (@pFId,0))
	AND (ISNULL(@pCId,0)=0 OR P.CorporateId IN (@pCId,0))
	
	IF @pWithAccessedRoles=1 AND @pRoleId > 0
		Begin
			Select @PatientInfoAccessible=Cast((Case When Exists(Select 1 FROM RoleTabs R Where R.RoleID=@pRoleId
											And R.TabID IN (Select T.TabId From Tabs T Where T.TabName='Register New Patient' 
											And T.Controller='PatientInfo' ANd T.IsActive=1 AND T.IsVisible=1)) THEN 1 ELSE 0 END) as bit)
			Select @EhrViewAccessible=Cast((Case When Exists(Select 1 FROM RoleTabs R Where R.RoleID=@pRoleId
											And R.TabID IN (Select T.TabId From Tabs T Where T.TabName='EHR' 
											And T.Controller='Summary' ANd T.IsActive=1 AND T.IsVisible=1)) THEN 1 ELSE 0 END) as bit)
			Select @AuthorizationViewAccessible=Cast((Case When Exists(Select 1 FROM RoleTabs R Where R.RoleID=@pRoleId
											And R.TabID IN (Select T.TabId From Tabs T Where T.TabName='Obtain Insurance Authorization' 
											And T.Controller='Authorization' ANd T.IsActive=1 AND T.IsVisible=1)) THEN 1 ELSE 0 END) as bit)
			Select @BillHeaderViewAccessible=Cast((Case When Exists(Select 1 FROM RoleTabs R Where R.RoleID=@pRoleId
											And R.TabID IN (Select T.TabId From Tabs T Where T.TabName='Generate Preliminary Bill' 
											And T.Controller='BillHeader' ANd T.IsActive=1 AND T.IsVisible=1)) THEN 1 ELSE 0 END) as bit)
	End


	--Select PatientInfo=(Select
	--P.[PatientID]
	--,P.[PersonEmiratesIDNumber]
	--,P.[PersonEmiratesIDExpiration]
	--,P.[PersonSocialSecurityNumber]
	--,P.[PersonMedicalRecordNumber]
	--,P.[PersonFinancialNumber]
	--,P.[PersonMasterPatientNumber]
	--,P.[PersonPassportNumber]
	--,P.[PersonNatlHealthInsNumber]
	--,P.[PersonPassportExpirtyDate]
	--,P.[PersonInsuranceCompany]
	--,P.[PersonLastName]
	--,P.[PersonFirstName]
	--,P.[PersonSecondName]
	--,P.[PersonPrevLastName]
	--,P.[PersonPrevFirstName]
	--,P.[PersonPrevSecondName]
	--,P.[PersonArabicSecondName]
	--,P.[PersonTitle]
	--,P.[PersonVIP]
	--,P.[PersonGender]
	--,P.[PersonMotherName]
	--,P.[PersonNationality]
	--,P.[PersonNationalityGroup]
	--,P.[PersonEligibilityStatus]
	--,P.[PersonEligibilityStartDate]
	--,P.[PersonEligibilityEndDate]
	--,P.[PersonStreetAddress]
	--,P.[PersonStreetAddres2]
	--,P.[PersonPOBox]
	--,P.[PersonCountry]
	--,P.[PersonEmirate]
	--,P.[PersonCity]
	--,P.[PersonArea]
	--,P.[PersonHomePhone]
	--,P.[PersonContactNumber]
	--,P.[PersonBusinessPhone]
	--,(CASE WHEN 
	--	ISNULL(P.[PersonEmailAddress],'')='' 
	--	THEN P1.Email
	--	ELSE P.[PersonEmailAddress] END) As PersonEmailAddress
	--,P.[PersonResdidencyStatus]
	--,P.[PersonResidencyEmirate]
	--,P.[PersonBirthDate]
	--,P.[PersonAge]
	--,P.[PersonEstimatedAge]
	--,P.[PersonMaritalStatus]
	--,P.[PersonType]
	--,P.[PersonLastNameHusband]
	--,P.[PersonFirstNameHusband]
	--,P.[PersonHusbandNationality]
	--,P.[PersonHusbandPassportScan]
	--,P.[PersonHusbandPassportNumber]
	--,P.[PersonHusbandPassportExpiry]
	--,P.[PersonHusbandAddress]
	--,P.[PersonHusbandCountry]
	--,P.[PersonHusbandEmirateState]
	--,P.[PersonHusbandCity]
	--,P.[PersonHusbandZipCode]
	--,P.[PersonHusbandMobile]
	--,P.[PersonEmiratesIDScan]
	--,P.[PersonPassportScan]
	--,P.[PersonMarriageCertificate]
	--,P.[PersonInsuranceCardScan]
	--,P.[PersonPhoto]
	--,P.[FacilityId]
	--,P.[PersonContactMobileNumber]
	--,P.[PersonArabicFirstName]
	--,P.[PersonPayerID]
	--,P.[CorporateId]
	--,P.[CreatedBy]
	--,P.[CreatedDate]
	--,P.[ModifiedBy]
	--,P.[ModifiedDate]
	--,P.[IsDeleted]
	--,P.[DeletedBy]
	--,PatientPhone=(Select P2.* From PatientPhone P2 Where P.PatientID=P2.PatientID FOR JSON PATH)
	--FOR JSON PATH--,Without_Array_Wrapper
	--),
	--IsEncounterExist=(Select Cast(Case WHEN Exists (Select 1 From Encounter E Where E.PatientID=P.PatientID AND E.EncounterEndTime IS NULL) THEN 1 ELSE 0 END As Bit)),
	--IsAuthorizationExist=(Select Cast(Case WHEN 
	--									Exists (Select 1 From Encounter E INNER JOIN [Authorization] A ON E.EncounterID=A.EncounterID 
	--											AND ISNULL(A.IsDeleted,0)=0 Where E.PatientID=P.PatientID)
	--											THEN 1 ELSE 0 END As Bit)),
	--PatientActiveEncounterFacilityId=(Select ISNULL(E.EncounterFacility,0) From Encounter E Where E.PatientID=P.PatientID AND E.EncounterEndTime IS NULL),
	--FacilityName=(Select ISNULL(F.FacilityName,'') From Facility F Where F.FacilityId=P.FacilityId),
	--CorporateName=(Select ISNULL(C.CorporateName,'') From Corporate C Where C.CorporateID=P.CorporateId),
	--PatientInfoAccessible=@PatientInfoAccessible,
	--EhrViewAccessible=@EhrViewAccessible,
	--AuthorizationViewAccessible=@AuthorizationViewAccessible,
	--BillHeaderViewAccessible=@BillHeaderViewAccessible
	--FROM [dbo].[PatientInfo] P
	--LEFT OUTER JOIN PatientLoginDetail P1 ON P.PatientID=P1.PatientID
	--Where P.PatientID IN (Select PatientId From @TPatients)
	--Order By P.PersonFirstName
	--FOR JSON PATH,Root('PatientSearchResults'),INCLUDE_NULL_VALUES

	Select P.[PatientID]
	,P.[PersonEmiratesIDNumber]
	,P.[PersonEmiratesIDExpiration]
	,P.[PersonSocialSecurityNumber]
	,P.[PersonMedicalRecordNumber]
	,P.[PersonFinancialNumber]
	,P.[PersonMasterPatientNumber]
	,P.[PersonPassportNumber]
	,P.[PersonNatlHealthInsNumber]
	,P.[PersonPassportExpirtyDate]
	,P.[PersonInsuranceCompany]
	,P.[PersonLastName]
	,P.[PersonFirstName]
	,P.[PersonSecondName]
	,P.[PersonPrevLastName]
	,P.[PersonPrevFirstName]
	,P.[PersonPrevSecondName]
	,P.[PersonArabicSecondName]
	,P.[PersonTitle]
	,P.[PersonVIP]
	,P.[PersonGender]
	,P.[PersonMotherName]
	,P.[PersonNationality]
	,P.[PersonNationalityGroup]
	,P.[PersonEligibilityStatus]
	,P.[PersonEligibilityStartDate]
	,P.[PersonEligibilityEndDate]
	,P.[PersonStreetAddress]
	,P.[PersonStreetAddres2]
	,P.[PersonPOBox]
	,P.[PersonCountry]
	,P.[PersonEmirate]
	,P.[PersonCity]
	,P.[PersonArea]
	,P.[PersonHomePhone]
	,P.[PersonContactNumber]
	,P.[PersonBusinessPhone]
	,(CASE WHEN 
		ISNULL(P.[PersonEmailAddress],'')='' 
		THEN P1.Email
		ELSE P.[PersonEmailAddress] END) As PersonEmailAddress
	,P.[PersonResdidencyStatus]
	,P.[PersonResidencyEmirate]
	,P.[PersonBirthDate]
	,P.[PersonAge]
	,P.[PersonEstimatedAge]
	,P.[PersonMaritalStatus]
	,P.[PersonType]
	,P.[PersonLastNameHusband]
	,P.[PersonFirstNameHusband]
	,P.[PersonHusbandNationality]
	,P.[PersonHusbandPassportScan]
	,P.[PersonHusbandPassportNumber]
	,P.[PersonHusbandPassportExpiry]
	,P.[PersonHusbandAddress]
	,P.[PersonHusbandCountry]
	,P.[PersonHusbandEmirateState]
	,P.[PersonHusbandCity]
	,P.[PersonHusbandZipCode]
	,P.[PersonHusbandMobile]
	,P.[PersonEmiratesIDScan]
	,P.[PersonPassportScan]
	,P.[PersonMarriageCertificate]
	,P.[PersonInsuranceCardScan]
	,P.[PersonPhoto]
	,P.[FacilityId]
	,P.[PersonContactMobileNumber]
	,P.[PersonArabicFirstName]
	,P.[PersonPayerID]
	,P.[CorporateId]
	,P.[CreatedBy]
	,P.[CreatedDate]
	,P.[ModifiedBy]
	,P.[ModifiedDate]
	,P.[IsDeleted]
	,P.[DeletedBy]
	,PatientPhone=(Select P2.* From PatientPhone P2 Where P.PatientID=P2.PatientID FOR JSON PATH)
	FROM [dbo].[PatientInfo] P
	LEFT OUTER JOIN PatientLoginDetail P1 ON P.PatientID=P1.PatientID
	Where P.PatientID IN (Select PatientId From @TPatients)
	Order By P.PersonFirstName
	FOR JSON PATH,Root('PatientInfo'),INCLUDE_NULL_VALUES



	Select P.PatientId As Id, IsEncounterExist=(Select Cast(Case WHEN Exists (Select 1 From Encounter E Where E.PatientID=P.PatientID AND E.EncounterEndTime IS NULL) THEN 1 ELSE 0 END As Bit)),
	IsAuthorizationExist=(Select Cast(Case WHEN 
										Exists (Select 1 From Encounter E INNER JOIN [Authorization] A ON E.EncounterID=A.EncounterID 
												AND ISNULL(A.IsDeleted,0)=0 Where E.PatientID=P.PatientID)
												THEN 1 ELSE 0 END As Bit)),
	PatientActiveEncounterFacilityId=(Select ISNULL(E.EncounterFacility,0) From Encounter E Where E.PatientID=P.PatientID AND E.EncounterEndTime IS NULL),
	FacilityName=(Select ISNULL(F.FacilityName,'') From Facility F Where F.FacilityId=P.FacilityId),
	CorporateName=(Select ISNULL(C.CorporateName,'') From Corporate C Where C.CorporateID=P.CorporateId),
	PatientInfoAccessible=@PatientInfoAccessible,
	EhrViewAccessible=@EhrViewAccessible,
	AuthorizationViewAccessible=@AuthorizationViewAccessible,
	BillHeaderViewAccessible=@BillHeaderViewAccessible
	FROM [dbo].[PatientInfo] P
	LEFT OUTER JOIN PatientLoginDetail P1 ON P.PatientID=P1.PatientID
	Where P.PatientID IN (Select PatientId From @TPatients)
	Order By P.PersonFirstName
	FOR JSON PATH,Root('PatientSearchResults'),INCLUDE_NULL_VALUES
End