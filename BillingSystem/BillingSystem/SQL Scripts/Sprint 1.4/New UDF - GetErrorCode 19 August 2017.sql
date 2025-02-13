IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[GetErrorCode]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[GetErrorCode]

Go

CREATE FUNCTION dbo.GetErrorCode
(
@pErrorName varchar(20)
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ErrorCode int
	
	Select @ErrorCode=ErrorCode From ErrorCode Where ErrorDetail=@pErrorName
	
	-- Return the result of the function
	RETURN @ErrorCode

END




