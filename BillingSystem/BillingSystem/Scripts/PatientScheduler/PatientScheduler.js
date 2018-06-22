var firstTimeLoad = false;
$(function () {
    //Js Code to be added here
    LoadFacilityTimeslotsData();
    $('#btnClearFacultyTimeslots').on('click', function () {
        $("#PatientScheduleDiv").clearForm(true);
        $("#divPatientCalenderView").empty();
        $.validationEngine.closePrompt(".formError", true);
    });
    $('#ddlCorporate').on('change', function () {
        BindFacilityByCoporateId();
    });
    $('#ddlFacility').on('change', function () {
        BindUsersTypeDDL();
    });
    $('#ddlUserType').on('change', function () {
        BindUsersDDL();
    });

    $('#btnViewCalender').on('click', function () {
        //----Ajax call to get the Calender view
        var isValid = jQuery("#PatientScheduleDiv").validationEngine({ returnIsValid: true });
        if (isValid == true) {
            var jsonData = JSON.stringify({
                corporateId: $('#ddlCorporate').val(),
                facilityId: $('#ddlFacility').val(),
                userType: $('#ddlUserType').val(),
                userid: $('#ddlUser').val(),
                patientId: $("#hfPatientId").val()
            });
            $.ajax({
                type: "POST",
                url: '/PatientScheduler/GetCalender',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonData,
                success: function (data) {
                    $("#divPatientCalenderView").show();
                    $("#hfPhysicianSpeciality").val(data.PhysicianSpeciality);
                    $("#hfPhysicianLocation").val(data.PhysicianLocation);
                    /*if (!firstTimeLoad) {
                        SchedulerInit(data);
                    }*/
                    BindDataInScheduler(data);
                    scheduler.clearAll();
                    scheduler.parse(data.SchPatientSchedulingDataList, "json");
                    scheduler.updateView();
                },
                error: function (msg) {
                }
            });
        }
    });

    //-------------Added for Super Powers functionality-------------///
    var patientId = $("#hfPatientId").val();

    $("#GlobalPatientId").val(patientId);
    BindLinkUrlsForSuperPowers();
    //-------------Added for Super Powers functionality-------------///

    SchedulerInit();
});

var LoadFacilityTimeslotsData = function () {
    //BindFacilityByCoporateId();
    $("#PatientScheduleDiv").validationEngine();
    BindCorporates("#ddlCorporate", "#hdCorporateId");
};

function BindUsersDDL() {
    /// <summary>
    /// Binds the users DDL.
    /// </summary>
    /// <returns></returns>
    var usertypeVal = $('#ddlUserType').val();
    var jsonData = JSON.stringify({
        usertypeId: usertypeVal
    });

    $.ajax({
        type: "POST",
        url: "/Physician/BindUserOnUserRoleSelection",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                BindDropdownData(data, "#ddlUser", "#hdUserID");
            }
        },
        error: function (msg) {
        }
    });
}

var SaveTimeSlotData = function (id, ev) {
    var jsonData = [];
    if (ev._timed === true) {
        var recurancedates = scheduler.getRecDates(id);
        if (recurancedates.length > 0) {
            for (var i = 0; i < recurancedates.length; i++) {
                jsonData[i] = {
                    SchedulingId: ev.TimeSlotId,
                    PatientId: $("#hfPatientId").val(),
                    ScheduleFrom: recurancedates[i].start_date,
                    ScheduleTo: recurancedates[i].end_date,
                    PhysicianId: $("#ddlUser").val(),
                    TypeOfVisit: ev.VisitType,
                    //ScheduleDate: '',
                    PhysicianSpeciality: $("#hfPhysicianSpeciality").val(),
                    //TypeOfProcedure: '',
                    FacilityId: $("#ddlFacility").val(),
                    CorporateId: $('#ddlCorporate').val(),
                    Comments: ev.text,
                    Location: $("#hfPhysicianLocation").val(),
                    ConfirmedByPatient: "1",//set it by default
                    IsRecurring: ev._timed,
                    RecurringDateFrom: ev.start_date,
                    RecurringDateTill: ev.end_date,
                    EventId: id,
                    RecType: ev.rec_type,
                    RecPattern: ev.rec_pattern,
                    RecEventlength: ev.event_length,
                    RecEventPId: ev.event_pid,
                    WeekDay: ''
                };
            }
        }
    } else {
        jsonData[0] = ({
            SchedulingId: ev.TimeSlotId,
            PatientId: $("#hfPatientId").val(),
            ScheduleFrom: ev.start_date,
            ScheduleTo: ev.end_date,
            PhysicianId: $("#ddlUser").val(),
            TypeOfVisit: ev.VisitType,
            //ScheduleDate: '',
            PhysicianSpeciality: $("#hfPhysicianSpeciality").val(),
            //TypeOfProcedure: '',
            FacilityId: $("#ddlFacility").val(),
            CorporateId: $('#ddlCorporate').val(),
            Comments: ev.text,
            Location: $("#hfPhysicianLocation").val(),
            ConfirmedByPatient: "1",//set it by default
            IsRecurring: ev._timed,
            RecurringDateFrom: ev.start_date,
            RecurringDateTill: ev.end_date,
            EventId: id,
            RecType: ev.rec_type,
            RecPattern: ev.rec_pattern,
            RecEventlength: ev.event_length,
            RecEventPId: ev.event_pid,
            WeekDay: ''
        });
    }
    var jsonD = JSON.stringify(jsonData);
    $.ajax({
        type: "POST",
        url: '/PatientScheduler/SavePatientScheduling',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonD,
        success: function (data) {
            ShowMessage('Records Saved Succesfully!', "Success", "success", true);
        },
        error: function (msg) {
        }
    });
};

function SchedulerInit(jsonData) {
    scheduler.config.multi_day = true;
    scheduler.config.mark_now = true;

    scheduler.config.xml_date = "%m-%d-%Y %H:%i";
    scheduler.xy.editor_width = 0; //disable editor's auto-size

    scheduler.config.start_on_monday = true;
    scheduler.config.time_step = 15;
    scheduler.xy.min_event_height = 21;
    scheduler.config.hour_size_px = 88;
    //scheduler.config.show_quick_info = false;

    var format = scheduler.date.date_to_str("%H:%i");
    var step = 15;

    scheduler.templates.hour_scale = function (date) {
        html = "";
        for (var i = 0; i < 60 / step; i++) {
            html += "<div style='height:21px;line-height:10px;'>" + format(date) + "</div>";
            date = scheduler.date.add(date, step, "minute");
        }
        return html;
    }

    scheduler.locale.labels.workweek_tab = "W-Week";
    scheduler.locale.labels.decade_tab = "Decade";
    scheduler.config.details_on_dblclick = true;
    scheduler.config.details_on_create = true;
    scheduler.config.scroll_hour = (new Date).getHours();
    //scheduler.config.full_day = true; // enable parameter to get full day event option on the lightbox form
    scheduler.locale.labels.section_visittype = "Type of visit";
    var visittype_select_options = [
        { key: '0', label: "---Select---" },
        { key: '1', label: "ERPatient" },
        { key: '2', label: "InPatient" },
        { key: '3', label: "OutPatient" }
    ];

    var parent_onchange = function (event) {

    };

    scheduler.attachEvent("onViewChange", function (new_mode, new_date, id) {
        var tt = scheduler._mode;
        var minDate = scheduler.getState().min_date;
        if (firstTimeLoad) {
            firstTimeLoad = false;
            GetPatientScheduleViaWeek(minDate);
        }
    });

    scheduler.templates.event_class = function (start, end, event) {

        var css = "";
        if (event.facilityapproved == "1") // if event has subject property then special class should be assigned
        {
            css = "patient_confirmed";
        }
        else if (event.facilityapproved == "0") {
            css = "patient_cancelled_notrefilled";
        }
        return css; // default return

        /*
         Note that it is possible to create more complex checks
         events with the same properties could have different CSS classes depending on the current view:
    
         var mode = scheduler.getState().mode;
         if(mode == "day"){
          // custom logic here
         }
         else {
          // custom logic here
         }
        */
    };

    scheduler.config.lightbox.sections = [
        { name: "description", height: 130, map_to: "text", type: "textarea", focus: true },
        { name: "visittype", height: 30, type: "select", options: visittype_select_options, map_to: "VisitType" },
        {
            name: "recurring", height: 115, type: "recurring", map_to: "rec_type",
            button: "recurring"
        },
        { name: "time", height: 72, type: "time", map_to: "auto" }
    ];

    scheduler.attachEvent("onEventAdded", function (id, ev) {

        var datesObj = scheduler.getRecDates(id);
        SaveTimeSlotData(id, ev);
        return false;
    });

    scheduler.attachEvent("onEventChanged", function (id, ev) {

        var datesObj = scheduler.getRecDates(id);
        SaveTimeSlotData(id, ev);
        return false;
    });

    scheduler.attachEvent("onEventDeleted", function (id, ev) {
        DeleteEventFromScheduler(id, ev);
    });


    scheduler.attachEvent("onTemplatesReady", function () {
        //work week
        scheduler.date.workweek_start = scheduler.date.week_start;
        scheduler.templates.workweek_date = scheduler.templates.week_date;
        scheduler.templates.workweek_scale_date = scheduler.templates.week_scale_date;
        scheduler.date.add_workweek = function (date, inc) { return scheduler.date.add(date, inc * 7, "day"); }
        scheduler.date.get_workweek_end = function (date) { return scheduler.date.add(date, 5, "day"); }
        //decade
        scheduler.date.decade_start = function (date) {
            var ndate = new Date(date.valueOf());
            ndate.setDate(Math.floor(date.getDate() / 10) * 10 + 1);
            return this.date_part(ndate);
        }
        scheduler.templates.decade_date = scheduler.templates.week_date;
        scheduler.templates.decade_scale_date = scheduler.templates.week_scale_date;
        scheduler.date.add_decade = function (date, inc) { return scheduler.date.add(date, inc * 10, "day"); }
    });

    scheduler.init('scheduler_here', new Date(), "week");

}
var html = function (id) { return document.getElementById(id); }; //just a helper

function save_form() {
    var ev = scheduler.getEvent(scheduler.getState().lightbox_id);
    ev.text = html("description").value;
    //ev.Availability = html("Availability").value;


    scheduler.endLightbox(true, html("my_form"));
}

function close_form() {
    scheduler.endLightbox(false, html("my_form"));
}

function delete_event() {
    var event_id = scheduler.getState().lightbox_id;
    scheduler.endLightbox(false, html("my_form"));
    scheduler.deleteEvent(event_id);
}

var GetPatientScheduleViaWeek = function (startdate) {
    var jsonData = JSON.stringify({
        corporateId: $('#ddlCorporate').val(),
        facilityId: $('#ddlFacility').val(),
        userType: $('#ddlUserType').val(),
        userid: $('#ddlUser').val(),
        weekStartDate: startdate,
        patientId: $("#hfPatientId").val()
    });

    $.ajax({
        type: "POST",
        url: "/PatientScheduler/GetCalender",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            //SchedulerInit(data);
            scheduler.clearAll();
            BindDataInScheduler(data);
            scheduler.parse(data.SchPatientSchedulingDataList, "json");
            scheduler.updateView();
        },
        error: function (msg) {
        }
    });
};

function BindUsersTypeDDL() {
    var jsonData = JSON.stringify({
        corporateId: $("#ddlCorporate").val(),
        facilityId: $("#ddlFacility").val()
    });
    $.ajax({
        type: "POST",
        url: "/PatientScheduler/BindUsersType",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {

                if (data.length > 0) {
                    BindDropdownData(data, "#ddlUserType", "#hdUserType");
                }
            }
        },
        error: function (msg) {
        }
    });
}

function BindDataInScheduler(jsonData) {
    if (jsonData.SchDepartmentOpeningSlotsList.length > 0) {
        if (jsonData.SchDepartmentOpeningSlotsList[0].dept_Name !== null && jsonData.SchDepartmentOpeningSlotsList[0].dept_Name !== "") {

        }
        for (var j = 0; j < jsonData.SchDepartmentOpeningSlotsList.length; j++) {
            scheduler.deleteMarkedTimespan({
                days: new Date(jsonData.SchDepartmentOpeningSlotsList[j].start_date),
                zones: "fullday"
            });
            var daysarrayc = [];
            daysarrayc = daysarrayc.push(jsonData.SchDepartmentOpeningSlotsList[j].dept_OpeningDays);
            scheduler.addMarkedTimespan({
                days: new Date(jsonData.SchDepartmentOpeningSlotsList[j].start_date),
                zones: "fullday", // marks the entire day
                css: "patient_expired_notbooking",
                type: "dhx_time_block",
                //html: "<strong> Department : " + jsonData.SchDepartmentOpeningSlotsList[0].dept_Name + " </strong>  <strong> (Closed) </strong>",
            });
            scheduler.deleteMarkedTimespan({
                days: new Date(jsonData.SchDepartmentOpeningSlotsList[j].start_date),
                zones: [parseInt(jsonData.SchDepartmentOpeningSlotsList[j].dept_Openingtime) * 60, parseInt(jsonData.SchDepartmentOpeningSlotsList[j].dept_Closingtime) * 60]
            });
            scheduler.addMarkedTimespan({
                days: new Date(jsonData.SchDepartmentOpeningSlotsList[j].start_date),
                zones: [parseInt(jsonData.SchDepartmentOpeningSlotsList[j].dept_Openingtime) * 60, parseInt(jsonData.SchDepartmentOpeningSlotsList[j].dept_Closingtime) * 60],
                css: "patient_available_open"
            });
        }
    }
    if (jsonData.SchLunchSlotsList.length > 0) {
        for (var k = 0; k < jsonData.SchLunchSlotsList.length; k++) {

            var from = parseFloat(jsonData.SchLunchSlotsList[k].lunch_starttime);
            var to = parseFloat(jsonData.SchLunchSlotsList[k].lunch_endtime);
            scheduler.addMarkedTimespan({
                days: new Date(jsonData.SchLunchSlotsList[k].start_date),
                zones: [from * 60, to * 60],
                css: "patient_lunch_meeting_adm",
                type: "dhx_time_block",
                //html: "<strong> Lunch Break </strong>",
            });
        }
    }
    if (jsonData.SchHolidayList.length > 0) {
        for (var i = 0; i < jsonData.SchHolidayList.length; i++) {
            scheduler.deleteMarkedTimespan({
                days: new Date(jsonData.SchHolidayList[i].start_date),
                zones: "fullday",
            });
            scheduler.addMarkedTimespan({
                days: new Date(jsonData.SchHolidayList[i].start_date),
                zones: "fullday",
                css: "patient_holiday",
                type: "dhx_time_block",
            });
        }
    }
}

var DeleteEventFromScheduler = function (id, ev) {
    var jsonData = JSON.stringify({
        SchedulingId: ev.TimeSlotId
    });
    $.ajax({
        type: "POST",
        url: '/PatientScheduler/DeletePatientScheduling',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data == true) {
                ShowMessage('Records deleted Succesfully!', "Success", "success", true);
            } else {
                ShowMessage('Unable to delete record!', "Error", "error", true);
            }
            scheduler.updateView();
        },
        error: function (msg) {
        }
    });
};