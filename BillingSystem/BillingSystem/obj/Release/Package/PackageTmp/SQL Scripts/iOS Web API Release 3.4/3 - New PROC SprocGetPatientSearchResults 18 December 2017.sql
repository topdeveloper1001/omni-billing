-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetPatientSearchResults','P') IS NOT NULL
   DROP PROCEDURE SprocGetPatientSearchResults
GO

CREATE PROCEDURE SprocGetPatientSearchResults
(
@pText nvarchar(100),
@pFId bigint=null,
@pCId bigint=null
)
As
Begin
	Declare @TSearch Table (SearchedText nvarchar(100))
	Declare @TPatients Table (PatientId bigint)

	SET @pFId = ISNULL(@pFId,0)
	SET @pCId = ISNULL(@pCId,0)

	
	IF ISNULL(@pText,'') !=''
	BEGIN
		INSERT INTO @TSearch
		Select IDValue From dbo.Split(' ',@pText)


		--Select * From @TSearch

		INSERT INTO @TPatients
		Select P.PatientID From PatientInfo P
		Where 
		Exists (Select 1 From @TSearch S Where 
		P.PersonFirstName like '%' + S.SearchedText + '%'
		OR 
		P.PersonLastName like '%' + S.SearchedText + '%'
		)
		And ISNULL(P.IsDeleted,0)=0
		AND (ISNULL(@pFId,0)=0 OR P.FacilityId=@pFId)
		AND (ISNULL(@pCId,0)=0 OR P.CorporateId=@pCId)
		
		--Select * From @TPatients

		IF Exists (Select 1 From @TPatients)
		Begin
			Select 
			P.[PatientID]
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
			Order by P.PersonFirstName
			FOR JSON PATH,Root('PatientSearchResults') 
		End
	END
End