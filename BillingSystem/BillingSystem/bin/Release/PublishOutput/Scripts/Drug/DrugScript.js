$(function () {
    JsCalls();

    $("#btnMarkAsInActive").click(function () {
        var stringArray = new Array();
        var checkedItems = $('.check-box:checked');
        for (var i = 0, l = checkedItems.length; i < l; i++) {
            stringArray.push(checkedItems[i].defaultValue);
        }
        if (stringArray.length > 0) {
            var jsonData = JSON.stringify({
                codeValues: stringArray,
                orderType: "DRUG"
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
                        BindDrugGrid(blockNumber);
                        ShowMessage("Selected Codes marked as InActive successfully", "Success", "success", true);
                    }
                },
                error: function (msg) {

                }
            });
        }
        else {
            ShowMessage("Select at least one billing code!", "warning", "warning", true);
        }
    });

    BindTableSetList("5", "#ddlTableSet", "0");

});
var blockNumber = 2;        //Infinate Scroll starts from second block
var noMoreData = false;
var inProgress = false;

function JsCalls() {
    $("#DrugFormDiv").validationEngine();
    $(".collapseTitle").bind("click", function () {
        $.validationEngine.closePrompt(".formError", true);
    });
    //InitializeDateTimePicker();
    BindDrugGrid(blockNumber);
    InitializeDatesInDrug();
    BindCorporates('#ddlCorporate', '6');
    setTimeout(BindFacilitiesDropdownDataWithFacilityNumbers('#ddlFacility', '1002'), 500);
    $("#ddlCorporate").on('change', function () {
        if ($("#ddlCorporate").val() != '0') {
            BindFacilitiesDropdownDataWithFacilityNumbers('#ddlFacility', '');
        }
    });

    SetGridCheckBoxes();
}

function SaveDrug(id) {
    var isValid = jQuery("#DrugFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var drugStatus = SetDrugStatus();
        //var txtDrugTableNumber = $("#txtDrugTableNumber").val();
        //var txtDrugDescription = $("#txtDrugDescription").val();
        var txtDrugCode = $("#txtDrugCode").val();
        var txtDrugInsurancePlan = $("#txtDrugInsurancePlan").val();
        var txtDrugPackageName = $("#txtDrugPackageName").val();
        var txtDrugGenericName = $("#txtDrugGenericName").val();
        var txtDrugStrength = $("#txtDrugStrength").val();
        var txtDrugDosage = $("#txtDrugDosage").val();
        var txtDrugPackageSize = $("#txtDrugPackageSize").val();
        var txtDrugPricePublic = $("#txtDrugPricePublic").val();
        var txtDrugPricePharmacy = $("#txtDrugPricePharmacy").val();
        var txtDrugUnitPricePublic = $("#txtDrugUnitPricePublic").val();
        var txtDrugUnitPricePharmacy = $("#txtDrugUnitPricePharmacy").val();
        //var txtDrugStatus = $("#txtDrugStatus").val();
        var dtDrugDeleteDate = $("#dtDrugDeleteDate").val();
        var dtDrugLastChange = $("#dtDrugLastChange").val();
        var txtDrugAgentName = $("#txtDrugAgentName").val();
        var txtDrugManufacturer = $("#txtDrugManufacturer").val();
        var txtDrugStrengthHardcode = $("#txtDrugStrengthHardcode").val();
        var txtDrugStrengthHardcodeUOM = $("#txtDrugStrengthHardcodeUOM").val();
        var txtDrugPackagesizeHardcode = $("#txtDrugPackagesizeHardcode").val();
        var txtDrugPackagesizeHardcodeUOM = $("#txtDrugPackagesizeHardcodeUOM").val();
        var txtBrandCode = $("#txtBrandCode").val();
        var txtATCCode = $("#txtATCCode").val();
        var chkIsInStock = $("#InStock").val();

        var jsonData = JSON.stringify({
            Id: $("#hdDrugId").val(),
            //DrugTableNumber: txtDrugTableNumber,
            //DrugDescription: txtDrugDescription,
            DrugCode: txtDrugCode,
            DrugInsurancePlan: txtDrugInsurancePlan,
            DrugPackageName: txtDrugPackageName,
            DrugGenericName: txtDrugGenericName,
            DrugStrength: txtDrugStrength,
            DrugDosage: txtDrugDosage,
            DrugPackageSize: txtDrugPackageSize,
            DrugPricePublic: txtDrugPricePublic,
            DrugPricePharmacy: txtDrugPricePharmacy,
            DrugUnitPricePublic: txtDrugUnitPricePublic,
            DrugUnitPricePharmacy: txtDrugUnitPricePharmacy,
            DrugStatus: drugStatus,
            DrugDeleteDate: dtDrugDeleteDate,
            DrugLastChange: dtDrugLastChange,
            DrugAgentName: txtDrugAgentName,
            DrugManufacturer: txtDrugManufacturer,
            DrugStrengthHardcode: txtDrugStrengthHardcode,
            DrugStrengthHardcodeUOM: txtDrugStrengthHardcodeUOM,
            DrugPackagesizeHardcode: txtDrugPackagesizeHardcode,
            DrugPackagesizeHardcodeUOM: txtDrugPackagesizeHardcodeUOM,
            BrandCode: txtBrandCode,
            ATCCode: txtATCCode,
            InStock: chkIsInStock
        });
        $.ajax({
            type: "POST",
            url: '/Drug/SaveDrug',
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

function EditDrug(drugId) {
    var drugStatus = SetDrugStatus();
    var jsonData = JSON.stringify({
        id: drugId
    });

    $.ajax({
        type: "POST",
        url: '/Drug/GetDrug',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#DrugFormDiv').empty();
                $('#DrugFormDiv').html(data);
                $('#collapseOne').addClass('in');
                JsCalls();
            }
            else {
            }
        },
        error: function (msg) {

        }
    });
}

function DeleteDrug() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val(),
            DrugStatus: "Deleted"
        });
        $.ajax({
            type: "POST",
            url: '/Drug/SaveDrug',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindDrugGrid(blockNumber);
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

//function DeleteDrug(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var jsonData = JSON.stringify({
//            Id: id,
//            DrugStatus: "Deleted"
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Drug/SaveDrug',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindDrugGrid(blockNumber);
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

function BindDrugGrid(bn) {
    
    var viewVal = $('#ddlDrugGridView').val() == "0" ? "All" : $('#ddlDrugGridView').val();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Drug/BindDrugListNew?blockNumber=" + bn + "&viewVal=" + viewVal,
        dataType: "html",
        async: true,
        // data: jsonData,
        success: function (data) {
            $("#DrugListDiv").empty();
            $("#DrugListDiv").html(data);

            SetGridCheckBoxes();
        },
        error: function (msg) {

        }

    });
}

function ClearAll() {
    $("#DrugFormDiv").clearForm(true);
    $('#collapseOne').removeClass('in');
    $('#collapseTwo').addClass('in');
    $("#dtDrugDeleteDate").val($.datepicker.formatDate("mm/dd/yy", new Date()));
    $.validationEngine.closePrompt(".formError", true);

    $.ajax({
        type: "POST",
        url: '/Drug/ResetDrugForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            if (data) {
                $('#DrugFormDiv').empty();
                $('#DrugFormDiv').html(data);
                $('#collapseTwo').addClass('in');
                BindDrugGrid(blockNumber);
                InitializeDatesInDrug();
                SetGridCheckBoxes();
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

function SetDrugStatus() {
    var checkStatus = $("#chkDrugStatus")[0].checked;
    if (checkStatus == true)
        return "Active";
    return "Deleted";
}

function BindDrugGridCustom() {
    blockNumber = 2;
    var viewVal = $('#ddlDrugGridView').val() == "0" ? "All" : $('#ddlDrugGridView').val();
    if (viewVal != '0') {
        var jsonData = JSON.stringify({
            ViewVal: viewVal
        });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Drug/BindDrugListNew?blockNumber=" + blockNumber + "&viewVal=" + viewVal,
            dataType: "html",
            async: true,
            //data: jsonData,
            success: function (data) {
                $("#DrugListDiv").empty();
                $("#DrugListDiv").html(data);
                //SetGridPaging('BindDrugList', 'DrugMain');
                //$('.table_scroll').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
            },
            error: function (msg) {

            }

        });
    } else {
        BindDrugGrid(blockNumber);
    }
}

function AppendDataToDrugGrid(data, userId) {
    var html = "";
    if (data.length > 0) {
        $.each(data, function (i, obj) {
            var parsedDate = new Date(parseInt(obj.DrugLastChange.substr(6)));            var jsDate = new Date(parsedDate);
            var date = jsDate.getDate() + "/" + (jsDate.getMonth() + 1) + "/" + jsDate.getFullYear();
            html += "<tr class=\"gridRow\">";
            //if (userId == 1) {
            html += "<td><input type=\"checkbox\" value=\"" + obj.Id + "\" name=\"assignChkBx\" id=\"assignChkBx\" class=\"check-box\"></td>";
            //}
            html += "<td class=\"col1\">" + obj.DrugTableNumber + "</td>";
            html += "<td class=\"col1\">" + obj.DrugCode + "</td>";
            html += "<td class=\"col2\">" + obj.DrugGenericName + "</td>";
            html += "<td class=\"col3\">" + obj.DrugPackageName + "</td>";
            html += "<td class=\"col4\">" + obj.DrugStrength + "</td>";
            html += "<td class=\"col5\">" + obj.DrugDosage + "</td>";
            html += "<td class=\"col6\">" + date + "</td>";
            html += "<td class=\"col6\">" + obj.DrugPackageSize + "</td>";
            html += "<td class=\"col6\">" + obj.DrugPricePublic + "</td>";
            html += "<td class=\"col6\">" + obj.DrugUnitPricePublic + "</td>";
            html += "<td class=\"col11\">";
            html += "<a class=\"unFav\" style=\"float: left; margin-right: 7px; width: 15px;\" onclick=\"EditDrug('" + obj.Id + "') \" href=\"javascript:;\">";
            html += "<img src=\"/images/edit.png\">";
            html += "</a>";
            //html += "<a class=\"unFav\" style=\"float: left; margin-right: 7px; width: 15px;\" onclick=\"return DeleteDrug('" + obj.Id + "'); \" title=\"Remove\" href=\"javascript:;\">";
            html += "<a class=\"unFav\" style=\"float: left; margin-right: 7px; width: 15px;\" onclick=\"return OpenConfirmPopup('" + obj.Id + "','Delete Drug','',DeleteDrug,null); \" title=\"Remove\" href=\"javascript:;\">";

            html += "<img src=\"/images/delete.png\">";
            html += "</a>";
            html += "<a class=\"unFav\" style=\"float: left; margin-right: 7px; width: 15px; display:none;\" onclick=\"return MarkAsFav('" + obj.Id + "'); \" title=\"Add As Favorite\" href=\"javascript:;\">";
            html += "<img src=\"/images/Fav (1).png\">";
            html += "</a>";
            html += "</td>";
            html += "</tr>";
        });
    }
    return html;
}

function InitializeDatesInDrug() {
    if ($("#dtDrugDeleteDate").length > 0) {
        $("#dtDrugDeleteDate").datetimepicker({
            format: 'm/d/Y',
            minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
            timepicker: false,
            closeOnDateSelect: true
        });
    }


    if ($("#dtDrugLastChange").length > 0) {
        $("#dtDrugLastChange").datetimepicker({
            format: 'm/d/Y',
            minDate: '1901/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
            timepicker: false,
            closeOnDateSelect: true
        });
    }

}

function SortDrugCodeGrid(event) {
    var viewVal = $('#ddlDrugGridView').val() == "0" ? "All" : $('#ddlDrugGridView').val();
    var url = "";
    var searchText = $("#SearchCodeOrDesc").val();
    if (searchText != "" && searchText != null) {
        url = "/Home/GetFilteredCodes";
        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
            url += "?text=" + searchText + "&searchType=" + 5 + "&drugStatus=" + viewVal + "&" + event.data.msg;
        }
    } else {
        url = "/Drug/BindDrugListNew";
        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
            url += "?blockNumber=" + blockNumber + "&viewVal=" + viewVal + "&" + event.data.msg;
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
            $("#DrugListDiv").empty();
            $("#DrugListDiv").html(data);
            SetGridCheckBoxes();

        },
        error: function (msg) {
        }
    });
}