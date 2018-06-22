IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocSavePatientEvaluation')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocSavePatientEvaluation
GO

/****** Object:  StoredProcedure [dbo].[SPROC_UpdatePatientEvaluation]    Script Date: 4/12/2018 12:00:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[SprocSavePatientEvaluation]
(
@pDataArray ValuesArrayT Readonly,
@pPatientId bigint,
@pEncounterId bigint,
@pCId bigint,
@pFId bigint,
@pUserId bigint,
@pSetId bigint
)
As
Begin
	Declare @LocalDateTime datetime=(Select [dbo].[GetCurrentDatetimeByEntity](@pFId))
	Declare @ExecutionStatus int=0
	DECLARE @ErrorMessage NVARCHAR(4000)=''

	Begin Try;
		If @pSetId > 0
		Begin
			Update PatientEvaluationSet SET UpdateBy=@pUserId,UpdateDate=@LocalDateTime Where SetId=@pSetId
			Set @ExecutionStatus=1
		End
		ELSE
		Begin
			INSERT INTO PatientEvaluationSet (EncounterId,PatientId,CreatedBy,CreatedDate,FormType
			,Title)
			Select @pEncounterId,@pPatientId,@pUserId,@LocalDateTime,'Evaluation Management','E&M Form'

			SET @pSetId = SCOPE_IDENTITY()

			Set @ExecutionStatus=1
		End

		--Delete Operation of PatientEvaluation Records 
		IF @pSetId > 0 AND @ExecutionStatus=1 AND Exists (Select 1 From PatientEvaluation M Where NOT EXISTS (Select 1 From @pDataArray D Where M.CategoryValue=D.Value1
					AND M.CodeValue=D.Value2 AND M.EncounterId=@pEncounterId AND M.PatientId=@pPatientId) AND M.PatientID=@pPatientId 
					AND M.EncounterId=@pEncounterId)
		Begin
			Delete M From PatientEvaluation M Where NOT EXISTS (Select 1 From @pDataArray D Where M.CategoryValue=D.Value1 
			AND M.CodeValue=D.Value2 AND M.EncounterId=@pEncounterId AND M.PatientId=@pPatientId) AND M.PatientID=@pPatientId 
			AND M.EncounterId=@pEncounterId

			Set @ExecutionStatus=1
		End
		Else
			Set @ExecutionStatus=1

		--Update Operation of PatientEvaluation Records
		If @pSetId > 0 AND @ExecutionStatus=1 AND Exists (Select 1 From PatientEvaluation M INNER JOIN @pDataArray G 
					ON M.CategoryValue=G.Value1 AND M.CodeValue=G.Value2 AND M.PatientID=@pPatientId 
					AND M.EncounterId=@pEncounterId)
		Begin
			Update M SET M.ModifiedBy=@pUserId,M.ModifiedDate=@LocalDateTime,M.IsDeleted=0
			,M.[Value]=D.Value4,M.ExternalValue1=D.Value5,M.ExternalValue2=@pSetId,M.ParentCodeValue=D.Value3
			,ExternalValue3=D.Value6
			From PatientEvaluation M INNER JOIN @pDataArray D
			ON D.Value1=M.CategoryValue AND M.CodeValue=D.Value2 AND M.PatientID=@pPatientId AND M.EncounterId=@pEncounterId
			Where M.PatientID=@pPatientId AND M.EncounterId=@pEncounterId

			Set @ExecutionStatus=1
		End
		Else
			Set @ExecutionStatus=1


		--INSERT Operation of PatientEvaluation Records
		IF @pSetId > 0 AND @ExecutionStatus=1
		Begin
			INSERT INTO PatientEvaluation 
			(CorporateId,FacilityId,PatientId,EncounterId,CategoryValue,CodeValue,ParentCodeValue
			,[Value],ExternalValue1,ExternalValue2,ExternalValue3,CreatedBy,CreatedDate,IsDeleted)
			Select @pCId,@pFId,@pPatientId,@pEncounterId,D.Value1,D.Value2,D.Value3
			,D.Value4,D.Value5,@pSetId,D.Value6,@pUserId,@LocalDateTime,0 As IsDeleted
			From @pDataArray D
			LEFT JOIN PatientEvaluation M ON M.CategoryValue=D.Value1
			AND M.CodeValue=D.Value2 AND M.PatientID=@pPatientId AND M.EncounterId=@pEncounterId
			Where M.Id IS NULL

			SET @ExecutionStatus=1
		End
	End Try
	Begin Catch
		DECLARE @ErrorSeverity INT;  
		DECLARE @ErrorState INT;  

		SELECT @ErrorMessage = ERROR_MESSAGE();
		SET @ExecutionStatus=0
	End Catch
	
	Select @ExecutionStatus AS [Status],@ErrorMessage As [Message]
END
GO


