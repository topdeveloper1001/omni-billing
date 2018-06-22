IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocSaveOrderDetail')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocSaveOrderDetail
GO

/****** Object:  StoredProcedure [dbo].[SprocSaveOrderDetail]    Script Date: 22-03-2018 17:03:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocSaveOrderDetail]
(
@PhysicianId int,
@MostRecentDays int,
@CId int,
@FId int,
@EncId int,
@GCCategoryCodes nvarchar(max),
@PatientId int,
@OpCode nvarchar(50)='',
@ExParam1 nvarchar(50)='',

@ODiagnosisCode nvarchar(50),  
@OPeriodDays nvarchar(10),
@OOrderStatus nvarchar(20),
@OIsApproved bit=0,
@OCategoryId int,
@OOpenOrderId bigint,
@OModifiedBy bigint=0,
@OStartDate datetime,
@OEndDate datetime,
@OSubCategoryId int,
@OOrderType nvarchar(10),
@OOrderCode nvarchar(20),
@OQuantity numeric(9,2),
@OFrequencyCode nvarchar(10),
@OIsActivitySchecduled bit=0,
@OActivitySchecduledOn datetime=null,
@OItemName nvarchar(100)='',
@OItemStrength nvarchar(100)='',
@OItemDosage nvarchar(100)='',
@OIsActive bit=1,
@OOrderNotes nvarchar(500)='',
@OEV1 nvarchar(50)='',
@OEV2 nvarchar(50)='',
@OEV3 nvarchar(50)='',
@OEV4 nvarchar(50)=''
)
AS
BEGIN
	Declare @LocalTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@FId))
	
	/* -------------Add / Update Order Details, Start here---------------- */
	IF @OOpenOrderId > 0 
	Begin
		Exec SPROC_ChangeCurrentOrderDetail @PatientId,@OOrderCode,@OOrderType,@EncId
		,@OOrderStatus,@OFrequencyCode,@OStartDate, @OEndDate,@OQuantity,@PhysicianId
		,@OOpenOrderId,@OOrderNotes,@OCategoryId,@OSubCategoryId
	End
	Else
	Begin
		INSERT INTO OpenOrder ([OpenOrderPrescribedDate],[PhysicianID],[PatientID],[EncounterID],[DiagnosisCode],[StartDate]
		,[EndDate],[CategoryId],[SubCategoryId],[OrderType],[OrderCode],[Quantity],[FrequencyCode],[PeriodDays],[OrderNotes]
		,[OrderStatus],[IsActivitySchecduled],[ActivitySchecduledOn],[ItemName],[ItemStrength],[ItemDosage],[IsActive]
		,[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate]
		,[CorporateID],[FacilityID],[IsApproved],[EV1],[EV2],[EV3],[EV4])
		Select @LocalTime, @PhysicianId,@PatientId,@EncId,@ODiagnosisCode,@OStartDate
		,@OEndDate,@OCategoryId,@OSubCategoryId,@OOrderType,@OOrderCode,@OQuantity,@OFrequencyCode,@OPeriodDays,@OOrderNotes
		,@OOrderStatus,@OIsActivitySchecduled,@OActivitySchecduledOn,@OItemName, @OItemStrength, @OItemDosage,@OIsActive
		,@PhysicianId,@LocalTime,null As ModifiedBy,null AS ModifiedDate,0 AS IsDeleted,null AS DeletedBy,null AS DeletedDate
		,@CId,@FId,@OIsApproved,@OEV1,@OEV2,@OEV3,@OEV4
		SET @OOpenOrderId = SCOPE_IDENTITY()
	End
	/* -------------Add Order Details, End here---------------- */


	/* -------------Apply Order Details to Bill, Start here---------------- */
	If @OOrderStatus != 1
		Exec SPROC_ApplyOrderToBill @pEncounuterID=@EncId

	/* -------------Apply Order Details to Bill, End here---------------- */

	/*------------Get ALL Data after Open Order Details Updates Done------------------------------------*/
	Exec SprocOrdersViewData @PhysicianId=@PhysicianId, @MostRecentDays=@MostRecentDays, @CId=@CId,@FId=@FId, 
	@EncId=@EncId, @GCCategoryCodes=@GCCategoryCodes, @PatientId=@PatientId, @OpCode=@OpCode, @ExParam1=@ExParam1
	/*------------Get ALL Data after Open Order Details Updates Done------------------------------------*/



	Select @OOpenOrderId As NewOrderId
END


GO


