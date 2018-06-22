IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'iSprocGetCitiesByState')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE iSprocGetCitiesByState
GO

/****** Object:  StoredProcedure [dbo].[iSprocGetCitiesByState]    Script Date: 4/10/2018 5:04:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[iSprocGetCitiesByState]
(
@pStateId bigint=0
)
As
Begin
	Select [Name]=LTRIM(RTRIM([Name])),[Value]=CAST(CityID as bigint)
	From City Where IsActive=1 And IsDeleted=0 
	And (@pStateId=0 OR StateID=@pStateId)
	--And CityID IN (Select DISTINCT FacilityCity From Facility)
	Order by [Name]
	FOR JSON PATH,ROOT('Cities')
End
GO


