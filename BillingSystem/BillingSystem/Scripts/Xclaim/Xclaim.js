$(function () {
    $("#XclaimFormDiv").validationEngine();
    BindPatients();
});

function BindPatients() {
    $.ajax({
        type: "POST",
        url: '/Login/GetPatientList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            BindDropdownData(data, "#ddlPatients", "#hdPatientId");
            var selectedValue = $("#hdPatientId").val();
            if (selectedValue > 0) {
                BindEncounters();
            }
        },
        error: function (msg) {
        }
    });
}

function BindClaimGrid() {
  
    var pid = $('#ddlPatients').val();
    var eid = $('#ddlEncounters').val();
    var claimid = $('#ddlClaim').val() == null || $('#ddlClaim').val() == '' ? 0 : $('#ddlClaim').val();
    if (pid > 0) {
        var jsonData = JSON.stringify({
            pid: pid,
            eid: eid,
            claimid: claimid
        });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Xclaim/GetXClaimList",
            dataType: "html",
            async: true,
            data: jsonData,
            success: function(data) {
                $("#XclaimListDiv").empty();
                $("#XclaimListDiv").html(data);
            },
            error: function(msg) {

            }

        });
    }
}

function ClearForm() {
    
}

function ClearAll() {
    $("#XclaimFormDiv").clearForm();
    $('#collapseXclaimAddEdit').removeClass('in');
    $('#collapseXclaimList').addClass('in');
    $("#XclaimFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/Xclaim/ResetXclaimForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            if (data) {
                $('#XclaimFormDiv').empty();
                $('#XclaimFormDiv').html(data);
                $('#collapseXclaimList').addClass('in');
                BindClaimGrid();
            }
            else {
                return false;
            }
        },
        error: function (msg) {


            return true;
        }
    });

}

function BindClaimsDDL() {
    var encounterid = $("#ddlEncounters").val();
    if (encounterid > 0) {
        $.ajax({
            type: "POST",
            url: '/Xclaim/GetClaimsByEncounterId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ encounterid: encounterid }),
            success: function (data) {
                BindDropdownData(data, "#ddlClaim", "#hdClaimId");
                var selectedValue = $("#hdClaimId").val();
                if (selectedValue > 0) {
                    BindClaimGrid();
                }
            },
            error: function (msg) {
            }
        });
    }
}

//function ViewXML(id) {
//    window.location.href = window.location.protocol + "//" + window.location.host + "/XPaymentFileXML/XPaymentFileXMLMain?claimId=" + id;
//}

function ViewPaymentDetails(id,encounterid,patientid) {
    window.location.href = window.location.protocol + "//" + window.location.host + "/XPaymentReturn/XPaymentReturnMain?claimid=" + id + "&encid=" + encounterid + "&Pid=" + patientid;
}

function ApplyCharges(id) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Xclaim/ApplyChargesonFile",
        dataType: "json",
        async: true,
        data: JSON.stringify({ fileid : id }),
        success: function (data) {
            if (data) {
                ShowMessage("Charges applied successfully.", "Success", "success", true);
            } else {
                ShowMessage("Unable to apply charges.", "Warning", "warning", true);
            }
        },
        error: function (msg) {

        }

    });
}

function BindEncounters() {
    var patientId = $("#ddlPatients").val();
    $('.emptyddl').empty();
    if (patientId > 0) {
        $.ajax({
            type: "POST",
            url: '/Home/GetEncountersListByPatientId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ patientId: patientId }),
            success: function (data) {
                BindDropdownData(data, "#ddlEncounters", "#hdEncounterId");
                var selectedValue = $("#hdEncounterId").val();
                if (selectedValue > 0) {
                    BindClaimsDDL();
                }
            },
            error: function (msg) {
            }
        });
    }
}


function ViewXML(id) {
    var jsonData = JSON.stringify({
        id: id
    });
    $.ajax({
        type: "POST",
        url: '/Xclaim/ViewFile',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "text",
        data: jsonData,
        success: function (data) {
            if (data != '') {
                $('#txtXmlBillingView').val('');
                $('#txtXmlBillingView').val(data);
                $('#divhidepopup').show();
            } else {
                ShowMessage('Something went wrong in the xml. Please try again later!', "", "warning", true);
            }
        },
        error: function (response) {
        }
    });
}