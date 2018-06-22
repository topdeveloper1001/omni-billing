$(function() {
    $("#HolidayPlannerDetailsFormDiv").validationEngine();
});

function SaveHolidayPlannerDetails(id) {
    var isValid = jQuery("#HolidayPlannerDetailsFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtId = $("#txtId").val();
        var txtHolidayPlannerId = $("#txtHolidayPlannerId").val();
        var dtHolidayDate = $("#dtHolidayDate").val();
        var txtIsActive = $("#txtIsActive").val();
        var txtCreatedBy = $("#txtCreatedBy").val();
        var dtCreatedDate = $("#dtCreatedDate").val();
        var txtDescription = $("#txtDescription").val();
        var txtIsActive = $("#txtIsActive").val();
        var jsonData = JSON.stringify({
            Id: txtId,
            HolidayPlannerId: txtHolidayPlannerId,
            HolidayDate: dtHolidayDate,
            IsActive: txtIsActive,
            CreatedBy: txtCreatedBy,
            CreatedDate: dtCreatedDate,
            Description: txtDescription,
            IsActive: txtIsActive,
            //HolidayPlannerDetailsId: id,
            //HolidayPlannerDetailsMainPhone: txtHolidayPlannerDetailsMainPhone,
            //HolidayPlannerDetailsFax: txtHolidayPlannerDetailsFax,
            //HolidayPlannerDetailsLicenseNumberExpire: dtHolidayPlannerDetailsLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - HolidayPlannerDetails
        });
        $.ajax({
            type: "POST",
            url: '/HolidayPlannerDetails/SaveHolidayPlannerDetails',
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

function EditHolidayPlannerDetails(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/HolidayPlannerDetails/GetHolidayPlannerDetails',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function(data) {
            $('#HolidayPlannerDetailsFormDiv').empty();
            $('#HolidayPlannerDetailsFormDiv').html(data);
            $('#collapseHolidayPlannerDetailsAddEdit').addClass('in');
            $("#HolidayPlannerDetailsFormDiv").validationEngine();
        },
        error: function(msg) {

        }
    });
}

function DeleteHolidayPlannerDetails(id) {
    if (confirm("Do you want to delete this record? ")) {
        var txtHolidayPlannerDetailsId = id;
        var jsonData = JSON.stringify({
            Id: id
        });
        $.ajax({
            type: "POST",
            url: '/HolidayPlannerDetails/DeleteHolidayPlannerDetails',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function(data) {
                if (data) {
                    BindHolidayPlannerDetailsGrid();
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                } else {
                    return false;
                }
            },
            error: function(msg) {
                return true;
            }
        });
    }
}

function BindHolidayPlannerDetailsGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/HolidayPlannerDetails/BindHolidayPlannerDetailsList",
        dataType: "html",
        async: true,
        data: null,
        success: function(data) {
            $("#HolidayPlannerDetailsListDiv").empty();
            $("#HolidayPlannerDetailsListDiv").html(data);
        },
        error: function(msg) {

        }

    });
}

function ClearForm() {

}

function ClearAll() {
    $("#HolidayPlannerDetailsFormDiv").clearForm();
    $('#collapseHolidayPlannerDetailsAddEdit').removeClass('in');
    $('#collapseHolidayPlannerDetailsList').addClass('in');
    $("#HolidayPlannerDetailsFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/HolidayPlannerDetails/ResetHolidayPlannerDetailsForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function(data) {
            if (data) {
                $('#HolidayPlannerDetailsFormDiv').empty();
                $('#HolidayPlannerDetailsFormDiv').html(data);
                $('#collapseHolidayPlannerDetailsList').addClass('in');
                BindHolidayPlannerDetailsGrid();
            } else {
                return false;
            }
        },
        error: function(msg) {


            return true;
        }
    });

}