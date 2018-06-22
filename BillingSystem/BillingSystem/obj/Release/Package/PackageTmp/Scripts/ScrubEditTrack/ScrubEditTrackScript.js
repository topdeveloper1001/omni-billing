$(function () {
    $("#ScrubEditTrackFormDiv").validationEngine();
});



function SortScrubEditTrackGrid(event) {
    var url = "/ScrubEditTrack/GetScrubEditTrackList";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#ScrubEditTrackListDiv").empty();
            $("#ScrubEditTrackListDiv").html(data);

        },
        error: function (msg) {
        }
    });
}



//function SaveScrubEditTrack(id) {
//    var isValid = jQuery("#ScrubEditTrackFormDiv").validationEngine({ returnIsValid: true });
//    if (isValid == true) {
//             var txtScrubEditTrackID = $("#txtScrubEditTrackID").val();
//             var txtTrackRuleMasterID = $("#txtTrackRuleMasterID").val();
//             var txtTrackRuleStepID = $("#txtTrackRuleStepID").val();
//             var txtTrackType = $("#txtTrackType").val();
//             var txtTrackTable = $("#txtTrackTable").val();
//             var txtTrackColumn = $("#txtTrackColumn").val();
//             var txtTrackKeyColumn = $("#txtTrackKeyColumn").val();
//             var txtTrackValueBefore = $("#txtTrackValueBefore").val();
//             var txtTrackValueAfter = $("#txtTrackValueAfter").val();
//             var txtTrackKeyIDValue = $("#txtTrackKeyIDValue").val();
//             var txtTrackSide = $("#txtTrackSide").val();
//             var txtIsActive = $("#txtIsActive").val();
//             var txtCreatedBy = $("#txtCreatedBy").val();
//             var dtCreatedDate = $("#dtCreatedDate").val();
//             var txtCorporateId = $("#txtCorporateId").val();
//             var txtFacilityId = $("#txtFacilityId").val();
//        var jsonData = JSON.stringify({
//             ScrubEditTrackID: txtScrubEditTrackID
//             TrackRuleMasterID: txtTrackRuleMasterID
//             TrackRuleStepID: txtTrackRuleStepID
//             TrackType: txtTrackType
//             TrackTable: txtTrackTable
//             TrackColumn: txtTrackColumn
//             TrackKeyColumn: txtTrackKeyColumn
//             TrackValueBefore: txtTrackValueBefore
//             TrackValueAfter: txtTrackValueAfter
//             TrackKeyIDValue: txtTrackKeyIDValue
//             TrackSide: txtTrackSide
//             IsActive: txtIsActive
//             CreatedBy: txtCreatedBy
//             CreatedDate: dtCreatedDate
//             CorporateId: txtCorporateId
//             FacilityId: txtFacilityId
//            //ScrubEditTrackId: id,
//            //ScrubEditTrackMainPhone: txtScrubEditTrackMainPhone,
//            //ScrubEditTrackFax: txtScrubEditTrackFax,
//            //ScrubEditTrackLicenseNumberExpire: dtScrubEditTrackLicenseNumberExpire,
//            // 2MAPCOLUMNSHERE - ScrubEditTrack
//        });
//        $.ajax({
//            type: "POST",
//            url: '/ScrubEditTrack/SaveScrubEditTrack',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: jsonData,
//            success: function (data) {
//                ClearAll();
//                var msg = "Records Saved successfully !";
//                if (id > 0)
//                    msg = "Records updated successfully";

//                ShowMessage(msg, "Success", "success", true);
//            },
//            error: function (msg) {

//            }
//        });
//    }
//}

//function EditScrubEditTrack(id) {
//    var jsonData = JSON.stringify({
//             ScrubEditTrackID: txtScrubEditTrackID
//             TrackRuleMasterID: txtTrackRuleMasterID
//             TrackRuleStepID: txtTrackRuleStepID
//             TrackType: txtTrackType
//             TrackTable: txtTrackTable
//             TrackColumn: txtTrackColumn
//             TrackKeyColumn: txtTrackKeyColumn
//             TrackValueBefore: txtTrackValueBefore
//             TrackValueAfter: txtTrackValueAfter
//             TrackKeyIDValue: txtTrackKeyIDValue
//             TrackSide: txtTrackSide
//             IsActive: txtIsActive
//             CreatedBy: txtCreatedBy
//             CreatedDate: dtCreatedDate
//             CorporateId: txtCorporateId
//             FacilityId: txtFacilityId
//        Id: id
//    });
//    $.ajax({
//        type: "POST",
//        url: '/ScrubEditTrack/GetScrubEditTrack',
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        data: jsonData,
//        success: function (data) {
//            $('#ScrubEditTrackFormDiv').empty();
//            $('#ScrubEditTrackFormDiv').html(data);
//            $('#collapseScrubEditTrackAddEdit').addClass('in');
//            $("#ScrubEditTrackFormDiv").validationEngine();
//        },
//        error: function (msg) {

//        }
//    });
//}

//function DeleteScrubEditTrack(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtScrubEditTrackId = id;
//        var jsonData = JSON.stringify({
//             ScrubEditTrackID: txtScrubEditTrackID
//             TrackRuleMasterID: txtTrackRuleMasterID
//             TrackRuleStepID: txtTrackRuleStepID
//             TrackType: txtTrackType
//             TrackTable: txtTrackTable
//             TrackColumn: txtTrackColumn
//             TrackKeyColumn: txtTrackKeyColumn
//             TrackValueBefore: txtTrackValueBefore
//             TrackValueAfter: txtTrackValueAfter
//             TrackKeyIDValue: txtTrackKeyIDValue
//             TrackSide: txtTrackSide
//             IsActive: txtIsActive
//             CreatedBy: txtCreatedBy
//             CreatedDate: dtCreatedDate
//             CorporateId: txtCorporateId
//             FacilityId: txtFacilityId
//            Id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/ScrubEditTrack/DeleteScrubEditTrack',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindScrubEditTrackGrid();
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

//function BindScrubEditTrackGrid() {
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        url: "/ScrubEditTrack/BindScrubEditTrackList",
//        dataType: "html",
//        async: true,
//        data: null,
//        success: function (data) {
//            $("#ScrubEditTrackListDiv").empty();
//            $("#ScrubEditTrackListDiv").html(data);
//        },
//        error: function (msg) {

//        }

//    });
//}

//function ClearForm() {
    
//}

//function ClearAll() {
//    $("#ScrubEditTrackFormDiv").clearForm();
//    $('#collapseScrubEditTrackAddEdit').removeClass('in');
//    $('#collapseScrubEditTrackList').addClass('in');
//    $("#ScrubEditTrackFormDiv").validationEngine();
//    $.validationEngine.closePrompt(".formError", true);
//    $.ajax({
//        type: "POST",
//        url: '/ScrubEditTrack/ResetScrubEditTrackForm',
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        data: null,
//        success: function (data) {
//            if (data) {
//                $('#ScrubEditTrackFormDiv').empty();
//                $('#ScrubEditTrackFormDiv').html(data);
//                $('#collapseScrubEditTrackList').addClass('in');
//                BindScrubEditTrackGrid();
//            }
//            else {
//                return false;
//            }
//        },
//        error: function (msg) {


//            return true;
//        }
//    });

//}




