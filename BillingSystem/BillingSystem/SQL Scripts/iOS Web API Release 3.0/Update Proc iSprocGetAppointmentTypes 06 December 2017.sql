IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'[dbo].[iSprocGetAppointmentTypes]')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROC iSprocGetAppointmentTypes

GO
/****** Object:  StoredProcedure [dbo].[iSprocGetAppointmentTypes]    Script Date: 06-12-2017 11:20:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[iSprocGetAppointmentTypes]
AS
BEGIN
	Select 
	MAX(Id) As AppointmentTypeId, MAX([Description]) As Title
	,(Select TOP 1 R.RoleName From [Role] R Where R.RoleID=Max(ExtValue2)) As ClinicianType
	From AppointmentTypes Where IsActive=1
	Group By [Description] Order by Title
	For Json Path, Root('AppointmentTypes')
END





