IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'GetCurrentDatetimeByEntity')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[GetCurrentDatetimeByEntity]
GO

/****** Object:  UserDefinedFunction [dbo].[GetCurrentDatetimeByEntity]    Script Date: 3/20/2018 3:25:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
Create FUNCTION [dbo].[GetCurrentDatetimeByEntity]
(
@pFacilityId int=null
)
RETURNS datetime
AS
BEGIN
	IF ISNULL(@pFacilityId,0)=0
		SET @pFacilityId=1106

	DECLARE @CurrentDateTime datetime, @TimeZone nvarchar(50)
	SET @TimeZone=(Select TimeZ from Facility Where Facilityid=@pFacilityId)
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
GO


