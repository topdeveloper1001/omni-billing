IF EXISTS ( SELECT * 
            FROM   sysobjects 
            WHERE  id = object_id(N'[dbo].[iSprocAuthenticateUser]') 
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[iSprocAuthenticateUser]

Go

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[iSprocAuthenticateUser]
(
@pUsername nvarchar(100),
@pPassword nvarchar(100),
@pLoginType nvarchar(20),
@pDeviceToken nvarchar(50),
@pPlatform nvarchar(50),
@pIsPatient bit=1,
@pUserId int=0
)
AS
BEGIN
	
	--$$$$$$$$$$ Declarations $$$$$$$$$$$$$$----
	
	Declare @UserId bigint=0,@IsDefaultDevice bit=0,@IsAuthenticated bit=0,@CurrentDate datetime=null
	
	--$$$$$$$$$$ Declarations $$$$$$$$$$$$$$----

	--Check if User is Patient or Medical Provider.
	IF ISNULL(@pIsPatient,1)=1
	Begin
		--Check If Patient exists in the system
		Select @UserId=PatientId From PatientLoginDetail Where Email=@pUsername And [Password]=@pPassword And ISNULL(IsDeleted,0)=0		
	End
	Else
		Select @UserId=UserID from Users Where UserName=@pUsername And [Password]=@pPassword And IsActive=1

	IF @UserId > 0
	Begin
		IF @CurrentDate IS NULL
			SET @CurrentDate=GETUTCDATE()

		--Patient is using app for the first time, then device is set to be default
		IF (Select Count(1) From UserDevice Where UserId=@UserId)=0
			Set @IsDefaultDevice=1


		IF Exists(Select 1 From UserDevice Where DeviceToken=@pDeviceToken And IsActive=1 And UserId=@UserId)
		Begin
			Update UserDevice SET [Platform]=@pPlatform, ModifiedBy=@UserId,ModifiedDate=@CurrentDate 
			Where UserId=@UserId And DeviceToken=@pDeviceToken
		End
		Else
			INSERT INTO UserDevice (UserId,IsPatient,DeviceToken,[Platform],IsDefault,CreatedBy,CreatedDate,IsActive)
			Select @UserId,1,@pDeviceToken,@pPlatform,@IsDefaultDevice,@UserId,@CurrentDate,1


		SET @IsAuthenticated=1

		Select @IsAuthenticated As IsAuthenticated

		IF @pIsPatient=1
			Select @pIsPatient As IsPatient,PatientID As UserID,PersonEmailAddress As Email
			,PersonContactMobileNumber [Phone], (PersonFirstName +  ' ' + PersonLastName) [Name]
			,PersonEmailAddress As UserName,'Email' As LoginType
			From PatientInfo Where PatientID = @UserId And ISNULL(IsDeleted,0)=0
			FOR JSON PATH, ROOT('UserDto')
		Else
			Select @pIsPatient As IsPatient,UserID,Email,Phone,(FirstName + ' ' + LastName) As [Name]
			,UserName,'Username' as LoginType
			From Users Where IsActive=1 And UserName=@pUsername And [Password]=@pPassword And ISNULL(IsDeleted,0)=0 
			FOR	JSON PATH, ROOT('UserDto')
	End
	Else
		Select @IsAuthenticated As IsAuthenticated
END
