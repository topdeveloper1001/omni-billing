
-- Create the data type
CREATE TYPE TypeDocumentTemplate AS TABLE 
(
	[DocumentsTemplatesID] [int],
	[DocumentTypeID] [int] NOT NULL,
	[DocumentName] [nvarchar](200) NULL,
	[DocumentNotes] [nvarchar](100) NULL,
	[AssociatedID] [int] NULL,
	[AssociatedType] [int] NOT NULL,
	[FileName] [nvarchar](100) NULL,
	[FilePath] [varchar](500) NULL,
	[IsTemplate] [bit] NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NULL,
	[DeletedBy] [int] NULL,
	[DeletedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[CorporateID] [int] NULL,
	[FacilityID] [int] NULL,
	[PatientID] [int] NULL,
	[EncounterID] [int] NULL,
	[ExternalValue1] [varchar](50) NULL,
	[ExternalValue2] [varchar](50) NULL,
	[ExternalValue3] [varchar](50) NULL
)
GO
