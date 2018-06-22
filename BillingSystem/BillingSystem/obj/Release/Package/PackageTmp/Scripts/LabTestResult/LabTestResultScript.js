$(function () {
    $("#LabTestResultFormDiv").validationEngine();
    LoadLabTestValues();
});

var     LoadLabTestValues = function() {
    BindGlobalCodesWithValue('#ddlLabTestResultSpecimen', 3105, '#hdLabTestResultSpecimen');
    BindGlobalCodesWithValue('#ddlLabTestResultAgeFrom', 3106, '#hdLabTestResultAgeFrom');
    BindGlobalCodesWithValue('#ddlLabTestResultAgeTo', 3106, '#hdLabTestResultAgeTo');
    BindGlobalCodesWithValue('#ddlLabTestResultGender', 3107, '#hdLabTestResultGender');
    BindGlobalCodesWithValue('#ddlLabTestResultMeasurementValue', 3108, '#hdLabTestResultMeasurementValue');
};

function SaveLabTestResult(id) {
    var isValid = jQuery("#LabTestResultFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtLabTestResultID = id;
        var txtLabTestResultTableNumber = $("#txtLabTestResultTableNumber").val();
        var txtLabTestResultTableName = $("#txtLabTestResultTableName").val();
        var txtLabTestResultCPTCode = $("#txtLabTestResultCPTCode").val();
        var txtLabTestResultTestName = $("#txtLabTestResultTestName").val();
        var txtLabTestResultSpecimen = $("#ddlLabTestResultSpecimen").val();
        var txtLabTestResultGender = $("#ddlLabTestResultGender").val();
        var txtLabTestResultAgeFrom = $("#ddlLabTestResultAgeFrom").val();
        var txtLabTestResultAgeTo = $("#ddlLabTestResultAgeTo").val();
        var txtLabTestResultMeasurementValue = $("#ddlLabTestResultMeasurementValue").val();
        var txtLabTestResultLowRangeResult = $("#txtLabTestResultLowRangeResult").val();
        var txtLabTestResultHighRangeResult = $("#txtLabTestResultHighRangeResult").val();
        var txtLabTestResultGoodFrom = $("#txtLabTestResultGoodFrom").val();
        var txtLabTestResultGoodTo = $("#txtLabTestResultGoodTo").val();
        var txtLabTestResultCautionFrom = $("#txtLabTestResultCautionFrom").val();
        var txtLabTestResultCautionTo = $("#txtLabTestResultCautionTo").val();
        var txtLabTestResultBadFrom = $("#txtLabTestResultBadFrom").val();
        var txtLabTestResultBadTo = $("#txtLabTestResultBadTo").val();

        var jsonData = JSON.stringify({
            LabTestResultID: txtLabTestResultID,
            LabTestResultTableNumber: txtLabTestResultTableNumber,
            LabTestResultTableName: txtLabTestResultTableName,
            LabTestResultCPTCode: txtLabTestResultCPTCode,
            LabTestResultTestName: txtLabTestResultTestName,
            LabTestResultSpecimen: txtLabTestResultSpecimen,
            LabTestResultGender: txtLabTestResultGender,
            LabTestResultAgeFrom: txtLabTestResultAgeFrom,
            LabTestResultAgeTo: txtLabTestResultAgeTo,
            LabTestResultMeasurementValue: txtLabTestResultMeasurementValue,
            LabTestResultLowRangeResult: txtLabTestResultLowRangeResult,
            LabTestResultHighRangeResult: txtLabTestResultHighRangeResult,
            LabTestResultGoodFrom: txtLabTestResultGoodFrom,
            LabTestResultGoodTo: txtLabTestResultGoodTo,
            LabTestResultCautionFrom: txtLabTestResultCautionFrom,
            LabTestResultCautionTo: txtLabTestResultCautionTo,
            LabTestResultBadFrom: txtLabTestResultBadFrom,
            LabTestResultBadTo: txtLabTestResultBadTo,
        });
        $.ajax({
            type: "POST",
            url: '/LabTestResult/SaveLabTestResult',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function(data) {
                ClearLabTestResultAll();
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

function EditLabTestResult(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/LabTestResult/GetLabTestResult',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $('#LabTestResultFormDiv').empty();
            $('#LabTestResultFormDiv').html(data);
            $('#collapseLabTestResultAddEdit').addClass('in');
            $("#LabTestResultFormDiv").validationEngine();
            LoadLabTestValues();
        },
        error: function (msg) {

        }
    });
}

function DeleteLabTestResult() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/LabTestResult/DeleteLabTestResult',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindLabTestResultGrid();
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
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
}

//function DeleteLabTestResult(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var jsonData = JSON.stringify({
//            Id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/LabTestResult/DeleteLabTestResult',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindLabTestResultGrid();
//                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//                }
//                else {
//                    return false;
//                }
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function BindLabTestResultGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/LabTestResult/BindLabTestResultList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#LabTestResultListDiv").empty();
            $("#LabTestResultListDiv").html(data);
        },
        error: function (msg) {
        }
    });
}

function ClearForm() {
    
}

function ClearLabTestResultAll() {
    $("#LabTestResultFormDiv").clearForm();
    $('#collapseLabTestResultAddEdit').removeClass('in');
    $('#collapseLabTestResultList').addClass('in');
    $("#LabTestResultFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $('.emptyddl').val('0');
    $('.emptytxt').val('');
    BindLabTestResultGrid();
}
function SortLabTestResult(event) {
    var url = '/LabTestResult/BindLabTestResultList';
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
            $("#LabTestResultListDiv").empty();
            $("#LabTestResultListDiv").html(data);
        },
        error: function (msg) {
        }
    });
}