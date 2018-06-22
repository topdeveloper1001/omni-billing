IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE [name] = N'Code'
          AND Object_ID = Object_ID(N'dbo.ATCCodes'))
Begin
	ALTER TABLE ATCCodes
	DROP Column Code
End

