Declare @FId bigint=8, @CId bigint=0,@UId bigint=10


IF NOT EXISTS (Select 1 From GlobalCodes Where GlobalCodeCategoryValue = '1121' And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0 And FacilityNumber=@FId)
Begin
	INSERT INTO GlobalCodes
	Select @FId,[GlobalCodeCategoryValue],[GlobalCodeValue],[GlobalCodeName],[Description],[ExternalValue1],[ExternalValue2],[ExternalValue3],[ExternalValue4]
	,[ExternalValue5],[SortOrder],[IsActive],@UId,GETDATE(),NULL,NULL,[IsDeleted],NULL AS [DeletedBy],NULL AS [DeletedDate],[ExternalValue6] 
	From GlobalCodes Where GlobalCodeCategoryValue = '1121' And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0 And FacilityNumber ='0'
End