IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'AssociatedFacilities'
          AND Object_ID = Object_ID(N'dbo.Physician'))
BEGIN
    ALTER TABLE Physician
	ADD AssociatedFacilities nvarchar(MAX) DEFAULT('')
END


IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'IsSchedulingPublic'
          AND Object_ID = Object_ID(N'dbo.Physician'))
BEGIN
    ALTER TABLE Physician
	ADD IsSchedulingPublic bit NOT NULL Default(0)
END