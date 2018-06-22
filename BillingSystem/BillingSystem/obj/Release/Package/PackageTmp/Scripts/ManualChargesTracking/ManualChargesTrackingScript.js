$(function () {
    $("#ManualChargesTrackingFormDiv").validationEngine();
});

function SaveManualChargesTracking(id) {
    var isValid = jQuery("#ManualChargesTrackingFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
             var txtManualChargesTrackingID = $("#txtManualChargesTrackingID").val();
             var txtTrackingType = $("#txtTrackingType").val();
             var txtTrackingTypeNameVal = $("#txtTrackingTypeNameVal").val();
             var txtTrackingColumnName = $("#txtTrackingColumnName").val();
             var txtTrackingTableName = $("#txtTrackingTableName").val();
             var txtTrackingValue = $("#txtTrackingValue").val();
             var txtTrackingBillStatus = $("#txtTrackingBillStatus").val();
             var txtBillHeaderID = $("#txtBillHeaderID").val();
             var txtEncounterID = $("#txtEncounterID").val();
             var txtPatientID = $("#txtPatientID").val();
             var txtFacilityID = $("#txtFacilityID").val();
             var txtCorporateID = $("#txtCorporateID").val();
             var txtCreatedBy = $("#txtCreatedBy").val();
             var dtCreatedDate = $("#dtCreatedDate").val();
             var txtModifiedBy = $("#txtModifiedBy").val();
             var dtModifiedDate = $("#dtModifiedDate").val();
             var txtDeletedBy = $("#txtDeletedBy").val();
             var dtDeletedDate = $("#dtDeletedDate").val();
             var txtExternalValue1 = $("#txtExternalValue1").val();
             var txtExternalValue2 = $("#txtExternalValue2").val();
             var txtExternalValue3 = $("#txtExternalValue3").val();
             var txtExternalValue4 = $("#txtExternalValue4").val();
             var txtExternalValue5 = $("#txtExternalValue5").val();
             var txtExternalValue6 = $("#txtExternalValue6").val();
             var txtIsVisible = $("#txtIsVisible").val();
        var jsonData = JSON.stringify({
             ManualChargesTrackingID: txtManualChargesTrackingID
             TrackingType: txtTrackingType
             TrackingTypeNameVal: txtTrackingTypeNameVal
             TrackingColumnName: txtTrackingColumnName
             TrackingTableName: txtTrackingTableName
             TrackingValue: txtTrackingValue
             TrackingBillStatus: txtTrackingBillStatus
             BillHeaderID: txtBillHeaderID
             EncounterID: txtEncounterID
             PatientID: txtPatientID
             FacilityID: txtFacilityID
             CorporateID: txtCorporateID
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             DeletedBy: txtDeletedBy
             DeletedDate: dtDeletedDate
             ExternalValue1: txtExternalValue1
             ExternalValue2: txtExternalValue2
             ExternalValue3: txtExternalValue3
             ExternalValue4: txtExternalValue4
             ExternalValue5: txtExternalValue5
             ExternalValue6: txtExternalValue6
             IsVisible: txtIsVisible
            //ManualChargesTrackingId: id,
            //ManualChargesTrackingMainPhone: txtManualChargesTrackingMainPhone,
            //ManualChargesTrackingFax: txtManualChargesTrackingFax,
            //ManualChargesTrackingLicenseNumberExpire: dtManualChargesTrackingLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - ManualChargesTracking
        });
        $.ajax({
            type: "POST",
            url: '/ManualChargesTracking/SaveManualChargesTracking',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                BindList("#ManualChargesTrackingListDiv", data);
                ClearManualChargesTrackingForm();
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

function EditManualChargesTracking(id) {
    var jsonData = JSON.stringify({
             ManualChargesTrackingID: txtManualChargesTrackingID
             TrackingType: txtTrackingType
             TrackingTypeNameVal: txtTrackingTypeNameVal
             TrackingColumnName: txtTrackingColumnName
             TrackingTableName: txtTrackingTableName
             TrackingValue: txtTrackingValue
             TrackingBillStatus: txtTrackingBillStatus
             BillHeaderID: txtBillHeaderID
             EncounterID: txtEncounterID
             PatientID: txtPatientID
             FacilityID: txtFacilityID
             CorporateID: txtCorporateID
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             DeletedBy: txtDeletedBy
             DeletedDate: dtDeletedDate
             ExternalValue1: txtExternalValue1
             ExternalValue2: txtExternalValue2
             ExternalValue3: txtExternalValue3
             ExternalValue4: txtExternalValue4
             ExternalValue5: txtExternalValue5
             ExternalValue6: txtExternalValue6
             IsVisible: txtIsVisible
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/ManualChargesTracking/GetManualChargesTracking',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            BindManualChargesTrackingDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteManualChargesTracking(id) {
    if (confirm("Do you want to delete this record? ")) {
        var jsonData = JSON.stringify({
             ManualChargesTrackingID: txtManualChargesTrackingID
             TrackingType: txtTrackingType
             TrackingTypeNameVal: txtTrackingTypeNameVal
             TrackingColumnName: txtTrackingColumnName
             TrackingTableName: txtTrackingTableName
             TrackingValue: txtTrackingValue
             TrackingBillStatus: txtTrackingBillStatus
             BillHeaderID: txtBillHeaderID
             EncounterID: txtEncounterID
             PatientID: txtPatientID
             FacilityID: txtFacilityID
             CorporateID: txtCorporateID
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             DeletedBy: txtDeletedBy
             DeletedDate: dtDeletedDate
             ExternalValue1: txtExternalValue1
             ExternalValue2: txtExternalValue2
             ExternalValue3: txtExternalValue3
             ExternalValue4: txtExternalValue4
             ExternalValue5: txtExternalValue5
             ExternalValue6: txtExternalValue6
             IsVisible: txtIsVisible
            id: id
        });
        $.ajax({
            type: "POST",
            url: '/ManualChargesTracking/DeleteManualChargesTracking',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#ManualChargesTrackingListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function ClearManualChargesTrackingForm() {
    $("#ManualChargesTrackingFormDiv").clearForm(true);
    $('#collapseManualChargesTrackingAddEdit').removeClass('in');
    $('#collapseManualChargesTrackingList').addClass('in');
    $("#ManualChargesTrackingFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
}

function BindManualChargesTrackingDetails(data) {

    $("#btnSave").val("Update");
    $('#collapseManualChargesTrackingList').removeClass('in');
    $('#collapseManualChargesTrackingAddEdit').addClass('in');
    $("#ManualChargesTrackingFormDiv").validationEngine();
}




