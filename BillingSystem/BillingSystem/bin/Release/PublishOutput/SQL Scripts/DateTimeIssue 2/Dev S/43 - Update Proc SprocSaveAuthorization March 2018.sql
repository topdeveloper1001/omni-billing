IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocSaveAuthorization')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocSaveAuthorization
GO

/****** Object:  StoredProcedure [dbo].[SprocSaveAuthorization]    Script Date: 22-03-2018 20:07:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocSaveAuthorization]
(
	@Id int,
	@pCId bigint,
	@pFId bigint,
	@pPId bigint,
	@pEId bigint=0,
	@pDateOrdered datetime=null,
	@pStart datetime,
	@pEnd datetime,
	@pCode nvarchar(50),
	@pType int=0,
	@pComments nvarchar(150)=null,
	@pDenialCode int=null,
	@pIDPayer nvarchar(50)=null,
	@pLimit decimal(18, 2),
	@pMemberId nvarchar(50),
	@pResult int=null,
	@pUserId bigint,
	@pCreatedDate datetime=null,
	@pServiceLevel nvarchar(10)=null,
	@pFilesDataInJson nvarchar(max)=null,
	@pWithAuthList bit=1,
	@pWithDocs bit=1,
	@pMissedEId bigint
)
AS
BEGIN
	--Declaration
	Declare @AssociatedTypeId nvarchar(10)=(Select TOP 1 GlobalCodeValue From GlobalCodes Where GlobalCodeCategoryValue='80442' And GlobalCodeName='Authorization')
	,@EncounterNumber nvarchar(100)=(Select TOP 1 EncounterNumber From Encounter Where EncounterID=@pEId)
	,@PatientName nvarchar(200)=(Select TOP 1 (PersonFirstName + ' ' + PersonLastName) As PatientName From PatientInfo Where PatientID=@pPId)
	,@FacilityName nvarchar(200)=''
	,@CorporateName nvarchar(200)=(Select TOP 1 CorporateName From Corporate Where CorporateID=@pFId)
	,@Status int=@Id

	Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFId))

	--Temporary Table to get files data from json string.
	Declare @TFiles Table (Id int, F1 nvarchar(max), F2 nvarchar(max))
	Declare @pivot_columns nvarchar(max)
	----------------------------------------------------------------------------------------------------------------------------------------------------------------

	--Get Current Date time based on Facility's Time Zone and Facility Name.
	--(Select TOP 1 @FacilityName=FacilityName, @TimeZone=TimeZ From Facility Where FacilityId=@pFId)

	--Check If Current Date is null, get it from UDF.
	--IF @pCreatedDate IS NULL AND ISNULL(@TimeZone,'') !=''
	--	Set @pCreatedDate = dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,@CurrentDate)
	

	--Check the existing record while adding new entry, based on Dates.
	If @Id = 0 And Not Exists (
		Select 1 From [Authorization] 
		Where CAST(AuthorizationStart as date) >= CAST(@pStart as date)
		And CAST(AuthorizationEnd as date) = CAST(@pEnd as date) And PatientID=@pPId)
	Begin
		INSERT INTO [dbo].[Authorization] (
		[CorporateID],[FacilityID],[PatientID],[EncounterID],[AuthorizationDateOrdered]
		,[AuthorizationStart],[AuthorizationEnd],[AuthorizationCode],[AuthorizationType],[AuthorizationComments]
		,[AuthorizationDenialCode],[AuthorizationIDPayer],[AuthorizationLimit],[AuthorizationMemberID]
		,[AuthorizationResult],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy]
		,[DeletedDate],[AuthorizedServiceLevel]
		)
		Select @pCId,@pFId,@pPId,@pEId,@pDateOrdered
		,@pStart,@pEnd,@pCode,@pType,@pComments
		,@pDenialCode,@pIDPayer,@pLimit,@pMemberId
		,@pResult,@pUserId,@pCreatedDate,NULL As ModifiedBy,NULL As ModifiedDate,0 As IsDeleted,NULL AS DeletedBy
		,NULL AS DeletedDate,@pServiceLevel

		SET @Id=SCOPE_IDENTITY()
		Set @Status=@Id

		--Update the Encounter End Check
		IF @Id > 0 And @pMissedEId>0 And @pUserId > 0
			Exec SPROC_EncounterEndCheckBillEdit @pMissedEId,@pUserId 
	End
	Else 
	Begin
		SET @Status=dbo.GetErrorCode('Duplicate')	--Return Error Code in case of Duplicate found.
		Update [Authorization] Set
		[AuthorizationStart]=@pStart,[AuthorizationEnd]=@pEnd,[AuthorizationCode]=@pCode
		,[AuthorizationComments]=@pComments
		,[AuthorizationDenialCode]=@pDenialCode
		,[AuthorizationIDPayer]=@pIDPayer
		,[AuthorizationLimit]=@pLimit
		,[AuthorizationResult]=@pResult,[ModifiedBy]=@pUserId,[ModifiedDate]=@pCreatedDate
		,[AuthorizedServiceLevel]=@pServiceLevel
		,[AuthorizationMemberID]=@pMemberId
		Where AuthorizationID=@Id 

		Set @Status=@Id
	End	

	--Update the Encounter End Check
	IF @Id > 0 And @pMissedEId>0 And @pUserId > 0
		Exec SPROC_EncounterEndCheckBillEdit @pMissedEId,@pUserId 

	--Save Document Templates against Authorization.
	If @Status > 0 And ISNULL(@pFilesDataInJson,'') !=''
	Begin
		Delete From @TFiles

		INSERT INTO @TFiles
		select Id,StringValue ,[Name]
		from (
			Select row_number() over (partition by [Name] order by [Name]) As Id, * From dbo.parseJSON(@pFilesDataInJson)
		Where [Name] IN ('FileName','FilePath')
		) Temp

		SELECT
		@pivot_columns = 
		isnull(@pivot_columns + ', ', '') + 
		'[' +  F2 + ']'
		from (select distinct F2 from @TFiles) as T

		INSERT INTO DocumentsTemplates (
		[DocumentTypeID],[DocumentName],[DocumentNotes],[AssociatedID],[AssociatedType]
		,[FileName],[FilePath],[IsTemplate],[IsRequired],[CreatedBy],[CreatedDate],[IsDeleted],[DeletedBy],[DeletedDate]
		,[ModifiedBy],[ModifiedDate],[CorporateID],[FacilityID],[PatientID],[EncounterID],[ExternalValue1],[ExternalValue2]
		,[ExternalValue3]
		)
		SELECT @AssociatedTypeId,'Authorization File','',@Id,@AssociatedTypeId
		,[FileName],[FilePath],0,0,@pUserId,@pCreatedDate,0 As IsDeleted,NULL As DeletedBy,NULL
		,NULL AS ModifiedBy,NULL As ModifiedDate,@pCId,@pFId,@pPId,@pEId,'','',''
		from @TFiles
		pivot
		(
		min(F1)
		for F2 in ([FileName], [FilePath])
		) as PT
	End


	--Return the latest ID as a result of the above queries.
	Select @Status

	--Return list of Authorization with other details
	IF @pWithAuthList=1
		Select A.*
		--,@EncounterNumber As EncounterNumber,@PatientName As PatientName,@FacilityName As FacilityName,@CorporateName As CorporateName
		,(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeCategoryValue='1701' And G.GlobalCodeValue=A.AuthorizationType) As AuthrizationTypeStr
		,(Case 
			When ISNULL(A.IsDeleted,0)=0 And A.AuthorizationEnd IS NOT NULL AND CAST(A.AuthorizationEnd as date) >= CAST(@pCreatedDate as date)
			THEN CAST(1 as bit) 
			Else 
			Cast(0 As bit)
			End) As isActive
		,(Select TOP 1 C.InsuranceCompanyLicenseNumber From InsuranceCompany C Where C.InsuranceCompanyLicenseNumber=A.AuthorizationIDPayer) As IdPayer
		From [Authorization] A Where AuthorizationID=@Id

	IF @pWithDocs=1
		Exec SprocGetDocumentsByPatient @pFId,@pCId,@pUserId,@pPId,'profilepicture'
		--Select D.*,@EncounterNumber As EncounterNumber,@PatientName As PatientName,@FacilityName As FacilityName,@CorporateName As CorporateName
		--From DocumentsTemplates D Where D.AssociatedID=@Id And D.AssociatedType=@AssociatedTypeId
END
GO


