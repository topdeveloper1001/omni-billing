/*
Status for different Duplicate CODES:
-3 ->Diagnosis Code
-4 -> DRG Code 
-5 -> Major CPT Code
*/

var duplicateStatusCodes = { "Diagnosis Code": -3, "DRG": -4, "Major CPT": -5 };

$(function () {
 
    //$("#txtDRGStartDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));
    $("#diagnosisAddEdit").validationEngine();
    var isPrimary = $("#hdIsPrimary").val();
    SetValueInDiagnosisType(isPrimary);
    $(".ddlType1").prop('disabled', 'disabled');
    $('.AddAsDiagnosis').show();

    //-------------Added for Super Powers functionality-------------///
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdEncounterId").val();

    $("#GlobalPatientId").val(patientId);
    $("#GlobalEncounterId").val(encounterId);
    BindLinkUrlsForSuperPowers();
    //-------------Added for Super Powers functionality-------------///


   
});


function SelectDiagnosisCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#primaryDiagnosisCode").val(dataItem.Menu_Title);
    $("#hdprimaryCodeId").val(dataItem.ID);
    $("#hdprimaryCodeValue").val(dataItem.Name);
}

function ClearAll() {
    var hdPatientid = $('#hdPatientId').val();
    var hdDiagnosisID = $('#hdDiagnosisID').val();
    var hdIsPrimary = $('#hdIsPrimary').val();
    var hdCreatedBy = $('#hdCreatedBy').val();
    var hdCreatedDate = $('#hdCreatedDate').val();
    var hdEncounterId = $('#hdEncounterId').val();
    var hdCorporateId = $('#hdCorporateId').val();
    var hdfacilityId = $('#hdfacilityId').val();
    var hdMedicalRecordNumber = $('#hdMedicalRecordNumber').val();
    var hdIsMajorCPTEntered = $('#hdIsMajorCPTEntered').val();
    var hdIsMajorDRGEntered = $('#hdIsMajorDRGEntered').val();

    $("#diagnosisAddEdit").clearForm(true);
    $.validationEngine.closePrompt(".formError", true);

    $('#hdPatientId').val(hdPatientid);
    $('#hdDiagnosisID').val(hdDiagnosisID);
    $('#hdIsPrimary').val(hdIsPrimary);
    $('#hdCreatedBy').val(hdCreatedBy);
    $('#hdCreatedDate').val(hdCreatedDate);
    $('#hdEncounterId').val(hdEncounterId);
    $('#hdCorporateId').val(hdCorporateId);
    $('#hdfacilityId').val(hdfacilityId);
    $('#hdMedicalRecordNumber').val(hdMedicalRecordNumber);
    $('#hdIsMajorCPTEntered').val(hdIsMajorCPTEntered);
    $('#hdIsMajorDRGEntered').val(hdIsMajorDRGEntered);

    var hdPatientId = $("#hdPatientId").val();
    BindDiagnosisList(hdPatientId);
    $('.btnSave').val('Save');
    $('.btnSave').attr('onclick', 'SaveDiagnosisData("0");');
    $("#hdDiagnosisID").val('');
    $("#rdReviewedByPhysician").prop("checked", "checked");
    $("#rdIntiallyEnteredBy").prop("checked", "checked");
    //$('#collapseDiagnosisAddEdit').removeClass('in');
    $('#collapseDiagnosisList').addClass('in');
    var isPrimary = $("#hdIsPrimary").val();
    SetValueInDiagnosisType(isPrimary);
}

function BindDiagnosisList(patientId) {
    var jsonData = JSON.stringify({
        patientId: patientId,
    });
    $.ajax({
        type: "POST",
        url: '/Diagnosis/GetList',
        //async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                $('#CurrentDiagnosisGrid').html();
                $('#CurrentDiagnosisGrid').html(data);

                $('#colCurrentDiagnosisMain').html();
                $('#colCurrentDiagnosisMain').html(data);
                var enId = $("#hdEncounterId").val();
                //SetGridPaging('?', '?patientId=' + patientId + '&');
            }
        },
        error: function (msg) {
        }
    });
}

function SetValueInDiagnosisType(isPrimary) {
    if (isPrimary != "True") {
        $(".ddlType1").val(2);
        $(".ddlType1").prop('disabled', 'disabled');
        //$("#DRGCodesInDiagnosisDiv").removeAttr('disabled');      //Enable DRG Div
    }
    else {
        $(".ddlType1").val(1);
        $(".ddlType1").attr('disabled', false);
        //$("#DRGCodesInDiagnosisDiv").prop('disabled', 'disabled');      //Disable DRG Div
    }
    DisableCPTPanel($("#hdIsMajorCPTEntered").val());
    DisableDRGPanel($("#hdIsMajorDRGEntered").val());
}

function SaveDiagnosisData(id) {
    var isValid = $("#diagnosisAddEdit").validationEngine({ returnIsValid: true });
    var hdEncounterId = $("#hdEncounterId").val() == "" ? $("#hdCurrentEncounterId").val() : $("#hdEncounterId").val();
    if (hdEncounterId == '' || hdEncounterId == null) {
        ShowMessage("Encounter not started yet, Unable to add diagnosis!", "Warning", "warning", true);
        return false;
    }
    var drgCodeId = $("#hdDrgCodeID").val();
    var cptCodeId = $("#hdCPTCodeValue").val();
    if (drgCodeId > 0 || cptCodeId != '') {
        isValid = true;
    }
    if (isValid == true) {
        id = $("#hdDiagnosisID").val();
        var txtNotes = $("#txtNotes").val();
        var hdCodeId = $("#hdprimaryCodeId").val();
        //var primaryDiagnosisCode = $("#hdprimaryCodeId").val();
        var txtPrimaryDiagnosisDescription = $("#hdprimaryCodeValue").val();
        var rdIntiallyEnteredBy = $("#rdIntiallyEnteredBy:checked").length;
        var rdReviewedByPhysician = $("#rdReviewedByPhysician:checked").length;
        var rdReviewedByCoder = $("#rdReviewedByCoder:checked").length;
        var diagnosisType = $(".ddlType1").val();
        var hdPatientId = $("#hdPatientId").val();
        var corporateId = $("#hdCorporateId").val();
        var facilityId = $("#hdfacilityId").val();
        var hdMedicalRecordNumber = $("#hdMedicalRecordNumber").val();
        var hdCreatedBy = $("#hdCreatedBy").val();
        var hdCreatedDate = $("#hdCreatedDate").val();
        //var DrgCodeValue = $("#hdDrgCodeValue").val();
        if (drgCodeId > 0) {
            rdIntiallyEnteredBy = 0;
        }

        diagnosisType = $("#primaryDiagnosisCode").val() == '' ? 0 : diagnosisType;

        if (hdEncounterId != '' && hdEncounterId > 0) {
            var jsonData = JSON.stringify({
                DiagnosisID: id,
                DiagnosisType: diagnosisType,
                CorporateID: corporateId,
                FacilityID: facilityId,
                PatientID: hdPatientId,
                EncounterID: hdEncounterId,
                DiagnosisCodeId: hdCodeId,
                DiagnosisCode: hdCodeId,//primaryDiagnosisCode,
                DiagnosisCodeDescription: txtPrimaryDiagnosisDescription,
                Notes: txtNotes,
                InitiallyEnteredByPhysicianId: rdIntiallyEnteredBy,
                ReviewedByCoderID: rdReviewedByCoder,
                ReviewedByPhysicianID: rdReviewedByPhysician,
                MedicalRecordNumber: hdMedicalRecordNumber,
                CreatedBy: hdCreatedBy,
                CreatedDate: new Date(hdCreatedDate),
                IsDeleted: false,
                DRGCodeID: drgCodeId,
                MajorCPTCodeId: cptCodeId
            });
            if ((hdCodeId == "" || hdCodeId == null) && (drgCodeId == "" || drgCodeId == null)
                && (cptCodeId == "" || cptCodeId == null)) {
                ShowMessage("Please search valid code!", "Warning", "warning", true);
                return false;
            }
            $.ajax({
                type: "POST",
                url: '/Diagnosis/SaveDiagnosisCustomCode',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonData,
                success: function (data) {
                    if (data != null) {
                        var dataResult = data.result;
                        var primaryExists = data.primaryDone;

                        if (dataResult == "-1") {
                            ShowMessage("Unable to add another primary diagnosis!", "Warning", "warning", true);
                        }
                        else if (dataResult == "-2") {
                            ShowMessage("Record with same diagnosis code exist!", "Warning", "warning", true);
                        }
                        else {
                            $("#hdIsPrimary").val(primaryExists ? "False" : "True");
                            $("#hdIsMajorCPTEntered").val(data.majorCptDone != null && data.majorCptDone == true ? "False" : "True");
                            $("#hdIsMajorDRGEntered").val(data.majorDrgDone != null && data.majorDrgDone == true ? "False" : "True");

                            ClearAll(hdPatientId);

                            $("#hdDiagnosisID").val('');
                            var msg = "Records Saved successfully !";
                            if (id > 0)
                                msg = "Records updated successfully";

                            $('#collapseDiagnosisAddEdit').addClass('in');
                            $(".ddlType1").prop('disabled', 'disabled');
                            ShowMessage(msg, "Success", "success", true);
                        }
                    }
                },
                error: function (msg) {
                }
            });
        }
        else {
            ShowMessage("Enter the required fields! ", "Alert", "warning", true);
        }
    }
    else {
        ShowMessage("Enter the required fields! ", "Alert", "warning", true);
        return false;
    }
    return false;
}

function SetValue(selector, value) {
    $(selector).val(value);
}

function OnChangeDiagnosisType(lblselector, ddlSelector) {
    var value = $(ddlSelector + " option:selected").text();
    $(lblselector).text(value);
}

function AddPreviuosDiagnosisToCurrent(id) {
    var jsonData = JSON.stringify({
        Id: id,
    });
    $.ajax({
        type: "POST",
        url: '/Diagnosis/AddDiagnosisById',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                BindDiagnosisDetails(data);
            }
        },
        error: function (msg) {
        }
    });
}

function EditCurrentDiagnosis(id) {
    var jsonData = JSON.stringify({
        Id: id,
        ViewOnly: ''
    });
    $.ajax({
        type: "POST",
        url: '/Diagnosis/GetDiagnosisById',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                BindDiagnosisDetails(data);
            }
        },
        error: function (msg) {
        }
    });
}

function BindDiagnosisDetails(data) {
    $("#txtNotes").val(data.Notes);
    $(".rdReviewedBy").attr("checked", false);
    if (data.type == 1 || data.type == 2) {
        $("#hdprimaryCodeId").val(data.code);
        $("#hdprimaryCodeValue").val(data.CodeDescription);
        $("#primaryDiagnosisCode").val(data.code + " " + data.CodeDescription);
    }
    $("#rdIntiallyEnteredBy").prop("checked", "checked");
    if (data.reviewedByCoder > 0) {
        $("#rdReviewedByCoder").prop("checked", "checked");
    }
    else {
        $("#rdReviewedByPhysician").prop("checked", "checked");
    }

    $(".ddlType1").val(data.type);
    $("#hdPatientId").val(data.patientId);
    $("#hdCorporateId").val(data.cId);
    $("#hdfacilityId").val(data.facilityId);
    $("#hdMedicalRecordNumber").val(data.mrn);
    $("#hdDiagnosisID").val(data.id);
    $("#hdCreatedBy").val(data.createdBy);
    $("#hdCreatedDate").val(data.CreatedDate);

    //------------DRG Section start here------------
    $("#hdDrgCodeID").val('');
    $("#hdDrgCodeValue").val('');
    $("#txtDRGCode").val('');
    if (data.type == 3) {
        $("#hdDrgCodeID").val(data.DrgCodeId);
        $("#hdDrgCodeValue").val(data.DrgCodeValue);
        $("#txtDRGCode").val(data.DrgCodeValue + " " + data.DrgCodeDescription);
        DisableDRGPanel("True");
        //$("#txtDRGDescription").val(data.DrgCodeDescription);
    }

    //------------CPT Section end here------------
    $("#hdCPTCodeID").val('');
    $("#hdCPTCodeValue").val('');
    $("#txtCPTCode").val('');
    if (data.type == 4) {
        $("#hdCPTCodeID").val(data.codeId);
        $("#hdCPTCodeValue").val(data.code);
        $("#txtCPTCode").val(data.code + " - " + data.CodeDescription);
        DisableCPTPanel("True");
        //$("#CPTCodesInDiagnosisDiv").prop('disabled', false);
    }

    if ($("#hdDiagnosisID").val() === '' || $("#hdDiagnosisID").val() === '0') {
        $('.btnSave').val('Save');
    } else {
        $('.btnSave').val('Update');
    }
    $('#collapseDiagnosisAddEdit').addClass('in');
    $(".ddlType1").prop('disabled', 'disabled');

    $('#txtDRGStartDate').val(GetDateFromDatetime(data.CreatedDate));
    $('#txtDRGStartTimeHrs').val(GetTimeFromDatetime(data.CreatedDate, 1));
    $('#txtDRGStartTimeMins').val(GetTimeFromDatetime(data.CreatedDate, 2));
}

function toggleRadioButtons(selector) {
    if (selector.toString().indexOf('Added') != -1) {
        $(".rdNotesBy").attr("checked", false);
        $(selector).prop("checked", true);
    } else {
        $(".rdReviewedBy").attr("checked", false);
        $(selector).prop("checked", "checked");
    }
}

function DeleteDiagnosis() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        $.ajax({
            type: "POST",
            url: '/Diagnosis/DeleteDiagnosis',
            //async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ id: $("#hfGlobalConfirmId").val() }),
            success: function (data) {
                if (data != null) {
                    if (data == "-1") {
                        ShowMessage("Unable to delete primary diagnosis!", "Warning", "warning", true);
                    }
                    else {
                        //$("#hdIsPrimary").val("False");
                        //if (diagnosisType == 4) {
                        //    $("#hdIsMajorCPTEntered").val("True");
                        //}
                        $("#hdIsPrimary").val(data.isPrimary ? "False" : "True");
                        $("#hdIsMajorCPTEntered").val(data.isMajorCptDone != null && data.isMajorCptDone == true ? "False" : "True");
                        $("#hdIsMajorDRGEntered").val(data.isDrgDone != null && data.isDrgDone == true ? "False" : "True");

                        ClearAll(hdPatientId);
                        $("#hdDiagnosisID").val('');
                        var msg = "Records deleted successfully !";
                        ShowMessage(msg, "Success", "success", true);
                        ShowDiagnosisActions();
                    }
                }
            },
            error: function (msg) {
            }
        });
    }
}

//function DeleteDiagnosis(id, diagnosisType) {
//    if (confirm("Do you really want to delete this record?")) {
//        $.ajax({
//            type: "POST",
//            url: '/Diagnosis/DeleteDiagnosis',
//            //async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: JSON.stringify({ id: id }),
//            success: function (data) {
//                if (data != null) {
//                    if (data == "-1") {
//                        ShowMessage("Unable to delete primary diagnosis!", "Warning", "warning", true);
//                    }
//                    else {
//                        //$("#hdIsPrimary").val("False");
//                        //if (diagnosisType == 4) {
//                        //    $("#hdIsMajorCPTEntered").val("True");
//                        //}
//                        $("#hdIsPrimary").val(data.isPrimary ? "False" : "True");
//                        $("#hdIsMajorCPTEntered").val(data.isMajorCptDone != null && data.isMajorCptDone == true ? "False" : "True");
//                        $("#hdIsMajorDRGEntered").val(data.isDrgDone != null && data.isDrgDone == true ? "False" : "True");

//                        ClearAll(hdPatientId);
//                        $("#hdDiagnosisID").val('');
//                        var msg = "Records deleted successfully !";
//                        ShowMessage(msg, "Success", "success", true);
//                        ShowDiagnosisActions();
//                    }
//                }
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

function EditDiagnosisRecord(id) {
    var jsonData = JSON.stringify({
        Id: id,
        ViewOnly: ''
    });
    $.ajax({
        type: "POST",
        url: '/Diagnosis/GetDiagnosisById',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                BindDiagnosisDetails(data);
            }
        },
        error: function (msg) {
        }
    });
}

//-----------DRG feature starts here-----------------------
function OnDRGCodeSelection(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtDRGCode").val(dataItem.Code);
    $('#hdDrgCodeValue').val(dataItem.Code);
    $("#hdDrgCodeID").val(dataItem.ID);
    $("#txtDRGDescription").val(dataItem.Name);
}

function OnDRGDescriptionSelection(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtDRGCode").val(dataItem.Code);
    $('#hdDrgCodeValue').val(dataItem.Code);
    $("#hdDrgCodeID").val(dataItem.ID);
    $("#txtDRGDescription").val(dataItem.Name);
}
//-----------DRG feature end here-----------------------

//-----------major CPT-------------
function OnCPTCodeSelection(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtCPTCode").val(dataItem.Menu_Title);
    $('#hdCPTCodeValue').val(dataItem.Code);
    $("#hdCPTCodeID").val(dataItem.ID);
}

function DisableCPTPanel(isMajorCpt) {
    if (isMajorCpt == "False") {
        $("#lblMajorCPTInstruction").text("* You have already entered the Major CPT");
        $("#txtCPTCode").prop("disabled", "disabled");
    }
    else {
        $("#lblMajorCPTInstruction").text("* You can search the Major CPT for the code or description.");
        $("#txtCPTCode").removeAttr("disabled");
    }
}

function DisableDRGPanel(isMajorDrg) {
    if (isMajorDrg == "False") {
        $("#lblMajorDRGInstruction").text("* You have already entered the DRG.");
        $("#txtDRGCode").prop("disabled", "disabled");
    }
    else {
        $("#lblMajorDRGInstruction").text("* You can search the DRG for the code or description.");
        $("#txtDRGCode").removeAttr("disabled");
    }
}

//-----------major CPT-------------



//------------Not in use---------------
function selectPrimaryDiagnosisDesc(e) {
    var dataItem = this.dataItem(e.item.index());
    //alert(dataItem.ID);
    //alert(dataItem.Menu_Title);
    $("#primaryDiagnosisCode").val(dataItem.ID);
    $("#hdprimaryCodeId").val(dataItem.ID);
    $("#txtPrimaryDiagnosisDescription").val(dataItem.Name);
}

function AddToCurrentDiagnosisRecord(id, pid) {
    var jsonData = JSON.stringify({
        Id: id,
        patientId: pid
    });
    $.ajax({
        type: "POST",
        url: '/Diagnosis/AddToCurrentDiagnosis',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                if (data == "-1") {
                    ShowMessage("Unable to add primary diagnosis!", "Warning", "warning", true);
                }
                else if (data == "-2") {
                    ShowMessage("Record with same diagnosis code exist!", "Warning", "warning", true);
                } else {
                    BindDiagnosisList(patientId);
                }
            }
        },
        error: function (msg) {
        }
    });
}
//------------Not in use---------------


function AddAsFavDiagnosis(id, diagnosisCode) {
    var jsonData = JSON.stringify({
        codeId: id,
        categoryId: diagnosisCode,
        id: 0,
        isFavorite: true,
        favoriteDesc: '',
        Dtype: 'true'
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/AddToFavorites",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            if (data != null) {
                if (data == "1") {
                    ShowMessage("Record already exist.", "Info", "warning", true);
                } else {
                    ShowMessage("Record added successfully.", "Success", "success", true);
                    $('#phyFavDiagnosisGrid').empty();
                    $('#phyFavDiagnosisGrid').html(data);
                    DiagnosisOnReady();
                }
            }
        },
        error: function (msg) {

        }
    });
}

function AddAsDiagnosis(codeid) {
    var txtPatientID = $("#hdPatientId").val();
    var txtEncounterID = $('#hdEncounterId').val(); //$('#hdCurrentEncounterId').val();
    var jsonData = JSON.stringify({
        Id: codeid,
        Pid: txtPatientID,
        Eid: txtEncounterID
    });
    $.ajax({
        type: "POST",
        url: '/Diagnosis/GetDiagnosisByCodeId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                BindDiagnosisDetails(data);
            }
        },
        error: function (msg) {
        }
    });
}

function DeleteFavDiagnosis() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        $.ajax({
            type: "POST",
            url: '/PhysicianFavorites/DeleteFav',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ Id: $("#hfGlobalConfirmId").val() }),
            success: function (data) {
                if (data != null) {
                    var msg = "Records Deleted successfully !";
                    GetPhysicianFavDiagnosis();
                    ShowMessage(msg, "Success", "success", true);
                }
            },
            error: function (msg) {
            }
        });
    }
}

//function DeleteFavDiagnosis(id) {
//    if (confirm("Do you really want to delete this record?")) {
//        $.ajax({
//            type: "POST",
//            url: '/PhysicianFavorites/DeleteFav',
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: JSON.stringify({ Id: id }),
//            success: function (data) {
//                if (data != null) {
//                    var msg = "Records Deleted successfully !";
//                    GetPhysicianFavDiagnosis();
//                    ShowMessage(msg, "Success", "success", true);
//                }
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

function GetPhysicianFavDiagnosis() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Diagnosis/GetFavoritesDiagnosis",
        data: null,
        dataType: "html",
        success: function (data) {
            $("#phyFavDiagnosisGrid").empty();
            $("#phyFavDiagnosisGrid").html(data);
            DiagnosisOnReady();
        },
        error: function (msg) {
        }
    });
    return false;
}

function DiagnosisOnReady() {
    $("#diagnosisAddEdit").validationEngine();
    var isPrimary = $("#hdIsPrimary").val();
    SetValueInDiagnosisType(isPrimary);
    $(".ddlType1").prop('disabled', 'disabled');
    $('.AddAsDiagnosis').show();
    $('.RemoveDiagnosis').hide();
    var isMajorCPTEntered = $("#hdIsMajorCPTEntered").val();
    DisableCPTPanel(isMajorCPTEntered);
}

function ShowDiagnosisActions() {
    BindDiagnosisTabData();
    $(".diagnosisActions").show();
    toggleRadioButtons("#rdReviewedByPhysician");
    $("#divDiagnosisReview").attr("disabled", false);
    //$("#DRGCodesInDiagnosisDiv").hide();
    var isPrimary = $("#hdIsPrimary").val();
    $('#hdCurrentDiagnosisID').val($('#hdDiagnosisID').val());
    SetValueInDiagnosisType(isPrimary);
}

function BindDiagnosisTabData() {
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: pid,
        encounterId: eid
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetDiagnosisTabData",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            if (data != null) {
                $('.ehrtabs').empty();
                $("#diagnosis").empty();
                $("#diagnosis").html(data);
                DiagnosisOnReady();
            }
        },
        error: function (msg) {

        }
    });
}

function SortPhFav(event) {
    var url = "/Diagnosis/GetFavoritesDiagnosis";
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
        success: function(data) {
            $("#phyFavDiagnosisGrid").empty();
            $("#phyFavDiagnosisGrid").html(data);
            $(".AddAsDiagnosis").show();
            $(".RemoveDiagnosis").hide();
        },
        error: function(msg) {
        }
    });
}

function SortPatientPreviousDiagnosis(event) {
    var url = "/Summary/GetPreviousDiagnosisData";
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?patientId=" + pid + "&encounterId=" + eid + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#previousDiagnosisGrid").empty();
            $("#previousDiagnosisGrid").html(data);
        },
        error: function (msg) {
        }
    });
}

function SortDiagnosisTabGrid(event) {
    var url = "/Diagnosis/SortDiagnosisTabGrid";
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    if (encounterId == null || encounterId == "") {
        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
            url += "?" + "&" + event.data.msg;
        }
    } else {
        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
            url += "?Pid=" + patientId + "&Eid=" + encounterId + "&" + event.data.msg;
        }
    }
    
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: JSON.stringify({ patientId: patientId, encounterId: encounterId }),
        success: function (data) {
            BindList("#CurrentDiagnosisGrid", data);
        },
        error: function (msg) {
        }
    });
}