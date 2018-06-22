var pageUrl = "/FacultyRooster/";

$(function () {
    BindDataOnLoadInClinicalRoster();

    $('#ddlCorporate').on('change', function () {
        BindFacilityByCoporateId();
    });

    $('#ddlFacility').on('change', function () {
        GetPhysiciansInCR($(this).val());
    });

    $('#Current_DateFrom').on('change', function () {
        CheckTwoDates($('#Current_DateFrom'), $('#Current_DateTo'), 'Current_DateFrom');
        //CheckValidDateNotLessThanTodayDate(this, 'Current_DateFrom');
    });

    $('#Current_DateTo').on('change', function () {
        CheckTwoDates($('#Current_DateFrom'), $('#Current_DateTo'), 'Current_DateTo');
        //CheckValidDateNotLessThanTodayDate(this, 'Current_DateTo');
    });

    $('#btnSave').on('click', SaveRecordCR);

    $('#btnCancel').on('click', function () {
        ClearAddEditForm('divClinicianRosterForm', 'collapseRAddEdit', 'collapseRList');
    });

    $("#Current_DateTo").datetimepicker({
        format: 'm/d/Y',
        minDate: new Date(),
        timepicker: false,
        closeOnDateSelect: true
    });

    $("#Current_DateFrom").datetimepicker({
        format: 'm/d/Y',
        minDate: new Date(),
        timepicker: false,
        closeOnDateSelect: true
    });

    $("#Current_TimeFrom").kendoTimePicker({
        format: "HH:mm",
    }).data("kendoTimePicker");

    $("#Current_TimeTo").kendoTimePicker({
        format: "HH:mm",
        animation: {
            close: {
                effects: "fadeOut zoom:out",
                duration: 300
            },
            open: {
                effects: "fadeIn zoom:in",
                duration: 300
            }
        }
    });

    function startChange() {
        var startTime = start.value();

        if (startTime) {
            startTime = new Date(startTime);

            end.max(startTime);

            startTime.setMinutes(startTime.getMinutes() + this.options.interval);

            end.min(startTime);
            end.value(startTime);
        }
    }

    //init start timepicker
    var start = $("#Current_TimeFrom").kendoTimePicker({
        format: "HH:mm",
        animation: {
            close: {
                effects: "fadeOut zoom:out",
                duration: 300
            },
            open: {
                effects: "fadeIn zoom:in",
                duration: 300
            }
        },
        change: startChange
    }).data("kendoTimePicker");

    //init end timepicker
    var end = $("#Current_TimeTo").kendoTimePicker({
        format: "HH:mm",
        animation: {
            close: {
                effects: "fadeOut zoom:out",
                duration: 300
            },
            open: {
                effects: "fadeIn zoom:in",
                duration: 300
            }
        }
    }).data("kendoTimePicker");

    //define min/max range
    start.min("8:00 AM");
    start.max("6:00 PM");

    $("#chkFullDay").change(function () {
        if ($(this).prop("checked")) {
            $("#Current_TimeFrom").removeClass("validate[required]");
            $("#spanTimeFrom").removeClass("mandatoryStar");
        }
        else {
            $("#Current_TimeFrom").addClass("validate[required]");
            $("#spanTimeFrom").addClass("mandatoryStar");
        }
    });

    $("#divClinicianRosterForm").validationEngine();

});

function BindDataOnLoadInClinicalRoster() {
    $.ajax({
        type: "POST",
        url: pageUrl + "BindDataOnPageLoadInCR",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            BindDropdownData(data.cList, "#ddlCorporate", data.cId);
            BindDropdownData(data.flist, "#ddlFacility", data.fId);
            BindDDPhysiciansInCR(data.pList, "#ddlFaculty", "");

            //Bind Reasons select picker
            BindDropdownData(data.reasons, "#ddlReason", "");
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

function SaveRecordCR() {
    //var daysOfWeek = 'ALL';
    //var weekDaysChecked = $(".weekday:checkbox:checked");

    //if (weekDaysChecked.length < 7 && weekDaysChecked.length > 0) {
    //    weekDaysChecked.each(function () {
    //        if (daysOfWeek == 'ALL')
    //            daysOfWeek = $(this).attr("id");
    //        else
    //            daysOfWeek += "," + $(this).attr("id");
    //    });
    //}

    var id = $("#Current_Id").val();
    var jsonData = JSON.stringify({
        Id: id,
        ClinicianId: $("#ddlFaculty").val(),
        ReasonId: $("#ddlReason").val(),
        Comments: '',
        RosterTypeId: '1',
        DateFrom: $("#Current_DateFrom").val(),
        TimeFrom: $("#Current_TimeFrom").val(),
        DateTo: $("#Current_DateTo").val(),
        TimeTo: $("#Current_TimeTo").val(),
        FacilityId: $("#ddlFacility").val(),
        CorporateId: $("#ddlCorporate").val(),
        RepeatitiveDaysInWeek: 'ALL',
        IsActive: 1,
        ExtValue1: $('#chkFullDay').is(':checked') ? "1" : "0",
        ExtValue2: ''
    });

    var isValid = jQuery("#divClinicianRosterForm").validationEngine({ returnIsValid: true });

    if (isValid) {
        $.ajax({
            type: "POST",
            url: pageUrl + "SaveRecordCR",
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (checkNumeric(data.replace(/[^a-z0-9\s]/gi, '').replace(/[_\s]/g, ' '))) {
                    RebindClinicianRosterList(true);
                    var msg = "Records Saved successfully !";
                    if (id > 0)
                        msg = "Records updated successfully";
                    ClearAddEditForm('divClinicianRosterForm', 'collapseRAddEdit', 'collapseRList');
                    ShowMessage(msg, "Success", "success", true);
                }
                else {
                    ShowMessage(data, "Error", "error", true);
                }
            },
            error: function (msg, status, ss) {
                ShowMessage(status, "Error", "error", true);
            }
        });
    }
}

function EditCurrentRecordCR(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: pageUrl + "GetSingleClinicianR",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindDataInEditCR(data);
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

function DeleteCurrentCR() {

    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val(),
            aStatus: true
        });
        $.ajax({
            type: "POST",
            url: pageUrl + "DeleteRecordCR",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {

                if (data) {
                    BindList("#divClinicianRosterList", data);
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                }
                else {
                    ShowMessage("Error while Deleting the Record!", "Error", "error", true);
                }
            },
            error: function (msg) {

                console.log(msg);
                return true;
            }
        });
    }
}

function RebindClinicianRosterList(aStatus) {
    var jsonData = JSON.stringify({
        fId: $("#ddlFacility").val(),
        cId: $("#ddlCorporate").val(),
        aStatus: aStatus
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: pageUrl + "RebindClinicianRosterList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            BindList("#divClinicianRosterList", data);
        },
        error: function (msg) {
            console.log(msg);

        }

    });
}

function BindDDPhysiciansInCR(data, ddlSelector, hdSelector) {
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
        var newItem = '<option value="' + obj.Id + '" facilityId="' + obj.FacilityId + '" department="' + obj.UserDepartmentStr + '" departmentId="' + obj.FacultyDepartment + '" speciality="' + obj.UserSpecialityStr + '" specialityId="' + obj.FacultySpeciality + '">' + obj.ClinicianName + '</option>';
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

function BindDataInEditCR(data) {
    $('#ddlCorporate').val(data.CorporateId);
    $('#ddlFacility').val(data.FacilityId);
    $('#ddlFaculty').val(data.ClinicianId);
    $('#Current_DateFrom').val(data.DFrom);
    $('#Current_DateTo').val(data.DTo);
    $('#Current_TimeFrom').val(data.TimeFrom);
    $('#Current_TimeTo').val(data.TimeTo);
    $('#ddlReason').val(data.ReasonId);
    $('#Current_TimeTo').val(data.TimeTo);
    $('#collapseRAddEdit').addClass('in');
    $("#Current_Id").val(data.Id);

    $('#chkFullDay').prop("checked", data.ExtValue1 == 1);

    //if (data.RepeatitiveDaysInWeek != '' && data.RepeatitiveDaysInWeek != 'All') {
    //    var arrDays = data.RepeatitiveDaysInWeek.split(',');
    //    for (var i = 0; i < arrDays.length; i++) {
    //        $("#" + arrDays[i]).prop("checked", true);
    //    }
    //}
    //else {
    //    var weekdays = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    //    weekdays.each(function () {
    //        $("#" + this).prop("checked", true);
    //    });
    //}

    if ($('#chkFullDay').prop("checked")) {
        $("#Current_TimeFrom").removeClass("validate[required]");
        $("#spanTimeFrom").removeClass("mandatoryStar");
    }
    else {
        $("#Current_TimeFrom").addClass("validate[required]");
        $("#spanTimeFrom").addClass("mandatoryStar");
    }
    $("#divClinicianRosterForm").validationEngine();
}

function GetPhysiciansInCR(fId) {
    if (fId > 0) {
        $.getJSON(pageUrl + "GetPhysiciansByFacility", { fId: fId }, function (data) {
            if (data != null) {
                BindDDPhysiciansInCR(data, "#ddlFaculty", "");
            }
        });
    }
    else {
        BindDropdownData(null, "#ddlFaculty", "");
    }
}

function CheckModelStateValid() {
    //CheckTwoDates($('#Insurance_Startdate'), $('#Insurance_Expirydate'), 'Insurance_Startdate')
    //CheckValidDateNotLessThanTodayDate(this,'CurrentPatient_PatientInfo_PersonEmiratesIDExpiration')
    var isValid = $("#divClinicianRosterForm").validationEngine({ returnIsValid: true });
    if (isValid) {
        if ($(".weekday:checkbox:checked").length == 0) {
            OpenConfirmPopup(0, 'Days of Week Not Selected!', 'This action will take ALL Days into consideration. Continue!', SaveRecordCR, null);
        }
        else
            SaveRecordCR();
    }
}

function checkNumeric(str) { return str.length == 0 ? false : !isNaN(str) }

