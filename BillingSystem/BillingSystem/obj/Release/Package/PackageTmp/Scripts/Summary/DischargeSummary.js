$(function() {
    $("#colPatientInstrutionsInDischarge").validationEngine();
    $("#colFollowupTypeInDischarge").validationEngine();
    $("#colActiveMedicalProblemsDischarge").validationEngine();
});

function BindDischargeSummaryData1() {
    //
    $("#aEvaluation").hide();
    $("#hfTabValue").val("2");
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Discharge/DischargePartialView",
        dataType: "html",
        async: false,
        data: JSON.stringify({ patientId: patientId, encounterId: encounterId }),
        success: function(data) {
            $(".ehrtabs").empty();
            BindList("#DischargeSummaryTab", data);
            BindGlobalCodesWithValue("#ddlMedicalProblems", 960, "");
            BindGlobalCodesWithValue("#ddlPatientInstructions", 961, "");
            BindGlobalCodesWithValue("#ddlFollowupTypes", 962, "");

            $(".hideSummary").hide();
            ClearHiddenFieldsInDischargeSummary();
            $("#txtFollowupDate")
                .datetimepicker({
                    format: "m/d/Y",
                    minDate: "1950/12/12",
                    maxDate: "2025/12/12",
                    timepicker: false,
                    closeOnDateSelect: true
                });
            //SetGridPaging('?', '?patientId=' + $("#hdPatientId").val() + '&encounterId=' + $("#hdCurrentEncounterId").val() + '&');
        },
        error: function(msg) {
        }
    });
}

/*
If Response Type = 1, then it means we are updating the discharge summary only
if its 2, then it saves the discharge summary first and then its details.
*/
function UpdateDischargeDetails(responseType) {
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    var dischargeSummaryId = $("#hdDischargeSummaryId").val();
    if (patientId > 0 && encounterId > 0) {
        var jsonData = JSON.stringify({
            Id: dischargeSummaryId,
            PatientId: patientId,
            EncounterId: encounterId,
            SameDiagnosisHistory: $("#chkSameDiagnosisHistory")[0].checked,
            FollowupRequired: $("#chkFollowupRequired")[0].checked,
            FollowupDate: $("#txtFollowupDate").val() != "" ? new Date($("#txtFollowupDate").val()) : null,
        });

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Discharge/UpdateDischargeDetails",
            dataType: "html",
            async: true,
            data: jsonData,
            success: function(response) {
                OnSuccessUpdateDischargeSummary(response, responseType);
            },
            error: function(msg) {
            }
        });
    }
}

function OnSuccessUpdateDischargeSummary(response, responseType) {
    $("#hdDischargeSummaryId").val(response);
    if (responseType == 1) {
        ShowMessage("Records updated successfully", "Success", "success", true);
    } else if (responseType == 2) {
        SaveDischargeSummaryDetails(response);
    }
}

/*-----Discharge Summary Detail Section starts here-------------*/

function AddDischargeDetails(ddlSelector, type) {
    var isValid = false;
    if (type == 961) {
        isValid = jQuery("#colPatientInstrutionsInDischarge").validationEngine({ returnIsValid: true });
    } else if (type == 962) {
        isValid = jQuery("#colFollowupTypeInDischarge").validationEngine({ returnIsValid: true });
    } else if (type == 960) {
        isValid = jQuery("#colActiveMedicalProblemsDischarge").validationEngine({ returnIsValid: true });
    }
    if (isValid) {
        SetDischargeDetailType(ddlSelector, type);
        var dishargeSummaryId = $("#hdDischargeSummaryId").val();
        if (!(dishargeSummaryId > 0)) {
            UpdateDischargeDetails(2);
        } else {
            SaveDischargeSummaryDetails(dishargeSummaryId);
        }
        //Update By Krishna on 24072015
        if (dishargeSummaryId == "") {
            ShowMessage("Encounter Is Not Started Yet !", "Warning", "warning", true);
        }
    }
}

function DeleteDischargeDetail() {
    if ($("#hfGlobalConfirmFirstId").val() > 0) {
        var detailId = $("#hfGlobalConfirmFirstId").val();
        var type = $("#hfGlobalConfirmedSecondId").val();
        $.ajax({
            type: "POST",
            url: "/Discharge/DeleteDischargeDetail",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({ id: detailId, typeId: type }),
            success: function(data) {
                BindDischargeDetailList(type, data);
                //var msg = "Records Saved successfully !";
                if (detailId > 0)
                    var msg = "Records Delete successfully";
                ShowMessage(msg, "Success", "success", true);
            },
            error: function(msg) {
            }
        });
    }
}

//function DeleteDischargeDetail(detailId, type) {
//    if (confirm("Do you still want to delete this Record?")) {
//        $.ajax({
//            type: "POST",
//            url: '/Discharge/DeleteDischargeDetail',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: JSON.stringify({ id: detailId, typeId: type }),
//            success: function (data) {
//                BindDischargeDetailList(type, data);
//                //var msg = "Records Saved successfully !";
//                if (detailId > 0)
//                   var msg = "Records Delete successfully";

//                ShowMessage(msg, "Success", "success", true);
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

function BindDischargeDetailList(type, data) {
    var divId = "#divActiveMedicalProblemsDischarge";
    if (type == 961) {
        divId = "#DivPatientInstrutionsInDischarge";
    } else if (type == 962) {
        divId = "#DivFollowupTypesInDischarge";
    }
    BindList(divId, data);
}

function CheckDuplicateSummaryDetail() {
    var result = true;
    var jsonData = JSON.stringify({
        AssociatedId: $("#hdAssociatedId").val(),
        AssociatedTypeId: $("#hdAssociatedTypeId").val()
    });
    $.ajax({
        type: "POST",
        url: "/Discharge/CheckDuplicateSummaryDetail",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function(data) {
            result = data;
        },
        error: function(msg) {
        }
    });
    return result;
}

function SetDischargeDetailType(ddlSelector, typeId) {
    var selectedValue = $(ddlSelector).val();
    if (selectedValue > 0 && typeId > 0) {
        $("#hdAssociatedId").val(selectedValue);
        $("#hdAssociatedTypeId").val(typeId);
    }
}

function ClearHiddenFieldsInDischargeSummary() {
    $("#hdAssociatedId").val("");
    $("#hdAssociatedTypeId").val("");
}

function SaveDischargeSummaryDetails(dischargeSummaryId) {
    var isExist = CheckDuplicateSummaryDetail();
    if (!isExist) {
        var jsonData = JSON.stringify({
            Id: 0,
            DischargeSummaryId: dischargeSummaryId,
            AssociatedId: $("#hdAssociatedId").val(),
            AssociatedTypeId: $("#hdAssociatedTypeId").val(),
            OtherValue: $("#txtFollowupDate").val()
        });
        $.ajax({
            type: "POST",
            url: "/Discharge/AddDischargeSummaryDetail",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function(data) {
                BindDischargeDetailList($("#hdAssociatedTypeId").val(), data);
                ClearHiddenFieldsInDischargeSummary();
                var msg = "Records Saved successfully !";
                if (data > 0)
                    msg = "Records updated successfully";

                ShowMessage(msg, "Success", "success", true);
            },
            error: function(msg) {
            }
        });
    } else {
        ShowMessage("Record Already Exists!", "Alert", "warning", true);
    }
}

///*--------------Sort Active Medicare Grid--------By krishna on 18082015---------*/
function SortMedicareActiveProblem(event) {
    var url = "/Discharge/SortMedicareActiveProblem";
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: true,
        data: null,
        success: function(data) {
            $("#divActiveMedicalProblemsDischarge").empty();
            $("#divActiveMedicalProblemsDischarge").html(data);
        },
        error: function(msg) {
        }
    });
    return false;
}

///*--------------Sort Follows Up Grid--------By krishna on 18082015---------*/
function SortFollowsUp(event) {
    var url = "/Discharge/SortFollowsUp";
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: true,
        data: null,
        success: function(data) {
            $("#DivFollowupTypesInDischarge").empty();
            $("#DivFollowupTypesInDischarge").html(data);
        },
        error: function(msg) {
        }
    });
    return false;
}

///*--------------Sort Patient Instruction Grid--------By krishna on 18082015---------*/
function SortPatientInstruction(event) {
    var url = "/Discharge/SortPatientInstruction";
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: true,
        data: null,
        success: function(data) {
            $("#DivPatientInstrutionsInDischarge").empty();
            $("#DivPatientInstrutionsInDischarge").html(data);
        },
        error: function(msg) {
        }
    });
    return false;
}

function SortDiagnosisGrid(event) {
    //
    //$('#aEvaluation').hide();
    //$('#hfTabValue').val('2');
    var url = "/Discharge/SortDiagnosisGrid";
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: JSON.stringify({ patientId: patientId, encounterId: encounterId }),
        success: function(data) {
            BindList("#DiagnosisListInDischarge", data);
        },
        error: function(msg) {
        }
    });
}

function SortProceduresPerformedGrid(event) {
    var url = "/Discharge/SortProceduresPerformedGrid";
    //var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: JSON.stringify({ encounterId: encounterId }),
        success: function(data) {
            BindList("#DivProceduresList", data);
        },
        error: function(msg) {
        }
    });
}

function SortLabTest(event) {
    var url = "/Discharge/SortLabTest";
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: null,
        success: function(data) {
            BindList("#LabTestListDiv", data);
        },
        error: function(msg) {
        }
    });
}

function SortLabOpenOrderList(event) {
    var url = "/Summary/SortLabOpenOrderList";
    //var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: JSON.stringify({ encounterId: encounterId }),
        success: function(data) {
            BindList("#NurseAdminOpenOrdersListDiv", data);
        },
        error: function(msg) {
        }
    });
}

function SortLabClosedOrderList(event) {
    var url = "/Summary/SortLabClosedOrderList";

    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterid=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: JSON.stringify({ encounterId: encounterId }),
        success: function(data) {
            BindList("#ClosedActivitiesDiv", data);
        },
        error: function(msg) {
        }
    });
}

function BindDischargeOrdersBySort(event) {
    var url = "/Discharge/BindDischargeEncounterOrderListSorted";
    var patientId = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encId=" + $("#hdCurrentEncounterId").val() + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function(data) {
            $("#colProceduresList").empty();
            $("#colProceduresList").html(data);
            setTimeout(function() {
                    if ($("#colProceduresList").hasClass("in")) {
                        $("#OpenOrderDischarge")
                            .fixedHeaderTable({ cloneHeadToFoot: true, altClass: "odd", autoShow: true });
                    } else {
                        $("#colProceduresList").addClass("in");
                        $("#OpenOrderDischarge")
                            .fixedHeaderTable({ cloneHeadToFoot: true, altClass: "odd", autoShow: true });
                        $("#colProceduresList").removeClass("in");
                    }
                    SetGridSorting(BindDischargeOrdersBySort, "#gridContentOpenOrderDischarge1");
                },
                500);
        },
        error: function(msg) {
        }
    });
}

function BindDischargeOpenOrderBySort(event) {
    var url = "/Discharge/BindDischargeOpenOrderBySort";
    var patientId = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encId=" + $("#hdCurrentEncounterId").val() + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function(data) {
            $("#DivMedicationsReceiveInHouse").empty();
            $("#DivMedicationsReceiveInHouse").html(data);
        },
        error: function(msg) {
        }
    });
}

function BindDischargeMedicationBySort(event) {
    var url = "/Discharge/BindDischargeMedicationBySort";
    var patientId = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encId=" + $("#hdCurrentEncounterId").val() + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function(data) {
            $("#DivDischargeMedications").empty();
            $("#DivDischargeMedications").html(data);


            setTimeout(function() {
                    if ($("#colDischargeMedications").hasClass("in")) {
                        $(".gridscrollC").fixedHeaderTable({ cloneHeadToFoot: true, altClass: "odd", autoShow: true });
                    } else {
                        $("#colDischargeMedications").addClass("in");
                        $(".gridscrollC").fixedHeaderTable({ cloneHeadToFoot: true, altClass: "odd", autoShow: true });
                        $("#colDischargeMedications").removeClass("in");
                    }
                    SetGridSorting(BindDischargeMedicationBySort, "#gridContentDischargeMedication");
                },
                500);

        },
        error: function(msg) {
        }
    });
}