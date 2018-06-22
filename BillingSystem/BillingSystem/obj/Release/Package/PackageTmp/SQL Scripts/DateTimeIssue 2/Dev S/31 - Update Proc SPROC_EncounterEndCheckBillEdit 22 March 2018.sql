IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_EncounterEndCheckBillEdit')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_EncounterEndCheckBillEdit
GO

/****** Object:  StoredProcedure [dbo].[SPROC_EncounterEndCheckBillEdit]    Script Date: 22-03-2018 19:40:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_EncounterEndCheckBillEdit] 
(  
	@pEncounterID int =1164,
	@pLoggedInUserId INT=1
)
AS  
BEGIN
	DECLARE @Facility_Id int
		set @Facility_Id=(select EncounterFacility from dbo.Encounter where EncounterID=@pEncounterID)

	Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))

	Declare @AuthID int, @Cur_BillHeaderID int,@RETRUN_Status int,@Cur_Status int, @RetStatus int,@SelfPayFlag bit=0;
	--- Get The Authorization Info eg: AuthCode,AuthType,PayerID and MemberID from EncounterID

	Set @RetStatus = 0  ---- FreshStart --- If this does not change means nothing happened (No selections for passed in Encounter)

	Set @AuthID = NULL
	Select TOp 1 @AuthID = A.AuthorizationID from [Authorization] A where EncounterID = @pEncounterID order by AuthorizationId desc

	Select TOP 1 @SelfPayFlag = EncounterSelfPayFlag from BillHeader where EncounterID = @pEncounterID order by BillHeaderID desc--and [Status] <=40

	Set @SelfPayFlag = ISNULL(@SelfPayFlag,1);

	Declare @spOpenOrderCount int = 0;
	---- Checks Added by shashank to stop the bill from scrubbing if any open order exist for the encounter
	Select @spOpenOrderCount = Count(1) from OpenOrder where EncounterId = @pEncounterID and OrderStatus = 1 
	-- Get the number of Open orders for the enocunter if count is more than 1 then don't scrub the bill

	If @AuthID is NULL and @SelfPayFlag  <> 1  
	Begin
		Set @RetStatus = 99  -- Means Authorization is Not Obtained so Cannot proceed to premlimnary Bill
	End
	ELSE IF (@spOpenOrderCount > 0)
	BEGIN
		Set @RetStatus = 99  -- Means Authorization is Not Obtained so Cannot proceed to premlimnary Bill
	END
	ELSE
	  Begin
		---- Set All Bills Without AuthID and Status still not on Prelimnary and Automatically Call Bill Scruber
		Update BillHeader Set  Status = 45, AuthID=@AuthID,DueDate = @LocalDateTime  Where EncounterID  = @pEncounterID and [Status] <=40;
	  End

	--- Now Call Bill Scrubber for all bills at Stage 45 for passed in Encounter
					
			DECLARE BH CURSOR FOR
			Select BillHeaderID,[Status] from BillHeader B Where B.Status = 45 AND EncounterID = @pEncounterID;
									
			OPEN BH;  					
			FETCH NEXT FROM BH INTO @Cur_BillHeaderID,@Cur_Status
	
			WHILE @@FETCH_STATUS = 0  
			BEGIN

				Exec [dbo].[SPROC_ScrubBill] @Cur_BillHeaderID,@pLoggedInUserId,@RETRUN_Status output
				
				Update BillHeader Set [Status] = CASE WHEN @RETRUN_Status = 0
									THEN [dbo].[GetBillNextStatus] (@Cur_Status,1) ELSE [dbo].[GetBillNextStatus] (@Cur_Status,2)  END  
				Where BillHeaderID = @Cur_BillHeaderID;
				
				Set @RetStatus = 1   ---- Scrub Ran
				FETCH NEXT FROM BH INTO @Cur_BillHeaderID,@Cur_Status
			
			END 
			
			CLOSE BH;  
			DEALLOCATE BH;
	
	Select @RetStatus 'RetStatus'  ----- 0 - means nothing happened or selected, 1 -- Means Successfully Scrub Ran  ---- 99 ---- Check Failed eg: AuthID is NULL

END



GO


