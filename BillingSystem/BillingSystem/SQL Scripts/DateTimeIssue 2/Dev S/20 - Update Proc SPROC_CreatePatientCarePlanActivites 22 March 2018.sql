IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CreatePatientCarePlanActivites')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CreatePatientCarePlanActivites
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CreatePatientCarePlanActivites]    Script Date: 22-03-2018 19:05:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_CreatePatientCarePlanActivites]
(
	@pFId int =4, 
	@pCId int =6
)
AS
BEGIN
		DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFId))

	Declare @ActivitesTemp Table ([TaskNumber] [nvarchar](50) NULL,[TaskName] [nvarchar](50) NULL,[Description] [nvarchar](500) NULL,[ActivityType] [nvarchar](50) NULL,
	[UserType] [nvarchar](50) NULL,[StartTime] [nvarchar](50) NULL,[EndTime] [nvarchar](50) NULL,[FacilityId] [int] NULL,[CorporateId] [int] NULL,[AdministrativeOn] [datetime] NULL,
	[CreatedBy] [int] NULL,[CreatedDate] [datetime] NULL,[ModifiedBy] [int] NULL,[ModeifiedDate] [datetime] NULL,[PatientId] [int] NULL,[EncounterId] [int] NULL,
	[ExtValue1] [nvarchar](50) NULL,[ExtValue2] [nvarchar](50) NULL,[ExtValue3] [nvarchar](50) NULL,[ExtValue4] [nvarchar](50) NULL,[ExtValue5] [nvarchar](50) NULL)

Declare @pPId int, @pEncId int

Declare @Cur_Id int,@Cur_CareplanId int, @Cur_TaskId int, @Cur_PatientId int, @Cur_EncounterId int, @Cur_FromDate datetime,@Cur_TillDate datetime,@IsRecuuring bit

Declare @TaskFrequency nvarchar(10),@TaskStartTime nvarchar(10),@TaskEndTime nvarchar(10),@TaskDesc nvarchar(500),@TaskName nvarchar(100),@TaskActivityType nvarchar(10),
@AssigendUserType int

--***********************************************************************************************************************************************************************
-- Used the ExtValue4 as the ActivityStatus
-- Used the ExtValue5 as the Care Plan Id 
-- while inserting the data in the table [PatientCareActivities]
--***********************************************************************************************************************************************************************
--Select * from PatientCarePlan
--where ExtValue5 is null and FacilityId = @pFId and CorporateId = @pCId and IsActive =1

Declare @DateDiffernece int =0,@ActivitySDate datetime,@ActivityEDate datetime;

Declare CurPatientCarePlanX cursor for 
select Id,CareplanId,TaskId,PatientId,EncounterId,FromDate,TillDate from PatientCarePlan
where ExtValue5 is null and FacilityId = @pFId and CorporateId = @pCId and IsActive =1

Open CurPatientCarePlanX

Fetch Next from CurPatientCarePlanX
into @Cur_Id,@Cur_CareplanId,@Cur_TaskId,@Cur_PatientId,@Cur_EncounterId,@Cur_FromDate,@Cur_TillDate


While @@FETCH_STATUS = 0
BEGIN
	
--Select *   from CarePlanTask CPT  where Id = @Cur_TaskId

 Select @IsRecuuring = CPT.IsRecurring,@TaskFrequency = CPT.RecurranceType,@TaskStartTime=CPT.StartTime,@TaskEndTime=CPT.EndTime,@TaskDesc = CPT.TaskDescription,@TaskName =CPT.TaskName,
 @TaskActivityType = CPT.ActivityType,@AssigendUserType =CPT.ResponsibleUserType
  from CarePlanTask CPT
 where Id = @Cur_TaskId

 --- Delete Previous Exsiting Data from activities for Selected encounter and Selected Patient
 If Exists (Select 1 from [PatientCareActivities] where [TaskNumber] = @Cur_TaskId and [PatientId] =@Cur_PatientId and [EncounterId] = @Cur_EncounterId and StartTime > @LocalDateTime)
	Delete From [PatientCareActivities] where [TaskNumber] = @Cur_TaskId and [PatientId] =@Cur_PatientId and [EncounterId] = @Cur_EncounterId and StartTime > @LocalDateTime

 Set @DateDiffernece = Datediff(dd,@Cur_FromDate,@Cur_TillDate);
 Declare @DateCounter int =0

 While @DateDiffernece >= @DateCounter
 BEGIN
	If @IsRecuuring = 1
	BEGIN
		If(@TaskFrequency = 1)--- Case for the "Once a Day"
		BEGIN
			SET @ActivitySDate = DateAdd(DD,@DateCounter,@Cur_FromDate)
			Set @ActivitySDate = CONVERT(DATETIME, CONVERT(CHAR(8), @ActivitySDate, 112)   + ' ' + CONVERT(CHAR(8), @TaskStartTime, 108))
			Set @ActivityEDate = CONVERT(DATETIME, CONVERT(CHAR(8), @ActivitySDate, 112)   + ' ' + CONVERT(CHAR(8), @TaskEndTime, 108))
			INSERT INTO @ActivitesTemp
		       ([TaskNumber],[TaskName],[Description],[ActivityType],[UserType],[StartTime],[EndTime],[FacilityId],[CorporateId],[AdministrativeOn],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModeifiedDate],[PatientId],[EncounterId],[ExtValue1],[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5])
			   Values 
			   (@Cur_TaskId,@TaskName,@TaskDesc,@TaskActivityType,@AssigendUserType,@ActivitySDate,@ActivityEDate,@pFId,@pCId,Null,1001,@LocalDateTime,NUll,NUll,@Cur_PatientId,@Cur_EncounterId,NUll,NUll
			   ,NUll,'1',@Cur_CareplanId)
		Set @DateCounter += 1;
		END
		ELSE If(@TaskFrequency = 2)--- Case for the "Multiple times in day"
		BEGIN
			--- Case is Empty in this case we will include one more check for much time and Frequency for occruance.
			SET @ActivitySDate = DateAdd(DD,@DateCounter,@Cur_FromDate)
			Set @ActivitySDate = CONVERT(DATETIME, CONVERT(CHAR(8), @ActivitySDate, 112)   + ' ' + CONVERT(CHAR(8), @TaskStartTime, 108))
			Set @ActivityEDate = CONVERT(DATETIME, CONVERT(CHAR(8), @ActivitySDate, 112)   + ' ' + CONVERT(CHAR(8), @TaskEndTime, 108))
			Set @DateCounter += 1;
		END
		ELSE If(@TaskFrequency = 3) --- Case for the "Once a week"
		BEGIN
			SET @ActivitySDate = DateAdd(WK,@DateCounter,@Cur_FromDate)
			Set @ActivitySDate = CONVERT(DATETIME, CONVERT(CHAR(8), @ActivitySDate, 112)   + ' ' + CONVERT(CHAR(8), @TaskStartTime, 108))
			Set @ActivityEDate = CONVERT(DATETIME, CONVERT(CHAR(8), @ActivitySDate, 112)   + ' ' + CONVERT(CHAR(8), @TaskEndTime, 108))
			INSERT INTO @ActivitesTemp
		       ([TaskNumber],[TaskName],[Description],[ActivityType],[UserType],[StartTime],[EndTime],[FacilityId],[CorporateId],[AdministrativeOn],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModeifiedDate],[PatientId],[EncounterId],[ExtValue1],[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5])
			   Values 
			   (@Cur_TaskId,@TaskName,@TaskDesc,@TaskActivityType,@AssigendUserType,@ActivitySDate,@ActivityEDate,@pFId,@pCId,Null,1001,@LocalDateTime,NUll,NUll,@Cur_PatientId,@Cur_EncounterId,NUll,NUll
			   ,NUll,'1',@Cur_CareplanId)
		Set @DateCounter += 1;
		END
		ELSE If(@TaskFrequency = 4) --- Case for the "Every other day"
		BEGIN
			SET @ActivitySDate = DateAdd(DD,@DateCounter,@Cur_FromDate)
			Set @ActivitySDate = CONVERT(DATETIME, CONVERT(CHAR(8), @ActivitySDate, 112)   + ' ' + CONVERT(CHAR(8), @TaskStartTime, 108))
			Set @ActivityEDate = CONVERT(DATETIME, CONVERT(CHAR(8), @ActivitySDate, 112)   + ' ' + CONVERT(CHAR(8), @TaskEndTime, 108))
			INSERT INTO @ActivitesTemp
		       ([TaskNumber],[TaskName],[Description],[ActivityType],[UserType],[StartTime],[EndTime],[FacilityId],[CorporateId],[AdministrativeOn],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModeifiedDate],[PatientId],[EncounterId],[ExtValue1],[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5])
			   Values 
			   (@Cur_TaskId,@TaskName,@TaskDesc,@TaskActivityType,@AssigendUserType,@ActivitySDate,@ActivityEDate,@pFId,@pCId,Null,1001,@LocalDateTime,NUll,NUll,@Cur_PatientId,@Cur_EncounterId,NUll,NUll
			   ,NUll,'1',@Cur_CareplanId)
		Set @DateCounter += 2;
		END
		ELSE
		BEGIN
			Set @DateCounter += 1;
		END
	END
	ELSE
	BEGIN
		    SET @ActivitySDate = DateAdd(DD,@DateCounter,@Cur_FromDate)
			Set @ActivitySDate = CONVERT(DATETIME, CONVERT(CHAR(8), @ActivitySDate, 112)   + ' ' + CONVERT(CHAR(8), @TaskStartTime, 108))
			Set @ActivityEDate = CONVERT(DATETIME, CONVERT(CHAR(8), @ActivitySDate, 112)   + ' ' + CONVERT(CHAR(8), @TaskEndTime, 108))
			INSERT INTO @ActivitesTemp
		       ([TaskNumber],[TaskName],[Description],[ActivityType],[UserType],[StartTime],[EndTime],[FacilityId],[CorporateId],[AdministrativeOn],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModeifiedDate],[PatientId],[EncounterId],[ExtValue1],[ExtValue2],[ExtValue3],[ExtValue4],[ExtValue5])
			   Values 
			   (@Cur_TaskId,@TaskName,@TaskDesc,@TaskActivityType,@AssigendUserType,@ActivitySDate,@ActivityEDate,@pFId,@pCId,Null,1001,@LocalDateTime,NUll,NUll,@Cur_PatientId,@Cur_EncounterId,NUll,NUll
			   ,NUll,'1',@Cur_CareplanId)
		Set @DateCounter += 1;
	END
 END
--- Update the patient cre plan to Set externalValue5 = '1'	 To specify that its Activites has been created.
Update PatientCarePlan Set ExtValue5 = '1' where Id =@Cur_Id and FacilityId = @pFId and CorporateId = @pCId and IsActive =1

Fetch Next from CurPatientCarePlanX
into @Cur_Id,@Cur_CareplanId,@Cur_TaskId,@Cur_PatientId,@Cur_EncounterId,@Cur_FromDate,@Cur_TillDate
END

Close CurPatientCarePlanX
Deallocate CurPatientCarePlanX;

---- Insert the Patient care Activites from the Temp table
Insert  into [PatientCareActivities]
Select * from @ActivitesTemp Where Cast(StartTime as Datetime) > @LocalDateTime

Delete From @ActivitesTemp
--Select * from @ActivitesTemp
END





GO


