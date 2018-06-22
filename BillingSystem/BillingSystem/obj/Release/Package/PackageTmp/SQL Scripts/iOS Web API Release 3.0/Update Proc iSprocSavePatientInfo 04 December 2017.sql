USE [OmniStagingDB]
GO
/****** Object:  StoredProcedure [dbo].[iSprocSavePatientInfo]    Script Date: 04-12-2017 23:12:21 ******/
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
ALTER PROCEDURE [dbo].[iSprocSavePatientInfo]
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
@pCity nvarchar(100)
)
AS
BEGIN
	---###### Declarations ##################

	Declare @LocalDateTime datetime, @TimeZone nvarchar(50),@Count int,@ResponseCode int=1,@Age int=null
	Declare @CodeValue nvarchar(50) = floor(rand() * 100000 - 1),@PreviousEmailId nvarchar(500)

	---###### Declarations ##################


	SET @TimeZone = '+04:00'

	IF ISNULL(@Age,0)=0 AND @pDob IS NOT NULL
		SET @Age = CONVERT(int,ROUND(DATEDIFF(hour,@pDob,GETDATE())/8766.0,0))

	SET @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,GETDATE()))

	IF EXISTS (Select 1 From PatientInfo Where PersonEmailAddress=@pEmail And (ISNULL(@pPid,0)=0 OR PatientId!=@pPid)) 
	   OR EXISTS(Select 1 From PatientPhone Where PhoneNo=@pPhone And (ISNULL(@pPid,0)=0 OR PatientId!=@pPid))
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
				,ModifiedBy=@pPid,ModifiedDate=@LocalDateTime,ExternalValue2=@pCity
				,[Password]=ISNULL(@pPwd,[Password]) Where PatientId=@pPid
			End

			IF NOT EXISTS (Select 1 From PatientPhone Where PatientId=@pPid)
				Insert Into PatientPhone (PatientId,[PhoneType],[PhoneNo],[IsPrimary],[CreatedBy],[CreatedDate])
				Select @pPid As PatientId,2,@pPhone,1,@pPid,@LocalDateTime
			ELSE
				Update PatientPhone SET [PhoneNo]=ISNULL(@pPhone,[PhoneNo]), ModifiedBy=@pPid, ModifiedDate=@LocalDateTime
				Where PatientId=@pPid
		End
		Else
		Begin
			Insert Into PatientInfo(CorporateId, FacilityId,CreatedBy,PersonFirstName,PersonLastName,IsDeleted,PersonBirthDate
			,PersonEmailAddress,PersonEmiratesIDNumber,CreatedDate,PersonAge)
			OUTPUT INSERTED.PatientID INTO @NewP
			Select 0 As CorporateId,0 As FacilityId,0,@pFirstName,@pLastName,0,@pDob,@pEmail,@pEmirates,@LocalDateTime,@Age

			Select @pPid = Id From @NewP

			Update PatientInfo SET CreatedBy = @pPid Where PatientID = @pPid

			IF NOT EXISTS (Select 1 From PatientPhone Where PatientId=@pPid)
				Insert Into PatientPhone (PatientId,[PhoneType],[PhoneNo],[IsPrimary],[CreatedBy],[CreatedDate])
				Select @pPid As PatientId,2,@pPhone,1,@pPid,@LocalDateTime
			ELSE
				Update PatientPhone SET [PhoneNo]=ISNULL(@pPhone,[PhoneNo]), ModifiedBy=@pPid, ModifiedDate=@LocalDateTime
				Where PatientId=@pPid

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
				,ModifiedBy=@pPid,ModifiedDate=@LocalDateTime,ExternalValue2=@pCity
				,[Password]=ISNULL(@pPwd,'') Where PatientId=@pPid
			End
		End
	End

	Exec iSprocAuthenticateUser '','','',@pDeviceToken,@pPlatform,1,@pPid
END





