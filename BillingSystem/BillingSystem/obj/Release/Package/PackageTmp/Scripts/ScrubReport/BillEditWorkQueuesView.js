$(function () {
    BindUsersDropdown();

    var isShow = $("#BillsCount").length > 0 && $("#BillsCount").val() > 0;
    $("#BtnSrubbBill").attr("disabled", !isShow);
    if (!isShow) {
        $("#SelectedHeader").hide();
    } else {
        $("#SelectedHeader").show();
    }
    
});

function ViewScrubReport(headerId, type) {
    if (headerId == 0) {
        headerId = $("#selectedHeaderId").val();
    }
    if (headerId > 0) {
        $("#selectedHeaderId").val(headerId);
        $("#lblHeaderId").text(headerId);
        $("#generalHeading").hide();
        $("#HeadingWithselectedHeader").show();
        $.ajax({
            type: "POST",
            url: '/ScrubReport/GetScrubReport',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({ scrubHeaderId: headerId, reportType: type }),
            success: function (data) {
                BindList("#ScrubReportListDiv", data);
                $("#collapseScrubReport").addClass("in");
            },
            error: function (msg) {
            }
        });
    }
}

function GenerateScrub() {
    var headerId = $('#hdBillheaderId').val();
    $.ajax({
        type: "POST",
        url: '/ScrubReport/GenerateScrubReport',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ billheaderid: headerId, isAllShown: headerId > 0 }),
        success: function (data) {
            BindList("#divScrubHeaderList", data);
            window.location.reload(true);
            ShowMessage("Bill Scrubbing is done Successfully!", "Success", "success", true);
        },
        error: function (msg) {
        }
    });
}

function GetScrubReportDetail(scrubReportId) {
    $.ajax({
        type: "POST",
        url: '/ScrubReport/GetScrubReportDetail',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ scrubReportId: scrubReportId }),
        success: function (data) {
            BindCorrectionViewDetails(data);
        },
        error: function (msg) {
        }
    });
}

function LoadCorrectionViewData(scrubReportId) {
    GetScrubReportDetail(scrubReportId);
}

function BindCorrectionViewDetails(data) {
    BindList("#BillCorrectionPopup", data);
    var txtSelector = $('.correctedValue');
    var cssClass = $("#hdCssClass").val();
    txtSelector.datetimepicker('destroy');
    if (cssClass != '') {
        txtSelector.addClass(cssClass);

        //For Datetime
        if (cssClass.indexOf('datetime') != -1) {
            $("#txtLHSValue").datetimepicker({
                minDate: '1950/12/12',
                maxDate: '2025/12/12',
                format: 'm/d/Y H:i',
                mask: false,
                closeOnDateSelect: false
            });

            $("#txtRHSValue").datetimepicker({
                minDate: '1950/12/12',
                maxDate: '2025/12/12',
                format: 'm/d/Y H:i',
                mask: false,
                closeOnDateSelect: false
            });
        }
    }

    $('#divhidepopup1').show();
    jQuery("#BillCorrectionPopup").validationEngine();
    // ShowPopupqTip();
    //DisableTextFields();
}

function UpdateValueInScrubReport(scrubReportId) {
    var isValid = jQuery("#BillCorrectionPopup").validationEngine({ returnIsValid: true });
    if (isValid) {
        var lhsValue = $("#txtLHSValue").val();
        var rhsValue = $("#txtRHSValue").val();
        var jsonData = JSON.stringify({ scrubReportId: scrubReportId, lhsValue: lhsValue, rhsValue: rhsValue });
        $.ajax({
            type: "POST",
            url: '/ScrubReport/BillEditCorrections',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#ScrubReportListDiv", data);
                $('#divhidepopup1').hide();
                ShowMessage("Value has been updated successfully!", "Success", "success", true);
            },
            error: function (msg) {
            }
        });
    }
}

function ShowPopupqTip() {
    $('.correctedValue[title]').qtip({
        show: {
            ready: true
        }
    });
}

function CloseThis() {
    $('#divhidepopup1').hide();
    $.validationEngine.closePrompt('.formError', true);
}

function OnBlurCheckNewValuesStatus() {
    var firstValue = $("#txtLHSValue").val();
    var secondValue = $("#txtRHSValue").val();
    var compareType = $("#compareTypeSpan").text();
    var status = false;
    var cssClass = $("#hdCssClass").val();
    var dataType = cssClass.toLowerCase().indexOf("datetime") != -1 ? 1 : 0;
    if (firstValue != '' && secondValue != '' && compareType != '') {
        status = CheckIfTestCasePassed(firstValue, secondValue, compareType, dataType);
        if (status) {
            $("#imgScrubReportStatus").attr('src', '/../images/passed32x32.png');
        }
        else {
            $("#imgScrubReportStatus").attr('src', '/../images/warning32x32.png');
        }
    }
}

function BindUsersDropdown() {
    var selector = $(".ddlUsers");
    $.ajax({
        type: "POST",
        url: '/Home/GetUsers',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $(selector).empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data, function (i, obj) {
                var newItem = "<option id='" + obj.Value + "'  value='" + obj.Value + "'>" + obj.Text + "</option>";
                items += newItem;
            });

            $(selector).html(items);
        },
        error: function (msg) {
        }
    });
}

function SelectUser(scrubHeaderId, assignedTo) {
    //var hdSelector = $("#hd" + assignedTo);
    var selector = $("#" + scrubHeaderId);
    if (selector != undefined && selector != null) {
        var selectedValue = selector.val();
        if (selectedValue > 0) {
            $.ajax({
                type: "POST",
                url: '/ScrubReport/AssignUserToScrubHeader',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({ assignedTo: selectedValue, scrubHeaderId: scrubHeaderId }),
                success: function (data) {
                    if (data != null && data != '') {
                        $("#lblTo" + scrubHeaderId).text(data.AssignedToUser);
                        $("#lblBy" + scrubHeaderId).text(data.AssignedByUser);
                    }
                },
                error: function (msg) {
                }
            });
        }
    }
}

function ReScrubBillEdit(billHeaderId) {
    var headerId = $('#hdBillheaderId').val();
    $.ajax({
        type: "POST",
        url: '/ScrubReport/RescrubAfterBillEdit',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ billHeaderId: billHeaderId, allShown: (headerId == 0) }),
        success: function (data) {
            BindList("#divScrubHeaderList", data);
            window.location.reload(true);
            ShowMessage("Bill Re-Scrubbing has been done successfully!", "Success", "success", true);
        },
        error: function (msg) {
        }
    });
}

function SortScrubReport(event) {
    var headerId = $("#selectedHeaderId").val();
    var url = "/ScrubReport/GetScrubReport";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?headerId=" + headerId + "&type=" + type + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ scrubHeaderId: headerId, reportType: type }),
        success: function (data) {

            $("#ScrubReportListDiv").empty();
            $("#ScrubReportListDiv").html(data);
            SetScrubReportGrouping();

        },
        error: function (msg) {
        }
    });
}