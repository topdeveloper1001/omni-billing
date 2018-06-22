function GetMedicalRecordByPatientIDEncounterID() {
    var jsonData = JSON.stringify({ patientID: $("#hdPatientId").val(), encounterID: $('#hdCurrentEncounterId').val() });
    $.ajax({
        type: "POST",
        url: '/MedicalRecord/GetMedicalRecordByPatientEncounter',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (obj) {
            $.each(obj, function (i, record) {
                if (record.GlobalCode == '0') {
                    if (record.GlobalCodeCategoryID != '8102') {
                        $('#' + record.GlobalCodeCategoryID).attr("checked", true);
                        $('#txt' + record.GlobalCodeCategoryID).val(record.DetailAnswer);
                        $('#txt' + record.GlobalCodeCategoryID).show();
                    } else {
                        $('#' + record.GlobalCodeCategoryID).attr("checked", true);
                        UpdateAndBindOtherDrugAllergy(0, '');
                    }
                }
                else {
                    $('#' + record.GlobalCode).prop('checked', true);
                }
            });
        },
        error: function (msg) {
            console.log(msg);
        }
    });

}

function AddAllergies() {

    var txtEncounterID = $('#hdCurrentEncounterId').val();
    if (txtEncounterID != '' && txtEncounterID > 0) {
        var Selected = [];
        var GlobalCodeCategory = [];
        var detailedAnswer = [];
        $('#checkBox_Allergies .liMainCheckboxes input:checked').each(function () {
            var categoryValue = $(this).attr('gcc');
            if (categoryValue != '' && categoryValue != null) {
                detailedAnswer.push('');
                Selected.push($(this).attr('id'));
                GlobalCodeCategory.push($(this).attr('gcc'));
            }
        });

        $('#checkBox_Allergies .liOtherCheckboxes input:checked').each(function () {
            var categoryValue = $(this).attr('gcc');
            if (categoryValue != '' && categoryValue != null && categoryValue != '8102') {
                var chkOther = $('#' + categoryValue);
                var otherChkChecked = chkOther[0].checked;
                if (otherChkChecked != null && otherChkChecked && chkOther.val() == 0) {
                    detailedAnswer.push($('#txt' + categoryValue).val());
                    Selected.push(chkOther.val());
                    GlobalCodeCategory.push($(this).attr('gcc'));
                }
            }
        });

        if (Selected.length == 0) {
            alert('Please select Allergies/History before Save.');
            return false;
        }
        var txtPatientID = $("#hdPatientId").val();
        var txtMedicalRecordNumber = $("#hdPatientMRN").val();
        var jsonData = [];
        for (var i = 0; i < Selected.length; i++) {
            jsonData[i] = {
                'GlobalCodeCategoryID': GlobalCodeCategory[i],
                'GlobalCode': Selected[i],
                'PatientID': txtPatientID,
                'EncounterID': txtEncounterID,
                'MedicalRecordNumber': txtMedicalRecordNumber,
                'DetailAnswer': detailedAnswer[i]
            };
        };
        var jsonD = JSON.stringify({ list: jsonData, patientId: txtPatientID, encounterId: txtEncounterID });
        $.ajax({
            type: "POST",
            url: '/MedicalRecord/AddMedicalRecord',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonD,
            success: function (data) {
                ShowMessage("Records saved successfully", "Success", "success", true);
                $("#AlergyFormDiv").validationEngine();
                BindMedicalRecordGrid();
            },
            error: function (msg) {
            }
        });
    }
    else {
        ShowMessage("This Patient's Encounter doesn't start yet. Try again later!", "Alert", "warning", true);
        return false;
    }
}

function OnChangeOtherCheckBox(selector) {
    var checked = $('#' + selector)[0].checked;
    if (checked) {
        $('#txt' + selector).show();
    }
    else {
        $('#txt' + selector).hide();
    }
}

function BindMedicalRecordGrid() {
    var jsonData = JSON.stringify({
        patientId: $("#hdPatientId").val()
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/MedicalRecord/BindMedicalRecordList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#colCurrentAllergiesMain").empty();
            $("#colCurrentAllergiesMain").html(data);
            JsCallsAlergy();
        },
        error: function (msg) {

        }

    });
}

function JsCallsAlergy() {
    $("#AlergyFormDiv").validationEngine();
    BindAlergyType();
}

function BindAlergyType() {

    $.ajax({
        type: "POST",
        url: '/MedicalRecord/GetAlergyType',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        // data: jsonData,
        success: function (data) {
            $("#ddlAlergyType").empty();
            $("#ddlAlergyType").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlAlergyType").append('<option value="' + code.GlobalCodeCategoryValue + '">' + code.GlobalCodeCategoryName + '</option>');
            });
            if ($('#hdAlergyTypeID').val() != '') {
                $("#ddlAlergyType").val($('#hdAlergyTypeID').val());
                BindAlergy($('#hdAlergyTypeID').val());
            }
        },
        error: function (msg) {

        }
    });
}

function BindAlergy(alergyType) {
    var jsonData = JSON.stringify({
        GlobalCodeCategoryValue: alergyType
    });
    $.ajax({
        type: "POST",
        url: '/MedicalRecord/GetAlergies',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $("#ddlAlergyName").empty();
            $("#ddlAlergyName").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlAlergyName").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
            });
            if ($('#hdAlergyNameID').val() != '') {
                $("#ddlAlergyName").val($('#hdAlergyNameID').val());
            }

        },
        error: function (msg) {

        }
    });
}

function SaveAlergyRecord(id) {
    var isValid = jQuery("#AlergyFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtMedicalRecordId = $("#hdMedicalRecordID").val();
        var txtPatientId = $("#hdPatientId").val();
        var txtEncounterId = $("#hdCurrentEncounterId").val();
        var txtMedicalRecordNumber = $("#hdPatientMRN").val();
        var txtGlobalCodeCategoryId = $("#ddlAlergyType").val();
        var txtGlobalCode = $("#ddlAlergyName").val();
        var txtDetailAnswer = $("#txtDetailAnswer").val();
        var txtCommentsAlergy = $("#txtCommentsAlergy").val();
        var rbtnYesNoStatus = $("input:radio[name='CurrentAlergyShortAnswer']:checked").attr('id');
        var shortAnswer = false;
        if (rbtnYesNoStatus == 'rbYes')
            shortAnswer = true;

        var jsonData = JSON.stringify({
            MedicalRecordID: txtMedicalRecordId,
            PatientID: txtPatientId,
            EncounterID: txtEncounterId,
            MedicalRecordNumber: txtMedicalRecordNumber,
            GlobalCodeCategoryID: txtGlobalCodeCategoryId,
            GlobalCode: txtGlobalCode,
            ShortAnswer: shortAnswer,
            DetailAnswer: txtDetailAnswer,
            Comments: txtCommentsAlergy
        });
        $.ajax({
            type: "POST",
            url: '/MedicalRecord/SaveMedicalRecord',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {

                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";

                ShowMessage(msg, "Success", "success", true);
                ClearAllAlergy();

            },
            error: function (msg) {

            }
        });
    }
}

function EditMedicalRecord(id) {
    var txtMedicalRecordId = id;
    var jsonData = JSON.stringify({
        MedicalRecordID: txtMedicalRecordId
    });
    $.ajax({
        type: "POST",
        url: '/MedicalRecord/GetMedicalRecord',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#AlergyAddAdit').empty();
                $('#AlergyAddAdit').html(data);
                $('#collapseK').addClass('in').attr('style', '');
                JsCallsAlergy();
            }
            else {
            }
        },
        error: function (msg) {

        }
    });
}

function DeleteMedicalRecord() {
    if ($("#hfGlobalConfirmId").val() > 0) {
   var jsonData = JSON.stringify({
            MedicalRecordID: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/MedicalRecord/DeleteMedicalRecord',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    // BindMedicalRecordGrid();
                    ClearAllAlergy();
                    ShowMessage("Records Deleted Successfully", "Success", "success", true);
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

function ClearFormNotInUse() {
    $("#AlergyAddAdit").clearForm();
    $('#collapseOne').removeClass('in');
    $('#collapseTwo').addClass('in');
}

function ClearAllAlergy() {
    ClearForm();
    $.validationEngine.closePrompt(".formError", true);

    $.ajax({
        type: "POST",
        url: '/MedicalRecord/ResetMedicalRecordForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        //data: jsonData,
        success: function (data) {
            $('#AlergyAddAdit').empty();
            $('#AlergyAddAdit').html(data);
            $('#collapseK2').addClass('in');
            BindMedicalRecordGrid();
        },
        error: function (msg) {
            console.log(msg);
        }
    });

}

function ShowAllergies() {
    var jsonData = JSON.stringify({
        patientId: $("#hdPatientId").val()
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/MedicalRecord/BindMedicalRecordList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#PatientSummarydiv").empty();
            $("#PatientSummarydiv").html(data);
            $("#divhidepopup").show();
        },
        error: function (msg) {

        }

    });

}



/////---------------Current Medication Section Start here---------
function ShowMedicalHistory(con, isCurrentMedcations) {
    if (isCurrentMedcations == 1 && con.name != 'Hide') {
        BindDropdownsInCurrentMedications();
        $("#MedHistoryAddEdit").show();
        $("#MedHistoryList").show();
        con.name = 'Hide';
        $('#' + con.id).text('Hide');
    }
    else {
        $("#MedHistoryAddEdit").hide();
        $("#MedHistoryList").hide();
        $('#' + con.id).text('Current Medications');
        con.name = 'Current Medications';
    }
}


function BindDropdownsInCurrentMedications() {
    BindGlobalCodesWithValueWithOrder("#ddlCurrentMedicationDuration", 4801, "");
    BindGlobalCodesWithValueWithOrder("#ddlCurrentMedicationVolume", 4802, "");
    BindGlobalCodesWithValueWithOrder("#ddlCurrentMedicationDosage", 4803, "");
    BindGlobalCodesWithValueWithOrder("#ddlCurrentMedicationFrequency", 1024, "");
}
/////---------------Current Medication Section ends here---------




//-----------Other Drug Allergies Section-------------
function SelectDrugCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtDrugCode").val(dataItem.Menu_Title);
    UpdateAndBindOtherDrugAllergy(0, dataItem.ID);
}

function UpdateAndBindOtherDrugAllergy(deletedId, newId) {
    $("#txtDrugCode").val('');
    var encounterId = $('#hdCurrentEncounterId').val();
    var isAddConfirmed = newId != '' ? confirm("Do you want to add this DRUG as allergic?", "Confirm") : true;
    if (encounterId > 0 && isAddConfirmed) {
        var isConfirmed = deletedId > 0 ? confirm("This action will delete the drug A", "Confirm") : true;
        if (isConfirmed) {
            $.post("/MedicalRecord/BindOtherDrugAllergiesByEncounter",
                {
                    encounterId: encounterId, deletedId: deletedId, newOrderCode: newId
                },
                function (htmlResponse) {
                    setTimeout(function () {
                        BindList("#DivOtherDrugAllergiesList", htmlResponse);
                    }, 500);
                });
        }
    }
}

//-----------Other Drug Allergies Section-------------