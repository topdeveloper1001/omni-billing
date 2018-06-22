IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CreateRuleForSysAdmin')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CreateRuleForSysAdmin
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CreateRuleForSysAdmin]    Script Date: 22-03-2018 19:15:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_CreateRuleForSysAdmin]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
Declare @TempTable Table 
(
[ID]int,[CorporateID] int,[FacilityID] int,[RuleCode] nvarchar(20),[RuleDescription] nvarchar(500),[RuleType] int,[ExtValue1] nvarchar(50),
[ExtValue2] nvarchar(50),[ExtValue3] nvarchar(50),[ExtValue4] nvarchar(50),[EffectiveStartDate] datetime,[EffectiveEndDate] datetime
,[ModifiedBy] int,[ModifiedDate] datetime,[IsActive] bit,[CreatedBy] int,[CreatedDate] datetime
,RoleId int,[RuleSpecifiedFor] nvarchar(10),[ExtValue5] nvarchar(100),[ExtValue6] nvarchar(100),[ExtValue7] nvarchar(100)
,[ExtValue8] nvarchar(100),[ExtValue9] nvarchar(100)
)

declare @Corporateid int,@RuleCode nvarchar(20);

Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))


Insert into @TempTable 
Select TOP(1) * from RuleMaster order by 1 desc

Select top(1) @Corporateid = [CorporateID], @RuleCode =[RuleCode] from @TempTable

If (@Corporateid <> 6) AND not Exists (Select 1 from [RuleMaster] where RuleCode = @RuleCode and CorporateId = 0)
	INSERT INTO [dbo].[RuleMaster]
		Select 0,0,[RuleCode],[RuleDescription],[RuleType],[ExtValue1],[ExtValue2],[ExtValue3],[ExtValue4],[EffectiveStartDate],[EffectiveEndDate]
	           ,[ModifiedBy],[ModifiedDate],[IsActive],[CreatedBy],@CurrentDate,null,null,[ExtValue5],[ExtValue6],[ExtValue7],[ExtValue8],[ExtValue9]
			   from @TempTable
END
GO


