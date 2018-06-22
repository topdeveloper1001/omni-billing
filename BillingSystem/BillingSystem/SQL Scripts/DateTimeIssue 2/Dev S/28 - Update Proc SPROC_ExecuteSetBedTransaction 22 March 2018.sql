IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ExecuteSetBedTransaction')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ExecuteSetBedTransaction
GO

/****** Object:  StoredProcedure [dbo].[SPROC_ExecuteSetBedTransaction]    Script Date: 22-03-2018 19:31:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_ExecuteSetBedTransaction]
AS
Begin
	Declare  @CurrentLocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(0))

	--DECLARE @ExecutionDate DATETIME = @CurrentLocalDateTime - 1
	Set @CurrentLocalDateTime=@CurrentLocalDateTime-1
	EXEC  [SPROC_SetBedTransaction] @CurrentLocalDateTime
End












GO


