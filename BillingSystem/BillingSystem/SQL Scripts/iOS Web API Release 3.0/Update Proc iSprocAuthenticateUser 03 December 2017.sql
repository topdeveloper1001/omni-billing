USE [OmniStagingDB]
GO
/****** Object:  StoredProcedure [dbo].[iSprocAuthenticateUser]    Script Date: 03-12-2017 14:50:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[iSprocAuthenticateUser]
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
	IF @pUserId=0
	Begin
		IF ISNULL(@pIsPatient,1)=1
		Begin
			--Check If Patient exists in the system
			Select @UserId=PatientId From PatientLoginDetail Where Email=@pUsername And [Password]=@pPassword And ISNULL(IsDeleted,0)=0		
		End
		Else
			Select @UserId=UserID from Users Where UserName=@pUsername And [Password]=@pPassword And IsActive=1
	End
	Else
	Begin
		Set @UserId=@pUserId
	End

	IF @UserId > 0
	Begin
		IF @CurrentDate IS NULL
			SET @CurrentDate=GETUTCDATE()

		--Patient is using app for the first time, then device is set to be default
		IF (Select Count(1) From UserDevice Where UserId=@UserId)=0
			Set @IsDefaultDevice=1


		IF ISNULL(@pDeviceToken,'') !='' AND ISNULL(@pPlatform,'') !=''
		Begin
			IF Exists(Select 1 From UserDevice Where DeviceToken=@pDeviceToken And IsActive=1 And UserId=@UserId)
			Begin
				Update UserDevice SET [Platform]=@pPlatform, ModifiedBy=@UserId,ModifiedDate=@CurrentDate 
				Where UserId=@UserId And DeviceToken=@pDeviceToken
			End
			Else
				INSERT INTO UserDevice (UserId,IsPatient,DeviceToken,[Platform],IsDefault,CreatedBy,CreatedDate,IsActive)
				Select @UserId,1,@pDeviceToken,@pPlatform,@IsDefaultDevice,@UserId,@CurrentDate,1
		End


		SET @IsAuthenticated=1

		Select @IsAuthenticated As IsAuthenticated

		IF @pIsPatient=1
			Select @pIsPatient As IsPatient,P.PatientID As UserID,P.PersonEmailAddress As Email
			,P.PersonContactMobileNumber [Phone], (P.PersonFirstName +  ' ' + P.PersonLastName) [Name]
			,P.PersonEmailAddress As UserName,'Email' As LoginType,
			P.PersonFirstName As FirstName, P.PersonLastName As LastName
			,(Select TOP 1 PhoneNo From PatientPhone PH Where P.PatientId=PH.PatientId AND IsPrimary=1) As HomePhone
			, ISNULL(P.PersonGender,'') As Sex
			,(Case 
				ISNULL(P.FacilityId,0) When 
											0 THEN  ''
									   ELSE
									(Select TOP 1 ISNULL(FacilityName,'') From Facility Where FacilityId=P.FacilityId) 
									END) As HomeCity
			,P.PersonBirthDate As BirthDate
			,Cast(0 As bit) As AdminUser, L.[Password]
			From PatientInfo P
			INNER JOIN PatientLoginDetail L ON P.PatientId=L.PatientId
			Where P.PatientID = @UserId And ISNULL(P.IsDeleted,0)=0
			FOR JSON PATH, ROOT('UserDto')
		Else
			Select @pIsPatient As IsPatient,U.UserID,U.Email,U.Phone,(U.FirstName + ' ' + U.LastName) As [Name]
			,U.UserName,'Username' as LoginType
			,U.FirstName, U.LastName
			,(Case 
				ISNULL(U.FacilityId,0) When 
											0 THEN  ''
									   ELSE
									(Select TOP 1 ISNULL(FacilityName,'') From Facility Where FacilityId=U.FacilityId) 
									END) As HomeCity
			,U.AdminUser, U.[Password]
			From Users U
			Where IsActive=1 And U.UserID=@UserId And ISNULL(IsDeleted,0)=0 
			FOR	JSON PATH, ROOT('UserDto')
	End
	Else
		Select @IsAuthenticated As IsAuthenticated
END
