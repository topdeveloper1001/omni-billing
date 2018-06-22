function viewEhrDetails(eid, pid) {
    /// <summary>
    /// Views the ehr details.
    /// </summary>
    /// <param name="eid">The eid.</param>
    /// <param name="pid">The pid.</param>
    /// <returns></returns>
    RedirectEHR(eid, pid);
}

function EditActiveEncounter(Encounterid, PatientId, messageid) {
    /// <summary>
    /// Edits the active encounter.
    /// </summary>
    /// <param name="Encounterid">The encounterid.</param>
    /// <param name="PatientId">The patient identifier.</param>
    /// <param name="messageid">The messageid.</param>
    /// <returns></returns>
    window.location.href = window.location.protocol + "//" + window.location.host + "/Encounter/Index?patientId=" + PatientId + "&messageId=" + messageid + "&encid=" + Encounterid;
    // Message id
    //1-- Inpatient, 2-- Outpatient, 3-- ER
}

function DeleteActiveEncounter(id) {
    /// <summary>
    /// Deletes the active encounter.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    return false;
}

function ViewActiveEncounter(id) {
    /// <summary>
    /// Views the active encounter.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    return false;
}

function RedirectEHR(PatientId) {
    /// <summary>
    /// Redirects the ehr.
    /// </summary>
    /// <param name="PatientId">The patient identifier.</param>
    /// <returns></returns>
    window.location.href = window.location.protocol + "//" + window.location.host + "/Summary/PatientSummary?pId=" + PatientId;
}

function ViewEHR(encounterid) {
    /// <summary>
    /// Views the ehr.
    /// </summary>
    /// <param name="encounterid">The encounterid.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        encounterId: encounterid,
    });
    $.ajax({
        type: "POST",
        url: '/ActiveEncounter/GetEncounterDetailById',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                var msg = data.message;
                if (msg != '' && msg != 'new') {
                    ShowMessage(msg, "Alert", "warning", true);
                }
                else if (!data.isRecordExist && data.isNew == false) {
                    ShowMessage("Encounter doesn't start yet", "Alert", "warning", true);
                }
                else {
                    window.location = window.location.protocol + "//" + window.location.host + "/Encounter/Index?patientId=" + PatientID + "&messageId=" + data.messageId;
                    return true;
                }
            }
        },
        error: function (msg) {
        }
    });
}

function DischargeActiveEncounter(Encounterid, PatientId, messageid) {
    /// <summary>
    /// Discharges the active encounter.
    /// </summary>
    /// <param name="Encounterid">The encounterid.</param>
    /// <param name="PatientId">The patient identifier.</param>
    /// <param name="messageid">The messageid.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        encounterId: Encounterid,
    });
    $.ajax({
        type: "POST",
        url: '/Encounter/EncounterOpenOrdersPending',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                if (data) {
                    ShowMessageWithDuration("Unable to End Encounter, Reason : Encounter have some open orders that need to be attended.", "Warning", "warning", true, 2000);
                    return false;
                }
                else {
                    window.location.href = window.location.protocol + "//" + window.location.host + "/Encounter/Index?patientId=" + PatientId + "&messageId=" + messageid + "&encid=" + Encounterid;
                    return true;
                }
            }
        },
        error: function (msg) {
        }
    });
    // Message id
    //1-- Inpatient, 2-- Outpatient, 3-- ER
}

function ViewTransactions(encounterId, patientId) {
    /// <summary>
    /// Views the transactions.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    /// <returns></returns>
    window.location.href = window.location.protocol + "//" + window.location.host + "/PreliminaryBill/Index?eId=" + encounterId + "&pId=" + patientId;
}

function AdmitPatientEncounter() {
    var encounterid = $("#hfGlobalConfirmFirstId").val();
    var patientId = $("#hfGlobalConfirmedSecondId").val();
    var messageid = $("#hfGlobalConfirmedThridId").val();
    window.location.href = window.location.protocol + "//" + window.location.host + "/Encounter/Index?patientId=" + patientId + "&messageId=" + messageid + "&encid=" + encounterid;
}

//function AdmitPatientEncounter(Encounterid, PatientId, messageid) {
//    /// <summary>
//    /// Admits the patient encounter.
//    /// </summary>
//    /// <param name="Encounterid">The encounterid.</param>
//    /// <param name="PatientId">The patient identifier.</param>
//    /// <param name="messageid">The messageid.</param>
//    /// <returns></returns>
//    if (confirm("Are you sure you want to stop outpatient and start inpatient admit using same Encounter number?")) {
//        window.location.href = window.location.protocol + "//" + window.location.host + "/Encounter/Index?patientId=" + PatientId + "&messageId=" + messageid + "&encid=" + Encounterid;
//    }
//}

$(function () {
    PrimaryDiagnosisRowColor();
    $('#btnVirtualDischargeConfirm').on('click', function () {
        var encounterid = $("#hidVirtualDischargeEncId").val();
        var patientId = $("#hidVirtualDischargePatientId").val();
        var messageid = $("#hidVirtualDischargeMessageId").val();
        window.location.href = window.location.protocol + "//" + window.location.host + "/Encounter/Index?patientId=" + patientId + "&messageId=" + messageid + "&encid=" + encounterid;
    });

    $('#btnVirtualDischargeCancel').on('click', function () {
        $("#divConfirmVirtualDischargeBox").modal('hide');
    });
});

function PrimaryDiagnosisRowColor() {
    /// <summary>
    /// Primaries the color of the diagnosis row.
    /// </summary>
    /// <returns></returns>
    $("#InPatientActiveEncounterList tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        if ($actualRow.find('.colPrimaryDiagnosis').html().indexOf("<span>No</span>") != -1) {
            $actualRow.addClass('rowColor3');
            $actualRow.attr('title', 'Primary diagnosis is not added!');
        }
        else { $actualRow.removeClass('rowColor3'); }
    });
}

function GetEncounterAuth(patientId) {
    /// <summary>
    /// Gets the encounter authentication.
    /// </summary>
    /// <param name="patientId">The patient identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        PatientID: patientId
    });
    $.ajax({
        type: "POST",
        url: '/PatientInfo/GetActiveEncounterId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                if (data == 0) {
                    ShowMessage("There is no encounter for current patient.", "Warning", "warning", true);
                } else {
                    $("#authorizationdiv").empty();
                    $("#authorizationdiv").html(data);
                    //$("#AuthorizationMemberID").val(patientId);
                    $(".hidePopUp").show();
                }
            }
        },
        error: function (msg) {
        }
    });
}

function EditPatient(id) {
    /// <summary>
    /// Edits the patient.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    window.location = window.location.protocol + "//" + window.location.host + "/PatientInfo/PatientInfo?patientId=" + id;
    //var txtPatientInfoId = id;
    //var jsonData = JSON.stringify({
    //    id: txtPatientInfoId,
    //    viewOnly: ''
    //});
    //$.ajax({
    //    type: "POST",
    //    url: '/PatientSearch/EditPatient',
    //    async: false,
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "html",
    //    data: jsonData,
    //    success: function (data) {
    //        window.location = window.location.protocol + "//" + window.location.host + "/PatientInfo/PatientInfo?patientId=" + id;
    //    },
    //    error: function (msg) {
    //    }
    //});
}

function HidePopup() {
    $.validationEngine.closePrompt(".formError", true);
    $('#divhidepopup').hide();
}




function AddUpdateTriage() {
    var selected = $("#divTriagePopop input[type='radio']:checked");
    var selectedRadion = selected.val();
    var jsonData = JSON.stringify({
        encounterId: $("#hdTempEncounterId").val(),
        triageLevel: selectedRadion
    });
    $.ajax({
        type: "POST",
        url: '/ActiveEncounter/UpdateTriageInEncounter',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data) {
                var eid = $("#hdTempEncounterId").val();
                switch (data) {
                    case "1":
                        $("#divTL_" + eid).attr("style", 'background-color:black; color: white;');
                        $("#divTL_" + eid).text("level 0");
                        break;
                    case "2":
                        $("#divTL_" + eid).attr("style", ' background-color:red; color: white;');
                        $("#divTL_" + eid).text("level 1");
                        break;
                    case "3":
                        $("#divTL_" + eid).attr("style", 'background-color:orange');
                        $("#divTL_" + eid).text("level 2");
                        break;
                    case "4":
                        $("#divTL_" + eid).attr("style", 'background-color:yellow');
                        $("#divTL_" + eid).text("level 3");
                        break;
                    case "5":
                        $("#divTL_" + eid).attr("style", ' background-color:green; color: white;');
                        $("#divTL_" + eid).text("level 4");
                        break;
                    case "6":
                        $("#divTL_" + eid).attr("style", ' background-color:blue; color: white;');
                        $("#divTL_" + eid).text("level 5");
                        break;
                }
                //$("#hdTempEncounterId").val('');
                $("#divhidepopup21").hide();
            }
        },
        error: function (msg) {
        }
    });
}

//function ChangeTriageLevelInEncounter(encounterId) {
//    $("#hdTempEncounterId").val(encounterId);

//}






function ChangeTriageLevelInEncounter(encounterId, triage) {
   $("#hdTempEncounterId").val(encounterId);
    var jsonData = JSON.stringify({
        encounterId: encounterId
    });
    $.ajax({
        type: "POST",
        url: "/ActiveEncounter/GetTriageData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            var checked = '';
            $("#divhidepopup21").show();
            var items = '';
            $.each(data.list, function (i, care) {
                if (data.triValue != '' && care.Value == data.triValue) {
                    checked = 'checked="checked"';
                } else
                    checked = '';

                items += '<li id="li3' + i + '"><input name="radioTriageValue" ' + checked + '  type="radio" value="' + care.Value + '"><p>' + care.Text + '</p></li>';
            });
            $("#divTriagePopop").html(items);

        },
        error: function (msg) {

        }
    });

}

function ChangePatientStageInEncounter(encounterId, e,pState) {
   var jsonData = JSON.stringify({
        encounterId: encounterId
    });
    $("#hdcolumnId").val((e));
    $("#hdTempEncounterId").val(encounterId);
    $.ajax({
        type: "POST",
        url: "/ActiveEncounter/GetPatientStageData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            var checked = '';
            $("#divhidepopup121").show();
            var items = '';
            $.each(data.list, function (i, care) {
                if (data.stateValue != '' && care.Value == data.stateValue) {
                    checked = 'checked="checked"';
                } else
                    checked = '';

                items += '<li id="li3' + i + '"><input name="radioPatientStageValue" ' + checked + ' type="radio" value="' + care.Value + '"><p>' + care.Text + '</p></li>';
            });
            $("#divPatientStatePopop").html(items);

        },
        error: function (msg) {

        }
    });
}



//function AddUpdatePatientState() {
//    //var hdPriState = $("#hdState").val();
//    var selected = $("#divPatientStatePopop input[type='radio']:checked");
   
//    var selectedLevel = selected.val();
//    //$("#hdState").val(selectedLevel);

//    var jsonData = JSON.stringify({
//        encounterId: $("#hdTempEncounterId").val(),
//        patientState: selectedLevel
//    });

//    if (confirm("This action will change the Current Encounter's Patient Stage. Continue?")) {
//        $.ajax({
//            type: "POST",
//            url: '/ActiveEncounter/UpdatePatientStageInEncounter',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: jsonData,
//            success: function (data) {
//                var ancorId = $("#hdcolumnId").val();
//                $('#' + ancorId).html(data);
//                if (data) {
//                    $("#hdTempEncounterId").val('');
//                    $("#divhidepopup121").hide();
//                }
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}


function AddUpdatePatientState() {
    //var hdPriState = $("#hdState").val();
    var selected = $("#divPatientStatePopop input[type='radio']:checked");
    var selectedLevel = selected.val();
    //$("#hdState").val(selectedLevel);

    var jsonData = JSON.stringify({
        encounterId: $("#hdTempEncounterId").val(),
        patientState: selectedLevel
    });

    $.ajax({
        type: "POST",
        url: '/ActiveEncounter/UpdatePatientStageInEncounter',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            var ancorId = $("#hdcolumnId").val();
            $('#' + ancorId).html(data);
            if (data) {
                //$("#hdTempEncounterId").val('');
                $("#divhidepopup121").hide();
            }
        },
        error: function (msg) {
        }
    });
}



function DischargeIPActiveEncounter(Encounterid, PatientId, messageid) {
    /// <summary>
    /// Discharges the active encounter.
    /// </summary>
    /// <param name="Encounterid">The encounterid.</param>
    /// <param name="PatientId">The patient identifier.</param>
    /// <param name="messageid">The messageid.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        encounterId: Encounterid,
    });
    $.ajax({
        type: "POST",
        url: '/Encounter/EncounterOpenOrdersPending',
        async: true,
        cache: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                if (data) {
                    VirtualDischargePatient(Encounterid, PatientId, 9, 'Discharge Patient', 'Encounter have orders that are open. Do you want to virtually discharge the patient?');
                    //ShowMessageWithDuration("Unable to End Encounter, Reason : Encounter have some open orders that need to be attended.", "Warning", "warning", true, 2000);
                }
                else {
                    window.location.href = window.location.protocol + "//" + window.location.host + "/Encounter/Index?patientId=" + PatientId + "&messageId=" + messageid + "&encid=" + Encounterid;
                    return true;
                }
            }
        },
        error: function (msg) {
        }
    });
    // Message id
    //1-- Inpatient, 2-- Outpatient, 3-- ER
}

var VirtualDischargePatient = function (encounterid, patientId, messageid, title, msg) {
    $("#hidVirtualDischargeEncId").val(encounterid);
    $("#hidVirtualDischargePatientId").val(patientId);
    $("#hidVirtualDischargeMessageId").val(messageid);
    $("#h5title").html(title);
    if (msg != '')
        $("#h5Message").html(msg);
    //$.blockUI({ message: $('#divConfirmVirtualDischargeBox'), css: { width: '350px' } });
    //setTimeout($.blockUI({ message: $('#divConfirmVirtualDischargeBox'), css: { width: '350px' } }), 500);

    $("#divConfirmVirtualDischargeBox").modal();
}