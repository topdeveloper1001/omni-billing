
-- Drop stored procedure if it already exists
IF OBJECT_ID('SprocSetExpiryToUntouchedAppointments','P') IS NOT NULL
   DROP PROCEDURE SprocSetExpiryToUntouchedAppointments
GO

CREATE PROCEDURE SprocSetExpiryToUntouchedAppointments
As
Begin
	Declare @TimeZone nvarchar(20)='+04:00'
	Declare @LocalDateTime datetime=GETDATE()

	SET @LocalDateTime = dbo.GetLocalTimeBasedOnTimeZone(@TimeZone,@LocalDateTime)

	/*
		Booking Statuses: (From GlobalCodes Table using CategoryValue 4903)
		=> Initial Booking: 1
		=> Cancelled Booking: 4
		=> Expired Booking: 5
	*/
	Update Scheduling SET [Status]=5, ModifiedDate=@LocalDateTime, ModifiedBy=1
	Where ScheduleFrom <= @LocalDateTime AND [Status] = 1
End