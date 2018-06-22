IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CancelOpenOrder')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CancelOpenOrder
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CancelOpenOrder]    Script Date: 22-03-2018 15:52:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_CancelOpenOrder]
(	
	@pOrderId int = 3119,
	@pUserId int = 1
)
AS
BEGIN
		DECLARE @Facility_Id int
		set @Facility_Id=(SELECT FacilityId from dbo.Users where UserID=@pUserId)
		Declare @LocalDateTime datetime = (Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))
----- Declarations----------------------------------------------------------------------------------------------------------------
Declare @pOrderStatus int = 9,@pOrderActivityStatus int =9;
Declare @spEncounterEndStatus int, @spEncounterId int,@spOpenOrderCount int;
------------------------------------------------ Main Procedure Calls-------------------------------------------------------------
Select @spEncounterId = EncounterId from OpenOrder where OpenOrderID = @pOrderId -- Get the EncounterId from the Open Order Id
--Get the Encounter End status, and If the Encounter Id ended and no orders are Pending then ran the scrub for the Encounter.... to view the bill for scrubber
Select @spEncounterEndStatus = Case when ENcounterEndTime Is NULL Then 0 Else 1 end from Encounter where EncounterId = @spEncounterId

---------------------------Update Operations for cancelling the order activity ----------------------------------------------------
Update OrderActivity SET OrderActivityStatus = @pOrderActivityStatus, ExecutedBy = @pUserId, ExecutedDate = @LocalDateTime
where OrderId = @pOrderId and OrderActivityStatus != 4 

----- Get the Status to be set for the Order from the Order activity --------------------------------------------------------------
Set @pOrderStatus = Case when (Select COunt(1) from OrderActivity where OrderId = @pOrderId and OrderActivityStatus = 4) > 0
THEN 4 ELSE 9 END

----- Update the Order status fetched from above query ----------------------------------------------------------------------------
Update OpenOrder Set OrderStatus = @pOrderStatus, ModifiedBy =@pUserId, ModifiedDate = @LocalDateTime 
Where OpenOrderId = @pOrderId

-------------------------------------- Scrub the Encounter If the Encounter had ended and have no open order pending
Select @spOpenOrderCount = Count(1) from OpenOrder where EncounterId = @spEncounterId and OrderStatus = 1 -- Get the number of Open orders for the enocunter

	---- Check the Encounter End status and Open order counts for the Encounter
	If(@spEncounterEndStatus = 1 and @spOpenOrderCount = 0)
	Begin
		EXEC [SPROC_EncounterEndCheckBillEdit]  @spEncounterId,@pUserId
	END

END


GO


