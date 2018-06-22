var cId = 0;
//$(function () {
//    $("#hdPayerId").val('');

//});
function ViewBillActivities(bHeaderId, urlAction) {
    /// <summary>
    /// Views the bill activities.
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
                BindList("#BillActivityListDiv", data);
                $("#CollapseBillActivitiesList").addClass("in");
                GetBillHeaderDetials(bHeaderId);
            },
            error: function () {
            }
        });
    }
}

//Older Code - Commented on 30122014
//function SendClaims() {
//    var ids = "";
//    $("#FinalBillsListDiv table input:checked").each(function () {
//        if (ids == '')
//            ids = $(this)[0].id;
//        else
//            ids += "," + $(this)[0].id;
//    });
//    if (ids != '') {
//        var jsonData = JSON.stringify({ billHeaderIds: ids });
//        $.ajax({
//            type: "POST",
//            url: "/BillHeader/SendEClaims",
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                BindList("#FinalBillsListDiv", data);
//                $("#CollapseBillHeaderList").addClass("in");
//                ShowMessage("E-Claims sent Successfully", "Sucess", "success", true);
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

//Modified on 30122014
//function SendClaimsByPayerIds(claimId, singlePayerId) {
//    var selected = '';
//    var count = 0;
//    $('#BillPayerClaimListDiv input:checked').each(function () {
//        selected += '' + $(this)[0].value + ',';
//        count++;
//    });
//    if (claimId == null || claimId == '')
//        claimId = 0;

//    var payerList = selected != '' ? selected.slice(0, -1) : '';

//    if (claimId > 0 && singlePayerId != '') {
//        payerList = singlePayerId;
//        cId = claimId;
//        count = 1;
//    }

//    if (payerList != '' && count > 0) {
//        var jsonData = JSON.stringify({
//            payerIds: payerList,
//            billHeaderId: claimId
//        });

//        $.ajax({
//            type: "POST",
//            url: "/BillHeader/SendEClaimsByPayerIds",
//            async: true,
//            cache: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: jsonData,
//            success: function (data) {
//                $('#BillActivityListDiv').empty();
//                $('#FinalBillsListDiv').empty();
//                BindList("#BillPayerClaimListDiv", data.payerClaimsView);
//                if (cId > 0) {
//                    BindList("#FinalBillsListDiv", data.billHeaderListView);
//                }
//                $("#CollapseBillHeaderList").addClass("in");
//                $("#CollapseBillPayerClaimsList").addClass("in");

//                //$('#BtnSendClaim').hide();
//                ShowMessage("E-Claims sent Successfully", "Sucess", "success", true);
//            },
//            error: function (msg) {
//                ShowMessage("Something went wrong while Claim Submissions. Try again later!", "Error", "error", true);
//            }
//        });
//    } else {
//        ShowMessage("Select at least One Payer from the list for CLAIM Submissions!", "Warning", "warning", true);
//    }
//}

var GetBillHeaderDetials = function (billheaderId) {
    if (billheaderId > 0) {
        $.ajax({
            type: "POST",
            url: "/BillHeader/GetBillHeaderDetails",
            async: true,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ billHeaderId: billheaderId }),
            success: function (data) {
                var str = " ( Bill Number:" + data.BillNumber + " )";
                $('#billnumberSpn').html(str);
            },
            error: function () {
            }
        });
    }
};

var ViewPreXMLFile = function (bHeaderId, facilityId) {
    $("#hdBHeaderId").val(bHeaderId);
    $("#hdfacilityId").val(facilityId);
    if (bHeaderId > 0) {
        $.ajax({
            type: "POST",
            url: '/BillHeader/GetPreXMLFile',
            async: true,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({ billHeaderId: bHeaderId, facilityId: facilityId }),
            success: function (data) {
                BindList("#BillActivityListDiv", data);
                $("#CollapseBillActivitiesList").addClass("in");
                $('#billnumberSpn').html("Claim View");
            },
            error: function () {
            }
        });
    }
};

//var BindClaimsByPayerInFinallBillsView = function (payerid) {
//    $("#BtnSelectAllClaim").removeAttr("disabled");
//    $("#hdPayerId").val(payerid);
//    if (payerid > 0) {
//        $.ajax({
//            type: "POST",
//            url: '/BillHeader/GetClaimsByPayerId',
//            async: true,
//            cache: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: JSON.stringify({ payerid: payerid }),
//            success: function (data) {
//                $('#FinalBillsListDiv').empty().html(data);
//                $('#BillActivityListDiv').empty();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

function SendClaimsOnDemand() {
    /// <summary>
    /// Sends the claims.
    /// </summary>
    /// <param name="Count">The count.</param>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        url: "/BillHeader/SendEClaims",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            //BindList("#FinalBillsListDiv", data);
            $("#FinalBillsListDiv").empty();
            $("#CollapseBillHeaderList").addClass("in");
            ShowMessage("E-Claims sent Successfully", "Sucess", "success", true);
            $('#BillActivityListDiv').empty();
            $('#BtnSendClaim').hide();
        },
        error: function () {
        }
    });
}









function SortViewPreXMLFile(event) {
    var hdHeaderId = $("#hdBHeaderId").val();
    var hdFacilityId = $("#hdfacilityId").val();
    var url = "/BillHeader/GetPreXMLFile";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?billHeaderId=" + hdHeaderId + "&facilityId=" + hdFacilityId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            BindList("#BillActivityListDiv", data);
            //$("#CollapseBillActivitiesList").addClass("in");
            //$('#billnumberSpn').html("Claim View");
        },
        error: function () {
        }
    });
}


function SelectAllFinalBills() {
    $("#FinalBillsListDiv input").removeAttr("disabled");
    $('#FinalBillsListDiv :checkbox:enabled').prop('checked', true);

}


function BindClaimsByPayerInFinallBillsView() {
    var selected = "0";
    $('#PayerEClaimGridContent input:checked').each(function () {
        selected = (selected == "0") ? $(this).attr('value') : (selected + ',' + $(this).attr('value'));
    });

    $("#BtnSelectAllClaim").removeAttr("disabled");
    //$("#hdPayerId").val(payerid);

    $.ajax({
        type: "POST",
        url: '/BillHeader/GetClaimsByPayerIdView',
        async: true,
        cache: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ payerIds: selected }),
        success: function (data) {
            $('#IpPatientFinalList').empty().html(data.inPatientView);
            $('#OutPatientFinalList').empty().html(data.outPatientView);
            $('#ErPatientFinalList').empty().html(data.erPatientView);
            $('.checkboxCls').prop('checked', selected != "0");
            $('.headerselect').prop('checked', selected != "0");

            $('#BillActivityListDiv').empty();

            $(".checkboxCls,.headerselect").click(function () {
                var checekdCheckBox = $("#FinalBillsListDiv input:checked").length;
                if (checekdCheckBox === 0) {
                    $(".check-boxInPayer").prop('checked', false);
                    BindClaimsByPayerInFinallBillsView();
                }
            });
        },
        error: function () {
        }
    });
    //}
}

function SetGridCheckBoxesInPateint() {
    var count = 1;
    $('.table_scroll_IP thead tr th').each(function () {
        if (count == 1) {
            this.innerHTML = "<input type='checkbox' id='chkHeaderInPatient' class='headerselect' title='Select All' />";
            count++;
        }
    });

    setTimeout(CheckBoxIsSelectedEventIP(), 500);
}

function CheckBoxIsSelectedEventIP() {
    $('#chkHeaderInPatient').on('click', function (e) {
        $(".check-boxInPatientList").prop('checked', e.target.checked);
    });

    $('.check-boxInPatientList').on('click', function (e) {
        var checkedCount = $('.check-boxInPatientList:checked').length;
        var classCount = $('.check-boxInPatientList').length;
        $("#chkHeaderInPatient").prop('checked', checkedCount == classCount);
    });
}

function SetGridCheckBoxesOutPatient() {
    var count = 1;
    $('.table_scroll_17 thead tr th').each(function () {
        if (count == 1) {
            this.innerHTML = "<input type='checkbox' id='chkHeaderOutPatient' class='headerselect' title='Select All' />";
            count++;
        }
    });

    setTimeout(CheckBoxIsSelectedEventOutPatient(), 500);
}

function CheckBoxIsSelectedEventOutPatient() {
    $('#chkHeaderOutPatient').on('click', function (e) {
        $(".check-boxOutPatient").prop('checked', e.target.checked);
    });

    $('.check-boxOutPatient').on('click', function (e) {
        var checkedCount = $('.check-boxOutPatient:checked').length;
        var classCount = $('.check-boxOutPatient').length;
        $("#chkHeaderOutPatient").prop('checked', checkedCount == classCount);
    });
}

function SendClaimsByPayerIds(claimId, singlePayerId) {
    var selected = '';
    var selectedInPatient = '';
    var selectedoutPatient = '';
    var selectedErPatinet = '';
    $('#BillPayerClaimListDiv input:checked').each(function () {
        if (selected != '') {
            selected += ',' + $(this)[0].value;
        }
        else
            selected = $(this)[0].value;
    });


    $('#IpPatientFinalList input:checked').each(function () {
        if ($(this).val() != 'on') {
            if (selectedInPatient != '') {
                selectedInPatient += ',' + $(this)[0].value;
            } else {
                selectedInPatient += $(this)[0].value;
            }
        }
    });

    $('#OutPatientFinalList input:checked').each(function () {
        if ($(this).val() != 'on') {
            if (selectedoutPatient != '') {
                selectedoutPatient += ',' + $(this)[0].value;
            } else {
                selectedoutPatient += $(this)[0].value;
            }
        }
    });


    $('#ErPatientFinalList input:checked').each(function () {
        if ($(this).val() != 'on') {
            if (selectedErPatinet != '') {
                selectedErPatinet += ',' + $(this)[0].value;
            } else {
                selectedErPatinet += $(this)[0].value;

            }
        }
    });

    var billheaderIds = "";
    if (selectedInPatient != "")
        billheaderIds += selectedInPatient;
    if (selectedoutPatient != "") {
        if (billheaderIds != "")
            billheaderIds += "," + selectedoutPatient;
        else
            billheaderIds += selectedoutPatient;
    }
    if (selectedErPatinet != "") {
        if (billheaderIds != "")
            billheaderIds += "," + selectedErPatinet;
        else
            billheaderIds += selectedErPatinet;
    }
    //var billheaderIds = selectedInPatient + ',' + selectedoutPatient + ',' + selectedErPatinet;

    if (claimId == null || claimId == '')
        claimId = 0;

    //var payerList = selected != '' ? selected.slice(0, -1) : '';
    //var newList = billheaderIds.slice(0, -1);

    //var newBillHeaderIds = newList.replace(/^,|,$/g, '');

    if (claimId > 0 && singlePayerId != '') {
        payerList = singlePayerId;
        cId = claimId;
    }

    if (selected != '') {
        var jsonData = JSON.stringify({
            payerId: selected,
            billHeaderIds: billheaderIds
        });

        $.ajax({
            type: "POST",
            url: "/BillHeader/SendEClaimsByPayerIds",
            async: true,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                //$('#BillActivityListDiv').empty();
                //$('#FinalBillsListDiv').empty();
                //BindList("#BillPayerClaimListDiv", data.payerClaimsView);
                $('#BillPayerClaimListDiv').empty().html(data.payerClaimsView);
                $('#IpPatientFinalList').empty().html(data.inPatientView);
                $('#OutPatientFinalList').empty().html(data.outPatientView);
                $('#ErPatientFinalList').empty().html(data.erPatientView);
                //$("#CollapseBillHeaderList").addClass("in");
                //$("#CollapseBillPayerClaimsList").addClass("in");

                //$('#BtnSendClaim').hide();
                ShowMessage("E-Claims sent Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                ShowMessage("Something went wrong while Claim Submissions. Try again later!", "Error", "error", true);
            }
        });
    } else {
        ShowMessage("Select at least One Payer from the list for CLAIM Submissions!", "Warning", "warning", true);
    }
}

function CheckBoxInPayerListInFinallBillsView(id) {
    $(".check-boxInPayer").prop('checked', false);
    var hdID = $("#hfCheckboxId").val();
    if (hdID == id) {
        $("#asignChkBox_" + id).prop('checked', false);
        $("#hfCheckboxId").val('');
    } else {
        $("#asignChkBox_" + id).prop('checked', 'checked');
        $("#hfCheckboxId").val(id);
    }
}

function SetGridCheckBoxesErPateint() {
    var count = 1;
    $('.table_scroll_15 thead tr th').each(function () {
        if (count == 1) {
            this.innerHTML = "<input type='checkbox' id='boxErPatient' class='headerselect' title='Select All' />";
            count++;
        }
    });

    setTimeout(CheckBoxIsSelectedEventErPatient(), 500);
}

function CheckBoxIsSelectedEventErPatient() {
    $('#boxErPatient').on('click', function (e) {
        $(".check-boxErPatient").prop('checked', e.target.checked);
    });

    $('.check-boxErPatient').on('click', function (e) {
        var checkedCount = $('.check-boxErPatient:checked').length;
        var classCount = $('.check-boxErPatient').length;
        $("#boxErPatient").prop('checked', checkedCount == classCount);
    });
}



/*Sortinig*/

//In Patient
function SortFinalBillInPatient(event) {
    //var payerId = $("#hdPayerId").val();
    var url = "/BillHeader/GetFinalBillByPayerHeadersList";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?payerId=0&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $('#IpPatientFinalList').empty().html(data.inPatientView);

        },
        error: function () {
        }
    });
}



//OutPatient
function SortFinalBillOutPatient(event) {
    //var payerId = $("#hdPayerId").val();
    var url = "/BillHeader/GetFinalBillByPayerHeadersList";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?payerId=0&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $('#OutPatientFinalList').empty().html(data.outPatientView);

        },
        error: function () {
        }
    });
}


//Er Patient
function SortFinalBillErPatient(event) {
    //var payerId = $("#hdPayerId").val();
    var url = "/BillHeader/GetFinalBillByPayerHeadersList";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?payerId=0&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $('#ErPatientFinalList').empty().html(data.erPatientView);
        },
        error: function () {
        }
    });
}

function SortBillActivityGrid(event) {
    var url = "/BillHeader/GetBillActivitiesByBillHeaderId";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?billHeaderId=" + $("#hdBHeaderId").val() + "&" + event.data.msg;
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


function GetBillPayerHeadersList(event) {
    var url = "/BillHeader/GetBillPayerHeadersList";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            //$('#BillPayerClaimListDiv').empty();
            //$("#BillActivityListDiv").empty();
            //BindList("#BillPayerClaimListDiv", data.payerclaimPartialView);
            //BindList("#FinalBillsListDiv", data.billHeaderPatialView);
            $('#BillPayerClaimListDiv').empty().html(data.payerclaimPartialView);
            $('#IpPatientFinalList').empty().html(data.inPatientView);
            $('#OutPatientFinalList').empty().html(data.outPatientView);
            $('#ErPatientFinalList').empty().html(data.erPatientView);


        },
        error: function () {
        }
    });
}


function SortFinalBillByPayerHeadersList(event) {
    //var payerId = $("#hdPayerId").val();
    var url = "/BillHeader/GetFinalBillByPayerHeadersList";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        //url += "?payerId=" + payerId + "&" + event.data.msg;
        url += "?payerId=0&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#BillActivityListDiv").empty();

            BindList("#FinalBillsListDiv", data);


        },
        error: function () {
        }
    });
}