IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetDBEncounterType')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetDBEncounterType
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetDBEncounterType]    Script Date: 20-03-2018 17:13:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_GetDBEncounterType]
(  
@pCorporateID int,  
@pFacilityID int,
@pAsOnDate Datetime,
@pViewType nvarchar(2)  
)  
AS  
BEGIN

Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

Declare @FacilityNumber varchar(50), @PastMonth int

--- Set Conditions -- STARTS
Set @pAsOnDate = isnull(@pAsOnDate,@LocalDateTime)

---- Needed due to different DataTypes in Different Tables
Set @FacilityNumber = Cast(@pFacilityID as Varchar(50))

---- Set Default View Type --- Monthly 
If @pViewType not in ('Y','M')
	Set @pViewType = 'M'

---- Set  Default Value of Past Month
Set @PastMonth = 1

------ Set Past Months incase of Years
If @pViewType = 'Y'
	Set @PastMonth = MONTH(@pAsOnDate)
	
---- Check if GlobalCodes are setup for passed in FacilityID --- If not then use Master FacilityID = 0
if NOT EXISTS (Select top 1 GlobalCodeValue from GlobalCodes GC Where GC.GlobalCodeCategoryValue = 1107 and FacilityNumber = @FacilityNumber)
	Set @FacilityNumber = '0'

--- Set Conditions -- ENDS

Select GC.GlobalCodeValue 'PatientType',GC.GlobalCodeName 'TypeName',isnull((CAST(GC.ExternalValue1 AS INT) * @PastMonth ),0)  'Budget',
[dbo].[GetEncounterCount] (@pFacilityID,GC.GlobalCodeValue,@pASOnDate,@pViewType,1) 'Current', 
[dbo].[GetEncounterCount] (@pFacilityID,GC.GlobalCodeValue,@pASOnDate,@pViewType,0) 'Previous'
from GlobalCodes GC
Where GC.GlobalCodeCategoryValue = 1107 and FacilityNumber = @FacilityNumber


END













GO


