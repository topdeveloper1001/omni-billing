IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_SetPersonAge_Nightly')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_SetPersonAge_Nightly
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SPROC_SetPersonAge_Nightly]    Script Date: 3/22/2018 8:15:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_SetPersonAge_Nightly]  

AS  
BEGIN  

Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))

Declare @PID int, @Age int, @DOB datetime

Declare AA Cursor for select PatientID from Patientinfo;
Open AA;

Fetch Next from AA into @PID;
While @@FETCH_STATUS = 0
Begin

	Select @DOB = PersonBirthDate from Patientinfo where PatientID = @PID
	Set @Age = DATEDIFF (YEAR,@DOB,@CurrentDate)
	
	Update Patientinfo Set PersonAge = @Age where PatientID = @PID;

	Fetch Next from AA into @PID;
End

Close AA;
Deallocate AA;

END











GO


