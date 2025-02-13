IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SPROC_GetActiveLabOrdersByPhysicianId') 
  DROP PROCEDURE SPROC_GetActiveLabOrdersByPhysicianId;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Proc [dbo].[SPROC_GetActiveLabOrdersByPhysicianId] 
(
@UserId	int =10,
@OrderActivityStatus int=0,
@OrderCategory nvarchar(100) ='11080',
@IsActiveEncountersOnly bit =1,
@EncounterId int =1077
)
As
Begin
	Declare @FId bigint=0

	IF @EncounterId > 0
		Select TOP 1 @FId=EncounterFacility From Encounter Where EncounterID=@EncounterId

	Select E.EncounterEndTime, OA.PatientID, 'CPT' OrderTypeName, OA.OrderCode,dbo.GetOrderDescription('3',OA.OrderCode) 'OrderDescription',
	(Select TOP 1 GlobalCodeCategoryName from GlobalCodeCategory where GlobalCodeCategoryValue = OA.OrderCategoryID And FacilityNumber=@FId) 'CategoryName',
	(Select TOP 1 GlobalCodeName from GlobalCodes where GlobalCodeCategoryValue=OA.OrderCategoryID AND GlobalCodeValue = CAST(OA.OrderSubCategoryID as nvarchar) And FacilityNumber=@FId) 'SubCategoryName'
	,(Select TOP 1 GC.GlobalCodeName from GlobalCodes GC where GC.GlobalCodeCategoryValue='3105'
		and GC.GlobalCodeValue = (Select TOP 1 LL.LabTestResultSpecimen from [dbo].[LabTestResult] LL where LL.LabTestResultCPTCode = OA.OrderCode)) AS 'SpecimenTypeStr'
	,OA.OrderScheduleDate,OA.ExecutedDate,OA.OrderActivityQuantity,dbo.GetGlobalCodeNameByCategoryAndCodeValue('3103',OA.OrderActivityStatus) 'Status'
	,OA.Comments,
	OA.OrderActivityID,
	CASE WHEN OA.ResultValueMin IS NULL THEN 'UnKnown' ELSE dbo.GetLabTestResultStatus(OA.ResultValueMin,OA.OrderCode,OA.PatientID) END 'LabResultTypeStr' ,
	OA.ResultValueMin
	From OrderActivity OA
	INNER JOIN OpenOrder O ON OA.OrderID = O.OpenOrderID
	INNER JOIN Encounter E ON O.EncounterID = E.EncounterID
	Where OA.OrderBy=@UserId									--10
	And OA.OrderActivityStatus NOT IN (0,1,20)
	And OA.OrderCategoryId = @OrderCategory						-----11080
	AND OA.EncounterID = @EncounterId
	And E.EncounterEndTime IS NULL 
	Order by OA.OrderScheduleDate DESC

End

--SPROC_GetActiveLabOrdersByPhysicianId 10,1,'11080',1,1077











