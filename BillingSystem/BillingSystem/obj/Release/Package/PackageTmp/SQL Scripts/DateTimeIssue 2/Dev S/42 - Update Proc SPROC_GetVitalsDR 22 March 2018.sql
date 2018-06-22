IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetVitalsDR')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetVitalsDR
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetVitalsDR]    Script Date: 22-03-2018 20:05:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
CREATE PROCEDURE [dbo].[SPROC_GetVitalsDR]  
(  
@pPatientID int,  --- PatientID for whom the Data is requested  
@pFromDate datetime,  --- If NULL Then Default is Today --- From Mid Night
@pTillDate datetime		---- If NULL then will Set same as From Date Till Mid Night
)  
AS  
BEGIN  
      
 Declare @CurrentDate datetime= (Select dbo.GetCurrentDatetimeByEntity(0))

  --- Check and SET the SELECTION Date  
 Set @pFromDate = isnull(@pFromDate, @CurrentDate) 
 Set @pTillDate = isnull(@pTillDate, @pFromDate)  

 Set @pFromDate = Cast(@pFromDate as Date)
 Set @pTillDate = Cast(@pTillDate as Date)

 Set @pFromDate = @pFromDate + '00:00'
 Set @pTillDate = @pTillDate + '23:59:59'

       
   Select max(PI.PersonFirstName) 'Name', MV.GlobalCode 'VitalCode', max(GC.GlobalCodeName) 'VitalName',Cast(MV.CommentDate as nvarchar(20)) 'XAxis',  
   CAST(min(isnull(MV.AnswerValueMin,0)) as Numeric(8,2)) 'Minimum',CAST(max(isnull(MV.AnswerValueMax,0)) as Numeric(8,2)) 'Maximum' ,  
   CAST(min(isnull(GC.ExternalValue1,0)) as Numeric(8,2)) 'LowerLimit',CAST(min(isnull(GC.ExternalValue2,0)) as Numeric(8,2)) 'UpperLimit' from MedicalVital MV  
   inner join PatientInfo PI on PI.PatientID = MV.PatientID  
   --inner join GlobalCodes GC on GC.GlobalCodeID = MV.GlobalCode  
     inner join GlobalCodes GC on GC.GlobalCodeValue = MV.GlobalCode  and GC.GlobalCodeCategoryValue ='1901'
   Where MV.PatientID = @pPatientID and MV.CommentDate between @pFromDate and @pTillDate  and isnull(MV.IsDeleted,0) = 0 
   group by MV.GlobalCode,MV.CommentDate
   Order by MV.GlobalCode, MV.CommentDate
  
            
END  












GO


