IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CreatePatientCarePlanActivites_V1')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CreatePatientCarePlanActivites_V1
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CreatePatientCarePlanActivites_V1]    Script Date: 22-03-2018 19:07:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<SPA- BB>
-- Create date: <Feb-2016>
-- Description:	<This Proc will Schedule Activities for a Care Plan as per setup for a Given Patient and Encounter>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_CreatePatientCarePlanActivites_V1]
(
	@pPId int =1048, --- PatientID
	@pEId int =2205	 --- EncounterID
)
AS
BEGIN
		DECLARE @Facility_Id int=(select FacilityId from dbo.PatientInfo where PatientID=@pPId)
		DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))

		 
	--***********************************************************************************************************************************************************************
	-- NOTE :--->  Used the ExtValue4 as the ActivityStatus (1=Open, 3=Adminstered,  AND -- Used the ExtValue5 as the Care Plan Id 
				-->> while inserting the data in the table [PatientCareActivities]
	--***********************************************************************************************************************************************************************

Declare @EndDate Datetime

Select Top(1) @EndDate = Dateadd(DD,1,TillDate) from PatientCarePlan Where PatientID = @pPId and EncounterID = @pEId AND ExtValue5 IS NULL

; WITH SchDateTime
     AS (-- SELECT @StartDate AS DateTimes   --- PCP.PatientID, PCP.EncounterID,
		 Select  CPT.TaskNumber,CPT.TaskName,CPT.TaskDescription,CPT.ActivityType,CPT.ResponsibleUserType,(PCP.FromDate + CPT.StartTime) AS FromDate,
		 DATEDIFF(MINUTE,Cast(CPT.StartTime as Time),Cast(CPT.EndTime as Time)) AS TimeFrame ,CPT.FacilityID, CPT.CorporateID,NULL AS AdministrativeOn,PCP.CreatedBy,@LocalDateTime AS CreatedDate,
		 NULL AS ModifiedBy,NULL AS ModeifiedDate,PCP.PatientID, PCP.EncounterID,NULL AS EV1,NULL AS EV2,NULL AS EV3,1 AS EV4,CPT.CarePlanID AS EV5,PCP.TaskID,
		 Dateadd(DD,1,PCP.TillDate) AS TillDate, CPT.IsRecurring,CPT.RecTimeInterval,CPT.RecTImeIntervalType
		 from PatientCarePlan PCP inner join [dbo].[CarePlanTask] CPT on CPT.ID = PCP.TaskID
		 Where PCP.PatientID = @pPId and PCP.EncounterID = @pEId and ExtValue5 is null
         UNION ALL
         SELECT TaskNumber,TaskName,TaskDescription,ActivityType,ResponsibleUserType,
				CASE When RecTImeIntervalType = 1 Then  Dateadd(Minute,Cast(isnull(RecTimeInterval,0) as int), FromDate)
					 When RecTImeIntervalType = 2 Then  Dateadd(HH,Cast(isnull(RecTimeInterval,0) as int), FromDate) 
					 When RecTImeIntervalType = 3 Then  Dateadd(DD,Cast(isnull(RecTimeInterval,0) as int), FromDate) 
					 When RecTImeIntervalType = 4 Then  Dateadd(WK,Cast(isnull(RecTimeInterval,0) as int), FromDate)
					 When RecTImeIntervalType = 5 Then  Dateadd(MM,Cast(isnull(RecTimeInterval,0) as int), FromDate)
					 When RecTImeIntervalType = 6 Then  Dateadd(YY,Cast(isnull(RecTimeInterval,0) as int), FromDate) 
			    END AS FromDate,TimeFrame,FacilityID, CorporateID,AdministrativeOn,CreatedBy,CreatedDate,ModifiedBy,ModeifiedDate,PatientID, EncounterID,
				EV1,EV2,EV3,EV4,EV5,TaskID,	TillDate,IsRecurring,RecTimeInterval,RecTImeIntervalType
         FROM   SchDateTime  WHERE  FromDate < TillDate)


		---->>>>>>>>>>>>>>>> Inserting/Scheduling into the Patient care Activites from Results --- STARTS
	Insert  into [PatientCareActivities]
		SELECT TaskNumber,TaskName,TaskDescription,ActivityType,ResponsibleUserType,FromDate,Dateadd(Minute,TimeFrame,FromDate) TillDate ,FacilityID, CorporateID,AdministrativeOn,CreatedBy,CreatedDate,
				ModifiedBy,ModeifiedDate,PatientID, EncounterID,EV1,EV2,EV3,EV4,EV5
		 FROM   SchDateTime Where FromDate <= @EndDate and FromDate is not NULL Order by TaskID, FromDate OPTION (MAXRECURSION 0)

		---->>>>>>>>>>>>>>>> Inserting/Scheduling into the Patient care Activites from Results --- ENDS


		--- MARKING/Update the patient care plan to Set externalValue5 = '1'	-->> To specify that its Activites has been created.
			Update PatientCarePlan Set ExtValue5 = '1' Where PatientID = @pPId and EncounterID = @pEId and ExtValue5 is NULL and IsActive = 1

		----- Delete from [PatientCareActivities] Where PatientID = 1048 and EncounterID = 2205 and CreatedDate > '2016-02-04'
	
END





GO


