var firstTimeLoad = false;
var firstTimeSave = true;
var firstTimeBind = true;
var apptTypeIdArray = [];
var apptRecArray = [];
var schedularEventObj;
var presentDate = new Date(), y = presentDate.getFullYear(), m = presentDate.getMonth();
var monthFirstDay = new Date(y, m, 1);
var monthLastDay = new Date(y, m + 1, 0);
var html = function (id) { return document.getElementById(id); }; //just a helper


var schedulerUrl = "/Scheduler/";

$(function () {
    $("#divDatetimePicker").datepicker({
        //showWeek: true,
        firstDay: 0,
        numberOfMonths: [2, 1],
        //showButtonPanel: true,
        onSelect: function (dateText, inst) {
            onCheckFilters();
        }
    });
    BindAppointmentAvailability();
    BindPreSchedulingStatus();
    BindLocations();
    PatientSchedulerInit(null);

    GetSchedularData();
    BindCountryDataWithCountryCode("#ddlPatientCountries", "#hfPatientCountry", "#lblPatientCountryCode");
    BindFacility("#ddFacility");
    BindFacility("#selOVFacility");//Overview popup
    BindGlobalCodesWithValue("#ddSpeciality", 1121, "");
    $('#btnScheduleAppointment').on('click', function () {
        $('#hidSchedulingType').val('1');
        $("#divSchedularPopUpContent .modal-title").html("Appointments Scheduler");
        ShowLightBoxStyle("1");//1 means schedule appointment
        scheduler.addEventNow();
    });

    $('#btnSaveSchedulingData').on('click', function () {
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
    $('#btnCancelSchedulingData').on('click', function () {
        blockRecurrenceDiv("Cancelling...");
        //$("#loader_event").show();
        $('#hfAppointmentTypes').val('');
        scheduler.cancel_lightbox();
        //$('.hidePopUp').hide();
        $("#divReccurrencePopup .popup_frame").removeClass("moveLeft");
        $.validationEngine.closePrompt('.formError', true);
        firstTimeLoad = true;
        firstTimeBind = true;
        //onCheckFilters();
        ClearSchedulingPopup();
        //scheduler.endLightbox(false, html("my_form"));
    });
    $('#btnDeleteSchedulingData').on('click', function () {
        //$("#loader_event").show();
        blockRecurrenceDiv("Deleting...");
        firstTimeBind = true;
        var eventParentId = $("#hidEventParentId").val();
        var schedulingId = $("#hfSchedulingId").val();
        var schType = $('#hidSchedulingType').val();
        var externalValue3 = $("#hfExternalValue3").val();
        //DeleteSchduling(eventParentId, schedulingId, schType, externalValue3);
    });

    $('.preDisabled').removeAttr('disabled');// user can not change the status of the scheduling
    /*over view popup*/
    $("#txtOVDateFrom").datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });
    $("#txtOVDateFrom").val(presentDate.getMonth() + 1 + "/" + presentDate.getDate() + '/' + presentDate.getFullYear());
    $("#txtOVTimeFrom").kendoTimePicker({
        interval: 30,
        format: "HH:mm"
    });
    $("#txtOVTimeFrom").val("00:00");
    $("#txtOVDateTo").datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });
    $("#txtOVTimeTo").kendoTimePicker({
        interval: 30,
        format: "HH:mm"
    });
    $("#txtOVTimeTo").val("23:30");


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
});

function PatientSchedulerInit(jsonData) {
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
    scheduler.locale.labels.section_parent = "Availability";

    scheduler.templates.tooltip_text = function (start, end, event) {
        if (event.PatientName != null) {
            return "<h4> " + event.text + "</h4>" +
                "<div class='row'><div class='col-sm-5'><b>Physician Name: </b></div><div class='col-sm-7'>" + event.PhysicianName + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Patient Name: </b></div><div class='col-sm-7'>" + event.PatientName + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Scheduled Date: </b></div><div class='col-sm-7'>" + start.toLocaleDateString() + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Time Slot: </b></div><div class='col-sm-7'>" + format(start) + " - " + format(end) + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Description: </b></div><div class='col-sm-7'><p class='event_description'>" + event.text + "</p></div></div>";
        }
        else {
            return "<h4> " + event.text + "</h4>" +
               "<div class='row'><div class='col-sm-5'><b>Faculty Name: </b></div><div class='col-sm-7'>" + event.PhysicianName + "</div></div>" +
               "<div class='row'><div class='col-sm-5'><b>Scheduled Date: </b></div><div class='col-sm-7'>" + start.toLocaleDateString() + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Time Slot: </b></div><div class='col-sm-7'>" + format(start) + " - " + format(end) + "</div></div>" +
                "<div class='row'><div class='col-sm-5'><b>Description: </b></div><div class='col-sm-7'><p class='event_description'>" + event.text + "</p></div></div>";
        }
    };

    scheduler.templates.event_class = function (start, end, event) {
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
            } else if (event.Availability == 90) {
                css = "patient_pre_scheduling";
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

    scheduler.attachEvent("onLightbox", function (ev) {
        LightBoxCode(ev);
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

    if (firstTimeLoad == false) {
        $("#btnScheduleAppointment").prop("disabled", false);
        scheduler.init('scheduler_here', new Date($('#divDatetimePicker').val()), "day");
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
                scheduler.updateView(new Date($('#divDatetimePicker').val()), "day");
                $('#hidSelectedView').val('1');
                $('#hidSelectedViewName').val('Day');
            break;
        case 'Week':
            scheduler.updateView(new Date($('#divDatetimePicker').val()), "week");
            break;
        case 'Month':
            scheduler.updateView(new Date($('#divDatetimePicker').val()), "month");
            break;
        default:
            scheduler.updateView(new Date($('#divDatetimePicker').val()), "day");
            $('#hidSelectedView').val('1');
            $('#hidSelectedViewName').val('Day');
    }
    scheduler.config.scroll_hour = (new Date).getHours();
}

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
    var startTime = tpFromVal;//start.value();

    if (startTime) {
        startTime = new Date(startTime);
        //tpTo.max(startTime);
        startTime.setMinutes(startTime.getMinutes() + timeInterval);
        tpTo.min(startTime);
        tpTo.value(startTime);
    }
}

/// <var>The get schedular data</var>
var GetSchedularData = function () {
    setTimeout(LoadPatientSchedulngData(), 500);
}

var LoadPatientSchedulngData = function () {
    var jsonData = {
        selectedDate: $('#divDatetimePicker').val(),
        patientId: $('#hdPatientId').val(),
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/PatientSchedulerPortal/LoadPatientSchedulngData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            PatientSchedulerInit(data);
        },
        error: function (msg) {
        }
    });
};


/// <var>The bind corporate physician</var>
var BindCorporatePhysician = function() {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Physician/GetCorporatePhysicians",
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
};

/// <var>The bind scheduling status</var>
var BindPreSchedulingStatus = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/GlobalCode/GetGlobalCodesCheckListViewPreScheduling",
        async: false,
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

/// <var>The bind locations</var>
var BindLocations = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Facility/GetFaciltyListTreeView",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $('#divLocationList').empty();
            $('#divLocationList').html(data);
            //BindFacilityDepartments();
            //BindFacilityRooms();
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
    }
}

var onCheckView = function (e) {
    var node = e.node;
    var selectedViewname = this.text(e.node);
    var selectedViewValue = $(node).closest("li").data("id");
    $('#hidSelectedView').val(selectedViewValue);
    $('#hidSelectedViewName').val(selectedViewname);
    onCheckFilters();
    //switch (selectedViewname) {
    //    case 'Day':
    //        scheduler.updateView(new Date($('#divDatetimePicker').val()), "day");
    //        break;
    //    case 'Week':
    //        scheduler.updateView(new Date($('#divDatetimePicker').val()), "week");
    //        break;
    //    case 'Month':
    //        scheduler.updateView(new Date($('#divDatetimePicker').val()), "month");
    //        break;
    //    default:
    //}
}

function onCheckFilters() {
    apptRecArray = [];//Clear reccurrence array

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    checkedNodeIds1(treeViewStatus.dataSource.view(), checkedStatusNodes);

    BindSchedularWithFilters(checkedStatusNodes);
}

function onCheckLocation(e) {
    var t = $('#treeviewLocations').data('kendoTreeView');
    var node = e.node;
    var locationname = this.text(e.node);
    var nodeValueId = $(node).closest("li").data("id");
    $('#hidFacilityName').val(locationname);
    $('#hidFacilitySelected').val(nodeValueId);
}

var BindSchedularWithFilters = function (statusfilter) {
    blockOnlyDiv("Loading...");
    var jsonData = [];
    var physicianData = [];
    var statusData = [];
    var deptData = [];
    var roomsData = [];
   
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
        StatusType: statusData,
        SelectedDate: $('#divDatetimePicker').val(),
        Facility: $('#hidFacilitySelected').val(),
        ViewType: $('#hidSelectedView').val(),
        PatientId: $('#hdPatientId').val(),
    };

    $.ajax({
        cache: false,
        type: "POST",
        url: "/PatientSchedulerPortal/GetPatientSchedular",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            PatientSchedulerInit(data);
            setTimeout(unBlockOnlyDiv(), 1000);
        },
        error: function (msg) {
        }
    });
};

var blockOnlyDiv = function (text) {
    $("#pLoadingText_1").html(text);
    $("#loader_event_1").show();
}

var unBlockOnlyDiv = function () {
    $("#pLoadingText_1").html();
    $("#loader_event_1").hide();
}

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
        var scrollPosition = -1;
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
            },
        });
        $('#divContent_1').scroll(function () {
            if (scrollPosition != -1) {
                $('#divContent_1').scrollTop(scrollPosition);
            }
        });
        //$(".dhx_cal_light").attr();//To hide the light box.
        //$('.hidePopUp').show();
        ajaxStartActive = true;
        $("#divSchedulerData").validationEngine();
    }
}

var ShowLightBoxStyle = function (schedulingType) {
    switch (schedulingType) {
        case "1":
            $("#divMultipleSingle").show();
            $("#divPhysicianPatient").show();
            $("#divTypeofProc").show();
            $("#divHoliday").hide();
            $("#divDateTime").hide();
            $("#divSchedularPopUp .main_content").css("height", "496px");
            LoadPatientDetails();
            break;
        case "2":
            $("#divMultipleSingle").hide();
            $("#divPhysicianPatient").hide();
            $("#divTypeofProc").hide();
            $("#divHoliday").show();
            $("#divDateTime").hide();
            $("#divSchedularPopUp .main_content").css({ "height": "180px", "margin-top": "0" });

            break;
        default:
            break;
    }
    $("#btnDeleteSchedulingData").hide();
}

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
                $("#btnEditRecurrenceSeries").click(function (e) {
                    
                    $('#divSchedularPopUp').show();
                    $("#divRecurrenceEventPopup").hide();
                    var procObj = obj.TypeOfProcedureCustomModel;
                    for (var k = 0; k < procObj.length; k++) {
                        $("#date" + procObj[k].TypeOfProcedureId).val(procObj[k].Rec_Start_date);
                    }
                });
                $("#btnEditRecurrenceOccurrence").click(function (e) {
                    
                    $("#rbMultiple").prop("checked", false);
                    $("#rbSingle").prop("checked", true);
                    $("#rbMultiple").prop("disabled", true);
                    $('#divSchedularPopUp').show();
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
            BindFacilityDepartmentspopup('#ddFacility');
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
            schedularEventObj = obj.id;
            $("#hidEventParentId").val(obj.EventParentId);
            /*Bind patient section - end*/

            /*Bind appointment types - start*/
            var ctrl = $("#tbApptTypesList");
            var aptObj = obj.TypeOfProcedureCustomModel;
            var html = "";
            $('.preDisabled').attr('disabled', 'disabled');
            //data = jLinq.from(data).orderBy("GlobalCodeValue").select();
            //var customdata = jLinq.from(data)
            //    .where(function(record) { return record.DateSelected == aptObj[0].DateSelected; }).select();
            //
            for (var i = 0; i < aptObj.length; i++) {
                html += "<tr id='tr" + aptObj[i].TypeOfProcedureId + "'><input type='hidden' id='hfMain" + aptObj[i].TypeOfProcedureId + "' value='" + aptObj[i].MainId + "'/>";
                html += "<td>" + aptObj[i].TypeOfProcedureName + "</td><input type='hidden' id='hfProdTimeSlot" + aptObj[i].TypeOfProcedureId + "'  value='" + aptObj[i].TimeSlotTimeInterval + "'/>";
                html += "<td><input id='date" + aptObj[i].TypeOfProcedureId + "' type='text' value='" + aptObj[i].DateSelected + "' class='validate[required]'/><input type='button' id='btnShowTimeSlots'  class='btn btn-sm btn-primary marginLR10' value='View Slots' onclick='OnChangeAppointmentDate(this," + aptObj[i].TypeOfProcedureId + ")'/></td><input type='hidden' id='hfTypeOfProcedureId" + aptObj[i].TypeOfProcedureId + "'  value='" + aptObj[i].ProcedureAvailablityStatus + "'/>";
                html += "<td><input id='timef" + aptObj[i].TypeOfProcedureId + "' type='text' value='" + aptObj[i].TimeFrom + "' class='validate[required]'/></td>";
                html += "<td><input id='timet" + aptObj[i].TypeOfProcedureId + "' type='text' value='" + aptObj[i].TimeTo + "' class='validate[required]'/></td>";
                if (aptObj[i].IsRecurrance) {
                    html += "<td><input type='hidden' id='hfDone" + aptObj[i].TypeOfProcedureId + "' value='1'/><input id='chkIsRec" + aptObj[i].TypeOfProcedureId + "' mon_rpt_date='" + obj.start_date.getDate() + "' freqtypeattr='' statusattr='edit' end_By='" + aptObj[i].end_By + "' dept_opn_days='" + aptObj[i].DeptOpeningDays + "' rec_pattern='" + aptObj[i].Rec_Pattern + "' rec_type='" + aptObj[i].Rec_Type + "' type='checkbox' checked='checked' onchange='OnChangeIsReccurrenceChk(this," + aptObj[i].TypeOfProcedureId + ");'/>" +
                        "<input type='button' mon_rpt_date='" + obj.start_date.getDate() + "' freqtypeattr='' statusattr='edit' end_By='" + aptObj[i].end_By + "' rec_pattern='" + aptObj[i].Rec_Pattern + "' checked='checked' dept_opn_days='" + aptObj[i].DeptOpeningDays + "' rec_type='" + aptObj[i].Rec_Type + "' id='btnEditRecurrence" + aptObj[i].TypeOfProcedureId + "' class='btn btn-xs btn-default' style='margin-left:5px;' value='Edit Recurrence' onclick='OnChangeIsReccurrenceChk(this," + aptObj[i].TypeOfProcedureId + ");'/></td>";
                } else {
                    html += "<td><input type='hidden' id='hfDone" + aptObj[i].TypeOfProcedureId + "' value='0'/><input id='chkIsRec" + aptObj[i].TypeOfProcedureId + "' type='checkbox' freqtypeattr='' value='' onchange='OnChangeIsReccurrenceChk(this," + aptObj[i].TypeOfProcedureId + ");'/>" +
                        "<input type='button' style='display:none;'  id='btnEditRecurrence" + aptObj[i].TypeOfProcedureId + "' class='btn btn-xs btn-default' style='margin-left:5px;' freqtypeattr='' value='Edit Recurrence' statusattr='new' onclick='OnChangeIsReccurrenceChk(this," + aptObj[i].TypeOfProcedureId + ");'/></td>";
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
            //test
            for (var u = 0; u < loop.length; u++) {
                $("#date" + loop[u]).datetimepicker({
                    format: 'm/d/Y',
                    minDate: 0,
                    timepicker: false,
                    closeOnDateSelect: true
                    , onChangeDateTime: function (e) {
                        //OnChangeAppointmentDate(e, loop[u]);
                        //return false;
                        //OnChangeDate(this);
                        $("#timef" + $("#hfAppointmentTypes").val()).val('');
                        $("#timet" + $("#hfAppointmentTypes").val()).val('');
                    }
                });
                $("#timef" + loop[u]).kendoTimePicker({
                    interval: parseInt($("#hfProdTimeSlot" + loop[u]).val()),
                    format: "HH:mm",
                    change: function (e) {
                        
                        startChange("#" + e.sender.element[0].id, "#" + e.sender.element[0].id.replace("f", "t"), parseInt(e.sender.options.interval));//fromtimeId, totimeId, timeinterval
                    }
                }).data("kendoTimePicker").readonly();
                $("#timet" + loop[u]).kendoTimePicker({
                    interval: parseInt($("#hfProdTimeSlot" + loop[u]).val()),
                    format: "HH:mm"
                }).data("kendoTimePicker").readonly();
                //$("#aptAvail" + loop[u]).html($("#ddAvailability").html());
                //$("#aptAvail" + loop[u]).val($("#hfTypeOfProcedureId" + loop[u]).val());
                // startChange("#timef" + loop[u], "#timet" + loop[u], parseInt($("#hfProdTimeSlot" + loop[u]).val()));
            }
            /*Bind appointment types - end*/
            $("#txtEmiratesNationalId").val(obj.EmirateIdnumber);
            break;
        case "2":
            $("#divSchedularPopUpContent .modal-title").html("Holiday/Vacations Scheduler");
            /*Bind radio button - start*/
            if (obj.MultipleProcedure) {
                $("#rbHolidayMultiple").prop("checked", true);
                $("#eventToDate").addClass('validate[required]');
                $("#divHolidayToDateTime").show();
                /*for multiple, first we need to show edit multiple series popup and then on its input show real popup*/
                $("#divMultipleEventPopup").show();
                $("#btnEditSeries").click(function (e) {
                    
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
                $("#btnEditOccurrence").click(function (e) {
                    
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
                $("#eventToDate").removeClass('validate[required]');
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
            $("#ddHolidayPhysician").prop("disabled", true);
            /*Bind main section - end*/
            break;
        default:
            break;
    }
    if (obj.start_date < new Date() && scheduleType == "1") {
        $("#btnSaveSchedulingData").hide();
        $("#btnDeleteSchedulingData").hide();
    }
}

var LoadPatientDetails = function() {
    var pid = $('#hdPatientId').val();
    var jsonData = JSON.stringify({
        patientId: pid,
    });
    $.ajax({
        cache: false,
        type: "POST",
        url: "/PatientSchedulerPortal/GetPatientDetails",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $('#patientname').val(data.PersonFirstName);
            $('#txtPersonDOB').val(data.PersonDateOfBirth);
            $('#email').val(data.PersonEmailId);
            $('#txtEmiratesNationalId').val(data.EmirateId);
            $('#phoneno').val(data.PersonPhoneNumber);
            FormatMaskedPhone('#lblPatientCountryCode', '#ddlPatientCountries', '#phoneno');
        },
        error: function (msg) {
        }
    });
}

function BindFacility(selector) {
    $.ajax({
        type: "POST",
        url: "/Facility/GetFacilitiesDropdownDataOnScheduler",
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
                $(selector).html('');
                $(selector).html(items);
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

var BindAppointmentAvailability = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/GlobalCode/GetGlobalCodesAvailability",
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
            appointmentHTML += '<option value="90">Pre Scheduling</option>';

            $("#ddAvailability").html(appointmentHTML);
            $("#ddAvailability").val("90");// To select "Initial Booking" by default
            /*Bind Availability dropdown - end*/

            /*Bind Physician dropdown - start*/
            appointmentHTML = '<option value="0">--Select--</option>';
            for (var j = 0; j < data.physicians.length; j++) {
                appointmentHTML += '<option value="' + data.physicians[j].Physician.Id + '" facilityId="' + data.physicians[j].Physician.FacilityId + '" department="' + data.physicians[j].UserDepartmentStr + '" departmentId="' + data.physicians[j].Physician.FacultyDepartment + '" speciality="' + data.physicians[j].UserSpecialityStr + '" specialityId="' + data.physicians[j].Physician.FacultySpeciality + '">' + data.physicians[j].Physician.PhysicianName + '(' + data.physicians[j].UserTypeStr + ')</option>'; // Availability -  Field
            }
            $("#ddPhysician").html(appointmentHTML);
            $("#ddHolidayPhysician").html(appointmentHTML);/*Bind holiday's physician drop down*/
            /*Bind Physician dropdown - end*/
        },
        error: function (msg) {
        }
    });
};

function BindFacilityDepartmentspopup(facilityDDId) {
    
    $("#ddSpeciality").val("0");
    $.ajax({
        type: "POST",
        url: "/Physician/GetPhysicianByFacility",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            facilityId: $(facilityDDId).val()
        }),
        success: function (data) {
            
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data.phyList, function (i, physician) {
                    items += '<option value="' + physician.Physician.Id + '" facilityId="' + physician.Physician.FacilityId + '" department="' + physician.UserDepartmentStr + '" departmentId="' + physician.Physician.FacultyDepartment + '" speciality="' + physician.UserSpecialityStr + '" specialityId="' + physician.Physician.FacultySpeciality + '">' + physician.Physician.PhysicianName + '(' + physician.UserTypeStr + ')</option>'; // Availability -  Field
                });
                $("#ddPhysician").html(items);
                BindAppointmentTypes(facilityDDId);
                //BindPatient("#selOVPatient");
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

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
                $.each(data, function (i, appt) {
                    items += "<option value='" + appt.Id + "' timeslot='" + appt.TimeSlot + "'>" + appt.Name + "</option>";
                });
                if (facilityDDId == "#ddFacility") {
                    $("#appointmentTypes").html('');
                    $("#appointmentTypes").html(items);
                } else {
                    $("#selOVApptTypes").html('');
                    $("#selOVApptTypes").html(items);
                }
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
            specialityId: $("#ddSpeciality").val()
        }),
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, physician) {
                    items += '<option value="' + physician.Physician.Id + '" facilityId="' + physician.Physician.FacilityId + '" department="' + physician.UserDepartmentStr + '" departmentId="' + physician.Physician.FacultyDepartment + '" speciality="' + physician.UserSpecialityStr + '" specialityId="' + physician.Physician.FacultySpeciality + '">' + physician.Physician.PhysicianName + '(' + physician.UserTypeStr + ')</option>'; // Availability -  Field
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

var SetDepartmentAndSpeciality = function(e) {
    var deptt = $(e).find('option:selected').attr("departmentId");
    var spec = $(e).find('option:selected').attr("specialityId");
    var fid = $(e).find('option:selected').attr("facilityId");
    $("#ddSpeciality").val(spec);
    $("#ddFacility").val(fid);
    $("#ddlDepartment").val(deptt);
    //------- Code added By Shashank to restrict user to View only selected Physician Appointment types
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

    var html = "";
    html += "<tr id='tr" + apptTypeValue + "'><input type='hidden' id='hfMain" + apptTypeValue + "' value='0'/>";
    html += "<td>" + apptTypeName + "</td><input type='hidden' id='hfProdTimeSlot" + apptTypeValue + "'  value='" + $("#hfTimeSlot").val() + "'/>";
    html += "<td><input id='date" + apptTypeValue + "' type='text' class='validate[required]'/><input type='button' id='btnShowTimeSlots' class='btn btn-sm btn-primary marginLR10' style='height:28px;' value='View Slots' onclick='OnChangeAppointmentDate(this," + apptTypeValue + ")'/></td>";
    html += "<td><input id='timef" + apptTypeValue + "' type='text' class='validate[required]'/></td>";
    html += "<td><input id='timet" + apptTypeValue + "' type='text' class='validate[required]'/><input type='hidden' id='hfDone" + apptTypeValue + "' value='0'/></td>";
    html += "<td class='recurrence_event'><input id='chkIsRec" + apptTypeValue + "' freqtypeattr='' type='checkbox' onchange='OnChangeIsReccurrenceChk(this," + apptTypeValue + ");'/><input type='button' id='btnEditRecurrence" + apptTypeValue + "' class='btn btn-xs btn-default' style='margin-left:5px;display:none;' value='Edit Recurrence' statusattr='new' freqtypeattr='' onclick='OnChangeIsReccurrenceChk(this," + apptTypeValue + ");'/></td>";
    html += "<td><img src='/images/delete.png' width='15px' onclick='RemoveAppointmentProcedures(" + apptTypeValue + ")'/></td>";
    html += "</tr>";
    ctrl.append(html);
   
    $("#date" + apptTypeValue).datetimepicker({
        format: 'm/d/Y',
        minDate: 0,
        timepicker: false,
        closeOnDateSelect: true
        , onChangeDateTime: function (e) {
            //OnChangeAppointmentDate(e, loop[u]);
            //return false;
            $("#timef" + apptTypeValue).val('');
            $("#timet" + apptTypeValue).val('');
        }
    });
    $("#timef" + apptTypeValue).kendoTimePicker({
        interval: parseInt($("#hfTimeSlot").val()),
        format: "HH:mm",
        change: function (e) {
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
    $("#date" + apptTypeValue).val(date.getMonth() + 1 + "/" + date.getDate() + '/' + date.getFullYear());//adding today's as by default
    var initialDate = "";
    if ($("#hfAppointmentTypes").val() != "") {
        var hfAptypesObjValueArray = $("#hfAppointmentTypes").val().split(",");
        initialDate = $("#date" + hfAptypesObjValueArray[0]).val();
        $("#date" + apptTypeValue).val(initialDate);
    }
}

var LoadPhysicianPatientData = function () {
    
    var pid = $('#hdPatientId').val();
    var phyId = $('#hdPreviousEncounterPhysicianId').val();
    $('#ddFacility').val($('#hdFacilityId').val());
    BindFacilityDepartmentspopup('#ddFacility');
    setTimeout(function() {
        $('#ddPhysician').val(phyId);
        SetDepartmentAndSpeciality('#ddPhysician');
    }, 500);
}

var OnChangeAppointmentDate = function (obj, apptTypeId) {
    var selectedDate = $("#date" + apptTypeId).val(); //obj;
    var facilityId = $("#ddFacility").val();
    var physicianId = $("#ddPhysician").val();
    var typeOfProcedure = apptTypeId;
    var jsonData = { facilityId: facilityId, physicianId: physicianId, dateselected: selectedDate, typeofproc: typeOfProcedure };
    $.ajax({
        type: "POST",
        url: schedulerUrl + 'GetAvailableTimeSlots',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            
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
                    html += '<li id="' + data.avialableTimeslotListing[i].TimeSlot + '" dopd="' + data.deptOpeningDays + '" onclick="SelectTimeSlot(this,' + apptTypeId + ',1)">' + data.avialableTimeslotListing[i].TimeSlot + '</li>';
                }
                _ctrl.html('');
                _ctrl.html(html);
                $('#divAvailableTimeSlots').addClass('moveLeft2');
            }
        },
        error: function (msg) {
        }
    });
}

var SelectTimeSlot = function (e, aTypeId, pName) {
    
    if (pName == 2) {
        var facilityId = $(e).attr("attr-fid");
        var physicianId = $(e).attr("attr-pid");
        var sTime = $(e).attr("attr-stime");
        var eTime = $(e).attr("attr-etime");
        var fdate = $(e).attr("attr-selDate");
        $("#ddFacility").val(facilityId);
        BindFacilityDepartmentspopup('#ddFacility');
        $("#ddPhysician").val(physicianId);
        SetDepartmentAndSpeciality($("#ddPhysician"));
        $("#appointmentTypes").val(aTypeId);
        OnChangeApptTypes($("#appointmentTypes"));
        AddAppointmentTypesTimeSlot();
        $("#timef" + aTypeId).val(sTime);
        $("#timet" + aTypeId).val(eTime);
        $("#date" + aTypeId).val(fdate);
        $("#divOverViewPopup").hide();
        $('#divOVAvailableTimeSlots').removeClass('moveLeft2');
        $("#divSchedularPopUp").show();
        $(".newrowadded").remove();
        $(".selected_column").hide();
    } else {
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
    }
    //startChange("#timef" + aTypeId, "#timet" + aTypeId, parseInt($("#hfTimeSlot").val()));
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
                unBlockRecurrenceDiv();
                return;
            } else {
                var isRecurrance = $('#chkIsRec' + selectedAppointmenttype).prop('checked');
                jsonData[0] = ({
                    SchedulingId: $('#hfMain' + selectedAppointmenttype).val(),
                    AssociatedId: $('#hdPatientId').val(),//$('#hfPatientId').val(),
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
                    RemovedAppointmentTypes: $("#hfRemovedAppTypes").val(),
                    RoomAssigned: $("#hfRoomId").val()
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
                    AssociatedId: $('#hdPatientId').val(),//$('#hfPatientId').val(),
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
                    RemovedAppointmentTypes: $("#hfRemovedAppTypes").val(),
                    RoomAssigned: $("#hfRoomId").val()
                });
            }
        }
        var jsonD = JSON.stringify(jsonData);
        $.ajax({
            type: "POST",
            url: '/PatientSchedulerPortal/SavePatientPreScheduling',
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
        unBlockRecurrenceDiv();
    }
}

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
        }
        else if (ctrl.attr("freqtypeattr") == "weekly") {
            $("#rbtnWeeklyRecType").prop("checked", true);
            $("#weekly").show();
        }
        else if (ctrl.attr("freqtypeattr") == "monthly") {
            $("#rbtnMonthlyRecType").prop("checked", true);
            $("#monthly").show();
        }
    }
    else if (ctrl.attr("statusattr") == "edit") {
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
            $("#ddMonthWeekDays option[value=" + deptOpeningDays[i] + "]").prop('disabled', false);
            $("#ddMonthWeekDays option[value=" + deptOpeningDays[i] + "]").prop('title', "");
        }
    }
    $("#rbtnAllEndBy").prop("checked", true);
};

var OnClickRecurrenceTypeBtn = function (e) {
    
    var ctrl = $(e)[0];
    var aptDate = $("#date" + $("#hfRecAppTypeId").val()).val();
    var recEndDate = new Date(aptDate);
    switch (ctrl.id) {
        case "rbtnDailyRecType":
            var txtRecEveryDay = $("#txtRecEveryDay").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryDay == "" ? 0 : txtRecEveryDay));
            $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
            $("#weekly, #monthly").hide(); $("#daily").show();
            break;
        case "rbtnWeeklyRecType":
            var txtRecEveryWeekDays = $("#txtRecEveryWeekDays").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryWeekDays == "" ? 0 : txtRecEveryWeekDays * 7));
            $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
            $("#daily, #monthly").hide(); $("#weekly").show();
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
            $("#weekly, #daily").hide();
            $("#monthly").show();
            break;
    }
}

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
                $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
            }
            break;
        case "txtRecEveryWeekDays":
            var txtRecEveryWeekDays = $("#txtRecEveryWeekDays").val();
            recEndDate.setDate(recEndDate.getDate() + parseInt(txtRecEveryWeekDays == "" ? 0 : txtRecEveryWeekDays * 7));
            if (recEndDate > recEndByDate) {
                $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
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
                        $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
                    }
                    break;
                case "2":
                    var ddOnEveryMonth = $("#ddOnEveryMonth").val();
                    recEndDate.setDate(recEndDate.getDate() + parseInt(ddOnEveryMonth == "" ? 0 : ddOnEveryMonth * 30));
                    if (recEndDate > recEndByDate) {
                        $("#txtRecEndByDate").val(recEndDate.getMonth() + 1 + "/" + recEndDate.getDate() + '/' + recEndDate.getFullYear());
                    }
                    break;
            }
            break;
        default:
            break;
    }
}

var DoneApppointTypeReccurrence = function () {
    var recTypeValue = $("input[name='rbtnRecType']:checked").val();
    $("#btnEditRecurrence" + $("#hfRecAppTypeId").val()).attr("freqtypeattr", recTypeValue);
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
            recObject.end_By = recEndDate;//$("#txtRecEndByDate").val();
            recObject.Rec_Start_Date = recStartDateDaily;
            recObject.Frequency_Type = 1;//1 is for daily selected radio button
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
    $("#hfDone" + $("#hfRecAppTypeId").val()).val("1");//Added 1 to check the recurrence is done for particular procedure.
    apptRecArray.push(recObject);
    CancelApppointTypeReccurrence('done');
    $("#chkIsRec" + $("#hfRecAppTypeId").val()).prop("checked", true);
}

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
    $("#daily, #weekly, #monthly").show();
}

var blockRecurrenceDiv = function (text) {
    $("#pLoadingText").html(text);
    $("#loader_event").show();
}

var unBlockRecurrenceDiv = function () {
    $("#pLoadingText").html();
    $("#loader_event").hide();
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
    $("#daily").show();
    $("#weekly").hide();
    $("#monthly").hide();
    //$('#btnAddAppointmentType').removeAttr('disabled');
    $("#btnDeleteSchedulingData").hide();
    $("#hfSelPhysicianId").val("0");
    $("#hfExternalValue3").val("False");
    $("#hfRemovedAppTypes").val("");
    $("#hfRoomId").val('');
    $("#ddHolidayPhysician").prop("disabled", false);
    $("#btnSaveSchedulingData").show();
    $("#btnDeleteSchedulingData").show();
    $("#divHolidayToDateTime").hide();
    //$('.preDisabled').removeAttr('disabled');
}

function RemoveAppointmentProcedures(apptId) {
    var selectedTypes = $("#hfAppointmentTypes").val();
    var selectedTypesArray = selectedTypes.split(',');
    //var tt = apptRecArray;
    $.each(apptRecArray, function (i, val) {
        $.each(val, function (key, name) {
            
            if (name == apptId) {
                apptRecArray.splice(i, 1);
            }
        });
    });
    
    //var index = selectedTypesArray.indexOf(apptId.toString());
    selectedTypesArray.remove(apptId.toString());
    var removeAppTypes = $("#hfRemovedAppTypes").val();
    removeAppTypes += "," + $("#hfMain" + apptId).val();
    $("#hfRemovedAppTypes").val(removeAppTypes.replace(/^,/, ''));
    $("#hfAppointmentTypes").val(selectedTypesArray.join(','));
    $("#tr" + apptId).remove();
}

Array.prototype.remove = function (val) {
    var i = this.indexOf(val);
    return i > -1 ? this.splice(i, 1) : [];
};

function OnClickOverViewBtn() {
    $("#divOverViewPopup").show();
    $('#divSchedularPopUp').hide();
    $('#divFacilityOverview').validationEngine();
}

function OnClickOverViewCancel() {
    $("#divOverViewPopup").hide();
    $('#divSchedularPopUp').show();
}

function OnChangeOVAppointTypes(e) {
    var timeSlot = $(e).find('option:selected').attr("timeslot");
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
    var searchUrl = schedulerUrl + 'GetOverView?FromDate=' + fromDate + "&ToDate=" + endDate + "&FromTime=" + fromTime + "&ToTime=" + endTime +
        "&TimeSlotFrequency=" + timeSlotFrequency + "&AppointmentType=" + appointmentType + "&FacilityId=" + facilityId +
        "&DepartmentId=" + departmentId + "&Physician=" + physician + "&Patient=" + patient + "&Room=" + room;
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

            $("#ulOVTimeSlots").html('');
            var html = "";
            var data1 = GetFilteredResult(day, data);
            if (data1.length > 0) {
                for (var i = 0; i < data1.length; i++) {
                    html += '<li id="' + data1[i].STime + ' - ' + data1[i].ETime + '" attr-selDate="' + fromDate + '" attr-etime="' + data1[i].ETime + '" attr-stime="' + data1[i].STime + '" attr-fid="' + facilityId + '" attr-pid="' + physician + '" dopd="" onclick="SelectTimeSlot(this,' + appointmentType + ',2)">' + data1[i].STime + ' - ' + data1[i].ETime + '</li>';
                }
            } else {
                html += '<li>No Time slots are available</li>';
            }
            var newrow = $('<tr id="newrow' + clickedRowId + '" class="newrowadded"><td colspan="35" class="timingslots"><ul>' + html + '</ul><div class="deleterow" onclick="OnClickHideMinus();"><span class="glyphicon glyphicon-remove" aria-hidden="true"></span></div></td></tr>');
            $('#' + clickedRowId).after(newrow);
            //$("#ulOVTimeSlots").html(html);
            //$('#divOVAvailableTimeSlots').addClass('moveLeft2');
        },
        error: function (msg) {
        }
    });
}
function OnClickOVHide() {
    $('#divOVAvailableTimeSlots').removeClass('moveLeft2');
}
function OnClickHideMinus() {
    $(".newrowadded").remove();
    $(".selected_column").hide();
}
function GetFilteredResult(day, data) {
    var d = "";
    switch (day) {
        case "1":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D1).html().substring(0, $(record.D1).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "2":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D2).html().substring(0, $(record.D2).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "3":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D3).html().substring(0, $(record.D3).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "4":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D4).html().substring(0, $(record.D4).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "5":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D5).html().substring(0, $(record.D5).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "6":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D6).html().substring(0, $(record.D6).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "7":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D7).html().substring(0, $(record.D7).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "8":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D8).html().substring(0, $(record.D8).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "9":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D9).html().substring(0, $(record.D9).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "10":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D10).html().substring(0, $(record.D10).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "11":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D11).html().substring(0, $(record.D11).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "12":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D12).html().substring(0, $(record.D12).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "13":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D13).html().substring(0, $(record.D13).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "14":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D14).html().substring(0, $(record.D14).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "15":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D15).html().substring(0, $(record.D15).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "16":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D16).html().substring(0, $(record.D16).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "17":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D17).html().substring(0, $(record.D17).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "18":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D18).html().substring(0, $(record.D18).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "19":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D19).html().substring(0, $(record.D19).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "20":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D20).html().substring(0, $(record.D20).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "21":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D21).html().substring(0, $(record.D21).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "22":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D22).html().substring(0, $(record.D22).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "23":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D23).html().substring(0, $(record.D23).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "24":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D24).html().substring(0, $(record.D24).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "25":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D25).html().substring(0, $(record.D25).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "26":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D26).html().substring(0, $(record.D26).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "27":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D27).html().substring(0, $(record.D27).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "28":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D28).html().substring(0, $(record.D28).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "29":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D29).html().substring(0, $(record.D29).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "30":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D30).html().substring(0, $(record.D30).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
        case "31":
            d = jLinq.from(data.aaData).where(function (record) {
                
                if ($(record.D31).html().substring(0, $(record.D31).html().lastIndexOf("\n")) == "0")
                    return record;
            }).select();
            break;
    }
    return d;
}
function GetShowDataValue(value) {
    value = parseInt(value);
    var cls = "green_column";
    if (value == 0) {
        cls = "green_column";
    }
    else if (value > 0 && value <= 25) {
        cls = "light_green_column";
    }
    else if (value > 25 && value <= 50) {
        cls = "yellow_column";
    }
    else if (value > 50 && value <= 75) {
        cls = "pink_column";
    }
    else if (value > 75 && value < 100) {
        cls = "purple_column";
    }
    else if (value >= 100) {
        cls = "red_column";
    }
    return cls;
}

var CheckAllCheckBox = function (divId) {
    $('#' + divId + ' input:checkbox').prop('checked', true);
    SetSelectedViewAsWeek();
    switch (divId.toLowerCase()) {
        case 'treeviewfacilitydepartment':
            var treeViewDepartments = $("#treeviewFacilityDepartment").data("kendoTreeView");
            CheckNodes(treeViewDepartments.dataSource.view());
            onCheckDepartment();
            break;
        case 'treeviewphysician':
            var treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");
            CheckNodes(treeViewPhysician.dataSource.view());
            onCheckFilters();
            break;
        case 'treeviewstatuscheck':
            var treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");
            CheckNodes(treeViewStatus.dataSource.view());
            onCheckFilters();
            break;
        case 'treeviewfacilityrooms':
            var treeViewRooms = $("#treeviewFacilityRooms").data("kendoTreeView");
            CheckNodes(treeViewRooms.dataSource.view());
            onCheckFilters();
            break;
        default:
    }
    //scheduler.updateView(new Date($('#divDatetimePicker').val()), "week");
}

function CheckNodes(nodes) {
    /// <summary>
    /// Checkeds the node ids1.
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

function BindFacilityDepartmentspopup(facilityDDId) {
    
    $("#ddSpeciality").val("0");
    $.ajax({
        type: "POST",
        url: "/Home/GetDepartmentsByFacility",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            facilityId: $(facilityDDId).val()
        }),
        success: function (data) {
            
            if (data) {
                var items = '<option value="0">--Select--</option>';
                //$("#ddlDepartment").html(items);
                $.each(data.deptList, function (i, deaprtments) {
                    items += "<option value='" + deaprtments.Value + "'>" + deaprtments.Text + "</option>";
                });
                //$("#ddlDepartment").html(items);
                $("#selOVDepartment").html('');
                $("#selOVDepartment").html(items);//over view popup
                items = '<option value="0">--Select--</option>';
                $.each(data.phyList, function (i, physician) {
                    
                    items += '<option value="' + physician.Physician.Id + '" facilityId="' + physician.Physician.FacilityId + '" department="' + physician.UserDepartmentStr + '" departmentId="' + physician.Physician.FacultyDepartment + '" speciality="' + physician.UserSpecialityStr + '" specialityId="' + physician.Physician.FacultySpeciality + '">' + physician.Physician.PhysicianName + '(' + physician.UserTypeStr + ')</option>'; // Availability -  Field
                    //items += "<option value='" + physician.Id + "' PhysicianLicenseNumber='" + physician.PhysicianLicenseNumber + "'>" + physician.PhysicianName + "</option>";
                });

                if (facilityDDId != "#selOVFacility") {
                    $("#ddPhysician").html(items);
                } else {
                    $("#selOVPhysician").html('');
                    $("#selOVPhysician").html(items); //over view popup
                }
                BindAppointmentTypes(facilityDDId);
                BindPatient("#selOVPatient");
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
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


        var searchUrl = schedulerUrl + 'GetOverView?FromDate=' + fromDate + "&ToDate=" + endDate + "&FromTime=" + fromTime + "&ToTime=" + endTime +
            "&TimeSlotFrequency=" + timeSlotFrequency + "&AppointmentType=" + appointmentType + "&FacilityId=" + facilityId +
            "&DepartmentId=" + departmentId + "&Physician=" + physician + "&Patient=" + patient + "&Room=0&ViewType=0";

        if ($.fn.dataTable.isDataTable('#tblOVGrid')) {
            var tablen = $('#tblOVGrid').DataTable();
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
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 1);
                    }*/
                },
                {
                    "mDataProp": "D2",
                    "aTargets": [5],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 2);
                    }*/
                },
                {
                    "mDataProp": "D3",
                    "aTargets": [6],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 3);
                    }*/
                },
                {
                    "mDataProp": "D4",
                    "aTargets": [7],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 4);
                    }*/
                },
                {
                    "mDataProp": "D5",
                    "aTargets": [8],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 5);
                    }*/
                },
                {
                    "mDataProp": "D6",
                    "aTargets": [9],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 6);
                    }*/
                },
                {
                    "mDataProp": "D7",
                    "aTargets": [10],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 7);
                    }*/
                },
                {
                    "mDataProp": "D8",
                    "aTargets": [11],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 8);
                    }*/
                },
                {
                    "mDataProp": "D9",
                    "aTargets": [12],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 9);
                    }*/
                },
                {
                    "mDataProp": "D10",
                    "aTargets": [13],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 10);
                    }*/
                },
                {
                    "mDataProp": "D11",
                    "aTargets": [14],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 11);
                    }*/
                },
                {
                    "mDataProp": "D12",
                    "aTargets": [15],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 12);
                    }*/
                },
                {
                    "mDataProp": "D13",
                    "aTargets": [16],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 13);
                    }*/
                },
                {
                    "mDataProp": "D14",
                    "aTargets": [17],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 14);
                    }*/
                },
                {
                    "mDataProp": "D15",
                    "aTargets": [18],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 15);
                    }*/
                },
                {
                    "mDataProp": "D16",
                    "aTargets": [19],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 16);
                    }*/
                },
                {
                    "mDataProp": "D17",
                    "aTargets": [20],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 17);
                    }*/
                },
                {
                    "mDataProp": "D18",
                    "aTargets": [21],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 18);
                    }*/
                },
                {
                    "mDataProp": "D19",
                    "aTargets": [22],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 19);
                    }*/
                },
                {
                    "mDataProp": "D20",
                    "aTargets": [23],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 20);
                    }*/
                },
                {
                    "mDataProp": "D21",
                    "aTargets": [24],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 21);
                    }*/
                },
                {
                    "mDataProp": "D22",
                    "aTargets": [25],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 22);
                    }*/
                },
                {
                    "mDataProp": "D23",
                    "aTargets": [26],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 23);
                    }*/
                },
                {
                    "mDataProp": "D24",
                    "aTargets": [27],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 24);
                    }*/
                },
                {
                    "mDataProp": "D25",
                    "aTargets": [28],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 25);
                    }*/
                },
                {
                    "mDataProp": "D26",
                    "aTargets": [29],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 26);
                    }*/
                },
                {
                    "mDataProp": "D27",
                    "aTargets": [30],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 27);
                    }*/
                },
                {
                    "mDataProp": "D28",
                    "aTargets": [31],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 28);
                    }*/
                },
                {
                    "mDataProp": "D29",
                    "aTargets": [32],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 29);
                    }*/
                },
                {
                    "mDataProp": "D30",
                    "aTargets": [33],
                    "bVisible": true/*,
                    "render": function (data, type, full, meta) {
                        return GetGolorCodeString(data, full, 30);
                    }*/
                },
                {
                    "mDataProp": "D31",
                    "aTargets": [34],
                    "bVisible": true/*,
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
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                //$('#example tbody tr td').each(function () {
                //    this.setAttribute('title', $(this).text());
                //});
                //$('td', nRow).prop("title", "test");
                return nRow;
            }

        }
        var table = $('#tblOVGrid').DataTable(properties);
        setTimeout(function () {
            $('div[title]').qtip({
                position: {
                    my: 'top center',
                    target: 'mouse',
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
                    fixed: true// Make it fixed so it can be hovered over
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
        }, 10000);
        //new $.fn.dataTable.FixedHeader(table);
    }
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
                $.each(data, function (i, facility) {
                    items += "<option value='" + facility.PatientID + "'>" + facility.PersonFirstName + " " + facility.PersonLastName + "</option>";
                });
                $(selector).html('');
                $(selector).html(items);
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}