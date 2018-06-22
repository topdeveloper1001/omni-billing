IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ApplyOrderToBill')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ApplyOrderToBill
		 
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SPROC_ApplyOrderToBill]    Script Date: 3/22/2018 8:05:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_ApplyOrderToBill]
(  
@pEncounuterID int  --- EncounterID for whom the Order need to be processed 
)
AS
BEGIN
	Declare @LocalTime datetime=(Select dbo.GetCurrentDatetimeByEntity(0))

	--- Declare Cursor Fetch Variables
	DECLARE @Cur_OpenOrderID INT, @Cur_PhysicianID INT, @Cur_CorporateID INT,@Cur_FacilityID INT
	,@Cur_PatientID INT,@Cur_EncounterID INT
	,@Cur_PrescribedDate datetime, @Cur_OrderStartDate datetime, @Cur_OrderEndDate datetime
	,@Cur_Quantity numeric(18,2),@Cur_OrderType nvarchar(20),@Cur_OrderCode nvarchar(20)
	,@Cur_DiagnosisCode nvarchar(20), @Cur_ClaimId nvarchar(50),@Cur_Ev1 nvarchar(50);
			
	--- Declare Other Variables
	DECLARE @UnitPrice numeric(18,2), @TotalPrice numeric(18,2)=0,
	@HeaderTotalPrice numeric(18,2) =0, @BillNumber nvarchar(50)
	,@BillFormat nvarchar(20),@UpriceString nvarchar(20), @EncounterFlag bit = 0
	,@BillHeaderID INT, @BillDetailLineNumber INT = 0,@AuthID INT, @AuthType INT
	,@AuthCode nvarchar(50), @PayerID nvarchar(20),@MCMultiplier numeric (18,2)
	,@MCPatientShare numeric (18,2),@SelfPayFlag bit ;
			
	-- Declare Cursor with Closed Order but not on any Bill
	DECLARE OrdersForBill1 CURSOR FOR
	SELECT OpenOrderID,OpenOrderPrescribedDate,PhysicianID,CorporateID,FacilityID,PatientID
	,EncounterID,OrderType,OrderCode,Quantity,StartDate,Enddate,DiagnosisCode, EV2 ,EV1
	FROM OpenOrder 
	WHERE EncounterID = @pEncounuterID and OrderStatus in (2,3) 
	AND OrderStatus <> 4 and ISNULL(IsActive,0) = 1;

	OPEN OrdersForBill1;
	
	
	FETCH NEXT FROM OrdersForBill1 INTO 
	@Cur_OpenOrderID,@Cur_PrescribedDate,@Cur_PhysicianID,@Cur_CorporateID,@Cur_FacilityID,@Cur_PatientID
	,@Cur_EncounterID,@Cur_OrderType,@Cur_OrderCode,@Cur_Quantity,@Cur_OrderStartDate,@Cur_OrderEndDate
	,@Cur_DiagnosisCode,@Cur_ClaimId,@Cur_Ev1;
		WHILE @@FETCH_STATUS = 0  
		BEGIN
				SET @LocalTime=(Select dbo.GetCurrentDatetimeByEntity(@Cur_FacilityID))
									  
				---- Close the Activities if any for this OrderID
				Set @Cur_Ev1 =  ISNULL(@Cur_Ev1,'')
				if(@Cur_Ev1 != '')
				BEGIN
				Update OrderActivity Set OrderActivityStatus = 2,ExecutedDate=@Cur_OrderEndDate Where  OrderId = @Cur_OpenOrderID and OrderActivityStatus < 2;
				END
				ELSE
				Update OrderActivity Set OrderActivityStatus = 2,ExecutedDate=@LocalTime Where  OrderId = @Cur_OpenOrderID and OrderActivityStatus < 2;


				----**********Here, Check the Claim Flag and BillHeader ID to pass to the below procedure					 
				Set @BillHeaderID = Cast(ISNULL(@Cur_ClaimId,0) as int)

				If (@BillHeaderID > 0)
				Begin
					Exec [dbo].[SPROC_ApplyOrderActivityToBill]
					@Cur_CorporateID,@Cur_FacilityID,@Cur_EncounterID,'1',@BillHeaderID
				End
				Else
				Begin
					Exec [dbo].[SPROC_ApplyOrderActivityToBill]
					@Cur_CorporateID,@Cur_FacilityID,@Cur_EncounterID,NULL,NULL
				End

					  
				----- XXXXXXXXXXXXXXXXXXXXXXXX----
				-- Update OpenOrder to state that it is on Bill now - Meaning Status Update to 4 - On Bill - STARTS
				Update OpenOrder Set OrderStatus = 4 Where OpenOrderID = @Cur_OpenOrderID;
					  
		--- Fetch Next Record/Order from Cursor
		FETCH NEXT FROM OrdersForBill1 INTO 
		@Cur_OpenOrderID,@Cur_PrescribedDate,@Cur_PhysicianID,@Cur_CorporateID,@Cur_FacilityID
		,@Cur_PatientID,@Cur_EncounterID,@Cur_OrderType,@Cur_OrderCode,@Cur_Quantity,@Cur_OrderStartDate
		,@Cur_OrderEndDate,@Cur_DiagnosisCode,@Cur_ClaimId,@Cur_Ev1;
							
		END  --END OF @@FETCH_STATUS = 0
	CLOSE OrdersForBill1;  
	DEALLOCATE OrdersForBill1;
END
GO


