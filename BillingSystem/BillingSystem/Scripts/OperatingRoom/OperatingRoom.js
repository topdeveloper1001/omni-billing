var formSelector = "";
var IdSelector = "";
var colSelectorAddEdit = "";
var colSelectorList = "";
var ddlSelector = "";
$(function () {
    $("#OperatingRoomForm").validationEngine();
    $("#AnesthesiaTimeForm").validationEngine();
    BindOperatingRoomCodeDropdown(0);
    BindOperatingRoomCodeDropdown(1);

    $("#OperatingRoom_Save").click(function () {
        SetFormSelector(0);
        $("#OperatingRoom_OperatingType").val(1);
        SaveOperatingRoomData(0);
        return false;
    });

    $("#AnesthesiaTime_Save").click(function () {
        SetFormSelector(1);
        $("#AnesthesiaTime_OperatingType").val(2);
        SaveOperatingRoomData(1);
        return false;
    });

    $("#btnSurgeryCancel").click(function () {
        ClearOperatingRoomForm(0);
        return false;
    });

    $("#btnAnesthesiaCancel").click(function () {
        ClearOperatingRoomForm(1);
        return false;
    });

    $("#ddlServiceCodes").change(function () {
        $("#OperatingRoom_CodeValue").val($(this).val());
        return false;
    });

    $("#ddlAnesthesiaCPTCodes").change(function () {
        $("#AnesthesiaTime_CodeValue").val($(this).val());
        return false;
    });
});

function SaveOperatingRoomData(type) {
    /// <summary>
    /// Saves the operating room data.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    var isValid = formSelector.validationEngine({ returnIsValid: true });
    if (isValid) {
        var formData = formSelector.serializeArray();
        var isExist = false;
        $.post("/OperatingRoom/CheckDuplicateRecord", formData, function (response) {
            isExist = response;
            if (!isExist) {
                $.post("/OperatingRoom/SaveOperatingRoomData", formData, function (data) {
                    if (type == 0) {
                        BindList("#OperatingRoom_ListDiv", data);
                    } else {
                        BindList("#AnesthesiaTime_ListDiv", data);
                    }
                    var msg = "Records Saved successfully! ";
                    ClearOperatingRoomForm(type);
                    if (IdSelector.val() > 0)
                        msg = "Records updated successfully";
                    ShowMessage(msg, "Success", "success", true);
                });
            } else {
                ShowMessage("This records already exists! ", "Success", "success", true);
            }
        });
    }
}

function EditOperatingRoomData(id) {
    /// <summary>
    /// Edits the operating room data.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    $.post("/OperatingRoom/GetOperatingRoomDetails", { id: id }, function (data) {
        BindOperatingRoomDetails(data);
    });
}

function DeleteOperatingRoomData(id, type) {
    /// <summary>
    /// Deletes the operating room data.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    $.post("/OperatingRoom/DeleteOperatingRoomData", { id: id }, function (data) {
        if (type == 1) {
            BindList("#OperatingRoom_ListDiv", data);
        } else {
            BindList("#AnesthesiaTime_ListDiv", data);
        }
        ShowMessage("Record(s) Deleted Successfully!", "Success", "success", true);
    });
}

function ClearOperatingRoomForm(type) {
    /// <summary>
    /// Clears the operating room form.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    SetFormSelector(type);
    formSelector.clearForm(false);
    IdSelector.val(0);
    SetFormSelector2(type);
    colSelectorAddEdit.removeClass('in');
    colSelectorList.addClass('in');
    $.validationEngine.closePrompt(".formError", true);
    InitializeDateTimePicker();
    $(".btnSave").text("Save");
}

function BindOperatingRoomDetails(data) {
    /// <summary>
    /// Binds the operating room details.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    var formPrefix = "#OperatingRoom_";
    switch (data.OperatingType) {
        case 1:
            formPrefix = "#OperatingRoom_";
            SetFormSelector(0);
            SetFormSelector2(0);
            break;
        case 2:
            formPrefix = "#AnesthesiaTime_";
            SetFormSelector(1);
            SetFormSelector2(1);
            break;
    }

    IdSelector.val(data.Id);
    $(formPrefix + 'Id').val(data.Id);
    $(formPrefix + 'OperatingType').val(data.OperatingType);
    $(formPrefix + 'StartDay').val(data.StartDay);
    $(formPrefix + 'EndDay').val(data.EndDay);
    $(formPrefix + 'StartTime').val(data.StartTime);
    $(formPrefix + 'EndTime').val(data.EndTime);
    $(formPrefix + 'CalculatedHours').val(data.CalculatedHours);
    $(formPrefix + 'PatientId').val(data.PatientId);
    $(formPrefix + 'EncounterId').val(data.EncounterId);
    $(formPrefix + 'CodeValue').val(data.CodeValue);
    if (ddlSelector != '')
        $(ddlSelector).val(data.CodeValue);

    $(formPrefix + 'CodeValueType').val(data.CodeValueType);
    $(formPrefix + 'Save').text("Update");
    colSelectorList.removeClass("in");
    colSelectorAddEdit.addClass("in");
    InitializeDateTimePicker();
}

function SetFormSelector(type) {
    /// <summary>
    /// Sets the form selector.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    switch (type) {
        case 0:
            formSelector = $("#OperatingRoomForm");
            IdSelector = $("#OperatingRoom.Id");
            break;
        case 1:
            formSelector = $("#AnesthesiaTimeForm");
            IdSelector = $("#AnesthesiaTime.Id");
            break;
        default:
            formSelector = $("#OperatingRoomForm");
            IdSelector = $("#OperatingRoom.Id");
    }
}

function SetFormSelector2(type) {
    /// <summary>
    /// Sets the form selector2.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    switch (type) {
        case 0:
            colSelectorAddEdit = $("#collapseSurgeryAddEdit");
            colSelectorList = $("#collapseSurgeryList");
            ddlSelector = "#ddlServiceCodes";
            break;
        case 1:
            colSelectorAddEdit = $("#collapseAnesthesiaTimeAddEdit");
            colSelectorList = $("#collapseAnesthesiaTimeList");
            ddlSelector = "#ddlAnesthesiaCPTCodes";
            break;
        default:
            colSelectorAddEdit = $("#collapseSurgeryAddEdit");
            colSelectorList = $("#collapseSurgeryList");
            ddlSelector = "#ddlServiceCodes";
    }
}

function CalculateHours(startTimeSelector, endTimeSelector, txtCalculatorHoursSelector) {
    //create date format          
    /// <summary>
    /// Calculates the hours.
    /// </summary>
    /// <param name="startTimeSelector">The start time selector.</param>
    /// <param name="endTimeSelector">The end time selector.</param>
    /// <param name="txtCalculatorHoursSelector">The text calculator hours selector.</param>
    /// <returns></returns>
    if ($(startTimeSelector).val() != '' && $(endTimeSelector).val() != '') {
        var startDatetime = new Date("01/01/2007 " + $(startTimeSelector).val());
        var endDatetime = new Date("01/01/2007 " + $(endTimeSelector).val());
        var hourStart = startDatetime.getHours();
        var hoursEnd = endDatetime.getHours();
        var minsStart = startDatetime.getMinutes();
        var minsEnd = endDatetime.getMinutes();
        //if (hourStart > 0 && hoursEnd > 0 && hoursEnd > hourStart) {
        if (hourStart > 0 && hoursEnd > 0) {
            var hours = hoursEnd - hourStart;
            var mins = minsEnd - minsStart;
            if (mins > 0) {
                //hours = hours
            }
            $(txtCalculatorHoursSelector).val(hours + "." + mins);
        }
    }
}

/// <var>The calculate hours mins</var>
var CalculateHoursMins = function (startTimeSelector, endTimeSelector, txtCalculatorHoursSelector) {
    if ($(startTimeSelector).val() != '' && $(endTimeSelector).val() != '') {
        var dateStart = new Date("01/01/2007 " + $(startTimeSelector).val());
        var dateFuture = new Date("01/01/2007 " + $(endTimeSelector).val());
        var seconds = Math.floor((dateFuture - (dateStart)) / 1000);
        var minutes = Math.floor(seconds / 60);
        var hours = Math.floor(minutes / 60);
        var days = Math.floor(hours / 24);
        hours = hours - (days * 24);
        minutes = minutes - (days * 24 * 60) - (hours * 60);
        seconds = seconds - (days * 24 * 60 * 60) - (hours * 60 * 60) - (minutes * 60);
        $(txtCalculatorHoursSelector).val(hours + "." + minutes);
    }
};

function BindOperatingRoomCodeDropdown(type) {
    /// <summary>
    /// Binds the operating room code dropdown.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    var ddlSelector = "";
    var hdSelector = "";
    var jsonData = "";
    var url = "";

    switch (type) {
        case 0:
            ddlSelector = "#ddlServiceCodes";
            hdSelector = "#OperatingRoom_CodeValue";
            jsonData = { codeMainValue: "2404", rowCount: 3 };
            url = "/Home/GetServiceCodesByCodeMainValue";
            break;
        case 1:
            ddlSelector = "#ddlAnesthesiaCPTCodes";
            hdSelector = "#AnesthesiaTime_CodeValue";
            url = "/Home/GetCptCodesListByMueValue";
            jsonData = { mueValue: "Anesthesia" };
            break;
        default:
    }

    $.post(url, jsonData, function (data) {
        BindDropdownData(data, ddlSelector, hdSelector);
    });
}




