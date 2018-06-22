IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_FindEClaim')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_FindEClaim
GO

/****** Object:  StoredProcedure [dbo].[SPROC_FindEClaim]    Script Date: 22-03-2018 19:51:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_FindEClaim]
(
	@pSearchString nvarchar(max) ='',
	@pClaimStatus nvarchar(50)  ='',
	@pDateFrom datetime = null, 
	@pDateTill Datetime =null,
	@pFID int =8, 
	@pCID int = 9,
	@pFileId int =139
)
AS
BEGIN

Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFID))
Set  @pDateFrom = ISNULL(@pDateFrom,DATEADD(month, DATEDIFF(month, 0, @CurrentDate), 0))
Set  @pDateTill = ISNULL(@pDateTill,EOMONTH(@CurrentDate))

Declare @tempTableFindClaim Table
(
	BillHeaderId int,EncounterId int,BillNumber nvarchar(100),PatientId int,FacilityId int,Corporateid int,PayerId nvarchar(50),Gross numeric(18,2),
	PatientShare numeric(18,2),PayerShareNet numeric(18,2),BillDate Datetime,[Status] nvarchar(50),EncounterNumber Nvarchar(50),PatientName nvarchar(100),
	InsuranceCompany nvarchar(50),GrossChargesSum  numeric(18,2),EncounterStatus nvarchar(100),IsAutoClosed int,BillHeaderStatus nvarchar(50),
	ActualPayerPayment numeric(18,2),VariancePayerPayment numeric(18,2),ActualPatientPayment numeric(18,2),VariancePatientPayment numeric(18,2),ClaimFileId int,RemittanceFileid int
)

Declare @tempTableToReturn Table
(
	BillHeaderId int,EncounterId int,BillNumber nvarchar(100),PatientId int,FacilityId int,Corporateid int,PayerId nvarchar(50),Gross numeric(18,2),
	PatientShare numeric(18,2),PayerShareNet numeric(18,2),BillDate Datetime,[Status] nvarchar(50),EncounterNumber Nvarchar(50),PatientName nvarchar(100),
	InsuranceCompany nvarchar(50),GrossChargesSum  numeric(18,2),EncounterStatus nvarchar(100),IsAutoClosed int,BillHeaderStatus nvarchar(50),
	ActualPayerPayment numeric(18,2),VariancePayerPayment numeric(18,2),ActualPatientPayment numeric(18,2),VariancePatientPayment numeric(18,2),ClaimFileId int,RemittanceFileid int
)

if(@pFileId <> 0)
BEGIN
		Insert INTO @tempTableFindClaim
		select BH.BillHeaderId,BH.EncounterId,BH.BillNumber,BH.PatientId,BH.FacilityId,BH.Corporateid,BH.PayerId,BH.Gross,ISNULL(BH.PatientSHare,0),ISNULL(BH.PayerShareNet,0),BH.BillDate,BH.[Status]
		,ENC.EncounterNumber,Pinfo.PersonFirstname + ' '+Pinfo.PersonLastname 'PatientName',
		(Select InsuranceCompanyName From InsuranceCompany where [InsuranceCompanyLicenseNumber] = BH.PayerId) 'InsuranceCompany',
		ISNULL(BH.PayerShareNet,0) + ISNULL(BH.patientSHare,0) 'GrossChargesSum',
		CASE WHEN BH.Status >= 60 THEN (Select [Description] from GlobalCodes where GlobalCodeValue = BH.[Status] and GlobalCodeCategoryValue = '14700') ELSE
		CASE WHEN (Select COUNT(1) from scrubHeader where BillHeaderID = BH.BillHeaderID) = 0 THEN 'Not Scrubbed' ELSE
		CASE WHEN (Select COUNT(1) from scrubHeader where BillHeaderID = BH.BillHeaderID) > 0 THEN 'Scrubbed' ELSE
		(Select Description from GlobalCodes where GlobalCodeValue = BH.[Status] and GlobalCodeCategoryValue = '14700') END  END END 'EncounterStatus',
		ISNULL(ENC.IsAutoClosed,0),
		Case when BH.Status <= 40 
		THEN 'NA'
		ELSE (Select GlobalCodeName from GlobalCodes where GlobalCodeValue = BH.[Status] and GlobalCodeCategoryValue = '14700') END,
		isnull(BH.PaymentAmount,0) 'ActualPayerPayment',
		ISNULL(BH.PayerShareNet,0) - isnull(BH.PaymentAmount,0) 'VariancePayerPayment',
		isnull(BH.PatientPayAmount,0) 'ActualPatientPayment',
		ISNULL(BH.PatientSHare,0) - isnull(BH.PatientPayAmount,0) 'VariancePatientPayment',
		ISNULL(BH.FileId,0) 'ClaimFileId',
		ISNULL(BH.ARFileId,0) 'RemiitanceFileid'
		 from BillHeader BH 
		 INNER JOIN ENCOUNTER ENC on ENC.EncounterId = BH.EncounterId
		 INNER JOIN PatientInfo Pinfo on Pinfo.PatientId = BH.PatientID
		 where BH.CorporateId = @pCID and BH.facilityId =@pFID 
		 Order by BH.BillHeaderId desc
END
ELSE
BEGIN
		Insert INTO @tempTableFindClaim
		select BH.BillHeaderId,BH.EncounterId,BH.BillNumber,BH.PatientId,BH.FacilityId,BH.Corporateid,BH.PayerId,BH.Gross,ISNULL(BH.PatientSHare,0),ISNULL(BH.PayerShareNet,0),BH.BillDate,BH.[Status]
		,ENC.EncounterNumber,Pinfo.PersonFirstname + ' '+Pinfo.PersonLastname 'PatientName',
		(Select InsuranceCompanyName From InsuranceCompany where [InsuranceCompanyLicenseNumber] = BH.PayerId) 'InsuranceCompany',
		ISNULL(BH.PayerShareNet,0) + ISNULL(BH.patientSHare,0) 'GrossChargesSum',
		CASE WHEN BH.Status >= 60 THEN (Select [Description] from GlobalCodes where GlobalCodeValue = BH.[Status] and GlobalCodeCategoryValue = '14700') ELSE
		CASE WHEN (Select COUNT(1) from scrubHeader where BillHeaderID = BH.BillHeaderID) = 0 THEN 'Not Scrubbed' ELSE
		CASE WHEN (Select COUNT(1) from scrubHeader where BillHeaderID = BH.BillHeaderID) > 0 THEN 'Scrubbed' ELSE
		(Select Description from GlobalCodes where GlobalCodeValue = BH.[Status] and GlobalCodeCategoryValue = '14700') END  END END 'EncounterStatus',
		ISNULL(ENC.IsAutoClosed,0),
		Case when BH.Status <= 40 
		THEN 'NA'
		ELSE (Select GlobalCodeName from GlobalCodes where GlobalCodeValue = BH.[Status] and GlobalCodeCategoryValue = '14700') END,
		isnull(BH.PaymentAmount,0) 'ActualPayerPayment',
		ISNULL(BH.PayerShareNet,0) - isnull(BH.PaymentAmount,0) 'VariancePayerPayment',
		isnull(BH.PatientPayAmount,0) 'ActualPatientPayment',
		ISNULL(BH.PatientSHare,0) - isnull(BH.PatientPayAmount,0) 'VariancePatientPayment',
		ISNULL(BH.FileId,0) 'ClaimFileId',
		ISNULL(BH.ARFileId,0) 'RemiitanceFileid'
		 from BillHeader BH 
		 INNER JOIN ENCOUNTER ENC on ENC.EncounterId = BH.EncounterId
		 INNER JOIN PatientInfo Pinfo on Pinfo.PatientId = BH.PatientID
		 where BH.CorporateId = @pCID and BH.facilityId =@pFID and
		 (Cast(BH.BillDate as Date) between Cast(@pDateFrom as Date) and Cast(@pDateTill as Date))
		 Order by BH.BillHeaderId desc
END


 If (@pFileId <> 0)
 BEGIN
 Insert into @tempTableToReturn
	Select * from @tempTableFindClaim
	 where (ClaimFileId = @pFileId or RemittanceFileid = @pFileId)
 END
 ELSE If (@pClaimStatus = 'Send')
 BEGIn
 Insert into @tempTableToReturn
	Select * from @tempTableFindClaim where 
	(@pClaimStatus = '' or EncounterStatus LIKE '%'+@pClaimStatus+'%')
	AND (@pSearchString = '' OR BillNumber LIKE '%'+@pSearchString+'%' OR PatientName LIKE '%' + @pSearchString+'%' OR EncounterNumber Like '%'+@pSearchString+'%' or InsuranceCompany Like '%'+@pSearchString+'%'
	or EncounterStatus Like '%'+@pSearchString+'%' or BillHeaderStatus Like '%'+@pSearchString+'%')
 END
 --Paid
 ELSE If (@pClaimStatus = 'Paid')
 BEGIn
 Insert into @tempTableToReturn
	Select * from @tempTableFindClaim where 
	[Status] > 60 
	AND (@pSearchString = '' OR BillNumber LIKE '%'+@pSearchString+'%' OR PatientName LIKE '%'+@pSearchString+'%' OR EncounterNumber Like '%'+@pSearchString+'%' or InsuranceCompany Like '%'+@pSearchString+'%'
	or EncounterStatus Like '%'+@pSearchString+'%' or BillHeaderStatus Like '%'+@pSearchString+'%')
 END
 ELSE
 BEGIN
	Insert into @tempTableToReturn
	Select * from @tempTableFindClaim where 
	(@pClaimStatus = '' or EncounterStatus = @pClaimStatus)
	AND (@pSearchString = '' OR BillNumber LIKE '%'+@pSearchString+'%' OR PatientName LIKE '%'+@pSearchString+'%' OR EncounterNumber Like '%'+@pSearchString+'%' or InsuranceCompany Like '%'+@pSearchString+'%'
	or EncounterStatus Like '%'+@pSearchString+'%' or BillHeaderStatus Like '%'+@pSearchString+'%' )
 END



 /*
WHO: Shashank Awasthy
WHEN: 18 April 2016
WHAT: Added the Total at the end of the listing
WHY: Requirement by the client
*/
IF EXISTS (Select 1 from @tempTableToReturn)
BEGIN
	--Insert into @tempTableToReturn 
	--Select 0,0,'TOTAL',0,@pFID,@pCID,'',SUM(ISNULL(Gross,0)),SUM(ISNULL(PatientShare,0)),SUM(ISNULL(PayerShareNet,0)),null,'','','',InsuranceCompany,SUM(ISNULL(GrossChargesSum,0)),'',0,'',
	--SUM(ISNULL(ActualPayerPayment,0)),SUM(ISNULL(VariancePayerPayment,0)),SUM(ISNULL(ActualPatientPayment,0)),SUM(ISNULL(VariancePatientPayment,0)),
	--case when @pFileId is null THEN null else @pFileId END,case when @pFileId is null THEN null else @pFileId END
	--from @tempTableToReturn
	--Group by InsuranceCompany
	--- Above query is to show the charges sum and Group by Insurance company,,,as client can ask this feature in the future for all listing case
	Insert into @tempTableToReturn 
	Select 0,0,'TOTAL',0,@pFID,@pCID,'',SUM(ISNULL(Gross,0)),SUM(ISNULL(PatientShare,0)),SUM(ISNULL(PayerShareNet,0)),null,'','','','',SUM(ISNULL(GrossChargesSum,0)),'',0,'',
	SUM(ISNULL(ActualPayerPayment,0)),SUM(ISNULL(VariancePayerPayment,0)),SUM(ISNULL(ActualPatientPayment,0)),SUM(ISNULL(VariancePatientPayment,0)),
	case when @pFileId is null THEN null else @pFileId END,case when @pFileId is null THEN null else @pFileId END
	from @tempTableToReturn
END

Select * from @tempTableToReturn
END





GO


