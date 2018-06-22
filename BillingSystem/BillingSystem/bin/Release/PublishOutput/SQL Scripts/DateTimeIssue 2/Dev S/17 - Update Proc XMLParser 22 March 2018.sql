IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'XMLParser')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE XMLParser
GO

/****** Object:  StoredProcedure [dbo].[XMLParser]    Script Date: 22-03-2018 18:56:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--use XMLParse
--use USE [BillingSystem6OCT2014]
-- =============================================
-- Author:		<BB - Spadez>
-- Create date: <DEc- 2014>
-- Description:	<For Parsing INcoming Files from Third Parties - Format Claim Submission
-- =============================================
    
CREATE Proc [dbo].[XMLParser]        
(       
@pCID int,
@XMLIN XML,
@FullPath nvarchar(250),
@pFID int,
@SuccessFlag Bit Output,
@UBatchNumber Nvarchar(MAX),
@pExecuteDetails bit=1,
@pLoggedInUserId bigint=0
)
AS       
      
BEGIN
	Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFID))
	Declare @FileNewID bigint,@SysBatchNumber nvarchar(MAX),@UserBatchNumber nvarchar(MAX)
	Declare @SuccessFlag_New int = -1

 Set @pCID = isnull(@pCID,0)
 Set @pFID = isnull(@pFID,0)
 
 If @pFID > 0 
 Begin
---- FILE HEADER -- STARTS

INSERT INTO [dbo].[TPFileHeader] ([FileName],[FileType],[SenderID],[ReceiverID],[TransactionDate],[RecordCount],[DispositionFlag],[Status],[SentDate],[CorporateID],[FacilityID],[FileBatchNumber],[SystemBatchNumber])

Select @FullPath as 'FilePath','IN' as 'FileType',C.H.value('SenderID[1]','nvarchar(50)') as 'SenderID',C.H.value('ReceiverID[1]','nvarchar(50)') as 'ReceiverID',
	C.H.value('TransactionDate[1]','nvarchar(20)') as 'TransactionDate',C.H.value('RecordCount[1]','nvarchar(20)') as 'RecordCount',
	C.H.value('DispositionFlag[1]','nvarchar(50)') as 'DispositionFlag','FL0' as 'Status',@LocalDateTime as 'SentDate',@pCID,@pFID,null,null
from @XMLIN.nodes('//Header') C(H)  

Set @FileNewID = SCOPE_IDENTITY();

	--- Bugs fixes Before XMl billing Demo -- TMS Issue Number :	21961
	--What --Added the Dulplicate Check for the file by three Parameters :  [SenderID],[ReceiverID],[TransactionDate] Combinatio of these three must be unique
	--Why -- Because if same file is being uploaded by the user then it will corrupt the data
	--When -- 3rd March 2016,
	--Who -- Shashank Awasthy
	IF Exists (Select Top (1) TPFileHEaderId  From [TPFileHeader] Where TPFileHEaderId <> @FileNewID and
	[SenderID] = (Select [SenderID] from [TPFileHeader] where TPFileHEaderId = @FileNewID) AND
	[ReceiverID] = (Select [ReceiverID] from [TPFileHeader] where TPFileHEaderId = @FileNewID) And
	[TransactionDate] =(Select [TransactionDate] from [TPFileHeader] where TPFileHEaderId = @FileNewID)
	AND CorporateID = @pCID and FacilityID = @pFID)
	BEGIN
		Set @SuccessFlag_New = 2 -- already exists Flag
	END

--- Added By Shashank---------- TO Add the Unique number to Batch Number
;WITH Split_Names (xmlname)
AS
(
    SELECT
    CONVERT(XML,'<Names><name>'  
    + REPLACE(@FullPath,'/', '</name><name>') + '</name></Names>') AS xmlname
)

 (Select  @SysBatchNumber = ( SUbstring(xmlname.value('/Names[1]/name[7]','varchar(100)'),1,2)+
CAST(( SELECT LEFT(Val,PATINDEX('%[^0-9]%', Val+'a')-1) from(
    SELECT SUBSTRING(xmlname.value('/Names[1]/name[4]','varchar(100)'), PATINDEX('%[0-9]%', xmlname.value('/Names[1]/name[4]','varchar(100)')), LEN(xmlname.value('/Names[1]/name[4]','varchar(100)'))) Val
)X) as nvarchar(10))+
CAST(( SELECT LEFT(Val,PATINDEX('%[^0-9]%', Val+'a')-1) from(
    SELECT SUBSTRING(xmlname.value('/Names[1]/name[5]','varchar(100)'), PATINDEX('%[0-9]%', xmlname.value('/Names[1]/name[5]','varchar(100)')), LEN(xmlname.value('/Names[1]/name[5]','varchar(100)'))) Val
)X) as nvarchar(10)) + 
CAST(( SELECT LEFT(Val,PATINDEX('%[^0-9]%', Val+'a')-1) from(
    SELECT SUBSTRING(xmlname.value('/Names[1]/name[6]','varchar(100)'), PATINDEX('%[0-9]%', xmlname.value('/Names[1]/name[6]','varchar(100)')), LEN(xmlname.value('/Names[1]/name[6]','varchar(100)'))) Val
)X) as nvarchar(10)) + '_'+ Cast(@FileNewID as nvarchar(10)))
 FROM Split_Names)


 --Set @UserBatchNumber = CASE WHEN @UBatchNumber <> '' and @UBatchNumber is not null then @UBatchNumber ELSE @SysBatchNumber END
 Set @UserBatchNumber = @SysBatchNumber;
 ----ADDED END on AUGUST 13 2015


Update [dbo].[TPFileHeader]  Set FileBatchId = @FileNewID,SystemBatchId = @FileNewID,[FileBatchNumber] =@UserBatchNumber ,[SystemBatchNumber] = @SysBatchNumber 
where TPFileHeaderID =@FileNewID
--- Insert the XML file into Table for Archive purpose

INSERT INTO [dbo].[TPFileXML]
           ([TPFileXMLID]
           ,[XFileXML]
           ,[ModifiedDate])
		   Select @FileNewID,@XMLIN,@LocalDateTime


---- FILE HEADER -- ENDS
 

---- FILE DETAILS - CLAIM-Encounter-Diagnosis-Activity-Plan -- STARTS





INSERT INTO [dbo].[TPXMLParsedData] ([TPFileID],[CClaimID],[CMemberID],[CPayerID],[CProviderID],[CEmiratesIDNumber],[CGross],[CPatientShare],[CNet]
           ,[EFacilityID],[EType],[EPatientID],[EligibilityIDPayer],[EStart],[EEnd],[EStartType],[EEndType],[DType],[DCode],[AStart],[AType],[ACode],[AQuantity] ,[ANet]
           ,[AOrderingClinician],[AExecutingClinician],[APriorAuthorizationID],[CNPackageName],[ModifiedDate],[OMCorporateID],[OMFacilityID],[FileBatchId],
		   [FileBatchNumber],[SystemBatchId],[SystemBatchNumber])
Select @FileNewID,C.E.value('../ID[1]','nvarchar(50)') as 'ID',C.E.value('../MemberID[1]','nvarchar(50)') as 'MemberID',C.E.value('../PayerID[1]','nvarchar(50)') as 'PayerID',
	   C.E.value('../ProviderID[1]','nvarchar(50)') as 'ProviderID'
	   ,('XML' + LTRIM(RTRIM(C.E.value('../EmiratesIDNumber[1]','nvarchar(100)')))) as 'EmiratesIDNumber',
	   C.E.value('../Gross[1]','nvarchar(20)') as 'Gross',C.E.value('../PatientShare[1]','nvarchar(20)') as 'PatientShare'
	   ,LTRIM(RTRIM(C.E.value('../Net[1]','nvarchar(20)'))) as 'NET'
	   ,C.E.value('../Encounter[1]/FacilityID[1]','nvarchar(50)') as 'FacilityID',C.E.value('../Encounter[1]/Type[1]','nvarchar(50)') as 'EncounterType'
	   ,LTRIM(RTRIM(C.E.value('../Encounter[1]/PatientID[1]','nvarchar(50)'))) as 'PatientID',C.E.value('../Encounter[1]/EligibilityIDPayer[1]','nvarchar(50)') as 'EligibilityIDPayer',
		C.E.value('../Encounter[1]/Start[1]','nvarchar(20)') as 'StartDate',
		C.E.value('../Encounter[1]/End[1]','nvarchar(20)') as 'EndDate',C.E.value('../Encounter[1]/StartType[1]','nvarchar(50)') as 'StartType',C.E.value('../Encounter[1]/EndType[1]','nvarchar(50)') as 'EndType',
		C.E.value('../Diagnosis[1]/Type[1]','nvarchar(50)') as 'DiagnosisType',C.E.value('../Diagnosis[1]/Code[1]','nvarchar(50)') as 'DiagnosisCode',
		C.E.value('Start[1]','nvarchar(25)') as 'ActivityStart',C.E.value('Type[1]','nvarchar(50)') as 'ActivityType',C.E.value('Code[1]','nvarchar(50)') as 'ActivityCode',
		C.E.value('Quantity[1]','nvarchar(20)') as 'Quantity',C.E.value('Net[1]','nvarchar(20)') as 'ActivityNet',
		C.E.value('OrderingClinician[1]','nvarchar(50)') as 'OrderingClinician',C.E.value('Clinician[1]','nvarchar(50)') as 'ExecutingClinician',
		C.E.value('PriorAuthorizationID[1]','nvarchar(50)') as 'PriorAuthorizationID',
		C.E.value('../Contract[1]/PackageName[1]','nvarchar(50)') as 'PackageName',@LocalDateTime,@pCID,@pFID,@FileNewID,@UserBatchNumber,@FileNewID,@SysBatchNumber
from @XMLIN.nodes('//Claim/Activity') C(E)   


INSERT INTO [dbo].[TPXMLDiagnosis] ([TPFileID], [DiagnosisType], [DiagnosisCode], [DRGCodeID], [ClaimID], [MemberID], [PayerID], [ProviderID], [EmiratesIDNumber])
Select @FileNewID, C.E.value('Type[1]','nvarchar(50)') as 'DiagnosisType', C.E.value('Code[1]','nvarchar(50)') as 'DiagnosisCode',
NULL, C.E.value('../ID[1]','nvarchar(50)') as 'ID', C.E.value('../MemberID[1]','nvarchar(50)') as 'MemberID',C.E.value('../PayerID[1]','nvarchar(50)') as 'PayerID',
C.E.value('../ProviderID[1]','nvarchar(50)') as 'ProviderID', ('XML' + LTRIM(RTRIM(C.E.value('../EmiratesIDNumber[1]','nvarchar(100)')))) as 'EmiratesIDNumber'
from @XMLIN.nodes('//Claim/Diagnosis') C(E) 



---- FILE DETAILS - CLAIM-Encounter-Diagnosis-Activity-Plan -- ENDS


--Set @SuccessFlag = isnull(@SuccessFlag,1)
Set @SuccessFlag_New = CAST(isnull(@SuccessFlag_New,1) as int)


--- Check for the Wrong counts Error---
Declare @RecordCounts int  = (Select ISNULL([RecordCount],0) from [TPFileHeader] where [FileBatchId] =@FileNewID)
Declare @ClaimsCounts int  = (Select COUNT(Distinct([CClaimID])) from [TPXMLParsedData] where [TPFileID] =@FileNewID)

--Set @SuccessFlag =  Case when @RecordCounts = @ClaimsCounts Then 1 ELSE 0 END
If @SuccessFlag_New <> 2 -- already exists Flag Check if not exist then perform the scrubing of the records else return status
	Set @SuccessFlag_New =  Case when @RecordCounts = @ClaimsCounts Then 1 ELSE 0 END

--- Check for the wrong counts ends
--Added by Shashank ---- Status will be comming from GLobalCodes with globalcodecategory value =14700
if @SuccessFlag_New = 1 
Begin
	Update [dbo].[TPFileHeader]  Set [Status] = 'FL2' where TPFileHeaderID =@FileNewID

	IF @pExecuteDetails=1
		Exec [dbo].[XMLParsedUpload] @pCID,@pFID,@pLoggedInUserId
END
ELSE if @SuccessFlag_New = 2
Begin
	Update [dbo].[TPFileHeader]  Set [Status] = 'FL1' where TPFileHeaderID =@FileNewID
	Delete From [TPXMLParsedData] Where [TPFileID] =@FileNewID
END
ELse
Begin
	Update [dbo].[TPFileHeader]  Set [Status] = 'FL0' where [TPFileHeaderID] =@FileNewID
	Delete From [TPXMLParsedData] Where [TPFileID] =@FileNewID
END
----Added by ENd on Aug 13 2015

End ---- End for FacilityID exists and is setup for passed in CorporateName
--- Check Purpose ONLY

--Select * from TPFileHeader order by 1  desc
-- Select * from TPFileXML order by 1  desc
-- Select * from TPXMLParsedData order by 1  desc

Select CAST(isnull(@SuccessFlag_New,1) as Nvarchar(10)) AS 'SuccessFlag'

END
GO


