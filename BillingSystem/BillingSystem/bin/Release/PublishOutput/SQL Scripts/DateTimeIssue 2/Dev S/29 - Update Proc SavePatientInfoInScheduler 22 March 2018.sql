IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SavePatientInfoInScheduler')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SavePatientInfoInScheduler
GO

/****** Object:  StoredProcedure [dbo].[SavePatientInfoInScheduler]    Script Date: 22-03-2018 19:34:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SavePatientInfoInScheduler]
(
@pCId int=0,
@pFId int=0,
@pDate datetime=null,
@pPid int,
@pFirstName nvarchar(100),
@pLastName nvarchar(100),
@pDob datetime,
@pEmail nvarchar(100),
@pEmirates nvarchar(100)=null,
@pLoggedInUserId int=null,
@pPhone nvarchar(20),
@pAge int=0,
@pPwd nvarchar(200)=null
)
AS
BEGIN
	---###### Declarations ##################

	Declare @Count int

	Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFId))

	---###### Declarations ##################

	--SET @TimeZone=(Select TimeZ from Facility Where Facilityid=@pFId)

	--IF ISNULL(@TimeZone,'')=''
	--	SET @TimeZone = '+04:00'

	IF ISNULL(@pAge,0)=0
		SET @pAge = CONVERT(int,ROUND(DATEDIFF(hour,@pDob,@LocalDateTime)/8766.0,0))

	--SET @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,@LocalDateTime))

	IF Exists (Select 1 From PatientLoginDetail Where Email=@pEmail And ISNULL(IsDeleted,0)=0 
	AND PatientId != @pPid)
	OR Exists (Select 1 From PatientInfo Where PersonEmailAddress=@pEmail And ISNULL(IsDeleted,0)=0
	AND PatientId != @pPid)
	Begin
		--Select TOP 1 @pPid=PatientId From PatientInfo Where PersonEmailAddress=@pEmail And PatientId != @pPid And ISNULL(IsDeleted,0)=0 
		Select Cast(-2 as int) As [Result]
		Return;
	End


	IF (@pPid=0)
	Begin
		Declare @NewP Table (Id int)

		Insert Into PatientInfo(CorporateId, FacilityId,CreatedBy,PersonFirstName,PersonLastName,IsDeleted,PersonBirthDate
		,PersonEmailAddress,PersonEmiratesIDNumber,CreatedDate,PersonAge)
		OUTPUT INSERTED.PatientID INTO @NewP
		Select ISNULL(@pCId,0),ISNULL(@pFId,0),@pLoggedInUserId,@pFirstName,@pLastName,0,@pDob,@pEmail,@pEmirates,@LocalDateTime,@pAge

		Select @pPid = Id From @NewP

		IF ISNULL(@pLoggedInUserId,0)=0
		Begin	
			SET @pLoggedInUserId = @pPid
			Update PatientInfo SET CreatedBy = @pLoggedInUserId Where PatientID = @pPid
		End

		Insert Into PatientPhone (PatientId,[PhoneType],[PhoneNo],[IsPrimary],[CreatedBy],[CreatedDate])
		Select @pPid As PatientId,2,@pPhone,1,@pLoggedInUserId,@LocalDateTime
	End
	Else
	Begin
		IF ISNULL(@pLoggedInUserId,0)=0
			SET @pLoggedInUserId = @pPid

		Update PatientInfo Set PersonFirstName=@pFirstName,PersonLastName=@pLastName,PersonBirthDate=@pDob
		,PersonEmailAddress=@pEmail,PersonEmiratesIDNumber=@pEmirates,ModifiedBy=@pLoggedInUserId,[ModifiedDate]=@LocalDateTime,
		PersonAge=@pAge
		Where PatientId = @pPid

		IF NOT Exists (Select 1 From PatientPhone Where PatientID = @pPid And ISNULL(IsDeleted,0)=0)
		Begin
			Insert Into PatientPhone (PatientId,[PhoneType],[PhoneNo],[IsPrimary],[CreatedBy],[CreatedDate])
			Select @pPid,2,@pPhone,1,@pLoggedInUserId,@LocalDateTime
		End
		Else
		Begin
			Update PatientPhone Set PatientId=@pPid,PhoneType=2,[PhoneNo]=@pPhone,IsPrimary=1,ModifiedBy=@pLoggedInUserId
			,ModifiedDate=@LocalDateTime
			Where PatientID=@pPid
		End
	End

	--Saving Patient's Login Details
	Declare @CodeValue nvarchar(50) = floor(rand() * 100000 - 1),@PreviousEmailId nvarchar(500)

	IF NOT EXISTS (Select 1 from PatientLoginDetail WHERE PatientId=@pPid AND ISNULL(ISDeleted,0)=0)
	BEGIN
		INSERT INTO [dbo].[PatientLoginDetail]
		([PatientId],[Email],[TokenId],[PatientPortalAccess],[CodeValue],[ExternalValue1],[FailedLoginAttempts]
		,[CreatedBy],[CreatedDate],[IsDeleted],[Password])
		Select @pPid,@pEmail,@CodeValue As TokenId,1 As PatientPortalAccess,'101'+@CodeValue,'1',0
		,@pLoggedInUserId,@LocalDateTime,0,ISNULL(@pPwd,'')
	END
	ELSE
	BEGIN
		Select @PreviousEmailId = [Email] From [PatientLoginDetail] Where [PatientId] = @pPid AND (ISDeleted is null OR ISDeleted =0)
		IF(@PreviousEmailId <> @pEmail)
		BEGIN
			Update PatientLoginDetail SET Email = @pEmail, ModifiedBy=@pLoggedInUserId, ModifiedDate=@LocalDateTime 
			Where PatientId=@pPid AND (ISDeleted is null OR ISDeleted =0)
		END
	END

	Select @pPid
END





GO


