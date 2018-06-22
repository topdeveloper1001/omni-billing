IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocGetGlobalCodesByCategory') 
  DROP PROCEDURE SprocGetGlobalCodesByCategory;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE SprocGetGlobalCodesByCategory
(
@pCategoryValue nvarchar(10),
@pFId bigint=0,
@pCId bigint=0,
@pUId bigint=0,
@pId nvarchar(10)='',
@pFacilitySpecific bit=0,
@pStatus bit=1 
)
AS
BEGIN
	Declare @GCodes Table([GlobalCodeID] [int],
	[FacilityNumber] [nvarchar](50),
	[GlobalCodeCategoryValue] [varchar](20),
	[GlobalCodeValue] [varchar](20),
	[GlobalCodeName] [varchar](250),
	[Description] [varchar](1000),
	[ExternalValue1] [varchar](50),
	[ExternalValue2] [varchar](50),
	[ExternalValue3] [varchar](50),
	[ExternalValue4] [varchar](50),
	[ExternalValue5] [varchar](50),
	[SortOrder] [int],
	[IsActive] [bit],
	[CreatedBy] [int],
	[CreatedDate] [datetime],
	[ModifiedBy] [int],
	[ModifiedDate] [datetime],
	[IsDeleted] [bit],
	[DeletedBy] [int],
	[DeletedDate] [datetime],
	[ExternalValue6] [varchar](50))

	--IF @pFacilitySpecific=0
	--	Set @pFId=0

	Declare @GCCategoryName nvarchar(100)=''

	If @pCategoryValue !=''
		Select TOP 1 @GCCategoryName = GlobalCodeCategoryName From GlobalCodeCategory Where  GlobalCodeCategoryValue=@pCategoryValue

	INSERT INTO @GCodes
	Select * From GlobalCodes Where ISNULL(IsDeleted,0)=0 And ISNULL(IsActive,1)=@pStatus
	And (ISNULL(@pCategoryValue,'')='' OR GlobalCodeCategoryValue=@pCategoryValue)
	And ((@pFacilitySpecific=0 AND FacilityNumber IN (@pFId,'0')) OR (@pFacilitySpecific=1 AND FacilityNumber=@pFId))

	Select * From @GCodes

	Select @GCCategoryName As GlobalCodeCustomValue


    --Get the New Global Code Value for the new record to be added in the table.
	Select CAST(MAX(CAST(GlobalCodeValue as bigint) + 1) as bigint)  As NewGlobalCodeId From GlobalCodes
	Where GlobalCodeCategoryValue = @pCategoryValue And FacilityNumber=@pFId And ISNULL(IsDeleted,0)=0 And ISNULL(IsActive,1)=1 


	If Exists (Select 1 From @GCodes Where ISNULL(ExternalValue3,'') !='')
		Select (
			Case When ISNULL(GC.ExternalValue3,'') !='' 
				THEN (Select TOP 1 GlobalCodeName From GlobalCodes GC1 Where GC1.GlobalCodeCategoryValue='3101' And GC1.GlobalCodeValue=GC.ExternalValue3) 
				ELSE '' END
		  ) As UnitOfMeasure
		  , GlobalCodeID As GcId
		From @GCodes GC
END
GO
