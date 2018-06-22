
Declare @FId bigint=8,@UserId bigint=10,@CDate datetime=GETDATE(),@GCC nvarchar(10)='1121'

IF NOT Exists (Select 1 From GlobalCodes Where FacilityNumber=@FId And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0
And GlobalCodeCategoryValue=@GCC And (CAST(GlobalCodeValue as bigint) Between 2 and 50))
Begin
	INSERT INTO GlobalCodes([FacilityNumber],[GlobalCodeCategoryValue],[GlobalCodeValue],[GlobalCodeName],[Description]
      ,[ExternalValue1],[ExternalValue2],[ExternalValue3],[ExternalValue4],[ExternalValue5],[SortOrder],[IsActive],[CreatedBy]
      ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate],[ExternalValue6])
	Select @FId,[GlobalCodeCategoryValue],[GlobalCodeValue],[GlobalCodeName],[Description]
      ,[ExternalValue1],[ExternalValue2],[ExternalValue3],[ExternalValue4],[ExternalValue5],GlobalCodeValue,[IsActive],@UserId
      ,@CDate,[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate],[ExternalValue6] 
	  From GlobalCodes
	  Where FacilityNumber='0' 
	  And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0 And GlobalCodeCategoryValue=@GCC
	  And (CAST(GlobalCodeValue as bigint) between 2 and 50)
	  Order by CAST(GlobalCodeValue as bigint)

	Delete From GlobalCodes Where GlobalCodeID IN (
	Select MAX(GlobalCodeID) From GlobalCodes Where GlobalCodeCategoryValue=@GCC And FacilityNumber=@FId
	Group by GlobalCodeName Having COUNT(1) > 1)
End

Set @FId=1093
SET @UserId=1141
IF NOT Exists (Select 1 From GlobalCodes Where FacilityNumber=@FId And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0
And GlobalCodeCategoryValue=@GCC And (CAST(GlobalCodeValue as bigint) Between 3 and 50))
Begin
	INSERT INTO GlobalCodes([FacilityNumber],[GlobalCodeCategoryValue],[GlobalCodeValue],[GlobalCodeName],[Description]
      ,[ExternalValue1],[ExternalValue2],[ExternalValue3],[ExternalValue4],[ExternalValue5],[SortOrder],[IsActive],[CreatedBy]
      ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate],[ExternalValue6])
	Select @FId,[GlobalCodeCategoryValue],[GlobalCodeValue],[GlobalCodeName],[Description]
      ,[ExternalValue1],[ExternalValue2],[ExternalValue3],[ExternalValue4],[ExternalValue5],GlobalCodeValue,[IsActive],@UserId
      ,@CDate,[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate],[ExternalValue6] 
	  From GlobalCodes
	  Where FacilityNumber='0' 
	  And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0 And GlobalCodeCategoryValue=@GCC
	  And (CAST(GlobalCodeValue as bigint) between 3 and 50)
	  Order by CAST(GlobalCodeValue as bigint)

	Delete From GlobalCodes Where GlobalCodeID IN (
	Select MAX(GlobalCodeID) From GlobalCodes Where GlobalCodeCategoryValue=@GCC And FacilityNumber=@FId
	Group by GlobalCodeName Having COUNT(1) > 1)
End

