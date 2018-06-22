var firstTimeLoad = false;
var firstTimeSave = true;
var firstTimeBind = true;
var apptTypeIdArray = [];
var apptRecArray = [];
var schedularEventObj;

$(function() {
    /// <summary>
    ///     s this instance.
    /// </summary>
    /// <returns></returns>
    $("#divDatetimePicker").datepicker({
        //showWeek: true,
        firstDay: 0,
        numberOfMonths: [2, 1],
        //showButtonPanel: true,
        onSelect: function(dateText, inst) {
            onCheckFilters();
        }
    });
    /*var _gotoToday = jQuery.datepicker._gotoToday;
    jQuery.datepicker._gotoToday = function (a) {
        var target = jQuery(a);
        var inst = this._getInst(target[0]);
        _gotoToday.call(this, a);
        jQuery.datepicker._selectDate(a, jQuery.datepicker._formatDate(inst, inst.selectedDay, inst.selectedMonth, inst.selectedYear));
    };*/
    BindAppointmentAvailability();
    BindLocations();
    BindCorporatePhysician();
    BindSchedulingStatus();
    BindFacility("#ddFacility");
    BindGlobalCodesWithValue("#ddSpeciality", 1121, "");
    BindGlobalCodesWithValueOrderBy("#ddMonthWeekDays", 901, "");

    //setTimeout(SchedulerInit(null, null), 500);
    $('#btnScheduleAppointment').on('click', function() {
        $('#hidSchedulingType').val('1');
        $(".modal-title").html("Appointments Scheduler");
        ShowLightBoxStyle("1"); //1 means schedule appointment
        var checkedBoxes = GetCheckedCheckBoxes('treeviewPhysician');
        //if (checkedBoxes.length > 0) {
        scheduler.addEventNow();
        //} else {
        // ShowMessage('Select Any Physician first!', "Error", "error", true);
        //}
    });

    $('#btnAddHoliday').on('click', function() {
        $('#hidSchedulingType').val('2');
        $(".modal-title").html("Holiday/Vacations Scheduler");
        ShowLightBoxStyle("2"); //2 means Holiday
        var checkedBoxes = GetCheckedCheckBoxes('treeviewPhysician');
        //if (checkedBoxes.length > 0) {
        scheduler.addEventNow();
        //} else {
        //  ShowMessage('Select Any Physician first!', "Error", "error", true);
        //}
    });

    $('#btnAddVacation').on('click', function() {
        $('#hidSchedulingType').val('3');
        var checkedBoxes = GetCheckedCheckBoxes('treeviewPhysician');
        if (checkedBoxes.length > 0) {
            scheduler.addEventNow();
        } else {
            ShowMessage('Select Any Physician first!', "Error", "error", true);
        }
    });

    $('#chkViewAvialableSlots').on('change', function(e) {
        //        
        if ($('#chkViewAvialableSlots').prop('checked')) {
            $('#chkViewVocationHolidays').prop('checked', false);
            GetSchedularCustomData('1');
        }
    });

    $('#chkViewVocationHolidays').on('change', function(e) {
        //        
        if ($('#chkViewVocationHolidays').prop('checked')) {
            $('#chkViewAvialableSlots').prop('checked', false);
            GetSchedularCustomData('3');
        }
    });

    $('input[name=rdbtnSelection]').on('change', function() {
        //var selected = $('input[name=rdbtnSelection]:checked').val();
        //if (selected == "2") {
        //    $('#divTypeofProc').hide();
        //} else { $('#divTypeofProc').show(); }
    });

    $('#btnSaveSchedulingData').on('click', function() {
        
        var schedulingType = $('#hidSchedulingType').val();
        if (schedulingType === "1") {
            var isdivSchedulerDataValid = jQuery("#divSchedulerData").validationEngine({ returnIsValid: true });
            var isdivAppointmentTypeProceduresValid = jQuery("#divAppointmentTypeProcedures").validationEngine({ returnIsValid: true });
            if (isdivSchedulerDataValid && isdivAppointmentTypeProceduresValid) {
                blockRecurrenceDiv("Saving...");
                SaveCustomSchedular();
                //setTimeout(SaveCustomSchedular(), 5000);
            }
        } else {
            CheckTwoDates($('#eventFromDate'), $('#eventToDate'), 'eventToDate');
            var isdivHolidayValid = jQuery("#divHoliday").validationEngine({ returnIsValid: true });
            //if (isdivHolidayValid && $('#eventToDate').val() != '') {
            if (isdivHolidayValid) {
                blockRecurrenceDiv("Saving...");
                SaveHolidaySchedular();
                scheduler.cancel_lightbox();
                //$('.hidePopUp').hide();
            }
        }
        firstTimeBind = true;
    });
    $('#btnCancelSchedulingData').on('click', function() {
        blockRecurrenceDiv("Cancelling...");
        //$("#loader_event").show();
        $('#hfAppointmentTypes').val('');
        scheduler.cancel_lightbox();
        //$('.hidePopUp').hide();
        $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
        $.validationEngine.closePrompt('.formError', true);
        firstTimeLoad = true;
        firstTimeBind = true;
        onCheckFilters();
        ClearSchedulingPopup();
        //scheduler.endLightbox(false, html("my_form"));
    });
    $('#btnDeleteSchedulingData').on('click', function() {
        //$("#loader_event").show();
        blockRecurrenceDiv("Deleting...");
        firstTimeBind = true;
        var eventParentId = $("#hidEventParentId").val();
        var schedulingId = $("#hfSchedulingId").val();
        var schType = $('#hidSchedulingType').val();
        var externalValue3 = $("#hfExternalValue3").val();
        DeleteSchduling(eventParentId, schedulingId, schType, externalValue3);
    });

    GetSchedularData();

    $("#eventFromDate").datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });
    $("#eventFromTime").kendoTimePicker({
        interval: 15,
        format: "HH:mm"
    });
    $('.main_content').scroll(function() {
        $.validationEngine.closePrompt(".formError", true);
    });

    $("#eventToDate").datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });
    $("#eventToTime").kendoTimePicker({
        interval: 15,
        format: "HH:mm"
    });
    $(".EmiratesMask").mask("999-99-9999");
    $("#txtPersonDOB").datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12', //yesterday is minimum date(for today use 0 or -1970/01/01)
        maxDate: new Date(),
        timepicker: false,
        closeOnDateSelect: true
    });
    $("#txtRecEndByDate").datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });
    //$("#txtRecEndByDate").val(new Date().format('mm/dd/yyyy'));
    $('.searchHitMe, #spnPrevList').on('click', function(e) { //function to toggle previous visit list in & out
        //$('.searchSlide').toggleClass('moveLeft');
    });
    $('#spnAvailTimeSlots').on('click', function(e) { //function to toggle previous visit list in & out
        $('#divAvailableTimeSlots').removeClass('moveLeft2');
    });
    $('#btnSearchPatient').on('click', function() {
        PatientSearchPopupOpen();
        $('#divSearchPatient').show();
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
    BindCountryDataWithCountryCode("#ddlPatientCountries", "#hfPatientCountry", "#lblPatientCountryCode");
    $('#txtRecEveryDay, #txtRepeatMonthDay, #txtRecEveryWeekDays').keypress(function(event) {
        return isNumber(event, this);
    });
    $("#divPrevListSlider, #divAvailableTimeSlots").click(function(e) {
        return false;
    });
    $('body').click(function(evt) {
        if (evt.target.id == "divPreVisitList") {
            if ($('.searchSlide').hasClass('moveLeft')) {
                $('.searchSlide').removeClass('moveLeft');
            } else {
                $('.searchSlide').addClass('moveLeft');
                $('.searchSlide1').removeClass('moveLeft2');
                $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
            }
            return false;
        } else if (evt.target.id == "btnShowTimeSlots") {
            if ($('.searchSlide1').hasClass('moveLeft2')) {
                $('.searchSlide1').removeClass('moveLeft2');
            } else {
                $('.searchSlide1').addClass('moveLeft2');
                $('.searchSlide').removeClass('moveLeft');
                $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
            }
            return false;
        } else if (evt.target.id == "divReccurrencePopup") {
            if ($("#divReccurrencePopup .popup_frame").hasClass('moveLeft')) {
                $("#divReccurrencePopup .popup_frame").removeClass('moveLeft');
            } else {
                $('.searchSlide1').removeClass('moveLeft2');
                $('.searchSlide').removeClass('moveLeft');
                $("#divReccurrencePopup .popup_frame").addClass("moveLeft");
            }
            return false;
        } else {
            $('.searchSlide').removeClass('moveLeft');
            $('.searchSlide1').removeClass('moveLeft2');
            //$("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
            //return false;
        }
        //For descendants of menu_content being clicked, remove this check if you do not want to put constraint on descendants.
        /*if ($(evt.target).closest('#menu_content').length)
            return;*/
        //Do processing of click event here for every element except with id menu_content

    });
});

var clearSelectedPatient = function() {
    $('#spnSelectedPatient').html('');
    $('#btnClearPatient').hide();
    onCheckFilters();
};

var PatientSearchPopupOpen = function() {
    $(".EmiratesMask").mask("999-99-9999");
    var ButtonKeys = { "EnterKey": 13 };
    $(".white-bg").keypress(function(e) {
        if (e.which == ButtonKeys.EnterKey) {
            $("#btnSearch").click();
        }
    });
    $('#collapseSerachResult').removeClass('in');
    $('#collapsePatientSearch').addClass('in');
    BindCountryDataWithCountryCode("#ddlCountries", "#hdCountry", "#lblCountryCode");
};

/*Method is used to the set the apointments to time by changing appointments from time*/
var startChange = function(timeFromId, timeToId, timeInterval) {
    
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
    var startTime = tpFromVal; //start.value();

    if (startTime) {
        startTime = new Date(startTime);
        //tpTo.max(startTime);
        startTime.setMinutes(startTime.getMinutes() + timeInterval);
        tpTo.min(startTime);
        tpTo.value(startTime);
    }
}; /// <var>The get checked check boxes</var>

var GetCheckedCheckBoxes = function(divid) {
    var selected = [];
    $('#' + divid + ' input:checked').each(function() {
        selected.push($(this).attr('name'));
    });
    return selected;
}; /// <var>The get schedular data</var>

var GetSchedularData = function() {
    setTimeout(LoadSchedulngData(), 500);
}; /// <var>The bind corporate physician</var>

var BindCorporatePhysician = function() {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetCorporatePhysicians",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({
            corporateId: '',
            facilityId: $('#hidFacilitySelected').val()
        }),
        success: function(data) {
            $('#divPhysicianList').empty();
            $('#divPhysicianList').html(data);
        },
        error: function(msg) {
        }
    });
}; //GetCorporatePhysicians}

/// <var>The bind scheduling status</var>
var BindSchedulingStatus = function() {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetGlobalCodesCheckListView",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({
            ggcValue: '4903'
        }),
        success: function(data) {
            $('#divStatusList').empty();
            $('#divStatusList').html(data);
        },
        error: function(msg) {
        }
    });
};

/// <var>The bind locations</var>
var BindLocations = function() {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetFaciltyListTreeView",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function(data) {
            $('#divLocationList').empty();
            $('#divLocationList').html(data);
            BindFacilityDepartments();
            BindFacilityRooms();
        },
        error: function(msg) {
        }
    });
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
    $('#spnSelectedPatient').html('');
    apptRecArray = []; //Clear reccurrence array
    var checkedPhysician = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    var checkedDepartments = [],
        treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");

    var checkedRoom = [],
        treeViewRoom = $("#treeviewFacilityRooms").data("kendoTreeView");

    checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysician);
    checkedNodeIds1(treeViewStatus.dataSource.view(), checkedStatusNodes);
    checkedNodeIds1(treeViewDepartments.dataSource.view(), checkedDepartments);
    checkedNodeIds1(treeViewRoom.dataSource.view(), checkedRoom);

    BindSchedularWithFilters(checkedPhysician, checkedStatusNodes, checkedDepartments, checkedRoom);
}

function onCheckLocation(e) {
    $('#spnSelectedPatient').html('');
    var t = $('#treeviewLocations').data('kendoTreeView');
    var node = e.node;
    var locationname = this.text(e.node);
    var nodeValueId = $(node).closest("li").data("id");
    //var selected = t.select(),
    //        item = t.dataItem(selected);
    //if (item) {
    //    nodeValueId = item.id;
    //}
    $('#hidFacilityName').val(locationname);
    $('#hidFacilitySelected').val(nodeValueId);
    BindCorporatePhysician();
    BindFacilityDepartments();

    var checkedPhysician = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");
    checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysician);
    if (checkedPhysician.length > 0) {
        $("#btnScheduleAppointment").prop("disabled", false);
        $("#btnAddHoliday").prop("disabled", false);
        onCheckFilters();
    } else {
        $("#btnScheduleAppointment").prop("disabled", true);
        $("#btnAddHoliday").prop("disabled", true);
        scheduler.clearAll();
        scheduler.updateView(new Date($('#divDatetimePicker').val()), "week");
        ShowMessage('There is no Physician available for this facility!', "Warning", "warning", true);
    }
    //BindFacilityPhysicians(nodeValueId);
}

/// <var>The bind schedular with filters</var>
var BindSchedularWithFilters = function(physicianIds, statusfilter, deptIds, roomIds) {
    var jsonData = [];
    var physicianData = [];
    var statusData = [];
    var deptData = [];
    var roomsData = [];
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

    //------------Check for the Selected Room Ids
    if (roomIds.length > 0) {
        for (var l = 0; l < roomIds.length; l++) {
            roomsData[l] = {
                Id: roomIds[l]
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
        SelectedDate: $('#divDatetimePicker').val(),
        Facility: $('#hidFacilitySelected').val(),
        ViewType: $('#hidSelectedView').val(),
        DeptData: deptData,
        PatientId: $('#hidPatientId').val(),
        RoomIds: roomsData,
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/FacultyTimeslots/GetSchedularWithFilters",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function(data) {

            SchedulerInit(data, physicianData, '1');
        },
        error: function(msg) {
        }
    });
};

function SchedulerInit(jsonData, physicianData, type) {
    /// <summary>
    ///     Schedulers the initialize.
    /// </summary>
    /// <param name="jsonData">The json data.</param>
    /// <param name="physicianData">The physician data.</param>
    /// <returns></returns>
    scheduler.config.xml_date = "%m-%d-%Y %H:%i";
    scheduler.xy.editor_width = 0; //disable editor's auto-size

    var format = scheduler.date.date_to_str("%H:%i");
    var step = 15;

    var sections = [];
    if (type == "1") {
        if (physicianData != null && physicianData.length > 0 && physicianData[0].Id > 0) {
            for (var i = 0; i < physicianData.length; i++) {
                var physicianid = physicianData[i].Id;
                var phyName = $("#treeviewPhysician [name='checkedFiles'][value=" + physicianid + "]").parent().next().text();
                sections.push({ key: physicianid, label: phyName });
            }
        } else {
            //var physicianCount = $("#treeviewPhysician [name='checkedFiles']"); //.parent().next().text();
            //for (var j = 0; j < physicianCount.length; j++) {
            //    var phyName1 = $("#treeviewPhysician [name='checkedFiles'][value=" + physicianCount[j].value + "]").parent().next().text();
            //    sections.push({ key: physicianCount[j].value, label: phyName1 });
            //}
        }
    } else {
        if (physicianData != null && physicianData.length > 0 && physicianData[0].Id > 0) {
            for (var k = 0; k < physicianData.length; k++) {
                var deptid = physicianData[k].Id;
                var deptName = $("#treeviewFacilityDepartment [name='checkedFiles'][value=" + deptid + "]").parent().next().text();
                sections.push({ key: deptid, label: deptName });
            }
        } else {
            //var physicianCountval = $("#treeviewFacilityDepartment [name='checkedFiles']");
            //for (var l = 0; l < physicianCountval.length; l++) {
            //    var deptName1 = $("#treeviewFacilityDepartment [name='checkedFiles'][value=" + physicianCountval[l].value + "]").parent().next().text();
            //    sections.push({ key: physicianCountval[l].value, label: deptName1 });
            //}
        }
    }

    scheduler.templates.hour_scale = function(date) {
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
        { key: '0', label: "---Select---" },
        { key: '1', label: "Available/Open" },
        { key: '2', label: "Initial Booking " },
        { key: '3', label: "Confirmed" },
        { key: '4', label: "Lunch/Meetings/Adm" },
        { key: '5', label: "Holiday" },
        { key: '6', label: "Cancel" }
    ];

    scheduler.attachEvent("onViewChange", function(new_mode, new_date) {
        var minDate = scheduler.getState().min_date;

    });

    scheduler.templates.event_class = function(start, end, event) {
        var css = "";
        if (event.Availability) // if event has subject property then special class should be assigned
        {
            if (event.Availability == 1) {
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
            } else if (event.Availability == 15) {
                css = "patient_cancelled_notrefilled";
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
        { name: "parent", height: 30, type: "select", options: parent_select_options, map_to: "Availability" },
        { name: "time", height: 72, type: "time", map_to: "auto" }
    ];
    scheduler.config.icons_select = [
    ];
    scheduler.templates.tooltip_text = function(start, end, event) {
        if (event.PhysicianName != null) {
            return "<h4> " + event.text + "</h4>" +
                "<div class='row'><div class='col-sm-5'><b>Physician Name: </b></div><div class='col-sm-7'>" + event.PhysicianName + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Patient Name: </b></div><div class='col-sm-7'>" + event.PatientName + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Scheduled Date: </b></div><div class='col-sm-7'>" + start.toLocaleDateString() + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Time Slot: </b></div><div class='col-sm-7'>" + format(start) + " - " + format(end) + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Description: </b></div><div class='col-sm-7'><p class='event_description'>" + event.text + "</p></div></div>";
        } else {
            return "<h4> " + event.text + "</h4>" +
                "<div class='row'><div class='col-sm-5'><b>Scheduled Date: </b></div><div class='col-sm-7'>" + start.toLocaleDateString() + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Time Slot: </b></div><div class='col-sm-7'>" + format(start) + " - " + format(end) + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Description: </b></div><div class='col-sm-7'><p class='event_description'>" + event.text + "</p></div></div>";
        }
    };
    scheduler.locale.labels.confirm_deleting = null;
    scheduler.attachEvent("onBeforeDrag", function(id) {
        return false;
    });
    scheduler.attachEvent("onBeforeLightbox", function(ev) {
        
        var eventDetails = scheduler.getEvent(ev);
        if (eventDetails.Availability == "6") {
            return false;
        }
        return true;
    });
    //scheduler.attachEvent("onDblClick", function (id, e) {
    //    
    //    var tt = scheduler.getEvent(id);
    //    return true;
    //});
    scheduler.attachEvent("onLightbox", function(ev) {
        LightBoxCode(ev);
    });

    scheduler.attachEvent("onEventAdded", function(id, ev) {
        SaveSchedularData(id, ev);
    });

    scheduler.attachEvent("onEventChanged", function(id, ev) {
        $('#hidSchedulingType').val(ev.SchedulingType);
        var datesObj = scheduler.getRecDates(id);
        SaveSchedularData(id, ev);
    });

    scheduler.attachEvent("onBeforeEventDelete", function(id, ev) {
        $(".tooltip").hide();
        if (ev.SchedulingType == "2") {
            $("#divMultipleDeletePopup").show();
            $("#btnDeleteSeries").click(function(e) {
                DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
            });
            $("#btnDeleteOccurrence").click(function(e) {
                $("#rbHolidaySingle").prop("checked", true);
                DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
            });
        } else {
            DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
        }
    });

    scheduler.attachEvent("onEventDeleted", function(id, ev) {
        //DeleteSchedulerEvent(id, ev);
        //DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
    });

    //scheduler.attachEvent("onBeforeDrag", function() {
    //    return false;
    //});

    //scheduler.attachEvent("onBeforeDrag", function (event_id, mode, native_event_object) {
    //    var ev = scheduler.getEvent(event_id);
    //    return false; // blocked drag if status_no = 1
    //});

    scheduler.attachEvent("onTemplatesReady", function() {
        scheduler.date.week_unit_start = scheduler.date.week_start;
        scheduler.templates.week_unit_date = scheduler.templates.week_date;
        scheduler.templates.week_unit_scale_date = scheduler.templates.week_scale_date;
        scheduler.date.add_week_unit = function(date, inc) { return scheduler.date.add(date, inc * 7, "day"); };
        scheduler.date.get_week_unit_end = function(date) { return scheduler.date.add(date, 5, "day"); };
        scheduler.config.start_on_monday = true;
        scheduler.config.time_step = 15;
        scheduler.xy.min_event_height = 21;
        scheduler.config.hour_size_px = 88;
        scheduler.config.lightbox_recurring = ''; //To remove edit confirmation box
    });
    //$('.schduler_content').empty();
    if (firstTimeLoad == false) {
        if (sections.length > 0) {
            $("#btnScheduleAppointment").prop("disabled", false);
            $("#btnAddHoliday").prop("disabled", false);
            scheduler.init('scheduler_here', new Date($('#divDatetimePicker').val()), "single_unit");
        } else {
            $("#btnScheduleAppointment").prop("disabled", true);
            $("#btnAddHoliday").prop("disabled", true);
            scheduler.init('scheduler_here', new Date($('#divDatetimePicker').val()), "week");

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
    var selectedViewname = $('#hidSelectedViewName').val();
    switch (selectedViewname) {
    case 'Day':
        if (sections.length > 0) {
            scheduler.updateView(new Date($('#divDatetimePicker').val()), "single_unit");
            $('#hidSelectedView').val('1');
            $('#hidSelectedViewName').val('Day');
        } else {
            SetSelectedViewAsWeek();
            scheduler.updateView(new Date($('#divDatetimePicker').val()), "week");
        }
        break;
    case 'Week':
        scheduler.updateView(new Date($('#divDatetimePicker').val()), "week");
        break;
    case 'Month':
        scheduler.updateView(new Date($('#divDatetimePicker').val()), "month");
        break;
    default:
    }
    if (sections.length > 0) {
        $("#btnScheduleAppointment").prop("disabled", false);
        $("#btnAddHoliday").prop("disabled", false);
    } else {
        $("#btnScheduleAppointment").prop("disabled", true);
        $("#btnAddHoliday").prop("disabled", true);
    }
    scheduler.config.scroll_hour = (new Date).getHours();
}

var html = function(id) { return document.getElementById(id); }; //just a helper

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

var SaveSchedularData = function(id, ev) {
    if (firstTimeSave) { // ----- this is to avoid multiple save of an Event
        firstTimeSave = false;
        var jsonData = [];
        if (ev.rec_type == null && ev.rec_type !== '') {
            var recurancedates = scheduler.getRecDates(id);
            if (recurancedates.length > 1) {
                for (var i = 0; i < recurancedates.length; i++) {
                    jsonData[i] = {
                        SchedulingId: ev.TimeSlotId,
                        AssociatedId: 0,
                        AssociatedType: '0',
                        SchedulingType: $('#hidSchedulingType').val(),
                        Status: ev.Availability,
                        StatusType: '0',
                        ScheduleFrom: recurancedates[i].start_date,
                        ScheduleTo: recurancedates[i].end_date,
                        PhysicianId: ev.section_id,
                        TypeOfProcedure: ev.VisitType,
                        PhysicianSpeciality: $("#hfPhysicianSpeciality").val(),
                        FacilityId: $("#hidFacilitySelected").val(),
                        //CorporateId: $('#ddlCorporate').val(),
                        Comments: ev.text,
                        Location: $('#hidFacilityName').val(),
                        ConfirmedByPatient: "1", //set it by default
                        IsRecurring: ev.rec_type == null && ev.rec_type !== '' ? false : ev._timed,
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
                AssociatedId: 0,
                AssociatedType: '0',
                SchedulingType: $('#hidSchedulingType').val(),
                Status: ev.Availability,
                StatusType: '0',
                ScheduleFrom: ev.start_date,
                ScheduleTo: ev.end_date,
                PhysicianId: ev.section_id,
                TypeOfProcedure: ev.VisitType,
                PhysicianSpeciality: $("#hfPhysicianSpeciality").val(),
                FacilityId: $("#hidFacilitySelected").val(),
                Comments: ev.text,
                Location: $('#hidFacilityName').val(),
                ConfirmedByPatient: "1", //set it by default
                IsRecurring: ev.rec_type == null && ev.rec_type !== '' ? false : ev._timed,
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
            success: function(data) {
                ShowMessage('Records Saved Succesfully!', "Success", "success", true);
            },
            error: function(msg) {
            }
        });
    }
};

var DeleteSchedulerEvent = function(id, ev) {
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
        success: function(data) {
            if (data == true) {
                ShowMessage('Records deleted Succesfully!', "Success", "success", true);
            } else {
                ShowMessage('Unable to delete record!', "Error", "error", true);
            }
            scheduler.updateView();
        },
        error: function(msg) {
        }
    });
};

var GetSchedularCustomData = function(type) {
    var jsonData = [];
    var physicianData = [];
    var checkedPhysician = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");
    checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysician);
    var selectedLocation = $('#hidFacilitySelected').val();
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
        SelectedDate: $('#divDatetimePicker').val(),
        Facility: selectedLocation,
        ViewType: type
    };
    //        
    $.ajax({
        cache: false,
        type: "POST",
        url: "/FacultyTimeslots/GetCustomSchedular",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function(data) {
            //        
            SchedulerInit(data, physicianData);
        },
        error: function(msg) {
        }
    });
};

var LoadSchedulngData = function() {
    var jsonData = {
        selectedDate: $('#divDatetimePicker').val(),
        facility: $('#hidFacilitySelected').val(),
        viewType: '1'
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/FacultyTimeslots/LoadSchedulngData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function(data) {
            //SchedulerInit1(data.savedSlots, null, '1');
            SchedulerInit(data.savedSlots, data.selectedPhysicians, '1');
            //AddDepartmentTimming(data.departmentTimmingsList);
        },
        error: function(msg) {
        }
    });
};

var AddDepartmentTimming = function(data) {
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

var BindFacilityDepartments = function() {
    var jsonData = {
        facilityid: $('#hidFacilitySelected').val(),
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/LoadFacilityDepartmentData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify(jsonData),
        success: function(data) {
            $('#divFacilityDepartmentList').empty();
            $('#divFacilityDepartmentList').html(data);
        },
        error: function(msg) {
        }
    });
};

var BindFacilityRooms = function() {
    var jsonData = {
        facilityid: $('#hidFacilitySelected').val(),
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/LoadFacilityRoomsData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify(jsonData),
        success: function(data) {
            $('#divFacilityRoomList').empty();
            $('#divFacilityRoomList').html(data);
        },
        error: function(msg) {
        }
    });
};

function onCheckDepartment() {
    var checkedDepartments = [],
        treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    var checkedPhysicianNodes = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysicianNodes);
    checkedNodeIds1(treeViewDepartments.dataSource.view(), checkedDepartments);
    checkedNodeIds1(treeViewStatus.dataSource.view(), checkedStatusNodes);

    BindSchedularWithDeptFilters(checkedPhysicianNodes, checkedDepartments, checkedStatusNodes);
}

var onCheckView = function(e) {
    var node = e.node;
    var selectedViewname = this.text(e.node);
    var selectedViewValue = $(node).closest("li").data("id");
    $('#hidSelectedView').val(selectedViewValue);
    $('#hidSelectedViewName').val(selectedViewname);
    onCheckFilters();
    //BindCorporatePhysician();
};

var BindSchedularWithDeptFilters = function(phySlected, deptIds, statusfilter) {
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
        SelectedDate: $('#divDatetimePicker').val(),
        Facility: $('#hidFacilitySelected').val(),
        ViewType: $('#hidSelectedView').val(),
        DeptData: deptData
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/FacultyTimeslots/GetSchedularWithFilters",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function(data) {
            if (deptData[0].Id !== 0) {
                SchedulerInit(data, deptData, '2');
            } else {
                SchedulerInit(data, phyData, '1');
            }
        },
        error: function(msg) {
        }
    });
};

var BindAppointmentAvailability = function() {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetGlobalCodesAvailability",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            ggcValue: '4903'
        }),
        success: function(data) {
            /*Bind Availability dropdown - start*/
            var appointmentHTML = '<option value="0">--Select--</option>';
            for (var i = 0; i < data.gClist.length; i++) {
                appointmentHTML += '<option value="' + data.gClist[i].GlobalCodeValue + '">' + data.gClist[i].GlobalCodeName + '</option>'; // Availability -  Field
            }
            $("#ddAvailability").html(appointmentHTML);
            $("#ddAvailability").val("1"); // To select "Initial Booking" by default
            /*Bind Availability dropdown - end*/

            /*Bind Physician dropdown - start*/
            appointmentHTML = '<option value="0">--Select--</option>';
            for (var j = 0; j < data.physicians.length; j++) {
                appointmentHTML += '<option value="' + data.physicians[j].Physician.Id + '" facilityId="' + data.physicians[j].Physician.FacilityId + '" department="' + data.physicians[j].UserDepartmentStr + '" departmentId="' + data.physicians[j].Physician.FacultyDepartment + '" speciality="' + data.physicians[j].UserSpecialityStr + '" specialityId="' + data.physicians[j].Physician.FacultySpeciality + '">' + data.physicians[j].Physician.PhysicianName + '(' + data.physicians[j].UserTypeStr + ')</option>'; // Availability -  Field
            }
            $("#ddPhysician").html(appointmentHTML);
            $("#ddHolidayPhysician").html(appointmentHTML); /*Bind holiday's physician drop down*/
            /*Bind Physician dropdown - end*/

            /*Bind holiday status dropdown - start*/
            appointmentHTML = '<option value="0">--Select--</option>';
            for (var k = 0; k < data.hStatus.length; k++) {
                appointmentHTML += '<option value="' + data.hStatus[k].GlobalCodeValue + '">' + data.hStatus[k].GlobalCodeName + '</option>';
            }
            $("#ddHolidayStatus").html(appointmentHTML);
            $("#ddHolidayStatus").val("7"); // To select "Initial Booking" by default
            /*Bind holiday status dropdown - end*/

            /*Bind holiday types dropdown - start*/
            appointmentHTML = '<option value="0">--Select--</option>';
            for (var x = 0; x < data.hTypes.length; x++) {
                appointmentHTML += '<option value="' + data.hTypes[x].GlobalCodeValue + '">' + data.hTypes[x].GlobalCodeName + '</option>';
            }
            $("#ddHolidayType").html(appointmentHTML);
            /*Bind holiday types dropdown - end*/
        },
        error: function(msg) {
        }
    });
};

var SetDepartmentAndSpeciality = function(e) {
    
    var deptt = $(e).find('option:selected').attr("departmentId");
    var spec = $(e).find('option:selected').attr("specialityId");
    var fid = $(e).find('option:selected').attr("facilityId");
    $("#ddSpeciality").val(spec);
    $("#ddFacility").val(fid);
    BindFacilityDepartments();
    $("#ddlDepartment").val(deptt);

};

function BindFacilityDepartmentspopup() {
    $.ajax({
        type: "POST",
        url: "/Home/GetDepartmentsByFacility",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            facilityId: $("#ddFacility").val()
        }),
        success: function(data) {
            
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $("#ddlDepartment").html(items);
                /*$.each(data, function (i, deaprtments) {
                    items += "<option value='" + deaprtments.Value + "'>" + deaprtments.Text + "</option>";
                });
                $("#ddlDepartment").html(items);*/
                $.each(data.phyList, function(i, physician) {
                    
                    items += '<option value="' + physician.Physician.Id + '" facilityId="' + physician.Physician.FacilityId + '" department="' + physician.UserDepartmentStr + '" departmentId="' + physician.Physician.FacultyDepartment + '" speciality="' + physician.UserSpecialityStr + '" specialityId="' + physician.Physician.FacultySpeciality + '">' + physician.Physician.PhysicianName + '(' + physician.UserTypeStr + ')</option>'; // Availability -  Field
                    //items += "<option value='" + physician.Id + "' PhysicianLicenseNumber='" + physician.PhysicianLicenseNumber + "'>" + physician.PhysicianName + "</option>";
                });
                $("#ddPhysician").html(items);
                BindAppointmentTypes();

            } else {
            }
        },
        error: function(msg) {
        }
    });
}

function BindFacility(selector) {
    $.ajax({
        type: "POST",
        url: "/Home/GetFacilitiesDropdownDataOnScheduler",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function(data) {

            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function(i, facility) {
                    items += "<option value='" + facility.Value + "'>" + facility.Text + "</option>";
                });
                $(selector).html(items);
            } else {
            }
        },
        error: function(msg) {
        }
    });
}

function SelectPhysician(facilityId, physicianId, departmentId, specialityId) {
    BindAppointmentAvailability();
    $("#ddFacility").val(facilityId);
    BindFacilityDepartmentspopup();
    $("#ddPhysician").val(physicianId);
    $("#ddlDepartment").val(departmentId);
    $("#ddSpeciality").val(specialityId);
    $(".searchSlide").removeClass("moveLeft");
}

function BindAppointmentTypes() {
    $.ajax({
        type: "POST",
        url: "/AppointmentTypes/GetAppointmentTypesList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            facilityId: $("#ddFacility").val()
        }),
        success: function(data) {

            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function(i, appt) {
                    items += "<option value='" + appt.Id + "' timeslot='" + appt.TimeSlot + "'>" + appt.Name + "</option>";
                });
                $("#appointmentTypes").html('');
                $("#appointmentTypes").html(items);
            } else {
            }
        },
        error: function(msg) {
        }
    });
}

function AddAppointmentTypesTimeSlot() {
    
    var isMultipleChecked = $("#rbMultiple").prop("checked");
    var isSingleChecked = $("#rbSingle").prop("checked");
    var ctrl = $("#tbApptTypesList");
    var apptTypeName = $("#appointmentTypes option:selected").text();
    var apptTypeValue = $("#appointmentTypes option:selected").val();
    if (apptTypeValue == undefined) {
        ShowMessage('Please select facility from the top and then appointment!', "Warning", "warning", true);
        return false;
    }
    if (apptTypeValue == "0") {
        ShowMessage('Please select atleast one appointments!', "Warning", "warning", true);
        return false;
    }
    var obj = $("#date" + apptTypeValue);
    if (obj.length > 0) {
        ShowMessage('Appointment alreay added in the below listing!', "Warning", "warning", true);
        return false;
    }
    var aptTypesListCount = $("#tbApptTypesList tr").length;
    if (isSingleChecked && aptTypesListCount == 1) {
        ShowMessage('For multiple Appointment you have to click Multiple procedure at the top!', "Warning", "warning", true);
        return false;
    }
    //var dt = new Date();
    //var tt = dt.valueOf();

    var html = "";
    html += "<tr id='tr" + apptTypeValue + "'><input type='hidden' id='hfMain" + apptTypeValue + "' value='0'/>";
    html += "<td>" + apptTypeName + "</td><input type='hidden' id='hfProdTimeSlot" + apptTypeValue + "'  value='" + $("#hfTimeSlot").val() + "'/>";
    html += "<td><input id='date" + apptTypeValue + "' type='text' class='validate[required]'/><input type='button' id='btnShowTimeSlots' class='btn btn-sm btn-primary marginLR10' style='height:28px;' value='View Slots' onclick='OnChangeAppointmentDate(this," + apptTypeValue + ")'/></td>";
    html += "<td><input id='timef" + apptTypeValue + "' type='text' class='validate[required]'/></td>";
    html += "<td><input id='timet" + apptTypeValue + "' type='text' class='validate[required]'/></td>";
    html += "<td class='recurrence_event'><input id='chkIsRec" + apptTypeValue + "' type='checkbox' onchange='OnChangeIsReccurrenceChk(this," + apptTypeValue + ");'/></td>";
    //html += "<td><select id='aptAvail" + apptTypeValue + "' class='validate[required]'></select></td>";
    html += "<td><img src='/images/delete.png' width='15px' onclick='RemoveAppointmentProcedures(" + apptTypeValue + ")'/></td>";
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
    $("#date" + apptTypeValue).datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        timepicker: false,
        closeOnDateSelect: true
        /*,onChangeDateTime: function (e) {
            OnChangeAppointmentDate(e, apptTypeValue);
            return false;
        }*/
    });
    /* $("#timef" + apptTypeValue).datetimepicker({
         datepicker: false,
         format: 'H:i',
         step: parseInt($("#hfTimeSlot").val()),
         mask: false,
     });*/
    $("#timef" + apptTypeValue).kendoTimePicker({
        interval: parseInt($("#hfTimeSlot").val()),
        format: "HH:mm",
        change: function(e) {
            startChange("#timef" + apptTypeValue, "#timet" + apptTypeValue, parseInt($("#hfTimeSlot").val()));
        }
    }).data("kendoTimePicker").readonly();
    $("#timet" + apptTypeValue).kendoTimePicker({
        interval: parseInt($("#hfTimeSlot").val()),
        format: "HH:mm"
    }).data("kendoTimePicker").readonly();
    //$("#aptAvail" + apptTypeValue).html($("#ddAvailability").html());
    var hfApptTypes = $("#hfAppointmentTypes").val();
    var concatApptTypes = hfApptTypes + "," + apptTypeValue;
    $("#hfAppointmentTypes").val(concatApptTypes.replace(/^,|,$/g, ''));

    $("#appointmentTypes").val("0");

    var date = new Date();
    $("#date" + apptTypeValue).val(date.getMonth() + 1 + "/" + date.getDate() + '/' + date.getFullYear()); //adding today's as by default
    var initialDate = "";
    if ($("#hfAppointmentTypes").val() != "") {
        var hfAptypesObjValueArray = $("#hfAppointmentTypes").val().split(",");
        initialDate = $("#date" + hfAptypesObjValueArray[0]).val();
        $("#date" + apptTypeValue).val(initialDate);
    }
    //var btnSelection = $('input[name=rdbtnSelection]:checked').val();
    //if (btnSelection === "2") {
    //    $('#btnAddAppointmentType').attr('disabled', 'disabled');
    //} else {
    //    $('#btnAddAppointmentType').removeAttr('disabled');
    //}
}

function OnChangeApptTypes(e) {
    var ts = $(e).find('option:selected').attr("timeslot");
    var value = $(e).find('option:selected').attr("value");
    $("#hfTimeSlot").val(ts);
    apptTypeIdArray.push(value);
}

var SaveCustomSchedular = function() {
    
    var apptRecArray1 = apptRecArray;
    var btnSelection = $('input[name=rdbtnSelection]:checked').val();
    var jsonData = [];
    var selectedAppointmenttype = $('#hfAppointmentTypes').val();
    if (selectedAppointmenttype != "") {
        if (btnSelection == "2") {
            var appointmenttypeArrayVal = selectedAppointmenttype.split(',');
            if (appointmenttypeArrayVal.length > 1) {
                ShowMessage('For multiple Appointment you have to click Multiple procedure at the top!', "Warning", "warning", true);
                unBlockRecurrenceDiv();
                return;
            } else {
                var isRecurrance = $('#chkIsRec' + selectedAppointmenttype).prop('checked');
                jsonData[0] = ({
                    SchedulingId: $('#hfMain' + selectedAppointmenttype).val(),
                    AssociatedId: $('#hfPatientId').val(),
                    AssociatedType: '1',
                    SchedulingType: $('#hidSchedulingType').val(),
                    Status: $('#ddAvailability').val(), //Status: ($('#aptAvail' + selectedAppointmenttype).val()), //$('#ddAvailability').val(),
                    StatusType: '', //$('#ddAvailability').val(),
                    ScheduleFrom: isRecurrance ? apptRecArray1[0].Rec_Start_Date : ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()),
                    ScheduleTo: isRecurrance ? apptRecArray1[0].end_By : ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timet' + selectedAppointmenttype).val()),
                    PhysicianId: $('#ddPhysician').val(),
                    TypeOfProcedure: $('#hfAppointmentTypes').val(),
                    PhysicianSpeciality: $("#ddSpeciality").val(),
                    ExtValue1: $("#ddlDepartment").val(),
                    ExtValue5: $("#txtPhysicianComment").val(),
                    FacilityId: $("#ddFacility").val(),
                    Comments: $('#txtDescription').val(),
                    Location: $('#ddFacility :selected').text(),
                    PhysicianName: $('#ddPhysician :selected').text(),
                    ConfirmedByPatient: "1", //set it by default  Frequency_Type
                    IsRecurring: isRecurrance,
                    RecurringDateFrom: isRecurrance ? ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()) : '', // apptRecArray1[0].Frequency_Type === 3 ? apptRecArray1[0].Rec_Start_Date : ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()), //isRecurrance ? ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()) : null,
                    RecurringDateTill: isRecurrance ? apptRecArray1[0].end_By : '', //apptRecArray1[0].end_By, // isRecurrance ? apptRecArray1[0].end_By : null,
                    //EventId: id,
                    RecType: isRecurrance ? apptRecArray1[0].Rec_Type : '', //apptRecArray1[0].Rec_Type, //
                    RecPattern: isRecurrance ? apptRecArray1[0].Rec_Pattern : '', //apptRecArray1[0].Rec_Pattern, // isRecurrance ? apptRecArray1[0].Rec_Pattern : null,
                    RecEventlength: isRecurrance ? CalculateMilliSecondsBetweenDates(($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()), ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timet' + selectedAppointmenttype).val())) : null,
                    //CalculateMilliSecondsBetweenDates(($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()), ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timet' + selectedAppointmenttype).val())),
                    //isRecurrance ? CalculateMilliSecondsBetweenDates(($('#date' + selectedAppointmenttype).val() + ' ' + $('#timef' + selectedAppointmenttype).val()), ($('#date' + selectedAppointmenttype).val() + ' ' + $('#timet' + selectedAppointmenttype).val())) : null,
                    //RecEventPId: isRecurrance ? ev.event_pid,
                    WeekDay: '',
                    EventParentId: $("#hidEventParentId").val(),
                    PatientName: $('#patientname').val(),
                    PatientDOB: $('#txtPersonDOB').val(),
                    PatientEmailId: $('#email').val(),
                    PatientPhoneNumber: $("#lblPatientCountryCode").html() + "-" + $('#phoneno').val(),
                    PatientId: $('#hfPatientId').val(),
                    PatientEmirateIdNumber: $('#txtEmiratesNationalId').val(),
                    MultipleProcedures: false,
                    EventId: schedularEventObj,
                });
            }
        } else {
            var appointmenttypeArray = selectedAppointmenttype.split(',');
            for (var i = 0; i < appointmenttypeArray.length; i++) {
                var appointmenttypeobj = appointmenttypeArray[i];
                var isRecurranceMultiple = $('#chkIsRec' + appointmenttypeobj).prop('checked');
                var recuranceArrayObj = null;
                $.each(apptRecArray, function(k1, val) {
                    $.each(val, function(key, name) {
                        if (name == appointmenttypeobj) {
                            recuranceArrayObj = apptRecArray[k1];
                        }
                    });
                });
                jsonData[i] = ({
                    SchedulingId: $('#hfMain' + appointmenttypeobj).val(),
                    AssociatedId: $('#hfPatientId').val(),
                    AssociatedType: '1',
                    SchedulingType: $('#hidSchedulingType').val(),
                    Status: $('#ddAvailability').val(), // ($('#aptAvail' + appointmenttypeobj).val()), //$('#ddAvailability').val(),
                    StatusType: '', //$('#ddAvailability').val(),
                    ScheduleFrom: isRecurranceMultiple ? recuranceArrayObj.Rec_Start_Date : ($('#date' + appointmenttypeobj).val() + ' ' + $('#timef' + appointmenttypeobj).val()),
                    ScheduleTo: isRecurranceMultiple ? recuranceArrayObj.end_By : ($('#date' + appointmenttypeobj).val() + ' ' + $('#timet' + appointmenttypeobj).val()),
                    //($('#date' + appointmenttypeobj).val() + ' ' + $('#timet' + appointmenttypeobj).val()),
                    PhysicianId: $('#ddPhysician').val(),
                    TypeOfProcedure: appointmenttypeobj,
                    PhysicianSpeciality: $("#ddSpeciality").val(),
                    ExtValue1: $("#ddlDepartment").val(),
                    FacilityId: $("#ddFacility").val(),
                    Comments: $('#txtDescription').val(),
                    Location: $('#ddFacility :selected').text(),
                    ConfirmedByPatient: "1", //set it by default
                    PhysicianName: $('#ddPhysician :selected').text(),
                    ExtValue5: $("#txtPhysicianComment").val(),
                    IsRecurring: isRecurranceMultiple,
                    RecurringDateFrom: isRecurranceMultiple ? ($('#date' + appointmenttypeobj).val() + ' ' + $('#timef' + appointmenttypeobj).val()) : null,
                    //recuranceArrayObj.Frequency_Type === 3 ? recuranceArrayObj.Rec_Start_Date : ($('#date' + appointmenttypeobj).val() + ' ' + $('#timef' + appointmenttypeobj).val()), // isRecurranceMultiple ? ($('#date' + appointmenttypeobj).val() + ' ' + $('#timef' + appointmenttypeobj).val()) : null,
                    RecurringDateTill: isRecurranceMultiple ? recuranceArrayObj.end_By : null, //recuranceArrayObj.end_By, //isRecurranceMultiple ? recuranceArrayObj.end_By : null,
                    //EventId: id,
                    RecType: isRecurranceMultiple ? recuranceArrayObj.Rec_Type : null, //recuranceArrayObj.Rec_Type, // isRecurranceMultiple ? recuranceArrayObj.Rec_Type : null,
                    RecPattern: isRecurranceMultiple ? recuranceArrayObj.Rec_Pattern : null, // recuranceArrayObj.Rec_Pattern, //isRecurranceMultiple ? recuranceArrayObj.Rec_Pattern : null,
                    RecEventlength: isRecurranceMultiple ? CalculateMilliSecondsBetweenDates(($('#date' + appointmenttypeobj).val() + ' ' + $('#timef' + appointmenttypeobj).val()), ($('#date' + appointmenttypeobj).val() + ' ' + $('#timet' + appointmenttypeobj).val())) : null,
                    //CalculateMilliSecondsBetweenDates(($('#date' + appointmenttypeobj).val() + ' ' + $('#timef' + appointmenttypeobj).val()), ($('#date' + appointmenttypeobj).val() + ' ' + $('#timet' + appointmenttypeobj).val())),
                    //isRecurranceMultiple ? CalculateMilliSecondsBetweenDates(($('#date' + appointmenttypeobj).val() + ' ' + $('#timef' + appointmenttypeobj).val()), ($('#date' + appointmenttypeobj).val() + ' ' + $('#timet' + appointmenttypeobj).val())) : null,
                    //RecEventPId: isRecurrance ? ev.event_pid,
                    WeekDay: '',
                    EventParentId: $("#hidEventParentId").val(),
                    PatientName: $('#patientname').val(),
                    PatientDOB: $('#txtPersonDOB').val(),
                    PatientEmailId: $('#email').val(),
                    PatientPhoneNumber: $("#lblPatientCountryCode").html() + "-" + $('#phoneno').val(),
                    PatientEmirateIdNumber: $('#txtEmiratesNationalId').val(),
                    MultipleProcedures: true,
                    PatientId: $('#hfPatientId').val(),
                    EventId: schedularEventObj,
                });
            }
        }
        var jsonD = JSON.stringify(jsonData);
        $.ajax({
            type: "POST",
            url: '/FacultyTimeslots/SavePatientScheduling',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonD,
            success: function(data) {
                if (data.IsError) {
                    if (data.notAvialableTimeslotslist.length > 0) {
                        for (var j = 0; j < data.notAvialableTimeslotslist.length; j++) {
                            ShowMessage('Unable to book slot for Date :' + data.notAvialableTimeslotslist[j].DateFromSTR + ' Time Range:' + data.notAvialableTimeslotslist[j].TimeFromStr + ' - ' + data.notAvialableTimeslotslist[j].TimeTOStr + ' With physician' + data.notAvialableTimeslotslist[j].PhysicianName + ' (' + data.notAvialableTimeslotslist[j].PhysicianSpl + ')', "Warning", "warning", true);
                            unBlockRecurrenceDiv();
                        }
                    } else if (data.roomEquipmentnotaviable.length > 0) {
                        for (var k = 0; k < data.roomEquipmentnotaviable.length; k++) {
                            ShowMessage(data.roomEquipmentnotaviable[k].Reason, "Warning", "warning", true);
                            unBlockRecurrenceDiv();
                        }
                    }
                } else {
                    ShowMessage('Records Saved Succesfully!', "Success", "success", true);
                    onCheckFilters();
                    //$('.hidePopUp').hide();
                    $.validationEngine.closePrompt('.formError', true);
                    ClearSchedulingPopup();
                }
                $('#hfPatientId').val(data.patientid);
            },
            error: function(msg) {
            }
        });
    } else {
        ShowMessage('Please add atleast one appointment!', "Warning", "warning", true);
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
        success: function(data) {

            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function(i, physician) {
                    
                    items += '<option value="' + physician.Physician.Id + '" facilityId="' + physician.Physician.FacilityId + '" department="' + physician.UserDepartmentStr + '" departmentId="' + physician.Physician.FacultyDepartment + '" speciality="' + physician.UserSpecialityStr + '" specialityId="' + physician.Physician.FacultySpeciality + '">' + physician.Physician.PhysicianName + '(' + physician.UserTypeStr + ')</option>'; // Availability -  Field
                    //items += "<option value='" + physician.Id + "' PhysicianLicenseNumber='" + physician.PhysicianLicenseNumber + "'>" + physician.PhysicianName + "</option>";
                });
                $("#ddPhysician").html(items);
            } else {
            }
        },
        error: function(msg) {
        }
    });
}

var ClearSchedulingPopup = function() {
    $("#divSchedularPopUpContent").clearForm(true);
    $('#tbList').empty();
    $('#hfAppointmentTypes').val('');
    $("#tbApptTypesList").html('');
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
    $('html, body').css({
        'overflow': 'auto',
        'height': 'auto'
    });
    $("#hidEventParentId").val('');
    BindAppointmentAvailability();
    $('.searchSlide').removeClass('moveLeft');
    $('.searchSlide1').removeClass('moveLeft');
    unBlockRecurrenceDiv();
    $("#divRecurrenceEventPopup").hide();
    $("#divSchedularPopUp").hide();

    $("#rbMultiple").prop("checked", false);
    $("#rbSingle").prop("checked", true);
    $("#rbMultiple").prop("disabled", false);
    $("#daily").show();
    $("#weekly").hide();
    $("#monthly").hide();
    //$('#btnAddAppointmentType').removeAttr('disabled');
    $("#btnDeleteSchedulingData").hide();
    $("#hfSelPhysicianId").val("0");
    $("#hfExternalValue3").val("False");
};

var BindLightBox = function(obj) {
    
    $("#btnSaveSchedulingData").val("Update");
    $("#btnDeleteSchedulingData").show();
    var scheduleType = obj.SchedulingType;
    switch (scheduleType) {
    case "1":
        $("#imgPatientExist").show();
        $("#imgPatientNew").hide();
        $(".modal-title").html("Appointments Scheduler");

        $("#divRecurrenceEventPopup").show();
        if (obj.IsRecurrance) {
            $("#btnEditRecurrenceSeries").click(function(e) {
                
                $('#divSchedularPopUp').show();
                $("#divRecurrenceEventPopup").hide();
                var procObj = obj.TypeOfProcedureCustomModel;
                for (var k = 0; k < procObj.length; k++) {
                    $("#date" + procObj[k].TypeOfProcedureId).val(procObj[k].Rec_Start_date);
                }
            });
            $("#btnEditRecurrenceOccurrence").click(function(e) {
                
                $("#rbMultiple").prop("checked", false);
                $("#rbSingle").prop("checked", true);
                $("#rbMultiple").prop("disabled", true);
                $('#divSchedularPopUp').show();
                $("#divRecurrenceEventPopup").hide();
                var procObj = obj.TypeOfProcedureCustomModel;
                for (var k = 0; k < procObj.length; k++) {
                    $("#chkIsRec" + procObj[k].TypeOfProcedureId).prop("checked", false);
                    $("#chkIsRec" + procObj[k].TypeOfProcedureId).prop("disabled", true);
                    $("#btnEditRecurrence" + procObj[k].TypeOfProcedureId).hide();
                }
            });
        } else {
            $('#divSchedularPopUp').show();
            $("#divRecurrenceEventPopup").hide();
            var procObj = obj.TypeOfProcedureCustomModel;
            for (var k = 0; k < procObj.length; k++) {
                $("#date" + procObj[k].TypeOfProcedureId).val(procObj[k].Rec_Start_date);
            }
        }
        //$('#divSchedularPopUp').show();
        /*Bind radio button - start*/
        if (obj.MultipleProcedure) {
            $("#rbMultiple").prop("checked", true);
            //$('#btnAddAppointmentType').removeAttr('disabled');
        } else {
            $("#rbSingle").prop("checked", true);
            //$('#btnAddAppointmentType').attr('disabled', 'disabled');
        }
        /*Bind radio button - end*/
        $('#hidSchedulingType').val(obj.SchedulingType);
        /*Bind physician section - start*/
        $("#txtDescription").val(obj.text);
        $("#ddFacility").val(obj.location);
        BindFacilityDepartmentspopup();
        $("#ddlDepartment").val(obj.Department);
        $("#ddSpeciality").val(obj.PhysicianSpeciality);
        $("#ddPhysician").val(obj.physicianid);
        $("#ddAvailability").val(obj.Availability);
        $("#txtPhysicianComment").val(obj.PhysicianComments);
        /*Bind physician section - end*/

        /*Bind patient section - start*/
        $("#hfPatientId").val(obj.patientid);
        $("#patientname").val(obj.PatientName);
        $("#txtPersonDOB").val(obj.PatientDOB);
        $("#email").val(obj.PatientEmailId);
        $("#phoneno").val(obj.PatientPhoneNumber);
        FormatMaskedPhone('#lblPatientCountryCode', "#ddlPatientCountries", "#phoneno");
        BindPatientEncounterList(obj.patientid);
        schedularEventObj = obj.id;
        $("#hidEventParentId").val(obj.EventParentId);
        /*Bind patient section - end*/

        /*Bind appointment types - start*/
        var ctrl = $("#tbApptTypesList");
        var aptObj = obj.TypeOfProcedureCustomModel;
        var html = "";
        //data = jLinq.from(data).orderBy("GlobalCodeValue").select();
        //var customdata = jLinq.from(data)
        //    .where(function(record) { return record.DateSelected == aptObj[0].DateSelected; }).select();
        //
        for (var i = 0; i < aptObj.length; i++) {
            html += "<tr id='tr" + aptObj[i].TypeOfProcedureId + "'><input type='hidden' id='hfMain" + aptObj[i].TypeOfProcedureId + "' value='" + aptObj[i].MainId + "'/>";
            html += "<td>" + aptObj[i].TypeOfProcedureName + "</td><input type='hidden' id='hfProdTimeSlot" + aptObj[i].TypeOfProcedureId + "'  value='" + aptObj[i].TimeSlotTimeInterval + "'/>";
            html += "<td><input id='date" + aptObj[i].TypeOfProcedureId + "' type='text' value='" + aptObj[i].DateSelected + "'/><input type='button' id='btnShowTimeSlots'  class='btn btn-sm btn-primary marginLR10' value='View Slots' onclick='OnChangeAppointmentDate(this," + aptObj[i].TypeOfProcedureId + ")'/></td><input type='hidden' id='hfTypeOfProcedureId" + aptObj[i].TypeOfProcedureId + "'  value='" + aptObj[i].ProcedureAvailablityStatus + "'/>";
            html += "<td><input id='timef" + aptObj[i].TypeOfProcedureId + "' type='text' value='" + aptObj[i].TimeFrom + "'/></td>";
            html += "<td><input id='timet" + aptObj[i].TypeOfProcedureId + "' type='text' value='" + aptObj[i].TimeTo + "'/></td>";
            if (aptObj[i].IsRecurrance) {
                html += "<td><input id='chkIsRec" + aptObj[i].TypeOfProcedureId + "' mon_rpt_date='" + obj.start_date.getDate() + "' statusattr='edit' end_By='" + aptObj[i].end_By + "' dept_opn_days='" + aptObj[i].DeptOpeningDays + "' rec_pattern='" + aptObj[i].Rec_Pattern + "' rec_type='" + aptObj[i].Rec_Type + "' type='checkbox' checked='checked' onchange='OnChangeIsReccurrenceChk(this," + aptObj[i].TypeOfProcedureId + ");'/><input type='button' mon_rpt_date='" + obj.start_date.getDate() + "' statusattr='edit' end_By='" + aptObj[i].end_By + "' rec_pattern='" + aptObj[i].Rec_Pattern + "' checked='checked' dept_opn_days='" + aptObj[i].DeptOpeningDays + "' rec_type='" + aptObj[i].Rec_Type + "' id='btnEditRecurrence" + aptObj[i].TypeOfProcedureId + "' class='btn btn-xs btn-default' style='margin-left:5px;' value='Edit Recurrence' onclick='OnChangeIsReccurrenceChk(this," + aptObj[i].TypeOfProcedureId + ");'/></td>";
            } else {
                html += "<td><input id='chkIsRec" + aptObj[i].TypeOfProcedureId + "' type='checkbox' value='' onchange='OnChangeIsReccurrenceChk(this," + aptObj[i].TypeOfProcedureId + ");'/></td>";
            }
            //html += "<td><select id='aptAvail" + aptObj[i].TypeOfProcedureId + "'></select></td>";
            html += "<td><img src='/images/delete.png' width='15px' onclick='RemoveAppointmentProcedures(" + aptObj[i].TypeOfProcedureId + ")'/></td>";
            html += "</tr>";
            ctrl.html(html);

            var hfApptTypes = $("#hfAppointmentTypes").val();
            var concatApptTypes = hfApptTypes + "," + aptObj[i].TypeOfProcedureId;
            $("#hfAppointmentTypes").val(concatApptTypes.replace(/^,|,$/g, ''));
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
                    var monthPatternObj = aptObj[i].Rec_Pattern.split('_');
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
        for (var u = 0; u < loop.length; u++) {
            $("#date" + loop[u]).datetimepicker({
                format: 'm/d/Y',
                minDate: 0,
                timepicker: false,
                closeOnDateSelect: true
                /*,onChangeDateTime: function (e) {
                        OnChangeAppointmentDate(e, loop[u]);
                        return false;
                    }*/
            });
            $("#timef" + loop[u]).kendoTimePicker({
                interval: parseInt($("#hfProdTimeSlot" + loop[u]).val()),
                format: "HH:mm",
                change: function(e) {
                    
                    startChange("#" + e.sender.element[0].id, "#" + e.sender.element[0].id.replace("f", "t"), parseInt(e.sender.options.interval)); //fromtimeId, totimeId, timeinterval
                }
            }).data("kendoTimePicker").readonly();
            $("#timet" + loop[u]).kendoTimePicker({
                interval: parseInt($("#hfProdTimeSlot" + loop[u]).val()),
                format: "HH:mm"
            }).data("kendoTimePicker").readonly();
            //$("#aptAvail" + loop[u]).html($("#ddAvailability").html());
            //$("#aptAvail" + loop[u]).val($("#hfTypeOfProcedureId" + loop[u]).val());
            startChange("#timef" + loop[u], "#timet" + loop[u], parseInt($("#hfProdTimeSlot" + loop[u]).val()));
        }
        /*Bind appointment types - end*/
        $("#txtEmiratesNationalId").val(obj.EmirateIdnumber);
        break;
    case "2":
        $(".modal-title").html("Holiday/Vacations Scheduler");
        /*Bind radio button - start*/
        if (obj.MultipleProcedure) {
            $("#rbHolidayMultiple").prop("checked", true);
            $("#divHolidayToDateTime").show();
            /*for multiple, first we need to show edit multiple series popup and then on its input show real popup*/
            $("#divMultipleEventPopup").show();
            $("#btnEditSeries").click(function(e) {
                
                $('#divSchedularPopUp').show();
                $("#hidEventParentId").val(obj.EventParentId);
                $("#hfSchedulingId").val(obj.TimeSlotId);
                $("#divMultipleEventPopup").hide();
                $("#rbHolidaySingle").prop("disabled", true);
                $("#rbHolidayMultiple").prop("disabled", true);
                $("#eventFromDate").val(obj.Rec_Start_date);
                $("#eventToDate").val(obj.Rec_end_date);
                $('#hidEventFromDate').val(obj.Rec_Start_date);
                $('#hidEventToDate').val(obj.Rec_end_date);
            });
            $("#btnEditOccurrence").click(function(e) {
                
                $('#divSchedularPopUp').show();
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
            $("#hidEventParentId").val(obj.EventParentId);
            $("#hfSchedulingId").val(obj.TimeSlotId);
            $("#rbHolidaySingle").prop("disabled", true);
            $("#rbHolidayMultiple").prop("disabled", true);
            $("#rbHolidaySingle").prop("checked", true);
            $("#rbHolidayMultiple").prop("checked", false);
            $("#divHolidayToDateTime").hide();
            $('#divSchedularPopUp').show();
            $("#hfSelPhysicianId").val(obj.section_id);
            $("#hfExternalValue3").val(obj.MultipleProcedure == false ? "False" : "True");
        }
        var objStartDate = obj.start_date;
        var newlyCreatedStDate = (objStartDate.getMonth() + 1) + "/" + objStartDate.getDate() + "/" + objStartDate.getFullYear();

        var objEndDate = obj.end_date;
        var newlyCreatedEndDate = (objEndDate.getMonth() + 1) + "/" + objEndDate.getDate() + "/" + objEndDate.getFullYear();
        /*Bind radio button - end*/
        $("#eventFromDate").datetimepicker({
            format: 'm/d/Y',
            minDate: 0, //yesterday is minimum date(for today use 0 or -1970/01/01)
            maxDate: '2025/12/12',
            timepicker: false,
            value: newlyCreatedStDate,
            closeOnDateSelect: true
        });
        $("#eventToDate").datetimepicker({
            format: 'm/d/Y',
            minDate: 0, //yesterday is minimum date(for today use 0 or -1970/01/01)
            maxDate: '2025/12/12',
            timepicker: false,
            value: newlyCreatedEndDate,
            closeOnDateSelect: true
        });
        /*Bind main section - start*/
        $('#hidEventFromDate').val(obj.start_date);
        $('#hidEventToDate').val(obj.end_date);
        $("#txtHolidayDescription").val(obj.text);
        $("#ddHolidayType").val(obj.VacationType);
        $("#ddHolidayStatus").val(obj.Availability);
        ShowPhysicianBasedOnHolidayType(obj.VacationType);
        //$("#eventFromDate").val(obj.start_date.toLocaleDateString());
        //$("#eventToDate").val(obj.end_date.toLocaleDateString());
        $("#ddHolidayPhysician").val(obj.section_id);
        /*Bind main section - end*/
        break;
    default:
        break;
    }
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
        success: function(data) {
            
            var html = "";
            for (var k = 0; k < data.length; k++) {
                html += "<tr>";
                html += "<td>" + data[k].EncounterNumber + "</td>";
                html += "<td>" + data[k].Physician.PhysicianName + "</td>";
                html += "<td><input type='button' value='select' onclick='SelectPhysician(" + data[k].Physician.FacilityId + "," + data[k].Physician.Id + "," + data[k].Physician.FacultyDepartment + "," + data[k].Physician.FacultySpeciality + ")'/></td>";
                html += "</tr>";
            }
            $("#tbList").html(html);
        }
    });
}

function RemoveAppointmentProcedures(apptId) {
    var selectedTypes = $("#hfAppointmentTypes").val();
    var selectedTypesArray = selectedTypes.split(',');
    //var tt = apptRecArray;
    $.each(apptRecArray, function(i, val) {
        $.each(val, function(key, name) {
            
            if (name == apptId) {
                apptRecArray.splice(i, 1);
            }
        });
    });
    
    //var index = selectedTypesArray.indexOf(apptId.toString());
    selectedTypesArray.remove(apptId.toString());
    $("#hfAppointmentTypes").val(selectedTypesArray.join(','));
    $("#tr" + apptId).remove();
}

Array.prototype.remove = function(val) {
    var i = this.indexOf(val);
    return i > -1 ? this.splice(i, 1) : [];
};

var ShowLightBoxStyle = function(schedulingType) {
    switch (schedulingType) {
    case "1":
        $("#divMultipleSingle").show();
        $("#divPhysicianPatient").show();
        $("#divTypeofProc").show();
        $("#divHoliday").hide();
        $("#divDateTime").hide();
        $(".main_content").css("height", "496px");
        break;
    case "2":
        $("#divMultipleSingle").hide();
        $("#divPhysicianPatient").hide();
        $("#divTypeofProc").hide();
        $("#divHoliday").show();
        $("#divDateTime").hide();
        $(".main_content").css({ "height": "180px", "margin-top": "0" });

        break;
    default:
        break;
    }
};

var OnChangeType = function(e) {
    var obj = $(e);
    var value = obj.val();
    ShowPhysicianBasedOnHolidayType(value);
};

var ShowPhysicianBasedOnHolidayType = function(val) {
    
    switch (val) {
    case "11":
        $("#divHolidayPhysician").show();
        $("#ddHolidayPhysician").addClass("validate[required]");
        break;
    case "12":
        $("#divHolidayPhysician").hide();
        $("#ddHolidayPhysician").removeClass("validate[required]");
        break;
    default:
        break;
    }
};

function myFunction(stringVal) {
    // var dateStartObj = stringVal.split('/');
    var dateStartObj = $('#eventFromDate').val().split('/');
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

var SaveHolidaySchedular = function() {
    var btnSelection = $('input[name=rdbtnHolidaySelection]:checked').val();
    var jsonData = [];
    if (btnSelection == "2") {
        // single Day Event/ Leave
        var dateStartObj = $('#eventFromDate').val().split('/');
        //var testdate = myFunction($('#eventFromDate').val());
        var datecustomObj;
        var startFromDate;
        if (!isNaN(parseInt(dateStartObj[1]))) {
            datecustomObj = new Date($('#eventFromDate').val());
            startFromDate = new Date($('#eventFromDate').val());
        } else {
            datecustomObj = new Date($('#hidEventFromDate').val());
            startFromDate = new Date($('#hidEventFromDate').val());
        }

        datecustomObj.setDate(datecustomObj.getDate() + 1);
        datecustomObj.setMinutes(datecustomObj.getMinutes() - 1);
        jsonData[0] = ({
            SchedulingId: $('#hfSchedulingId').val() === "" ? 0 : $('#hfSchedulingId').val(),
            AssociatedId: $('#ddHolidayType').val() == "11" ? $('#ddHolidayPhysician').val() : $('#hidFacilitySelected').val(),
            AssociatedType: $('#hidSchedulingType').val(),
            SchedulingType: $('#hidSchedulingType').val(),
            Status: ($('#ddHolidayStatus').val()), //$('#ddAvailability').val(),
            StatusType: '', //$('#ddAvailability').val(),
            ScheduleFrom: startFromDate,
            ScheduleTo: datecustomObj,
            PhysicianId: '',
            TypeOfProcedure: $('#ddHolidayType').val(),
            FacilityId: $("#hidFacilitySelected").val(),
            Comments: $('#txtHolidayDescription').val(),
            Location: $('#hidFacilityName').val(),
            ConfirmedByPatient: "", //set it by default
            ExtValue2: $('#ddHolidayType').val() == "12" ? $('#hidFacilitySelected').val() : '',
            //IsRecurring: ev.rec_type == null && ev.rec_type !== '' ? false : ev._timed,
            RecurringDateFrom: $('#hfIsReccurrence').val() === 'True' ? $('#hfRecStartDate').val() : '',
            RecurringDateTill: $('#hfIsReccurrence').val() === 'True' ? $('#hfRecEndDate').val() : '',
            //hfRecStartDate
            //hfRecEndDate
            //hfIsReccurrence
            //EventId: id,
            //RecType: ev.rec_type,
            //RecPattern: ev.rec_pattern,
            //RecEventlength: ev.event_length,
            //RecEventPId: ev.event_pid,
            WeekDay: '',
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
        var dateMStartObj = $('#eventFromDate').val().split('/');
        var datecustomFromObj;

        var dateMToObj = $('#eventToDate').val().split('/');
        var datecustomToObj;

        if (!isNaN(parseInt(dateMStartObj[1]))) {
            datecustomFromObj = new Date($('#eventFromDate').val());
        } else {
            datecustomFromObj = new Date($('#hidEventFromDate').val());
        }

        if (!isNaN(parseInt(dateMToObj[1]))) {
            datecustomToObj = new Date($('#eventToDate').val());
        } else {
            datecustomToObj = new Date($('#hidEventToDate').val());
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
                SchedulingId: $('#hfSchedulingId').val() === "" ? 0 : $('#hfSchedulingId').val(), //$('#hfMain' + selectedAppointmenttype).val(),
                AssociatedId: $('#ddHolidayType').val() == "11" ? $('#ddHolidayPhysician').val() : $('#hidFacilitySelected').val(),
                AssociatedType: $('#hidSchedulingType').val(),
                SchedulingType: $('#hidSchedulingType').val(),
                Status: ($('#ddHolidayStatus').val()), //$('#ddAvailability').val(),
                StatusType: '', //$('#ddAvailability').val(),
                ScheduleFrom: dateStartfrom,
                ScheduleTo: dateEndTo,
                PhysicianId: '',
                TypeOfProcedure: $('#ddHolidayType').val(),
                FacilityId: $("#hidFacilitySelected").val(),
                Comments: $('#txtHolidayDescription').val(),
                Location: $('#hidFacilityName').val(),
                ConfirmedByPatient: "", //set it by default
                ExtValue2: $('#ddHolidayType').val() == "12" ? $('#hidFacilitySelected').val() : '',
                RecurringDateFrom: $('#eventFromDate').val(),
                RecurringDateTill: $('#eventToDate').val(),
                //IsRecurring: ev.rec_type == null && ev.rec_type !== '' ? false : ev._timed,
                //RecurringDateFrom: ev.start_date,
                //RecurringDateTill: ev.end_date,
                //EventId: id,
                //RecType: ev.rec_type,
                //RecPattern: ev.rec_pattern,
                //RecEventlength: ev.event_length,
                //RecEventPId: ev.event_pid,
                WeekDay: '',
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
        url: '/FacultyTimeslots/SaveHolidayPlannerScheduling',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonD,
        success: function(data) {
            if (data.IsError) {
                for (var j = 0; j < data.notAvialableTimeslotslist.length; j++) {
                    ShowMessage(data.notAvialableTimeslotslist[j].Reason, "Warning", "warning", true);
                    $('html, body').css({
                        'overflow': 'auto',
                        'height': 'auto'
                    });
                    unBlockRecurrenceDiv();
                }
            } else {
                ShowMessage('Records Saved Succesfully!', "Success", "success", true);
                onCheckFilters();
                //$('.hidePopUp').hide();
                $.validationEngine.closePrompt('.formError', true);
                ClearSchedulingPopup();
            }
        },
        error: function(msg) {
        }
    });
};

var OnChangeHolidayRB = function(e) {
    var obj = $(e);
    var value = obj.val();
    switch (value) {
    case "2":
        $("#divHolidayToDateTime").hide();
        break;
    case "1":
        $("#divHolidayToDateTime").show();
        break;
    default:
        break;
    }
};

var CalculateDaysBetweenDates = function(startdate, enddate) {
    var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
    var firstDate = new Date(Date.parse(startdate));
    var secondDate = new Date(Date.parse(enddate));
    var diffDays = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));
    return diffDays;
};

var DeleteSchduling = function(epid, schid, schtype, extVal3) {
    var jsonData = "";
    switch (schtype) {
    case "1":
        jsonData = { eventParentId: epid, schedulingId: schid, schedulingType: schtype, externalValue3: extVal3 };
        break;
    case "2":
        if ($("#rbHolidaySingle").prop("checked") == true) {
            jsonData = { eventParentId: "", schedulingId: schid, schedulingType: schtype, externalValue3: extVal3 };
        } else {
            jsonData = { eventParentId: epid, schedulingId: schid, schedulingType: schtype, externalValue3: extVal3 };
        }
        break;
    }
    $.ajax({
        type: "POST",
        url: '/FacultyTimeslots/DeleteSchduling',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function(data) {
            if (data == true)
                ShowMessage('Records Deleted Succesfully!', "Success", "success", true);
            else
                ShowMessage('Unable to delete Old records!', "Warning", "warning", true);
            onCheckFilters();
            //$('.hidePopUp').hide();
            $.validationEngine.closePrompt('.formError', true);
            ClearSchedulingPopup();
        },
        error: function(msg) {
        }
    });
};

var SetSelectedViewAsWeek = function() {
    var treeView = $("#treeviewCalenderType").data("kendoTreeView");
    treeView.select($());
    $('#treeviewCalenderType li[data-id="2"]').find('span').addClass('k-state-selected');
    $('#hidSelectedView').val('2');
    $('#hidSelectedViewName').val('Week');
};

var OnChangeAppointmentDate = function(obj, apptTypeId) {
    
    var selectedDate = $("#date" + apptTypeId).val(); //obj;
    var facilityId = $("#ddFacility").val();
    var physicianId = $("#ddPhysician").val();
    var typeOfProcedure = apptTypeId;
    var jsonData = { facilityId: facilityId, physicianId: physicianId, dateselected: selectedDate, typeofproc: typeOfProcedure };
    $.ajax({
        type: "POST",
        url: '/FacultyTimeslots/GetAvailableTimeSlots',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function(data) {
            
            var _ctrl;
            if (data.avialableTimeslotListing[0].TimeSlot == 0) {
                _ctrl = $("#ulTimeSlots");
                _ctrl.html('');
                _ctrl.html("<li class='alert alert-danger'>Time slots are not available for the selected date</li>");
                //$('#divAvailableTimeSlots').addClass('moveLeft2');
            } else {
                _ctrl = $("#ulTimeSlots");
                var html = '';
                for (var i = 0; i < data.avialableTimeslotListing.length; i++) {
                    html += '<li id="' + data.avialableTimeslotListing[i].TimeSlot + '" dopd="' + data.deptOpeningDays + '" onclick="SelectTimeSlot(this,' + apptTypeId + ')">' + data.avialableTimeslotListing[i].TimeSlot + '</li>';
                }
                _ctrl.html('');
                _ctrl.html(html);
                //$('#divAvailableTimeSlots').addClass('moveLeft2');
            }
        },
        error: function(msg) {
        }
    });
};

var SelectTimeSlot = function(e, aTypeId, deptOpnDays) {
    
    var timeSlot = $(e)[0].id.split('-');
    $("#timef" + aTypeId).val(timeSlot[0]);
    $("#timet" + aTypeId).val(timeSlot[1]);
    $('#divAvailableTimeSlots').removeClass('moveLeft2');
    $("#chkIsRec" + aTypeId).prop("checked", false);
    $("#btnEditRecurrence" + aTypeId).hide();
    var deptOpeningDays = $(e).attr("dopd").split(",");
    for (var i = 0; i < deptOpeningDays.length; i++) {
        $("input[name='week_day'][value=" + deptOpeningDays[i] + "]").prop("disabled", false);
        $("input[name='week_day'][value=" + deptOpeningDays[i] + "]").prop("title", "");
        $("#ddMonthWeekDays option[value=" + deptOpeningDays[i] + "]").prop('disabled', false);
        $("#ddMonthWeekDays option[value=" + deptOpeningDays[i] + "]").prop('title', "");
    }
    //startChange("#timef" + aTypeId, "#timet" + aTypeId, parseInt($("#hfTimeSlot").val()));
}; /*Method is used to call on IsReccurrence checkbox change under Appointment types list*/

var OnChangeIsReccurrenceChk = function(e, appTypeId) {
    
    var ctrl = $(e);
    if (!ctrl[0].checked && ctrl.attr("type") == "checkbox") {
        $("#btnEditRecurrence" + appTypeId).hide();
        return false;
    } else {
        $("#btnEditRecurrence" + appTypeId).show();
    }
    if (ctrl.attr("statusattr") == "edit") {
        var rpattern = ctrl.attr("rec_pattern");
        var rtype = ctrl.attr("rec_type");
        var endBy = ctrl.attr("end_By");
        //if (ctrl[0].checked) {
        $("#divReccurrencePopup .popup_frame").addClass("moveLeft");
        $("#hfRecAppTypeId").val(appTypeId);
        if (rpattern.indexOf("day") > -1) {
            var dayValue = rpattern.split('_')[1];
            $("#rbtnDailyRecType").prop("checked", true);
            $("#txtRecEveryDay").val(dayValue);
            $("#txtRecEndByDate").val(endBy);
            $(".reccurrence_field").hide();
            $("#rbtnEveryDay").prop("checked", true);
            $("#daily").show();
        } else if (rpattern.indexOf("week") > -1) {
            var weekPatternObj = rpattern.split('_');
            var weekPatternDaysObj = weekPatternObj[4].split(",");
            if (weekPatternDaysObj.length > 0) {
                for (var u = 0; u < weekPatternDaysObj.length; u++) {
                    $('tr input[type=checkbox][value=' + weekPatternDaysObj[u] + ']').prop("checked", true);
                }
            }
            $("#rbtnWeeklyRecType").prop("checked", true);
            $("#txtRecEveryWeekDays").val(weekPatternObj[1]);
            $("#txtRecEndByDate").val(endBy);
            $(".reccurrence_field").hide();
            $("#weekly").show();
        } else {
            var monthPatternObj = rpattern.split('_');
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
            $("#ddMonthWeekDays option[value=" + deptOpeningDays[i] + "]").prop('disabled', false);
            $("#ddMonthWeekDays option[value=" + deptOpeningDays[i] + "]").prop('title', "");
        }
    }
    $("#rbtnAllEndBy").prop("checked", true);
};

var DoneApppointTypeReccurrence = function() {
    
    $.each(apptRecArray, function(i, val) {
        
        $.each(val, function(key, name) {
            
            if (name == $("#hfRecAppTypeId").val()) {
                apptRecArray.splice(i, 1);
            }
        });
        return false; //return false is used to break the loop
    });
    var recObject = new Object();
    recObject.appoint_Type_Id = $("#hfRecAppTypeId").val();
    //var recTypeObj = $("input[name='rbtnRecType']");
    var recTypeValue = $("input[name='rbtnRecType']:checked").val();
    //for (var i = 0; i < recTypeObj.length ; i++) {
    //    if (recTypeObj[i].checked) {
    //        recTypeValue = recTypeObj[i].value;
    //    }
    //}
    // 
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
        recObject.Frequency_Type = 2; //2 is for weekly selected radio button
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
            recObject.Rec_Start_Date = date; //new Date(year, month, parseInt($("#txtRepeatMonthDay").val()));
            recObject.Frequency_Type = 3; //3 is for Monthly selected radio button & Repeat selected radio button
            break;
        case "2":
            var recStartDateMonthly = new Date(apptStartDate);
            recStartDateMonthly.setDate(recStartDateMonthly.getDate());
            recStartDateMonthly.setHours(hours);
            recStartDateMonthly.setMinutes(minutes);
            recObject.Rec_Pattern = "month_" + $("#ddOnEveryMonth").val() + "_" + $("#ddMonthWeekDays").val() + "_" + $("#ddOnRepeatMonth").val() + "_";
            recObject.Rec_Type = "month_" + $("#ddOnEveryMonth").val() + "_" + $("#ddMonthWeekDays").val() + "_" + $("#ddOnRepeatMonth").val() + "_#";
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
    apptRecArray.push(recObject);
    CancelApppointTypeReccurrence();
};

var GetRecuranceObject = function() {
    //var recuranceObject = 
};

var CalculateMilliSecondsBetweenDates = function(startdate, enddate) {
    var fromTime = new Date(startdate);
    var toTime = new Date(enddate);
    var differenceTravel = toTime.getTime() - fromTime.getTime();
    var seconds = Math.floor((differenceTravel) / (1000));
    return seconds;
};

var CancelApppointTypeReccurrence = function() {
    $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
    if ($("#hidEventParentId").val() == "") {
        //$("#chkIsRec" + $("#hfRecAppTypeId").val()).prop("checked", false);
        $("#hfRecAppTypeId").val("0");
        $("#txtRecEndByDate").val("");
        $("#txtRecEveryDay").val("1");
    }
};

function SearchSchedulingPatient() {
    /// <summary>
    ///     Searches the patient.
    /// </summary>
    /// <returns></returns>
    var isvalidSearch = ValidSechulingSearch();
    if (isvalidSearch) {
        var contactnumber = $("#txtMobileNumber").val() != '' ? $('#lblCountryCode').html() + '-' + $("#txtMobileNumber").val() : "";
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
            url: '/FacultyTimeslots/GetPatientInfoSchedulingSearchResult',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function(data) {
                $('#divSearchResult').empty();
                $('#divSearchResult').html(data);
                $('#collapseSerachResult').addClass('in');
                $('#collapsePatientSearch').removeClass('in');
                SetGridPaging('?', '?PatientID=' + 0 + '&PersonLastName=' + $("#txtLastName").val() + '&PersonEmiratesIDNumber=' + $("#txtEmiratesNationalId").val()
                    + '&PersonPassportNumber=' + $("#txtPassportnumber").val() + '&PersonBirthDate=' + $("#txtBirthDate").val() + '&ContactMobilePhone=' + $("#txtMobileNumber").val() + '&');
            },
            error: function(msg) {
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
    $('#ValidatePatientSearch input[type=text]').each(function() {
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
var ViewPatientScheduling = function(pid, pfirstname, plastname) {
    apptRecArray = []; //Clear reccurrence array
    $('#hidPatientId').val(pid);
    $('#spnSelectedPatient').html(pfirstname + " " + plastname);
    var checkedPhysician = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysician);
    checkedNodeIds1(treeViewStatus.dataSource.view(), checkedStatusNodes);
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
        SelectedDate: $('#divDatetimePicker').val(),
        Facility: $('#hidFacilitySelected').val(),
        ViewType: $('#hidSelectedView').val(),
        DeptData: deptData,
        PatientId: pid
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/FacultyTimeslots/GetPatientScheduling",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function(data) {
            SchedulerInit(data.mainList, physicianData, '1');
            $('#ValidatePatientSearch').clearForm();
            BuildPatientSchedulingView(data.patientNextAppointments);
            if (pid > 0) {
                $('#btnClearPatient').show();
            }
        },
        error: function(msg) {
        }
    });
};

var OnClickRecurrenceTypeBtn = function(e) {
    
    var ctrl = $(e)[0];
    var aptDate = $("#date" + $("#hfRecAppTypeId").val()).val();
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
};

var OnchangeRecurrenceCtrl = function(e) {
    
    var ctrl = e;
    var aptDate = $("#date" + $("#hfRecAppTypeId").val()).val();
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
};

var blockRecurrenceDiv = function(text) {
    $("#pLoadingText").html(text);
    $("#loader_event").show();
};

var unBlockRecurrenceDiv = function() {
    $("#pLoadingText").html();
    $("#loader_event").hide();
};

function waitSeconds(iMilliSeconds) {
    var counter = 0,
        start = new Date().getTime(),
        end = 0;
    while (counter < iMilliSeconds) {
        end = new Date().getTime();
        counter = end - start;
    }
}

var BuildPatientSchedulingView = function(data) {
    var htmlHeaderRow = '<table class="table table-grid" id="PatientScheduledAppointment"><thead><tr class="gridHead"><th scope="col">Patient Name</th><th scope="col">Appointment Type</th><th scope="col">Physician Name</th><th scope="col">Physician Speciality</th><th scope="col">Physician Department</th><th scope="col">Appointment Date</th><th scope="col">Appointment Time Slot</th><th scope="col">Action</th></tr></thead><tbody>';
    var pagehtml = htmlHeaderRow;
    $('#divShowPatientNextAppointment').show();
    if (data.length > 0) {
        scheduler.updateView(new Date($('#divDatetimePicker').val()), "month");
        $.each(data, function(i, obj) {
            var startDate = new Date(obj.start_date);
            var endDate = new Date(obj.end_date);
            var startHours = startDate.getHours().toString().length > 1 ? startDate.getHours().toString() : "0" + startDate.getHours().toString();
            var startMins = startDate.getMinutes().toString().length > 1 ? startDate.getMinutes().toString() : "0" + startDate.getMinutes().toString();
            var endHours = endDate.getHours().toString().length > 1 ? endDate.getHours().toString() : "0" + endDate.getHours().toString();
            var endMins = endDate.getMinutes().toString().length > 1 ? endDate.getMinutes().toString() : "0" + endDate.getMinutes().toString();
            var timeslot = startHours + ':' + startMins + " - " + endHours + ':' + endMins;
            pagehtml += '<tr class="gridRow"><td class="col1"><span>' + obj.PatientName + '</span></td><td class="col2"><span>' + obj.AppointmentTypeStr + '</span></td> <td class="col3"><span>' + obj.PhysicianName + '</span></td><td class="col4">' + obj.PhysicianSpecialityStr + '</td><td class="col5"> <span>' + obj.DepartmentName + '</span></td><td class="col7">' + startDate.toLocaleDateString() + '</td><td class="col8 align-center">' + timeslot + '</td><td class="col10"><span><a title="Select Patient" style="width: 15px; margin-right: 7px; float: left;" schId="' + obj.id + '" onclick="return EditScheduling(this);"  href="javascript:void(0);"><img src="/images/edit.png"></a></span></td></tr>';;
            $('#spnPatientName').html(obj.PatientName);
        });
        $('#patientAppointmentTypes').empty().html(pagehtml);
    } else {
        $('#patientAppointmentTypes').empty().html('<h2>No Record found</h2>');
    }
    $('#collapseSerachResult').removeClass('in');

    $('#collapsePatientScheduledAppointments').addClass('in');
};

var EditScheduling = function(e) {
    var ctrl = $(e);
    var schedulingEventId = ctrl.attr('schId');
    HidepopupObject('divSearchPatient');
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

var HidepopupObject = function(divid) {
    $('#' + divid).hide();
    $.validationEngine.closePrompt('.formError', true);
    $('#divShowPatientNextAppointment').hide();
};

function LightBoxCode(ev) {
    
    $('html, body').css({
        'overflow': 'hidden',
        'height': '100%'
    });
    $(".tooltip").hide();
    $(".dhx_cal_light").hide(); //To hide the light box.
    $('.dhx_cal_cover').attr('style', 'z-index:0');
    $('.dhx_cal_cover').hide();
    if (firstTimeBind) {
        firstTimeBind = false;
        ajaxStartActive = false;
        var eventDetails = scheduler.getEvent(ev);
        schedularEventObj = ev;
        if (eventDetails.EventParentId != null) {
            $('#hidSchedulingType').val(eventDetails.SchedulingType);
            ShowLightBoxStyle(eventDetails.SchedulingType);
            $("#divPhysicianComment").show();
            BindLightBox(eventDetails);
        } else {
            $('#divSchedularPopUp').show();
            $("#btnSaveSchedulingData").val("Save");
        }
        $(".search").autocomplete({
            autoFocus: true,
            minLength: 3, // only start searching when user enters 3 characters
            source: function(request, response) {
                ajaxStartActive = false;
                $.ajax({
                    url: "/PatientInfo/GetPatientInfoByPatientName",
                    type: "POST",
                    dataType: "json",
                    data: {
                        patientName: $("#patientname").val()
                    },
                    success: function(data) {
                        
                        $("#imgPatientExist").hide();
                        $("#imgPatientNew").show();
                        $("#hfPatientId").val("0");
                        response(jQuery.map(data, function(item) {
                            
                            var dateString = item.PersonBirthDate.substr(6);
                            var currentTime = new Date(parseInt(dateString));
                            var month = currentTime.getMonth() + 1;
                            var day = currentTime.getDate();
                            var year = currentTime.getFullYear();
                            var date = month + "/" + day + "/" + year;
                            return {
                                person_email_id: item.PersonEmailAddress,
                                perosn_first_name: item.PersonFirstName,
                                person_last_name: item.PersonLastName,
                                person_phone_number: item.PatientPhone[0].PhoneNo,
                                person_id: item.PatientID,
                                person_dob: item.PersonBirthDate,
                                person_emirates_national_id: item.PersonEmiratesIDNumber,
                                label: item.PersonFirstName + ' ' + item.PersonLastName + ' (' + date + ')',
                                value: item.PersonFirstName + ' ' + item.PersonLastName,
                            };
                        }));
                    }
                });
            },
            select: function(event, ui) {
                ajaxStartActive = false;
                $("#email").val(ui.item.person_email_id);
                $("#phoneno").val(ui.item.person_phone_number);
                FormatMaskedPhone('#lblPatientCountryCode', "#ddlPatientCountries", "#phoneno");
                $("#patientname").val(ui.item.perosn_first_name + ' ' + ui.item.person_last_name);
                $("#hfPatientId").val(ui.item.person_id);
                /*Convert json date to date format - start*/
                var dateString = ui.item.person_dob.substr(6);
                var currentTime = new Date(parseInt(dateString));
                var month = currentTime.getMonth() + 1;
                var day = currentTime.getDate();
                var year = currentTime.getFullYear();
                var date = month + "/" + day + "/" + year;
                /*Convert json date to date format - end*/
                $("#txtPersonDOB").val(date);
                $("#txtEmiratesNationalId").val(ui.item.person_emirates_national_id);
                $("#imgPatientExist").show();
                $("#imgPatientNew").hide();
                BindPatientEncounterList(ui.item.person_id);
                return false;
            },
            change: function(event, ui) {
                ajaxStartActive = false;
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
    scheduler.addEventNow();
}

var UncheckAllCheckBox = function(divId) {
    $('#' + divId + ' input:checkbox').removeAttr('checked');
    SetSelectedViewAsWeek();
    switch (divId.toLowerCase()) {
    case 'treeviewfacilitydepartment':
        var treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");
        UncheckCheckedNode(treeViewDepartments.dataSource.view());
        onCheckDepartment();
        break;
    case 'treeviewphysician':
        var treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");
        UncheckCheckedNode(treeViewPhysician.dataSource.view());
        onCheckFilters();
        break;
    case 'treeviewstatuscheck':
        var treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");
        UncheckCheckedNode(treeViewStatus.dataSource.view());
        onCheckFilters();
        break;
    case 'treeviewfacilityrooms':
        var treeViewRooms = $("#treeviewFacilityRooms").data("kendoTreeView");
        UncheckCheckedNode(treeViewRooms.dataSource.view());
        onCheckFilters();
        break;
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

function onCheckRooms() {
    var checkedDepartments = [],
        treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    var checkedPhysicianNodes = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    var checkedRoom = [],
        treeViewRoom = $("#treeviewFacilityRooms").data("kendoTreeView");

    checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysicianNodes);
    checkedNodeIds1(treeViewDepartments.dataSource.view(), checkedDepartments);
    checkedNodeIds1(treeViewStatus.dataSource.view(), checkedStatusNodes);
    checkedNodeIds1(treeViewRoom.dataSource.view(), checkedRoom);

    BindSchedularWithFilters(checkedPhysicianNodes, checkedStatusNodes, checkedDepartments, checkedRoom);
}