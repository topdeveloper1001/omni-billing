var firstTimeLoad = false;

$(function () {
    $("#HolidayPlannerFormDiv").validationEngine();
    BindCorporateDataInHolidayPlanner();
    $("#showHideItems").hide();
    $("#showHideYear").hide();

    $("#ddlFacilityFilter").change(function () {
        BindItemTypes();
        $("#ddlItemTypeId").attr("disabled", $("#ddlFacilityFilter").val() == 0);
    });

    $("#ddlItemTypeId").change(function () {
        BindItems();
    });

    //$("#ddlYear").change(function () {
    //    var selected = $(this).val();
    //    if (selected > 0) {
    //        GetHolidayPlannerList();
    //    }
    //});

    $("#btnShowCalendar").on("click", GetHolidayPlannerList);
});

function EditHolidayPlanner(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/HolidayPlanner/GetHolidayPlanner',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $('#HolidayPlannerFormDiv').empty();
            $('#HolidayPlannerFormDiv').html(data);
            $('#collapseHolidayPlannerAddEdit').addClass('in');
            $("#HolidayPlannerFormDiv").validationEngine();
        },
        error: function () {

        }
    });
}

function DeleteHolidayPlanner(id) {
    if (confirm("Do you want to delete this record? ")) {
        //var txtHolidayPlannerId = id;
        var jsonData = JSON.stringify({
            Id: id
        });
        $.ajax({
            type: "POST",
            url: '/HolidayPlanner/DeleteHolidayPlanner',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindHolidayPlannerGrid();
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                } else {
                    return false;
                }
                return false;
            },
            error: function () {
                return true;
            }
        });
    }
}

function BindHolidayPlannerGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/HolidayPlanner/BindHolidayPlannerList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#HolidayPlannerListDiv").empty();
            $("#HolidayPlannerListDiv").html(data);

        },
        error: function () {

        }

    });
}

//function ClearAll() {
//    $("#HolidayPlannerFormDiv").clearForm(true);
//    $('#collapseHolidayPlannerAddEdit').removeClass('in');
//    $('#collapseHolidayPlannerList').addClass('in');
//    $("#HolidayPlannerFormDiv").validationEngine();
//    $.validationEngine.closePrompt(".formError", true);
//}

function BindCorporateDataInHolidayPlanner() {
    //Bind Corporates
    /// <summary>
    /// Binds the corporates.
    /// </summary>
    /// <param name="selector">The selector.</param>
    /// <param name="selectedId">The selected identifier.</param>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        url: "/RoleSelection/GetCorporatesDropdownData",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            BindDropdownData(data, "#ddlCorporate", "#hfCorporateId");
            var corporaeIdFilter = $("#ddlCorporate").val();
            if (corporaeIdFilter > 0) {
                BindFacilityDropdownFilterInHolidayPlanner(corporaeIdFilter);
            }
        },
        error: function () {
        }
    });
}

function BindFacilityDropdownFilterInHolidayPlanner(cId) {
    if (cId > 0) {
        $.ajax({
            type: "POST",
            url: "/Facility/GetFacilitiesbyCorporate",
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ corporateid: cId }),
            success: function (data) {
                BindDropdownData(data, "#ddlFacilityFilter", "#ddlFacilityFilter");
                $("#ddlFacilityFilter")[0].selectedIndex = 0;
                if ($("#hfFacilityId").val() > 0) {
                    $("#ddlFacilityFilter").val($("#hfFacilityId").val());
                }

                BindItemTypes();
                $("#ddlItemTypeId").attr("disabled", $("#ddlFacilityFilter").val() == 0);
            },
            error: function (msg) {
                console.log(msg);
            }
        });
    }
    else {
        BindDropdownData('', "#ddlFacilityFilter", "");
        $("#ddlFacilityFilter")[0].selectedIndex = 0;
    }
    $("#hfCorporateId").val(cId > 0 ? cId : "");
}

function ShowHideYear() {
    var itemId = $("#ddlItemId").val();
    if (itemId != 0) {
        $("#showHideYear").show();

    } else {
        $("#showHideYear").hide();
    }
}

function BindItems() {
    if ($("#ddlItemTypeId").val() > 0) {
        $.ajax({
            type: "POST",
            url: "/HolidayPlanner/GetItemsDropdownData",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ cId: $("#ddlCorporate").val(), fId: $("#ddlFacilityFilter").val(), itemTypeId: $("#ddlItemTypeId").val() }),
            success: function (data) {
                BindDropdownData(data, "#ddlItemId", "");
                $("#showHideItems").show();

            },
            error: function (msg) {
                console.log(msg);
            }
        });
    } else {
        $("#showHideItems").hide();
    }
}

function BindFacilityDeapartments(selector, hidValueSelector) {
    $.ajax({
        type: "POST",
        url: "/Login/GetFacilityDeapartments",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, deaprtments) {
                    items += "<option value='" + deaprtments.Value + "'>" + deaprtments.Text + "</option>";
                });
                $(selector).html(items);

                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
                    $(selector).val($(hidValueSelector).val());
            }
            else {
            }
        },
        error: function () {
        }
    });
}

function SaveHolidayPlanner(id, ev) {

    var jsonTimeSlotsArray = [];
    var hdHolidayPlannerId = 0;
    var txtCorporateId = $("#ddlCorporate").val();
    var txtFacilityId = $("#ddlFacilityFilter").val();
    var txtItemtypeId = $("#ddlItemTypeId").val();
    var txtItemId = $("#ddlItemId").val();
    var txtYear = $("#ddlYear").val();
    var txtDescription = $("#txtDescription").val();

    if (ev._timed == true) {
        var recurancedates = scheduler.getRecDates(id);
        if (recurancedates.length > 0) {
            for (var i = 0; i < recurancedates.length; i++) {
                jsonTimeSlotsArray[i] = {
                  Id:ev.Id,
                    HolidayPlannerId: hdHolidayPlannerId,
                    SlotColor: "green",
                    EventId: id,
                    IsRecurring: ev._timed,
                    RecType: ev.rec_type,
                    RecPattern: ev.rec_pattern,
                    RecEventlength: ev.event_length,
                    RecEventId: ev.event_pid,
                    IsFullDay: ev.full_day,
                    Description: ev.text,
                    _timed: ev._timed,
                    rec_type: ev.rec_type,
                    rec_pattern: ev.rec_pattern,
                    event_length: ev.event_length,
                    event_pid: ev.event_pid,
                    start_date: ev.start_date,
                    end_date: ev.end_date,
                    full_day: ev.full_day,
                    text: ev.text,
                    HolidayDate: ev.start_date
                };
            }
        }
    } else {
        jsonTimeSlotsArray[0] = {
            Id: ev.Id,
            HolidayPlannerId: hdHolidayPlannerId,
            SlotColor: "green",
            EventId: id,
            IsRecurring: ev._timed,
            RecType: ev.rec_type,
            RecPattern: ev.rec_pattern,
            RecEventlength: ev.event_length,
            RecEventId: ev.event_pid,
            IsFullDay: ev.full_day,
            Description: ev.text,
            _timed: ev._timed,
            rec_type: ev.rec_type,
            rec_pattern: ev.rec_pattern,
            event_length: ev.event_length,
            event_pid: ev.event_pid,
            start_date: ev.start_date,
            end_date: ev.end_date,
            full_day: ev.full_day,
            text: ev.text,
            HolidayDate: ev.start_date
        };
    }

    var jsonMain = {
        HolidayPlannerId: hdHolidayPlannerId,
        CorporateId: txtCorporateId,
        FacilityId: txtFacilityId,
        ItemtypeId: txtItemtypeId,
        ItemId: txtItemId,
        Year: txtYear,
        Description: txtDescription,
        IsActive: true,
        TimeSlots: jsonTimeSlotsArray
    };
    var jsonD = JSON.stringify(jsonMain);
    $.ajax({
        type: "POST",
        url: '/HolidayPlanner/SaveHolidayPlanner',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonD,
        success: function () {
            var msg = "Records Saved successfully !";
            if (id > 0)
                msg = "Records updated successfully";

            ShowMessage(msg, "Success", "success", true);
        },
        error: function () {

        }
    });
}

function GetHolidayPlannerList() {
    var isValid = $("#HolidayPlannerFormDiv").validationEngine({ returnIsValid: true });
    if (isValid) {
        var jsonData = JSON.stringify(
        {
            CorporateId: $("#ddlCorporate").val(),
            FacilityId: $("#ddlFacilityFilter").val(),
            Year: $("#ddlYear").val(),
            ItemtypeId: $("#ddlItemTypeId").val(),
            ItemId: $("#ddlItemId").val()
        });
        $.ajax({
            type: "POST",
            url: "/HolidayPlanner/GetHolidayPlannerList",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                $("#scheduler_here").show();
                if (!firstTimeLoad) {
                    InitializeCalender();
                    firstTimeLoad = true;
                }
                scheduler.clearAll();
                scheduler.parse(data.TimeSlots, "json");
                scheduler.updateView();
            },
            error: function (err) {
                ShowErrorMessage("Error while fetching the List. Please try again later! ", true);
            }
        });
    } else {
        ShowErrorMessage("Kindly fill all the selections first and then try again!", true);
        $("#scheduler_here").hide();
    }
}

function BindItemTypes() {
    var selected = $("#ddlFacilityFilter").val();
    if (selected > 0 && ($("#ddlItemTypeId").val() == 0 || $("#ddlItemTypeId").val() == null)) {
        BindGlobalCodesWithValue("#ddlItemTypeId", 4902, '');
    }

    if ($("#ddlItemTypeId").val() > 0) {
        BindItems();
    }
}



//---------------------SCHEDULAR EVENTS-----------------------
function InitializeCalender() {
    scheduler.config.multi_day = true;
    scheduler.config.mark_now = true;
    scheduler.config.first_hour = 9;//start time of the calendar view
    scheduler.config.last_hour = 17;// end time of the calendar view
    scheduler.config.xml_date = "%m-%d-%Y %H:%i";

    scheduler.xy.editor_width = 0; //disable editor's auto-size
    scheduler.config.details_on_create = true;
    //scheduler.config.full_day = true;   
    scheduler.config.time_step = 15;
    scheduler.xy.min_event_height = 21;
    scheduler.config.hour_size_px = 88;

    //scheduler.config.show_loading = true;
    scheduler.config.full_day = true;

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

    //scheduler.locale.labels.workweek_tab = "W-Week";
    //scheduler.locale.labels.decade_tab = "Decade";
    scheduler.config.details_on_dblclick = true;
    scheduler.config.details_on_create = true;


    scheduler.attachEvent("onTemplatesReady", function () {
        ////work week
        //scheduler.date.workweek_start = scheduler.date.week_start;
        //scheduler.templates.workweek_date = scheduler.templates.week_date;
        //scheduler.templates.workweek_scale_date = scheduler.templates.week_scale_date;
        //scheduler.date.add_workweek = function (date, inc) { return scheduler.date.add(date, inc * 7, "day"); }
        //scheduler.date.get_workweek_end = function (date) { return scheduler.date.add(date, 5, "day"); }



        ////decade
        //scheduler.date.decade_start = function (date) {
        //    var ndate = new Date(date.valueOf());
        //    ndate.setDate(Math.floor(date.getDate() / 10) * 10 + 1);
        //    return this.date_part(ndate);
        //}
        //scheduler.templates.decade_date = scheduler.templates.week_date;
        //scheduler.templates.decade_scale_date = scheduler.templates.week_scale_date;
        //scheduler.date.add_decade = function (date, inc) { return scheduler.date.add(date, inc * 10, "day"); }

        scheduler.config.start_on_monday = true;
        scheduler.config.time_step = 15;
        scheduler.xy.min_event_height = 21;
        scheduler.config.hour_size_px = 88;
    });

    scheduler.attachEvent("onLightbox", function () {

        $(".dhx_section_time").hide();
        var obj = $("input[name*='full_day']");
        $(obj).prop("checked", true);
    });

    scheduler.config.lightbox.sections = [
        { name: "description", height: 130, map_to: "text", type: "textarea", focus: true },
        //{ name: "parent", height: 23, type: "textarea", map_to: "custom1" },
        {
            name: "recurring", height: 115, type: "recurring", map_to: "rec_type",
            button: "recurring"
        }
        ,
        { name: "time", height: 72, type: "time", map_to: "auto" }
    ];


    //scheduler.attachEvent("onViewChange", function (new_mode, new_date) {
    //    //var minDate = scheduler.getState().min_date;
    //    if (firstTimeLoad) {
    //        firstTimeLoad = false;
    //        GetHolidayPlannerList();
    //    }
    //});

    //scheduler.attachEvent("onEventChanged", function (id, ev) {
    //    var datesObj = scheduler.getRecDates(id);
    //    SaveHolidayPlanner(id, ev);
    //});

    scheduler.attachEvent("onEventAdded", function (id, ev) {
        
        var datesObj = scheduler.getRecDates(id);
        SaveHolidayPlanner(id, ev);
    });

    scheduler.attachEvent("onEventChanged", function (id, ev) {
     
        var datesObj = scheduler.getRecDates(id);
        SaveHolidayPlanner(id, ev);
    });
   

    scheduler.attachEvent("onEventDeleted", function (id, ev) {
        alert('Event not ready yet..:)');
        DeleteEventFromHolidayPlannerDetail(id, ev);
    });

    var year = $("#ddlYear").val();
    var date = new Date();
    var selectedDate = new Date(year, date.getMonth(), date.getDay());
    scheduler.init('scheduler_here', selectedDate, "month");


    //scheduler.parse([
    //    { text: "Meeting", start_date: "10/07/2015 14:00", end_date: "10/07/2015 17:00", custom1: "c1", custom2: "c1" },
    //    { text: "Conference", start_date: "10/07/2015 12:00", end_date: "10/07/2015 19:00", custom1: "c1", custom2: "c2" },
    //    { text: "Interview", start_date: "10/07/2015 09:00", end_date: "10/07/2015 10:00", custom1: "c1", custom2: "c2" }
    //], "json");

}


var html = function (id) {
    return document.getElementById(id);
}; //just a helper

function save_form() {
    var ev = scheduler.getEvent(scheduler.getState().lightbox_id);
    ev.text = html("description").value;
    //ev.text = html("description").value;
    //ev.custom1 = html("custom1").value;
    //ev.custom2 = html("custom2").value;

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


//---------------------SCHEDULAR EVENTS-----------------------


function DeleteEventFromHolidayPlannerDetail(id, ev) {
    
    var jsonData = JSON.stringify({
        id: ev.Id
    });
    $.ajax({
        type: "POST",
        url: '/HolidayPlannerDetails/DeleteHolidayPlannerEvent',
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

