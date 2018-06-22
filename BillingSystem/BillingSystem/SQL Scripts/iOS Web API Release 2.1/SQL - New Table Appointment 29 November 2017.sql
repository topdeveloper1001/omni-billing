IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
           WHERE TABLE_NAME = N'Appointment')
	DROP TABLE Appointment


/****** Object:  Table [dbo].[Appointment]    Script Date: 11/29/2017 6:03:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Appointment](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[AppointmentDetails] [nvarchar](200) NULL,
	[AppointmentTypeId] [nvarchar](10) NOT NULL,
	[ClinicianId] [bigint] NOT NULL,
	[Specialty] [bigint] NOT NULL,
	[PatientId] [nchar](10) NULL,
	[ClinicianReferredBy] [bigint] NULL,
	[Status] [nvarchar](50) NOT NULL,
	[ScheduleDate] [date] NOT NULL,
	[TimeFrom] [nvarchar](7) NOT NULL,
	[TimeTill] [nvarchar](7) NOT NULL,
	[Comments] [nvarchar](100) NULL,
	[Address] [nvarchar](200) NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[DeletedBy] [bigint] NULL,
	[DeletedDate] [datetime] NULL,
	[IsRecurring] [bit] NOT NULL,
	[RecurringInterval] [nvarchar](20) NULL,
	[IsAddedToMain] bit NOT NULL DEFAULT(0)
 CONSTRAINT [PK_Appointment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Appointment] ADD  CONSTRAINT [DF_Appointment_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Appointment] ADD  CONSTRAINT [DF_Appointment_IsRecurring]  DEFAULT ((0)) FOR [IsRecurring]
GO


