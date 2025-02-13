IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SPROC_ExecuteSetBedTransaction') 
  DROP PROCEDURE SPROC_ExecuteSetBedTransaction;
 
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
CREATE PROCEDURE [dbo].[SPROC_ExecuteSetBedTransaction]
AS
Begin
	Declare  @TimeZ nvarchar(20), @CurrentLocalDateTime DateTime
	Select @TimeZ = TimeZ from Facility Where FacilityId=8 
	set @CurrentLocalDateTime= dbo.GetLocalTimeBasedOnTimeZone(@TimeZ,GETDATE())

	--DECLARE @ExecutionDate DATETIME = GETDATE() - 1
	Set @CurrentLocalDateTime=@CurrentLocalDateTime-1
	EXEC  [SPROC_SetBedTransaction] @CurrentLocalDateTime
End












