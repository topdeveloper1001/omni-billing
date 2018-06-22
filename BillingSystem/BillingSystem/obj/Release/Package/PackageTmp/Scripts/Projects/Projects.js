$(function () {
    $("#ProjectsFormDiv").validationEngine();
    var loggedInFacilityId = $("#hfLoggedInFacility").val();
    BindFacilities('#ddlFacility', loggedInFacilityId);
    BindGlobalCodesWithValue("#ddlDashboardProjectType", 4408, "");
    BindGlobalCodesWithValue("#ddlProjectStatusColor", 4406, "");
    $("#ddlFacility").change(function () {
        var selectedValue = $(this).val();
        window.location.href = "/Projects/Index?fId=" + selectedValue;
    });

    /*Set today's date as default date*/ /*Comment By Krishna*/
    //$("#dtStartDate").datepicker().datepicker("setDate", new Date());
    //$("#dtRevisedCompletionDate").datepicker().datepicker("setDate", new Date());
    //$("#dtEstCompletionDate").datepicker().datepicker("setDate", new Date());
    //$("#dtTaskStartDate").datepicker().datepicker("setDate", new Date());
    //$("#dtTaskEstCompletionDate").datepicker().datepicker("setDate", new Date());
    //$("#dtTargetTaskDate").datepicker().datepicker("setDate", new Date());
    $("#dtStartDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));
    $("#dtRevisedCompletionDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));
    $("#dtEstCompletionDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));
    $("#dtTaskStartDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));
    $("#dtTaskEstCompletionDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));
    $("#dtTargetTaskDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));

    InitializeDatesInProjects();
});

var oldProjectNumber = '';

function SaveProjects() {

    if ($('#dtRevisedCompletionDate').val() < $('#dtStartDate').val()) {
        ShowMessage("End Date Should Be Greater!", "Warning", "warning", true);
        return false;
    }

    if ($('#dtEstCompletionDate').val() < $('#dtStartDate').val()) {
        ShowMessage("Project Estimated Completion Date Should Be Greater To Start Date!", "Warning", "warning", true);
        return false;
    }

    var isValid = jQuery("#ProjectsFormDiv").validationEngine({ returnIsValid: true });
    if ($("#ddlFacility").val() == "0" && isValid == true) {
        isValid = false;
        var msg = "Please select facility";
        ShowMessage(msg, "Warning", "warning", true);
        $("#ddlFacility").focus();
    }
    var txtProjectNumber = $("#txtProjectNumber").val();
    var hfProjectId = $("#hfProjectId").val() == "" ? "0" : $("#hfProjectId").val();
    if (isValid == true) {
        $.post("/Projects/CheckDuplicateProjectNumber", { projectNumber: txtProjectNumber, projectId: hfProjectId }, function (isExists) {
            if (!isExists) {
                var txtName = $("#txtName").val();
                var txtProjectDescription = $("#txtProjectDescription").val();
                var dtStartDate = $("#dtStartDate").val();
                var dtEstCompletionDate = $("#dtEstCompletionDate").val();
                var txtUserResponsible = $("#ddlUsersInProject").val();
                var dtRevisedCompletionDate = $("#dtRevisedCompletionDate").val();
                var jsonData = JSON.stringify({
                    ProjectId: $("#hfProjectId").val(),
                    ProjectNumber: txtProjectNumber,
                    Name: txtName,
                    ProjectDescription: txtProjectDescription,
                    StartDate: dtStartDate,
                    EstCompletionDate: dtEstCompletionDate,
                    UserResponsible: txtUserResponsible,
                    IsActive: $('#ProjectsIsActive').prop("checked"), //$("#ProjectsIsActive")[0].checked
                    ExternalValue1: dtRevisedCompletionDate,
                    ExternalValue2: $("#ddlDashboardProjectType").val(),
                    ExternalValue3: $("#ddlProjectStatusColor").val(),
                    FacilityId: $("#ddlFacility").val()
                });
                $.ajax({
                    type: "POST",
                    url: '/Projects/SaveProjects',
                    async: false,
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    data: jsonData,
                    success: function (data) {
                        BindList("#ProjectsListDiv", data);
                        GetProjectTasksList();
                        //After saving, rebind the Project Numbers dropdown
                        if ($("#ddlProjectNumber").length > 0)
                            BindProjectNumbersDropDown();
                        var newProjectNumber = txtProjectNumber;

                        ClearProjectsForm();
                        var msg = "Records Saved successfully !";
                        if (parseInt($("#hfProjectId").val()) > 0)
                            msg = "Records updated successfully";
                        ShowMessage(msg, "Success", "success", true);

                        BindProjectNumbersDropDown("#ddlProjectNumber1");
                        BindProjectNumbersDropDown("#ddlTaskProjectNumber");

                        if (oldProjectNumber != '' && oldProjectNumber != newProjectNumber) {
                            RebindProjectTasks();
                        }
                    },
                    error: function (msg) {

                    }
                });
            } else {
                //ShowWarningMessage("Project Number already exists in the list!", true);
                ShowMessage("Project Number already exists in the list!", "Warning", "warning", true);
                $("#txtProjectNumber").focus();
            }
        });
    }
}

function EditProjects(id) {
    $.validationEngine.closePrompt(".formError", true);
    var jsonData = JSON.stringify({
        id: id,
    });
    $.ajax({
        type: "POST",
        url: '/Projects/GetProjectsDetails',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindProjectsDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteProjects() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val(),
        });
        $.ajax({
            type: "POST",
            url: '/Projects/DeleteProjects',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#ProjectsListDiv", data);
                BindProjectNumbersDropDown("#ddlProjectNumber1");
                BindProjectNumbersDropDown("#ddlTaskProjectNumber");
                ShowMessage("Records Deleted Successfully", "Success", "success", true);
                $("#dtStartDate").prop("disabled", false);
            },
            error: function (msg) {
                return true;
            }
        });

    }
}

//function DeleteProjects(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var jsonData = JSON.stringify({

//            id: id,
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Projects/DeleteProjects',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                BindList("#ProjectsListDiv", data);
//                BindProjectNumbersDropDown("#ddlProjectNumber1");
//                BindProjectNumbersDropDown("#ddlTaskProjectNumber");
//                ShowMessage("Records Deleted Successfully", "Success", "success", true);
//                $("#dtStartDate").prop("disabled", false);
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function ClearProjectsForm() {
    $("#ProjectsFormDiv").clearForm(true);
    $('#collapseProjectsAddEdit').addClass('in');
    $('#collapseProjectsList').addClass('in');
    $("#ProjectsFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
    $("#ProjectsIsActive").prop("checked", true);
    $("#txtProjectNumber").prop("disabled", false);
    $("#dtStartDate").prop("disabled", false);
    /*Comment By Krishna*/
    //$("#dtStartDate").datepicker().datepicker("setDate", new Date());
    //$("#dtEstCompletionDate").datepicker().datepicker("setDate", new Date());
    //$("#dtRevisedCompletionDate").datepicker().datepicker("setDate", new Date());
    $("#dtStartDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));
    $("#dtRevisedCompletionDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));
    $("#dtEstCompletionDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));
    InitializeDatesInProjects();
}

function BindProjectsDetails(data) {
    $("#btnSave").val("Update");
    $('#collapseProjectsList').addClass('in');
    $('#collapseProjectsAddEdit').addClass('in');
    $("#ProjectsFormDiv").validationEngine();
    $("#txtProjectNumber").val(data.ProjectNumber);
    oldProjectNumber = data.ProjectNumber;

    //$("#txtProjectNumber").prop("disabled", true);
    $("#txtName").val(data.Name);
    $("#ddlDashboardProjectType").val(data.ExternalValue2);
    $("#ddlProjectStatusColor").val(data.ExternalValue3);
    $("#txtProjectDescription").val(data.ProjectDescription);
    if (data.StartDate != null && data.StartDate != "") {
        $("#dtStartDate").val(data.StartDate);
        $("#dtStartDate").prop("disabled", true);
    }
    else if (data.StartDate == "") {
        $("#dtStartDate").val(data.StartDate);
    }
    $("#dtEstCompletionDate").val(data.EstCompletionDate);
    $("#ddlUsersInProject").val(data.UserResponsible);
    $("#hfProjectId").val(data.ProjectId);

    if (data.ExternalValue1 != null && data.ExternalValue1 != "") {
        $("#dtRevisedCompletionDate").val(data.ExternalValue1);
    }
    else if (data.ExternalValue1 == "") {
        $("#dtStartDate").val(data.ExternalValue1);
    }

    $("#ProjectsIsActive").prop("checked", data.IsActive);

    $('html,body').animate({ scrollTop: $("#collapseProjectsAddEdit").offset().top }, 'fast');
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function InitializeDatesInProjects() {
    if ($("#dtStartDate").length > 0) {
        $("#dtStartDate").datetimepicker({
            format: 'm/d/Y',
            minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
            timepicker: false,
            closeOnDateSelect: true
        });
    }

    if ($("#dtRevisedCompletionDate").length > 0) {
        $("#dtRevisedCompletionDate").datetimepicker({
            format: 'm/d/Y',
            minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
            timepicker: false,
            closeOnDateSelect: true
        });
    }

    if ($("#dtTaskStartDate").length > 0) {
        $("#dtTaskStartDate").datetimepicker({
            format: 'm/d/Y',
            minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
            timepicker: false,
            closeOnDateSelect: true
        });
    }

    if ($("#dtEstCompletionDate").length > 0) {
        $("#dtEstCompletionDate").datetimepicker({
            format: 'm/d/Y',
            minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
            timepicker: false,
            closeOnDateSelect: true
        });
    }

    if ($("#dtTaskEstCompletionDate").length > 0) {
        $("#dtTaskEstCompletionDate").datetimepicker({
            format: 'm/d/Y',
            minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
            timepicker: false,
            closeOnDateSelect: true
        });
    }

    if ($("#dtTargetTaskDate").length > 0) {
        $("#dtTargetTaskDate").datetimepicker({
            format: 'm/d/Y',
            minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
            timepicker: false,
            closeOnDateSelect: true
        });
    }

}