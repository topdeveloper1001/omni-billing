-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocSaveMedicalRecord','P') IS NOT NULL
   DROP PROCEDURE SprocSaveMedicalRecord
GO

CREATE PROCEDURE SprocSaveMedicalRecord
(
@pDataArray ValuesArrayT Readonly,
@pPatientId bigint,
@pEncounterId bigint,
@pCId bigint,
@pFId bigint,
@pUserId bigint
)
As
Begin
	Declare @LocalDateTime datetime=(Select [dbo].[GetCurrentDatetimeByEntity](@pFId))
	Declare @MedicalRecordNumber nvarchar(150)=(Select TOP 1 PersonMedicalRecordNumber From PatientInfo Where PatientId=@pPatientId)
	Declare @RecordType nvarchar(10)=(Select TOP 1 Value6 From @pDataArray)
	Declare @ExecutionStatus bit=0

	Begin Try;
		--Delete Operation
		IF Exists (Select 1 From MedicalRecord M Where NOT EXISTS (Select 1 From @pDataArray D Where M.GlobalCodeCategoryID=D.Value1 AND M.GlobalCode=D.Value2 AND M.MedicalRecordType=@RecordType AND M.PatientID=@pPatientId) AND M.PatientID=@pPatientId)
		Begin
			Delete M From MedicalRecord M Where NOT EXISTS (Select 1 From @pDataArray D Where M.GlobalCodeCategoryID=D.Value1 AND M.GlobalCode=D.Value2 AND M.MedicalRecordType=@RecordType AND M.PatientID=@pPatientId) AND M.PatientID=@pPatientId

			Set @ExecutionStatus=1
		End
		Else
			Set @ExecutionStatus=1

		--Update Operation
		If @ExecutionStatus=1 AND Exists (Select 1 From MedicalRecord M INNER JOIN @pDataArray G 
					ON M.GlobalCodeCategoryID=G.Value1 AND M.GlobalCode=G.Value2 AND M.PatientID=@pPatientId 
					AND M.MedicalRecordType=@RecordType)
		Begin
			Update M SET M.ModifiedBy=@pUserId,M.ModifiedDate=@LocalDateTime,M.IsDeleted=0
			,MedicalRecordNumber=ISNULL(MedicalRecordNumber,@MedicalRecordNumber)
			From MedicalRecord M INNER JOIN @pDataArray D
			ON D.Value1=M.GlobalCodeCategoryID AND M.GlobalCode=D.Value2 AND M.PatientID=@pPatientId
			Where M.PatientID=@pPatientId

			Set @ExecutionStatus=1
		End
		Else
			Set @ExecutionStatus=1


		--INSERT Operation
		IF @ExecutionStatus=1
		Begin
			INSERT INTO MedicalRecord 
			(MedicalRecordType,CorporateID,FacilityID,EncounterID,PatientID,MedicalRecordNumber,GlobalCodeCategoryID
			,GlobalCode,ShortAnswer,DetailAnswer,Comments,CommentBy,CommentDate,CreatedBy,CreatedDate,IsDeleted)
			Select @RecordType,@pCId,@pFId,@pEncounterId,@pPatientId,@MedicalRecordNumber,D.Value1,D.Value2,CAST(D.Value3 as bit) As ShortAnswer
			,(Case ISNULL(D.Value4,'') WHEN '' THEN NULL ELSE D.Value4 END) As DetailAnswer,D.Value5
			,(CASE ISNULL(D.Value5,'') WHEN '' THEN NULL ELSE @pUserId END)
			,(CASE ISNULL(D.Value5,'') WHEN '' THEN NULL ELSE @LocalDateTime END)
			,@pUserId,@LocalDateTime,0 As IsDeleted
			From @pDataArray D
			LEFT JOIN MedicalRecord M ON M.GlobalCodeCategoryID=D.Value1
				AND M.GlobalCode=D.Value2 AND M.PatientID=@pPatientId 
				AND M.MedicalRecordType=@RecordType
			Where M.MedicalRecordID IS NULL

			SET @ExecutionStatus=1
		End
	End Try
	Begin Catch
		SET @ExecutionStatus=0
	End Catch
	
	Select @ExecutionStatus
End