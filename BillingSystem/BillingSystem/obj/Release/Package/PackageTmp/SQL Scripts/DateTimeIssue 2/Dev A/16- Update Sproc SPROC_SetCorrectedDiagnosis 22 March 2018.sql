IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_SetCorrectedDiagnosis')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_SetCorrectedDiagnosis

/****** Object:  StoredProcedure [dbo].[SPROC_SetCorrectedDiagnosis]    Script Date: 3/22/2018 7:34:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_SetCorrectedDiagnosis] 
(  
@pCorporateID int,@FacilityID int,@PatientID int, @EncounterID int,@CreatedBy int, @pDiagnosisCode nvarchar(100)
)
AS  
BEGIN

		Declare @LocalDateTime datetime  = (Select dbo.GetCurrentDatetimeByEntity(@FacilityID))
	Declare @DCID int, @DCDescription nvarchar(1000),@DRGCOdeID int, @RetStatus int = 0


	Select @DCID=DiagnosisTableNumberId,@DCDescription=DiagnosisFullDescription from DiagnosisCode Where DiagnosisCode1 = @pDiagnosisCode


	----Select * from Diagnosis
	----Select * from DiagnosisCode
	-- Select @pCorporateID,@FacilityID,@PatientID,@EncounterID,@DCID,@pDiagnosisCode,@DCDescription,@CreatedBy,@CreatedBy,@CreatedBy,getdate(),0
 If Exists ( Select 1 from DiagnosisCode where DiagnosisCode1 = @pDiagnosisCode)
 Begin
	If EXISTS (Select 1 from Diagnosis Where PatientID = @PatientID and EncounterID = @EncounterID and DiagnosisCode = @pDiagnosisCode)
	Begin
		Set @RetStatus = 98
	End
	ELSE
	Begin 

			insert into Diagnosis (CorporateID,FacilityID,PatientID,EncounterID,DiagnosisCodeId,DiagnosisType,DiagnosisCode,DiagnosisCodeDescription,
						InitiallyEnteredByPhysicianId,ReviewedByCoderID,CreatedBy,CreatedDate,Isdeleted)
			Select @pCorporateID,@FacilityID,@PatientID,@EncounterID,@DCID,2,@pDiagnosisCode,@DCDescription,@CreatedBy,@CreatedBy,@CreatedBy,@LocalDateTime,0
	End
 End
 ELSE
	Begin
		Set @RetStatus = 99
	End

	Select @RetStatus 'RetStatus'

END











GO


