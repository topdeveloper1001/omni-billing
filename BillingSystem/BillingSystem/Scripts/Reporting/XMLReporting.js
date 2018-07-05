$(function() {
    BindReportingData();
    var reportingType = $("#hdReportType").val();
    if (reportingType == '2') {
        BindGlobalCodesWithValue("#ddlEncounterType", '1107');
        BindClinicalIDNumber("#ddlClinician");
        InitializeDatesForReporting();
    }
});

var BindReportingData = function () {
    var reportingType = $("#hdReportType").val();
    if (reportingType == '1') {
        BindXMLBatchReportData();
    } else if (reportingType == '2') {
        BindXMLInitialClaimErrorReportData();
    }
}

function InitializeDatesForReporting() {
    /// <summary>
    /// Initializes the dates for reporting.
    /// </summary>
    /// <returns></returns>
    $("#txtFromDate").datetimepicker({
        minDate: '-1990/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
        maxDate: '+2020/12/12',  //tommorow is maximum date calendar
        format: 'm/d/Y',
        mask: true,
        timepicker: false
    });

    $("#txtTillDate").datetimepicker({
        minDate: '-1990/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
        maxDate: '+2020/12/12',  //tommorow is maximum date calendar
        format: 'm/d/Y',
        mask: true,
        timepicker: false
    });
}

function BindXMLBatchReportData() {
    $('#loader_event').show();
    var reportingType = $("#hdReportType").val();
    var jsonData = JSON.stringify({
        reportingTypeId: reportingType,
    });
    $.ajax({
        type: "POST",
        url: '/Reporting/GetXMLBatchReport',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            BindList("#divXMLReporting", data);
            $('#loader_event').hide();
        },
        error: function (msg) {
        }
    });
}

function BindXMLInitialClaimErrorReportData() {
    $('#loader_event').show();
    var jsonData = JSON.stringify({
        startdate: new Date($("#txtFromDate").val()),
        enddate: new Date($("#txtTillDate").val()),
        encType: $('#ddlEncounterType').val(),
        clinicalId: $('#ddlClinician').val()
    });
    $.ajax({
        type: "POST",
        url: '/Reporting/GetXMLInitialClaimErrorReport',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            BindList("#divXMLReporting", data);
            $('#loader_event').hide();
        },
        error: function (msg) {
        }
    });
}

function BindClinicalIDNumber(ddlid) {
    $.ajax({
        type: "POST",
        url: '/Physician/GetClinicalIDNumber',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, faculty) {
                    items += "<option value='" + faculty.Value + "'>" + faculty.Text + "</option>";
                });
                $(ddlid).empty().html(items);
            }
        },
        error: function (msg) {
        }
    });
}