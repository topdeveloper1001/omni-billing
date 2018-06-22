IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GenerateUpdateToken')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GenerateUpdateToken
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GenerateUpdateToken]    Script Date: 22-03-2018 20:00:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<BB>
-- Create date: <Jul-2015>
-- Description:	<Generate and Update Token>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GenerateUpdateToken]   -----  Exec [dbo].[SPROC_GenerateUpdateToken] 'Sysadmin','2015-12-31'
	(
		@pUserName nvarchar(100),
		@pExpiryDate datetime
	)
AS
BEGIN
Declare @TokenNumber nvarchar(25)

Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))

Set @pExpiryDate = isnull(@pExpiryDate, DATEADD(Month,1,@CurrentDate))

Select Top(1) @TokenNumber = (Upper(SUBSTRING(Username,1,2)) + Cast(CorporateID as Nvarchar(8)) + Cast(FacilityId as Nvarchar(8)) +  Cast(UserID as Nvarchar(8)))
 from users where UserName = @pUserName

Set @TokenNumber = isnull(@TokenNumber,' ')

If @TokenNumber <> ' '
	Update users Set userToken = @TokenNumber, TokenExpiryDate = @pExpiryDate where UserName = @pUserName

Select @pUserName 'UserName',@TokenNumber 'TokenNumber',@pExpiryDate 'ExpiryDate'

END











GO


