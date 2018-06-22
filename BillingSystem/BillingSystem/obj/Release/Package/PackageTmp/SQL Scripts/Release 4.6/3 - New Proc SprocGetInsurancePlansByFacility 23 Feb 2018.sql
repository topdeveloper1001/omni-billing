-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetInsurancePlansByFacility','P') IS NOT NULL
   DROP PROCEDURE SprocGetInsurancePlansByFacility
GO

CREATE PROCEDURE SprocGetInsurancePlansByFacility
(
@pFId bigint,
@pCId bigint,
@pIsActive bit,
@pUserId bigint
)
As
Begin
	Select P.[InsurancePlanId]		As 'InsurancePlan.InsurancePlanId'
      ,P.[InsuranceCompanyId]	As 'InsurancePlan.InsuranceCompanyId'
      ,P.[PlanName]				As 'InsurancePlan.PlanName'
      ,P.[PlanNumber]			As 'InsurancePlan.PlanNumber'
      ,P.[PlanBeginDate]		As 'InsurancePlan.PlanBeginDate'
      ,P.[PlanEndDate]			As 'InsurancePlan.PlanEndDate'
      ,P.[PlanDescription]		As 'InsurancePlan.PlanDescription'
      ,P.[CreatedBy]			As 'InsurancePlan.CreatedBy'
      ,P.[CreatedDate]			As 'InsurancePlan.CreatedDate'
      ,P.[ModifiedBy]			As 'InsurancePlan.ModifiedBy'
      ,P.[ModifiedDate]			As 'InsurancePlan.ModifiedDate'
      ,P.[IsDeleted]			As 'InsurancePlan.IsDeleted'
      ,P.[DeletedBy]			As 'InsurancePlan.DeletedBy'
      ,P.[IsActive]				As 'InsurancePlan.IsActive'
      ,P.[DeletedDate]			As 'InsurancePlan.DeletedDate'
	,InsuranceCompanyName=(Select TOP 1 I.InsuranceCompanyName From InsuranceCompany I Where I.InsuranceCompanyId=P.InsuranceCompanyId)
	From InsurancePlans P
	Where P.InsuranceCompanyId IN (Select I.InsuranceCompanyId From InsuranceCompany I 
	Where I.CorporateId=@pCId AND I.FacilityId=@pFId And I.IsActive=@pIsActive)
	And P.IsActive=@pIsActive AND P.IsDeleted=0
	And Exists (Select 1 From [Users] U Where U.UserID=@pUserId And U.IsActive=1 And ISNULL(U.IsDeleted,0)=0)
	FOR JSON Path,ROOT('PlanResult'),INCLUDE_NULL_VALUES
End
