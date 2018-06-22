IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
           WHERE TABLE_NAME = N'ClinicianAppointmentType')
/****** Object:  Table [dbo].[ClinicianAppointmentType]    Script Date: 7/5/2017 8:00:47 PM ******/
DROP TABLE [dbo].[ClinicianAppointmentType]
GO

/****** Object:  Table [dbo].[ClinicianAppointmentType]    Script Date: 7/5/2017 8:00:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClinicianAppointmentType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ClinicianId] [bigint] NOT NULL,
	[AppointmentTypeId] [bigint] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ClinicianAppointmentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ClinicianAppointmentType] ADD  CONSTRAINT [DF_ClinicianAppointmentType_CreatedBy]  DEFAULT ((1)) FOR [CreatedBy]
GO

ALTER TABLE [dbo].[ClinicianAppointmentType] ADD  CONSTRAINT [DF_ClinicianAppointmentType_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[ClinicianAppointmentType] ADD  CONSTRAINT [DF_ClinicianAppointmentType_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO


