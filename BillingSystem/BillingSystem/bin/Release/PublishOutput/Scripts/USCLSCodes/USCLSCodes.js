

function SaveUSCLSCodes() {
    
    var txtCodeTableNumber = $("#txtCodeTableNumber").val();
    var txtCodeTableDescription = $("#txtCodeTableDescription").val();
    var txtCodeNumbering = $("#txtCodeNumbering").val();
    var txtCodeDescription = $("#txtCodeDescription").val();
    var txtCodePrice = $("#txtCodePrice").val();
    var dtCodeEffectiveDate = $("#dtCodeEffectiveDate").val();
    var dtCodeExpiryDate = $("#dtCodeExpiryDate").val();
    var txtCodeBasicProductApplicationRule = $("#txtCodeBasicProductApplicationRule").val();
    var txtCodeOtherProductsApplicationRule = "txtCodeOtherProductsApplicationRule";//$("#txtOrderedDateTime").val();
    var txtCodeServiceMainCategory = $("#txtCodeServiceMainCategory").val();
    var txtCodeServiceCodeSubCategory = $("#txtCodeServiceCodeSubCategory").val();
    var txtCodeUSCLSChapter = $("#txtCodeUSCLSChapter").val();
    var txtUSCLSToothNumber = $("#txtUSCLSToothNumber").val();
    var jsonData = JSON.stringify({
        CodeTableNumber: txtCodeTableNumber,
        CodeNumbering: txtCodeTableDescription,
        ServiceCodeValue: txtCodeNumbering,
        CodeDescription: txtCodeDescription,
        CodePrice: txtCodePrice,
        CodeEffectiveDate: dtCodeEffectiveDate,
        CodeExpiryDate: dtCodeExpiryDate,
        CodeBasicProductApplicationRule: txtCodeBasicProductApplicationRule,
        CodeOtherProductsApplicationRule: txtCodeOtherProductsApplicationRule,
        CodeServiceMainCategory: txtCodeServiceMainCategory,
        CodeServiceCodeSubCategory: txtCodeServiceCodeSubCategory,
        CodeUSCLSChapter: txtCodeUSCLSChapter,
        USCLSToothNumber: txtUSCLSToothNumber,
    });
    $.ajax({
        type: "POST",
        url: '/CPTCodes/SaveCPTCodes',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            
            if (data) {
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

function EditUSCLSCodes(id) {
    
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/USCLSCodes/GetUSCLSCodes',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            
            if (data) {
                $('#USCLSCodesDiv').empty();
                $('#USCLSCodesDiv').html(data);
                $('#collapseOne').addClass('in');
            } else {
            }
        },
        error: function (msg) {
        }
    });
}

function ViewUSCLSCodes(id) {
    
    var txtUSCLSCodesId = id;
    var jsonData = JSON.stringify({
        Id: txtUSCLSCodesId,
        ViewOnly: 'true'
    });
    $.ajax({
        type: "POST",
        url: '/USCLSCodes/GetUSCLSCodes',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            
            if (data) {
                $('#USCLSCodesDiv').empty();
                $('#USCLSCodesDiv').html(data);
                $('#collapseOne').addClass('in');
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

function DeleteUSCLSCodes(id) {
    
    var txtCPTCodesId = id;
    var jsonData = JSON.stringify({
        Id: txtCPTCodesId
    });
    $.ajax({
        type: "POST",
        url: '/CPTCodes/DeleteCPTCodes',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#divCPTCodes').empty();
                $('#divCPTCodes').html(data);
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

function ExportUSCLSCodes() {
    $.ajax({
        type: "POST",
        url: '/CPTCodes/ExportCPTCodesToExcel',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: '',
        success: function (data) {
        },
        error: function (msg) {
            return true;
        }
    });
}

function OnChangeGlobalCodeCategory() {
    var categoryId = $("#ddlGlobalCodeCategories").val();
    BindSubCategories(categoryId);
    return false;
}

function BindSubCategories(categoryId) {
    var jsonData = JSON.stringify({ categoryId: categoryId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/ServiceCode/BindSubCategories",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlGlobalCodes").empty();
            $("#ddlGlobalCodes").append('<option value="0">--Select One--</option>');
            
            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlGlobalCodes").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
            });
        },
        error: function (msg) {
            Console.log(msg);
        }
    });
    return false;
}

$(function () {
    $("#dtCodeEffectiveDate").datepicker({
        yearRange: "-130: +0",
        changeMonth: true,
        dateFormat: 'dd/mm/yy',
        changeYear: true
    });

    $("#dtCodeExpiryDate").datepicker({
        yearRange: "-130: +0",
        changeMonth: true,
        dateFormat: 'dd/mm/yy',
        changeYear: true
    });
});

