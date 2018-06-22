IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_DeleteBillActivites')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_DeleteBillActivites
GO

/****** Object:  StoredProcedure [dbo].[SPROC_DeleteBillActivites]    Script Date: 22-03-2018 19:24:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_DeleteBillActivites]
	(
	@pBillActivityID int,
	@pCreatedBy int = 9090
	)
AS
BEGIN

	Declare @ActivityType int, @ActivityID int, @OpenorderID int, @OpenOrderActivityCount int, @BillHeaderID int,
			@HeaderTotalPrice numeric(18,2) =0,@TotalPatientShare numeric(18,2) =0, @TotalPayerNETShare numeric(18,2) =0,@poHeaderMCDiscount numeric(18,2)=0,@FId int

		
		

--- Set the Declared Parameters from the billActivity
Select @ActivityType = ActivityType,@ActivityID = ActivityID,@BillHeaderID = BillHeaderID
,@FId=FacilityID 
from BillActivity Where BillActivityId =@pBillActivityID

Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(@FId))

If isnull(@ActivityID,0) <> 0 

BEGIN  --- If Activity is ZERO Checks Ends
if(@ActivityType = 8) ----- Bed Charges to Be Deleted
BEGIN
	----- Clear the BedCharges --- Backup will be taken in BedChargesLOG automatically
	------ First Mark the Fields that it is getting deleted so same goes on LOG table
		Update BedCharges Set BCModifiedBy=@pCreatedBy, BCModifiedDate = @CurrentDate,BCIsActive = 0 Where BedChargesID =@ActivityID
	Delete from BedCharges Where BedChargesID =@ActivityID

END
ELSE
Begin  ----- For All Other Order Types -- STARTS
	
	Select  Top(1) @OpenorderID = OrderID from OrderActivity Where OrderActivityID =@ActivityID
	Select @OpenOrderActivityCount = Count(1) from OrderActivity Where OrderID =@OpenorderID Group By OrderID

	----- Clear the OrderActivity --- Backup will be taken in OrderActivityLOG automatically
	------ First Mark the Fields that it is getting deleted so same goes on LOG table
		Update OrderActivity Set ModifiedBy=@pCreatedBy, ModifiedDate = @CurrentDate,IsActive = 0 Where OrderActivityID =@ActivityID
	Delete from OrderActivity Where OrderActivityID =@ActivityID
	
	----- ONLY Delete from Open Order if there are no other Activities linked to same ORDER (Eg: Order Set can have many activities for same order)
	If isnull(@OpenOrderActivityCount,0) = 1  
	Begin
		----- Clear the OpenOrder --- Backup will be taken in OpenOrderLOG automatically
		------ First Mark the Fields that it is getting deleted so same goes on LOG table
			Update OpenOrder Set DeletedBy=@pCreatedBy, DeletedDate = @CurrentDate,IsDeleted = 1 Where OpenOrderID =@OpenorderID
		 Delete from OpenOrder Where OpenOrderID =@OpenorderID
	End

End  ----- For All Other Order Types -- ENDS

---- Finally Clear the Bill Activity --- Backup will be taken in BillActivityLOG automatically
		------ First Mark the Fields that it is getting deleted so same goes on LOG table
			Update BillActivity Set DeletedBy=@pCreatedBy, DeletedDate = @CurrentDate,IsDeleted = 1 Where BillActivityId =@pBillActivityID
		Delete from BillActivity Where BillActivityId =@pBillActivityID

						/* Changes done by Shashank
                         * WHO: Shashank
                         * WHEN: 30 March, 2016
                         * WHY: The below code is added to delete the DRG from other tables i.e. OrderActivity and BillActivity
                         * WHAT: If the activity type is DRG then We have to recalculate the bill and scrub the bill again from back end
                         */
						 Declare @saCID int, @saFID int,@saEncounterId int
						 Select @saCID= CorporateID , @saFID= FacilityId ,@saEncounterId= EncounterId from Billheader where BillHeaderId = @BillHeaderID
						 if(@ActivityType = 9) ----- DRG type activity
						 BEGIN
							--- Have to test this change.
								EXEC [dbo].[SPROC_ReValuateCurrentBill] @saCID,@saFID,@saEncounterId,'1',@BillHeaderID,@pCreatedBy
							--- Have to test this change.
						 END
		---------------   UPDATING BILL HEAEDER WITH LATEST CHANGES ---- STARTS
------ NOW Call for Re-Evealuating Bill after removing the Activity Line --- So it will Set Charges correctly on Bill Header with latest effect on the Bill
		Select @HeaderTotalPrice = sum(gross),@TotalPatientShare = sum(PatientShare),@TotalPayerNETShare = sum(PayerShareNet), @poHeaderMCDiscount = sum(MCDiscount)  
			from BillActivity Where BillHeaderID = @BillHeaderID;

---- Now Update Total Header Price
				Update BillHeader Set Gross = Isnull(@HeaderTotalPrice,0),PatientShare = isnull(@TotalPatientShare,0),
				MCDiscount = isnull(@poHeaderMCDiscount,0),PayerShareNet = isnull(@TotalPayerNETShare,0), 
				ModifiedBy=@pCreatedBy, ModifiedDate = @CurrentDate Where BillheaderID = @BillHeaderID;

	  ---------------   UPDATING BILL HEAEDER WITH LATEST CHANGES ---- ENDS
END --- If Activity is ZERO Checks Ends
END










GO


