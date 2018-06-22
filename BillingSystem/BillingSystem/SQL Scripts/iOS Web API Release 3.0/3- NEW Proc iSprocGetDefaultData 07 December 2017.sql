-- Drop stored procedure if it already exists
IF OBJECT_ID('iSprocGetDefaultData','P') IS NOT NULL
   DROP PROCEDURE iSprocGetDefaultData
GO

CREATE PROCEDURE iSprocGetDefaultData
As
Begin
	Select Country=ISNULL((Select [Name]=CountryName,[Value]=CountryID From Country
	Where ISNULL(IsDeleted,0)=0 And CountryID='45' FOR JSON PATH),'[]')
	,
	[State]=ISNULL((Select [Name]=StateName,[Value]=StateID From [State] 
	Where ISNULL(IsDeleted,0)=0 And [StateID]=3 FOR JSON PATH),'[]') 
	,
	City=ISNULL((Select [Name],[Value]=CityID From City 
	Where ISNULL(IsDeleted,0)=0 And CityID=3 FOR JSON PATH),'[]') 
	FOR JSON PATH,Root('Defaults')
End

