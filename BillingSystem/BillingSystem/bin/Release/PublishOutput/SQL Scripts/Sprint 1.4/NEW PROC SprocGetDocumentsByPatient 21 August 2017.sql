IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocGetDocumentsByPatient') 
  DROP PROCEDURE SprocGetDocumentsByPatient;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE SprocGetDocumentsByPatient
(
@pFId bigint=null,
@pCId bigint=null,
@pUserId nvarchar(100)=null,
@pPId bigint=null,
@pExclusions nvarchar(500)=null
)
As
BEGIN
	Declare 
	--@AssociatedTypeId nvarchar(10)=(Select TOP 1 GlobalCodeValue From GlobalCodes Where GlobalCodeCategoryValue='80442' And GlobalCodeName='Authorization')
	@PatientName nvarchar(200)
	,@FacilityName nvarchar(200)=(Select TOP 1 FacilityName From Facility Where FacilityId=@pFId)
	--,@CorporateName nvarchar(200)=(Select TOP 1 CorporateName From Corporate Where CorporateID=@pFId)
	
	Select D.[DocumentsTemplatesID],D.[DocumentTypeID],D.[DocumentName],D.[DocumentNotes],D.[AssociatedID],D.[AssociatedType]
	,left([FileName], charindex('.', [FileName] + '.')-1) As [FileName]
	,D.[FilePath],D.[IsTemplate],D.[IsRequired],D.[CreatedBy],D.[CreatedDate],D.[IsDeleted],D.[DeletedBy],D.[DeletedDate],D.[ModifiedBy]
	,D.[ModifiedDate],D.[CorporateID],D.[FacilityID],D.[PatientID],D.[EncounterID]
	,(Select TOP 1 EncounterNumber From Encounter Where EncounterID=D.EncounterID) As [ExternalValue1]
	,(Select TOP 1 (PersonFirstName + ' ' + PersonLastName) As PatientName From PatientInfo Where PatientID=D.PatientID) As [ExternalValue2]
	,(Select TOP 1 FacilityName From Facility Where FacilityId=D.FacilityID) As [ExternalValue3]
	From DocumentsTemplates D
	Where (ISNULL(@pPId,0)=0 OR D.PatientID=@pPId) And
	(ISNULL(@pExclusions,'')='' OR D.DocumentName NOT IN (@pExclusions))
	Order by D.CreatedDate Desc
END
GO
