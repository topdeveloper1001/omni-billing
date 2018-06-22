IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'XMLRemittanceAdviceParser')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE XMLRemittanceAdviceParser
GO

/****** Object:  StoredProcedure [dbo].[XMLRemittanceAdviceParser]    Script Date: 3/22/2018 7:50:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Proc [dbo].[XMLRemittanceAdviceParser]
(          
	@XMLIN XML,
	@FullPath nvarchar(250),
	@CorporateID int,
	@FacilityID int,
	@SuccessFlag Bit Output
)
AS
BEGIN 

		Declare @CurrentDate datetime = (Select dbo.GetCurrentDatetimeByEntity(@FacilityID))

declare  @FileNewID bigint
Declare @SuccessFlag_New int = 1

	--- Success Flag - It will be set to Zero if Failed
	Set @SuccessFlag = 0
	Declare @RecordCounts int  =0
	Declare @ClaimsCounts int  =0
--set language british
BEGIN TRY
	Select Cast(Replace(C.H.value('TransactionDate[1]','nvarchar(20)'),'T',' ') as Datetime) as 'TransactionDate'
	from @XMLIN.nodes('//Header') C(H);
	set language Us_english
END TRY
BEGIN CATCH
set language british
END CATCH



---- FILE HEADER -- STARTS

/*
Set the File ID as Auto-Identity Column in the XFileHeader Table due to the mis-matching in the XFileHeader and XFileXML tables. 
To achieve this, remove the column FileID in the below insert query,
*/

/* Changes on 13 September, 2017 start here */
--INSERT INTO [dbo].[XFileHeader] ([FileID],[XPath],[FileType],[SenderID],[ReceiverID],[TransactionDate],[RecordCount],[DispositionFlag],[Status],[SentDate],[ModifiedBy],[ModifiedDate],[CorporateID],[FacilityID])

--Select @FileNewID,@FullPath as 'FilePath','IN' as 'FileType',C.H.value('SenderID[1]','nvarchar(50)') as 'SenderID',C.H.value('ReceiverID[1]','nvarchar(50)') as 'ReceiverID',
--	CONVERT(VARCHAR(16),Replace(C.H.value('TransactionDate[1]','nvarchar(20)'),'T',' '),121) as 'TransactionDate',C.H.value('RecordCount[1]','nvarchar(20)') as 'RecordCount',
--	C.H.value('DispositionFlag[1]','nvarchar(50)') as 'DispositionFlag','0' as 'Status',@CurrentDate as 'SentDate',1 as 'ModifiedBy',@CurrentDate as 'ModifiedDate',@CorporateID,@FacilityID
--from @XMLIN.nodes('//Header') C(H);

INSERT INTO [dbo].[XFileHeader] ([XPath],[FileType],[SenderID],[ReceiverID],[TransactionDate],[RecordCount],[DispositionFlag],[Status],[SentDate],[ModifiedBy],[ModifiedDate],[CorporateID],[FacilityID])

Select @FullPath as 'FilePath','IN' as 'FileType',C.H.value('SenderID[1]','nvarchar(50)') as 'SenderID',C.H.value('ReceiverID[1]','nvarchar(50)') as 'ReceiverID',
	CONVERT(VARCHAR(16),Replace(C.H.value('TransactionDate[1]','nvarchar(20)'),'T',' '),121) as 'TransactionDate',C.H.value('RecordCount[1]','nvarchar(20)') as 'RecordCount',
	C.H.value('DispositionFlag[1]','nvarchar(50)') as 'DispositionFlag','0' as 'Status',@CurrentDate as 'SentDate',1 as 'ModifiedBy',@CurrentDate as 'ModifiedDate',@CorporateID,@FacilityID
from @XMLIN.nodes('//Header') C(H);

Set @FileNewID =SCOPE_IDENTITY();

/* Changes on 13 September, 2017 end here */




--- Insert the XML file into Table for Archive purpose

If @@ROWCOUNT > 0
Begin
	INSERT INTO [dbo].[XFileXML]
           ([FileXMLID]
           ,[XFileXML]
           ,[ModifiedDate])
		   Select @FileNewID,@XMLIN,@CurrentDate

-- Set @FileNewID = SCOPE_IDENTITY();
	--What --Added the Dulplicate Check for the file by three Parameters :  [SenderID],[ReceiverID],[TransactionDate] Combinatio of these three must be unique
	--Why -- Because same file is being uploaded by the user then it will corrupt the data
	--When -- 7th April 2016,
	--Who -- Shashank Awasthy
	IF Exists (Select Top (1) [FileID]  From [XFileHeader] Where [FileID] <> @FileNewID and
	[SenderID] = (Select [SenderID] from [XFileHeader] where [FileID] = @FileNewID) AND
	[ReceiverID] = (Select [ReceiverID] from [XFileHeader] where [FileID] = @FileNewID) And
	[TransactionDate] =(Select [TransactionDate] from [XFileHeader] where [FileID] = @FileNewID)
	AND CorporateID = @CorporateID and FacilityID = @FacilityID)
	BEGIN
		Set @SuccessFlag_New = 2 -- already exists Flag
	END
---- FILE HEADER -- ENDS
 

---- FILE DETAILS - CLAIM-Encounter-Diagnosis-Activity-Plan -- STARTS

INSERT INTO [dbo].[XAdviceXMLParsedData] ([XAFileHeaderFileID],[XACClaimID],[XACIDPayer],[XACProviderID],
			[XACDenialCode],[XACPaymentReference],
			[XACDateSettlement],[XAEFacilityID],
			[XAAActivityID],[XAAStart],[XAAType],
			[XaACode],[XaAQuantity] ,[XAANet],
			[XAAOrderingClinician],[XAAPriorAuthorizationID],
		    [XAAGross],[XAAPatientShare],
			[XAAPaymentAmount],[XAADenialCode],[XAModifiedBy],[XAModifiedDate],CorporateID,FacilityID)


Select @FileNewID,C.E.value('../ID[1]','nvarchar(50)') as 'ClaimID',C.E.value('../IDPayer[1]','nvarchar(50)') as 'PayerID',C.E.value('../ProviderID[1]','nvarchar(50)') as 'ProviderID',
		C.E.value('../DenialCode[1]','nvarchar(50)') as 'CDenialCode',C.E.value('../PaymentReference[1]','nvarchar(50)') as 'PaymentReference',
		CONVERT(VARCHAR(16),Replace(C.E.value('../DateSettlement[1]','nvarchar(20)'), 'T',' '),121) as 'DateSettlement',C.E.value('../Encounter[1]/FacilityID[1]','nvarchar(50)') as 'FacilityID',
		C.E.value('ID[1]','nvarchar(20)') as 'ActivityID',CONVERT(VARCHAR(16),Replace(C.E.value('Start[1]','nvarchar(20)'), 'T',' '),121) as 'ActivityStart',C.E.value('Type[1]','nvarchar(50)') as 'ActivityType',
		C.E.value('Code[1]','nvarchar(50)') as 'ActivityCode',C.E.value('Quantity[1]','nvarchar(20)') as 'Quantity',C.E.value('Net[1]','nvarchar(20)') as 'ActivityNet',
		C.E.value('OrderingClinician[1]','nvarchar(50)') as 'OrderingClinician',C.E.value('PriorAuthorizationID[1]','nvarchar(50)') as 'PriorAuthorizationID',
		C.E.value('Gross[1]','nvarchar(20)') as 'Gross',C.E.value('PatientShare[1]','nvarchar(20)') as 'PatientShare',
		C.E.value('PaymentAmount[1]','nvarchar(20)') as 'PaymentAmount',C.E.value('DenialCode[1]','nvarchar(50)') as 'ADenialCode',1,@CurrentDate,@CorporateID,@FacilityID
		from @XMLIN.nodes('//Claim/Activity') C(E)

---- FILE DETAILS - CLAIM-Encounter-Diagnosis-Activity-Plan -- STARTS
---- Failed


Set @SuccessFlag_New = CAST(isnull(@SuccessFlag_New,1) as int)

SET @RecordCounts =(Select ISNULL(Cast([RecordCount] as int),0) from [XFileHeader] where [FileID] =@FileNewID)
SET @ClaimsCounts  = (Select ISNULL(COUNT(Distinct([XACClaimID])),0) from [XAdviceXMLParsedData] where [XAFileHeaderFileID] =@FileNewID)

If @SuccessFlag_New <> 2 -- already exists Flag Check if not exist then move furthur else error 
	Set @SuccessFlag_New =  Case when @RecordCounts = @ClaimsCounts Then 1 ELSE 0 END

if @SuccessFlag_New = 1  --- Successfully PArsed and Check implemented
Begin
	Update [dbo].[XFileHeader]  Set [Status] = '0' where FileId =@FileNewID
END
ELSE if @SuccessFlag_New = 2 --- Successfully PArsed and duplicate file
Begin
	Update [dbo].[XFileHeader]  Set [Status] = '3' where FileId =@FileNewID
	Delete From [XAdviceXMLParsedData] Where [XAFileHeaderFileID] =@FileNewID
END
ELse
Begin
	Update [dbo].[XFileHeader]  Set [Status] = '2' where FileId =@FileNewID
	Delete From [XAdviceXMLParsedData] Where [XAFileHeaderFileID] =@FileNewID
	Set @SuccessFlag_New =  3 -- Records Count does not match
END

--Set @SuccessFlag = 0

End
--set language Us_english

Select CAST(isnull(@SuccessFlag_New,1) as Nvarchar(10)) AS 'SuccessFlag',@RecordCounts as 'RecordCounts',@ClaimsCounts as 'ClaimCounts'
--- Check Purpose ONLY

--Select * from XFileHeader order by 1  desc
-- Select * from XFileXML order by 1  desc
-- Select * from [XAdviceXMLParsedData] order by 1  desc

END





GO


