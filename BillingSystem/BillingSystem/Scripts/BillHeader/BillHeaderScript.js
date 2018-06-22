$(function () {
    //-------------Added for Super Powers functionality-------------///
    var patientId = $("#bhPatientId").val();
    var encounterId = $("#hdId").val() > 0 && $("#hdTypeId").val() > 0 && $("#hdTypeId").val() == 1 ? $("#hdId").val() : 0;

    $("#GlobalPatientId").val(patientId);
    $("#GlobalEncounterId").val(encounterId);
    BindLinkUrlsForSuperPowers();
    //-------------Added for Super Powers functionality-------------///
});
var isAuth = false;

function RowColorBillHeader() {
    /// <summary>
    ///     Rows the color bill header.
    /// </summary>
    /// <returns></returns>
    $("#BillHeaderListDiv tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        $actualRow.removeClass('rowColor3');
        $actualRow.removeClass('rowColor1');
        $actualRow.removeClass('rowColor2');
        $actualRow.removeClass('rowColorPreliminary');
        $actualRow.removeClass('rowColorAuthrizationPending');
        $actualRow.removeClass('rowColorManagedCarePending');
        $actualRow.removeClass('rowColorBothAuthAndManagedCarePending');
        $actualRow.removeClass('rowColorReadyToPreliminary');
        $actualRow.removeClass('rowColorInitialized');
        if ($actualRow.find('.colStatus').html().indexOf('0') != -1) {
            $actualRow.removeClass('rowColor3');
            $actualRow.removeClass('rowColor1');
            $actualRow.removeClass('rowColor2');
            $actualRow.removeClass('rowColorPreliminary');
            $actualRow.removeClass('rowColorAuthrizationPending');
            $actualRow.removeClass('rowColorManagedCarePending');
            $actualRow.removeClass('rowColorBothAuthAndManagedCarePending');
            $actualRow.removeClass('rowColorInitialized');
        } else if ($actualRow.find('.colStatus').html().indexOf('Ready for Preliminary') != -1) {
            $actualRow.addClass('rowColorReadyToPreliminary'); //at Ready for preliminary stage
        } else if ($actualRow.find('.colStatus').html().indexOf('Initialized') != -1) {
            $actualRow.addClass('rowColorInitialized'); //at Initialized stage
        } else if ($actualRow.find('.colStatus').html().indexOf('Authorization') != -1) {
            $actualRow.addClass('rowColorAuthrizationPending');
        } else if ($actualRow.find('.colStatus').html().indexOf('Managed Care') != -1) {
            $actualRow.addClass('rowColorManagedCarePending');
        } else if ($actualRow.find('.colStatus').html().indexOf('Authorization & Managed Care') != -1) {
            $actualRow.addClass('rowColorBothAuthAndManagedCarePending');
        } else if ($actualRow.find('.colStatus').html().indexOf('P') != -1) {
            $actualRow.addClass('rowColorPreliminary'); //at Preliminary Bill Stage
        } else {
            $actualRow.addClass('rowColor3');
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
                $("#CollapseBillActivitiesList").addClass("in").attr('style','height:auto;');
                BindList("#BillActivityListDiv", data);
                GetBillHeaderDetials(bHeaderId);
                $('#hidBillHeaderId').val(bHeaderId);
                var divId = '#CollapseBillActivitiesList';
                $('html, body').animate({
                    scrollTop: $(divId).offset().top
                }, 2000);
            },
            error: function (msg) {
            }
        });
    }
}

function OnChangeBillHeaderItem() {
    /// <summary>
    ///     Called when [change bill header item].
    /// </summary>
    /// <returns></returns>
    if ($('#ChkSelectAll')[0].checked)
        $('#ChkSelectAll')[0].checked = true;
    $("#ChkSelectAll .chkBillHeader").each(function () {
        if (this.checked == false) {
            status = false;
            $('#ChkSelectAll')[0].checked = false;
            return;
        }
    });
    RowColorBillHeader();
}

function CheckIsAuthorized() {
    /// <summary>
    ///     Checks the is authorized.
    /// </summary>
    /// <returns></returns>
    var enId = $("#hdEncounterId").val();
    var jsonData = JSON.stringify({ encounterId: enId });
    $.ajax({
        type: "POST",
        url: "/BillHeader/IsAuthorizedEncounter",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data == true) {
                isAuth = true;
            } else {
                var authLink = window.location.protocol + "//" + window.location.host + "/PatientSearch/PatientSearch?messageid=7";
                var manageCareLink = window.location.protocol + "//" + window.location.host + "/ManagedCare/Index";
                var msg = '<div class="billWarningDiv">Unable to send the Bill, Please check that encounter has authorization code <br/><a href="' + authLink + '" target="_blank">Add Authorization</a>' +
                    '<br/> and have managed care data <br/><a href="' + manageCareLink + '" target="_blank">Add Manage Care data</a></div>' +
                    '</div>';
                ShowCustomMessage(msg, "Warning", "warning", true);
                isAuth = false;
            }
        },
        error: function (msg) {
            isAuth = false;
        }
    });
}

function SetBillHeaderStatusForPreBill() {
    /// <summary>
    ///     Sets the bill header status for pre bill.
    /// </summary>
    /// <returns></returns>
    var ids = "";
    /*
    Owner: Amit Jain 
    On: 24122014
    Purpose: Earlier this screen was shown only by current Encounter but now it can be opened without Encounter. 
             Thats why, changes done below.
    */
    //CheckIsAuthorized();

    $("#BillHeaderListDiv table input:checked").each(function () {
        if (ids == '')
            ids = $(this)[0].id;
        else
            ids += "," + $(this)[0].id;
    });
    //if (isAuth && ids != "") {
    if (ids != "") {
        var enId = $("#hdEncounterId").val();
        var queryStringId = $("#hdId").val();
        var queryStringTypeId = $("#hdTypeId").val();
        var jsonData = JSON.stringify({ billHeaderIds: ids, oldStatus: "40", typeId: queryStringTypeId, id: queryStringId });
        $.ajax({
            type: "POST",
            url: "/BillHeader/SetBillHeaderStatusForPreBill",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#BillHeaderListDiv", data);
                $("#CollapseBillHeaderList").addClass("in");
                ShowMessage("Records processed Successfully", "Sucess", "success", true);
                RowColorBillHeader();
                window.location.reload();
            },
            error: function (msg) {
            }
        });
    } else {
        ShowMessage("Select the Bill Headers from the list below first!", "", "warning", true);
    }
}

//Refresh BillHeader List
function RefreshBillHeader(billHeaderId) {
    /// <summary>
    ///     Refreshes the bill header.
    /// </summary>
    /// <param name="billHeaderId">The bill header identifier.</param>
    /// <returns></returns>
    var typeId = $("#hdTypeId").val();
    var encounterId = 0;
    var patientId = 0;
    if (typeId > 0) {
        var id = $("#hdId").val();
        if (typeId == 2)
            patientId = id;
        else
            encounterId = id;
    }
    var jsonData = JSON.stringify({ billHeaderId: billHeaderId, patientId: patientId, encounterId: encounterId, });
    $.ajax({
        type: "POST",
        url: "/BillHeader/RefreshBillHeaderList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                BindList("#BillHeaderListDiv", data);
                //RowColorBillHeader();
                ShowMessage("Records Updated Successfully.", "Sucess", "success", true);
            }
        },
        error: function (msg) {
        }
    });
}

//-----------Currently not in use-----------------

function SendClaims(urlAction) {
    /// <summary>
    ///     Sends the claims.
    /// </summary>
    /// <param name="urlAction">The URL action.</param>
    /// <returns></returns>
    var ids = "";
    var enId = $("#hdEncounterId").val();
    CheckIsAuthorized();
    if (isAuth) {
        $("#BillHeaderListDiv table input:checked").each(function () {
            if (ids == '')
                ids = $(this)[0].id;
            else
                ids += "," + $(this)[0].id;
        });
        if (ids != '') {
            var jsonData = JSON.stringify({ billHeaderIds: ids });
            $.ajax({
                type: "POST",
                url: "/BillHeader/UpdateBillHeaderStatus",
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                data: jsonData,
                success: function (data) {
                    BindList("#BillHeaderListDiv", data);
                    $("#CollapseBillHeaderList").addClass("in");
                    ShowMessage("Records Updated Successfully.", "Sucess", "success", true);
                    RowColorBillHeader();
                },
                error: function (msg) {
                }
            });
        }
    }
}

function ApplyBillCharges() {
    /// <summary>
    ///     Applies the bill charges.
    /// </summary>
    /// <returns></returns>
    var enId = $("#hdEncounterId").val();
    if (enId > 0) {
        var jsonData = JSON.stringify({ encounterId: enId, applyBedCharges: true });
        $.ajax({
            type: "POST",
            url: "/BillHeader/RefreshBillCharges",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data != null) {
                    BindList("#BillHeaderListDiv", data);
                    RowColorBillHeader();
                    ShowMessage("Records Updated Successfully.", "Sucess", "success", true);
                }
            },
            error: function (msg) {
            }
        });
    }
}

//-----------Currently not in use-----------------

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


function SortBillHeaderGrid(event) {
    var url = "/BillHeader/SortBillHeaderGrid";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $('#BillHeaderListDiv').empty().html(data);
        },
        error: function (msg) {
        }
    });
}

function SortBillActivityGrid(event) {
    var url = "/BillHeader/GetBillActivitiesByBillHeaderId";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?billHeaderId=" + $('#hidBillHeaderId').val() + "&" + event.data.msg;
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