IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ScrubBill_Batch')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ScrubBill_Batch
GO

/****** Object:  StoredProcedure [dbo].[SPROC_ScrubBill_Batch]    Script Date: 20-03-2018 17:21:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_ScrubBill_Batch]  
( 
@pCorporateID int,
@pFacilityID int, 
@pExecutedBy int  --- Person LoginID who executed this Test
)  
AS  
BEGIN  
     
	 	Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

    		--- Declare Cursor Fetch Variables
		    DECLARE @Cur_BillHeaderID INT
			
			--- Declare Other Variables
			DECLARE @CurrentDate DATETIME = @LocalDateTime,@Status INT=1,@RETRUN_Status int,@Cur_Status int
			--- 50
			
			DECLARE BH CURSOR FOR
			Select BillHeaderID,Status from BillHeader B Where B.Status in (45,50,100,150) and CorporateID = @pCorporateID and FacilityID = @pFacilityID;
			  
						
			OPEN BH;  					
			FETCH NEXT FROM BH INTO @Cur_BillHeaderID,@Cur_Status
	
			WHILE @@FETCH_STATUS = 0  
			BEGIN

				Exec [dbo].[SPROC_ScrubBill] @Cur_BillHeaderID,@pExecutedBy,@RETRUN_Status output

				--- Update Bill Header Status as per Scrub Status
				-- Passed
				----If @RETRUN_Status = 0
				----	Set @Status = 5

				------ If Errors
				----If @RETRUN_Status <> 0
				----	Set @Status = 2

				Update BillHeader Set [Status] = CASE WHEN @RETRUN_Status = 0
									THEN [dbo].[GetBillNextStatus] (@Cur_Status,1) ELSE [dbo].[GetBillNextStatus] (@Cur_Status,2)  END  
				Where BillHeaderID = @Cur_BillHeaderID;
				
				FETCH NEXT FROM BH INTO @Cur_BillHeaderID,@Cur_Status
			
			END 
			
			CLOSE BH;  
			DEALLOCATE BH;

				
            
END  













GO


