--Is Generic Column
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'IsGeneric'
          AND Object_ID = Object_ID(N'dbo.Role'))
Begin
	ALTER TABLE [Role]
	ADD IsGeneric bit Default(1)
End

--Role Key Column 
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'RoleKey'
          AND Object_ID = Object_ID(N'dbo.Role'))
Begin
	ALTER TABLE [Role]
	ADD RoleKey nvarchar(10) Default('')
End
