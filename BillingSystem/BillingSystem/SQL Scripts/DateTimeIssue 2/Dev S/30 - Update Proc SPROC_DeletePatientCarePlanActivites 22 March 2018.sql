IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_DeletePatientCarePlanActivites')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_DeletePatientCarePlanActivites
GO

/****** Object:  StoredProcedure [dbo].[SPROC_DeletePatientCarePlanActivites]    Script Date: 22-03-2018 19:37:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_DeletePatientCarePlanActivites]
(
@pPId int =3224,
@pEId int =2199,
@pId int =11,
@pTaskNumber nvarchar(10) ='12'
)
AS
BEGIN
	DECLARE @Facility_Id int=(select EncounterFacility from dbo.Encounter where EncounterID=@pEId)

	Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))

-- SET NOCOUNT ON added to prevent extra result sets from
Declare @Temo Table (ID int)
Declare @TaskNumberVal nvarchar(10) = (Select Tasknumber from CarePlanTask where Id = @pTaskNumber )


Insert into @Temo
Select Distinct PCA.Id from PatientCareActivities PCA
INNER JOIN PatientCarePlan PCP on PCP.CarePlanId = PCA.ExtValue5 
where PCA.PatientId = @pPId and  PCA.EncounterId = @pEId --and PCA.ExtValue5 = @pId
and PCA.TaskNumber = @TaskNumberVal and PCA.ExtValue4 = '1'
 --CAst(PCA.StartTime as Datetime) > @LocalDateTime



update PatientCareActivities set ExtValue4 = 9,ModifiedBy=9001,ModeifiedDate=@LocalDateTime where Id in (Select ID from @Temo)

Delete from PatientCareActivities Where Id in (Select ID from @Temo)

Update PatientCarePlan Set ISActive = 0 ,ModifiedBy = 9001,ModifiedDate = @LocalDateTime  where Id= @pId

END





GO


