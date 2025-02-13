IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocGetClinicianRosterList') 
  DROP PROCEDURE SprocGetClinicianRosterList;
 
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
CREATE PROCEDURE [dbo].[SprocGetClinicianRosterList]
(
@CId bigint,
@FId bigint,
@UserId bigint,
@Id bigint=0,
@AStatus bit=1
)
AS
BEGIN
	Select *, '' As Department, '' As [DayOfWeek],'' As RosterType, '' As ClinicianName
	,'' As [Reason],'' As FacilityNumber,'' As ClinicianDepartment From ClinicianRoster
	Where FacilityId=@FId And IsActive=@AStatus 
	And (@Id=0 OR Id=@Id)
END
