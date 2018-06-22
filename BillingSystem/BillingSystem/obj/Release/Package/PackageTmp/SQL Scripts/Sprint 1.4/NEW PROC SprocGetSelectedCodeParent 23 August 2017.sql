IF EXISTS (SELECT * 
           FROM   information_schema.routines 
           WHERE  specific_schema = N'dbo' 
                  AND specific_name = N'SprocGetSelectedCodeParent') 
  DROP PROCEDURE SprocGetSelectedCodeParent;
 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>


--Exec SprocGetSelectedCodeParent '10061','3',8,'4010'
--Select TOP 500 * From CPTCodes Where CodeTableNumber='4010'
-- =============================================
CREATE PROCEDURE SprocGetSelectedCodeParent 
(
@pOrderCode nvarchar(100),
@pOrderCodeType nvarchar(10),
@pFId bigint,
@pTableNumber nvarchar(100)
)
AS
BEGIN
	Declare @CodeRangeValue bigint
	IF @pOrderCodeType=3
	Begin
		Declare @GC Table([GlobalCodeID] [int],[FacilityNumber] [nvarchar](50) NOT NULL,[GlobalCodeCategoryValue] [varchar](20) NULL,
		[GlobalCodeValue] [varchar](20) NULL,[GlobalCodeName] [varchar](250) NULL,[Description] [varchar](1000) NULL,[ExternalValue1] [varchar](50) NULL,
		[ExternalValue2] bigint,[ExternalValue3] bigint,[ExternalValue4] [varchar](50) NULL,[ExternalValue5] [varchar](50) NULL,
		[SortOrder] [int] NULL,[IsActive] [bit] NOT NULL,[CreatedBy] [int] NULL,[CreatedDate] [datetime] NOT NULL,[ModifiedBy] [int] NULL,[ModifiedDate] [datetime] NULL,
		[IsDeleted] [bit] NULL,[DeletedBy] [int] NULL,[DeletedDate] [datetime] NULL,[ExternalValue6] [varchar](50) NULL,GlobalCodeCategoryNameStr nvarchar(100))

		INSERT INTO @GC
		Select GC.[GlobalCodeID],GC.[FacilityNumber],GC.[GlobalCodeCategoryValue],GC.[GlobalCodeValue],GC.[GlobalCodeName],GC.[Description],GC.[ExternalValue1]
		,CAST(GC.[ExternalValue2] as bigint),CAST(GC.[ExternalValue3] as bigint),GC.[ExternalValue4],GC.[ExternalValue5],GC.[SortOrder],GC.[IsActive],GC.[CreatedBy],GC.[CreatedDate]
		,GC.[ModifiedBy],GC.[ModifiedDate],GC.[IsDeleted],GC.[DeletedBy],GC.[DeletedDate],GC.[ExternalValue6]
		,(Select TOP 1 G.GlobalCodeCategoryName From GlobalCodeCategory G Where G.GlobalCodeCategoryValue=GC.GlobalCodeCategoryValue) As GlobalCodeCategoryNameStr 
		From GlobalCodes GC Where ISNULL(GC.IsDeleted,0)=0 
		And ISNULL(GC.IsActive,1)=1 And GC.ExternalValue1=@pOrderCodeType		
		And (ISNULL(@pFId,0)=0 OR GC.FacilityNumber=@pFId)
		
		Select @CodeRangeValue=ISNULL(CTPCodeRangeValue,0) From CPTCodes Where CodeNumbering=@pOrderCode And CodeTableNumber=@pTableNumber

		Select * From @GC
		Where ExternalValue2 <= @CodeRangeValue
		And ExternalValue3 >= @CodeRangeValue
	End
	ELSE IF @pOrderCodeType=5
	Begin
		Select @CodeRangeValue=ISNULL(BrandCode,'0') From Drug Where DrugCode=@pOrderCode And DrugTableNumber=@pTableNumber
		Select GC.*
		,(Select TOP 1 G.GlobalCodeCategoryName From GlobalCodeCategory G Where G.GlobalCodeCategoryValue=GC.GlobalCodeCategoryValue) As GlobalCodeCategoryNameStr 
		From GlobalCodes GC Where ISNULL(GC.IsDeleted,0)=0 
		And ISNULL(GC.IsActive,1)=1 AND GC.GlobalCodeValue=CAST(@CodeRangeValue as nvarchar)
		And (ISNULL(@pFId,0)=0 OR GC.FacilityNumber=@pFId)
	End
	IF @pOrderCodeType=4
	Begin
		--Select @CodeRangeValue=ISNULL(CTPCodeRangeValue,0) From HCPCSCodes Where CodeNumbering=@pOrderCode And CodeTableNumber=@pTableNumber
		--Select GC.*
		--,(Select TOP 1 G.GlobalCodeCategoryName From GlobalCodeCategory G Where G.GlobalCodeCategoryValue=GC.GlobalCodeCategoryValue) As GlobalCodeCategoryNameStr 
		--From GlobalCodes GC Where ISNULL(GC.IsDeleted,0)=0 
		--And ISNULL(GC.IsActive,1)=1 And GC.ExternalValue1=@pOrderCodeType
		Select 'HCPCS Codes' As GlobalCodeCategoryNameStr, '11200' As GlobalCodeCategoryValue,'NA' As GlobalCodeName, 9090 As GlobalCodeId
	End
END
GO
