$(function () {
    $("#PaymentTypeDetailFormDiv").validationEngine();
});

function SavePaymentTypeDetail(id) {
    var isValid = jQuery("#PaymentTypeDetailFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
             var txtId = $("#txtId").val();
             var txtPaymentType = $("#txtPaymentType").val();
             var txtCardNumber = $("#txtCardNumber").val();
             var txtExpiryMonth = $("#txtExpiryMonth").val();
             var txtExpiryYear = $("#txtExpiryYear").val();
             var txtCardHolderName = $("#txtCardHolderName").val();
             //var txtExtValue1 = $("#txtExtValue1").val();
             //var txtExtValue2 = $("#txtExtValue2").val();
             //var txtExtValue3 = $("#txtExtValue3").val();
             //var txtExtValue4 = $("#txtExtValue4").val();
             //var txtExtValue6 = $("#txtExtValue6").val();
             //var txtCreatedBy = $("#txtCreatedBy").val();
             //var dtCreatedDate = $("#dtCreatedDate").val();
        var jsonData = JSON.stringify({
             Id: txtId,
             PaymentType: txtPaymentType,
             CardNumber: txtCardNumber,
             ExpiryMonth: txtExpiryMonth,
             ExpiryYear: txtExpiryYear,
             CardHolderName: txtCardHolderName,
             //ExtValue1: txtExtValue1,
             //ExtValue2: txtExtValue2,
             //ExtValue3: txtExtValue3,
             //ExtValue4: txtExtValue4,
             //ExtValue6: txtExtValue6,
             //CreatedBy: txtCreatedBy,
             //CreatedDate: dtCreatedDate,
            //PaymentTypeDetailId: id,
            //PaymentTypeDetailMainPhone: txtPaymentTypeDetailMainPhone,
            //PaymentTypeDetailFax: txtPaymentTypeDetailFax,
            //PaymentTypeDetailLicenseNumberExpire: dtPaymentTypeDetailLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - PaymentTypeDetail
        });
        $.ajax({
            type: "POST",
            url: '/PaymentTypeDetail/SavePaymentTypeDetail',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearAll();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";

                ShowMessage(msg, "Success", "success", true);
            },
            error: function (msg) {

            }
        });
    }
}

function EditPaymentTypeDetail(id) {
    var jsonData = JSON.stringify({
             Id: txtId,
             PaymentType: txtPaymentType,
             CardNumber: txtCardNumber,
             ExpiryMonth: txtExpiryMonth,
             ExpiryYear: txtExpiryYear,
             CardHolderName: txtCardHolderName,
             //ExtValue1: txtExtValue1
             //ExtValue2: txtExtValue2
             //ExtValue3: txtExtValue3
             //ExtValue4: txtExtValue4
             //ExtValue6: txtExtValue6
             //CreatedBy: txtCreatedBy
             //CreatedDate: dtCreatedDate
               //Id: id
    });
    $.ajax({
        type: "POST",
        url: '/PaymentTypeDetail/GetPaymentTypeDetail',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $('#PaymentTypeDetailFormDiv').empty();
            $('#PaymentTypeDetailFormDiv').html(data);
            $('#collapsePaymentTypeDetailAddEdit').addClass('in');
            $("#PaymentTypeDetailFormDiv").validationEngine();
        },
        error: function (msg) {

        }
    });
}

function DeletePaymentTypeDetail() {
    if ($("#hfGlobalConfirmId").val() > 0) {
       var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val(),
            PaymentType: txtPaymentType,
            CardNumber: txtCardNumber,
            ExpiryMonth: txtExpiryMonth,
            ExpiryYear: txtExpiryYear,
            CardHolderName: txtCardHolderName,
           });
        $.ajax({
            type: "POST",
            url: '/PaymentTypeDetail/DeletePaymentTypeDetail',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindPaymentTypeDetailGrid();
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

//function DeletePaymentTypeDetail(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtPaymentTypeDetailId = id;
//        var jsonData = JSON.stringify({
//             Id: txtId,
//             PaymentType: txtPaymentType,
//             CardNumber: txtCardNumber,
//             ExpiryMonth: txtExpiryMonth,
//             ExpiryYear: txtExpiryYear,
//             CardHolderName: txtCardHolderName,
//            // ExtValue1: txtExtValue1
//            // ExtValue2: txtExtValue2
//            // ExtValue3: txtExtValue3
//            // ExtValue4: txtExtValue4
//            // ExtValue6: txtExtValue6
//            // CreatedBy: txtCreatedBy
//            // CreatedDate: dtCreatedDate
//            //Id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/PaymentTypeDetail/DeletePaymentTypeDetail',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindPaymentTypeDetailGrid();
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

function BindPaymentTypeDetailGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PaymentTypeDetail/BindPaymentTypeDetailList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#PaymentTypeDetailListDiv").empty();
            $("#PaymentTypeDetailListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearForm() {
    
}

function ClearAll() {
    $("#PaymentTypeDetailFormDiv").clearForm();
    $('#collapsePaymentTypeDetailAddEdit').removeClass('in');
    $('#collapsePaymentTypeDetailList').addClass('in');
    $("#PaymentTypeDetailFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/PaymentTypeDetail/ResetPaymentTypeDetailForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            if (data) {
                $('#PaymentTypeDetailFormDiv').empty();
                $('#PaymentTypeDetailFormDiv').html(data);
                $('#collapsePaymentTypeDetailList').addClass('in');
                BindPaymentTypeDetailGrid();
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




