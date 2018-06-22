IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_AddFacultyDefaultTiming')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_AddFacultyDefaultTiming

/****** Object:  StoredProcedure [dbo].[SPROC_AddFacultyDefaultTiming]    Script Date: 3/22/2018 7:36:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_AddFacultyDefaultTiming]
(
@pFacultyId int = 0,
@pUserType int=0, 
@pDeptId int=0, 
@pFId int =0,
@pCId int =0
)
AS
BEGIN
	Declare @LocalDateTime datetime  = (Select dbo.GetCurrentDatetimeByEntity(@pFId))
--Declare  @pFacultyId int = 0,@pUserType int=0, @pDeptId int=0, @pFId int =0,@pCId int =0

--select * from FacultyRooster where FacultyId = @pFacultyId and ExtValue1 = '1' and IsActive =1

--(select @FacultyType=UserType from Physician where ID = @pPhyId)

IF Exists (select 1 from FacultyRooster where FacultyId = @pFacultyId and ExtValue1 = '1' and IsActive =1)
BEGIN
	Delete from FacultyRooster where FacultyId = @pFacultyId and ExtValue1 = '1' and IsActive =1
END

Insert into FacultyRooster
	Select  @pFacultyId , @pUserType ,@pDeptId  ,'1'  ,NUll  ,NUll  ,
					NUll  ,NUll  ,NUll  ,@pFId  ,@pCId ,1  ,1  ,@LocalDateTime  ,NUll ,NUll  ,'1'  ,
					'Default Value'  ,NUll ,NUll

END





GO


