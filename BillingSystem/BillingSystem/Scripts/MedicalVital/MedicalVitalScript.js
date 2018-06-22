$(function () {
    
});

function JsVitalCalls() {
    $("#MedicalVitalFormDiv").validationEngine();
    $(".collapseTitle").bind("click", function () {
        $.validationEngine.closePrompt(".formError", true);
    });

    //Filling all DropDown in page.
    //BindGlobalCodesWithValue("#ddlVitalType", 1901, "#hdVitalTypeID");
    //BindGlobalCodesWithValue("#ddlVitalUOM", 3101, "#hdVitalUOM");
}

function SaveMedicalVital(id) {
    var isValid = jQuery("#MedicalVitalFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtMedicalVitalID = id;
        var txtMedicalVitalType = 1;
        var txtPatientID = $("#hdPatientId").val();
        var txtEncounterID = $("#hdCurrentEncounterId").val();
        var txtMedicalRecordNumber = $("#hdPatientMRN").val();
        var txtGlobalCodeCategoryID = 1901;
        var txtGlobalCode = $("#ddlVitalType").val();
        var txtBloodPressureSystolic = $("#txtBloodPressureSystolic").val();
        var txtBloodPressureDiastolic = $("#txtBloodPressureDiastolic").val();
        var txtTemperature = $("#txtTemperature").val();
        var txtPulseRate = $("#txtPulseRate").val();
        var txtWeight = $("#txtWeight").val();
        var txtGlucose = $("#txtGlucose").val();
        if (txtBloodPressureSystolic === "" && txtBloodPressureDiastolic === "" && txtTemperature === "" && txtPulseRate === "" && txtWeight === "" && txtGlucose ==="") {
            ShowMessage("Please enter any vital.", "Warning", "warning", true);
            return false;
        }
        var jsonData = [];
        for (var i = 0; i < 6; i++) {
            switch (i) {
                case 0:
                case 1:
                    if (txtBloodPressureSystolic != "" && txtBloodPressureDiastolic === "") {
                        ShowMessage("Please enter valid Blood Pressure values.", "Warning", "warning", true);
                        return false;
                    }
                    else if (txtBloodPressureSystolic === "" && txtBloodPressureDiastolic != "") {
                        ShowMessage("Please enter valid Blood Pressure values.", "Warning", "warning", true);
                        return false;
                    }
                    else if (txtBloodPressureSystolic === "" && txtBloodPressureDiastolic === "") {
                        break;
                    } else {
                        jsonData[i] = {
                            'MedicalVitalID': 0,
                            'MedicalVitalType': 1,
                            'PatientID': txtPatientID,
                            'EncounterID': txtEncounterID,
                            'MedicalRecordNumber': txtMedicalRecordNumber,
                            'GlobalCodeCategoryID': txtGlobalCodeCategoryID,
                            'GlobalCode': VitalType.BloodPressureSystolic, // From Enum//Blood Pressure Id 
                            'AnswerValueMin': txtBloodPressureSystolic,
                            'AnswerValueMax': "",
                            'AnswerUOM': 3,
                            'txtComments': ""
                        };
                        i++;
                         jsonData[i] = {
                            'MedicalVitalID': 0,
                            'MedicalVitalType': 1,
                            'PatientID': txtPatientID,
                            'EncounterID': txtEncounterID,
                            'MedicalRecordNumber': txtMedicalRecordNumber,
                            'GlobalCodeCategoryID': txtGlobalCodeCategoryID,
                            'GlobalCode': VitalType.BloodPressureDiastolic, // From Enum//Blood Pressure Id 
                            'AnswerValueMin': txtBloodPressureDiastolic,
                            'AnswerValueMax': "",
                            'AnswerUOM': 3,
                            'txtComments': ""
                        };
                    }
                    break;
                case 2:
                    if (txtTemperature === "") {
                        break;
                    } else {
                        var UOM = $("input:radio[name='TemperatureUOM']:checked").val();
                        jsonData[i] = {
                            'MedicalVitalID': 0,
                            'MedicalVitalType': 1,
                            'PatientID': txtPatientID,
                            'EncounterID': txtEncounterID,
                            'MedicalRecordNumber': txtMedicalRecordNumber,
                            'GlobalCodeCategoryID': txtGlobalCodeCategoryID,
                            'GlobalCode': VitalType.Temperature, // From Enum // temperature value
                            'AnswerValueMin': txtTemperature,
                            'AnswerValueMax': 0,
                            'AnswerUOM': parseFloat(UOM),
                            'txtComments': ""
                        };
                    }
                    break;
                case 3:
                    if (txtPulseRate === "") {
                        break;
                    } else {
                        jsonData[i] = {
                            'MedicalVitalID': 0,
                            'MedicalVitalType': 1,
                            'PatientID': txtPatientID,
                            'EncounterID': txtEncounterID,
                            'MedicalRecordNumber': txtMedicalRecordNumber,
                            'GlobalCodeCategoryID': txtGlobalCodeCategoryID,
                            'GlobalCode': VitalType.Pulse, // From Enum // Pulse rate Value
                            'AnswerValueMin': txtPulseRate,
                            'AnswerValueMax': 0,
                            'AnswerUOM': VitalUnits.bpm,
                            'txtComments': ""
                        };
                    }
                    break;
                case 4:
                    if (txtWeight === "") {
                        break;
                    } else {
                        var weightUom = $("input:radio[name='WeightUOM']:checked").val();
                        jsonData[i] = {
                            'MedicalVitalID': 0,
                            'MedicalVitalType': 1,
                            'PatientID': txtPatientID,
                            'EncounterID': txtEncounterID,
                            'MedicalRecordNumber': txtMedicalRecordNumber,
                            'GlobalCodeCategoryID': txtGlobalCodeCategoryID,
                            'GlobalCode': VitalType.Weight, // From Enum //Weight Value
                            'AnswerValueMin': txtWeight,
                            'AnswerValueMax': 0,
                            'AnswerUOM': parseFloat(weightUom),
                            'txtComments': ""
                        };
                    }
                    break;
                case 5:
                    if (txtGlucose === "") {
                        break;
                    } else {
                        jsonData[i] = {
                            'MedicalVitalID': 0,
                            'MedicalVitalType': 1,
                            'PatientID': txtPatientID,
                            'EncounterID': txtEncounterID,
                            'MedicalRecordNumber': txtMedicalRecordNumber,
                            'GlobalCodeCategoryID': txtGlobalCodeCategoryID,
                            'GlobalCode': VitalType.Glucose, // From Enum //Weight Value
                            'AnswerValueMin': txtGlucose,
                            'AnswerValueMax': 0,
                            'AnswerUOM': VitalUnits.mgdl,
                            'txtComments': ""
                        };
                    }
                    break;
                default:
            }
        };
        var jsonD = JSON.stringify(jsonData);
        $.ajax({
            type: "POST",
            url: "/MedicalVital/SaveMedicalVitals",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonD,
            success: function (data) {
                ClearVitalAll();
                JsVitalCalls();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";
                BindMedicalVitalGrid();
                ShowMessage(msg, "Success", "success", true);
            },
            error: function (msg) {

            }
        });
    }
}

function EditMedicalVital(id) {
    var txtMedicalVitalId = id;
    var jsonData = JSON.stringify({
        MedicalVitalID: txtMedicalVitalId
    });
    $.ajax({
        type: "POST",
        url: "/MedicalVital/GetMedicalVital",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $("#MedicalVitalFormDiv").empty();
                $("#MedicalVitalFormDiv").html(data);
                $("#collapseVitalAddEdit").addClass("in").attr("style", "");
                JsVitalCalls();

            }
            else {
            }
        },
        error: function (msg) {

        }
    });
}

function DeleteMedicalVital() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var txtMedicalVitalId = $("#hfGlobalConfirmId").val();
        var jsonData = JSON.stringify({
            MedicalVitalID: txtMedicalVitalId
        });
        $.ajax({
            type: "POST",
            url: "/MedicalVital/DeleteMedicalVital",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindMedicalVitalGrid();
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

//function DeleteMedicalVital(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtMedicalVitalId = id;
//        var jsonData = JSON.stringify({
//            MedicalVitalID: txtMedicalVitalId
//        });
//        $.ajax({
//            type: "POST",
//            url: '/MedicalVital/DeleteMedicalVital',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindMedicalVitalGrid();
//                    ShowMessage("Records Deleted Successfully", "Success", "success", true);
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

function BindMedicalVitalGrid() {
    var jsonData = JSON.stringify({
        PatientID: $("#hdPatientId").val(),
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/MedicalVital/BindMedicalVitalList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {

            $("#MedicalVitalListDiv").empty();
            $("#MedicalVitalListDiv").html(data);

            if ($("#colCurrentVitalsMain").length > 0) {
                $("#colCurrentVitalsMain").empty();
                $("#colCurrentVitalsMain").html(data);
            }
        },
        error: function (msg) {

        }

    });
}

function ClearVitalForm() {
    $("#MedicalVitalFormDiv").clearForm();
    $("#collapseOne").removeClass("in");
    $("#collapseTwo").addClass("in");
}

function ClearVitalAll() {
    ClearVitalForm();
    $(".emptytxt").val("");
    $.validationEngine.closePrompt(".formError", true);
    $("#rbtnTempC").prop("checked", true);
    $("#rbtnWeightKg").prop("checked", true);
}
function ClickNurseNote() {
    $("#NurseNote").click();
}
function ClickVital() {
    $("#Vital").click();
}


var ConvertTemp = function() {
    var tempC = $("#txtTemperature").val();
    var tempF = tempC * 9 / 5 + 32;
    $("#txtTemperatureF").val(tempF);
};

var ConvertWeight = function () {
    var weightKG = $("#txtWeight").val();
    var weightlbs = weightKG * 2.20462262185;
    $("#txtWeightlbs").val(weightlbs);
};