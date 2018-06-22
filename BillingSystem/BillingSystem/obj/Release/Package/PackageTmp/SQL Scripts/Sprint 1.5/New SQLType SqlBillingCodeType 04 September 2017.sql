IF TYPE_ID(N'SqlBillingCodeType') IS NOT NULL 
	DROP TYPE SqlBillingCodeType

Go
-- ================================
-- Create User-defined Table Type
-- ================================

-- Create the data type
CREATE TYPE SqlBillingCodeType As Table
(
Id bigint,
TableNumber nvarchar(20),
Code nvarChar(500),
CodeDescription nvarChar(max),
Price nvarchar(100),
CodeGroup nvarchar(max),
OtherValue1 nvarchar(max),
OtherValue2 nvarchar(max),
OtherValue3 nvarchar(500),
OtherValue4 nvarchar(500),
OtherValue5 nvarchar(500),
OtherValue6 nvarchar(500),
EffectiveFrom datetime NULL,
EffectiveTill datetime NULL
)
GO
