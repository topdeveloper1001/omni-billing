IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocCheckAndCloseOPandEREncounters')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocCheckAndCloseOPandEREncounters
GO

/****** Object:  StoredProcedure [dbo].[SprocCheckAndCloseOPandEREncounters]    Script Date: 20-03-2018 17:32:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--SprocCheckAndCloseOPandEREncounters 9,'3001',8
CREATE PROCEDURE [dbo].[SprocCheckAndCloseOPandEREncounters]
(  
@pCorporateID int,  
@pFacilityNumber nvarchar(50),
@pFacilityID int
)
AS  

BEGIN
	DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

	DECLARE @SelectionDate datetime, @SystemOPTimeParam nvarchar(50), @SystemERTimeParam nvarchar(50),@CurrentDate varchar(50)

	Declare @TempEncounterIds As Table (EncounterID bigint,EncounterStartTime datetime,EType bigint)


	----- Set Defaults - STARTS

	----- Select below two Params from System Parameters Table
	Select @SystemOPTimeParam = Cast(DATEPART(hour, BSP.OupatientCloseBillsTime) as varchar(5)),@SystemERTimeParam = BSP.ERCloseBillsHours 
	FROM BillingSystemParameters BSP
	WHere BSP.CorporateId =@pCorporateID and BSP.FacilityNumber = @pFacilityNumber

	--- Default Setting
	Set @SystemOPTimeParam = ISNULL (@SystemOPTimeParam,'17')
	Set @SystemERTimeParam = ISNULL(@SystemERTimeParam,'24')	

	----- Set Defaults - ENDS


	Set @SelectionDate = DATEADD(DAY,1,@LocalDateTime)

	SET @CurrentDate = CAST(Cast(@LocalDateTime as DATE) as Varchar(20)) +' 00:00:00'

	SET @SystemOPTimeParam = DATEADD(hour,Cast(@SystemOPTimeParam as INT),Cast(@CurrentDate as datetime))
	

----------- OUT PATIENT  -- STARTS
	
	IF (Cast(@LocalDateTime as Time) > Cast(@SystemOPTimeParam as Time)) OR (CAST(@LocalDateTime as DATE) > CAST(@SystemOPTimeParam as date))
	BEGIN
			---- Encounter Which have Not ENDED
			INSERT into @TempEncounterIds
			Select E.EncounterID,E.EncounterStartTime,E.EncounterPatientType As EType
			from Encounter E
			Where E.CorporateID=@pCorporateID and E.EncounterFacility = @pFacilityID 
			AND E.EncounterPatientType='1'
			AND E.EncounterStartTime < @SelectionDate and E.EncounterEndTime is NULL 
			AND Cast(@LocalDateTime as Time) > Cast(@SystemOPTimeParam as Time)

			If @SystemOPTimeParam < @LocalDateTime
				Delete from @TempEncounterIds Where EncounterStartTime >= @LocalDateTime
	END

----------- OUT PATIENT  -- ENDS



----------- ER PATIENT  -- STARTS
	
	SET @SelectionDate=@LocalDateTime

	INSERT into @TempEncounterIds
	Select E.EncounterID,E.EncounterStartTime,E.EncounterPatientType
	from Encounter E
	Where E.CorporateID=@pCorporateID and E.EncounterFacility = @pFacilityID 
	AND E.EncounterPatientType ='3'
	AND E.EncounterStartTime < @SelectionDate and E.EncounterEndTime is NULL 
	AND DATEDIFF(Hour,E.EncounterStartTime,@LocalDateTime) > CAST(@SystemERTimeParam as INT) 

	Delete from @TempEncounterIds Where DATEDIFF(Hour,EncounterStartTime,@LocalDateTime) < CAST(@SystemERTimeParam as INT)

------------- ER PATIENT  -- ENDS

	--Select @LocalDateTime

	------ Close the OutPatient AND ER Encounters Automatically and Update IsAutoClosed Flag
	Update Encounter Set EncounterEndTime=@LocalDateTime,IsAutoClosed=1 WHERE EncounterID in (Select EncounterID from @TempEncounterIds);
	Update BillHeader SET Billdate = DATEADD(minute,1,@LocalDateTime) WHERE EncounterID in (Select EncounterID from @TempEncounterIds) and [Status] >= 40

	-- New Update to scrub the Encounter after autoclose the encounter  --- STARTS
	DECLARE cursorAutoClose CURSOR Fast_Forward FOR (Select EncounterID from @TempEncounterIds)
   
	DECLARE @EncounterId INT
	OPEN cursorAutoClose
	FETCH NEXT FROM cursorAutoClose INTO @EncounterId
   
	WHILE @@FETCH_STATUS = 0
	BEGIN
		/*
		This below SP i.e. SPROC_UpdateBillDate has been added to update the BillDate of the Current Encounter in the BillHeader Table once that Encounter Ends.
		BillDate is setting as EncounterEndTime plus 1.
		By Amit Jain on 07 March, 2016
		*/
		Exec SPROC_UpdateBillDate @EncounterId;

   		Exec [dbo].[SPROC_EncounterEndCheckBillEdit] @EncounterId,1

		FETCH NEXT FROM cursorAutoClose INTO @EncounterId
	END
   
	CLOSE cursorAutoClose   
	DEALLOCATE cursorAutoClose  
	---- New Update to scrub the Encounter after autoclose the encounter --- ENDS

	Delete from @TempEncounterIds
END



GO


