IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetXMLReport_InitialClaimErrorReport')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetXMLReport_InitialClaimErrorReport
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetXMLReport_InitialClaimErrorReport]    Script Date: 22-03-2018 20:10:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<SHashank,,Name>
-- Create date: <March 02-2016,,Daytime>
-- Description:	<Get the Report as per the client Requirements from the XML billing task list, Sheet name XStep 6,,>\
-- Only part missing is the calculate the % of Revenue with Errors
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetXMLReport_InitialClaimErrorReport]  --  SPROC_GetXMLReport_InitialClaimErrorReport  '2016-01-01','2016-03-21','0','0',1022,1056
(
@pStartDate datetime, 
@pEndDate datetime,
@pEncounterType nvarchar(50),
@pClinicalId nvarchar(10),
@pCId int,
@pFId int
)
AS
BEGIN
	

	Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@pFId))

--Declare @pStartDate datetime = Dateadd(dd,-3,@CurrentDate), @pEndDate datetime = @CurrentDate,@pEncounterType nvarchar(50) ='0',@pClinicalId nvarchar(10) ='0',
--@pCId int,@pFId int


--Select Tp.SentDate 'DateReceived',Tp.SystemBatchNumber 'BatchNumber',Tp.CorporateID,
--(Select top 1 CorporateName from Corporate where CorporateID = Tp.CorporateID) 'CorporateName',
--Tp.SenderID,TPXML.AExecutingClinician 'ClinicianID',
--(Select top 1 FacilityName from Facility where FacilityId = Tp.FacilityId) 'FacilityName',
--MAX(Tp.RecordCount) 'Claims',
--SUM(CAST(ISNULL(TpXML.CGross,0.00) as Numeric(18,2)))  'Gross',
--Count( DISTINCT(BHC.BillHeaderId)) 'ClaimsWithError',
--SUM(ISNULL(BHC.Gross,0.00)) 'GrossChargesWithError',
--CAST(CAST(MAX(Tp.RecordCount) / Count( DISTINCT(BHC.BillHeaderId)) as Numeric(18,2)) *100 as Numeric(18,2)) 'InitErrorPercentage',
--CAST(SUM(ISNULL(BHC.Gross,0.00)) / SUM(CAST(ISNULL(TpXML.CGross,0.00) as Numeric(18,2))) *100 as Numeric(18,2)) 'ErrorRevenuePercentage'
--From TPFileHeader Tp
--INNER JOIN TPXMLParsedData TpXML on TpXML.TPFileID = TP.TPFileHeaderID
--INNER JOIN BILLHEader BHC on BHC.BillHeaderId = TPXML.OMBillID
--Where 
--Tp.CorporateId = @pCId and (@pFId = 0 or Tp.FacilityID = @pFId)
--and Tp.SentDate between @pStartDate and @pEndDate 
--AND (@pEncounterType = '0' or TpXML.EType = @pEncounterType )
--and (@pClinicalId = '0' or TPXML.AExecutingClinician = @pClinicalId)
--AND BHC.Status in (50,55)
--Group by Tp.SentDate,Tp.SystemBatchNumber,Tp.CorporateID,Tp.FacilityId,Tp.SenderID,TPXML.AExecutingClinician,Tp.RecordCount,TP.TPFileHeaderID

Declare @InComingClaims table(CID int, FID int,TPFileID int, SentDate DateTime, BatchNumber Nvarchar(100), SenderID Nvarchar(100), BID int, 
AExecutingClinician nvarchar(20),RecordCount int,Gross Numeric(18,2), BillError int,GrossChargesWithError numeric(18,2),EncounterType nvarchar(20) )

---- Get Initial Data UpLoaded
insert into @InComingClaims
Select TH.CorporateID,TH.FacilityId,TH.TPFileHeaderID,Cast(TH.SentDate as DateTime) 'SentDate',TH.SystemBatchNumber,TH.SenderID,TD.OMBillID,TD.OMExecutingClincialID,
TH.RecordCount,Sum(Cast(TD.CGross as numeric(18,2))) 'Gross',0,0,MAX(TD.EType)
 from TPXMLParsedData TD
inner join TPFileHeader TH on TH.TPFileHeaderID = TD.TPFileID
Where 
TH.CorporateID = @pCId 
and (@pFId = 0 or TH.FacilityID = @pFId)
and (CAST(TH.SentDate as Date) Between CAST(@pStartDate as Date) and CAST(@pEndDate as Date))
and (@pClinicalId = '0' or TD.OMExecutingClincialID = @pClinicalId)
AND (@pEncounterType = '0' or TD.EType = @pEncounterType )
Group By TH.TPFileHeaderID,Cast(TH.SentDate as DateTime),TH.SystemBatchNumber,TH.CorporateID,TH.FacilityId,TH.SenderID,TD.OMBillID,TD.OMExecutingClincialID,TH.RecordCount;

---- Error Bills 
Update @InComingClaims Set BillError = 1 Where BID in (Select distinct BillHeaderID from ScrubHeader
 Where BillHeaderID in (Select Distinct BID from @InComingClaims) and Failed > 0 ) 

Update @InComingClaims Set GrossChargesWithError =  (CASE WHEN BillError = 1 THEN Gross ELSE 0 END)

---- Return Results
Select IC.SentDate 'DateReceived',IC.BatchNumber,C.CorporateID,C.CorporateName,IC.SenderID,IC.AExecutingClinician 'ClinicianID',F.FacilityName, 
IC.RecordCount 'Claims', sum(Gross) 'Gross', sum(BillError) 'ClaimsWithError', sum(GrossChargesWithError) 'GrossChargesWithError',
CAST((sum(CAST(BillError as numeric(18,2))) /Cast(Isnull(IC.RecordCount,1) as NUmeric(18,2)))*100 as NUmeric(18,2)) 'InitErrorPercentage',
 CAST((sum(GrossChargesWithError)/sum(Gross))*100 as NUmeric(18,2)) 'ErrorRevenuePercentage'
 from @InComingClaims IC
inner join Corporate C on C.CorporateID = IC.CID
inner join Facility F on F.FacilityID = IC.FID
Group By  IC.SentDate,IC.BatchNumber,C.CorporateID,C.CorporateName,IC.SenderID,IC.AExecutingClinician,F.FacilityName, IC.RecordCount
Order by  IC.SentDate,IC.BatchNumber


END





GO


