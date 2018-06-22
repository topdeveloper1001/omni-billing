IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_BedChargesNightly')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_BedChargesNightly
GO

/****** Object:  StoredProcedure [dbo].[SPROC_BedChargesNightly]    Script Date: 22-03-2018 19:27:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_BedChargesNightly] ---- [dbo].[SPROC_BedChargesNightly] NULL
(  
	@pComputedOn Datetime  --- Date on which this processed - Default to Todays date
)
AS
BEGIN

	Declare @CurrentDate datetime

	Select TOP 1 FacilityNumber From MappingPatientBed Where BedType IS NULL
------- Update BedType and FacilityStructureID in MPB will be removed once it is fixed from Front End - STARTS
		Update MappingPatientBed Set BedType = (Select BedType from UBedMaster where Bedid = BedNumber)
		,FacilityStructureID = (Select FacilityStructureID from UBedMaster where Bedid = BedNumber) 
		, Corporateid = (Select F.CorporateID from Facility F Where F.FacilityID = MappingPatientBed.FacilityNumber)
		Where BedType is NULL
------- Update BedType and FacilityStructureID in MPB will be removed once it is fixed from Front End - ENDS

Declare @Cur_MappingID int, @Cur_FacilityID int,@Cur_PatientID int,@Cur_EncounterID int,@Cur_BedID int,@Cur_BedType int,@Cur_OverrideBedType int, 
		@Cur_EffectiveDays int,@Cur_CorporateID int, @Cur_StartDate datetime, @Cur_EndDate datetime,@Cur_LastComputedOn datetime;

Declare @DayCount int, @TransactionDate datetime, @EffectiveRate numeric(18,2),@EffectiveBedType int, @RangeStart int, 
		@RangeEnd int,@LastRangeStartChecked int, @DaysToCalculate int, @BedChargeIDSelected bigint, @NewChargeInsertedFlag bit, @ServiceCodeValue nvarchar(20);

Set @CurrentDate = Cast(@pComputedOn as Date)

	Declare Cursor_MPB Cursor For
	Select MPB.MappingPatientBedId,MPB.FacilityNumber,MPB.PatientID,MPB.EncounterID,MPB.BedNumber, MPB.BedType,MPB.OverrideBedType, 
		DateDiff(DAY,MPB.StartDate,@CurrentDate) 'EffectiveDays',MPB.Corporateid,MPB.StartDate,MPB.EndDate,MPB.ChargesAppliedTillDate 
	from MappingPatientBed MPB
	Where    MPB.EndDate is NULL  -- and  MappingPatientBedId=1161
	order by MPB.FacilityNumber,MPB.PatientID,MPB.EncounterID;

Open Cursor_MPB;
Fetch Next From Cursor_MPB into @Cur_MappingID,@Cur_FacilityID,@Cur_PatientID,@Cur_EncounterID,@Cur_BedID,@Cur_BedType,@Cur_OverrideBedType,@Cur_EffectiveDays,@Cur_CorporateID,@Cur_StartDate,@Cur_EndDate,@Cur_LastComputedOn;

WHILE @@FETCH_STATUS = 0  
	BEGIN 
	Set @CurrentDate=(Select dbo.GetCurrentDatetimeByEntity(@Cur_FacilityID))	

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
				  3. CHanged the @CurrentDate with @pComputedOn parameter
			WHY: UnitType removed since earlier added this check in case of Bed Charges are put hourly. But currently, hourly rates 
			are not applied. So, Removed the same.
			Also, Added the IsDeleted and IsActive since there are some records in the 
			BedRateCard that have been Deleted or made in-active. 
			Therefore, those rates shouldn't be taken into consideration while applying.

			*/

			--#######################Changes starts here##########################

			--Select Top(1) @EffectiveRate = Rates, @RangeStart = DayStart, @RangeEnd = DayEnd, @ServiceCodeValue = ServiceCodeValue from BedRateCard 
			--Where BedTypes = @EffectiveBedType and UnitType = 3 and  DayStart  <= @Cur_EffectiveDays 
			--and @TransactionDate between EffectiveFrom and isnull(EffectiveTill,@CurrentDate)
			--order by EffectiveFrom,EffectiveTill,DayStart Desc;

			Select Top(1) @EffectiveRate = Rates, @RangeStart = DayStart, @RangeEnd = DayEnd, @ServiceCodeValue = ServiceCodeValue from BedRateCard 
			Where BedTypes = @EffectiveBedType 
			--and UnitType = 107 
			and  DayStart  <= @Cur_EffectiveDays 
			and @TransactionDate between EffectiveFrom and isnull(EffectiveTill,@pComputedOn)
			And ISNULL(IsDeleted,'0')=0 AND ISNULL(IsActive,'1')=1
			order by EffectiveFrom,EffectiveTill,DayStart Desc;
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
Declare Cursor_NewBedCharges Cursor For
Select Distinct(BCEncounterID) from BedCharges Where BCStatus = 0 ;

Open Cursor_NewBedCharges;
Fetch Next From Cursor_NewBedCharges into @Cur_EncounterID;

WHILE @@FETCH_STATUS = 0  
BEGIN  
	Exec [SPROC_ApplyBedChargesToBill] @Cur_EncounterID
	Fetch Next From Cursor_NewBedCharges into @Cur_EncounterID;
END

-- Clean Up
CLOSE Cursor_NewBedCharges;  
DEALLOCATE Cursor_NewBedCharges;

END














GO


