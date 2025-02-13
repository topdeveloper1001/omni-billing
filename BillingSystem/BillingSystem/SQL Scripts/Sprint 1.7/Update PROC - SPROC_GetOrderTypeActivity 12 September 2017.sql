IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SPROC_GetOrderTypeActivity') 
  DROP PROCEDURE SPROC_GetOrderTypeActivity;
 
GO
CREATE PROCEDURE [dbo].[SPROC_GetOrderTypeActivity]
(
@pEncounterId int =2199, -- 1170
@pCategoryId int=0,
@pStatus nvarchar(50) ='1,2,3,4',
@pPatientId int= 2196,
@pFlag int=1---- 1 for Encounter and 2 is for Patient
)
AS
BEGIN

Declare @LocalDateTable Table(ID int, Value nvarchar(MAX));
Insert into @LocalDateTable
Select * from dbo.Split(',',@pStatus)

DECLARE @OrderActivityTempTable Table(
		[OrderActivityID][int] NULL,[OrderType][int] NULL,[OrderCode][nvarchar](50) NULL,[OrderCategoryID][int] NULL,[OrderSubCategoryID][int] NULL,
		[OrderActivityStatus][int] NULL,[CorporateID][int] NULL,[FacilityID][int] NULL,[PatientID][int] NULL,[EncounterID][int] NULL,
		[MedicalRecordNumber][nvarchar](50),[OrderID][int] NULL,[OrderBy][int] NULL,[OrderActivityQuantity][decimal](18,2),[OrderScheduleDate][datetime] NULL,
		[PlannedBy][int] NULL,[PlannedDate][datetime] NULL,[PlannedEndDate][datetime] NULL,[PlannedFor][int] NULL,[ExecutedBy][int] NULL,[ExecutedDate][datetime] null,[ExecutedQuantity][decimal](18,2) null,
		[ResultValueMin][decimal](18,4),[ResultValueMax][decimal](18,2),[ResultUOM][int] NULL,[Comments][nvarchar](500),[PatientName][nvarchar](500),
		[CategoryName][nvarchar](500),[SubCategoryName][nvarchar](500),[OrderDescription][nvarchar](500),[Status][nvarchar](500),[OrderTypeName][nvarchar](	500))
				


			
If(@pFlag=1)
BEGIN
insert into @OrderActivityTempTable
Select DISTINCT OA.OrderActivityId, OA.OrderType, OA.OrderCode,OA.OrderCategoryID,OA.OrderSubCategoryID,OA.OrderActivityStatus,
OA.CorporateID,OA.FacilityID,OA.PatientID,OA.EncounterID,OA.MedicalRecordNumber,OA.OrderID,OA.OrderBy,OA.OrderActivityQuantity,OA.OrderScheduleDate,
OA.PlannedBy,OA.PlannedDate,OA.PlannedDate,OA.PlannedFor,OA.ExecutedBy,OA.ExecutedDate,OA.ExecutedQuantity,OA.ResultValueMin,OA.ResultValueMax,OA.ResultUOM,OA.Comments,
Pinfo.PersonFirstName+' '+Pinfo.PersonLastName AS PatientName,
(Select TOP 1 Gcg.GlobalCodeCategoryName From GlobalCodeCategory Gcg 
Where Gcg.GlobalCodeCategoryValue=CAST(OA.OrderCategoryID as nvarchar) And Gcg.FacilityNumber=CAST(OA.FacilityID as nvarchar)) CategoryName,
(Select TOP 1 GC.GlobalCodeName From GlobalCodes GC
Where GC.GlobalCodeValue = CAST(OA.OrderSubCategoryID as nvarchar) AND GC.FacilityNumber=CAST(OA.FacilityID as nvarchar)) SubCategoryName,
SUBSTRING(dbo.[GetOrderDescription](OA.OrderType,OA.OrderCode),0,100) AS OrderDescription,
 (Select TOP 1 GC1.GlobalCodeName From GlobalCodes GC1
Where GC1.GlobalCodeValue = CAST(OA.OrderActivityStatus as nvarchar) And GC1.GlobalCodeCategoryValue='3103') AS [Status],
(Select TOP 1 GC2.GlobalCodeName From GlobalCodes GC2
Where GC2.GlobalCodeValue = CAST(OA.OrderType as nvarchar) And GC2.GlobalCodeCategoryValue='1201') AS OrderTypeName
from OrderActivity OA
INNER JOIN PatientInfo Pinfo on Pinfo.PatientId = OA.PatientId
--Select OA.OrderActivityId, OA.OrderType, OA.OrderCode,OA.OrderCategoryID,OA.OrderSubCategoryID,OA.OrderActivityStatus,
--OA.CorporateID,OA.FacilityID,OA.PatientID,OA.EncounterID,OA.MedicalRecordNumber,OA.OrderID,OA.OrderBy,OA.OrderActivityQuantity,OA.OrderScheduleDate,
--OA.PlannedBy,OA.PlannedDate,OA.PlannedDate,OA.PlannedFor,OA.ExecutedBy,OA.ExecutedDate,OA.ExecutedQuantity,OA.ResultValueMin,OA.ResultValueMax,OA.ResultUOM,OA.Comments,
--Pinfo.PersonFirstName+' '+Pinfo.PersonLastName AS PatientName,
--SUBSTRING(Gcg.GlobalCodeCategoryName,0,100) AS CategoryName,
--SUBSTRING(GC.GlobalCodeName,0,100) AS SubCategoryName,
-- SUBSTRING(dbo.[GetOrderDescription](OA.OrderType,OA.OrderCode),0,100) AS OrderDescription,
--SUBSTRING(GC1.GlobalCodeName,0,100) AS [Status],
--SUBSTRING(GC2.GlobalCodeName,0,100) AS OrderTypeName
--from OrderActivity OA
--INNER JOIN PatientInfo Pinfo on Pinfo.PatientId = OA.PatientId
--INNER JOIN GlobalCodeCategory Gcg on Gcg.GlobalCodeCategoryValue=OA.OrderCategoryID
--INNER JOIN GlobalCodes GC on GC.GlobalCodeId=OA.OrderSubCategoryID
--INNER JOIN GlobalCodes GC1 on GC1.GlobalCodeValue=OA.OrderActivityStatus and  GC1.GlobalCodeCategoryValue='3103'
--INNER JOIN GlobalCodes GC2 on GC2.GlobalCodeValue=OA.OrderType and  GC2.GlobalCodeCategoryValue='1201'
where OA.EncounterID=@pEncounterId and (@pCategoryId=0 Or OA.OrderCategoryId=@pCategoryId ) 
and CAST(OA.OrderActivityStatus as nvarchar(MAX)) in (Select Value from @LocalDateTable)



insert into @OrderActivityTempTable
Select PA.Id,NULL,PA.TaskNumber,NULL,NULL,CAST(PA.ExtValue4 as Int),PA.CorporateId,PA.FacilityId,PA.PatientId,PA.EncounterId,NULL,NULL,NULL,1.00,PA.StartTime,NULL,
Pa.StartTime,PA.EndTime,NULL,NULL,Pa.AdministrativeOn,NULL,NULL,NULL,NULL,NULL,
Pinfo.PersonFirstName+' '+Pinfo.PersonLastName AS PatientName,CP.Name,PA.TaskName,PA.[Description],case when CAST(PA.ExtValue4 as Int) =1 THEN 'Open' ELSE case when CAST(PA.ExtValue4 as Int) =3 THEN 'Administered' ELSE 'Cancel/Revoked' END END,'Care Task'
from PatientCareActivities PA 
INNER JOIN PatientInfo Pinfo on Pinfo.PatientId = PA.PatientId
INNER JOIN CarePlan CP on CP.Id = PA.ExtValue5
where PA.EncounterId = @pEncounterId and PA.ExtValue4  in (Select Value from @LocalDateTable)
UNION ALL
Select PA.Id,NULL,PA.TaskNumber,NULL,NULL,CAST(PA.ExtValue4 as Int),PA.CorporateId,PA.FacilityId,PA.PatientId,PA.EncounterId,NULL,NULL,NULL,1.00,PA.StartTime,NULL,
Pa.StartTime,PA.EndTime,NULL,NULL,Pa.AdministrativeOn,NULL,NULL,NULL,NULL,NULL,
Pinfo.PersonFirstName+' '+Pinfo.PersonLastName AS PatientName,'Single Task',PA.TaskName,PA.[Description],case when CAST(PA.ExtValue4 as Int) =1 THEN 'Open' ELSE case when CAST(PA.ExtValue4 as Int) =3 THEN 'Administered' ELSE 'Cancel/Revoked' END END,'Care Task'
from PatientCareActivities PA 
INNER JOIN PatientInfo Pinfo on Pinfo.PatientId = PA.PatientId and PA.ExtValue4  in (Select Value from @LocalDateTable)
Where PA.EncounterId = @pEncounterId
and PA.ExtValue5 = '9999'


END

ELSE ---- For Patient 
BEGIN
insert into @OrderActivityTempTable
Select OA.OrderActivityId, OA.OrderType, OA.OrderCode,OA.OrderCategoryID,OA.OrderSubCategoryID,OA.OrderActivityStatus,
OA.CorporateID,OA.FacilityID,OA.PatientID,OA.EncounterID,OA.MedicalRecordNumber,OA.OrderID,OA.OrderBy,OA.OrderActivityQuantity,OA.OrderScheduleDate,
OA.PlannedBy,OA.PlannedDate,OA.PlannedDate,OA.PlannedFor,OA.ExecutedBy,OA.ExecutedDate,OA.ExecutedQuantity,OA.ResultValueMin,OA.ResultValueMax,OA.ResultUOM,OA.Comments,
Pinfo.PersonFirstName+' '+Pinfo.PersonLastName AS PatientName,
--SUBSTRING(Gcg.GlobalCodeCategoryName,0,100) AS CategoryName,
--SUBSTRING(GC.GlobalCodeName,0,100) AS SubCategoryName,
--SUBSTRING(dbo.[GetOrderDescription](OA.OrderType,OA.OrderCode),0,100) AS OrderDescription,
--SUBSTRING(GC1.GlobalCodeName,0,100) AS [Status],
--SUBSTRING(GC2.GlobalCodeName,0,100) AS OrderTypeName
(Select TOP 1 Gcg.GlobalCodeCategoryName From GlobalCodeCategory Gcg 
Where Gcg.GlobalCodeCategoryValue=CAST(OA.OrderCategoryID as nvarchar) And Gcg.FacilityNumber=CAST(OA.FacilityID as nvarchar)) CategoryName,
(Select TOP 1 GC.GlobalCodeName From GlobalCodes GC
Where GC.GlobalCodeValue = CAST(OA.OrderSubCategoryID as nvarchar) AND GC.FacilityNumber=CAST(OA.FacilityID as nvarchar)) SubCategoryName,
SUBSTRING(dbo.[GetOrderDescription](OA.OrderType,OA.OrderCode),0,100) AS OrderDescription,
 (Select TOP 1 GC1.GlobalCodeName From GlobalCodes GC1
Where GC1.GlobalCodeValue = CAST(OA.OrderActivityStatus as nvarchar) And GC1.GlobalCodeCategoryValue='3103') AS [Status],
(Select TOP 1 GC2.GlobalCodeName From GlobalCodes GC2
Where GC2.GlobalCodeValue = CAST(OA.OrderType as nvarchar) And GC2.GlobalCodeCategoryValue='1201') AS OrderTypeName
from OrderActivity OA
INNER JOIN PatientInfo Pinfo on Pinfo.PatientId = OA.PatientId
--INNER JOIN GlobalCodeCategory Gcg on Gcg.GlobalCodeCategoryValue=OA.OrderCategoryID
--INNER JOIN GlobalCodes GC on GC.GlobalCodeId=OA.OrderSubCategoryID
--INNER JOIN GlobalCodes GC1 on GC1.GlobalCodeValue=OA.OrderActivityStatus and  GC1.GlobalCodeCategoryValue='3103'
--INNER JOIN GlobalCodes GC2 on GC2.GlobalCodeValue=OA.OrderType and  GC2.GlobalCodeCategoryValue='1201'
where OA.PatientID=@pPatientId and OA.OrderCategoryId=@pCategoryId and OA.OrderActivityStatus  in (Select Value from @LocalDateTable)

insert into @OrderActivityTempTable
Select PA.Id,NULL,PA.TaskNumber,NULL,NULL,CAST(PA.ExtValue4 as Int),PA.CorporateId,PA.FacilityId,PA.PatientId,PA.EncounterId,NULL,NULL,NULL,1.00,PA.StartTime,NULL,
Pa.StartTime,PA.EndTime,NULL,NULL,Pa.AdministrativeOn,NULL,NULL,NULL,NULL,NULL,
Pinfo.PersonFirstName+' '+Pinfo.PersonLastName AS PatientName,CP.Name,PA.TaskName,PA.[Description],case when CAST(PA.ExtValue4 as Int) =1 THEN 'Open' ELSE case when CAST(PA.ExtValue4 as Int) =3 THEN 'Administered' ELSE 'Cancel/Revoked' END END,'Care Task'
from PatientCareActivities PA 
INNER JOIN PatientInfo Pinfo on Pinfo.PatientId = PA.PatientId
INNER JOIN CarePlan CP on CP.Id = PA.ExtValue5
where PA.PatientId = @pPatientId and PA.ExtValue4  in (Select Value from @LocalDateTable)
Union ALL
Select PA.Id,NULL,PA.TaskNumber,NULL,NULL,CAST(PA.ExtValue4 as Int),PA.CorporateId,PA.FacilityId,PA.PatientId,PA.EncounterId,NULL,NULL,NULL,1.00,PA.StartTime,NULL,
Pa.StartTime,PA.EndTime,NULL,NULL,Pa.AdministrativeOn,NULL,NULL,NULL,NULL,NULL,
Pinfo.PersonFirstName+' '+Pinfo.PersonLastName AS PatientName,'Single Task',PA.TaskName,PA.[Description],case when CAST(PA.ExtValue4 as Int) =1 THEN 'Open' ELSE case when CAST(PA.ExtValue4 as Int) =3 THEN 'Administered' ELSE 'Cancel/Revoked' END END,'Care Task'
from PatientCareActivities PA 
INNER JOIN PatientInfo Pinfo on Pinfo.PatientId = PA.PatientId
where PA.PatientId = @pPatientId and PA.ExtValue4  in (Select Value from @LocalDateTable) 
and  PA.ExtValue5 = '9999'

END

select * from @OrderActivityTempTable

END




