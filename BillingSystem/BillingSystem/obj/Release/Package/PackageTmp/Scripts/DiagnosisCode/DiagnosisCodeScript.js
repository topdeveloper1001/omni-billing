$(function () {
    JsCalls();
    BindTableSetList("16", "#ddlTableSet", "0");
});
var blockNumber = 2;        //Infinate Scroll starts from second block
var noMoreData = false;
var inProgress = false;

function JsCalls() {
    $("#btnMarkAsInActive").click(function () {
        var stringArray = new Array();
        var checkedItems = $('.check-box:checked');
        for (var i = 0, l = checkedItems.length; i < l; i++) {
            stringArray.push(checkedItems[i].defaultValue);
        }

        if (stringArray.length > 0) {
            var tn = $("#ddlTableSet").length > 0 && $("#ddlTableSet").val() > 0 ? $("#ddlTableSet").val() : "";

            var jsonData = JSON.stringify({
                codeValues: stringArray,
                orderType: "Diagnosis"
            });
            $.ajax({
                type: "POST",
                url: '/Home/MarkInActive',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonData,
                success: function (data) {
                    if (data == "true") {
                        BindDiagnosisCodeGrid(blockNumber);
                        ShowMessage("Selected Codes marked as InActive successfully", "Success", "success", true);
                    }
                },
                error: function (msg) {

                }
            });
        } else {
            ShowMessage("Select at least one billing code!", "warning", "warning", true);
        }
    });

    $("#DiagnosisCodeFormDiv").validationEngine();

    $(".collapseTitle").bind("click", function () {
        $.validationEngine.closePrompt(".formError", true);
    });
    BindCorporates('#ddlCorporate', '6');
    setTimeout(BindFacilitiesDropdownDataWithFacilityNumbers('#ddlFacility', '1002'), 500);
    $("#ddlCorporate").on('change', function () {
        if ($("#ddlCorporate").val() != '0') {
            BindFacilitiesDropdownDataWithFacilityNumbers('#ddlFacility', '');
            ShowHideViewRecords();
        }
    });

    SetGridCheckBoxes();
}

function SaveDiagnosisCode(id) {
    var isValid = jQuery("#DiagnosisCodeFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {

        //var txtDiagnosisTableNumber = $("#txtDiagnosisTableNumber").val().trim();
        var txtDiagnosisTableName = $("#txtDiagnosisTableName").val().trim();
        var txtDiagnosisCode = $("#txtDiagnosisCode").val().trim();
        var txtShortDescription = $("#txtShortDescription").val().trim();
        var txtDiagnosisMediumDescription = $("#txtDiagnosisMediumDescription").val().trim();
        var txtDiagnosisFullDescription = $("#txtDiagnosisFullDescription").val().trim();
        var txtDiagnosisWeight = $("#txtDiagnosisWeight").val().trim();
        var dtDiagnosisEffectiveStartDate = $("#dtDiagnosisEffectiveStartDate").val();
        var dtDiagnosisEffectiveEndDate = $("#dtDiagnosisEffectiveEndDate").val();
        var txtDiagnosisDiseaseGroup = $("#txtDiagnosisDiseaseGroup").val().trim();
        var txtDiagnosisDiseaseCategory = $("#txtDiagnosisDiseaseCategory").val().trim();
        var txtDiagnosisDiseaseChapter = $("#txtDiagnosisDiseaseChapter").val().trim();

        var jsonData = JSON.stringify({
            DiagnosisTableNumberId: id,
            //DiagnosisTableNumber: txtDiagnosisTableNumber,
            DiagnosisTableName: txtDiagnosisTableName,
            DiagnosisCode1: txtDiagnosisCode,
            ShortDescription: txtShortDescription,
            DiagnosisMediumDescription: txtDiagnosisMediumDescription,
            DiagnosisFullDescription: txtDiagnosisFullDescription,
            DiagnosisWeight: txtDiagnosisWeight,
            DiagnosisEffectiveStartDate: dtDiagnosisEffectiveStartDate,
            DiagnosisEffectiveEndDate: dtDiagnosisEffectiveEndDate,
            DiagnosisDiseaseGroup: txtDiagnosisDiseaseGroup,
            DiagnosisDiseaseCategory: txtDiagnosisDiseaseCategory,
            DiagnosisDiseaseChapter: txtDiagnosisDiseaseChapter,
            IsDeleted: false
            //DiagnosisCodeId: id,
            //DiagnosisCodeMainPhone: txtDiagnosisCodeMainPhone,
            //DiagnosisCodeFax: txtDiagnosisCodeFax,
            //DiagnosisCodeLicenseNumberExpire: dtDiagnosisCodeLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - DiagnosisCode


        });
        $.ajax({
            type: "POST",
            url: '/DiagnosisCode/SaveDiagnosisCode',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearAll();
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

function EditDiagnosisCode(id) {
    var jsonData = JSON.stringify({
        id: id
    });
    $.ajax({
        type: "POST",
        url: '/DiagnosisCode/GetDiagnosisCode',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#DiagnosisCodeFormDiv').empty();
                $('#DiagnosisCodeFormDiv').html(data);
                $('#collapseOne').addClass('in');
                JsCalls();
                InitializeDateTimePicker();//initialize the datepicker by ashwani
                $('html,body').animate({ scrollTop: $("#collapseOne").offset().top }, 'fast');
            }
            else {
            }
        },
        error: function (msg) {

        }
    });
}

function DeleteDiagnosisCode() {
    if ($("#hfGlobalConfirmId").val() > 0) {
      var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/DiagnosisCode/DeleteDiagnosisCode',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindDiagnosisCodeGrid(blockNumber);
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
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

//function DeleteDiagnosisCode(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtDiagnosisCodeId = id;
//        var jsonData = JSON.stringify({
//            id: txtDiagnosisCodeId
//        });
//        $.ajax({
//            type: "POST",
//            url: '/DiagnosisCode/DeleteDiagnosisCode',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindDiagnosisCodeGrid(blockNumber);
//                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
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

function BindDiagnosisCodeGrid(bn) {
    var activeInActive = $("#chkShowInActive").is(':checked');
    //if (activeInActive) {
    //    activeInActive = false;
    //} else {
    //    activeInActive = true;
    //}
    var jsonData = JSON.stringify({
        blockNumber: bn,
        showInActive: activeInActive
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/DiagnosisCode/BindDiagnosisCodeListNew",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#DiagnosisCodeListDiv").empty();
            $("#DiagnosisCodeListDiv").html(data);
            SetGridCheckBoxes();
        },
        error: function (msg) {

        }
    });
}

function ClearAll() {
    $("#DiagnosisCodeFormDiv").clearForm();
    $('#collapseOne').removeClass('in');
    $('#collapseTwo').addClass('in');
    $.validationEngine.closePrompt(".formError", true);

    $.ajax({
        type: "POST",
        url: '/DiagnosisCode/ResetDiagnosisCodeForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        //data: jsonData,
        success: function (data) {
            if (data) {

                $('#DiagnosisCodeFormDiv').empty();
                $('#DiagnosisCodeFormDiv').html(data);
                $('#collapseTwo').addClass('in');
                BindDiagnosisCodeGrid(blockNumber);
                InitializeDateTimePicker();//initialize the datepicker by ashwani
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

function ValidateFile() {
    if ($('#ImportExcelfile').val() == '') {
        ShowMessage('Select File first', "Alert", "warning", true);
        return false;
    }
    var isValid = jQuery(".validateUploadExcel").validationEngine({ returnIsValid: true });
    if (isValid == true) {
    }
    else {
        return false;
    }
}

function AppendDataToDiagnosisCodeGrid(data, userId) {
    var html = "";
    if (data.length > 0) {
        $.each(data, function (i, obj) {
            html += "<tr class=\"gridRow\">";
            //if (userId == 1) {
                html += "<td><input type=\"checkbox\" value=\"" + obj.DiagnosisTableNumberId + "\" name=\"assignChkBx\" id=\"assignChkBx\" class=\"check-box\"></td>";
            //}
            html += "<td class=\"col1\">" + obj.DiagnosisTableNumber + "</td>";
            html += "<td class=\"col2\">" + obj.DiagnosisTableName + "</td>";
            html += "<td class=\"col3\">" + obj.DiagnosisCode1 + "</td>";
            html += "<td class=\"col4\">" + obj.ShortDescription + "</td>";
            html += "<td class=\"col5\">" + obj.DiagnosisMediumDescription + "</td>";
            html += "<td class=\"col6\">" + obj.DiagnosisFullDescription + "</td>";
            html += "<td class=\"col11\">";
            html += "<a class=\"unFav\" style=\"float: left; margin-right: 7px; width: 15px;\" onclick=\"EditDiagnosisCode('" + obj.DiagnosisTableNumberId + "') \" href=\"javascript:;\">";
            html += "<img src=\"/images/edit.png\">";
            html += "</a>";
            //html += "<a class=\"unFav\" style=\"float: left; margin-right: 7px; width: 15px;\" onclick=\"return DeleteDiagnosisCode('" + obj.DiagnosisTableNumberId + "'); \" title=\"Remove\" href=\"javascript:;\">";
            html += "<a class=\"unFav\" style=\"float: left; margin-right: 7px; width: 15px;\" onclick=\"return OpenConfirmPopup('" + obj.DiagnosisTableNumberId + "','Delete Diagnosis Code','',DeleteDiagnosisCode,null); \" title=\"Remove\" href=\"javascript:;\">";

            html += "<img src=\"/images/delete.png\">";
            html += "</a>";
            html += "</td>";
            html += "</tr>";
        });
    }
    return html;
}

function SortDiagnosisCodeList(event) {

    var activeInActive = $("#chkShowInActive").is(':checked');
    //if (activeInActive) {
    //    activeInActive = false;
    //} else {
    //    activeInActive = true;
    //}
    var url = "";
    var searchText = $("#SearchCodeOrDesc").val();
    if (searchText != "" && searchText != null) {
        url = "/Home/GetFilteredCodes";
        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
            url += "?text=" + searchText + "&searchType=" + 16 + "&drugStatus=" + 0 + "&" + event.data.msg;
        }
    } else {
        url = "/DiagnosisCode/BindDiagnosisCodeListNew";
        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
            url += "?blockNumber=" + blockNumber + "&showInActive=" + activeInActive + "&" + event.data.msg;
        }
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#DiagnosisCodeListDiv").empty();
            $("#DiagnosisCodeListDiv").html(data);
            SetGridCheckBoxes();
        },
        error: function (msg) {
        }
    });
}

function ShowInActiveDiagnosisCodes(chkSelector) {
    $("#chkActive").prop("checked", false);
    var active = $(chkSelector)[0].checked;
    var isActive = active;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/DiagnosisCode/BindDiagnosisCodeListNew",
        data: JSON.stringify({ blockNumber: blockNumber, showInActive: isActive }),
        dataType: "html",
        success: function (data) {
            if (data != null) {
                $("#DiagnosisCodeListDiv").empty();
                $("#DiagnosisCodeListDiv").html(data);
                SetGridCheckBoxes();
            }
        },
        error: function (msg) {

        }
    });
}