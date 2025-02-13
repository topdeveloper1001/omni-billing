IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetPatientInfoView')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetPatientInfoView
GO
/****** Object:  StoredProcedure [dbo].[SprocGetPatientInfoView]    Script Date: 3/27/2018 9:10:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetPatientInfoView]
(
@PId bigint,
@EId bigint=0
)
AS
BEGIN
	--PatientInfoCustomModel
	Exec SprocGetPatientDetails @PId=@PId,@EId=@EId,@ShowEncounters=0

	--Get Patient Insurance View (PatientInsuranceCustomModel)
	Select I.*
	,(Select TOP 1 InsuranceCompanyName From InsuranceCompany Where InsuranceCompanyId = I.InsuranceCompanyId) As CompanyName
	,(Select TOP 1 PlanName From InsurancePlans Where InsuranceCompanyId = I.InsurancePlanId) As PlanName
	,(Select TOP 1 PolicyName From InsurancePolices Where InsuranceCompanyId = I.InsurancePolicyId) As PolicyName
	From PatientInsurance I Where I.PatientId=@PId


	/* Get Patient's Login details (PatientLoginDetail) */
	--;With Fac (FacilityId,FacilityName,FacilityNumber,PatientId,PatientName,BirthDate,EmirateId,CorporateId)
	--As
	--(
	--Select F.FacilityId,F.FacilityName,F.FacilityNumber,P.PatientID,(P.PersonFirstName + ' ' + P.PersonLastName)
	--,P.PersonBirthDate,P.PersonEmiratesIDNumber,F.CorporateID
	--From Facility F 
	--INNER JOIN PatientInfo P ON F.FacilityId=P.FacilityId
	--Where P.PatientID=@PId
	--)

	Select L.*
	,P.FacilityId,P.FacilityName,P.FacilityNumber,P.PatientName,P.BirthDate,P.EmriateId,P.CorporateId
	From (
		Select TOP 1 PP.PatientId,FacilityId=ISNULL(F.FacilityId,0),FacilityName=ISNULL(F.FacilityName,''),FacilityNumber=ISNULL(F.FacilityNumber,'')
		,PatientName=(PP.PersonFirstName + ' ' + PP.PersonLastName)
		,BirthDate=PP.PersonBirthDate,EmriateId=PP.PersonEmiratesIDNumber,CorporateId=ISNULL(F.CorporateId,0)
		From PatientInfo PP
		Left Join Facility F ON F.FacilityId=PP.FacilityId
		Where PP.PatientId=@PId
	) P
	INNER JOIN PatientLoginDetail L ON L.PatientId=P.PatientID

	--Select P.*
	--,F.FacilityId,F.FacilityName,F.FacilityNumber,F.PatientName,F.BirthDate,F.EmirateId As EmriateId,F.CorporateId
	--From 
	--(
	--Select L.*
	--From PatientLoginDetail L 
	--Where L.PatientId=@PId
	--) P
	--INNER JOIN Fac F ON P.PatientId = F.PatientId
	/* Get Patient's Login details (PatientLoginDetail) */

	--Get Patient's Phone Details	(PatientPhone)
	Select TOP 1 * From PatientPhone Where PatientID=@PId	



	--Is Encounter Open (Bool)
	Select 
	(CASE When Count(1) > 0 THEN Cast(1 as bit) ELSE Cast(0 as bit) End) As EncounterOpen 
	From Encounter Where EncounterEndTime Is NUll And PatientID=@PId
END


