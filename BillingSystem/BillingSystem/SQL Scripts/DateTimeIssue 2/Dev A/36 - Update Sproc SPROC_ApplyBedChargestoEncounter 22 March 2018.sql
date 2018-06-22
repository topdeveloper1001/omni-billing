IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplyBedChargestoEncounter')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplyBedChargestoEncounter
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SPROC_ApplyBedChargestoEncounter]    Script Date: 3/22/2018 8:14:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Proc Starts Here
CREATE PROCEDURE [dbo].[SPROC_ApplyBedChargestoEncounter]
(
@pChargesTillDate DATETIME
)
AS
BEGIN

Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))

    --BEGIN TRY
        SET NOCOUNT ON
        SET XACT_ABORT ON
	Declare @cEID Integer
	Declare @cTotalCharges Decimal (18,2)

	--- Update Date to Today if NULL
	If @pChargesTillDate is NULL
		Set @pChargesTillDate = @CurrentDate;


	DECLARE Cursor_BedTransactionCharges CURSOR FOR 
		Select EncounterID, sum(BedCharges) from BedTransaction where BedTransactionComputedOn <= @pChargesTillDate  group by EncounterID ;

	OPEN Cursor_BedTransactionCharges;  
	FETCH NEXT FROM Cursor_BedTransactionCharges INTO @cEID,@cTotalCharges;

	WHILE @@FETCH_STATUS = 0  
		BEGIN
			--- Update Charges to Encounter 
			Update Encounter Set Charges = @cTotalCharges Where EncounterID = @cEID;

			FETCH NEXT FROM Cursor_BedTransactionCharges INTO @cEID,@cTotalCharges;
		END		
	
	--- Clean Up
	CLOSE Cursor_BedTransactionCharges;  
	DEALLOCATE Cursor_BedTransactionCharges; 

END














GO


