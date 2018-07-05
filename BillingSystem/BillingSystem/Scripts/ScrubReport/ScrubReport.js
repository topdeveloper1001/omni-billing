$(function () {
    BindUsersDropdown();
    SetRowColor();
});


function SetRowColor() {
    /// <summary>
    /// Sets the color of the row.
    /// </summary>
    /// <returns></returns>
    $("#ScrubHeaderBillEdits tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        if ($actualRow.find('.col1 input[type="hidden"]').val() == 'High') {
            $actualRow.addClass('rowColor3');
        } else if ($actualRow.find('.col1 input[type="hidden"]').val() == 'Medium') {
            $actualRow.addClass('rowColor2');
        } else {
            $actualRow.removeClass('rowColor3');
            $actualRow.removeClass('rowColor2');
        }
    });

    $("#ScrubContentHeaderList tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        if ($actualRow.find('.col1 input[type="hidden"]').val() == 'High') {
            $actualRow.addClass('rowColor3');
        } else if ($actualRow.find('.col1 input[type="hidden"]').val() == 'Medium') {
            $actualRow.addClass('rowColor2');
        } else {
            $actualRow.removeClass('rowColor3');
            $actualRow.removeClass('rowColor2');
        }
    });
    SetScrubReportGrouping();
}

function SetScrubReportGrouping() {
    $("#ScrubReportBillEdit tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        if ($actualRow.find('.Col10 input[type="hidden"]').val() == 'diff') {
            $actualRow.addClass('rowColor1');
            $actualRow.attr('title', 'Rule Code :' + $actualRow.find('.Col1 input[type="hidden"]').val());
        } else if ($actualRow.find('.Col10 input[type="hidden"]').val() == 'same') {
            $actualRow.addClass('rowColor2');
            $actualRow.attr('title', 'Rule Code :' + $actualRow.find('.Col1 input[type="hidden"]').val());
        } else {
            $actualRow.removeClass('rowColor1');
            $actualRow.removeClass('rowColor5');
        }
    });
}

function ViewScrubReport(headerId, type) {
    /// <summary>
    /// Views the scrub report.
    /// </summary>
    /// <param name="headerId">The header identifier.</param>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    $('.rdScrubReportType').prop('checked', false);
    $('#rdShowAll').prop('checked', true);
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
            async: true,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({ scrubHeaderId: headerId, reportType: type }),
            success: function (data) {
                $("#collapseScrubReport").addClass("in").attr('style','height:auto;');
                BindList("#ScrubReportListDiv", data);
                $('#BillActivityListDiv').empty();
                SetScrubReportGrouping();
                SetRadioButton(type);
                $('html, body').animate({
                    scrollTop: $("#collapseScrubReport").offset().top
                }, 2000);
            },
            error: function (msg) {
            }
        });
    }
}

function GenerateScrub() {
    /// <summary>
    /// Generates the scrub.
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Gets the scrub report detail.
    /// </summary>
    /// <param name="scrubReportId">The scrub report identifier.</param>
    /// <returns></returns>
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
    /// <summary>
    /// Loads the correction view data.
    /// </summary>
    /// <param name="scrubReportId">The scrub report identifier.</param>
    /// <returns></returns>
    GetScrubReportDetail(scrubReportId);
}

function BindCorrectionViewDetails(data) {
    /// <summary>
    /// Binds the correction view details.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns></returns>
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
    BindCorrectionCodeData();
    $("#BillCorrectionPopup").validationEngine();
}

function UpdateValueInScrubReport(scrubReportId) {
    /// <summary>
    /// Updates the value in scrub report.
    /// </summary>
    /// <param name="scrubReportId">The scrub report identifier.</param>
    /// <returns></returns>
    var isValid = $("#BillCorrectionPopup").validationEngine({ returnIsValid: true });
    if (isValid) {
        var lhsValue = $("#txtLHSValue").val();
        var rhsValue = $("#txtRHSValue").val();
        var correctioncodeVal = $("#ddlCorrectionCodeList").val();
        var hdPatientId = $("#hdPatientId").val();
        var hdEncounterId = $("#hdEncounterId").val();
        var jsonData = JSON.stringify({
            scrubReportId: scrubReportId,
            lhsValue: lhsValue,
            rhsValue: rhsValue,
            correctionCodeId: correctioncodeVal,
            patientId: hdPatientId,
            encounterId: hdEncounterId
        });
        $.ajax({
            type: "POST",
            url: '/ScrubReport/BillEditCorrections',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data == "0") {
                    // BindList("#ScrubReportListDiv", data);
                    $('#divhidepopup1').hide();
                    ShowMessage("Value has been updated successfully!", "Success", "success", true);
                    BindScrubReportList(scrubReportId);
                } else if (data == "99") {
                    ShowMessage("Not Valid diagnosis Code!", "Warning", "warning", true);
                }
                else if (data == "99") {
                    ShowMessage("Diagnosis Code already exist for given Patient and Encounter", "Success", "success", true);
                } else {
                    //BindList("#ScrubReportListDiv", data);
                    $('#divhidepopup1').hide();
                    ShowMessage("Value has been updated successfully!", "Success", "success", true);
                    BindScrubReportList(scrubReportId);
                }
            },
            error: function (msg) {
            }
        });
    }
}

function BindScrubReportList(scrubReportId) {
    var jsonData = JSON.stringify({
        scrubReportId: scrubReportId,
    });
    $.ajax({
        type: "POST",
        url: '/ScrubReport/GetScurbReportbyScrubReportId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindList("#ScrubReportListDiv", data);
        },
        error: function (msg) {
        }
    });
}

function ShowPopupqTip() {
    /// <summary>
    /// Shows the popupq tip.
    /// </summary>
    /// <returns></returns>
    $('.correctedValue[title]').qtip({
        show: {
            ready: true
        }
    });
}

//function DisableTextFields() {
//    var rdLhsv = $("#rdLHSV")[0].checked;
//    if (rdLhsv) {
//        $('#txtRHSValue').attr('disabled', true);
//        $('#txtLHSValue').attr('disabled', false);
//    }
//    else {
//        $('#txtLHSValue').attr('disabled', true);
//        $('#txtRHSValue').attr('disabled', false);
//    }
//}

function CloseThis() {
    /// <summary>
    /// Closes the this.
    /// </summary>
    /// <returns></returns>
    $('#divhidepopup1').hide();
    $.validationEngine.closePrompt('.formError', true);
}

function OnBlurCheckNewValuesStatus() {
    /// <summary>
    /// Called when [blur check new values status].
    /// </summary>
    /// <returns></returns>

    var firstValue = $("#txtLHSValue").val();
    var secondValue = $("#txtRHSValue").val();
    var compareType = $("#compareTypeSpan").text();
    var status = false;
    var cssClass = $("#hdCssClass").val();
    var dataType = cssClass.toLowerCase().indexOf("datetime") != -1 ? 1 : 0;
    var secondValueCheck = $('#hdRHSVDesc').val() == "direct value" ? true : secondValue != '' ? true : false;
    if (firstValue != '' && secondValueCheck && compareType != '') {
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
    /// <summary>
    /// Binds the users dropdown.
    /// </summary>
    /// <returns></returns>
    var selector = $(".ddlUsers");
    $.ajax({
        type: "POST",
        url: '/Security/GetUsers',
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
    /// <summary>
    /// Selects the user.
    /// </summary>
    /// <param name="scrubHeaderId">The scrub header identifier.</param>
    /// <param name="assignedTo">The assigned to.</param>
    /// <returns></returns>
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
    /// <summary>
    /// Res the scrub bill edit.
    /// </summary>
    /// <param name="billHeaderId">The bill header identifier.</param>
    /// <returns></returns>
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

function BindCorrectionCodeData() {
    /// <summary>
    /// Binds the correction code data.
    /// </summary>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        url: '/ScrubReport/GetCorrectionCodeList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $('#ddlCorrectionCodeList').empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data, function (i, codeData) {
                var newItem = "<option value='" + codeData.Value + "'>" + codeData.Text + "</option>";
                items += newItem;
            });
            $('#ddlCorrectionCodeList').html(items);
            var correctioncode = $('#hdCorrectionCode').val();
            if (correctioncode != '') {
                $('#ddlCorrectionCodeList').val(correctioncode);
            } else {
                $('#ddlCorrectionCodeList').val('0');
            }
        },
        error: function (msg) {
        }
    });
}

function ViewBillActivities(bHeaderId, urlAction) {
    /// <summary>
    ///     Views the bill activities.
    /// </summary>
    /// <param name="bHeaderId">The b header identifier.</param>
    /// <param name="urlAction">The URL action.</param>
    /// <returns></returns>
    if (bHeaderId > 0) {
        $.ajax({
            type: "POST",
            url: urlAction,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({ billHeaderId: bHeaderId }),
            success: function (data) {
                $("#CollapseBillActivitiesList").addClass('in').attr('style', 'height:auto;');
                BindList("#BillActivityListDiv", data);
                $("#CollapseBillActivitiesList").addClass("in");
                GetBillHeaderDetials(bHeaderId);
                $('#hidSelectedBillHeaderId').val(bHeaderId);
                $("#ScrubReportListDiv").empty();
                $('html, body').animate({
                    scrollTop: $("#CollapseBillActivitiesList").offset().top
                }, 2000);
            },
            error: function (msg) {
            }
        });
    }
}

var GetBillHeaderDetials = function (billheaderId) {
    if (billheaderId > 0) {
        $.ajax({
            type: "POST",
            url: "/BillHeader/GetBillHeaderDetails",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ billHeaderId: billheaderId }),
            success: function (data) {
                var str = " ( Bill Number:" + data.BillNumber + " )";
                $('#billnumberSpn').html(str);
            },
            error: function (msg) {
            }
        });
    }
};



/*-----------Sorting-----------*/

function SortScrubReport(event) {

    var headerId = $("#selectedHeaderId").val();
    //var showAll = $('input[name=All]:checked').val();
    var pass = $('#rdPassed').prop('checked'); //$('input[name=Pass]:checked').val();
    var warning = $('#rdWarning').prop('checked'); //$('input[name=Warning]:checked').val();
    var error = $('#rdError').prop('checked'); //rdError $('input[name=Error]:checked').val();
    var notAplicable = $('#rdNotApplicable').prop('checked'); //$('input[name=NotApplicable]:checked').val();

    var type = "";
    if (notAplicable) {
        type = 99;
    } else if (pass) {
        type = 0;
    } else if (warning) {
        type = 1;
    } else if (error) {
        type = 2;
    } else {
        type = 999;
    }
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
    //}
}

function SortCleanClaims(event) {
    var headerId = $("#selectedHeaderId").val();
    var url = "/ScrubReport/SortCleanClaims";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?headerId=" + headerId + "&generateScrub=false&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#divScrubHeaderAllPassedList").empty();
            $("#divScrubHeaderAllPassedList").html(data);
        },
        error: function (msg) {
        }
    });
}

function SortClaimsCorrected(event) {
    var headerId = $("#selectedHeaderId").val();
    var url = "/ScrubReport/SortClaimsCorrected";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?headerId=" + headerId + "&generateScrub=false&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#divScrubHeaderPassedAfterBillEditList").empty();
            $("#divScrubHeaderPassedAfterBillEditList").html(data);
            BindUsersDropdown();
        },
        error: function (msg) {
        }
    });
}

function SortClaimswithPotentialEdit(event) {
    var headerId = $("#selectedHeaderId").val();
    var url = "/ScrubReport/SortClaimswithPotentialEdit";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?headerId=" + headerId + "&generateScrub=false&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#divScrubHeaderWithErrorsList").empty();
            $("#divScrubHeaderWithErrorsList").html(data);
            BindUsersDropdown();
        },
        error: function (msg) {
        }
    });
}

function SortClaimsdeniedwithErrors(event) {
    var headerId = $("#selectedHeaderId").val();
    var url = "/ScrubReport/SortClaimsdeniedwithErrors";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?headerId=" + headerId + "&generateScrub=false&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#divScrubHeaderWithDenialsList").empty();
            $("#divScrubHeaderWithDenialsList").html(data);
            BindUsersDropdown();
        },
        error: function (msg) {
        }
    });
}

function SortBillActivityGrid(event) {
    var url = "/BillHeader/GetBillActivitiesByBillHeaderId";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?billHeaderId=" + $('#hidSelectedBillHeaderId').val() + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#CollapseBillActivitiesList").addClass("in").attr('style', 'height:auto;');
            $('#BillActivityListDiv').empty().html(data);
        },
        error: function (msg) {
        }
    });
}
//function SortClaimsNeedingApprovalGrid(event) {
//    
//    var headerId = $("#selectedHeaderId").val();
//    if (headerId == "") {
//        headerId = 0;
//    }

//    var url = "/ScrubReport/SortClaimsNeedingApprovalGrid";
//    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
//        url += "?billHeaderId=" + headerId + "&generateScrub=" + false + "&" + event.data.msg;
//    }
//    $.ajax({
//        type: "POST",
//        url: url,
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        data: JSON.stringify({ billHeaderId: headerId, generateScrub: false }),
//        success: function (data) {
//            
//            $("#divScrubHeaderPassedAfterBillEditList").empty();
//            $("#divScrubHeaderPassedAfterBillEditList").html(data);
//            SetScrubReportGrouping();

//        },
//        error: function (msg) {
//        }
//    });

//}

var SetRadioButton = function (type) {
    $('.rdScrubReportType').prop('checked', false);
    switch (type) {
        case 999:
            $('#rdShowAll').prop('checked', 'checked');
            break;
        case 0:
            $('#rdPassed').prop('checked', 'checked');
            break;
        case 1:
            $('#rdWarning').prop('checked', 'checked');
            break;
        case 2:
            $('#rdError').prop('checked', 'checked');
            break;
        case 99:
            $('#rdNotApplicable').prop('checked', 'checked');
            break;
        
    default:
    }
}