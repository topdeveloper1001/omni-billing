$(function () {
    $("#PatientCarePlanFormDiv").validationEngine();
    BindCarePlan();
});

function SavePatientCarePlan(id) {
    var isValid = jQuery("#PatientCarePlanFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtId = $("#hdPatientCareId").val();
             var txtPatientId = $("#txtPatientId").val();
             var txtCarePlanId = $("#ddlCarePlan").val();
             var txtTaskId = $("#ddlCareTask").val();
                var txtEncounterId = $("#txtEncounterId").val();
             var dtFromDate = $("#dtFromDate").val();
             var dtTillDate = $("#dtTillDate").val();
           
        var jsonData = JSON.stringify({
             Id: txtId,
            PatientId: txtPatientId,
             CarePlanId: txtCarePlanId,
             TaskId: txtTaskId,
             EncounterId: txtEncounterId,
             FromDate: dtFromDate,
             TillDate: dtTillDate,
           });
        $.ajax({
            type: "POST",
            url: '/PatientCarePlan/SavePatientCarePlan',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearPatientCarePlanForm();
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

function EditPatientCarePlan(id) {
    var jsonData = JSON.stringify({
             Id: id
            });
    $.ajax({
        type: "POST",
        url: '/PatientCarePlan/GetPatientCarePlan',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $('#PatientCarePlanFormDiv').empty();
            $('#PatientCarePlanFormDiv').html(data);
            $('#collapsePatientCarePlanAddEdit').addClass('in');
            $("#PatientCarePlanFormDiv").validationEngine();
        },
        error: function (msg) {

        }
    });
}

function DeletePatientCarePlan() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/PatientCarePlan/DeletePatientCarePlan',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindPatientCarePlanGrid();
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

//function DeletePatientCarePlan(id) {
//    if (confirm("Do you want to delete this record? ")) {
//      var jsonData = JSON.stringify({
//            Id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/PatientCarePlan/DeletePatientCarePlan',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindPatientCarePlanGrid();
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

function BindPatientCarePlanGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PatientCarePlan/BindPatientCarePlanList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#PatientCarePlanListDiv").empty();
            $("#PatientCarePlanListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearPatientCarePlanForm() {
    $("#PatientCarePlanFormDiv").clearForm();
    $('#collapsePatientCarePlanList').addClass('in');
    $("#PatientCarePlanFormDiv").validationEngine();
    $("#btnSavePatientCarePlan").val("Save");
}



//function ClearAll() {
//    $("#PatientCarePlanFormDiv").clearForm();
//    $('#collapsePatientCarePlanAddEdit').removeClass('in');
//    $('#collapsePatientCarePlanList').addClass('in');
//    $("#PatientCarePlanFormDiv").validationEngine();
//    $.validationEngine.closePrompt(".formError", true);
//    $.ajax({
//        type: "POST",
//        url: '/PatientCarePlan/ResetPatientCarePlanForm',
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        data: null,
//        success: function (data) {
//            if (data) {
//                $('#PatientCarePlanFormDiv').empty();
//                $('#PatientCarePlanFormDiv').html(data);
//                $('#collapsePatientCarePlanList').addClass('in');
//                BindPatientCarePlanGrid();
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



function BindCarePlan() {
    $.ajax({
        type: "POST",
        url: "/CarePlanTask/BindCarePlanDropdown",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $("#ddlCarePlan").empty();

            var items = '<option value="0">--Select--</option>';

            $.each(data, function (i, care) {
                items += "<option value='" + care.Value + "'>" + care.Text + "</option>";
            });

            $("#ddlCarePlan").html(items);
      
           
        },
        error: function (msg) {

        }
    });
}


function BindCarePlanTask() {
    var carePlanId = $("#ddlCarePlan").val();
    if (carePlanId > 0 && carePlanId != null) {
        var jsonData = JSON.stringify({
            careId: carePlanId
        });
        $.ajax({
            type: "POST",
            url: "/PatientCarePlan/BindCarePlanTask",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function(data) {
                $("#ddlCareTask").empty();

                var items = '<option value="0">--Select--</option>';

                $.each(data, function(i, care) {
                    items += "<option value='" + care.TaskNumber + "'>" + care.TaskDescription + "</option>";
                });

                $("#ddlCareTask").html(items);

            },
            error: function(msg) {

            }
        });
    } else {
        
    }

}