IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetOrderCodesToExport')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocGetOrderCodesToExport
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SprocGetOrderCodesToExport]    Script Date: 3/22/2018 8:23:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Exec [SprocGetOrderCodesToExport] '','ATC',9,8,'','',10
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetOrderCodesToExport]
(
@pSearchText nvarchar(max)='',
@pCodeType nvarchar(10),
@pCId bigint, 
@pFId bigint,
@pTableNumber nvarchar(10),
@pTableName nvarchar(50),
@pUserId bigint
)
AS
BEGIN
	
	Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFId))

	Declare @columns nvarchar(2000)=''

	---- Delcare the Table to return
	Declare @GeneralCodesTable SqlBillingCodeType
	
	---Now fetch the Code data from the tables
	IF @pCodeType = 'CPT'			--'3'
	BEGIN
		Insert into @GeneralCodesTable(Id,TableNumber,Code,CodeDescription,Price,EffectiveFrom,EffectiveTill,CodeGroup,OtherValue1)
		Select CPTCodesId,@pTableNumber,CodeNumbering,CodeDescription, CodePrice,CodeEffectiveDate,CodeExpiryDate,CodeGroup,CodeTableDescription
		From CptCodes where (@pSearchText='' OR (CodeNumbering like '%'+ @pSearchText  +'%' or CodeDescription like '%'+ @pSearchText  +'%'))
		AND (CAST(@LocalDateTime as date) between CodeEffectiveDate And ISNULL(CodeExpiryDate,@LocalDateTime))
		AND CodeTableNumber = @pTableNumber and ISNULL(IsActive,1) = 1 and ISNULL(IsDeleted,0) = 0

		SET @columns = 'TableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate,CodeGroup'
	END

	ELSE if @pCodeType = 'HCPCS'	--'4'
	BEGIN
		Insert into @GeneralCodesTable(Id,TableNumber,Code,CodeDescription,Price,EffectiveFrom,EffectiveTill,OtherValue1)
		Select  HCPCSCodesId,@pTableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate,CodeTableDescription
		from HCPCSCodes where (@pSearchText='' OR(CodeNumbering like '%'+ @pSearchText  +'%' or CodeDescription like '%'+ @pSearchText  +'%'))
		AND (CAST(@LocalDateTime as date) between CodeEffectiveDate And ISNULL(CodeExpiryDate,@LocalDateTime))
		and CodeTableNumber = @pTableNumber and ISNULL(IsActive,1) = 1 and ISNULL(IsDeleted,0) = 0

		SET @columns = 'TableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate'
	END

	ELSE if @pCodeType = 'DRUG'		--'5'
	BEGIN
		--DrugTableNumber,DrugCode,DrugGenericName,DrugPackageName,DrugPricePharmacy,DrugPricePublic
		--,DrugUnitPricePharmacy,DrugUnitPricePublic,DrugDeleteDate,DrugDosage,DrugStrength
		Insert into @GeneralCodesTable(Id,TableNumber,Code,CodeDescription,CodeGroup,OtherValue1,OtherValue2
		,OtherValue3,OtherValue4,EffectiveFrom,EffectiveTill,OtherValue5,OtherValue6)
		Select Id,@pTableNumber,DrugCode,DrugGenericName,DrugPackageName,DrugPricePharmacy,DrugPricePublic
		,DrugUnitPricePharmacy,DrugUnitPricePublic,DrugLastChange,DrugDeleteDate,DrugDosage,DrugStrength
		from DRUG where DRUGTableNumber = @pTableNumber AND @LocalDateTime <= ISNULL(DrugDeleteDate,@LocalDateTime)
		AND DrugStatus NOT IN ('Deleted') and ISNULL(InStock,1)=1
		AND (@pSearchText='' OR (DrugCode like '%'+ @pSearchText  +'%' or DrugGenericName like '%'+ @pSearchText  +'%' or DrugPackageName like '%'+ @pSearchText  +'%'))

		SET @columns = 'TableNumber,DrugCode,GenericName,PackageName,PricePharmacy,PricePublic,UnitPricePharmacy,UnitPricePublic,LastChangeOn,DeleteDate,Dosage,Strength'
	END
	Else If @pCodeType = 'Diagnosis' --'16'
	BEGIN
		Insert into @GeneralCodesTable(Id,TableNumber,Code,CodeDescription,EffectiveFrom,EffectiveTill,CodeGroup,OtherValue1,OtherValue2)
		Select DiagnosisTableNumberId,@pTableNumber,DiagnosisCode1,DiagnosisFullDescription,ISNULL(DiagnosisEffectiveStartDate,'') As DiagnosisEffectiveStartDate
		,ISNULL(DiagnosisEffectiveEndDate,'') As DiagnosisEffectiveEndDate,ShortDescription,DiagnosisMediumDescription,DiagnosisTableName
		From DiagnosisCode Where DiagnosisTableNumber=@pTableNumber And ISNULL(IsDeleted,0)=0
		AND (CAST(@LocalDateTime as date) between ISNULL(DiagnosisEffectiveStartDate,@LocalDateTime-1) And ISNULL(DiagnosisEffectiveEndDate,@LocalDateTime + 1))

		SET @columns = 'TableNumber,DiagnosisCode,CodeDescription,DiagnosisEffectiveStartDate,DiagnosisEffectiveEndDate,ShortDescription,DiagnosisMediumDescription'
	END
	Else If @pCodeType = 'DRG'		--'9'
	BEGIN
		Insert into @GeneralCodesTable(Id,TableNumber,Code,CodeDescription,Price,EffectiveFrom,EffectiveTill,OtherValue1,OtherValue2)
		Select DRGCodesId, @pTableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate,CodeDRGWeight As OtherValue,Alos As OtherValue2
		from DRGCodes where (@pSearchText='' OR(CodeNumbering like '%'+ @pSearchText  +'%' or CodeDescription like '%'+ @pSearchText  +'%'))
		AND (CAST(@LocalDateTime as date) between CodeEffectiveDate And ISNULL(CodeExpiryDate,@LocalDateTime))
		and CodeTableNumber = @pTableNumber and IsActive = 1 and ISNULL(IsDeleted,0) = 0

		SET @columns = 'TableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate,CodeDRGWeight,Alos'		
	END
	Else If @pCodeType = 'ATC'
	BEGIN
		Insert into @GeneralCodesTable(Id,TableNumber,Code,CodeDescription,Price,EffectiveFrom,EffectiveTill,CodeGroup,OtherValue1,OtherValue2)
		Select DISTINCT ATCCodeID,CodeTableNumber,ATCCode,CodeDescription,'' AS Price,CodeEffectiveFrom,CodeEffectiveTill,DrugName As CodeGroup,Purpose As OtherValue,DrugDescription As OtherValue2
		FROM ATCCodes
		WHERE IsActive = 1
		--And
		--((ISNULL(@pTableNumber,'')='' AND ISNULL(CodeTableNumber,'')='') OR CodeTableNumber = @pTableNumber)
		And
		(@pSearchText='' OR(ATCCode like '%'+ @pSearchText  +'%' or CodeDescription like '%'+ @pSearchText  +'%'))
		--AND (@LocalDateTime between CodeEffectiveDate And ISNULL(CodeExpiryDate,@LocalDateTime))
		--and CodeTableNumber = @pTableNumber and IsActive = 1 and ISNULL(IsDeleted,0) = 0

		SET @columns = 'TableNumber,CodeNumbering,CodeDescription,CodePrice,EffectiveFrom,EffectiveTill,DrugName,Purpose,DrugDescription'		
	END

	Select * from @GeneralCodesTable

	Select @columns
END





GO


