Select * From ATCCodes

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [name] = N'CodeEffectiveFrom'
          AND Object_ID = Object_ID(N'dbo.ATCCodes'))
Begin
	ALTER TABLE ATCCodes
	ADD CodeEffectiveFrom datetime NULL,
	CodeEffectiveTill datetime NULL,
	IsActive bit NOT NULL Default(1),
	ModifiedBy bigint Default(0),
	ModifiedDate datetime NULL
End


IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [name] = N'CodeTableNumber'
          AND Object_ID = Object_ID(N'dbo.ATCCodes'))
Begin
	ALTER TABLE ATCCodes
	ADD CodeTableNumber nvarchar(50)
End


IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [name] = N'CreatedBy'
          AND Object_ID = Object_ID(N'dbo.ATCCodes'))
Begin
	ALTER TABLE ATCCodes
	ADD CreatedBy bigint NOT NULL Default(1),
	CreatedDate datetime NOT NULL Default(GETDATE())
End
