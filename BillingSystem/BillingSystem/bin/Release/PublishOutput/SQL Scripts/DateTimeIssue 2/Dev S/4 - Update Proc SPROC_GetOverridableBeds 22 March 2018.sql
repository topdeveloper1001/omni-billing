IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetOverridableBeds')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetOverridableBeds
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetOverridableBeds]    Script Date: 22-03-2018 15:41:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetOverridableBeds]
(
@ServiceCodeTableNumber nvarchar(50) = '4010'
)
AS
BEGIN

	Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))

	Declare @AsOfDate Date
	Set @AsOfDate = CAST(@CurrentDate as date)

	Select DISTINCT S.* From ServiceCode S 
	INNER JOIN BedRateCard B ON S.ServiceCodeValue = B.ServiceCodeValue
	Where S.CanOverRide = 1	
	And (@AsOfDate between S.ServiceCodeEffectiveDate and isnull(S.ServiceExpiryDate,@CurrentDate))
	And S.ServiceCodeTableNumber = @ServiceCodeTableNumber	
	And ISNULL(S.IsDeleted,0) = 0
	And ISNULL(S.IsActive,1) = 1
	Order By S.ServiceCodeValue
END





GO


