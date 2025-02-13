IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocDeleteClinicianRoster') 
  DROP PROCEDURE SprocDeleteClinicianRoster;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocDeleteClinicianRoster]
(
@Id bigint,
@CId bigint,
@FId bigint,
@UserId bigint
)
AS
BEGIN
	Delete From [dbo].[ClinicianRoster] Where Id=@Id
	Delete From dbo.Scheduling Where StatusType = @Id

	Exec SprocGetClinicianRosterList @CId,@FId,@UserId,0
END
