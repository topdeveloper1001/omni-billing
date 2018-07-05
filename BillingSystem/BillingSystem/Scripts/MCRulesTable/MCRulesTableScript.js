$(function() {
    $("#MCRulesTableFormDiv").validationEngine();
    GetRuleStepDropdownData();
    
});

function SaveMCRulesTable(id) {
    var isValid = jQuery("#MCRulesTableFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtRuleSetNumber = $("#hdRuleSetNumber").val();
        if (txtRuleSetNumber != null && txtRuleSetNumber != '') {
            var txtRuleNumber = $("#txtRuleNumber").val();
            var txtAppliestoPatientType = $("#ddlAppliestoPatientType").val();
            var txtConStart = $("#ddlConStart").val();
            var txtCalculation = $("#ddlCalculationType").val();
            var txtConNextLine = $("#ddlConNextLine").val();
            
            var txtDirectValue = $("#txtDirectValue").val();
            var txtCalculationMethod = $("#ddlCalculationMethod").val();
            var txtCalcualtionRate = $("#txtCalcualtionRate").val();
            var txtIsActive = $("#chkIsActive").prop('checked');

            var txtLHSTableName = $("#ddlLHSTableName :selected").text();
            var txtLHSTableColumn = $("#ddlLHSTableColumn :selected").text();
            var txtLHSTableKeyColumn = $("#ddlLHSTableKeyColumn").val();

            var txtRHSTableName = $("#ddlRHSTableName :selected").text();
            var txtRHSTableColumn = $("#ddlRHSTableColumn :selected").text();
            var txtRHSTableKeyColumn = $("#ddlRHSTableKeyColumn").val();

            var rdRHSFromTableSelected = $("#rdRhsFromTable")[0].checked;
            var rdRhsFromDirectValueSelected = $("#rdRhsFromDirectValue")[0].checked;

            if (rdRHSFromTableSelected) {
                txtDirectValue = "";
            } else if (rdRhsFromDirectValueSelected) {
                txtRHSTableName = "";
                txtRHSTableColumn = "";
                txtRHSTableKeyColumn = "";
            }

            var jsonData = JSON.stringify({
                ManagedCareRuleId: id,
                RuleSetNumber: txtRuleSetNumber,
                RuleNumber: txtRuleNumber,
                AppliestoPatientType: txtAppliestoPatientType,
                ConStart: txtConStart,
                Calculation: txtCalculation,
                ConNextLine: txtConNextLine,
                RHSTableName: txtRHSTableName,
                RHSTableColumn: txtRHSTableColumn,
                RHSTableKeyColumn: txtRHSTableKeyColumn,
                LHSTableName: txtLHSTableName,
                LHSTableColumn: txtLHSTableColumn,
                LHSTableKeyColumn: txtLHSTableKeyColumn,
                DirectValue: txtDirectValue,
                CalculationMethod: txtCalculationMethod,
                CalcualtionRate: txtCalcualtionRate,
                IsActive: true
            });
            $.ajax({
                type: "POST",
                url: '/MCRulesTable/SaveMCRulesTable',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                data: jsonData,
                success: function(data) {
                    //BindList("#RuleStepListDiv", data);
                    GetStepList();
                    ClearMCRulesTableForm();
                    var msg = "Records Saved successfully !";
                    if (id > 0)
                        msg = "Records updated successfully";
                    ShowMessage(msg, "Success", "success", true);
                },
                error: function(msg) {

                }
            });
        } else {
            ShowMessage('Please select the Managed care from the list to add the rule step.', "Info", "warning", true);
        }
    }
}

function EditMCRulesTable(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/MCRulesTable/GetMCRulesTableDetails',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function(data) {
            BindMCRulesTableDetails(data);
        },
        error: function(msg) {

        }
    });
}

function DeleteMCRulesTable() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/MCRulesTable/DeleteMCRulesTable',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                //BindList("#MCRulesTableListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                GetStepList();
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

//function DeleteMCRulesTable(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var jsonData = JSON.stringify({
//            id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/MCRulesTable/DeleteMCRulesTable',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: jsonData,
//            success: function(data) {
//                //BindList("#MCRulesTableListDiv", data);
//                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//                GetStepList();
//            },
//            error: function(msg) {
//                return true;
//            }
//        });
//    }
//}

function ClearMCRulesTableForm() {
    var ruleSetNumber = $("#hdRuleSetNumber").val();
    $("#MCRulesTableFormDiv").clearForm(true);
    $('#collapseMCRulesTableAddEdit').removeClass('in');
    $('#collapseMCRulesTableList').addClass('in');
    $("#MCRulesTableFormDiv").validationEngine();
    $("#btnSave1").val("Save");
    $("#btnSave1").attr("onclick", "return SaveMCRulesTable('0');");
    $.validationEngine.closePrompt(".formError", true);
    $("#chkIsActive").prop('checked', true);
    $("#hdRuleSetNumber").val(ruleSetNumber);
    GetMaxStepNumber();
    ColorCodeRuleSteps();
}

function BindMCRulesTableDetails(data) {
    $("#btnSave1").val("Update");
    $('#collapseRuleStepList').removeClass('in');
    $('#collapseRuleStepAddEdit').addClass('in');
    $("#MCRulesTableFormDiv").validationEngine();
    $("#btnSave1").attr("onclick", "return SaveMCRulesTable('" + data.ManagedCareRuleId + "');");

    
    //ShowhideCalculationDiv();
    $("#hdRuleSetNumber").val(data.RuleSetNumber);
    $("#ddlAppliestoPatientType").val(data.AppliestoPatientType);
    $("#ddlConStart").val(data.ConStart);

    $('#hdLHSTableColumn').val(data.LHSTableColumn);
    $('#hdLHSTableKeyColumn').val(data.LHSTableKeyColumn);

    $('#ddlLHSTableKeyColumn').empty();
    $('#ddlLHSTableKeyColumn').append($('<option>', {
        value: data.LHSTableKeyColumn,
        text: data.LHSTableKeyColumn
    }));

    $('#hdRHSTableColumn').val(data.RHSTableColumn);
    $('#hdRHSTableKeyColumn').val(data.RHSTableKeyColumn);
    $("#ddlRHSTableKeyColumn").empty();
    $('#ddlRHSTableKeyColumn').append($('<option>', {
        value: data.RHSTableKeyColumn,
        text: data.RHSTableKeyColumn
    }));

    $("#ddlLHSTableName option").filter(function (index) { return $(this).text() === "" + data.LHSTableName + ""; }).attr('selected', 'selected');
    $("#ddlRHSTableName option").filter(function (index) { return $(this).text() === "" + data.RHSTableName + ""; }).attr('selected', 'selected');

    OnChangeTableDropdown('#ddlLHSTableName', 1);

    //setTimeout(function () {
    //    $("#ddlRHSTableColumn option").filter(function (index) { return $(this).text() === "" + data.RHSTableColumn + ""; }).attr('selected', 'selected');
    //    $("#ddlLHSTableColumn option").filter(function (index) { return $(this).text() === "" + data.LHSTableColumn + ""; }).attr('selected', 'selected');
    //    $("#ddlLHSTableKeyColumn").val(data.LHSTableKeyColumn);
    //}, 1000);

    if (data.DirectValue == null || data.DirectValue == "") {
        OnChangeTableDropdown('#ddlRHSTableName', 2);
        ToggleRadioButtons('#rdRhsFromTable', '.rdRhsFrom');
        ShowHideRHSDivs('#rhsFromTable');
    } else {
        ToggleRadioButtons('#rdRhsFromDirectValue', '.rdRhsFrom');
        ShowHideRHSDivs('#rhsFromDirectValue');
    }
    $("#ddlCalculationType").val(data.Calculation);
    $("#txtDirectValue").val(data.DirectValue);
    $("#ddlConNextLine").val(data.ConNextLine);
}

var GetMaxStepNumber = function() {
    var hdRuleSetNumber = $("#hdRuleSetNumber").val();
    var jsonData = JSON.stringify({
        RuleSetNumber: hdRuleSetNumber
    });
    $.ajax({
        type: "POST",
        url: '/MCRulesTable/MaxRuleStepNumber',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function(data) {
            $('#txtRuleNumber').val(data);
        },
        error: function(msg) {

        }
    });
};

var ColorCodeRuleSteps = function() {
    $("#McContractRuleStepGrid tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        if ($actualRow.find('.col3').html().indexOf("IF") != -1) {
            $actualRow.addClass('rowColor3');
        } else if ($actualRow.find('.col3').html().indexOf('THEN') != -1) {
            $actualRow.addClass('rowColor5');
        } else {
            $actualRow.removeClass('rowColor3');
            $actualRow.removeClass('rowColor5');
        }
    });
};

function GetRuleStepDropdownData() {
    BindGlobalCodesWithValue("#ddlLHSTableName", 1016, "#hdLHSTableName");
    BindGlobalCodesWithValue("#ddlRHSTableName", 1016, "#hdRHSTableName");
    BindGlobalCodesWithValue("#ddlCalculationType", 1015, "#hdCalculation");
    BindGlobalCodesWithValue("#ddlConNextLine", 1015, "#hdConNextLine");
    BindGlobalCodesWithValue("#ddlAppliestoPatientType", 1107, "#hdAppliestoPatientType");
    $('#ddlConStart').append($('<option>', {
        value: '1',
        text: 'ELSE'
    }));

    //$.ajax({
    //    type: "POST",
    //    url: '/RuleStep/GetRuleStepDropdownData',
    //    async: false,
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    data: JSON.stringify({ startRange: 1015, endRange: 1020 }),
    //    success: function(data) {
    //        if (data != null) {
    //            //BindDropdownData(data.DataTypes, "#ddlDataTypes", "#hdDataType");
    //            //BindDropdownData(data.CompareTypes, "#ddlCompareType", "#hdFieldName");

    //            BindDropdownData(data.TablesList, "#ddlLHSTableName", "#hdLHSTableName");
    //            BindDropdownData(data.TablesList, "#ddlRHSTableName", "#hdRHSTableName");
                
    //        }
    //    },
    //    error: function(msg) {

    //    }
    //});
}

function OnChangeTableDropdown(ddlSelector, dropdownType) {
    var value = $(ddlSelector).val();
    if (value != null && value != '') {
        $.ajax({
            type: "POST",
            url: '/GlobalCode/GetColumnForManagedCareTable',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ tableid: value }),
            success: function (data) {
                if (dropdownType == '1') {
                    BindDropdownData(data.List, "#ddlLHSTableColumn", "#hdLHSTableColumn");
                    $('#ddlLHSTableKeyColumn').empty();
                    $('#ddlLHSTableKeyColumn').append($('<option>', {
                        value: data.KeyColumn,
                        text: data.KeyColumn
                    }));
                } else {
                    BindDropdownData(data.List, "#ddlRHSTableColumn", "#hdRHSTableColumn");
                    $('#ddlRHSTableKeyColumn').empty();
                    $('#ddlRHSTableKeyColumn').append($('<option>', {
                        value: data.KeyColumn,
                        text: data.KeyColumn
                    }));
                }
            },
            error: function(msg) {

            }
        });
    }
}

var GetFieldDataType = function() {
    var selectedColumnvalue = $('#ddlTableColumn :selected').text();
    var selectedtablevalue = $('#ddlTableName :selected').text();
    if (selectedColumnvalue != '' || selectedColumnvalue != '0') {
        var jsonData = JSON.stringify({
            tableName: selectedtablevalue,
            columnName: selectedColumnvalue
        });
        $.ajax({
            type: "POST",
            url: '/Home/GetColumnDataType',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function(data) {
                //  $('#ddlDataTypes option:contains(' + data + ')').attr("selected", "selected");
            },
            error: function(msg) {

            }
        });
    }
};

var GetStepList = function () {
    var hdRuleSetNumber = $("#hdRuleSetNumber").val();
    var jsonData = JSON.stringify({
        McContractID: hdRuleSetNumber
    });
    $.ajax({
        type: "POST",
        url: '/MCRulesTable/BindRuleStepListObj',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            BindList("#RuleStepListDiv", data);
            ColorCodeRuleSteps();
        },
        error: function (msg) {

        }
    });
};

var ShowhideCalculationDiv = function() {
    var conditionvalue = $('#ddlConStart').val();
    if (conditionvalue == '1') {
        $('#DirectValueDiv').show();
        $('#calculationDiv').hide();
    } else {
        $('#DirectValueDiv').hide();
        $('#calculationDiv').show();
    }
};

function ShowHideRHSDivs(divSelector) {
    $(".rhsFromDiv").hide();
    $(divSelector).show();
    $('#txtDirectValue').val('');
}

