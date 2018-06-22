IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'DeleteAllEncounterRelatedDataByUserFacility') 
  DROP PROCEDURE DeleteAllEncounterRelatedDataByUserFacility;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Proc [dbo].[DeleteAllEncounterRelatedDataByUserFacility]
(
@UserId int =10,
@EncounterID int = 0,
@DeleteAllTransactionCounters bit =1,
@ResetBedOccupany bit =1,
@DeletePatientInfo bit = 1,
@DeleteRolesRelatedData bit=0
)
As
Begin
	Begin TRY
		Begin Tran
			Declare @Count int
			Declare @TempEncounters Table (EncounterId int)
			Declare @TempScrubHeaders Table (ScrubHeaderId int)
			Declare @TempClaims Table (ClaimId int)
			Declare @CorporateId int

			Select TOP 1 @CorporateId = CorporateId From Users Where UserId = @UserId
			
			IF(@EncounterID = 0)
			BEGIN
				INSERT INTO @TempEncounters 
				Select EncounterID From Encounter Where EncounterFacility = (Select TOP 1 FacilityID From Users Where UserId = @UserId)
			END
			ELSE 
			Begin
				INSERT INTO @TempEncounters Select @EncounterID
			End
			
			Select @Count = Count(1) From @TempEncounters 

			PRINT @Count
		
			If	@Count > 0
			Begin
				--Attached with Encounter and Billing
				IF @EncounterID = 0 And @ResetBedOccupany = 1
					Update UBedMaster Set IsOccupied  = 0 Where FacilityID IN (Select TOP 1 FacilityID From Users Where UserId = @UserId)

				IF @EncounterID = 0 And @DeleteAllTransactionCounters = 1
					DELETE From DashboardTransactionCounter Where FacilityId IN (Select TOP 1 FacilityID From Users Where UserId = @UserId) 
					And Cast(ActivityDay as date) < CAST(GETDATE() as date)

				DELETE From BillHeader			Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From BedCharges			Where BCEncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From BillActivity			Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From XActivity				Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From BedTransaction		Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From ManualChargesTracking	Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From MappingPatientBed		Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From MedicalNotes			Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From MedicalRecord			Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From MedicalVital			Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From OpenOrder				Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From OrderActivity			Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From XEncounter			Where EncounterID IN (Select EncounterId From @TempEncounters)
				DELETE From Payment			Where PayEncounterID IN (Select EncounterId From @TempEncounters)
				
				Delete From Diagnosis Where EncounterID IN (Select EncounterId From @TempEncounters)

				INSERT INTO @TempScrubHeaders Select ScrubHeaderID From ScrubHeader Where EncounterID IN (Select EncounterId From @TempEncounters)

				Set @Count = 0
				Select @Count = Count(1) From @TempScrubHeaders
		
				IF @Count > 0
				Begin
					DELETE From ScrubReport Where ScrubHeaderID IN (Select ScrubHeaderId From @TempScrubHeaders)
					DELETE From ScrubHeader Where EncounterID IN (Select EncounterId From @TempEncounters)
				End

				INSERT INTO @TempClaims Select XClaimID From XClaim Where EncounterID IN (Select EncounterId From @TempEncounters)

				Set @Count = 0
				Select @Count = Count(1) From @TempClaims

				If @Count = 0
				Begin
					DELETE From XPaymentFileXML Where XClaimID IN (Select T.ClaimId From @TempClaims T)
					DELETE From XClaim Where EncounterID IN (Select EncounterId From @TempEncounters)
				End

				--lastly, Delete encounters from Master Encounter Table.
				DELETE From Encounter Where EncounterID IN (Select EncounterId From @TempEncounters)

			End

			Delete From AuditLog		Where FacilityId IN (Select TOP 1 FacilityID From Users Where UserId = @UserId) 
			Delete From LoginTracking	Where CorporateId = @CorporateId
			Delete From ScrubEditTrack  Where CorporateId = @CorporateId
			
			
			TRUNCATE Table TPXMLParsedData
			TRUNCATE Table TPClaim
			TRUNCATE Table TPFileXML
			TRUNCATE Table TPFileHeader

			If @DeletePatientInfo=1
			Begin
				Exec CleanUpPatientsDataByCorporateId @CorporateId;
			End

			--Delete the Dashboard Actuals
			Update DashboardBudget Set 
			[JanuaryBudget] = 0.00
			,[FebruaryBudget] = 0.00
			,[MarchBudget] = 0.00
			,[AprilBudget] = 0.00
			,[MayBudget] = 0.00
			,[JuneBudget] =	0.00
			,[JulyBudget] =	0.00
			,[AugustBudget] = 0.00
			,[SeptemberBudget] = 0.00
			,[OctoberBudget] = 0.00
			,[NovemberBudget] =	0.00
			,[DecemberBudget] =	0.00 
			Where FiscalYear=DATEPART(yyyy, GETDATE()) And BudgetType='2' --2 Here means Actuals


			Delete From XFileHeader Where FacilityId IN (Select TOP 1 FacilityID From Users Where UserId = @UserId) 

			Declare @FID bigint=0
			Select TOP 1 @FID=FacilityID From Users Where UserId = @UserId
			Delete From XClaim Where FacilityID IN (Select FacilityNumber From Facility WHere FacilityId=@FID)


			IF @DeleteRolesRelatedData = 1
			Begin
				Declare @RoleTemp Table (RoleId int)

				INSERT INTO @RoleTemp
				Select RoleId From [Role] Where CorporateId = @CorporateId

				Delete From FacilityRole Where RoleId IN (Select RoleId From @RoleTemp)
				Delete From RoleTabs Where RoleId IN (Select RoleId From @RoleTemp)
				Delete From UserRole Where RoleId IN (Select RoleId From @RoleTemp)
				Delete From [Role] Where RoleId IN (Select RoleId From @RoleTemp)
			End 


			IF @@ERROR > 0
			Begin
				RollBack Tran
				PRINT 'Error while executing this command.'
			End
			Else
			Begin
				Commit Tran
				PRINT 'All Deleted - Thank You'		
			End
	End TRY
	Begin Catch
		RollBack Tran
	End Catch
End





