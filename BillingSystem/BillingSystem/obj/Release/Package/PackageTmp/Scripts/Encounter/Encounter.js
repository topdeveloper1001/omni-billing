//********GLobal Codes:
//    1-Outpatient(Day Patient)
//    2-Inpatient(Admit Patient)
//    3-ER(Emergency)


//******ENUM: to flag the encpunter type 
//1- Admit Encounter type
//2- Outpatient Encounter type
//3- Discharge Inpatient encounter type
//4- End Outpaitent Encounter type

$(function () {
    $("#ddlPhysicians").change(function () {
        var selected = $(this).val();
        if (selected > 0) {
            var physicianSpecility = $('option:selected', this).attr("specility");
            var licenseType = $('option:selected', this).attr("lType");
            //var roleName = $('option:selected', this).attr("roleName");

            //SetSpecialtyDropdownVisibility(roleName);
            $("#ddlEncounterSpeciality").val(physicianSpecility);
            $("#ddlPhysicianType").val(licenseType);
        } else {
            $("#ddlUserType").val(0);
            $("#ddlPhysicianType").val('');
            $("#ddlEncounterSpeciality").empty();
        }
    });


    $("#AdmitPatientDiv").validationEngine();

    //Bind Facility to Facility TextBox, starts here
    var facilityNumber = $("#hdEncounterFacility").val();
    if (facilityNumber != '' && facilityNumber != null) {
        facilityNumber = "";
    }

    BindGlobalCodesWithValue("#ddlEncounterSpeciality", 1121, "#hdEncounterSpecialty");
    GetFacilityName(facilityNumber);
    //Bind Facility to Facility TextBox end here

    //Bind EncounterPatientType    
    var hdEncounterPatientType = $("#hdEncounterPatientType").val();
    BindPatientTypesDropdownData(1107, hdEncounterPatientType);
    //hdencounterPatientTypechecked

    setTimeout(function () { ShowHideControls(); }, 1000);
    //Added by Amit Jain on 14012015
    if (hdEncounterPatientType != '' && hdEncounterPatientType == 3) {
        $("#DivHomeCareRecurring").show();
    } else {
        $("#DivHomeCareRecurring").hide();
    }


    //Bind EncounterType    
    var hdEncounterType = $("#hdEncounterType").val();
    BindEncounterTypesDropdownData(1113, hdEncounterType, hdEncounterPatientType);

    BindAllPhysiciansData();

    //Bind Physician Type
    //var hdPhysicianType = $("#hdPhysicianType").val();
    //BindPhysiciansTypeDropdownData(1104, hdPhysicianType);

    //if (hdPhysicianType != '') {
    //    var hdEncounterAttendingPhysician = $("#hdEncounterAttendingPhysician").val();
    //    BindPhysicianDropdownData(hdPhysicianType, hdEncounterAttendingPhysician);
    //}


    //var hdEncounterAttendingPhysician = $("#hdEncounterAttendingPhysician").val();
    //BindPhysicianDropdownData(hdPhysicianType, hdEncounterAttendingPhysician);


    //Bind EncounterStartType
    var hdEncounterStartType = $("#hdEncounterStartType").val();
    BindEncounterStartTypeDropdownData(1116, hdEncounterStartType);

    //Commented on 16062015 by Amit Jain
    ////Bind ServiceCategory
    //var hdEncounterServiceCategory = $("#hdEncounterServiceCategory").val();
    //BindServiceCategoryDropdownData(1115, hdEncounterServiceCategory);

    //Bind ConfidentialLevel
    // var hdEncounterConfidentialityLevel = $("#hdEncounterConfidentialityLevel").val();
    // BindConfidentialLevelDropdownData(1105, hdEncounterConfidentialityLevel);

    //Bind Encounter Speciality
    //var hdEncounterSpecialty = $("#hdEncounterSpecialty").val();
    //BindEncounterSpecialityDropdownData(1117, hdEncounterSpecialty);

    //Bind EncounterModeOfArrival
    var hdEncounterModeofArrival = $("#hdEncounterModeofArrival").val();
    BindEncounterModeOfArrivalDropdownData(1106, hdEncounterModeofArrival);

    //Bind EncounterAdmitType
    var hdEncounterAdmitType = $("#hdEncounterAdmitType").val();
    BindEncounterAdmitTypeDropdownData(1108, hdEncounterAdmitType);

    //Bind EncounterAccidentRelated
    var hdEncounterAccidentRelated = $("#hdEncounterAccidentRelated").val();
    BindEncounterAccidentRelatedDropdownData(1109, hdEncounterAccidentRelated);

    if ($("#hdencounterPatientTypechecke").val() == "3" || $("#hdencounterPatientTypechecke").val() == "4" || $("#hdencounterPatientTypechecke").val() == "9") {
        // ENUM: 3 Specify the User being discharged , 4: ending the outpatient encounter; 5: Specifies the transfer ptient.
        $(".endTypeDiv").show();//... Show the End encounter type div
    } else {
        $(".endTypeDiv").hide();//... Hide the End encounter type div
    }

    //Bind EncounterEndType
    var hdEncounterEndType = $("#hdEncounterEndType").val();
    if ($("#hdencounterPatientTypechecke").val() == "9") {
        hdEncounterEndType = "5";
    }
    BindEncounterEndTypeDropdownData(1114, hdEncounterEndType);

    if ($("#hdencounterPatientTypechecke").val() == "2" || $("#hdencounterPatientTypechecke").val() == "4") {
        $('.AssignBed').hide();
        $("#ddlPatientTypes option[value='2']").remove();
    }

    if ($("#hdencounterPatientTypechecke").val() == "3" || $("#hdencounterPatientTypechecke").val() == "4" || $("#hdencounterPatientTypechecke").val() == "9") {
        $(".starttype").attr("disabled", "disabled");
    }


    $("#ddlPatientTypes").on("change", function () {
        if ($("#ddlPatientTypes").val() == "3") {
            $("#DivHomeCareRecurring").show();
        } else {
            $("#DivHomeCareRecurring").hide();
        }

        if ($("#ddlPatientTypes").val() == "2") {
            $('.AssignBed').show();
            $("#aChooseBed").show();
            $("#ddlEncounterAdmitType").addClass("validate[required]");
        } else {
            $('.AssignBed').hide();
            $("#aChooseBed").hide();
            $("#ddlEncounterAdmitType").removeClass("validate[required]");
        }
    });

    //-------------Added for Super Powers functionality-------------///
    $("#GlobalPatientId").val($('#hdPatientID').length > 0 && $('#hdPatientID').val() > 0 ? $("#hdPatientID").val() : 0);
    $("#GlobalEncounterId").val($('#EncounterID').length > 0 && $('#EncounterID').val() > 0 ? $("#EncounterID").val() : 0);
    BindLinkUrlsForSuperPowers();
    //-------------Added for Super Powers functionality-------------///

    setTimeout(function () {
        if ($("#ddlPatientTypes").val() == "2") {
            $('.AssignBed').show(); $("#aChooseBed").show();
            $("#ddlEncounterAdmitType").addClass("validate[required]");
        } else {
            $('.AssignBed').hide(); $("#aChooseBed").hide();
            $("#ddlEncounterAdmitType").removeClass("validate[required]");
        }
        if ($("#hdencounterPatientTypechecke").val() == "9") {
            $('#ddlEncounterEndType').attr('disabled', 'disabled');
            $('#ddlEncounterEndType').val('5');
        }
    }, 800);


    //$("#ddlEncounterStartType").onChange(function () {
    //    var ddlEncounterStartType = $("#ddlEncounterStartType").val();
    //    if (hdEncounterPatientType == 3) {
    //        if (ddlEncounterStartType == 1) {
    //            $("#ddlPatientTypes").val(3);
    //        }
    //        else if (ddlEncounterStartType == 2) {
    //            $("#ddlPatientTypes").val(1);
    //        }
    //    }
    //});
});

function SetEncounterType() {
    var hdEncounterPatientType = $("#hdEncounterPatientType").val();
    var ddlEncounterStartType = $("#ddlEncounterStartType").val();
    //var value = $(this).val();
    if (hdEncounterPatientType == 3) {
        if (ddlEncounterStartType == 1) {
            $("#ddlPatientTypes").val(3);
        }
        else if (ddlEncounterStartType == 2) {
            $("#ddlPatientTypes").val(1);
        }
        OnChangePatientType('#ddlPatientTypes');
    }
}

function ShowHideControls() {
    if ($("#hdEncounterPatientType").val() == "2") {//Global Code: 2 specify the Admit Patient from global codes.
        $("#ddlPatientTypes").attr("disabled", "disabled");// .. drop down will be disable for admit patient type
        if ($("#hdencounterPatientTypechecke").val() == "1" || $("#hdencounterPatientTypechecke").val() == "3" || $("#hdencounterPatientTypechecke").val() == "5" || $("#hdencounterPatientTypechecke").val() == "9") {
            // ENUM: 1 Specify the User id being admitted, 3 means encunter end and 5 means that encounter transfer. 
            // 9 means that the patient is being virtually discharged
            $('#ddlPatientTypes').val('2');
            $('#ddlPatientTypes').attr('disabled', 'disabled');
            $('.AssignBed').show();//...Assign bed will show for Admiiting the patient.
            if ($("#hdencounterPatientTypechecke").val() == "5") {
                $('#divTransferPatient').addClass('yellowBox1');
                $('#btnUpdateEncounter').val('Admit');
                $("#hdEncounterType").val('');
            } else {
                $('#divTransferPatient').removeClass('yellowBox1');
                $('#btnUpdateEncounter').val('Update');
            }
        } else {
            $('#ddlPatientTypes').removeAttr('disabled');
            $('.AssignBed').hide();//...Assign bed will hide for other encounter type for the patient.btnUpdateEncounter
            $('#divTransferPatient').removeClass('yellowBox1');
            $('#btnUpdateEncounter').val('Update');
        }
        if ($("#hdencounterPatientTypechecke").val() == "3" || $("#hdencounterPatientTypechecke").val() == "9") {
            $("#aChooseBed").hide();
        } else {
            $("#aChooseBed").show();
        }
    } else {
        $("#ddlPatientTypes").removeAttr("disabled");
    }
}

function AdmitPatient(id) {
    var isValid = $("#AdmitPatientDiv").validationEngine({ returnIsValid: true });

    if (isValid == true) {
        if ($("#ddlPatientTypes").val() == "2") {
            if ($("#ddlEncounterEndType").val() == null) {
                if ($("#hidBedId").val() == "0" || $("#hidBedId").val() == "") {
                    ShowMessage("Please select bed for in patient type.", "error", "warning", true);
                    return false;
                }
            }
        }
        var hdPatientID = $("#hdPatientID").val();

        var hdEncounterFacility = $("#hdEncounterFacility").val();
        var ddlPatientTypes = $("#ddlPatientTypes").val();
        var ddlPhysicianType = $("#ddlPhysicianType").val();
        var ddlPhysicians = $("#ddlPhysicians").val();
        var txtEncounterTransferHospital = $("#txtEncounterTransferHospital").val();
        var txtEncounterTransferSource = $("#txtEncounterTransferSource").val();
        var txtEncounterAdmitReason = $("#txtEncounterAdmitReason").val();
        var ddlEncounterTypes = $("#ddlEncounterTypes").val();
        var ddlEncounterStartType = $("#ddlEncounterStartType").val();
        var txtEncounterStartTime = $("#txtEncounterStartTime").val();//new Date($("#txtEncounterStartTime").val());
        var txtEncounterInpatientAdmitDate = new Date($("#txtEncounterInpatientAdmitDate").val());

        //Commented on 16062015 by Amit Jain
        //var ddlServiceCategory = $("#ddlServiceCategory").val();

        var ddlEncounterSpeciality = $("#ddlEncounterSpeciality").val();
        var ddlEncounterModeOfArrival = $("#ddlEncounterModeOfArrival").val();
        var ddlEncounterAdmitType = $("#ddlEncounterAdmitType").val();
        var ddlAccidentRelated = $("#ddlAccidentRelated").val();
        var ddlAccidentType = $("#ddlAccidentType").val();

        //In case of Discharge Patient / End Encounter
        if (($("#hdencounterPatientTypechecke").val() == "1" || $("#hdencounterPatientTypechecke").val() == "2")) {
            $("#txtEncounterEndTime").val(null);
            $("#ddlEncounterEndType").val(null);
        }
        var ddlEncounterEndType = $("#ddlEncounterEndType").val() != null ? $("#ddlEncounterEndType").val() : null;
        var txtEncounterEndTime = $("#txtEncounterEndTime").val() != null ? $("#txtEncounterEndTime").val() : null;
        var hidBedId = $("#hidBedId").val();
        var newDateCheck = new Date();
        var newdateCustom = (newDateCheck.getMonth() + 1) + "/" + (newDateCheck.getDate()) + "/" + (newDateCheck.getFullYear());
        var txtBedStartDate = newdateCustom;// $("#txtStartDate").val() != "" ? $("#txtStartDate").val() : newdateCustom;

        var txtBedExpectedEndDate = newdateCustom;// $("#txtEndDate").val() != "" ? $("#txtEndDate").val() : newdateCustom;
        var hdOverrideBedType = $("#hidBedOverideType").val();
        var recurring = false;

        if ($("#chkHomeCareRecurring") != null && $("#chkHomeCareRecurring")[0].checked) {
            recurring = true;
        }
        var jsonData = JSON.stringify({
            EncounterID: id,
            EncounterFacility: hdEncounterFacility,
            PatientID: hdPatientID,
            EncounterStartTime: txtEncounterStartTime,
            EncounterPatientType: ddlPatientTypes,
            //  EncounterConfidentialityLevel: ddlConfidentialLevel,

            //Commented on 16062015 by Amit Jain
            //EncounterServiceCategory: ddlServiceCategory,
            EncounterServiceCategory: 1,
            EncounterTransferHospital: txtEncounterTransferHospital,
            EncounterTransferSource: txtEncounterTransferSource,
            EncounterAdmitReason: txtEncounterAdmitReason,
            EncounterType: ddlEncounterTypes,
            EncounterStartType: ddlEncounterStartType,
            EncounterInpatientAdmitDate: txtEncounterInpatientAdmitDate,
            EncounterSpecialty: ddlEncounterSpeciality,
            EncounterModeofArrival: ddlEncounterModeOfArrival,
            EncounterAdmitType: ddlEncounterAdmitType,
            EncounterAccidentRelated: ddlAccidentRelated,
            EncounterAccidentType: ddlAccidentType,
            EncounterEndType: ddlEncounterEndType != null ? ddlEncounterEndType : 0,
            EncounterEndTime: txtEncounterEndTime != null ? txtEncounterEndTime : null,//new Date(txtEncounterEndTime) : null,
            patientBedStartDate: txtBedStartDate,
            patientBedExpectedEndDate: txtBedExpectedEndDate,
            patientBedService: 0,
            patientBedId: hidBedId,
            patientBedEndDate: txtEncounterEndTime,
            EncounterPhysicianType: ddlPhysicianType,
            EncounterAttendingPhysician: ddlPhysicians,
            OverrideBedType: hdOverrideBedType,
            EncounterPatientTypecheck: $("#hdencounterPatientTypechecke").val(),
            HomeCareRecurring: recurring
        });

        $.ajax({
            type: "POST",
            url: '/Encounter/AddUpdateEncounter',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data) {
                    if (data.IsFutureOpenOrders) {
                        ShowMessageWithDuration("Encounter Starts successfully!", "Success", "success", true, 2000);
                        if (confirm("Patient have future open orders! Do you like to add those orders to current encounter")) {
                            ShowFutureOpenOrders(hdPatientID, data.encId);
                        };
                        ClearAll();
                    }
                    else if (data == "Success") {
                        var msg = "Records Saved successfully !";
                        if (id > 0)
                            msg = "Records updated successfully";

                        ShowMessageWithDuration(msg, "Success", "success", true, 2000);
                        ClearAll();
                        setTimeout(function () {
                            window.location = window.location.protocol + "//" + window.location.host + "/ActiveEncounter/ActiveEncounter";
                        }, 1000);
                    }
                    else if (data == "AuthError") {
                        ShowMessageWithDuration("Unable to End Encounter, Reason : Authorization is not obtained so cannot proceed to Bill", "Warning", "warning", true, 2000);
                    }
                    else if (data == "Error") {
                        ShowMessageWithDuration("Unable to End Encounter, Reason : Authorization is not obtained so cannot proceed to Bill", "Warning", "warning", true, 2000);
                    }
                    else if (data == "OrderError") {
                        ShowMessageWithDuration("Unable to End Encounter, Reason : Encounter have some open orders that need to be attended.", "Warning", "warning", true, 2000);
                    }
                    else if (data == "OrderError") {
                        ShowMessageWithDuration("Unable to End Encounter, Reason : Encounter have some open orders that need to be attended.", "Warning", "warning", true, 2000);
                    }
                    else {
                        var msg1 = "Records Saved successfully !";
                        if (id > 0)
                            msg1 = "Records updated successfully";

                        ShowMessageWithDuration(msg1, "Success", "success", true, 2000);
                        ClearAll();
                        setTimeout(function () {
                            window.location = window.location.protocol + "//" + window.location.host + "/ActiveEncounter/ActiveEncounter";
                        }, 1000);
                    }
                }
            },
            error: function (msg) {

            }
        });
    }
    return false;
}

function ClearForm() {
    $("#AdmitPatientDiv").clearForm();
    $('#collapseThree').addClass('in');
}

function ClearAll() {
    ClearForm();
    $.validationEngine.closePrompt(".formError", true);
}

function BindPatientTypesDropdownData(categoryId, codeId) {
    var jsonData = JSON.stringify({ categoryId: categoryId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetGlobalCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlPatientTypes").empty();
            var items = '<option value="0">-Select-</option>';
            $.each(data, function (i, code) {
                items += "<option value='" + code.GlobalCodeValue + "'>" + code.GlobalCodeName + "</option>";
            });
            $("#ddlPatientTypes").html(items);

            if (codeId == null || codeId == '')
                codeId = "0";
            $("#ddlPatientTypes").val(codeId);
            if (codeId == "2") {
                $('.AssignBed').show();
                $("#aChooseBed").show();
                $("#ddlEncounterAdmitType").addClass("validate[required]");
            }//...Assign bed will show for Admiiting the patient.
        },
        error: function (msg) {

        }
    });
    return false;
}

function BindEncounterTypesDropdownData(categoryId, codeId, pTypeId) {
    var jsonData = JSON.stringify({ categoryId: categoryId, patientTypeId: pTypeId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetEncounterTypes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlEncounterTypes").empty();
            var items = '<option value="0">-Select-</option>';
            $.each(data, function (i, code) {
                items += "<option value='" + code.GlobalCodeValue + "'>" + code.GlobalCodeName + "</option>";
            });
            $("#ddlEncounterTypes").html(items);

            if (codeId == null || codeId == '') {
                codeId = "0";
                $("#ddlEncounterTypes")[0].selectedIndex = 1; //Commented by Nitin on 12-01-2015, Issue: The default for Encounter Type when admitting a patient should be type 3.
                // $("#ddlEncounterTypes")[0].selectedIndex = 2;
                return;
            }
            else if (codeId > 0) {
                $("#ddlEncounterTypes").val(codeId);
            }
            //else if (codeId == 2) {
            //    $("#ddlEncounterTypes").val(1);
            //} 

        },
        error: function (msg) {

        }
    });
    return false;
}

function BindPhysiciansTypeDropdownData(categoryId, codeId) {
    var jsonData = JSON.stringify({ categoryId: categoryId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetGlobalCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlPhysicianType").empty();
            $("#ddlPhysicianType").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlPhysicianType").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
            });

            if (codeId == null || codeId == '')
                codeId = "0";
            $("#ddlPhysicianType").val(codeId);

        },
        error: function (msg) {

        }
    });
    return false;
}

function BindEncounterStartTypeDropdownData(categoryId, codeId) {
    var jsonData = JSON.stringify({ categoryId: categoryId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetGlobalCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlEncounterStartType").empty();
            $("#ddlEncounterStartType").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlEncounterStartType").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
            });

            if (codeId == null || codeId == '')
                codeId = "0";

            $("#ddlEncounterStartType").val(codeId);
        },
        error: function (msg) {

            Console.log(msg);
        }
    });
    return false;
}

function BindConfidentialLevelDropdownData(categoryId, codeId) {
    var jsonData = JSON.stringify({ categoryId: categoryId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetGlobalCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlConfidentialLevel").empty();
            $("#ddlConfidentialLevel").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlConfidentialLevel").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
            });

            if (codeId == null || codeId == '')
                codeId = "0";
            $("#ddlConfidentialLevel").val(codeId);
        },
        error: function (msg) {

            Console.log(msg);
        }
    });
    return false;
}

function OnChangePhysicianType(physicianTypeSelector, attendingPhysiciantype) {

    BindPhysicianDropdownData($("#ddlPhysicianType").val(), attendingPhysiciantype);
    return false;

}

function OnChangeAccidentRelated(AccidentRelatedSelector, categoryId, codeId) {
    var admitTypeId = $(AccidentRelatedSelector).val();
    if (admitTypeId == 2) {
        var jsonData = JSON.stringify({ categoryId: categoryId });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Home/GetGlobalCodes",
            data: jsonData,
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                $("#ddlAccidentType").empty();
                $("#ddlAccidentType").append('<option value="0">--Select One--</option>');

                /*
                data contains the JSON formatted list of codes 
                passed from the controller
                */
                $.each(data, function (i, code) {
                    $("#ddlAccidentType").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
                });

                if (codeId == null || codeId == '')
                    codeId = "0";
                $("#ddlAccidentType").val(codeId);

            },
            error: function (msg) {

                Console.log(msg);
            }
        });
    }
    else {
        $("#ddlAccidentType").empty();
    }
    return false;
}

function ChooseBed() {
    OpenPatientsBedPopupView();
    //$('#lblInPatientBed').text(' Test Bed');
    //$('#lblInPatientBed').css('font-weight', 'bold');
    //$('#aChooseBed').hide();
    return false;
}

//function BindEncounterSpecialityDropdownData(categoryId, codeId) {
//    var jsonData = JSON.stringify({ categoryId: categoryId });
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        url: "/Home/GetGlobalCodes",
//        data: jsonData,
//        dataType: "json",
//        beforeSend: function () { },
//        success: function (data) {
//            $("#ddlEncounterSpeciality").empty();
//            $("#ddlEncounterSpeciality").append('<option value="0">--Select One--</option>');

//            /*
//            data contains the JSON formatted list of codes 
//            passed from the controller
//            */
//            $.each(data, function (i, code) {
//                $("#ddlEncounterSpeciality").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
//            });

//            if (codeId == null || codeId == '')
//                codeId = "0";
//            $("#ddlEncounterSpeciality").val(codeId);
//        },
//        error: function (msg) {

//            Console.log(msg);
//        }
//    });
//    return false;
//}

function BindEncounterModeOfArrivalDropdownData(categoryId, codeId) {
    var jsonData = JSON.stringify({ categoryId: categoryId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetGlobalCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlEncounterModeOfArrival").empty();
            $("#ddlEncounterModeOfArrival").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlEncounterModeOfArrival").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
            });

            if (codeId == null || codeId == '')
                codeId = "0";
            $("#ddlEncounterModeOfArrival").val(codeId);
        },
        error: function (msg) {

            //Console.log(msg);
        }
    });
    return false;
}

function BindEncounterAdmitTypeDropdownData(categoryId, codeId) {
    var jsonData = JSON.stringify({ categoryId: categoryId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetGlobalCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlEncounterAdmitType").empty();
            $("#ddlEncounterAdmitType").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlEncounterAdmitType").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
            });

            if (codeId == null || codeId == '')
                codeId = "0";
            $("#ddlEncounterAdmitType").val(codeId);
        },
        error: function (msg) {

            Console.log(msg);
        }
    });
    return false;
}

function BindEncounterAccidentRelatedDropdownData(categoryId, codeId) {
    var jsonData = JSON.stringify({ categoryId: categoryId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetGlobalCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlAccidentRelated").empty();
            $("#ddlAccidentRelated").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlAccidentRelated").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
            });

            if (codeId == null || codeId == '')
                codeId = "0";
            $("#ddlAccidentRelated").val(codeId);
        },
        error: function (msg) {

            Console.log(msg);
        }
    });
    return false;
}

function BindEncounterEndTypeDropdownData(categoryId, codeId) {
    var jsonData = JSON.stringify({ categoryId: categoryId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetGlobalCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlEncounterEndType").empty();
            $("#ddlEncounterEndType").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlEncounterEndType").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
            });

            if (codeId == null || codeId == '')
                codeId = "0";
            $("#ddlEncounterEndType").val(codeId);
            $("#ddlEncounterEndType").addClass("validate[required]");
            $("#EncounterEndTime").addClass("validate[required]");
        },
        error: function (msg) {
            Console.log(msg);
        }
    });
    return false;
}

function AddValidation(txtId, dropdownId) {
    $.validationEngine.closePrompt(".formError", true);
    var txtValue = $("#" + txtId).val();
    $("#" + dropdownId).removeClass('validate[required]');

    if (txtValue != '') {
        $("#" + dropdownId).addClass('validate[required]');
    }
    $("#AdmitPatientDiv").validationEngine();
}

function GetFacilityName(facilityNumber) {
    var jsonData = JSON.stringify({ facilityNumber: facilityNumber });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetFacilityNameById",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#lblFacilityName").val(data);
        },
        error: function (msg) {

            Console.log(msg);
        }
    });
    return false;
}

function OnChangePatientType(selector) {
    var patientTypeId = $(selector).val();
    if (patientTypeId != '') {
        //Bind EncounterType    
        //var hdEncounterType = $("#hdEncounterType").val();
        BindEncounterTypesDropdownData(1113, '', patientTypeId);
    }
}

/*
* Owner: Shashank Awasthy
* On: 06092014
* Purpose: Js Events associated with the PatientBed.cshtml to filter the grid view
*/
/// <reference path="../../Views/Encounter/UserControls/_PatientBedAssignment.cshtml" />
function OpenPatientsBedPopupView() {
    //$('#patientBedsDiv').empty();
    //$('#patientBedsDiv').load("/Encounter/SelectBedGrid", function () {
    //    $('#patientBedsDiv').show();
    //});
    $.ajax({
        type: "POST",
        url: '/Encounter/SelectBedGrid',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $('#patientBedsDiv').empty();
            $('#patientBedsDiv').html(data);
            $('#patientBedsDiv').show();
        },
        error: function (msg) {
        }
    });
    return false;
}
/*
* Owner: Shashank Awasthy
* On: 06092014
* Purpose: Js Events associated with the PatientBed.cshtml to filter the grid view
*/
function closeThis() {
    $('#patientBedsDiv').hide();
    return false;
}

//function selectBed() {
//    var bedId = $("#hfGlobalConfirmFirstId").val();
//    var bedName = $("#hfGlobalConfirmedSecondId").val();
//    var obj = $("#hfGlobalConfirmedThridId").val();
//    var isValid = jQuery("#divBedinformation").validationEngine({ returnIsValid: true });
//    if (isValid == true) {
//        var patientBedText = "Bed Selected: " + bedName;
//        $("#hidBedId").val(bedId);
//        var overrideBedservice = $("#" + obj).parent().parent().find("#ddlOverrideBedService").val();
//        $("#hidBedOverideType").val(overrideBedservice);
//        var patientId = $("#hdPatientID").val();
//        $.ajax({
//            type: "POST",
//            contentType: "application/json; charset=utf-8",
//            url: "/Encounter/GetPatientBedInformation",
//            data: JSON.stringify({ patientId: patientId, bedId: bedId, serviceCodeValue: overrideBedservice }),
//            dataType: "json",
//            beforeSend: function () { },
//            success: function (data) {
//                var bedInfo = data.FloorName + " / " + data.Room + " / " + data.BedName;
//                $('#lblBedInfo').text(bedInfo);
//                $('#lblDepartment').text(data.DepartmentName);
//                var serviceCodeInfo = data.patientBedService + " / " + data.BedRateApplicable;
//                $('#lblServiceCodeInfo').text(serviceCodeInfo);
//                $('#lblInPatientBed').text(patientBedText);
//                $('#patientBedsDiv').hide();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

//function selectBed(bedId, bedName, obj) {
//    if (confirm("Do you want to select this Bed?")) {
//        var isValid = jQuery("#divBedinformation").validationEngine({ returnIsValid: true });
//        if (isValid == true) {
//            var patientBedText = "Bed Selected: " + bedName;
//            $("#hidBedId").val(bedId);
//            var overrideBedservice = $("#" + obj).parent().parent().find("#ddlOverrideBedService").val();
//            $("#hidBedOverideType").val(overrideBedservice);
//            var patientId = $("#hdPatientID").val();
//            $.ajax({
//                type: "POST",
//                contentType: "application/json; charset=utf-8",
//                url: "/Encounter/GetPatientBedInformation",
//                data: JSON.stringify({ patientId: patientId, bedId: bedId, serviceCodeValue: overrideBedservice }),
//                dataType: "json",
//                beforeSend: function () { },
//                success: function (data) {
//                    var bedInfo = data.FloorName + " / " + data.Room + " / " + data.BedName;
//                    $('#lblBedInfo').text(bedInfo);
//                    $('#lblDepartment').text(data.DepartmentName);
//                    var serviceCodeInfo = data.patientBedService + " / " + data.BedRateApplicable;
//                    $('#lblServiceCodeInfo').text(serviceCodeInfo);
//                    $('#lblInPatientBed').text(patientBedText);
//                    $('#patientBedsDiv').hide();
//                },
//                error: function (msg) {
//                }
//            });
//        }
//        return false;
//    }
//    return false;
//}

function selectBed() {
    var bedId = $("#hfGlobalConfirmFirstId").val();
    var bedName = $("#hfGlobalConfirmedSecondId").val();
    var obj = $("#hfGlobalConfirmedThridId").val();
    var isValid = jQuery("#divBedinformation").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var patientBedText = "Bed Selected: " + bedName;
        $("#hidBedId").val(bedId);
        var overrideBedservice = $("#" + obj).parent().parent().find("#ddlOverrideBedService").val();
        $("#hidBedOverideType").val(overrideBedservice);
        var patientId = $("#hdPatientID").val();
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Encounter/GetPatientBedInformation",
            data: JSON.stringify({ patientId: patientId, bedId: bedId, serviceCodeValue: overrideBedservice }),
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                var bedInfo = data.FloorName + " / " + data.Room + " / " + data.BedName;
                $('#lblBedInfo').text(bedInfo);
                $('#lblDepartment').text(data.DepartmentName);
                var serviceCodeInfo = data.patientBedService + " / " + data.BedRateApplicable;
                $('#lblServiceCodeInfo').text(serviceCodeInfo);
                $('#lblInPatientBed').text(patientBedText);
                $('#patientBedsDiv').hide();
            },
            error: function (msg) {
            }
        });
    }
    return false;
    return false;
}



function AssignBed() {
    var selected = $('input[type="radio"]:checked');
    if (selected.length != 0) {
        var selectedId = $(selected).attr('id');
        var selectedTr = $('#' + selectedId).closest('tr');
        var roomservice = $(selectedTr).find('select option:selected');
        if (roomservice.text().toLocaleLowerCase() != "--select--") {
            var serviceId = roomservice.val();
            $("#hidBedId").val(selectedId);
            $("#lblInPatientBed").val(selectedId);
            //$(selectedTr).each(function () {
            //    $("#selectedBed").text($(this).find("td#bedNumber").html());
            //    $("#PatientBedSelected").val($(this).find("td#bedNumber").html());
            //    $("#InpatientBed").val($(this).find("td#bedNumber").html());
            //});
            $('#patientBedsDiv').hide();
            return true;
        }
        else {
            alert("Please select any service.");
        }
    }
    return false;
}

function BindPhysicianDropdownData(physicianTypeSelectorval, attendingPhysiciantype) {
    if (physicianTypeSelectorval > 0) {
        var jsonData = JSON.stringify({ physicianTypeId: physicianTypeSelectorval });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Encounter/GetPhysiciansByPhysicianType",
            async: false,
            data: jsonData,
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {

                $("#ddlPhysicians").empty();
                $("#ddlPhysicians").append('<option value="0">--Select One--</option>');

                /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
                $.each(data, function (i, code) {
                    $("#ddlPhysicians").append('<option value="' + code.Id + '">' + code.PhysicianName + '</option>');
                });

                if (attendingPhysiciantype == null || attendingPhysiciantype == '')
                    attendingPhysiciantype = "0";
                $("#ddlPhysicians").val(attendingPhysiciantype);

            },
            error: function (msg) {


            }
        });
    }
}

function CalculateExpectedEndDate() {
    var txtstartDateval = new Date($('#txtStartDate').val());
    var txtExpectedEndDays = $('#txtExpectedEndDays').val();
    var txtExpectedendDate = new Date();

    txtExpectedendDate.setDate(txtstartDateval.getDate() + parseInt(txtExpectedEndDays));
    $('#txtEndDate').val(txtExpectedendDate.getDate() + "/" + (txtExpectedendDate.getMonth() + 1) + "/" + txtExpectedendDate.getFullYear());
}
//function to redirect
function RedirectToAdmit() {
    var pateintID = $('#hdPatientID').val();
    if (pateintID > 0) {
        window.location = window.location.protocol + "//" + window.location.host + "/PatientInfo/PatientInfo?patientId=" + pateintID;
    }
    else
        window.location.reload(true);
}




////----------Not in use right now-------------

function BindServiceCategoryDropdownData(categoryId, codeId) {
    var jsonData = JSON.stringify({ categoryId: categoryId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/GetGlobalCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlServiceCategory").empty();
            $("#ddlServiceCategory").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlServiceCategory").append('<option value="' + code.GlobalCodeValue + '">' + code.GlobalCodeName + '</option>');
            });

            if (codeId == null || codeId == '')
                codeId = "0";
            $("#ddlServiceCategory").val(codeId);
        },
        error: function (msg) {

        }
    });
    return false;
}

////----------Not in use right now-------------

var ShowFutureOpenOrders = function (pid, encId) {
    var jsonData = JSON.stringify({ patientId: pid });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Encounter/GetPatientFutureOrder",
        data: jsonData,
        dataType: "html",
        success: function (data) {
            $('#divFutureOpenOrders').empty().html(data);
            $('.hidePopUp').show();
            $('#hidEncounterID_new').val(encId);
        },
        error: function (msg) {

        }
    });
    return false;
}

var AddSelectedOrders = function () {
    var assignedOrders = [];
    $("#gridContentFutureOpenOrder").find("input:checked").map(function () {
        assignedOrders.push(this.value);
    });
    if (assignedOrders.length > 0) {
        var jsonData = JSON.stringify({ orderIds: assignedOrders, encId: $('#hidEncounterID_new').val() });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Encounter/AddFutureOrders",
            data: jsonData,
            dataType: "json",
            success: function (data) {
                $('#divFutureOpenOrders').empty();
                $('.hidePopUp').hide();
                if (data) {
                    ShowMessageWithDuration("Orders added successfully!", "Success", "success", true, 2000);
                }
                setTimeout(function () {
                    window.location = window.location.protocol + "//" + window.location.host + "/ActiveEncounter/ActiveEncounter";
                }, 1000);
            },
            error: function (msg) {

            }
        });
        return false;
    } else {
        ShowMessageWithDuration('Please select any order', "Success", "success", true, 500);
    }
}



function LicenseTypes() {
    $.getJSON("/Encounter/BindLicenseTypes", { roleName: "Physicians" }, function (data) {
        if (data != null) {
            BindDropdownData(data, "#ddlPhysicianType", "");
        }
    });
}



function BindUsersData() {
    $.getJSON("/Encounter/BindAllEncountersDataOnLoad", {}, function (data) {
        if (data != null) {
            BindUsersDropdownData(data.uList, "#ddlPhysicians", "");
            


        }
    });
}



function BindUsersDropdownData(data, ddlSelector, hdSelector) {
    $(ddlSelector).empty();
    var items = '<option value="0">--Select--</option>';
    $.each(data,
        function (i, obj) {
            var newItem = '<option value="' + obj.Id + '" userName="' + obj.PhysicianName + '"  specility="' + obj.FacultySpeciality + '" lType="' + obj.PhysicianLicenseType + '" >' + obj.PhysicianName + "</option>";
            items += newItem;
        });

    $(ddlSelector).html(items);
    var hdValue = "";
    if (hdSelector.indexOf("#") != -1) {
        hdValue = $(hdSelector).val();
    } else {
        hdValue = hdSelector;
    }
    //
    if (hdValue != null && hdValue != "") {
        $(ddlSelector).val(hdValue);
        if ($(ddlSelector).val() == null || $(ddlSelector).val() == undefined) {
            $(ddlSelector + " option")
                .filter(function (index) { return $(this).text() === "" + hdValue + ""; })
                .attr("selected", "selected");
        }
    } else {
        if ($(ddlSelector).length > 0)
            $(ddlSelector)[0].selectedIndex = 0;
    }
}


function BindAllPhysiciansData() {
    $.getJSON("/Encounter/BindAllEncountersDataOnLoad", {}, function (data) {  
        if (data != null) {
            BindUsersDropdownData(data.uList, "#ddlPhysicians", "#hdEncounterAttendingPhysician");
            BindDropdownData(data.licenceList, "#ddlPhysicianType", "#hdPhysicianType");
            BindDropdownData(data.sPList, "#ddlEncounterSpeciality", "#hdEncounterSpecialty");
           }
    });
}