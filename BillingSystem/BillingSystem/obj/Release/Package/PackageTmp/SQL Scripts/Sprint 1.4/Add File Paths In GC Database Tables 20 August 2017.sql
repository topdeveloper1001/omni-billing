
Declare @GCCValue nvarchar(100)='80443'
Delete From GlobalCodes Where GlobalCodeCategoryValue=@GCCValue And GlobalCodeValue='1'
IF NOT Exists (Select 1 From GlobalCodeCategory Where GlobalCodeCategoryValue = @GCCValue)
Begin
	INSERT INTO GlobalCodeCategory ([FacilityNumber],[GlobalCodeCategoryValue],[GlobalCodeCategoryName],[CreatedBy]
	      ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate],[IsActive],[GroupCode]
	      ,[ExternalValue1],[ExternalValue2],[ExternalValue3],[ExternalValue4],[ExternalValue5],[SortOrder])
	Select TOP 1 0 As [FacilityNumber],@GCCValue As [GlobalCodeCategoryValue]
	,'File Paths' As [GlobalCodeCategoryName],1 As [CreatedBy]
	,GETDATE() AS [CreatedDate],0 As [ModifiedBy],NULL AS [ModifiedDate],0 As [IsDeleted],0 As [DeletedBy]
	,NULL AS [DeletedDate],1 As [IsActive],'' As [GroupCode]
	,'' As [ExternalValue1],'' As [ExternalValue2],'' As [ExternalValue3]
	,'' As [ExternalValue4],'' As [ExternalValue5], 1 As [SortOrder] 
	From GlobalCodeCategory Where GlobalCodeCategoryValue = '80441'
End


IF NOT Exists (Select 1 From GlobalCodes Where GlobalCodeCategoryValue = @GCCValue)
Begin
	Declare @TempValues Table (N nvarchar(100),Id int,SortOrder int)

	INSERT INTO @TempValues
	Select N,ROW_NUMBER() OVER(ORDER BY N) As Id,ROW_NUMBER() OVER(ORDER BY N) As SortOrder
	From
	(
		Select '/Documents/Authorization/Corporate{0}/Facility{1}/Patient{2}/' As N		
	) a

	INSERT INTO GlobalCodes ([FacilityNumber],[GlobalCodeCategoryValue]
      ,[GlobalCodeValue],[GlobalCodeName],[Description],[ExternalValue1],[ExternalValue2],[ExternalValue3]
      ,[ExternalValue4],[ExternalValue5],[SortOrder],[IsActive],[CreatedBy],[CreatedDate],[ModifiedBy]
      ,[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate],[ExternalValue6])
	Select '0' As [FacilityNumber],@GCCValue As [GlobalCodeCategoryValue]
      ,Id as [GlobalCodeValue],N as [GlobalCodeName],'Authorization File Path' As [Description],'Authorization' As [ExternalValue1]
	  ,'' AS [ExternalValue2],'' AS [ExternalValue3]
      ,'' AS [ExternalValue4],'' AS [ExternalValue5],SortOrder AS [SortOrder],1 As [IsActive],1 As [CreatedBy]
	  ,GETDATE() AS [CreatedDate],0 As [ModifiedBy]
      ,NULL AS [ModifiedDate],0 As [IsDeleted],0 As [DeletedBy],GETDATE() As [DeletedDate],'' As [ExternalValue6]
	From @TempValues
End

