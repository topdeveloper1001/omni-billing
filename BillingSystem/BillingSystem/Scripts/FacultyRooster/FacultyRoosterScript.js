var apptRecArray = [];
$(function () { main(); });

var main = function () {
    $("#divFacultyTimming").validationEngine();
    BindDataOnLoad();

    $('#ddlCorporate').on('click', function () {
        BindFacilityByCoporateId();
    });
    $('#ddlFacility').on('click', function () {
        if ($('#ddlFacility').val() != "0") {
            BindPhysiciansAndDepartmentsInFacultyTiming();
        }
        else {
            $('#ddlFaculty').val(0);
            $('#divDepartmentList').empty();
            $('#divDepartments').hide();
        }
    });
    $('#btnSaveSchedulingData').on('click', function () {
        SaveFacultyTimmingData();
    });

    $('#btnCancelSchedulingData').on('click', function () {
        $('#hfAppointmentTypes').val('');
        $("#divPhysicianPatient").clearForm();
        $("#tbDepartmentTimmingList").empty();
        $('.editdisabled').removeAttr('disabled');
        apptRecArray = [];//Clear reccurrence array
        $.validationEngine.closePrompt(".formError", true);

        $("#divReccurrencePopup").hide();
    });

    $('input[name=rbtnRecType]').change(function () {
        if ($(this).is(":checked")) {
            var radioval = $(this).val();
            if (radioval == "daily") {
                $(".reccurrence_field").hide();
                $("#daily").show();
            };
            if (radioval == "weekly") {
                $(".reccurrence_field").hide();
                $("#weekly").show();
            };
            if (radioval == "monthly") {
                $(".reccurrence_field").hide();
                $("#monthly").show();
            };
        }


    });
};

function SaveFacultyRooster(id) {
    var isValid = jQuery("#divFacultyTimming").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtId = $("#txtId").val();
        var txtFacultyId = $("#txtFacultyId").val();
        var txtFacultyType = $("#txtFacultyType").val();
        var txtDeptId = $("#txtDeptId").val();
        var txtAvailabilityType = $("#txtAvailabilityType").val();
        var txtPattern = $("#txtPattern").val();
        var txtWorkingDay = $("#txtWorkingDay").val();
        var txtWeekNumber = $("#txtWeekNumber").val();
        var dtFromDate = $("#dtFromDate").val();
        var txtToDate = $("#txtToDate").val();
        var txtFacilityId = $("#txtFacilityId").val();
        var txtCorporateId = $("#txtCorporateId").val();
        var txtIsActive = $("#txtIsActive").val();
        var txtCreatedBy = $("#txtCreatedBy").val();
        var dtCreatedDate = $("#dtCreatedDate").val();
        var dtRecurreanceDateTo = $("#dtRecurreanceDateTo").val();
        var txtRecurreanceDateFrom = $("#txtRecurreanceDateFrom").val();
        var txtExtValue1 = $("#txtExtValue1").val();
        var txtExtValue2 = $("#txtExtValue2").val();
        var txtExtValue3 = $("#txtExtValue3").val();
        var jsonData = JSON.stringify({
            Id: txtId,
            FacultyId: txtFacultyId,
            FacultyType: txtFacultyType,
            DeptId: txtDeptId,
            AvailabilityType: txtAvailabilityType,
            Pattern: txtPattern,
            WorkingDay: txtWorkingDay,
            WeekNumber: txtWeekNumber,
            FromDate: dtFromDate,
            ToDate: txtToDate,
            FacilityId: txtFacilityId,
            CorporateId: txtCorporateId,
            IsActive: txtIsActive,
            CreatedBy: txtCreatedBy,
            CreatedDate: dtCreatedDate,
            RecurreanceDateTo: dtRecurreanceDateTo,
            RecurreanceDateFrom: txtRecurreanceDateFrom,
            ExtValue1: txtExtValue1,
            ExtValue2: txtExtValue2,
            ExtValue3: txtExtValue3,
            //FacultyRoosterId: id,
            //FacultyRoosterMainPhone: txtFacultyRoosterMainPhone,
            //FacultyRoosterFax: txtFacultyRoosterFax,
            //FacultyRoosterLicenseNumberExpire: dtFacultyRoosterLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - FacultyRooster
        });
        $.ajax({
            type: "POST",
            url: '/FacultyRooster/SaveFacultyRooster',
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data > 0) {
                    ClearFaultyTimingForm();
                    var msg = "Records Saved successfully !";
                    if (id > 0)
                        msg = "Records updated successfully";

                    ShowMessage(msg, "Success", "success", true);
                }
            },
            error: function (msg) {
                console.log(msg);
            }
        });
    }
}

function EditFacultyRooster(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/FacultyRooster/GetFacultyRooster',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $('#ddlCorporate').val(data.CorporateId);
            $('#hdCorporateId').val(data.CorporateId);
            BindFacilityByCoporateId();
            $('#ddlFacility').val(data.FacilityId);
            $('#hdFacilityId').val(data.FacilityId);
            BindPhysiciansAndDepartmentsInFacultyTiming();
            $('#ddlFaculty').val(data.FacultyId);
            $('#hdFacultyId').val(data.FacultyId);
            $('#ddlDepartmentType').val(data.DeptId);
            SetDepartmentAndSpeciality('#ddlFaculty');
            AddDepartmentTimmingObj(data);
            $('#collapseFacultyRoosterAddEdit').addClass('in');
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

function DeleteFacultyRooster() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/FacultyRooster/DeleteFacultyRooster',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data != -1) {
                    BindFacultyRoosterGrid();
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                }
                else {
                    ShowMessage("Unable to Delete the record!", "Warning", "warning", true);
                }
            },
            error: function (msg) {
                console.log(msg);
                return true;
            }
        });
    }
}

function BindFacultyRoosterGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/FacultyRooster/BindFacultyRoosterList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#FacultyRoosterListDiv").empty();
            $("#FacultyRoosterListDiv").html(data);
        },
        error: function (msg) {
            console.log(msg);

        }

    });
}

function ClearFaultyTimingForm() {
    $("#FacultyRoosterFormDiv").clearForm();
    $('#collapseFacultyRoosterAddEdit').removeClass('in');
    $('#collapseFacultyRoosterList').addClass('in');
    $("#FacultyRoosterFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/FacultyRooster/ResetFacultyRoosterForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            if (data) {
                $('#FacultyRoosterFormDiv').empty();
                $('#FacultyRoosterFormDiv').html(data);
                $('#collapseFacultyRoosterList').addClass('in');
                BindFacultyRoosterGrid();
            }
        },
        error: function (msg) {
            console.log(msg);
            return true;
        }
    });
}

var GetFacilityDepartments = function () {
    var coporateId = $('#ddlCorporate').val();
    var facilityId = $('#ddlFacility').val();
    var jsonData = JSON.stringify({
        coporateId: coporateId,
        facilityId: facilityId
    });
    $.ajax({
        type: "POST",
        url: "/FacultyRooster/GetFacilityDepartments",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $('#ddlDepartmentType').empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data, function (i, department) {
                items += "<option value='" + department.FacilityStructureId + "'>" + department.FacilityStructureName + "</option>";
            });
            $('#ddlDepartmentType').html(items);
            $('#ddlDepartmentType').val("0");
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

var GetFacilityPhycisian = function () {
    var coporateId = $('#ddlCorporate').val();
    var facilityId = $('#ddlFacility').val();
    var jsonData = JSON.stringify({
        coporateId: coporateId,
        facilityId: facilityId
    });
    $.ajax({
        type: "POST",
        url: "/Home/GetFacilityPhycisian",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $('#ddlFaculty').empty();

            var items = '<option value="0">--Select--</option>';

            $.each(data, function (i, physician) {
                items += "<option value='" + physician.Id + "'>" + physician.PhysicianName + "</option>";
            });
            $('#ddlFaculty').html(items);

            if ($('#hdFacultyId').val() != '' && $('#hdFacultyId').val() != '0') {
                $('#ddlFaculty').val($('#hdFacultyId').val());
            }
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

function AddDepartmentTimeSlot() {
    var ctrl = $("#tbDepartmentTimmingList");
    var departmentName = $("#ddlDepartmentType :selected").text();
    var departmentValue = $("#ddlDepartmentType").val();
    if (departmentValue == undefined) {
        ShowMessage('Please select facility from the top and then department!', "Warning", "warning", true);
        return false;
    }
    if (departmentValue == "0") {
        ShowMessage('Please select department!', "Warning", "warning", true);
        return false;
    }
    var obj = $("#datef" + departmentValue);
    if (obj.length > 0) {
        ShowMessage('department already added in the below listing!', "Warning", "warning", true);
        return false;
    }



    var html = "";
    html += "<tr id='tr" + departmentValue + "'><input type='hidden' id='hfMain" + departmentValue + "' value='0'/><input type='hidden' id='hfDeptOpeningDays" + departmentValue + "' value=''/>";
    html += "<td>" + departmentName + "</td><input type='hidden' id='hfDepartmentid" + departmentValue + "'  value='" + departmentValue + "'/>";
    html += "<td><input id='datef" + departmentValue + "' type='text' class='validate[required]' onblur='ValidateOpeningData(this)';/></td>";
    //html += "<td><input id='datet" + departmentValue + "' type='text' class='validate[required]'/></td>";
    html += "<td><input id='timef" + departmentValue + "' type='text' class='validate[required]'/></td>";
    html += "<td><input id='timet" + departmentValue + "' type='text' class='validate[required]'/></td>";
    html += "<td class='recurrence_event'><input id='chkIsRec" + departmentValue + "' type='checkbox' class='departmentChk' onchange='OnChangeIsSpecialChk(this," + departmentValue + ");'/><input type='button' id='btnEditRecurrence" + departmentValue + "' class='btn btn-xs btn-default btnShowHide' style='margin-left:5px;display:none;' value='Edit' statusattr='new' onclick='OnChangeIsSpecialChk(this," + departmentValue + ");'/></td>";
    html += "<td><img src='/images/delete.png' width='15px' onclick='RemoveAppointmentProcedures(" + departmentValue + ")'/></td>";
    html += "</tr>";
    ctrl.append(html);

    $("#datef" + departmentValue).datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        timepicker: false,
        closeOnDateSelect: true
    });
    $("#datet" + departmentValue).datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        timepicker: false,
        closeOnDateSelect: true
    });
    $("#timef" + departmentValue).kendoTimePicker({
        format: "HH:mm",
        change: function (e) {
            startChange("#timef" + departmentValue, "#timet" + departmentValue, parseInt(30));
        }
    }).data("kendoTimePicker");
    $("#timet" + departmentValue).kendoTimePicker({
        format: "HH:mm"
    }).data("kendoTimePicker");

    var hfApptTypes = $("#hfAppointmentTypes").val();
    var concatApptTypes = hfApptTypes + "," + departmentValue;
    $("#hfAppointmentTypes").val(concatApptTypes.replace(/^,|,$/g, ''));

    $("#appointmentTypes").val("0");
    GetDeptOpeningDays(departmentValue);
    var date = new Date();
    $("#datef" + departmentValue).val(date.getMonth() + 1 + "/" + date.getDate() + '/' + date.getFullYear());//adding today's as by default
    $("#datet" + departmentValue).val(date.getMonth() + 1 + "/" + date.getDate() + '/' + date.getFullYear());//adding today's as by default
    var initialDate = "";
    if ($("#hfAppointmentTypes").val() != "") {
        var hfAptypesObjValueArray = $("#hfAppointmentTypes").val().split(",");
        initialDate = $("#date" + hfAptypesObjValueArray[0]).val();
        $("#datef" + departmentValue).val(initialDate);
        $("#datet" + departmentValue).val(initialDate);
    }
}

var startChange = function (timeFromId, timeToId, timeInterval) {
    
    var tpFrom = $(timeFromId).data("kendoTimePicker");
    var tpTo = $(timeToId).data("kendoTimePicker");
    var tpFromVal = tpFrom.value();
    var tpToVal = tpTo.value();
    var index = timeToId.replace("#timet", "");
    var enteredDate = new Date($("#date" + index).val()).toLocaleDateString();
    var todayDate = new Date().toLocaleDateString();
    /*Implement check to check time from should not be lesser than present time*/
    if (enteredDate == todayDate) {
        var presentTime = new Date().getTime();
        var tpFromTime = tpFromVal.getTime();
        if (tpFromTime < presentTime) {
            ShowMessage('Selected time must be greater than the present time!', "Warning", "warning", true);
            tpFrom.value('');
            tpTo.value('');
            return false;
        }
    }
    /****************************************************************************/
    var startTime = tpFromVal;//start.value();

    if (startTime) {
        startTime = new Date(startTime);
        //tpTo.max(startTime);
        startTime.setMinutes(startTime.getMinutes() + timeInterval);
        tpTo.min(startTime);
        tpTo.value(startTime);
    }
}

function BindPhysicianBySpeciality() {
    $.ajax({
        type: "POST",
        url: "/Physician/BindPhysicianBySpeciality",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            facilityId: $("#ddlFacility").val(),
            specialityId: $("#ddlSpeciality").val()
        }),
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, physician) {
                    items += '<option value="' + physician.Physician.Id + '" facilityId="' + physician.Physician.FacilityId + '" department="' + physician.UserDepartmentStr + '" departmentId="' + physician.Physician.FacultyDepartment + '" speciality="' + physician.UserSpecialityStr + '" specialityId="' + physician.Physician.FacultySpeciality + '">' + physician.Physician.PhysicianName + '(' + physician.UserTypeStr + ')</option>'; // Availability -  Field
                });
                $("#ddlFaculty").html(items);
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

var OnChangeIsSpecialChk = function (e, appTypeId) {
    $("#divReccurrencePopup").show();
    var ctrl = $(e);
    if (!ctrl[0].checked && ctrl.attr("type") == "checkbox") {
        $("#btnEditRecurrence" + appTypeId).hide();
        return false;
    } else {
        $("#btnEditRecurrence" + appTypeId).show();
    }
    if (ctrl.attr("statusattr") == "new") {
        $("#divReccurrencePopup .popup_frame").addClass("moveLeft");
    }
    $("#hfRecAppTypeId").val(appTypeId);
    $("#divReccurrencePopup .popup_frame").addClass("moveLeft");
};

var CancelApppointTypeReccurrence = function (from) {
    $(".departmentChk").prop('checked', false);
    $(".btnShowHide").hide();
    $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
    if ($("#hidEventParentId").val() == "" && from == "cancel") {
        $("#btnEditRecurrence" + $("#hfRecAppTypeId").val()).hide();
        $("#chkIsRec" + $("#hfRecAppTypeId").val()).prop("checked", false);
        $("#hfRecAppTypeId").val("0");
        $("#txtRecEndByDate").val("");
        $("#txtRecEveryDay").val("1");
    }
}

var DoneApppointTypeReccurrence = function () {
    var recTypeValue = $("input[name='rbtnRecType']:checked").val();
    if (recTypeValue == "daily") {
        if ($("#txtRecEveryDay").val() == "") {
            ShowMessage('Please fill Every day field!', "Warning", "warning", true);
            return false;
        }
    }
    else if (recTypeValue == "weekly") {
        var chkLength = $('#weekly').find('input:checked').length;
        if ($("#txtRecEveryWeekDays").val() == "") {
            ShowMessage('Please fill Every week(s) field!', "Warning", "warning", true);
            return false;
        }
        if (chkLength == 0) {
            ShowMessage('Please check atleast one day from the list!', "Warning", "warning", true);
            return false;
        }
    }
    else if (recTypeValue == "monthly") {
        var recRepeatMonthValue1 = $("input[name='rbtnRepeatMonth']:checked").val();
        if (recRepeatMonthValue1 == "1" && $("#txtRepeatMonthDay").val() == "") {
            ShowMessage('Please fill Repeat day field!', "Warning", "warning", true);
            return false;
        }
    }
    
    $.each(apptRecArray, function (i, val) {
        $.each(val, function (key, name) {
            if (name == $("#hfRecAppTypeId").val()) {
                apptRecArray.splice(i, 1);
            }
        });
        return false;//return false is used to break the loop
    });
    var recObject = new Object();
    recObject.appoint_Type_Id = $("#hfRecAppTypeId").val();
    //var recTypeObj = $("input[name='rbtnRecType']");

    //for (var i = 0; i < recTypeObj.length ; i++) {
    //    if (recTypeObj[i].checked) {
    //        recTypeValue = recTypeObj[i].value;
    //    }
    //}
    // 
    var apptStartDate = $("#datef" + $("#hfRecAppTypeId").val()).val();

    switch (recTypeValue) {
        case "daily":
            var recEndDate = new Date($("#txtRecEndByDate").val());
            //recEndDate.setDate(recEndDate.getDate() + 1);
            recEndDate.setDate(recEndDate.getDate());
            var recStartDateDaily = new Date(apptStartDate);
            recStartDateDaily.setDate(recStartDateDaily.getDate());
            recObject.Rec_Pattern = "day_" + $("#txtRecEveryDay").val() + "__";
            recObject.Rec_Type = "day_" + $("#txtRecEveryDay").val() + "__#";
            recObject.end_By = recEndDate;//$("#txtRecEndByDate").val();
            recObject.Rec_Start_Date = recStartDateDaily;
            recObject.Frequency_Type = 1;//1 is for daily selected radio button
            break;
        case "weekly":
            var d = new Date($("#datef" + $("#hfRecAppTypeId").val()).val());
            var n = d.getDay();
            var recStartDateWeekly = new Date(apptStartDate);
            recStartDateWeekly.setDate(recStartDateWeekly.getDate());
            //var weeklyDaysChk = $(".dhx_repeat_checkbox");
            var weeklyDayChk = $("input[name='week_day']");
            var selectedWeekDays = "";
            if (weeklyDayChk.length > 0) {
                for (var i = 0; i < weeklyDayChk.length ; i++) {
                    if (weeklyDayChk[i].checked) {
                        selectedWeekDays += weeklyDayChk[i].value + ",";
                    }
                }
                //selectedWeekDays = selectedWeekDays.replace(/,\s*$/, "");//To remove last comma from the string
                selectedWeekDays = selectedWeekDays.replace(/,\s*$/, "").split(',').sort().join(',');
            }
            if (selectedWeekDays == "") {
                recObject.Rec_Pattern = "week_" + $("#txtRecEveryWeekDays").val() + "___" + n;
                recObject.Rec_Type = "week_" + $("#txtRecEveryWeekDays").val() + "___" + n + "#";
            } else {
                recObject.Rec_Pattern = "week_" + $("#txtRecEveryWeekDays").val() + "___" + selectedWeekDays;
                recObject.Rec_Type = "week_" + $("#txtRecEveryWeekDays").val() + "___" + selectedWeekDays + "#";
            }
            recObject.end_By = $("#txtRecEndByDate").val();
            recObject.Rec_Start_Date = recStartDateWeekly;
            recObject.Frequency_Type = 2;//2 is for weekly selected radio button
            break;
        case "monthly":
            var recRepeatMonthValue = $("input[name='rbtnRepeatMonth']:checked").val();
            var dateObj = new Date();
            var month = dateObj.getUTCMonth();
            var year = dateObj.getUTCFullYear();
            var hours = $("#timef" + $("#hfRecAppTypeId").val()).val().split(':')[0];
            var minutes = $("#timef" + $("#hfRecAppTypeId").val()).val().split(':')[1];
            switch (recRepeatMonthValue) {
                case "1":
                    var date = new Date(year, month, parseInt($("#txtRepeatMonthDay").val()));
                    date.setHours(hours);
                    date.setMinutes(minutes);
                    recObject.Rec_Pattern = "month_" + $("#ddEveryMonthDay").val() + "___";
                    recObject.Rec_Type = "month_" + $("#ddEveryMonthDay").val() + "___#";
                    recObject.Rec_Start_Date = date;//new Date(year, month, parseInt($("#txtRepeatMonthDay").val()));
                    recObject.Frequency_Type = 3;//3 is for Monthly selected radio button & Repeat selected radio button
                    break;
                case "2":
                    var recStartDateMonthly = new Date(apptStartDate);
                    recStartDateMonthly.setDate(recStartDateMonthly.getDate());
                    recStartDateMonthly.setHours(hours);
                    recStartDateMonthly.setMinutes(minutes);
                    recObject.Rec_Pattern = "month_" + $("#ddOnEveryMonth").val() + "_" + $("#ddMonthWeekDays").val() + "_" + $("#ddOnRepeatMonth").val() + "_";
                    recObject.Rec_Type = "month_" + $("#ddOnEveryMonth").val() + "_" + $("#ddMonthWeekDays").val() + "_" + $("#ddOnRepeatMonth").val() + "_#";
                    recObject.Rec_Start_Date = recStartDateMonthly;
                    recObject.Frequency_Type = 4;//4 is for Monthly selected radio button & On selected radio button
                    break;
                default:
                    break;
            }
            recObject.end_By = $("#txtRecEndByDate").val();
            break;
        default:
            recObject.Rec_Pattern = "";
            recObject.Rec_Type = "";
            recObject.end_By = "";
            recObject.Rec_Start_Date = "";
            recObject.Frequency_Type = 0;
            break;
    }
    $("#btnEditRecurrence" + $("#hfRecAppTypeId").val()).show();
    apptRecArray.push(recObject);
    CancelApppointTypeReccurrence('done');
    $("#chkIsRec" + $("#hfRecAppTypeId").val()).prop("checked", true);
    $(".btnShowHide").show();
    return false;
}

var OnchangeRecurrenceCtrl = function (e) {
    
    var ctrl = e;
    var aptDate = $("#datef" + $("#hfRecAppTypeId").val()).val();
    var recEndDate = new Date(aptDate);
    switch (ctrl) {
        case "txtRecEveryDay":
            var txtRecEveryDay = $("#txtRecEveryDay").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryDay == "" ? 0 : txtRecEveryDay));
            $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
            break;
        case "txtRecEveryWeekDays":
            var txtRecEveryWeekDays = $("#txtRecEveryWeekDays").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryWeekDays == "" ? 0 : txtRecEveryWeekDays * 7));
            $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
            break;
        case "ddEveryMonthDay":
        case "ddOnEveryMonth":
            var recRepeatMonthValue = $("input[name='rbtnRepeatMonth']:checked").val();
            switch (recRepeatMonthValue) {
                case "1":
                    var ddEveryMonthDay = $("#ddEveryMonthDay").val();
                    recEndDate.setDate(recEndDate.getDate() + parseInt(ddEveryMonthDay == "" ? 0 : ddEveryMonthDay * 30));
                    $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
                    break;
                case "2":
                    var ddOnEveryMonth = $("#ddOnEveryMonth").val();
                    recEndDate.setDate(recEndDate.getDate() + parseInt(ddOnEveryMonth == "" ? 0 : ddOnEveryMonth * 30));
                    $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
                    break;
            }
            break;
        default:
            break;
    }
}

function RemoveAppointmentProcedures(apptId) {
    var selectedTypes = $("#hfAppointmentTypes").val();
    var selectedTypesArray = selectedTypes.split(',');
    $.each(apptRecArray, function (i, val) {
        $.each(val, function (key, name) {
            if (name == apptId) {
                apptRecArray.splice(i, 1);
            }
        });
    });

    $.each(selectedTypesArray, function (i) {
        if (selectedTypesArray[i] === apptId.toString()) {
            selectedTypesArray.splice(i, 1);
            return false;
        }
    });
    //selectedTypesArray.remove(apptId);
    var removeAppTypes = $("#hfRemovedAppTypes").val();
    removeAppTypes += "," + $("#hfMain" + apptId).val();
    $("#hfRemovedAppTypes").val(removeAppTypes.replace(/^,/, ''));
    $("#hfAppointmentTypes").val(selectedTypesArray.join(','));
    $("#tr" + apptId).remove();
}

var OnClickRecurrenceTypeBtn = function (e) {
    var ctrl = $(e)[0];
    var aptDate = $("#datef" + $("#hfRecAppTypeId").val()).val();
    var recEndDate = new Date(aptDate);
    switch (ctrl.id) {
        case "rbtnDailyRecType":
            var txtRecEveryDay = $("#txtRecEveryDay").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryDay == "" ? 0 : txtRecEveryDay));
            $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
            break;
        case "rbtnWeeklyRecType":
            var txtRecEveryWeekDays = $("#txtRecEveryWeekDays").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryWeekDays == "" ? 0 : txtRecEveryWeekDays * 7));
            $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
            break;
        case "rbtnMonthlyRecType":
            var recRepeatMonthValue = $("input[name='rbtnRepeatMonth']:checked").val();
            switch (recRepeatMonthValue) {
                case "1":
                    var ddEveryMonthDay = $("#ddEveryMonthDay").val();
                    recEndDate.setDate(recEndDate.getDate() + parseInt(ddEveryMonthDay == "" ? 0 : ddEveryMonthDay * 30));
                    $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
                    break;
                case "2":
                    var ddOnEveryMonth = $("#ddOnEveryMonth").val();
                    recEndDate.setDate(recEndDate.getDate() + parseInt(ddOnEveryMonth == "" ? 0 : ddOnEveryMonth * 30));
                    $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
                    break;
            }
            break;
    }
    $("#txtRecEndByDate").datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        timepicker: false,
        closeOnDateSelect: true
    });
}

var SaveFacultyTimmingData = function () {
    var isValid = jQuery("#divFacultyTimming").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var jsonData = [];
        var selectedAppointmenttype = $('#hfAppointmentTypes').val();
        var appointmenttypeArray = selectedAppointmenttype.split(',');
        if (appointmenttypeArray.length > 0) {
            for (var i = 0; i < appointmenttypeArray.length; i++) {
                var appointmenttypeobj = appointmenttypeArray[i];
                var isRecurranceMultiple = $('#chkIsRec' + appointmenttypeobj).prop('checked');
                var recuranceArrayObj = null;
                $.each(apptRecArray, function (k1, val) {
                    $.each(val, function (key, name) {
                        if (name == appointmenttypeobj) {
                            recuranceArrayObj = apptRecArray[k1];
                        }
                    });
                });
                jsonData[i] = {
                    Id: $('#hfMain' + appointmenttypeobj).val(),
                    FacultyId: $('#ddlFaculty').val(),
                    FacultyType: 0,
                    DeptId: $('#hfDepartmentid' + appointmenttypeobj).val(),
                    AvailabilityType: isRecurranceMultiple ? null : '1',
                    Pattern: isRecurranceMultiple ? recuranceArrayObj.Rec_Pattern : '',
                    WorkingDay: 0,
                    WeekNumber: 0,
                    FromDate: ($('#datef' + appointmenttypeobj).val() + ' ' + $('#timef' + appointmenttypeobj).val()),
                    ToDate: ($('#datef' + appointmenttypeobj).val() + ' ' + $('#timet' + appointmenttypeobj).val()),
                    FacilityId: $('#ddlFacility').val(),
                    CorporateId: $('#ddlCorporate').val(),
                    ExtValue1: '2',
                    ExtValue3: 'Simple',
                    RecurreanceDateFrom: isRecurranceMultiple ? recuranceArrayObj.Rec_Start_Date : ($('#datef' + appointmenttypeobj).val() + ' ' + $('#timef' + appointmenttypeobj).val()),
                    RecurreanceDateTo: isRecurranceMultiple ? recuranceArrayObj.end_By : ($('#datef' + appointmenttypeobj).val() + ' ' + $('#timet' + appointmenttypeobj).val()),
                };
            }
            var jsonD = JSON.stringify(jsonData);
            $.ajax({
                type: "POST",
                url: '/FacultyRooster/SaveFacultyTimmingData',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonD,
                success: function (data) {
                    if (data.errorLog > 0) {
                        $.each(data.duplicateEntryLog, function (i, reasonData) {
                            ShowMessage(reasonData.Reason, "Warning", "warning", true);
                        });
                    } else {
                        ShowMessage("Record Saved successfully!", "Success", "success", true);
                        BindFacultyRoosterListByFacility($('#ddlFacility').val());
                        //$('#FacultyRoosterListDiv').html(data);
                        $('#hfAppointmentTypes').val('');
                        $("#divPhysicianPatient").clearForm();
                        $("#tbDepartmentTimmingList").empty();
                        apptRecArray = [];//Clear reccurrence array
                        $('.editdisabled').removeAttr('disabled');
                    }
                },
                error: function (msg) {
                    ShowMessage("Unable to save the record!", "Warning", "warning", true);
                }
            });
        }
    }
}

var SetDepartmentAndSpeciality = function (e) {
    var spec = $(e).find('option:selected').attr("specialityId");
    var fid = $(e).find('option:selected').attr("facilityId");
    $("#ddlSpeciality").val(spec);
    $("#ddlFacility").val(fid);
};

function BindPhysiciansAndDepartmentsInFacultyTiming() {
    $.ajax({
        type: "POST",
        url: "/Home/GetDepartmentsByFacility",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            facilityId: $("#ddlFacility").val()
        }),
        success: function (data) {
            
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data.deptList, function (i, deaprtments) {
                    items += "<option value='" + deaprtments.Value + "'>" + deaprtments.Text + "</option>";
                });
                $("#ddlDepartmentType").html(items);
                items = '<option value="0">--Select--</option>';
                $.each(data.phyList, function (i, physician) {
                    items += '<option value="' + physician.Physician.Id + '" facilityId="' + physician.Physician.FacilityId + '" department="' + physician.UserDepartmentStr + '" departmentId="' + physician.Physician.FacultyDepartment + '" speciality="' + physician.UserSpecialityStr + '" specialityId="' + physician.Physician.FacultySpeciality + '">' + physician.Physician.PhysicianName + '(' + physician.UserTypeStr + ')</option>'; // Availability -  Field
                });
                $("#ddlFaculty").html(items);
            }
        },
        error: function (msg) {
        }
    });
}

var AddDepartmentTimmingObj = function (obj) {
    $('.editdisabled').attr('disabled', 'disabled');
    $("#tbDepartmentTimmingList").empty();
    var html = "";
    html += "<tr id='tr" + obj.DeptId + "'><input type='hidden' id='hfMain" + obj.DeptId + "' value='" + obj.Id + "'/>";
    html += "<td>" + $('#ddlDepartmentType :selected').text() + "</td><input type='hidden' id='hfDepartmentid" + obj.DeptId + "'  value='" + obj.DeptId + "'/>";
    html += "<td><input id='datef" + obj.DeptId + "' type='text' class='validate[required]' value='" + obj.FromDateStr + "'/></td>";
    //html += "<td><input id='datet" + obj.DeptId + "' type='text' class='validate[required]' value='" + obj.ToDateStr + "'/></td>";
    html += "<td><input id='timef" + obj.DeptId + "' type='text' class='validate[required]' value='" + obj.FromTimeStr + "'/></td>";
    html += "<td><input id='timet" + obj.DeptId + "' type='text' class='validate[required]' value='" + obj.ToTimeStr + "'/></td>";
    html += "<td class='recurrence_event'><input id='chkIsRec" + obj.DeptId + "' disabled='disabled' type='checkbox' onchange='OnChangeIsSpecialChk(this," + obj.DeptId + ");'/><input type='button' id='btnEditRecurrence" + obj.DeptId + "' class='btn btn-xs btn-default btnShowHide' style='margin-left:5px;display:none;' value='Edit' statusattr='new' onclick='OnChangeIsSpecialChk(this," + obj.DeptId + ");'/></td>";
    html += "<td>&nbsp;</td>"; //"<td><img src='/images/delete.png' width='15px' onclick='RemoveAppointmentProcedures(" + obj.DeptId + ")'/></td>";
    html += "</tr>";
    $('#tbDepartmentTimmingList').append(html);

    var hfApptTypes = $("#hfAppointmentTypes").val();
    var concatApptTypes = hfApptTypes + "," + obj.DeptId;
    $("#hfAppointmentTypes").val(concatApptTypes.replace(/^,|,$/g, ''));

    $("#datef" + obj.DeptId).datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        timepicker: false,
        closeOnDateSelect: true
    });
    $("#datet" + obj.DeptId).datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        timepicker: false,
        closeOnDateSelect: true
    });
    $("#timef" + obj.DeptId).kendoTimePicker({
        format: "HH:mm",
        change: function (e) {
            startChange("#timef" + obj.DeptId, "#timet" + obj.DeptId, parseInt(30));
        }
    }).data("kendoTimePicker");
    $("#timet" + obj.DeptId).kendoTimePicker({
        format: "HH:mm"
    }).data("kendoTimePicker");
}

var GetDeptOpeningDays = function (deptId) {
    $.ajax({
        type: "POST",
        url: "/Home/GetDepartmentTiming",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            deptId: deptId
        }),
        success: function (data) {
            $('#hfDeptOpeningDays' + deptId).val(data.deptOpeningDays);
        },
        error: function (msg) {
        }
    });
}

var ValidateOpeningData = function (e) {
    var ctrl = $(e);
    var selectedDate = new Date(ctrl.val()).getDay();
    var deptId = $(e).attr('id').slice(5);
    var openingDays = $('#hfDeptOpeningDays' + deptId).val();
    if (openingDays === undefined || openingDays === '') {
        ShowMessage("Department Timings not available!", "Warning", "warning", true);
        $('#timef' + deptId).attr('disabled', 'disabled').attr('title', 'Dept Timming not available on selected day');
        $('#timet' + deptId).attr('disabled', 'disabled').attr('title', 'Dept Timming not available on selected day');
        $('#timef' + deptId).data('kendoTimePicker').enable(false);
        $('#timet' + deptId).data('kendoTimePicker').enable(false);
    } else {
        if (openingDays.indexOf(selectedDate) != -1) {
            $('#timef' + deptId).removeAttr('disabled').attr('title', '');
            $('#timet' + deptId).removeAttr('disabled').attr('title', '');
            $('#timef' + deptId).data('kendoTimePicker').enable(true);
            $('#timet' + deptId).data('kendoTimePicker').enable(true);
        } else {
            $('#timef' + deptId).attr('disabled', 'disabled').attr('title', 'Dept Timming not available on selected day');
            $('#timet' + deptId).attr('disabled', 'disabled').attr('title', 'Dept Timming not available on selected day');
            $('#timef' + deptId).data('kendoTimePicker').enable(false);
            $('#timet' + deptId).data('kendoTimePicker').enable(false);
        }
    }
}

var BindFacultyRoosterListByFacility = function (fid) {
    $.ajax({
        type: "POST",
        url: "/FacultyRooster/BindFacultyRoosterListByFacility",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({
            facilityId: fid
        }),
        success: function (data) {
            $('#FacultyRoosterListDiv').empty().html(data);
        },
        error: function (msg) {
        }
    });
}


//var DoneApppointTypeReccurrence = function () {
//    var recTypeValue = $("input[name='rbtnRecType']:checked").val();
//    if (recTypeValue == "daily") {
//        if ($("#txtRecEveryDay").val() == "") {
//            ShowMessage('Please fill Every day field!', "Warning", "warning", true);
//            return false;
//        }
//    }
//    else if (recTypeValue == "weekly") {
//        var chkLength = $('#weekly').find('input:checked').length;
//        if ($("#txtRecEveryWeekDays").val() == "") {
//            ShowMessage('Please fill Every week(s) field!', "Warning", "warning", true);
//            return false;
//        }
//        if (chkLength == 0) {
//            ShowMessage('Please check atleast one day from the list!', "Warning", "warning", true);
//            return false;
//        }
//    }
//    else if (recTypeValue == "monthly") {
//        var recRepeatMonthValue1 = $("input[name='rbtnRepeatMonth']:checked").val();
//        if (recRepeatMonthValue1 == "1") {
//            ShowMessage('Please fill Repeat day field!', "Warning", "warning", true);
//            return false;
//        }
//    }
//    
//    $.each(apptRecArray, function (i, val) {
//        
//        $.each(val, function (key, name) {
//            
//            if (name == $("#hfRecAppTypeId").val()) {
//                apptRecArray.splice(i, 1);
//            }
//        });
//        return false;//return false is used to break the loop
//    });
//    var recObject = new Object();
//    recObject.appoint_Type_Id = $("#hfRecAppTypeId").val();
//    //var recTypeObj = $("input[name='rbtnRecType']");

//    //for (var i = 0; i < recTypeObj.length ; i++) {
//    //    if (recTypeObj[i].checked) {
//    //        recTypeValue = recTypeObj[i].value;
//    //    }
//    //}
//    // 
//    var apptStartDate = $("#datef" + $("#hfRecAppTypeId").val()).val();

//    switch (recTypeValue) {
//        case "daily":
//            var recEndDate = new Date($("#txtRecEndByDate").val());
//            //recEndDate.setDate(recEndDate.getDate() + 1);
//            recEndDate.setDate(recEndDate.getDate());
//            var recStartDateDaily = new Date(apptStartDate);
//            recStartDateDaily.setDate(recStartDateDaily.getDate());
//            recObject.Rec_Pattern = "day_" + $("#txtRecEveryDay").val() + "__";
//            recObject.Rec_Type = "day_" + $("#txtRecEveryDay").val() + "__#";
//            recObject.end_By = recEndDate;//$("#txtRecEndByDate").val();
//            recObject.Rec_Start_Date = recStartDateDaily;
//            recObject.Frequency_Type = 1;//1 is for daily selected radio button
//            break;
//        case "weekly":
//            var d = new Date($("#date" + $("#hfRecAppTypeId").val()).val());
//            var n = d.getDay();
//            var recStartDateWeekly = new Date(apptStartDate);
//            recStartDateWeekly.setDate(recStartDateWeekly.getDate());
//            //var weeklyDaysChk = $(".dhx_repeat_checkbox");
//            var weeklyDayChk = $("input[name='week_day']");
//            var selectedWeekDays = "";
//            if (weeklyDayChk.length > 0) {
//                for (var i = 0; i < weeklyDayChk.length ; i++) {
//                    if (weeklyDayChk[i].checked) {
//                        selectedWeekDays += weeklyDayChk[i].value + ",";
//                    }
//                }
//                //selectedWeekDays = selectedWeekDays.replace(/,\s*$/, "");//To remove last comma from the string
//                selectedWeekDays = selectedWeekDays.replace(/,\s*$/, "").split(',').sort().join(',');
//            }
//            if (selectedWeekDays == "") {
//                recObject.Rec_Pattern = "week_" + $("#txtRecEveryWeekDays").val() + "___" + n;
//                recObject.Rec_Type = "week_" + $("#txtRecEveryWeekDays").val() + "___" + n + "#";
//            } else {
//                recObject.Rec_Pattern = "week_" + $("#txtRecEveryWeekDays").val() + "___" + selectedWeekDays;
//                recObject.Rec_Type = "week_" + $("#txtRecEveryWeekDays").val() + "___" + selectedWeekDays + "#";
//            }
//            recObject.end_By = $("#txtRecEndByDate").val();
//            recObject.Rec_Start_Date = recStartDateWeekly;
//            recObject.Frequency_Type = 2;//2 is for weekly selected radio button
//            break;
//        case "monthly":
//            var recRepeatMonthValue = $("input[name='rbtnRepeatMonth']:checked").val();
//            var dateObj = new Date();
//            var month = dateObj.getUTCMonth();
//            var year = dateObj.getUTCFullYear();
//            var hours = $("#timef" + $("#hfRecAppTypeId").val()).val().split(':')[0];
//            var minutes = $("#timef" + $("#hfRecAppTypeId").val()).val().split(':')[1];
//            switch (recRepeatMonthValue) {
//                case "1":
//                    var date = new Date(year, month, parseInt($("#txtRepeatMonthDay").val()));
//                    date.setHours(hours);
//                    date.setMinutes(minutes);
//                    recObject.Rec_Pattern = "month_" + $("#ddEveryMonthDay").val() + "___";
//                    recObject.Rec_Type = "month_" + $("#ddEveryMonthDay").val() + "___#";
//                    recObject.Rec_Start_Date = date;//new Date(year, month, parseInt($("#txtRepeatMonthDay").val()));
//                    recObject.Frequency_Type = 3;//3 is for Monthly selected radio button & Repeat selected radio button
//                    break;
//                case "2":
//                    var recStartDateMonthly = new Date(apptStartDate);
//                    recStartDateMonthly.setDate(recStartDateMonthly.getDate());
//                    recStartDateMonthly.setHours(hours);
//                    recStartDateMonthly.setMinutes(minutes);
//                    recObject.Rec_Pattern = "month_" + $("#ddOnEveryMonth").val() + "_" + $("#ddMonthWeekDays").val() + "_" + $("#ddOnRepeatMonth").val() + "_";
//                    recObject.Rec_Type = "month_" + $("#ddOnEveryMonth").val() + "_" + $("#ddMonthWeekDays").val() + "_" + $("#ddOnRepeatMonth").val() + "_#";
//                    recObject.Rec_Start_Date = recStartDateMonthly;
//                    recObject.Frequency_Type = 4;//4 is for Monthly selected radio button & On selected radio button
//                    break;
//                default:
//                    break;
//            }
//            //var recStartDateMonthly = new Date(apptStartDate);
//            //recStartDateMonthly.setDate(recStartDateMonthly.getDate() + 1);
//            //recObject.Rec_Start_Date = recStartDateMonthly;

//            recObject.end_By = $("#txtRecEndByDate").val();
//            break;
//        default:
//            recObject.Rec_Pattern = "";
//            recObject.Rec_Type = "";
//            recObject.end_By = "";
//            recObject.Rec_Start_Date = "";
//            recObject.Frequency_Type = 0;
//            break;
//    }
//    $("#btnEditRecurrence" + $("#hfRecAppTypeId").val()).show();
//    apptRecArray.push(recObject);
//    CancelApppointTypeReccurrence('done');
//    $("#chkIsRec" + $("#hfRecAppTypeId").val()).prop("checked", true);
//    $(".btnShowHide").show();
//    return false;
//}


function BindDataOnLoad() {
    $.ajax({
        type: "POST",
        url: "/FacultyRooster/BindDataOnPageLoad",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            BindDropdownData(data.cList, "#ddlCorporate", data.cId);
            BindDropdownData(data.specialties, "#ddlSpeciality", "");
            BindDropdownData(data.flist, "#ddlFacility", data.fId);

            BindPhysiciansDataInFaultyTiming(data.pList, "#ddlFaculty", "");
            BindDropdownData(data.listDepartments, "#ddlDepartmentType", "");
            //Bind Monthly Week Days
            BindDropdownData(data.mWeekDays, "#ddMonthWeekDays", "");
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}


function BindPhysiciansDataInFaultyTiming(data, ddlSelector, hdSelector) {
    /// <summary>
    /// Binds the dropdown data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="ddlSelector">The DDL selector.</param>
    /// <param name="hdSelector">The hd selector.</param>
    /// <returns></returns>

    $(ddlSelector).empty();
    var items = '<option value="0">--Select--</option>';
    $.each(data, function (i, obj) {
        var newItem = '<option value="' + obj.Physician.Id + '" facilityId="' + obj.Physician.FacilityId + '" department="' + obj.UserDepartmentStr + '" departmentId="' + obj.Physician.FacultyDepartment + '" speciality="' + obj.UserSpecialityStr + '" specialityId="' + obj.Physician.FacultySpeciality + '">' + obj.Physician.PhysicianName + '(' + obj.UserTypeStr + ')</option>';
        items += newItem;
    });

    $(ddlSelector).html(items);
    var hdValue = "";
    if (hdSelector.indexOf('#') != -1) {
        hdValue = $(hdSelector).val();
    }
    else {
        hdValue = hdSelector;
    }
    //
    if (hdValue != null && hdValue != '') {
        $(ddlSelector).val(hdValue);
        if ($(ddlSelector).val() == null || $(ddlSelector).val() == undefined) {
            $(ddlSelector + " option").filter(function (index) { return $(this).text() === "" + hdValue + ""; }).attr('selected', 'selected');
        }
    }
    else {
        if ($(ddlSelector).length > 0)
            $(ddlSelector)[0].selectedIndex = 0;
    }
}