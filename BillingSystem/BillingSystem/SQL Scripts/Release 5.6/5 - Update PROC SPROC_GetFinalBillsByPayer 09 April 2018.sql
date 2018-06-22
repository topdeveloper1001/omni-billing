IF EXISTS ( SELECT *
            FROM   sysobjects
            WHERE  id = object_id(N'SPROC_GetFinalBillsByPayer')
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
         DROP PROCEDURE SPROC_GetFinalBillsByPayer
GO

/****** Object:  StoredProcedure [dbo].[SPROC_GetFinalBillsByPayer]    Script Date: 4/9/2018 12:36:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPROC_GetFinalBillsByPayer]
(
	@pFID int,
	@pCID int,
	@pPayerId nvarchar(50)
)
AS
BEGIN
	IF ISNULL(@pPayerId,'')=''
		SET @pPayerId='0'
		 
	Select BH.BillHeaderId,BH.FacilityID,BillNumber
	,EncounterNumber=ENC.EncounterNumber,InsuranceCompany=IC.InsuranceCompanyName
	,BH.Gross, GrossChargesSum=(BH.PatientShare + BH.PayerShareNet),BH.PayerShareNet,BH.PatientShare,BH.PayerID
	,[Status]=(Select TOP 1 G.GlobalCodeName From GlobalCodes G Where G.GlobalCodeCategoryValue='14700' AND G.GlobalCodeValue=BH.[Status])
	,EncounterPatientType=(Select TOP 1 G.GlobalCodeName From GlobalCodes G 
							Where G.GlobalCodeCategoryValue='1107' AND G.GlobalCodeValue=ENC.EncounterPatientType)
	,PatientName=(Pinfo.PersonFirstName+ ' ' +Pinfo.PersonLastName)
	from Billheader BH
	INNER JOIN InsuranceCompany IC on IC.InsuranceCompanyId = BH.PayerId
	INNER JOIN Encounter ENC on ENC.EncounterID = BH.EncounterID
	INNER JOIN PatientInfo Pinfo on Pinfo.PatientID = BH.PatientID
	WHERE [Status] in ('55','105','155')
	and  BH.FacilityID = @pFID and BH.CorporateID = @pCID
	and (ISNULL(@pPayerId,'0') = '0' or BH.PayerId IN (Select IDValue From dbo.Split(',',@pPayerId)))
	For Json Path, Root('Claims'),INCLUDE_NULL_VALUES
END
GO


