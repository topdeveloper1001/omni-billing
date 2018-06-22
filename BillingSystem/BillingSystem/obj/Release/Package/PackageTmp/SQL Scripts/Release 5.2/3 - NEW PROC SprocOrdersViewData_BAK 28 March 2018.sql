 
/****** Object:  StoredProcedure [dbo].[SprocOrdersViewData_BAK]    Script Date: 3/28/2018 6:54:19 PM ******/
DROP PROCEDURE [dbo].[SprocOrdersViewData_BAK]
GO

/****** Object:  StoredProcedure [dbo].[SprocOrdersViewData_BAK]    Script Date: 3/28/2018 6:54:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[SprocOrdersViewData_BAK]
(
@PhysicianId int,
@MostRecentDays int,
@CId int,
@FId int,
@EncId int,
@GCCategoryCodes nvarchar(max),
@PatientId int,
@OpCode nvarchar(50),
@ExParam1 nvarchar(50)
)
AS
BEGIN
	IF @OpCode = '7'
	Begin
		--Get Most Orders by Physcian ID
		Exec SPROC_GetMostOrdered @PhysicianId,@MostRecentDays					--OpenOrderCustomModel

		--Get Current Physician's Previous Orders by Physcian ID
		Exec SPROC_GetPhysicianPreviousOrders @PhysicianId,@CId,@FId			--OpenOrderCustomModel


		--Get Current Physician's Favorite Orders by Physcian ID
		Exec SPROC_GetPhysicianFavoriteOrders @PhysicianId,@CId,@FId			--OpenOrderCustomModel

		--Get All Open Orders by Encounter ID
		Exec SPROC_GetOrdersByEncounterId @EncId								--Order by Encounter End Date Desc	--OpenOrderCustomModel

		
		--Get All Order Activities by Encounter ID
		Exec SPROC_GetOrderActivitiesByEncounterId @EncId	--OrderActivityCustomModel

		--Get Patient ID by Encounter ID
		Select @PatientId = PatientId From Encounter Where EncounterID = @EncId


		--Get Future Orders Only by Patient ID
		Exec SPROC_GetPaitentFutureOrder @PatientId				--FutureOpenOrderCustomModel


		--Get Global Codes by GC Category ID
		Exec SPROC_GetGlobalCodesByCategories @GCCategoryCodes--'1024,3102,1011,2305,963'		--GlobalCodes
	End
END


GO


