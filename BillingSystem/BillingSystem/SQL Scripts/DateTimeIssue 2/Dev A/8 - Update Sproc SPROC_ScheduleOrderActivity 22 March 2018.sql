IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ScheduleOrderActivity')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ScheduleOrderActivity

/****** Object:  StoredProcedure [dbo].[SPROC_ScheduleOrderActivity]    Script Date: 3/22/2018 7:10:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_ScheduleOrderActivity]
AS
BEGIN

BEGIN TRY

SET NOCOUNT ON
SET XACT_ABORT ON

--  Code Which Doesn't Require Transaction

BEGIN TRANSACTION
	
DECLARE @tmpActivityOpenOrderSteps TABLE
(
tmpActivityOpenOrderStepsID INT IDENTITY(1,1)
,ActvityTime TIME
,ActiviyDayTime DATETIME
,OpenOrderID INT
,OrderCodeSchedule nvarchar(50)
)

DECLARE @OpenOrderID INT, @FrequencyCode INT,@PeriodDays INT,@PrescribeDate DATETIME,@IndexDays INT,@DaysTimeIndex INT,@DaysTimeLoopCount INT,
@PrescribeDateForActivity DATETIME,@StartDate DATETIME,@EndDate DATETIME
,@CurrentDate DATETIME=(Select dbo.GetCurrentDatetimeByEntity(0))
,@Cur_OpenOrderCode nvarchar(50),
@Cur_GlobalCodeValue Nvarchar(50), @Cur_ExternalValue1 Nvarchar(50), @Cur_ExternalValue2 Nvarchar(50),@FirstTimeFlag bit = 1


--- Changes done BY Shashank As of 3rd march 2016 TO Craete the Orderactivities for the Orders from XML upload Or Upload charges manually.
Declare @CUR_ForceCreateActvities nvarchar(100)


DECLARE Cursor_PeriodDays CURSOR FOR
SELECT OpenOrderID,FrequencyCode,Perioddays,OpenOrderPrescribedDate,StartDate,EndDate,OrderCode,EV1
FROM OpenOrder WHERE ISNULL(IsActive,0) = 1 AND ISNULL(IsActivitySchecduled,0)=0 AND ISNULL(IsApproved,0) = 1



OPEN Cursor_PeriodDays
FETCH NEXT FROM Cursor_PeriodDays INTO @OpenOrderID,@FrequencyCode,@PeriodDays,@PrescribeDate,@StartDate,@EndDate,@Cur_OpenOrderCode,@CUR_ForceCreateActvities


WHILE @@FETCH_STATUS = 0  
BEGIN  
	----- BB- 4-Feb-2015 New Change Fixing StartDate Time as now due ot Person not able to Administer immediately (Not from Mid Night) - STARTS

		Set @FirstTimeFlag = 1
		Set @CUR_ForceCreateActvities  = ISNULL(@CUR_ForceCreateActvities,'0')
		If @CUR_ForceCreateActvities = '0'
			Set @StartDate = (Select	CONVERT(DATETIME, CONVERT(CHAR(8), @StartDate, 112)   + ' ' + CONVERT(CHAR(8), @CurrentDate, 108)))

	----- BB- 4-Feb-2015 New Change Fixing StartDate Time as now due ot Person not able to Administer immediately (Not from Mid Night) - ENDS

	SET @IndexDays = 0;
	SET @DaysTimeIndex = 0;
	SET @DaysTimeLoopCount = 0;

	--If EXISTS (Select Top(1) GlobalCodeID from GlobalCodes Where GlobalCodeCategoryValue = @Cur_OpenOrderCode)
	If EXISTS (Select Top(1) ID from LabTestOrderSet Where OrderSetValue = @Cur_OpenOrderCode)
	 BEGIN
		---- >>>>> Scheduling Lab Orders which have ORDER SET (Multiple Orders to be given from One Order) - STARTS
						--- >>>>>>>>  BUILD up Row Wise Table from Column Wise ------ STARTS
		Declare @TID int,@OrderSetValue nvarchar(20), @OrderSetDescription nvarchar(100),@Code1 nvarchar(20),@Time1 nvarchar(20),@Table1 nvarchar(50),@Counter int = 1,@Q1 nvarchar(200),
				@Column1 nvarchar(20), @Column2 nvarchar(20);
		Declare  @Temp1 table (OrderSetValue nvarchar(20),Code1 nvarchar(20),Time1 nvarchar(20))
		
		Declare OrderSet Cursor For Select ID,OrderSetValue from LabTestOrderSet Where OrderSetValue = @Cur_OpenOrderCode;
		
		Open OrderSet;
					
		Fetch next from OrderSet into @TID,@OrderSetValue;
		While @@FETCH_STATUS = 0
		BEGIN
			Set @Counter = 1
			While @Counter <= 15
			Begin
							Set @Column1 = 'CodeValue'+Cast(@Counter as nvarchar(2))
							Set @Column2 = 'CodeTime'+Cast(@Counter as nvarchar(2))
					
						Set @Q1 = 'Select @Code1 ='+ @Column1 +',@Time1 ='+ @Column2+' From LabTestOrderSet Where ID =' + Cast(@TID as nvarchar(10))
					
						Exec sp_executesql @Q1, N'@Code1 nvarchar(50) output,@Time1 nvarchar(50) output', @Code1=@Code1 Output,@Time1=@Time1 output

						If @Time1 <> 0
						Insert into @Temp1 Select @Code1,@Time1,1
					
						Set @Counter = @Counter+1

				End
			
				Fetch next from OrderSet into @TID,@OrderSetValue;
			
				END
				Close OrderSet;
				Deallocate OrderSet;

						--- >>>>>>>>  BUILD up Row Wise Table from Column Wise ------ STARTS
						DECLARE Cur_OrderSet CURSOR FOR Select * from @Temp1;
						OPEN Cur_OrderSet;
						Fetch Next from Cur_OrderSet INTO @Cur_GlobalCodeValue,@Cur_ExternalValue1, @Cur_ExternalValue2 ;

						WHILE @@FETCH_STATUS = 0 
						Begin
							--- Convert Date and time as per Set up
							--Set @IndexDays = Cast(@Cur_ExternalValue1 as Int)
							Set @IndexDays = Case WHEN Cast(@Cur_ExternalValue1 as Int) = 33 THEN 0 ELSE Cast(@Cur_ExternalValue1 as Int) END
							Set @DaysTimeIndex = Cast(@Cur_ExternalValue2 as Int)
						----- BB- 4-Feb-2015 New Change Fixing StartDate Time as now due to Person not able to Administer immediately (Not from Mid Night) - STARTS
						  If @FirstTimeFlag = 1
						  Begin
							--PRINT '@FirstTimeFlag Case RUNS'
							SET @PrescribeDateForActivity = DATEADD(HOUR, -1, @StartDate)			--@StartDate
							Set @FirstTimeFlag = 0
						  End
						  ELSE
						  Begin
							If @DaysTimeIndex = 1 
							Begin
								SET @PrescribeDateForActivity = DATEADD(HOUR,@IndexDays, @PrescribeDateForActivity);
							End
							Else
							Begin
								--PRINT '@DaysTimeIndex != 1 Case RUNS'
								SET @PrescribeDateForActivity = DATEADD(DAY,@IndexDays, @PrescribeDateForActivity);
								SET @PrescribeDateForActivity = DATEADD(DAY,@IndexDays, DATEADD(HOUR, -1, @PrescribeDateForActivity));
							End
						  End
							--PRINT 'ScheduleDate TO BE INSERTED INTO TEMP TABLE: '
							--PRINT @PrescribeDateForActivity
							----- BB- 4-Feb-2015 New Change Fixing StartDate Time as now due to Person not able to Administer immediately (Not from Mid Night) - ENDS
							--- Insert Below
								Insert into @tmpActivityOpenOrderSteps (ActiviyDayTime,OpenOrderID,OrderCodeSchedule)
									Select 	@PrescribeDateForActivity,@OpenOrderID,@Cur_GlobalCodeValue	


							Fetch Next from Cur_OrderSet INTO @Cur_GlobalCodeValue,@Cur_ExternalValue1, @Cur_ExternalValue2 ;
						End 
						CLOSE Cur_OrderSet;  
						DEALLOCATE Cur_OrderSet;


					 END ---- >>>>> Scheduling Lab Orders which have ORDER SET (Multiple Orders to be given from One Order) - ENDS
			ELSE
		BEGIN
		--------XXXXXXXXXXXXXXXXXXX>>>>> Loop for Making Scheduled time based on Frequency selected by User and NOT for Lab Orders which have ORDER SET - STARTS
		--print 'Section check;'
		--while loop for add time in activity
		SELECT @DaysTimeLoopCount = COUNT(*) FROM  OpenOrderActivityTime WHERE globalcodeValue = @FrequencyCode
		----- New Logic to take care of Start Date and End Date to Schedule Activities
		---- Set Period of Days
		Set @PeriodDays = DATEDIFF(DAY, @StartDate, @EndDate) + 1

		WHILE (@IndexDays < @PeriodDays)
		BEGIN
		  ------WHILE (@DaysTimeIndex < @DaysTimeLoopCount)
		   ------BEGIN
			   ---------->  SET @PrescribeDateForActivity = DATEADD( DAY,@DaysTimeIndex, @PrescribeDate);

		----- BB- 4-Feb-2015 New Change (ONLY for NA GlobalCCode=10) Fixing StartDate Time as now due ot Person not able to Administer immediately (Not from Mid Night) - STARTS
				
				If @FrequencyCode = '10' Or @FrequencyCode = '23'
				Begin  ---- Only one time which Selected Frequency is N/A = FrequencyCode = 10

						If @FirstTimeFlag = 1
						  Begin
						  ---- BB (11-Feb-2016) - Following is made to be MidNight Time in Case of NA selceted as per Client 
						  ------ >>>>> Issue can be that if Encounter is opened on same date (say Noon) then Mid Night will make Claim Reject as Activity Ordered cannot be less Encounter Time?
						  ------ >>>>>>>>>>>>>  Made aware of this to Client but he still wants this to be Moved to Mid Night - So it can show properly on MAR view

							SET @PrescribeDateForActivity = Cast(@StartDate as Date);
							--- Check is added so that the time for order is considered as the time for the activity to open
							If @CUR_ForceCreateActvities = '0'
							BEGIN
								--PRINT '---------SPECIAL CASE 1-----------------------'
								--PRINT 'ScheduleDate TO BE INSERTED INTO TEMP TABLE: '
								Declare @TimeForNA time = Cast(@CurrentDate as Time);
								SET @PrescribeDateForActivity = @PrescribeDateForActivity + @TimeForNA
								--Select @PrescribeDateForActivity 
								--PRINT '-----------SPECIAL CASE 1---------------------'
							END
							ELSE
							BEGIN
							--SET @PrescribeDateForActivity =Cast(@LocalTime as Date) ;
							--- BB just adding a Minute so it is always selected or shown on Proper Day on reports and views in Future
								--PRINT '-----------SPECIAL CASE 2---------------------'
								--Select @PrescribeDateForActivity 
								SET @PrescribeDateForActivity = @PrescribeDate;
							END
							--- Setting Back to UAE time
							---- BB (11-Feb-2016) Following is Not needed any more as Solution is Made to UAE now so Commenting Below
							--- SET @PrescribeDateForActivity = DATEADD(MINUTE,-90,@PrescribeDateForActivity)
							Set @FirstTimeFlag = 0
						  End
						  ELSE
						  Begin
								--Select @PrescribeDateForActivity 

							SET @PrescribeDateForActivity = Cast(@PrescribeDateForActivity as Date)
							SET @PrescribeDateForActivity = DATEADD(DAY,1, @PrescribeDateForActivity);
							SET @PrescribeDateForActivity = DATEADD(MINUTE,1, @PrescribeDateForActivity);
								--Select @PrescribeDateForActivity 

							--- Setting Back to UAE time
							---- BB (11-Feb-2016) Following is Not needed any more as Solution is Made to UAE now so Commenting Below
							--SET @PrescribeDateForActivity = DATEADD(MINUTE,-90,@PrescribeDateForActivity)
						  End
								--PRINT '-----------BEFORE Added to temp table (Case 1)---------------------'

								--Select @PrescribeDateForActivity 

								INSERT INTO @tmpActivityOpenOrderSteps
								SELECT '00:00:00',@PrescribeDateForActivity AS ActiviyDayTime,@OpenOrderID AS OpenOrderID, @Cur_OpenOrderCode 
				End
				ELSE
				Begin			   
								--PRINT '-----------BEFORE Added to temp table (Case 2)---------------------'
								--Select @PrescribeDateForActivity 
							SET @PrescribeDateForActivity = DATEADD( DAY,@IndexDays, @StartDate);
							--- Setting Back to UAE time
							---- BB (11-Feb-2016) Following is Not needed any more as Solution is Made to UAE now so Commenting Below
							-- SET @PrescribeDateForActivity = DATEADD(MINUTE,-90,@PrescribeDateForActivity)
							INSERT INTO @tmpActivityOpenOrderSteps
								SELECT TimeOfActivity AS ActvityTime, 
									DATEADD(DAY, DATEDIFF(DAY, 0, @PrescribeDateForActivity),CONVERT(VARCHAR(10), TimeOfActivity)) AS ActiviyDayTime, 
									@OpenOrderID AS OpenOrderID, @Cur_OpenOrderCode 
							FROM OpenOrderActivityTime WHERE globalcodeValue = @FrequencyCode
				End
		----- BB- 4-Feb-2015 New Change (ONLY for NA GlobalCCode=10) Fixing StartDate Time as now due ot Person not able to Administer immediately (Not from Mid Night) - ENDS
								----------SET @DaysTimeIndex = @DaysTimeIndex + 1;
								----------END-- end of WHILE (@DaysTimeIndex < @DaysTimeLoopCounT)
								---end of while loop for add time in activity

								SET @IndexDays = @IndexDays + 1;
						  END --END OF  WHILE (@IndexDays <= @PeriodDays)
					END	 
					--------XXXXXXXXXXXXXXXXXXX>>>>> Loop for Making Scheduled time based on Frequency selected by User and NOT for Lab Orders which have ORDER SET - ENDS



					FETCH NEXT FROM Cursor_PeriodDays INTO @OpenOrderID,@FrequencyCode,@PeriodDays,@PrescribeDate,@StartDate,@EndDate,@Cur_OpenOrderCode,@CUR_ForceCreateActvities
				END  --END OF @@FETCH_STATUS = 0  

				CLOSE Cursor_PeriodDays  
				DEALLOCATE Cursor_PeriodDays 

				-- Updates

				UPDATE OpenOrder SET ActivitySchecduledOn = @CurrentDate, IsActivitySchecduled = 1
				WHERE OpenOrderID IN (SELECT DISTINCT OpenOrderID FROM @tmpActivityOpenOrderSteps)AND  ISNULL(IsActive ,0)=1 AND ISNULL(IsDeleted,0)=0

				--Select * from @tmpActivityOpenOrderSteps
				--Set @CUR_ForceCreateActvities  = ISNULL(@CUR_ForceCreateActvities,'0')

				IF(@CUR_ForceCreateActvities <> '0')
				BEGIN
					----- BB 14-Mar-2016 ---- Following If EV1 = 100 logic added to Schedule Activites only For which were not executed ---- STARTS 
					--------------  Meaning if Activity was either executed/Cancelled before Changing/Updating Order those should not be re-scheduled -- 
					------------IF(@CUR_ForceCreateActvities = '100')
					------------Begin
					------------	---- This Part will Only be Executed for Changed or Updated orders
					------------	INSERT INTO OrderActivity (OrderID, OrderCode,OrderScheduleDate,PlannedDate,OrderActivityStatus,IsActive, CreatedDate, CreatedBy )
					------------	SELECT OpenOrderID,OrderCodeSchedule,ActiviyDayTime AS ActivitySchecduleOn,ActiviyDayTime AS PlanDate,1 AS ActitvityStatus, 1 AS IsActive,
					------------	@CurrentDate AS CreatedDate, 1 AS CreatedBy  FROM @tmpActivityOpenOrderSteps 
					------------	Where OrderCodeSchedule not in 
					------------	(Select distinct OrderCode from OrderActivity Where OrderID IN (SELECT DISTINCT OpenOrderID FROM @tmpActivityOpenOrderSteps))
					------------End
					------------ELSE ----- BB 14-Mar-2016 ---- Following If EV1 = 100 logic added to Schedule Activites only For which were not executed ---- ENDS
					------------Begin
						---- Insert the Schedules Here from Temp Table Created --- SA logic to handle Scheduling of Activities even in Past dates
						------- Needed for XML Uploads and Manual Charges screen where orders can be added for Previous past dates
								--Select 'Before inserting into Main Table i.e. OrderActivity (Case 1)' 
								--Select @PrescribeDateForActivity 
								SET @PrescribeDateForActivity = DATEADD(minute,-1,@PrescribeDateForActivity)
						INSERT INTO OrderActivity (OrderID, OrderCode,OrderScheduleDate,PlannedDate,OrderActivityStatus,IsActive, CreatedDate, CreatedBy )
						SELECT OpenOrderID,OrderCodeSchedule,ActiviyDayTime AS ActivitySchecduleOn,ActiviyDayTime AS PlanDate,1 AS ActitvityStatus, 1 AS IsActive,
						@CurrentDate AS CreatedDate, 1 AS CreatedBy  FROM @tmpActivityOpenOrderSteps
					------------End 
				END
				ELSE
				BEGIN
					--Select @PrescribeDateForActivity
					--Select * From @tmpActivityOpenOrderSteps
				--Select 'Before inserting into Main Table i.e. OrderActivity (Case 2)' 
								--Select @PrescribeDateForActivity 
					---- Insert the Schedules Here from Temp Table Created
					INSERT INTO OrderActivity (OrderID, OrderCode,OrderScheduleDate,PlannedDate,OrderActivityStatus,IsActive, CreatedDate, CreatedBy )
					SELECT OpenOrderID,OrderCodeSchedule,ActiviyDayTime AS ActivitySchecduleOn,ActiviyDayTime AS PlanDate,1 AS ActitvityStatus, 1 AS IsActive,@CurrentDate AS CreatedDate, 1 AS CreatedBy  
					FROM @tmpActivityOpenOrderSteps
					where ActiviyDayTime >= DATEADD(MINUTE,-1,@CurrentDate)
				END
				--Old Code
				--update OOAS	set	 OOAS.OrderType = OO.OrderType,OOAS.OrderCategoryID = OO.CategoryID,OOAS.OrderSubCategoryID = OO.SubCategoryID
				--		,OOAS.OrderActivityQuantity = OO.Quantity,OOAS.PatientID = OO.PatientID,OOAS.EncounterID = OO.EncounterID,OOAS.OrderBy = OO.PhysicianID
				--		,OOAS.PlannedBy = OO.PhysicianID,OOAS.PlannedDate = OOAS.OrderScheduleDate,OOAS.CorporateID = OO.CorporateID,OOAS.FacilityID = OO.FacilityID
				-- from OpenOrder OO
				-- inner join OrderActivity OOAS on OO.OpenOrderID = OOAS.OrderID  And (OOAS.OrderActivityStatus = 1)

				-- New code with the change in the where Clause ''''''and OOAS.OrderScheduleDate > @LocalTime'''
				-- This will onlt update the future order activites not the old activities
				-- Changes done by Shashank on 15 march 2015
				IF(@CUR_ForceCreateActvities <> '0')
				BEGIN
					update OOAS	set	 OOAS.OrderType = OO.OrderType,OOAS.OrderCategoryID = OO.CategoryID,OOAS.OrderSubCategoryID = OO.SubCategoryID
						,OOAS.OrderActivityQuantity = OO.Quantity,OOAS.PatientID = OO.PatientID,OOAS.EncounterID = OO.EncounterID,OOAS.OrderBy = OO.PhysicianID
						,OOAS.PlannedBy = OO.PhysicianID,OOAS.PlannedDate = OOAS.OrderScheduleDate,OOAS.CorporateID = OO.CorporateID,OOAS.FacilityID = OO.FacilityID
					from OpenOrder OO
					inner join OrderActivity OOAS on OO.OpenOrderID = OOAS.OrderID  And (OOAS.OrderActivityStatus = 1) 
				END
				ELSE
				BEGIN
					update OOAS	set	 OOAS.OrderType = OO.OrderType,OOAS.OrderCategoryID = OO.CategoryID,OOAS.OrderSubCategoryID = OO.SubCategoryID
						,OOAS.OrderActivityQuantity = OO.Quantity,OOAS.PatientID = OO.PatientID,OOAS.EncounterID = OO.EncounterID,OOAS.OrderBy = OO.PhysicianID
						,OOAS.PlannedBy = OO.PhysicianID,OOAS.PlannedDate = OOAS.OrderScheduleDate,OOAS.CorporateID = OO.CorporateID,OOAS.FacilityID = OO.FacilityID
					from OpenOrder OO
					inner join OrderActivity OOAS on OO.OpenOrderID = OOAS.OrderID  And (OOAS.OrderActivityStatus = 1) and OOAS.OrderScheduleDate > DATEADD(MINUTE,-5,@CurrentDate)
				END
				

				--end of cursor

			commit TRANSACTION

		END TRY

		BEGIN CATCH
		--print 'Execption Occured;'
			IF @@TRANCOUNT > 0 AND XACT_STATE() <> 0 
				SELECT  
				ERROR_NUMBER() AS ErrorNumber  
				,ERROR_SEVERITY() AS ErrorSeverity  
				,ERROR_STATE() AS ErrorState  
				,ERROR_PROCEDURE() AS ErrorProcedure  
				,ERROR_LINE() AS ErrorLine  
				,ERROR_MESSAGE() AS ErrorMessage; 
				ROLLBACK TRAN

			-- Do the Necessary Error logging if required
			-- Take Corrective Action if Required

		END CATCH
END





GO


