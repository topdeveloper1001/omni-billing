IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'iSprocGetPatientsByUserId')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE iSprocGetPatientsByUserId


GO
/****** Object:  StoredProcedure [dbo].[iSprocGetPatientsByUserId]    Script Date: 20-12-2017 22:58:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Exec [iSprocGetPatientsByUserId] 22
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[iSprocGetPatientsByUserId]
(
@pUserId bigint=null
)
As
Begin
	Declare @FacilityId int=0


	SET @pUserId=ISNULL(@pUserId,0)
	
	IF @pUserId>0
		Select @FacilityId=FacilityId From [Users] Where UserID=@pUserId

	Select PatientID,PersonFirstName as FirstName,PersonLastName As LastName
	,(PersonFirstName + ' ' + PersonLastName) As PatientName
	From PatientInfo 
	Where (@FacilityId=0 OR FacilityId=@FacilityId)
	And ISNULL(IsDeleted,0)=0
	For Json Path, Root('PatientSearchResults')
End