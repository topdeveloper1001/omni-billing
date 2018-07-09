IF EXISTS (
		SELECT *
		FROM sysobjects
		WHERE id = object_id(N'SprocSaveFacility') AND OBJECTPROPERTY(id, N'IsProcedure') = 1
		)
BEGIN
	DROP PROCEDURE SprocSaveFacility
END
GO

/****** Object:  UserDefinedTableType [dbo].[FacilityContactT]    Script Date: 7/9/2018 5:15:40 PM ******/
DROP TYPE [dbo].[FacilityContactT]
GO

/****** Object:  UserDefinedTableType [dbo].[FacilityContactT]    Script Date: 7/9/2018 5:15:41 PM ******/
CREATE TYPE [dbo].[FacilityContactT] AS TABLE ([ContactName] [nvarchar](100) NULL, [Email] [nvarchar](100) NULL, [IsMain] [bit] NULL)
GO

CREATE PROCEDURE dbo.SprocSaveFacility 
(@pId INT, @pNumber VARCHAR(50), @pName VARCHAR(50), @pStreetAddress VARCHAR(40), @pStreetAddress2 VARCHAR(40)=NULL, @pCity VARCHAR(30), @pState INT, 
@pZipCode INT=NULL, @pMainPhone VARCHAR(20)=NULL, @pFax VARCHAR(20)=NULL, @pSecondPhone VARCHAR(20)=NULL, @pPOBox VARCHAR(20)=NULL, @pLicenseNumber VARCHAR(150)=NULL, 
@pLicenseNumberExpire DATETIME=NULL, @pTypeLicense VARCHAR(20)=NULL, @pFacilityRelated VARCHAR(40)=NULL, @pTotalLicenseBed INT=NULL, @pTotalStaffedBed INT=NULL, @pAffiliationNumber INT=NULL, 
@pRegionId VARCHAR(10)=NULL, @pCountryID INT, @pCorporateID INT, @pTimeZone VARCHAR(100), @pSenderID VARCHAR(50)=NULL, @pLoggedInUserId INT,  @pCurrentDate DATETIME, 
 @pContact FacilityContactT readonly)
AS
BEGIN
	IF @pId > 0
	BEGIN
		UPDATE Facility
		SET FacilityNumber = @pNumber, FacilityName = @pName, FacilityStreetAddress = @pStreetAddress, FacilityStreetAddress2 = @pStreetAddress2,
		 FacilityCity = @pCity, FacilityState = @pState, FacilityZipCode = @pZipCode, FacilityMainPhone = @pMainPhone, FacilityFax = @pFax, 
		 FacilitySecondPhone = @pSecondPhone, FacilityPOBox = @pPOBox, FacilityLicenseNumber = @pLicenseNumber, FacilityLicenseNumberExpire = @pLicenseNumberExpire, 
		 FacilityTypeLicense = @pTypeLicense, FacilityRelated = @pFacilityRelated, FacilityTotalLicenseBed = @pTotalLicenseBed, FacilityTotalStaffedBed = @pTotalStaffedBed,
		 FacilityAffiliationNumber = @pAffiliationNumber, RegionId = @pRegionId, ModifiedBy = @pLoggedInUserId, ModifiedDate = @pCurrentDate,
		 CountryID = @pCountryID, CorporateID = @pCorporateID, FacilityTimeZone = @pTimeZone, SenderID = @pSenderID
		WHERE FacilityId = @pId
	END
	ELSE
	BEGIN
		INSERT INTO Facility(FacilityNumber,FacilityName,FacilityStreetAddress,FacilityStreetAddress2,FacilityCity,FacilityState,FacilityZipCode,FacilityMainPhone,
		FacilityFax,FacilitySecondPhone,FacilityPOBox,FacilityLicenseNumber,FacilityLicenseNumberExpire,FacilityTypeLicense,FacilityRelated,FacilityTotalLicenseBed,
		FacilityTotalStaffedBed,FacilityAffiliationNumber,RegionId,CreatedBy,CreatedDate,IsDeleted,IsActive,CountryID,CorporateID,FacilityTimeZone,SenderID,LoggedInID) 
		VALUES (@pNumber,@pName,@pStreetAddress,@pStreetAddress2,@pCity,@pState,@pZipCode,@pMainPhone,@pFax,@pSecondPhone,@pPOBox,@pLicenseNumber,
		@pLicenseNumberExpire,@pTypeLicense,@pFacilityRelated,@pTotalLicenseBed,@pTotalStaffedBed,@pAffiliationNumber,@pRegionId,@pLoggedInUserId,
		@pCurrentDate,0,1,@pCountryID,@pCorporateID,@pTimeZone,@pSenderID,@pLoggedInUserId)

		SET @pId = SCOPE_IDENTITY()
	END

	IF @pId > 0
	BEGIN
		DELETE
		FROM FacilityContact
		WHERE FacilityId = @pId

		INSERT INTO FacilityContact (ContactName, Email, FacilityId, IsMain)
		SELECT C.ContactName, C.Email, @pId, C.IsMain
		FROM @pContact C
	END
	EXEC SPROC_DefaultFacilityItems @pId,@pName,@pLoggedInUserId

	SELECT @pId

	SELECT F.*, FacilityContact = (
			SELECT F1.ContactName, F1.Email, F1.FacilityId, F1.IsMain
			FROM FacilityContact F1
			WHERE F1.FacilityId = F.FacilityId
			FOR JSON PATH
			)
	FROM Facility F
	WHERE F.CorporateId = @pCorporateId
	FOR JSON PATH, Root('Facility'), INCLUDE_NULL_VALUES
END