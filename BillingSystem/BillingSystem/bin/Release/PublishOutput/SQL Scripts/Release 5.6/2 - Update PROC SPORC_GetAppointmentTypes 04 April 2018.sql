IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPORC_GetAppointmentTypes')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[SPORC_GetAppointmentTypes]
GO

/****** Object:  StoredProcedure [dbo].[SPORC_GetAppointmentTypes]    Script Date: 4/4/2018 7:26:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[SPORC_GetAppointmentTypes]  ---- SPORC_GetAppointmentTypes 8,9, 1
(
@FacilityId int,
@CorporateId int,
@ShowInActive bit
)
AS
BEGIN
	Declare @AppointmentTempTable table(
	[Id] [int] ,[Description] [nvarchar](max) NULL,[CategoryNumber] [nvarchar](50) NULL,[CategoryNumber1] int,[CptRangeFrom] [nvarchar](50) NULL,
	[CptRangeTo] [nvarchar](50) NULL,[DefaultTime] [nvarchar](50) NULL,[CorporateId] [int] NULL,[FacilityId] [int] NULL,
	[ExtValue1] [nvarchar](50) NULL,[CreatedBy] [int] NULL,	[IsActive] [bit] NOT NULL,
	[Name] [nvarchar](50) NOT NULL, [CPTRange] [nvarchar](50) NULL, [TimeSlot] [nvarchar](50), [EquipmentRequired][nvarchar](20),[ExtValue2] [nvarchar](50) NULL
	)


	Insert Into @AppointmentTempTable
	Select A.[Id], A.[Description], A.[CategoryNumber], CAST(A.CategoryNumber as int) CategoryNumber1, A.[CptRangeFrom], A.[CptRangeTo],
	 A.[DefaultTime], A.[CorporateId],A.[FacilityId], A.[ExtValue1], A.[CreatedBy], A.[IsActive], A.[Name],
	 (ISNULL([CptRangeFrom],'') +'-'+ISNULL(A.[CptRangeTo],'')) AS CPTRange, ISNULL(A.DefaultTime,'0') AS TimeSlot,
	 (Case when Cast(ExtValue1 as int) =1 Then 'True' else 'False' END) as  EquipmentRequired,
	 R.RoleName as ExtValue2
	 --A.[ExtValue2]

	from AppointmentTypes A 
	--inner join GlobalCodes GC on A.DefaultTime=GC.GlobalCodeValue and GC.GlobalCodeCategoryValue=4904
	LEFT join [Role] R on A.ExtValue2 =R.RoleID
	where A.FacilityId=@FacilityId and A.CorporateId=@CorporateId and A.IsActive=@ShowInActive

	SELECT * From @AppointmentTempTable Order by Cast([CategoryNumber] as int)
END