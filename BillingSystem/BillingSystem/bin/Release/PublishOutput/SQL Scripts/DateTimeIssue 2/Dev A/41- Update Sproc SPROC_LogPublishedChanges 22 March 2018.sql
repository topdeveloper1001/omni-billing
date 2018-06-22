IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_LogPublishedChanges')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_LogPublishedChanges
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SPROC_LogPublishedChanges]    Script Date: 3/22/2018 8:22:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_LogPublishedChanges]
AS
BEGIN
	Declare @CurrentDate datetime  = (Select dbo.GetCurrentDatetimeByEntity(0))

	Declare @Version nvarchar(100), @PublishedFrom datetime

	Select @Version = Cast(('V ' + CONVERT(VARCHAR(12),@CurrentDate)) as nvarchar)
	
	--PRINT @Version
	Select TOP 1 @PublishedFrom = PublishedTill From PublishVersioning Order By AddedOn Desc

	INSERT INTO PublishVersioning
	VALUES (@Version,@PublishedFrom,@CurrentDate,HOST_NAME(),HOST_NAME(),'OMNI QA','DEV Server',@CurrentDate)
END





GO


