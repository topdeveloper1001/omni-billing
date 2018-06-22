Select * From XEncounter Order by 1 Desc
Select * From XFileXML Order by 1 Desc
Select * From XActivity Where ClaimID=1906 Order by 1 Desc
--Select * From BillHeader Where BillHeaderID=1906 Order by ModifiedDate Desc

Declare @FileId int =89
--Delete From XClaim Where XClaimID = 160
--Delete From XEncounter Where XEncounterID = 160
--Delete From XActivity Where ClaimID = 1906
--Update BillHeader Set [Status]=55 Where BillHeaderId=1906
--Delete From XFileHeader Where FileID = 90
--Delete From XFileXML Where FileXMLID = 90


Select ExternalValue1,* From GlobalCodes Where GlobalCodeCategoryValue = '14700'


Exec SendEClaimByPayerIDs 8,'','4',1906

