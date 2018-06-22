$(function () {
    $("#CAppTypeForm").validationEngine();
    $("#btnSave").click(SaveClinicianAppType);
    $("#btnCancel").click(ClearClinicianATForm);

    BindClinicianATDataOnLoad();
    ClearClinicianATForm();
});

function SaveClinicianAppType() {
    /// <summary>
    /// Saves the physician.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var isValid = jQuery("#CAppTypeForm").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var appList = [];
        $('#checkBox_Facilities input:checked').each(function () {
            appList.push({ "Text": $(this)[0].name, "Value": $(this)[0].value });
        });

        if (appList.length == 0) {
            ShowMessage("At least 1 Appointment Type has to be associated with the Clinical Staff!!", "Warning", "warning", true);
            return;
        }

        var clinicianId = $("#ddlClinician").val();
        var jsonData = JSON.stringify({
            Id: $("#CurrentClinician_Id").val(),
            ClinicianId: $("#ddlClinician").val(),
            AppointmentTypes: appList
        });
        $.ajax({
            type: "POST",
            url: '/Physician/SaveClinicianAppTypeData',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindClinicianAppointmentTypesData();
                    ClearClinicianATForm();

                    var msg = "Records Saved successfully !";
                    ShowMessage(msg, "Success", "success", true);
                }
                else
                    ShowMessage(msg, "Error", "error", false);
            },
            error: function (msg) {
            }
        });
    }

}

function EditClinicianAppointmentTypesData(id) {
    /// <summary>
    /// Edits the physician.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        clinicianId: id,
    });
    $.ajax({
        type: "POST",
        url: '/Physician/EditClinicianAppointmentTypesData',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindCAppTypesDetailsInEditMode(data);
        },
        error: function (msg) {
        }
    });
}

function BindClinicianAppointmentTypesData() {
    /// <summary>
    /// Binds the physician grid.
    /// </summary>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Physician/BindClinicianAppointmentTypesData",
        dataType: "html",
        async: true,
        success: function (data) {
            $("#divClinicianAppList").empty();
            $("#divClinicianAppList").html(data);
        },
        error: function (msg) {
            //alert(msg);
        }
    });
}

function ClearClinicianATForm() {
    /// <summary>
    /// Clears all.
    /// </summary>
    /// <returns></returns>
    $("#CAppTypeForm").clearForm(true);
    $('#cForm').removeClass('in');
    $('#cForm1').addClass('in');
    $('#btnSave').val('Save');
    $.validationEngine.closePrompt(".formError", true);
    $(".dis").attr("disabled", false);
}

function BindCAppTypesDetailsInEditMode(data) {
    $(".dis").attr("disabled", true);

    $('input[type=checkbox]:checked').each(function () {
        this.checked = false;
    });

    var sAppTypes = data.AppointmentTypes;

    if (sAppTypes != null) {
        $("#checkBox_Facilities").find("input[type=checkbox]").each(function () {
            var id = $(this).val();
            var arr = $.grep(sAppTypes, function (n) {
                return (n.Value === id);
            });

            if (arr.length > 0) {
                $(this).prop("checked", "checked");
            }
        });
    }

    $("#CurrentClinician_ClinicianId").val(data.Id);
    $("#ddlClinician").val(data.ClinicianId);
    $("#btnSave").val("Update");
    $('#cForm').addClass('in');
}

function BindClinicianATDataOnLoad() {
    $.getJSON("/Physician/BindClinicianAppointmentDataOnLoad", {}, function (data) {
        if (data != null) {
            BindDropdownData(data.Physicians, "#ddlClinician", '');
            BindList("#divAppointments", data.facilityListView);
            $("#divAppointments ul li").toggleClass('col-sm-12 col-sm-4');
        }
    });
}

function DeleteClinicianAppointmentTypeAssigned() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        $.post("/Physician/SaveClinicianAppTypeData", { Id: $("#hfGlobalConfirmId").val(), IsDeleted: true }, function (data) {
            if (data >= 0) {
                BindClinicianAppointmentTypesData();
                ShowMessage("Record Deleted Successfully!", "Success", "info", true);
            }
        });
    }
    return false;
}