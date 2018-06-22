var firstTimeLoad = false;
var firstTimeSave = true;
var firstTimeBind = true;
var apptTypeIdArray = [];
var apptRecArray = [];
var schedularEventObj;
var presentDate = new Date(), y = presentDate.getFullYear(), m = presentDate.getMonth();
var monthFirstDay = new Date(y, m, 1);
var monthLastDay = new Date(y, m + 1, 0);
var firstHour = 0
var lastHour = 24;

var schedulerUrl = "/Scheduler/";
var schedulerViewId = 0;

var clinicianUpdated = false;

$(function () {
    schedulerViewId = $("#hfViewId").val();

    /// <summary>
    ///     s this instance.
    /// </summary>
    /// <returns></returns>
    $("#divDatetimePicker")
        .datepicker({
            //showWeek: true,
            firstDay: 0,
            numberOfMonths: [2, 1],
            //showButtonPanel: true,
            onSelect: function (dateText, inst) {
                onCheckFilters();
            }
        });

    BindAllSchedularData();

    $("#btnScheduleAppointment")
        .on("click",
        function () {
            $("#hidSchedulingType").val("1");
            $("#divSchedularPopUpContent .modal-title").html("Appointments Scheduler");
            ShowLightBoxStyle("1"); //1 means schedule appointment
            var checkedBoxes = GetCheckedCheckBoxes("treeviewPhysician");
            //if (checkedBoxes.length > 0) {
            scheduler.addEventNow();
            //} else {
            // ShowMessage('Select Any Physician first!', "Error", "error", true);
            //}
        });

    //$("#btnAddHoliday")
    //    .on("click", function () {
    //        $("#hidSchedulingType").val("2");
    //        $("#divSchedularPopUpContent .modal-title").html("Holiday/Vacations Scheduler");
    //        ShowLightBoxStyle("2"); //2 means Holiday
    //        var checkedBoxes = GetCheckedCheckBoxes("treeviewPhysician");
    //        scheduler.addEventNow();
    //        $("#divShowPhysicianVacationList").hide();
    //    });

    $("#btnAddVacation").on("click", function () {
        $("#hidSchedulingType").val("3");
        var checkedBoxes = GetCheckedCheckBoxes("treeviewPhysician");
        if (checkedBoxes.length > 0) {
            scheduler.addEventNow();
        } else {
            ShowMessage("Select Any Physician first!", "Error", "error", true);
        }
    });

    $("#chkViewAvialableSlots").on("change",
        function (e) {
            if ($("#chkViewAvialableSlots").prop("checked")) {
                $("#chkViewVocationHolidays").prop("checked", false);
                GetSchedularCustomData("1");
            }
        });

    $("#chkViewVocationHolidays").on("change",
        function (e) {
            if ($("#chkViewVocationHolidays").prop("checked")) {
                $("#chkViewAvialableSlots").prop("checked", false);
                GetSchedularCustomData("3");
            }
        });

    $("#ddPhysician").change(function () {
        clinicianUpdated = true;
    });

    $("#btnSaveSchedulingData")
        .on("click", function () {
            var schedulingType = $("#hidSchedulingType").val();
            if (schedulingType == "1") {
                var isdivSchedulerDataValid = jQuery("#divSchedulerData").validationEngine({ returnIsValid: true });
                var isdivAppointmentTypeProceduresValid = jQuery("#divAppointmentTypeProcedures")
                    .validationEngine({ returnIsValid: true });
                if (isdivSchedulerDataValid && isdivAppointmentTypeProceduresValid) {
                    blockRecurrenceDiv("Saving...");
                    var isMultipleType = $("input[name=rdbtnSelection]:checked").val();
                    if (clinicianUpdated && isMultipleType == "2" && $("#hfSchedulingId").val() != "" && $("#hfSchedulingId").val() > 0) {
                        OpenConfirmPopup(0, 'Clinician Updated!', 'Clinician has been updated to the selected Appointment Type. Do you want to Continue!', SaveCustomSchedular, null);
                    }
                    else
                        SaveCustomSchedular();
                }
            } else {
                CheckTwoDates($("#eventFromDate"), $("#eventToDate"), "eventToDate");
                var isdivHolidayValid = jQuery("#divHoliday").validationEngine({ returnIsValid: true });
                //if (isdivHolidayValid && $('#eventToDate').val() != '') {
                if (isdivHolidayValid) {
                    blockRecurrenceDiv("Saving...");
                    SaveHolidaySchedular();
                    scheduler.cancel_lightbox();
                }
            }
            firstTimeBind = true;
        });

    $(document).keyup(function (e) {
        if ($("#divSchedularPopUp").is(" :visible")) {
            if (!e) e = window.event; // fix IE

            if (e.keyCode) // IE
            {
                if (e.keyCode == "27") $("#btnCancelSchedulingData").click();
            }
            else if (e.charCode) // Netscape/Firefox/Opera
            {
                if (e.keyCode == "27") $("#btnCancelSchedulingData").click();
            }
        }
    });

    $("#btnCancelSchedulingData")
        .on("click",
        function () {
            blockRecurrenceDiv("Cancelling...");
            $("#hfAppointmentTypes").val("");
            scheduler.cancel_lightbox();
            $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
            $.validationEngine.closePrompt(".formError", true);
            firstTimeLoad = true;
            firstTimeBind = true;
            ClearSchedulingPopup();
        });
    $("#btnDeleteSchedulingData")
        .on("click",
        function () {
            //$("#loader_event").show();
            blockRecurrenceDiv("Deleting...");
            firstTimeBind = true;
            var eventParentId = $("#hidEventParentId").val();
            var schedulingId = $("#hfSchedulingId").val();
            var schType = $("#hidSchedulingType").val();
            var externalValue3 = $("#hfExternalValue3").val();
            DeleteSchduling(eventParentId, schedulingId, schType, externalValue3);
        });


    $("#eventFromDate")
        .datetimepicker({
            format: "m/d/Y",
            minDate: 0,
            maxDate: "2025/12/12",
            timepicker: false,
            closeOnDateSelect: true
        });
    $("#eventFromTime")
        .kendoTimePicker({
            interval: 15,
            format: "HH:mm"
        });
    $(".main_content")
        .scroll(function () {
            $.validationEngine.closePrompt(".formError", true);
        });

    $("#eventToDate")
        .datetimepicker({
            format: "m/d/Y",
            minDate: 0,
            maxDate: "2025/12/12",
            timepicker: false,
            closeOnDateSelect: true
        });
    $("#eventToTime")
        .kendoTimePicker({
            interval: 15,
            format: "HH:mm"
        });

    /*over view popup*/
    $("#txtOVDateFrom")
        .datetimepicker({
            format: "m/d/Y",
            minDate: 0,
            maxDate: "2025/12/12",
            timepicker: false,
            closeOnDateSelect: true
        });
    $("#txtOVDateFrom").val(presentDate.getMonth() + 1 + "/" + presentDate.getDate() + "/" + presentDate.getFullYear());
    $("#txtOVTimeFrom")
        .kendoTimePicker({
            interval: 30,
            format: "HH:mm"
        });
    $("#txtOVTimeFrom").val("00:00");
    $("#txtOVDateTo")
        .datetimepicker({
            format: "m/d/Y",
            minDate: 0,
            maxDate: "2025/12/12",
            timepicker: false,
            closeOnDateSelect: true
        });
    $("#txtOVTimeTo")
        .kendoTimePicker({
            interval: 30,
            format: "HH:mm"
        });
    $("#txtOVTimeTo").val("23:30");


    $(".EmiratesMask").mask("999-99-9999");
    $("#txtPersonDOB")
        .datetimepicker({
            format: "m/d/Y",
            minDate: "1901/12/12", //yesterday is minimum date(for today use 0 or -1970/01/01)
            maxDate: new Date(),
            timepicker: false,
            closeOnDateSelect: true
        });
    $("#txtRecEndByDate")
        .datetimepicker({
            format: "m/d/Y",
            minDate: 0,
            maxDate: "2025/12/12",
            timepicker: false,
            closeOnDateSelect: true
        });
    //$("#txtRecEndByDate").val(new Date().format('mm/dd/yyyy'));
    $(".searchHitMe")
        .on("click",
        function (e) { //function to toggle previous visit list in & out
            //$('.searchSlide').toggleClass('moveLeft');
        });
    $("#spnPrevList")
        .on("click",
        function (e) { //function to toggle previous visit list in & out
            $(".searchSlide").removeClass("moveLeft");
        });
    $("#spnAvailTimeSlots")
        .on("click",
        function (e) { //function to toggle previous visit list in & out
            $("#divAvailableTimeSlots").removeClass("moveLeft2");
        });
    $("#btnSearchPatient")
        .on("click",
        function () {
            PatientSearchPopupOpen();
            $("#divSearchPatient").show();
        });

    //$("#parentOfTextbox").on('keydown', function (e) {
    //    var keyCode = e.keyCode || e.which;
    //    if (keyCode == 9) {
    //        e.preventDefault();
    //        // call custom function here
    //    }
    //});
    //$('#btnClearPatient').on('click', function() {
    //    $('#spnSelectedPatient').html('');
    //    $('#btnClearPatient').hide();
    //    onCheckFilters();
    //});
    //$('#spnClearSelectedPatient').on('click', function () {
    //    $('#spnSelectedPatient').html('');
    //    $('#btnClearPatient').hide();
    //    onCheckFilters();
    //});
    //$('#page-wrapper').on('click', function() {
    //    $('#spnSelectedPatient').html('');
    //});
    //$('#divSchedularPopUpContent').on('click', function() {
    //    $('.searchSlide').removeClass('moveLeft');
    //    $('.searchSlide1').removeClass('moveLeft');
    //});
    //$('.sliderpopup').on('click', function (e) { e.stopPropagation(); });
    //$('.searchSlide1').on('click', function (e) { e.stopPropagation(); });

    $("#txtRecEveryDay, #txtRepeatMonthDay, #txtRecEveryWeekDays, #txtOVApptTypesFrequency")
        .keypress(function (event) {

            return checkIsNumber(event, this);
        });
    $("#divPrevListSlider, #divAvailableTimeSlots")
        .click(function (e) {
            return false;
        });
    $("body")
        .click(function (evt) {
            if (evt.target.id == "divPreVisitList") {
                if ($(".searchSlide").hasClass("moveLeft")) {
                    $(".searchSlide").removeClass("moveLeft");
                } else {
                    $(".searchSlide").addClass("moveLeft");
                    $(".searchSlide1").removeClass("moveLeft2");
                    $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
                }
                return false;
            } else if (evt.target.id == "btnShowTimeSlots" || evt.target.id == "btnViewNearestTime") {
                if ($(".searchSlide1").hasClass("moveLeft2")) {
                    $(".searchSlide1").removeClass("moveLeft2");
                } else {
                    $(".searchSlide1").addClass("moveLeft2");
                    $(".searchSlide").removeClass("moveLeft");
                    $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
                }
                return false;
            } else if (evt.target.id == "divReccurrencePopup") {
                if ($("#divReccurrencePopup .popup_frame").hasClass("moveLeft")) {
                    $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
                } else {
                    $(".searchSlide1").removeClass("moveLeft2");
                    $(".searchSlide").removeClass("moveLeft");
                    $("#divReccurrencePopup .popup_frame").addClass("moveLeft");
                }
                return false;
            } else {
                $(".searchSlide").removeClass("moveLeft");
                $(".searchSlide1").removeClass("moveLeft2");
                //$("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
                //return false;
            }
            //For descendants of menu_content being clicked, remove this check if you do not want to put constraint on descendants.
            /*if ($(evt.target).closest('#menu_content').length)
                return;*/
            //Do processing of click event here for every element except with id menu_content

        });

    $("#ddHolidayPhysician")
        .on("change",
        function () {
            BindPhyPreviousVacations();

        });

});


//***************************--Scheduler Tool Initialization--**************************************

function IntializeScheduler(jsonData, physicianData, type) {
    /// <summary>
    ///     Schedulers the initialize.
    /// </summary>
    /// <param name="jsonData">The json data.</param>
    /// <param name="physicianData">The physician data.</param>
    /// <returns></returns>
    scheduler.config.xml_date = "%m-%d-%Y %H:%i";
    scheduler.xy.editor_width = 0; //disable editor's auto-size

    //Setting the Time range of the Scheduler, it should be according to the phyicians clinic.
    scheduler.config.first_hour = firstHour;
    scheduler.config.last_hour = lastHour;

    var format = scheduler.date.date_to_str("%H:%i");
    var step = 15;

    var sections = [];
    if (type == "1") {
        if (physicianData != null && physicianData.length > 0 && physicianData[0].Id > 0) {
            for (var i = 0; i < physicianData.length; i++) {
                var physicianid = physicianData[i].Id;
                //var phyName = $("#treeviewPhysician [name='checkedFiles'][value=" + physicianid + "]").parent().next().text();
                var phyName = $("#Phy_" + physicianid).text() == "" ? physicianData[i].Name : $("#Phy_" + physicianid).text();
                sections.push({ key: physicianid, label: phyName });
            }
        }
        //else {
        //var physicianCount = $("#treeviewPhysician [name='checkedFiles']"); //.parent().next().text();
        //for (var j = 0; j < physicianCount.length; j++) {
        //    var phyName1 = $("#treeviewPhysician [name='checkedFiles'][value=" + physicianCount[j].value + "]").parent().next().text();
        //    sections.push({ key: physicianCount[j].value, label: phyName1 });
        //}
        //}
    }
    else if (type == "3") {
        $.each(physicianData, function (index, rm) {
            var roomId = rm.FacilityStructureId;
            var roomName = rm.FacilityStructureName;
            sections.push({ key: roomId, label: roomName });
        });
    }
    else if (physicianData != null && physicianData.length > 0 && physicianData[0].Id > 0) {
        for (var k = 0; k < physicianData.length; k++) {
            var deptid = physicianData[k].Id;
            //var deptName = $("#treeviewFacilityDepartment [name='checkedFiles'][value=" + deptid + "]").parent().next().text();
            var deptName = $("#Dept_" + deptid).text();
            sections.push({ key: deptid, label: deptName });
        }
    }

    scheduler.templates.hour_scale = function (date) {
        html = "";
        for (var k = 0; k < 60 / step; k++) {
            html += "<div style='height:21px;line-height:10px;'>" + format(date) + "</div>";
            date = scheduler.date.add(date, step, "minute");
        }
        return html;
    };
    scheduler.config.prevent_cache = true;
    scheduler.config.details_on_dblclick = true;
    scheduler.config.details_on_create = true;
    scheduler.config.scroll_hour = (new Date).getHours();
    scheduler.config.show_loading = true;
    scheduler.config.start_on_monday = false;
    scheduler.config.dblclick_create = false;
    //scheduler.config.limit_start = new Date();
    //scheduler.config.limit_end = new Date(new Date().setFullYear(new Date().getFullYear() + 1));


    //scheduler.locale.labels.week_unit_tab = "Week units";
    //scheduler.locale.labels.single_unit_tab = "Units";

    scheduler.locale.labels.section_parent = "Availability";

    scheduler.createUnitsView({
        name: "week_unit",
        property: "section_id",
        list: sections,
        size: 5,
        step: 1,
        days: 5
    });
    //scheduler.date.week_unit_start = scheduler.date.week_start;
    if (sections.length > 0) {
        scheduler.createUnitsView({
            name: "single_unit",
            property: "section_id",
            size: 5,
            step: 1,
            list: sections
        });
    }
    //scheduler.templates.units_second_scale_date = function (date) {
    //    return scheduler.templates.week_scale_date(date);
    //};
    var parent_select_options = [
        { key: "0", label: "---Select---" },
        { key: "1", label: "Available/Open" },
        { key: "2", label: "Initial Booking " },
        { key: "3", label: "Confirmed" },
        { key: "4", label: "Lunch/Meetings/Adm" },
        { key: "5", label: "Holiday" },
        { key: "6", label: "Cancel" }
    ];

    scheduler.attachEvent("onViewChange",
        function (new_mode, new_date) {
            var minDate = scheduler.getState().min_date;

        });

    scheduler.templates.event_class = function (start, end, event) {
        var css = "";
        if (event.Availability) // if event has subject property then special class should be assigned
        {
            if (event.Availability == 1 && event.ExtValue5 == "ios")
                css = "patient_approved_leave"
            else if (event.Availability == 1) {
                css = "patient_initial_booking";
            } else if (event.Availability == 2) {
                css = "patient_confirmed";
            } else if (event.Availability == 3) {
                css = "patient_approved_booking";
            } else if (event.Availability == 4) {
                css = "patient_cancelled_booking";
            } else if (event.Availability == 5) {
                css = "patient_expired_notbooking";
            } else if (event.Availability == 6) {
                css = "patient_lunch_meeting_adm";
            } else if (event.Availability == 7) {
                css = "patient_confirmed_leave";
            } else if (event.Availability == 8) {
                css = "patient_approved_leave";
            } else if (event.Availability == 9) {
                css = "patient_cancelled_notrefilled";
            } else if (event.Availability == 10) {
                css = "patient_not_approved_mail";
            } else if (event.Availability == 13) {
                css = "Patient_ChecksIn";
            }
            else if (event.Availability == 14) {
                css = "Service_Administered";
            }
            else if (event.Availability == 15) {
                css = "Encounter_Ends";
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
        { name: "Physician", height: 23, type: "select", options: sections, map_to: "section_id" },
        { name: "parent", height: 30, type: "select", options: parent_select_options, map_to: "Availability" }
        , { name: "time", height: 72, type: "time", map_to: "auto" }
    ];


    scheduler.attachEvent("onClick",
        function (id) {
            scheduler.config.select = true;
            var event = scheduler.getEvent(id);
            var sId = event.TimeSlotId;
            if (event.Availability == "6") {
                scheduler.config.select = false;
            }
            else if (event.Availability == "13") {

                scheduler.config.icons_select = [
                    "icon_arrive_check",
                    "icon_deport_uncheck",
                    "icon_noshow_uncheck"
                ];
            }
            else if (event.Availability == "15") {
                scheduler.config.icons_select = [
                    "icon_arrive_uncheck",
                    "icon_deport_check",
                    "icon_noshow_uncheck"
                ];
            }
            else if (event.Availability == "5") {
                scheduler.config.icons_select = [
                    "icon_arrive_uncheck",
                    "icon_deport_uncheck",
                    "icon_noshow_check"
                ];
            }
            else {
                scheduler.config.icons_select = [
                    "icon_arrive_uncheck",
                    "icon_deport_uncheck",
                    "icon_noshow_uncheck"
                ];
            }
            scheduler.locale.labels.icon_arrive_uncheck = "Arrive";
            scheduler.locale.labels.icon_deport_uncheck = "Depart";
            scheduler.locale.labels.icon_noshow_uncheck = "No Show";
            scheduler.locale.labels.icon_arrive_check = "Arrive";
            scheduler.locale.labels.icon_deport_check = "Depart";
            scheduler.locale.labels.icon_noshow_check = "No Show";
            //var caption = "Change Appointment Status!"
            //var msg = "This action will change the Appointment Status to ##. Continue!";
            scheduler._click.buttons.arrive_uncheck = function (id) {
                UpdateAppointmentStatus(sId, "13");
                //OpenConfirmPopupWithTwoId("13", sId, caption, msg.replace("##", "'Patient Checks In'"), UpdateAppointmentStatus, null);
            };
            scheduler._click.buttons.deport_uncheck = function (id) {
                //OpenConfirmPopupWithTwoId("15", sId, caption, msg.replace("##", "'Encounter Ends'"), UpdateAppointmentStatus, null);
                UpdateAppointmentStatus(sId, "15");
            };
            scheduler._click.buttons.noshow_uncheck = function (id) {
                //OpenConfirmPopupWithTwoId("5", sId, caption, msg.replace("##", "'No Show'"), UpdateAppointmentStatus, null);
                UpdateAppointmentStatus(sId, "5");
            };
            scheduler._click.buttons.arrive_check = function (id) {
                //OpenConfirmPopupWithTwoId("1", sId, caption, msg.replace("##", "'Initial'"), UpdateAppointmentStatus, null);
                UpdateAppointmentStatus(sId, "1");
            };
            scheduler._click.buttons.noshow_check = function (id) {
                //OpenConfirmPopupWithTwoId("1", sId, caption, msg.replace("##", "'Initial'"), UpdateAppointmentStatus, null);
                UpdateAppointmentStatus(sId, "1");
            };
            scheduler._click.buttons.deport_check = function (id) {
                //OpenConfirmPopupWithTwoId("1", sId, caption, msg.replace("##", "'Initial'"), UpdateAppointmentStatus, null);
                UpdateAppointmentStatus(sId, "1");
            };
            return true;
        });

    scheduler.templates.tooltip_text = function (start, end, event) {
        var toolHtml = "";
        if (event.PatientName != null) {
            toolHtml = "<h4> " +
                event.TypeOfVisit +
                "</h4>";

            toolHtml += "<div class='row'><div class='col-sm-5'><b>Appointment Type: </b></div><div class='col-sm-7'><p class='event_description'>" +
                event.TypeOfVisit +
                "</p></div></div>";

            toolHtml += "<div class='row'><div class='col-sm-5'><b>Physician Name: </b></div><div class='col-sm-7'>" +
                event.PhysicianName +
                "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Patient Name: </b></div><div class='col-sm-7'>" +
                event.PatientName +
                "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Scheduled Date: </b></div><div class='col-sm-7'>" +
                start.toLocaleDateString() +
                "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Time Slot: </b></div><div class='col-sm-7'>" +
                format(start) +
                " - " +
                format(end) +
                "</div></div>";


            if (event.RoomAssignedSTR != null && event.RoomAssignedSTR != '') {
                toolHtml += "<div class='row'><div class='col-sm-5'><b>Room Assigned: </b></div><div class='col-sm-7'>" +
                    event.RoomAssignedSTR +
                    "</div></div>";
            }


            if (event.text != null && event.text != '') {
                toolHtml += "<div class='row'><div class='col-sm-5'><b>Description: </b></div><div class='col-sm-7'>" +
                    event.text +
                    "</div></div>";
            }
        }
        else {
            toolHtml = "<h4> " +
                event.TypeOfVisit +
                "</h4>" +
                "<div class='row'><div class='col-sm-5'><b>Faculty Name: </b></div><div class='col-sm-7'>" +
                event.PhysicianName +
                "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Scheduled Date: </b></div><div class='col-sm-7'>" +
                start.toLocaleDateString() +
                "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Time Slot: </b></div><div class='col-sm-7'>" +
                format(start) +
                " - " +
                format(end) +
                "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Description: </b></div><div class='col-sm-7'><p class='event_description'>" +
                event.TypeOfVisit +
                "</p></div></div>";
        }
        return toolHtml;
    };
    scheduler.locale.labels.confirm_deleting = null;


    scheduler.attachEvent("onBeforeDrag",
        function (id) {
            return false;
        });
    scheduler.attachEvent("onBeforeLightbox",
        function (ev) {

            var eventDetails = scheduler.getEvent(ev);
            if (eventDetails.Availability == "6") {
                return false;
            }
            return true;
        });

    scheduler.attachEvent("onLightbox",
        function (ev) {
            LightBoxCode(ev);
        });

    //scheduler.attachEvent("onEventAdded",
    //    function (id, ev) {
    //        SaveSchedularData(id, ev);
    //    });

    scheduler.attachEvent("onEventChanged",
        function (id, ev) {
            $("#hidSchedulingType").val(ev.SchedulingType);
            var datesObj = scheduler.getRecDates(id);
            SaveSchedularData(id, ev);
        });

    scheduler.attachEvent("onBeforeEventDelete",
        function (id, ev) {
            $(".tooltip").hide();
            if (ev.SchedulingType == "2") {
                $("#divMultipleDeletePopup").show();
                $("#btnDeleteSeries")
                    .click(function (e) {
                        DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
                    });
                $("#btnDeleteOccurrence")
                    .click(function (e) {
                        $("#rbHolidaySingle").prop("checked", true);
                        DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
                    });
            } else {
                DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
            }
        });

    scheduler.attachEvent("onEventDeleted",
        function (id, ev) {
            //DeleteSchedulerEvent(id, ev);
            //DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
        });

    scheduler.attachEvent("onTemplatesReady",
        function () {
            scheduler.date.week_unit_start = scheduler.date.week_start;
            scheduler.templates.week_unit_date = scheduler.templates.week_date;
            scheduler.templates.week_unit_scale_date = scheduler.templates.week_scale_date;
            scheduler.date.add_week_unit = function (date, inc) { return scheduler.date.add(date, inc * 7, "day"); };
            scheduler.date.get_week_unit_end = function (date) { return scheduler.date.add(date, 5, "day"); };
            scheduler.config.start_on_monday = true;
            scheduler.config.time_step = 15;
            scheduler.xy.min_event_height = 21;
            scheduler.config.hour_size_px = 88;
            scheduler.config.lightbox_recurring = ""; //To remove edit confirmation box
            scheduler.templates.event_text = function (start, end, event) {
                return "<b>" + event.TypeOfVisit + "</b>";
            }
        });
    //$('.schduler_content').empty();
    if (firstTimeLoad == false) {
        if (sections.length > 0) {
            $("#btnScheduleAppointment").prop("disabled", false);
            //$("#btnAddHoliday").prop("disabled", false);
            scheduler.init("scheduler_here", new Date($("#divDatetimePicker").val()), "single_unit");
        } else {
            $("#btnScheduleAppointment").prop("disabled", true);
            //$("#btnAddHoliday").prop("disabled", true);
            scheduler.init("scheduler_here", new Date($("#divDatetimePicker").val()), "week");

        }
        firstTimeLoad = true;
    }

    if (jsonData != null && jsonData.length > 0) {
        scheduler.clearAll();
        scheduler.parse(jsonData, "json");

    } else {
        scheduler.clearAll();
        scheduler.parse(null, "json");
    }
    var selectedViewname = $("#hidSelectedViewName").val();
    switch (selectedViewname) {
        case "Day":
            if (sections.length > 0) {
                scheduler.updateView(new Date($("#divDatetimePicker").val()), "single_unit");
                $("#hidSelectedView").val("1");
                $("#hidSelectedViewName").val("Day");
                $("#treeviewCalenderType li").removeClass("active");
                $("#liviewCalenderType_1").addClass("active");
            } else {
                SetSelectedViewAsWeek();
                scheduler.updateView(new Date($("#divDatetimePicker").val()), "week");
            }
            break;
        case "Week":
            scheduler.updateView(new Date($("#divDatetimePicker").val()), "week");
            break;
        case "Month":
            scheduler.updateView(new Date($("#divDatetimePicker").val()), "month");
            break;
        case "Year":
            scheduler.config.year_y = 3;
            scheduler.updateView(new Date($("#divDatetimePicker").val()), "year");
            break;
        default:
    }
    if (sections.length > 0) {
        $("#btnScheduleAppointment").prop("disabled", false);
        //$("#btnAddHoliday").prop("disabled", false);
    } else {
        $("#btnScheduleAppointment").prop("disabled", true);
        //$("#btnAddHoliday").prop("disabled", true);
    }
    scheduler.config.scroll_hour = (new Date).getHours();
}

//***************************--Scheduler Tool Initialization--**************************************

function UpdateAppointmentStatus(schedulingId, status) {
    var filters = BindFiltersOnly();
    //var jsonData = { status: $("#hfGlobalConfirmFirstId").val(), id: $("#hfGlobalConfirmedSecondId").val(), filters: filters };
    var jsonData = { status: status, id: schedulingId, filters: filters };

    $.ajax({
        type: "POST",
        url: schedulerUrl + "UpdateAppointmentStatus",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            var msg = "Appointment Status updated Successfully!";
            var caption = "Success";
            var captionType = "success";
            if (data == null || data == "") {
                caption = "Error";
                captionType = "error";
                msg = "Error while Updating the Appointment Status";
            }
            else {
                var eventId = data.id;

                var newStatus = getStatusCssClass(data.Availability, eventId);
                $("div[event_id=" + eventId + "]").removeAttr("class").attr("class", "dhx_cal_event " + newStatus);

                //cancelled_booking
                //IntializeScheduler(data, filters[0].PhysicianId, "1");
            }
            ShowMessage(msg, caption, captionType, true);
            $("#hfGlobalConfirmFirstId").val('');
            $("#hfGlobalConfirmedSecondId").val('');

        },
        error: function (msg) {
        }
    });
}

var getStatusCssClass = function (StatusId, eventId) {
    var result = "";
    var newClass = "dhx_menu_icon ";

    switch (StatusId) {
        case "1":
            result = "patient_initial_booking";

            $("div[event_id=" + eventId + "]").find(".icon_arrive_check").removeClass("icon_arrive_check").addClass("icon_arrive_uncheck");
            $("div[event_id=" + eventId + "]").find(".icon_noshow_check").removeClass("icon_noshow_check").addClass("icon_noshow_uncheck");
            $("div[event_id=" + eventId + "]").find(".icon_deport_check").removeClass("icon_deport_check").addClass("icon_deport_uncheck");
            break;
        case "2":
            result = "color_code3";
            break;
        case "3":
            result = "approved_booking";
            break;
        case "4":
            result = "cancelled_booking";
            break;
        case "5":
            result = "patient_expired_notbooking";

            var div = $("div[event_id=" + eventId + "]").find(".icon_noshow_uncheck");
            if (div != null && div != undefined) {
                newClass += "icon_noshow_check";
                div.removeClass("icon_noshow_uncheck").addClass(newClass);

                $("div[event_id=" + eventId + "]").find(".icon_arrive_check").removeClass("icon_arrive_check").addClass("icon_arrive_uncheck");

                $("div[event_id=" + eventId + "]").find(".icon_deport_check").removeClass("icon_deport_check").addClass("icon_deport_uncheck");
            }
            else {
                newClass += "icon_noshow_uncheck";
                $("div[event_id=" + eventId + "]").find(".dhx_menu_icon .icon_noshow_check").removeClass("icon_noshow_check").addClass(newClass);
            }

            break;
        case "6":
            result = "color_code4";
            break;
        case "7":
            result = "confirmed_leave";
            break;
        case "8":
            result = "approved_leave";
            break;
        case "9":
            result = "color_code6";
            break;
        case "10":
            result = "not_approved_leave";
            break;
        case "11":
            result = "color_code11";
            break;
        case "12":
            result = "color_code5";
            break;
        case "13":
            result = "Patient_ChecksIn";

            var div = $("div[event_id=" + eventId + "]").find(".icon_arrive_uncheck");
            if (div != null && div != undefined) {
                newClass += "icon_arrive_check";
                div.removeClass("icon_arrive_uncheck").addClass(newClass);
                $("div[event_id=" + eventId + "]").find(".icon_noshow_check").removeClass("icon_noshow_check").addClass("icon_noshow_uncheck");
                $("div[event_id=" + eventId + "]").find(".icon_deport_check").removeClass("icon_deport_check").addClass("icon_deport_uncheck");
            }
            else {
                newClass += "icon_arrive_uncheck";
                $("div[event_id=" + eventId + "]").find(".dhx_menu_icon .icon_arrive_check").removeClass("icon_arrive_check").addClass(newClass);

            }
            break;
        case "14":
            result = "Service_Administered";
            break;
        case "15":
            result = "Encounter_Ends";

            var div = $("div[event_id=" + eventId + "]").find(".icon_deport_uncheck");
            if (div != null && div != undefined) {
                newClass += "icon_deport_check";

                div.removeClass("icon_deport_uncheck").addClass(newClass);
                $("div[event_id=" + eventId + "]").find(".icon_arrive_check").removeClass("icon_arrive_check").addClass("icon_arrive_uncheck");
                $("div[event_id=" + eventId + "]").find(".icon_noshow_check").removeClass("icon_noshow_check").addClass("icon_noshow_uncheck");
            }
            else {
                newClass += "icon_deport_uncheck";
                $("div[event_id=" + eventId + "]").find(".dhx_menu_icon .icon_deport_check").removeClass("icon_deport_check").addClass(newClass);
            }

            break;
        default:
            result = "patient_initial_booking";
    }
    return result;
};
var clearSelectedPatient = function () {
    $("#spnSelectedPatient").html("");
    $("#btnClearPatient").hide();
    onCheckFilters();
};
var PatientSearchPopupOpen = function () {
    $(".EmiratesMask").mask("999-99-9999");
    var ButtonKeys = { "EnterKey": 13 };
    $(".white-bg")
        .keypress(function (e) {
            if (e.which == ButtonKeys.EnterKey) {
                $("#btnSearch").click();
            }
        });
    $("#collapseSerachResult").removeClass("in");
    $("#collapsePatientSearch").addClass("in");
    BindCountryDataWithCountryCode("#ddlCountries", "#hdCountry", "#lblCountryCode");
};

/*Method is used to the set the apointments to time by changing appointments from time*/
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
            /*ShowMessage('Selected time must be greater than the present time!', "Warning", "warning", true);
            tpFrom.value('');
            tpTo.value('');
            return false;*/
        }
    }
    /****************************************************************************/
    var startTime = tpFromVal; //start.value();

    if (startTime) {
        startTime = new Date(startTime);
        //tpTo.max(startTime);
        startTime.setMinutes(startTime.getMinutes() + timeInterval);
        tpTo.min(startTime);
        tpTo.value(startTime);
    }
};
/// <var>The get checked check boxes</var>
var GetCheckedCheckBoxes = function (divid) {
    var selected = [];
    $("#" + divid + " input:checked")
        .each(function () {
            selected.push($(this).attr("name"));
        });
    return selected;
};

function checkedNodeIds1(nodes, checkedNodes1) {
    /// <summary>
    ///     Checkeds the node ids1.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    /// <param name="checkedNodes1">The checked nodes1.</param>
    /// <returns></returns>
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].checked) {
            checkedNodes1.push(nodes[i].id);
        }
    }
}

// show checked node IDs on datasource change
function onCheckFilters() {
    var jsonData = BindFiltersOnly();

    //$("#spnSelectedPatient").html("");
    //apptRecArray = []; //Clear reccurrence array
    //var checkedPhysician = [],
    //    treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    //var checkedStatusNodes = [],
    //    treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    //var checkedDepartments = [],
    //    treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");

    //var checkedRoom = [],
    //    treeViewRoom = $("#treeviewFacilityRooms").data("kendoTreeView");

    //GetCheckedCheckBoxesTreeView("treeviewStatusCheck", checkedStatusNodes);
    //GetCheckedCheckBoxesTreeView("treeviewPhysician", checkedPhysician);
    //GetCheckedCheckBoxesTreeView("treeviewFacilityDepartment", checkedDepartments);
    //GetCheckedCheckBoxesTreeView("treeviewFacilityRooms", checkedRoom);

    //BindSchedularWithFilters(checkedPhysician, checkedStatusNodes, checkedDepartments, checkedRoom);
    BindSchedularWithFilters(jsonData);
}

function onCheckLocation(e) {
    $("#spnSelectedPatient").html("");
    var t = $("#treeviewLocations").data("kendoTreeView");
    var node = e.node;
    var locationname = this.text(e.node);
    var nodeValueId = $(node).closest("li").data("id");
    $("#hidFacilityName").val(locationname);
    $("#hidFacilitySelected").val(nodeValueId);

    if (nodeValueId > 0)
        BindLocationDataInScheduler(nodeValueId, true, true);

    var checkedPhysician = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");
    checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysician);
    if (checkedPhysician.length > 0) {
        $("#btnScheduleAppointment").prop("disabled", false);
        //$("#btnAddHoliday").prop("disabled", false);
        onCheckFilters();
    } else {
        $("#btnScheduleAppointment").prop("disabled", true);
        //$("#btnAddHoliday").prop("disabled", true);
        scheduler.clearAll();
        scheduler.updateView(new Date($("#divDatetimePicker").val()), "week");
        ShowMessage("There is no Physician available for this facility!", "Warning", "warning", true);
    }
}

/// <var>The bind schedular with filters</var>
var BindSchedularWithFilters = function (jsonData) {
    var phIds = jsonData != null && jsonData.length > 0 ? jsonData[0].PhysicianId : [];
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "GetSchedulerDataUpdated",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            IntializeScheduler(data, phIds, "1");
        },
        error: function (msg) {
        }
    });
};

var html = function (id) { return document.getElementById(id); }; //just a helper

function save_form() {
    /// <summary>
    ///     Save_forms this instance.
    /// </summary>
    /// <returns></returns>
    var ev = scheduler.getEvent(scheduler.getState().lightbox_id);
    ev.text = html("description").value;
    //ev.Availability = html("Availability").value;
    scheduler.endLightbox(true, html("my_form"));
}

function close_form() {
    /// <summary>
    ///     Close_forms this instance.
    /// </summary>
    /// <returns></returns>
    scheduler.endLightbox(false, html("my_form"));
}

function delete_event() {
    /// <summary>
    ///     Delete_events this instance.
    /// </summary>
    /// <returns></returns>
    var event_id = scheduler.getState().lightbox_id;
    scheduler.endLightbox(false, html("my_form"));
    scheduler.deleteEvent(event_id);
}

var SaveSchedularData = function (id, ev) {
    if (firstTimeSave) { // ----- this is to avoid multiple save of an Event
        firstTimeSave = false;
        var jsonData = [];
        if (ev.rec_type == null && ev.rec_type !== "") {
            var recurancedates = scheduler.getRecDates(id);
            if (recurancedates.length > 1) {
                for (var i = 0; i < recurancedates.length; i++) {
                    jsonData[i] = {
                        SchedulingId: ev.TimeSlotId,
                        AssociatedId: 0,
                        AssociatedType: "0",
                        SchedulingType: $("#hidSchedulingType").val(),
                        Status: ev.Availability,
                        StatusType: "0",
                        ScheduleFrom: recurancedates[i].start_date,
                        ScheduleTo: recurancedates[i].end_date,
                        PhysicianId: ev.section_id,
                        TypeOfProcedure: ev.VisitType,
                        PhysicianSpeciality: $("#hfPhysicianSpeciality").val(),
                        FacilityId: $("#hidFacilitySelected").val(),
                        //CorporateId: $('#ddlCorporate').val(),
                        Comments: ev.text,
                        Location: $("#hidFacilityName").val(),
                        ConfirmedByPatient: "1", //set it by default
                        IsRecurring: ev.rec_type == null && ev.rec_type !== "" ? false : ev._timed,
                        RecurringDateFrom: ev.start_date,
                        RecurringDateTill: ev.end_date,
                        EventId: id,
                        RecType: ev.rec_type,
                        RecPattern: ev.rec_pattern,
                        RecEventlength: ev.event_length,
                        RecEventPId: ev.event_pid,
                        WeekDay: ""
                    };
                }
            }
        } else {
            jsonData[0] = ({
                SchedulingId: ev.TimeSlotId,
                AssociatedId: 0,
                AssociatedType: "0",
                SchedulingType: $("#hidSchedulingType").val(),
                Status: ev.Availability,
                StatusType: "0",
                ScheduleFrom: ev.start_date,
                ScheduleTo: ev.end_date,
                PhysicianId: ev.section_id,
                TypeOfProcedure: ev.VisitType,
                PhysicianSpeciality: $("#hfPhysicianSpeciality").val(),
                FacilityId: $("#hidFacilitySelected").val(),
                Comments: ev.text,
                Location: $("#hidFacilityName").val(),
                ConfirmedByPatient: "1", //set it by default
                IsRecurring: ev.rec_type == null && ev.rec_type !== "" ? false : ev._timed,
                RecurringDateFrom: ev.start_date,
                RecurringDateTill: ev.end_date,
                EventId: id,
                RecType: ev.rec_type,
                RecPattern: ev.rec_pattern,
                RecEventlength: ev.event_length,
                RecEventPId: ev.event_pid,
                WeekDay: ""
            });
        }
        var jsonD = JSON.stringify(jsonData);
        $.ajax({
            type: "POST",
            url: "/PatientScheduler/SavePatientScheduling",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonD,
            success: function (data) {
                ShowMessage("Records Saved Succesfully!", "Success", "success", true);
            },
            error: function (msg) {
            }
        });
    }
};

var DeleteSchedulerEvent = function (id, ev) {
    var jsonData = JSON.stringify({
        SchedulingId: ev.TimeSlotId
    });
    $.ajax({
        type: "POST",
        url: "/PatientScheduler/DeletePatientScheduling",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data == true) {
                ShowMessage("Records deleted Succesfully!", "Success", "success", true);
            } else {
                ShowMessage("Unable to delete record!", "Error", "error", true);
            }
            scheduler.updateView();
        },
        error: function (msg) {
        }
    });
};

var GetSchedularCustomData = function (type) {
    var jsonData = [];
    var physicianData = [];
    var checkedPhysician = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");
    checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysician);
    var selectedLocation = $("#hidFacilitySelected").val();
    //------------Check for the Selected Physciain Ids
    if (checkedPhysician.length > 0) {
        for (var i = 0; i < checkedPhysician.length; i++) {
            physicianData[i] = {
                Id: checkedPhysician[i]
            };
        }
    } else {
        physicianData[0] = {
            Id: 0
        };
    }
    jsonData[0] = {
        PhysicianId: physicianData,
        StatusType: null,
        SelectedDate: $("#divDatetimePicker").val(),
        Facility: selectedLocation,
        ViewType: type
    };

    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "GetCustomSchedular",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {

            IntializeScheduler(data, physicianData);
        },
        error: function (msg) {
        }
    });
};
var AddDepartmentTimming = function (data) {
    if (data.length > 0) {

        for (var j = 0; j < data.length; j++) {
            var arrayObj = [];
            arrayObj.push(data[j].physicianid);
            scheduler.addMarkedTimespan({
                //days: new Date(data[j].start_date),
                type: "dhx_time_block",
                zones: "fullday",
                days: 0,
                sections: { unit: 2036 }
                //sections: { unit: arrayObj }
            });
            scheduler.deleteMarkedTimespan({
                //days: new Date(data[j].start_date),
                zones: [parseInt(data[j].dept_Openingtime) * 60, parseInt(data[j].dept_Closingtime) * 60],
                sections: { unit: 2036 }
            });
        }
        scheduler.config.scroll_hour = (new Date).getHours();
        scheduler.updateView();
    }
};

function onCheckDepartment() {
    var checkedDepartments = [],
        treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    var checkedPhysicianNodes = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    var checkedRoom = [],
        treeViewRoom = $("#treeviewFacilityRooms").data("kendoTreeView");

    //checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysicianNodes);
    //checkedNodeIds1(treeViewDepartments.dataSource.view(), checkedDepartments);
    //checkedNodeIds1(treeViewStatus.dataSource.view(), checkedStatusNodes);

    GetCheckedCheckBoxesTreeView("treeviewPhysician", checkedPhysicianNodes);
    GetCheckedCheckBoxesTreeView("treeviewFacilityDepartment", checkedDepartments);
    GetCheckedCheckBoxesTreeView("treeviewFacilityRooms", checkedRoom);
    GetCheckedCheckBoxesTreeView("treeviewStatusCheck", checkedStatusNodes);
    //00------- Commented for now but we need it may be later to filter the rooms according to the department wise---------
    //BindDepartmentRooms(checkedDepartments);


    BindSchedularDataByDeparment(checkedPhysicianNodes, checkedStatusNodes, checkedDepartments, checkedRoom);
    //BindSchedularWithDeptFilters(checkedPhysicianNodes, checkedDepartments, checkedStatusNodes);
}

var onCheckView = function (e) {
    var node = e.node;
    var selectedViewname = this.text(e.node);
    var selectedViewValue = $(node).closest("li").data("id");
    $("#hidSelectedView").val(selectedViewValue);
    $("#hidSelectedViewName").val(selectedViewname);
    onCheckFilters();
};
var BindSchedularWithDeptFilters = function (phySlected, deptIds, statusfilter) {
    var jsonData = [];
    var deptData = [];
    var phyData = [];
    var statusData = [];
    //------------Check for the Selected Physciain Ids
    if (phySlected.length > 0) {
        for (var i = 0; i < phySlected.length; i++) {
            phyData[i] = {
                Id: phySlected[i]
            };
        }
    } else {
        phyData[0] = {
            Id: 0
        };
    }
    //-------------- Selected Department Id check
    if (deptIds.length > 0) {
        for (var k = 0; k < deptIds.length; k++) {
            deptData[k] = {
                Id: deptIds[k]
            };
        }
    } else {
        deptData[0] = {
            Id: 0
        };
    }
    //------------Check for the Selected Status
    if (statusfilter.length > 0) {
        for (var j = 0; j < statusfilter.length; j++) {
            statusData[j] = {
                Id: statusfilter[j]
            };
        }
    } else {
        statusData[0] = {
            Id: 0
        };
    }
    jsonData[0] = {
        PhysicianId: phyData,
        StatusType: statusData,
        SelectedDate: $("#divDatetimePicker").val(),
        Facility: $("#hidFacilitySelected").val(),
        ViewType: $("#hidSelectedView").val(),
        DeptData: deptData,
        ShowAllRooms: true
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "GetSchedulerDataUpdated",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {

            //if (deptData[0].Id !== 0) {
            //    IntializeScheduler(data, deptData, "2");
            //} else {
            //    IntializeScheduler(data, phyData, "1");
            //}
        },
        error: function (msg) {
        }
    });
};
var BindAppointmentAvailability = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetGlobalCodesAvailability",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            ggcValue: "4903"
        }),
        success: function (data) {
            /*Bind Availability dropdown - start*/
            var appointmentHTML = '<option value="0">--Select--</option>';
            for (var i = 0; i < data.gClist.length; i++) {
                appointmentHTML += '<option value="' +
                    data.gClist[i].GlobalCodeValue +
                    '">' +
                    data.gClist[i].GlobalCodeName +
                    "</option>"; // Availability -  Field
            }
            $("#ddAvailability").html(appointmentHTML);
            $("#ddAvailability").val("1"); // To select "Initial Booking" by default
            /*Bind Availability dropdown - end*/

            /*Bind Physician dropdown - start*/
            appointmentHTML = '<option value="0">--Select--</option>';
            for (var j = 0; j < data.physicians.length; j++) {
                appointmentHTML += '<option value="' +
                    data.physicians[j].Physician.Id +
                    '" facilityId="' +
                    data.physicians[j].Physician.FacilityId +
                    '" department="' +
                    data.physicians[j].UserDepartmentStr +
                    '" departmentId="' +
                    data.physicians[j].Physician.FacultyDepartment +
                    '" speciality="' +
                    data.physicians[j].UserSpecialityStr +
                    '" specialityId="' +
                    data.physicians[j].Physician.FacultySpeciality +
                    '">' +
                    data.physicians[j].Physician.PhysicianName +
                    "(" +
                    data.physicians[j].UserTypeStr +
                    ")</option>"; // Availability -  Field
            }
            $("#ddPhysician").html(appointmentHTML);
            $("#ddHolidayPhysician").html(appointmentHTML); /*Bind holiday's physician drop down*/
            /*Bind Physician dropdown - end*/

            /*Bind holiday status dropdown - start*/
            appointmentHTML = '<option value="0">--Select--</option>';
            for (var k = 0; k < data.hStatus.length; k++) {
                appointmentHTML += '<option value="' +
                    data.hStatus[k].GlobalCodeValue +
                    '">' +
                    data.hStatus[k].GlobalCodeName +
                    "</option>";
            }
            $("#ddHolidayStatus").html(appointmentHTML);
            $("#ddHolidayStatus").val("7"); // To select "Initial Booking" by default
            /*Bind holiday status dropdown - end*/

            /*Bind holiday types dropdown - start*/
            appointmentHTML = '<option value="0">--Select--</option>';
            for (var x = 0; x < data.hTypes.length; x++) {
                appointmentHTML += '<option value="' +
                    data.hTypes[x].GlobalCodeValue +
                    '">' +
                    data.hTypes[x].GlobalCodeName +
                    "</option>";
            }
            $("#ddHolidayType").html(appointmentHTML);
            /*Bind holiday types dropdown - end*/
        },
        error: function (msg) {
        }
    });
};

var SetDepartmentAndSpeciality = function (e) {
    var deptt = $(e).find("option:selected").attr("departmentId");
    var spec = $(e).find("option:selected").attr("specialityId");

    if (spec != null && $("#ddSpeciality").val() <= 0)
        $("#ddSpeciality").val(spec);

    if (deptt != null && $("#ddlDepartment").val() <= 0)
        $("#ddlDepartment").val(deptt);
};

function BindAppointmentTypes(facilityDDId) {
    $.ajax({
        type: "POST",
        url: "/AppointmentTypes/GetAppointmentTypesList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            facilityId: $(facilityDDId).val()
        }),
        success: function (data) {

            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data,
                    function (i, appt) {
                        items += "<option value='" +
                            appt.Id +
                            "' timeslot='" +
                            appt.TimeSlot +
                            "'>" +
                            appt.Name +
                            "</option>";
                    });
                if (facilityDDId == "#ddFacility") {
                    $("#appointmentTypes").html("");
                    $("#appointmentTypes").html(items);
                } else {
                    $("#selOVApptTypes").html("");
                    $("#selOVApptTypes").html(items);
                }
            } else {
            }
        },
        error: function (msg) {
        }
    });
}


function AddAppointmentTypesTimeSlot() {
    var isMultipleChecked = $("#rbMultiple").prop("checked");
    var isSingleChecked = $("#rbSingle").prop("checked");
    var ctrl = $("#tbApptTypesList");
    var apptTypeName = $("#appointmentTypes option:selected").text();
    var apptTypeValue = $("#appointmentTypes option:selected").val();
    apptTypeValue = apptTypeValue != null ? apptTypeValue : $("#selOVApptTypes option:selected").val();
    apptTypeName = apptTypeName != "" ? apptTypeName : $("#selOVApptTypes option:selected").text();
    if (apptTypeValue == undefined) {
        ShowMessage("Please select facility from the top and then appointment!", "Warning", "warning", true);
        return false;
    }
    if (apptTypeValue == "0") {
        ShowMessage("Please select atleast one appointments!", "Warning", "warning", true);
        return false;
    }
    var obj = $("#date" + apptTypeValue);
    if (obj.length > 0) {
        ShowMessage("Appointment alreay added in the below listing!", "Warning", "warning", true);
        return false;
    }
    var aptTypesListCount = $("#tbApptTypesList tr").length;
    if (isSingleChecked && aptTypesListCount == 1) {
        ShowMessage("For multiple Appointment you have to click Multiple procedure at the top!",
            "Warning",
            "warning",
            true);
        return false;
    }

    var html = "";
    html += "<tr id='tr" + apptTypeValue + "'><input type='hidden' id='hfMain" + apptTypeValue + "' value='0'/>";
    html += "<input type='hidden' id='hfClinician" + apptTypeValue + "' value='0'/>";
    html += "<td id='td" + apptTypeValue + "'>" +
        apptTypeName +
        "</td><input type='hidden' id='hfProdTimeSlot" +
        apptTypeValue +
        "'  value='" +
        $("#hfTimeSlot").val() +
        "'/>";
    html += "<td><input id='date" +
        apptTypeValue +
        "' type='text' class='validate[required]'/>";

    html += "<input type='button' id='btnShowTimeSlots' class='btn btn-sm btn-primary marginLR10' style='height:28px;' value='View Slots' onclick='OnChangeAppointmentDate(this," +
        apptTypeValue + ")'/>";

    /* => View Nearest/Closest Time-Slots */
    html += "<input type='button' id='btnViewNearestTime'  class='btn btn-sm btn-primary marginLR10' value='First Available' onclick='BindClosestAvailableTimeSlots(this," +
        apptTypeValue + ")'/></td>";
    /* => View Nearest/Closest Time-Slots */

    html += "<td><input id='timef" + apptTypeValue + "' type='text' class='validate[required]'/></td>";




    html += "<td><input id='timet" +
        apptTypeValue +
        "' type='text' class='validate[required]'/><input type='hidden' id='hfDone" +
        apptTypeValue +
        "' value='0'/></td>";
    html += "<td class='recurrence_event'><input id='chkIsRec" +
        apptTypeValue +
        "' freqtypeattr='' type='checkbox' onchange='OnChangeIsReccurrenceChk(this," +
        apptTypeValue +
        ");'/><input type='button' id='btnEditRecurrence" +
        apptTypeValue +
        "' class='btn btn-xs btn-default' style='margin-left:5px;display:none;' value='Edit Recurrence' statusattr='new' freqtypeattr='' onclick='OnChangeIsReccurrenceChk(this," +
        apptTypeValue +
        ");'/></td>";
    html += "<td><img src='/images/delete.png' width='15px' onclick='RemoveAppointmentProcedures(" +
        apptTypeValue +
        ")'/></td>";
    html += "</tr>";
    ctrl.append(html);

    /*for (var j = 0; j < apptTypeIdArray.length; j++) {
        $("#date" + apptTypeIdArray[j]).datetimepicker({
            format: 'm/d/Y',
            minDate: '1901/12/12',
            maxDate: '2025/12/12',
            timepicker: false,
            closeOnDateSelect: true
        });
    }*/
    $("#date" + apptTypeValue)
        .datetimepicker({
            format: "m/d/Y",
            minDate: 0,
            timepicker: false,
            closeOnDateSelect: true,
            onChangeDateTime: function (e) {
                $("#timef" + apptTypeValue).val("");
                $("#timet" + apptTypeValue).val("");
            }
        });
    /* $("#timef" + apptTypeValue).datetimepicker({
         datepicker: false,
         format: 'H:i',
         step: parseInt($("#hfTimeSlot").val()),
         mask: false,
     });*/
    $("#timef" + apptTypeValue)
        .kendoTimePicker({
            interval: parseInt($("#hfTimeSlot").val()),
            format: "HH:mm",
            min: new Date(2017, 10, 01, firstHour, 00, 00),
            max: new Date(2017, 10, 01, lastHour, 00, 00),
            change: function (e) {
                startChange("#timef" + apptTypeValue, "#timet" + apptTypeValue, parseInt($("#hfTimeSlot").val()));
            }
        })
        .data("kendoTimePicker");
    //.readonly();

    $("#timet" + apptTypeValue)
        .kendoTimePicker({
            min: new Date(2017, 10, 01, firstHour, 00, 00),
            max: new Date(2017, 10, 01, lastHour, 00, 00),
            interval: parseInt($("#hfTimeSlot").val()),
            format: "HH:mm"
        })
        .data("kendoTimePicker");
    //.readonly();

    var hfApptTypes = $("#hfAppointmentTypes").val();
    var concatApptTypes = hfApptTypes + "," + apptTypeValue;
    $("#hfAppointmentTypes").val(concatApptTypes.replace(/^,|,$/g, ""));

    $("#appointmentTypes").val("0");

    var date = new Date();
    $("#date" + apptTypeValue).val(date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear());

    //adding today's as by default
    var initialDate = "";
    if ($("#hfAppointmentTypes").val() != "") {
        var hfAptypesObjValueArray = $("#hfAppointmentTypes").val().split(",");
        initialDate = $("#date" + hfAptypesObjValueArray[0]).val();
        $("#date" + apptTypeValue).val(initialDate);
    }
}

function OnChangeApptTypes(e) {
    var ts = $(e).find("option:selected").attr("timeslot");
    var value = $(e).find("option:selected").attr("value");
    $("#hfTimeSlot").val(ts);
    apptTypeIdArray.push(value);
}

var SaveCustomSchedular = function () {
    var apptRecArray1 = apptRecArray;
    var btnSelection = $("input[name=rdbtnSelection]:checked").val();
    var jsonData = [];
    var actualDValue1 = null;
    var fromDate = null;
    var toDate = null;
    var schUpdateFlag = 0;

    var scheFromDate = null;
    var scheToDate = null;
    var recDateFrom = null;
    var recDateTo = null;
    var recEventLength = 0;
    var selectedPhysicianId = "0";
    var selectedAppointmenttype = $("#hfAppointmentTypes").val();
    if (selectedAppointmenttype != "") {
        if (btnSelection == "2") {
            var appointmenttypeArrayVal = selectedAppointmenttype.split(",");
            if (appointmenttypeArrayVal.length > 1) {
                ShowMessage("For multiple Appointment you have to click Multiple procedure at the top!",
                    "Warning",
                    "warning",
                    true);
                unBlockRecurrenceDiv();
                return;
            } else {
                var isRecurrance = $("#chkIsRec" + selectedAppointmenttype).prop("checked");

                actualDValue1 = $("#date" + selectedAppointmenttype).val().indexOf(" ") != -1 ? $("#date" + selectedAppointmenttype).val()[0] : $("#date" + selectedAppointmenttype).val().trim();
                fromDate = actualDValue1 + " " + $("#timef" + selectedAppointmenttype).val();
                toDate = actualDValue1 + " " + $("#timet" + selectedAppointmenttype).val();
                selectedPhysicianId = $("#hfClinician" + selectedAppointmenttype) != undefined && $("#hfClinician" + selectedAppointmenttype).length > 0 ? $("#hfClinician" + selectedAppointmenttype).val() : "0";

                scheFromDate = isRecurranceMultiple ? apptRecArray1[0].Rec_Start_Date : fromDate;
                scheToDate = isRecurranceMultiple ? apptRecArray1[0].end_By : toDate;
                recDateFrom = null;
                recDateTo = null;
                recEventLength = 0;

                if (isRecurrance) {
                    recDateFrom = fromDate;
                    recDateTo = apptRecArray1[0].end_By;
                    recEventLength = CalculateMilliSecondsBetweenDates(fromDate, toDate);
                }

                /*
                    Just to let the system know that current Appointment Type has been modified so that I could call the Procedure for update
                */
                if ($("#schUpdateFlag" + selectedAppointmenttype).length > 0)
                    schUpdateFlag = $("#schUpdateFlag" + selectedAppointmenttype).val();

                if (clinicianUpdated) {
                    selectedPhysicianId = $("#ddPhysician").val();
                }

                jsonData[0] = ({
                    SchedulingId: $("#hfMain" + selectedAppointmenttype).val(),
                    AssociatedId: $("#hfPatientId").val(),
                    AssociatedType: "1",
                    SchedulingType: $("#hidSchedulingType").val(),
                    Status: $("#ddAvailability").val(),
                    StatusType: "", //$('#ddAvailability').val(),
                    ScheduleFrom: scheFromDate,
                    ScheduleTo: scheToDate,
                    PhysicianId: selectedPhysicianId,//$("#ddPhysician").val(),
                    TypeOfProcedure: $("#hfAppointmentTypes").val(),
                    PhysicianSpeciality: $("#ddSpeciality").val(),//selectedPhysicianId > 0 && selectedPhysicianId == $("#ddPhysician").val() && $("#ddSpeciality").val() > 0 ? $("#ddSpeciality").val() : "0",
                    ExtValue1: $("#ddlDepartment").val(),
                    ExtValue5: $("#txtPhysicianComment").val(),
                    FacilityId: $("#ddFacility").val(),
                    Comments: $("#txtDescription").val(),
                    Location: $("#ddFacility :selected").text(),
                    PhysicianName: $("#ddPhysician :selected").text(),
                    ConfirmedByPatient: "1", //set it by default  Frequency_Type
                    IsRecurring: isRecurrance,
                    RecurringDateFrom: recDateFrom,
                    // apptRecArray1[0].Frequency_Type == 3 ? apptRecArray1[0].Rec_Start_Date : ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()), //isRecurrance ? ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()) : null,
                    RecurringDateTill: recDateTo,
                    //apptRecArray1[0].end_By, // isRecurrance ? apptRecArray1[0].end_By : null,
                    //EventId: id,
                    RecType: isRecurrance ? apptRecArray1[0].Rec_Type : "", //apptRecArray1[0].Rec_Type, //
                    RecPattern: isRecurrance ? apptRecArray1[0].Rec_Pattern : "",
                    //apptRecArray1[0].Rec_Pattern, // isRecurrance ? apptRecArray1[0].Rec_Pattern : null,
                    RecEventlength: recEventLength,
                    //CalculateMilliSecondsBetweenDates(($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()), ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timet' + selectedAppointmenttype).val())),
                    //isRecurrance ? CalculateMilliSecondsBetweenDates(($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()), ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timet' + selectedAppointmenttype).val())) : null,
                    //RecEventPId: isRecurrance ? ev.event_pid,
                    WeekDay: "",
                    EventParentId: $("#hidEventParentId").val(),
                    PatientName: $("#patientname").val(),
                    PatientDOB: $("#txtPersonDOB").val(),
                    PatientEmailId: $("#email").val(),
                    PatientPhoneNumber: $("#lblPatientCountryCode").html() + "-" + $("#phoneno").val(),
                    PatientId: $("#hfPatientId").val(),
                    PatientEmirateIdNumber: $("#txtEmiratesNationalId").val(),
                    MultipleProcedures: false,
                    EventId: schedularEventObj,
                    RemovedAppointmentTypes: $("#hfRemovedAppTypes").val(),
                    RoomAssigned: $("#hfRoomId").val(),
                    PhysicianReferredBy: $("#ddPhysician").val(),
                    UpdateFlag: schUpdateFlag > 0,
                    ClinicianChanged: clinicianUpdated
                });
            }
        }
        else {
            var appointmenttypeArray = selectedAppointmenttype.split(",");
            for (var i = 0; i < appointmenttypeArray.length; i++) {
                var appointmenttypeobj = appointmenttypeArray[i];
                /*
                    Just to let the system know that current Appointment Type has been modified so that I could call the Procedure for update
                */
                if ($("#schUpdateFlag" + selectedAppointmenttype).length > 0)
                    schUpdateFlag = $("#schUpdateFlag" + selectedAppointmenttype).val();


                var isRecurranceMultiple = $("#chkIsRec" + appointmenttypeobj).prop("checked");
                var recuranceArrayObj = null;
                $.each(apptRecArray, function (k1, val) {
                    $.each(val, function (key, name) {
                        if (name == appointmenttypeobj) {
                            recuranceArrayObj = apptRecArray[k1];
                        }
                    });
                });

                selectedPhysicianId = $("#hfClinician" + appointmenttypeobj) != undefined && $("#hfClinician" + appointmenttypeobj).length > 0 ? $("#hfClinician" + appointmenttypeobj).val() : "0";

                actualDValue1 = $("#date" + appointmenttypeobj).val().indexOf(" ") != -1 ? $("#date" + appointmenttypeobj).val()[0] : $("#date" + appointmenttypeobj).val().trim();
                fromDate = actualDValue1 + " " + $("#timef" + appointmenttypeobj).val();
                toDate = actualDValue1 + " " + $("#timet" + appointmenttypeobj).val();


                scheFromDate = isRecurranceMultiple ? recuranceArrayObj.Rec_Start_Date : fromDate;
                scheToDate = isRecurranceMultiple ? recuranceArrayObj.end_By : toDate;
                recDateFrom = null;
                recDateTo = null;
                recEventLength = 0;

                if (isRecurranceMultiple) {
                    recDateFrom = fromDate;
                    recDateTo = recuranceArrayObj.end_By;
                    recEventLength = CalculateMilliSecondsBetweenDates(fromDate, toDate);
                }

                jsonData[i] = ({
                    SchedulingId: $("#hfMain" + appointmenttypeobj).val(),
                    AssociatedId: $("#hfPatientId").val(),
                    AssociatedType: "1",
                    SchedulingType: $("#hidSchedulingType").val(),
                    Status: $("#ddAvailability").val(),
                    StatusType: "",
                    ScheduleFrom: scheFromDate,
                    ScheduleTo: scheToDate,
                    PhysicianId: selectedPhysicianId, //$("#ddPhysician").val(),
                    TypeOfProcedure: appointmenttypeobj,
                    PhysicianSpeciality: $("#ddSpeciality").val(),
                    ExtValue1: $("#ddlDepartment").val(),
                    FacilityId: $("#ddFacility").val(),
                    Comments: $("#txtDescription").val(),
                    Location: $("#ddFacility :selected").text(),
                    ConfirmedByPatient: "1", //set it by default
                    PhysicianName: $("#ddPhysician :selected").text(),
                    ExtValue5: $("#txtPhysicianComment").val(),
                    IsRecurring: isRecurranceMultiple,
                    RecurringDateFrom: recDateFrom,
                    RecurringDateTill: recDateTo,
                    RecType: isRecurranceMultiple ? recuranceArrayObj.Rec_Type : null,
                    RecPattern: isRecurranceMultiple ? recuranceArrayObj.Rec_Pattern : null,
                    RecEventlength: recEventLength,
                    WeekDay: "",
                    EventParentId: $("#hidEventParentId").val(),
                    PatientName: $("#patientname").val(),
                    PatientDOB: $("#txtPersonDOB").val(),
                    PatientEmailId: $("#email").val(),
                    PatientPhoneNumber: $("#lblPatientCountryCode").html() + "-" + $("#phoneno").val(),
                    PatientEmirateIdNumber: $("#txtEmiratesNationalId").val(),
                    MultipleProcedures: true,
                    PatientId: $("#hfPatientId").val(),
                    EventId: schedularEventObj,
                    RemovedAppointmentTypes: $("#hfRemovedAppTypes").val(),
                    RoomAssigned: $("#hfRoomId").val(),
                    PhysicianReferredBy: $("#ddPhysician").val(),
                    UpdateFlag: schUpdateFlag > 0,
                    ClinicianChanged: clinicianUpdated
                });
            }
        }
        var jsonD = JSON.stringify(jsonData);
        $.ajax({
            type: "POST",
            url: schedulerUrl + "SaveAppointment",  //SavePatientSchedulingUpdated
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonD,
            success: function (data) {
                if (data.IsError) {
                    if (data.IsError = true && data.ErrorStatus != null && data.ErrorStatus == -2) {
                        unBlockRecurrenceDiv();
                        ShowMessage(data.errorMessage != "" ? data.errorMessage : "This Email has already been registered at other Facility. Try with the other Email. Thank you!", "Message", "error", true);
                    }
                    else if (data.booked != null && data.booked.length > 0 && data.ErrorStatus != null && data.ErrorStatus == 2) {
                        unBlockRecurrenceDiv();
                        ShowMessage(data.errorMessage != "" ? data.errorMessage : "Already have an appointment at this time!", "Message", "error", true);
                    }
                    else if (data.ErrorStatus != null && data.ErrorStatus != 0) {
                        unBlockRecurrenceDiv();
                        if (data.ErrorStatus == 1)
                            ShowMessage("One of the Appointment Types doesn't belong to the selected Physician. Change the selections. Thank you", "Invalid Appointment!!", "error", true);
                        else
                            ShowMessage("Something wrong with the selections!", "Invalid Appointment!!", "error", true);
                    }
                    else if (data.notAvialableTimeslotslist.length > 0) {
                        for (var j = 0; j < data.notAvialableTimeslotslist.length; j++) {
                            ShowMessage("Unable to book slot for Date :" +
                                data.notAvialableTimeslotslist[j].DateFromSTR +
                                " Time Range:" +
                                data.notAvialableTimeslotslist[j].TimeFromStr +
                                " - " +
                                data.notAvialableTimeslotslist[j].TimeTOStr +
                                " With physician" +
                                data.notAvialableTimeslotslist[j].PhysicianName +
                                " (" +
                                data.notAvialableTimeslotslist[j].PhysicianSpl +
                                ")",
                                "Warning",
                                "warning",
                                true);
                            unBlockRecurrenceDiv();
                        }
                    } else if (data.roomEquipmentnotaviable.length > 0) {
                        for (var k = 0; k < data.roomEquipmentnotaviable.length; k++) {
                            ShowMessage(data.roomEquipmentnotaviable[k].Reason, "Warning", "warning", true);
                            unBlockRecurrenceDiv();
                        }
                    }
                    else if (data.isAllReadyAppointed) {
                        //$('td' + data.appId).prop('color', '#003F87');
                        document.getElementById('td' + data.appId).style.color = '#FF0000';
                        $("#timef" + data.appId).val('');
                        $("#timet" + data.appId).val('');

                        ShowMessage("This patient has already been assigned with this Time-Slot. You can check other Time-Slots and try again. Thank you!", "Alert!!", "warning", true);
                        unBlockRecurrenceDiv();
                    }
                    else if (data.SameTimeApp) {
                        ShowMessage("You con not select appointment with same time", "Alert!!", "warning", true);
                        unBlockRecurrenceDiv();
                    }
                }
                else {
                    ShowMessage("Records Saved Succesfully!", "Success", "success", true);
                    onCheckFilters();
                    //$('.hidePopUp').hide();
                    $.validationEngine.closePrompt(".formError", true);
                    ClearSchedulingPopup();
                }
                $("#hfPatientId").val(data.patientid);
            },
            error: function (msg) {
            }
        });
    } else {
        ShowMessage("Please add atleast one appointment!", "Warning", "warning", true);
        unBlockRecurrenceDiv();
    }
};

function BindPhysicianBySpeciality() {
    $.ajax({
        type: "POST",
        url: "/Physician/BindPhysicianBySpeciality",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            facilityId: $("#ddFacility").val(),
            //departmentId: $("#ddlDepartment").val(),
            specialityId: $("#ddSpeciality").val()
        }),
        success: function (data) {

            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data,
                    function (i, physician) {

                        items += '<option value="' +
                            physician.Physician.Id +
                            '" facilityId="' +
                            physician.Physician.FacilityId +
                            '" department="' +
                            physician.UserDepartmentStr +
                            '" departmentId="' +
                            physician.Physician.FacultyDepartment +
                            '" speciality="' +
                            physician.UserSpecialityStr +
                            '" specialityId="' +
                            physician.Physician.FacultySpeciality +
                            '">' +
                            physician.Physician.PhysicianName +
                            "(" +
                            physician.UserTypeStr +
                            ")</option>"; // Availability -  Field
                        //items += "<option value='" + physician.Id + "' PhysicianLicenseNumber='" + physician.PhysicianLicenseNumber + "'>" + physician.PhysicianName + "</option>";
                    });
                $("#ddPhysician").html(items);
            } else {
            }
        },
        error: function (msg) {
        }
    });
}

var ClearSchedulingPopup = function () {
    var defaultCountryId = $('#hfPatientCountry').val();
    var fId = $("#hidFacilitySelected").val();
    $("#divSchedularPopUpContent").clearForm(true);
    $("#tbList").empty();
    $("#hfAppointmentTypes").val("");
    $("#tbApptTypesList").html("");
    $("#rbSingle").prop("checked", true);
    $("#divPhysicianComment").hide();
    $("#rbHolidaySingle").prop("checked", true);
    $("#divMultipleEventPopup").hide();
    firstTimeBind = true;
    $("#ddHolidayStatus").val("7"); // To select "Initial Booking" by default
    $("#ddAvailability").val("1"); // To select "Initial Booking" by default
    $("#imgPatientExist").hide();
    $("#imgPatientNew").show();
    $("#rbHolidaySingle").prop("disabled", false);
    $("#rbHolidayMultiple").prop("disabled", false);
    $("#divHolidayPhysician").hide();
    $("#divMultipleDeletePopup").hide();
    $(".tooltip").show();
    $("body")
        .css({
            'overflow-y': "auto",
            'padding-left': "0px"
        });
    $("#hidEventParentId").val("");
    $(".searchSlide").removeClass("moveLeft");
    $(".searchSlide1").removeClass("moveLeft");
    unBlockRecurrenceDiv();
    $("#divRecurrenceEventPopup").hide();
    $("#divSchedularPopUp").hide();

    $("#rbMultiple").prop("checked", false);
    $("#rbSingle").prop("checked", true);
    $("#rbMultiple").prop("disabled", false);
    $("#daily").show();
    $("#weekly").hide();
    $("#monthly").hide();
    $("#btnDeleteSchedulingData").hide();
    $("#hfSelPhysicianId").val("0");
    $("#hfExternalValue3").val("False");
    $("#hfRemovedAppTypes").val("");
    $("#hfRoomId").val("");
    $("#ddHolidayPhysician").prop("disabled", false);
    $("#btnSaveSchedulingData").show();
    $("#btnDeleteSchedulingData").show();
    $("#divHolidayToDateTime").hide();
    clinicianUpdated = false;
    $("#ddFacility").val(fId);

    $("#hidFacilitySelected").val(fId);
    $('#ddlPatientCountries').val(defaultCountryId);
    $('#hfPatientCountry').val(defaultCountryId);
};


var BindLightBox = function (obj) {
    $("#btnSaveSchedulingData").val("Update");
    $("#btnDeleteSchedulingData").show();
    var scheduleType = obj.SchedulingType;
    switch (scheduleType) {
        case "1":
            $("#imgPatientExist").show();
            $("#imgPatientNew").hide();
            $("#divSchedularPopUpContent .modal-title").html("Appointments Scheduler");

            $("#divRecurrenceEventPopup").show();
            if (obj.IsRecurrance) {
                $("#btnEditRecurrenceSeries")
                    .click(function (e) {

                        $("#divSchedularPopUp").show();
                        $("#divRecurrenceEventPopup").hide();
                        var procObj = obj.TypeOfProcedureCustomModel;
                        for (var k = 0; k < procObj.length; k++) {
                            $("#date" + procObj[k].TypeOfProcedureId).val(procObj[k].Rec_Start_date);
                        }
                    });
                $("#btnEditRecurrenceOccurrence")
                    .click(function (e) {

                        $("#rbMultiple").prop("checked", false);
                        $("#rbSingle").prop("checked", true);
                        $("#rbMultiple").prop("disabled", true);
                        $("#divSchedularPopUp").show();
                        $("#divRecurrenceEventPopup").hide();
                        $("#hfSchedulingId").val(obj.TimeSlotId);
                        var procObj = obj.TypeOfProcedureCustomModel;
                        for (var k = 0; k < procObj.length; k++) {
                            $("#chkIsRec" + procObj[k].TypeOfProcedureId).prop("checked", false);
                            $("#chkIsRec" + procObj[k].TypeOfProcedureId).prop("disabled", true);
                            $("#btnEditRecurrence" + procObj[k].TypeOfProcedureId).hide();
                        }
                    });
            } else {
                $("#divSchedularPopUp").show();
                $("#divRecurrenceEventPopup").hide();
                var procObj = obj.TypeOfProcedureCustomModel;
                for (var k = 0; k < procObj.length; k++) {
                    $("#date" + procObj[k].TypeOfProcedureId).val(procObj[k].Rec_Start_date);
                }
            }
            /*Bind radio button - start*/
            if (obj.MultipleProcedure) {

                $("#rbMultiple").prop("checked", true);
            } else {
                $("#rbSingle").prop("checked", true);
            }
            /*Bind radio button - end*/
            $("#hidSchedulingType").val(obj.SchedulingType);

            /*Bind physician section - start*/

            $("#txtDescription").val(obj.text);
            $("#ddFacility").val(obj.location);

            if (obj != null && obj.PhysicianReferredBy > 0) {
                if ($("#ddPhysician").length > 0)
                    $("#ddPhysician").val(obj.PhysicianReferredBy);

                if ($("#selOVPhysician").length > 0)
                    $("#selOVPhysician").val(obj.PhysicianReferredBy);
            }

            if ($("#ddlDepartment").length > 0)
                $("#ddlDepartment").val(obj.Department);

            $("#ddSpeciality").val(obj.PhysicianSpeciality);
            $("#ddAvailability").val(obj.Availability);
            $("#txtPhysicianComment").val(obj.PhysicianComments);
            /*Bind physician section - end*/

            /*Bind patient section - start*/
            $("#hfPatientId").val(obj.patientid);
            $("#patientname").val(obj.PatientName);
            $("#txtPersonDOB").val(obj.PatientDOB);
            $("#email").val(obj.PatientEmailId);
            $("#phoneno").val(obj.PatientPhoneNumber);

            //FormatMaskedPhone("#lblPatientCountryCode", "#ddlPatientCountries", "#phoneno");

            MaskedPhone("#lblPatientCountryCode", "#ddlPatientCountries", "#phoneno");
            BindPatientEncounterList(obj.patientid);
            schedularEventObj = obj.id;
            $("#hidEventParentId").val(obj.EventParentId);
            /*Bind patient section - end*/

            /*Bind appointment types - start*/
            var ctrl = $("#tbApptTypesList");
            var aptObj = obj.TypeOfProcedureCustomModel;
            var html = "";


            for (var i = 0; i < aptObj.length; i++) {
                html += "<input type='hidden' value='" + aptObj[i].PhysicianId + "' id='hfClinician" + aptObj[i].TypeOfProcedureId + "'/>";
                html += "<tr id='tr" +
                    aptObj[i].TypeOfProcedureId +
                    "'><input type='hidden' id='hfMain" +
                    aptObj[i].TypeOfProcedureId +
                    "' value='" +
                    aptObj[i].MainId +
                    "'/>";
                html += "<td>" +
                    aptObj[i].TypeOfProcedureName +
                    "</td><input type='hidden' id='hfProdTimeSlot" +
                    aptObj[i].TypeOfProcedureId +
                    "'  value='" +
                    aptObj[i].TimeSlotTimeInterval +
                    "'/>";
                html += "<td><input id='date" +
                    aptObj[i].TypeOfProcedureId +
                    "' type='text' value='" +
                    aptObj[i].DateSelected +
                    "' class='validate[required]'/>";

                html += "<input type='button' id='btnShowTimeSlots'  class='btn btn-sm btn-primary marginLR10' value='View Slots' onclick='OnChangeAppointmentDate(this," +
                    aptObj[i].TypeOfProcedureId + ")'/>";

                /* => View Nearest/Closest Time-Slots */
                html += "<input type='button' id='btnViewNearestTime'  class='btn btn-sm btn-primary marginLR10' value='First Available' onclick='BindClosestAvailableTimeSlots(this," +
                    aptObj[i].TypeOfProcedureId + ")'/></td>";
                /* => View Nearest/Closest Time-Slots */

                html += "<input type='hidden' id='hfTypeOfProcedureId" +
                    aptObj[i].TypeOfProcedureId +
                    "'  value='" +
                    aptObj[i].ProcedureAvailablityStatus +
                    "'/>";
                html += "<td><input id='timef" +
                    aptObj[i].TypeOfProcedureId +
                    "' type='text' value='" +
                    aptObj[i].TimeFrom +
                    "' class='validate[required]'/></td>";
                html += "<td><input id='timet" +
                    aptObj[i].TypeOfProcedureId +
                    "' type='text' value='" +
                    aptObj[i].TimeTo +
                    "' class='validate[required]'/></td>";
                if (aptObj[i].IsRecurrance) {
                    html += "<td><input type='hidden' id='hfDone" +
                        aptObj[i].TypeOfProcedureId +
                        "' value='1'/><input id='chkIsRec" +
                        aptObj[i].TypeOfProcedureId +
                        "' mon_rpt_date='" +
                        obj.start_date.getDate() +
                        "' freqtypeattr='' statusattr='edit' end_By='" +
                        aptObj[i].end_By +
                        "' dept_opn_days='" +
                        aptObj[i].DeptOpeningDays +
                        "' rec_pattern='" +
                        aptObj[i].Rec_Pattern +
                        "' rec_type='" +
                        aptObj[i].Rec_Type +
                        "' type='checkbox' checked='checked' onchange='OnChangeIsReccurrenceChk(this," +
                        aptObj[i].TypeOfProcedureId +
                        ");'/>" +
                        "<input type='button' mon_rpt_date='" +
                        obj.start_date.getDate() +
                        "' freqtypeattr='' statusattr='edit' end_By='" +
                        aptObj[i].end_By +
                        "' rec_pattern='" +
                        aptObj[i].Rec_Pattern +
                        "' checked='checked' dept_opn_days='" +
                        aptObj[i].DeptOpeningDays +
                        "' rec_type='" +
                        aptObj[i].Rec_Type +
                        "' id='btnEditRecurrence" +
                        aptObj[i].TypeOfProcedureId +
                        "' class='btn btn-xs btn-default' style='margin-left:5px;' value='Edit Recurrence' onclick='OnChangeIsReccurrenceChk(this," +
                        aptObj[i].TypeOfProcedureId +
                        ");'/></td>";
                } else {
                    html += "<td><input type='hidden' id='hfDone" +
                        aptObj[i].TypeOfProcedureId +
                        "' value='0'/><input id='chkIsRec" +
                        aptObj[i].TypeOfProcedureId +
                        "' type='checkbox' freqtypeattr='' value='' onchange='OnChangeIsReccurrenceChk(this," +
                        aptObj[i].TypeOfProcedureId +
                        ");'/>" +
                        "<input type='button' style='display:none;'  id='btnEditRecurrence" +
                        aptObj[i].TypeOfProcedureId +
                        "' class='btn btn-xs btn-default' style='margin-left:5px;' freqtypeattr='' value='Edit Recurrence' statusattr='new' onclick='OnChangeIsReccurrenceChk(this," +
                        aptObj[i].TypeOfProcedureId +
                        ");'/></td>";
                }
                //html += "<td><select id='aptAvail" + aptObj[i].TypeOfProcedureId + "'></select></td>";
                html += "<td><img src='/images/delete.png' width='15px' onclick='RemoveAppointmentProcedures(" +
                    aptObj[i].TypeOfProcedureId +
                    ")'/></td>";
                html += "</tr>";
                ctrl.html(html);

                var hfApptTypes = $("#hfAppointmentTypes").val();
                var concatApptTypes = hfApptTypes + "," + aptObj[i].TypeOfProcedureId;
                $("#hfAppointmentTypes").val(concatApptTypes.replace(/^,|,$/g, ""));
                /*Create object for reccurrence at the time of edit- start*/
                var recObject = new Object();
                recObject.appoint_Type_Id = aptObj[i].TypeOfProcedureId;
                recObject.Rec_Pattern = aptObj[i].Rec_Pattern;
                recObject.Rec_Type = aptObj[i].Rec_Type;
                recObject.end_By = aptObj[i].end_By;
                recObject.Rec_Start_Date = new Date(aptObj[i].DateSelected);
                if (aptObj[i].Rec_Pattern != null) {
                    if (aptObj[i].Rec_Pattern.indexOf("day") > -1) {
                        recObject.Frequency_Type = 1;
                    } else if (aptObj[i].Rec_Pattern.indexOf("week") > -1) {
                        recObject.Frequency_Type = 2;
                    } else {
                        var monthPatternObj = aptObj[i].Rec_Pattern.split("_");
                        if (monthPatternObj[3] == "") {
                            recObject.Frequency_Type = 3;
                        } else {
                            recObject.Frequency_Type = 4;
                        }
                    }
                }
                apptRecArray.push(recObject);
                /*Create object for reccurrence at the time of edit- end*/
            }
            var loop = $("#hfAppointmentTypes").val().split(",");
            //test
            for (var u = 0; u < loop.length; u++) {
                $("#date" + loop[u])
                    .datetimepicker({
                        format: "m/d/Y",
                        minDate: 0,
                        timepicker: false,
                        closeOnDateSelect: true,
                        onChangeDateTime: function (e) {
                            $("#timef" + $("#hfAppointmentTypes").val()).val("");
                            $("#timet" + $("#hfAppointmentTypes").val()).val("");
                        }
                    });
                $("#timef" + loop[u])
                    .kendoTimePicker({
                        interval: parseInt($("#hfProdTimeSlot" + loop[u]).val()),
                        format: "HH:mm",
                        min: new Date(2017, 10, 01, firstHour, 00, 00),
                        max: new Date(2017, 10, 01, lastHour, 00, 00),
                        change: function (e) {

                            startChange("#" + e.sender.element[0].id,
                                "#" + e.sender.element[0].id.replace("f", "t"),
                                parseInt(e.sender.options.interval)); //fromtimeId, totimeId, timeinterval
                        }
                    })
                    .data("kendoTimePicker");
                //.readonly();

                $("#timet" + loop[u])
                    .kendoTimePicker({
                        min: new Date(2017, 10, 01, firstHour, 00, 00),
                        max: new Date(2017, 10, 01, lastHour, 00, 00),
                        interval: parseInt($("#hfProdTimeSlot" + loop[u]).val()),
                        format: "HH:mm"
                    })
                    .data("kendoTimePicker");
                //.readonly();
            }
            /*Bind appointment types - end*/
            $("#txtEmiratesNationalId").val(obj.EmirateIdnumber);
            break;
        case "2":
            $("#divSchedularPopUpContent .modal-title").html("Holiday/Vacations Scheduler");
            /*Bind radio button - start*/
            if (obj.MultipleProcedure) {
                $("#rbHolidayMultiple").prop("checked", true);
                $("#eventToDate").addClass("validate[required]");
                $("#divHolidayToDateTime").show();
                /*for multiple, first we need to show edit multiple series popup and then on its input show real popup*/
                $("#divMultipleEventPopup").show();
                $("#btnEditSeries")
                    .click(function (e) {

                        $("#divSchedularPopUp").show();
                        $("#hidEventParentId").val(obj.EventParentId);
                        $("#hfSchedulingId").val(obj.TimeSlotId);
                        $("#divMultipleEventPopup").hide();
                        $("#rbHolidaySingle").prop("disabled", true);
                        $("#rbHolidayMultiple").prop("disabled", true);
                        $("#eventFromDate").val(obj.Rec_Start_date);
                        $("#eventToDate").val(obj.Rec_end_date);
                        $("#hidEventFromDate").val(obj.Rec_Start_date);
                        $("#hidEventToDate").val(obj.Rec_end_date);
                    });
                $("#btnEditOccurrence")
                    .click(function (e) {

                        $("#divSchedularPopUp").show();
                        $("#hidEventParentId").val(obj.EventParentId);
                        $("#hfSchedulingId").val(obj.TimeSlotId);
                        $("#divMultipleEventPopup").hide();
                        $("#rbHolidaySingle").prop("disabled", true);
                        $("#rbHolidayMultiple").prop("disabled", true);
                        $("#rbHolidaySingle").prop("checked", true);
                        $("#rbHolidayMultiple").prop("checked", false);
                        $("#divHolidayToDateTime").hide();
                        $("#hfSelPhysicianId").val(obj.section_id);
                        //$("#eventFromDate").val(obj.Rec_Start_date);
                        //$("#eventToDate").val(obj.Rec_Start_date);
                        //$('#hidEventFromDate').val(obj.Rec_Start_date);
                        //$('#hidEventToDate').val(obj.Rec_Start_date);
                    });
                $("#hfExternalValue3").val(obj.MultipleProcedure == true ? "True" : "False");
            } else {
                $("#rbHolidaySingle").prop("checked", true);
                $("#eventToDate").removeClass("validate[required]");
                $("#hidEventParentId").val(obj.EventParentId);
                $("#hfSchedulingId").val(obj.TimeSlotId);
                $("#rbHolidaySingle").prop("disabled", true);
                $("#rbHolidayMultiple").prop("disabled", true);
                $("#rbHolidaySingle").prop("checked", true);
                $("#rbHolidayMultiple").prop("checked", false);
                $("#divHolidayToDateTime").hide();
                $("#divSchedularPopUp").show();
                $("#hfSelPhysicianId").val(obj.section_id);
                $("#hfExternalValue3").val(obj.MultipleProcedure == false ? "False" : "True");
            }
            var objStartDate = obj.start_date;
            var newlyCreatedStDate = (objStartDate.getMonth() + 1) +
                "/" +
                objStartDate.getDate() +
                "/" +
                objStartDate.getFullYear();

            var objEndDate = obj.end_date;
            var newlyCreatedEndDate = (objEndDate.getMonth() + 1) +
                "/" +
                objEndDate.getDate() +
                "/" +
                objEndDate.getFullYear();
            /*Bind radio button - end*/
            $("#eventFromDate")
                .datetimepicker({
                    format: "m/d/Y",
                    minDate: 0, //yesterday is minimum date(for today use 0 or -1970/01/01)
                    maxDate: "2025/12/12",
                    timepicker: false,
                    value: newlyCreatedStDate,
                    closeOnDateSelect: true
                });
            $("#eventToDate")
                .datetimepicker({
                    format: "m/d/Y",
                    minDate: 0, //yesterday is minimum date(for today use 0 or -1970/01/01)
                    maxDate: "2025/12/12",
                    timepicker: false,
                    value: newlyCreatedEndDate,
                    closeOnDateSelect: true
                });
            /*Bind main section - start*/
            $("#hidEventFromDate").val(obj.start_date);
            $("#hidEventToDate").val(obj.end_date);
            $("#txtHolidayDescription").val(obj.text);
            $("#ddHolidayType").val(obj.VacationType);
            $("#ddHolidayStatus").val(obj.Availability);
            ShowPhysicianBasedOnHolidayType(obj.VacationType);
            //$("#eventFromDate").val(obj.start_date.toLocaleDateString());
            //$("#eventToDate").val(obj.end_date.toLocaleDateString());
            $("#ddHolidayPhysician").val(obj.section_id);
            $("#ddHolidayPhysician").prop("disabled", true);
            /*Bind main section - end*/
            break;
        default:
            break;
    }
    //if (obj.start_date < new Date() && scheduleType == "1") {
    //    $("#btnSaveSchedulingData").hide();
    //    $("#btnDeleteSchedulingData").hide();
    //}
};

function BindPatientEncounterList(patientId) {
    $.ajax({
        url: "/Physician/GetPhysicianListByPatientId",
        type: "POST",
        dataType: "json",
        async: false,
        data: {
            patientId: patientId
        },
        success: function (data) {
            var html = "";
            for (var k = 0; k < data.length; k++) {
                html += "<tr>";
                html += "<td>" + data[k].EncounterNumber + "</td>";
                html += "<td>" + data[k].Physician.PhysicianName + "</td>";
                html += "<td><input type='button' value='select' onclick='OnPhysicianSelectionInPreviousEncounters(" +
                    data[k].Physician.FacilityId +
                    "," +
                    data[k].Physician.Id +
                    "," +
                    data[k].Physician.FacultyDepartment +
                    "," +
                    data[k].Physician.FacultySpeciality +
                    ")'/></td>";
                html += "</tr>";
            }
            $("#tbList").html(html);
        }
    });
}

function RemoveAppointmentProcedures(apptId) {
    var selectedTypes = $("#hfAppointmentTypes").val();
    var selectedTypesArray = selectedTypes.split(",");
    //var tt = apptRecArray;
    //$.each(apptRecArray,
    //    function (i, val) {
    //        $.each(val,
    //            function (key, name) {
    //                
    //                if (name == apptId) {
    //                    apptRecArray.splice(i, 1);
    //                }
    //            });
    //    });



    $.each(apptRecArray, function (key, name) {
        if (name == apptId) {
            apptRecArray.splice(i, 1);
        }
    });

    selectedTypesArray.remove(apptId.toString());
    var removeAppTypes = $("#hfRemovedAppTypes").val();
    removeAppTypes += "," + $("#hfMain" + apptId).val();
    $("#hfRemovedAppTypes").val(removeAppTypes.replace(/^,/, ""));
    $("#hfAppointmentTypes").val(selectedTypesArray.join(","));
    $("#tr" + apptId).remove();
}

Array.prototype.remove = function (val) {
    var i = this.indexOf(val);
    return i > -1 ? this.splice(i, 1) : [];
};
var ShowLightBoxStyle = function (schedulingType) {
    switch (schedulingType) {
        case "1":
            $("#divMultipleSingle").show();
            $("#divPhysicianPatient").show();
            $("#btnOverView").show();
            $("#divTypeofProc").show();
            $("#divHoliday").hide();
            $("#divDateTime").hide();
            $("#divSchedularPopUp .main_content").css("height", "496px");
            break;
        case "2":
            $("#divMultipleSingle").hide();
            $("#divPhysicianPatient").hide();
            $("#btnOverView").hide();
            $("#divTypeofProc").hide();
            $("#divHoliday").show();
            $("#divDateTime").hide();
            $("#divSchedularPopUp .main_content").css({ "height": "180px", "margin-top": "0" });

            break;
        default:
            break;
    }
    $("#btnDeleteSchedulingData").hide();
};
var OnChangeType = function (e) {
    var obj = $(e);
    var value = obj.val();
    $("#divShowPhysicianVacationList").hide();
    ShowPhysicianBasedOnHolidayType(value);
};
var ShowPhysicianBasedOnHolidayType = function (val) {

    switch (val) {
        case "11":
            $("#divHolidayPhysician").show();
            $("#ddHolidayPhysician").addClass("validate[required]");
            break;
        case "12":
            $("#divHolidayPhysician").hide();
            $("#ddHolidayPhysician").removeClass("validate[required]");
            BindHolidayView();
            break;
        default:
            break;
    }
};

function myFunction(stringVal) {
    // var dateStartObj = stringVal.split('/');
    var dateStartObj = $("#eventFromDate").val().split("/");
    var ddV = dateStartObj[1];
    var dd = String(String(ddV));
    var mmV = dateStartObj[0];
    var mm = parseInt(mmV) - 1;
    var yyV = dateStartObj[2];
    var yyvX = Math.floor(yyV);
    var yy = parseInt(yyV);
    var testdate = new Date(ddV, mmV, yyV);
    var datecustomObj = new Date();
    datecustomObj.setDate(parseInt(dateStartObj[1]));
    datecustomObj.setMonth(parseInt(dateStartObj[0]) - 1);
    datecustomObj.setYear(parseInt(dateStartObj[2]));
    return datecustomObj;
}

var SaveHolidaySchedular = function () {
    schSessionInterval();
    var btnSelection = $("input[name=rdbtnHolidaySelection]:checked").val();
    var jsonData = [];
    if (btnSelection == "2") {
        // single Day Event/ Leave
        var dateStartObj = $("#eventFromDate").val().split("/");
        //var testdate = myFunction($('#eventFromDate').val());
        var datecustomObj;
        var startFromDate;
        if (!isNaN(parseInt(dateStartObj[1]))) {
            datecustomObj = new Date($("#eventFromDate").val());
            startFromDate = new Date($("#eventFromDate").val());
        } else {
            datecustomObj = new Date($("#hidEventFromDate").val());
            startFromDate = new Date($("#hidEventFromDate").val());
        }

        datecustomObj.setDate(datecustomObj.getDate() + 1);
        datecustomObj.setMinutes(datecustomObj.getMinutes() - 1);
        jsonData[0] = ({
            SchedulingId: $("#hfSchedulingId").val() == "" ? 0 : $("#hfSchedulingId").val(),
            AssociatedId: $("#ddHolidayType").val() == "11"
                ? $("#ddHolidayPhysician").val()
                : $("#hidFacilitySelected").val(),
            AssociatedType: $("#hidSchedulingType").val(),
            SchedulingType: $("#hidSchedulingType").val(),
            Status: ($("#ddHolidayStatus").val()), //$('#ddAvailability').val(),
            StatusType: "", //$('#ddAvailability').val(),
            ScheduleFrom: startFromDate,
            ScheduleTo: datecustomObj,
            PhysicianId: "",
            TypeOfProcedure: $("#ddHolidayType").val(),
            FacilityId: $("#hidFacilitySelected").val(),
            Comments: $("#txtHolidayDescription").val(),
            Location: $("#hidFacilityName").val(),
            ConfirmedByPatient: "", //set it by default
            ExtValue2: $("#ddHolidayType").val() == "12" ? $("#hidFacilitySelected").val() : "",
            //IsRecurring: ev.rec_type == null && ev.rec_type !== '' ? false : ev._timed,
            RecurringDateFrom: $("#hfIsReccurrence").val() == "True" ? $("#hfRecStartDate").val() : "",
            RecurringDateTill: $("#hfIsReccurrence").val() == "True" ? $("#hfRecEndDate").val() : "",
            //hfRecStartDate
            //hfRecEndDate
            //hfIsReccurrence
            //EventId: id,
            //RecType: ev.rec_type,
            //RecPattern: ev.rec_pattern,
            //RecEventlength: ev.event_length,
            //RecEventPId: ev.event_pid,
            WeekDay: "",
            EventParentId: $("#hidEventParentId").val(),
            MultipleProcedures: false,
            EventId: schedularEventObj,
            SelectedPhysicianId: $("#hfSelPhysicianId").val(),
            ExternalValue3: $("#hfExternalValue3").val()
        });
    } else {
        // Multiple Day Event / Leave
        //var dateMStartObj = $('#eventFromDate').val().split('/');
        //var dateMToObj = $('#eventToDate').val().split('/');
        //var datecustomFromObj = new Date();
        var dateMStartObj = $("#eventFromDate").val().split("/");
        var datecustomFromObj;

        var dateMToObj = $("#eventToDate").val().split("/");
        var datecustomToObj;

        if (!isNaN(parseInt(dateMStartObj[1]))) {
            datecustomFromObj = new Date($("#eventFromDate").val());
        } else {
            datecustomFromObj = new Date($("#hidEventFromDate").val());
        }

        if (!isNaN(parseInt(dateMToObj[1]))) {
            datecustomToObj = new Date($("#eventToDate").val());
        } else {
            datecustomToObj = new Date($("#hidEventToDate").val());
        }
        //datecustomToObj.setHours(00);
        //datecustomToObj.setMinutes(00);
        datecustomToObj.setDate(datecustomToObj.getDate() + 1);
        datecustomToObj.setMinutes(datecustomToObj.getMinutes() - 1);
        var daterangediffernece = CalculateDaysBetweenDates(datecustomFromObj, datecustomToObj);

        for (var i = 0; i < daterangediffernece; i++) {
            var dateStartfrom = new Date(datecustomFromObj);
            dateStartfrom.setDate(dateStartfrom.getDate() + i);
            //dateStartfrom.setHours(00);
            //dateStartfrom.setMinutes(00);

            var dateEndTo = new Date(datecustomFromObj);
            dateEndTo.setDate(dateEndTo.getDate() + (i + 1));
            dateEndTo.setMinutes(dateEndTo.getMinutes() - 1);

            jsonData[i] = ({
                SchedulingId: $("#hfSchedulingId").val() == "" ? 0 : $("#hfSchedulingId").val(),
                //$('#hfMain' + selectedAppointmenttype).val(),
                AssociatedId: $("#ddHolidayType").val() == "11"
                    ? $("#ddHolidayPhysician").val()
                    : $("#hidFacilitySelected").val(),
                AssociatedType: $("#hidSchedulingType").val(),
                SchedulingType: $("#hidSchedulingType").val(),
                Status: ($("#ddHolidayStatus").val()), //$('#ddAvailability').val(),
                StatusType: "", //$('#ddAvailability').val(),
                ScheduleFrom: dateStartfrom,
                ScheduleTo: dateEndTo,
                PhysicianId: "",
                TypeOfProcedure: $("#ddHolidayType").val(),
                FacilityId: $("#hidFacilitySelected").val(),
                Comments: $("#txtHolidayDescription").val(),
                Location: $("#hidFacilityName").val(),
                ConfirmedByPatient: "", //set it by default
                ExtValue2: $("#ddHolidayType").val() == "12" ? $("#hidFacilitySelected").val() : "",
                RecurringDateFrom: $("#eventFromDate").val(),
                RecurringDateTill: $("#eventToDate").val(),
                //IsRecurring: ev.rec_type == null && ev.rec_type !== '' ? false : ev._timed,
                //RecurringDateFrom: ev.start_date,
                //RecurringDateTill: ev.end_date,
                //EventId: id,
                //RecType: ev.rec_type,
                //RecPattern: ev.rec_pattern,
                //RecEventlength: ev.event_length,
                //RecEventPId: ev.event_pid,
                WeekDay: "",
                EventParentId: $("#hidEventParentId").val(),
                MultipleProcedures: true,
                EventId: schedularEventObj,
                SelectedPhysicianId: $("#hfSelPhysicianId").val(),
                ExternalValue3: $("#hfExternalValue3").val()
            });
        }
    }
    var jsonD = JSON.stringify(jsonData);
    $.ajax({
        type: "POST",
        url: schedulerUrl + "SaveHolidayPlannerScheduling",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonD,
        success: function (data) {
            if (data.IsError) {
                for (var j = 0; j < data.notAvialableTimeslotslist.length; j++) {
                    ShowMessage(data.notAvialableTimeslotslist[j].Reason, "Warning", "warning", true);
                    $("body")
                        .css({
                            'overflow-y': "auto",
                            'padding-left': "0px"
                        });
                    unBlockRecurrenceDiv();
                }
            } else {
                var msg = "";
                if (data.SkippedHolidaysData1.length > 0) {
                    msg =
                        "Records Saved Succesfully!, But some of the Physicians have Vacation or Appointments booked during selected date period.";
                } else {
                    msg = "Records Saved Succesfully!";
                }
                ShowMessage(msg, "Success", "success", true);
                onCheckFilters();
                //$('.hidePopUp').hide();
                $.validationEngine.closePrompt(".formError", true);
                ClearSchedulingPopup();
            }
        },
        error: function (msg) {
        }
    });
};

var OnChangeHolidayRB = function (e) {
    var obj = $(e);
    var value = obj.val();
    switch (value) {
        case "2":
            $("#divHolidayToDateTime").hide();
            $("#eventToDate").removeClass("validate[required]");
            break;
        case "1":
            $("#divHolidayToDateTime").show();
            $("#eventToDate").addClass("validate[required]");
            break;
        default:
            break;
    }
};
var CalculateDaysBetweenDates = function (startdate, enddate) {
    var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
    var firstDate = new Date(Date.parse(startdate));
    var secondDate = new Date(Date.parse(enddate));
    var diffDays = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));
    return diffDays;
};
var DeleteSchduling = function (epid, schid, schtype, extVal3) {
    var jsonData = "";
    switch (schtype) {
        case "1":
            jsonData = {
                eventParentId: schid == "" ? epid : "",
                schedulingId: schid,
                schedulingType: schtype,
                externalValue3: extVal3
            };
            break;
        case "2":
            if ($("#rbHolidaySingle").prop("checked") == true) {
                jsonData = { eventParentId: "", schedulingId: schid, schedulingType: schtype, externalValue3: extVal3 };
            } else {
                jsonData = { eventParentId: epid, schedulingId: schid, schedulingType: schtype, externalValue3: extVal3 };
            }
            break;
    }
    // return false;
    $.ajax({
        type: "POST",
        url: schedulerUrl + "DeleteSchduling",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            if (data == true)
                ShowMessage("Records Deleted Succesfully!", "Success", "success", true);
            else
                ShowMessage("Unable to delete Old records!", "Warning", "warning", true);
            onCheckFilters();
            //$('.hidePopUp').hide();
            $.validationEngine.closePrompt(".formError", true);
            ClearSchedulingPopup();
        },
        error: function (msg) {
        }
    });
};
var SetSelectedViewAsWeek = function () {
    //var treeView = $("#treeviewCalenderType").data("kendoTreeView");
    //treeView.select($());
    //$('#treeviewCalenderType li[data-id="2"]').find('span').addClass('k-state-selected');
    $("#treeviewCalenderType li").removeClass("active");
    $("#liviewCalenderType_2").addClass("active");
    $("#hidSelectedView").val("2");
    $("#hidSelectedViewName").val("Week");
};
var OnChangeDate = function (obj) {

};

var OnChangeAppointmentDate = function (obj, apptTypeId) {
    $("#lblTimeSlotsDate").text('');
    var selectedDate = $("#date" + apptTypeId).val(); //obj;
    var facilityId = $("#ddFacility").val();
    var physicianId = $("#ddPhysician").val();
    var typeOfProcedure = apptTypeId;
    var jsonData = {
        facilityId: facilityId,
        physicianId: physicianId,
        dateselected: selectedDate,
        typeofproc: typeOfProcedure
    };
    $.ajax({
        type: "POST",
        url: schedulerUrl + "GetAvailableTimeSlots",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            $("#lblViewTimeSlotsTitle").text('Available Time Slots');
            $("#ulTimeSlots").empty();
            var _ctrl;
            if (data.length == 0 || data[0].TimeSlot == 0) {
                _ctrl = $("#ulTimeSlots");
                _ctrl.html("");
                _ctrl.html("<li class='alert alert-danger'>Time slots are not available for the selected date</li>");
            } else if (data[0].TimeSlot == 1) {
                _ctrl = $("#ulTimeSlots");
                _ctrl.html("");
                _ctrl.html("<li class='alert alert-danger'>Rooms Not available for selected appointment type.</li>");
            } else if (data[0].TimeSlot == 2) {
                _ctrl = $("#ulTimeSlots");
                _ctrl.html("");
                _ctrl.html("<li class='alert alert-danger'>Faculty is not available due to holiday</li>");
            }
            else {
                _ctrl = $("#ulTimeSlots");
                var html = "";//"<input id='hfClinician" + apptTypeId + "' type='hidden' />";
                var clinicianId = '';

                html += "<input type='hidden' id='schUpdateFlag'" + apptTypeId + " value='0' />";

                for (var i = 0; i < data.length; i++) {
                    if (clinicianId == '' || data[i].PhysicianId != clinicianId) {
                        clinicianId = data[i].PhysicianId;
                        html += "<li class='liPhyicianHeading'><h3>" + data[i].Clinician + "</h3></li>";
                    }

                    html += '<li id="' +
                        data[i].TimeSlot +
                        '" dopd="' +
                        data[i].DepOpeningDays +
                        '" onclick="SelectTimeSlot(this,' +
                        apptTypeId +
                        ',1,' + clinicianId + ')">' +
                        data[i].TimeSlot +
                        "</li>";
                }
                _ctrl.html("");
                _ctrl.html(html);
                //$('#divAvailableTimeSlots').addClass('moveLeft2');
            }
        },
        error: function (msg) {
            console.log(msg);
        }
    });
};

function BindClosestAvailableTimeSlots(obj, apptTypeId) {
    var selectedDate = $("#date" + apptTypeId).val(); //obj;
    var facilityId = $("#ddFacility").val();
    var physicianId = $("#ddPhysician").val();
    var typeOfProcedure = apptTypeId;
    var jsonData = {
        facilityId: facilityId,
        physicianId: physicianId,
        dateselected: selectedDate,
        typeofproc: typeOfProcedure
    };
    $.ajax({
        type: "POST",
        url: schedulerUrl + "GetClosestAvailableTimeSlots",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            $("#lblViewTimeSlotsTitle").text('First Available Appointments');
            $("#lblTimeSlotsDate").text('(' + data.dt + ')');
            $("#lblTimeSlotsDate").show();
            $("#ulTimeSlots").empty();
            var list = data != null && data.list != null ? data.list : '0';
            var _ctrl;
            if (list == null || list.length == 0 || list[0].TimeSlot == 0) {
                _ctrl = $("#ulTimeSlots");
                _ctrl.html("");
                _ctrl.html("<li class='alert alert-danger'>Time slots are not available for the selected date</li>");
            } else if (list[0].TimeSlot == 1) {
                _ctrl = $("#ulTimeSlots");
                _ctrl.html("");
                _ctrl.html("<li class='alert alert-danger'>Rooms Not available for selected appointment type.</li>");
            } else if (list[0].TimeSlot == 2) {
                _ctrl = $("#ulTimeSlots");
                _ctrl.html("");
                _ctrl.html("<li class='alert alert-danger'>Faculty is not available due to holiday</li>");
            }
            else {
                _ctrl = $("#ulTimeSlots");
                var html = "";
                var clinicianId = '';

                html += "<input type='hidden' id='schUpdateFlag'" + apptTypeId + " value='0' />";

                for (var i = 0; i < list.length; i++) {
                    if (clinicianId == '' || list[i].PhysicianId != clinicianId) {
                        clinicianId = list[i].PhysicianId;
                        html += "<li class='liPhyicianHeading'><h3>" + list[i].Clinician + "</h3></li>";
                    }

                    html += '<li id="' +
                        list[i].TimeSlot +
                        '" dopd="' +
                        list[i].DepOpeningDays +
                        '" onclick="SelectTimeSlot(this,' +
                        apptTypeId +
                        ',1,' + clinicianId + ')">' +
                        list[i].TimeSlot +
                        "</li>";
                }
                _ctrl.html("");
                _ctrl.html(html);
            }
        },
        error: function (msg) {
            console.log(msg);
        }
    });
};

var SelectTimeSlot = function (e, aTypeId, pName, clinicianId) {
    if (pName == 2) {
        $(".change0").find("select").val("0");
        var facilityId = $(e).attr("attr-fid");
        var physicianId = $(e).attr("attr-pid");
        var sTime = $(e).attr("attr-stime");
        var eTime = $(e).attr("attr-etime");
        var fdate = $(e).attr("attr-selDate");
        $("#ddFacility").val(facilityId);
        OnTimeSlotsSelectionInAppOverview("#ddFacility", physicianId);
        $("#ddPhysician").val(physicianId);
        SetDepartmentAndSpeciality($("#selOVPhysician"));
        $("#appointmentTypes").val(aTypeId);
        OnChangeApptTypes($("#appointmentTypes"));
        AddAppointmentTypesTimeSlot();
        $("#timef" + aTypeId).val(sTime);
        $("#timet" + aTypeId).val(eTime);
        $("#date" + aTypeId).val(fdate);
        $("#divOverViewPopup").hide();
        $("#divOVAvailableTimeSlots").removeClass("moveLeft2");
        $("#divSchedularPopUp").show();
        $(".newrowadded").remove();
        $(".selected_column").hide();
    } else {
        var timeSlot = $(e)[0].id.split("-");
        $("#timef" + aTypeId).val(timeSlot[0]);
        $("#timet" + aTypeId).val(timeSlot[1]);
        $("#divAvailableTimeSlots").removeClass("moveLeft2");
        $("#chkIsRec" + aTypeId).prop("checked", false);
        $("#btnEditRecurrence" + aTypeId).hide();
        $("#hfClinician" + aTypeId).val(clinicianId);

        /* Setting Selected Physicians and the associated speciality */
        if ($("#ddPhysician").val() <= 0) {
            $("#ddPhysician").val(clinicianId);
            SetDepartmentAndSpeciality($("#ddPhysician"));
        }

        var deptOpeningDays = $(e).attr("dopd").split(",");
        for (var i = 0; i < deptOpeningDays.length; i++) {
            $("input[name='week_day'][value=" + deptOpeningDays[i] + "]").prop("disabled", false);
            $("input[name='week_day'][value=" + deptOpeningDays[i] + "]").prop("title", "");
            $("#ddMonthWeekDays option[value=" + deptOpeningDays[i] + "]").prop("disabled", false);
            $("#ddMonthWeekDays option[value=" + deptOpeningDays[i] + "]").prop("title", "");
        }

        /*
        Just to let the system know that current Appointment Type has been modified so that I could call the Procedure for update
        */
        if ($("#schUpdateFlag" + aTypeId).length > 0)
            $("#schUpdateFlag" + aTypeId).val(1);
    }

    $.unblockUI();
    //startChange("#timef" + aTypeId, "#timet" + aTypeId, parseInt($("#hfTimeSlot").val()));
};

/*Method is used to call on IsReccurrence checkbox change under Appointment types list*/
var OnChangeIsReccurrenceChk = function (e, appTypeId) {
    var ctrl = $(e);
    if (!ctrl[0].checked && ctrl.attr("type") == "checkbox") {
        $("#btnEditRecurrence" + appTypeId).hide();
        return false;
    } else {
        $("#btnEditRecurrence" + appTypeId).show();
    }
    if (ctrl.attr("statusattr") == "new") {
        $(".reccurrence_field").hide();
        $("#divReccurrencePopup .popup_frame").addClass("moveLeft");
        $("#hfRecAppTypeId").val(appTypeId);
        if (ctrl.attr("freqtypeattr") == "daily") {
            $("#rbtnDailyRecType").prop("checked", true);
            $("#daily").show();
        } else if (ctrl.attr("freqtypeattr") == "weekly") {
            $("#rbtnWeeklyRecType").prop("checked", true);
            $("#weekly").show();
        } else if (ctrl.attr("freqtypeattr") == "monthly") {
            $("#rbtnMonthlyRecType").prop("checked", true);
            $("#monthly").show();
        }
    } else if (ctrl.attr("statusattr") == "edit") {
        if ($("#hfDone" + $("#hfRecAppTypeId").val()).val() == "1") {
            $("#divReccurrencePopup .popup_frame").addClass("moveLeft");
            $("#hfRecAppTypeId").val(appTypeId);
            $(".reccurrence_field").hide();
            if (ctrl.attr("freqtypeattr") == "daily") {
                $("#rbtnDailyRecType").prop("checked", true);
                $("#daily").show();
            } else if (ctrl.attr("freqtypeattr") == "weekly") {
                $("#rbtnWeeklyRecType").prop("checked", true);
                $("#weekly").show();
            } else if (ctrl.attr("freqtypeattr") == "monthly") {
                $("#rbtnMonthlyRecType").prop("checked", true);
                $("#monthly").show();
            }
        } else {
            var rpattern = ctrl.attr("rec_pattern");
            var rtype = ctrl.attr("rec_type");
            var endBy = ctrl.attr("end_By");
            //if (ctrl[0].checked) {
            $("#divReccurrencePopup .popup_frame").addClass("moveLeft");
            $("#hfRecAppTypeId").val(appTypeId);
            if (rpattern.indexOf("day") > -1) {
                var dayValue = rpattern.split("_")[1];
                $("#rbtnDailyRecType").prop("checked", true);
                $("#txtRecEveryDay").val(dayValue);
                $("#txtRecEndByDate").val(endBy);
                $(".reccurrence_field").hide();
                $("#rbtnEveryDay").prop("checked", true);
                $("#daily").show();
            } else if (rpattern.indexOf("week") > -1) {
                var weekPatternObj = rpattern.split("_");
                var weekPatternDaysObj = weekPatternObj[4].split(",");
                if (weekPatternDaysObj.length > 0) {
                    for (var u = 0; u < weekPatternDaysObj.length; u++) {
                        $("tr input[type=checkbox][value=" + weekPatternDaysObj[u] + "]").prop("checked", true);
                    }
                }
                $("#rbtnWeeklyRecType").prop("checked", true);
                $("#txtRecEveryWeekDays").val(weekPatternObj[1]);
                $("#txtRecEndByDate").val(endBy);
                $(".reccurrence_field").hide();
                $("#weekly").show();
            } else {
                var monthPatternObj = rpattern.split("_");
                $("#rbtnMonthlyRecType").prop("checked", true);
                if (monthPatternObj[3] == "") {
                    $("#rbtnRepeatMonthly").prop("checked", true);
                    $("#txtRepeatMonthDay").val(ctrl.attr("mon_rpt_date"));
                    $("#ddEveryMonthDay").val(monthPatternObj[1]);
                } else {
                    $("#rbtnRepeatOnMonthly").prop("checked", true);
                    $("#ddOnRepeatMonth").val(monthPatternObj[3]);
                    $("#ddMonthWeekDays").val(monthPatternObj[2]);
                    $("#ddOnEveryMonth").val(monthPatternObj[1]);
                }
                $("#txtRecEndByDate").val(endBy);

                $(".reccurrence_field").hide();
                $("#monthly").show();
            }
            //}
        }
    } else {
        var isChecked = ctrl[0].checked;
        if (isChecked) {
            if ($("#date" + appTypeId).val() != "") {
                $("#divReccurrencePopup .popup_frame").addClass("moveLeft");
            }
            $("#hfRecAppTypeId").val(appTypeId);
            $("#txtRecEndByDate").val($("#date" + appTypeId).val());
        }
        $("#rbtnDailyRecType").prop("checked", true);
        $("#rbtnEveryDay").prop("checked", true);
    }
    //dept_opn_days
    if ($(e).attr("dept_opn_days") != undefined) {
        var deptOpeningDays = $(e).attr("dept_opn_days").split(",");
        for (var i = 0; i < deptOpeningDays.length; i++) {
            $("input[name='week_day'][value=" + deptOpeningDays[i] + "]").prop("disabled", false);
            $("input[name='week_day'][value=" + deptOpeningDays[i] + "]").prop("title", "");
            $("#ddMonthWeekDays option[value=" + deptOpeningDays[i] + "]").prop("disabled", false);
            $("#ddMonthWeekDays option[value=" + deptOpeningDays[i] + "]").prop("title", "");
        }
    }
    $("#rbtnAllEndBy").prop("checked", true);
};

var DoneApppointTypeReccurrence = function () {
    var recTypeValue = $("input[name='rbtnRecType']:checked").val();
    $("#btnEditRecurrence" + $("#hfRecAppTypeId").val()).attr("freqtypeattr", recTypeValue);
    if (recTypeValue == "daily") {
        if ($("#txtRecEveryDay").val() == "") {
            ShowMessage("Please fill Every day field!", "Warning", "warning", true);
            return false;
        }
    } else if (recTypeValue == "weekly") {
        var chkLength = $("#weekly").find("input:checked").length;
        if ($("#txtRecEveryWeekDays").val() == "") {
            ShowMessage("Please fill Every week(s) field!", "Warning", "warning", true);
            return false;
        }
        if (chkLength == 0) {
            ShowMessage("Please check atleast one day from the list!", "Warning", "warning", true);
            return false;
        }
    } else if (recTypeValue == "monthly") {
        var recRepeatMonthValue1 = $("input[name='rbtnRepeatMonth']:checked").val();
        if (recRepeatMonthValue1 == "1" && $("#txtRepeatMonthDay").val() == "") {
            ShowMessage("Please fill Repeat day field!", "Warning", "warning", true);
            return false;
        }
    }

    $.each(apptRecArray,
        function (i, val) {

            $.each(val,
                function (key, name) {

                    if (name == $("#hfRecAppTypeId").val()) {
                        apptRecArray.splice(i, 1);
                    }
                });
            return false; //return false is used to break the loop
        });
    var recObject = new Object();
    recObject.appoint_Type_Id = $("#hfRecAppTypeId").val();
    var apptStartDate = $("#date" + $("#hfRecAppTypeId").val()).val();

    switch (recTypeValue) {
        case "daily":
            var recEndDate = new Date($("#txtRecEndByDate").val());
            //recEndDate.setDate(recEndDate.getDate() + 1);
            recEndDate.setDate(recEndDate.getDate());
            var recStartDateDaily = new Date(apptStartDate);
            recStartDateDaily.setDate(recStartDateDaily.getDate());
            recObject.Rec_Pattern = "day_" + $("#txtRecEveryDay").val() + "__";
            recObject.Rec_Type = "day_" + $("#txtRecEveryDay").val() + "__#";
            recObject.end_By = recEndDate; //$("#txtRecEndByDate").val();
            recObject.Rec_Start_Date = recStartDateDaily;
            recObject.Frequency_Type = 1; //1 is for daily selected radio button
            break;
        case "weekly":
            var d = new Date($("#date" + $("#hfRecAppTypeId").val()).val());
            var n = d.getDay();
            var recStartDateWeekly = new Date(apptStartDate);
            recStartDateWeekly.setDate(recStartDateWeekly.getDate());
            //var weeklyDaysChk = $(".dhx_repeat_checkbox");
            var weeklyDayChk = $("input[name='week_day']");
            var selectedWeekDays = "";
            if (weeklyDayChk.length > 0) {
                for (var i = 0; i < weeklyDayChk.length; i++) {
                    if (weeklyDayChk[i].checked) {
                        selectedWeekDays += weeklyDayChk[i].value + ",";
                    }
                }
                //selectedWeekDays = selectedWeekDays.replace(/,\s*$/, "");//To remove last comma from the string
                selectedWeekDays = selectedWeekDays.replace(/,\s*$/, "").split(",").sort().join(",");
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
            recObject.Frequency_Type = 2; //2 is for weekly selected radio button
            break;
        case "monthly":
            var recRepeatMonthValue = $("input[name='rbtnRepeatMonth']:checked").val();
            var dateObj = new Date();
            var month = dateObj.getUTCMonth();
            var year = dateObj.getUTCFullYear();
            var hours = $("#timef" + $("#hfRecAppTypeId").val()).val().split(":")[0];
            var minutes = $("#timef" + $("#hfRecAppTypeId").val()).val().split(":")[1];
            switch (recRepeatMonthValue) {
                case "1":
                    var date = new Date(year, month, parseInt($("#txtRepeatMonthDay").val()));
                    date.setHours(hours);
                    date.setMinutes(minutes);
                    recObject.Rec_Pattern = "month_" + $("#ddEveryMonthDay").val() + "___";
                    recObject.Rec_Type = "month_" + $("#ddEveryMonthDay").val() + "___#";
                    recObject.Rec_Start_Date = date; //new Date(year, month, parseInt($("#txtRepeatMonthDay").val()));
                    recObject.Frequency_Type = 3; //3 is for Monthly selected radio button & Repeat selected radio button
                    break;
                case "2":
                    var recStartDateMonthly = new Date(apptStartDate);
                    recStartDateMonthly.setDate(recStartDateMonthly.getDate());
                    recStartDateMonthly.setHours(hours);
                    recStartDateMonthly.setMinutes(minutes);
                    recObject.Rec_Pattern = "month_" +
                        $("#ddOnEveryMonth").val() +
                        "_" +
                        $("#ddMonthWeekDays").val() +
                        "_" +
                        $("#ddOnRepeatMonth").val() +
                        "_";
                    recObject.Rec_Type = "month_" +
                        $("#ddOnEveryMonth").val() +
                        "_" +
                        $("#ddMonthWeekDays").val() +
                        "_" +
                        $("#ddOnRepeatMonth").val() +
                        "_#";
                    recObject.Rec_Start_Date = recStartDateMonthly;
                    recObject.Frequency_Type = 4; //4 is for Monthly selected radio button & On selected radio button
                    break;
                default:
                    break;
            }
            //var recStartDateMonthly = new Date(apptStartDate);
            //recStartDateMonthly.setDate(recStartDateMonthly.getDate() + 1);
            //recObject.Rec_Start_Date = recStartDateMonthly;

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
    $("#hfDone" + $("#hfRecAppTypeId").val()).val("1");
    //Added 1 to check the recurrence is done for particular procedure.
    apptRecArray.push(recObject);
    CancelApppointTypeReccurrence("done");
    $("#chkIsRec" + $("#hfRecAppTypeId").val()).prop("checked", true);
};
var GetRecuranceObject = function () {
    //var recuranceObject = 
};
var CalculateMilliSecondsBetweenDates = function (startdate, enddate) {
    var fromTime = new Date(startdate);
    var toTime = new Date(enddate);
    var differenceTravel = toTime.getTime() - fromTime.getTime();
    var seconds = Math.floor((differenceTravel) / (1000));
    return seconds;
};
var CancelApppointTypeReccurrence = function (from) {
    $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
    if (from == "cancel" && $("#hfDone" + $("#hfRecAppTypeId").val()).val() == "0") {
        $("#btnEditRecurrence" + $("#hfRecAppTypeId").val()).hide();
        $("#chkIsRec" + $("#hfRecAppTypeId").val()).prop("checked", false);
        $("#hfRecAppTypeId").val("0");
        $("#txtRecEndByDate").val("");
        $("#txtRecEveryDay").val("1");
    }
    $("#rbtnDailyRecType").prop("checked", true);
    $("#daily").show();
    $("#weekly").hide();
    $("#monthly").hide();
};

function SearchSchedulingPatient() {
    /// <summary>
    ///     Searches the patient.
    /// </summary>
    /// <returns></returns>
    var isvalidSearch = ValidSechulingSearch();
    if (isvalidSearch) {
        var contactnumber = $("#txtMobileNumber").val() != ""
            ? $("#lblCountryCode").html() + "-" + $("#txtMobileNumber").val()
            : "";
        var jsonData = JSON.stringify({
            PatientID: 0,
            PersonLastName: $("#txtLastName").val(),
            PersonEmiratesIDNumber: $("#txtEmiratesNationalId").val(),
            PersonPassportNumber: $("#txtPassportnumber").val(),
            PersonBirthDate: $("#txtBirthDate").val(),
            ContactMobilePhone: contactnumber
        });
        $.ajax({
            type: "POST",
            url: schedulerUrl + "GetPatientInfoSchedulingSearchResult",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                $("#divSearchResult").empty();
                $("#divSearchResult").html(data);
                $("#collapseSerachResult").addClass("in");
                $("#collapsePatientSearch").removeClass("in");
                SetGridPaging("?",
                    "?PatientID=" +
                    0 +
                    "&PersonLastName=" +
                    $("#txtLastName").val() +
                    "&PersonEmiratesIDNumber=" +
                    $("#txtEmiratesNationalId").val() +
                    "&PersonPassportNumber=" +
                    $("#txtPassportnumber").val() +
                    "&PersonBirthDate=" +
                    $("#txtBirthDate").val() +
                    "&ContactMobilePhone=" +
                    $("#txtMobileNumber").val() +
                    "&");
            },
            error: function (msg) {
            }
        });
    }
}

function ValidSechulingSearch() {
    /// <summary>
    ///     Valids the search.
    /// </summary>
    /// <returns></returns>
    var txtvalue = 0;
    $("#ValidatePatientSearch input[type=text]")
        .each(function () {
            if ($(this).val() != "") {
                txtvalue = txtvalue + 1;
            }
        });
    if (txtvalue < 1) {
        ShowMessage("Confirm at least one piece of information", "Alert", "warning", true);
        return false;
    } else {
        return true;
    }
    return false;
}

/// <var>The view patient scheduling</var>
var ViewPatientScheduling = function (pid, pfirstname, plastname) {
    apptRecArray = []; //Clear reccurrence array
    $("#hidPatientId").val(pid);
    $("#spnSelectedPatient").html(pfirstname + " " + plastname);
    var checkedPhysician = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    //checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysician);
    //checkedNodeIds1(treeViewStatus.dataSource.view(), checkedStatusNodes);
    GetCheckedCheckBoxesTreeView("treeviewPhysician", checkedPhysician);
    GetCheckedCheckBoxesTreeView("treeviewStatusCheck", checkedStatusNodes);
    var jsonData = [];
    var physicianData = [];
    var statusData = [];
    var deptData = [];
    //------------Check for the Selected Physciain Ids
    if (checkedPhysician.length > 0) {
        for (var k = 0; k < checkedPhysician.length; k++) {
            physicianData[k] = {
                Id: checkedPhysician[k]
            };
        }
    } else {
        physicianData[0] = {
            Id: 0
        };
    }
    //------------Check for the Selected Status
    if (checkedStatusNodes.length > 0) {
        for (var j = 0; j < checkedStatusNodes.length; j++) {
            statusData[j] = {
                Id: checkedStatusNodes[j]
            };
        }
    } else {
        statusData[0] = {
            Id: 0
        };
    }
    jsonData[0] = {
        PhysicianId: physicianData,
        StatusType: statusData,
        SelectedDate: $("#divDatetimePicker").val(),
        Facility: $("#hidFacilitySelected").val(),
        ViewType: "3",//$("#hidSelectedView").val(),
        DeptData: deptData,
        PatientId: pid
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "GetPatientSchedulingUpdated",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            IntializeScheduler(data.mainList, physicianData, "1");
            $("#ValidatePatientSearch").clearForm();
            BuildPatientSchedulingView(data.patientNextAppointments);
            if (pid > 0) {
                $("#btnClearPatient").show();
            }
        },
        error: function (msg) {
        }
    });
};
var OnClickRecurrenceTypeBtn = function (e) {

    var ctrl = $(e)[0];
    var aptDate = $("#date" + $("#hfRecAppTypeId").val()).val();
    var recEndDate = new Date(aptDate);
    switch (ctrl.id) {
        case "rbtnDailyRecType":
            var txtRecEveryDay = $("#txtRecEveryDay").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryDay == "" ? 0 : txtRecEveryDay));
            $("#txtRecEndByDate")
                .val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + "/" + recEndDate.getFullYear());
            break;
        case "rbtnWeeklyRecType":
            var txtRecEveryWeekDays = $("#txtRecEveryWeekDays").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryWeekDays == "" ? 0 : txtRecEveryWeekDays * 7));
            $("#txtRecEndByDate")
                .val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + "/" + recEndDate.getFullYear());
            break;
        case "rbtnMonthlyRecType":
            var recRepeatMonthValue = $("input[name='rbtnRepeatMonth']:checked").val();
            switch (recRepeatMonthValue) {
                case "1":
                    var ddEveryMonthDay = $("#ddEveryMonthDay").val();
                    recEndDate.setDate(recEndDate.getDate() + parseInt(ddEveryMonthDay == "" ? 0 : ddEveryMonthDay * 30));
                    $("#txtRecEndByDate")
                        .val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + "/" + recEndDate.getFullYear());
                    break;
                case "2":
                    var ddOnEveryMonth = $("#ddOnEveryMonth").val();
                    recEndDate.setDate(recEndDate.getDate() + parseInt(ddOnEveryMonth == "" ? 0 : ddOnEveryMonth * 30));
                    $("#txtRecEndByDate")
                        .val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + "/" + recEndDate.getFullYear());
                    break;
            }
            break;
    }
};
var OnchangeRecurrenceCtrl = function (e) {

    var ctrl = e;
    var aptDate = $("#date" + $("#hfRecAppTypeId").val()).val();
    var recEndDate = new Date(aptDate);
    var recEndByDate = new Date($("#txtRecEndByDate").val());
    switch (ctrl) {
        case "txtRecEveryDay":
            var txtRecEveryDay = $("#txtRecEveryDay").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryDay == "" ? 0 : txtRecEveryDay));
            if (recEndDate > recEndByDate) {
                $("#txtRecEndByDate")
                    .val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + "/" + recEndDate.getFullYear());
            }
            break;
        case "txtRecEveryWeekDays":
            var txtRecEveryWeekDays = $("#txtRecEveryWeekDays").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryWeekDays == "" ? 0 : txtRecEveryWeekDays * 7));
            if (recEndDate > recEndByDate) {
                $("#txtRecEndByDate")
                    .val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + "/" + recEndDate.getFullYear());
            }
            break;
        case "ddEveryMonthDay":
        case "ddOnEveryMonth":
            var recRepeatMonthValue = $("input[name='rbtnRepeatMonth']:checked").val();
            switch (recRepeatMonthValue) {
                case "1":
                    var ddEveryMonthDay = $("#ddEveryMonthDay").val();
                    recEndDate.setDate(recEndDate.getDate() + parseInt(ddEveryMonthDay == "" ? 0 : ddEveryMonthDay * 30));
                    if (recEndDate > recEndByDate) {
                        $("#txtRecEndByDate")
                            .val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + "/" + recEndDate.getFullYear());
                    }
                    break;
                case "2":
                    var ddOnEveryMonth = $("#ddOnEveryMonth").val();
                    recEndDate.setDate(recEndDate.getDate() + parseInt(ddOnEveryMonth == "" ? 0 : ddOnEveryMonth * 30));
                    if (recEndDate > recEndByDate) {
                        $("#txtRecEndByDate")
                            .val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + "/" + recEndDate.getFullYear());
                    }
                    break;
            }
            break;
        default:
            break;
    }
};
var blockRecurrenceDiv = function (text) {
    $("#pLoadingText").html(text);
    $("#loader_event").show();
};
var unBlockRecurrenceDiv = function () {
    $("#pLoadingText").html();
    $("#loader_event").hide();
};
var blockOnlyDiv = function (text) {
    $("#pLoadingText_1").html(text);
    $("#loader_event_1").show();
};
var unBlockOnlyDiv = function () {
    $("#pLoadingText_1").html();
    $("#loader_event_1").hide();
};

function waitSeconds(iMilliSeconds) {
    var counter = 0, start = new Date().getTime(), end = 0;
    while (counter < iMilliSeconds) {
        end = new Date().getTime();
        counter = end - start;
    }
}


var BuildPatientSchedulingView = function (data) {
    var htmlHeaderRow =
        '<table class="table table-grid" id="PatientScheduledAppointment"><thead><tr class="gridHead"><th scope="col">Patient Name</th><th scope="col">Appointment Type</th><th scope="col">Physician Name</th><th scope="col">Physician Speciality</th><th scope="col">Physician Department</th><th scope="col">Appointment Date</th><th scope="col">Appointment Time Slot</th><th scope="col">Action</th></tr></thead><tbody>';
    var pagehtml = htmlHeaderRow;
    $("#divShowPatientNextAppointment").show();
    if (data.length > 0) {
        scheduler.updateView(new Date($("#divDatetimePicker").val()), "month");
        $.each(data,
            function (i, obj) {
                var startDate = new Date(obj.start_date);
                var endDate = new Date(obj.end_date);
                var startHours = startDate.getHours().toString().length > 1
                    ? startDate.getHours().toString()
                    : "0" + startDate.getHours().toString();
                var startMins = startDate.getMinutes().toString().length > 1
                    ? startDate.getMinutes().toString()
                    : "0" + startDate.getMinutes().toString();
                var endHours = endDate.getHours().toString().length > 1
                    ? endDate.getHours().toString()
                    : "0" + endDate.getHours().toString();
                var endMins = endDate.getMinutes().toString().length > 1
                    ? endDate.getMinutes().toString()
                    : "0" + endDate.getMinutes().toString();
                var timeslot = startHours + ":" + startMins + " - " + endHours + ":" + endMins;
                pagehtml += '<tr class="gridRow"><td class="col1"><span>' +
                    obj.PatientName +
                    '</span></td><td class="col2"><span>' +
                    obj.AppointmentTypeStr +
                    '</span></td> <td class="col3"><span>' +
                    obj.PhysicianName +
                    '</span></td><td class="col4">' +
                    obj.PhysicianSpecialityStr +
                    '</td><td class="col5"> <span>' +
                    obj.DepartmentName +
                    '</span></td><td class="col7">' +
                    startDate.toLocaleDateString() +
                    '</td><td class="col8 align-center">' +
                    timeslot +
                    '</td><td class="col10"><span><a title="Select Patient" style="width: 15px; margin-right: 7px; float: left;" schId="' +
                    obj.id +
                    '" onclick="return EditScheduling(this);"  href="javascript:void(0);"><img src="/images/edit.png"></a></span></td></tr>';;
                $("#spnPatientName").html(obj.PatientName);
            });
        $("#patientAppointmentTypes").empty().html(pagehtml);
    } else {
        $("#patientAppointmentTypes").empty().html("<h2>No Record found</h2>");
    }
    $("#collapseSerachResult").removeClass("in");

    $("#collapsePatientScheduledAppointments").addClass("in");
};

var EditScheduling = function (e) {
    var ctrl = $(e);
    var schedulingEventId = ctrl.attr("schId");
    HidepopupObject("divSearchPatient");
    OpenPatientLightBox(schedulingEventId);
    /*var jsonData = JSON.stringify({
        id: schedulingEventId
    });
    $.ajax({
        type: "POST",
        url: '/FacultyTimeslots/GetSchedulingById',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            HidepopupObject('divSearchPatient');
            OpenPatientLightBox(data);
        },
        error: function (msg) {
        }
    });*/
};
var HidepopupObject = function (divid) {
    $("#" + divid).hide();
    $.validationEngine.closePrompt(".formError", true);
    $("#divShowPatientNextAppointment").hide();
};

function LightBoxCode(ev) {
    $(".tooltip").hide();
    $(".dhx_cal_light").hide(); //To hide the light box.
    $(".dhx_cal_cover").attr("style", "z-index:0");
    $(".dhx_cal_cover").hide();
    if (firstTimeBind) {
        firstTimeBind = false;
        //ajaxStartActive = false;
        var eventDetails = scheduler.getEvent(ev);
        if (eventDetails == undefined || eventDetails == null) {
            firstTimeBind = true;
            ajaxStartActive = true;
            ShowMessage("Something went wrong! Please try after some time.", "Alert", "warning", true);
            setTimeout(function () { window.location.reload(); }, 2000);
            return false;
        }
        schedularEventObj = ev;
        if (eventDetails.EventParentId != null) {
            $("#hidSchedulingType").val(eventDetails.SchedulingType);
            ShowLightBoxStyle(eventDetails.SchedulingType);
            $("#divPhysicianComment").show();
            BindLightBox(eventDetails);
        } else {
            $("#divSchedularPopUp").show();
            $("#btnSaveSchedulingData").val("Save");
        }
        var scrollPosition = -1;
        $(".search")
            .autocomplete({
                autoFocus: true,
                minLength: 3, // only start searching when user enters 3 characters
                source: function (request, response) {
                    ajaxStartActive = false;
                    $.ajax({
                        url: "/PatientInfo/GetPatientInfoByPatientName",
                        type: "POST",
                        dataType: "json",
                        data: {
                            patientName: $("#patientname").val(),
                            facilityId: $("#ddFacility").val()
                        },
                        success: function (data) {
                            $("#imgPatientExist").hide();
                            $("#imgPatientNew").show();
                            $("#hfPatientId").val("0");
                            response(jQuery.map(data,
                                function (item) {
                                    //var dateString = item.PersonBirthDate.substr(6);
                                    //var currentTime = new Date(parseInt(dateString));
                                    //var month = currentTime.getMonth() + 1;
                                    //var day = currentTime.getDate();
                                    //var year = currentTime.getFullYear();
                                    //var date = month + "/" + day + "/" + year;
                                    return {
                                        person_email_id: item.PersonEmailAddress,
                                        perosn_first_name: item.PersonFirstName,
                                        person_last_name: item.PersonLastName,
                                        person_phone_number: item.PatientPhone[0].PhoneNo,
                                        person_id: item.PatientID,
                                        person_dob: item.PersonBirthDate,
                                        person_emirates_national_id: item.PersonEmiratesIDNumber,
                                        label: item.PersonTitle, //item.PersonFirstName + " " + item.PersonLastName + " (" + date + ")",
                                        value: item.PersonFirstName + " " + item.PersonLastName,
                                        country: item.PersonCountry != null ? item.PersonCountry : 199,
                                        person_dob_str: item.PersonHusbandMobile
                                    };
                                }));
                        }
                    });
                },
                select: function (event, ui) {
                    ajaxStartActive = false;
                    $("#email").val(ui.item.person_email_id);
                    $("#phoneno").val(ui.item.person_phone_number);

                    $("#ddlPatientCountries").val(ui.item.country);
                    MaskedPhone("#lblPatientCountryCode", "#ddlPatientCountries", "#phoneno");

                    $("#patientname").val(ui.item.perosn_first_name + " " + ui.item.person_last_name);
                    $("#hfPatientId").val(ui.item.person_id);

                    ///*Convert json date to date format - start*/
                    //var dateString = ui.item.person_dob.substr(6);
                    //var currentTime = new Date(parseInt(dateString));
                    //var month = currentTime.getMonth() + 1;
                    //var day = currentTime.getDate();
                    //var year = currentTime.getFullYear();
                    //var date = month + "/" + day + "/" + year;
                    ///*Convert json date to date format - end*/
                    $("#txtPersonDOB").val(ui.item.person_dob_str);
                    $("#txtEmiratesNationalId").val(ui.item.person_emirates_national_id);
                    $("#imgPatientExist").show();
                    $("#imgPatientNew").hide();
                    BindPatientEncounterList(ui.item.person_id);
                    return false;
                },
                change: function (event, ui) {
                    ajaxStartActive = false;
                },
            });
        $("#divContent_1")
            .scroll(function () {
                if (scrollPosition != -1) {
                    $("#divContent_1").scrollTop(scrollPosition);
                }
            });
        //$(".dhx_cal_light").attr();//To hide the light box.
        //$('.hidePopUp').show();
        ajaxStartActive = true;
        $("#divSchedulerData").validationEngine();
    }
}

function OpenPatientLightBox(ev) {
    //scheduler.showLightbox(1448878480941);
    LightBoxCode(ev);
    //scheduler.addEventNow();
}

var UncheckAllCheckBox = function (divId) {
    $("#" + divId + " input:checkbox").removeAttr("checked");
    SetSelectedViewAsWeek();
    switch (divId.toLowerCase()) {
        case "treeviewfacilitydepartment":
        case "treeviewfacilityrooms":
            //var treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");
            //UncheckCheckedNode(treeViewDepartments.dataSource.view());
            onCheckDepartment();
            break;
        case "treeviewphysician":
            //var treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");
            //UncheckCheckedNode(treeViewPhysician.dataSource.view());
            onCheckFilters();
            break;
        case "treeviewstatuscheck":
            //var treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");
            //UncheckCheckedNode(treeViewStatus.dataSource.view());
            onCheckFilters();
            break;
        //case "treeviewfacilityrooms":
        //    //var treeViewRooms = $("#treeviewFacilityRooms").data("kendoTreeView");
        //    //UncheckCheckedNode(treeViewRooms.dataSource.view());
        //    onCheckFilters();
        //    break;
        default:
    }
    //scheduler.updateView(new Date($('#divDatetimePicker').val()), "week");
};

function UncheckCheckedNode(nodes) {
    /// <summary>
    ///     Checkeds the node ids1.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    /// <param name="checkedNodes1">The checked nodes1.</param>
    /// <returns></returns>
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].checked) {
            nodes[i].checked = false;
        }
    }
}


function ShowOverViewData() {
    schSessionInterval();
    var isValid = jQuery("#divFacilityOverview").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var facilityId = $("#selOVFacility").val();
        var departmentId = $("#selOVDepartment").val();
        var physician = $("#selOVPhysician").val();
        var patient = $("#selOVPatient").val();
        var appointmentType = $("#selOVApptTypes").val();
        var timeSlotFrequency = $("#txtOVApptTypesFrequency").val();
        var fromDate = $("#txtOVDateFrom").val();
        var endDate = $("#txtOVDateTo").val();
        var fromTime = $("#txtOVTimeFrom").val();
        var endTime = $("#txtOVTimeTo").val();


        var searchUrl = schedulerUrl + "GetOverView?FromDate=" +
            fromDate +
            "&ToDate=" +
            endDate +
            "&FromTime=" +
            fromTime +
            "&ToTime=" +
            endTime +
            "&TimeSlotFrequency=" +
            timeSlotFrequency +
            "&AppointmentType=" +
            appointmentType +
            "&FacilityId=" +
            facilityId +
            "&DepartmentId=" +
            departmentId +
            "&Physician=" +
            physician +
            "&Patient=" +
            patient +
            "&Room=0&ViewType=0";

        if ($.fn.dataTable.isDataTable("#tblOVGrid")) {
            var tablen = $("#tblOVGrid").DataTable();
            tablen.destroy();
        }
        var properties = {
            bProcessing: true,
            ajax: searchUrl,
            fixedHeader: true,
            'bDestroy': true,
            "bInfo": true,
            "bDeferRender": true,
            //"bDestroy": true,
            "sScrollY": "289",
            "sScrollX": "100%",
            "aoColumnDefs": [
                {
                    "mDataProp": "PhysicianName",
                    "aTargets": [0],
                    "bVisible": true,
                    //,
                    //"render": function (data, type, full, meta) {
                    //    return '<div  style="padding-left: 5px !important; cursor:pointer;" onclick="RedirectToYearlyView(' + full.ID + ',' + $("#StockSubCategory").val() + ')">' + data + '</div>';
                    //}
                },
                {
                    "mDataProp": "RID",
                    "aTargets": [1],
                    "bVisible": true
                    //,
                    //"render": function (data, type, full, meta) {
                    //    return '<div class="colorDiv" style="color:' + GetForColorCode(data) + '; background-color:' + GetColorCode(data) + '">' + data + '</div>';
                    //    //height: 15px; width: 15px;
                    //}
                },
                {
                    "mDataProp": "STime",
                    "aTargets": [2],
                    "bVisible": true /*,
                "render": function (data, type, full, meta) {
                    //return '<div class="colorDiv" style=" background-color:' + data + '"></div>';
                    return '<div class="colorDiv" style="color:' + GetForColorCode(data) + '; background-color:' + GetColorCode(data) + '">' + data + '</div>';
                }*/
                },
                {
                    "mDataProp": "ETime",
                    "aTargets": [3],
                    "bVisible": true /*,
                "render": function (data, type, full, meta) {
                    //return '<div class="colorDiv" style=" background-color:' + data + '"></div>';
                    return '<div class="colorDiv" style="color:' + GetForColorCode(data) + '; background-color:' + GetColorCode(data) + '">' + data + '</div>';
                }*/
                },
                {
                    "mDataProp": "D1",
                    "aTargets": [4],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 1);
                    }*/
                },
                {
                    "mDataProp": "D2",
                    "aTargets": [5],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 2);
                    }*/
                },
                {
                    "mDataProp": "D3",
                    "aTargets": [6],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 3);
                    }*/
                },
                {
                    "mDataProp": "D4",
                    "aTargets": [7],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 4);
                    }*/
                },
                {
                    "mDataProp": "D5",
                    "aTargets": [8],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 5);
                    }*/
                },
                {
                    "mDataProp": "D6",
                    "aTargets": [9],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 6);
                    }*/
                },
                {
                    "mDataProp": "D7",
                    "aTargets": [10],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 7);
                    }*/
                },
                {
                    "mDataProp": "D8",
                    "aTargets": [11],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 8);
                    }*/
                },
                {
                    "mDataProp": "D9",
                    "aTargets": [12],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 9);
                    }*/
                },
                {
                    "mDataProp": "D10",
                    "aTargets": [13],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 10);
                    }*/
                },
                {
                    "mDataProp": "D11",
                    "aTargets": [14],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 11);
                    }*/
                },
                {
                    "mDataProp": "D12",
                    "aTargets": [15],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 12);
                    }*/
                },
                {
                    "mDataProp": "D13",
                    "aTargets": [16],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 13);
                    }*/
                },
                {
                    "mDataProp": "D14",
                    "aTargets": [17],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 14);
                    }*/
                },
                {
                    "mDataProp": "D15",
                    "aTargets": [18],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 15);
                    }*/
                },
                {
                    "mDataProp": "D16",
                    "aTargets": [19],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 16);
                    }*/
                },
                {
                    "mDataProp": "D17",
                    "aTargets": [20],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 17);
                    }*/
                },
                {
                    "mDataProp": "D18",
                    "aTargets": [21],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 18);
                    }*/
                },
                {
                    "mDataProp": "D19",
                    "aTargets": [22],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 19);
                    }*/
                },
                {
                    "mDataProp": "D20",
                    "aTargets": [23],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 20);
                    }*/
                },
                {
                    "mDataProp": "D21",
                    "aTargets": [24],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 21);
                    }*/
                },
                {
                    "mDataProp": "D22",
                    "aTargets": [25],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 22);
                    }*/
                },
                {
                    "mDataProp": "D23",
                    "aTargets": [26],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 23);
                    }*/
                },
                {
                    "mDataProp": "D24",
                    "aTargets": [27],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 24);
                    }*/
                },
                {
                    "mDataProp": "D25",
                    "aTargets": [28],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 25);
                    }*/
                },
                {
                    "mDataProp": "D26",
                    "aTargets": [29],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 26);
                    }*/
                },
                {
                    "mDataProp": "D27",
                    "aTargets": [30],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 27);
                    }*/
                },
                {
                    "mDataProp": "D28",
                    "aTargets": [31],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 28);
                    }*/
                },
                {
                    "mDataProp": "D29",
                    "aTargets": [32],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 29);
                    }*/
                },
                {
                    "mDataProp": "D30",
                    "aTargets": [33],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 30);
                    }*/
                },
                {
                    "mDataProp": "D31",
                    "aTargets": [34],
                    "bVisible": true /*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 31);
                    }*/
                },
                {
                    "mDataProp": "PID",
                    "aTargets": [35],
                    "bVisible": false
                },
                {
                    "mDataProp": "DPID",
                    "aTargets": [36],
                    "bVisible": false
                }
            ],
            //"aaSortingFixed": [[0, 'asc']],
            //"aaSorting": [[1, 'asc']],
            "bSort": false,
            "iDisplayLength": 100,
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).attr("id", "tblData_" + iDataIndex);
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                //$('#example tbody tr td').each(function () {
                //    this.setAttribute('title', $(this).text());
                //});
                //$('td', nRow).prop("title", "test");
                return nRow;
            }

        };
        var table = $("#tblOVGrid").DataTable(properties);
        setTimeout(function () {
            $("div[title]")
                .qtip({
                    position: {
                        my: "top center",
                        target: "mouse",
                        adjust: {
                            mouse: false
                        }
                    },
                    content: {
                        //text: 'I really like owls!',
                        //title: 'Tooltip Tittle'

                    },
                    //style: {
                    //    border: {
                    //        width: 3,
                    //        radius: 8,
                    //        color: '#6699CC'
                    //    },
                    //    //width: 1000
                    //},
                    hide: {
                        delay: 200,
                        fixed: true // Make it fixed so it can be hovered over
                    }
                });
            //if ($('#tblOVGrid tr').length > 0) {
            //    $('#tblOVGrid tbody tr').each(function (i, row) {
            //        
            //        var $actualRow = $(row).find('td');
            //        for (var l = 0; l < $actualRow.length; l++) {
            //            var phyName = $($actualRow[0]).html();
            //            var roomId = $($actualRow[1]).html();
            //            var depId = $($actualRow[36]).html();
            //            var sTime = $($actualRow[2]).html();
            //            var eTime = $($actualRow[3]).html();
            //            var phyId = $($actualRow[35]).html();
            //            var data = $($actualRow[l]).html();
            //            var day = 21;
            //            var retHTML = GetColorCodeStringAfterBind(data, phyName, roomId, depId, eTime, sTime, phyId, day);
            //            $($actualRow[l]).html(retHTML);
            //        }
            /*$actualRow.each(function() {
                var t = $(this).html();
            });*/

            //console.log($actualRow.find('.color_td').html());
            //console.log($actualRow.find('.color_td').html().indexOf('0'));
            /*var tt = $actualRow.find('.color_td').html().indexOf('0');
            if (parseInt(tt) > -1) {
                $actualRow.find('.color_td').addClass('green_column');
            }*/


            // });
            //}
        },
            10000);
        //new $.fn.dataTable.FixedHeader(table);
    }
}

function OnClickOverViewBtn() {
    $("#divOverViewPopup").show();
    $("#divSchedularPopUp").hide();
    $("#divFacilityOverview").validationEngine();
    if ($.fn.dataTable.isDataTable("#tblOVGrid")) {
        var tablen = $("#tblOVGrid").DataTable();
        tablen.destroy();
    }
    $("#tblOVGrid").empty();
}

function OnClickOverViewCancel() {
    $("#divOverViewPopup").hide();
    $("#divSchedularPopUp").show();
}

function BindPatient(selector) {
    $.ajax({
        type: "POST",
        url: "/PatientInfo/GetPatientInfoByPatientName",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            facilityId: $("#selOVFacility").val()
        }),
        success: function (data) {

            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data,
                    function (i, facility) {
                        items += "<option value='" +
                            facility.PatientID +
                            "'>" +
                            facility.PersonFirstName +
                            " " +
                            facility.PersonLastName +
                            "</option>";
                    });
                $(selector).html("");
                $(selector).html(items);
            } else {
            }
        },
        error: function (msg) {
        }
    });
}

function GetColorCodeStringAfterBind(data, phyName, RID, DPID, ETime, STime, PID, day) {
    // var tt = '<div title="<div class=qtip_title><h2>Physician: 2036</h2><h2>Room: 36</h2></div> <div class=tooltip_row>~FROM- 00:00 TO-23:59 --) Faculty Unavailable Due To: Holiday</div>" class="color_td grey_column" attr-DeptId="6" attr-RoomId="36" attr-Day="11"  attr-EndTime ="00:00" attr-StartTime ="04:00" attr-PhysicianId ="2036" onclick="OnClickGetData(this);">8<div class="selected_column" style="display:none;"></div></div>';
    var showData = "";
    var showTitle = "";
    var showClass = "green_column";
    if (data.indexOf("~") > -1) {

        var splitArray = data.split("~");
        showData = splitArray[0];
        showClass = GetShowDataValue(splitArray[1]);
        showTitle += "<div class='qtip_title'><h2>Physician: " + phyName + "</h2><h2>Room: " + RID + "</h2></div>";
        for (var i = 2; i < splitArray.length; i++) {
            showTitle += "<div class='tooltip_row'>" + splitArray[i] + "</div>";
        }
    } else {
        showData = data;
    }
    var currentDate = new Date($("#txtOVDateFrom").val());
    var retValue = "";
    if (day >= currentDate.getDate()) {
        retValue = '<div title="' +
            showTitle.replace(/<br>\s*$/, "") +
            '" class="color_td ' +
            showClass +
            '" attr-DeptId="' +
            DPID +
            '" attr-RoomId="' +
            RID +
            '" attr-Day="' +
            day +
            '"  attr-EndTime ="' +
            ETime +
            '" attr-StartTime ="' +
            STime +
            '" attr-PhysicianId ="' +
            PID +
            '"  onclick=OnClickGetData(this)>' +
            showData +
            '<div class="selected_column" style="display:none;"></div></div>';
    } else {
        retValue = '<div title="' +
            showTitle.replace(/<br>\s*$/, "") +
            '" class="color_td grey_column" attr-DeptId="' +
            DPID +
            '" attr-RoomId="' +
            RID +
            '" attr-Day="' +
            day +
            '"  attr-EndTime ="' +
            ETime +
            '" attr-StartTime ="' +
            STime +
            '" attr-PhysicianId ="' +
            PID +
            '"  >' +
            showData +
            '<div class="selected_column" style="display:none;"></div></div>';
    }
    return retValue;
}

function GetGolorCodeString(data, full, day) {
    var showData = "";
    var showTitle = "";
    var showClass = "green_column";
    if (data.indexOf("~") > -1) {

        var splitArray = data.split("~");
        showData = splitArray[0];
        showClass = GetShowDataValue(splitArray[1]);
        showTitle += "<div class='qtip_title'><h2>Physician: " +
            full.PhysicianName +
            "</h2><h2>Room: " +
            full.RID +
            "</h2></div>";
        for (var i = 2; i < splitArray.length; i++) {
            showTitle += "<div class='tooltip_row'>" + splitArray[i] + "</div>";
        }
    } else {
        showData = data;
    }
    var currentDate = new Date($("#txtOVDateFrom").val());
    var retValue = "";
    if (day >= currentDate.getDate()) {
        retValue = '<div title="' +
            showTitle.replace(/<br>\s*$/, "") +
            '" class="color_td ' +
            showClass +
            '" attr-DeptId="' +
            full.DPID +
            '" attr-RoomId="' +
            full.RID +
            '" attr-Day="' +
            day +
            '"  attr-EndTime ="' +
            full.ETime +
            '" attr-StartTime ="' +
            full.STime +
            '" attr-PhysicianId ="' +
            full.PID +
            '"  onclick=OnClickGetData(this)>' +
            showData +
            '<div class="selected_column" style="display:none;"></div></div>';
        /*switch (showData) {
            case "0":
                retValue = '<div title="' + showTitle.replace(/<br>\s*$/, "") + '" class="color_td ' + showClass + '" attr-DeptId="' + full.DPID + '" attr-RoomId="' + full.RID + '" attr-Day="' + day + '"  attr-EndTime ="' + full.ETime + '" attr-StartTime ="' + full.STime + '" attr-PhysicianId ="' + full.PID + '"  onclick=OnClickGetData(this)>' + showData + '<div class="selected_column" style="display:none;"></div></div>';
                break;
            case "1":
                retValue = '<div title="' + showTitle.replace(/<br>\s*$/, "") + '" class="color_td ' + showClass + '" attr-DeptId="' + full.DPID + '" attr-RoomId="' + full.RID + '" attr-Day="' + day + '"  attr-EndTime ="' + full.ETime + '" attr-StartTime ="' + full.STime + '" attr-PhysicianId ="' + full.PID + '"  onclick=OnClickGetData(this)>' + showData + '<div class="selected_column" style="display:none;"></div></div>';
                break;
            case "2":
                retValue = '<div title="' + showTitle.replace(/<br>\s*$/, "") + '" class="color_td ' + showClass + '" attr-DeptId="' + full.DPID + '" attr-RoomId="' + full.RID + '" attr-Day="' + day + '"  attr-EndTime ="' + full.ETime + '" attr-StartTime ="' + full.STime + '" attr-PhysicianId ="' + full.PID + '"  onclick=OnClickGetData(this)>' + showData + '<div class="selected_column" style="display:none;"></div></div>';
                break;
            default:
                retValue = '<div title="' + showTitle.replace(/<br>\s*$/, "") + '" class="color_td ' + showClass + '" attr-DeptId="' + full.DPID + '" attr-RoomId="' + full.RID + '" attr-Day="' + day + '"  attr-EndTime ="' + full.ETime + '" attr-StartTime ="' + full.STime + '" attr-PhysicianId ="' + full.PID + '"  onclick=OnClickGetData(this)>' + showData + '<div class="selected_column" style="display:none;"></div></div>';
                break;
        }*/
    } else {
        retValue = '<div title="' +
            showTitle.replace(/<br>\s*$/, "") +
            '" class="color_td grey_column" attr-DeptId="' +
            full.DPID +
            '" attr-RoomId="' +
            full.RID +
            '" attr-Day="' +
            day +
            '"  attr-EndTime ="' +
            full.ETime +
            '" attr-StartTime ="' +
            full.STime +
            '" attr-PhysicianId ="' +
            full.PID +
            '"  >' +
            showData +
            '<div class="selected_column" style="display:none;"></div></div>';
        /*switch (showData) {
            case "0":
                retValue = '<div title="' + showTitle.replace(/<br>\s*$/, "") + '" class="color_td grey_column" attr-DeptId="' + full.DPID + '" attr-RoomId="' + full.RID + '" attr-Day="' + day + '"  attr-EndTime ="' + full.ETime + '" attr-StartTime ="' + full.STime + '" attr-PhysicianId ="' + full.PID + '" >' + showData + '<div class="selected_column" style="display:none;"></div></div>';
                break;
            case "1":
                retValue = '<div title="' + showTitle.replace(/<br>\s*$/, "") + '" class="color_td grey_column" attr-DeptId="' + full.DPID + '" attr-RoomId="' + full.RID + '" attr-Day="' + day + '"  attr-EndTime ="' + full.ETime + '" attr-StartTime ="' + full.STime + '" attr-PhysicianId ="' + full.PID + '" >' + showData + '<div class="selected_column" style="display:none;"></div></div>';
                break;
            case "2":
                retValue = '<div title="' + showTitle.replace(/<br>\s*$/, "") + '" class="color_td grey_column" attr-DeptId="' + full.DPID + '" attr-RoomId="' + full.RID + '" attr-Day="' + day + '"  attr-EndTime ="' + full.ETime + '" attr-StartTime ="' + full.STime + '" attr-PhysicianId ="' + full.PID + '"  >' + showData + '<div class="selected_column" style="display:none;"></div></div>';
                break;
            default:
                retValue = '<div title="' + showTitle.replace(/<br>\s*$/, "") + '" class="color_td grey_column" attr-DeptId="' + full.DPID + '" attr-RoomId="' + full.RID + '" attr-Day="' + day + '"  attr-EndTime ="' + full.ETime + '" attr-StartTime ="' + full.STime + '" attr-PhysicianId ="' + full.PID + '"  >' + showData + '<div class="selected_column" style="display:none;"></div></div>';
                break;
        }*/
    }
    return retValue;
}

function OnChangeOVAppointTypes(e) {
    var timeSlot = $(e).find("option:selected").attr("timeslot");
    $("#txtOVApptTypesFrequency").val(timeSlot);
    $("#hfOVApptTypesFrequency").val(timeSlot);
}

function OnClickGetData(e, physicianId, sTime, eTime) {

    $(".selected_column").hide();
    $(e).find(".selected_column").show();
    var day = $(e).attr("attr-Day");
    var year = new Date($("#txtOVDateFrom").val()).getFullYear();
    var month = new Date($("#txtOVDateFrom").val()).getMonth();
    var facilityId = $("#selOVFacility").val();
    var departmentId = $(e).attr("attr-DeptId"); //$("#selOVDepartment").val();
    var physician = $(e).attr("attr-PhysicianId");
    var patient = $("#selOVPatient").val();
    var appointmentType = $("#selOVApptTypes").val();
    var timeSlotFrequency = $("#hfOVApptTypesFrequency").val();
    var fromDate = (month + 1) + "/" + day + "/" + year; //$("#txtOVDateFrom").val();
    var endDate = $("#txtOVDateTo").val();
    var fromTime = $(e).attr("attr-StartTime");
    var endTime = $(e).attr("attr-EndTime");
    var room = $(e).attr("attr-RoomId");
    $("#hfRoomId").val(room);
    var searchUrl = schedulerUrl + "GetOverView?FromDate=" +
        fromDate +
        "&ToDate=" +
        endDate +
        "&FromTime=" +
        fromTime +
        "&ToTime=" +
        endTime +
        "&TimeSlotFrequency=" +
        timeSlotFrequency +
        "&AppointmentType=" +
        appointmentType +
        "&FacilityId=" +
        facilityId +
        "&DepartmentId=" +
        departmentId +
        "&Physician=" +
        physician +
        "&Patient=" +
        patient +
        "&Room=" +
        room;
    $.ajax({
        type: "POST",
        url: searchUrl,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //data: JSON.stringify(searchUrl),
        success: function (data) {

            $(".newrowadded").remove();
            var clickedRowId = $(e).parent().parent()[0].id;

            $("#ulOVTimeSlots").html("");
            var html = "";
            var data1 = GetFilteredResult(day, data);
            if (data1.length > 0) {
                for (var i = 0; i < data1.length; i++) {
                    html += '<li id="' +
                        data1[i].STime +
                        " - " +
                        data1[i].ETime +
                        '" attr-selDate="' +
                        fromDate +
                        '" attr-etime="' +
                        data1[i].ETime +
                        '" attr-stime="' +
                        data1[i].STime +
                        '" attr-fid="' +
                        facilityId +
                        '" attr-pid="' +
                        physician +
                        '" dopd="" onclick="SelectTimeSlot(this,' +
                        appointmentType +
                        ',2,"")">' +
                        data1[i].STime +
                        " - " +
                        data1[i].ETime +
                        "</li>";
                }
            } else {
                html += "<li>No Time slots are available</li>";
            }
            var newRowHtml = '<tr id="newrow' +
                clickedRowId +
                '" class="newrowadded"><td colspan="35" class="timingslots"><ul>' +
                html +
                '</ul><div class="deleterow" onclick="OnClickHideMinus();"><span class="glyphicon glyphicon-remove" aria-hidden="true"></span></div></td></tr>';
            var newrow = $(newRowHtml);
            $("#" + clickedRowId).after(newrow);
            //$("#ulOVTimeSlots").html(html);
            //$('#divOVAvailableTimeSlots').addClass('moveLeft2');
        },
        error: function (msg) {
        }
    });
}

function OnClickOVHide() {
    $("#divOVAvailableTimeSlots").removeClass("moveLeft2");
}

function OnClickHideMinus() {
    $(".newrowadded").remove();
    $(".selected_column").hide();
}

function GetFilteredResult(day, data) {
    var d = "";
    switch (day) {
        case "1":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D1).html().substring(0, $(record.D1).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "2":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D2).html().substring(0, $(record.D2).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "3":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D3).html().substring(0, $(record.D3).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "4":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D4).html().substring(0, $(record.D4).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "5":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D5).html().substring(0, $(record.D5).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "6":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D6).html().substring(0, $(record.D6).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "7":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D7).html().substring(0, $(record.D7).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "8":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D8).html().substring(0, $(record.D8).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "9":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D9).html().substring(0, $(record.D9).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "10":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D10).html().substring(0, $(record.D10).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "11":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D11).html().substring(0, $(record.D11).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "12":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D12).html().substring(0, $(record.D12).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "13":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D13).html().substring(0, $(record.D13).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "14":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D14).html().substring(0, $(record.D14).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "15":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D15).html().substring(0, $(record.D15).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "16":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D16).html().substring(0, $(record.D16).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "17":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D17).html().substring(0, $(record.D17).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "18":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D18).html().substring(0, $(record.D18).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "19":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D19).html().substring(0, $(record.D19).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "20":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D20).html().substring(0, $(record.D20).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "21":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D21).html().substring(0, $(record.D21).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "22":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D22).html().substring(0, $(record.D22).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "23":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D23).html().substring(0, $(record.D23).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "24":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D24).html().substring(0, $(record.D24).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "25":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D25).html().substring(0, $(record.D25).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "26":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D26).html().substring(0, $(record.D26).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "27":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D27).html().substring(0, $(record.D27).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "28":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D28).html().substring(0, $(record.D28).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "29":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D29).html().substring(0, $(record.D29).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "30":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D30).html().substring(0, $(record.D30).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
        case "31":
            d = jLinq.from(data.aaData)
                .where(function (record) {

                    if ($(record.D31).html().substring(0, $(record.D31).html().lastIndexOf("\n")) == "0")
                        return record;
                })
                .select();
            break;
    }
    return d;
}

function GetShowDataValue(value) {
    value = parseInt(value);
    var cls = "green_column";
    if (value == 0) {
        cls = "green_column";
    } else if (value > 0 && value <= 25) {
        cls = "light_green_column";
    } else if (value > 25 && value <= 50) {
        cls = "yellow_column";
    } else if (value > 50 && value <= 75) {
        cls = "pink_column";
    } else if (value > 75 && value < 100) {
        cls = "purple_column";
    } else if (value >= 100) {
        cls = "red_column";
    }
    return cls;
}

var CheckAllCheckBox = function (divId) {
    $("#" + divId + " input:checkbox").prop("checked", true);
    SetSelectedViewAsWeek();
    switch (divId.toLowerCase()) {
        case "treeviewfacilitydepartment":
        case "treeviewfacilityrooms":
            //var treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");
            //CheckNodes(treeViewDepartments.dataSource.view());
            onCheckDepartment();
            break;
        case "treeviewphysician":
            //var treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");
            //CheckNodes(treeViewPhysician.dataSource.view());
            onCheckFilters();
            break;
        case "treeviewstatuscheck":
            //var treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");
            //CheckNodes(treeViewStatus.dataSource.view());
            onCheckFilters();
            break;
        //case "treeviewfacilityrooms":
        //    //var treeViewRooms = $("#treeviewFacilityRooms").data("kendoTreeView");
        //    //CheckNodes(treeViewRooms.dataSource.view());
        //    onCheckFilters();
        //    break;
        default:
    }
    //scheduler.updateView(new Date($('#divDatetimePicker').val()), "week");
};

function CheckNodes(nodes) {
    /// <summary>
    ///     Checkeds the node ids1.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    /// <param name="checkedNodes1">The checked nodes1.</param>
    /// <returns></returns>
    for (var i = 0; i < nodes.length; i++) {
        if (!nodes[i].checked) {
            nodes[i].checked = true;
        }
    }
}

var ValidateDepartmentRooms = function (e) {
    var jsonData = {
        facilityid: $("#hidFacilitySelected").val(),
        deptid: $(e).val()
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/ValidateDepartmentRooms",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            if (!data) {
                ShowMessage("Rooms are not available under selected department!", "Warning", "warning", true);
                $("#btnSreachOverview").attr("disabled", "disabled");
            } else {
                $("#btnSreachOverview").removeAttr("disabled");
            }
        },
        error: function (msg) {
        }
    });
};
var BindPhysiciansApptTypes = function (fid, deptid) {
    var jsonData = {
        facilityid: fid,
        deptid: deptid
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetPhysiciansApptTypes",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            var items = '<option value="0">--Select--</option>';
            $.each(data,
                function (i, appt) {
                    items += "<option value='" +
                        appt.Id +
                        "' timeslot='" +
                        appt.TimeSlot +
                        "'>" +
                        appt.Name +
                        "</option>";
                });
            $("#appointmentTypes").html("");
            $("#appointmentTypes").html(items);
        },
        error: function (msg) {
        }
    });
};

var CheckUnCheckBox = function (evt, div, mtype) {
    $("#" + div + " li").removeClass("active");
    //var checked = $(evt).prop("checked");
    //var obj = "";

    switch (mtype) {
        case "dept":
        case "rooms":
            //obj = $("#liDeptCheckBox_" + $(evt).attr("id").split("_")[1]);
            onCheckDepartment();
            break;
        //case "rooms":
        //obj = $("#liRoomCheckBox_" + $(evt).attr("id").split("_")[1]);
        //onCheckRooms();
        //break;
        case "physician":
            //obj = $("#liPhysicianCheckBox_" + $(evt).attr("id").split("_")[1]);
            onCheckFilters();
            break;
        case "status":
            //obj = $("#liStatusCheckBox_" + $(evt).attr("id").split("_")[1]);
            onCheckFilters();
            break;
        default:
    }
    //if (checked)
    //    $(obj).addClass("active");
    //else
    //    $(obj).removeClass("active");
};
/*
Who: Amit Jain
When: 13th March, 2016
What: Optimzated Code Written
*/
//##################################---New Code of SCHEDULER start here---###################################################


function BindAllSchedularData() {
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "GetAllSchedulingData",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            category: "4903",
            facilityId: $("#hidFacilitySelected").val(),
            corporateId: "",
            selectedDate: $("#divDatetimePicker").val(),
            viewType: $("#hidSelectedView").length > 0 && $("#hidSelectedView").val() > 0
                ? $("#hidSelectedView").val()
                : "1"
        }),
        success: function (data) {
            if (data != null) {
                //Bind Availibility Data
                BindDropdownData(data.gClist, "#ddAvailability", "1");

                //Bind Physicians Data
                BindPhysiciansDataInScheduler(data.pList, "#ddPhysician", "");
                BindPhysiciansDataInScheduler(data.pList, "#ddHolidayPhysician", "");
                BindPhysiciansDataInScheduler(data.pList, "#selOVPhysician", "");

                //Bind Holiday Statues Data
                BindDropdownData(data.hStatus, "#ddHolidayStatus", "7");

                //Bind Holiday Types List
                BindDropdownData(data.hTypes, "#ddHolidayType", "");

                //Bind Facilities
                BindDropdownData(data.fList, "#ddFacility", "#hidFacilitySelected");
                BindDropdownData(data.fList, "#selOVFacility", "#hidFacilitySelected");

                //$("#hidFacilitySelected").val(),
                //Bind Specialities
                BindDropdownData(data.specialities, "#ddSpeciality", "");

                //Bind Monthly Week Days
                BindDropdownData(data.mWeekDays, "#ddMonthWeekDays", "");

                //Initialize the first and last hourrs for the calendar view
                firstHour = data.StartHour != null && data.StartHour > 0 ? data.StartHour : 8;
                lastHour = data.EndHour != null && data.EndHour > 0 ? data.EndHour : 17;


                //Initialize the Schedular Control with the Current User's Data binded 
                IntializeScheduler(data.savedSlots, data.selectedPhysicians, "1");


                BindList("#divLocationList", data.facilityView);

                BindList("#divFacilityDepartmentList", data.fDepView);

                BindList("#divFacilityRoomList", data.fRoomsView);

                BindList("#divPhysicianList", data.phyView);

                BindList("#divStatusList", data.bStatusView);

                $('#hfPatientCountry').val(199);
                BindCountryCodeData("#ddlPatientCountries", "#hfPatientCountry", "#lblPatientCountryCode", data.countries);


                //Binding Deparments Data to Dropdown
                BindDropdownData(data.depList, "#selOVDepartment", "");


                //Binding Appointment Types Data to Dropdown
                var items = '<option value="0">--Select--</option>';
                if (data.appTypes != null) {
                    $.each(data.appTypes,
                        function (i, appt) {
                            items += "<option value='" +
                                appt.Id +
                                "' timeslot='" +
                                appt.TimeSlot +
                                "'>" +
                                appt.Name +
                                "</option>";
                        });

                    if ($("#appointmentTypes").length > 0)
                        BindList("#appointmentTypes", items);

                    if ($("#selOVApptTypes").length > 0)
                        BindList("#selOVApptTypes", items);
                }
            }
        },
        error: function (msg) {
        }
    });
}


function BindPhysiciansDataInScheduler(data, ddlSelector, hdSelector) {
    /// <summary>
    ///     Binds the dropdown data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="ddlSelector">The DDL selector.</param>
    /// <param name="hdSelector">The hd selector.</param>
    /// <returns></returns>
    $(ddlSelector).empty();
    var items = '<option value="0">--Select--</option>';
    $.each(data,
        function (i, obj) {
            var newItem = '<option value="' +
                obj.Physician.Id +
                '" facilityId="' +
                obj.Physician.FacilityId +
                '" department="' +
                obj.UserDepartmentStr +
                '" departmentId="' +
                obj.Physician.FacultyDepartment +
                '" speciality="' +
                obj.UserSpecialityStr +
                '" specialityId="' +
                obj.Physician.FacultySpeciality +
                '">' +
                obj.Physician.PhysicianName +
                " (" +
                obj.UserTypeStr +
                ")</option>";
            items += newItem;
        });

    $(ddlSelector).html(items);
    var hdValue = "";
    if (hdSelector.indexOf("#") != -1) {
        hdValue = $(hdSelector).val();
    } else {
        hdValue = hdSelector;
    }
    //
    if (hdValue != null && hdValue != "") {
        $(ddlSelector).val(hdValue);
        if ($(ddlSelector).val() == null || $(ddlSelector).val() == undefined) {
            $(ddlSelector + " option")
                .filter(function (index) { return $(this).text() == "" + hdValue + ""; })
                .attr("selected", "selected");
        }
    } else {
        if ($(ddlSelector).length > 0)
            $(ddlSelector)[0].selectedIndex = 0;
    }
}


function OnPhysicianSelectionInPreviousEncounters(facilityId, physicianId, departmentId, specialityId) {
    //BindAppointmentAvailability();
    //BindFacilityDepartmentspopup('#ddFacility');

    //BindPhysiciansApptTypes(facilityId, departmentId);
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "OnPhysicianSelectionFromPreviousEncountersList",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            category: "4903",
            facilityId: facilityId,
            facilityStructureId: departmentId
        }),
        success: function (data) {
            if (data != null) {

                //Bind Availibility Data
                BindDropdownData(data.finalGcList, "#ddAvailability", "1");

                //Bind Physicians Data
                BindPhysiciansDataInScheduler(data.pList, "#ddPhysician", physicianId.toString());

                //if ($("#ddHolidayPhysician").length > 0)
                //    BindPhysiciansDataInScheduler(data.pList, "#ddHolidayPhysician", physicianId);

                ////Bind Holiday Statues Data
                //if ($("#ddHolidayStatus").length > 0)
                //    BindDropdownData(data.hStatus, "#ddHolidayStatus", "7");

                ////Bind Holiday Types List
                //if ($("#ddHolidayType").length > 0)
                //    BindDropdownData(data.hTypes, "#ddHolidayType", "");

                //Binding Appointment Types Data to Dropdown
                var items = '<option value="0">--Select--</option>';
                if (data.appTypes != null && $("#appointmentTypes").length > 0) {
                    $.each(data.appTypes,
                        function (i, appt) {
                            items += "<option value='" + appt.Id + "'>" + appt.Name + "</option>";
                        });
                    BindList("#appointmentTypes", items);
                }

                $("#ddFacility").val(facilityId);
                //$("#ddPhysician").val(physicianId);

                $("#ddSpeciality").val(specialityId);

                //Remove css class to hide the Previous Encounters List Popup window
                $(".searchSlide").removeClass("moveLeft");
            }
        },
        error: function (msg) {
            console.log(msg);
        }
    });

}


//##################################---New Code of SCHEDULER start here---###################################################


/*
Who: Amit Jain
When: 17 May, 2016
What: Optimzated Code Written
*/
//##################################---New Code of Appointments Overview starts here---###################################################

function OnChangeFacilityDropdownInAppointmentsOverview(facilityDropdownSelector, physicianId) {
    if (facilityDropdownSelector == "")
        facilityDropdownSelector = "#ddFacility";
    $("#ddSpeciality").val("0");
    $.ajax({
        type: "POST",
        url: schedulerUrl + "OnChangeFacilityDropdownInAppointmentsOverview",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            fId: $(facilityDropdownSelector).val()
        }),
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';

                //Binding Deparments Data to Dropdown
                if (facilityDropdownSelector != "#ddFacility")
                    BindDropdownData(data.deps, "#selOVDepartment", "");

                //Binding Physicians Data to Dropdown
                BindPhysiciansDataInScheduler(data.physicians,
                    facilityDropdownSelector != "#selOVFacility" ? "#ddPhysician" : "#selOVPhysician",
                    physicianId.toString());

                //Binding Appointment Types Data to Dropdown
                if (data.aList != null) {
                    $.each(data.aList,
                        function (i, appt) {
                            items += "<option value='" +
                                appt.Id +
                                "' timeslot='" +
                                appt.TimeSlot +
                                "'>" +
                                appt.Name +
                                "</option>";
                        });
                    BindList(facilityDropdownSelector == "#ddFacility" ? "#appointmentTypes" : "#selOVApptTypes",
                        items);
                }

                //Binding Patients Data to Dropdown
                items = '<option value="0">--Select--</option>';
                $.each(data.pInfo,
                    function (i, facility) {
                        items += "<option value='" +
                            facility.PatientID +
                            "'>" +
                            facility.PersonFirstName +
                            " " +
                            facility.PersonLastName +
                            "</option>";
                    });
                BindList("#selOVPatient", items);
            }
        },
        error: function (msg) {
        }
    });
}


function OnTimeSlotsSelectionInAppOverview(facilityDropdownSelector, physicianId) {
    if (facilityDropdownSelector == "")
        facilityDropdownSelector = "#ddFacility";
    $("#ddSpeciality").val("0");
    $.ajax({
        type: "POST",
        url: schedulerUrl + "OnChangeFacilityDropdownInAppointmentsOverview",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            fId: $(facilityDropdownSelector).val()
        }),
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';

                //Binding Deparments Data to Dropdown
                if (facilityDropdownSelector != "#ddFacility")
                    BindDropdownData(data.deps, "#selOVDepartment", "");

                //Binding Physicians Data to Dropdown
                BindPhysiciansDataInScheduler(data.physicians,
                    facilityDropdownSelector != "#selOVFacility" ? "#ddPhysician" : "#selOVPhysician",
                    physicianId.toString());

                //Binding Appointment Types Data to Dropdown
                if (data.aList != null) {
                    $.each(data.aList,
                        function (i, appt) {
                            items += "<option value='" +
                                appt.Id +
                                "' timeslot='" +
                                appt.TimeSlot +
                                "'>" +
                                appt.Name +
                                "</option>";
                        });
                    BindList(facilityDropdownSelector == "#ddFacility" ? "#appointmentTypes" : "#selOVApptTypes",
                        items);
                }

                //Binding Patients Data to Dropdown
                items = '<option value="0">--Select--</option>';
                $.each(data,
                    function (i, facility) {
                        items += "<option value='" +
                            facility.PatientID +
                            "'>" +
                            facility.PersonFirstName +
                            " " +
                            facility.PersonLastName +
                            "</option>";
                    });
                BindList("#selOVPatient", items);
            }
        },
        error: function (msg) {
        }
    });
}

//##################################---New Code of Appointments Overview ends here---###################################################


//##################################---Changes done to remove kendo and add the HTML trees---###################################################
var BindCalenderView = function (viewtype, viewTypeName) {
    $("#hidSelectedView").val(viewtype);
    $("#hidSelectedViewName").val(viewTypeName);
    $("#treeviewCalenderType li").removeClass("active");
    if ($("#liviewCalenderType_" + viewtype).hasClass("active"))
        $("#liviewCalenderType_" + viewtype).removeClass("active");
    else
        $("#liviewCalenderType_" + viewtype).addClass("active");
    onCheckFilters();
};
var CheckFacilityPhysicians = function (id) {
    $("#treeviewPhysician li").removeClass("active");
    var previousCheck = $("#chk_" + id).prop("checked");
    $("#chk_" + id).prop("checked", !previousCheck);
    if ($("#liLocationCheckBox_" + id).hasClass("active"))
        $("#liLocationCheckBox_" + id).removeClass("active");
    else
        $("#liLocationCheckBox_" + id).addClass("active");
    onCheckFilters();
};
var BindLocationSechudler = function (id, name) {
    if (id > 0)
        BindLocationDataInScheduler(id, true, true);
    $("#treeviewLocations li").removeClass("active");
    $("#liLocationCheckBox_" + id).addClass("active");
    $("#hidFacilityName").val(name);
    $("#hidFacilitySelected").val(id);



    var checkedPhysician = [];
    GetCheckedCheckBoxesTreeView("treeviewPhysician", checkedPhysician);
    if (checkedPhysician.length > 0) {
        $("#btnScheduleAppointment").prop("disabled", false);
        //$("#btnAddHoliday").prop("disabled", false);
        onCheckFilters();
    } else {
        $("#btnScheduleAppointment").prop("disabled", true);
        //$("#btnAddHoliday").prop("disabled", true);
        scheduler.clearAll();
        scheduler.updateView(new Date($("#divDatetimePicker").val()), "week");
        ShowMessage("There is no Physician available for this facility!", "Warning", "warning", true);
    }
};
var GetCheckedCheckBoxesTreeView = function (objectToTraverse, checkedlist) {
    $("#" + objectToTraverse + " li input:checked")
        .each(function () {
            checkedlist.push($(this).attr("id").split("_")[1]);
        });
};
var CheckFacilityDepartments = function (id) {
    $("#treeviewFacilityDepartment li").removeClass("active");
    var previousCheck = $("#chkDept_" + id).prop("checked");
    $("#chkDept_" + id).prop("checked", !previousCheck);
    $("#liDeptCheckBox_" + id).addClass("active");
    onCheckDepartment();
};
var CheckFacilityRooms = function (id) {
    $("#treeviewFacilityRooms li").removeClass("active");
    var previousCheck = $("#chkRoom_" + id).prop("checked");
    $("#chkRoom_" + id).prop("checked", !previousCheck);
    $("#liRoomCheckBox_" + id).addClass("active");
    onCheckDepartment();
    //onCheckRooms();
};
var CheckStatus = function (id) {
    $("#treeviewStatusCheck li").removeClass("active");
    var previousCheck = $("#ChkStatus_" + id).prop("checked");
    $("#ChkStatus_" + id).prop("checked", !previousCheck);
    if ($("#liStatusCheckBox_" + id).hasClass("active"))
        $("#liStatusCheckBox_" + id).removeClass("active");
    else
        $("#liStatusCheckBox_" + id).addClass("active");
    onCheckFilters();
};
var BindPhyPreviousVacations = function () {
    var jsonData = {
        facilityid: $("#hidFacilitySelected").val(),
        physicianId: $("#ddHolidayPhysician").val()
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "GetPhyPreviousVacations",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            BuildPhysicianVacationView(data.objtoReturn);
        },
        error: function (msg) {
        }
    });
};
var BuildPhysicianVacationView = function (data) {
    var htmlHeaderRow =
        '<table class="table" id="PhysicianVacationList"><thead><tr class="gridHead"><th scope="col">Physician Name</th><th scope="col">Physician Speciality</th><th scope="col">Physician Department</th><th scope="col">Date</th></tr></thead><tbody>';
    var pagehtml = htmlHeaderRow;
    if (data.length > 0) {
        $.each(data,
            function (i, obj) {
                var startDate = new Date(obj.start_date);
                var endDate = new Date(obj.end_date);
                var startHours = startDate.getHours().toString().length > 1
                    ? startDate.getHours().toString()
                    : "0" + startDate.getHours().toString();
                var startMins = startDate.getMinutes().toString().length > 1
                    ? startDate.getMinutes().toString()
                    : "0" + startDate.getMinutes().toString();
                var endHours = endDate.getHours().toString().length > 1
                    ? endDate.getHours().toString()
                    : "0" + endDate.getHours().toString();
                var endMins = endDate.getMinutes().toString().length > 1
                    ? endDate.getMinutes().toString()
                    : "0" + endDate.getMinutes().toString();
                var timeslot = startHours + ":" + startMins + " - " + endHours + ":" + endMins;
                pagehtml += '<tr class="gridRow"> <td class="col3"><span>' +
                    obj.PhysicianName +
                    '</span></td><td class="col4">' +
                    obj.PhysicianSpecialityStr +
                    '</td><td class="col5"> <span>' +
                    obj.DepartmentName +
                    '</span></td><td class="col7">' +
                    startDate.toLocaleDateString() +
                    "</td></tr>";;
            });
        $("#divContent_1").attr("style", "height:270px;");
        $("#divShowPhysicianVacationList").show();
        $("#PhysicianVacationListTypes").empty().html(pagehtml);
    } else {
        $("#divContent_1").attr("style", "height:180px;");
        $("#PhysicianVacationListTypes").empty().html("<h2>No Record found</h2>");
    }
};


function BindLocationDataInScheduler(fId, physiciansRequired, depsRequired) {
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "BindLocationDataInScheduler",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ facilityId: fId, physiciansRequired: physiciansRequired, dRequired: depsRequired }),
        success: function (data) {
            if (data != null) {
                if (physiciansRequired)
                    BindList("#divPhysicianList", data.phyView);
                if (depsRequired)
                    BindList("#divFacilityDepartmentList", data.fDepView);
            }
        },
        error: function (msg) {
        }
    });
}

//##################################---Changes done to remove kendo and add the HTML trees---###################################################


//xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-------Below Code / Methods are no-longer in use----xxxxxxxxxxxxxxxxxxxxxxxxxxx
function onCheckRooms() {
    var checkedDepartments = [],
        treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    var checkedPhysicianNodes = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    var checkedRoom = [],
        treeViewRoom = $("#treeviewFacilityRooms").data("kendoTreeView");

    //checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysicianNodes);
    //checkedNodeIds1(treeViewDepartments.dataSource.view(), checkedDepartments);
    //checkedNodeIds1(treeViewStatus.dataSource.view(), checkedStatusNodes);
    //checkedNodeIds1(treeViewRoom.dataSource.view(), checkedRoom);

    GetCheckedCheckBoxesTreeView("treeviewPhysician", checkedPhysicianNodes);
    GetCheckedCheckBoxesTreeView("treeviewFacilityDepartment", checkedDepartments);
    GetCheckedCheckBoxesTreeView("treeviewFacilityRooms", checkedRoom);
    GetCheckedCheckBoxesTreeView("treeviewStatusCheck", checkedStatusNodes);
    BindSchedularWithFilters(checkedPhysicianNodes, checkedStatusNodes, checkedDepartments, checkedRoom);
}


function SchedulerInit1(jsonData, physicianData, type) {


    var sections = [
        { key: 1, label: "Section A" },
        { key: 2, label: "Section B" },
        { key: 3, label: "Section C" },
        { key: 4, label: "Section D" },
        { key: 5, label: "Section E" }
    ];


    scheduler.locale.labels.week_unit_tab = "Week units";
    scheduler.locale.labels.single_unit_tab = "Units";
    scheduler.locale.labels.section_custom = "Section";
    scheduler.config.details_on_create = true;
    scheduler.config.details_on_dblclick = true;
    scheduler.config.xml_date = "%Y-%m-%d %H:%i";

    scheduler.config.first_hour = 8;
    scheduler.config.last_hour = 19;

    scheduler.config.lightbox.sections = [
        { name: "description", height: 130, map_to: "text", type: "textarea", focus: true },
        { name: "custom", height: 23, type: "select", options: sections, map_to: "section_id" },
        { name: "time", height: 72, type: "time", map_to: "auto" }
    ];

    scheduler.createUnitsView({
        name: "week_unit",
        property: "section_id",
        list: sections,
        days: 7,
        size: 4,
        step: 1
    });
    scheduler.date.week_unit_start = scheduler.date.week_start;


    scheduler.createUnitsView({
        name: "single_unit",
        property: "section_id",
        list: sections
    });

    scheduler.addMarkedTimespan({
        type: "dhx_time_block",
        start_date: new Date($('#divDatetimePicker').val()),
        end_date: new Date($('#divDatetimePicker').val()),
        sections: { 'single_unit': [3, 4] }
    });


    scheduler.addMarkedTimespan({
        type: "dhx_time_block",
        zones: "fullday",
        days: 0,
        sections: { 'single_unit': [5] }
    });

    scheduler.addMarkedTimespan({
        type: "dhx_time_block",
        zones: "fullday",
        days: 6
    });

    scheduler.init('scheduler_here', new Date($('#divDatetimePicker').val()), "week_unit");
}

var BindDepartmentRooms = function (deptIds) {
    var jsonData = [];
    var deptData = [];
    //------------Check for the Selected Room Ids
    if (deptIds.length > 0) {
        for (var l = 0; l < deptIds.length; l++) {
            deptData[l] = {
                Id: deptIds[l]
            };
        }
    } else {
        deptData[0] = {
            Id: 0
        };
    }

    jsonData[0] = {
        Facility: $("#hidFacilitySelected").val(),
        DeptData: deptData,
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetDepartmentRooms",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify(jsonData),
        success: function (data) {
            $("#divFacilityRoomList").empty();
            $("#divFacilityRoomList").html(data);
        },
        error: function (msg) {
        }
    });
};

function BindFacilityDepartmentspopup(facilityDDId) {
    $("#ddSpeciality").val("0");
    $.ajax({
        type: "POST",
        url: "/Home/GetDepartmentsByFacility",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            facilityId: $(facilityDDId).val()
        }),
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                //$("#ddlDepartment").html(items);
                $.each(data.deptList,
                    function (i, deaprtments) {
                        items += "<option value='" + deaprtments.Value + "'>" + deaprtments.Text + "</option>";
                    });
                //$("#ddlDepartment").html(items);
                $("#selOVDepartment").html("");
                $("#selOVDepartment").html(items); //over view popup
                items = '<option value="0">--Select--</option>';
                $.each(data.phyList,
                    function (i, physician) {

                        items += '<option value="' +
                            physician.Physician.Id +
                            '" facilityId="' +
                            physician.Physician.FacilityId +
                            '" department="' +
                            physician.UserDepartmentStr +
                            '" departmentId="' +
                            physician.Physician.FacultyDepartment +
                            '" speciality="' +
                            physician.UserSpecialityStr +
                            '" specialityId="' +
                            physician.Physician.FacultySpeciality +
                            '">' +
                            physician.Physician.PhysicianName +
                            "(" +
                            physician.UserTypeStr +
                            ")</option>"; // Availability -  Field
                        //items += "<option value='" + physician.Id + "' PhysicianLicenseNumber='" + physician.PhysicianLicenseNumber + "'>" + physician.PhysicianName + "</option>";
                    });

                if (facilityDDId != "#selOVFacility") {
                    $("#ddPhysician").html(items);
                } else {
                    $("#selOVPhysician").html("");
                    $("#selOVPhysician").html(items); //over view popup
                }
                BindAppointmentTypes(facilityDDId);
                BindPatient("#selOVPatient");
            }
        },
        error: function (msg) {
        }
    });
}


function GetDataOnPhysicianSelection(facilityId, physicianId, departmentId, specialityId) {
    $.ajax({
        type: "POST",
        url: schedulerUrl + "GetAllDataOnPhysicianSelection",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        data: JSON.stringify({
            category: "4903",
            facilityId: facilityId,
            deptId: departmentId
        }),
        success: function (data) {
            if (data != null) {
                /*Bind Availability dropdown - start*/
                BindDropdownData(data.hStatus, "#ddAvailability", "");
                /*Bind Availability dropdown - end*/



                /*Bind Physician dropdown - start*/
                var items = '<option value="0">--Select--</option>';
                $.each(data.pList,
                    function (i, obj) {
                        items += '<option value="' +
                            obj.Physician.Id +
                            '" facilityId="' +
                            obj.Physician.FacilityId +
                            '" department="' +
                            obj.UserDepartmentStr +
                            '" departmentId="' +
                            obj.Physician.FacultyDepartment +
                            '" speciality="' +
                            obj.UserSpecialityStr +
                            '" specialityId="' +
                            obj.Physician.FacultySpeciality +
                            '">' +
                            obj.Physician.PhysicianName +
                            "(" +
                            obj.UserTypeStr +
                            ")</option>"; // Availability -  Field
                    });

                BindList("#selOVPhysician", items);
                BindList("#ddPhysician", items);
                BindList("#ddHolidayPhysician", items); /*Bind holiday's physician drop down*/
                /*Bind Physician dropdown - end*/

                /*Bind holiday status dropdown - start*/
                BindDropdownData(data.hStatus, "#ddHolidayStatus", "");
                /*Bind holiday status dropdown - end*/

                /*Bind holiday types dropdown - start*/
                BindDropdownData(data.hTypes, "#ddHolidayType", "");
                /*Bind holiday types dropdown - end*/


                /*Bind Departments*/
                BindDropdownData(data.depList, "#selOVDepartment", ""); //over view popup

                /*Bind Appointment Types dropdown - start*/
                items = '<option value="0">--Select--</option>';
                $.each(data.appTypes,
                    function (i, appt) {
                        items += "<option value='" +
                            appt.Id +
                            "' timeslot='" +
                            appt.TimeSlot +
                            "'>" +
                            appt.Name +
                            "</option>";
                    });
                $("#appointmentTypes").html("");
                $("#appointmentTypes").html(items);
                /*Bind Appointment Types dropdown - end*/


                /*Bind Patients dropdown - start*/
                $.each(data.patients,
                    function (i, p) {
                        items += "<option value='" +
                            p.PatientID +
                            "'>" +
                            p.PersonFirstName +
                            " " +
                            p.PersonLastName +
                            "</option>";
                    });
                $(selector).html("");
                $(selector).html(items);
                /*Bind Patients dropdown - end*/

                $("#ddFacility").val(facilityId);
                $("#ddPhysician").val(physicianId);
                $("#ddlDepartment").val(departmentId);
                $("#ddSpeciality").val(specialityId);
                $(".searchSlide").removeClass("moveLeft");
            }
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

function SelectPhysician(facilityId, physicianId, departmentId, specialityId) {
    GetDataOnPhysicianSelection(facilityId, physicianId, departmentId, specialityId);
    //$("#ddFacility").val(facilityId);
    //BindAppointmentAvailability();
    //BindFacilityDepartmentspopup("#ddFacility");
    //$("#ddPhysician").val(physicianId);
    //$("#ddlDepartment").val(departmentId);
    //$("#ddSpeciality").val(specialityId);
    //$(".searchSlide").removeClass("moveLeft");
    ////------- Code added By Shashank to restrict user to View only selected Physician Appointment types
    //BindPhysiciansApptTypes(facilityId, departmentId);
}




var BindLocationsInSchedular = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetFaciltyListTreeView",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#divLocationList").empty();
            $("#divLocationList").html(data);
            BindFacilityDepartments();
            BindFacilityRooms();
        },
        error: function (msg) {
        }
    });
};

var BindSchedulingStatus = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetGlobalCodesCheckListView",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({
            ggcValue: "4903"
        }),
        success: function (data) {
            $("#divStatusList").empty();
            $("#divStatusList").html(data);
        },
        error: function (msg) {
        }
    });
};

/// <var>The bind corporate physician</var>
var BindCorporatePhysician = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetCorporatePhysicians",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({
            corporateId: "",
            facilityId: $("#hidFacilitySelected").val()
        }),
        success: function (data) {
            $("#divPhysicianList").empty();
            $("#divPhysicianList").html(data);
        },
        error: function (msg) {
        }
    });
};


var BindFacilityDepartments = function () {
    var jsonData = {
        facilityid: $("#hidFacilitySelected").val(),
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/LoadFacilityDepartmentData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify(jsonData),
        success: function (data) {
            $("#divFacilityDepartmentList").empty();
            $("#divFacilityDepartmentList").html(data);
        },
        error: function (msg) {
        }
    });
};
var BindFacilityRooms = function () {
    var jsonData = {
        facilityid: $("#hidFacilitySelected").val(),
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/LoadFacilityRoomsDataCustom",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify(jsonData),
        success: function (data) {
            $("#divFacilityRoomList").empty();
            $("#divFacilityRoomList").html(data);
        },
        error: function (msg) {
        }
    });
};
var CheckFacilityDepartments = function (id) {
    $("#treeviewFacilityDepartment li").removeClass("active");
    var previousCheck = $("#chkDept_" + id).prop("checked");
    $("#chkDept_" + id).prop("checked", !previousCheck);
    $("#liDeptCheckBox_" + id).addClass("active");
    onCheckDepartment();
};
var CheckFacilityRooms = function (id) {
    $("#treeviewFacilityRooms li").removeClass("active");
    var previousCheck = $("#chkRoom_" + id).prop("checked");
    $("#chkRoom_" + id).prop("checked", !previousCheck);
    $("#liRoomCheckBox_" + id).addClass("active");
    onCheckDepartment();
    //onCheckRooms();
};

/// <var>The get schedular data</var>
var GetSchedularData = function () {
    setTimeout(LoadSchedulngData(), 500);
};
var LoadSchedulngData = function () {
    var jsonData = {
        selectedDate: $("#divDatetimePicker").val(),
        facility: $("#hidFacilitySelected").val(),
        viewType: "1"
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "LoadSchedulngData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            //SchedulerInit1(data.savedSlots, null, '1');
            IntializeScheduler(data.savedSlots, data.selectedPhysicians, "1");
            //AddDepartmentTimming(data.departmentTimmingsList);
        },
        error: function (msg) {
        }
    });
};


function BindFacility(selector) {
    $.ajax({
        type: "POST",
        url: "/Home/GetFacilitiesDropdownDataOnScheduler",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data,
                    function (i, facility) {
                        items += "<option value='" + facility.Value + "'>" + facility.Text + "</option>";
                    });
                $(selector).html("");
                $(selector).html(items);
            } else {
            }
        },
        error: function (msg) {
        }
    });
}

var BuildPhysicianVacationView = function (data) {
    var htmlHeaderRow =
        '<table class="table" id="PhysicianVacationList"><thead><tr class="gridHead"><th scope="col">Physician Name</th><th scope="col">Physician Speciality</th><th scope="col">Physician Department</th><th scope="col">Date</th></tr></thead><tbody>';
    var pagehtml = htmlHeaderRow;
    if (data.length > 0) {
        $.each(data,
            function (i, obj) {
                var startDate = new Date(obj.start_date);
                var endDate = new Date(obj.end_date);
                var startHours = startDate.getHours().toString().length > 1
                    ? startDate.getHours().toString()
                    : "0" + startDate.getHours().toString();
                var startMins = startDate.getMinutes().toString().length > 1
                    ? startDate.getMinutes().toString()
                    : "0" + startDate.getMinutes().toString();
                var endHours = endDate.getHours().toString().length > 1
                    ? endDate.getHours().toString()
                    : "0" + endDate.getHours().toString();
                var endMins = endDate.getMinutes().toString().length > 1
                    ? endDate.getMinutes().toString()
                    : "0" + endDate.getMinutes().toString();
                var timeslot = startHours + ":" + startMins + " - " + endHours + ":" + endMins;
                pagehtml += '<tr class="gridRow"> <td class="col3"><span>' +
                    obj.PhysicianName +
                    '</span></td><td class="col4">' +
                    obj.PhysicianSpecialityStr +
                    '</td><td class="col5"> <span>' +
                    obj.DepartmentName +
                    '</span></td><td class="col7">' +
                    startDate.toLocaleDateString() +
                    "</td></tr>";;
            });
        $("#divContent_1").attr("style", "height:270px;");
        $("#divShowPhysicianVacationList").show();
        $("#PhysicianVacationListTypes").empty().html(pagehtml);
    } else {
        $("#divContent_1").attr("style", "height:180px;");
        $("#PhysicianVacationListTypes").empty().html("<h2>No Record found</h2>");
    }
};

var OnTimeSlotSelectionOverView = function (e) {
    var deptt = $(e).find("option:selected").attr("departmentId");
    var spec = $(e).find("option:selected").attr("specialityId");
    var fid = $(e).find("option:selected").attr("facilityId");
    $("#ddSpeciality").val(spec);
    $("#ddFacility").val(fid);

    BindFacilityDepartments();
    $("#ddlDepartment").val(deptt);

    //------- Code added By Shashank to restrict user to View only selected Physician Appointment types
    BindPhysiciansApptTypes(fid, deptt);
};
var OverviewDeptSelection = function (id) {
    var jsonData = {
        facilityid: $("#hidFacilitySelected").val(),
        deptId: id
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/BindDepartmentAppointment",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify(jsonData),
        success: function (data) {
            $("#divFacilityDepartmentList").empty();
            $("#divFacilityDepartmentList").html(data);
        },
        error: function (msg) {
        }
    });
};




function BindSchedularDataByDeparment(physicianIds, statusfilter, deptIds, roomIds) {
    //blockOnlyDiv("Loading...");
    var jsonData = [];
    var physicianData = [];
    var statusData = [];
    var deptData = [];
    var roomsData = [];
    var showAllRooms = false;

    //------------Check for the Selected Physciain Ids
    if (physicianIds.length > 0) {
        for (var i = 0; i < physicianIds.length; i++) {
            physicianData[i] = {
                Id: physicianIds[i]
            };
        }
    } else {
        physicianData[0] = {
            Id: 0
        };
    }
    //-------------- Selected Department Id check

    var depList = "";
    if (deptIds.length > 0) {
        for (var k = 0; k < deptIds.length; k++) {
            deptData[k] = { Id: deptIds[k] };

            if (depList == "")
                depList = deptIds[k];
            else
                depList += "," + deptIds[k];
        }
    } else {
        //deptData[0] = {
        //    Id: 0
        //};
    }
    //------------Check for the Selected Status
    if (statusfilter.length > 0) {
        for (var j = 0; j < statusfilter.length; j++) {
            statusData[j] = {
                Id: statusfilter[j]
            };
        }
    } else {
        //statusData[0] = {
        //    Id: 0
        //};
    }

    //------------Check for the Selected Room Ids
    if (roomIds.length > 0) {
        for (var l = 0; l < roomIds.length; l++) {
            roomsData[l] = {
                Id: roomIds[l]
            };
        }
        showAllRooms = false;
    } else {
        showAllRooms = true;
    }

    jsonData[0] = {
        PhysicianId: physicianData,
        StatusType: statusData,
        SelectedDate: $("#divDatetimePicker").val(),
        Facility: $("#hidFacilitySelected").val(),
        ViewType: $("#hidSelectedView").val(),
        DeptData: deptData,
        PatientId: $("#hidPatientId").val(),
        RoomIds: roomsData,
        ShowAllRooms: showAllRooms
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "GetFilteredSchedulerData",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            $("#hidSelectedViewName").val("Day");
            var newData = data.SchData;

            /*
            In to room view, hide the break time.
            */
            if (data.SchData != null && data.SchData.length > 0 && ((data.rList != null && data.rList.length > 0) || (deptData.length > 0))) {
                newData = $.grep(data.SchData, function (item) {
                    return item.SchedulingType != '2' && item.SchedulingType != '3';
                });
            }
            IntializeScheduler(newData, data.rList, "3");
        },
        error: function (msg) {
        }
    });
};












//##################################---Changes done to remove kendo and add the HTML trees---###################################################m
var BindHolidayView = function () {
    var facId = $("#hidFacilitySelected").val();
    //$("#hidSelectedViewName").val("Year");
    var jsonData = {
        facilityid: facId
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "GetFacilityHolidays",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            BuildFacilityHolidayView(data.objtoReturn);
        },
        error: function (msg) {
        }
    });
}

var BuildFacilityHolidayView = function (data) {
    var htmlHeaderRow =
        '<table class="table" id="FacilityHolidayList"><thead><tr class="gridHead"><th scope="col">Description</th><th scope="col">Date</th><th scope="col">Actions</th></tr></thead><tbody>';
    var pagehtml = htmlHeaderRow;
    if (data.length > 0) {
        $.each(data,
            function (i, obj) {
                var startDate = new Date(obj.start_date);
                var endDate = new Date(obj.end_date);
                var startHours = startDate.getHours().toString().length > 1
                    ? startDate.getHours().toString()
                    : "0" + startDate.getHours().toString();
                var startMins = startDate.getMinutes().toString().length > 1
                    ? startDate.getMinutes().toString()
                    : "0" + startDate.getMinutes().toString();
                var endHours = endDate.getHours().toString().length > 1
                    ? endDate.getHours().toString()
                    : "0" + endDate.getHours().toString();
                var endMins = endDate.getMinutes().toString().length > 1
                    ? endDate.getMinutes().toString()
                    : "0" + endDate.getMinutes().toString();
                var timeslot = startHours + ":" + startMins + " - " + endHours + ":" + endMins;
                pagehtml += '<tr class="gridRow" id="trHoliday_' + obj.TimeSlotId + '"> <td class="col3"><span>' +
                    obj.text +
                    '</span></td>' +
                    '<td class="col7">' +
                    startDate.toLocaleDateString() +
                    "</td><td>" +
                    //'<a title="Delete Holiday" style="width: 15px; margin-right: 7px; float: left;" onclick="return DeleteHoliday(' + obj.TimeSlotId + ',' + obj.EventParentId + ',this); " href="javascript:void(0);">' +
                    "<a title=\"Delete Holiday\" style=\"width: 15px; margin-right: 7px; float: left;\" onclick=\"return OpenConfirmPopupWithTwoId('" + obj.TimeSlotId + "', '" + obj.EventParentId + "','Delete Holiday','Do you really want to delete holiday?', DeleteHoliday, null);\" href=\"javascript:void(0);\">" +





                    '<img src="/images/delete.png"></a>' + "</td></tr>";
            });
        $("#divContent_1").attr("style", "height:270px;");
        $("#divShowPhysicianVacationList").show();
        $("#PhysicianVacationListTypes").empty().html(pagehtml);
    } else {
        $("#divContent_1").attr("style", "height:180px;");
        $("#PhysicianVacationListTypes").empty().html("<h2>No Record found</h2>");
    }
};

//var DeleteHoliday = function (timeSlotId, id, obj) {
//    var facId = $("#hidFacilitySelected").val();
//    var tsId = $("#hfGlobalConfirmFirstId").val();
//    var jsonData = {
//        id: id,
//        facilityid: facId
//    };
//    $.ajax({
//        cache: false,
//        type: "POST",
//        url: schedulerUrl + "DeleteHoliday",
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        data: JSON.stringify(jsonData),
//        success: function (data) {
//            if (data) {
//                $(this).parent('tr').remove();
//                $("#trHoliday_" + timeSlotId).remove();
//                ShowMessage('Holiday deleted successfully', "Success", "success", true);
//            }
//        },
//        error: function (msg) {
//        }
//    });
//}

var DeleteHoliday = function () {
    var facId = $("#hidFacilitySelected").val();
    var tsId = $("#hfGlobalConfirmFirstId").val();
    var sid = $("#hfGlobalConfirmedSecondId").val();
    var objData = $("#hfGlobalConfirmedThridId").val();
    var jsonData = {
        id: sid,
        facilityid: facId
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: schedulerUrl + "DeleteHoliday",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            if (data) {
                $(this).parent('tr').remove();
                $("#trHoliday_" + tsId).remove();
                ShowMessage('Holiday deleted successfully', "Success", "success", true);
            }
        },
        error: function (msg) {
        }
    });
}


function OpenConfirmPopupWithTwoId(confirmedFirstId, confirmedSecondId, title, msg, confirmEvent, cancelEvent) {
    $("#hfGlobalConfirmFirstId").val(confirmedFirstId);
    $("#hfGlobalConfirmedSecondId").val(confirmedSecondId);

    $("#h5Globaltitle").html(title);
    if (msg != '')
        $("#h5GlobalMessage").html(msg);

    if (cancelEvent == null)
        cancelEvent = CancelEvent;

    $.blockUI({ message: $('#divConfirmBox'), css: { width: '350px' } });

    //Remove Button Clicks
    $('#divConfirmBox').off('click', '#btnGlobalConfirm', confirmEvent);
    $('#divConfirmBox').off('click', '#btnGlobalCancel', cancelEvent);

    //Add Button Clicks
    $('#divConfirmBox').on('click', '#btnGlobalConfirm', confirmEvent);
    $('#divConfirmBox').on('click', '#btnGlobalCancel', cancelEvent);
}


function CancelEvent() {
    $('#hfGlobalConfirmId').val('');
    $("#hfGlobalConfirmFirstId").val('');
    $("#hfGlobalConfirmedSecondId").val('');
    $("#hfGlobalConfirmedThridId").val('');
    $.unblockUI();
    return false;
}


function BindFiltersOnly() {
    $("#spnSelectedPatient").html("");
    apptRecArray = []; //Clear reccurrence array
    var checkedPhysician = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    var checkedDepartments = [],
        treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");

    var checkedRoom = [],
        treeViewRoom = $("#treeviewFacilityRooms").data("kendoTreeView");

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");



    GetCheckedCheckBoxesTreeView("treeviewStatusCheck", checkedStatusNodes);
    GetCheckedCheckBoxesTreeView("treeviewPhysician", checkedPhysician);
    GetCheckedCheckBoxesTreeView("treeviewFacilityDepartment", checkedDepartments);
    GetCheckedCheckBoxesTreeView("treeviewFacilityRooms", checkedRoom);


    var jsonData = [];
    var physicianData = [];
    var statusData = [];
    var deptData = [];
    var roomsData = [];

    //------------Check for the Selected Physciain Ids
    if (checkedPhysician.length > 0) {
        for (var i = 0; i < checkedPhysician.length; i++) {
            physicianData[i] = {
                Id: checkedPhysician[i]
            };
        }
    } else {
        physicianData[0] = {
            Id: 0
        };
    }
    //-------------- Selected Department Id check
    if (checkedDepartments.length > 0) {
        for (var k = 0; k < checkedDepartments.length; k++) {
            deptData[k] = {
                Id: checkedDepartments[k]
            };
        }
    } else {
        deptData[0] = {
            Id: 0
        };
    }
    //------------Check for the Selected Status
    if (checkedStatusNodes.length > 0) {
        for (var j = 0; j < checkedStatusNodes.length; j++) {
            statusData[j] = { Id: checkedStatusNodes[j] };
        }
    } else {
        statusData[0] = {
            Id: 0
        };
    }

    //------------Check for the Selected Room Ids
    if (checkedRoom.length > 0) {
        for (var l = 0; l < checkedRoom.length; l++) {
            roomsData[l] = {
                Id: checkedRoom[l]
            };
        }
    } else {
        roomsData[0] = {
            Id: 0
        };
    }

    jsonData[0] = {
        PhysicianId: physicianData,
        StatusType: statusData,
        SelectedDate: $("#divDatetimePicker").val(),
        Facility: $("#hidFacilitySelected").val(),
        ViewType: $("#hidSelectedView").val(),
        DeptData: deptData,
        PatientId: $("#hidPatientId").val(),
        RoomIds: roomsData,
    };

    return jsonData;
}