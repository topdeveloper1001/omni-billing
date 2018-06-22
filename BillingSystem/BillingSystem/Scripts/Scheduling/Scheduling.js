//.................. Local variables to be used in the schedular
var firstTimeLoad = false;
var firstTimeSave = true;
var firstTimeBind = true;
var apptTypeIdArray = [];
var apptRecArray = [];
var schedularEventObj;

$(function () {
    /// <summary>
    /// s this instance.
    /// </summary>
    /// <returns></returns>

    //***************************************** Controls Initilization starts******************************//
    $("#divDatetimePicker").datepicker({
        firstDay: 0,
        numberOfMonths: [2, 1],
        onSelect: function () {
            onCheckFilters();
        }
    });
    $("#eventFromDate").datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });
    //$("#eventFromTime").kendoTimePicker({
    //    interval: 15,
    //    format: "HH:mm"
    //});
    $('.main_content').scroll(function () {
        $.validationEngine.closePrompt(".formError", true);
    });
    $("#eventToDate").datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });
    //$("#eventToTime").kendoTimePicker({
    //    interval: 15,
    //    format: "HH:mm"
    //});
    $(".EmiratesMask").mask("999-99-9999");
    $("#txtPersonDOB").datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
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
    //***************************************** Controls Initilization Ends ******************************//

    // ************************************ Page/Div/Button events starts******************************************//
    $('.searchHitMe').on('click', function (e) {
        //function to toggle previous visit list in & out
        $('.searchSlide').toggleClass('moveLeft');
    });

    //search patient using the same screen as the patient search screen
    $('#btnSearchPatient').on('click', function () {
        PatientSearchPopupOpen();
        $('#divSearchPatient').show();
    });

    //......Schedule Appointment button which enable the pop up to create the scheduling events
    $('#btnScheduleAppointment').on('click', function () {
        BindFacility("#ddFacility");
        BindGlobalCodesWithValue("#ddSpeciality", 1121, "");
        BindGlobalCodesWithValueOrderBy("#ddMonthWeekDays", 901, "");
        BindCountryDataWithCountryCode("#ddlPatientCountries", "#hfPatientCountry", "#lblPatientCountryCode");
        $('#hidSchedulingType').val('1');
        $(".modal-title").html("Appointments Scheduler");
        ShowLightBoxStyle("1");//1 means schedule appointment
        scheduler.addEventNow();
    });

    //......Holiday appointment button which enable the pop up to create the Holiday events
    $('#btnAddHoliday').on('click', function () {
        $('#hidSchedulingType').val('2');
        $(".modal-title").html("Holiday/Vacations Scheduler");
        ShowLightBoxStyle("2");//2 means Holiday
        scheduler.addEventNow();
    });

    $('#chkViewAvialableSlots').on('change', function (e) {
        if ($('#chkViewAvialableSlots').prop('checked')) {
            $('#chkViewVocationHolidays').prop('checked', false);
            GetSchedularCustomData('1');
        }
    });

    $('#chkViewVocationHolidays').on('change', function (e) {
        if ($('#chkViewVocationHolidays').prop('checked')) {
            $('#chkViewAvialableSlots').prop('checked', false);
            GetSchedularCustomData('3');
        }
    });

    $('input[name=rdbtnSelection]').on('change', function () {
        //var selected = $('input[name=rdbtnSelection]:checked').val();
        //if (selected == "2") {
        //    $('#divTypeofProc').hide();
        //} else { $('#divTypeofProc').show(); }
    });

    // Save scheduling button event
    $('#btnSaveSchedulingData').on('click', function () {
        var schedulingType = $('#hidSchedulingType').val();
        if (schedulingType === "1") {
            var isdivSchedulerDataValid = jQuery("#divSchedulerData").validationEngine({ returnIsValid: true });
            var isdivAppointmentTypeProceduresValid = jQuery("#divAppointmentTypeProcedures").validationEngine({ returnIsValid: true });
            if (isdivSchedulerDataValid && isdivAppointmentTypeProceduresValid) {
                blockRecurrenceDiv("Saving...");
                SaveCustomSchedular();
            }
        } else {
            CheckTwoDates($('#eventFromDate'), $('#eventToDate'), 'eventToDate');
            var isdivHolidayValid = jQuery("#divHoliday").validationEngine({ returnIsValid: true });
            if (isdivHolidayValid) {
                blockRecurrenceDiv("Saving...");
                SaveHolidaySchedular();
                scheduler.cancel_lightbox();
            }
        }
        firstTimeBind = true;
    });

    // Cancel scheduling button event
    $('#btnCancelSchedulingData').on('click', function () {
        blockRecurrenceDiv("Cancelling...");
        $('#hfAppointmentTypes').val('');
        scheduler.cancel_lightbox();
        $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
        $.validationEngine.closePrompt('.formError', true);
        firstTimeLoad = true;
        firstTimeBind = true;
        onCheckFilters();
        ClearSchedulingPopup();
    });

    // Delete scheduling button event
    $('#btnDeleteSchedulingData').on('click', function () {
        //$("#loader_event").show();
        blockRecurrenceDiv("Deleting...");
        firstTimeBind = true;
        var eventParentId = $("#hidEventParentId").val();
        var schedulingId = $("#hfSchedulingId").val();
        var schType = $('#hidSchedulingType').val();
        DeleteSchduling(eventParentId, schedulingId, schType);
    });
    // ************************************ Button click events ******************************************//

    BindAppointmentAvailability();
    BindLocations();
    BindCorporatePhysician();
    BindSchedulingStatus();
    //BindFacility("#ddFacility");
    //BindGlobalCodesWithValue("#ddSpeciality", 1121, "");
    //BindGlobalCodesWithValueOrderBy("#ddMonthWeekDays", 901, "");

    GetSchedularData();
    //
    //BindCountryDataWithCountryCode("#ddlPatientCountries", "#hfPatientCountry", "#lblPatientCountryCode");
});

/// <var>The bind locations</var>
var BindLocations = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/scheduling/GetFaciltyListTreeView",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $('#divLocationList').empty();
            $('#divLocationList').html(data);
            BindFacilityDepartments();
        },
        error: function (msg) {
        }
    });
};

var BindCorporatePhysician = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Scheduling/GetCorporatePhysicians",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({
            corporateId: '',
            facilityId: $('#hidFacilitySelected').val()
        }),
        success: function (data) {
            $('#divPhysicianList').empty();
            $('#divPhysicianList').html(data);
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
            ggcValue: '4903'
        }),
        success: function (data) {
            /*Bind Availability dropdown - start*/
            var appointmentHTML = '<option value="0">--Select--</option>';
            for (var i = 0; i < data.gClist.length; i++) {
                appointmentHTML += '<option value="' + data.gClist[i].GlobalCodeValue + '">' + data.gClist[i].GlobalCodeName + '</option>'; // Availability -  Field
            }
            $("#ddAvailability").html(appointmentHTML);
            $("#ddAvailability").val("1");// To select "Initial Booking" by default
            /*Bind Availability dropdown - end*/

            /*Bind Physician dropdown - start*/
            appointmentHTML = '<option value="0">--Select--</option>';
            for (var j = 0; j < data.physicians.length; j++) {
                appointmentHTML += '<option value="' + data.physicians[j].Physician.Id + '" facilityId="' + data.physicians[j].Physician.FacilityId + '" department="' + data.physicians[j].UserDepartmentStr + '" departmentId="' + data.physicians[j].Physician.FacultyDepartment + '" speciality="' + data.physicians[j].UserSpecialityStr + '" specialityId="' + data.physicians[j].Physician.FacultySpeciality + '">' + data.physicians[j].Physician.PhysicianName + '(' + data.physicians[j].UserTypeStr + ')</option>'; // Availability -  Field
            }
            $("#ddPhysician").html(appointmentHTML);
            $("#ddHolidayPhysician").html(appointmentHTML);/*Bind holiday's physician drop down*/
            /*Bind Physician dropdown - end*/

            /*Bind holiday status dropdown - start*/
            appointmentHTML = '<option value="0">--Select--</option>';
            for (var k = 0; k < data.hStatus.length; k++) {
                appointmentHTML += '<option value="' + data.hStatus[k].GlobalCodeValue + '">' + data.hStatus[k].GlobalCodeName + '</option>';
            }
            $("#ddHolidayStatus").html(appointmentHTML);
            $("#ddHolidayStatus").val("7");// To select "Initial Booking" by default
            /*Bind holiday status dropdown - end*/

            /*Bind holiday types dropdown - start*/
            appointmentHTML = '<option value="0">--Select--</option>';
            for (var x = 0; x < data.hTypes.length; x++) {
                appointmentHTML += '<option value="' + data.hTypes[x].GlobalCodeValue + '">' + data.hTypes[x].GlobalCodeName + '</option>';
            }
            $("#ddHolidayType").html(appointmentHTML);
            /*Bind holiday types dropdown - end*/
        },
        error: function (msg) {
        }
    });
};

/// <var>The bind scheduling status</var>
var BindSchedulingStatus = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Scheduling/GetGlobalCodesCheckListView",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({
            ggcValue: '4903'
        }),
        success: function (data) {
            $('#divStatusList').empty();
            $('#divStatusList').html(data);
        },
        error: function (msg) {
        }
    });
};

var BindFacilityDepartments = function () {
    var jsonData = {
        facilityid: $('#hidFacilitySelected').val(),
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/scheduling/LoadFacilityDepartmentData",
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify(jsonData),
        success: function (data) {
            $('#divFacilityDepartmentList').empty();
            $('#divFacilityDepartmentList').html(data);
        },
        error: function (msg) {
        }
    });
}

var onCheckView = function (e) {
    var node = e.node;
    var selectedViewname = this.text(e.node);
    var selectedViewValue = $(node).closest("li").data("id");
    $('#hidSelectedView').val(selectedViewValue);
    $('#hidSelectedViewName').val(selectedViewname);
    onCheckFilters();
}

function onCheckLocation(e) {
    $('#spnSelectedPatient').html('');
    var node = e.node;
    var locationname = this.text(e.node);
    var nodeValueId = $(node).closest("li").data("id");
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
}

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

function onCheckFilters() {
    //        
    $('#spnSelectedPatient').html('');
    apptRecArray = [];//Clear reccurrence array
    var checkedPhysician = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    var checkedDepartments = [],
        treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");

    checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysician);
    checkedNodeIds1(treeViewStatus.dataSource.view(), checkedStatusNodes);
    checkedNodeIds1(treeViewDepartments.dataSource.view(), checkedDepartments);

    BindSchedularWithFilters(checkedPhysician, checkedStatusNodes, checkedDepartments);
}

var GetSchedularData = function () {
    setTimeout(LoadSchedulngData(), 500);
}

var LoadSchedulngData = function () {
    var jsonData = {
        selectedDate: $('#divDatetimePicker').val(),
        facility: $('#hidFacilitySelected').val(),
        viewType: '1'
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Scheduling/LoadSchedulngData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            //SchedulerInit1(data.savedSlots, null, '1');
            SchedulerInit(data.savedSlots, data.selectedPhysicians, '1');
            //AddDepartmentTimming(data.departmentTimmingsList);
        },
        error: function (msg) {
        }
    });
};

/// <var>The bind schedular with filters</var>
var BindSchedularWithFilters = function (physicianIds, statusfilter, deptIds) {
    var jsonData = [];
    var physicianData = [];
    var statusData = [];
    var deptData = [];
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
    jsonData[0] = {
        PhysicianId: physicianData,
        StatusType: statusData,
        SelectedDate: $('#divDatetimePicker').val(),
        Facility: $('#hidFacilitySelected').val(),
        ViewType: $('#hidSelectedView').val(),
        DeptData: deptData,
        PatientId: $('#hidPatientId').val()
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Scheduling/GetSchedularWithFilters",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            //scheduler.clearAll();
            //        
            SchedulerInit(data, physicianData, '1');
        },
        error: function (msg) {
        }
    });
};

function checkedNodeIds1(nodes, checkedNodes1) {
    /// <summary>
    /// Checkeds the node ids1.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    /// <param name="checkedNodes1">The checked nodes1.</param>
    /// <returns></returns>
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].checked) {
            checkedNodes1.push(nodes[i].id);
        }
        if (nodes[i].hasChildren) {
            checkedNodeIds1(nodes[i].children.view(), checkedNodes1);
        }
    }
}

function SchedulerInit(jsonData, physicianData, type) {
    /// <summary>
    /// Schedulers the initialize.
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
        { key: '0', label: "---Select---" },
        { key: '1', label: "Available/Open" },
        { key: '2', label: "Initial Booking " },
        { key: '3', label: "Confirmed" },
        { key: '4', label: "Lunch/Meetings/Adm" },
        { key: '5', label: "Holiday" },
        { key: '6', label: "Cancel" }
    ];

    scheduler.attachEvent("onViewChange", function (new_mode, new_date) {
        var minDate = scheduler.getState().min_date;

    });

    scheduler.templates.event_class = function (start, end, event) {
        var css = "";
        if (event.Availability) // if event has subject property then special class should be assigned
        {
            if (event.Availability == 1) {
                css = "patient_initial_booking";
            } else if (event.Availability == 2 || event.Availability == 3 || event.Availability == 7 || event.Availability == 8) {
                css = "patient_confirmed";
            } else if (event.Availability == 4 || event.Availability == 9 || event.Availability == 10) {
                css = "patient_cancelled_notrefilled";
            } else if (event.Availability == 5) {
                css = "patient_expired_notbooking";
            } else if (event.Availability == 6) {
                css = "patient_lunch_meeting_adm";
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
                        "icon_details"
    ];
    scheduler.templates.tooltip_text = function (start, end, event) {
        return "<h4> " + event.text + "</h4>" +
            "<p><b>Scheduled Date: </b> " + start.toLocaleDateString() + "</p>" +
            "<p><b>Time Slot: </b>" + format(start) + " - " + format(end) + "</p>" +
            "<p class='event_description'><b>Description: </b>" + event.text + "</p>";
    };
    scheduler.locale.labels.confirm_deleting = null;
    scheduler.attachEvent("onBeforeDrag", function (id) {
        return false;
    });
    scheduler.attachEvent("onBeforeLightbox", function (ev) {
        
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
    scheduler.attachEvent("onLightbox", function (ev) {
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
                source: function (request, response) {
                    ajaxStartActive = false;
                    $.ajax({
                        url: "/PatientInfo/GetPatientInfoByPatientName",
                        type: "POST",
                        dataType: "json",
                        data: {
                            patientName: $("#patientname").val()
                        },
                        success: function (data) {
                            
                            $("#imgPatientExist").hide();
                            $("#imgPatientNew").show();
                            $("#hfPatientId").val("0");
                            response(jQuery.map(data, function (item) {
                                
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
                                }
                            }));
                        }
                    });
                },
                select: function (event, ui) {
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
                change: function (event, ui) {
                    ajaxStartActive = false;
                }
            });
            //$(".dhx_cal_light").attr();//To hide the light box.
            //$('.hidePopUp').show();
            ajaxStartActive = true;
            $("#divSchedulerData").validationEngine();
        }
    });

    scheduler.attachEvent("onEventAdded", function (id, ev) {
        SaveSchedularData(id, ev);
    });

    scheduler.attachEvent("onEventChanged", function (id, ev) {
        $('#hidSchedulingType').val(ev.SchedulingType);
        var datesObj = scheduler.getRecDates(id);
        SaveSchedularData(id, ev);
    });

    scheduler.attachEvent("onBeforeEventDelete", function (id, ev) {
        $(".tooltip").hide();
        if (ev.SchedulingType == "2") {
            $("#divMultipleDeletePopup").show();
            $("#btnDeleteSeries").click(function (e) {
                DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
            });
            $("#btnDeleteOccurrence").click(function (e) {
                $("#rbHolidaySingle").prop("checked", true);
                DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
            });
        } else {
            DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
        }
    });

    scheduler.attachEvent("onEventDeleted", function (id, ev) {
        //DeleteSchedulerEvent(id, ev);
        //DeleteSchduling(ev.EventParentId, ev.TimeSlotId, ev.SchedulingType);
    });

    scheduler.attachEvent("onTemplatesReady", function () {
        scheduler.date.week_unit_start = scheduler.date.week_start;
        scheduler.templates.week_unit_date = scheduler.templates.week_date;
        scheduler.templates.week_unit_scale_date = scheduler.templates.week_scale_date;
        scheduler.date.add_week_unit = function (date, inc) { return scheduler.date.add(date, inc * 7, "day"); }
        scheduler.date.get_week_unit_end = function (date) { return scheduler.date.add(date, 5, "day"); }
        scheduler.config.start_on_monday = true;
        scheduler.config.time_step = 15;
        scheduler.xy.min_event_height = 21;
        scheduler.config.hour_size_px = 88;
        scheduler.config.lightbox_recurring = '';//To remove edit confirmation box
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

var ShowLightBoxStyle = function (schedulingType) {
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
}

function BindFacility(selector) {
    $.ajax({
        type: "POST",
        url: "/Scheduling/GetFacilitiesDropdownDataOnScheduler",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, facility) {
                    items += "<option value='" + facility.Value + "'>" + facility.Text + "</option>";
                });
                $(selector).html(items);
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

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
        success: function (data) {
            
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $("#ddlDepartment").html(items);
                /*$.each(data, function (i, deaprtments) {
                    items += "<option value='" + deaprtments.Value + "'>" + deaprtments.Text + "</option>";
                });
                $("#ddlDepartment").html(items);*/
                $.each(data.phyList, function (i, physician) {
                    
                    items += '<option value="' + physician.Physician.Id + '" facilityId="' + physician.Physician.FacilityId + '" department="' + physician.UserDepartmentStr + '" departmentId="' + physician.Physician.FacultyDepartment + '" speciality="' + physician.UserSpecialityStr + '" specialityId="' + physician.Physician.FacultySpeciality + '">' + physician.Physician.PhysicianName + '(' + physician.UserTypeStr + ')</option>'; // Availability -  Field
                    //items += "<option value='" + physician.Id + "' PhysicianLicenseNumber='" + physician.PhysicianLicenseNumber + "'>" + physician.PhysicianName + "</option>";
                });
                $("#ddPhysician").html(items);
                BindAppointmentTypes();

            }
            else {
            }
        },
        error: function (msg) {
        }
    });
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
        success: function (data) {

            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, appt) {
                    items += "<option value='" + appt.Id + "' timeslot='" + appt.TimeSlot + "'>" + appt.Name + "</option>";
                });
                $("#appointmentTypes").html(items);
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

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
                $.each(data, function (i, physician) {
                    
                    items += '<option value="' + physician.Physician.Id + '" facilityId="' + physician.Physician.FacilityId + '" department="' + physician.UserDepartmentStr + '" departmentId="' + physician.Physician.FacultyDepartment + '" speciality="' + physician.UserSpecialityStr + '" specialityId="' + physician.Physician.FacultySpeciality + '">' + physician.Physician.PhysicianName + '(' + physician.UserTypeStr + ')</option>'; // Availability -  Field
                    //items += "<option value='" + physician.Id + "' PhysicianLicenseNumber='" + physician.PhysicianLicenseNumber + "'>" + physician.PhysicianName + "</option>";
                });
                $("#ddPhysician").html(items);
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

var ClearSchedulingPopup = function () {
    $("#divSchedularPopUpContent").clearForm(true);
    $('#tbList').empty();
    $('#hfAppointmentTypes').val('');
    $("#tbApptTypesList").html('');
    $("#rbSingle").prop("checked", true);
    $("#divPhysicianComment").hide();
    $("#rbHolidaySingle").prop("checked", true);
    $("#divMultipleEventPopup").hide();
    firstTimeBind = true;
    $("#ddHolidayStatus").val("7");// To select "Initial Booking" by default
    $("#ddAvailability").val("1");// To select "Initial Booking" by default
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
    //$('#btnAddAppointmentType').removeAttr('disabled');
}

var SetDepartmentAndSpeciality = function (e) {
    
    var deptt = $(e).find('option:selected').attr("departmentId");
    var spec = $(e).find('option:selected').attr("specialityId");
    var fid = $(e).find('option:selected').attr("facilityId");
    $("#ddSpeciality").val(spec);
    $("#ddFacility").val(fid);
    BindFacilityDepartments();
    $("#ddlDepartment").val(deptt);

};

function OnChangeApptTypes(e) {
    var ts = $(e).find('option:selected').attr("timeslot");
    var value = $(e).find('option:selected').attr("value");
    $("#hfTimeSlot").val(ts);
    apptTypeIdArray.push(value);
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
        change: function (e) {
            startChange("#timef" + apptTypeValue, "#timet" + apptTypeValue, parseInt($("#hfTimeSlot").val()));
        }
    }).data("kendoTimePicker");
    $("#timet" + apptTypeValue).kendoTimePicker({
        interval: parseInt($("#hfTimeSlot").val()),
        format: "HH:mm"
    });
    //$("#aptAvail" + apptTypeValue).html($("#ddAvailability").html());
    var hfApptTypes = $("#hfAppointmentTypes").val();
    var concatApptTypes = hfApptTypes + "," + apptTypeValue;
    $("#hfAppointmentTypes").val(concatApptTypes.replace(/^,|,$/g, ''));

    $("#appointmentTypes").val("0");

    var date = new Date();
    $("#date" + apptTypeValue).val(date.getMonth() + 1 + "/" + date.getDate() + '/' + date.getFullYear());//adding today's as by default
    //var btnSelection = $('input[name=rdbtnSelection]:checked').val();
    //if (btnSelection === "2") {
    //    $('#btnAddAppointmentType').attr('disabled', 'disabled');
    //} else {
    //    $('#btnAddAppointmentType').removeAttr('disabled');
    //}
}

var OnChangeAppointmentDate = function (obj, apptTypeId) {
    
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
        success: function (data) {
            
            var _ctrl = $("#ulTimeSlots");
            var html = '';
            for (var i = 0; i < data.length; i++) {
                html += '<li id="' + data[i].TimeSlot + '" onclick="SelectTimeSlot(this,' + apptTypeId + ')">' + data[i].TimeSlot + '</li>';
            }
            _ctrl.html('');
            _ctrl.html(html);
            $('#divAvailableTimeSlots').addClass('moveLeft2');
        },
        error: function (msg) {
        }
    });
}

function SearchSchedulingPatient() {
    /// <summary>
    /// Searches the patient.
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
            success: function (data) {
                $('#divSearchResult').empty();
                $('#divSearchResult').html(data);
                $('#collapseSerachResult').addClass('in');
                $('#collapsePatientSearch').removeClass('in');
                SetGridPaging('?', '?PatientID=' + 0 + '&PersonLastName=' + $("#txtLastName").val() + '&PersonEmiratesIDNumber=' + $("#txtEmiratesNationalId").val()
                     + '&PersonPassportNumber=' + $("#txtPassportnumber").val() + '&PersonBirthDate=' + $("#txtBirthDate").val() + '&ContactMobilePhone=' + $("#txtMobileNumber").val() + '&');
            },
            error: function (msg) {
            }
        });
    }
}

function ValidSechulingSearch() {
    /// <summary>
    /// Valids the search.
    /// </summary>
    /// <returns></returns>
    var txtvalue = 0;
    $('#ValidatePatientSearch input[type=text]').each(function () {
        if ($(this).val() != "") {
            txtvalue = txtvalue + 1;
        }
    });
    if (txtvalue < 1) {
        ShowMessage("Confirm at least one piece of information", "Alert", "warning", true);
        return false;
    }
    else {
        return true;
    }
    return false;
}

/// <var>The view patient scheduling</var>
var ViewPatientScheduling = function (pid, pfirstname, plastname) {
    apptRecArray = [];//Clear reccurrence array
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
        success: function (data) {
            SchedulerInit(data, physicianData, '1');
            $('#divSearchResult').empty();
            $('#divSearchPatient').hide();
            if (pid > 0) {
                $('#btnClearPatient').show();
            }
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

var SetSelectedViewAsWeek = function () {
    var treeView = $("#treeviewCalenderType").data("kendoTreeView");
    treeView.select($());
    $('#treeviewCalenderType li[data-id="2"]').find('span').addClass('k-state-selected');
    $('#hidSelectedView').val('2');
    $('#hidSelectedViewName').val('Week');
}

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
                html += "<td><input type='button' value='select' onclick='SelectPhysician(" + data[k].Physician.FacilityId + "," + data[k].Physician.Id + "," + data[k].Physician.FacultyDepartment + "," + data[k].Physician.FacultySpeciality + ")'/></td>";
                html += "</tr>";
            }
            $("#tbList").html(html);
        }
    });
}

var SaveCustomSchedular = function () {
    
    var apptRecArray1 = apptRecArray;
    var btnSelection = $('input[name=rdbtnSelection]:checked').val();
    var jsonData = [];
    var selectedAppointmenttype = $('#hfAppointmentTypes').val();
    if (selectedAppointmenttype != "") {
        if (btnSelection == "2") {
            var appointmenttypeArrayVal = selectedAppointmenttype.split(',');
            if (appointmenttypeArrayVal.length > 1) {
                ShowMessage('For multiple Appointment you have to click Multiple procedure at the top!', "Warning", "warning", true);
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
                $.each(apptRecArray, function (k1, val) {
                    $.each(val, function (key, name) {
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
                    RecurringDateTill: isRecurranceMultiple ? recuranceArrayObj.end_By : null,//recuranceArrayObj.end_By, //isRecurranceMultiple ? recuranceArrayObj.end_By : null,
                    //EventId: id,
                    RecType: isRecurranceMultiple ? recuranceArrayObj.Rec_Type : null,//recuranceArrayObj.Rec_Type, // isRecurranceMultiple ? recuranceArrayObj.Rec_Type : null,
                    RecPattern: isRecurranceMultiple ? recuranceArrayObj.Rec_Pattern : null,// recuranceArrayObj.Rec_Pattern, //isRecurranceMultiple ? recuranceArrayObj.Rec_Pattern : null,
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
            url: '/Scheduling/SavePatientScheduling',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonD,
            success: function (data) {
                if (data.IsError) {
                    if (data.notAvialableTimeslotslist.length > 0) {
                        for (var j = 0; j < data.notAvialableTimeslotslist.length; j++) {
                            ShowMessage('Unable to book slot for Date :' + data.notAvialableTimeslotslist[j].DateFromSTR + ' Time Range:' + data.notAvialableTimeslotslist[j].TimeFromStr + ' - ' + data.notAvialableTimeslotslist[j].TimeTOStr + ' With physician' + data.notAvialableTimeslotslist[j].PhysicianName + ' (' + data.notAvialableTimeslotslist[j].PhysicianSpl + ')', "Warning", "warning", true);
                            unBlockRecurrenceDiv();
                        }
                    }
                    else if (data.roomEquipmentnotaviable.length > 0) {
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
            error: function (msg) {
            }
        });
    } else {
        ShowMessage('Please add atleast one appointment!', "Warning", "warning", true);
    }
}