IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplyMCContractToBillActivity')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplyMCContractToBillActivity
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SPROC_ApplyMCContractToBillActivity]    Script Date: 3/22/2018 8:11:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<BB>
-- Create date: <Jan-2015>
-- Description:	<This is to get Managed Care Contract details and Apply to Detail lines while taking them to BillActivity>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_ApplyMCContractToBillActivity]
(
	@pPatientID int, @pEncounterID int, @pOrderType nvarchar(20), @pOrderCode nvarchar(20)
	,@pGross numeric(18,2) output,@poPatientShare numeric(18,2) output,@poPayerNETShare numeric(18,2) output
	,@poAAPShare numeric(18,2) output, @poMCDiscount numeric(18,2) output,
	@poIsMCContractBaseRateApplied bit output,@pSelfPayFlag bit,@poDRGCode nvarchar(20) output
)
AS
BEGIN

Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))
Declare @PolicyBeginDate datetime, @PolicyEndDate datetime, @EncounterType nvarchar(20)
,@DaysSincePolicyStarted int,
		@Cur_Percent int, @Cur_Fixed int, @Cur_Capping int,@Cur_Multiplier numeric(10,4),@Cur_MCOrderCode nvarchar(20), @Cur_MCLevel nvarchar(20), @Cur_OrderType  nvarchar(20),
		@AAPShare numeric(18,2),@INSPolicyId int, @Remarks nvarchar(500) ='>>',@poMCContractAppliedID int, @poMCContractCode nvarchar(20),
		@MCExpectedFixed numeric(18,2),@MCPercent numeric(18,2),@MCDiscount numeric(18,2),@MCIPDeduct numeric(18,2),@MCOPDeduct numeric(18,2),@MCAddOn numeric(18,2),
		@MCMultiplierOP numeric(5,2),@MCMultiplierER numeric(5,2),@MCMultiplierOthers numeric(5,2),@MCIPBaseRate numeric(18,2), @DRGCodeID int,@MCPerDiem numeric(18,2),
		@IPServiceCode nvarchar(10),@OriginalCostPassedIn numeric(18,2);

Declare @IsInpatientBedrateApplied bit, @IsLongTermPerDiemApplicable int;
--------  GET ALL NEEDED information for a Selection --- STARTS
---- Get MCContractID or CODE - Policy Start and End Date
	
--------- >>>>>>>>>>>>>>>   XXXXXXXXXXXXXXXXX   BB - As per Client needed New Logic Added on 8-Mar-2016  -- New Columns Added  ActivityCost , UnitCost
------- Keeping the passed in Cost so Managed Care Discount could be calculated at the End
Set @OriginalCostPassedIn = @pGross

Select @INSPolicyId = InsurancePolicyId from PatientInsurance Where PatientID = @pPatientID and IsActive = 1
Select @poMCContractCode= McContractCode  from InsurancePolices where InsurancePolicyId = @INSPolicyId


--Set @DaysSincePolicyStarted = DATEDIFF(DAY,@PolicyBeginDate,Getdate())
Select @EncounterType = Cast(EncounterpatientType as nvarchar(20)) from Encounter Where EncounterID = @pEncounterID

Set @Cur_MCOrderCode = isnull(@pOrderCode,0)
--------  GET ALL NEEDED information for a Selection --- ENDS
------ RULE Apply STARTS
--- @MCAddon = MCAddon (Not Needed in Following)

---- Get Rules as Set based on Params passed (Note it is Sorted by Weightage so highest weightage will be picked
--- @Cur_Multiplier = isnull(MCMultiplier,0), 
Select Top(1) @Cur_Percent=isnull(MCPatientPercent,0), @Cur_Fixed= isnull(MCPatientFixed,0), @Cur_Capping = isnull(MCPatientCapping,0), 
@Cur_MCLevel = MCLevel, @Cur_OrderType = MCOrderType, @poMCContractAppliedID = MCContractID,@poMCContractCode = MCCode,
@MCExpectedFixed = MCExpectedFixedrate, @MCPercent = MCExpectedPercentage,@MCMultiplierOP = MCMultiplierOutpatient,
@MCMultiplierER = MCMultiplierEmergencyRoom, @MCMultiplierOthers = MCMultiplierOther,@MCIPDeduct = MCInpatientDeduct,@MCOPDeduct = MCOutpatientDeduct,
@MCIPBaseRate = MCInpatientBaseRate from MCContract 
where MCCode = @poMCContractCode 


SET @poDRGCode = ISNULL(@poDRGCode,'0')
Set @MCIPBaseRate = isnull(@MCIPBaseRate,0)
Set @Cur_Multiplier =isnull(@Cur_Multiplier,1) 
---- Check for Any exceptions for incoming Order-Code
	 Select Top(1) @MCPerDiem=OrderCodePerDiemRate,@MCAddOn=OrderCodeAddons from MCOrderCodeRates where ordercode = @pOrderCode and MCCode = @poMCContractCode
		Set @MCPerDiem = isnull(@MCPerDiem,0)
		Set @MCAddOn = isnull(@MCAddOn,0)
		SET @IsLongTermPerDiemApplicable = 0;
---- Setting Multplier as per Setup
If @EncounterType = 2
Begin
	Set @poIsMCContractBaseRateApplied = (Select  TOP(1) IsMCContractBaseRateApplied from Encounter Where PatientID = @pPatientID and EncounterID = @pEncounterID);
    Set @poIsMCContractBaseRateApplied = ISNULL(@poIsMCContractBaseRateApplied,0);

	---- For CPT,DRUG and etc (Except) ServiceCode Bed Transactions --- Use Gross for Expected payment Calculations
		  SET @IPServiceCode = (Select [dbo].[GetLongTermPatientPerDiemApplicable] (@pEncounterID,@poMCContractCode));
		---- BB - 2015-Jun-15 --- Following Function is commented because no need all params needed to count are already here --- STARTS
		-- SET @IsLongTermPerDiemApplicable = (Select [dbo].[GetLongTermPatientPerDiemApplicable] (@pEncounterID,@poMCContractCode)); 
		Select Top(1) @IsLongTermPerDiemApplicable = Count(*) from MCOrderCodeRates where MCCode = @poMCContractCode and  OrderCode = @IPServiceCode; 
			--Select Top(1) @IsLongTermPerDiemApplicable = Count(*) from MCOrderCodeRates where MCCode = @poMCContractCode and  OrderCode = @pOrderCode;
		---- BB - 2015-Jun-15 --- Following Function is commented because no need all params needed to count are already here --- ENDS
	--- For In Patient Multiplier not be used so set to 1
	---- Set @Cur_Multiplier = isnull(@MCMultiplierOthers,1)
	--Set @Cur_Multiplier = 1 -- Commented on 03/08/2016 by SHASHANK
	Set @MCExpectedFixed = isnull(@MCIPDeduct,0)
	
	
	--If @pOrderType = 8 
	--Begin 
		If @MCIPBaseRate <= 0
			Set @MCIPBaseRate = @pGross
			
	--Select @IsInpatientBedrateApplied = TOP(1) from BillActivity where patientId = @pPatientID and Encounterid = @pEncounterID and ActivityType= 8

	---- Check for Any exceptions for incoming Order-Code
		Select Top(1) @MCPerDiem=OrderCodePerDiemRate,@MCAddOn=OrderCodeAddons from MCOrderCodeRates where ordercode = @IPServiceCode and MCCode = @poMCContractCode
		Set @MCPerDiem = isnull(@MCPerDiem,0)
		If @MCPerDiem = 0 
		Begin   ----- No Exception Get Multiplier 
			Select @DRGCodeID = DRGCodeID from Diagnosis Where EncounterID = @pEncounterID and DiagnosisType = 3
			Set @DRGCodeID = isnull(@DRGCodeID,0)
			If @DRGCodeID > 0
				Begin
					--SET @poDRGCode = @DRGCodeID
					IF  @pSelfPayFlag = 1 
					BEGIN 
						IF @pOrderType = '9'
							Set @Cur_Multiplier = 0
						ELSE
							Set @Cur_Multiplier = 1
					END
					ELSE
					BEGIN
						--Select @Cur_Multiplier=Cast(CodeDRGWeight as numeric(5,4)),@poDRGCode = CodeNumbering from DRGCodes Where DRGCodesId = @DRGCodeID
						Select @Cur_Multiplier=FORMAT(CAST(CodeDRGWeight as NUmeric(18,4)), 'N4'),@poDRGCode = CodeNumbering from DRGCodes Where DRGCodesId = @DRGCodeID
						Set @Cur_Multiplier = isnull(@Cur_Multiplier,1)
					END
					--- BB - 2015-Jun-15 ---- DOUBTFUL --- Commented Below but need to confirm later with Ken --- STARTS
					--If @poIsMCContractBaseRateApplied = 1
					--	Set @Cur_Multiplier = 0
					--- BB - 2015-Jun-15 ---- DOUBTFUL --- Commented Below but need to confirm later with Ken --- ENDS

					Set @poIsMCContractBaseRateApplied = 1;

					--- As Per Spec Multiplier should not be used at all for Bed Transactions (per Diem or not)
					----If @Cur_Multiplier = 0
					----	Set @Cur_Multiplier = isnull(@MCMultiplierOthers,1)
				End
			ELSE
				Begin
					Set @MCIPBaseRate = @pGross
					Set @Cur_Multiplier = ISNULL(@Cur_Multiplier,1)
				End
			END
		--End
		ELSE
		BEGIN
			Set @MCIPBaseRate = @MCPerDiem
		END
	--End
	
End
If @EncounterType = 1
Begin
	Set @Cur_Multiplier = isnull(@MCMultiplierER,1)
	Set @MCExpectedFixed = isnull(@MCOPDeduct,0)
	Set @MCIPBaseRate = @pGross
End
If @EncounterType = 3
Begin
	Set @Cur_Multiplier = isnull(@MCMultiplierOP,1)
	Set @MCExpectedFixed = isnull(@MCOPDeduct,0)
	Set @MCIPBaseRate = @pGross
End
----- New Logic (----> Get Present AAPShare (AlreadyAppliedPatientShare) --- STARTS

Select @AAPShare= ISNULL(Sum(PatientShare),0) from BillActivity Where PatientID = @pPatientID and EncounterID = @pEncounterID
Set @AAPShare = isnull(@AAPShare,0)
Set @MCIPBaseRate = isnull(@MCIPBaseRate,0)

Set @poPatientShare = 0
Set @poPayerNETShare = 0 
Set @poMCDiscount = 0 
Set @poIsMCContractBaseRateApplied = @poIsMCContractBaseRateApplied
--- Gross not to be changed -- 15-Feb-2015
--- Set @pGross = @pGross*@Cur_Multiplier
Set @MCIPBaseRate = ISNULL(@MCIPBaseRate,0)* ISNULL(@Cur_Multiplier,0)


---- If there is AddON  --> this must be below After Multiplier has taken Effect
	Set @MCIPBaseRate = ISNULL(@MCIPBaseRate,0) + ISNULL(@MCAddOn,0)
	--- Set @pGross = @pGross + @MCAddOn
	
If @AAPShare < @MCExpectedFixed
Begin
	If (@MCIPBaseRate + @AAPShare) <= ISNULL(@MCExpectedFixed,0)
		Set @poPatientShare = @MCIPBaseRate 
		--if (@MCIPBaseRate - @AAPShare) > 0
		--	Set @poPatientShare = @MCIPBaseRate - @AAPShare
		--Else
		--	If(@AAPShare+@poPatientShare) <= @MCExpectedFixed
		--		Set @poPatientShare = @MCIPBaseRate 
		--	Else
		--		Set @poPatientShare = @MCExpectedFixed - @AAPShare
	ELSE
		Set @poPatientShare = @MCExpectedFixed - @AAPShare
End
ELSE
Begin
	Set @poPatientShare = 0
End
If (@AAPShare+@poPatientShare) >= ISNULL(@MCExpectedFixed,0)
Begin
	If isnull(@MCPercent,0) > 0 
		Set @poPayerNETShare = ((@MCIPBaseRate * (@MCPercent / 100.0))- @poPatientShare) 
	Else
		Set @poPayerNETShare = @MCIPBaseRate - @poPatientShare
End
ELSE
Begin
	Set @poPayerNETShare =0
End

-- Check added by Shashank to fetch that person is in long term and perdiem is applicable to it
If(@IsLongTermPerDiemApplicable <> 0 and @pOrderType <> 8)
BEGIN
	Set @poPayerNETShare = 0;
	Set @poPatientShare = 0;
END

--------- >>>>>>>>>>>>>>>   XXXXXXXXXXXXXXXXX   BB - As per Client needed New Logic Added on 8-Mar-2016  -- New Columns Added  ActivityCost , UnitCost
------------------------------- NOTE: COMMENTED BELOW --- >> Gross = PatientShare + PayorNET and Placed at the End of of PROC
---- >>>>>>>>>   COREECTION on Gross Charges in Case of Out Patient and Emergency   ELSE for InPatient Case DO NOT TOUCH the Gross Charges --- STARTS
---- BB --- 2-Dec-2015 ---- Checked Again the Flow --- For Put Patient and in Emergency Case ---- Gross should be Multiplied on Indivdual Line of Activity --- STARTS
	---- ---- If @EncounterType <> 2 Set @pGross = @MCIPBaseRate
---- BB --- 2-Dec-2015 ---- Checked Again the Flow --- For Put Patient and in Emergency Case ---- Gross should be Multiplied on Indivdual Line of Activity --- ENDS
---- >>>>>>>>>   COREECTION on Gross Charges in Case of Out Patient and Emergency   ELSE for InPatient Case DO NOT TOUCH the Gross Charges --- ENDS

---- BB--> MC Discount can be Negative in some cases and let it be - Confirmed on 15-Feb-2015
-- if (@pGross - @poPayerNETShare - @poPatientShare) > 0 
--------- >>>>>>>>>>>>>>>   XXXXXXXXXXXXXXXXX   BB - As per Client needed New Logic Added on 8-Mar-2016 
----------------------->>>> Commented below as placed in the End to Calculate ManagedCareDiscount MCDiscount
	--- --- Set @MCDiscount = @pGross - @poPayerNETShare - @poPatientShare
-- Else
--	Set @MCDiscount = 0

---- Below set for Out going params
Set @poMCDiscount = @MCDiscount
Set @poAAPShare = @AAPShare
--- Self pay flag check
IF @pSelfPayFlag = 1
BEGIN
--SET @poPatientShare =  @poPayerNETShare
 SET @poPatientShare = @poPatientShare + @poPayerNETShare;-- Sum up the both Shares for Patient share if it is Self pay
 SET @poPatientShare = @poPatientShare *  (CASE WHEN  @pOrderType = '9' THEN 0 ELSE CASE WHEN @EncounterType = '2' THEN ISNULL(@MCMultiplierOthers,1) ELSE 1 END  END)
 -- If IP type then Multiply with IP Multiplier Else WIth 1 AS the calculation already being done
 SET @poPayerNETShare = 0; -- Set the Payor share net to zero
END

--------- >>>>>>>>>>>>>>>   XXXXXXXXXXXXXXXXX   BB - As per Client needed New Logic Added on 8-Mar-2016  -- New Columns Added  ActivityCost , UnitCost
------------   ActivityCost and UnitCost to be as it is handled in Calling PROC and here we set GROSS = PatientShare + PayerNET
----------------------  Also Setting MCDiscount as per new Logic
Set @pGross =  @poPayerNETShare + @poPatientShare
Set @MCDiscount = @pGross - @OriginalCostPassedIn


----- New Logic (----> Get Present AAPShare (AlreadyAppliedPatientShare) --- ENDS

----  Select @pGross,'Cal - EXpected Payment From->','BAseRate->',@MCIPBaseRate,'PAYER->',@poPayerNETShare,'PATIENT->',@poPatientShare,'Discount->',@MCDiscount

------ TRACKING PURPOSE --- Can Be commented once Satisified or leave it as Tracking and answering difffernt questions from Client -- STARTS

Insert into MCContractTracking
( PatientID, EncounterID, MCOrderType, MCOrderCode, PolicyID, PolicyStartDate, PolicyEndDate, MCContractID, MCContractCode, 
                         MCLevel, MCEncounterType, MCPatientPercent, MCPatientFixed, MCPatientCapping, MCMultiplier, AlreadyAppliedPatientShare, GrossAfterMultiplier, PatientShare, 
                         PayerNET, Remarks, BCCreatedBy, BCCreatedDate,BCIsActive)
Select @pPatientID 'PatientID', @pEncounterID 'EncounterID',@pOrderType 'OrderType',@Cur_MCOrderCode'OrderCode',@INSPolicyId 'PolicyID', 
Cast(@PolicyBeginDate as Date) 'PolicyStartDate', Cast(@PolicyEndDate as Date) 'PolicyEnDDate',@poMCContractAppliedID 'MCContractIDApplied',@poMCContractCode 'MCContractCode',
@Cur_MCLevel 'MCLevel',@EncounterType,@Cur_Percent 'Percent',@Cur_Fixed 'Fixed',@Cur_Capping 'Capping',@Cur_Multiplier 'Multiplier', 
@AAPShare 'AlreadyAppliedShare',@pGross 'GrossAfterMultiplier', @poPatientShare 'PatientShare', @poPayerNETShare 'PAYER-NET', @Remarks,'1',@CurrentDate,1;

------ TRACKING PURPOSE --- Can Be commented once Satisified or leave it as Tracking and answering difffernt questions from Client -- ENDS


END  ---- Procedure Ends





GO


