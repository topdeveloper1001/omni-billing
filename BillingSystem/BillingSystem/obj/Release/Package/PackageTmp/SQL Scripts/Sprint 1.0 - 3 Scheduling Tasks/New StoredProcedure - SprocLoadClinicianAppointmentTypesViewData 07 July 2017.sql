IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocLoadClinicianAppointmentTypesViewData') 
  DROP PROCEDURE SprocLoadClinicianAppointmentTypesViewData;

/****** Object:  StoredProcedure [dbo].[SprocLoadClinicianAppointmentTypesViewData]    Script Date: 7/7/2017 11:50:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SprocLoadClinicianAppointmentTypesViewData]
(
@FId bigint,
@UserId bigint
)
AS
BEGIN
	
	/*-------------Getting the list of Physicians ----------------*/
	Exec [SPROC_GetPhysiciansByFacility] @FId,@UserId



	/*-------------Getting the list of Appointment Types  ----------------*/
	Select CAST(Id as nvarchar) As [Value],[Name] As [Text],CptRangeFrom As ExternalValue1,CptRangeTo As ExternalValue2  
	From AppointmentTypes Where IsActive=1 And FacilityId = @FId
END
GO


