$(function () {
    $("#diagnosisAddEdit").validationEngine();
    var isPrimary = $("#hdIsPrimary").val();
    SetValueInDiagnosisType(isPrimary);
    $(".ddlType1").prop('disabled', 'disabled');
    $('.AddAsDiagnosis').show();

    //-------------Added for Super Powers functionality-------------///
    var patientId = $("#hdPatientId").val();
    var encounterId = GetEncounterIdInDiagnosis();

    $("#GlobalPatientId").val(patientId);
    $("#GlobalEncounterId").val(encounterId);
    BindLinkUrlsForSuperPowers();
    //-------------Added for Super Powers functionality-------------///


    GetDiagnosisTabDetails();
});

function SelectDiagnosisCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#primaryDiagnosisCode").val(dataItem.Menu_Title);
    $("#hdprimaryCodeId").val(dataItem.ID);
    $("#hdprimaryCodeValue").val(dataItem.Name);
}

function ResetDiagnosisForm() {
    var hdPatientid = $('#hdPatientId').val();
    var hdIsPrimary = $('#hdIsPrimary').val();
    var hdCreatedBy = $('#hdCreatedBy').val();
    var hdCreatedDate = $('#hdCreatedDate').val();
    var encounterId = GetEncounterIdInDiagnosis();
    var hdCorporateId = $('#hdCorporateId').val();
    var hdfacilityId = $('#hdfacilityId').val();
    var hdMedicalRecordNumber = $('#hdMedicalRecordNumber').val();
    var hdIsMajorCPTEntered = $('#hdIsMajorCPTEntered').val();
    var hdIsMajorDRGEntered = $('#hdIsMajorDRGEntered').val();

    $("#diagnosisAddEdit").clearForm(true);
    $.validationEngine.closePrompt(".formError", true);

    $('#hdPatientId').val(hdPatientid);
    $('#hdIsPrimary').val(hdIsPrimary);
    $('#hdCreatedBy').val(hdCreatedBy);
    $('#hdCreatedDate').val(hdCreatedDate);

    SetEncounterIdInDiagnosis(encounterId);


    $('#hdCorporateId').val(hdCorporateId);
    $('#hdfacilityId').val(hdfacilityId);
    $('#hdMedicalRecordNumber').val(hdMedicalRecordNumber);
    $('#hdIsMajorCPTEntered').val(hdIsMajorCPTEntered);
    $('#hdIsMajorDRGEntered').val(hdIsMajorDRGEntered);

    $('.btnSave').val('Save');
    $("#hdDiagnosisID").val(0);
    $("#rdReviewedByPhysician").prop("checked", "checked");
    $("#rdIntiallyEnteredBy").prop("checked", "checked");
    $('#collapseDiagnosisList').addClass('in');
    SetValueInDiagnosisType(hdIsPrimary);
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
                BindList('#CurrentDiagnosisGrid', data);
                BindList('#colCurrentDiagnosisMain', data);
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
    var encounterId = GetEncounterIdInDiagnosis();
    if (encounterId == 0) {
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

        if (encounterId > 0) {
            var jsonData = JSON.stringify({
                DiagnosisID: id,
                DiagnosisType: diagnosisType,
                CorporateID: corporateId,
                FacilityID: facilityId,
                PatientID: hdPatientId,
                EncounterID: encounterId,
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
                        var currenttabData = data.currenttabData;

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

                            ResetDiagnosisForm(hdPatientId);
                            BindCurrentDiagnosis(currenttabData);
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
                BindDiagnosisDetails(data, 0);
                DiagnosisOnReady();
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
                BindDiagnosisDetails(data, data.id);
            }
        },
        error: function (msg) {
        }
    });
}

function BindDiagnosisDetails(data, id) {
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
    $("#hdDiagnosisID").val(id);
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
                var msg = "Records Deleted successfully!";
                if (data.Status < 0) {
                    switch (data.Status) {
                        case -1:
                            msg = "You are not authorized to delete the record!!";
                            break;
                        case -2:
                            msg = "Primary Diagnosis cannot be deleted!!";
                            break;
                        default:
                            msg = "Error deleting the Record. Try again in few minutes!!";
                    }
                    ShowMessage(msg, "Warning", "warning", true);
                }
                else {
                    $("#hdIsPrimary").val(data.IsPrimary ? "False" : "True");
                    $("#hdIsMajorCPTEntered").val(data.IsMajorCPT != null && data.isMajorCptDone == true ? "False" : "True");
                    $("#hdIsMajorDRGEntered").val(data.IsMajorDRG != null && data.isDrgDone == true ? "False" : "True");
                    ResetDiagnosisForm();
                    $(".diagnosisActions").show();
                    toggleRadioButtons("#rdReviewedByPhysician");
                    if ($("#divDiagnosisReview").length > 0)
                        $("#divDiagnosisReview").attr("disabled", false);

                    if ($('#hdCurrentDiagnosisID').length > 0)
                        $('#hdCurrentDiagnosisID').val($('#hdDiagnosisID').val());

                    BindCurrentDiagnosis(data.list);
                    ShowMessage(msg, "Success", "success", true);
                }
            },
            error: function (msg) {
            }
        });
    }
}

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
                BindDiagnosisDetails(data, data.id);
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


    var isMajorCPTEntered = $("#hdIsMajorCPTEntered").val();
    DisableCPTPanel(isMajorCPTEntered);
}

function BindDiagnosisData() {
    var pid = $("#hdPatientId").val();
    var eid = GetEncounterIdInDiagnosis();
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
            //GetDiagnosisTabData();

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
        success: function (data) {
            $("#phyFavDiagnosisGrid").empty();
            $("#phyFavDiagnosisGrid").html(data);
            $(".AddAsDiagnosis").show();
            //$(".RemoveDiagnosis").hide();
        },
        error: function (msg) {
        }
    });
}

function SortPatientPreviousDiagnosis(event) {
    var url = "/Summary/GetPreviousDiagnosisData";
    var pid = $("#hdPatientId").val();
    var eid = GetEncounterIdInDiagnosis();
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

//function SortDiagnosisTabGrid(event) {
//    var url = "/Diagnosis/SortDiagnosisTabGrid";
//    var patientId = $("#hdPatientId").val();
//    var encounterId = GetEncounterIdInDiagnosis();
//    if (encounterId == null || encounterId == "") {
//        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
//            url += "?" + "&" + event.data.msg;
//        }
//    } else {
//        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
//            url += "?Pid=" + patientId + "&Eid=" + encounterId + "&" + event.data.msg;
//        }
//    }

//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        url: url,
//        dataType: "html",
//        async: false,
//        data: JSON.stringify({ patientId: patientId, encounterId: encounterId }),
//        success: function (data) {
//            BindList("#CurrentDiagnosisGrid", data);
//        },
//        error: function (msg) {
//        }
//    });
//}

function DeleteFavoriteDiagnosis() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        $.ajax({
            type: "POST",
            url: "/PhysicianFavorites/DeleteFav",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ Id: $("#hfGlobalConfirmId").val() }),
            success: function (data) {
                if (data != null) {
                    var msg = "Records Deleted successfully !";
                    ShowMessage(msg, "Success", "success", true);
                }
            },
            error: function (msg) {
            }
        });
    }
}





//////$$$$$$$$$$$$$$$$$$$

function GetDiagnosisTabDetails() {
    var pid = $("#hdPatientId").val();
    var eid = GetEncounterIdInDiagnosis();
    var jsonData = JSON.stringify({
        patientId: pid,
        encounterId: eid
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetDiagnosisTabDetails",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            BindCurrentDiagnosis(data.DiagnosisList);
            BindFavoriteDiagnosis(data.FavoriteDiagnosisList);
        },
        error: function (msg) {

        }
    });
}

function BindCurrentDiagnosis(data) {
    var cColumns = [{ "targets": 0, "visible": false }, { "targets": 1, "visible": false },
    {
        "targets": 8,
        "mRender": function (data, type, full) {
            var DiagnosisType = full[8];
            var diagcode = full[3];

            var anchortags = "<div style='display:flex'>";
            var openconfirm = "return OpenConfirmPopup('" + full[0] + "','Delete Current Diagnosis','',DeleteDiagnosis,null); ";

            if (DiagnosisType == 1 || DiagnosisType == 2) {
                var edit = "EditCurrentDiagnosis('" + full[0] + "'); ";
                anchortags += '<a href="#" title="Edit Current Diagnosis" onclick="' + edit + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src= "../images/edit_small.png" /></a>';
            }
            var edit = "EditDiagnosisRecord('" + full[0] + "'); ";

            if (DiagnosisType == 3) {
                var addfav = 'AddAsFavDiagnosisInDiagnosis("' + diagcode + '", "9")';

                anchortags += '<a href="javascript:void(0);" class="hideSummary" title="Edit Current DRG" onclick="' + edit + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src="../images/edit_small.png" /></a>' +
                    '<a href="javascript:void(0);" class="hideSummary favdiag" title="Add As Favorite" onclick="' + addfav + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src="../images/Fav (1).png" /></a>';
            }
            if (DiagnosisType == 4) {
                anchortags += '<a href="javascript:void(0);" class="hideSummary" title="Edit Current CPT" onclick="' + edit + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src="../images/edit_small.png" /></a>';
            }
            if (DiagnosisType != 1) {
                anchortags += '<a href="javascript:void(0);" title="Delete Current Diagnosis" class="hideSummary" onclick="' + openconfirm + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src="../images/delete_small.png" /></a>';

            }
            if (DiagnosisType == 1 || DiagnosisType == 2) {
                var addfav = "AddAsFavDiagnosisInDiagnosis('" + diagcode + "', '16') ";
                anchortags += '<a href="javascript:void(0);" title="Add As Favorite" class="hideSummary favdiag" onclick="' + addfav + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src="../images/Fav (1).png" /></a>';
            }
            return anchortags + "</div>";
        }
    }];
    $('#diagnosisCurrent').dataTable({
        destroy: true,
        aaData: data,
        scrollY: "200px",
        scrollCollapse: true,
        bProcessing: true,
        paging: true,
        aoColumnDefs: cColumns
    });
}

function BindFavoriteDiagnosis(data) {
    var favColumns = [{ "targets": 0, "visible": false },
    {
        "targets": 5,
        "mRender": function (data, type, full) {
            var openconfirm = "return OpenConfirmPopup('" + full[0] + "','Delete Favorite','',DeleteFavoriteDiagnosisRecord," + null + "); ";
            var adddia = "AddAsDiagnosis('" + full[5] + "')";
            var deleteicon = '<div style="display:flex;"><a href="#" class="AddAsDiagnosis" title="Add as Diagnosis" onclick="' + adddia + '" style="float: left; margin-right: 7px; margin-left: 5px; width: 15px;"><img class="img-responsive" src="../images/edit_small.png" /></a><a href="#" class="RemoveDiagnosis" title="Remove Favorite" onclick="' + openconfirm + '" style="float: left; margin-right: 7px; margin-left: 5px; width: 15px;"><img class="img-responsive" src= "../images/delete_small.png" /></a></div>';
            return deleteicon;
        }
    }];
    $('#diagnosisfav').dataTable({
        destroy: true,
        aaData: data,
        scrollY: "200px",
        scrollCollapse: true,
        bProcessing: true,
        paging: true,
        aoColumnDefs: favColumns
    });
}

function DeleteFavoriteDiagnosisRecord() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        $.ajax({
            type: "POST",
            url: "/PhysicianFavorites/DeleteFav",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ Id: $("#hfGlobalConfirmId").val() }),
            success: function (data) {
                if (data != null) {
                    var msg = "Records Deleted successfully !";
                    GetFavoriteDiagnosisData();
                    ShowMessage(msg, "Success", "success", true);
                }
            },
            error: function (msg) {
            }
        });
    }
}

function GetFavoriteDiagnosisData() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Diagnosis/GetFavoriteDiagnosisData",
        data: null,
        dataType: "json",
        success: function (data) {
            BindFavoriteDiagnosisData(data);
            DiagnosisOnReady();
        },
        error: function (msg) {
        }
    });
    return false;
}

function AddAsDiagnosis(codeid) {
    debugger;

    var txtPatientId = $("#hdPatientId").val();
    var txtEncounterId = GetEncounterIdInDiagnosis();
    var jsonData = JSON.stringify({
        Id: codeid,
        Pid: txtPatientId,
        Eid: txtEncounterId
    });
    $.ajax({
        type: "POST",
        url: "/Diagnosis/GetDiagnosisByCodeId",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                BindDiagnosisDetails(data);
                DiagnosisOnReady();
            }
        },
        error: function (msg) {
        }
    });
}

function AddAsFavDiagnosisInDiagnosis(id, diagnosisCode) {
    var jsonData = JSON.stringify({
        codeId: id,
        categoryId: diagnosisCode,
        id: 0,
        isFavorite: true,
        favoriteDesc: "",
        Dtype: "true"
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/AddToFavorites",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            if (data != null) {
                if (data == 1) {
                    ShowMessage("This is already in your Favorite Diagnosis List. Try the other one to add into your bucket!", "Info", "warning", true);
                } else {
                    BindFavoriteDiagnosisData(data);
                    DiagnosisOnReady();
                    ShowMessage("Record added successfully.", "Success", "success", true);
                }
            }
        },
        error: function (msg) {

        }
    });
}

function GetEncounterIdInDiagnosis() {
    var encounterId = 0;
    if ($("#hdCurrentEncounterId").length == 0)
        encounterId = $("#hdEncounterId").val() != "" && $("#hdEncounterId").val() != null ? $("#hdEncounterId").val() : 0;
    else
        encounterId = $("#hdCurrentEncounterId").val() != "" && $("#hdCurrentEncounterId").val() != null ? $("#hdCurrentEncounterId").val() : 0;

    return encounterId;
}

function SetEncounterIdInDiagnosis(encounterId) {
    if ($('#hdEncounterId').length > 0)
        $('#hdEncounterId').val(encounterId);
    if ($('#hdCurrentEncounterId').length > 0)
        $('#hdCurrentEncounterId').val(encounterId);
}