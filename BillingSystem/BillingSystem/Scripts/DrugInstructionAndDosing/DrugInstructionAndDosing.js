$(function () {
    $("#DrugInstructionAndDosingFormDiv").validationEngine();
});

function SaveDrugInstructionAndDosing() {
    var isValid = jQuery("#DrugInstructionAndDosingFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var id = $('#hdId').val();
        var txtGreenrainCode = $("#txtGreenrainCode").val();
        var txtATCCode = $("#txtATCCode").val();
        var txtPackageName = $("#txtPackageName").val();
        var txtGenericName = $("#txtGenericName").val();
        var txtAdminInstructions = $("#txtAdminInstructions").val();
        var txtRecommendedDosing = $("#txtRecommendedDosing").val();
        var txtIsActive = $("#chkStatus").prop('checked');
        var jsonData = JSON.stringify({
            Id: id,
            GreenrainCode: txtGreenrainCode,
            ATCCode: txtATCCode,
            PackageName: txtPackageName,
            GenericName: txtGenericName,
            AdminInstructions: txtAdminInstructions,
            RecommendedDosing: txtRecommendedDosing,
            IsActive: txtIsActive,
        });
        $.ajax({
            type: "POST",
            url: '/DrugInstructionAndDosing/SaveDrugInstructionAndDosing',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                $('#collapseDrugInstructionAndDosingList').addClass('in').attr('style', 'height:auto;');
                BindList("#DrugInstructionAndDosingListDiv", data);
                ClearDrugInstructionAndDosingForm();
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

function EditDrugInstructionAndDosing(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/DrugInstructionAndDosing/GetDrugInstructionAndDosingDetails',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindDrugInstructionAndDosingDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteDrugInstructionAndDosing() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/DrugInstructionAndDosing/DeleteDrugInstructionAndDosing',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#DrugInstructionAndDosingListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function ClearDrugInstructionAndDosingForm() {
    $("#DrugInstructionAndDosingFormDiv").clearForm(true);
    $('#collapseDrugInstructionAndDosingAddEdit').removeClass('in');
    $('#collapseDrugInstructionAndDosingList').addClass('in');
    $("#DrugInstructionAndDosingFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
    $("#chkStatus").prop('checked','checked');
}

function BindDrugInstructionAndDosingDetails(data) {
    $("#btnSave").val("Update");
    $('#hdId').val(data.Id);
    $('#txtGreenrainCode').val(data.GreenrainCode);
    $("#txtATCCode").val(data.ATCCode);
    $("#txtPackageName").val(data.PackageName);
    $("#txtGenericName").val(data.GenericName);
    $("#txtAdminInstructions").val(data.AdminInstructions);
    $("#txtRecommendedDosing").val(data.RecommendedDosing);
    $("#chkStatus").prop('checked', data.IsActive);
    $('#collapseDrugInstructionAndDosingList').removeClass('in');
    $('#collapseDrugInstructionAndDosingAddEdit').addClass('in');
    $("#DrugInstructionAndDosingFormDiv").validationEngine();
}


function SortDrugInstructionAndDosingGrid(event) {
    var url = "/DrugInstructionAndDosing/GetDrugInstructionAndDosingList";
    //var patientId = $("#hdPatientId").val();
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
            BindList("#DrugInstructionAndDosingListDiv", data);
        },
        error: function (msg) {
        }
    });
}