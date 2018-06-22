IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_SetCountersData')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_SetCountersData

/****** Object:  StoredProcedure [dbo].[SPROC_SetCountersData]    Script Date: 3/22/2018 7:38:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_SetCountersData]
(
	@CounterDate datetime=null
)
AS
BEGIN
	
DECLARE  @Cur_CorporateID INT,@Cur_FacilityID INT, @Cur_PatientID INT,@Cur_ActivityTotal INT,@Cur_DepartmentNumber numeric(15,0),@Cur_GrossCharges numeric(18,2),
@CurrentDate date, @CurrentDateTime Datetime,@GlobalCodeValue int,@IsExist int, @PreviousDate date;

Declare @LocalTime datetime = (Select dbo.GetCurrentDatetimeByEntity(0))

IF (@CounterDate is null)
	SET @CounterDate = @LocalTime; --getdate();

Set @CurrentDate = cast(@CounterDate as Date);
Set @CurrentDateTime = @CounterDate;
Set @PreviousDate= Cast(DATEADD(DAY, -1, @CounterDate) as Date);
--print @CurrentDate
-- Patient Days -- 1 GlobalCodeValue Starts
DECLARE CountersData1 CURSOR FOR
			(Select * from [dbo].[GetCounterDataForPetientDays] (@PreviousDate,1,2))
			--Select ISNULL(BCCorporateID,0),ISNULL(BCFacilityID,0),Count(*) from BedCharges
			--Where Cast(DATEADD(DAY, -1, BCCreatedDate) as Date) =  @PreviousDate --Cast(BCCreatedDate as Date) = @CurrentDate
			--group by ISNULL(BCCorporateID,0),ISNULL(BCFacilityID,0); 

Set @GlobalCodeValue =1001 --Patient Days -- 1
OPEN CountersData1;  

FETCH NEXT FROM  CountersData1 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal,@Cur_GrossCharges,@Cur_DepartmentNumber;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter
						 where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID 
						 and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @PreviousDate and DepartmentNumber = @Cur_DepartmentNumber); 
	IF @IsExist = 0
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES (@GlobalCodeValue,@PreviousDate,@Cur_ActivityTotal,@Cur_DepartmentNumber,@Cur_CorporateID
		 ,@Cur_FacilityID,1,@CurrentDateTime,NULL,NULL,1);
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue and DepartmentNumber = @Cur_DepartmentNumber  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and  Cast(ActivityDay as date) =@PreviousDate;

FETCH NEXT FROM  CountersData1 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal,@Cur_GrossCharges,@Cur_DepartmentNumber;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData1;  
DEALLOCATE CountersData1; 
-- Patient Days -- 1 GlobalCodeValue  Ends

-- Outpatient Encounter -- 2 GlobalCodeValue Starts
DECLARE CountersData2 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(EncounterFacility,0),Count(*) from Encounter where EncounterPatientType = 3 and Cast(EncounterStartTime as Date) = @CurrentDate group by ISNULL(CorporateID,0),ISNULL(EncounterFacility,0); 
Set @GlobalCodeValue = 1002 -- Outpatient Encounter -- 2
OPEN CountersData2;  

FETCH NEXT FROM  CountersData2 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID
		 ,@Cur_FacilityID,1,@CurrentDateTime,NULL,NULL,1);
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;

FETCH NEXT FROM  CountersData2 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

END  --END OF @@FETCH_STATUS = 0  

Set @IsExist =	0
CLOSE CountersData2;  
DEALLOCATE CountersData2; 
-- Outpatient Encounter -- 2 GlobalCodeValue  Ends


-- Emergency Room Visit --4 GlobalCodeValue Starts
DECLARE CountersData4 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(EncounterFacility,0),Count(*) from Encounter where EncounterPatientType = 1 and Cast(EncounterStartTime as Date) = @CurrentDate group by ISNULL(CorporateID,0),ISNULL(EncounterFacility,0); 
Set @GlobalCodeValue =1004 -- Emergency Room Visit -- 4
OPEN CountersData4;  

FETCH NEXT FROM  CountersData4 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN

		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData4 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData4;  
DEALLOCATE CountersData4; 
-- Emergency Room Visit --4 GlobalCodeValue Ends


--  Bill Edited and Corrected --6 GlobalCodeValue Starts
DECLARE CountersData6 CURSOR FOR
			Select ISNULL(CorporateID,0),ISNULL(FacilityId,0),Count(*) from ScrubEditTrack where Cast(CreatedDate as Date) = @CurrentDate group by ISNULL(CorporateID,0),ISNULL(FacilityId,0); 
Set @GlobalCodeValue =1006 -- Bill Edited and Corrected --6 
OPEN CountersData6;  

FETCH NEXT FROM  CountersData6 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
		---Insert Command
		BEGIN
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData6 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData6;  
DEALLOCATE CountersData6; 
--  Bill Edited and Corrected --6 GlobalCodeValue Ends

--  Physician Examination --7 GlobalCodeValue Starts
DECLARE CountersData7 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(FacilityId,0),Count(*) from OrderActivity where OrderCategoryID = 11009 and Cast(ExecutedDate as Date) = @CurrentDate Group By ISNULL(CorporateID,0),ISNULL(FacilityId,0); 
Set @GlobalCodeValue =1007 -- Physician Examination --7
OPEN CountersData7;  

FETCH NEXT FROM  CountersData7 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
		---Insert Command
		BEGIN
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData7 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData7;  
DEALLOCATE CountersData7;  
--  Physician Examination --7 GlobalCodeValue Ends


--  Lab Test --9 GlobalCodeValue Starts
DECLARE CountersData9 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(FacilityId,0),Count(*) from OrderActivity where OrderCategoryID = 11080 and Cast(ExecutedDate as Date) = @CurrentDate Group By ISNULL(CorporateID,0),ISNULL(FacilityId,0); 
Set @GlobalCodeValue =1009 -- Lab Test -- 9
OPEN CountersData9;  

FETCH NEXT FROM  CountersData9 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData9 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData9;  
DEALLOCATE CountersData9; 
--  Lab Test --9 GlobalCodeValue Ends

--  Radiology/Imaging Procedures --10 GlobalCodeValue Starts
DECLARE CountersData10 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(FacilityId,0),Count(*) from OrderActivity where OrderCategoryID = 11070 and Cast(ExecutedDate as Date) = @CurrentDate Group By ISNULL(CorporateID,0),ISNULL(FacilityId,0); 
Set @GlobalCodeValue =1010 -- Radiology/Imaging Procedures --10
OPEN CountersData10;  

FETCH NEXT FROM  CountersData10 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData10 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData10;  
DEALLOCATE CountersData10; 
--  Radiology/Imaging Procedures --10 GlobalCodeValue Ends 


--  --Pharmacy Orders --11 GlobalCodeValue Starts
DECLARE CountersData11 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(FacilityId,0),Count(*) from OrderActivity where OrderCategoryID = 11100 and Cast(ExecutedDate as Date) = @CurrentDate Group By ISNULL(CorporateID,0),ISNULL(FacilityId,0); 
Set @GlobalCodeValue =1011 -- --Pharmacy Orders --11 
OPEN CountersData11;  

FETCH NEXT FROM  CountersData11 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData11 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData11;  
DEALLOCATE CountersData11; 
--  --Pharmacy Orders --11 GlobalCodeValue Ends 

--  Inpatient Room Gross Charges  :12 GlobalCodeValue Starts
DECLARE CountersData12 CURSOR FOR
			(Select * from [dbo].[GetCounterDataForPetientDays] (@PreviousDate,1,2))
			--Select IsNULL(BA.CorporateId,0), IsNULL(BA.FacilityId,0),IsNUll(Sum(Gross),0) from BillActivity BA
			--	Inner join Encounter EN on EN.EncounterID = BA.EncounterID
			--	Where Cast(DATEADD(DAY, -1, BA.CreatedDate) as Date) =  @PreviousDate
			--	and ActivityType= 8 and EncounterPatientType = 2  Group by IsNULL(BA.CorporateId,0), IsNULL(BA.FacilityId,0);
Set @GlobalCodeValue =10 -- --Inpatient Room Gross Charges  :12
OPEN CountersData12;  
FETCH NEXT FROM  CountersData12 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal,@Cur_GrossCharges,@Cur_DepartmentNumber;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @PreviousDate
							and DepartmentNumber = @Cur_DepartmentNumber); 
	IF @IsExist = 0
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@PreviousDate,@Cur_GrossCharges,@Cur_DepartmentNumber,@Cur_CorporateID
		 ,@Cur_FacilityID,1,@CurrentDateTime,NULL,NULL,1);
	 ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_GrossCharges Where [StatisticDescription] =@GlobalCodeValue and DepartmentNumber = @Cur_DepartmentNumber  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @PreviousDate;
FETCH NEXT FROM  CountersData12 INTO   @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal,@Cur_GrossCharges,@Cur_DepartmentNumber;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData12;  
DEALLOCATE CountersData12; 
-- Inpatient Room Gross Charges  :12 GlobalCodeValue Ends 


--  Registrations :13 GlobalCodeValue Starts
DECLARE CountersData13 CURSOR FOR
			Select ISNULL(CorporateID,0),ISNULL(FacilityId,0),Count(*) from PatientInfo where Cast(CreatedDate as Date) = @CurrentDate Group by ISNULL(CorporateID,0),ISNULL(FacilityId,0);
Set @GlobalCodeValue =1013 -- Registrations :13
OPEN CountersData13;  

FETCH NEXT FROM  CountersData13 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData13 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData13;  
DEALLOCATE CountersData13; 
-- Registrations :13 GlobalCodeValueEnds 

--  --	Discharges :14 GlobalCodeValue Starts
DECLARE CountersData14 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(EncounterFacility,0),Count(*) from Encounter where EncounterPatientType = 2 and Cast(EncounterEndTime as Date) = @CurrentDate Group by ISNULL(CorporateID,0),ISNULL(EncounterFacility,0);
Set @GlobalCodeValue =1014 -- Discharges :14
OPEN CountersData14;  

FETCH NEXT FROM  CountersData14 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData14 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData14;  
DEALLOCATE CountersData14; 
--	Discharges :14 GlobalCodeValue Ends 


--  Admissions :15 GlobalCodeValue Starts
DECLARE CountersData15 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(EncounterFacility,0),Count(*) from Encounter where EncounterPatientType = 2 and Cast(EncounterStartTime as Date) = @CurrentDate Group by ISNULL(CorporateID,0),ISNULL(EncounterFacility,0);
Set @GlobalCodeValue =1015 -- Admissions :15 GlobalCodeValue
OPEN CountersData15;  

FETCH NEXT FROM  CountersData15 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

--print 'Admissions :15 GlobalCodeValue';
WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData15 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData15;  
DEALLOCATE CountersData15;  
--	Admissions :15 GlobalCodeValue Ends 


--  Insurance Cash Collected :16 GlobalCodeValue Starts
set language british
DECLARE CountersData16 CURSOR FOR
			Select ISNULL(CorporateID,0),ISNULL(FacilityID,0),IsNUll(Sum(Cast(Isnull(XAAPaymentAmount,0) as Decimal(15,2))),0) from XAdviceXMLParsedData where Cast(XACDateSettlement AS date) = @CurrentDate
				Group by ISNULL(CorporateID,0),ISNULL(FacilityID,0)
Set @GlobalCodeValue =1016 -- Insurance Cash Collected :16 GlobalCodeValue
OPEN CountersData16;  

FETCH NEXT FROM  CountersData16 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

--print 'Insurance Cash Collected :16';
WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData16 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData16;  
DEALLOCATE CountersData16;  

--	Insurance Cash Collected :16 GlobalCodeValue Ends 
set language Us_english
--  1st Time Denial :17 GlobalCodeValue Starts
DECLARE CountersData17 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(FacilityID,0),Count(*) from BillHeader Where Status =70 and (PatientDateSettlement is not null and  Cast(PatientDateSettlement as date) = @CurrentDate)
				Group by ISNULL(CorporateID,0),ISNULL(FacilityID,0)
Set @GlobalCodeValue =1017 -- 1st Time Denial :17
OPEN CountersData17;  

FETCH NEXT FROM  CountersData17 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData17 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData17;  
DEALLOCATE CountersData17;  
--	1st Time Denial :17 GlobalCodeValue Ends 

--  Inpatient Ancillary Gross Charges  :18 GlobalCodeValue Starts
DECLARE CountersData18 CURSOR FOR
			(Select * from [dbo].[GetCounterDataForPetientDays] (@PreviousDate,2,2))
			--Select IsNULL(BA.CorporateID,0),ISNULL(BA.FacilityID,0),ISNULL(Sum(BA.Gross),0) from BillActivity BA
			--	Inner join Encounter EN on EN.EncounterID = BA.EncounterID
			--	Where Cast(BA.CreatedDate as Date) = @CurrentDate and BA.ActivityType <> 8
			--	and EncounterPatientType = 2 -- Inpatient Encounterstart type
			--	Group by IsNULL(BA.CorporateID,0),ISNULL(BA.FacilityID,0)
Set @GlobalCodeValue =1018 -- Inpatient Ancillary Gross Charges  :18
OPEN CountersData18;  

FETCH NEXT FROM  CountersData18 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal,@Cur_GrossCharges,@Cur_DepartmentNumber;

--print 'Inpatient Ancillary Gross Charges  :18';
WHILE @@FETCH_STATUS = 0  
	BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate
						and DepartmentNumber = @Cur_DepartmentNumber); 
	IF @IsExist = 0
	BEGIN
		---Insert Command 
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_GrossCharges,@Cur_DepartmentNumber,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_GrossCharges Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate and DepartmentNumber = @Cur_DepartmentNumber;
FETCH NEXT FROM  CountersData18 INTO   @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal,@Cur_GrossCharges,@Cur_DepartmentNumber;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData18;  
DEALLOCATE CountersData18; 
--	Inpatient Ancillary Gross Charges  :18 GlobalCodeValue Ends 


--  Outpatient Ancillary Gross Charges  :19 GlobalCodeValue Starts
DECLARE CountersData19 CURSOR FOR
		(Select * from [dbo].[GetCounterDataForPetientDays] (@PreviousDate,0,3))
			
Set @GlobalCodeValue =1019 -- Outpatient Ancillary Gross Charges  :19
OPEN CountersData19;  

FETCH NEXT FROM  CountersData19 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal,@Cur_GrossCharges,@Cur_DepartmentNumber;

WHILE @@FETCH_STATUS = 0  
	BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate
						and DepartmentNumber = @Cur_DepartmentNumber); 
	IF @IsExist = 0
	BEGIN
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_GrossCharges,@Cur_DepartmentNumber,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
	END
	ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_GrossCharges Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate and DepartmentNumber = @Cur_DepartmentNumber;
FETCH NEXT FROM  CountersData19 INTO   @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal,@Cur_GrossCharges,@Cur_DepartmentNumber;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData19;  
DEALLOCATE CountersData19; 
--	Outpatient Ancillary Gross Charges  :19 GlobalCodeValue Ends 

--  Emergency Room Gross Charges  :20 GlobalCodeValue Starts
DECLARE CountersData20 CURSOR FOR
			(Select * from [dbo].[GetCounterDataForPetientDays] (@PreviousDate,0,1))
Set @GlobalCodeValue =1020 -- Emergency Room Gross Charges  :20
OPEN CountersData20;  

FETCH NEXT FROM  CountersData20 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal,@Cur_GrossCharges,@Cur_DepartmentNumber;

WHILE @@FETCH_STATUS = 0  
	BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate
						and DepartmentNumber = @Cur_DepartmentNumber); 
	IF @IsExist = 0
	BEGIN
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_GrossCharges,@Cur_DepartmentNumber,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
	END
	ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_GrossCharges Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate and DepartmentNumber = @Cur_DepartmentNumber;
FETCH NEXT FROM  CountersData20 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal,@Cur_GrossCharges,@Cur_DepartmentNumber;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData20;  
DEALLOCATE CountersData20;  
--	Emergency Room Gross Charges  :20 GlobalCodeValue Ends 


--  Surgury Orders --21 GlobalCodeValue Starts
DECLARE CountersData21 CURSOR FOR
			select IsNULL(CorporateId,0),ISNULL(FacilityId,0),Count(*) from OrderActivity where OrderCategoryID = 11010 and Cast(ExecutedDate as Date) = @CurrentDate 
			Group By IsNULL(CorporateId,0),ISNULL(FacilityId,0); 
Set @GlobalCodeValue =1021 -- Surgury Orders --21
OPEN CountersData21;  

FETCH NEXT FROM  CountersData21 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

--Print 'Surgury Orders --21 GlobalCodeValue Starts';
WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
	END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData21 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData21;  
DEALLOCATE CountersData21;  
--  Surgury Orders --21 GlobalCodeValue Ends 


--  Patient Cash Collected --22 GlobalCodeValue Starts
DECLARE CountersData22 CURSOR FOR
--			select IsNULL(PayCorporateId,0),ISNULL(PayFacilityId,0),ISNULL(Sum(Payamount),0) from Payment where PayType = 500 and Cast(PayDate AS date) = @CurrentDate
--Group by  IsNULL(PayCorporateId,0),ISNULL(PayFacilityId,0);
select IsNULL(PayCorporateId,0),ISNULL(PayFacilityId,0),sum(ISNULL(Payamount,0)) from Payment where PayType = 500 and Cast(PayDate AS date) = @CurrentDate
Group by  IsNULL(PayCorporateId,0),ISNULL(PayFacilityId,0);
Set @GlobalCodeValue =1022 -- Patient Cash Collected --22 
OPEN CountersData22;  

FETCH NEXT FROM  CountersData22 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

--Print 'Patient Cash Collected --22 GlobalCodeValue Starts';
WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData22 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData22;  
DEALLOCATE CountersData22; 
--  Patient Cash Collected --22 GlobalCodeValue Ends 


-- 2nd Time Denial :23 GlobalCodeValue Starts
DECLARE CountersData23 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(FacilityID,0),Count(*) from BillHeader Where Status =120 and (PatientDateSettlement is not null and  Cast(PatientDateSettlement as date) = @CurrentDate)
				Group by ISNULL(CorporateID,0),ISNULL(FacilityID,0)
Set @GlobalCodeValue =1023 -- 2nd Time Denial :23
OPEN CountersData23;  

FETCH NEXT FROM  CountersData23 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

--Print '2nd Time Denial :23 GlobalCodeValue Starts';
WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and Cast(ActivityDay as date) = @CurrentDate;
FETCH NEXT FROM  CountersData23 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData23;  
DEALLOCATE CountersData23;  
--2nd Time Denial :23 GlobalCodeValue Ends 


-- Managed Care Discounts :25 GlobalCodeValue Starts
DECLARE CountersData25 CURSOR FOR
			select ISNULL(CorporateID,0),ISNULL(FacilityID,0),SUM(ISNULL(MCDiscount,0)) from BillActivity Where Cast(DATEADD(DAY, -1, CreatedDate) as Date) =  @PreviousDate
			Group by ISNULL(CorporateID,0),ISNULL(FacilityID,0)
Set @GlobalCodeValue =1025 -- Managed Care Discounts :25
OPEN CountersData25;  

FETCH NEXT FROM  CountersData25 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

--Print ' Managed Care Discounts :25 GlobalCodeValue Starts';
WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId = @Cur_FacilityID and ActivityDay =@PreviousDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@PreviousDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and  Cast(ActivityDay as date) =@PreviousDate;
FETCH NEXT FROM  CountersData25 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData25;  
DEALLOCATE CountersData25; 
--Managed Care Discounts :25 GlobalCodeValue Ends

--IP Bills:27 GlobalCodeValue Starts
DECLARE CountersData27 CURSOR FOR
			select ISNULL(BH.CorporateID,0),ISNULL(BH.FacilityID,0),COUNT(ISNULL(BH.BillHeaderid,0))
				from BillHeader BH
				INNER JOIN Encounter EN on EN.EncounterID =BH.EncounterID
				Where BH.DueDate is not null  and EN.EncounterPatientType = 2
				and CAST(DueDate as Date) = CAST(@CurrentDate as Date)
				Group by ISNULL(BH.CorporateID,0),ISNULL(BH.FacilityID,0),CAST(BH.DueDate as Date)
Set @GlobalCodeValue =1027 -- IP Bills :27
OPEN CountersData27;  

FETCH NEXT FROM  CountersData27 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

--Print 'IP Bills :27 GlobalCodeValue Starts';
WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId = @Cur_FacilityID and ActivityDay =@CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and  Cast(ActivityDay as date) =@CurrentDate;
FETCH NEXT FROM  CountersData27 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData27;  
DEALLOCATE CountersData27; 
--IP Bills :27 GlobalCodeValue Ends


--OP Bills :28 GlobalCodeValue Starts
DECLARE CountersData28 CURSOR FOR
			select ISNULL(BH.CorporateID,0),ISNULL(BH.FacilityID,0),COUNT(ISNULL(BH.BillHeaderid,0))
				from BillHeader BH
				INNER JOIN Encounter EN on EN.EncounterID =BH.EncounterID
				Where BH.DueDate is not null  and EN.EncounterPatientType = 3
				and CAST(DueDate as Date) = CAST(@CurrentDate as Date)
				Group by ISNULL(BH.CorporateID,0),ISNULL(BH.FacilityID,0),CAST(BH.DueDate as Date)
Set @GlobalCodeValue =1028 -- OP Bills :28
OPEN CountersData28;  

FETCH NEXT FROM  CountersData28 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

--Print 'OP Bills :28 GlobalCodeValue Starts';
WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId = @Cur_FacilityID and ActivityDay =@CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and  Cast(ActivityDay as date) =@CurrentDate;
FETCH NEXT FROM  CountersData28 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData28;  
DEALLOCATE CountersData28; 
--OP Bills :28 GlobalCodeValue Ends

--ER Bills :29 GlobalCodeValue Starts
DECLARE CountersData29 CURSOR FOR
			select ISNULL(BH.CorporateID,0),ISNULL(BH.FacilityID,0),COUNT(ISNULL(BH.BillHeaderid,0))
				from BillHeader BH
				INNER JOIN Encounter EN on EN.EncounterID =BH.EncounterID
				Where BH.DueDate is not null  and EN.EncounterPatientType = 1
				and CAST(DueDate as Date) = CAST(@CurrentDate as Date)
				Group by ISNULL(BH.CorporateID,0),ISNULL(BH.FacilityID,0),CAST(BH.DueDate as Date)
Set @GlobalCodeValue =1029 -- ER Bills :29
OPEN CountersData29;  

FETCH NEXT FROM  CountersData29 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;

--Print 'ER Bills :29 GlobalCodeValue Starts';
WHILE @@FETCH_STATUS = 0  
				BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId = @Cur_FacilityID and ActivityDay =@CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and  Cast(ActivityDay as date) =@CurrentDate;
FETCH NEXT FROM  CountersData29 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData29;  
DEALLOCATE CountersData29; 
--ER Bills :29 GlobalCodeValue Ends


-- Adjusted Patient Days :30 GlobalCodeValue Starts
DECLARE CountersData30 CURSOR FOR
			(Select * from [dbo].[GetCounterDataForAdjustedPatientDays] (@CurrentDate))

Set @GlobalCodeValue =1030 -- Adjusted Patient Days :29
OPEN CountersData30;  
--Print 'Adjusted Patient Days :30 GlobalCodeValue Starts';
FETCH NEXT FROM  CountersData30 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;


WHILE @@FETCH_STATUS = 0  
BEGIN 
		Set @IsExist =	(Select Count(*) from DashboardTransactionCounter where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId = @Cur_FacilityID and ActivityDay =@CurrentDate); 
	IF @IsExist = 0
	BEGIN
		---Insert Command
		INSERT INTO [DashboardTransactionCounter]([StatisticDescription],[ActivityDay],[ActivityTotal],[DepartmentNumber],[CorporateId],[FacilityId],[CreatedBy]
		       ,[CreatedDate],[ModifiedBy],[ModifiedDate],[IsActive])
		 VALUES
		 (@GlobalCodeValue,@CurrentDate,@Cur_ActivityTotal,1000,@Cur_CorporateID,@Cur_FacilityID,1
		 ,@CurrentDateTime,NULL,NULL,1);
		 END
	  ELSE
	     Update [DashboardTransactionCounter] Set  ActivityTotal = @Cur_ActivityTotal Where [StatisticDescription] =@GlobalCodeValue  and  CorporateId = @Cur_CorporateID and FacilityId =@Cur_FacilityID and  Cast(ActivityDay as date) =@CurrentDate;
FETCH NEXT FROM  CountersData30 INTO  @Cur_CorporateID,@Cur_FacilityID,@Cur_ActivityTotal;
END  --END OF @@FETCH_STATUS = 0  
Set @IsExist =	0
CLOSE CountersData30;  
DEALLOCATE CountersData30; 
--Adjusted Patient Days :30 GlobalCodeValue Ends


		/*--------------This is to set the Values in the Dashboard Budget Table, fetched from DashboardTransactionCounter-------- */
		Declare @TempFacility Table(CId int, FId int)
		Declare @Count int, @CurrentYear int 

		SET @CurrentYear = DatePart(Year,@LocalTime)

		INSERT INTO @TempFacility 
		Select CorporateId, FacilityId From Facility Where ISNULL(IsActive,1)=1 And ISNULL(IsDeleted,0)=0 Order by CorporateId

		Select @Count = Count(1) From @TempFacility

		While @Count > 0
		Begin
			Declare @CId int, @FId int
			Select TOP 1 @CId = @CId, @FId = FId From @TempFacility;
			
			Exec SPROC_SetDBChargesActuals @CId, @FId, '',@CurrentYear;

			Delete From @TempFacility Where CId = @CId And FId = @FId;

			SET @Count -=1
		End
		/*--------------This is to set the Values in the Dashboard Budget Table, fetched from DashboardTransactionCounter-------- */
END
GO


