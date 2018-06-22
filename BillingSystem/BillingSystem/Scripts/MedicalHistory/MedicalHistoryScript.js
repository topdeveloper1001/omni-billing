$(function () {
    $("#MedicalHistoryFormDiv").validationEngine();
    $(".collapseTitle")
        .bind("click",
        function () {
            $.validationEngine.closePrompt(".formError", true);
        });
});

function SaveMedicalHistory() {
    var isValid = jQuery("#MedicalHistoryFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var jsonData = JSON.stringify({
            Id: $("#hfCurrentMedicationId").val(),
            PatientId: $("#hdPatientId").val(),
            EncounterId: $("#hdCurrentEncounterId").val(),
            DrugName: $("#hfCurrentMedicationDrugCode").val(),
            Dosage: $("#ddlCurrentMedicationDosage").val().trim(),
            Frequency: $("#ddlCurrentMedicationFrequency").val(),
            Duration: $("#ddlCurrentMedicationDuration").val(),
            Volume: $("#ddlCurrentMedicationVolume").val(),
            IsDeleted: false
        });

        $.ajax({
            type: "POST",
            url: "/MedicalHistory/SaveMedicalHistory",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                var msg = "Records Saved successfully !";
                if ($("#hfCurrentMedicationId").val() > 0)
                    msg = "Records updated successfully";

                ClearAllMedicalHistory();
                BindList("#MedicalHistoryListDiv", data);
                ShowMessage(msg, "Success", "success", true);
            },
            error: function (msg) {

            }
        });
    }
}

function EditMedicalHistory(id) {
    var jsonData = JSON.stringify({
        medicalRecordId: id
    });
    $.ajax({
        type: "POST",
        url: "/MedicalHistory/GetMedicalHistory",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindCurrentMedicationDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteMedicalHistory() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val(),
            IsDeleted: true
        });
        $.ajax({
            type: "POST",
            url: "/MedicalHistory/SaveMedicalHistory",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindList("#MedicalHistoryListDiv", data);
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                }
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function ClearAllMedicalHistory() {
    $("#MedicalHistoryFormDiv").clearForm(true);
    $("#colCurrentMedicationAddEdit").removeClass("in");
    $("#colCurrentMedicationList").addClass("in");
    $("#btnCurrentMedicationsSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);

    if ($("#hfCurrentMedicationId").length > 0)
        $("#hfCurrentMedicationId").val(0);
}


///-------Smart Search DRUGs in the Current Medications starts here-----------
function SelectCurrentMedicationDrugCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtCurrentMedicationDrugCode").val(dataItem.Menu_Title);
    $("#hfCurrentMedicationDrugCode").val(dataItem.ID);
}

///-------Smart Search DRUGs in the Current Medications starts here-----------

function BindCurrentMedicationDetails(data) {
    $("#txtCurrentMedicationDrugCode").val(data.DrugDecription);
    $("#hfCurrentMedicationId").val(data.Id);
    $("#hdPatientId").val(data.PatientId);
    $("#hdCurrentEncounterId").val(data.EncounterId);
    $("#hfCurrentMedicationDrugCode").val(data.DrugName);
    $("#ddlCurrentMedicationDosage").val(data.Dosage.trim());
    $("#ddlCurrentMedicationFrequency").val(data.Frequency);
    $("#ddlCurrentMedicationDuration").val(data.Duration);
    $("#ddlCurrentMedicationVolume").val(data.Description);
    $("#btnCurrentMedicationsSave").val("Update");
    $("#colCurrentMedicationList").removeClass("in");
    $("#colCurrentMedicationAddEdit").addClass("in");
    $.validationEngine.closePrompt(".formError", true);
}