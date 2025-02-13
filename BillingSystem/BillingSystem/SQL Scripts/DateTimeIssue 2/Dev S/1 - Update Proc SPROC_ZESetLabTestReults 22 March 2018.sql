IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ZESetLabTestReults')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ZESetLabTestReults
GO
/****** Object:  StoredProcedure [dbo].[SPROC_ZESetLabTestReults]    Script Date: 22-03-2018 11:48:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_ZESetLabTestReults]
(  
	@pXMLIN nvarchar(max)
)

AS
BEGIN

Declare @XMLIN xml
Declare @Txml table(TDBID int,TCode int, TName nvarchar(100), TSpecimen nvarchar(50),TGender nvarchar(10),TAgeFrom nvarchar(50),TAgeTo nvarchar(50), TMeasurement nvarchar(50), 
					TLR numeric(18,4),THR numeric(18,4),TGF numeric(18,4),TGT numeric(18,4),TCF numeric(18,4),TCT numeric(18,4),TBF numeric(18,4),TBT numeric(18,4))

Declare @CurrentDate datetime=(Select dbo.GetCurrentDatetimeByEntity(0))

Set @XMLIN = @pXMLIN

 -- insert into [dbo].[ZEDBManualXML] (DBXML) Select @XMLIN

	insert into @Txml
	Select C.H.value('DBID[1]','int'),C.H.value('DBCD[1]','int'),C.H.value('DBNAME[1]','nvarchar(100)'), C.H.value('SPE[1]','nvarchar(50)'), C.H.value('GEN[1]','nvarchar(10)'), 
				C.H.value('AF[1]','nvarchar(50)'),C.H.value('AT[1]','nvarchar(50)'),C.H.value('MEA[1]','nvarchar(50)'),
				C.H.value('LR[1]','numeric(18,4)'),C.H.value('HR[1]','numeric(18,4)'),C.H.value('GF[1]','numeric(18,4)'),C.H.value('GT[1]','numeric(18,4)'),
				C.H.value('CF[1]','numeric(18,4)'),C.H.value('CT[1]','numeric(18,4)'),C.H.value('BF[1]','numeric(18,4)'),C.H.value('BT[1]','numeric(18,4)')
	from @XMLIN.nodes('//RData') C(H) 


	update LTR Set LTR.LabTestResultCPTCode = Tx.TCode, LTR.LabTestResultTestName = Tx.TName, LTR.LabTestResultSpecimen= Tx.TSpecimen, LTR.LabTestResultGender = Tx.TGender,
				LTR.LabTestResultAgeFrom = Tx.TAgeFrom, LTR.LabTestResultAgeTo = Tx.TAgeTo, LTR.LabTestResultMeasurementValue = TX.TMeasurement,
				LTR.LabTestResultLowRangeResult = Tx.TLR, LTR.LabTestResultHighRangeResult = Tx.THR, LTR.LabTestResultGoodFrom = Tx.TGF, LTR.LabTestResultGoodTo = Tx.TGT,
				LTR.LabTestResultCautionFrom = Tx.TCF, LTR.LabTestResultCautionTo = Tx.TCT, LTR.LabTestResultBadFrom = TX.TBF, LTR.LabTestResultBadTo = Tx.TBT
				from LabTestResult LTR inner join @Txml TX on Tx.TDBID = LTR.LabTestResultID and Tx.TDBID > 0 and Tx.TName <> 'X';

	Insert into LabTestResult ( LabTestResultTableNumber, LabTestResultTableName, LabTestResultCPTCode, LabTestResultTestName, LabTestResultSpecimen, 
                         LabTestResultGender, LabTestResultAgeFrom, LabTestResultAgeTo, LabTestResultMeasurementValue, LabTestResultLowRangeResult, 
                         LabTestResultHighRangeResult, LabTestResultGoodFrom, LabTestResultGoodTo, LabTestResultCautionFrom, LabTestResultCautionTo, LabTestResultBadFrom, 
                         LabTestResultBadTo, IsDeleted,  CreatedBy, CreatedDate)
	Select '1001','Abu Dhabi',TCode,TName,TSpecimen,TGender,TAgeFrom,TAgeTo,TMeasurement,TLR,THR,TGF,TGT,TCF,TCT,TBF,TBT,0,100,@CurrentDate from @Txml where TDBID = 0

	Delete from LabTestResult Where LabTestResultID in (Select TDBID from @Txml where TName = 'X')
END











