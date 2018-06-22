IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GET_REP_PhysicianChargeReportDetails')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GET_REP_PhysicianChargeReportDetails
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GET_REP_PhysicianChargeReportDetails]    Script Date: 20-03-2018 18:01:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- [SPROC_GET_REP_PhysicianChargeReportDetails] 6,4,'2016-01-01','2016-11-11',0
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GET_REP_PhysicianChargeReportDetails]
(
	@pCorporateId int,
	@pFacilityId int,
	@pFromDate datetime,
	@pTillDate datetime,
	@pPhysicianId int
)
AS
BEGIN
		Declare @LocalDateTime datetime=(Select dbo.GetCurrentDatetimeByEntity(@pFacilityID))

Declare @TReturn table(EncounterId int,PatientId int,PatientName nvarchar(100),EncounterNumber Nvarchar(50),ActivityType nvarchar(50),
ActivityTotal numeric(18,2),OtherTotal numeric(18,2),CorporateId int,FacilityId int,Department numeric(15,0) null,CreatedDate datetime null,
DepartmentName nvarchar(50),PhysicianName nvarchar(200))

Declare @tableToReturn table(OrderCloseDate Date,PhysicianName nvarchar(50),ActivityType nvarchar(100),ActivityCode Nvarchar(50),CodeDescription nvarchar(500),
EncounterId int,PatientId int,PatientName nvarchar(50),EncounterNumber Nvarchar(50),GrossCharges nvarchar(20),TotalGrossCharges numeric(18,2),TotalActivityCost numeric(18,2),
DepartmentNumber nvarchar(50),CreatedDate datetime null,
Department nvarchar(50),Qunatity numeric(18,2))


--- Column Count -= 16

--- Select  column Count -= 13
Insert Into @tableToReturn
Select MAX(Cast(BA.OrderCloseDate as Date)) OrderCloseDate,MAX(P.PhysicianName) 'PhysicianName',MAX(BA.ActivityType) ActivityType,MAX(BA.ActivityCode) ActivityCode, dbo.[GetOrderDescription](MAX(BA.Activitytype),MAX(BA.ActivityCode)) CodeDescription,MAX(BA.EncounterId) EncounterId,
BA.PatientId PatientId,MAX(Pinfo.PersonFirstName) +' '+MAX(Pinfo.PersonLastName) 'PatientName',
	EN.EncounterNumber EncounterNumber,'Gross Charges' 'GrossCharges',
	SUM(BA.Gross) TotalGrossCharges,
	 CAST((dbo.[GetUnitPrice](MAX(BA.Activitytype),MAX(BA.ActivityCode),MAX(BA.CorporateId),MAX(BA.FacilityId),@LocalDateTime) * ISNULL(Sum(BA.QuantityOrdered),0.0)) as numeric(18,2)) 'TotalActivityCost',
	[dbo].GetDepartmentNumberByOrderType(OO.CategoryId) DepartmentNumber,
	MAX(BA.CreatedDate) CreatedDate
	,[dbo].[GetDepartmentNameByOrderType](OO.CategoryId) 'Department',
	MAX(ISNULL(BA.QuantityOrdered,0.00)) 'Qunatity'
from BillActivity BA
	INNER JOIN Encounter EN on EN.ENcounterId = BA.EncounterId
	INNER JOIN PatientInfo Pinfo on Pinfo.PatientId = BA.PatientId
	INNER JOIN OpenOrder OO on OO.OpenOrderId = BA.ActivityID
	LEFT JOIN Physician P ON EN.EncounterAttendingPhysician = P.Id
	LEFT JOIN PatientInsurance I ON BA.PatientId = I.PatientID AND I.IsPrimary = 1
WHERE BA.CorporateId=@pCorporateId and (BA.FacilityId =@pFacilityId OR @pFacilityId = 0)
	and BA.Activitytype <> 8
	and  Cast(OrderCloseDate as Date) between CAST(@pFromDate as Date) and CAST(@pTillDate as Date)
	and (P.Id = @pPhysicianId OR @pPhysicianId=0)
--GROUP by EN.EncounterAttendingPhysician,EN.EncounterNumber,BA.PatientId,OO.CategoryId
GROUP by EN.EncounterNumber,EN.EncounterAttendingPhysician,BA.EncounterId,BA.PatientId,BA.CorporateId,BA.FacilityId,Cast(BA.OrderCloseDate as Date),OO.CategoryId, I.InsuranceCompanyId,BA.ActivityCode, BA.Activitytype
--Order by Cast(BA.OrderCloseDate as Date) desc
Order by Cast(BA.OrderCloseDate as Date) desc
--Order By Department

	-- Total Row for Dept
	Insert into @tableToReturn 
	Select NUll,MAX(PhysicianName),Null,Null,'TOTAL',MAX(EncounterId),MAX(PatientId),Null,Null,NULL
	,SUM(TotalGrossCharges),SUM(TotalActivityCost),MAX(DepartmentNumber),MAX(CreatedDate),MAX(Department),SUM(Qunatity)
	From  @tableToReturn Group by Department

	-- Average Row for Dept
	Insert into @tableToReturn 
	Select NUll,MAX(PhysicianName),Null,Null,'Average',MAX(EncounterId),MAX(PatientId),Null,Null,NULL
	,Cast(SUM(TotalGrossCharges)/SUM(Qunatity) as Numeric(18,2)),Cast(SUM(TotalActivityCost)/SUM(Qunatity) as Numeric(18,2)),MAX(DepartmentNumber),MAX(CreatedDate),MAX(Department),SUM(Qunatity)
	From  @tableToReturn Group by Department
	
	-- Total Row for PhysicianName
	Insert into @tableToReturn 
	Select NUll,MAX(PhysicianName),Null,Null,'Total for the Physician',MAX(EncounterId),MAX(PatientId),Null,Null,NULL
	,SUM(TotalGrossCharges),SUM(TotalActivityCost),MAX(DepartmentNumber),MAX(CreatedDate),MAX(Department),SUM(Qunatity)
	From  @tableToReturn where CodeDescription='TOTAL' Group by PhysicianName

	-- Average Row for PhysicianName
	Insert into @tableToReturn 
	Select NUll,MAX(PhysicianName),Null,Null,'Average for the Physician',MAX(EncounterId),MAX(PatientId),Null,Null,NULL
	,Cast(SUM(TotalGrossCharges)/SUM(Qunatity) as Numeric(18,2)),Cast(SUM(TotalActivityCost)/SUM(Qunatity) as Numeric(18,2)),MAX(DepartmentNumber),MAX(CreatedDate),MAX(Department),SUM(Qunatity)
	From  @tableToReturn where CodeDescription='TOTAL' Group by PhysicianName

	-- Total Row for Report
	Insert into @tableToReturn 
	Select NUll,MAX(PhysicianName),Null,Null,'Total for the Report',MAX(EncounterId),MAX(PatientId),Null,Null,NULL
	,SUM(TotalGrossCharges),SUM(TotalActivityCost),MAX(DepartmentNumber),MAX(CreatedDate),MAX(Department),SUM(Qunatity)
	From  @tableToReturn where CodeDescription='Total for the Physician' Group by PhysicianName

	-- Average Row for Report
	Insert into @tableToReturn 
	Select NUll,MAX(PhysicianName),Null,Null,'Average for the Report',MAX(EncounterId),MAX(PatientId),Null,Null,NULL
	,Cast(SUM(TotalGrossCharges)/SUM(Qunatity) as Numeric(18,2)),Cast(SUM(TotalActivityCost)/SUM(Qunatity) as Numeric(18,2)),MAX(DepartmentNumber),MAX(CreatedDate),MAX(Department),SUM(Qunatity)
	From  @tableToReturn where CodeDescription='Total for the Physician' Group by PhysicianName

	Select * from @tableToReturn Order By  PhysicianName,Department,OrderCloseDate desc --PhysicianName,Department,CodeDescription 
END





GO


