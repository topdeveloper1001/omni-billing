IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SProc_UpdatePatientEmailId')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SProc_UpdatePatientEmailId
		 
		 GO

/****** Object:  StoredProcedure [dbo].[SProc_UpdatePatientEmailId]    Script Date: 3/22/2018 8:01:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SProc_UpdatePatientEmailId]
(
	@pPid int,
	@EmailId nvarchar(500)
)
AS
BEGIN
Declare  @Facility_Id int= (select FacilityId from dbo.PatientInfo where PatientID=@pPid)

		Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))

Declare @CodeValue nvarchar(50) = floor(rand() * 100000 - 1),@PreviousEmailId nvarchar(500)

IF NOT EXISTS (Select 1 from PatientLoginDetail where (@pPid=0 OR PatientId = @pPid) AND (ISDeleted is null OR ISDeleted =0))
BEGIN
	INSERT INTO [dbo].[PatientLoginDetail]
       ([PatientId],[Email],[TokenId],[PatientPortalAccess],[Password],[CodeValue],[ExternalValue1],[ExternalValue2],[FailedLoginAttempts],[LastInvalidLogin]
       ,[LastResetPassword],[LastLogin],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate])
	   Select @pPid,@EmailId,'',1,N'GyHl3AQLBV4=','101'+@CodeValue,'1',NULL,NULL,NULL,NULL,NULL,999,@LocalDateTime,NULL,NULL,0,NULL,NULL
END
ELSE
BEGIN
	Select @PreviousEmailId = [Email] From [PatientLoginDetail] Where [PatientId] = @pPid AND (ISDeleted is null OR ISDeleted =0)
	IF(@PreviousEmailId <> @EmailId)
	BEGIN
		--Update [PatientLoginDetail] Set IsDeleted =1,DeletedBy =999, DeletedDate=Getdate() where [PatientId] = @pId AND (ISDeleted is null OR ISDeleted =0)

		Delete From [PatientLoginDetail] where [PatientId] = @pPid AND (ISDeleted is null OR ISDeleted =0)

		INSERT INTO [dbo].[PatientLoginDetail]
       ([PatientId],[Email],[TokenId],[PatientPortalAccess],[Password],[CodeValue],[ExternalValue1],[ExternalValue2],[FailedLoginAttempts],[LastInvalidLogin]
       ,[LastResetPassword],[LastLogin],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[IsDeleted],[DeletedBy],[DeletedDate])
	   Select @pPid,@EmailId,'',1,N'GyHl3AQLBV4=','101'+@CodeValue,'1',NULL,NULL,NULL,NULL,NULL,999,@LocalDateTime,NULL,NULL,0,NULL,NULL
	END
END
END





GO


