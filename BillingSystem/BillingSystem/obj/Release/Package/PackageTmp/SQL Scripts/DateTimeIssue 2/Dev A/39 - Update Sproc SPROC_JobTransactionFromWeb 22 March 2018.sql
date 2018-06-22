IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_JobTransactionFromWeb')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_JobTransactionFromWeb
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SPROC_JobTransactionFromWeb]    Script Date: 3/22/2018 8:20:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_JobTransactionFromWeb]
(
@pBedChargesComputationDate datetime
)
as
begin
declare @startDate datetime=(Select dbo.GetCurrentDatetimeByEntity(0))

while (@startDate <=@pBedChargesComputationDate)
begin
set @startDate = DATEADD(day,1,@startDate);
exec SPROC_SetBedTransaction @startDate

exec SPROC_ApplyBedChargestoEncounter @startDate
end
end













GO


