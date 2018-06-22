IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'iSprocSavePatientInfo') 
  DROP PROCEDURE iSprocSavePatientInfo;
GO
/****** Object:  StoredProcedure [dbo].[SprocGetAvailableTimeSlots]    Script Date: 10/4/2017 5:30:09 PM ******/
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
@pDeviceToken nvarchar(200),
@pPlatform nvarchar(20)
)
AS
BEGIN
	---###### Declarations ##################

	Declare @LocalDateTime datetime, @TimeZone nvarchar(50),@Count int,@ResponseCode int=-1,@pAge int=0
	Declare @CodeValue nvarchar(50) = floor(rand() * 100000 - 1),@PreviousEmailId nvarchar(500)

	---###### Declarations ##################


	SET @TimeZone = '+04:00'

	IF ISNULL(@pAge,0)=0
		SET @pAge = CONVERT(int,ROUND(DATEDIFF(hour,@pDob,GETDATE())/8766.0,0))

	SET @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,GETDATE()))

	IF EXISTS (Select 1 From PatientInfo Where PersonEmailAddress=@pEmail ) 
	   OR EXISTS(Select 1 From PatientPhone Where PhoneNo=@pPhone)
			SET @ResponseCode=0

	IF (@ResponseCode !=0)
	Begin
		Declare @NewP Table (Id int)
		IF @pPid > 0 
		Begin
			Update PatientInfo Set PersonBirthDate=@pDob, PersonFirstName=@pFirstName,PersonLastName=@pLastName,PersonGender=@pGender
			,ModifiedBy=@pPid,ModifiedDate=GETUTCDATE()
			Where PatientID=@pPid
		End
		Else
		Begin
			Insert Into PatientInfo(CorporateId, FacilityId,CreatedBy,PersonFirstName,PersonLastName,IsDeleted,PersonBirthDate
			,PersonEmailAddress,PersonEmiratesIDNumber,CreatedDate,PersonAge)
			OUTPUT INSERTED.PatientID INTO @NewP
			Select 0 As CorporateId,0 As FacilityId,0,@pFirstName,@pLastName,0,@pDob,@pEmail,@pEmirates,@LocalDateTime,@pAge

			Select @pPid = Id From @NewP

			Update PatientInfo SET CreatedBy = @pPid Where PatientID = @pPid

			Insert Into PatientPhone (PatientId,[PhoneType],[PhoneNo],[IsPrimary],[CreatedBy],[CreatedDate])
			Select @pPid As PatientId,2,@pPhone,1,@pPid,@LocalDateTime

			--Saving Patient's Login Details
			INSERT INTO [dbo].[PatientLoginDetail]
			([PatientId],[Email],[TokenId],[PatientPortalAccess],[CodeValue],[ExternalValue1],[FailedLoginAttempts]
			,[CreatedBy],[CreatedDate],[IsDeleted],[Password])
			Select @pPid,@pEmail,@CodeValue As TokenId,1 As PatientPortalAccess,'101'+@CodeValue,'1',0
			,@pPid,@LocalDateTime,0,ISNULL(@pPwd,'')
		End
	End

	Exec iSprocAuthenticateUser '','','',@pDeviceToken,@pPlatform,1,@pPid
END





