IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocImportBillingCodes') 
  DROP PROCEDURE SprocImportBillingCodes;
 
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
	DECLARE @LocalDateTime datetime, @TimeZone nvarchar(50)

	--Get the local time based on Time ZONE of the current facility
	SET @TimeZone=(Select TimeZ from Facility Where Facilityid=@pFId)
	SET @LocalDateTime=(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,GETDATE()))


	
	---Now fetch the Code data from the tables
	IF @pCodeType = 'CPT'			--'3'
	BEGIN
		--Select T.* From @TCodes T
		--INNER JOIN CPTCodes C ON T.Code = C.CodeNumbering AND T.TableNumber = C.CodeTableNumber

		;With ExistingCodes
		As
		(
		Select TOP 1 T.CPTCodesId, T.EffectiveFrom,T.EffectiveTill
		From 
		(
		Select C.CPTCodesId, T.EffectiveFrom,T.EffectiveTill From CPTCodes C 
		INNER JOIN @TCodes T ON C.CodeNumbering = T.Code AND C.CodeTableNumber=T.TableNumber
		And ISNULL(C.IsActive,1)=1 And ISNULL(C.IsDeleted,0)=0
		--And ISNULL(CodeExpiryDate,@LocalDateTime) <= @LocalDateTime
		) T
		Order by T.EffectiveTill Desc
		)

		Update C SET C.CodeExpiryDate = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END), ModifiedDate = @LocalDateTime, ModifiedBy = @pUserId
		From CPTCodes C
		INNER JOIN ExistingCodes E ON C.CPTCodesId=E.CPTCodesId


		INSERT INTO CPTCodes (CodeTableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate
		,CodeGroup,CreatedBy,CreatedDate,CTPCodeRangeValue,IsActive,IsDeleted)
		Select T.TableNumber,T.Code,T.CodeDescription,T.Price,(Case 
								When EffectiveFrom IS NOT NULL THEN CAST(EffectiveFrom as DATE)
								ELSE @LocalDateTime END),(Case 
								WHEN EffectiveTill IS NOT NULL AND EffectiveTill > @LocalDateTime THEN CAST(EffectiveTill as date)
								ELSE @LocalDateTime END),T.CodeGroup,@pUserId,@LocalDateTime,T.OtherValue1,1,0
		From @TCodes T
	END

	ELSE if @pCodeType = 'HCPCS'	--'4'
	BEGIN
		;With ExistingCodes
		As
		(
		Select TOP 1 T.HCPCSCodesId, T.EffectiveFrom,T.EffectiveTill
		From 
		(
		Select C.HCPCSCodesId, T.EffectiveFrom,T.EffectiveTill From HCPCSCodes C 
		INNER JOIN @TCodes T ON C.CodeNumbering = T.Code AND C.CodeTableNumber=T.TableNumber
		And ISNULL(C.IsActive,1)=1 And ISNULL(C.IsDeleted,0)=0
		--And ISNULL(CodeExpiryDate,@LocalDateTime) <= @LocalDateTime
		) T
		Order by T.EffectiveTill Desc
		)

		Update C SET C.CodeExpiryDate = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END), ModifiedDate = @LocalDateTime, ModifiedBy = @pUserId
		From HCPCSCodes C
		INNER JOIN ExistingCodes E ON C.HCPCSCodesId=E.HCPCSCodesId


		INSERT INTO HCPCSCodes (CodeTableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate
		,CreatedBy,CreatedDate)
		Select T.TableNumber,T.Code,T.CodeDescription,T.Price,(Case 
								When EffectiveFrom IS NOT NULL THEN CAST(EffectiveFrom as DATE)
								ELSE @LocalDateTime END),(Case 
								WHEN EffectiveTill IS NOT NULL AND EffectiveTill > @LocalDateTime THEN CAST(EffectiveTill as date)
								ELSE @LocalDateTime END),@pUserId,@LocalDateTime
		From @TCodes T
	END

	ELSE if @pCodeType = 'DRUG'		--'5'
	BEGIN
		;With ExistingCodes
		As
		(
		Select TOP 1 D.Id, D.EffectiveFrom,D.EffectiveTill
		From 
			(
			Select  C.Id, T.EffectiveFrom,T.EffectiveTill	From Drug C 
			INNER JOIN @TCodes T ON C.DrugCode= T.Code AND C.DrugTableNumber=T.TableNumber
			--And DrugDeleteDate >= @LocalDateTime 
			AND DrugStatus NOT IN ('Deleted')
			) D
		Order by D.EffectiveTill Desc
		)

		Update D SET D.DrugDeleteDate = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END)
		From Drug D
		INNER JOIN ExistingCodes E ON D.Id=E.Id

		--'TableNumber,DrugCode,GenericName,PackageName,PricePharmacy,PricePublic,UnitPricePharmacy,UnitPricePublic,LastChangeOn,DeleteDate,Dosage,Strength'
		INSERT INTO DRUG (DrugTableNumber,DrugCode,DrugGenericName,DrugPackageName,DrugPricePharmacy,DrugPricePublic
		,DrugUnitPricePharmacy,DrugUnitPricePublic,DrugDeleteDate,DrugLastChange,DrugStatus)
		Select T.TableNumber,T.Code,T.CodeDescription,T.CodeGroup As PackageName,T.OtherValue1,T.OtherValue2
		,T.OtherValue3,T.OtherValue4,(Case 
								When EffectiveFrom IS NOT NULL THEN CAST(EffectiveFrom as DATE)
								ELSE @LocalDateTime END),(Case 
								WHEN EffectiveTill IS NOT NULL AND EffectiveTill > @LocalDateTime THEN CAST(EffectiveTill as date)
								ELSE @LocalDateTime END),'Active'
		From @TCodes T
	END
	Else If @pCodeType = 'Diagnosis' --'16'
	BEGIN
		;With ExistingCodes
		As
		(
		Select TOP 1 D.DiagnosisTableNumberId, D.EffectiveFrom,D.EffectiveTill
		From 
			(
			Select  C.DiagnosisTableNumberId, T.EffectiveFrom,T.EffectiveTill	
			From DiagnosisCode C 
			INNER JOIN @TCodes T ON C.DiagnosisCode1= T.Code AND C.DiagnosisTableNumber=T.TableNumber
			--And DrugDeleteDate >= @LocalDateTime
			AND ISNULL(C.IsDeleted,0)=0
			) D
		Order by D.EffectiveTill Desc
		)

		Update D SET D.DiagnosisEffectiveEndDate = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END)
		,ModifiedBy = @pUserId, ModifiedDate = @LocalDateTime
		From DiagnosisCode D
		INNER JOIN ExistingCodes E ON D.DiagnosisTableNumberId=E.DiagnosisTableNumberId


		INSERT INTO DiagnosisCode (DiagnosisTableNumber,DiagnosisCode1,ShortDescription,DiagnosisMediumDescription
		,DiagnosisFullDescription,DiagnosisEffectiveStartDate,DiagnosisEffectiveEndDate
		,CreatedBy,CreatedDate,IsDeleted,DiagnosisWeight)
		Select T.TableNumber,T.Code,T.CodeDescription,T.OtherValue1,T.OtherValue2,(Case 
								When EffectiveFrom IS NOT NULL THEN CAST(EffectiveFrom as DATE)
								ELSE @LocalDateTime END),(Case 
								WHEN EffectiveTill IS NOT NULL AND EffectiveTill > @LocalDateTime THEN CAST(EffectiveTill as date)
								ELSE @LocalDateTime END)
		,@pUserId,@LocalDateTime,0,T.OtherValue3
		From @TCodes T
	END
	Else If @pCodeType = 'DRG'		--'9'
	BEGIN
		;With ExistingCodes
		As
		(
		Select TOP 1 D.DRGCodesId, D.EffectiveFrom,D.EffectiveTill
		From 
			(
			Select C.DRGCodesId, T.EffectiveFrom,T.EffectiveTill	
			From DRGCodes C 
			INNER JOIN @TCodes T ON C.CodeNumbering= T.Code AND C.CodeTableNumber=T.TableNumber
			AND ISNULL(C.IsDeleted,0)=0 And ISNULL(IsActive,1)=1
			) D
		Order by D.EffectiveTill Desc
		)

		Update D SET D.CodeEffectiveDate = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END)
		,ModifiedBy = @pUserId, ModifiedDate = @LocalDateTime
		From DRGCodes D
		INNER JOIN ExistingCodes E ON D.DRGCodesId=E.DRGCodesId


		INSERT INTO DRGCodes (CodeTableNumber,CodeNumbering,CodeDescription,CodePrice,CodeEffectiveDate,CodeExpiryDate
		,CreatedBy,CreatedDate,CodeDRGWeight,IsActive,IsDeleted)
		Select T.TableNumber,T.Code,T.CodeDescription,T.Price,@LocalDateTime,NULL,@pUserId,@LocalDateTime,T.OtherValue1,1,0
		From @TCodes T
	END
	Else If @pCodeType = 'ATC'
	BEGIN
		;With ExistingCodes
		As
		(
		Select TOP 1 D.ATCCodeID, D.EffectiveFrom,D.EffectiveTill
		From 
			(
			Select C.ATCCodeID, T.EffectiveFrom,T.EffectiveTill	
			From ATCCodes C
			INNER JOIN @TCodes T ON C.Code= T.Code
			Where C.IsActive=1
			) D
		Order by D.EffectiveTill Desc
		)

		Update D SET D.CodeEffectiveTill = (Case 
								When E.EffectiveFrom IS NOT NULL THEN CAST(E.EffectiveFrom as DATE) 
								ELSE @LocalDateTime - 1 END)
		,ModifiedBy = @pUserId, ModifiedDate = @LocalDateTime
		From ATCCodes D
		INNER JOIN ExistingCodes E ON D.ATCCodeID=E.ATCCodeID

		INSERT INTO ATCCodes(Code,ATCCode,CodeDescription,DrugDescription,DrugName,SubCode,SubcodeDescription,IsActive)
		Select T.Code,T.OtherValue1,T.CodeDescription,T.OtherValue2,T.OtherValue3,T.OtherValue4,T.OtherValue5,1
		From @TCodes T
	END
END





