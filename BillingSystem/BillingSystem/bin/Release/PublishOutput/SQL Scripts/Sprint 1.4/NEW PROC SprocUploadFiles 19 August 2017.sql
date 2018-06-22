IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocUploadFiles') 
  DROP PROCEDURE SprocUploadFiles;
 
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
CREATE PROCEDURE SprocUploadFiles
(
@pDocs TypeDocumentTemplate ReadOnly,
@pWithDocs bit=1,
@pExclusions nvarchar(500)
)
AS
BEGIN
	Declare @pUserId bigint, @pCId bigint, @pFId bigint, @pPatientId bigint


	IF Exists (Select 1 From @pDocs)
		Select TOP 1 @pUserId=CreatedBy, @pCId=CorporateID, @pFId=FacilityID, @pPatientId=PatientId From @pDocs


	INSERT INTO DocumentsTemplates ([DocumentTypeID],[DocumentName],[DocumentNotes],[AssociatedID],[AssociatedType]
      ,[FileName],[FilePath],[IsTemplate],[IsRequired],[CreatedBy],[CreatedDate],[IsDeleted],[DeletedBy],[DeletedDate]
      ,[ModifiedBy],[ModifiedDate],[CorporateID],[FacilityID],[PatientID],[EncounterID],[ExternalValue1],[ExternalValue2]
      ,[ExternalValue3]
	)
	Select [DocumentTypeID],[DocumentName],[DocumentNotes],[AssociatedID],[AssociatedType]
      ,[FileName],[FilePath],[IsTemplate],[IsRequired],[CreatedBy],[CreatedDate],[IsDeleted],[DeletedBy],[DeletedDate]
      ,[ModifiedBy],[ModifiedDate],[CorporateID],[FacilityID],[PatientID],[EncounterID],[ExternalValue1],[ExternalValue2]
      ,[ExternalValue3]
	From @pDocs

	If @pWithDocs=1
		Exec SprocGetDocumentsByPatient @pFId,@pCId,@pUserId,@pPatientId,@pExclusions
END
GO
