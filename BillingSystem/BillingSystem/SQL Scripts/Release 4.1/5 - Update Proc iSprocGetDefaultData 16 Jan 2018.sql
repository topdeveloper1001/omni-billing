IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'iSprocGetDefaultData')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE iSprocGetDefaultData
GO
/****** Object:  StoredProcedure [dbo].[iSprocGetDefaultData]    Script Date: 16-01-2018 12:36:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[iSprocGetDefaultData]
(
@pUserId bigint
)
As
Begin
	Declare @CountrId bigint,@StateId bigint,@CityId bigint

	IF Exists (Select 1 From [Users] U INNER JOIN UserRole UR ON U.UserId=UR.UserId
				Where U.UserId=@pUserId)
	Begin
		Select @CountrId=U.CountryID,@StateId=U.StateID,@CityId=U.CityID From [Users] U
		Where U.UserID=@pUserId	
	End
	Else IF Exists (Select 1 From PatientInfo Where PatientId=@pUserId)
	Begin
		Select @CountrId=PersonCountry,@StateId=PersonArea,@CityId=PersonCity From PatientInfo
		Where PatientId=@pUserId
	End

	SET @CountrId = ISNULL(@CountrId,45)
	SET @StateId = ISNULL(@StateId,3)
	SET @CityId = ISNULL(@CityId,3)

	--Select Country=ISNULL((Select [Name]=CountryName,[Value]=CountryID From Country
	--Where ISNULL(IsDeleted,0)=0 And CountryID=@CountrId FOR JSON PATH),'[]')
	--,
	--[State]=ISNULL((Select [Name]=StateName,[Value]=StateID From [State] 
	--Where ISNULL(IsDeleted,0)=0 And [StateID]=@StateId FOR JSON PATH),'[]') 
	--,
	--City=ISNULL((Select [Name],[Value]=CityID From City 
	--Where ISNULL(IsDeleted,0)=0 And CityID=@CityId FOR JSON PATH),'[]') 
	--FOR JSON PATH,Root('Defaults')

	Select [Name]=CountryName,[Value]=CAST(CountryID as bigint) From Country
	Where ISNULL(IsDeleted,0)=0 And CountryID=@CountrId


	Select [Name]=StateName,[Value]=CAST(StateID as bigint) From [State] 
	Where ISNULL(IsDeleted,0)=0 And [StateID]=@StateId
	
	Select [Name],[Value]=CAST(CityID as bigint) From City 
	Where ISNULL(IsDeleted,0)=0 And CityID=@CityId
End

