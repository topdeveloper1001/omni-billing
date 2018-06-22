IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_CreatePartialOrderActvities')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_CreatePartialOrderActvities
GO

/****** Object:  StoredProcedure [dbo].[SPROC_CreatePartialOrderActvities]    Script Date: 22-03-2018 18:59:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_CreatePartialOrderActvities] -- Drop PRoc CreatePartialOrderActvities 
(
	@pActivityId int= 8267,
	@pOrderActivityQuantity numeric(18,2) =1.00,
	@pActvityStatus nvarchar(50)='1'
)
AS
BEGIN
		DECLARE @Facility_Id int=(select FacilityId from dbo.OrderActivity where OrderActivityID=@pActivityId)
		Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))


Declare @tempOrderActivity TABLE (
	[OrderType] [int] NULL,[OrderCode] [nvarchar](50) NULL,[OrderCategoryID] [int] NULL,[OrderSubCategoryID] [int] NULL,[OrderActivityStatus] [int] NULL,
	[CorporateID] [int] NULL,[FacilityID] [int] NULL,[PatientID] [int] NULL,[EncounterID] [int] NULL,[MedicalRecordNumber] [nvarchar](20) NULL,
	[OrderID] [int] NULL,[OrderBy] [int] NULL,[OrderActivityQuantity] [numeric](18, 2) NULL,[OrderScheduleDate] [datetime] NULL,[PlannedBy] [int] NULL,
	[PlannedDate] [datetime] NULL,[PlannedFor] [int] NULL,[ExecutedBy] [int] NULL,[ExecutedDate] [datetime] NULL,[ExecutedQuantity] [numeric](18, 2) NULL,[ResultValueMin] [numeric](18, 4) NULL,
	[ResultValueMax] [numeric](18, 2) NULL,[ResultUOM] [int] NULL,[Comments] [nvarchar](250) NULL,[IsActive] [bit] NULL,[ModifiedBy] [int] NULL,[ModifiedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,[CreatedDate] [datetime] NULL,[BarCodeValue] [nvarchar](500) NULL,[BarCodeHtml] [nvarchar](max) NULL)


Insert into @tempOrderActivity
Select [OrderType],[OrderCode],[OrderCategoryID],[OrderSubCategoryID],[OrderActivityStatus],[CorporateID],[FacilityID],[PatientID],[EncounterID]
           ,[MedicalRecordNumber],[OrderID],[OrderBy],[OrderActivityQuantity],DATEADD(MINUTE,1,[OrderScheduleDate]),[PlannedBy],[PlannedDate],[PlannedFor],[ExecutedBy]
           ,[ExecutedDate],[ExecutedQuantity],[ResultValueMin],[ResultValueMax],[ResultUOM],[Comments],[IsActive],[ModifiedBy],[ModifiedDate],[CreatedBy]
           ,[CreatedDate],[BarCodeValue],[BarCodeHtml] from OrderActivity where OrderActivityID = @pActivityId

if(@pActvityStatus = '1' )
BEGIN
	Update @tempOrderActivity 
	Set [OrderActivityStatus] = '40' ,OrderActivityQuantity = @pOrderActivityQuantity,[ExecutedBy]=NULL,[Comments] = 'Partially Administered Order',
	    [ExecutedDate]=NULL,[ExecutedQuantity]=NULL,[ResultValueMin]=NULL,[ResultValueMax]=NULL,[ResultUOM]=NULL,[BarCodeValue]=NULL,[BarCodeHtml]=NULL

END
ELSE
BEGIN
	Update @tempOrderActivity 
	Set [OrderActivityStatus] = '9',OrderActivityQuantity = @pOrderActivityQuantity,[ExecutedBy]=9999,[Comments] = 'Cancel remaining activities',
	    [ExecutedDate]=@LocalDateTime,[ExecutedQuantity]=@pOrderActivityQuantity
END
--Select * from @tempOrderActivity

insert into OrderActivity
Select * from @tempOrderActivity

Update OrderActivity Set MedicalRecordNumber = '1' where OrderActivityID = @pActivityId


END





GO


