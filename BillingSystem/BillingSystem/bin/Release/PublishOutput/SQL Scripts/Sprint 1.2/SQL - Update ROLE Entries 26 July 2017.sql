--Delete Duplicate Roles
Declare @RoleTemp Table (RoleId int)
;WITH cte AS (
  SELECT *, 
     row_number() OVER(PARTITION BY RoleName, CorporateId,FacilityId,IsActive,IsDeleted ORDER BY RoleId desc) AS [rn]
  FROM [Role]
)

INSERT INTO @RoleTemp
Select RoleID From cte where rn > 1

Delete From RoleTabs Where RoleID IN (Select RoleId From @RoleTemp)
Delete From FacilityRole Where RoleId IN (Select RoleId From @RoleTemp)
Delete From UserRole Where RoleId IN (Select RoleId From @RoleTemp)
Delete From [Role] Where RoleID IN (Select RoleId From @RoleTemp)


PRINT 'Duplicate Roles Deleted Successfully :)'

--Is Generic Column
Update [Role] Set IsGeneric=1
Update [Role] Set IsGeneric=0 Where RoleID IN (218,329,644,240,284,351,373,621,666,688)
Update [Role] Set IsGeneric=0,IsActive=0,IsDeleted=1 Where RoleName = 'ModelAdmin'

ALTER TABLE [Role]
ALTER COLUMN IsGeneric bit NOT NULL

PRINT 'IsGeneric Column Added and Updated Successfully'




--Role Key Column 
ALTER TABLE [Role]
ALTER COLUMN RoleKey nvarchar(10)

--Set Role Keys
Declare @RoleKey int=0
Update [Role] Set @RoleKey=@RoleKey+1, RoleKey = Cast(@RoleKey as nvarchar)  Where CorporateId=6 ANd IsGeneric=1

PRINT 'RoleKey Column Added and Updated Successfully'


--Update with Role Key in Generic Roles
;With URoles (RoleName,RoleKey)
As
(
Select RoleName,RoleKey From [Role] Where CorporateId=6 And IsGeneric=1 And IsActive=1 And IsDeleted=0
)
Update [Role] Set RoleKey = R2.RoleKey From [Role] R1 INNER JOIN URoles R2 ON R1.RoleName = R2.RoleName And R1.IsActive=1 And R1.IsDeleted=0 And R1.IsGeneric=1

PRINT 'RoleKey Updated successfully in Other Corporates and Entities'

--Update the URL Values of Scheduler
Update Tabs Set Controller='scheduler', [Action]='index' Where TabName='Scheduler'

--To Check if Duplicate Role still Exists in Database, It should not return any rows
--Select RoleName From [Role] Where IsActive=1 And IsDeleted=0 Group by RoleName,CorporateId,FacilityId Having Count(1) > 1