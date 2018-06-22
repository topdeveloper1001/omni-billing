IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_AddFutureOrdersToCurrentEncounter')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_AddFutureOrdersToCurrentEncounter

/****** Object:  StoredProcedure [dbo].[SPROC_AddFutureOrdersToCurrentEncounter]    Script Date: 3/22/2018 7:40:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_AddFutureOrdersToCurrentEncounter]
(
@pEncId int =1112,
@pFutureOpenOrderId nvarchar(200) = '6,7',
@pCId int =6,
@pFId int =4
)
AS
BEGIN
	Declare @LocalDateTime datetime  = (Select dbo.GetCurrentDatetimeByEntity(@pFId))
	Declare @TempTble Table(Id int, Value nvarchar(20))

Declare @OpenOrderTem Table (ID int,[OpenOrderPrescribedDate] [datetime] NULL,
	[PhysicianID] [int] NULL,[PatientID] [int] NULL,[EncounterID] [int] NULL,[DiagnosisCode] [nvarchar](300) NULL,[StartDate] [datetime] NULL,	[EndDate] [datetime] NULL,
	[CategoryId] [int] NULL,[SubCategoryId] [int] NULL,[OrderType] [nvarchar](100) NULL,[OrderCode] [nvarchar](500) NULL,[Quantity] [decimal](18, 2) NULL,[FrequencyCode] [nvarchar](100) NULL,
	[PeriodDays] [nvarchar](20) NULL,[OrderNotes] [nvarchar](500) NULL,[OrderStatus] [nvarchar](500) NULL,[IsActivitySchecduled] [bit] NULL,[ActivitySchecduledOn] [datetime] NULL,
	[ItemName] [varchar](100) NULL,[ItemStrength] [varchar](50) NULL,[ItemDosage] [varchar](50) NULL,[IsActive] [bit] NULL,[CreatedBy] [int] NULL,[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,[ModifiedDate] [datetime] NULL,[IsDeleted] [bit] NULL,[DeletedBy] [int] NULL,[DeletedDate] [datetime] NULL,[CorporateID] [int] NULL,[FacilityID] [int] NULL)

Insert into @TempTble 
select * from dbo.[Split](',', @pFutureOpenOrderId)

Declare @EncStartTime datetime

--Select * from FutureOpenOrder where FutureOpenOrderID in (Select Value from @TempTble)

Select @EncStartTime = EncounterStartTime from Encounter where EncounterId = @pEncId

DECLARE cursorPIFix CURSOR fast_forward FOR  
SELECT FutureOpenOrderID,StartDate,EndDate from FutureOpenOrder where FutureOpenOrderID in (Select Value from @TempTble)

DECLARE @cCursorFutureOpenOrderID INT, @cOrderStartDate Datetime,@cOrderEndDate datetime,@cOrderDateDiff int
OPEN cursorPIFix   
FETCH NEXT FROM cursorPIFix INTO @cCursorFutureOpenOrderID,@cOrderStartDate,@cOrderEndDate

WHILE @@FETCH_STATUS = 0   
BEGIN   
       SELECT @cOrderDateDiff = Datediff(DD,@cOrderStartDate,@cOrderEndDate);

	   Insert into @OpenOrderTem
	   Select [FutureOpenOrderID],[OpenOrderPrescribedDate],[PhysicianID],[PatientID],[EncounterID],[DiagnosisCode],[StartDate],[EndDate],[CategoryId],[SubCategoryId],[OrderType]
           ,[OrderCode],[Quantity],[FrequencyCode],[PeriodDays],[OrderNotes],[OrderStatus],[IsActivitySchecduled],[ActivitySchecduledOn],[ItemName]
           ,[ItemStrength],[ItemDosage],[IsActive],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate]
           ,[CorporateID],[FacilityID] from FutureOpenOrder Where FutureOpenOrderID = @cCursorFutureOpenOrderID

		Update @OpenOrderTem Set [EncounterID] = @pEncId,[OpenOrderPrescribedDate]=@EncStartTime,  [StartDate]=@EncStartTime,[EndDate] = DATEADD(dd,@cOrderDateDiff,@EncStartTime),
		[CreatedDate] = @LocalDateTime,[CorporateID] =@pCId,[FacilityID] = @pFId 
		Where [ID] = @cCursorFutureOpenOrderID

       FETCH NEXT FROM cursorPIFix INTO @cCursorFutureOpenOrderID,@cOrderStartDate,@cOrderEndDate
END   

CLOSE cursorPIFix   
DEALLOCATE cursorPIFix  

Insert into OpenOrder
([OpenOrderPrescribedDate],[PhysicianID],[PatientID],[EncounterID],[DiagnosisCode],[StartDate],[EndDate],[CategoryId],[SubCategoryId],[OrderType]
           ,[OrderCode],[Quantity],[FrequencyCode],[PeriodDays],[OrderNotes],[OrderStatus],[IsActivitySchecduled],[ActivitySchecduledOn],[ItemName]
           ,[ItemStrength],[ItemDosage],[IsActive],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate]
           ,[CorporateID],[FacilityID],[IsApproved])
Select [OpenOrderPrescribedDate],[PhysicianID],[PatientID],[EncounterID],[DiagnosisCode],[StartDate],[EndDate],[CategoryId],[SubCategoryId],[OrderType]
           ,[OrderCode],[Quantity],[FrequencyCode],[PeriodDays],[OrderNotes],[OrderStatus],[IsActivitySchecduled],[ActivitySchecduledOn],[ItemName]
           ,[ItemStrength],[ItemDosage],[IsActive],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate]
           ,[CorporateID],[FacilityID],0 from @OpenOrderTem


Delete from FutureOpenOrder Where FutureOpenOrderID in (Select Value from @TempTble)

END





GO


