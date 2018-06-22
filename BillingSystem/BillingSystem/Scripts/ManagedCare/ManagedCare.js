$(function () {
    $("#managedCareDiv").validationEngine();
    $("#txtmanagedCareLicenseExpire").datepicker({
        yearRange: "-130: +0",
        changeMonth: true,
        dateFormat: 'dd/mm/yy',
        changeYear: true
    });

    BindCountryData("#ddlCountries", "#hdCountry");
});



function SavemanagedCare(id) {
    var isValid = jQuery("#ManagedCareDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtManagedCareInsuranceID = $("#txtManagedCareInsuranceID").val();
        var txtManagedCarePlanID = $("#txtManagedCarePlanID").val();
        var txtManagedCarePolicyID = $("#txtManagedCarePolicyID").val();
        var txtManagedCareMultiplier = $("#txtManagedCareMultiplier").val();
        var txtManagedCareInpatientDeduct = $("#txtManagedCareInpatientDeduct").val();
        var txtManagedCareOutpatientDeduct = $("#txtManagedCareOutpatientDeduct").val();
        var txtManagedCarePerDiems = $("#txtManagedCarePerDiems").val();

        var jsonData = JSON.stringify({
            ManagedCareTableID: id,
            ManagedCareInsuranceID: txtManagedCareInsuranceID,
            ManagedCarePlanID: txtManagedCarePlanID,
            ManagedCarePolicyID: txtManagedCarePolicyID,
            ManagedCareMultiplier: txtManagedCareMultiplier,
            ManagedCareInpatientDeduct: txtManagedCareInpatientDeduct,
            ManagedCareOutpatientDeduct: txtManagedCareOutpatientDeduct,
            ManagedCarePerDiems: txtManagedCarePerDiems
        });
        $.ajax({
            type: "POST",
            url: '/managedCare/SavemanagedCare',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindManagedCareGrid();
                    ClearAll();
                    var msg = "Records Saved successfully !";
                    if (id > 0)
                        msg = "Records updated successfully";
                    ShowMessage(msg, "Success", "success", true);
                }
                else {
                }
            },
            error: function (msg) {
            }
        });
    }
    return false;
}
function editDetails(e) {
   
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.ManagedCareTableID;
    EditmanagedCare(id);

}
function deleteDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.ManagedCareTableID;
    DeletemanagedCare(id);
}

function EditmanagedCare(id) {
    var jsonData = JSON.stringify({
        Id: id,
        ViewOnly: 'false'
    });
    $.ajax({
        type: "POST",
        url: '/ManagedCare/GetmanagedCare',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
           
            alert(data);
            if (data != null) {
                $('#managedCareDiv').empty();
                $('#managedCareDiv').html(data);
                //$('#managedCareDiv').removeClass('in');
                $('#collapseOne').addClass('in');
            }
        },
        error: function (msg) {
           
        }
    });
}

function ViewmanagedCare(id) {
    var txtmanagedCareId = id;
    var jsonData = JSON.stringify({
        Id: txtmanagedCareId,
        ViewOnly: 'true'
    });
    $.ajax({
        type: "POST",
        url: '/managedCare/GetmanagedCare',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#managedCareDiv').empty();
                $('#managedCareDiv').html(data);
                $('#collapseOne').addClass('in');
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

//function DeletemanagedCare() {
//    if ($("#hfGlobalConfirmId").val() > 0) {
//        var txtmanagedCareId = id;
//        var jsonData = JSON.stringify({
//            Id: txtmanagedCareId,
//            IsDeleted: true,
//            DeletedBy: 1,//Put logged in user id here
//            DeletedDate: new Date(),
//            IsActive: false
//        });
//        $.ajax({
//            type: "POST",
//            url: '/managedCare/DeletemanagedCare',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    $('#managedCareGrid').empty();
//                    $('#managedCareGrid').html(data);
//                    BindManagedCareGrid();
//                    ShowMessage("managedCare Deleted Successfully!", "Success", "info", true);
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

function DeletemanagedCare(id) {
    if (confirm("Do you want to delete this record? ")) {
        var txtmanagedCareId = id;
        var jsonData = JSON.stringify({
            Id: txtmanagedCareId,
            IsDeleted: true,
            DeletedBy: 1,//Put logged in user id here
            DeletedDate: new Date(),
            IsActive: false
        });
        $.ajax({
            type: "POST",
            url: '/managedCare/DeletemanagedCare',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    $('#managedCareGrid').empty();
                    $('#managedCareGrid').html(data);
                    BindManagedCareGrid();
                    ShowMessage("managedCare Deleted Successfully!", "Success", "info", true);
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
    return false;
}

function BindManagedCareGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/managedCare/BindManagedCareList",
        dataType: "html",
        async: true,
        // data: jsonData,
        success: function (data) {
            $("#managedCareGrid").empty();
            $("#managedCareGrid").html(data);
        },
        error: function (msg) {
            alert(msg);
        }

    });
}

function ClearForm() {
    $("#managedCareDiv").clearForm();
    $('#collapseOne').removeClass('in');
    $('#collapseTwo').addClass('in');
}

function ClearAll() {
    ClearForm();
    $.validationEngine.closePrompt(".formError", true);
    var jsonData = JSON.stringify({
        Id: 0,
    });
    $.ajax({
        type: "POST",
        url: '/managedCare/ResetmanagedCareForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                BindManagedCareGrid();
                $('#managedCareDiv').empty();
                $('#managedCareDiv').html(data);
                $('#collapseTwo').addClass('in');
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

