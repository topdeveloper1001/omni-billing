$(function () {
    $("#DrugAllergyLogFormDiv").validationEngine();
});

function SaveDrugAllergyLog(id) {
    var isValid = jQuery("#DrugAllergyLogFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtId = $("#txtId").val();
        var txtOrderCode = $("#txtOrderCode").val();
        var txtOrderCategory = $("#txtOrderCategory").val();
        var txtOrderSubCategory = $("#txtOrderSubCategory").val();
        var txtOrderType = $("#txtOrderType").val();
        var txtOrderName = $("#txtOrderName").val();
        var txtAllergyType = $("#txtAllergyType").val();
        var txtReactionType = $("#txtReactionType").val();
        var txtPatientId = $("#txtPatientId").val();
        var txtEncounterId = $("#txtEncounterId").val();
        var txtReactionOrderCode = $("#txtReactionOrderCode").val();
        var txtAllergyFromName = $("#txtAllergyFromName").val();
        var txtIsOrderCancel = $("#txtIsOrderCancel").val();
        var txtOrderBy = $("#txtOrderBy").val();
        var dtOrderedDate = $("#dtOrderedDate").val();
        var txtCreatedBy = $("#txtCreatedBy").val();
        var dtCreatedDate = $("#dtCreatedDate").val();
        var jsonData = JSON.stringify({
            Id: txtId,
            OrderCode: txtOrderCode,
            OrderCategory: txtOrderCategory,
            OrderSubCategory: txtOrderSubCategory,
            OrderType: txtOrderType,
            OrderName: txtOrderName,
            AllergyType: txtAllergyType,
            ReactionType: txtReactionType,
            PatientId: txtPatientId,
            EncounterId: txtEncounterId,
            ReactionOrderCode: txtReactionOrderCode,
            AllergyFromName: txtAllergyFromName,
            IsOrderCancel: txtIsOrderCancel,
            OrderBy: txtOrderBy,
            OrderedDate: dtOrderedDate,
            CreatedBy: txtCreatedBy,
            CreatedDate: dtCreatedDate,
            //DrugAllergyLogId: id,
            //DrugAllergyLogMainPhone: txtDrugAllergyLogMainPhone,
            //DrugAllergyLogFax: txtDrugAllergyLogFax,
            //DrugAllergyLogLicenseNumberExpire: dtDrugAllergyLogLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - DrugAllergyLog
        });
        $.ajax({
            type: "POST",
            url: '/DrugAllergyLog/SaveDrugAllergyLog',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function(data) {
                BindList("#DrugAllergyLogListDiv", data);
                ClearDrugAllergyLogForm();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";
                ShowMessage(msg, "Success", "success", true);
            },
            error: function(msg) {

            }
        });
    }
}

function EditDrugAllergyLog(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/DrugAllergyLog/GetDrugAllergyLog',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            BindDrugAllergyLogDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteDrugAllergyLog() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/DrugAllergyLog/DeleteDrugAllergyLog',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#DrugAllergyLogListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

//function DeleteDrugAllergyLog(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var jsonData = JSON.stringify({
//            id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/DrugAllergyLog/DeleteDrugAllergyLog',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                BindList("#DrugAllergyLogListDiv", data);
//                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function ClearDrugAllergyLogForm() {
    $("#DrugAllergyLogFormDiv").clearForm(true);
    $('#collapseDrugAllergyLogAddEdit').removeClass('in');
    $('#collapseDrugAllergyLogList').addClass('in');
    $("#DrugAllergyLogFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
}

function BindDrugAllergyLogDetails(data) {

    $("#btnSave").val("Update");
    $('#collapseDrugAllergyLogList').removeClass('in');
    $('#collapseDrugAllergyLogAddEdit').addClass('in');
    $("#DrugAllergyLogFormDiv").validationEngine();
}




