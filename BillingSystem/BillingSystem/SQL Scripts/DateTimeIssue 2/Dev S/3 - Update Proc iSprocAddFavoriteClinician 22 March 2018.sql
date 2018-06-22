IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'iSprocAddFavoriteClinician')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE iSprocAddFavoriteClinician
GO

/****** Object:  StoredProcedure [dbo].[iSprocAddFavoriteClinician]    Script Date: 22-03-2018 15:39:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[iSprocAddFavoriteClinician]
(
@pPatientId bigint,
@pClinicianId bigint
)
As
Begin
	Declare @Id bigint
	Declare @CurrentDate datetime=(Select dbo.GetCurrentDatetimeByEntity(0))

	Select @Id=Id From FavoriteClinician
	Where PatientId=@pPatientId AND ClinicianId=@pClinicianId AND IsActive=1

	IF @Id > 0
		Update FavoriteClinician Set ModifiedBy=@pPatientId, ModifiedDate=@CurrentDate
		Where PatientId=@pPatientId AND ClinicianId=@pClinicianId
	ELSE
	Begin
		INSERT INTO FavoriteClinician (ClinicianId,PatientId,CreatedBy,CreatedDate,IsActive)
		Select @pClinicianId,@pPatientId,@pPatientId,@CurrentDate,1

		SET @Id=Scope_identity()
	End

	Select @Id
End
GO


