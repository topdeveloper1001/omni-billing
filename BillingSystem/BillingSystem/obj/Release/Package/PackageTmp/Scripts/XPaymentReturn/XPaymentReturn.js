$(function () {
    $("#XPaymentReturnFormDiv").validationEngine();
    $("#txtDenialCode").val($('#hdDenialCode').val());
    //BindPatients();
});

function BindPatients() {
    $.ajax({
        type: "POST",
        url: '/Home/GetPatientList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            BindDropdownData(data, "#ddlPatients", "0");
            var selectedValue = $("#hdPatientId").val();
            if (selectedValue > 0) {
                BindEncounters();
            }
        },
        error: function (msg) {
        }
    });
}

function SaveXPaymentReturn(id) {
    var isValid = jQuery("#collapseXPaymentReturnAddEdit").validationEngine({ returnIsValid: true });
    if (isValid == true && id != '0') {
        var txtDenialCode = $("#hdDenialCode").val();
        var txtPaymentReference = $("#txtPaymentReference").val();
        var dtDateSettlement = $("#txtDateSettlement").val();
        var jsonData = JSON.stringify({
            ID: id,
            DenialCode: txtDenialCode,
            PaymentReference: txtPaymentReference,
            DateSettlement: dtDateSettlement
        });
        $.ajax({
            type: "POST",
            url: '/XPaymentReturn/SaveXPaymentReturn',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function(data) {
                ClearAll();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";

                ShowMessage(msg, "Success", "success", true);
            },
            error: function(msg) {

            }
        });
    } else {
        ShowMessage('Unable to add record.', "Warning", "warning", true);
    }
}

function CancelXPaymentReturn() {
    $('#divClaimInfo').empty();
    $.validationEngine.closePrompt(".formError", true);
    window.location.href = window.location.protocol + "//" + window.location.host + "/Xclaim/XclaimMain?claimid=" + $('#hdClaimId').val() + "&encid=" + $('#hdEncounterId').val() + "&Pid=" + $('#hdPatientId').val();
}

function CancelXPayment() {
    $('#XPaymentDetailAddEditDiv').empty();
    $.validationEngine.closePrompt(".formError", true);
}

function SaveXPaymentDetailReturn(id) {
    var isValid = jQuery("#XPaymentDetailAddEditDiv").validationEngine({ returnIsValid: true });
    if (isValid == true && id != '0') {
        var txtDenialCode = $("#hdAADenialCode").val();
        var txtPaymentAmount = $("#txtAAPaymentAmount").val();
        var txtPatientShare = $("#txtPatientShare").val();
        var jsonData = JSON.stringify({
            XPaymentReturnID: id,
            AADenialCode: txtDenialCode,
            AAPaymentAmount: txtPaymentAmount,
        });
        $.ajax({
            type: "POST",
            url: '/XPaymentReturn/SaveXPaymentDetailReturn',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function(data) {
                BindGridData();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";
                ShowMessage(msg, "Success", "success", true);
                $('#divAddUpdateRemittanceDetail').hide();
            },
            error: function(msg) {
            }
        });
    } else {
        ShowMessage('Unable to add record.', "Warning", "warning", true);
    }
}

function EditXPaymentReturn(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/XPaymentReturn/GetXPaymentReturn',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $('#divAddUpdateRemittanceDetail').show();
            $('#XPaymentDetailAddEditDiv').empty();
            $('#XPaymentDetailAddEditDiv').html(data);
            if ($('#hdAADenialCodeName').val() != '') {
                $("#txtAADenialCode").val($('#hdAADenialCodeName').val());
            }
        },
        error: function (msg) {

        }
    });
}

function BindXPaymentReturnGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/XPaymentReturn/BindXPaymentReturnList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#XPaymentReturnListDiv").empty();
            $("#XPaymentReturnListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearAll() {
    $("#XPaymentReturnFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/XPaymentReturn/ResetXPaymentReturnForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            if (data) {
                BindGridData();
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

function GenerateClaim() {
    var isValid = jQuery("#collapseXPaymentReturnAddEdit").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var claimid = $('#ddlClaim').val();
        var jsonData = JSON.stringify({
            claimid: claimid
        });
        $.ajax({
            type: "POST",
            url: '/XPaymentReturn/GenerateRemittanceInfo',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function(data) {
                if (data) {
                    $('#divClaimInfo').empty();
                    $('#divClaimInfo').html(data);
                    if ($('#hdDenialCode').val() != '') {
                        $("#txtDenialCode").val($('#hdDenialCode').val());
                    }
                    InitializeDateTimePicker();
                    BindGridData();
                    $('#BtnGenerateXMLFile').show();
                }
            },
            error: function(msg) {
                return true;
            }
        });
    }
}

function SelectDenailCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtDenialCode").val(dataItem.Menu_Title);
    $("#hdDenialCode").val(dataItem.ID);
    $("#hdDenialCodeName").val(dataItem.Name);
}

function SelectAADenailCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtAADenialCode").val(dataItem.Menu_Title);
    $("#hdAADenialCode").val(dataItem.ID);
    $("#hdAADenialCodeName").val(dataItem.Name);
}

function ViewClaimDetails() {
    var isValid = jQuery("#collapseXPaymentReturnAddEdit").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var claimid = $('#hdClaimId').val();
        var jsonData = JSON.stringify({
            claimid: claimid
        });
        $.ajax({
            type: "POST",
            url: '/XPaymentReturn/GetRemittanceInfoByClaimId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function(data) {
                if (data) {
                    $('#divClaimInfo').empty();
                    $('#divClaimInfo').html(data);
                    if ($('#hdDenialCode').val() != '') {
                        $("#txtDenialCode").val($('#hdDenialCode').val());
                    }
                    InitializeDateTimePicker();
                    BindGridData();
                    $('#BtnGenerateXMLFile').show();
                }
            },
            error: function(msg) {
                return true;
            }
        });
    }
}

function BindGridData() {
    var claimid = $('#hdClaimId').val();
    var jsonData = JSON.stringify({
        claimid: claimid
    });
    $.ajax({
        type: "POST",
        url: '/XPaymentReturn/GetRemittanceInfoListByClaimId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#XPaymentReturnListDiv').empty();
                $('#XPaymentReturnListDiv').html(data);
            }
        },
        error: function (msg) {
            return true;
        }
    });
}

function GenerateXMLFile() {
    var isValid = jQuery("#collapseXPaymentReturnAddEdit").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        $.ajax({
            type: "POST",
            url: '/XPaymentReturn/GenerateRemittanceXmlFile',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            success: function(data) {
                if (data) {
                    ShowMessage("XML file successfully created.", "Success", "success", true);
                } else {
                    ShowMessage("Unable to generate XML File.", "Warning", "warning", true);
                }
                setTimeout(function() {
                    window.location.href = window.location.protocol + "//" + window.location.host + "/Xclaim/XclaimMain?claimid=" + $('#hdClaimId').val() + "&encid=" + $('#hdEncounterId').val() + "&Pid=" + $('#hdPatientId').val();
                }, 1000);
            },
            error: function(msg) {
                ShowMessage('Unable to generate XML File.', "Warning", "warning", true);
            }
        });
    }
}

function BindPatients() {
    $.ajax({
        type: "POST",
        url: '/Home/GetPatientList',
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

function BindEncounters() {
    var patientId = $("#ddlPatients").val();
    $('.emptyddl').empty();
    $('#BtnGenerateXMLFile').hide();
    $('#divClaimInfo').empty();
    $('#XPaymentDetailAddEditDiv').empty();
    $('#divAddUpdateRemittanceDetail').hide();
    $('#XPaymentReturnListDiv').empty();
    if (patientId > 0) {
        $.ajax({
            type: "POST",
            url: '/Home/GetEncountersListByPatientId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ patientId: patientId }),
            success: function (data) {
                BindDropdownData(data, "#ddlEncounters", "0");
                var selectedValue = $("#hdEncounterId").val();
                if (selectedValue > 0) {
                    GetBillHeaderListByEncounterId();
                }
            },
            error: function (msg) {
            }
        });
    }
}

function GetBillHeaderListByEncounterId() {
    var encounterId = $("#ddlEncounters").val();
    $('.emptyddl1').empty();
    $('#BtnGenerateXMLFile').hide();
    $('#divClaimInfo').empty();
    $('#XPaymentDetailAddEditDiv').empty();
    $('#divAddUpdateRemittanceDetail').hide();
    $('#XPaymentReturnListDiv').empty();
    if (encounterId > 0) {
        $.ajax({
            type: "POST",
            url: '/Home/GetBillHeaderListByEncounterId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ encounterId: encounterId }),
            success: function (data) {
                BindDropdownData(data, "#ddlBillHeaders", "0");
            },
            error: function (msg) {
            }
        });
    }
}

function BindClaimId() {
    var isValid = jQuery("#collapseXPaymentReturnAddEdit").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var billheaderId = $("#ddlBillHeaders").val();
        $('.emptyddl2').empty();
        $('#BtnGenerateXMLFile').hide();
        $('#divClaimInfo').empty();
        $('#XPaymentDetailAddEditDiv').empty();
        $('#divAddUpdateRemittanceDetail').hide();
        $('#XPaymentReturnListDiv').empty();
        if (billheaderId > 0) {
            $.ajax({
                type: "POST",
                url: '/BillHeader/GetClaimIdForBill',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({ billheaderId: billheaderId }),
                success: function(data) {
                    BindDropdownData(data, "#ddlClaim", "0");
                    $("#ddlClaim").empty();
                    var items = '<option value="0">--Select--</option>';
                    $.each(data, function (i, obj) {
                        var newItem = "<option id='" + obj.ClaimID + "'  value='" + obj.ClaimID + "'>" + obj.ClaimID + "</option>";
                        items += newItem;
                    });
                    $("#ddlClaim").html(items);
                    $("#ddlClaim").val("0");
                },
                error: function(msg) {
                }
            });
        }
    }
}



