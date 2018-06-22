IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetVitals')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetVitals
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetVitals]    Script Date: 22-03-2018 20:03:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_GetVitals]  
(  
@pPatientID int,  --- PatientID for whom the Data is requested  
@pDisplayTypeID int, --- Display Value on X Axis to be Seen VALID Entries --> 900=Hours, 901=WeekDays, 902=Weeks, 903=Months  
@pRECDate datetime  --- If NULL Then Default is Today --- If Passed then(will be treated as TILL Date) which will be the reference point from where Past Data will be presented based on DisplayType requested  
)  
AS  
BEGIN  
      DECLARE @Facility_Id int=(select FacilityId from dbo.PatientInfo WHERE PatientID=@pPatientID)

		Declare @LocalDateTime datetime= (Select dbo.GetCurrentDatetimeByEntity(@Facility_Id))

 Declare @DateFrom datetime,
		 @DateTill datetime  
     
    
 --- Check and SET the SELECTION Date  
 If @pRECDATE is NULL  
  Set @pRECDATE = @LocalDateTime --@LocalDateTime  
  
 --- Selection Based on Display Type  
 If @pDisplayTypeID = 900  -- Hour Wise  
 BEGIN  
   SET @DateFrom = @pRECDATE
   SET @DateTill = DATEADD(day,1,@DateFrom)  
     
   Select max(PI.PersonFirstName) 'Name', MV.GlobalCode 'VitalCode', max(GC.GlobalCodeName) 'VitalName', 'Hrs-'+DateName(HOUR,MV.CommentDate)  'XAxis',  
   CAST(Avg(MV.AnswerValueMin) as Numeric(8,2)) 'Average',CAST(max(MV.AnswerValueMin) as Numeric(8,2)) 'Maximum' ,  
   CAST(min(MV.AnswerValueMin) as Numeric(8,2)) 'Minimum',CAST(min(GC.ExternalValue1) as Numeric(8,2)) 'LowerLimit',CAST(min(GC.ExternalValue2) as Numeric(8,2)) 'UpperLimit' from MedicalVital MV  
   inner join PatientInfo PI on PI.PatientID = MV.PatientID  
   inner join GlobalCodes GC on GC.GlobalCodeID = MV.GlobalCode  
   Where MV.PatientID = @pPatientID and MV.CommentDate between @DateFrom and @DateTill  
   group by MV.GlobalCode,DateName(HOUR,MV.CommentDate)  
   Order by MV.GlobalCode, CAST(DateName(HOUR,MV.CommentDate) as Integer)
 END  
  
 ---- WeekDay Wise  
 If @pDisplayTypeID = 901    
 BEGIN  
	---- Week Logic --- Need to make 1 less day as DATEPART  will start from Sunday 
   SET @DateFrom = DATEADD(week, DATEDIFF(week, 0, (@pRECDATE)), 0)-1
   SET @DateTill = DATEADD(day,7,@DateFrom)  
  
   Select max(PI.PersonFirstName) 'Name', MV.GlobalCode 'VitalCode', max(GC.GlobalCodeName) 'VitalName', DateName(WEEKDAY,MV.CommentDate) 'XAxis', 
   CAST(Avg(MV.AnswerValueMin) as Numeric(8,2)) 'Average',CAST(max(MV.AnswerValueMin) as Numeric(8,2)) 'Maximum' ,  
   CAST(min(MV.AnswerValueMin) as Numeric(8,2)) 'Minimum',CAST(min(GC.ExternalValue1) as Numeric(8,2)) 'LowerLimit',CAST(min(GC.ExternalValue2) as Numeric(8,2)) 'UpperLimit'  from MedicalVital MV  
   inner join PatientInfo PI on PI.PatientID = MV.PatientID  
   inner join GlobalCodes GC on GC.GlobalCodeID = MV.GlobalCode  
   Where MV.PatientID = @pPatientID and MV.CommentDate between @DateFrom and @DateTill  
   group by MV.GlobalCode,DateName(WEEKDAY,MV.CommentDate), Datepart(DW, MV.CommentDate)  
   Order by MV.GlobalCode,  Datepart(DW, MV.CommentDate)  
 END  
  
 ---- Month Wise (WEEKLY)  
 If @pDisplayTypeID = 902    
 BEGIN  
   SET @DateFrom = DATEADD(month, DATEDIFF(MONTH, 0, @pRECDATE), 0)
   SET @DateTill = DATEADD(Month,1,@DateFrom)  
  
   Select max(PI.PersonFirstName) 'Name', MV.GlobalCode 'VitalCode', max(GC.GlobalCodeName) 'VitalName', 'Week-'+ DateName(WEEK,MV.CommentDate) 'XAxis',   
   CAST(Avg(MV.AnswerValueMin) as Numeric(8,2)) 'Average',CAST(max(MV.AnswerValueMin) as Numeric(8,2)) 'Maximum' ,  
   CAST(min(MV.AnswerValueMin) as Numeric(8,2)) 'Minimum',CAST(min(GC.ExternalValue1) as Numeric(8,2)) 'LowerLimit',CAST(min(GC.ExternalValue2) as Numeric(8,2)) 'UpperLimit' from MedicalVital MV  
   inner join PatientInfo PI on PI.PatientID = MV.PatientID  
   inner join GlobalCodes GC on GC.GlobalCodeID = MV.GlobalCode  
   Where MV.PatientID = @pPatientID and MV.CommentDate between @DateFrom and @DateTill  
   group by MV.GlobalCode,DateName(WEEK,MV.CommentDate)  
   Order by MV.GlobalCode, CAST(DateName(WEEK,MV.CommentDate) as Integer)
 END  
  
 ---- Month Wise (DAILY)  
 If @pDisplayTypeID = 903    
 BEGIN  
   SET @DateFrom = DATEADD(month, DATEDIFF(MONTH, 0, @pRECDATE), 0)
   SET @DateTill = DATEADD(Month,1,@DateFrom)    
  
   Select max(PI.PersonFirstName) 'Name', MV.GlobalCode 'VitalCode', max(GC.GlobalCodeName) 'VitalName', 'Day-'+DateName(DAY,MV.CommentDate) 'XAxis',   
   CAST(Avg(MV.AnswerValueMin) as Numeric(8,2)) 'Average',CAST(max(MV.AnswerValueMin) as Numeric(8,2)) 'Maximum' ,  
   CAST(min(MV.AnswerValueMin) as Numeric(8,2)) 'Minimum',CAST(min(GC.ExternalValue1) as Numeric(8,2)) 'LowerLimit',CAST(min(GC.ExternalValue2) as Numeric(8,2)) 'UpperLimit' from MedicalVital MV  
   inner join PatientInfo PI on PI.PatientID = MV.PatientID  
   inner join GlobalCodes GC on GC.GlobalCodeID = MV.GlobalCode  
   Where MV.PatientID = @pPatientID and MV.CommentDate between @DateFrom and @DateTill  
   group by MV.GlobalCode,DateName(DAY,MV.CommentDate)  
   Order by MV.GlobalCode, CAST(DateName(DAY,MV.CommentDate) as Integer)  
 END  
  
 ---- Year Wise  
 If @pDisplayTypeID = 904    
 BEGIN  
   SET @DateFrom = DATEADD(YEAR, DATEDIFF(YEAR, 0, @pRECDATE), 0)  
   SET @DateTill = DATEADD(Year,1,@DateFrom)

   Select max(PI.PersonFirstName) 'Name', MV.GlobalCode 'VitalCode', max(GC.GlobalCodeName) 'VitalName', DateName(MONTH,MV.CommentDate) 'XAxis',   
   CAST(Avg(MV.AnswerValueMin) as Numeric(8,2)) 'Average',CAST(max(MV.AnswerValueMin) as Numeric(8,2)) 'Maximum' ,  
   CAST(min(MV.AnswerValueMin) as Numeric(8,2)) 'Minimum',CAST(min(GC.ExternalValue1) as Numeric(8,2)) 'LowerLimit',CAST(min(GC.ExternalValue2) as Numeric(8,2)) 'UpperLimit' from MedicalVital MV  
   inner join PatientInfo PI on PI.PatientID = MV.PatientID  
   inner join GlobalCodes GC on GC.GlobalCodeID = MV.GlobalCode  
   Where MV.PatientID = @pPatientID and MV.CommentDate between @DateFrom and @DateTill  
   group by MV.GlobalCode,DateName(MONTH,MV.CommentDate),Datepart(MM, MV.CommentDate)  
   Order by MV.GlobalCode,Datepart(MM, MV.CommentDate)  
 END  
  
            
END  













GO


