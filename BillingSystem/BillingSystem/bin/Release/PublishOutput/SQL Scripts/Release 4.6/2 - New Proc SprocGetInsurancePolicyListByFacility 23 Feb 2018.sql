-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetInsurancePolicyListByFacility','P') IS NOT NULL
   DROP PROCEDURE SprocGetInsurancePolicyListByFacility
GO

CREATE PROCEDURE SprocGetInsurancePolicyListByFacility
(
@pFId bigint,
@pCId bigint,
@pIsActive bit,
@pUserId bigint
)
As
Begin
	Select P.[InsurancePolicyId] As 'InsurancePolices.InsurancePolicyId'
      ,P.[PlanName] As 'InsurancePolices.PlanName'
      ,P.[PlanNumber] As 'InsurancePolices.PlanNumber'
      ,P.[PolicyName] As 'InsurancePolices.PolicyName'
      ,P.[PolicyNumber]				 As 'InsurancePolices.PolicyNumber'
      ,P.[PolicyBeginDate]			 As 'InsurancePolices.PolicyBeginDate'
      ,P.[PolicyEndDate]			 As 'InsurancePolices.PolicyEndDate'
      ,P.[PolicyDescription]		 As 'InsurancePolices.PolicyDescription'
      ,P.[PolicyHolderName]			 As 'InsurancePolices.PolicyHolderName'
      ,P.[InsuranceCompanyId]		 As 'InsurancePolices.InsuranceCompanyId'
      ,P.[McContractCode]			 As 'InsurancePolices.McContractCode'
      ,P.[CreatedBy]				 As 'InsurancePolices.CreatedBy'
      ,P.[CreatedDate]				 As 'InsurancePolices.CreatedDate'
      ,P.[ModifiedBy]				 As 'InsurancePolices.ModifiedBy'
      ,P.[ModifiedDate]				 As 'InsurancePolices.ModifiedDate'
      ,P.[IsDeleted]				 As 'InsurancePolices.IsDeleted'
      ,P.[DeletedBy]				 As 'InsurancePolices.DeletedBy'
      ,P.[IsActive]					 As 'InsurancePolices.IsActive'
      ,P.[DeletedDate]				 As 'InsurancePolices.DeletedDate'
      ,P.[InsurancePlanId]			 As 'InsurancePolices.InsurancePlanId'
      ,P.[SocialSecurityNumber]		 As 'InsurancePolices.SocialSecurityNumber'
      ,P.[FacilityId]				 As 'InsurancePolices.FacilityId'
      ,P.[CorporateId]				As 'InsurancePolices.CorporateId'
	,InsuranceCompanyName=(Select TOP 1 I.InsuranceCompanyName From InsuranceCompany I Where I.InsuranceCompanyId=P.InsuranceCompanyId)
	,PlanName=(Select TOP 1 I.PlanName From InsurancePlans I Where I.InsurancePlanId=P.InsurancePlanId)
	,PlanNumber=P.PlanNumber
	,ManagedCareCode=(CASE WHEN ISNULL(P.McContractCode,'') !='' 
						THEN 
							(Select TOP 1 M.MCOrderCode + ' - ' + M.ModelName From MCContract M Where M.MCCode=P.McContractCode)
							Else ''
						END)
	From InsurancePolices P
	Where P.CorporateId=@pCId And P.FacilityId=@pFId And P.IsActive=@pIsActive AND P.IsDeleted=0
	And Exists (Select 1 From [Users] U Where U.UserID=@pUserId And U.IsActive=1 And ISNULL(U.IsDeleted,0)=0)
	FOR JSON Path,ROOT('PolicyResult'),INCLUDE_NULL_VALUES
End
