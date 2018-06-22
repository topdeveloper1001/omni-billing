$(GetDenialCodeById);

function EditBillHeader(bHeaderId) {
    /// <summary>
    /// Edits the bill header.
    /// </summary>
    /// <param name="bHeaderId">The b header identifier.</param>
    /// <returns></returns>
    if (bHeaderId > 0) {
        $.ajax({
            type: "POST",
            url: "/BillHeader/GetBillHeaderById",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({ billHeaderId: bHeaderId }),
            success: function (data) {
                $("#BillHeaderAddEditForm").empty();
                $("#BillHeaderAddEditForm").html(data);
                $("#CollapseBillHeaderAddEdit").addClass("in");
                InitializeDateTimePicker();
                $("#BillHeaderAddEditForm").validationEngine();
                $('#txtDenialCodes').val($('#hdDenialCodeDescritption').val());
            },
            error: function (msg) {
            }
        });
    }
}

function SaveManualPayment(bhId) {
    /// <summary>
    /// Saves the manual payment.
    /// </summary>
    /// <param name="bhId">The bh identifier.</param>
    /// <returns></returns>
    var isValid = jQuery("#BillHeaderAddEditForm").validationEngine({ returnIsValid: true });
    if (isValid) {
        var jsonData = JSON.stringify({
            DenialCode: $("#hdDenialCode").val(),
            PaymentReference: $("#txtPaymentReference").val(),
            DateSettlement: new Date($("#txtDateSettlement").val()),
            PaymentAmount: $("#txtPaymentAmount").val(),
            PatientPayReference: $("#txtPatientPayReference").val(),
            PatientDateSettlement: new Date($("#txtPatientDateSettlement").val()),
            PatientPayAmount: $("#txtPatientPayAmount").val(),
            BillHeaderID: bhId
        });
        $.ajax({
            type: "POST",
            url: "/BillHeader/SaveBillHeaderDetails",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearManualPaymentForm();
                //RefreshBillHeader(bhId);
            },
            error: function (msg) {
            }
        });
    }
}

function ValidateFile() {
    /// <summary>
    /// Validates the file.
    /// </summary>
    /// <returns></returns>
    if ($('#ImportXMLfile').val() == '') {
        ShowMessage('Select File first', "Alert", "warning", true);
        return false;
    }
    var isValid = jQuery(".validateUploadXml").validationEngine({ returnIsValid: true });
    if (!isValid) {
        ShowMessage('Invalid File Upload. Try again later!', "Alert", "warning", true);
        return false;
    }
    return true;
}

function ClearManualPaymentForm() {
    /// <summary>
    /// Clears the manual payment form.
    /// </summary>
    /// <returns></returns>
    $("#BillHeaderAddEditForm").empty();
    $("#CollapseBillHeaderAddEdit").removeClass("in");
    $("#CollapseBillHeaderList").addClass("in");
    GetDenialCodeById();
}

//function BindBillHeaderList() {
//    var enId = $("#hdEncounterId").val();
//    if (enId > 0) {
//        var jsonData = JSON.stringify({ encounterId: enId, applyBedCharges: false });
//        $.ajax({
//            type: "POST",
//            url: "/BillHeader/RefreshBillCharges",
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data != null) {
//                    BindList("#BillHeaderListDiv", data);
//                    ShowMessage("Records Updated Successfully.", "Sucess", "success", true);
//                    GetDenialCodeById();
//                    RowColor();
//                }
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

//Smart Search for Denial Codes
function SelectDenialCode(e) {
    /// <summary>
    /// Selects the denial code.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns></returns>
    var dataItem = this.dataItem(e.item.index());
    $("#txtDenialCodes").val(dataItem.Menu_Title);
    $("#hdDenialCode").val(dataItem.ID);
}

function GetDenialCodeById() {
    /// <summary>
    /// Gets the denial code by identifier.
    /// </summary>
    /// <returns></returns>
    var id = $("#hdDenialCode").val();
    if (id != undefined && id != null && id > 0) {
        var jsonData = JSON.stringify({ ErrorMasterID: id });
        $.ajax({
            type: "POST",
            url: "/ErrorMaster/GetDenialCodeById",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data != null) {
                    var ec = data.ErrorCode + " - " + data.ErrorDescription;
                    if (ec != '') {
                        $("#txtDenialCodes").val(ec);
                    }
                    $("#hdDenialCode").val(data.ErrorMasterID);
                }
            },
            error: function (msg) {
            }
        });

    }
}
