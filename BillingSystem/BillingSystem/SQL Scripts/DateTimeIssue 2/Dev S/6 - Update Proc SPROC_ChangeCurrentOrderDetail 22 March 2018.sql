IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ChangeCurrentOrderDetail')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ChangeCurrentOrderDetail
GO

/****** Object:  StoredProcedure [dbo].[SPROC_ChangeCurrentOrderDetail]    Script Date: 22-03-2018 15:57:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<BB>
-- Create date: <Jan-2016>
-- Description:	<This PROC is called on Changing of Order by Pharmacy already given to Patient - Which will remove any unadministered Activitites
	----			 and Update Open Order so it is set back to Approval MODE
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_ChangeCurrentOrderDetail]   
	(
	@pPatientId int,@pOrderCode nvarchar(500),@pOrderType nvarchar(100),@pEncounterId int,@pOrderStatus nvarchar(500)
	,@pOrderFrequency nvarchar(100),@pOrderStartDate datetime, @pOrderEndDate datetime,@pSchQuantity decimal(18,2),@pPhysicianId int,@pOpenOrderId int
	,@pOrderNotes nvarchar(500) ,@pOrderCategory int,@pOrderSubCategory int
	)
AS
BEGIN
Declare @LocalTime datetime, @TimeZone nvarchar(50), @Facility_Id int,@OrderStart_Date datetime, @OrderEnd_Date datetime
set @Facility_Id =(select top 1 FacilityId from PatientInfo Where PatientId=@pPatientId)
set @TimeZone=(Select TimeZ from Facility Where Facilityid=@Facility_Id)
Set @LocalTime =(Select dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,GETDATE()))
set @OrderStart_Date =(Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,@pOrderStartDate))
set @OrderEnd_Date = (Select  dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,@pOrderEndDate))



----- Local Variables
Declare @PeriodDays nvarchar(20), @UPD_ISApproved bit = 1

Set @PeriodDays = DATEDIFF(DAY,@OrderStart_Date,@OrderEnd_Date) + 1

-------- ONLY Set IsApproved as ZERO for Pharmacy Orders
If @pOrderType = '5' Set @UPD_ISApproved = 0 ELSE Set @UPD_ISApproved = 1

------ ReSET Open Order with passed in Param and Set it back to Approval Mode
Update OpenOrder Set OrderType=@pOrderType,OrderCode=@pOrderCode,OrderStatus=@pOrderStatus,StartDate=@pOrderStartDate, EndDate=@pOrderEndDate, 
		Quantity=@pSchQuantity, FrequencyCode=@pOrderFrequency, PeriodDays=@PeriodDays, OrderNotes=@pOrderNotes,PhysicianID = @pPhysicianId,
		CategoryID = @pOrderCategory, SubCategoryID = @pOrderSubCategory, IsApproved = @UPD_ISApproved , IsActivitySchecduled = NULL,ActivitySchecduledOn = NULL, EV1 = NULL
Where OpenOrderID = @pOpenOrderId

---- Remove all Scheduled activities which are not Adminstered
If @pOrderType = '5' -- special case for the Pharmacy -- delete all the ending and future activities for pharmacy orders.
	Delete from OrderActivity Where OrderID = @pOpenOrderId and isnull(ExecutedQuantity,0) = 0
ELSE
	Delete from OrderActivity Where OrderID = @pOpenOrderId and isnull(ExecutedQuantity,0) = 0 and OrderScheduleDate > @LocalTime

---- Now Call PROC for Scheduling Activities for the Updated Ored with EV1 Field Updated as 100 as above - 
----- NOTE: EV1 = 100 Means ONLY Schedule Activities which were deleted with new Frequency, Qty. Etc. for Given Order (Specially In OrderSet Case) 
------ Also Minor Change done in SPROC_ScheduleOrderActivity To Schedule Activities Based on EV1 Flag = 100 

 EXEC [SPROC_ScheduleOrderActivity] 

END ---- End of Proc





