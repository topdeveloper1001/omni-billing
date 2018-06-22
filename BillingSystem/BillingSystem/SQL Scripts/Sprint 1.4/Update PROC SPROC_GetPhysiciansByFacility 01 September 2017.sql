IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SPROC_GetPhysiciansByFacility') 
  DROP PROCEDURE SPROC_GetPhysiciansByFacility;
 
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
CREATE PROCEDURE [dbo].[SPROC_GetPhysiciansByFacility]
(
@FId int,
@UserId int=0
)
AS
BEGIN
	Declare @results varchar(500)

	Select P.Id          
	,P.PhysicianEmployeeNumber                 
	,P.PhysicianName                                      
	,P.PhysicianLicenseNumber                             
	,P.PhysicianLicenseType 
	,P.PhysicianLicenseEffectiveStartDate 
	,P.PhysicianLicenseEffectiveEndDate 
	,(Select FacilityName From Facility Where FacilityId= P.FacilityId) PrimaryFacilityName
	--,(CASE 
	--	WHEN ISNULL(P.PhysicianSecondaryFacility,'0') !='0' 
	--	THEN (Select FacilityName From Facility Where FacilityId= P.PhysicianSecondaryFacility) 
	--	ELSE '' END) SecondaryFacilityName
	--,(CASE 
	--	WHEN ISNULL(P.PhysicianThirdFacility,'0') !='0' 
	--	THEN (Select FacilityName From Facility Where FacilityId= P.PhysicianThirdFacility) 
	--	ELSE '' END) ThirdFacilityName
	,(CASE 
		WHEN ISNULL(P.AssociatedFacilities,'') !='' 
		THEN RTRIM(LTRIM(STUFF((Select ', ' + FacilityName From Facility 
				Where FacilityId IN 
					(Select IDValue From dbo.Split(',',P.AssociatedFacilities))
					And FacilityId != P.FacilityId
				FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')))
		ELSE '' END) OtherFacilities
	,P.CreatedBy
	,P.CreatedDate             
	,P.ModifiedBy  
	,P.IsDeleted 
	,P.DeletedBy   
	,P.ModifiedDate            
	,P.IsActive 
	,P.DeletedDate             
	,P.UserType    
	,P.UserId      
	,P.FacultySpeciality                                  
	,P.FacultyDepartment
	,P.FacultyLunchTimeFrom
	,P.FacultyLunchTimeTill
	,P.ExtValue1
	,P.ExtValue2
	,P.CorporateId
	,P.FacilityId
	,(Select GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = '1104' And GlobalCodeValue = P.PhysicianLicenseType And ExternalValue1 = R.RoleName) PhysicanLicenseTypeName
	,R.RoleName UserTypeStr
	,(Select GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue = '1121' And GlobalCodeValue = P.FacultySpeciality And FacilityNumber=@FId) UserSpecialityStr
	,(Select FacilityStructureName From FacilityStructure Where FacilityStructureId = P.FacultyDepartment) UserDepartmentStr	
	,(P.PhysicianName + ' (' + R.RoleName + ')') AS ClinicianName
	From Physician P
	INNER JOIN [Role] R ON P.UserType = R.RoleId

	Where P.FacilityId = @FId
END
