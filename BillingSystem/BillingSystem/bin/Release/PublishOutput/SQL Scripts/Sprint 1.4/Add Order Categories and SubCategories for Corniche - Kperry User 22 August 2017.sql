
Declare @FId bigint=8,@UserId bigint=10,@CDate datetime=GETDATE()
IF NOT Exists (Select 1 From GlobalCodeCategory Where FacilityNumber=@FId And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0
And GroupCode='CPT')
Begin
	INSERT INTO GlobalCodeCategory ([FacilityNumber],[GlobalCodeCategoryValue],[GlobalCodeCategoryName],[CreatedBy]
	,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate],[IsActive],[GroupCode],[ExternalValue1]
	,[ExternalValue2],[ExternalValue3],[ExternalValue4],[ExternalValue5],[SortOrder])
	Select @FId,[GlobalCodeCategoryValue],[GlobalCodeCategoryName],@UserId
	,@CDate,[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate],[IsActive],[GroupCode],[ExternalValue1]
	,[ExternalValue2],[ExternalValue3],[ExternalValue4],[ExternalValue5],[SortOrder] From GlobalCodeCategory Where FacilityNumber='0' 
	And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0
	And GroupCode='CPT'
End

IF NOT Exists (Select 1 From GlobalCodes Where FacilityNumber=@FId And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0
And GlobalCodeCategoryValue Between 11000 And 11999)
Begin
	INSERT INTO GlobalCodes([FacilityNumber],[GlobalCodeCategoryValue],[GlobalCodeValue],[GlobalCodeName],[Description]
      ,[ExternalValue1],[ExternalValue2],[ExternalValue3],[ExternalValue4],[ExternalValue5],[SortOrder],[IsActive],[CreatedBy]
      ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate],[ExternalValue6])
	Select @FId,[GlobalCodeCategoryValue],[GlobalCodeValue],[GlobalCodeName],[Description]
      ,[ExternalValue1],[ExternalValue2],[ExternalValue3],[ExternalValue4],[ExternalValue5],[SortOrder],[IsActive],@UserId
      ,@CDate,[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate],[ExternalValue6] 
	  From GlobalCodes
	  Where FacilityNumber='0' And ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0 And GlobalCodeCategoryValue Between 11000 And 11999
End

