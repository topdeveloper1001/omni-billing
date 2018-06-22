IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetSrubHeaderDetail')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetSrubHeaderDetail
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetSrubHeaderDetail]    Script Date: 21-03-2018 10:32:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetSrubHeaderDetail]
(
	@pCorporateId int =9, 
	@pFacilityId int=8
)
AS
BEGIN

		Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityId))

	Declare @McContractCode nvarchar(20),@InitialDayToSubmit int, @ResubmitDays1 int, @ResubmitDays2 int,@BillPenalityStatus nvarchar(20), @CurrentDate datetime,
	@DateDiffernece int,@BillDaysLeftToEClaim int,@lBillNumber nvarchar(200);

	DECLARE @Cur_CorporateID INT,@Cur_FacilityID INT,@Cur_EncounterID INT, @Cur_PatientID INT, @Cur_BillHeaderId int,
			@Cur_ScrubHeaderID int,@Cur_Performed int,@Cur_Passed int, @Cur_Failed int,@Cur_NotApplicable int,@Cur_ExecutedBy int,@Cur_ScrubDate datetime,
			@Cur_PatientName nvarchar(100),@Cur_ExecutedByStr nvarchar(100),@Cur_Status int,@Cur_AssignedTo int,@Cur_AssignedBy int,@Cur_EncounterEndTime datetime,
			@DueDate datetime,@CUR_EncounterNumber nvarchar(20),@CUR_EncounterPatientType nvarchar(20);

	Declare @ScrubHeaderTable Table( CorporateId int,FacilityId int, EncounterId  int, PatientId int,BillHeaderId int,ScrubHeaderID int,Performed int,
									Passed int, Failed int,NotApplicable int,ExecutedBy int,ScrubDate datetime,PatientName nvarchar(100),ExecutedByStr nvarchar(100),
									[Status] int,AssignedTo int,AssignedBy int,EncounterEndTime datetime, BillPenality nvarchar(20),BillDaysLeftToEClaim int,EncounterNumber 
									nvarchar(20),EncounterPatientType nvarchar(20),BillNumber nvarchar(200));

	Set @CurrentDate = @LocalDateTime --@LocalDateTime;

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
					Set @lBillNumber = (Select BillNumber from BillHeader where BillheaderId = @Cur_BillHeaderId)
		
					Insert Into @ScrubHeaderTable 
					Values
					(	
						@Cur_CorporateID,@Cur_FacilityID,@Cur_EncounterID,@Cur_PatientID,@Cur_BillHeaderId,@Cur_ScrubHeaderID,
						@Cur_Performed,@Cur_Passed,@Cur_Failed,@Cur_NotApplicable,@Cur_ExecutedBy,@Cur_ScrubDate,@Cur_PatientName,@Cur_ExecutedByStr,@Cur_Status,
						@Cur_AssignedTo,@Cur_AssignedBy,@Cur_EncounterEndTime,@BillPenalityStatus,@BillDaysLeftToEClaim,@CUR_EncounterNumber,@CUR_EncounterPatientType,
						@lBillNumber
					)

				End --- End McContract code not null check
				Else 
				BEGIN
					Insert Into @ScrubHeaderTable 
					Values
					(	
						@Cur_CorporateID,@Cur_FacilityID,@Cur_EncounterID,@Cur_PatientID,@Cur_BillHeaderId,@Cur_ScrubHeaderID,
						@Cur_Performed,@Cur_Passed,@Cur_Failed,@Cur_NotApplicable,@Cur_ExecutedBy,@Cur_ScrubDate,@Cur_PatientName,@Cur_ExecutedByStr,@Cur_Status,
						@Cur_AssignedTo,@Cur_AssignedBy,@Cur_EncounterEndTime,'Low',0,@CUR_EncounterNumber,@CUR_EncounterPatientType,
						@lBillNumber
					)
				END
			FETCH NEXT FROM Cursur_ScurbHeader INTO @Cur_CorporateID,@Cur_FacilityID,@Cur_EncounterID,@Cur_PatientID,@Cur_BillHeaderId,@Cur_ScrubHeaderID,
			@Cur_Performed,@Cur_Passed,@Cur_Failed,@Cur_NotApplicable,@Cur_ExecutedBy,@Cur_ScrubDate,@Cur_PatientName,@Cur_ExecutedByStr,@Cur_Status,@Cur_AssignedTo,
			@Cur_AssignedBy,@Cur_EncounterEndTime,@DueDate,@CUR_EncounterNumber,@CUR_EncounterPatientType;

			END--END OF @@FETCH_STATUS = 0  
	CLOSE Cursur_ScurbHeader;  
	DEALLOCATE Cursur_ScurbHeader; 
				
	--Select * from @ScrubHeaderTable		

	Select CorporateId 'CorporateId',FacilityId 'FacilityId', EncounterId,PatientId,BillHeaderId,MAX(ScrubHeaderID) 'ScrubHeaderID',MAX(Performed) 'Performed',
									MAX(Passed) 'Passed', MAX(Failed) 'Failed', MAX(NotApplicable) 'NotApplicable', MAX(ExecutedBy) 'ExecutedBy', MAX(ScrubDate) 'ScrubDate',
									PatientName ,MAX(ExecutedByStr) 'ExecutedByStr',
									MAX(Status) 'Status', MAX(AssignedTo) 'AssignedTo', MAX(AssignedBy) 'AssignedBy',EncounterEndTime , BillPenality ,BillDaysLeftToEClaim ,EncounterNumber,MAX(EncounterPatientType) 'EncounterPatientType',
									MAX(BillNumber) 'BillNumber'
									from @ScrubHeaderTable	Group by  CorporateId,FacilityId,PatientId,EncounterId,BillHeaderId,PatientName ,
									EncounterEndTime , BillPenality ,BillDaysLeftToEClaim ,EncounterNumber

END
GO


