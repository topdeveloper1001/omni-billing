$(function () {
    JsCalls();
});

function JsCalls() {

    /// <summary>
    /// Jses the calls.
    /// </summary>
    /// <returns></returns>
    $("#BedTransactionFormDiv").validationEngine();

    $(".collapseTitle").bind("click", function () {
        $.validationEngine.closePrompt(".formError", true);
    });

    $("#dtBedTransactionLicenseNumberExpire").datepicker({
        yearRange: "-130: +0",
        changeMonth: true,
        dateFormat: 'dd/mm/yy',
        changeYear: true
    });

    //-------------Added for Super Powers functionality-------------///
    var patientId = $("#PatientId").val();
    var encounterId = $("#EncounterId").val();

    $("#GlobalPatientId").val(patientId);
    $("#GlobalEncounterId").val(encounterId);
    BindLinkUrlsForSuperPowers();
}

function SaveBedTransaction(id) {
    /// <summary>
    /// Saves the bed transaction.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var isValid = jQuery("#BedTransactionFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtBedTransactionID = $("#txtBedTransactionID").val();
        var txtMappingPatientBedID = $("#txtMappingPatientBedID").val();
        var txtFacilityID = $("#txtFacilityID").val();
        var txtEncounterID = $("#txtEncounterID").val();
        var txtPatientID = $("#txtPatientID").val();
        var txtRoomNumber = $("#txtRoomNumber").val();
        var txtBedID = $("#txtBedID").val();
        var txtBedType = $("#txtBedType").val();
        var txtEffectiveDays = $("#txtEffectiveDays").val();
        var dtBedTransactionComputedOn = $("#dtBedTransactionComputedOn").val();
        var txtChargeUnit = $("#txtChargeUnit").val();
        var txtTransactionbracket = $("#txtTransactionbracket").val();
        var txtBedCharges = $("#txtBedCharges").val();
        var txtProcessStatus = $("#txtProcessStatus").val();
        var txtBillNumber = $("#txtBillNumber").val();
        var txtBillStatus = $("#txtBillStatus").val();
        var txtCreatedBy = $("#txtCreatedBy").val();
        var dtCreatedDate = $("#dtCreatedDate").val();
        var txtModifiedBy = $("#txtModifiedBy").val();
        var dtModifiedDate = $("#dtModifiedDate").val();
        var txtIsDeleted = $("#txtIsDeleted").val();
        var txtDeletedBy = $("#txtDeletedBy").val();
        var dtDeletedDate = $("#dtDeletedDate").val();
        var txtCorporateID = $("#txtCorporateID").val();
        //var txtBedTransactionFax = $("#txtBedTransactionFax").val();
        //var txtBedTransactionMainPhone = $("#txtBedTransactionMainPhone").val();


        //if (txtBedTransactionFax != '') {
        //    var countryCodeFax = $('#ddlCompanyFaxCode').val();
        //    txtBedTransactionFax = countryCodeFax + "-" + txtBedTransactionFax;
        //	}


        //var txtBedTransactionId = $("#hdBedTransactionId").val();
        // var dtBedTransactionLicenseNumberExpire = $("#dtBedTransactionLicenseNumberExpire").val();
        // 1MAPCOLUMNSHERE - BedTransaction


        var jsonData = JSON.stringify({
            BedTransactionID: txtBedTransactionID,
            MappingPatientBedID: txtMappingPatientBedID,
            FacilityID: txtFacilityID,
            EncounterID: txtEncounterID,
            PatientID: txtPatientID,
            RoomNumber: txtRoomNumber,
            BedID: txtBedID,
            BedType: txtBedType,
            EffectiveDays: txtEffectiveDays,
            BedTransactionComputedOn: dtBedTransactionComputedOn,
            ChargeUnit: txtChargeUnit,
            Transactionbracket: txtTransactionbracket,
            BedCharges: txtBedCharges,
            ProcessStatus: txtProcessStatus,
            BillNumber: txtBillNumber,
            BillStatus: txtBillStatus,
            CreatedBy: txtCreatedBy,
            CreatedDate: dtCreatedDate,
            ModifiedBy: txtModifiedBy,
            ModifiedDate: dtModifiedDate,
            IsDeleted: txtIsDeleted,
            DeletedBy: txtDeletedBy,
            DeletedDate: dtDeletedDate,
            CorporateID: txtCorporateID,
            //BedTransactionId: id,                                     ,
            //BedTransactionMainPhone: txtBedTransactionMainPhone,
            //BedTransactionFax: txtBedTransactionFax,
            //BedTransactionLicenseNumberExpire: dtBedTransactionLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - BedTransaction
        });
        $.ajax({
            type: "POST",
            url: '/BedTransaction/SaveBedTransaction',
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



function EditBedTransaction(id) {
    /// <summary>
    /// Edits the bed transaction.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var txtBedTransactionId = id;
    var jsonData = JSON.stringify({
        BedTransactionID: txtBedTransactionId
    });
    $.ajax({
        type: "POST",
        url: '/BedTransaction/GetBedTransaction',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#BedTransactionFormDiv').empty();
                $('#BedTransactionFormDiv').html(data);
                $('#collapseOne').addClass('in');
                JsCalls();
            } else {
            }
        },
        error: function (msg) {

        }
    });
}

function ViewBedTransaction(id) {

    /// <summary>
    /// Views the bed transaction.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var txtServiceCodeId = id;
    var jsonData = JSON.stringify({
        BedTransactionID: txtServiceCodeId
    });
    $.ajax({
        type: "POST",
        url: '/BedTransaction/GetBedTransaction',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {

            if (data) {
                $('#serviceCodeDiv').empty();
                $('#serviceCodeDiv').html(data);
                $('#collapseOne').addClass('in');
            } else {
            }
        },
        error: function (msg) {
        }
    });
}

function DeleteBedTransaction(id) {
    /// <summary>
    /// Deletes the bed transaction.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    if (confirm("Do you want to delete this record? ")) {
        var txtBedTransactionId = id;
        var jsonData = JSON.stringify({
            BedTransactionID: txtBedTransactionId
        });
        $.ajax({
            type: "POST",
            url: '/BedTransaction/DeleteBedTransaction',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindBedTransactionGrid();
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                } else {
                    return false;
                }
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function BindBedTransactionGrid() {
    /// <summary>
    /// Binds the bed transaction grid.
    /// </summary>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/BedTransaction/BindBedTransactionList",
        dataType: "html",
        async: true,
        // data: jsonData,
        success: function (data) {

            $("#BedTransactionListDiv").empty();
            $("#BedTransactionListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearForm() {
    /// <summary>
    /// Clears the form.
    /// </summary>
    /// <returns></returns>
    $("#BedTransactionFormDiv").clearForm();
    $('#collapseOne').removeClass('in');
    $('#collapseTwo').addClass('in');
}

function ClearAll() {
    /// <summary>
    /// Clears all.
    /// </summary>
    /// <returns></returns>
    ClearForm();
    $.validationEngine.closePrompt(".formError", true);
    var jsonData = JSON.stringify({
        BedTransactionID: 0
    });
    $.ajax({
        type: "POST",
        url: '/BedTransaction/ResetBedTransactionForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {

                $('#BedTransactionFormDiv').empty();
                $('#BedTransactionFormDiv').html(data);
                $('#collapseTwo').addClass('in');
                BindBedTransactionGrid();
            } else {
                return false;
            }
        },
        error: function (msg) {


            return true;
        }
    });

}


//function DeleteBedTransaction() {
//    if ($("#hfGlobalConfirmId").val() > 0) {
//        var txtBedTransactionId = id;
//        var jsonData = JSON.stringify({
//            BedTransactionID: $("#hfGlobalConfirmId").val()
//        });
//        $.ajax({
//            type: "POST",
//            url: '/BedTransaction/DeleteBedTransaction',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindBedTransactionGrid();
//                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//                } else {
//                    return false;
//                }
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

//function SortTransctionGrid(event) {
//    var url = "/BedTransaction/BindBedTransactionList";
  

//    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
//        url += "?" + "&" + event.data.msg;
//    }
//    $.ajax({
//        type: "POST",
//        url: url,
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        data: null,
//        success: function (data) {
//            $("#BedTransactionListDiv").empty();
//            $("#BedTransactionListDiv").html(data);

//        },
//        error: function (msg) {
//        }
//    });
//}