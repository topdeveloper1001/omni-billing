$(function () {
    $("#PatientPreSchedulingFormDiv").validationEngine();
});

function SavePatientPreScheduling(id) {
    var isValid = jQuery("#PatientPreSchedulingFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
             var txtPatientPreSchedulingId = $("#txtPatientPreSchedulingId").val();
             var txtPatientId = $("#txtPatientId").val();
             var txtEncounterId = $("#txtEncounterId").val();
             var txtFacilityId = $("#txtFacilityId").val();
             var txtPhysicianId = $("#txtPhysicianId").val();
             var txtPhysicianSpeciality = $("#txtPhysicianSpeciality").val();
             var txtTypeOfProcedure = $("#txtTypeOfProcedure").val();
             var txtCorporateId = $("#txtCorporateId").val();
             var txtDescription = $("#txtDescription").val();
             var txtCreatedBy = $("#txtCreatedBy").val();
             var dtCreatedDate = $("#dtCreatedDate").val();
             var txtModifiedBy = $("#txtModifiedBy").val();
             var dtModifiedDate = $("#dtModifiedDate").val();
             var txtIsActive = $("#txtIsActive").val();
             var txtDeletedBy = $("#txtDeletedBy").val();
             var dtDeletedDate = $("#dtDeletedDate").val();
             var txtExtValue1 = $("#txtExtValue1").val();
             var txtExtValue2 = $("#txtExtValue2").val();
             var txtExtValue3 = $("#txtExtValue3").val();
             var txtExtValue4 = $("#txtExtValue4").val();
             var txtExtValue5 = $("#txtExtValue5").val();
             var dtPreferredDate = $("#dtPreferredDate").val();
             var txtPreferredTimeSlots = $("#txtPreferredTimeSlots").val();
        var jsonData = JSON.stringify({
            PatientPreSchedulingId: txtPatientPreSchedulingId,
            PatientId: txtPatientId,
            EncounterId: txtEncounterId,
            FacilityId: txtFacilityId,
            PhysicianId: txtPhysicianId,
            PhysicianSpeciality: txtPhysicianSpeciality,
            TypeOfProcedure: txtTypeOfProcedure,
            CorporateId: txtCorporateId,
            Description: txtDescription,
            CreatedBy: txtCreatedBy,
            CreatedDate: dtCreatedDate,
            ModifiedBy: txtModifiedBy,
            ModifiedDate: dtModifiedDate,
            IsActive: txtIsActive,
            DeletedBy: txtDeletedBy,
            DeletedDate: dtDeletedDate,
            ExtValue1: txtExtValue1,
            ExtValue2: txtExtValue2,
            ExtValue3: txtExtValue3,
            ExtValue4: txtExtValue4,
            ExtValue5: txtExtValue5,
            PreferredDate: dtPreferredDate,
            PreferredTimeSlots: txtPreferredTimeSlots,
            //PatientPreSchedulingId: id,
            //PatientPreSchedulingMainPhone: txtPatientPreSchedulingMainPhone,
            //PatientPreSchedulingFax: txtPatientPreSchedulingFax,
            //PatientPreSchedulingLicenseNumberExpire: dtPatientPreSchedulingLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - PatientPreScheduling
        });
        $.ajax({
            type: "POST",
            url: '/PatientPreScheduling/SavePatientPreScheduling',
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

function EditPatientPreScheduling(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/PatientPreScheduling/GetPatientPreScheduling',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $('#PatientPreSchedulingFormDiv').empty();
            $('#PatientPreSchedulingFormDiv').html(data);
            $('#collapsePatientPreSchedulingAddEdit').addClass('in');
            $("#PatientPreSchedulingFormDiv").validationEngine();
        },
        error: function (msg) {

        }
    });
}

function DeletePatientPreScheduling() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/PatientPreScheduling/DeletePatientPreScheduling',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindPatientPreSchedulingGrid();
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

//function DeletePatientPreScheduling(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtPatientPreSchedulingId = id;
//        var jsonData = JSON.stringify({
//            Id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/PatientPreScheduling/DeletePatientPreScheduling',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindPatientPreSchedulingGrid();
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

function BindPatientPreSchedulingGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PatientPreScheduling/BindPatientPreSchedulingList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#PatientPreSchedulingListDiv").empty();
            $("#PatientPreSchedulingListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearForm() {
    
}

function ClearAll() {
    $("#PatientPreSchedulingFormDiv").clearForm();
    $('#collapsePatientPreSchedulingAddEdit').removeClass('in');
    $('#collapsePatientPreSchedulingList').addClass('in');
    $("#PatientPreSchedulingFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/PatientPreScheduling/ResetPatientPreSchedulingForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            if (data) {
                $('#PatientPreSchedulingFormDiv').empty();
                $('#PatientPreSchedulingFormDiv').html(data);
                $('#collapsePatientPreSchedulingList').addClass('in');
                BindPatientPreSchedulingGrid();
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