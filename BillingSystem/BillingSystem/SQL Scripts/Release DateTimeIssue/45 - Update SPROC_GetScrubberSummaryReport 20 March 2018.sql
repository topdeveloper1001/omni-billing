IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetScrubberSummaryReport')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetScrubberSummaryReport
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetScrubberSummaryReport]    Script Date: 21-03-2018 10:34:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- [SPROC_GetScrubberSummaryReport] 9,8,'2015-01-01','2015-09-01'
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetScrubberSummaryReport]
(
	@pCorporateId int =9, 
	@pFacilityId int=8,
	@pdtFrom datetime,
	@pdtTill datetime
)
AS
BEGIN
	Declare @McContractCode nvarchar(20),@InitialDayToSubmit int, @ResubmitDays1 int, @ResubmitDays2 int
	,@BillPenalityStatus nvarchar(20), @CurrentDate datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityId)),
	@DateDiffernece int,@BillDaysLeftToEClaim int;

	DECLARE @Cur_CorporateID INT,@Cur_FacilityID INT,@Cur_EncounterID nvarchar(100), @Cur_PatientID INT, @Cur_BillHeaderId int,
			@Cur_ScrubHeaderID int,@Cur_Performed int,@Cur_Passed int, @Cur_Failed int,@Cur_NotApplicable int,@Cur_ExecutedBy int,@Cur_ScrubDate datetime,
			@Cur_PatientName nvarchar(100),@Cur_ExecutedByStr nvarchar(100),@Cur_Status int,@Cur_AssignedTo int,@Cur_AssignedBy int,@Cur_EncounterEndTime datetime,
			@DueDate datetime,@CUR_EncounterNumber nvarchar(20),@CUR_EncounterPatientType nvarchar(20);

	Declare @ScrubHeaderTable Table( CorporateId int,FacilityId int, EncounterId  int, PatientId int,BillHeaderId int,ScrubHeaderID int,Performed int,
									Passed int, Failed int,NotApplicable int,ExecutedBy int,ScrubDate datetime,PatientName nvarchar(100),ExecutedByStr nvarchar(100),
									[Status] int,AssignedTo int,AssignedBy int,EncounterEndTime datetime, BillPenality nvarchar(20),BillDaysLeftToEClaim int,EncounterNumber 
									nvarchar(20),EncounterPatientType nvarchar(20));


	Declare @tableToReturn Table( CorporateId int,FacilityId int, EncounterId  int, PatientId int,BillHeaderId int,ScrubHeaderID int,Performed int,
	Passed int, Failed int,NotApplicable int,ExecutedBy int,ScrubDate datetime,PatientName nvarchar(100),ExecutedByStr nvarchar(100),
	[Status] int,AssignedTo int,AssignedBy int,EncounterEndTime datetime, BillPenality nvarchar(20),BillDaysLeftToEClaim int,EncounterNumber 
	nvarchar(20),EncounterPatientType nvarchar(20));

	DECLARE Cursur_ScurbHeader CURSOR FOR
					Select SCH.CorporateId,SCH.FacilityId,SCH.EncounterId,SCH.PatientId,SCH.BillHeaderId,MAX(SCH.ScrubHeaderID),Max(SCH.Performed),Max(SCH.Passed),
					MAX(SCH.Failed),Max(SCH.NotApplicable),MAX(SCH.ExecutedBy),MAX(SCH.ScrubDate),MAX(pInfo.PersonFirstName) +' '+ MAX(pInfo.PersonLastName),
					MAX(US.UserName),MIN(SCH.Status),MAX(SCH.AssignedTo),MAX(SCH.AssignedBy),MAX(ENC.EncounterEndTime),Max(BH.DueDate),Max(ENC.EncounterNumber),MAX(GC.GlobalCodeName)
					from ScrubHeader SCH
						INNER JOIN PatientInfo pInfo on pInfo.PatientID = SCH.PatientID
						INNER JOIN Users US on Us.UserID = SCH.ExecutedBy
						INNER JOIN Encounter ENC on ENC.EncounterId = SCH.EncounterId
						INNER JOIN BillHeader BH on BH.BillHeaderId = SCH.BillHeaderId
						LEFT JOIN GlobalCodes GC on GC.GlobalCodeValue = ENC.EncounterPatientType and GC.GlobalCodeCategoryValue = '1107'
					WHERE SCH.IsActive =1 and SCH.CorporateID = @pCorporateId and SCH.FacilityID=@pFacilityId
					and SCH.BillheaderID in (Select Distinct BillHeaderId from BillHeader Where CorporateID = @pCorporateId and FacilityID=@pFacilityId)
					and SCH.ScrubDate in (
					--Select ScrubDate From (
					Select MAX(X.ScrubDate) from ScrubHeader X Where X.CorporateID = @pCorporateId and X.FacilityID=@pFacilityId 					
					GROUP BY X.BillHeaderId
					--)CC
					)
					And (CAST(SCH.ScrubDate as date) BETWEEN @pdtFrom and @pdtTill)
					Group by 
					SCH.BillheaderId,SCH.CorporateId,SCH.FacilityId,SCH.EncounterId,SCH.patientId,Cast(SCH.ScrubDate as Date)
					Order by Cast(SCH.ScrubDate as Date) Desc
			

	OPEN Cursur_ScurbHeader;  
			
			FETCH NEXT FROM Cursur_ScurbHeader INTO @Cur_CorporateID,@Cur_FacilityID,@Cur_EncounterID,@Cur_PatientID,@Cur_BillHeaderId,@Cur_ScrubHeaderID,
			@Cur_Performed,@Cur_Passed,@Cur_Failed,@Cur_NotApplicable,@Cur_ExecutedBy,@Cur_ScrubDate,@Cur_PatientName,@Cur_ExecutedByStr,@Cur_Status,@Cur_AssignedTo,
			@Cur_AssignedBy,@Cur_EncounterEndTime,@DueDate,@CUR_EncounterNumber,@CUR_EncounterPatientType;
			

			WHILE @@FETCH_STATUS = 0  
			BEGIN  
						
			 ---- PRINT @Cur_PatientID
			    -- Get patinet selected Managed care contract
				Select @McContractCode = [dbo].[GetPatinetMCContractId](@Cur_PatientID);
				-- if(@McContractCode is not NUll)
								
				Set @McContractCode = isnull(@McContractCode,0)

			----	PRINT  @McContractCode
				if(@McContractCode > 0 )
				Begin
					Select @InitialDayToSubmit = InitialSubmitDay,@ResubmitDays1= ResubmitDays1,@ResubmitDays2 =ResubmitDays2 
						from McContract where McCode = @McContractCode

					Set @DateDiffernece = DATEDIFF(day,@DueDate,@CurrentDate) --- Get The Date Differnece

					Set @BillDaysLeftToEClaim = @InitialDayToSubmit - @DateDiffernece;

					IF(@BillDaysLeftToEClaim <= 7)
						Begin
							Set @BillPenalityStatus =	'High'
						END
					ELSE IF(@BillDaysLeftToEClaim <= 14) -- Set the Bill Penality Date
						BEgin
							Set @BillPenalityStatus =	'Medium'
						END
					ELSE
						Begin 
							Set @BillPenalityStatus =	'Low'
						END;

						--PRINT @Cur_EncounterID
					
		
					Insert Into @ScrubHeaderTable 
					Values
					(	
						@Cur_CorporateID,@Cur_FacilityID,@Cur_EncounterID,@Cur_PatientID,@Cur_BillHeaderId,@Cur_ScrubHeaderID,
						@Cur_Performed,@Cur_Passed,@Cur_Failed,@Cur_NotApplicable,@Cur_ExecutedBy,@Cur_ScrubDate,@Cur_PatientName,@Cur_ExecutedByStr,@Cur_Status,
						@Cur_AssignedTo,@Cur_AssignedBy,@Cur_EncounterEndTime,@BillPenalityStatus,@BillDaysLeftToEClaim,@CUR_EncounterNumber,@CUR_EncounterPatientType
					)

				End --- End McContract code not null check
				Else 
				BEGIN
					Insert Into @ScrubHeaderTable 
					Values
					(	
						@Cur_CorporateID,@Cur_FacilityID,@Cur_EncounterID,@Cur_PatientID,@Cur_BillHeaderId,@Cur_ScrubHeaderID,
						@Cur_Performed,@Cur_Passed,@Cur_Failed,@Cur_NotApplicable,@Cur_ExecutedBy,@Cur_ScrubDate,@Cur_PatientName,@Cur_ExecutedByStr,@Cur_Status,
						@Cur_AssignedTo,@Cur_AssignedBy,@Cur_EncounterEndTime,'Low',0,@CUR_EncounterNumber,@CUR_EncounterPatientType
					)
				END
			FETCH NEXT FROM Cursur_ScurbHeader INTO @Cur_CorporateID,@Cur_FacilityID,@Cur_EncounterID,@Cur_PatientID,@Cur_BillHeaderId,@Cur_ScrubHeaderID,
			@Cur_Performed,@Cur_Passed,@Cur_Failed,@Cur_NotApplicable,@Cur_ExecutedBy,@Cur_ScrubDate,@Cur_PatientName,@Cur_ExecutedByStr,@Cur_Status,@Cur_AssignedTo,
			@Cur_AssignedBy,@Cur_EncounterEndTime,@DueDate,@CUR_EncounterNumber,@CUR_EncounterPatientType;

			END--END OF @@FETCH_STATUS = 0  
	CLOSE Cursur_ScurbHeader;  
	DEALLOCATE Cursur_ScurbHeader; 
				
	--Select CorporateId 'CorporateId',FacilityId 'FacilityId', EncounterId,PatientId,BillHeaderId,MAX(ScrubHeaderID) 'ScrubHeaderID',MAX(Performed) 'Performed',
	--								MAX(Passed) 'Passed', MAX(Failed) 'Failed', MAX(NotApplicable) 'NotApplicable', MAX(ExecutedBy) 'ExecutedBy', MAX(ScrubDate) 'ScrubDate',
	--								PatientName ,MAX(ExecutedByStr) 'ExecutedByStr',
	--								MAX(Status) 'Status', MAX(AssignedTo) 'AssignedTo', MAX(AssignedBy) 'AssignedBy',EncounterEndTime , BillPenality ,BillDaysLeftToEClaim ,EncounterNumber,MAX(EncounterPatientType) 'EncounterPatientType'
	--								from @ScrubHeaderTable	Group by  CorporateId,FacilityId,PatientId,EncounterId,BillHeaderId,PatientName ,
	--								EncounterEndTime , BillPenality ,BillDaysLeftToEClaim ,EncounterNumber  

	Insert into @tableToReturn 
	Select CorporateId 'CorporateID',FacilityId 'FacilityID', EncounterID,PatientId,MAX(BillHeaderId) 'BillHeaderId',MAX(ScrubHeaderID) 'ScrubHeaderID',MAX(Performed) 'Performed',
	MAX(Passed) 'Passed', MAX(Failed) 'Failed', MAX(NotApplicable) 'NotApplicable', MAX(ExecutedBy) 'ExecutedBy', MAX(ScrubDate) 'ScrubDate',
	PatientName ,MAX(ExecutedByStr) 'ExecutedByStr',
	MAX(Status) 'Status', MAX(AssignedTo) 'AssignedTo', MAX(AssignedBy) 'AssignedBy',EncounterEndTime , BillPenality ,BillDaysLeftToEClaim,
	EncounterNumber,
	MAX(EncounterPatientType) 'EncounterPatientType'
	from @ScrubHeaderTable	Group by  CorporateId,FacilityId,PatientId,EncounterId,BillHeaderId,PatientName ,
	EncounterEndTime , BillPenality ,BillDaysLeftToEClaim ,EncounterNumber
	Order By EncounterPatientType


	--Insert into @tableToReturn
	--Select MAX(CorporateId) 'CorporateID',MAX(FacilityId) 'FacilityID', MAX(EncounterID),MAX(PatientId),MAX(BillHeaderId) 'BillHeaderId',MAX(ScrubHeaderID) 'ScrubHeaderID',SUM(Performed) 'Performed',
	--SUM(Passed) 'Passed', SUM(Failed) 'Failed', SUM(NotApplicable) 'NotApplicable', MAX(ExecutedBy) 'ExecutedBy', MAX(ScrubDate) 'ScrubDate',
	--MAX(PatientName) ,MAX(ExecutedByStr) 'ExecutedByStr',
	--MAX(Status) 'Status', MAX(AssignedTo) 'AssignedTo', MAX(AssignedBy) 'AssignedBy',MAX(EncounterEndTime) , MAX(BillPenality) ,MAX(BillDaysLeftToEClaim),
	--'TOTAL',
	--EncounterPatientType 'EncounterPatientType'
	--from @ScrubHeaderTable	Group by  EncounterPatientType
	--Order By EncounterPatientType
	Insert into @tableToReturn
	Select MAX(CorporateId) 'CorporateID',MAX(FacilityId) 'FacilityID', MAX(EncounterID),'',MAX(BillHeaderId) 'BillHeaderId',MAX(ScrubHeaderID) 'ScrubHeaderID',SUM(Performed) 'Performed',
	SUM(Passed) 'Passed', SUM(Failed) 'Failed', SUM(NotApplicable) 'NotApplicable', MAX(ExecutedBy) 'ExecutedBy', null,
	MAX(PatientName) ,MAX(ExecutedByStr) 'ExecutedByStr',
	MAX(Status) 'Status', MAX(AssignedTo) 'AssignedTo', MAX(AssignedBy) 'AssignedBy',MAX(EncounterEndTime) , MAX(BillPenality) ,MAX(BillDaysLeftToEClaim),
	'TOTAL',
	EncounterPatientType 'EncounterPatientType'
	from @ScrubHeaderTable	Group by  EncounterPatientType


	
	--Insert into @tableToReturn
	--Select MAX(CorporateId) 'CorporateID',MAX(FacilityId) 'FacilityID', MAX(EncounterID),'',MAX(BillHeaderId) 'BillHeaderId',MAX(ScrubHeaderID) 'ScrubHeaderID',SUM(Performed) 'Performed',
	--CAST(CAST((SUM(Passed) * 100)/ SUM(Performed) as float) as nvarchar) as 'Passed', CAST(CAST((SUM(Failed) * 100)/ SUM(Performed) as float) as nvarchar) as 'Failed', CAST(CAST((SUM(NotApplicable) * 100)/ SUM(Performed) as numeric(18,2)) as nvarchar) as 'NotApplicable', MAX(ExecutedBy) 'ExecutedBy', null,
	--MAX(PatientName) ,MAX(ExecutedByStr) 'ExecutedByStr', MAX(Status) 'Status', 
	--MAX(AssignedTo) 'AssignedTo', MAX(AssignedBy) 'AssignedBy',MAX(EncounterEndTime) , MAX(BillPenality) ,MAX(BillDaysLeftToEClaim),
	--'TOTAL',
	--EncounterPatientType 'EncounterPatientType'
	--from @ScrubHeaderTable	Group by  EncounterPatientType


	Select * from @tableToReturn Order By EncounterPatientType,ScrubDate desc
END





