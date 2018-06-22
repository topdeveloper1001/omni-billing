var firstTimeLoad = false;

$(function () {
    //Js Code to be added here
    LoadFacilityTimeslotsData();
    $('#btnClearFacultyTimeslots').on('click', function() {
        $("#FacultyTimeslotsDiv").clearForm(true);
        $("#divFacultyCalenderView").hide();
        $.validationEngine.closePrompt(".formError", true);
    });
    $('#ddlCorporate').on('change', function () {
        BindFacilityByCoporateId();
    });
    $('#ddlFacility').on('change', function () {
        //BindGlobalCodesWithValue("#ddlFacultyType", 2309, "#hdFacultyType");
        BindUsersTypeDDL();
    });
    $('#ddlFacultyType').on('change', function () {
        BindUsersDDL();
    });
    
    $('#btnViewCalender').on('click', function () {
        //----Ajax call to get the Calender view
        var isValid = jQuery("#FacultyTimeslotsDiv").validationEngine({ returnIsValid: true });
        if (isValid == true) {
            var jsonData = JSON.stringify({
                corporateId: $('#ddlCorporate').val(),
                facilityId: $('#ddlFacility').val(),
                facultyType: $('#ddlFacultyType').val(),
                userid: $('#ddlUser').val(),
            });
            $.ajax({
                type: "POST",
                url: '/FacultyTimeslots/GetFacultyCalender',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonData,
                success: function (data) {
                    $("#divFacultyCalenderView").show();
                    if (!firstTimeLoad) {
                        SchedulerInit(data);
                    }
                    scheduler.clearAll();
                    AddDepartmentTimming(data);
                    //AddFacultyLunchTimming(data);
                    scheduler.parse(data.FacultySavedSlotsList, "json");
                    scheduler.updateView();
                },
                error: function (msg) {
                }
            });
        }
    });
});

var LoadFacilityTimeslotsData = function() {
    //BindFacilityByCoporateId();
    $("#FacultyTimeslotsDiv").validationEngine();
    BindCorporates("#ddlCorporate", "#hdCorporateId");
};

function BindUsersDDL() {
    /// <summary>
    /// Binds the users DDL.
    /// </summary>
    /// <returns></returns>
    var usertypeVal = $('#ddlFacultyType').val();
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
            BindDropdownData(data, "#ddlUser", "#hdUserID");
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
                    ID: ev.TimeSlotId,
                    FacultyType: $('#ddlFacultyType').val(),
                    UserID: $('#ddlUser').val(),
                    WeekDay: '',
                    AvailableDateTill: recurancedates[i].end_date,
                    AvailableDateFrom: recurancedates[i].start_date,
                    Description: ev.text,
                    SlotAvailability: ev.Availability,
                    SlotColor: ev.Availability == "1" ? "green" : ev.Availability == "2" ? "red" : "blue",
                    FacilityId: $('#ddlFacility').val(),
                    CorporateId: $('#ddlCorporate').val(),
                    IsRecurring: ev._timed,
                    RecurringDateFrom: ev.start_date,
                    RecurringDateTill: ev.end_date,
                    EventId: id,
                    RecType: ev.rec_type,
                    RecPattern: ev.rec_pattern,
                    RecEventlength: ev.event_length,
                    RecEventPId: ev.event_pid
                };
            }
        }
    } else {
        jsonData[0] = ({
            ID: ev.TimeSlotId,
            FacultyType: $('#ddlFacultyType').val(),
            UserID: $('#ddlUser').val(),
            WeekDay: '',
            AvailableDateTill: ev.end_date,
            AvailableDateFrom: ev.start_date,
            Description: ev.text,
            SlotAvailability: ev.Availability,
            SlotColor: ev.Availability == "1" ? "green" : ev.Availability == "2" ? "red" : ev.Availability == "3" ? "pink" : ev.Availability == "4"? "blue":"blue",
            FacilityId: $('#ddlFacility').val(),
            CorporateId: $('#ddlCorporate').val(),
            IsRecurring: ev._timed,
            RecurringDateFrom: ev.start_date,
            RecurringDateTill: ev.end_date,
            EventId: ev.id,
            RecType: ev.rec_type,
            RecPattern: ev.rec_pattern,
            RecEventlength: ev.event_length,
            RecEventPId: ev.event_pid
        });
    }
    var jsonD = JSON.stringify(jsonData);
    $.ajax({
        type: "POST",
        url: '/FacultyTimeslots/SaveTimeSlotValue',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonD,
        success: function(data) {
            ShowMessage('Records Saved Succesfully!', "Success", "success", true);
            scheduler.updateView();
        },
        error: function(msg) {
        }
    });
};

function SchedulerInit(jsonData) {
    scheduler.config.xml_date = "%m-%d-%Y %H:%i";
    scheduler.xy.editor_width = 0; //disable editor's auto-size

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
    scheduler.config.prevent_cache = true;
    scheduler.config.details_on_dblclick = true;
    scheduler.config.details_on_create = true;    scheduler.config.scroll_hour = (new Date).getHours();
    scheduler.locale.labels.section_parent = "Availability";
    var parent_select_options = [
        { key: '0', label: "---Select---" },
        { key: '1', label: "Available/Open" },
        { key: '2', label: "Busy" },
        { key: '3', label: "Lunch/Meetings/Adm" },
        { key: '4', label: "Holiday" }
    ];
    scheduler.attachEvent("onViewChange", function (new_mode, new_date) {
        var minDate = scheduler.getState().min_date;
        if (firstTimeLoad) {
            firstTimeLoad = false;
            GetFacultyScheduleViaWeek(minDate);
        }
    });

    scheduler.templates.event_class = function (start, end, event) {
        var css = "";
        if (event.Availability) // if event has subject property then special class should be assigned
        {
            if (event.Availability == 1) {
                css = "physicians_available";
            } else if (event.Availability == 2) {
                css = "physicians_noshow_sick";
            } else if (event.Availability == 3) {
                css = "physicians_lunch_meeting_adm";
            } else if (event.Availability == 4) {
                css = "physicians_holiday";
            }
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
        { name: "parent", height: 30, type: "select", options: parent_select_options, map_to: "Availability" },
         {
             name: "recurring", height: 115, type: "recurring", map_to: "rec_type",
             button: "recurring"
         },
        { name: "time", height: 72, type: "time", map_to: "auto" }
    ];
    scheduler.attachEvent("onEventAdded", function (id, ev) {
        var datesObj = scheduler.getRecDates(id);
        SaveTimeSlotData(id, ev);
    });

    scheduler.attachEvent("onEventChanged", function (id, ev) {
        var datesObj = scheduler.getRecDates(id);
        SaveTimeSlotData(id,ev);
    });

    scheduler.attachEvent("onEventDeleted", function (id, ev) {
        DeleteEventFromScheduler(id, ev);
    });


    scheduler.attachEvent("onTemplatesReady", function () {
        //work week
        scheduler.config.start_on_monday = true;
        scheduler.config.time_step = 15;
        scheduler.xy.min_event_height = 21;
        scheduler.config.hour_size_px = 88;
    });
    scheduler.init('scheduler_here', new Date(), "day");


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

/// <var>
/// The get faculty schedule via week
/// </var>
var GetFacultyScheduleViaWeek = function(startdate) {
    var jsonData = JSON.stringify({
        corporateId: $('#ddlCorporate').val(),
        facilityId: $('#ddlFacility').val(),
        facultyType: $('#ddlFacultyType').val(),
        userid: $('#ddlUser').val(),
        weekStartDate: startdate
    });

    $.ajax({
        type: "POST",
        url: "/FacultyTimeslots/GetFacultyCalender",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            //SchedulerInit(data);
            AddDepartmentTimming(data);
            //AddFacultyLunchTimming(data);
            scheduler.clearAll();
            scheduler.parse(data.FacultySavedSlotsList, "json");
            //scheduler.updateView();
        },
        error: function (msg) {
        }
    });
};

/// <var>add department timming</var>
var AddDepartmentTimming = function (data) {
    if (data.DepartmentOpeningSlotsList.length > 0) {
        if (data.DepartmentOpeningSlotsList[0].dept_Name !== null && data.DepartmentOpeningSlotsList[0].dept_Name !== "") {

        }
        for (var j = 0; j < data.DepartmentOpeningSlotsList.length; j++) {
            scheduler.addMarkedTimespan({
                days: new Date(data.DepartmentOpeningSlotsList[j].start_date),
                zones: "fullday", // marks the entire day
                type: "dhx_time_block",
                html: "<strong> Department : " + data.DepartmentOpeningSlotsList[0].dept_Name + " </strong>  <strong> (Closed) </strong>",
            });
            scheduler.deleteMarkedTimespan({
                days: new Date(data.DepartmentOpeningSlotsList[j].start_date),
                zones: [parseInt(data.DepartmentOpeningSlotsList[j].dept_Openingtime) * 60, parseInt(data.DepartmentOpeningSlotsList[j].dept_Closingtime) * 60]
            });
        }
        scheduler.config.scroll_hour = (new Date).getHours();
        scheduler.updateView();
    }
}

var AddFacultyLunchTimming = function (data) {
    var isReloadNeeded = false;
    if (data.DepartmentOpeningSlotsList.length > 0) {
        for (var j = 0; j < data.DepartmentOpeningSlotsList.length; j++) {
            if (data.DepartmentOpeningSlotsList[j].LunchTimeFrom !== '') {
                isReloadNeeded = true;
                scheduler.addEvent({
                    start_date: new Date(data.DepartmentOpeningSlotsList[j].LunchTimeFrom),
                    end_date: new Date(data.DepartmentOpeningSlotsList[j].LunchTimeTill),
                    text: "Lunch Time",
                    Availability: "3",
                });
            }
        }
        if (isReloadNeeded) GetFacultyScheduleViaWeek(data.DepartmentOpeningSlotsList[0].LunchTimeFrom);
        scheduler.config.scroll_hour = (new Date).getHours();
        scheduler.updateView();
    }
}

function BindUsersTypeDDL() {
    var jsonData = JSON.stringify({
        corporateId: $("#ddlCorporate").val(),
        facilityId: $("#ddlFacility").val()
    });
    $.ajax({
        type: "POST",
        url: "/Home/BindUsersType",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                if (data.length > 0) {
                    BindDropdownData(data, "#ddlFacultyType", "#hdFacultyType");
                }
            }
        },
        error: function (msg) {
        }
    });
}

var DeleteEventFromScheduler = function (id, ev) {
    var jsonData = JSON.stringify({
        id: ev.TimeSlotId
    });
    $.ajax({
        type: "POST",
        url: '/FacultyTimeslots/DeleteTimeSlot',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data == true)
                ShowMessage('Records deleted Succesfully!', "Success", "success", true);
            else
                ShowMessage('Unable to delete record!', "Error", "warning", true);
        },
        error: function (msg) {
        }
    });
};