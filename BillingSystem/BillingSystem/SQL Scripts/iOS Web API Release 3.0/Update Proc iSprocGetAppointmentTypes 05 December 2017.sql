IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'[dbo].[iSprocGetAppointmentTypes]')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[iSprocGetAppointmentTypes]

GO
/****** Object:  StoredProcedure [dbo].[iSprocGetAppointments]    Script Date: 05-12-2017 17:41:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[iSprocGetAppointmentTypes]
AS
BEGIN
	Select 
	--ROW_NUMBER() OVER (ORDER BY Max(Id) DESC) AS RN,
	MAX(Id) As Id, MAX([Description]) As AppointmentType
	,(Select TOP 1 R.RoleName From [Role] R Where R.RoleID=Max(ExtValue2)) As ClinicianType
	From AppointmentTypes Where IsActive=1
	Group By CategoryNumber Order by AppointmentType
	For Json Path, Root('AppointmentTypes')
END





