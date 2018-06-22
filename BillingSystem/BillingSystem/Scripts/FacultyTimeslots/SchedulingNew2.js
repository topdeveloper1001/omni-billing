var firstTimeLoad = false;
var appointmentHTML = "";
$(function () {
    /// <summary>
    /// s this instance.
    /// </summary>
    /// <returns></returns>
    $("#divDatetimePicker").datepicker({
        numberOfMonths: [3, 1],
        showButtonPanel: true,
        onSelect: function (dateText, inst) {
            onCheckFilters();
        }
    });
    BindLocations();
    BindCorporatePhysician();
    BindSchedulingStatus();
    BindAppointmentAvailability();
    //GetSchedularData();
    setTimeout(SchedulerInit(null, null), 500);
    $('#btnScheduleAppointment').on('click', function () {
        var checkedBoxes = GetCheckedCheckBoxes('treeviewPhysician');
        if (checkedBoxes.length > 0) {
            scheduler.addEventNow({ tid: 1 });
        } else {
            ShowMessage('Select Any Physician first!', "Error", "error", true);
        }
    });

    $('#btnAddHoliday').on('click', function () {
        var checkedBoxes = GetCheckedCheckBoxes('treeviewPhysician');
        if (checkedBoxes.length > 0) {
            scheduler.addEventNow();
        } else {
            ShowMessage('Select Any Physician first!', "Error", "error", true);
        }
    });

    $('#btnAddVacation').on('click', function () {
        var checkedBoxes = GetCheckedCheckBoxes('treeviewPhysician');
        if (checkedBoxes.length > 0) {
            scheduler.addEventNow();
        } else {
            ShowMessage('Select Any Physician first!', "Error", "error", true);
        }
    });

    $('#chkViewAvialableSlots').on('change', function (e) {

        if (this.prop('checked')) {

        }
    });

});

var GetCheckedCheckBoxes = function (divid) {
    var selected = [];
    $('#' + divid + ' input:checked').each(function () {
        selected.push($(this).attr('name'));
    });
    return selected;
}


var GetSchedularData = function () {
    onCheckFilters();
}

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
}; //GetCorporatePhysicians}

/// <var>The bind scheduling status</var>
var BindSchedulingStatus = function () {
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
        success: function (data) {
            $('#divStatusList').empty();
            $('#divStatusList').html(data);
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
        async: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            ggcValue: '4903'
        }),
        success: function (data) {
            //debugger;
            /*Create Appointment popup - start*/
            appointmentHTML = '<div class="dhx_cal_ltext">';
            appointmentHTML += '<div class="col-md-6"><div class="form-group"><label>Notes: </label><textarea id="notes" placeholder="" class="form-control"></textarea></div>'; // Notes -  Field
            appointmentHTML += '<div class="form-group"><label>Availability: </label><select class="form-control" id="ddAvailability">';
            for (var i = 0; i < data.gClist.length; i++) {
                appointmentHTML += '<option value="' + data.gClist[i].GlobalCodeValue + '">' + data.gClist[i].GlobalCodeName + '</option>'; // Availability -  Field
            }
            appointmentHTML += '</select></div><div class="form-group"><label>Physician: </label><select class="form-control" id="ddPhysician" onchange="SetDepartmentAndSpeciality(this);">';
            for (var j = 0; j < data.physicians.length; j++) {
                appointmentHTML += '<option value="' + data.physicians[j].Physician.Id + '" department="' + data.physicians[j].UserDepartmentStr + '" speciality="' + data.physicians[j].UserSpecialityStr + '">' + data.physicians[j].Physician.PhysicianName + '</option>'; // Availability -  Field
            }
            appointmentHTML += '</select></div>';// Physician Selected -  Field
            appointmentHTML += '<div class="form-group"><label>Physician Comments: </label><textarea id="physiciancomments" placeholder="" class="form-control"></textarea></div></div>';// Physician Comments -  Field
            appointmentHTML += '<div class="col-md-6"><div class="form-group"><label>Patient Name: </label><input type="text" class="form-control search" placeholder="" size="25" id="patientname"></div>';// Patient Name -  Field
            appointmentHTML += '<div class="form-group"><label>Email: </label><input type="text" class="form-control" placeholder="" size="25" id="email"></div>';// Email -  Field
            appointmentHTML += '<div class="form-group"><label>Phone Number: </label><input type="text" class="form-control" placeholder="" size="25" id="phoneno"></div>';// Phone Number -  Field
            appointmentHTML += '<div class="form-group"><label>Department: </label><input type="text" class="form-control" placeholder="" size="25" id="Department"></div>';// Phone Number -  Field
            appointmentHTML += '<div class="form-group"><label>Speciality: </label><input type="text" class="form-control" placeholder="" size="25" id="Speciality"></div></div>';// Phone Number -  Field
            appointmentHTML += '</div>';

            /*Create Appointment popup - end*/
        },
        error: function (msg) {
        }
    });
};
var SetDepartmentAndSpeciality = function (e) {
    //debugger;
    var deptt = $(e).find('option:selected').attr("department");
    var spec = $(e).find('option:selected').attr("speciality");
    $("#Department").val(deptt);
    $("#Speciality").val(spec);

};
/// <var>The bind locations</var>
var BindLocations = function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/Home/GetFaciltyListTreeView",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $('#divLocationList').empty();
            $('#divLocationList').html(data);
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

// show checked node IDs on datasource change
function onCheckFilters() {
    var checkedPhysician = [],
        treeViewPhysician = $("#treeviewPhysician").data("kendoTreeView");

    var checkedStatusNodes = [],
        treeViewStatus = $("#treeviewStatusCheck").data("kendoTreeView");

    checkedNodeIds1(treeViewPhysician.dataSource.view(), checkedPhysician);
    checkedNodeIds1(treeViewStatus.dataSource.view(), checkedStatusNodes);

    BindSchedularWithFilters(checkedPhysician, checkedStatusNodes);
}

function onCheckLocation(e) {
    this.text(e.node);
    var nodeValueId = e.node.attributes[0].nodeValue;
    $('#hidFacilitySelected').val(nodeValueId);
    BindCorporatePhysician();
    //BindFacilityPhysicians(nodeValueId);
}

/// <var>The bind schedular with filters</var>
var BindSchedularWithFilters = function (physicianIds, statusfilter) {
    var jsonData = [];
    var physicianData = [];
    var statusData = [];
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
        SelectedDate: $('#divDatetimePicker').val()
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: "/FacultyTimeslots/GetSchedularWithFilters",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(jsonData),
        success: function (data) {
            //scheduler.clearAll();
            SchedulerInit(data, physicianData);
        },
        error: function (msg) {
        }
    });
};

function SchedulerInit(jsonData, physicianData) {
    /// <summary>
    /// Schedulers the initialize.
    /// </summary>
    /// <param name="jsonData">The json data.</param>
    /// <param name="physicianData">The physician data.</param>
    /// <returns></returns>
    scheduler.config.xml_date = "%m-%d-%Y %H:%i";
    scheduler.xy.editor_width = 0; //disable editor's auto-size
    scheduler.locale.labels.section_type = "Type:";    scheduler.locale.labels.section_status = "Status:";
    var format = scheduler.date.date_to_str("%H:%i");
    var step = 15;

    var appointmentLightBox = [
        { name: "appointment", height: 230, map_to: "name", type: "appointment", focus: true },
        { name: "recurring", height: 115, type: "recurring", map_to: "rec_type", button: "recurring" },
        { name: "time", height: 72, type: "time", map_to: "auto" }
    ];
    var snacks = [
				{ key: 5, label: 'Pineapple' },
				{ key: 6, label: 'Chocolate' },
				{ key: 7, label: 'Chips' },
				{ key: 8, label: 'Apple pie' }
    ];
    var holidayLightBox = [
       { name: "description", height: 130, map_to: "text", type: "textarea", focus: true },
        { name: "recurring", height: 115, type: "recurring", map_to: "rec_type", button: "recurring" },
        { name: "time", height: 72, type: "time", map_to: "auto" }
    ];



    var sections = [];
    if (physicianData != null && physicianData.length > 0 && physicianData[0].Id > 0) {
        for (var i = 0; i < physicianData.length; i++) {
            var physicianid = physicianData[i].Id;
            var phyName = $("#treeviewPhysician [name='checkedFiles'][value=" + physicianid + "]").parent().next().text();
            sections.push({ key: physicianid, label: phyName });
        }
    } else {
        var physicianCount = $("#treeviewPhysician [name='checkedFiles']");//.parent().next().text();
        for (var j = 0; j < physicianCount.length; j++) {
            var phyName1 = $("#treeviewPhysician [name='checkedFiles'][value=" + physicianCount[j].value + "]").parent().next().text();
            sections.push({ key: physicianCount[j].value, label: phyName1 });
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
    scheduler.locale.labels.section_parent = "Availability";
    scheduler.locale.labels.single_unit_tab = "Units";
    scheduler.locale.labels.section_custom = "Section";
    scheduler.createUnitsView({
        name: "single_unit",
        property: "section_id",
        list: sections
    });

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
            //GetFacultyScheduleViaWeek(minDate);
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
    /*Cusotm light box popup - start*/
    scheduler.form_blocks["appointment"] = {
        render: function (sns) {
            return appointmentHTML;
        },
        set_value: function (node, value, ev) {
            var _ctlObject = $(".dhx_cal_ltext .form-control");
            // we must loop through all nodes because we use the autocomplete plugin which sometimes adds
            // additional html elements therefore changing the position of the inputs
            $.each(_ctlObject, function (index, item) {
                if ($(item).attr("id") == "notes") {
                    _ctlObject[index].value = ev.text || "";
                }
                else if ($(item).attr("id") == "ddAvailability") {
                    _ctlObject[index].value = ev.Availability || "";
                }
                else if ($(item).attr("id") == "ddPhysician") {
                    _ctlObject[index].value = ev.physicianid || "";
                }
                else if ($(item).attr("id") == "physiciancomments") {
                    _ctlObject[index].value = ev.PhysicianComments || "";
                }
                else if ($(item).attr("id") == "patientname") {
                    _ctlObject[index].value = ev.PatientName || "";
                }
                else if ($(item).attr("id") == "email") {
                    _ctlObject[index].value = ev.PatientEmailId || "";
                }
                else if ($(item).attr("id") == "phoneno") {
                    _ctlObject[index].value = ev.PatientPhoneNumber || "";
                }
                else if ($(item).attr("id") == "Department") {
                    _ctlObject[index].value = ev.Department || "";
                }
                else if ($(item).attr("id") == "Speciality") {
                    _ctlObject[index].value = ev.PhysicianSpeciality || "";
                }
            });
        },
        get_value: function (node, ev) {
            //debugger;
            var _ctlObject = $(".dhx_cal_ltext .form-control");
            // we must loop through all nodes because we use the autocomplete plugin which sometimes adds
            // additional html elements therefore changing the position of the inputs
            $.each(_ctlObject, function (index, item) {
                if ($(item).attr("id") == "notes") {
                    ev.text = _ctlObject[index].value;
                }
                else if ($(item).attr("id") == "ddAvailability") {
                    ev.Availability = _ctlObject[index].value;
                }
                else if ($(item).attr("id") == "ddPhysician") {
                    ev.physicianid = _ctlObject[index].value;
                }
                else if ($(item).attr("id") == "physiciancomments") {
                    ev.PhysicianComments = _ctlObject[index].value;
                }
                else if ($(item).attr("id") == "patientname") {
                    ev.PatientName = _ctlObject[index].value;
                }
                else if ($(item).attr("id") == "email") {
                    ev.PatientEmailId = _ctlObject[index].value;
                }
                else if ($(item).attr("id") == "phoneno") {
                    ev.PatientPhoneNumber = _ctlObject[index].value;
                }
                else if ($(item).attr("id") == "Department") {
                    ev.Department = _ctlObject[index].value;
                }
                else if ($(item).attr("id") == "Speciality") {
                    ev.PhysicianSpeciality = _ctlObject[index].value;
                }
            });
            return _ctlObject[1].value;
        },
        focus: function (node) {
            //var a = node.childNodes[0]; a.select(); a.focus();
        }
    }
    /*Cusotm light box popup - end*/
    scheduler.config.lightbox.sections = appointmentLightBox;
    scheduler.templates.event_text = function (start, end, ev) {
        //debugger;
        return ev.text;
    };
    scheduler.attachEvent("onBeforeLightbox", function (event_id) {
        //debugger;
        scheduler.resetLightbox();
        var ev = scheduler.getEvent(event_id);
        scheduler.config.lightbox.sections = ev.tid == 2 ? appointmentLightBox : holidayLightBox;
        //scheduler.updateView();
        return true;

    });
    scheduler.attachEvent("onLightbox", function (event_id) {

        //debugger;
        //$(".dhx_cal_light").hide();//To hide the light box.
        /*$(".search").autocomplete({
              source: mainList,
              select: function (event, ui) {
                  debugger;
                  return false;
              }
          });*/
        $(".search").autocomplete({
            autoFocus: true,
            minLength: 3, // only start searching when user enters 3 characters
            source: function (request, response) {
                $.ajax({
                    url: "/PatientInfo/GetPatientInfoByPatientName",
                    type: "POST",
                    dataType: "json",
                    data: {
                        patientName: $("#patientname").val()
                    },
                    success: function (data) {
                        //debugger;
                        response(jQuery.map(data, function (item) {
                            //debugger;
                            return {
                                person_email_id: item.PersonEmailAddress,
                                perosn_first_name: item.PersonFirstName,
                                person_phone_number: item.PersonContactNumber,
                                person_id: item.PatientID,
                                label: item.PersonFirstName,
                                value: item.PersonFirstName,
                            }
                        }));
                    }
                });
            },
            select: function (event, ui) {
                $("#email").val(ui.item.person_email_id);
                $("#phoneno").val(ui.item.person_phone_number);
                return false;
            },
            change: function (event, ui) {
            }
        });
    });
    scheduler.attachEvent("onEventAdded", function (id, ev) {
        debugger;
        //SaveSchedularData(id, ev);
    });

    scheduler.attachEvent("onEventChanged", function (id, ev) {
        var datesObj = scheduler.getRecDates(id);
        // SaveTimeSlotData(id, ev);
    });

    scheduler.attachEvent("onEventDeleted", function (id, ev) {
        DeleteSchedulerEvent(id, ev);
    });

    //scheduler.attachEvent("onBeforeDrag", function() {
    //    return false;
    //});

    scheduler.attachEvent("onTemplatesReady", function () {
       
        //work week
        scheduler.config.start_on_monday = true;
        scheduler.config.time_step = 15;
        scheduler.xy.min_event_height = 21;
        scheduler.config.hour_size_px = 88;
    });
    scheduler.init('scheduler_here', new Date($('#divDatetimePicker').val()), "single_unit");

}

var html = function (id) { return document.getElementById(id); }; //just a helper

function save_form() {
    /// <summary>
    /// Save_forms this instance.
    /// </summary>
    /// <returns></returns>
    var ev = scheduler.getEvent(scheduler.getState().lightbox_id);
    ev.text = html("description").value;
    //ev.Availability = html("Availability").value;
    scheduler.endLightbox(true, html("my_form"));
}

function close_form() {
    /// <summary>
    /// Close_forms this instance.
    /// </summary>
    /// <returns></returns>
    scheduler.endLightbox(false, html("my_form"));
}

function delete_event() {
    /// <summary>
    /// Delete_events this instance.
    /// </summary>
    /// <returns></returns>
    var event_id = scheduler.getState().lightbox_id;
    scheduler.endLightbox(false, html("my_form"));
    scheduler.deleteEvent(event_id);
}

var SaveSchedularData = function (id, ev) {
    var jsonData = [];

    if (ev._timed === true) {
        var recurancedates = scheduler.getRecDates(id);
        if (recurancedates.length > 0) {
            for (var i = 0; i < recurancedates.length; i++) {
                jsonData[i] = {
                    SchedulingId: ev.TimeSlotId,
                    AssociatedId: 0,
                    AssociatedType: '0',
                    Status: '0',
                    StatusType: '0',
                    //PatientId: $("#hfPatientId").val(),
                    ScheduleFrom: recurancedates[i].start_date,
                    ScheduleTo: recurancedates[i].end_date,
                    PhysicianId: $("#ddlUser").val(),
                    TypeOfProcedure: ev.VisitType,
                    PhysicianSpeciality: $("#hfPhysicianSpeciality").val(),
                    //FacilityId: $("#ddlFacility").val(),
                    //CorporateId: $('#ddlCorporate').val(),
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
            AssociatedId: 0,
            AssociatedType: '0',
            Status: '0',
            StatusType: '0',
            //PatientId: $("#hfPatientId").val(),
            ScheduleFrom: recurancedates[i].start_date,
            ScheduleTo: recurancedates[i].end_date,
            PhysicianId: 0, //$("#ddlUser").val(),
            TypeOfProcedure: ev.VisitType,
            PhysicianSpeciality: '', //$("#hfPhysicianSpeciality").val(),
            //FacilityId: $("#ddlFacility").val(),
            //CorporateId: $('#ddlCorporate').val(),
            Comments: ev.text,
            Location: '', //$("#hfPhysicianLocation").val(),
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

var DeleteSchedulerEvent = function (id, ev) {
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