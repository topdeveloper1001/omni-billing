IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'iSprocSavePatientInfo')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE iSprocSavePatientInfo
GO

/****** Object:  StoredProcedure [dbo].[iSprocSavePatientInfo]    Script Date: 22-03-2018 19:54:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Exec [SprocGetOrderCodesToExport] '','ATC',9,8,'','',10
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[iSprocSavePatientInfo]
(
@pPid int,
@pFirstName nvarchar(100),
@pLastName nvarchar(100),
@pDob datetime,
@pEmail nvarchar(100),
@pEmirates nvarchar(100)=null,
@pPhone nvarchar(20),
@pPwd nvarchar(200)=null,
@pGender nvarchar(100),
@pDeviceToken nvarchar(200)=null,
@pPlatform nvarchar(20)=null,
@pCityId bigint=null,
@pStateId bigint=null,
@pCountryId bigint=null
)
AS
BEGIN
	---###### Declarations ##################

	Declare @Count int,@ResponseCode int=1,@Age int=null
	Declare @CodeValue nvarchar(50) = floor(rand() * 100000 - 1),@PreviousEmailId nvarchar(500),@phone nvarchar(20)=@pPhone
	Declare @CountryCode nvarchar(10)=(Select CodeValue From [Country] Where CountryID=@pCountryId)

	---###### Declarations ##################


	IF ISNULL(@phone,'') !='' AND CHARINDEX('+',@phone,0) <= 0 AND @CountryCode !=''
		Set @phone='+' + @CountryCode + '-' + @phone

	Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(0))

	IF ISNULL(@Age,0)=0 AND @pDob IS NOT NULL
		SET @Age = CONVERT(int,ROUND(DATEDIFF(hour,@pDob,@LocalDateTime)/8766.0,0))

	IF EXISTS (Select 1 From PatientInfo Where PersonEmailAddress=@pEmail And (ISNULL(@pPid,0)=0 OR PatientId!=@pPid)) 
	   OR EXISTS(Select 1 From PatientPhone Where PhoneNo=@phone And (ISNULL(@pPid,0)=0 OR PatientId!=@pPid))
			SET @ResponseCode=0

	IF (@ResponseCode !=0)
	Begin
		Declare @NewP Table (Id int)
		IF @pPid > 0 
		Begin
			Update PatientInfo Set PersonBirthDate=ISNULL(@pDob,PersonBirthDate), PersonFirstName=@pFirstName,PersonLastName=@pLastName
			,PersonGender=@pGender, PersonEmiratesIDNumber=ISNULL(@pEmirates,PersonEmiratesIDNumber)
			,PersonAge=ISNULL(@Age,PersonAge)
			,ModifiedBy=@pPid,ModifiedDate=GETUTCDATE()
			,PersonCity=ISNULL(@pCityId,0)
			,PersonCountry=ISNULL(@pCountryId,0)
			,PersonArea=ISNULL(@pStateId,0)
			Where PatientID=@pPid

			IF NOT EXISTS (Select 1 From PatientLoginDetail Where PatientId=@pPid)
			Begin
				--Saving Patient's Login Details
				INSERT INTO [dbo].[PatientLoginDetail]
				([PatientId],[Email],[TokenId],[PatientPortalAccess],[CodeValue],[ExternalValue1],[FailedLoginAttempts]
				,[CreatedBy],[CreatedDate],[IsDeleted],[Password])
				Select @pPid,@pEmail,@CodeValue As TokenId,1 As PatientPortalAccess,'101'+@CodeValue,'1',0
				,@pPid,@LocalDateTime,0,ISNULL(@pPwd,'')
			End
			Else
			Begin
				Update PatientLoginDetail SET TokenId=@CodeValue, CodeValue='101'+@CodeValue,ExternalValue1='1'
				,ModifiedBy=@pPid,ModifiedDate=@LocalDateTime,ExternalValue2=@pCityId
				,[Password]=ISNULL(@pPwd,[Password]) Where PatientId=@pPid
			End


			IF NOT EXISTS (Select 1 From PatientPhone Where PatientId=@pPid)
				Insert Into PatientPhone (PatientId,[PhoneType],[PhoneNo],[IsPrimary],[CreatedBy],[CreatedDate])
				Select @pPid As PatientId,2,@phone,1,@pPid,@LocalDateTime
			ELSE
				Update PatientPhone SET [PhoneNo]=ISNULL(@phone,[PhoneNo]), ModifiedBy=@pPid, ModifiedDate=@LocalDateTime
				Where PatientId=@pPid
		End
		Else
		Begin
			Insert Into PatientInfo(CorporateId, FacilityId,CreatedBy,PersonFirstName,PersonLastName,IsDeleted,PersonBirthDate
			,PersonEmailAddress,PersonEmiratesIDNumber,CreatedDate,PersonAge,PersonCountry,PersonCity
			,PersonArea)
			OUTPUT INSERTED.PatientID INTO @NewP
			Select 0 As CorporateId,0 As FacilityId,0,@pFirstName,@pLastName,0,@pDob,@pEmail,@pEmirates,@LocalDateTime,@Age
			,@pCountryId,@pCityId,@pStateId

			Select @pPid = Id From @NewP

			Update PatientInfo SET CreatedBy = @pPid Where PatientID = @pPid

			IF NOT EXISTS (Select 1 From PatientPhone Where PatientId=@pPid)
				Insert Into PatientPhone (PatientId,[PhoneType],[PhoneNo],[IsPrimary],[CreatedBy],[CreatedDate])
				Select @pPid As PatientId,2,@phone,1,@pPid,@LocalDateTime
			ELSE
				Update PatientPhone SET [PhoneNo]=ISNULL(@phone,[PhoneNo]), ModifiedBy=@pPid, ModifiedDate=@LocalDateTime
				Where PatientId=@pPid

			IF NOT EXISTS (Select 1 From PatientLoginDetail Where PatientId=@pPid)
			Begin
				--Saving Patient's Login Details
				INSERT INTO [dbo].[PatientLoginDetail]
				([PatientId],[Email],[TokenId],[PatientPortalAccess],[CodeValue],[ExternalValue1],[FailedLoginAttempts]
				,[CreatedBy],[CreatedDate],[IsDeleted],[Password],ExternalValue2)
				Select @pPid,@pEmail,@CodeValue As TokenId,1 As PatientPortalAccess,'101'+@CodeValue,'1',0
				,@pPid,@LocalDateTime,0,ISNULL(@pPwd,''),@pCityId
			End
			Else
			Begin
				Update PatientLoginDetail SET TokenId=@CodeValue, CodeValue='101'+@CodeValue,ExternalValue1='1'
				,ModifiedBy=@pPid,ModifiedDate=@LocalDateTime,ExternalValue2=@pCityId
				,[Password]=ISNULL(@pPwd,'') Where PatientId=@pPid
			End
		End
	End

	Exec iSprocAuthenticateUser '','',@pDeviceToken,@pPlatform,1,@pPid
END





GO


