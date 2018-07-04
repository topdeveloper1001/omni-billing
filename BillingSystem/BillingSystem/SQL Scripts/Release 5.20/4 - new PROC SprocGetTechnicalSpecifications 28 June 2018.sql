-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocGetTechnicalSpecifications','P') IS NOT NULL
   DROP PROCEDURE SprocGetTechnicalSpecifications
GO

CREATE Procedure [dbo].[SprocGetTechnicalSpecifications]  ---- SPROC_GetTechnicalSpecifications
(
@FacilityId int,
@CorporateId int
)
AS
BEGIN
	
	Declare @TechnicalSpecificationTempTable table(
	[Id] [int] ,[ItemID] [bigint],[TechSpec] [nvarchar](120) NULL,[CorporateId] [int] NULL,[FacilityId] [int] NULL,
	[CreatedBy] [int] NULL,	
	[EName] [nvarchar](500) NULL
	)
	
	Insert Into @TechnicalSpecificationTempTable
	Select T.[Id], T.[ItemID], T.[TechSpec], T.[CorporateId],T.[FacilityId],
		T.[CreatedBy],
	 E.EquipmentName as EName
	

	from [TechnicalSpecifications] T 
	
	LEFT join [EquipmentMaster] E on T.ItemID =E.EquipmentMasterId
	where T.FacilityId=@FacilityId and T.CorporateId=@CorporateId

	SELECT * From @TechnicalSpecificationTempTable
	For Json Path, Root('DashboardResult'),Include_Null_Values
END