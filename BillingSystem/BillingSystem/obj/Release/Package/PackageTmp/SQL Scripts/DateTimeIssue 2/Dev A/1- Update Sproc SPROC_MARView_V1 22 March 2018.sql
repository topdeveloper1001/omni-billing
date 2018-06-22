IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_MARView_V1')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_MARView_V1
GO

/****** Object:  StoredProcedure [dbo].[SPROC_MARView_V1]    Script Date: 3/22/2018 6:14:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<BB>
-- Create date: <Jan-2016>
-- Description:	<MAR - Medical Administration Report - This will Display What all has been Adminstered for Given Patient and Encounter within MONTH (Date Ranges)
				----- NOTE: PassedDate Range must be a full Month Range
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_MARView_V1]   -- [SPROC_MARView_V1] 3336,3442,'2016-03-03','2016-03-31',1 
	(
	@PID int = 3336, ---- PatientID  (MUST)
	@EID int = 3442, ---- EncounterID (MUST)
	@FromDate datetime = '2016-03-03', ---- Start of The Month (MUST)
	@TillDate datetime = '2016-03-31',  ---- End of the Month (MUST)
	@DisplayFlag int = 1 -----   Default = 0 =(Detail Display Line Per OrderStatus, 1 = (Consolidate Status in One Line) 
	)
AS
BEGIN

		DECLARE @LocalDateTime datetime, @Facility_Id int
		set @Facility_Id=(SELECT FacilityId  from dbo.PatientInfo where PatientID=@PID)
	SET @LocalDateTime = (Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))
	 

---- Following is to take care of TillDate also to be included in selection
Set @TillDate = DATEADD(Day,1,@TillDate)
----- Local Variables
Declare @DataCollection table (OrderInfo nvarchar(500), RID int,OrderStatus nvarchar(500),ActivityOn nvarchar(5), SchTime nvarchar(10), Quantity numeric (18,2), FreequencyStatus int)
Declare @RecordTypes table (TID int, RTID int, RTDesc nvarchar(50))
insert into @RecordTypes Select 1,1,'Ordered' union all  Select 1,2,'Administered' union all  Select 1,3,'Cancelled' union all  Select 1,4,'Balance'

If @DisplayFlag = 0 
Begin
		Insert into @DataCollection
		--Select 	'By: ' + U.UserName + '`Rx: '+Cast(Cast(OO.OpenOrderPrescribedDate As Date) As NVarchar(20))+'`'	+dbo.GetOrderDescriptionMar (OA.OrderType,OA.OrderCode) 'OrderInfo',RT.RTID,RT.RTDesc,
		Select 	'Ordering Physician: ' + U.UserName + '`Start Date and Time: '+LEFT(CONVERT(VARCHAR, OO.OpenOrderPrescribedDate, 120), 16)+'`'	+dbo.GetOrderDescriptionMar (OA.OrderType,OA.OrderCode) 'OrderInfo',RT.RTID,RT.RTDesc,--Changed by Nitin on 8Feb, 2016 as requested by Ken
					'D' + Cast(DATEPART(DAY,OA.OrderScheduleDate) as Nvarchar(3)) 'OrderDay',
					CASE WHEN OO.FrequencyCode=23 THEN CONVERT(VARCHAR(8), DATEADD(MINUTE, 20, OA.OrderScheduleDate), 108) ELSE CONVERT(nvarchar(5),OA.OrderScheduleDate,108) END,
					-- CONVERT(nvarchar(5),OA.OrderScheduleDate,108)
					(CASE When RT.RTID = 1 Then OA.OrderActivityQuantity When RT.RTID = 2 Then isnull(OA.ExecutedQuantity,0)  
						When RT.RTID = 3 Then 0 When RT.RTID = 4 Then (OA.OrderActivityQuantity - isnull(OA.ExecutedQuantity,0)) END),OO.FrequencyCode   
					---,isnull(OA.OrderActivityQuantity,0) 'OrderQuantity',isnull(OA.ExecutedQuantity,0) 'ExecutedQuantity'
					from OrderActivity OA inner join Users U on U.UserID = OA.OrderBy
					inner join @RecordTypes RT on RT.TID = 1
					inner join OpenOrder OO on OA.OrderID = OO.OpenOrderID
					Where OA.PatientID = @PID and OA.EncounterID = @EID and OrderScheduleDate between @FromDate and @TillDate
					And OA.OrderType = 5
End

If @DisplayFlag = 1 
Begin
		Insert into @DataCollection
		--Select 	'By: ' + U.UserName + '`Rx: '+Cast(Cast(OO.OpenOrderPrescribedDate As Date) As NVarchar(20))+'`'	+dbo.GetOrderDescriptionMar (OA.OrderType,OA.OrderCode) 'OrderInfo',1,
		Select 	'Ordering Physician: ' + U.UserName + '`Start Date and Time: '+LEFT(CONVERT(VARCHAR, OO.OpenOrderPrescribedDate, 120), 16)+'`'	+dbo.GetOrderDescriptionMar (OA.OrderType,OA.OrderCode) 'OrderInfo',1,--Changed by Nitin on 8Feb, 2016 as requested by Ken
					--'Ordered: '+Cast(OA.OrderActivityQuantity as Nvarchar(20)) + '`Administered: '+Cast(isnull(OA.ExecutedQuantity,0)   as Nvarchar(20)) + 
					--'`Balance: ' + Cast((OA.OrderActivityQuantity - isnull(OA.ExecutedQuantity,0) ) as Nvarchar(50)),
					----- New Function is created to make sure lines do not get splitted on front due different Totals for different dates --- So all Total will be same per Order
					dbo.GetOrderAdminsterTotalsMAR (OA.OrderType,OA.OrderCode,@PID,@EID,@FromDate,@TillDate),
					'D' + Cast(DATEPART(DAY,OA.OrderScheduleDate) as Nvarchar(3)) 'OrderDay',
					--CASE WHEN OO.FrequencyCode=23 THEN CONVERT(VARCHAR(8), DATEADD(MINUTE, 20, OA.OrderScheduleDate), 108) ELSE CONVERT(nvarchar(5),OA.OrderScheduleDate,108) END,
					 CONVERT(nvarchar(5),OA.OrderScheduleDate,108),
					--isnull(OA.ExecutedQuantity,0) 
					(CASE	When (isnull(OA.ExecutedQuantity,0)=0) AND CASE WHEN OO.FrequencyCode=23 THEN DATEADD(MINUTE, 20, OA.OrderScheduleDate) ELSE OA.OrderScheduleDate END > @LocalDateTime AND OA.OrderActivityStatus <> 9 Then 1     ----- OPEN (Nothing Administered)
							When (isnull(OA.ExecutedQuantity,0)=0) AND CASE WHEN OO.FrequencyCode=23 THEN DATEADD(MINUTE, 20, OA.OrderScheduleDate) ELSE OA.OrderScheduleDate END < @LocalDateTime AND OA.OrderActivityStatus <> 9 Then 10     ----- DELAYED
							When ((OA.OrderActivityQuantity - isnull(OA.ExecutedQuantity,0)) = 0) AND OA.OrderActivityStatus <> 9 AND OA.MedicalRecordNumber is NULL Then 2  ----- FULLY Administered - Not Cancelled
							When ((OA.OrderActivityQuantity - isnull(OA.ExecutedQuantity,0)) = 0) AND OA.MedicalRecordNumber = '1'  Then 3 ----- PARTIALLY Administered
							--When ((OA.OrderActivityQuantity - isnull(OA.ExecutedQuantity,0)) = 0) AND OA.OrderActivityStatus = 9 Then 5  ----- Cancelled
							When (OA.OrderActivityStatus = 9) Then 5  ----- Cancelled
					END),OO.FrequencyCode  
					from OrderActivity OA inner join Users U on U.UserID = OA.OrderBy
					inner join OpenOrder OO on OA.OrderID = OO.OpenOrderID
					Where OA.PatientID = @PID and OA.EncounterID = @EID and OrderScheduleDate between @FromDate and @TillDate
					And OA.OrderType = 5

					------ Waiting Approvals need to be fetched from Open Order as there are No activities created for Orders Wating Approvals
		Insert into @DataCollection
		--Select 	'By: ' + U.UserName + '`Rx: '+Cast(Cast(OA.OpenOrderPrescribedDate As Date) As NVarchar(20))+'`'	+dbo.GetOrderDescriptionMar (OA.OrderType,OA.OrderCode) 'OrderInfo',1,
		Select 	'Ordering Physician: ' + U.UserName + '`Start Date and Time: '+LEFT(CONVERT(VARCHAR, OA.OpenOrderPrescribedDate, 120), 16)+'`'	+dbo.GetOrderDescriptionMar (OA.OrderType,OA.OrderCode) 'OrderInfo',1,--Changed by Nitin on 8Feb, 2016 as requested by Ken
					dbo.GetOrderAdminsterTotalsMAR (OA.OrderType,OA.OrderCode,@PID,@EID,@FromDate,@TillDate),
					'D' + Cast(DATEPART(DAY,OA.OpenOrderPrescribedDate) as Nvarchar(3)) 'OrderDay',Convert(nvarchar(5),OA.OpenOrderPrescribedDate,108) 'OrderTime',4,OA.FrequencyCode
			from OpenOrder OA inner join Users U on U.UserID = OA.CreatedBy
			Where OA.PatientID = @PID and OA.EncounterID = @EID and OA.OpenOrderPrescribedDate between @FromDate and @TillDate	And OA.OrderType = 5 AND OA.IsApproved = 0 and OA.OrderStatus not in (4,3,9)
			End
--Select * from @DataCollection
------ Collect Needed Information for Piv --- STARTS
--DECLARE @freequencyUnit INT
--SET @freequencyUnit=(SELECT * FROM dbo.OpenOrder )


;With Report     
		AS    
		(    
		select * from     
		(    
		Select * from @DataCollection
  
		) src    
		pivot    
		(    
		  MAX(Quantity)
		  for [ActivityOn] in (D1,D2,D3,D4,D5,D6,D7,D8,D9,D10,D11,D12,D13,D14,D15,D16,D17,D18,D19,D20,D21,D22,D23,D24,D25,D26,D27,D28,D29,D30,D31)    
		)piv    
		)    
------ Collect Needed Information for Piv --- ENDS
    
		
	----- Return Desired Results
		 Select OrderInfo, SchTime,OrderStatus,FreequencyStatus,  ISNULL(Cast(D1 As Int),0) D1, ISNULL(Cast(D2 As Int),0) D2,   ISNULL(Cast(D3 As Int),0) D3, ISNULL(Cast(D4 As Int),0) D4, ISNULL(Cast(D5 As Int),0) D5,   
		 ISNULL(Cast(D6 As Int),0) D6, ISNULL(Cast(D7 As Int),0) D7, ISNULL(Cast(D8 As Int),0) D8, ISNULL(Cast(D9 As Int),0) D9, ISNULL(Cast(D10 As Int),0) D10, ISNULL(Cast(D11 As Int),0) D11,     
		 ISNULL(Cast(D12 As Int),0) D12,  ISNULL(Cast(D13 As Int),0) D13,  ISNULL(Cast(D14 As Int),0) D14,  ISNULL(Cast(D15 As Int),0) D15,  ISNULL(Cast(D16 As Int),0) D16,  ISNULL(Cast(D17 As Int),0) D17,    
		 ISNULL(Cast(D18 As Int),0) D18,  ISNULL(Cast(D19 As Int),0) D19,  ISNULL(Cast(D20 As Int),0) D20,  ISNULL(Cast(D21 As Int),0) D21,  ISNULL(Cast(D22 As Int),0) D22,  ISNULL(Cast(D23 As Int),0) D23,    
		 ISNULL(Cast(D24 As Int),0) D24,  ISNULL(Cast(D25 As Int),0) D25,  ISNULL(Cast(D26 As Int),0) D26,  ISNULL(Cast(D27 As Int),0) D27,  ISNULL(Cast(D28 As Int),0) D28,  ISNULL(Cast(D29 As Int),0) D29,     
		 ISNULL(Cast(D30 As Int),0) D30,  ISNULL(Cast(D31 As Int),0) D31 
		 From Report Order By OrderInfo,SchTime, RID


END ---- End of Proc



GO


