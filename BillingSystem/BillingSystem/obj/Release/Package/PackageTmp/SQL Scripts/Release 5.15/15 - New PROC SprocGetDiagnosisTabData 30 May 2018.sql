IF OBJECT_ID('SprocGetDiagnosisTabData','P') IS NOT NULL
   DROP PROCEDURE SprocGetDiagnosisTabData
GO

/****** Object:  StoredProcedure [dbo].[SprocGetDiagnosisTabData]    Script Date: 5/30/2018 8:37:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetDiagnosisTabData]  
(
	@PId bigint,
	@EId bigint=0,
	@PhysicianId bigint=0,
	@DiagnosisTN nvarchar(10)='',
	@DrgTN nvarchar(10)=''
)
AS
BEGIN
	Declare @EncounterNumber nvarchar(100)=''

	--Getting the Encounter Number from the table ENCOUNTER, STARTs here
	If @EId = 0
		Select TOP 1 @EId= EncounterID, @EncounterNumber = EncounterNumber From Encounter Where PatientID=@PId 
		Order by EncounterEndTime Desc


	If ISNULL(@EncounterNumber,'') = ''
		Select @EncounterNumber = EncounterNumber From Encounter Where EncounterID = @EId

	--Getting the Encounter Number from the table ENCOUNTER, ENDs here
	

	/* ########-----------GETTING the Current Diagnosis LIST-------------######### */
	EXEC SprocGetCurrentDiagnosisData @PId,@EId,@EncounterNumber,@DrgTN
	/* ########-----------GETTING the Current Diagnosis LIST-------------######### */


	/* ########-----------GETTING the PREVIOUS Diagnosis LIST-------------######### */
	Exec SprocGetPreviousDiagnosisData @EId,@PId,@DrgTN,@DiagnosisTN,@EncounterNumber
	/* ########-----------GETTING the PREVIOUS Diagnosis LIST-------------######### */

	--Get Favorite Diagnosis Data of the Current Physician
	Exec SprocGetFavoriteDiagnosisData @PhysicianId,@DiagnosisTN,@DrgTN
END
GO


