$(function () {
    $("#CarePlanTaskFormDiv").validationEngine();
    BindActivityTypeDDL();
    BindCorporateFacilityRoles();
    BindOccuranceTypeDDL();
    BindTimeIntervalTypeDDL();
    BindCarePlanId();
    BindOverDueDays();
    GetMaxCarePlanNumber();
    $("#chkIsActive").prop('checked', true);
    $("#hideFrequency").hide();
    InitializeTaskDates();
    $('#chkShowInActive').change(function () {
        ClearFormInCarePlanTask();
        $.validationEngine.closePrompt(".formError", true);
        var showInActive = $("#chkShowInActive").prop("checked");
        BindCarePlanTaskGrid(showInActive ? 0 : 1);
    });
});

function SaveCarePlanTask(id) {
    var isValid = jQuery("#CarePlanTaskFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtId = $("#hidCarePlantaskId").val();
             var txtTaskNumber = $("#txtTaskNumber").val();
             var txtTaskDescription = $("#txtTaskDescription").val();
             var txtCarePlanId = $("#ddlCarePlanId").val();
             var txtActivityType = $("#ddlActivityType").val();
             var txtResponsibleUserType = $("#ddlRoles").val();
             var txtStartTime = $("#txtStartTime").val();
             var txtEndTime = $("#txtEndTime").val();
             var txtIsRecurring = $("#chkRecurring").is(':checked');
             var txtOccuranceType = $("#ddlOccuranceType").val();
             var txtRecTimeInterval = $("#ddlTimeInterval").val();
             var txtRecTImeIntervalType = $("#ddlTimeIntervalType").val();
             var txtIsActive = $("#chkIsActive").is(':checked');
             var txtTaskName = $("#txtTaskName").val();
             var ddlDuedays = $("#ddlDuedays").val();
             var txtExtValue2 = $("#ddlFrequencyType").val();
             //var txtModifiedBy = $("#txtModifiedBy").val();
             //var dtModifiedDate = $("#dtModifiedDate").val();
             //var txtFacilityId = $("#txtFacilityId").val();
             //var txtCorporateId = $("#txtCorporateId").val();
             //var txtExtValue1 = $("#txtExtValue1").val();
             //var txtExtValue2 = $("#txtExtValue2").val();
        var jsonData = JSON.stringify({
            Id: txtId,
            TaskNumber: txtTaskNumber,
            TaskDescription: txtTaskDescription,
            CarePlanId: txtCarePlanId,
            ActivityType: txtActivityType,
            ResponsibleUserType: txtResponsibleUserType,
            StartTime: txtStartTime,
            EndTime: txtEndTime,
            IsRecurring: txtIsRecurring,
            RecurranceType: txtOccuranceType,
            RecTimeInterval: txtRecTimeInterval,
            RecTImeIntervalType: txtRecTImeIntervalType,
            IsActive: txtIsActive,
            TaskName: txtTaskName,
            ExtValue1: ddlDuedays,
            ExtValue2: txtExtValue2,
            //CreatedBy: txtCreatedBy,
            //CreatedDate: dtCreatedDate,
            //ModifiedBy: txtModifiedBy,
            //ModifiedDate: dtModifiedDate,
            //FacilityId: txtFacilityId,
            //CorporateId: txtCorporateId,
            //ExtValue1: txtExtValue1,
            
            //CarePlanTaskId: id,
            //CarePlanTaskMainPhone: txtCarePlanTaskMainPhone,
            //CarePlanTaskFax: txtCarePlanTaskFax,
            //CarePlanTaskLicenseNumberExpire: dtCarePlanTaskLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - CarePlanTask
        });
        $.ajax({
            type: "POST",
            url: '/CarePlanTask/SaveCarePlanTask',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data == "-1") {
                    ShowMessage("Task Number Already Exist. It may be in Active/Inactive list", "Warning", "warning", true);
                    return false;
                }
                ClearFormInCarePlanTask();
                SetGridSorting(SortCarePlanTaskGrid, "#CarePlanTaskListGrid");
                var showInActive = $("#chkShowInActive").prop("checked");
                BindCarePlanTaskGrid(showInActive ? 0 : 1);
                InitializeTaskDates();
                if ($("#chkRecurring").prop("checked")) {
                    $("#recurrance_event").show();
                    $("#showHideOccurrence").show();
                }
                else {
                    $("#recurrance_event").hide();
                    $("#showHideOccurrence").hide();
                }
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

function EditCarePlanTask(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/CarePlanTask/GetCarePlanTaskData',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            //$('#CarePlanTaskFormDiv').empty();
            //$('#CarePlanTaskFormDiv').html(data);
            //$('#collapseCarePlanTaskAddEdit').addClass('in');
            //$("#CarePlanTaskFormDiv").validationEngine();
            //$("#btnSaveCarePlantask").val("Update");
            BindCareTaskData(data);
        },
        error: function (msg) {

        }
    });
}

//function DeleteCarePlanTask(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        //var txtCarePlanTaskId = id;
//        var showInActive = $("#chkShowInActive").prop("checked");
//        var jsonData = JSON.stringify({
//            Id: id,
//            inActive: showInActive ? 0 : 1
//        });
//        $.ajax({
//            type: "POST",
//            url: '/CarePlanTask/DeleteCarePlanTask',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    if (data == -5) {
//                        ShowMessage("Care plan task is already assign to patient", "Warning", "warning", true);
//                    } else {
//                        SetGridSorting(SortCarePlanTaskGrid, "#CarePlanTaskListGrid");
//                        BindCarePlanTaskGrid(showInActive ? 0 : 1);
//                        InitializeTaskDates();
//                        ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//                    }
//                }
//                else {
//                    return false;
//                }
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function BindCarePlanTaskGrid(val) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/CarePlanTask/BindCarePlanTaskList?val=" + val,
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            GetMaxCarePlanNumber();
            $("#CarePlanTaskListDiv").empty();
            $("#CarePlanTaskListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearFormInCarePlanTask() {
    $("#CarePlanTaskFormDiv").clearForm();
    $('#collapseCarePlanTaskList').addClass('in');
    $("#chkIsActive").prop('checked', true);
    $("#CarePlanTaskFormDiv").validationEngine();
    $("#btnSaveCarePlantask").val("Save");
    $("#hidCarePlantaskId").val('');
    $('#chkRecurring').prop('checked', false);
    $("#recurrance_event").hide();
    $("#showHideOccurrence").hide();
    $('#ddlTimeIntervalType').val('0').removeAttr('disabled');
    $('#ddlTimeInterval').val('0').removeAttr('disabled');
}



function DeleteCarePlanTask() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var showInActive = $("#chkShowInActive").prop("checked");
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val(),
            inActive: showInActive ? 0 : 1
        });
        $.ajax({
            type: "POST",
            url: '/CarePlanTask/DeleteCarePlanTask',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    if (data == -5) {
                        ShowMessage("Care plan task is already assign to patient", "Warning", "warning", true);
                    } else {
                        SetGridSorting(SortCarePlanTaskGrid, "#CarePlanTaskListGrid");
                        BindCarePlanTaskGrid(showInActive ? 0 : 1);
                        InitializeTaskDates();
                        ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                    }
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


//function ClearCarePlanTask() {
//    $("#CarePlanTaskFormDiv").clearForm();
//    $('#collapseCarePlanTaskAddEdit').removeClass('in');
//    $('#collapseCarePlanTaskList').addClass('in');
//    $("#CarePlanTaskFormDiv").validationEngine();
//    $.validationEngine.closePrompt(".formError", true);
//    $.ajax({
//        type: "POST",
//        url: '/CarePlanTask/ResetCarePlanTaskForm',
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        data: null,
//        success: function (data) {
//            if (data) {
//                $('#CarePlanTaskFormDiv').empty();
//                $('#CarePlanTaskFormDiv').html(data);
//                $('#collapseCarePlanTaskList').addClass('in');
//                BindCarePlanTaskGrid();
//            }
//            else {
//                return false;
//            }
//        },
//        error: function (msg) {


//            return true;
//        }
//    });

//}

function BindActivityTypeDDL() {
    BindGlobalCodesWithValue('#ddlActivityType', 1201, '');
}

function BindOccuranceTypeDDL() {
    BindGlobalCodesWithValueWithOrder('#ddlOccuranceType', 4906, '');
}

function BindTimeIntervalTypeDDL() {
    BindGlobalCodesWithValueWithOrder('#ddlTimeIntervalType', 4907, '');
}


function BindCorporateFacilityRoles() {
   $.ajax({
        type: "POST",
        url: "/CarePlanTask/BindUsersType",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $("#ddlRoles").empty();

            var items = '<option value="0">--Select--</option>';

            $.each(data, function (i, role) {
                items += "<option value='" + role.Value + "'>" + role.Text + "</option>";
            });

            $("#ddlRoles").html(items);

            //if ($("#hdRoleID") != null && $("#hdRoleID") != '')
            //    $("#ddlRoles").val($("#hdRoleID").val());
        },
        error: function (msg) {
            
        }
    });
}


function BindRecurranceTimeInterval() {
    var i = 0;
    var items = '<option value="0">--Select--</option>';
    var intervalType = $("#ddlTimeIntervalType").val();
    if (intervalType > 0) {
        if (intervalType == 1) {
            for (i = 1; i <= 60; i++) {
                items += "<option value='" + i + "'>" + i+ "</option>";
            }
            $("#ddlTimeInterval").html(items);
            
        } else if (intervalType == 2) {
            for (i = 1; i <= 24; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);
        } else if (intervalType == 3) {
            for (i = 1; i <= 31; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);
        } else if (intervalType == 4) {
            for (i = 1; i <= 4; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);
        } else if (intervalType == 5) {
            for (i = 1; i <= 12; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);
        } else {
            for (i = 1; i <= 10; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);
        }
    }
}


function BindCareTaskData(data) {
   
    if (data.IsRecurring) {
        $("#recurrance_event").show();
        $("#showHideOccurrence").show();
        $("#ddlOccuranceType").val(data.RecurranceType);
        $("#ddlTimeIntervalType").val(data.RecTImeIntervalType);
        BindRecurranceTimeInterval();
        $("#ddlTimeInterval").val(data.RecTimeInterval);
        BindRecrrencetype();
    } else {
        $("#recurrance_event").hide();
        $("#showHideOccurrence").hide();
    }


    $("#ddlDuedays").val(data.ExtValue1);
    $("#txtTaskName").val(data.TaskName);
    $("#ddlCarePlanId").val(data.CarePlanId);
    $("#hidCarePlantaskId").val(data.Id);
    $("#txtTaskNumber").val(data.TaskNumber);
    $("#ddlActivityType").val(data.ActivityType);
    $("#ddlRoles").val(data.ResponsibleUserType);
    $("#txtStartTime").val(data.StartTime);
    $("#txtEndTime").val(data.EndTime);
  
    if (data.RecurranceType == "2") {
        $("#hideFrequency").show();
    } else {
        $("#hideFrequency").hide();
    }

    InitializeTaskDates();
    //$("#ddlTimeInterval").val(data.RecTimeInterval);
    $("#ddlFrequencyType").val(data.ExtValue2);
    $("#chkRecurring").prop('checked', data.IsRecurring);
   

    $("#chkIsActive").prop('checked', data.IsActive);
    $("#txtTaskDescription").val(data.TaskDescription);
   
   
    $("#btnSaveCarePlantask").val("Update");
    $('#collapseCarePlanTaskAddEdit').addClass('in').attr('style', 'height: 250px;');;
}



function BindCarePlanId() {
    $.ajax({
        type: "POST",
        url: "/CarePlanTask/BindCarePlanDropdown",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $("#ddlCarePlanId").empty();

            var items = '<option value="0">--Select--</option>';

            $.each(data, function (i, care) {
                items += "<option value='" + care.Value + "'>" + care.Text + "</option>";
            });

            $("#ddlCarePlanId").html(items);

          },
        error: function (msg) {

        }
    });
}



function GetMaxCarePlanNumber() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/CarePlanTask/GetMaxTaskNumber",
        dataType: "Json",
        async: true,
        data: null,
        success: function (data) {
            $("#txtTaskNumber").val(data);
        },
        error: function (msg) {

        }

    });

}

function BindOverDueDays() {
    var i = 0;
    var items = '<option value="0">--Select--</option>';
 
            for (i = 1; i <= 120; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlDuedays").html(items);
 }
   

function BindFrequencyTypeDDL() {
    BindGlobalCodesWithValue('#ddlFrequencyType', 4909, '');
   
}


//function BindFrequencyTypeDropdown() {
//    var fType = $("#ddlOccuranceType").val();
//    if (fType == "2") {
//        $("#hideFrequency").show();
//        BindFrequencyTypeDDL();
//    } else {
//        $("#hideFrequency").hide();
//    }
//}


//var BindTextBoxValidation = function () {
//    var minvalue = $("#txtStartTime").val();
//    if (minvalue.indexOf(':') > 0) {
//        var minvalueInterval = minvalue.split(':')[1];
//        minvalue = minvalueInterval == "00" ? minvalue.split(':')[0] + ":30" : (parseInt(minvalue.split(':')[0], 10) + 1) + ":00";
//    }
//    $("#txtEndTime").datetimepicker({
//        datepicker: false,
//        format: 'H:i',
//        step: 30,
//        mask: true,
//        minTime: minvalue
//    });
//}


var BindTextBoxValidation = function () {
    $("#txtEndTime").val('');
    var minvalue = $("#txtStartTime").val();
    if (minvalue.indexOf(':') > 0) {
        var minvalueInterval = minvalue.split(':')[1];
        minvalue = minvalueInterval == "00" ? minvalue.split(':')[0] + ":30" : (parseInt(minvalue.split(':')[0], 10) + 1) + ":00";
    }
    $("#txtEndTime").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 30,
        mask: true,
        minTime: minvalue
    });
};



function InitializeTaskDates() {
    $("#txtStartTime").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 30,
        //allowTimes: [
        //    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00'
        //],
        mask: true,
        onChangeDateTime: BindTextBoxValidation,
        onShow: BindTextBoxValidation
    });

   $("#txtEndTime").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 30,
        //allowTimes: [
        //    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00'
        //],
        mask: true,
    });
}

var BindRecrrencetype = function() {
    var occurancetype = $('#ddlOccuranceType').val();
    switch (occurancetype) {
        case "1": //Every Day
            $('#ddlTimeIntervalType').val('3').attr('disabled','disabled');
            BindRecurranceTimeInterval();
            $('#ddlTimeInterval').val('1').attr('disabled', 'disabled');
            break;
        case "2"://Every Alternate Day
            $('#ddlTimeIntervalType').val('3').attr('disabled', 'disabled');
            BindRecurranceTimeInterval();
            $('#ddlTimeInterval').val('2').attr('disabled', 'disabled');
            break;
        case "3"://Every Week
            $('#ddlTimeIntervalType').val('4').attr('disabled', 'disabled');
            BindRecurranceTimeInterval();
            $('#ddlTimeInterval').val('1').attr('disabled', 'disabled');
            break;
        case "4"://Every Alternate Week
            $('#ddlTimeIntervalType').val('4').attr('disabled', 'disabled');
            BindRecurranceTimeInterval();
            $('#ddlTimeInterval').val('2').attr('disabled', 'disabled');
            break;
        case "5"://Every Month
            $('#ddlTimeIntervalType').val('5').attr('disabled', 'disabled');
            BindRecurranceTimeInterval();
            $('#ddlTimeInterval').val('1').attr('disabled', 'disabled');
            break;
        case "6"://Every Alternate Month
            $('#ddlTimeIntervalType').val('5').attr('disabled', 'disabled');
            BindRecurranceTimeInterval();
            $('#ddlTimeInterval').val('2').attr('disabled', 'disabled');
            break;
        default:
            $('#ddlTimeIntervalType').removeAttr('disabled');
            $('#ddlTimeInterval').removeAttr('disabled');
    }
}

function SortCarePlanTaskGrid(event) {
    var showInActive = $("#chkShowInActive").prop("checked") ? 0 : 1;
    var url = "/CarePlanTask/BindCarePlanTaskList";
    //var patientId = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?val=" + showInActive + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#CarePlanTaskListDiv").empty();
            $("#CarePlanTaskListDiv").html(data);

        },
        error: function (msg) {
        }
    });
}


