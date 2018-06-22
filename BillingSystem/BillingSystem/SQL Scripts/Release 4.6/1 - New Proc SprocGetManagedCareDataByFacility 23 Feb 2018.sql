-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetManagedCareDataByFacility','P') IS NOT NULL
   DROP PROCEDURE SprocGetManagedCareDataByFacility
GO

CREATE PROCEDURE SprocGetManagedCareDataByFacility
(
@pFId bigint,
@pCId bigint,
@pIsActive bit,
@pUserId bigint
)
As
Begin
	Select M.[MCContractID],M.[MCCode],M.[MCOrderType],M.[MCOrderCode],M.[MCPatientPercent],M.[MCInPatientBaseRate]
	,M.[MCAnnualOutOfPocket],M.[MCPatientFixed],M.[MCPatientCapping],M.[MCMultiplier],M.[MCWaitingDays],M.[MCExpireAfterDays],M.[MCApplyWeightAge]
	,M.[MCPerDiemsApplicable],M.[MCCarveoutsApplicable],M.[MCDRGTableNumber],M.[MCCPTTableNumber],M.[MCCodeRangeFrom],M.[MCCodeRangeTill],M.[BCCreatedBy]
    ,M.[BCCreatedDate],M.[BCModifiedBy],M.[BCModifiedDate],M.[BCIsActive],M.[ModelName],M.[InitialSubmitDay],M.[ResubmitDays1],M.[ResubmitDays2]
	,M.[PenaltyLateSubmission],M.[BillScrubberRule],M.[ExpectedPaymentDays],M.[MCMultiplierOutpatient],M.[MCMultiplierEmergencyRoom],M.[MCMultiplierOther]
	,M.[MCInpatientDeduct],M.[MCOutpatientDeduct],M.[MCEMCertified],M.[MCPenaltyRateResubmission],M.[MCRuleSetNumber],M.[MCAddon],M.[MCExpectedFixedrate]
	,M.[MCExpectedPercentage],M.[MCInPatientType],M.[MCOPPatientType],M.[MCERPatientType],M.[MCGeneralLedgerAccount],M.[ARGeneralLedgerAccount]
	,M.[CorporateId],M.[FacilityId]
	,MCEncounterType=
	(
		Case 
			WHEN ISNULL(M.MCEncounterType,'')='' THEN '' 
			ELSE (Select TOP 1 G.GlobalCodeName From GlobalCodes G 
					Where G.GlobalCodeCategoryValue='1107' And G.GlobalCodeValue=M.MCEncounterType)
		END
	)
	,MCLevel=
	(
		Case 
			WHEN ISNULL(M.MCLevel,'')='' THEN '' 
			ELSE (Select TOP 1 G.GlobalCodeName From GlobalCodes G 
					Where G.GlobalCodeCategoryValue='950' And G.GlobalCodeValue=M.MCLevel)
		END
	)
	From MCContract M Where M.CorporateId=@pCId And M.FacilityId=@pFId And M.BCIsActive=@pIsActive
	And Exists (Select 1 From [Users] U Where U.UserID=@pUserId And U.IsActive=1 And ISNULL(U.IsDeleted,0)=0)
	FOR JSON Path,ROOT('ManagedCareResult'),INCLUDE_NULL_VALUES
End
