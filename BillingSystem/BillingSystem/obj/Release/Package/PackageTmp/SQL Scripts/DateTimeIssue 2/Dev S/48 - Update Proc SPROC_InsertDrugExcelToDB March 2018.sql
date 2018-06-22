IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_InsertDrugExcelToDB')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_InsertDrugExcelToDB
GO

/****** Object:  StoredProcedure [dbo].[SPROC_InsertDrugExcelToDB]    Script Date: 22-03-2018 20:21:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[SPROC_InsertDrugExcelToDB]
    @insertDrugExcelToDB DrugTT readonly,
	@LoggedInUser Int,
	@TableNumber NVarchar(50),
	@Type NVarchar(50)
AS
BEGIN
	Delete From [dbo].DrugTT Where LoggedInUser = @LoggedInUser And TableNumber = @TableNumber And [Type] = @Type

	Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))

    Insert into [dbo].DrugTT([Greenrain Code],[Insurance Plan],[Package Name],[Generic Name],Strength,[Dosage Form],[Package Size],[Package Price to Public],
	[Package Price to Pharmacy],[Unit Price to Public],[Unit Price to Pharmacy],[Status],[Delete Effective Date],[Last Change],[Agent Name],[Manufacturer Name],
	LoggedInUser,TableNumber,[Type])
	Select [Greenrain Code],[Insurance Plan],[Package Name],[Generic Name],Strength,[Dosage Form],[Package Size],[Package Price to Public],
	[Package Price to Pharmacy],[Unit Price to Public],[Unit Price to Pharmacy],[Status],[Delete Effective Date],[Last Change],[Agent Name],[Manufacturer Name],
	@LoggedInUser,@TableNumber,@Type From @insertDrugExcelToDB 

	If(@Type = '6')
	Begin
		Insert into Drug
		(DrugTableNumber, DrugDescription, DrugCode, DrugInsurancePlan, DrugPackageName, DrugGenericName, DrugStrength, DrugDosage,DrugPackageSize,
		DrugPricePublic, DrugPricePharmacy, DrugUnitPricePublic, DrugUnitPricePharmacy, DrugStatus, DrugDeleteDate, DrugLastChange, DrugAgentName, DrugManufacturer,
		InStock)
		Select 0, 'Greenrain', [Greenrain Code], [Insurance Plan], [Package Name], [Generic Name], [Strength], [Dosage Form], [Package Size], [Package Price to Public],
		[Package Price to Pharmacy], [Unit Price to Public], [Unit Price to Pharmacy], [Status], Cast(convert(VARCHAR(10),[Delete Effective Date],108) As DateTime),
		Cast(convert(VARCHAR(10),[Last Change],108) As DateTime), [Agent Name], [Manufacturer Name], 1 From DrugTT 
		Where LoggedInUser = @LoggedInUser And TableNumber = @TableNumber And [Type] = @Type



		Declare @DrugName nvarchar(MAX)

		Declare drugCursor Cursor For
		Select LTRIM(RTRIM(DrugGenericName)) From Drug Where BrandCode IS NULL And DrugTableNumber = '0'
		
		OPEN drugCursor 
		Fetch Next From DrugCursor INTO @DrugName

		While @@Fetch_Status = 0
		Begin
			Declare @brandCode nvarchar(100)
 
			Select @brandCode = ISNULL(GlobalCodeValue,'') From GlobalCodes Where GlobalCodeName = @DrugName And GlobalCodeCategoryValue = '11100'
 
		IF @brandCode = ''
		Begin
			Select @brandCode = MAX(CAST(GlobalCodeValue as int)) + 1 From GlobalCodes Where GlobalCodeName = @DrugName And GlobalCodeCategoryValue = '11100'

		INSERT INTO [dbo].[GlobalCodes]
           (GlobalCodeValue,
			GlobalCodeName,
			[Description]
           ,[FacilityNumber]
           ,[GlobalCodeCategoryValue]
           ,[IsActive]
           ,[CreatedBy]
           ,[CreatedDate]
           )
		Select @brandCode,@DrugName,@DrugName,0,'11100',1,1,@CurrentDate from Drug D
		End
 
		Update Drug Set BrandCode = @brandCode Where DrugGenericName = @DrugName And DrugTableNumber = '0' And BrandCode IS NULL

		Fetch Next From DrugCursor INTO @DrugName
		End


		Close drugCursor 
		deallocate drugCursor


	End
END







GO


