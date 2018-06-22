IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_BedChargesNightlyPerEncounter')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_BedChargesNightlyPerEncounter
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SPROC_BedChargesNightlyPerEncounter]    Script Date: 3/22/2018 7:56:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_BedChargesNightlyPerEncounter]  ---- [SPROC_BedChargesNightlyPerEncounter] null,2515
(  
	@pComputedOn Datetime = null,  --- Date on which this processed - Default to Todays date
	@pEncounterID int = null
)
AS
BEGIN
	
	Declare   @CurrentLocalDateTime DateTime = (Select dbo.GetCurrentDatetimeByEntity(0))

	Set @pComputedOn = @CurrentLocalDateTime

--USE [BillingSystem6OCT2014_QA]
--Alter table MappingPatientBed
--Add FacilityStructureID int NULL

----USE BillingSystem6OCT2014_QA
----Alter table MappingPatientBed
----Add ChargesAppliedTillDate Datetime NULL


------- Update BedType and FacilityStructureID in MPB will be removed once it is fixed from Front End - STARTS
		Update MappingPatientBed Set BedType = (Select BedType from UBedMaster where Bedid = BedNumber)
		,FacilityStructureID = (Select FacilityStructureID from UBedMaster where Bedid = BedNumber) 
		, Corporateid = (Select F.CorporateID from Facility F Where F.FacilityID = MappingPatientBed.FacilityNumber)
		Where BedType is NULL
------- Update BedType and FacilityStructureID in MPB will be removed once it is fixed from Front End - ENDS

Declare @Cur_MappingID int, @Cur_FacilityID int,@Cur_PatientID int,@Cur_EncounterID int,@Cur_BedID int,@Cur_BedType int,@Cur_OverrideBedType int, 
		@Cur_EffectiveDays int,@Cur_CorporateID int, @Cur_StartDate datetime, @Cur_EndDate datetime,@Cur_LastComputedOn datetime;

Declare @CurrentDate datetime, @DayCount int, @TransactionDate datetime, @EffectiveRate numeric(18,2),@EffectiveBedType int, @RangeStart int, 
		@RangeEnd int,@LastRangeStartChecked int, @DaysToCalculate int, @BedChargeIDSelected bigint, @NewChargeInsertedFlag bit, @ServiceCodeValue nvarchar(20);

Set @CurrentDate = Cast(@pComputedOn as Date)

Declare Cursor_MPB Cursor For
	Select MPB.MappingPatientBedId,MPB.FacilityNumber,MPB.PatientID,MPB.EncounterID,MPB.BedNumber, MPB.BedType,MPB.OverrideBedType, 
		DateDiff(DAY,MPB.StartDate,@CurrentDate) 'EffectiveDays',MPB.Corporateid,MPB.StartDate,MPB.EndDate,MPB.ChargesAppliedTillDate 
	from MappingPatientBed MPB
	Where    MPB.EndDate is NULL and MPB.EncounterID = @pEncounterID ---- MappingPatientBedId=1066
	order by MPB.FacilityNumber,MPB.PatientID,MPB.EncounterID;

Open Cursor_MPB;
Fetch Next From Cursor_MPB into @Cur_MappingID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,@Cur_BedID,@Cur_BedType,@Cur_OverrideBedType,@Cur_EffectiveDays,@Cur_CorporateID,@Cur_StartDate,@Cur_EndDate,@Cur_LastComputedOn;

WHILE @@FETCH_STATUS = 0  
	BEGIN 

	Set @Cur_LastComputedOn = isnull(@Cur_LastComputedOn,@Cur_StartDate)
	Set @DaysToCalculate = DateDiff(DAY,@Cur_LastComputedOn,@CurrentDate)
	Set @NewChargeInsertedFlag = 0
	Set @LastRangeStartChecked = 0
	If isnull(@DaysToCalculate,0) > 0
	Begin
		Set @DayCount = 1
		Set @TransactionDate = DATEADD(DAY,(@DaysToCalculate * -1),@CurrentDate)
		Set @EffectiveBedType = CASE WHEN @Cur_OverrideBedType is not NULL AND @Cur_OverrideBedType > 0 THEN @Cur_OverrideBedType ELSE @Cur_BedType END
		
		WHILE @DayCount <= @DaysToCalculate
		Begin
		
		
			----- Get Effective Range and Rate based on BedType and Days
			Set @EffectiveRate = 0


			/*

			Who: Amit Jain
			When: 08 April, 2016
			What: 1. Removed the check of Unit Type,
				  2. Added the check of IsDeleted and IsActive
				  3. CHanged the getdate() with @pComputedOn parameter
			WHY: UnitType removed since earlier added this check in case of Bed Charges are put hourly. But currently, hourly rates 
			are not applied. So, Removed the same.
			Also, Added the IsDeleted and IsActive since there are some records in the 
			BedRateCard that have been Deleted or made in-active. Therefore, those rates shouldn't be come while applying.

			*/

			--#######################Changes starts here##########################

			--Select Top(1) @EffectiveRate = Rates, @RangeStart = DayStart, @RangeEnd = DayEnd, @ServiceCodeValue = ServiceCodeValue from BedRateCard 
			--Where BedTypes = @EffectiveBedType and UnitType = 107 and  DayStart  <= @Cur_EffectiveDays 
			--and @TransactionDate between EffectiveFrom and isnull(EffectiveTill,getdate())
			--order by DayStart Desc;

			Select Top(1) @EffectiveRate = Rates, @RangeStart = DayStart, @RangeEnd = DayEnd, @ServiceCodeValue = ServiceCodeValue from BedRateCard 
			Where BedTypes = @EffectiveBedType 
			--and UnitType = 107 
			and  DayStart  <= @Cur_EffectiveDays 
			and @TransactionDate between EffectiveFrom and isnull(EffectiveTill,@pComputedOn)
			And ISNULL(IsDeleted,'0') = 0 AND ISNULL(IsActive,'1')=1
			order by DayStart Desc;
			--#######################Changes ends here##########################

			Set @EffectiveRate = isnull(@EffectiveRate,0)

		-------->>>>>>>>>>>>>>   GROUP as Per Range Set in Rate Card <<<<<<<<<<<<<<<<<<<<<< ----------
		--------- For Performance Following check is applied
		----If @LastRangeStartChecked <> @RangeStart
		----Begin
		----	---- Check if Range is Already in BedCharges Table
		----	Select @BedChargeIDSelected=BedChargesID from BedCharges Where BCPatientID = @Cur_PatientID and BCEncounterID = @Cur_EncounterID and BCBedID = @Cur_BedID and BCRangeStart = @RangeStart
		----	If @@ROWCOUNT <= 0
		----	Begin
		----		Insert into BedCharges (BCCorporateID, BCFacilityID, BCPatientID, BCEncounterID, BCBedID, BCMappingBedPatientID, BCRangeStart, BCRangeEnd, BCBedRateTypeID,
		----				BCTransactionDate,BCRangeEffectiveDays, BCUnitRate, BCGross, BCActivityStartDate, BCActivityEndDate, BCTotalEffectiveDays, BCStatus, 
		----				BCCreatedBy, BCCreatedDate, BCModifiedBy, BCModifiedDate, BCIsActive)
		----		Select @Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,@Cur_BedID,@Cur_MappingID,@RangeStart,@RangeEnd,@EffectiveBedType,@TransactionDate,
		----				(@RangeEnd-@RangeStart+1),@EffectiveRate,@EffectiveRate,@Cur_StartDate,@Cur_EndDate,@Cur_EffectiveDays,0,1,@CurrentDate,NULL,NULL,1

			
		----		Set @LastRangeStartChecked = @RangeStart
		----		Set @NewChargeInsertedFlag = 1
				
		----	End
		----End --- Performance Check Ends Here
		-------->>>>>>>>>>>>>>   GROUP as Per Range Set in Rate Card <<<<<<<<<<<<<<<<<<<<<< ----------

		------ >>>>>>>>>>>>>>   EVERY DAY with Rates as Per Rate Card then Comment above Section and Use below Set of coding <<<<<<<<<<<<<<<<<<<<<< ----------
				Insert into BedCharges (BCCorporateID, BCFacilityID, BCPatientID, BCEncounterID, BCBedID, BCMappingBedPatientID, BCRangeStart, BCRangeEnd, BCBedRateTypeID,
						BCTransactionDate,BCRangeEffectiveDays, BCUnitRate, BCGross, BCActivityStartDate, BCActivityEndDate, BCTotalEffectiveDays, BCStatus, 
						BCCreatedBy, BCCreatedDate, BCModifiedBy, BCModifiedDate, BCIsActive,ServiceCodeValue)
				Select @Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,@Cur_BedID,@Cur_MappingID,@RangeStart,@RangeEnd,@EffectiveBedType,@TransactionDate,
						1,@EffectiveRate,@EffectiveRate,@Cur_StartDate,@TransactionDate,@Cur_EffectiveDays,0,1,@CurrentDate,NULL,NULL,1,@ServiceCodeValue
		------ >>>>>>>>>>>>>>   EVERY DAY with Rates as Per Rate Card then Comment above Section and Use below Set of coding <<<<<<<<<<<<<<<<<<<<<< ----------
			
				Set @DayCount = @DayCount + 1
				Set @TransactionDate = DATEADD(DAY,1,@TransactionDate)

		End --- While DayCount Ends Here
	End --- Check for Effective Day> 0 Ends here

	----- Update Calculated Date in Mapping Table for Performance for next time Calculations from
		Update MappingPatientBed Set ChargesAppliedTillDate = @CurrentDate	Where MappingPatientBedId = @Cur_MappingID;


	Fetch Next From Cursor_MPB into @Cur_MappingID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,@Cur_BedID,@Cur_BedType,@Cur_OverrideBedType,@Cur_EffectiveDays,@Cur_CorporateID,@Cur_StartDate,@Cur_EndDate,@Cur_LastComputedOn;
	END

-- Clean Up
CLOSE Cursor_MPB;  
DEALLOCATE Cursor_MPB;

---- Now Call PROC ApplyBedChargesToBill if New Charges are there 
----- (This Must be done here So it runs independtly of above logic for every night --> just in case missed one night it will be picked up in next cycle)
Exec [SPROC_ApplyBedChargesToBill] @pEncounterID


END














GO


