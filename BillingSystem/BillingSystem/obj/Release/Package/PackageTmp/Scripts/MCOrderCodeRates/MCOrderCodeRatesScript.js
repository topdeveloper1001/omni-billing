$(function () {
    $("#MCOrderCodeRatesFormDiv").validationEngine();
});

function SaveMCOrderCodeRates(id) {
    var isValid = jQuery("#MCOrderCodeRatesFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtMCOrderCodeRatesId = $("#hdMCOrderCodeRatesId").val();
        var txtMCCode = $("#txtMCCode1").val();
        var txtOrderCodeRatTableNumber = $("#txtOrderCodeRatTableNumber").val();
        var txtOrderCodeRateTableName = $("#txtOrderCodeRateTableName").val();
        var txtOrderType = $("#ddlOrderType").val();
        var txtOrderCode = $("#txtOrderCode").val();
        var txtOrderCodeDescription = $("#txtOrderCodeDescription").val();
        var txtOrderCodePerDiemRate = $("#txtOrderCodePerDiemRate").val() === "" ? 0 : $("#txtOrderCodePerDiemRate").val();
        var txtOrderCodeAddOns = $("#txtOrderCodeAddOns").val();
        var txtOrderCodeAddOnTableNumber = $("#txtOrderCodeAddOnTableNumber").val();
        var jsonData = JSON.stringify({
            MCOrderCodeRatesId: txtMCOrderCodeRatesId,
            MCCode: txtMCCode,
            OrderCodeRatTableNumber: txtOrderCodeRatTableNumber,
            OrderCodeRateTableName: txtOrderCodeRateTableName,
            OrderType: txtOrderType,
            OrderCode: txtOrderCode,
            OrderCodeDescription: txtOrderCodeDescription,
            OrderCodePerDiemRate: txtOrderCodePerDiemRate,
            OrderCodeAddOns: txtOrderCodeAddOns,
            OrderCodeAddOnTableNumber: txtOrderCodeAddOnTableNumber,
        });
        $.ajax({
            type: "POST",
            url: '/MCOrderCodeRates/SaveMCOrderCodeRates',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function(data) {
                BindList("#MCOrderCodeRatesListDiv", data);
                ClearMCOrderCodeRatesForm();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";
                ShowMessage(msg, "Success", "success", true);
            },
            error: function(msg) {

            }
        });
    }
}

function EditMCOrderCodeRates(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/MCOrderCodeRates/GetMCOrderCodeRatesDetails',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindMCOrderCodeRatesDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteMCOrderCodeRates() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/MCOrderCodeRates/DeleteMCOrderCodeRates',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#MCOrderCodeRatesListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

//function DeleteMCOrderCodeRates(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var jsonData = JSON.stringify({
//            id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/MCOrderCodeRates/DeleteMCOrderCodeRates',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                BindList("#MCOrderCodeRatesListDiv", data);
//                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function ClearMCOrderCodeRatesForm() {
    var mccode = $('#txtMCCode1').val();
    $("#MCOrderCodeRatesFormDiv").clearForm(true);
    $('#collapseMCOrderCodeRatesAddEdit').removeClass('in');
    $('#collapseMCOrderCodeRatesList').addClass('in');
    $("#MCOrderCodeRatesFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
    $('#txtMCCode1').val(mccode);
}

function BindMCOrderCodeRatesDetails(data) {
    $("#btnSave").val("Update");
    $('#collapseMCOrderCodeRatesList').removeClass('in');
    $('#collapseMCOrderCodeRatesAddEdit').addClass('in');
    $("#MCOrderCodeRatesFormDiv").validationEngine();

    $("#hdMCOrderCodeRatesId").val(data.MCOrderCodeRatesId);
    $("#txtMCCode1").val(data.MCCode);
    $("#txtOrderCodeRatTableNumber").val(data.OrderCodeRatTableNumber);
    $("#txtOrderCodeRateTableName").val(data.OrderCodeRateTableName);
    $("#ddlOrderType").val(data.OrderType);
    $("#txtOrderCode").val(data.OrderCode);
    $("#txtOrderCodeDescription").val(data.OrderCodeDescription);
    $("#txtOrderCodePerDiemRate").val(data.OrderCodePerDiemRate);
    $("#txtOrderCodeAddOns").val(data.OrderCodeAddOns);
    $("#txtOrderCodeAddOnTableNumber").val(data.OrderCodeAddOnTableNumber);
    if ($("#ddlOrderType").val() == "8") {
        $('#divPerDiemRate').show();
    } else {
        $('#divPerDiemRate').hide();
    }
    $('#collapseMCOrderCodeRatesAddEdit').addClass('in');
    $('#collapseMCOrderCodeRatesAddEdit').attr('style', 'height:100%');
}

var ColorCodeOrderRateSteps = function () {
    //$("#McContractOrderCodeRateGrid tbody tr").each(function (i, row) {
    //    var $actualRow = $(row);
    //    if ($actualRow.find('.col3').html().indexOf("IF") != -1) {
    //        $actualRow.addClass('rowColor3');
    //    } else if ($actualRow.find('.col3').html().indexOf('THEN') != -1) {
    //        $actualRow.addClass('rowColor5');
    //    } else {
    //        $actualRow.removeClass('rowColor3');
    //        $actualRow.removeClass('rowColor5');
    //    }
    //});
};

var GetOrderCodeDesc = function () {
    var ordertype = $('#ddlOrderType').val();
    var ordercode = $('#txtOrderCode').val();
    if (ordertype != '' && ordertype != '0' && ordercode != '') {
        var jsonData = JSON.stringify({
            ordercode: ordercode,
            ordrtype: ordertype
        });
        $.ajax({
            type: "POST",
            url: '/Home/GetOrderCodeDesc',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data === "") {
                    ShowMessage("Enter valid order code!", "Info", "warning", true);
                } else {
                    $('#txtOrderCodeDescription').val(data);
                }
            },
            error: function(msg) {
                return true;
            }
        });
        if (ordertype == "8") {
            $('#divPerDiemRate').show();
        } else {
            $('#divPerDiemRate').hide();
        }
    }
};

var ViewOrderRateList = function(ordertype) {
    var jsonData = JSON.stringify({
        ordrtype: ordertype,
        mcCode: $("#txtMCCode1").val()
    });
    $.ajax({
        type: "POST",
        url: '/MCOrderCodeRates/ViewOrderRateListByType',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            BindList("#MCOrderCodeRatesListDiv", data);
        },
        error: function (msg) {
            return true;
        }
    });
};

var ResetOrderCode = function() {
    $('#txtOrderCode').val('');
    $('#txtOrderCodeDescription').val('');
    if ($('#ddlOrderType').val() == "99") {
        $('#divMcRulesAddEdit').show();
    } else {
        $('#divMcRulesAddEdit').hide();
    }
};