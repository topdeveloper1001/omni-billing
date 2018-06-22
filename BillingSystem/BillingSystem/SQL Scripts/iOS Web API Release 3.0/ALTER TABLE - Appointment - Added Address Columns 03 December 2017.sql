IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'CountryId'
          AND Object_ID = Object_ID(N'dbo.Appointment'))
Begin
	ALTER TABLE Appointment 
	ADD CountryId bigint,
	[StateId] bigint,
	[CityId] bigint 
End