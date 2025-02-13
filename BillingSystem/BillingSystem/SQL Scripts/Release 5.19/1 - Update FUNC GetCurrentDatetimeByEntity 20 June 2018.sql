GO
/****** Object:  UserDefinedFunction [dbo].[GetCurrentDatetimeByEntity]    Script Date: 6/20/2018 1:06:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[GetCurrentDatetimeByEntity]
(
	@pFacilityId int=null
)
RETURNS datetime
AS
BEGIN
	IF ISNULL(@pFacilityId,0)=0
		SET @pFacilityId=4

	DECLARE @CurrentDateTime datetime, @TimeZone nvarchar(50)
	SET @TimeZone=(Select TimeZ from Facility Where Facilityid=@pFacilityId)

	IF @TimeZone IS NULL
		SET @TimeZone='-05:00'

	SET @CurrentDateTime=GETDATE()

	-- Declare the return variable here
	DECLARE @rtDatetime datetime, @d DATETIMEOFFSET, @Utc datetime

	Select @Utc = DateAdd(second,dateDiff(second, getDate(), getUtcDate()),@CurrentDateTime)
	Set @d = @Utc

	--Select SWITCHOFFSET(@d, '+05:30')
	Set @rtDatetime= SWITCHOFFSET(@d, @TimeZone)

	-- Return the result of the function
	RETURN @rtDatetime
END
