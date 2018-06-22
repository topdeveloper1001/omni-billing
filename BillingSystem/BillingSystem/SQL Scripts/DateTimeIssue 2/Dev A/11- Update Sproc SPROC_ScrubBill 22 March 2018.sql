IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ScrubBill')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ScrubBill

/****** Object:  StoredProcedure [dbo].[SPROC_ScrubBill]    Script Date: 3/22/2018 7:22:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_ScrubBill]  -- [SPROC_ScrubBill] 1516, 1,1
(  
@pBillHeaderID int = 1301,  --- Bill Header ID for whom Scrubbing is requested  
@pExecutedBy int = 28,  --- Person LoginID who executed this Test
@pRETStatus int  =1 Output
)  
AS  
BEGIN  
		Declare @LocalDateTime datetime
		,@Facility_Id int=(select FacilityId from BillHeader where BillHeaderID=@pBillHeaderID)

		SET @LocalDateTime = (Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))

    --- Declare Cursor Fetch Variables
	DECLARE @Cur_CorporateID INT,@Cur_FacilityID INT,@Cur_RuleID INT,@Cur_RuleCode nvarchar(20), @Cur_RuleStepID INT,@Cur_RuleDescription nvarchar(500),
			@Cur_RuleType INT,@Cur_RuleStepErrorID INT,@Cur_RSConStart int,@Cur_RSConEnd int,@Cur_RSConGroup int,@Cur_RSRuleStepNumber nvarchar(20)
			
	--- Declare Other Variables
	DECLARE @CurrentDate DATETIME = @LocalDateTime, @BillHeaderDate datetime, @PatientDOB datetime, @EncounterStartDate datetime, @PatientID INT, @EncounterID INT,
			@CorporateID INT,@FacilityID INT,@Status INT = 1,@ScrubHeaderID INT, @Performed INT = 0, @Passed INT = 0, @Failed INT = 0,@NotApplicable INT = 0, 
			@FirstTimeFlag BIT = 1, @ConFTFlag bit = 1, @PreviousRuleID int = 0, @RuleCheckFlag bit = 1, @RollBackFailCount int = 0,
			@PreviousCondition nvarchar(10),@ThenOrCase bit =0, @OnlyORCase bit =0
			
			Declare @TableNumber nvarchar(10),@FacilityNumber nvarchar(10)
			

			Select @CorporateID=CorporateID, @FacilityID= FacilityID, @EncounterID=EncounterID, @PatientID=PatientID,@BillHeaderDate = BillDate  
			from Billheader where BillHeaderID = @pBillHeaderID;
			Select @PatientDOB = PersonBirthDate from PatientInfo Where PatientID = @PatientID;
			Select @EncounterStartDate = EncounterStartTime from Encounter Where EncounterID = @EncounterID;

			 --- Get facilty NUmber from Facility ID
			Set @FacilityNumber =( Select FacilityNumber from facility where Corporateid = @CorporateID and FacilityId= @FacilityID)

			  --- Declare Variable for table numbers
			Declare @BillEditRuleTableNumber nvarchar(10)

			 ---Set Table Numbers by Corporate id and Facility number
			Select @BillEditRuleTableNumber = ISNULL(BillEditRuleTableNumber,'')
			From BillingSystemParameters Where CorporateId = @CorporateID And FacilityNumber = @FacilityNumber

			Set @BillEditRuleTableNumber = ISNULL(@BillEditRuleTableNumber,'')
			  -- IF Null Set table numbers from CorprateId Or by Default CorporateId
			IF @BillEditRuleTableNumber = ''
				Select @BillEditRuleTableNumber = ISNULL(BillEditRuleTableNumber,'0') From Corporate Where CorporateID = @CorporateID
			
			Set @BillEditRuleTableNumber = ISNULL(@BillEditRuleTableNumber,'0')
			-- Declare Cursor with Closed Order but not on any Bill
			
			DECLARE RuleToScrub CURSOR FOR
			SELECT RM.CorporateID,RM.FacilityID,RM.RuleMasterID, RM.RuleCode, RM.RuleType , RS.RuleStepID,RS.RuleStepDescription, RS.ErrorID, RS.ConStart,RS.ConEnd,RS.ConGroup,RS.RuleStepNumber
			from RuleMaster RM	inner join RuleStep RS on RS.RuleMasterID = RM.RuleMasterID
			Where RM.IsActive = 1 and  RM.ExtValue9 = @BillEditRuleTableNumber
			--and RM.CorporateId = @CorporateID and (FacilityId = @FacilityID or FacilityId =0) 
			Order by CAST(RM.RuleCode as INT),CAST(RS.RuleStepNumber as INT),RM.RuleMasterID,RS.RuleStepID,RS.ConStart; 
			--Order by RM.RuleMasterID,RS.RuleStepID,RS.ConStart; 
						
			OPEN RuleToScrub;  
					
			FETCH NEXT FROM RuleToScrub 
			INTO @Cur_CorporateID,@Cur_FacilityID,@Cur_RuleID,@Cur_RuleCode,@Cur_RuleType, @Cur_RuleStepID ,@Cur_RuleDescription,@Cur_RuleStepErrorID,@Cur_RSConStart,@Cur_RSConEnd,@Cur_RSConGroup,@Cur_RSRuleStepNumber ;
	

			WHILE @@FETCH_STATUS = 0  
			BEGIN
				---- XXXXXXXXXXXXXXXXXXXXXXXXXXXX --- Header Insert - STARTS----
					if @FirstTimeFlag = 1
					Begin
						INSERT INTO [dbo].[ScrubHeader] ([CorporateID],[FacilityID],[EncounterID],[PatientID],[BillHeaderID],[ScrubDate],[ExecutedBy],[IsActive])
						VALUES	(@CorporateID, @FacilityID,@EncounterID,@patientID,@pBillHeaderID,@CurrentDate,@pExecutedBy,1)
					 ---- Now Get the Latest ID for this TEST
	
						Select @ScrubHeaderID= max(ScrubHeaderID) from ScrubHeader
						Set @FirstTimeFlag = 0
					End
			---- XXXXXXXXXXXXXXXXXXXXXXXXXXXX --- Header Insert - ENDS----

			If @PreviousRuleID <> @Cur_RuleID
			Begin
				Set @ConFTFlag = 1
				Set @RuleCheckFlag = 1
				Set @Status = 0
			End

			

		---- Check for IF Conditional Rules  --- STARTS

		If @Cur_RSConStart = 2 and @ConFTFlag = 1
			Begin 
				Set @ConFTFlag = 0
				Set @RollBackFailCount = 0
				Select @RollBackFailCount = Count(1) from ScrubReport Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @Cur_RuleID and ConStart = 1 and [Status] > 0 
				Set @RollBackFailCount = isnull(@RollBackFailCount,0)
				If @RollBackFailCount  > 0 
				Begin
					Set @RuleCheckFlag = 0  --- Means IF part has one Condition which has failed - Stop further Check for this 
					Set @Status = 99
					--Print 'Update If Conditions to Mark as Not Applicable'
					---- Update If Conditions to Mark as Not Applicable
					Update ScrubReport Set [Status] = 99  Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @Cur_RuleID and ConStart = 1  
					---- Roll Back the Counted Failed Attempts
					Set @Failed = isnull(@Failed,0) - @RollBackFailCount
					Set @Passed = isnull(@Passed,0) + @RollBackFailCount
				End
			End

			--- Then AND OR Case to handle the Case of failed 
			If @PreviousRuleID <> @Cur_RuleID and @ThenOrCase =1
			Begin
				SET @ThenOrCase =0
				Set @RollBackFailCount = 0
				Select @RollBackFailCount = Count(1) from ScrubReport Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @PreviousRuleID and ConStart = 2  and [Status] = 0
				Set @RollBackFailCount = isnull(@RollBackFailCount,0)
				If @RollBackFailCount  > 0 
				Begin
					Update ScrubReport Set [Status] = 0  Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @PreviousRuleID 
					Set @Failed = isnull(@Failed,0)  - @RollBackFailCount
					Set @NotApplicable = isnull(@NotApplicable,0) + @RollBackFailCount
				END
			End

			If @PreviousRuleID <> @Cur_RuleID and @ThenOrCase =0 AND @OnlyORCase = 1
			Begin
				Declare @ISStatusCheck int = (Select Count(1) from ScrubReport Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @PreviousRuleID and [Status] = 0)
				SET @OnlyORCase =0
				Set @RollBackFailCount = 0
				Select @RollBackFailCount = Count(1) from ScrubReport Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @PreviousRuleID and [Status] = 1
				Set @RollBackFailCount = isnull(@RollBackFailCount,0)
				If @ISStatusCheck > 0 
				Begin
					Update ScrubReport Set [Status] = 0  Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @PreviousRuleID 
					Set @Failed = isnull(@Failed,0)  - @RollBackFailCount
					Set @NotApplicable = isnull(@NotApplicable,0) + @RollBackFailCount
				END
			End
			--If @Cur_RuleStepID in (1374,1375,1376)
			--Begin
				
			--END
	---- Check for IF Conditional Rules  --- ENDS

	--If @RuleCheckFlag = 1
	--Begin
		---- Call the Check Rule Procedure and retrive the Test results (0 = Passed, 1 = Failed, 99= Not Applicable
		-- Exec [dbo].[SPROC_ScrubCheckRule] @pBillHeaderID,@pExecutedBy,@ScrubHeaderID,@Cur_RuleID,@Cur_RuleStepID,@CorporateID, @FacilityID,@patientID,@EncounterID,@Status OUTPUT
		--print @Cur_RuleID 
		--print @Cur_RuleStepID
		Exec [dbo].[SPROC_ScrubCheckRule_NEW] @pBillHeaderID,@pExecutedBy,@ScrubHeaderID,@Cur_RuleID,@Cur_RuleStepID,@CorporateID, @FacilityID,@patientID,@EncounterID,@Status OUTPUT
	--End

				--If @Cur_RSConStart = 2 and (@Cur_RSConEnd = 0 OR @Cur_RSConEnd = 2) and @ConFTFlag = 0
				--Begin 
				--	Set @ConFTFlag = 0
				--	Set @RollBackFailCount = 0
				--	Select @RollBackFailCount = Count(1) from ScrubReport Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @Cur_RuleID and ConStart = 2 and (ConEnd = 0 or ConEnd = 2 ) and [Status] =1 
				--	Set @RollBackFailCount = isnull(@RollBackFailCount,0)
				--	If @RollBackFailCount  > 0 
				--	Begin
				--		Set @RuleCheckFlag = 0  --- Means IF part has one Condition which has failed - Stop further Check for this 
				--		Set @Status = 0
				--		---- Update If Conditions to Mark as Not Applicable
				--		Update ScrubReport Set [Status] = 99  Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @Cur_RuleID and ConStart = 2 and RuleStepid = @Cur_RuleStepID --and [RuleStepId]= @Cur_RuleStepID
				--		---- Roll Back the Counted Failed Attempts
				--		--Set @Failed = isnull(@Failed,0)  - @RollBackFailCount
				--		--Set @NotApplicable = isnull(@NotApplicable,0) + @RollBackFailCount
				--	End
				--End

				--If @Cur_RSConStart = 2 and (@PreviousCondition = 2)  and (@Status =0)
				--Begin 
				--	Set @ConFTFlag = 0
				--	Set @RollBackFailCount = 0
				--	Select @RollBackFailCount = Count(1) from ScrubReport Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @Cur_RuleID and ConStart = 2 and (ConEnd = 2 ) and [Status] =1 
				--	Set @RollBackFailCount = isnull(@RollBackFailCount,0)
				--	If @RollBackFailCount  > 0 
				--	Begin
				--		Set @Status = 0
				--		---- Update If Conditions to Mark as Not Applicable
				--		Update ScrubReport Set [Status] = 0  Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @Cur_RuleID and ConStart = 2  --and [RuleStepId]= @Cur_RuleStepID
				--		---- Roll Back the Counted Failed Attempts
				--		Set @Failed = isnull(@Failed,0)  - @RollBackFailCount
				--		Set @NotApplicable = isnull(@NotApplicable,0) + @RollBackFailCount
				--	END
				--END

				IF  @Cur_RSConStart = 2 and @Cur_RSConEnd = 2
				BEGIN
					SET @ThenOrCase = 1
				END 

				IF @Cur_RSConStart = 1 and @Cur_RSConEnd = 2
					Set @OnlyORCase = 1

				---- SET Counters - STARTS
				Set @Performed = isnull(@Performed,0) + 1;
		

				If @Status = 0 
					Set @Passed = isnull(@Passed,0) + 1
				If @Status = 1
					Set @Failed = isnull(@Failed,0) + 1
				If @Status = 99 OR @Status is NULL
					Set @NotApplicable = isnull(@NotApplicable,0) + 1
		---- SET Counters - ENDS
	

		Set @PreviousRuleID = @Cur_RuleID;
		Set @PreviousCondition = @Cur_RSConEnd

			FETCH NEXT FROM RuleToScrub INTO @Cur_CorporateID,@Cur_FacilityID,@Cur_RuleID,@Cur_RuleCode,@Cur_RuleType, @Cur_RuleStepID ,@Cur_RuleDescription,@Cur_RuleStepErrorID,@Cur_RSConStart,@Cur_RSConEnd,@Cur_RSConGroup,@Cur_RSRuleStepNumber;
			
			END 
			
			CLOSE RuleToScrub;  
			DEALLOCATE RuleToScrub;

			If @ThenOrCase =1
			Begin
				SET @ThenOrCase =0
				Set @RollBackFailCount = 0
				Select @RollBackFailCount = Count(1) from ScrubReport Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @PreviousRuleID and ConStart = 2  and ConEnd =2 and [Status] = 0 
				Set @RollBackFailCount = isnull(@RollBackFailCount,0)
				If @RollBackFailCount  > 0 
				Begin
					Update ScrubReport Set [Status] = 0  Where ScrubheaderID = @ScrubHeaderID AND RuleMasterID = @PreviousRuleID and ConStart = 2  
					Set @Failed = isnull(@Failed,0)  - @RollBackFailCount
					Set @NotApplicable = isnull(@NotApplicable,0) + @RollBackFailCount
				END
			End

		--- Update Results to Header
		---- NOTE Status on Scrub Header will be set as Priority (If any Error then Set to Error, Else if No Error then Passed)
		If @Passed > 0 
			Set @Status = 0
		--If @NotApplicable > 0 
		--	Set @Status = 99 
		If @Failed > 0 
			Set @Status = 1 
			
		--- If Exists (Select count(1) from ScrubReport Where ScrubHeaderID = @ScrubHeaderID and  [Status] = 1 and ConStart = 1)

		Update [dbo].[ScrubHeader] Set performed = @performed,Passed = @Passed, Failed = @Failed,NotApplicable = @NotApplicable, Status = @Status where ScrubHeaderID  = @ScrubHeaderID;

		--- Set Retrun Status
		Set @pRETStatus = @Status
END





GO


