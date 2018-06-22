$(function () {
    $("#PreSchedulingLinkFormDiv").validationEngine();
    BindCorporates('#ddlCorporateId', '#hdCorporateId');
    $('#ddlCorporateId').on('click', function() {
        BindFacilities();
    });
});

function SavePreSchedulingLink(id) {
    var isValid = jQuery("#PreSchedulingLinkFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var hdId = $("#hdId").val();
        var txtFacilityId = $("#ddlFacilityId").val();
        var txtCorporateId = $("#ddlCorporateId").val();
        var txtPublicLinkUrl = $("#txtPublicLinkUrl").val();
        var txtIsActive = true;
        var jsonData = JSON.stringify({
            Id: hdId,
            FacilityId: txtFacilityId,
            CorporateId: txtCorporateId,
            PublicLinkUrl: txtPublicLinkUrl,
            IsActive: txtIsActive,
        });
        $.ajax({
            type: "POST",
            url: '/PreSchedulingLink/SavePreSchedulingLink',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearPresechedulingLink();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";
                BindPreSchedulingLinkGrid();
                ShowMessage(msg, "Success", "success", true);
            },
            error: function (msg) {

            }
        });
    }
}

function EditPreSchedulingLink(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/PreSchedulingLink/GetPreSchedulingLink',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $('#ddlCorporateId').val(data.CorporateId);
            $('#hdFacilityId').val(data.FacilityId);
            BindFacilities();
            $('#ddlFacilityId').val(data.FacilityId);
            $('#hdId').val(data.Id);
            $('#txtPublicLinkUrl').val(data.PublicLinkUrl);
            $('.dis').attr('disabled', 'diabled');
            $('#collapsePreSchedulingLinkAddEdit').addClass('in');
        },
        error: function (msg) {

        }
    });
}

function DeletePreSchedulingLink() {
    if ($("#hfGlobalConfirmId").val() > 0) {
      var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/PreSchedulingLink/DeletePreSchedulingLink',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindPreSchedulingLinkGrid();
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

//function DeletePreSchedulingLink(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtPreSchedulingLinkId = id;
//        var jsonData = JSON.stringify({
//            Id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/PreSchedulingLink/DeletePreSchedulingLink',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindPreSchedulingLinkGrid();
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

function BindPreSchedulingLinkGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PreSchedulingLink/BindPreSchedulingLinkList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#PreSchedulingLinkListDiv").empty();
            $("#PreSchedulingLinkListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearForm() {
    
}

function ClearPresechedulingLink() {
    $("#PreSchedulingLinkFormDiv").clearForm(true);
    $('#collapsePreSchedulingLinkAddEdit').removeClass('in');
    $('#collapsePreSchedulingLinkList').addClass('in');
    $("#PreSchedulingLinkFormDiv").validationEngine();
    $('.dis').removeAttr('disabled', 'disabled');
}

function BindFacilities() {
    var ddlCorporateId = $('#ddlCorporateId').val();
    var jsonData = JSON.stringify({
        corporateid: ddlCorporateId != null ? ddlCorporateId : 0,
    });
    $.ajax({
        type: "POST",
        url: '/Home/GetFacilitiesbyCorporate',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindDropdownData(data, "#ddlFacilityId", "#hdFacilityId");
        },
        error: function (msg) {
        }
    });
}

function CreatePublicLink() {
    var isValid = jQuery("#PreSchedulingLinkFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        //var http = location.protocol;
        //var slashes = http.concat("//");
        //var host = slashes.concat(window.location.hostname);
        var full = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
        var host = full.concat("/PatientPreScheduling/Index");
        var htmlsitelink = host.concat("?CId=" + $('#ddlCorporateId').val() + "&FId=" + $('#ddlFacilityId').val());
        $('#txtPublicLinkUrl').val(htmlsitelink);
    }
}
