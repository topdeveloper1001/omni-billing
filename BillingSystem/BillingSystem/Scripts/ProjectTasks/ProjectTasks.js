$(function () {
    $("#ProjectTasksFormDiv").validationEngine();
    BindProjectNumbersDropDown("#ddlTaskProjectNumber");
    BindGlobalCodesWithValue("#ddlTaskColorCode", 4406, "");
    setTimeout(function () { BindFacilityUsers($("#ddlFacility").val()); }, 2000);
    GetProjectTasksList();
});

function GetProjectTasksList() {
    var userId = $("#ddlUsers").val() > 0 ? $("#ddlUsers").val() : '';
    $.ajax({
        type: "POST",
        url: '/Projects/ProjectTasksList',
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ userId: userId }),
        success: function (data) {
            BindList("#ProjectTasksListDiv", data);
        },
        error: function (msg) {

        }
    });
}

function SaveProjectTasks() {
    var isValid = jQuery("#ProjectTasksFormDiv").validationEngine({ returnIsValid: true });
    if ($("#ddlFacility").val() == "0" && isValid == true) {
        isValid = false;
        var msg = "Please select facility";
        ShowMessage(msg, "Warning", "warning", true);
        $("#ddlFacility").focus();
    }
    if (isValid == true) {
        var txtTaskNumber = $("#txtTaskNumber").val();
        var projectTaskId = $("#ProjectTaskId").val() == "" ? "0" : $("#ProjectTaskId").val();//added by Nitin on 20150723
        $.post("/Projects/CheckDuplicateTaskNumber", { projectNumber: $("#ddlTaskProjectNumber").val(), taskNumber: txtTaskNumber, taskId: projectTaskId }, function (isExists) {
            if (!isExists) {
                var txtTaskName = $("#txtTaskName").val();
                var txtTaskDescription = $("#txtTaskDescription").val();
                var dtStartDate = $("#dtTaskStartDate").val();
                var dtEstCompletionDate = $("#dtTaskEstCompletionDate").val();
                var txtUserResponsible = $("#ddlUsersInProjectTasks").val();
                var jsonData = JSON.stringify({
                    ProjectTaskId: $("#ProjectTaskId").val(),
                    ProjectNumber: $("#ddlTaskProjectNumber").val(),
                    TaskNumber: txtTaskNumber,
                    TaskName: txtTaskName,
                    TaskDescription: txtTaskDescription,
                    StartDate: dtStartDate,
                    EstCompletionDate: dtEstCompletionDate,
                    UserResponsible: txtUserResponsible,
                    IsActive: $('#taskIsActive').prop("checked"), //$("#taskIsActive")[0].checked
                    ExternalValue1: $("#milestone")[0].checked ? 1 : 0,
                    ExternalValue2: $("#ddlTaskColorCode").val(),
                    FacilityId: $("#ddlFacility").val(),
                    ExternalValue3: $("#ddlUsers").val() > 0 ? $("#ddlUsers").val() : "",
                    Comments: $("#Comments").val()
                });

                $.ajax({
                    type: "POST",
                    url: '/Projects/SaveProjectTasks',
                    async: false,
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    data: jsonData,
                    success: function (data) {
                        BindList("#ProjectTasksListDiv", data);
                        var msg1 = "Records Saved successfully !";
                        if ($("#ProjectTaskId").val() > 0)
                            msg1 = "Records updated successfully";
                        ClearProjectTasksForm();
                        ShowMessage(msg1, "Success", "success", true);
                        BindTaskNumbers();
                        GetProjectTaskTargetsList();
                    },
                    error: function (msg) {

                    }
                });
            }
            else {
                ShowMessage("This Task Number is already in the list under this Project Number!", "Alert", "warning", true);
            }
        });
    }
}

function EditProjectTasks(id) {
    $.validationEngine.closePrompt(".formError", true);
    $.post("/ProjectTasks/GetProjectTasksDetails", { id: id }, function (data) {
        BindProjectTasksDetails(data);
    });
}

function DeleteProjectTasks() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var id = $("#hfGlobalConfirmId").val();
        if (id > 0) {
            var userId = $("#ddlUsers").val() > 0 ? $("#ddlUsers").val() : '';
            $.post("/Projects/DeleteProjectTasks", { id: id, userId: userId }, function (data) {
                BindList("#ProjectTasksListDiv", data);
                ShowMessage("Records Deleted Successfully", "Success", "success", true);
                BindTaskNumbers();
                $('#txtTaskNumber').prop("disabled", false);
                $('#dtTaskStartDate').prop("disabled", false);
            });
        }
    }
}

//function DeleteProjectTasks(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        if (id > 0) {
//            var userId = $("#ddlUsers").val() > 0 ? $("#ddlUsers").val() : '';
//            $.post("/Projects/DeleteProjectTasks", { id: id, userId: userId }, function (data) {
//                BindList("#ProjectTasksListDiv", data);
//                ShowMessage("Records Deleted Successfully", "Success", "success", true);
//                BindTaskNumbers();
//                $('#txtTaskNumber').prop("disabled", false);
//                $('#dtTaskStartDate').prop("disabled", false);
//            });
//        }
//    }
//}

function ClearProjectTasksForm() {
    $("#ProjectTasksFormDiv").clearForm(true);
    $('#collapseProjectTasksAddEdit').addClass('in');
    $('#collapseProjectTasksList').addClass('in');
    $("#ProjectTasksFormDiv").validationEngine();
    $("#btnTasksSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
    $('#txtTaskNumber').prop("disabled", false);
    $('#dtTaskStartDate').prop("disabled", false);
    $("#milestone").prop("checked", true);
    $("#taskIsActive").prop("checked", true);
    $("#dtTaskStartDate").datepicker().datepicker("setDate", new Date());
    $("#dtTaskEstCompletionDate").datepicker().datepicker("setDate", new Date());
}

function BindProjectTasksDetails(data) {
    $("#btnTasksSave").val("Update");
    $('#collapseProjectTasksList').addClass('in');
    $('#collapseProjectTasksAddEdit').addClass('in');
    $('#ProjectTaskId').val(data.ProjectTaskId);
    $('#ddlTaskProjectNumber').val(data.ProjectNumber);
    $('#txtTaskNumber').val(data.TaskNumber);
    $('#txtTaskName').val(data.TaskName);
    //$('#txtTaskNumber').prop("disabled", true);
    $('#txtTaskDescription').val(data.TaskDescription);
    $('#dtTaskStartDate').val(data.StartDate);
    if (data.StartDate != null && data.StartDate != "") {
        $('#dtTaskStartDate').prop("disabled", true);
    }
    else if (data.StartDate == "") {
        $('#dtTaskStartDate').prop("disabled", false);
    }
    $("#Comments").val(data.Comments);
    $('#dtTaskEstCompletionDate').val(data.CompletionDate);
    $('#ddlUsersInProjectTasks').val(data.UserResponsible);
    $('#ddlTaskColorCode').val(data.ExternalValue2);
    $('#collapseProjectTasksAddEdit').val('in');
    $("#ProjectTasksFormDiv").validationEngine();

    if (data.IsActive) {
        $("#taskIsActive").prop("checked", true);
    } else {
        $("#taskIsActive").prop("checked", false);
    }

    if (data.ExternalValue1 == "1") {
        $("#milestone").prop("checked", true);
    } else {
        $("#milestone").prop("checked", false);
    }
    $('html,body').animate({ scrollTop: $("#collapseProjectTasksAddEdit").offset().top }, 'fast');
}

function RebindProjectTasks() {
    GetProjectTasksList();
}

function BindFacilityUsers(facilityId) {
    //$.post("/Home/GetNonAdminUsersByCorporate", {}, function (data) {
    //    BindDropdownData(data, "#ddlUsers", "");
    //    BindDropdownData(data, "#ddlUsersInProject", "");
    //    BindDropdownData(data, "#ddlUsersInProjectTasks", "");
    //});
    $.post("/Security/GetUsersByDefaultCorporateId", {}, function (data) {
        data.sort(SortByName);
        BindDropdownData(data, "#ddlUsers", "");
        BindDropdownData(data, "#ddlUsersInProject", "");
        BindDropdownData(data, "#ddlUsersInProjectTasks", "");
    });
}

function SortByName(a, b) {
    var aName = a.Text.trim().toLowerCase();
    var bName = b.Text.trim().toLowerCase();
    return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}