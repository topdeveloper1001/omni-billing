$(function () {
    $("#ProjectTaskTargetsFormDiv").validationEngine();

    BindTaskNumbers();
    GetProjectTaskTargetsList();
});
function GetProjectTaskTargetsList() {
    $.ajax({
        type: "POST",
        url: '/Projects/ProjectTaskTargetsList',
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            BindList("#ProjectTaskTargetsListDiv", data);
        },
        error: function (msg) {

        }
    });
}
function SaveProjectTaskTargets(id) {
    var isValid = jQuery("#ProjectTaskTargetsFormDiv").validationEngine({ returnIsValid: true });
    if ($("#ddlFacility").val() == "0" && isValid == true) {
        isValid = false;
        var msg = "Please select facility";
        ShowMessage(msg, "Warning", "warning", true);
        $("#ddlFacility").focus();
    }
    if (isValid == true) {
        var txtTaskNumber = $("#ddlTargetTaskNumbers").val();
        var dtTaskDate = $("#dtTargetTaskDate").val();
        var txtTargetedCompletionValue = $("#ddlPrTaskTargetedValue").val();
        var txtIsActive = $('#TaskTargetIsActive').prop("checked");//$("#TaskTargetIsActive")[0].checked;
        var jsonData = JSON.stringify({
            Id: $("#taskTargetId").val(),
            TaskNumber: txtTaskNumber,
            TaskDate: dtTaskDate,
            TargetedCompletionValue: txtTargetedCompletionValue,
            IsActive: txtIsActive,
            ExternalValue1: $("#ddlPrTaskTargetedValue option:selected").text().replace("%", "").trim(),
            FacilityId: $("#ddlFacility").val()
        });
        $.ajax({
            type: "POST",
            url: '/Projects/SaveProjectTaskTargets',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#ProjectTaskTargetsListDiv", data);
                var msg = "Records Saved successfully !";
                if ($("#taskTargetId").val() > 0)
                    msg = "Records updated successfully";
                ClearProjectTaskTargetsForm();
                ShowMessage(msg, "Success", "success", true);
            },
            error: function (msg) {

            }
        });
    }
}

function EditProjectTaskTargets(id) {
    var jsonData = JSON.stringify({
        id: id,
    });
    $.ajax({
        type: "POST",
        url: '/ProjectTaskTargets/GetProjectTaskTargetsDetails',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindProjectTaskTargetsDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteProjectTaskTargets() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val(),
        });
        $.ajax({
            type: "POST",
            url: '/Projects/DeleteProjectTaskTargets',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#ProjectTaskTargetsListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

//function DeleteProjectTaskTargets(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var jsonData = JSON.stringify({
//            id: id,
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Projects/DeleteProjectTaskTargets',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                BindList("#ProjectTaskTargetsListDiv", data);
//                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function ClearProjectTaskTargetsForm() {
    $("#ProjectTaskTargetsFormDiv").clearForm(true);
    $('#collapseProjectTaskTargetsAddEdit').addClass('in');
    $('#collapseProjectTaskTargetsList').addClass('in');
    $("#ProjectTaskTargetsFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
    $("#TaskTargetIsActive").prop("checked", true);
    $("#dtTargetTaskDate").datepicker().datepicker("setDate", new Date());
}

function BindProjectTaskTargetsDetails(data) {
    $("#btnSave").val("Update");
    $("#taskTargetId").val(data.Id);
    $("#ddlTargetTaskNumbers").val(data.TaskNumber);
    $("#dtTargetTaskDate").val(data.TaskDate1);
    $("#ddlPrTaskTargetedValue").val(data.TargetedCompletionValue);

    if (data.IsActive) {
        $("#TaskTargetIsActive").prop("checked", true);
    } else {
        $("#TaskTargetIsActive").prop("checked", false);
    }

    $('#collapseProjectTaskTargetsList').addClass('in');
    $('#collapseProjectTaskTargetsAddEdit').addClass('in');
    $("#ProjectTaskTargetsFormDiv").validationEngine();
}

function BindTaskNumbers() {
    $.ajax({
        type: "POST",
        url: '/Projects/BindTaskNumbers',
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            BindDropdownData(data, "#ddlTargetTaskNumbers", "");
        },
        error: function (msg) {

        }
    });
}




