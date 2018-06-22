IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SprocSetExpiryToUntouchedAppointments')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SprocSetExpiryToUntouchedAppointments
GO

/****** Object:  StoredProcedure [dbo].[SprocSetExpiryToUntouchedAppointments]    Script Date: 22-03-2018 17:01:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SprocSetExpiryToUntouchedAppointments]
As
Begin
	Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(0))

	/*
		Booking Statuses: (From GlobalCodes Table using CategoryValue 4903)
		=> Initial Booking: 1
		=> Cancelled Booking: 4
		=> Expired Booking: 5
	*/
	Update Scheduling SET [Status]=5, ModifiedDate=@LocalDateTime, ModifiedBy=1
	Where ScheduleFrom <= @LocalDateTime AND [Status] = 1
End
GO


