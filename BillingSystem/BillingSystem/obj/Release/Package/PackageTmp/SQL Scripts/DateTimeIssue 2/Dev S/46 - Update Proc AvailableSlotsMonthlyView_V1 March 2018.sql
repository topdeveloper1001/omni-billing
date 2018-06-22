IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'AvailableSlotsMonthlyView_V1')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE AvailableSlotsMonthlyView_V1
GO

/****** Object:  StoredProcedure [dbo].[AvailableSlotsMonthlyView_V1]    Script Date: 22-03-2018 20:16:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<BB>
-- Create date: <Dec-2015>
-- Description:	<Scheduling Monthly View - To Display Available Time Slots based on different option passed
				---- It is designed to make flexibility - To show Combinations or particular Physicians OR Rooms --- Later we can make it for Patients and Conflicts>
-- =============================================
CREATE PROCEDURE [dbo].[AvailableSlotsMonthlyView_V1]    ----->>> Exec [AvailableSlotsMonthlyView_V1_NY20151223_DIV] -- [AvailableSlotsMonthlyView_V1]
	(
	@StartDate date = '2016-02-01', --- (MUST Pass Start Date of the Month to View)
	@EndDate date = '2016-02-29',	--- (MUST Pass End Date of the Month to View)
	@StartTime time = '12:00:00',	--- (Must Pass Start Time from Where Slots need to Start --- NOTE: This can also be opening hours of Department)
	@EndTime time = '16:00:00',	--- (Must Pass End Time Till Where Slots need to End --- NOTE: This can also be opening hours of Department)
	@AvgTimeSlot numeric(7,2) = 120,	---- (Must Pass --- This will display Slots based on this Value (NOTE:It is in Minutes) --- Can be Default department Slot time
										---- Continued: This is important factor and will return display based on this -- Will help in two ways
										---- Continued: 1. Help Scheduler to view Less slots To get to slot needed faster and decide eg: 30 mins, 1 hour, 2 hour, 4 hour etc
										---- Continued: 2. It can be used also if Scheduler is looking for continuous slots available eg: 2 hours operation, 4 hours for a HomeCare etc.
	@PassedPhysician int = 0,		--- (OPTIONAL --> If passed only the Physician will show Else All eligible Physicians will come)
	@PassedRoom int = 0	, 			--- (OPTIONAL --> If passed only that Room will show Else All eligible Rooms will come)
	@DepartmentID int = 0,			--- (NOTE: NOT in USE but will require to make selections of Physicians and Rooms based on this )
	@FacilityID int =4,  -- Mandatory
	@ViewFlag	int = 0				--- (MUST Pass values are: 0=View, 1=Explanations, 3=Conflicts)
	)
AS
BEGIN

		Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@FacilityID))

		---- Local Variables
		Declare @DayTime time,@TotalMins numeric(7,2),@NumOfSlots int, @BreakStartSlot time, @BreakEndSlot time, @FormatedEndTime time,
					@DepDefaultTimeSlot numeric(7,2) = 30, @TotalAvailableSlots numeric(7,2)

		------ XXXXXXXXXXXXXXXXX Temp Tables to Collect information and format as per desired results  --- Starts
		-----> To Hold Creation of TimeSlots per Day based on Input Params
		Declare @DaysTimeSlots table (TID int, TSStart time, TSEnd time)
		-----> To Hold All different Physicians Capable of doing the Task
		Declare @CapableDepartments table (DPID int,FSID int)
		-----> To Hold All different Physicians Capable of doing the Task
		Declare @CapablePhySicians table (CPID int,PhyID int)
		-----> To Hold All different Rooms Possible of handling the Task
		Declare @PossibleRooms table (PRID int,RoomID int,DepID int)
		-----> XXXXXXXXXX  NEW Possible Departments
		Declare @DepWorkingDays table (WDID int,WDate Date,FSID int, DayWeek int)
		-----> To Hold Scheduled Data including PatientSchedules, BreakTimes and Vacations and Then formatting as per Desired Output
		Declare @ReportView table (DPID int,PID int, RID int, SchDate Date, TSStart time, TSEnd time, SchStatus int, BlockType int, ClientID int,ActBlockingID int,Explanations nvarchar(max))
		-----> To Hold Scheduled Data including PatientSchedules, BreakTimes and Vacations and Then formatting as per Desired Output
		Declare @DepView table (DPID int,PID int, RID int, SchDate Date, TSStart time, TSEnd time, SchStatus int, BlockType int, ClientID int,ActBlockingID int,Explanations nvarchar(max))
		-----> To Collect information for Explanations as one comments along with count of reasons --- Needed to make it easy on front end and better performance
		Declare @PivotTBL table (DPID int,PID int, RID int, TSStart time, TSEnd time, WorkDay nvarchar(5), ReasonCount int,BookedPercent numeric(7,2),Reasons nvarchar(max))
		------ XXXXXXXXXXXXXXXX Temp Tables to Collect information and format as per desired results  --- Ends

		------New calculated dates ----- Starts
			
			Set @StartDate = CONVERT(VARCHAR(25),DATEADD(dd,-(DAY(@StartDate)-1),@StartDate),101)
			Set @EndDate = CONVERT(VARCHAR(25),DATEADD(dd,-(DAY(DATEADD(mm,1,@StartDate))),DATEADD(mm,1,@StartDate)),101)
			
		------New calculated dates ----- Ends

		--- XXXXXXXXXXXXXXXXX Special Settings ---- STARTS
		If @EndTime = '00:00:00' Set @FormatedEndTime = Dateadd(minute,-1,@EndTime) ELSE Set @FormatedEndTime = @EndTime
		Set @PassedPhysician = isnull(@PassedPhysician,0)
		Set @PassedRoom = isnull(@PassedRoom,0)
		Set @ViewFlag = isnull(@ViewFlag,0)

		----- Total Avaialable Slots within Average Slot passed in 
		Set @TotalAvailableSlots  = @AvgTimeSlot/@DepDefaultTimeSlot
		----- Total Minutes in asked Timmings
		Set @TotalMins = DateDiff(minute,@StartTime,@FormatedEndTime)
		----- Number of Slots in Asked timing based on Average Time Slot passed in 
		Set @NumOfSlots = CEILING(@TotalMins/@AvgTimeSlot)
		----- Making Collection of Available Slots in a Day --- Starts
		While @NumOfSlots > 0
		Begin
			Set @EndTime = DateAdd(minute,@AvgTimeSlot,@StartTime)
			insert into @DaysTimeSlots values(1,@StartTime,DateAdd(minute,@AvgTimeSlot,@StartTime));

			Select @NumOfSlots = (@NumOfSlots - 1), @StartTime = @EndTime
		End
		----- Making Collection of Available Slots in a Day --- Ends

		--- XXXXXXXXXXXXXXXXX Special Settings ---- ENDS

		----->>>>>>>>>>>>>>>   Following is where Clauses will be adjusted to get Desired Physicians and Rooms as per Selections passed in ---- STARTS
		---- Now Getting all Capable Physician List
		If @PassedPhysician = 0
			insert into @CapablePhySicians 
			
			Select 1,[ID] from [dbo].[Physician] Where FacilityId = @FacilityID And [ID] in (Select distinct  PhysicianID from [dbo].[Scheduling] Where isnull(PhysicianID,0) <> 0 And FacilityId = @FacilityID) --- For Local on 10  (2040,2036) ---> For Staging UAE> (4,1007,1031,1024) 
			UNION
			Select 1,[ID] from [dbo].[Physician] Where FacilityId = @FacilityID And [ID] in (Select distinct  AssociatedId from [dbo].[Scheduling] Where isnull(AssociatedId,0) <> 0 And FacilityId = @FacilityID) --- For Local on 10  (2040,2036) ---> For Staging UAE> (4,1007,1031,1024) 
			
			--Select 1,[ID] from [dbo].[Physician] Where FacilityId = @FacilityID And FacultyDepartment = @DepartmentID
		ELSE insert into @CapablePhySicians Select 1,@PassedPhysician

		 --Select * from @CapablePhySicians
		---- Now Getting all Possible Rooms List
		/*If @PassedRoom = 0
			insert into @PossibleRooms Select 1,FacilityStructureID from [dbo].[FacilityStructure] Where GlobalCodeID = '84' And FacilityId = @FacilityID 
			and FacilityStructureID in (Select distinct top(5) RoomAssigned from [dbo].[Scheduling] Where isnull(RoomAssigned,0) <> 0 And FacilityId = @FacilityID) --- For Local 10 --- (35,1675,1677,1678,1679) ---> For Staging UAE>(36,37,2749,2837) 
		ELSE insert into @PossibleRooms Select 1,@PassedRoom*/

		---- Now Getting all Capable Department List
		If @DepartmentID = 0
			insert into @CapableDepartments 
			Select 1,FacilityStructureID from [dbo].[FacilityStructure] Where GlobalCodeID = '83' and FacilityID = @FacilityID
		ELSE insert into @CapableDepartments Select 1,@DepartmentID

		If @PassedRoom = 0
			insert into @PossibleRooms Select 1,FacilityStructureID,ParentID from [dbo].[FacilityStructure] Where GlobalCodeID = '84' And 
			ParentID in (Select FSID from @CapableDepartments) and FacilityID = @FacilityID
		ELSE insert into @PossibleRooms Select 1,@PassedRoom,@DepartmentID

		--Select * from @PossibleRooms
		----->>>>>>>>>>>>>>>   Following is where Clauses will be adjusted to get Desired Physicians and Rooms as per Selections passed in ---- ENDS

		----- Making sure One entry for each Capable Physician and Possible Room is there for all possible slots --- This is needed to display Pivot Correctly for All Combinations 
		--Insert into @ReportView Select PhyID,RoomID,@StartDate,TSStart,TSEnd,0,1,0,PhyID,'' --changed
		--from @CapablePhySicians inner join @DaysTimeSlots on TID = 1  inner join @PossibleRooms on PRID = 1 

		----->> Getting all Possible  BreakTimes and Vacations as per Format and TimeSlots asked for Physicians --- STARTS
		----- NOTE: Only get Time Slots Which are not Assigned to Any Room (Meaning Breaks/Vacations) 
		---- Because Slots with Rooms will be handled by Room Selection ELSE Duplicated Slots will come (That is Why RoomAssigned is NULL required in below Query)
		Insert into @ReportView
		Select 0,AssociatedId,0,Cast(ScheduleFrom as Date) 'OnDate',TSStart, TSEnd, 
		(CASE When  (Cast(ScheduleFrom as Time)  between TSStart and Dateadd(minute,-1,TSEnd)) OR (Cast(ScheduleTo as Time) = Dateadd(minute,-1,TSEnd)) OR
		(Cast(ScheduleFrom as Time) < TSStart and Cast(ScheduleTo as Time) > Dateadd(minute,-1,TSEnd) ) Then 
		((DATEDIFF(MINUTE,Case When Cast(ScheduleFrom as Time) < TSStart THEN TSStart ELSE Cast(ScheduleFrom as Time) END,
		CASE When TSEnd < Cast(ScheduleTo as Time) Then TSEnd ELSE Cast(ScheduleTo as Time) END ))/@AvgTimeSlot)*@TotalAvailableSlots
		Else 0 END),1,0,PhysicianID,'<div class=tooltip_row>'+
		'~FROM- '+ Cast((Cast(ScheduleFrom as Time)) as Nvarchar(5)) + ' TO-'+ Cast((Cast(ScheduleTo as Time)) as Nvarchar(5))+ ' --) Faculty Unavailable Due To: ' 
		+Comments+'</div>'
		from [dbo].[Scheduling] inner join @DaysTimeSlots on TID = 1 
		Where ScheduleFrom between @StartDate and @EndDate and 
		AssociatedId in (Select PhyID from @CapablePhySicians)
		and PhysicianID Is Null
		--(PhysicianID in (Select PhyID from @CapablePhySicians) or AssociatedId in (Select PhyID from @CapablePhySicians))
		and RoomAssigned is NULL And FacilityId = @FacilityID

		--select * from @ReportView
		----->> Getting all Possible Schedules, Out Of Service periods as per Format and TimeSlots asked for Rooms 
		Insert into @ReportView
		Select pr.DepID,PhysicianID,RoomAssigned,Cast(ScheduleFrom as Date) 'OnDate',TSStart, TSEnd, 
		(CASE When  (Cast(ScheduleFrom as Time)  between TSStart and Dateadd(minute,-1,TSEnd)) OR (Cast(ScheduleTo as Time) = Dateadd(minute,-1,TSEnd)) OR
		(Cast(ScheduleFrom as Time) < TSStart and Cast(ScheduleTo as Time) > Dateadd(minute,-1,TSEnd) ) Then 
		((DATEDIFF(MINUTE,Case When Cast(ScheduleFrom as Time) < TSStart THEN TSStart ELSE Cast(ScheduleFrom as Time) END,
		CASE When TSEnd < Cast(ScheduleTo as Time) Then TSEnd ELSE Cast(ScheduleTo as Time) END ))/@AvgTimeSlot)*@TotalAvailableSlots
		Else 0 END),2,AssociatedID,PhysicianID,
		(CASE When PhysicianID is not NULL then 
		'<div class=tooltip_row>'+
		'~FROM- '+ Cast((Cast(ScheduleFrom as Time)) as Nvarchar(5)) + ' TO-'+ Cast((Cast(ScheduleTo as Time)) as Nvarchar(5))+ ' --) ROOM Booked for: {Faculty=' +
		 Cast(PhysicianID as nvarchar(8))+ '}'+ ' {Patient=' +Cast(AssociatedID as nvarchar(8)) + '} '+ Comments +'</div>' 
		 ELSE '<div class=tooltip_row>'+'~FROM- '+ Cast((Cast(ScheduleFrom as Time)) as Nvarchar(5)) + ' TO-'+ Cast((Cast(ScheduleTo as Time)) as Nvarchar(5))+ ' --) ROOM OUT OF SERVICE: ' +Comments+'</div>' END)
		from [dbo].[Scheduling] inner join @DaysTimeSlots on TID = 1 
		inner join @PossibleRooms pr on pr.RoomID = RoomAssigned
		Where ScheduleFrom between @StartDate and @EndDate and  RoomAssigned in (Select RoomID from @PossibleRooms) And FacilityId = @FacilityID



		insert into @ReportView
		Select 0,FR.FacultyId,0,Cast(FromDate as Date) 'OnDate',DTS.TSStart, DTS.TSEnd, 
		(CASE When  (Cast(FR.FromDate as Time)  between DTS.TSStart and Dateadd(minute,-1,DTS.TSEnd)) OR (Cast(FR.FromDate as Time) = Dateadd(minute,-1,DTS.TSEnd)) OR
		(@StartTime <= DTS.TSStart and Cast(FR.FromDate as Time) > Dateadd(minute,-1,DTS.TSEnd) ) Then 
		((DATEDIFF(MINUTE,DTS.TSStart,CASE When DTS.TSEnd < Cast(FR.FromDate as Time) Then 
		DTS.TSEnd ELSE Cast(FR.FromDate as Time) END ))/@AvgTimeSlot)*@TotalAvailableSlots Else 0 END)
		,1,0,FR.FacultyId,'<div class=tooltip_row>'+' --) Physician Not Available till Time: ' + Cast(Cast(FR.FromDate as Time) as varchar(5)) + ' </div>'
		from FacultyRooster FR inner join @DaysTimeSlots DTS on DTS.TID = 1 
		--inner join @DepWorkingDays DWD on DWD.FSID = FR.DeptId --and DWD.DayWeek = DT.OpeningDayID
		Where FR.DeptId =  @DepartmentID And FR.FromDate Between @StartDate And @EndDate And FR.FromDate Is Not Null
		And FR.FacultyId in (Select PhyID from @CapablePhySicians) And FacilityId = @FacilityID

		/*Select 0,FR.FacultyId,0,Cast(FromDate as Date) 'OnDate',DTS.TSStart, DTS.TSEnd,
		(


		CASE When  (Cast(FR.ToDate as Time)  between DTS.TSStart and Dateadd(minute,-1,DTS.TSEnd)) OR (Cast(FR.ToDate as Time) < Dateadd(minute,-1,DTS.TSStart)) 
		--OR (@StartTime <= DTS.TSStart and Cast(FR.FromDate as Time) > Dateadd(minute,-1,DTS.TSEnd) ) 
		Then CEILING((DATEDIFF(MINUTE,(
										CASE WHEN Cast(FR.ToDate as Time) > DTS.TSStart then Cast(FR.ToDate as Time) ELSE DTS.TSStart END
									   ),
			(CASE WHEN DTS.TSEnd = '00:00' Then '23:59' ELSE DTS.TSEnd END)
			                  )/@AvgTimeSlot)*@TotalAvailableSlots) Else 0 END
		
		
		
		)	
			,1,0,FR.FacultyId, '<div class=tooltip_row>'+' --) Physician Not Available After Time: ' + Cast(Cast(FR.ToDate as Time) as varchar(5)) + '</div>'
		 from FacultyRooster FR inner join @DaysTimeSlots DTS on DTS.TID = 1 
		--inner join @DepWorkingDays DWD on DWD.FSID = FR.DeptId --and DWD.DayWeek = DT.OpeningDayID
		Where FR.DeptId =  @DepartmentID And FR.ToDate Between @StartDate And @EndDate And FR.ToDate Is Not Null
		And FR.FacultyId in (Select PhyID from @CapablePhySicians) And FacilityId = @FacilityID--and Cast(FR.FromDate as Time) <> @StartTime*/
		
		 insert into @ReportView
		 Select 0,FR.FacultyId,0,Cast(FromDate as Date) 'OnDate',DTS.TSStart, DTS.TSEnd,
		(CASE When  (Cast(FR.ToDate as Time)  between DTS.TSStart and Dateadd(minute,-1,DTS.TSEnd)) OR (Cast(FR.ToDate as Time) < Dateadd(minute,-1,DTS.TSStart)) 
		--OR (@StartTime <= DTS.TSStart and Cast(FR.FromDate as Time) > Dateadd(minute,-1,DTS.TSEnd) ) 
		Then CEILING((DATEDIFF(MINUTE,(CASE WHEN Cast(FR.ToDate as Time) > DTS.TSStart then Cast(FR.ToDate as Time) ELSE DTS.TSStart END),
			(CASE WHEN DTS.TSEnd = '00:00' Then '23:59' ELSE DTS.TSEnd END))/@AvgTimeSlot)*@TotalAvailableSlots) Else 0 END)	
			,1,0,FR.FacultyId, '<div class=tooltip_row>'+' --) Physician Not Available After Time: ' + Cast(Cast(FR.ToDate as Time) as varchar(5)) + '</div>'
		 from FacultyRooster FR inner join @DaysTimeSlots DTS on DTS.TID = 1 
		--inner join @DepWorkingDays DWD on DWD.FSID = FR.DeptId --and DWD.DayWeek = DT.OpeningDayID
		Where FR.DeptId =  @DepartmentID And FR.ToDate Between @StartDate And @EndDate And FR.ToDate Is Not Null
		And FR.FacultyId in (Select PhyID from @CapablePhySicians) And FacilityId = @FacilityID--and Cast(FR.FromDate as Time) <> @StartTime


				

		--select * from @ReportView
		-----XXXXXX Combining Rooms with Physicians - NOTE: Room once booked for one Physician should also be blocked for all other for same Slot --- STARTS
		----- That is Why Type 3 Entry (Combination is done + Type 1 will give a Combined View) Needed as done Below

		------>>>>>>>>>>>>>>>> Following is to be done for Rooms Slot Booked should be Blocked for All Other Physicians as well
		 Insert into @ReportView
		 Select DPID,PhyID,RID,SchDate,TSStart,TSEnd,SchStatus,3,ClientID,ActBlockingID,Explanations from @ReportView 
		 inner join @CapablePhySicians on CPID = 1 Where BlockType =2 and SchStatus > 0

		 ------>>>>>>>>>>>>>>>> Following is to be done for Physician Slot Booked should be Blocked for All Other Rooms as well
		 Insert into @ReportView 
		 Select DepID,PID,RoomID,SchDate,TSStart,TSEnd,SchStatus,3,ClientID,ActBlockingID,Explanations from @ReportView 
		 inner join @PossibleRooms on PRID = 1  Where BlockType =1 and SchStatus > 0
 
  


  ---->>>>>>>XXXXXXXXXXXXXXXXXXXXXXx>>>>>>>>>>>>>>>>  FOR DEPARTMENT WORKING HOURS LOGIC - STARTS
				Declare @RepeatNumber int = (31*(Select count(1) from @CapableDepartments))

				---- Collecting all Possible Working days within Month --- STARTS
				 ;with MC as (
				 select @StartDate as dt
				 union all
				 select dateadd(DAY, 1, dt)
				 from MC
				) 
				insert into @DepWorkingDays
				select top (@RepeatNumber) 1,dt,FSID,Datepart(Weekday,dt)-1   from MC inner join @CapableDepartments on DPID = 1 
				---- Collecting all Possible Working days within Month --- ENDS

				----- Marking Before Opening Hours as OFF as per setup  --- STARTS
				insert into @DepView
				Select DT.FacilityStructureID,0,0,DWD.WDate,DTS.TSStart, DTS.TSEnd, 
				(CASE When  (Cast(DT.OpeningTime as Time)  between DTS.TSStart and Dateadd(minute,-1,DTS.TSEnd)) OR (Cast(DT.OpeningTime as Time) = Dateadd(minute,-1,DTS.TSEnd)) OR
				(@StartTime <= DTS.TSStart and Cast(DT.OpeningTime as Time) > Dateadd(minute,-1,DTS.TSEnd) ) Then 
				((DATEDIFF(MINUTE,DTS.TSStart,CASE When DTS.TSEnd < Cast(DT.OpeningTime as Time) Then 
				DTS.TSEnd ELSE Cast(DT.OpeningTime as Time) END ))/@AvgTimeSlot)*@TotalAvailableSlots Else 0 END)
					,99,0,FacilityStructureID,'<div class=tooltip_row>'+' --) DEPARTMENT CLOSED - Opening Hours From: ' + DT.OpeningTime + '</div>'
				 from DeptTimming DT inner join @DaysTimeSlots DTS on DTS.TID = 1 
				 inner join @DepWorkingDays DWD on DWD.FSID = DT.FacilityStructureID and DWD.DayWeek = DT.OpeningDayID
				 Where DT.FacilityStructureID in (Select FSID from @CapableDepartments) and DT.OpeningTime <> @StartTime

				 ----- Marking Before Opening Hours as OFF as per setup  --- ENDS

				 ----- Marking Before Closing Hours as OFF as per setup  --- STARTS
				 insert into @DepView
				Select DT.FacilityStructureID,0,0,DWD.WDate,DTS.TSStart, DTS.TSEnd, 
				(CASE When  (Cast(DT.ClosingTime as Time)  between DTS.TSStart and Dateadd(minute,-1,DTS.TSEnd))  OR (Cast(DT.ClosingTime as Time) < Dateadd(minute,-1,DTS.TSStart)) 
				--- OR (@StartTime < DTS.TSStart and Cast(DT.OpeningTime as Time) > Dateadd(minute,-1,DTS.TSEnd) )  
				Then CEILING((DATEDIFF(MINUTE,(CASE WHEN Cast(DT.ClosingTime as Time) > DTS.TSStart then Cast(DT.ClosingTime as Time) ELSE DTS.TSStart END),
					(CASE WHEN DTS.TSEnd = '00:00' Then '23:59' ELSE DTS.TSEnd END))/@AvgTimeSlot)*@TotalAvailableSlots) Else 0 END)	
					,99,0,FacilityStructureID, '<div class=tooltip_row>'+' --) DEPARTMENT CLOSED - Closure Time From: ' + DT.ClosingTime + '</div>'
				 from DeptTimming DT inner join @DaysTimeSlots DTS on DTS.TID = 1 
				 inner join @DepWorkingDays DWD on DWD.FSID = DT.FacilityStructureID and DWD.DayWeek = DT.OpeningDayID
				 Where DT.FacilityStructureID in (Select FSID from @CapableDepartments) --and DT.OpeningTime <> @StartTime

				 ----- Marking Before Closing Hours as OFF as per setup  --- ENDS
				 

			---->>>>>>>XXXXXXXXXXXXXXXXXXXXXXx>>>>>>>>>>>>>>>>  FOR DEPARTMENT WORKING HOURS LOGIC - ENDS
			--Select * from @DepView Where SchDate = '2016-01-13'
			--Select * from @ReportView Where BlockType In(3,99) and SchStatus > 0
			/*Select r1.DPId,r1.PID,r1.RID,r2.DPID 'R2-DPID',r1.SchDate,r2.SchDate 'R2-Date',r1.TSStart,r2.TSStart 'R2-TSStart', 
			(r2.SchStatus + r1.SchStatus),r2.Explanations 'R2-Ex',r1.Explanations 'R1-Ex',(r2.Explanations+'--'+r1.Explanations)
			From @ReportView r1, @DepView r2 Where r1.BlockType = 3 And r1.DPID = r2.DPID And 
			r1.SchDate=r2.SchDate And r1.TSStart =r2.TSStart And r2.BlockType = 99 And r1.SchDate = '2016-01-13'
			And r2.SchStatus>0
			Order By r1.DPId,r1.RID,r1.SchDate,r1.TSStart*/
			Update r1 Set r1.SchStatus = (r2.SchStatus + r1.SchStatus),r1.Explanations=(r2.Explanations+r1.Explanations)
			From @ReportView r1, @DepView r2 Where r1.BlockType = 3 And r1.DPID = r2.DPID And 
			r1.SchDate=r2.SchDate And r1.TSStart =r2.TSStart And r2.BlockType = 99 And r2.SchStatus > 0
			--Select * from @ReportView
			--Select * from @ReportView Where BlockType = 3 Order By DPID,RID, SchDate,TSStart
 
		-----XXXXXX Combining Rooms with Physicians - NOTE: Room once booked for one Physician should also be blocked for all other for same Slot --- ENDS
		----->> Getting all Possible Schedules, BreakTimes and Vacations as per Format and TimeSlots asked for Physicians --- ENDS
					
		insert into @PivotTBL				
		Select T1.DPID,T1.PID, T1.RID,Cast(T1.TSStart as nvarchar(5)) 'TSStart',Cast(T1.TSEnd as nvarchar(5)) 'TSEnd', 'D'+ CAST((DAY(T1.SchDate)) AS Varchar(2)) WorkDay, SUM(T1.SchStatus), (SUM(T1.SchStatus)/@TotalAvailableSlots)*100 
		, (SELECT ' ' + T2.Explanations FROM @ReportView T2  WHERE T1.PID = T2.PID and T1.RID = T2.RID and T2.BlockType = 3 and
		  Cast(T1.TSStart as nvarchar(5)) = Cast(T2.TSStart as nvarchar(5)) and Cast(T1.TSEnd as nvarchar(5)) = Cast(T2.TSEnd as nvarchar(5)) and
		  'D'+ CAST((DAY(T1.SchDate)) AS Varchar(2)) = 'D'+ CAST((DAY(T2.SchDate)) AS Varchar(2))  FOR XML PATH (''),Type).value('.','VARCHAR(MAX)') Reasons
		from @ReportView T1 Where BlockType = 3 --and SchStatus = 1
		Group By T1.DPID,T1.PID, T1.RID,'D'+ CAST((DAY(T1.SchDate)) AS Varchar(2)),Cast(T1.TSStart as nvarchar(5)),Cast(T1.TSEnd as nvarchar(5)) ;
		
		----- Making sure One entry for each Capable Physician and Possible Room is there for all possible slots --- This is needed to display Pivot Correctly for All Combinations 				
		insert into @PivotTBL	Select DepID,PhyID,RoomID,TSStart,TSEnd,'D1',0,0,'0' 
		from @CapablePhySicians inner join @DaysTimeSlots on TID = 1  inner join @PossibleRooms on PRID = 1 
		
		----- Final Update of Comments with Count and Percentage of Slot used (Color Coding will be if percentage which starts from First Tild in each Block
		----- Color Coding Value Say XX which is after First Tild 
		----- XX = 0 (GREEN), XX between 0.1 and 25 (Light GREEN), XX between 25.01 and 50 (YELLOW), XX between 50.01 and 75 (PINK), XX >= 100 (RED) 
		------ >>>>>>>>>>>>>>>>>>>>>>/* OLD Part NOT NEEDED ANY More*/
		--Update @PivotTBL Set Reasons = Cast(ReasonCount as nvarchar(10))+'~'+substring((Cast((ReasonCount/@TotalAvailableSlots)*100 as nvarchar(50))),1,5)  + Reasons
		--Where ReasonCount > 0
		------Update @PivotTBL Set Reasons = Cast(ReasonCount as nvarchar(10))+'~'+substring((Cast(Bookedpercent as nvarchar(50))),1,5)  + Reasons
		------Where ReasonCount > 0
		------ >>>>>>>>>>>>>>>>>>>>>>/* OLD Part NOT NEEDED ANY More*/

		----XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX NEWWAY OUTPUT With DIV ----XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX---- STARTS
		--select * from @PivotTBL
		------ NOTE There are other changes made Above as well
		Update @PivotTBL Set Reasons = '<div ' + Case When Reasons <> '0' Then
		'title="<div class=qtip_title><h2>Physician: ' + Cast(PID as nvarchar(10)) + '</h2><h2>Room: ' + Cast(RID as nvarchar(10)) + '</h2></div>' + Reasons +'" '
		Else 'title="" ' End +'
		class="color_td ' + 
		--(Case When (Cast(Replace(WorkDay,'D','') As Int) >= DATEPART(dd,@LocalDateTime)) Then (Case When Bookedpercent = 0	Then 'green_column' 
		--(Case When (Cast(Replace(WorkDay,'D','') As Int) >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then 
		(Case When Bookedpercent = 0	Then 'green_column' 
			  When (Bookedpercent > 0 And Bookedpercent <=25)  Then 'light_green_column'
								 When (Bookedpercent > 25 And Bookedpercent <=50) Then 'yellow_column' 
								 When (Bookedpercent > 50 And Bookedpercent <=75) Then 'pink_column' 
								 When (Bookedpercent > 75 And Bookedpercent <100) Then 'pink_column' 
								 When  Bookedpercent >= 100                       Then 'red_column' 
								 Else 'green_column' END) +
		--Else 'grey_column' End) + 
		'" attr-DeptId="' + Cast(DPID as nvarchar(10))+ '" attr-RoomId="' + Cast(RID as nvarchar(10)) + '" attr-Day="' + Replace(WorkDay,'D','') + '"  attr-EndTime ="' + 
		Cast(TSEnd as nvarchar(5)) + '" attr-StartTime ="' + Cast(TSStart as nvarchar(5)) + '" attr-PhysicianId ="' + Cast(PID as nvarchar(10)) +
		 (Case When (Cast(Replace(WorkDay,'D','') As Int) >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then 
		 '" onclick="OnClickGetData(this);">' + Cast(ReasonCount as nvarchar(10)) 
		 Else '" >' + Cast(ReasonCount as nvarchar(10)) End)+
		 --'<div class=''qtip_title''><h2>Physician: ' + Cast(PID as nvarchar(10)) + '</h2><h2>Room: ' + Cast(RID as nvarchar(10)) + '</h2></div>' + 
			'<div class="selected_column" style="display:none;"></div></div>'
		--Where ReasonCount > 0
		----XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX NEWWAY OUTPUT With DIV ----XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX---- ENDS
		--select * from @PivotTBL
		---- Special Update asked to have proper color coding at front end --- ENDS
		
		----- View Display Formatting - STARTS
	IF @ViewFlag = 0
	Begin
		;With Report     
		AS    
		(    
		select * from 
		(    
				Select DPID,PID,RID,TSStart,TSEnd,WorkDay,Reasons from @PivotTBL			
  		) src    
		pivot    
		(    
		  Max(Reasons)    
		  for [WorkDay] in (D1,D2,D3,D4,D5,D6,D7,D8,D9,D10,D11,D12,D13,D14,D15,D16,D17,D18,D19,D20,D21,D22,D23,D24,D25,D26,D27,D28,D29,D30,D31)    
		)piv    
		)    
    
    
		----- Return Desired Results
		  Select  DPID,p.PhysicianName,PID,RID,Cast(TSStart as nvarchar(5)) 'STime',Cast(TSEnd as nvarchar(5)) 'ETime',
		  --D1 value start--  
		  Case When (1 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
			ISNULL(D1,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="1"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else 	ISNULL(D1,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="1"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D1, 
		 --D1 value end-- 
		 --D2 value start--  
		  --Case When (2 >= DATEPART(dd,@LocalDateTime)) Then
		  Case When (2 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D2,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="2"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D2,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="2"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D2,
		 --D2 value end--
		 --D3 value start--  
		  Case When (3 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D3,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="3"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D3,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="3"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D3,
		 --D3 value end-- 
		 --D4 value start--  
		  Case When (4 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D4,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="4"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D4,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="4"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D4,
		 --D4 value end--  
		 --D5 value start--  
		  Case When (5 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D5,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="5"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D5,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="5"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D5,
		 --D5 value end--    
		 --D6 value start--  
		  Case When (6 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D6,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="6"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D6,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="6"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D6,
		 --D6 value end--
		 --D7 value start--  
		  Case When (7 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D7,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="7"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D7,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="7"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D7,
		 --D7 value end--
		 --D8 value start--  
		  Case When (8 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D8,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="8"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D8,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="8"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D8,
		 --D8 value end-- 
		 --D9 value start--  
		  Case When (9 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D9,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="9"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D9,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="9"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D9,
		 --D9 value end--
		 --D10 value start--  
		  Case When (10 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D10,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="10"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D10,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="10"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D10,
		 --D10 value end--
		 --D11 value start--  
		  Case When (11 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D11,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="11"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D11,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="11"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D11,
		 --D11 value end--
		 --D12 value start--  
		  Case When (12 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D12,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="12"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D12,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="12"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D12,
		 --D12 value end--
		--D13 value start--  
		  Case When (13 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D13,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="13"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D13,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="13"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D13,
		 --D13 value end--  
		 --D14 value start--  
		  Case When (14 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		 ISNULL(D14,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="14"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
			<div class="selected_column" style="display:none;"></div></div>') 
		  Else ISNULL(D14,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="14"  
			attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
			<div class="selected_column" style="display:none;"></div></div>') End D14,
		 --D14 value end-- 
		 --D15 value start--  
		   Case When (15 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D15,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="15"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D15,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="15"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D15,
		 --D15 value end--  
		 --D16 value start--  
		   Case When (16 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D16,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="16"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D16,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="16"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D16,
		 --D16 value end--  
		 --D17 value start--  
		   Case When (17 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D17,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="17"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D17,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="17"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D17,
		 --D17 value end--    
		 --D18 value start--  
		   Case When (18 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D18,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="18"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D18,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="18"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D18,
		 --D18 value end-- 
		 --D19 value start--  
		   Case When (19 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D19,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="19"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D19,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="19"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D19,
		 --D19 value end--  
		 --D20 value start--  
		   Case When (20 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D20,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="20"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D20,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="20"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D20,
		 --D20 value end--
		 --D21 value start--  
		   Case When (21 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D21,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="21"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D21,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="21"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D21,
		 --D21 value end--
		 --D22 value start--  
		   Case When (22 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D22,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="22"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D22,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="22"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D22,
		 --D22 value end--  
		 --D23 value start--  
		   Case When (23 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D23,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="23"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D23,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="23"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D23,
		 --D23 value end--    
		 --D24 value start--  
		   Case When (24 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D24,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="24"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D24,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="24"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D24,
		 --D24 value end--  
		 --D25 value start--  
		   Case When (25 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D25,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="25"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D25,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="25"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D25,
		 --D25 value end--  
		 --D26 value start--  
		   Case When (26 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D26,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="26"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D26,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="26"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D26,
		 --D26 value end--  
		 --D27 value start--  
		   Case When (27 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D27,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="27"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D27,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="27"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D27,
		 --D27 value end-- 
		 --D28 value start--  
		   Case When (28 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D28,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="28"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D28,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="28"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D28,
		 --D28 value end-- 
		 --D29 value start--  
		   Case When (29 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D29,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="29"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D29,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="29"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D29,
		 --D29 value end--     
		 --D30 value start--  
		   Case When (30 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D30,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="30"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D30,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="30"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D30,
		 --D30 value end--  
		 --D31 value start--  
		   Case When (31 >= DATEPART(dd,@LocalDateTime) or (DATEPART(yyyy,Cast(@StartDate as date)) > DATEPART(yyyy,@LocalDateTime))) Then
		  ISNULL(D31,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="31"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'" onclick="OnClickGetData(this);">0
		 	<div class="selected_column" style="display:none;"></div></div>') 
		   Else ISNULL(D31,'<div title="" class="color_td green_column" attr-DeptId="'+Cast(DPID as nvarchar(10))+'" attr-RoomId="'+Cast(RID as nvarchar(10))+'" attr-Day="31"  
		 	attr-EndTime ="'+Cast(TSEnd as nvarchar(5))+'" attr-StartTime ="'+Cast(TSStart as nvarchar(5))+'" attr-PhysicianId ="'+ Cast(PID as nvarchar(10))+'">0
		 	<div class="selected_column" style="display:none;"></div></div>') End D31
		 --D31 value end--   
		 From Report r Inner Join Physician p On  r.PID = p.Id
		 --Order By DPID,RID,TSStart,TSEnd;  
		 Order By RID,DPID,TSStart,TSEnd;  
	End  ---- ViewFlag = 0 Ends
	
		 ----- View Display Formatting - ENDS
--*/
 END ----> Proc Ends  ----->>> Exec [AvailableSlotsMonthlyView_V1_BB20151216_DIV]





GO


