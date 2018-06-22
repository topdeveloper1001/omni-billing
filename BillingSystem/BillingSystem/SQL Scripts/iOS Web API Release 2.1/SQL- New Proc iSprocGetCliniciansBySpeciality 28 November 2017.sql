IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'iSprocGetCliniciansBySpecialty') 
  DROP PROCEDURE iSprocGetCliniciansBySpecialty;
GO
/****** Object:  StoredProcedure [dbo].[SprocGetAvailableTimeSlots]    Script Date: 10/4/2017 5:30:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Exec [SprocGetOrderCodesToExport] '','ATC',9,8,'','',10
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[iSprocGetCliniciansBySpecialty]
(
@sId int
)
AS
BEGIN
	Select Id,PhysicianName as [Name] From Physician Where FacultySpeciality=@sId
	For Json Path, Root('Clinicians')
END





