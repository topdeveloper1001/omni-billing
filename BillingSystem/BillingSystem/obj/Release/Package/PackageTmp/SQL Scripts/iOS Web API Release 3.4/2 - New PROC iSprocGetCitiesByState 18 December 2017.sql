-- Drop stored procedure if it already exists
IF OBJECT_ID('iSprocGetCitiesByState','P') IS NOT NULL
   DROP PROCEDURE iSprocGetCitiesByState
GO

CREATE PROCEDURE iSprocGetCitiesByState
(
@pStateId bigint=0
)
As
Begin
	Select [Name]=LTRIM(RTRIM([Name])),[Value]=CAST(CityID as bigint)
	From City Where IsActive=1 And IsDeleted=0 
	And (@pStateId=0 OR StateID=@pStateId)
	And CityID IN (Select DISTINCT FacilityCity From Facility)
	Order by [Name]
	FOR JSON PATH,ROOT('Cities')
End