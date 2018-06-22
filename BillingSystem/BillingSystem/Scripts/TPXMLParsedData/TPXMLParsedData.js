$(function () {
    $("#TPXMLParsedDataFormDiv").validationEngine();
});

function SaveTPXMLParsedData(id) {
    var isValid = jQuery("#TPXMLParsedDataFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
             var txtTPXMLParsedDataID = $("#txtTPXMLParsedDataID").val();
             var txtTPFileID = $("#txtTPFileID").val();
             var txtCClaimID = $("#txtCClaimID").val();
             var txtCMemberID = $("#txtCMemberID").val();
             var txtCPayerID = $("#txtCPayerID").val();
             var txtCProviderID = $("#txtCProviderID").val();
             var txtCEmiratesIDNumber = $("#txtCEmiratesIDNumber").val();
             var txtCGross = $("#txtCGross").val();
             var txtCPatientShare = $("#txtCPatientShare").val();
             var txtCNet = $("#txtCNet").val();
             var txtEFacilityID = $("#txtEFacilityID").val();
             var txtEType = $("#txtEType").val();
             var txtEPatientID = $("#txtEPatientID").val();
             var txtEligibilityIDPayer = $("#txtEligibilityIDPayer").val();
             var txtEStart = $("#txtEStart").val();
             var txtEEnd = $("#txtEEnd").val();
             var txtEStartType = $("#txtEStartType").val();
             var txtEEndType = $("#txtEEndType").val();
             var txtDType = $("#txtDType").val();
             var txtDCode = $("#txtDCode").val();
             var txtAStart = $("#txtAStart").val();
             var txtAType = $("#txtAType").val();
             var txtACode = $("#txtACode").val();
             var txtAQuantity = $("#txtAQuantity").val();
             var txtANet = $("#txtANet").val();
             var txtAOrderingClinician = $("#txtAOrderingClinician").val();
             var txtAExecutingClinician = $("#txtAExecutingClinician").val();
             var txtAPriorAuthorizationID = $("#txtAPriorAuthorizationID").val();
             var txtCNPackageName = $("#txtCNPackageName").val();
             var txtModifiedBy = $("#txtModifiedBy").val();
             var dtModifiedDate = $("#dtModifiedDate").val();
             var txtPStatus = $("#txtPStatus").val();
             var txtOMCorporateID = $("#txtOMCorporateID").val();
             var txtOMFacilityID = $("#txtOMFacilityID").val();
             var txtOMPatientID = $("#txtOMPatientID").val();
             var txtOMEncounterID = $("#txtOMEncounterID").val();
             var txtOMBillID = $("#txtOMBillID").val();
             var txtOMInsuranceID = $("#txtOMInsuranceID").val();
             var txtOMOrderingClincialID = $("#txtOMOrderingClincialID").val();
             var txtOMExecutingClincialID = $("#txtOMExecutingClincialID").val();
        var jsonData = JSON.stringify({
             TPXMLParsedDataID: txtTPXMLParsedDataID
             TPFileID: txtTPFileID
             CClaimID: txtCClaimID
             CMemberID: txtCMemberID
             CPayerID: txtCPayerID
             CProviderID: txtCProviderID
             CEmiratesIDNumber: txtCEmiratesIDNumber
             CGross: txtCGross
             CPatientShare: txtCPatientShare
             CNet: txtCNet
             EFacilityID: txtEFacilityID
             EType: txtEType
             EPatientID: txtEPatientID
             EligibilityIDPayer: txtEligibilityIDPayer
             EStart: txtEStart
             EEnd: txtEEnd
             EStartType: txtEStartType
             EEndType: txtEEndType
             DType: txtDType
             DCode: txtDCode
             AStart: txtAStart
             AType: txtAType
             ACode: txtACode
             AQuantity: txtAQuantity
             ANet: txtANet
             AOrderingClinician: txtAOrderingClinician
             AExecutingClinician: txtAExecutingClinician
             APriorAuthorizationID: txtAPriorAuthorizationID
             CNPackageName: txtCNPackageName
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             PStatus: txtPStatus
             OMCorporateID: txtOMCorporateID
             OMFacilityID: txtOMFacilityID
             OMPatientID: txtOMPatientID
             OMEncounterID: txtOMEncounterID
             OMBillID: txtOMBillID
             OMInsuranceID: txtOMInsuranceID
             OMOrderingClincialID: txtOMOrderingClincialID
             OMExecutingClincialID: txtOMExecutingClincialID
            //TPXMLParsedDataId: id,
            //TPXMLParsedDataMainPhone: txtTPXMLParsedDataMainPhone,
            //TPXMLParsedDataFax: txtTPXMLParsedDataFax,
            //TPXMLParsedDataLicenseNumberExpire: dtTPXMLParsedDataLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - TPXMLParsedData
        });
        $.ajax({
            type: "POST",
            url: '/TPXMLParsedData/SaveTPXMLParsedData',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#TPXMLParsedDataListDiv", data);
                ClearTPXMLParsedDataForm();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";
                ShowMessage(msg, "Success", "success", true);
            },
            error: function (msg) {

            }
        });
    }
}

function EditTPXMLParsedData(id) {
    var jsonData = JSON.stringify({
             TPXMLParsedDataID: txtTPXMLParsedDataID
             TPFileID: txtTPFileID
             CClaimID: txtCClaimID
             CMemberID: txtCMemberID
             CPayerID: txtCPayerID
             CProviderID: txtCProviderID
             CEmiratesIDNumber: txtCEmiratesIDNumber
             CGross: txtCGross
             CPatientShare: txtCPatientShare
             CNet: txtCNet
             EFacilityID: txtEFacilityID
             EType: txtEType
             EPatientID: txtEPatientID
             EligibilityIDPayer: txtEligibilityIDPayer
             EStart: txtEStart
             EEnd: txtEEnd
             EStartType: txtEStartType
             EEndType: txtEEndType
             DType: txtDType
             DCode: txtDCode
             AStart: txtAStart
             AType: txtAType
             ACode: txtACode
             AQuantity: txtAQuantity
             ANet: txtANet
             AOrderingClinician: txtAOrderingClinician
             AExecutingClinician: txtAExecutingClinician
             APriorAuthorizationID: txtAPriorAuthorizationID
             CNPackageName: txtCNPackageName
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             PStatus: txtPStatus
             OMCorporateID: txtOMCorporateID
             OMFacilityID: txtOMFacilityID
             OMPatientID: txtOMPatientID
             OMEncounterID: txtOMEncounterID
             OMBillID: txtOMBillID
             OMInsuranceID: txtOMInsuranceID
             OMOrderingClincialID: txtOMOrderingClincialID
             OMExecutingClincialID: txtOMExecutingClincialID
        Id: id,
    });
    $.ajax({
        type: "POST",
        url: '/TPXMLParsedData/GetTPXMLParsedData',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindTPXMLParsedDataDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteTPXMLParsedData(id) {
    if (confirm("Do you want to delete this record? ")) {
        var jsonData = JSON.stringify({
             TPXMLParsedDataID: txtTPXMLParsedDataID
             TPFileID: txtTPFileID
             CClaimID: txtCClaimID
             CMemberID: txtCMemberID
             CPayerID: txtCPayerID
             CProviderID: txtCProviderID
             CEmiratesIDNumber: txtCEmiratesIDNumber
             CGross: txtCGross
             CPatientShare: txtCPatientShare
             CNet: txtCNet
             EFacilityID: txtEFacilityID
             EType: txtEType
             EPatientID: txtEPatientID
             EligibilityIDPayer: txtEligibilityIDPayer
             EStart: txtEStart
             EEnd: txtEEnd
             EStartType: txtEStartType
             EEndType: txtEEndType
             DType: txtDType
             DCode: txtDCode
             AStart: txtAStart
             AType: txtAType
             ACode: txtACode
             AQuantity: txtAQuantity
             ANet: txtANet
             AOrderingClinician: txtAOrderingClinician
             AExecutingClinician: txtAExecutingClinician
             APriorAuthorizationID: txtAPriorAuthorizationID
             CNPackageName: txtCNPackageName
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             PStatus: txtPStatus
             OMCorporateID: txtOMCorporateID
             OMFacilityID: txtOMFacilityID
             OMPatientID: txtOMPatientID
             OMEncounterID: txtOMEncounterID
             OMBillID: txtOMBillID
             OMInsuranceID: txtOMInsuranceID
             OMOrderingClincialID: txtOMOrderingClincialID
             OMExecutingClincialID: txtOMExecutingClincialID
            id: id,
        });
        $.ajax({
            type: "POST",
            url: '/TPXMLParsedData/DeleteTPXMLParsedData',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#TPXMLParsedDataListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function ClearTPXMLParsedDataForm() {
    $("#TPXMLParsedDataFormDiv").clearForm(true);
    $('#collapseTPXMLParsedDataAddEdit').removeClass('in');
    $('#collapseTPXMLParsedDataList').addClass('in');
    $("#TPXMLParsedDataFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
}

function BindTPXMLParsedDataDetails(data) {

    $("#btnSave").val("Update");
    $('#collapseTPXMLParsedDataList').removeClass('in');
    $('#collapseTPXMLParsedDataAddEdit').addClass('in');
    $("#TPXMLParsedDataFormDiv").validationEngine();
}




