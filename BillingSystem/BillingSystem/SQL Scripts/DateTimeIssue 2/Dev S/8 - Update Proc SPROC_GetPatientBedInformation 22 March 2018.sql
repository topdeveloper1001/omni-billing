IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetPatientBedInformation')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetPatientBedInformation
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetPatientBedInformation]    Script Date: 22-03-2018 16:24:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPROC_GetPatientBedInformation] ----- [SPROC_GetPatientBedInformation] 2396
(  
@pPatientID int =1018

)
AS  
BEGIN

Declare @RetOutput nvarchar(250), @AssignedSCV nvarchar(20), @OverRidenSCV nvarchar(20), @AssignedRate numeric(9,2), @OverRidenRate numeric(9,2), 
		@FacilityStructureID int, @AssignedBedType int, @OverRidenBedType int,@AssignedSince Date, @PatientName nvarchar(100),
		@BedNumber nvarchar(20),@InRoom nvarchar(20), @InDepartment nvarchar(20),@OnFloor nvarchar(20)
Declare @FST table(BedNumber nvarchar(20),InRoom nvarchar(20), InDepartment nvarchar(20),OnFloor nvarchar(20))
----- First Get the Mapping of Patient With BED which is Still Assigned -- STARTS

Select @FacilityStructureID= FacilityStructureID, @AssignedBedType= BedType,@OverRidenBedType=OverrideBedType,@AssignedSince = Cast(StartDate as Date) 
from MappingPatientBed Where PatientID = @pPatientID and EndDate is NULL;

--- First Get the Mapping of Patient With BED which is Still Assigned -- ENDS



----- Get Bed ServiceCodes and Rates (Applicable - Top 1) --- STARTS
Select Top(1) @AssignedSCV = ServiceCodeValue, @AssignedRate = Rates from BedRateCard Where BedTypes = @AssignedBedType;


Set @OverRidenBedType = isnull(@OverRidenBedType,0)
If @OverRidenBedType > 0 
	Select Top(1) @OverRidenSCV = ServiceCodeValue, @OverRidenRate = Rates from BedRateCard Where BedTypes = @OverRidenBedType;


If isnull(@OverRidenSCV,'') <> ''
	Set @AssignedSCV = @OverRidenSCV


If isnull(@OverRidenRate,0) > 0 
	Set @AssignedRate = @OverRidenRate
----- Get Bed ServiceCodes and Rates (Applicable - Top 1) --- ENDS



----- Now Get the Structure Names (Floor,Dep,Room and BedNumber)--- STARTS
Insert into @FST

Select FS.FacilityStructureName 'Bed', 
(Select FacilityStructureName from FacilityStructure Where FacilityStructureId= FS.ParentID) 'Room',
(Select FacilityStructureName from FacilityStructure Where FacilityStructureId= (Select ParentID from FacilityStructure Where FacilityStructureId= FS.ParentID)) 'Department',
(Select FacilityStructureName from FacilityStructure Where FacilityStructureId= (Select ParentID from FacilityStructure Where FacilityStructureId= (Select ParentID from FacilityStructure Where FacilityStructureId= FS.ParentID))) 'Floor'
from FacilityStructure FS
Where FacilityStructureID = @FacilityStructureID



----- Now Get the Structure Names (Floor,Dep,Room and BedNumber)--- ENDS



Select @PatientName = (PersonFirstName) + ' - ' + (PersonLastName) from PatientInfo Where PatientID = @pPatientID



---- Send back the Results





--Select @BedNumber = BedNumber,@InRoom=InRoom,@InDepartment=InDepartment,@OnFloor=OnFloor from @FST

------  @OverRidenSCV 'OverRidenSCV',@OverRidenRate 'AssignedOverRidenRate'

 Select @pPatientID 'PatientID',@PatientName 'PatientName',BedNumber as BedName, InRoom 'Room', InDepartment 'DepartmentName',OnFloor 'FloorName',@AssignedSince 'BedAssignedOn',@AssignedSCV 'patientBedService',CAST(@AssignedRate as nvarchar) 'BedRateApplicable' from @FST



--Set @RetOutput = 'Bed Assigned:-' +isnull(@BedNumber,' ')+ ' Dated:-'+Cast(isnull(@AssignedSince,getdate()) as nvarchar(20)) +' In Room:-'+isnull(@InRoom,' ')+' In Department:-'+Isnull(@InDepartment,' ')+' on Floor:-'+isnull(@OnFloor,' ')

--Set @RetOutput = @RetOutput+ ' ServiceCode:-'+isnull(@AssignedSCV, ' ')+ ' Rate:-'+ Cast(isnull(@AssignedRate,0) as Nvarchar(12))



--Select @RetOutput 'ReturnString'



END











GO


