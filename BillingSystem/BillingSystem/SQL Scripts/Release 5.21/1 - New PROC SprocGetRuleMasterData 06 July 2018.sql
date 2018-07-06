IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetRuleMasterByTableNumber')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE SPROC_GetRuleMasterByTableNumber
GO

IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocGetRuleMasterData')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE SprocGetRuleMasterData
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetRuleMasterByTableNumber]    Script Date: 7/6/2018 4:31:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Krishna Prajapati>
-- Create date: <4/April/2016>
-- Description:	<TO get Rule Code>
-- =============================================
CREATE PROCEDURE [dbo].[SprocGetRuleMasterData] --[SPROC_GetRuleMasterByTableNumber]  '110',false
(
	@pCodeTableNumber nvarchar(20),
	@pIsNotActive bit
)
AS
BEGIN 
	Select R.ExtValue9, R.RuleMasterID, R.RuleCode, CAST(R.RuleCode as int) RuleCode1, R.RuleDescription, 
	R.RuleType,R.EffectiveStartDate,R.EffectiveEndDate,
	RuleSpecifiedForString=(Select TOP 1 GlobalCodeName From GlobalCodes Where GlobalCodeCategoryValue='14103' 
		And GlobalCodeValue = R.RuleSpecifiedFor)
	From RuleMaster R 
	Where ISNULL(R.ExtValue9,'')=@pCodeTableNumber And R.IsActive = @pIsNotActive
	Order By CAST(R.RuleCode as int)
	FOR JSON PATH, Root('RuleMaster'), INCLUDE_NULL_VALUES

END





GO


