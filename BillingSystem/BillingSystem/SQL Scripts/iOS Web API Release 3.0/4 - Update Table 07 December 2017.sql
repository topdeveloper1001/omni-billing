USE [OmniStagingDB]
GO
/****** Object:  StoredProcedure [dbo].[iSprocAddFavoriteClinician]    Script Date: 07-12-2017 23:22:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[iSprocAddFavoriteClinician]
(
@pPatientId bigint,
@pClinicianId bigint
)
As
Begin
	Declare @Id bigint

	Select @Id=Id From FavoriteClinician 
	Where PatientId=@pPatientId AND ClinicianId=@pClinicianId AND IsActive=1

	IF @Id > 0
		Update FavoriteClinician Set ModifiedBy=@pPatientId, ModifiedDate=GETDATE()
		Where PatientId=@pPatientId AND ClinicianId=@pClinicianId
	ELSE
	Begin
		INSERT INTO FavoriteClinician (ClinicianId,PatientId,CreatedBy,CreatedDate,IsActive)
		Select @pClinicianId,@pPatientId,@pPatientId,GETDATE(),1

		SET @Id=Scope_identity()
	End

	Select @Id
End