$(function () {
    $("#CarePlanFormDiv").validationEngine();
    BindEncounterTypeDDL();
    $("#associatedDiagnosis").hide();
    $("#selectedDiagnosis").hide();
    $("#chkIsActive").prop('checked', true);
    GetMaxCarePlanNumber();
    //$("#ddlLenghtType")[0].selectedIndex = 0;
    //$("#ddlPlanLength")[0].selectedIndex = 0;
    BindSearchEvent();
    BindPlanLengthType();
    BindPlanLength();
    $('#chkShowInActive').change(function () {
        ClearFormInCarePlan();
        $.validationEngine.closePrompt(".formError", true);
        var showInActive = $("#chkShowInActive").prop("checked");
        /*$.post("/DashboardIndicators/BindIndicatorsActiveInactive", { showInActive: showInActive ? 0 : 1 }, function (data) {
            BindList("#DashboardIndicatorsListDiv", data);
        });*/
        BindCarePlanGrid(showInActive ? 0 : 1);
    });
    (function ($) {
        $.fn.outerHTML = function () {
            return $(this).clone().wrap('<div></div>').parent().html();
        };
    })(jQuery);
});

function BindSearchEvent() {
    $("#searchDiv :CheckBox").change(function () {
        if (this.checked) {
            //BindSelectedDiagnosisLis();
        }
    });
}

function SaveCarePlan(id) {
    var selected = '';
    $('#selectedDiagnosisDiv input:checked').each(function () {
        //selected += '' + $(this)[0].value + ',';
        selected += '' + $(this)[0].value + ','+' ';

    });
    var isValid = jQuery("#CarePlanFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var diagnosisList = selected != '' ? selected.slice(0, -1) : '';
        var finalDiagnosisList = diagnosisList != '' ? diagnosisList.slice(0, -1) : '';
        var txtId = $("#hidCarePlanId").val();
             var txtPlanNumber = $("#txtPlanNumber").val();
             var txtPlanDescription = $("#txtPlanDescription").val();
        //var txtDiagnosisAssociated = $("#txtDiagnosisAssociated").val();
             var txtDiagnosisAssociated = finalDiagnosisList;
             var txtEncounterPatientType = $("#ddlEncounterType").val();
             var txtPlanLength = $("#ddlPlanLength").val();
             var txtPlanLengthType = $("#ddlLenghtType").val();
             var txtIsActive = $('#chkIsActive').is(':checked');
             var txtPlanName = $("#txtPlanName").val();
           
             //var dtCreatedDate = $("#dtCreatedDate").val();
             //var txtModifiedBy = $("#txtModifiedBy").val();
             //var dtModifiedDate = $("#dtModifiedDate").val();
             //var txtFacilityId = $("#txtFacilityId").val();
             //var txtCorporateId = $("#txtCorporateId").val();
             //var txtExtValue1 = $("#txtExtValue1").val();
           
        var jsonData = JSON.stringify({
            Id: txtId,
            PlanNumber: txtPlanNumber,
            PlanDescription: txtPlanDescription,
            DiagnosisAssociated: txtDiagnosisAssociated,
            EncounterPatientType: txtEncounterPatientType,
            PlanLength: txtPlanLength,
            PlanLengthType: txtPlanLengthType,
            IsActive: txtIsActive,
            Name: txtPlanName,
            //ExtValue2: txtExtValue2,
            //CreatedBy: txtCreatedBy,
            //CreatedDate: dtCreatedDate,
            //ModifiedBy: txtModifiedBy,
            //ModifiedDate: dtModifiedDate,
            //FacilityId: txtFacilityId,
            //CorporateId: txtCorporateId,
            //ExtValue1: txtExtValue1,
          
            //CarePlanId: id,
            //CarePlanMainPhone: txtCarePlanMainPhone,
            //CarePlanFax: txtCarePlanFax,
            //CarePlanLicenseNumberExpire: dtCarePlanLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - CarePlan
        });
        $.ajax({
            type: "POST",
            url: '/CarePlan/SaveCarePlan',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data == "-1") {
                    ShowMessage("Plan Number Already Exist..!! It may be in Active/Inactive list", "Warning", "warning", true);
                    return false;
                }
                ClearFormInCarePlan();
                SetGridSorting(SortCarePlanGrid, "#CarePlanListGrid");
                var showInActive = $("#chkShowInActive").prop("checked");
                BindCarePlanGrid(showInActive ? 0 : 1);
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

function EditCarePlan(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/CarePlan/GetCarePlanData',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            //$('#CarePlanFormDiv').empty();
            //$('#CarePlanFormDiv').html(data);
            //$("#CarePlanFormDiv").validationEngine();
            BindCarePlanData(data);
            $('#collapseCarePlanAddEdit').addClass('in').attr('style', 'height: auto;');;

        },
        error: function (msg) {

        }
    });
}

//function DeleteCarePlan(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtCarePlanId = id;
//        var showInActive = $("#chkShowInActive").prop("checked");
//        var jsonData = JSON.stringify({
//            Id: id,
//            inActive: showInActive ? 0 : 1
//        });
//        $.ajax({
//            type: "POST",
//            url: '/CarePlan/DeleteCarePlan',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    if (data == -5) {
//                        ShowMessage("Care plan is already assign to care plan task", "Warning", "warning", true);
//                    } else {
//                        SetGridSorting(SortCarePlanGrid, "#CarePlanListGrid");
//                        BindCarePlanGrid(showInActive ? 0 : 1);
//                        ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//                    }
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

function DeleteCarePlan() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var showInActive = $("#chkShowInActive").prop("checked");
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val(),
            inActive: showInActive ? 0 : 1
        });
        $.ajax({
            type: "POST",
            url: '/CarePlan/DeleteCarePlan',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    if (data == -5) {
                        ShowMessage("Care plan is already assign to care plan task", "Warning", "warning", true);
                    } else {
                        SetGridSorting(SortCarePlanGrid, "#CarePlanListGrid");
                        BindCarePlanGrid(showInActive ? 0 : 1);
                        ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                    }
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

function BindCarePlanGrid(val) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/CarePlan/BindCarePlanList?val=" + val,
        dataType: "html",
        async: true,
        data: null,
        success: function(data) {
            $("#CarePlanListDiv").empty();
            $("#CarePlanListDiv").html(data);

        },
        error: function(msg) {

        }

    });
}

function ClearFormInCarePlan() {
  
    $("#searchDiv").html('');
    $(".dropdown_list").hide();
    $("#CarePlanFormDiv").clearForm();
    $("#selectedDiagnosisDiv").html('');
    GetMaxCarePlanNumber();
    $("#chkIsActive").prop('checked', true);
    $('#collapseCarePlanList').addClass('in');
    $("#CarePlanTaskFormDiv").validationEngine();
    $("#btnSaveCarePlan").val("Save");
    $("#hidCarePlanId").val('');
}

function ClearAll() {
    $("#CarePlanFormDiv").clearForm();
    $('#collapseCarePlanAddEdit').removeClass('in');
    $('#collapseCarePlanList').addClass('in');
    $("#CarePlanFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/CarePlan/ResetCarePlanForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            if (data) {
                $('#CarePlanFormDiv').empty();
                $('#CarePlanFormDiv').html(data);
                $('#collapseCarePlanList').addClass('in');
                var showInActive = $("#chkShowInActive").prop("checked");
                BindCarePlanGrid(showInActive ? 0 : 1);
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




function BindEncounterTypeDDL() {
    BindGlobalCodesWithValue('#ddlEncounterType', 1107, '');
}



function BindCarePlanData(data) {

    $("#selectedDiagnosis").show();
    //$(".dropdown_list").show();
    $("#hidCarePlanId").val(data.Id);
    $("#txtPlanNumber").val(data.PlanNumber);
    $("#ddlEncounterType").val(data.EncounterPatientType);
    BindPlanLengthType();
    $("#ddlLenghtType").val(data.PlanLengthType);
    BindPlanLength();
    $("#ddlPlanLength").val(data.PlanLength);
 
    $("#txtPlanDescription").val(data.PlanDescription);
    $("#txtPlanName").val(data.Name);
    $("#chkIsActive").prop('checked', data.IsActive);
  
    //$("#txtDiagnosisAssociated").val(data.DiagnosisAssociated);
    SearchDiagnosisList(data.DiagnosisAssociated);
    //BindCheckListInCaseOfEdit(data.DiagnosisAssociated);
 
    $("#btnSaveCarePlan").val("Update");
    $('#collapseCarePlanAddEdit').addClass('in').attr('style', 'height: auto;');;

}

function SearchDiagnosis() {
    var txtSearch = $("#txtDiagnosisAssociated").val();
    if (txtSearch.length >= 3) {
        var rhsSelectDiagnosisList = [];
        $('#selectedDiagnosisDiv input:checked').each(function () {
            rhsSelectDiagnosisList.push($(this).attr('value'));
        });
        var jsonData = JSON.stringify({
            text: txtSearch,
            diagCode: rhsSelectDiagnosisList
        });
        $.ajax({
            type: "POST",
            url: "/Diagnosis/GetDiagnosisCodes",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                $("#associatedDiagnosis").show();
                $("#selectedDiagnosis").show();
                var items='';
                $.each(data, function(i, care) {
                    items += '<li id="li3'+i+'"><input name="chkSmartSearch" onchange="BindSelectedDiagnosisList(this);"  id="smartSeach'+i+'" attr-index="'+i+'" type="checkbox" value="' + care.ID + '"><p>' + care.Menu_Title + '</p></li>';
                });
                $("#searchDiv").html(items);

            },
            error: function(msg) {

            }
        });

    } else {
        $("#associatedDiagnosis").hide();
        $("#selectedDiagnosis").hide();
    }
}

//function BindCheckListInCaseOfEdit(diagnosisCode) {
//    var selectedDiagnosis = diagnosisCode;
//    if (selectedDiagnosis != null && selectedDiagnosis != '') {
//        if (selectedDiagnosis.indexOf(',') != -1) {
//            var selectAp = selectedDiagnosis.split(',');
//            $("#searchDiv").find("input[type=checkbox]").each(function () {
//                if (selectAp.indexOf($(this).val()) != -1) {
                 
//                    $(this).prop("checked", "checked");
//                }
//            });
//        } else {
//            $("#searchDiv").find("input[type=checkbox]").each(function () {
//                if (selectedDiagnosis.indexOf($(this).val()) != -1) {
//                    $(this).prop("checked", "checked");
//                }
//            });
//        }
//    }
//}


function SearchDiagnosisList(diagnosis) {
    
    /// <summary>
    /// Searches the diagnosis list.
    /// </summary>
    /// <param name="diagnosis">The diagnosis.</param>
    /// <returns></returns>
    var diagnosislist = diagnosis;
    if (diagnosislist != null) {
        var items = '';
       var arr = diagnosis.split(' ').join('').split(',');
       var jsonData = JSON.stringify({
            diagCode: arr
        });
        $.ajax({
            type: "POST",
            url: "/Diagnosis/GetDiagnosisCodesData",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                $.each(data, function (a, care) {
                    items += '<li><input name="chkSmartSearch" onchange="UnBindSelectedDiagnosisList();" id="smartSeach" type="checkbox" value="' + care.ID + '"><p>' + care.Menu_Title + '</p></li>';
                });
                BindSearchEvent();

            },
            error: function (msg) {

            }
        });
        //$(".dropdown_list").show();
        $("#selectedDiagnosisDiv").html(items);

        //BindCheckListInCaseOfEdit(diagnosis);
        $("#selectedDiagnosisDiv").find("input[type=checkbox]").each(function () {
            $(this).prop("checked", "checked");
        });

    } else {
        $("#selectedDiagnosis").hide();
    }
}


function BindPlanLength() {
    var i = 0;
    var items = '<option value="0">--Select--</option>';
    var lengthtype = $("#ddlLenghtType").val();
    if (lengthtype > 0) {
        if (lengthtype == "1") {
            for (i = 1; i <= 31; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
        }
        if (lengthtype == "2") {
            for (i = 1; i <= 24; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
        }


        $("#ddlPlanLength").html(items);
    } else {
        BindDropDownOnlyWithSelect("#ddlPlanLength");
      
    }
}





function BindPlanLengthType() {
    var encounterType = $("#ddlEncounterType").val();
    if (encounterType > 0) {
        BindGlobalCodesWithValue('#ddlLenghtType', 4908, '');
    } else {
        BindDropDownOnlyWithSelect("#ddlLenghtType");
        BindDropDownOnlyWithSelect("#ddlPlanLength");
    }
    
}





function GetMaxCarePlanNumber() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/CarePlan/GetTaskNumber",
        dataType: "Json",
        async: true,
        data: null,
        success: function (data) {
            $("#txtPlanNumber").val(data);
        },
        error: function (msg) {

        }

    });

}

var UnBindSelectedDiagnosisList = function(e) {
    var htmlDiagnosis = '';
    $('#selectedDiagnosisDiv :CheckBox').each(function () {
        if (!$(this).prop('checked')) {
            var parentId = $(this).parent().attr('id');
            var oy = $(this).parent();
            var outerhrmlobj = oy[0].outerHTML;
            htmlDiagnosis += outerhrmlobj;
            $(this).parent().html('');
            htmlDiagnosis = htmlDiagnosis.replace('UnBindSelectedDiagnosisList(this);', 'BindSelectedDiagnosisList(this);');
            $('#searchDiv').find('#' + parentId).append(htmlDiagnosis);
        }
    });
}

function BindSelectedDiagnosisList(e) {
    var htmlDiagnosis = '';
    $('#searchDiv :CheckBox:checked').each(function () {
        var oy = $(this).parent();
        var outerhrmlobj = oy[0].outerHTML;
        htmlDiagnosis += outerhrmlobj;
        $(this).parent().html('');
    });
    htmlDiagnosis = htmlDiagnosis.replace('BindSelectedDiagnosisList(this);', 'UnBindSelectedDiagnosisList(this);');
    $('#selectedDiagnosisDiv').append(htmlDiagnosis);
    $("#selectedDiagnosisDiv").find("input[type=checkbox]").each(function () {
        $(this).prop("checked", "checked");
      });

    //var htmlDiagnosis = '';
    //var id = '';
    //$('#searchDiv :CheckBox:checked').each(function (i, row) {
    //    htmlDiagnosis += $(this).parent().html();
    //    $(this).prop("checked", "checked");
    //    id += this.value;
    //    //$('#searchDiv').find('input[type=checkbox]:checked').removeAttr('checked');
    //});

    //
    //var checkedIndex = $(e).attr("attr-index");
    //$("#li3" + checkedIndex).hide();

    ////$('#selectedDiagnosisDiv').append(htmlDiagnosis);
    //CheckedSelectedList(e);
}

function SortCarePlanGrid(event) {
    var showInActive = $("#chkShowInActive").prop("checked") ? 0 : 1;
    var url = "/CarePlan/BindCarePlanList";
    //var patientId = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?val=" + showInActive + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#CarePlanListDiv").empty();
            $("#CarePlanListDiv").html(data);

        },
        error: function (msg) {
        }
    });
}
//function CheckedSelectedList(diagnosisCode) {
//    var selectedDiagnosis1 = diagnosisCode;
//    if (selectedDiagnosis1 != null && selectedDiagnosis1 != '') {
         
//        $("#selectedDiagnosisDiv").find("input[type=checkbox]").each(function () {
//                if (selectedDiagnosis1.indexOf($(this).val()) != -1) {
//                    $(this).prop("checked", "checked");
//                }
//            });
        
//    }
//}



//function CheckedSelectedList(diagnosisCode) {
//    var selectedDiagnosis = diagnosisCode;
//    if (selectedDiagnosis != null && selectedDiagnosis != '') {
//        if (selectedDiagnosis.indexOf(',') != -1) {
//            var selectAp = selectedDiagnosis.split(',');
//            $("#selectedDiagnosisDiv").find("input[type=checkbox]").each(function () {
//                if (selectAp.indexOf($(this).val()) != -1) {

//                    $(this).prop("checked", "checked");
//                }
//            });
//        } else {
//            $("#selectedDiagnosisDiv").find("input[type=checkbox]").each(function () {
//                if (selectedDiagnosis.indexOf($(this).val()) != -1) {
//                    $(this).prop("checked", "checked");
//                }
//            });
//        }
//    }
//}


//function BindCheckListInCaseOfEdit(diagnosisCode) {
//    var selectedDiagnosis = diagnosisCode;
//    if (selectedDiagnosis != null && selectedDiagnosis != '') {
//        if (selectedDiagnosis.indexOf(',') != -1) {
//            var selectAp = selectedDiagnosis.split(',');
//            $("#selectedDiagnosisDiv").find("input[type=checkbox]").each(function () {
//                if (selectAp.indexOf($(this).val()) != -1) {

//                    $(this).prop("checked", "checked");
//                }
//            });
//        } else {
//            $("#selectedDiagnosisDiv").find("input[type=checkbox]").each(function () {
//                if (selectedDiagnosis.indexOf($(this).val()) != -1) {
//                    $(this).prop("checked", "checked");
//                }
//            });
//        }
//    }
//}


