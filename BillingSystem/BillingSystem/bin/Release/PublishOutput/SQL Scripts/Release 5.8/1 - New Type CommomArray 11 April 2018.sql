IF TYPE_ID(N'ValuesArrayT') IS NOT NULL
	DROP TYPE ValuesArrayT
GO
-- Create the data type
CREATE TYPE ValuesArrayT AS TABLE 
(
Id int,
Value1 nvarchar(10),			--GLobalCodeCategoryValue
Value2 nvarchar(10),			--GlobalCodeValue
Value3 nvarchar(100),
Value4 nvarchar(200),
Value5 nvarchar(500),
Value6 nvarchar(200),
Value7 nvarchar(10)
)
GO
