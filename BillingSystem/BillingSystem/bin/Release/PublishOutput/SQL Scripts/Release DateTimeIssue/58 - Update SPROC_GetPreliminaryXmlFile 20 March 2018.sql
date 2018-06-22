IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetPreliminaryXmlFile')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetPreliminaryXmlFile
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetPreliminaryXmlFile]    Script Date: 21-03-2018 11:21:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetPreliminaryXmlFile]    -- [SPROC_GetPreliminaryXmlFile] 4,1371
(
	@pFacilityId int =8,
	@pBillHeaderId int =128
)
AS
BEGIN
Declare @XMLVIEW Table (
ViewID INT IDENTITY(1, 1) primary key,
SenderID nvarchar(50),ReceiverID nvarchar(50),TransactionDate datetime,RecordCount int,DispositionFlag nvarchar(50),ID int,
IDPayer nvarchar(50),ProviderID nvarchar(50),PaymentReference nvarchar(50),DateSettlement datetime,FacilityLicNumber nvarchar(50),ID2 nvarchar(20),Start Datetime,
EncounterID int, BillActivityID int,ActivityType nvarchar(20),DiagnosisCode nvarchar(20),[Type] nvarchar(20),Code nvarchar(20),Quantity numeric(18,2),Net numeric(18,2),List numeric(18,2),OrderingClinician nvarchar(50),
Clinician nvarchar(50),PriorAuthorizationID nvarchar(50),OrderingClinicianID int,BillHeaderID int,OrderedDate datetime,AdminstratingClinicianID int,AuthorizationCode nvarchar(50),
Gross numeric(18,2),PatientShare numeric(18,2), PaymentAmount numeric(18,2),MCDiscount numeric(18,2)
)
DECLARE @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))

Declare @RecordCount int = 0, @XMLVIEWID int,@FetchStatus int =0,@ID2ValueStr nvarchar(20);
--Select @RecordCount=Count(1) from [dbo].[BillActivity] BA where  
--Where BA.BillHeaderID = 
--(Select BillHeaderId from BillHeader where BillHeader.Status in (55,105,155) and FacilityID = 8)
--and Status is NULL and ProviderID = @FacilityNumber;

(Select @RecordCount = Count(1) from BillHeader where BillHeader.Status in (55,105,155) and FacilityID = @pFacilityId)

--SET IDENTITY_INSERT @XMLVIEW ON

Insert into @XMLVIEW (SenderID,ReceiverID,TransactionDate,RecordCount,DispositionFlag,ID,IDPayer,ProviderID,PaymentReference,DateSettlement,FacilityLicNumber,ID2,Start,EncounterID,
BillActivityID,ActivityType,DiagnosisCode,[Type],Code,Quantity,Net,List,OrderingClinician,Clinician,PriorAuthorizationID,OrderingClinicianID,BillHeaderID,OrderedDate,AdminstratingClinicianID,
AuthorizationCode,Gross,PatientShare,PaymentAmount,MCDiscount)

Select 
FC.SenderID as 'SenderID',
(Select TOP 1 (ExternalValue2) from BillingSystemParameters where FacilityNumber =FC.FacilityNumber) 'ReceiverID',
@LocalDateTime  'TransactionDate',
@RecordCount as 'RecordCount',
'PRODUCTION' as 'DispositionFlag', 
BA.BillHeaderID as 'ID',
(select MAX(InsuranceCompanyLicenseNumber) from InsuranceCompany where InsuranceCompanyId = BH.PayerID) IDPayer,
FC.FacilityLicenseNumber ProviderID,
'' PaymentReference,
null DateSettlement,
FC.FacilityLicenseNumber 'FacilityLicNumber',
'' 'ID2',
BA.ActivityStartDate 'Start',
BA.EncounterID,
BA.BillActivityID,
BA.ActivityType,
BA.DiagnosisCode,
BA.ActivityType 'Type', 
BA.ActivityCode 'Code',
BA.QuantityOrdered 'Quantity',
ISNULL(BA.PatientShare,0) + ISNULL(BA.PayerShareNet,0) 'Net',
ISNULL(BA.PatientShare,0) + ISNULL(BA.PayerShareNet,0) 'List',
Case when PY.PhysicianLicenseNumber is null THEN PY1.PhysicianLicenseNumber ELSE PY.PhysicianLicenseNumber END 'OrderingClinician',
Case when PY1.PhysicianLicenseNumber is null THEN PY.PhysicianLicenseNumber ELSE PY1.PhysicianLicenseNumber END  'Clinician',
--PY1.PhysicianLicenseNumber 'Clinician',
(Select MAX(A.AuthorizationCode) from  [Authorization] A where  A.EncounterID = BA.EncounterID) as 'PriorAuthorizationID' ,
BA.OrderingClinicianID,
BA.BillHeaderID,
BA.OrderedDate,
BA.AdminstratingClinicianID,
BA.AuthorizationCode,
ISNULL(BA.PatientShare,0) + ISNULL(BA.PayerShareNet,0) 'Gross',
BA.PatientShare 'PatientShare',
BA.PayerShareNet 'PaymentAmount',
BA.MCDiscount
from BillActivity BA
inner join Patientinfo P on P.PatientID = BA.PatientID
inner join Encounter E on E.EncounterID = BA.EncounterID
inner join BillHeader BH on BH.BillHeaderID = BA.BillHeaderID
inner join Facility FC on FC.FacilityId = BA.FacilityID
LEFT OUTER JOIN Physician PY on PY.UserId = BA.OrderingClinicianID
LEFT OUTER JOIN Physician PY1 on PY1.UserId = BA.AdminstratingClinicianID
Where BA.BillHeaderID = @pBillHeaderId and BA.FacilityID = @pFacilityId

--SET IDENTITY_INSERT @XMLVIEW OFF
Declare QD1 Cursor for
(
   Select ViewID from @XMLVIEW 
)
	
Open QD1;
Fetch Next from QD1 into @XMLVIEWID;


While @@FETCH_STATUS = 0
BEGIN
SET @FetchStatus = @FetchStatus +1;
Set @ID2ValueStr = CASE WHEN LEN(@FetchStatus) = 1 THEN '0'+Cast(@FetchStatus as nvarchar(20)) ELSE Cast(@FetchStatus as nvarchar(20)) END
Declare @ID2Value nvarchar(25) = Cast(@pBillHeaderId as nvarchar(20)) + '-'+@ID2ValueStr

Update @XMLVIEW Set ID2 = @ID2Value Where ViewID = @FetchStatus
	
Fetch Next from QD1 into @XMLVIEWID;
END -- While Ends

--- CleanUP - STARTS
Close QD1;
Deallocate QD1;

Select * from @XMLVIEW

END





GO


