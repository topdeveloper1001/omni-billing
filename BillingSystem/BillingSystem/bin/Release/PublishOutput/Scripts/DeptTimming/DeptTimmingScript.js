$(function() {
    $("#DeptTimmingFormDiv").validationEngine();
});

function SaveDeptTimming(id) {
    var isValid = jQuery("#DeptTimmingFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtTimingId = $("#txtTimingId").val();
        var txtFacilityStructureID = $("#txtFacilityStructureID").val();
        var txtOpeningDayId = $("#txtOpeningDayId").val();
        var txtOpeningTime = $("#txtOpeningTime").val();
        var txtClosingTime = $("#txtClosingTime").val();
        var txtTrunAroundTime = $("#txtTrunAroundTime").val();
        var txtIsActive = $("#txtIsActive").val();
        var txtCreatedBy = $("#txtCreatedBy").val();
        var dtCreatedDate = $("#dtCreatedDate").val();
        var txtModifiedBy = $("#txtModifiedBy").val();
        var dtModifiedDate = $("#dtModifiedDate").val();
        var txtExtValue1 = $("#txtExtValue1").val();
        var txtExtValue2 = $("#txtExtValue2").val();
        var jsonData = JSON.stringify({
            TimingId: txtTimingId,
            FacilityStructureID: txtFacilityStructureID,
            OpeningDayId: txtOpeningDayId,
            OpeningTime: txtOpeningTime,
            ClosingTime: txtClosingTime,
            TrunAroundTime: txtTrunAroundTime,
            IsActive: txtIsActive,
            CreatedBy: txtCreatedBy,
            CreatedDate: dtCreatedDate,
            ModifiedBy: txtModifiedBy,
            ModifiedDate: dtModifiedDate,
            ExtValue1: txtExtValue1,
            ExtValue2: txtExtValue2,
            //DeptTimmingId: id,
            //DeptTimmingMainPhone: txtDeptTimmingMainPhone,
            //DeptTimmingFax: txtDeptTimmingFax,
            //DeptTimmingLicenseNumberExpire: dtDeptTimmingLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - DeptTimming
        });
        $.ajax({
            type: "POST",
            url: '/DeptTimming/SaveDeptTimming',
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
    }
}

function EditDeptTimming(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/DeptTimming/GetDeptTimming',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function(data) {
            $('#DeptTimmingFormDiv').empty();
            $('#DeptTimmingFormDiv').html(data);
            $('#collapseDeptTimmingAddEdit').addClass('in');
            $("#DeptTimmingFormDiv").validationEngine();
        },
        error: function(msg) {

        }
    });
}

function DeleteDeptTimming() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/DeptTimming/DeleteDeptTimming',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindDeptTimmingGrid();
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

//function DeleteDeptTimming(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtDeptTimmingId = id;
//        var jsonData = JSON.stringify({
//            Id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/DeptTimming/DeleteDeptTimming',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function(data) {
//                if (data) {
//                    BindDeptTimmingGrid();
//                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//                } else {
//                    return false;
//                }
//            },
//            error: function(msg) {
//                return true;
//            }
//        });
//    }
//}

function BindDeptTimmingGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/DeptTimming/BindDeptTimmingList",
        dataType: "html",
        async: true,
        data: null,
        success: function(data) {
            $("#DeptTimmingListDiv").empty();
            $("#DeptTimmingListDiv").html(data);
        },
        error: function(msg) {

        }

    });
}

function ClearForm() {

}

function ClearAll() {
    $("#DeptTimmingFormDiv").clearForm();
    $('#collapseDeptTimmingAddEdit').removeClass('in');
    $('#collapseDeptTimmingList').addClass('in');
    $("#DeptTimmingFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/DeptTimming/ResetDeptTimmingForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function(data) {
            if (data) {
                $('#DeptTimmingFormDiv').empty();
                $('#DeptTimmingFormDiv').html(data);
                $('#collapseDeptTimmingList').addClass('in');
                BindDeptTimmingGrid();
            } else {
                return false;
            }
        },
        error: function(msg) {
            return true;
        }
    });

}