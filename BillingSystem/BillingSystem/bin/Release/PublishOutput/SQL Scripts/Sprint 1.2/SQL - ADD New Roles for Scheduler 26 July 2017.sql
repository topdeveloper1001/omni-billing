Declare @CDate datetime = GETDATE()
Exec SprocSaveFacilityRole @FRoleId=0,@FId=7,@RId=0,@CId=9,@UId=10,@AddToAll=1,@IsDeleted=0,@SchedulingApplied=1,@CarePlanApplied=0,@RName='Centeralized Scheduler',@CurrentDate= @CDate


