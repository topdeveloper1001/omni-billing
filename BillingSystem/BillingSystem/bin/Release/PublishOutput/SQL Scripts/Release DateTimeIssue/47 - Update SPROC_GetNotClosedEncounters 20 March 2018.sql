IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetNotClosedEncounters')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetNotClosedEncounters
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetNotClosedEncounters]    Script Date: 21-03-2018 10:40:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Shashank>
-- Create date: <Create Date,,2015-03-13>
-- Description:	<Description,,Purpose of this SP is to Close the ER-OP encounters after the time being elapsed as per the setting set in the Billing Parameter File>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetNotClosedEncounters]
(  
@pCorporateID int,  
@pFacilityNumber nvarchar(50),
@pFacilityID int
)
AS  

BEGIN
	DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

	DECLARE @SelectionDate datetime, @SystemOPTimeParam nvarchar(50), @SystemERTimeParam nvarchar(50)
	,@CurrentDate varchar(50)

	Select @SystemOPTimeParam = Cast(datepart(hour, BSP.OupatientCloseBillsTime) as varchar(5))
	FROM BillingSystemParameters BSP
	WHere BSP.CorporateId =@pCorporateID and BSP.FacilityNumber = @pFacilityNumber

	Select @SystemERTimeParam = BSP.ERCloseBillsHours from BillingSystemParameters BSP
	WHere BSP.CorporateId =@pCorporateID and BSP.FacilityNumber = @pFacilityNumber

	--- Default Setting
	Set @SystemOPTimeParam = ISNULL (@SystemOPTimeParam,'17')
	Set @SystemERTimeParam = ISNULL(@SystemERTimeParam,'24')

----- Set Defaults - ENDS
	Set @SelectionDate = DATEADD(DAY,1,@LocalDateTime)

SET @CurrentDate = CAST(Cast(@LocalDateTime as DATE) as Varchar(20)) +' 00:00:00'
SET @SystemOPTimeParam = DATEADD(hour,Cast(@SystemOPTimeParam as INT),Cast(@CurrentDate as datetime))

--Select Cast(@CurrentDate as Time),Cast(@SystemOPTimeParam as Time),@SelectionDate;

------- Drop table #T1

Create table #T1
(PatientIsVIP nvarchar(40), FirstName nvarchar(30),LastName nvarchar(30),BirthDate datetime, 
PersonEmiratesIDNumber nvarchar(30),EncounterNumber nvarchar(50), EncounterStartTime datetime,EncounterPatientType int,ErrorStatus nvarchar(20),EncounterID int,PatientID int)

----------- OUT PATIENT  -- STARTS
IF Cast(@LocalDateTime as Time) > Cast(@SystemOPTimeParam as Time)
BEGIN
---- Encounter Which have Not ENDED
	insert into #T1
		Select P.PersonVIP 'IsVIP',P.PersonFirstName 'FirstName', P.PersonLastName 'LastName', P.PersonBirthDate 'BirthDate', 
		E.EncounterNumber 'EncounterNumber',P.PersonEmiratesIDNumber 'EmirateID' ,E.EncounterStartTime 'EncounterStart', E.EncounterPatientType,'E',E.EncounterID, E.PatientID    
		from Encounter E
		inner join Patientinfo P on P.PatientID = E.PatientID
		Where E.CorporateID = @pCorporateID and E.EncounterFacility = @pFacilityID and E.EncounterPatientType = '3' and E.EncounterStartTime < @SelectionDate and E.EncounterEndTime is NULL 

------ Same Area Select Any Encounter Which has Open orders
--	insert into #T1
--		Select P.PersonVIP 'IsVIP',P.PersonFirstName 'FirstName', P.PersonLastName 'LastName', P.PersonBirthDate 'BirthDate', 
--		E.EncounterNumber 'EncounterNumber',P.PersonEmiratesIDNumber 'EmirateID' ,E.EncounterStartTime 'EncounterStart', E.EncounterPatientType,'O',E.EncounterID, E.PatientID   from Encounter E
--		inner join Patientinfo P on P.PatientID = E.PatientID
--		inner join OpenOrder OO on OO.EncounterID = E.EncounterID and OO.CorporateID = @pCorporateID and OO.FacilityID = @pFacilityNumber and OrderStatus < 3
--		Where E.CorporateID = @pCorporateID and E.EncounterFacility = @pFacilityNumber and E.EncounterPatientType = '3' and E.EncounterStartTime < @SelectionDate
-- -- Where E.EncounterStartTime < @SelectionDate 

 --- Check if Datetime Set for Today (System Param Table for OutPatient) has not reached the set time then remove Today's Encounter
--- Setting to Previous Mid - Night and Then Adding time from Paramater Tablew  
--Set @SelectionDate = DATEADD(DAY,-1,DATEADD(DAY,-1,Cast(@CurrentDate as datetime)));

--Set @SelectionDate = @SelectionDate + Cast(@SystemOPTimeParam as Time);

 If @SystemOPTimeParam < @LocalDateTime
	Delete from #T1 Where EncounterStartTime >= @LocalDateTime

---- Close the OutPatient Encounters Automatically and Update IsAutoClosed Flag
Update Encounter Set EncounterEndTime =@LocalDateTime,IsAutoClosed=1  where EncounterID in (Select EncounterID from #T1);
Update BillHeader set Billdate = dateadd(minute,1,@LocalDateTime) where EncounterID in (Select EncounterID from #T1) and Status >= 40




-- New Update to scrub the Encounter after autoclose the encounter  --- STARTS
   DECLARE cursorAutoCloseOP CURSOR fast_forward FOR  
   	(Select EncounterID from #T1)
   
   DECLARE @EncounterIDOP INT
   OPEN cursorAutoCloseOP   
   FETCH NEXT FROM cursorAutoCloseOP INTO @EncounterIDOP
   
   WHILE @@FETCH_STATUS = 0   
   BEGIN
		/*
		This below SP i.e. SPROC_UpdateBillDate has been added to update the BillDate of the Current Encounter in the BillHeader Table once that Encounter Ends.
		BillDate is setting as EncounterEndTime plus 1.
		By Amit Jain on 07 March, 2016
		*/
		Exec SPROC_UpdateBillDate @EncounterIDOP;

   	   Exec [dbo].[SPROC_EncounterEndCheckBillEdit] @EncounterIDOP,1
       FETCH NEXT FROM cursorAutoCloseOP INTO @EncounterIDOP
   END   
   
   CLOSE cursorAutoCloseOP   
   DEALLOCATE cursorAutoCloseOP  
---- New Update to scrub the Encounter after autoclose the encounter --- ENDS

Delete from #T1
END

----------- OUT PATIENT  -- ENDS

----------- ER PATIENT  -- STARTS

Set @SelectionDate = @LocalDateTime

---- Encounter Which have Not ENDED
	insert into #T1
		Select P.PersonVIP 'IsVIP',P.PersonFirstName 'FirstName', P.PersonLastName 'LastName', P.PersonBirthDate 'BirthDate', 
		E.EncounterNumber 'EncounterNumber',P.PersonEmiratesIDNumber 'EmirateID' ,E.EncounterStartTime 'EncounterStart', E.EncounterPatientType,'E',E.EncounterID, E.PatientID   
		from Encounter E
		inner join Patientinfo P on P.PatientID = E.PatientID
		Where E.CorporateID = @pCorporateID and E.EncounterFacility = @pFacilityID and E.EncounterPatientType = '1' 
		and E.EncounterStartTime < @SelectionDate and DATEDIFF(Hour,E.EncounterStartTime,@LocalDateTime) > CAST(@SystemERTimeParam as INT) and E.EncounterEndTime is NULL 
-- Where E.EncounterStartTime < @SelectionDate and E.EncounterEndTime is NULL

------ Same Area Select Any Encounter Which has Open orders
--	insert into #T1
--		Select P.PersonVIP 'IsVIP',P.PersonFirstName 'FirstName', P.PersonLastName 'LastName', P.PersonBirthDate 'BirthDate', 
--		E.EncounterNumber 'EncounterNumber',P.PersonEmiratesIDNumber 'EmirateID' ,E.EncounterStartTime 'EncounterStart', E.EncounterPatientType,'O',E.EncounterID, E.PatientID   from Encounter E
--		inner join Patientinfo P on P.PatientID = E.PatientID
--		inner join OpenOrder OO on OO.EncounterID = E.EncounterID and OO.CorporateID = @pCorporateID and OO.FacilityID = @pFacilityNumber and OrderStatus < 3
--		Where E.CorporateID = @pCorporateID and E.EncounterFacility = @pFacilityNumber and E.EncounterPatientType = '1' and E.EncounterStartTime < @SelectionDate
--		and DATEDIFF(Hour,E.EncounterStartTime,@CurrentDate) > CAST(@SystemERTimeParam as INT)
-- -- Where E.EncounterStartTime < @SelectionDate 


--- Check if Datetime Set for Today (System Param Table for OutPatient) has not reached the set time then remove Today's Encounter
--- Setting to Previous Mid - Night and Then Adding time from Paramater Tablew  
--Set @SelectionDate = DATEADD(DAY,-1,DATEADD(DAY,-1,Cast(@CurrentDate as datetime)))

--Set @SelectionDate = DATEADD(HH,-(CAST(@SystemERTimeParam as INT)),@SelectionDate)

 --If @SelectionDate > @CurrentDate
Delete from #T1 Where DATEDIFF(Hour,EncounterStartTime,@LocalDateTime) < CAST(@SystemERTimeParam as INT)
--EncounterStartTime  < @SelectionDate and DATEDIFF(Hour,EncounterStartTime,@CurrentDate) > CAST(@SystemERTimeParam as INT)

---- Close the OutPatient Encounters Automatically and Update IsAutoClosed Flag
Update Encounter Set EncounterEndTime = @LocalDateTime,IsAutoClosed=1  where EncounterID in (Select EncounterID from #T1);
Update BillHeader set Billdate = dateadd(minute,1,@LocalDateTime) where EncounterID in (Select EncounterID from #T1) and Status >= 40
-- New Update to scrub the Encounter after autoclose the encounter  --- STARTS
   DECLARE cursorERAutoClose CURSOR fast_forward FOR  
   	(Select EncounterID from #T1)
   
   DECLARE @EncounterIDER INT
   OPEN cursorERAutoClose   
   FETCH NEXT FROM cursorERAutoClose INTO @EncounterIDER
   
   WHILE @@FETCH_STATUS = 0   
   BEGIN   
   	   Exec [dbo].[SPROC_EncounterEndCheckBillEdit] @EncounterIDER,1
          FETCH NEXT FROM cursorERAutoClose INTO @EncounterIDER
   END   
   
   CLOSE cursorERAutoClose   
   DEALLOCATE cursorERAutoClose  
---- New Update to scrub the Encounter after autoclose the encounter --- ENDS
Delete from #T1
------------- ER PATIENT  -- ENDS

---- Return Results
-- Select * from #T1
--Select max(PatientIsVIP) 'PatientIsVIP', max(FirstName) 'FirstName',max(LastName) 'LastName',max(BirthDate) 'BirthDate', max(PersonEmiratesIDNumber) 'PersonEmiratesIDNumber',
--max(EncounterNumber) 'EncounterNumber', max(EncounterStartTime) 'EncounterStartTime',max(EncounterPatientType) 'EncounterPatientType',
--ErrorStatus,EncounterID,PatientID
--from #T1
--Group by PatientID,EncounterID,ErrorStatus

END



GO


