$(function () {
    $("#PatientInfoChangesQueueFormDiv").validationEngine();
});

function SavePatientInfoChangesQueue(id) {
    var isValid = jQuery("#PatientInfoChangesQueueFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
             var txtId = $("#txtId").val();
             var txtTableName = $("#txtTableName").val();
             var txtColumnName = $("#txtColumnName").val();
             var txtOldValue = $("#txtOldValue").val();
             var txtNewValue = $("#txtNewValue").val();
             var txtGlobalCodeValue = $("#txtGlobalCodeValue").val();
             var txtGlobalCodeCategory = $("#txtGlobalCodeCategory").val();
             var txtStatus = $("#txtStatus").val();
             var txtPatientId = $("#txtPatientId").val();
             var txtCorporatId = $("#txtCorporatId").val();
             var txtFacilityId = $("#txtFacilityId").val();
             var txtCreatedBy = $("#txtCreatedBy").val();
             var dtCreatedDate = $("#dtCreatedDate").val();
             var txtModifiedBy = $("#txtModifiedBy").val();
             var dtModifiedDate = $("#dtModifiedDate").val();
             var txtApprovedBy = $("#txtApprovedBy").val();
             var dtApprovedDate = $("#dtApprovedDate").val();
             var txtIsActive = $("#txtIsActive").val();
             var txtExternalValue1 = $("#txtExternalValue1").val();
             var txtExternalValue2 = $("#txtExternalValue2").val();
        var jsonData = JSON.stringify({
             Id: txtId
             TableName: txtTableName
             ColumnName: txtColumnName
             OldValue: txtOldValue
             NewValue: txtNewValue
             GlobalCodeValue: txtGlobalCodeValue
             GlobalCodeCategory: txtGlobalCodeCategory
             Status: txtStatus
             PatientId: txtPatientId
             CorporatId: txtCorporatId
             FacilityId: txtFacilityId
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             ApprovedBy: txtApprovedBy
             ApprovedDate: dtApprovedDate
             IsActive: txtIsActive
             ExternalValue1: txtExternalValue1
             ExternalValue2: txtExternalValue2
            //PatientInfoChangesQueueId: id,
            //PatientInfoChangesQueueMainPhone: txtPatientInfoChangesQueueMainPhone,
            //PatientInfoChangesQueueFax: txtPatientInfoChangesQueueFax,
            //PatientInfoChangesQueueLicenseNumberExpire: dtPatientInfoChangesQueueLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - PatientInfoChangesQueue
        });
        $.ajax({
            type: "POST",
            url: '/PatientInfoChangesQueue/SavePatientInfoChangesQueue',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                BindList("#PatientInfoChangesQueueListDiv", data);
                ClearPatientInfoChangesQueueForm();
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

function EditPatientInfoChangesQueue(id) {
    var jsonData = JSON.stringify({
             Id: txtId
             TableName: txtTableName
             ColumnName: txtColumnName
             OldValue: txtOldValue
             NewValue: txtNewValue
             GlobalCodeValue: txtGlobalCodeValue
             GlobalCodeCategory: txtGlobalCodeCategory
             Status: txtStatus
             PatientId: txtPatientId
             CorporatId: txtCorporatId
             FacilityId: txtFacilityId
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             ApprovedBy: txtApprovedBy
             ApprovedDate: dtApprovedDate
             IsActive: txtIsActive
             ExternalValue1: txtExternalValue1
             ExternalValue2: txtExternalValue2
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/PatientInfoChangesQueue/GetPatientInfoChangesQueue',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            BindPatientInfoChangesQueueDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeletePatientInfoChangesQueue(id) {
    if (confirm("Do you want to delete this record? ")) {
        var jsonData = JSON.stringify({
             Id: txtId
             TableName: txtTableName
             ColumnName: txtColumnName
             OldValue: txtOldValue
             NewValue: txtNewValue
             GlobalCodeValue: txtGlobalCodeValue
             GlobalCodeCategory: txtGlobalCodeCategory
             Status: txtStatus
             PatientId: txtPatientId
             CorporatId: txtCorporatId
             FacilityId: txtFacilityId
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             ApprovedBy: txtApprovedBy
             ApprovedDate: dtApprovedDate
             IsActive: txtIsActive
             ExternalValue1: txtExternalValue1
             ExternalValue2: txtExternalValue2
            id: id
        });
        $.ajax({
            type: "POST",
            url: '/PatientInfoChangesQueue/DeletePatientInfoChangesQueue',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#PatientInfoChangesQueueListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function ClearPatientInfoChangesQueueForm() {
    $("#PatientInfoChangesQueueFormDiv").clearForm(true);
    $('#collapsePatientInfoChangesQueueAddEdit').removeClass('in');
    $('#collapsePatientInfoChangesQueueList').addClass('in');
    $("#PatientInfoChangesQueueFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
}

function BindPatientInfoChangesQueueDetails(data) {

    $("#btnSave").val("Update");
    $('#collapsePatientInfoChangesQueueList').removeClass('in');
    $('#collapsePatientInfoChangesQueueAddEdit').addClass('in');
    $("#PatientInfoChangesQueueFormDiv").validationEngine();
}




