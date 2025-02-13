IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetPhysicianPreviousOrders')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetPhysicianPreviousOrders

GO
/****** Object:  StoredProcedure [dbo].[SPROC_GetPhysicianPreviousOrders]    Script Date: 3/15/2018 8:23:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPROC_GetPhysicianPreviousOrders]
(  
@pPhysicianID int,--- PhysicianID for whom the Order need to be processed 
@pCorporateID int,
@pFacilityID int  
)
AS
BEGIN
    
DECLARE @OpenOrderCustom TABLE
			(
			TempOpenOrderID INT IDENTITY(1,1)
			,OpenOrderID INT
			,ActivityCode varchar(20)
			,OrderCode varchar(20)
			,OrderType varchar(20)
			,OrderDescription varchar(500)
			)
			--- Declare Cursor Fetch Variables
		    DECLARE 
			--@Cur_OpenOrderID INT, 
		    @Cur_OrderCode nvarchar(20),
			@Cur_PhysicianID INT, 
			@Cur_CorporateID INT,
			@Cur_FacilityID INT, 
			@Cur_CategoryId INT,
			@Cur_SubCategoryId INT,
		    @Cur_OrderType nvarchar(20)
			
			DECLARE 
			@CurOpenOrderID INT, 
		    @CurActivityCode nvarchar(20),
			@CurOrderDescription varchar(500)
			-- Declare Cursor with Closed Order but not on any Bill
			
			DECLARE PhyAllOrders CURSOR FOR

			SELECT 
			Distinct (OrderCode),
            PhysicianID,
			CorporateID,
			FacilityID,
			CategoryId,
			SubCategoryId,
			OrderType
			FROM OpenOrder 
			WHERE PhysicianID = @pPhysicianID and FacilityID = @pFacilityID and CorporateID=@pCorporateID

				OPEN PhyAllOrders;  

				
				FETCH NEXT FROM PhyAllOrders INTO @Cur_OrderCode,@Cur_PhysicianID,@Cur_CorporateID,@Cur_FacilityID,@Cur_CategoryId,@Cur_SubCategoryId,
				@Cur_OrderType;
							
				
				WHILE @@FETCH_STATUS = 0  
				BEGIN  

                   Set @CurOpenOrderID =(Select top 1 (OpenOrderId) from OpenOrder WHERE PhysicianID = @Cur_PhysicianID and FacilityID = @Cur_FacilityID and CorporateID=@Cur_CorporateID and OrderCode =  @Cur_OrderCode);
				   Set @CurActivityCode = (Select top 1 (GlobalCodeName) from GlobalCodes Where GlobalCodeCategoryValue = 1201  and GlobalCodeValue = @Cur_OrderType);
					Set @CurOrderDescription = (Select [dbo].[GetOrderDescription](@Cur_OrderType,@Cur_OrderCode) OrderDescription)
				   --select @CurOpenOrderID as OpenOrderId
				   --select @CurActivityCode as ActivityCode
				   --select @Cur_OrderCode as OrderCode
				   --select @Cur_OrderType as OrderType

					INSERT INTO @OpenOrderCustom
					(OpenOrderID,
					ActivityCode,
					OrderCode,
					OrderType,
					OrderDescription)
                    VALUES
			           ( 
					     @CurOpenOrderID,
					     @CurActivityCode,
						 @Cur_OrderCode,
						 @Cur_OrderType,
						 @CurOrderDescription
						 )
					 					  
					  
				--- Fetch Next Record/Order from Cursor
				FETCH NEXT FROM PhyAllOrders INTO @Cur_OrderCode,@Cur_PhysicianID,@Cur_CorporateID,@Cur_FacilityID,@Cur_CategoryId,@Cur_SubCategoryId,
				@Cur_OrderType;
							
				END  --END OF @@FETCH_STATUS = 0  


			CLOSE PhyAllOrders;  
			DEALLOCATE PhyAllOrders; 
			Select * from @OpenOrderCustom Where ISNULL(OpenOrderID,0) !=0
END













