IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetPhysicianFavoriteOrders')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetPhysicianFavoriteOrders
GO
/****** Object:  StoredProcedure [dbo].[SPROC_GetPhysicianFavoriteOrders]    Script Date: 3/28/2018 9:54:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPROC_GetPhysicianFavoriteOrders]
(  
@pPhysicianID int =13,--- PhysicianID for whom the Order need to be processed 
@pCorporateID int=0,
@pFacilityID int  =0
)
AS
BEGIN
    
--DECLARE @OpenOrderCustom TABLE
--(
--TempOpenOrderID INT IDENTITY(1,1)
--,UserDefinedDescriptionID int
--,OrderTypeName varchar(20)
--,OrderCode varchar(20)
--,OrderType varchar(20)
--,OrderDescription varchar(500)
--,UserDefinedDescprition nvarchar(500)
--)
----- Declare Cursor Fetch Variables
--   DECLARE   @Cur_UserDefinedDescriptionID int, @Cur_OrderCode nvarchar(20),@Cur_UserID INT, @Cur_CategoryId INT,@Cur_SubCategoryId INT,@Cur_OrderType nvarchar(20),@CUR_UserDefineDescription nvarchar(1000)
			
--   DECLARE @CurOpenOrderID INT, @CurActivityCode nvarchar(20),@CurOrderDescription varchar(500)
--			-- Declare Cursor with Closed Order but not on any Bill
			
--   DECLARE PhyFavOrders CURSOR FOR SELECT Distinct (CodeId),CategoryId,UserID,UserDefineDescription,UserDefinedDescriptionID
--   FROM UserDefinedDescriptions WHERE UserID = @pPhysicianID and CategoryId in (3,4,5,8)

--   OPEN PhyFavOrders;
   
   
--   FETCH NEXT FROM PhyFavOrders INTO @Cur_OrderCode,@Cur_OrderType,@Cur_UserID,@CUR_UserDefineDescription,@Cur_UserDefinedDescriptionID;
					
		
--	WHILE @@FETCH_STATUS = 0  
--	BEGIN  

--	   Set @CurActivityCode = (Select top 1 (GlobalCodeName) from GlobalCodes Where GlobalCodeCategoryValue = 1201  and GlobalCodeValue = @Cur_OrderType);
--	   Set @CurOrderDescription = (Select [dbo].[GetOrderDescription](@Cur_OrderType,@Cur_OrderCode) OrderDescription)
	   
--		INSERT INTO @OpenOrderCustom
--		(UserDefinedDescriptionID,OrderTypeName,OrderCode,OrderType,OrderDescription,UserDefinedDescprition)
--        VALUES
--	    (@Cur_UserDefinedDescriptionID,@CurActivityCode,@Cur_OrderCode,@Cur_OrderType,@CurOrderDescription,@CUR_UserDefineDescription)
		 					  
		  
--	--- Fetch Next Record/Order from Cursor
	
--   FETCH NEXT FROM PhyFavOrders INTO @Cur_OrderCode,@Cur_OrderType,@Cur_UserID,@CUR_UserDefineDescription,@Cur_UserDefinedDescriptionID;
--	END  --END OF @@FETCH_STATUS = 0  

--	CLOSE PhyFavOrders;  
--	DEALLOCATE PhyFavOrders; 
--Select * from @OpenOrderCustom

	Select TempOpenOrderID=ROW_NUMBER() OVER(ORDER BY OO.OrderCode ASC),OpenOrderID=MAX(OO.OpenOrderID)
	,ActivityCode=(Select TOP 1 GC.GlobalCodeName From GlobalCodes GC Where GC.GlobalCodeValue = OO.OrderType and GC.GlobalCodeCategoryValue = '1201')
	,OrderTypeName=(Select TOP 1 GC.GlobalCodeName From GlobalCodes GC Where GC.GlobalCodeValue = OO.OrderType and GC.GlobalCodeCategoryValue = '1201')
	,OrderCode=OO.OrderCode
	,OO.OrderType
	,OrderDescription= (Case WHEN OO.OrderType='3' OR OO.OrderType='CPT' THEN 
									(Select TOP 1 CodeDescription from CPTCodes Where CodeNumbering = OO.OrderCode)
								WHEN OO.OrderType='4' OR OO.OrderType='HCPCS' THEN
									(Select TOP 1 CodeDescription from HCPCSCodes Where CodeNumbering = OO.OrderCode)
								WHEN OO.OrderType='5' OR OO.OrderType='DRUG' THEN 
									(Select TOP 1 DrugPackageName from Drug Where DrugCode = OO.OrderCode)
								WHEN OO.OrderType='8' OR OO.OrderType='ServiceCode' THEN 
									(Select TOP 1 ServiceCodeDescription from ServiceCode Where ServiceCodeValue = OO.OrderCode)
								WHEN OO.OrderType='9' OR OO.OrderType='DRG' THEN 
									(Select TOP 1 CodeDescription from DRGCodes Where CodeNumbering = OO.OrderCode)
								ELSE '' END)
	,UserDefineDescription=ISNULL(MAX(U.UserDefineDescription),'')
	,UserDefinedDescriptionID=MAX(U.UserDefinedDescriptionID)
	from [dbo].[OpenOrder] OO  
	INNER JOIN UserDefinedDescriptions U ON U.UserID = OO.PhysicianID AND OO.OrderCode=U.CodeId
	Where OO.PhysicianID = @pPhysicianID AND U.CategoryId !='16'
	Group by OO.PhysicianID,OO.OrderType,OO.OrderCode
	ORDER by OO.PhysicianID,OO.OrderType,OO.OrderCode
END












