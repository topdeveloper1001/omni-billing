IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocImportBillingCodes')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocImportBillingCodes
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SprocImportBillingCodes]    Script Date: 3/22/2018 8:21:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocImportBillingCodes]
(
@pCodeType nvarchar(10),
@pCId bigint, 
@pFId bigint,
@pTableNumber nvarchar(10),
@pTableName nvarchar(50),
@pUserId bigint,
@TCodes SqlBillingCodeType READONLY
)
AS
BEGIN
	DECLARE @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFId))

	---Now fetch the Code data from the tables
	IF @pCodeType = 'CPT'
	BEGIN
		;With ExistingCodes
		As
		(
		Select MAX(T.CPTCodesId) As CPTCodesId, MAX(T.EffectiveFrom) As EffectiveFrom,MAX(T.EffectiveTill) As EffectiveTill
		From 
		(
		Select C.CPTCodesId, T.EffectiveFrom,T.EffectiveTill,C.CodeNumbering From CPTCodes C 
		INNER JOIN @TCodes T ON C.CodeNumbering = T.Code AND C.CodeTableNumber=T.TableNumber
		And ISNULL(C.IsActive,1)=1 And ISNULL(C.IsDeleted,0)=0
		--And ISNULL(CodeExpiryDate,@LocalDateTime) <= @LocalDateTime
		) T
		Group by T.CodeNumbering
		)

		Update C SET C.CodeExpiryDate = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END), ModifiedDate = @LocalDateTime, ModifiedBy = @pUserId
		From CPTCodes C
		INNER JOIN ExistingCodes E ON C.CPTCodesId=E.CPTCodesId


		INSERT INTO CPTCodes (CodeTableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate
		,CodeGroup,CreatedBy,CreatedDate,IsActive,IsDeleted,CodeTableDescription)
		Select T.TableNumber,T.Code,T.CodeDescription,T.Price,(Case 
								When EffectiveFrom IS NOT NULL THEN CAST(EffectiveFrom as DATE)
								ELSE @LocalDateTime END),(Case 
								WHEN EffectiveTill IS NOT NULL AND EffectiveFrom IS NOT NULL AND CAST(EffectiveTill as date) > CAST(EffectiveFrom as date)
									THEN CAST(EffectiveTill as date)
								ELSE NULL END),T.CodeGroup,@pUserId,@LocalDateTime,1,0, (Case WHEN ISNULL(T.OtherValue1,'')='' THEN 'HAAD' ELSE T.OtherValue1 END) As TableDescription
								
		From @TCodes T
	END

	ELSE if @pCodeType = 'HCPCS'	--'4'
	BEGIN
		;With ExistingCodes
		As
		(
		Select MAX(T.HCPCSCodesId) As HCPCSCodesId, MAX(T.EffectiveFrom) As EffectiveFrom,MAX(T.EffectiveTill) As EffectiveTill
		From 
		(
		Select C.HCPCSCodesId, T.EffectiveFrom,T.EffectiveTill,C.CodeNumbering From HCPCSCodes C 
		INNER JOIN @TCodes T ON C.CodeNumbering = T.Code AND C.CodeTableNumber=T.TableNumber
		And ISNULL(C.IsActive,1)=1 And ISNULL(C.IsDeleted,0)=0
		) T
		GROUP by T.CodeNumbering
		)

		Update C SET C.CodeExpiryDate = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END), ModifiedDate = @LocalDateTime, ModifiedBy = @pUserId
		From HCPCSCodes C
		INNER JOIN ExistingCodes E ON C.HCPCSCodesId=E.HCPCSCodesId


		INSERT INTO HCPCSCodes (CodeTableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate
		,CreatedBy,CreatedDate,CodeTableDescription,IsActive,IsDeleted)
		Select T.TableNumber,T.Code,T.CodeDescription,T.Price,(Case 
								When EffectiveFrom IS NOT NULL THEN CAST(EffectiveFrom as DATE)
								ELSE @LocalDateTime END),(Case 
								WHEN EffectiveTill IS NOT NULL AND EffectiveFrom IS NOT NULL AND CAST(EffectiveTill as date) > CAST(EffectiveFrom as date)
									THEN CAST(EffectiveTill as date)
								ELSE NULL END),@pUserId,@LocalDateTime,(Case WHEN ISNULL(T.OtherValue1,'')='' THEN 'HAAD' ELSE T.OtherValue1 END) As TableDescription,1,0
		From @TCodes T
	END

	ELSE if @pCodeType = 'DRUG'		--'5'
	BEGIN
		;With ExistingCodes
		As
		(
		Select MAX(D.Id) As Id, MAX(D.EffectiveFrom) AS EffectiveFrom,MAX(D.EffectiveTill) As EffectiveTill
		From 
			(
			Select  C.Id, T.EffectiveFrom,T.EffectiveTill,C.DrugCode From Drug C 
			INNER JOIN @TCodes T ON C.DrugCode= T.Code AND C.DrugTableNumber=T.TableNumber
			--And DrugDeleteDate >= @LocalDateTime 
			AND DrugStatus NOT IN ('Deleted')
			) D
		GROUP by D.DrugCode
		)

		Update D SET D.DrugDeleteDate = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END)
		From Drug D
		INNER JOIN ExistingCodes E ON D.Id=E.Id

		--'TableNumber,DrugCode,GenericName,PackageName,PricePharmacy,PricePublic,UnitPricePharmacy,UnitPricePublic,LastChangeOn,DeleteDate,Dosage,Strength'
		INSERT INTO DRUG (DrugTableNumber,DrugCode,DrugGenericName,DrugPackageName,DrugPricePharmacy,DrugPricePublic
		,DrugUnitPricePharmacy,DrugUnitPricePublic,DrugDeleteDate,DrugLastChange,DrugStatus,DrugDosage,DrugStrength,DrugDescription,InStock
		,BrandCode,DrugAgentName,DrugManufacturer,DrugPackageSize,DrugStrengthHardcode,DrugStrengthHardcodeUOM,DrugPackagesizeHardcode,DrugPackagesizeHardcodeUOM
		,DrugInsurancePlan)
		Select T.TableNumber,T.Code,T.CodeDescription,T.CodeGroup As PackageName,T.OtherValue1,T.OtherValue2
		,T.OtherValue3,T.OtherValue4,(Case 
								When EffectiveFrom IS NOT NULL THEN CAST(EffectiveFrom as DATE)
								ELSE @LocalDateTime END),(Case 
								WHEN EffectiveTill IS NOT NULL AND EffectiveFrom IS NOT NULL AND CAST(EffectiveTill as date) > CAST(EffectiveFrom as date)
									THEN CAST(EffectiveTill as date)
								ELSE NULL END),'Active',T.OtherValue5,T.OtherValue6,'Greenrain',1
		,(Select TOP 1 BrandCode From Drug D Where D.DrugCode=T.Code And D.DrugTableNumber=T.TableNumber And D.DrugStatus='Active')
		,(Select TOP 1 DrugAgentName From Drug D Where D.DrugCode=T.Code And D.DrugTableNumber=T.TableNumber And D.DrugStatus='Active')
		,(Select TOP 1 DrugManufacturer From Drug D Where D.DrugCode=T.Code And D.DrugTableNumber=T.TableNumber And D.DrugStatus='Active')
		,(Select TOP 1 DrugPackageSize From Drug D Where D.DrugCode=T.Code And D.DrugTableNumber=T.TableNumber And D.DrugStatus='Active')
		,'' As DrugStrengthHardcode,'' As DrugStrengthHardcodeUOM,'' As DrugPackagesizeHardcode,'' As DrugPackagesizeHardcodeUOM
		,'' As DrugInsurancePlan
		From @TCodes T
	END
	Else If @pCodeType = 'Diagnosis' --'16'
	BEGIN
		;With ExistingCodes
		As
		(
		Select MAX(D.DiagnosisTableNumberId) As DiagnosisTableNumberId, MAX(D.EffectiveFrom) AS EffectiveFrom,MAX(D.EffectiveTill) As EffectiveTill
		From
			(
			Select  C.DiagnosisTableNumberId, T.EffectiveFrom,T.EffectiveTill,C.DiagnosisCode1
			From DiagnosisCode C 
			INNER JOIN @TCodes T ON C.DiagnosisCode1= T.Code AND C.DiagnosisTableNumber=T.TableNumber
			AND ISNULL(C.IsDeleted,0)=0
			) D
		Group by D.DiagnosisCode1
		)

		Update D SET D.DiagnosisEffectiveEndDate = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END)
		,ModifiedBy = @pUserId, ModifiedDate = @LocalDateTime
		From DiagnosisCode D
		INNER JOIN ExistingCodes E ON D.DiagnosisTableNumberId=E.DiagnosisTableNumberId

		INSERT INTO DiagnosisCode (DiagnosisTableNumber,DiagnosisCode1,ShortDescription,DiagnosisMediumDescription
		,DiagnosisFullDescription,DiagnosisEffectiveStartDate,DiagnosisEffectiveEndDate
		,CreatedBy,CreatedDate,IsDeleted,DiagnosisWeight,DiagnosisTableName)
		Select T.TableNumber,T.Code,T.CodeDescription,T.OtherValue1,T.OtherValue2,(Case 
								When EffectiveFrom IS NOT NULL THEN CAST(EffectiveFrom as DATE)
								ELSE @LocalDateTime END),(Case 
								WHEN EffectiveTill IS NOT NULL AND EffectiveFrom IS NOT NULL AND CAST(EffectiveTill as date) > CAST(EffectiveFrom as date)
									THEN CAST(EffectiveTill as date)
								ELSE NULL END)
		,@pUserId,@LocalDateTime,0,'0',(Case WHEN ISNULL(T.OtherValue4,'')='' THEN 'ICD10' ELSE T.OtherValue4 END) As TableDescription
		From @TCodes T
	END
	Else If @pCodeType = 'DRG'		--'9'
	BEGIN
		;With ExistingCodes
		As
		(
		Select MAX(D.DRGCodesId) As DRGCodesId, MAX(D.EffectiveFrom) AS EffectiveFrom,MAX(D.EffectiveTill) AS EffectiveTill
		From 
			(
			Select C.DRGCodesId, T.EffectiveFrom,T.EffectiveTill,C.CodeNumbering
			From DRGCodes C 
			INNER JOIN @TCodes T ON C.CodeNumbering= T.Code AND C.CodeTableNumber=T.TableNumber
			AND ISNULL(C.IsDeleted,0)=0 And ISNULL(IsActive,1)=1
			) D
		Group by D.CodeNumbering
		)

		Update D SET D.CodeExpiryDate = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END)
		,ModifiedBy = @pUserId, ModifiedDate = @LocalDateTime
		From DRGCodes D
		INNER JOIN ExistingCodes E ON D.DRGCodesId=E.DRGCodesId


		INSERT INTO DRGCodes (CodeTableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate
		,CreatedBy,CreatedDate,CodeDRGWeight,IsActive,IsDeleted,Alos)
		Select T.TableNumber,T.Code,T.CodeDescription,T.Price,(Case 
								When EffectiveFrom IS NOT NULL THEN CAST(EffectiveFrom as DATE)
								ELSE @LocalDateTime END),(Case 
								WHEN EffectiveTill IS NOT NULL AND EffectiveFrom IS NOT NULL AND CAST(EffectiveTill as date) > CAST(EffectiveFrom as date)
									THEN CAST(EffectiveTill as date)
								ELSE NULL END),@pUserId,@LocalDateTime,T.OtherValue1,1,0,ISNULL(T.OtherValue2,'')
		From @TCodes T
	END
	Else If @pCodeType = 'ATC'
	BEGIN
		;With ExistingCodes
		As
		(
		Select MAX(D.ATCCodeID) As ATCCodeID, MAX(D.EffectiveFrom) As EffectiveFrom,MAX(D.EffectiveTill) As EffectiveTill
		From
			(
			Select C.ATCCodeID, T.EffectiveFrom,T.EffectiveTill, C.ATCCode
			From ATCCodes C
			INNER JOIN @TCodes T ON C.ATCCode=T.Code
			Where C.IsActive=1
			) D
		GROUP by D.ATCCode
		)

		Update D SET D.CodeEffectiveTill = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END)
		,ModifiedBy = @pUserId, ModifiedDate = @LocalDateTime
		From ATCCodes D
		INNER JOIN ExistingCodes E ON D.ATCCodeID=E.ATCCodeID

		INSERT INTO ATCCodes(CodeTableNumber,ATCCode,CodeDescription,Purpose,DrugDescription,DrugName,SubCode,SubcodeDescription,IsActive,CreatedBy,
		CreatedDate,CodeEffectiveFrom,CodeEffectiveTill)
		Select T.TableNumber,T.Code,T.CodeDescription,T.OtherValue1,T.OtherValue2,T.CodeGroup,T.OtherValue4,T.OtherValue5,1,@pUserId,@LocalDateTime,(Case 
								When EffectiveFrom IS NOT NULL THEN CAST(EffectiveFrom as DATE)
								ELSE @LocalDateTime END),(Case 
								WHEN EffectiveTill IS NOT NULL AND EffectiveFrom IS NOT NULL AND CAST(EffectiveTill as date) > CAST(EffectiveFrom as date)
									THEN CAST(EffectiveTill as date)
								ELSE NULL END)
		From @TCodes T
	END
END
GO


