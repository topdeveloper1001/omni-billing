IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_ZDB_NewIndicator_DataUpload')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_ZDB_NewIndicator_DataUpload


GO

/****** Object:  StoredProcedure [dbo].[SPROC_ZDB_NewIndicator_DataUpload]    Script Date: 3/22/2018 7:59:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<BB>
-- Create date: <Feb-2016>
-- Description:	< New Indicator Created It willl Upload ZERO Data for DashBoardData and ManualDashBoard Tables With all Facilities and 9999>
				
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_ZDB_NewIndicator_DataUpload]  
	(
	@CID int = 12, ---- CorporateID  (MUST)
	@IndicatorNumber int = 701, ---- IndicatorNumber (MUST)  --- NOTE: It will TAKE Care of ALL SUB CATEGORIES AS WELL under the passed in Indicator Number
	@Year int = 2016, ---- Year for which Data needs to be Uploaded as ZERO  (MUST)
	@LIUser int = 911,  --- Logged In User (Just to Distinguish New Records Entered
	@SC1 int,
	@SC2 int
	)
AS
BEGIN

Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))
Delete from [dbo].[DashboardIndicatorData] Where CorporateID = @CID and IndicatorNumber = @IndicatorNumber and [Year] = @Year and subcategory1 = @SC1 and subcategory2 = @SC2
--- Select * from [dbo].[DashboardIndicatorData] Where IndicatorNumber = 701 
Delete from ManualDashBoard Where CorporateID = @CID and Indicators = @IndicatorNumber and [Year] = @Year and subcategory1 = @SC1 and subcategory2 = @SC2
--- Select * from ManualDashBoard Where CorporateID = 12 and Indicators = 701 --- and [Year] = 2016
---- Update  [dbo].[DashboardIndicatorData] Set FacilityID = 17 Where IndicatorNumber = 701 

--Select * from [dbo].[DashboardIndicatorData] Where IndicatorNumber = @IndicatorNumber and [Year] = @Year and StatisticData <> '0.0' and  StatisticData <> '0.0000' 
--order by FacilityID,[Year],[Month],[IndicatorNumber],[SubCategory1],[SubCategory2],[ExternalValue3],[ExternalValue1]

--Select * from Facility Where CorporateID = 12

----- Entries for New Indicator with ZERO Data
insert into [dbo].[DashboardIndicatorData] 
(IndicatorId, IndicatorNumber, SubCategory1, SubCategory2, StatisticData, Month, Year, FacilityId, CorporateId, CreatedBy, CreatedDate, ExternalValue1,
                          ExternalValue2, ExternalValue3, IsActive, DepartmentNumber)
Select DI.IndicatorID,DI.IndicatorNumber,DI.SubCategory1,DI.SubCategory2, DBND.NDData, DBND.NDMonth,@Year 'Year',F.FacilityID  'Facility',@CID 'Corporate',
@LIUser,@CurrentDate 'CreatedOn',DBND.NDEV1, DBND.NDEV2, DBND.NDEV3,DBND.NDIsActive, DBND.NDDepartment
 from [DashboardIndicators] DI inner join DBNewData DBND on 1 = 1
 inner join Facility F on F.CorporateID = @CID --- and F.FacilityID in (14,17,18)
Where DI.CorporateID = @CID and Di.IndicatorNumber = @IndicatorNumber and subcategory1 = @SC1 and subcategory2 = @SC2


------- FOR 9999 Entries  --- TOTAL of Facilities
insert into [dbo].[DashboardIndicatorData] 
(IndicatorId, IndicatorNumber, SubCategory1, SubCategory2, StatisticData, Month, Year, FacilityId, CorporateId, CreatedBy, CreatedDate, ExternalValue1,
                          ExternalValue2, ExternalValue3, IsActive, DepartmentNumber)
Select DI.IndicatorID,DI.IndicatorNumber,DI.SubCategory1,DI.SubCategory2, DBND.NDData, DBND.NDMonth,@Year 'Year', '9999',@CID 'Corporate',
@LIUser,@CurrentDate 'CreatedOn',DBND.NDEV1, DBND.NDEV2, DBND.NDEV3,DBND.NDIsActive, DBND.NDDepartment
 from [DashboardIndicators] DI inner join DBNewData DBND on 1 = 1
 ---inner join Facility F on F.CorporateID = @CID --- and F.FacilityID in (14,17,18)
Where DI.CorporateID = @CID and Di.IndicatorNumber = @IndicatorNumber and subcategory1 = @SC1 and subcategory2 = @SC2

----- Now COPY same to Manual DashBoard Table ---- IndicatorNumber,SubCategory1,SubCategory1   - STARTS
Declare @CopyDBData table (CID int, FID int,LiUser int, Indicator int, SC1 nvarchar(50),SC2 Nvarchar(50))
Declare @Cur_CID int, @Cur_FID int,@Cur_LiUser int, @Cur_Indicator int, @Cur_SC1 nvarchar(50),@Cur_SC2 Nvarchar(50)
Declare @TopFacility int

insert into @CopyDBData
 Select @CID,F.FacilityId,@LIUser,DI.IndicatorNumber,DI.SubCategory1,DI.SubCategory2 from [DashboardIndicators] DI
	inner join Facility F on F.CorporateID = @CID Where DI.CorporateID = @CID and DI.IndicatorNumber = @IndicatorNumber;

Select Top(1) @TopFacility = FID from @CopyDBData;
insert into @CopyDBData Select CID,9999,LiUser,Indicator,SC1,SC2 from @CopyDBData Where FID = @TopFacility

-- Select * from @CopyDBData;

Declare CopyDB Cursor For
			Select * from @CopyDBData;

		Open CopyDB;
		Fetch Next From CopyDB into @Cur_CID, @Cur_FID,@Cur_LiUser, @Cur_Indicator, @Cur_SC1 ,@Cur_SC2

		WHILE @@FETCH_STATUS = 0  
		BEGIN
			
			EXEC SPROC_CopyToManualDashboard_SA @Cur_Indicator,@Cur_CID, @Cur_FID, @Cur_LiUser,@Cur_SC1,@Cur_SC2

			Fetch Next From CopyDB into @Cur_CID, @Cur_FID,@Cur_LiUser, @Cur_Indicator, @Cur_SC1 ,@Cur_SC2

		END

-- Clean Up
	CLOSE CopyDB;  
	DEALLOCATE CopyDB;
----- Now COPY same to Manual DashBoard Table ---- IndicatorNumber,SubCategory1,SubCategory1   - ENDS

END





GO


