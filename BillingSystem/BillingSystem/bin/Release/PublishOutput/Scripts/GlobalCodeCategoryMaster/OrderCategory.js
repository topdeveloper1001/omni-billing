var controllerUrl = "/GlobalCodeCategoryMaster/";

$(document).ready(function () {
    $("#validate").validationEngine();
    $(".menu").click(function () {
        $(".out").toggleClass("in");
    });
});

function CheckDuplicateGlobalCodeCategory() {
    var isValid = jQuery("#validate").validationEngine({ returnIsValid: true });
    if (isValid) {
        var globalCodeCategoryId = $("#hfGlobalCodeCategoryID").val();
        var globalCodeCategoryName = $("#GlobalCodeCategoryName").val().trim();
        var jsonData = JSON.stringify({
            GlobalCodeCategoryName: globalCodeCategoryName,
            GlobalCodeCategoryId: globalCodeCategoryId
        });

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: controllerUrl + "CheckDuplicateGlobalCodeCategoryName",
            data: jsonData,
            dataType: "",
            beforeSend: function () { },
            success: function (data) {
                //Append Data to grid
                if (data != 'False') {
                    ShowMessage("Order category already exist!", "Alert", "info", true);
                }
                else {
                    AddGlobalCodeCategory();
                    return true;
                }
            },
            error: function (msg) {
            }
        });


        return false;
    }
}

function AddGlobalCodeCategory() {
    var isValid = jQuery("#validate").validationEngine({ returnIsValid: true });
    if (!isValid) {
        return false;
    }

    var globalCodeCategoryId = $("#hfGlobalCodeCategoryID").val();

    //  var facilityNumber = $("#ddlFacility").val();
    var globalCodeCategoryValue = $("#GlobalCodeCategoryValue").val();
    var globalCodeCategoryName = $("#GlobalCodeCategoryName").val().trim();
    var glNumber = $("#txtRevenueGeneralLedgerNumber").val().trim();
    var arMasterAccount = $("#txtARMasterAccount").val();
    var groupCode = 'CPT';

    var isActive;
    if ($('#chkActive').is(':checked'))
        isActive = true;
    else
        isActive = false;

    var jsonData = JSON.stringify({
        globalCodeCategoryId: globalCodeCategoryId,
        GlobalCodeCategoryName: globalCodeCategoryName,
        GlobalCodeCategoryValue: globalCodeCategoryValue,
        FacilityNumber: 0,
        GroupCode: groupCode,
        globalCodeCategory: globalCodeCategoryValue,
        IsActive: isActive,
        IsDeleted: false,
        ExternalValue1: glNumber,
        ExternalValue2: arMasterAccount,
        ExternalValue3: 'OrderCategory'
    });
    var msg = "";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: controllerUrl + "AddUpdateGlobalCodeCategory",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            //Append Data to grid
            if (data != null) {
                BindGlobalCodeCategoryList();
                ClearFields();
                $('#collapseTwo').addClass('in');
                msg = "Record Saved successfully !";
                if (globalCodeCategoryId > 0)
                    msg = "Record updated successfully";

                ShowMessage(msg, "Success", "success", true);
            }
        },
        error: function (msg) {

        }
    });
    return false;
}


function BindGlobalCodeCategoryList() {
    $.ajax({
        type: "POST",
        url: controllerUrl + 'GetGlobalCodeCategoryListOrderType',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        //data: jsonData,
        success: function (data) {
            if (data) {
                $('#GlobalCodeCategoryList').empty();
                $('#GlobalCodeCategoryList').html(data);
                $('#collapseTwo').addClass('in');
            } else {
            }
        },
        error: function (msg) {
        }
    });
}

function EditGlobalCodeCategory(GlobalCodeCategoryID) {
    var jsonData = JSON.stringify({ GlobalCodeCategoryId: GlobalCodeCategoryID });
    $.ajax({
        type: "POST",
        url: controllerUrl + 'EditGlobalCategoryCodeOrderType',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                $('#GlobalCodeCategoryInfo').empty();
                $('#GlobalCodeCategoryInfo').html(data);
                $("#validate").validationEngine();
                $('#collapseOne').addClass('in');
            } else {
            }
        },
        error: function (msg) {
        }
    });
}

function DeleteGlobalCodeCategory() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var url = controllerUrl + 'DeleteGlobalCodeCategoryOrderType';
        $.ajax({
            type: "POST",
            url: url,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({
                globalCodeCategoryId: $("#hfGlobalConfirmId").val()
            }),
            success: function (data) {
                if (data != null) {
                    ShowMessage("Category deleted successfully", "Warning", "warning", true);
                    ClearFields();
                    $('#GlobalCodeCategoryList').empty();
                    $('#GlobalCodeCategoryList').html(data);
                    $('#collapseTwo').addClass('in');
                }
                else {
                }
            },
            error: function (msg) {
            }
        });
    }
}

function ClearFields() {
    $('#GlobalCodeCategoryInfo').clearForm();
    $.validationEngine.closePrompt(".formError", true);
    $("#chkActive").prop('checked', true);
    $('#collapseOne').removeClass('in');
}


function SortOrderTypeCategoriesList(event) {
    var url = controllerUrl + "GetGlobalCodeCategoryListOrderType";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $('#GlobalCodeCategoryList').empty();
            $('#GlobalCodeCategoryList').html(data);
            $('#collapseTwo').addClass('in');
        },
        error: function (msg) {
        }
    });

    return false;
}




