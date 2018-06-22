$(LoadBedRateCardForm);

function LoadBedRateCardForm() {
    //BindFacilityDropdown();
    $("#BedRateCardDiv").validationEngine();
    //BindGlobalCodesWithValue('#ddlBedTypes', 1001, '');
    //BindGlobalCodesWithValue('#ddlUnitTypes', 18, '');
    BindServiceCodesInBedRateCard('');
    BindGlobalCodesDropdownData();

    $("#ddlServiceCodeValues").change(function () {
        var selectedValue = $(this).val();
        if (selectedValue != '' || selectedValue != '0') {
            var codeId = $('option:selected', this).attr('codeId');
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/BedRateCard/GetServiceCodeDetailById",
                dataType: "json",
                //async: true,
                data: JSON.stringify({ serviceCodeId: codeId }),
                success: function (data) {
                    $('#txtRates').val(data.ServiceCodePrice);
                    $('#lblSelectedServiceCode').html(data.ServiceCodeValue);

                    if (data.ServiceCodeEffectiveDate != null && data.ServiceCodeEffectiveDate != '')
                        $("#BedRateCard_EffectiveFrom").val(ToJavaScriptMonthDate(data.ServiceCodeEffectiveDate));

                    if (data.ServiceExpiryDate != null && data.ServiceExpiryDate != '')
                        $("#BedRateCard_EffectiveTill").val(ToJavaScriptMonthDate(data.ServiceExpiryDate));
                    InitializeDatePickerInBedRateCard();
                },
                error: function (msg) {
                }
            });
        }
    });
}

function SaveBedRateCard() {
    var isValid = jQuery("#BedRateCardDiv").validationEngine({ returnIsValid: true });
    if (!isValid) return;

    var ddlBedTypes = $("#ddlBedTypes").val();
    var ddlUnitTypes = $("#ddlUnitTypes").val();
    var txtRates = $("#txtRates").val();
    var txtDayStart = $("#txtDayStart").val();
    var txtDayEnd = $("#txtDayEnd").val();
    var ddlServiceCodeValue = $("#ddlServiceCodeValues").val();
    var id = $("#hfBedRateCardId").val();
    var effectiveFrom = $("#EffectiveStartDate").val();
    var effectiveTill = $("#EffectiveEndDate").val();
    //var facilityId = $("#ddlFacilityFilter").val();

    var jsonData = JSON.stringify({
        BedRateCardID: id,
        BedTypes: ddlBedTypes,
        UnitType: ddlUnitTypes,
        Rates: txtRates,
        DayStart: txtDayStart,
        DayEnd: txtDayEnd,
        ServiceCodeValue: ddlServiceCodeValue,
        IsActive: true,
        EffectiveFrom: effectiveFrom,
        EffectiveTill: effectiveTill,
        FacilityId: 0,
        IsDeleted:0
    });
    $.ajax({
        type: "POST",
        url: '/BedRateCard/AddUpdateBedRateCard',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null && data != -1) {
                ClearBedRateCardForm();
                BindBedRateCardGrid();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";
                ShowMessage(msg, "Success", "success", true);
            }
            else {
                if (data == -1) {
                    ShowMessage("Record already exists!", "Alert", "warning", true);
                }
            }
        },
        error: function (msg) {
        }
    });
}

function EditBedRateCard(id) {
    if (id > 0) {
        var jsonData = JSON.stringify({
            bedRateCardId: id,
        });
        $.ajax({
            type: "POST",
            url: '/BedRateCard/GetBedRateCard',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                BindBedRateCardDetailsInEditMode(data);
            },
            error: function (msg) {
            }
        });
    }
}

function DeleteBedRateCard(id) {
    //if ($("#ddlFacilityFilter").val() > 0 && id > 0) {

    //}
    if (confirm("Do you want to delete this record? ")) {
        var jsonData = JSON.stringify({
            id: id
        });
        $.ajax({
            type: "POST",
            url: '/BedRateCard/DeleteBedRateCard',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#physicianGrid", data);
                ShowMessage("Records Deleted Successfully", "Success", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function ClearBedRateCardForm() {
    $('#BedRateCardDiv').clearForm(true);
    $.validationEngine.closePrompt(".formError", true);
    $('#collapseTwo').addClass('in');
    $('#btnSubmit').val('Save');
    $('#lblSelectedServiceCode').html('');
    $('#BedRateCard_EffectiveFrom').val('');
    $('#BedRateCard_EffectiveTill').val('');
}

function BindBedRateCardGrid() {
    /// <summary>
    /// Binds the bed rate card grid.
    /// </summary>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/BedRateCard/GetBedRateCardList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $('#collapseOne').addClass('in');
            $("#physicianGrid").empty();
            $("#physicianGrid").html(data);
        },
        error: function (msg) {
        }
    });
}

function BindBedRateCardDetailsInEditMode(data) {
    $('#btnSubmit').val('Update');
    $('#hfBedRateCardId').val(data.BedRateCardID);
    $('#ddlBedTypes').val(data.BedTypes);
    $('#ddlUnitTypes').val(data.UnitType);
    $('#txtDayStart').val(data.DayStart);
    $('#txtDayEnd').val(data.DayEnd);
    $('#txtRates').val(data.Rates);
    $('#ddlServiceCodeValues').val(data.ServiceCodeValue);
    //$('#ddlFacilityFilter').val(data.FacilityId);

    //$("#BedRateCard_EffectiveFrom").html(ToJavaScriptMonthDate(data.EffectiveFrom));
    //$("#BedRateCard_EffectiveTill").html(ToJavaScriptMonthDate(data.EffectiveTill));

    $("#EffectiveStartDate").val(data.EffectiveFrom);
    $("#EffectiveEndDate").val(data.EffectiveTill);
    $("#lblSelectedServiceCode").html(data.ServiceCodeValue);
    $('#collapseOne').addClass('in');
}

function BindServiceCodesInBedRateCard(hidValueSelector) {
    var selector = "#ddlServiceCodeValues";
    $.ajax({
        type: "POST",
        url: "/BedRateCard/GetServiceCodesList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data != null) {
                var items = '<option codeId="0" value="0">--Select--</option>';
                $.each(data, function (i, obj) {
                    items += "<option codeId='" + obj.ExternalValue1 + "' value='" + obj.Value + "'>" + obj.Text + "</option>";
                });

                $(selector).html(items);

                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
                    $(selector).val($(hidValueSelector).val());
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

function InitializeDatePickerInBedRateCard() {
    $("#EffectiveStartDate").datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true

    });

    $("#EffectiveEndDate").datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true

    });
}

function SortBedRateCardGrid(event) {
    var url = "/BedRateCard/GetBedRateCardList";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#physicianGrid").empty();
            $("#physicianGrid").html(data);

        },
        error: function (msg) {
        }
    });
}

function CheckFrequencyStartDays() {
    var startDays = $("#txtDayStart").val();
    var endDays = $("#txtDayEnd").val();

    var startDaysPage = parseInt(startDays, 10);
    var endDaysPage = parseInt(endDays, 10);
    if (endDays != '') {
        if (startDaysPage > endDaysPage) {
            $("#txtDayEnd").val('');
            ShowMessage("Frequency End should be greater", "Warning", "warning", true);
            return false;
        }
    }
    return true;
}

function CheckTwoDatesInBedRateCard(startdata, enddate, selector) {
    /// <summary>
    /// Checks the two dates.
    /// </summary>
    /// <param name="startdata">The startdata.</param>
    /// <param name="enddate">The enddate.</param>
    /// <param name="selector">The selector.</param>
    /// <returns></returns>
    $("." + selector + "formError").remove();
    if (startdata != "" && enddate != "") {

        //Checke valid From date --------------------
        if (startdata.val() != "") {
            var validDate = startdata.val().split('/');
            if (parseInt(validDate[2], 10) < 1900) {
                startdata.val('');
                startdata.focus();
                ShowMessage('Invalid Date.', "Alert", "warning", true);
                return false;
            }
            var dt = new Date(parseInt(validDate[2], 10), parseInt(validDate[0], 10) - 1, parseInt(validDate[1], 10));
            if (dt.getDate() != parseInt(validDate[1], 10) || dt.getMonth() != (parseInt(validDate[0], 10) - 1) || dt.getFullYear() != parseInt(validDate[2], 10)) {
                startdata.val('');
                startdata.focus();
                ShowMessage('Invalid Date.', "Alert", "warning", true);
                return false;
            }
        }
        //------------------------------------------
        //Checke valid end date --------------------
        if (enddate.val() != "") {
            var validDate = enddate.val().split('/');
            if (parseInt(validDate[2], 10) < 1900) {
                enddate.val('');
                enddate.focus();
                ShowMessage('Invalid Date.', "Alert", "warning", true);
                return false;
            }
            var dt = new Date(parseInt(validDate[2], 10), parseInt(validDate[0], 10) - 1, parseInt(validDate[1], 10));
            if (dt.getDate() != parseInt(validDate[1], 10) || dt.getMonth() != (parseInt(validDate[0], 10) - 1) || dt.getFullYear() != parseInt(validDate[2], 10)) {
                enddate.val('');
                enddate.focus();
                ShowMessage('Invalid Date.', "Alert", "warning", true);
                return false;
            }
        }
        //------------------------------------------

        var srdate = new Date(startdata.val());
        var eddate = new Date(enddate.val());
        if (srdate > eddate) {
            enddate.val("");
            ShowMessage('Start date should not be greater than end date.', "Alert", "warning", true);
            enddate.focus();
            return false;
        }
        else {
            return true;
        }
    }
}


function BindFacilityDropdown() {
    $.ajax({
        type: "POST",
        url: "/BedRateCard/GetFacilitiesbyCorporate",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            BindDropdownData(data, "#ddlFacilityFilter", "");

        },
        error: function (msg) {
            console.log(msg);
        }
    });
}


function BindGlobalCodesDropdownData() {
    $.ajax({
        type: "POST",
        url: "/BedRateCard/BindGlobalCodesDropdownData",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data != null) {
                BindDropdownData(data.listUnitTypes, '#ddlUnitTypes', '');
                BindDropdownData(data.listBedTypes, "#ddlBedTypes", "");
                //BindDropdownData(data.listServiceCodes, "#ddlServiceCodeValues", "");
                //BindDropdownData(data.finalList, "#ddlFacilityFilter", "");
                }
        },
        error: function (msg) {
        }
    });
}


function DeleteBedRateCard() {
   if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/BedRateCard/DeleteBedRateCard',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#physicianGrid", data);
                ShowMessage("Records Deleted Successfully", "Success", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}