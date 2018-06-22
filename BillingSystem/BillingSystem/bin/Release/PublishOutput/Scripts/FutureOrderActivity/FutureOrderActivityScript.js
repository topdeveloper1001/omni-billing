$(function () {
    $("#FutureOrderActivityFormDiv").validationEngine();
});

function SaveFutureOrderActivity(id) {
    var isValid = jQuery("#FutureOrderActivityFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
             var txtFutureOrderActivityID = $("#txtFutureOrderActivityID").val();
             var txtOrderType = $("#txtOrderType").val();
             var txtOrderCode = $("#txtOrderCode").val();
             var txtOrderCategoryID = $("#txtOrderCategoryID").val();
             var txtOrderSubCategoryID = $("#txtOrderSubCategoryID").val();
             var txtOrderActivityStatus = $("#txtOrderActivityStatus").val();
             var txtCorporateID = $("#txtCorporateID").val();
             var txtFacilityID = $("#txtFacilityID").val();
             var txtPatientID = $("#txtPatientID").val();
             var txtEncounterID = $("#txtEncounterID").val();
             var txtMedicalRecordNumber = $("#txtMedicalRecordNumber").val();
             var txtOrderID = $("#txtOrderID").val();
             var txtOrderBy = $("#txtOrderBy").val();
             var txtOrderActivityQuantity = $("#txtOrderActivityQuantity").val();
             var dtOrderScheduleDate = $("#dtOrderScheduleDate").val();
             var txtPlannedBy = $("#txtPlannedBy").val();
             var dtPlannedDate = $("#dtPlannedDate").val();
             var txtPlannedFor = $("#txtPlannedFor").val();
             var txtExecutedBy = $("#txtExecutedBy").val();
             var dtExecutedDate = $("#dtExecutedDate").val();
             var txtExecutedQuantity = $("#txtExecutedQuantity").val();
             var txtResultValueMin = $("#txtResultValueMin").val();
             var txtResultValueMax = $("#txtResultValueMax").val();
             var txtResultUOM = $("#txtResultUOM").val();
             var txtComments = $("#txtComments").val();
             var txtIsActive = $("#txtIsActive").val();
             var txtModifiedBy = $("#txtModifiedBy").val();
             var dtModifiedDate = $("#dtModifiedDate").val();
             var txtCreatedBy = $("#txtCreatedBy").val();
             var dtCreatedDate = $("#dtCreatedDate").val();
        var jsonData = JSON.stringify({
             FutureOrderActivityID: txtFutureOrderActivityID
             OrderType: txtOrderType
             OrderCode: txtOrderCode
             OrderCategoryID: txtOrderCategoryID
             OrderSubCategoryID: txtOrderSubCategoryID
             OrderActivityStatus: txtOrderActivityStatus
             CorporateID: txtCorporateID
             FacilityID: txtFacilityID
             PatientID: txtPatientID
             EncounterID: txtEncounterID
             MedicalRecordNumber: txtMedicalRecordNumber
             OrderID: txtOrderID
             OrderBy: txtOrderBy
             OrderActivityQuantity: txtOrderActivityQuantity
             OrderScheduleDate: dtOrderScheduleDate
             PlannedBy: txtPlannedBy
             PlannedDate: dtPlannedDate
             PlannedFor: txtPlannedFor
             ExecutedBy: txtExecutedBy
             ExecutedDate: dtExecutedDate
             ExecutedQuantity: txtExecutedQuantity
             ResultValueMin: txtResultValueMin
             ResultValueMax: txtResultValueMax
             ResultUOM: txtResultUOM
             Comments: txtComments
             IsActive: txtIsActive
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
            //FutureOrderActivityId: id,
            //FutureOrderActivityMainPhone: txtFutureOrderActivityMainPhone,
            //FutureOrderActivityFax: txtFutureOrderActivityFax,
            //FutureOrderActivityLicenseNumberExpire: dtFutureOrderActivityLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - FutureOrderActivity
        });
        $.ajax({
            type: "POST",
            url: '/FutureOrderActivity/SaveFutureOrderActivity',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearAll();
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

function EditFutureOrderActivity(id) {
    var jsonData = JSON.stringify({
             FutureOrderActivityID: txtFutureOrderActivityID
             OrderType: txtOrderType
             OrderCode: txtOrderCode
             OrderCategoryID: txtOrderCategoryID
             OrderSubCategoryID: txtOrderSubCategoryID
             OrderActivityStatus: txtOrderActivityStatus
             CorporateID: txtCorporateID
             FacilityID: txtFacilityID
             PatientID: txtPatientID
             EncounterID: txtEncounterID
             MedicalRecordNumber: txtMedicalRecordNumber
             OrderID: txtOrderID
             OrderBy: txtOrderBy
             OrderActivityQuantity: txtOrderActivityQuantity
             OrderScheduleDate: dtOrderScheduleDate
             PlannedBy: txtPlannedBy
             PlannedDate: dtPlannedDate
             PlannedFor: txtPlannedFor
             ExecutedBy: txtExecutedBy
             ExecutedDate: dtExecutedDate
             ExecutedQuantity: txtExecutedQuantity
             ResultValueMin: txtResultValueMin
             ResultValueMax: txtResultValueMax
             ResultUOM: txtResultUOM
             Comments: txtComments
             IsActive: txtIsActive
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/FutureOrderActivity/GetFutureOrderActivity',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $('#FutureOrderActivityFormDiv').empty();
            $('#FutureOrderActivityFormDiv').html(data);
            $('#collapseFutureOrderActivityAddEdit').addClass('in');
            $("#FutureOrderActivityFormDiv").validationEngine();
        },
        error: function (msg) {

        }
    });
}

function DeleteFutureOrderActivity(id) {
    if (confirm("Do you want to delete this record? ")) {
        var txtFutureOrderActivityId = id;
        var jsonData = JSON.stringify({
             FutureOrderActivityID: txtFutureOrderActivityID
             OrderType: txtOrderType
             OrderCode: txtOrderCode
             OrderCategoryID: txtOrderCategoryID
             OrderSubCategoryID: txtOrderSubCategoryID
             OrderActivityStatus: txtOrderActivityStatus
             CorporateID: txtCorporateID
             FacilityID: txtFacilityID
             PatientID: txtPatientID
             EncounterID: txtEncounterID
             MedicalRecordNumber: txtMedicalRecordNumber
             OrderID: txtOrderID
             OrderBy: txtOrderBy
             OrderActivityQuantity: txtOrderActivityQuantity
             OrderScheduleDate: dtOrderScheduleDate
             PlannedBy: txtPlannedBy
             PlannedDate: dtPlannedDate
             PlannedFor: txtPlannedFor
             ExecutedBy: txtExecutedBy
             ExecutedDate: dtExecutedDate
             ExecutedQuantity: txtExecutedQuantity
             ResultValueMin: txtResultValueMin
             ResultValueMax: txtResultValueMax
             ResultUOM: txtResultUOM
             Comments: txtComments
             IsActive: txtIsActive
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
            Id: id
        });
        $.ajax({
            type: "POST",
            url: '/FutureOrderActivity/DeleteFutureOrderActivity',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindFutureOrderActivityGrid();
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                }
                else {
                    return false;
                }
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function BindFutureOrderActivityGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/FutureOrderActivity/BindFutureOrderActivityList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#FutureOrderActivityListDiv").empty();
            $("#FutureOrderActivityListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearForm() {
    
}

function ClearAll() {
    $("#FutureOrderActivityFormDiv").clearForm();
    $('#collapseFutureOrderActivityAddEdit').removeClass('in');
    $('#collapseFutureOrderActivityList').addClass('in');
    $("#FutureOrderActivityFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/FutureOrderActivity/ResetFutureOrderActivityForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            if (data) {
                $('#FutureOrderActivityFormDiv').empty();
                $('#FutureOrderActivityFormDiv').html(data);
                $('#collapseFutureOrderActivityList').addClass('in');
                BindFutureOrderActivityGrid();
            }
            else {
                return false;
            }
        },
        error: function (msg) {


            return true;
        }
    });

}




