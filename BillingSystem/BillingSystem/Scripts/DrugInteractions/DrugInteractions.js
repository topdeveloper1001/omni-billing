$(function () {
    $("#DrugInteractionsFormDiv").validationEngine();
    BindGlobalCodesWithValue("#ddlReactionCategory", 4999, "#hdReactionCategory");
    BindGlobalCodesWithValue("#ddlOrderType", 1201, "");
});

function SaveDrugInteractions() {
    var isValid = jQuery("#DrugInteractionsFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var id = $('#hdId').val();
        var txtGreenrainCode = $("#txtGreenrainCode").val();
        var txtATCCode = $("#txtATCCode").val();
        var txtPackageName = $("#txtPackageName").val();
        var txtGenericName = $("#txtGenericName").val();
        var txtReactionCategory = $("#ddlReactionCategory").val();
        var txtCPTCode = $("#txtCPTCode").val();
        var txtOrderName = $("#txtOrderName").val();
        var txtWarning = $("#txtWarning").val();
        var txtIsActive = $("#chkStatus").prop('checked');
        var jsonData = JSON.stringify({
            Id: id,
            GreenrainCode: txtGreenrainCode,
            ATCCode: txtATCCode,
            PackageName: txtPackageName,
            GenericName: txtGenericName,
            ReactionCategory: txtReactionCategory,
            OrderCode: txtCPTCode,
            OrderName: txtOrderName,
            Warning: txtWarning,
            IsActive: txtIsActive,
            OrderCodeType: $("#ddlOrderType").val()
        });
        $.ajax({
            type: "POST",
            url: '/DrugInteractions/SaveDrugInteractions',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#DrugInteractionsListDiv", data);
                ClearDrugInteractionsForm();
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

function EditDrugInteractions(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/DrugInteractions/GetDrugInteractionsDetails',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindDrugInteractionsDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteDrugInteractions() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/DrugInteractions/DeleteDrugInteractions',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#DrugInteractionsListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

//function DeleteDrugInteractions(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var jsonData = JSON.stringify({
//            id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/DrugInteractions/DeleteDrugInteractions',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                BindList("#DrugInteractionsListDiv", data);
//                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function ClearDrugInteractionsForm() {
    $("#DrugInteractionsFormDiv").clearForm(true);
    $('#collapseDrugInteractionsAddEdit').removeClass('in');
    $('#collapseDrugInteractionsList').addClass('in');
    $("#DrugInteractionsFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
    $("#chkStatus").prop('checked', 'checked');
}

function BindDrugInteractionsDetails(data) {
    $('#hdId').val(data.Id);
    $("#txtGreenrainCode").val(data.GreenrainCode);
    $("#txtATCCode").val(data.ATCCode);
    $("#txtPackageName").val(data.PackageName);
    $("#txtGenericName").val(data.GenericName);
    $("#ddlReactionCategory").val(data.ReactionCategory);
    $("#txtCPTCode").val(data.OrderCode);
    $("#txtOrderName").val(data.OrderName);
    $("#ddlOrderType").val(data.OrderCodeType);
    $("#txtWarning").val(data.Warning);
    $("#chkStatus").prop('checked', data.IsActive);
    $("#btnSave").val("Update");
    $('#collapseDrugInteractionsList').removeClass('in');
    $('#collapseDrugInteractionsAddEdit').addClass('in');
    $("#DrugInteractionsFormDiv").validationEngine();
}


function SortDrugIntractions(event) {
    var url = "/DrugInteractions/SortDrugInstructionlist";
  if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#DrugInteractionsListDiv").empty();
            $("#DrugInteractionsListDiv").html(data);

        },
        error: function (msg) {
        }
    });
}