IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ScrubReportCorrections')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ScrubReportCorrections
GO

/****** Object:  StoredProcedure [dbo].[SPROC_ScrubReportCorrections]    Script Date: 20-03-2018 17:19:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_ScrubReportCorrections]   -- [dbo].[SPROC_ScrubReportCorrections]    1757,47892,'17004','20000',99,9,8,'1001'
(  
@pScrubHeaderID int,  --- Scrub Header ID for whom Correction is Done
@pScrubReportID int,  --- Scrub Header ID for whom Correction is Done
@pLHSV nvarchar(50), --- Corrected Value of LHS
@pRHSV nvarchar (50),  --- Corrected Value of RHS
@pExecutedBy int,  --- Person LoginID who executed this Test
@pCorporateID int,
@pFacilityID int,
@pcorrectionCodeId nvarchar(10)
)  
AS  
BEGIN  

		Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

-- Declare the return variable here
	Declare @Query1 nvarchar(max), @Loc_LHST nvarchar(50), @Loc_LHSTC nvarchar(50), @Loc_LHSK nvarchar(50), @Loc_RHST nvarchar(50), @Loc_RHSTC nvarchar(50), @Loc_RHSK nvarchar(50),
	 @KeyValue nvarchar(50), @RuleMasterID INT, @RuleStepID INT, @LHSKeyID nvarchar(50),@RHSKeyID nvarchar(50), @RHSFrom int, @LHSOriginalValue nvarchar(50),@RHSOriginalValue nvarchar(50),
	 @DataType nvarchar(50), @DataTypeName nvarchar(50),@billHeaderId int

	--- Get The Tablename etc for LHS
	Select @RuleMasterID=SR.RuleMasterID,@RuleStepID=SR.RuleStepID, @LHSKeyID=SR.ExtValue1,@RHSKeyID=SR.ExtValue2, @LHSOriginalValue = SR.LHSV, @RHSOriginalValue = SR.RHSV,
	@billHeaderId =SH.BillHeaderId 
	from ScrubReport SR
	INNER JOIN ScrubHeader SH on SH.ScrubHeaderID = SR.ScrubHeaderID
	Where SR.ScrubHeaderID = @pScrubHeaderID and SR.ScrubReportID = @pScrubReportID;

	Select @Loc_LHST=LHST,@Loc_LHSTC=LHSC,@Loc_LHSK=LHSK,@Loc_RHST=RHST,@Loc_RHSTC=RHSC,@Loc_RHSK=RHSK,@RHSFrom = RHSFrom, @DataType = DataType  
	from RuleStep where RuleMasterID = @RuleMasterID and RuleStepID = @RuleStepID;

	Select @DataTypeName = GlobalCodeName from GlobalCodes Where GlobalCodeValue = @DataType and GlobalCodeCategoryValue = '14000'

	
	---- Update LHS
	if isnull(@LHSOriginalValue,0) <> @pLHSV
	Begin
		--Set @Query1 = 'Select @LHSV = Cast('+@ColumnName+' as nvarchar(50)) from ['+@TableName+ '] Where '+ @KeyColumn +' = '+ @KeyValue
		Set @Query1 = 'Update ['+@Loc_LHST+ '] Set ' +@Loc_LHSTC +' = Cast(''' + @pLHSV +''' as '+ @DataTypeName + ')  Where '+ @Loc_LHSK +' = '+ @LHSKeyID
		--- Print @Query1
		Exec sp_executesql @Query1;

		---- Log the Change in Tracking Table
		insert into ScrubEditTrack ([TrackRuleMasterID],[TrackRuleStepID],[TrackType],[TrackTable],[TrackColumn],[TrackKeyColumn],
				[TrackValueBefore],[TrackValueAfter],[TrackKeyIDValue],[TrackSide],[IsActive],[CreatedBy],[CreatedDate],[CorporateId],[FacilityId],[CorrectionCode],[BillHeaderId])
		Select @RuleMasterID,@RuleStepID,Cast(@RHSFrom as INT),@Loc_LHST,@Loc_LHSTC,@Loc_LHSK,@LHSOriginalValue,@pLHSV,@LHSKeyID,'LHS',1,@pExecutedBy,@LocalDateTime,@pCorporateID,@pFacilityID,@pcorrectionCodeId,@billHeaderId

		---- Correct the results in ScrubReport as well - So it is not confusing End User because Value should display Updated one
		Update ScrubReport Set LHSV = @pLHSV,Modifiedby = @pExecutedBy, ModifiedDate =@LocalDateTime  Where ScrubHeaderID = @pScrubHeaderID and ScrubReportID = @pScrubReportID;

	End

	 Select @RHSOriginalValue,@pRHSV,@RHSFrom
	---- Update RHS
	if isnull(@RHSOriginalValue,0) <> @pRHSV and @RHSFrom = 1
	Begin
		Set @Query1 = 'Update ['+@Loc_RHST+ '] Set ' +@Loc_RHSTC +' = Cast(''' + @pRHSV+''' as '+ @DataTypeName + ') Where '+ @Loc_RHSK +' = '+ @RHSKeyID
		 Print @Query1
		Exec sp_executesql @Query1;

		---- Log the Change in Tracking Table
		insert into ScrubEditTrack ([TrackRuleMasterID],[TrackRuleStepID],[TrackType],[TrackTable],[TrackColumn],[TrackKeyColumn],
				[TrackValueBefore],[TrackValueAfter],[TrackKeyIDValue],[TrackSide],[IsActive],[CreatedBy],[CreatedDate],[CorporateId],[FacilityId],[CorrectionCode],[BillHeaderId])
		Select @RuleMasterID,@RuleStepID,Cast(@RHSFrom as INT),@Loc_RHST,@Loc_RHSTC,@Loc_RHSK,@RHSOriginalValue,@pRHSV,@RHSKeyID,'RHS',1,@pExecutedBy,@LocalDateTime,@pCorporateID,@pFacilityID,@pcorrectionCodeId,@billHeaderId

		---- Correct the results in ScrubReport as well - So it is not confusing End User because Value should display Updated one
		Update ScrubReport Set RHSV = @pRHSV,Modifiedby = @pExecutedBy, ModifiedDate = @LocalDateTime  Where ScrubHeaderID = @pScrubHeaderID and ScrubReportID = @pScrubReportID;

	End
END





GO


