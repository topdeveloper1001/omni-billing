-- Drop stored procedure if it already exists
IF OBJECT_ID('SPROC_CloseOP_EREncounters','P') IS NOT NULL
   DROP PROCEDURE SPROC_CloseOP_EREncounters
GO

--SPROC_CloseOP_EREncounters 9,'3001',8
CREATE PROCEDURE [dbo].[SPROC_CloseOP_EREncounters] 
	
AS
BEGIN
	Declare @Entity As Table (CId bigint,FId bigint,FNumber nvarchar(50))
	Declare @Count bigint=0

	INSERT INTO @Entity (CId,FNumber,FId)
	Select CorporateId,FacilityNumber,FacilityId from Facility Where IsActive=1 And IsDeleted=0

	Select @Count=Count(1) From @Entity

	While @Count>0
	Begin
		Declare @FId bigint=0,@CId bigint=0,@FNumber nvarchar(50)
		Select TOP 1 @CId=CId,@FId=FId,@FNumber=FNumber From @Entity
		EXEC SprocCheckAndCloseOPandEREncounters @CId,@FNumber,@FId

		Delete From @Entity Where FId=@FId
		Set @Count=@Count-1;
	End
END












