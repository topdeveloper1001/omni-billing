$(function () {
    $("#XPaymentFileXMLFormDiv").validationEngine();
});

function ViewXMLFileData(id) {
    $.ajax({
        type: "POST",
        url: '/Home/GetBillHeaderListByEncounterId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ claimId: id }),
        success: function (data) {

        },
        error: function(msg) {
        }
    });
}

function ClearForm() {
    
}

function ClearAll() {
    $("#XPaymentFileXMLFormDiv").clearForm();
    $('#collapseXPaymentFileXMLAddEdit').removeClass('in');
    $('#collapseXPaymentFileXMLList').addClass('in');
    $("#XPaymentFileXMLFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/XPaymentFileXML/ResetXPaymentFileXMLForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            if (data) {
                $('#XPaymentFileXMLFormDiv').empty();
                $('#XPaymentFileXMLFormDiv').html(data);
                $('#collapseXPaymentFileXMLList').addClass('in');
                BindXPaymentFileXMLGrid();
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




