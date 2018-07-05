$(LoadMcContractData);

function LoadMcContractData() {
    $("#McContractDiv").validationEngine();
    BindCountryData("#ddlCountries", "#hdCountry");
    BindGlobalCodesWithValue("#ddlMcEncounterType", 1107, "#hdMCEncounterType");
    BindGlobalCodesWithValue("#ddlMcLevel", 950, "#hdMCLevel");
   // BindGlobalCodesWithValue("#ddlMCCodes", 951, "#hdMCCode");
    //BindOrderTypeCategories("#ddlMCOrderType", "#hdMCOrderType");
    BindCategories('#ddlOrderTypeCategory', "#hdMCOrderType");
    SetSelectedRowColor();
}

function SaveMcContract() {
    var isValid = jQuery("#McContractDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var id = $("#hdMCContractID").val();
        var ddlMcEncounterType = $("#ddlMcEncounterType").val();
        var ddlMcOrderType = $("#ddlMCOrderType").val();
        var txtMcOrderCode = $("#txtMCOrderCode").val();
        var txtMcPatientFixed = $("#txtMCPatientFixed").val();
        var txtMcPatientPercent = $("#txtMCPatientPercent").val();
        var txtMcPatientCapping = $("#txtMCPatientCapping").val();
        //var txtMcMultiplier = $("#txtMCMultiplier").val();
        //var txtMcWaitingDays = $("#txtMCWaitingDays").val();
       // var txtMcExpireAfterDays = $("#txtMCExpireAfterDays").val();
        //var ddlMcCodes = $("#ddlMCCodes").val();
        //var ddlMcApplyWeightAge = $("#ddlMCApplyWeightAge").val();
       // var ddlMcLevel = $("#ddlMcLevel").val();
        var mcInPatientBaseRate = $("#txtMCInPatientBaseRate").val();
        var txtMcAnnualOutOfPocket = $("#txtMCAnnualOutOfPocket").val();
        var txtMcdrgTableNumber = $("#txtMCDRGTableNumber").val();
        var txtMccptTableNumber = $("#txtMCCPTTableNumber").val();
        var chkMcPerDiemsApplicable = $("#chkMCPerDiemsApplicable")[0].checked;
        var chkMcCarveoutsApplicable = $("#chkMCCarveoutsApplicable")[0].checked;
        var bcIsActive = $("#chkBCIsActive")[0].checked;

        var modelName = $('#txtModelName').val();
        var mcCode = $('#txtMCCode').val();

        var initialSubmitDay = $('#txtInitialSubmitDay').val();
        var resubmitDays1 = $('#txtResubmitDays1').val();
        var resubmitDays2 = $('#txtResubmitDays2').val();
        var penaltyLateSubmission = $('#txtPenaltyLateSubmission').val();
        var billScrubberRule = $('#txtBillScrubberRule').val();
        var expectedPaymentDays = $('#txtExpectedPaymentDays').val();
        var MCInpatientDeduct = $("#txtMCPatientFixed").val();
        var mcOutpatientDeduct = $('#txtMCOutpatientDeduct').val();
        var mcMultiplierOutpatient = $('#txtMCMultiplierOutpatient').val();
        var mcMultiplierEmergencyRoom = $('#txtMCMultiplierEmergencyRoom').val();
        var mcMultiplierOther = $('#txtMCMultiplierOther').val();
        var mcRuleSetNumber = $('#txtMCRuleSetNumber').val();
        var mcPenaltyRateResubmission = $('#txtMCPenaltyRateResubmission').val();
        var chkMCEMCertified = $("#chkMCEMCertified")[0].checked;

        var chkMCInPatientType = $("#chkMCInPatientType")[0].checked;
        var chkMCOPPatientType = $("#chkMCOPPatientType")[0].checked;
        var chkMCERPatientType = $("#chkMCERPatientType")[0].checked;


        var mcAddon = $('#txtMCAddon').val();
        var mcExpectedFixedrate = $('#txtMCExpectedFixedrate').val();
        var mcExpectedPercentage = $('#txtMCExpectedPercentage').val();

        var txtArGeneralLedgerAccount = $('#txtARGeneralLedgerAccount').val();
        var txtMcGeneralLedgerAccount = $('#txtMCGeneralLedgerAccount').val();

        var jsonData = JSON.stringify({
            MCContractID: id,
            MCEncounterType: ddlMcEncounterType,
            MCOrderType: ddlMcOrderType,
            MCOrderCode: txtMcOrderCode,
            MCPatientFixed: txtMcPatientFixed,
            MCPatientPercent: txtMcPatientPercent,
            MCPatientCapping: txtMcPatientCapping,
            MCCode: mcCode, // ddlMcCodes,
            MCInPatientBaseRate: mcInPatientBaseRate,
            MCAnnualOutOfPocket: txtMcAnnualOutOfPocket,
            MCDRGTableNumber: txtMcdrgTableNumber,
            MCCPTTableNumber: txtMccptTableNumber,
            MCPerDiemsApplicable: chkMcPerDiemsApplicable,
            MCCarveoutsApplicable: chkMcCarveoutsApplicable,
            BCIsActive: bcIsActive,
            ModelName: modelName,
            ResubmitDays2: resubmitDays2,
            PenaltyLateSubmission: penaltyLateSubmission,
            InitialSubmitDay: initialSubmitDay,
            BillScrubberRule: billScrubberRule,
            ResubmitDays1: resubmitDays1,
            ExpectedPaymentDays: expectedPaymentDays,
            MCMultiplierOutpatient: mcMultiplierOutpatient,
            MCMultiplierEmergencyRoom: mcMultiplierEmergencyRoom,
            MCMultiplierOther: mcMultiplierOther,
            MCInpatientDeduct: MCInpatientDeduct,
            MCOutpatientDeduct: mcOutpatientDeduct,
            MCEMCertified: chkMCEMCertified,
            MCPenaltyRateResubmission: mcPenaltyRateResubmission,
            MCRuleSetNumber: mcRuleSetNumber,
            MCAddon: mcAddon,
            MCExpectedFixedrate: mcExpectedFixedrate,
            MCExpectedPercentage: mcExpectedPercentage,
            MCInPatientType: chkMCInPatientType,
            MCOPPatientType: chkMCOPPatientType,
            MCERPatientType: chkMCERPatientType,
            ARGeneralLedgerAccount: txtArGeneralLedgerAccount,
            MCGeneralLedgerAccount: txtMcGeneralLedgerAccount,
        });

        $.ajax({
            type: "POST",
            url: '/McContract/SaveMcContract',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindList('#McContractListDiv', data);
                    ClearMcContractForm();
                    var msg = "Records Saved successfully !";
                    if (id > 0)
                        msg = "Records updated successfully";
                    ShowMessage(msg, "Success", "success", true);
                }
            },
            error: function (msg) {
            }
        });
    }
    return false;
}

function EditMcContract(mcContractId) {
    var jsonData = JSON.stringify({
        id: mcContractId,
    });
    $.ajax({
        type: "POST",
        url: '/McContract/GetMcContractDetail',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindMcContractDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteMcContract() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var txtMcContractId = $("#hfGlobalConfirmId").val();
        var jsonData = JSON.stringify({
            Id: txtMcContractId,
            IsDeleted: true,
            DeletedBy: 1,//Put logged in user id here
            DeletedDate: new Date(),
            IsActive: false
        });
        $.ajax({
            type: "POST",
            url: '/McContract/DeleteMcContract',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindList('#McContractListDiv', data);
                    ShowMessage("McContract Deleted Successfully!", "Success", "info", true);
                    SetSelectedRowColor();
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

//function DeleteMcContract(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtMcContractId = id;
//        var jsonData = JSON.stringify({
//            Id: txtMcContractId,
//            IsDeleted: true,
//            DeletedBy: 1,//Put logged in user id here
//            DeletedDate: new Date(),
//            IsActive: false
//        });
//        $.ajax({
//            type: "POST",
//            url: '/McContract/DeleteMcContract',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindList('#McContractListDiv', data);
//                    ShowMessage("McContract Deleted Successfully!", "Success", "info", true);
//                    SetSelectedRowColor();
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
//    return false;
//}

function ClearMcContractForm() {
    $("#McContractDiv").clearForm(true);
    $('#collapseMcContractAddEditForm').removeClass('in');
    $('#collapseMcContractListForm').addClass('in');
    $.validationEngine.closePrompt(".formError", true);
    $("#BtnSaveMcContract").val('Save');
    LoadMcContractData();
    $('#chkBCIsActive').prop('checked', true);
}

function OnChangeOrderTypeDropdown() {
    var orderTypeSelectedValue = $("#hdMCOrderType ").val();
    $("#txtMcOrderCode").attr("disabled", orderTypeSelectedValue > 0);
}

function BindMcContractDetails(data) {
    $("#hdMCContractID").val(data.MCContractID);
    $("#ddlMCCodes").val(data.MCCode);
    $('#txtModelName').val(data.ModelName);
    $('#txtMCCode').val(data.MCCode);
    $('#txtInitialSubmitDay').val(data.InitialSubmitDay);
    $('#txtResubmitDays1').val(data.ResubmitDays1);
    $('#txtResubmitDays2').val(data.ResubmitDays2);
    $('#txtPenaltyLateSubmission').val(data.PenaltyLateSubmission);
    $('#txtBillScrubberRule').val(data.BillScrubberRule);
    $('#txtExpectedPaymentDays').val(data.ExpectedPaymentDays);
    $("#ddlMcEncounterType").val(data.MCEncounterType);
    $("#ddlMcLevel").val(data.MCLevel);
    $("#ddlMCOrderType").val(data.MCOrderType);
    $("#txtMcOrderCode").val(data.OrderCodeDescription);
    //$("#hdMCOrderCode").val(data.MCOrderCode);
    $("#txtMCOrderCode").val(data.MCOrderCode);
    $("#txtMCPatientFixed").val(data.MCPatientFixed);
    $("#txtMCPatientPercent").val(data.MCPatientPercent);
    $("#txtMCPatientCapping").val(data.MCPatientCapping);
    $("#txtMCMultiplier").val(data.MCMultiplier);
    $("#txtMCWaitingDays").val(data.MCWaitingDays);
    $("#txtMCExpireAfterDays").val(data.MCExpireAfterDays);
    $("#ddlMCApplyWeightAge").val(data.MCApplyWeightAge);
    $("#txtMCInPatientBaseRate").val(data.MCInPatientBaseRate);
    $("#txtMCAnnualOutOfPocket").val(data.MCAnnualOutOfPocket);
    $("#BtnSaveMcContract").val('Update');
    $('#collapseMcContractListForm').removeClass('in');
    //$('#collapseMcContractAddEditForm').addClass('in');
    //$("#txtMcOrderCode").attr("disabled", false);
    //ShowOrderPanel("#ddlMcLevel");
    //OnChangeEncounterLevel("#ddlMcEncounterType");
    $("#txtMCDRGTableNumber").val(data.MCDRGTableNumber);
    $("#txtMCCPTTableNumber").val(data.MCCPTTableNumber);
    if (data.MCPerDiemsApplicable != null && data.MCPerDiemsApplicable == true) {
        $("#chkMCPerDiemsApplicable").prop("checked", true);
    }
    if (data.MCCarveoutsApplicable != null && data.MCCarveoutsApplicable == true) {
        $("#chkMCCarveoutsApplicable").prop("checked", true);
    }
    if (data.BCIsActive != null && data.BCIsActive == true) {
        $("#chkBCIsActive").prop("checked", true);
    }

    if (data.MCLevel == '3' || data.MCLevel == "4") {
        $('#ddlOrderTypeCategory').val(data.OrderCategoryId);
        $('#txtOrderCode').val(data.MCOrderCode);
        $('#hdMCOrderCode').val(data.MCOrderCode);
        OnChangeCategory('#ddlOrderTypeCategory', "#ddlOrderTypeSubCategory");
        setTimeout(function () {
            $('#ddlOrderTypeSubCategory').val(data.OrderSubCategoryId);
            setTimeout(function () {
                OnChangeSubCatgory('#ddlOrderTypeSubCategory');
                $('#ddlOrderCodes').val(data.MCOrderCode);
            }, 1000);
        }, 2000);
    }
    $('#txtMCOutpatientDeduct').val(data.MCOutpatientDeduct);
    $('#txtMCMultiplierOutpatient').val(data.MCMultiplierOutpatient);
    $('#txtMCMultiplierEmergencyRoom').val(data.MCMultiplierEmergencyRoom);
    $('#txtMCMultiplierOther').val(data.MCMultiplierOther);
    $('#txtMCRuleSetNumber').val(data.MCRuleSetNumber);
    $('#txtMCPenaltyRateResubmission').val(data.MCPenaltyRateResubmission);
    $("#chkMCEMCertified").prop('checked', data.MCEMCertified);

    $('#txtMCAddon').val(data.MCAddon);
    $('#txtMCExpectedFixedrate').val(data.MCExpectedFixedrate);
    $('#txtMCExpectedPercentage').val(data.MCExpectedPercentage);

    $("#chkMCInPatientType").prop('checked', data.MCInPatientType);
    $("#chkMCOPPatientType").prop('checked', data.MCOPPatientType);
    $("#chkMCERPatientType").prop('checked', data.MCERPatientType);
    $('#txtARGeneralLedgerAccount').val(data.ARGeneralLedgerAccount);
    $('#txtMCGeneralLedgerAccount').val(data.MCGeneralLedgerAccount);
    $('#collapseMcContractAddEditForm').addClass('in');

    $('#collapseMcContractAddEditForm').attr('style', 'height:100%');
}

function BindOrderTypeCategories(ddlSelector, hdSelector) {
    var jsonData = JSON.stringify({
        startRange: "11000",
        endRange: "11999"
    });
    $.ajax({
        type: "POST",
        url: '/Home/GetGlobalCodeCatByExternalValue',   //GetGlobalCodeCategories
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $(ddlSelector).empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data, function (i, gcc) {
                items += "<option value='" + gcc.GlobalCodeCategoryValue.trim() + "'>" + gcc.GlobalCodeCategoryName + "</option>";
            });
            $(ddlSelector).html(items);

            var hdValue = $(hdSelector).val();
            if (hdValue != null && hdValue != '' && hdValue > 0) {
                $(ddlSelector).val(hdValue);
                OnChangeCategory(ddlSelector, "#ddlOrderTypeSubCategory", "");
            }
            else {
                // if ($(ddlSelector) != null){ $(ddlSelector)[0].selectedIndex = 0;}
            }
        },
        error: function (msg) {
        }
    });
}

//-------------Smart Search feature for "Order Codes" starts here---------------

//function SelectOrderCodeInMcContract(e) {
//    var dataItem = this.dataItem(e.item.index());
//    $('#hdMCOrderCode').val(dataItem.Code);
//}

//function OnMcOrderCodeSelection(e) {
//    var orderTypeId = $("#ddlMCOrderType").val();
//    var value = null;
//    if (e.filter.filters != null && e.filter.filters.length > 0) {
//        value = e.filter.filters[0].value;
//    }
//    return {
//        orderTypeId: orderTypeId,
//        text: value
//    };
//}

function BindCategories(ddlSelector, hdSelector) {
    var jsonData = JSON.stringify({
        startRange: "11000",
        endRange: "11999"
    });
    $.ajax({
        type: "POST",
        url: '/Home/GetGlobalCodeCatByExternalValue',        //GetGlobalCodeCategories
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $(ddlSelector).empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data, function (i, gcc) {
                items += "<option value='" + gcc.GlobalCodeCategoryValue.trim() + "'>" + gcc.GlobalCodeCategoryName + "</option>";
            });
            $(ddlSelector).html(items);

            var hdValue = hdSelector > 0 ? hdSelector : $(hdSelector).val();
            if (hdValue != null && hdValue != '' && hdValue > 0) {
                $(ddlSelector).val(hdValue);
                OnChangeCategory(ddlSelector, "#ddlOrderTypeSubCategory");
            }
            else {
                // if ($(ddlSelector) != null){ $(ddlSelector)[0].selectedIndex = 0;}
            }
        },
        error: function (msg) {
        }
    });
}

function OnChangeCategory(categorySelector, ddlSelector) {
    var categoryId = $(categorySelector).val();
    if (categoryId != '' && categoryId != "0") {
        var jsonData = JSON.stringify({
            categoryId: categoryId
        });
        if ($("#ddlOrderTypeCategory :selected").text() == "Pharmacy") {
            $("#lblSubcategory").html("Generic Drug Name");
        } else {
            $("#lblSubcategory").html("Order Type Sub-Category");
        }
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/GetOrderTypeSubCategories",
            data: jsonData,
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    BindDropdownData(data, ddlSelector, '');
                }
            },
            error: function (msg) {
            }
        });
        return false;
    }
}

function OnChangeSubCatgory(ddlSelector) {
    var value = $(ddlSelector).val();
    var jsonData = JSON.stringify({
        subCategoryId: value
    });
    var orderCategory = $("#ddlOrderTypeCategory :selected").text();
    if (orderCategory == "Pharmacy") {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/GetPharmacyOrderCodesBySubCategory",
            //data: JSON.stringify({ id: $("#hdOrderTypeSubCategoryID").val() }),
            data: jsonData,
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    //Set Order Type Code Name
                    $("#CodeTypeValue").text(data.codeTypeName);
                    //Set Order Type Code Id
                    $("#hdMCOrderType").val(data.codeTypeId);
                    BindDropdownData(data.codeList, "#ddlOrderCodes", "#hdMCOrderCode");
                }
            },
            error: function (msg) {
            }
        });
    } else if (orderCategory == "Lab Test") {
        var items = '<option value="0">--Select--</option>';
        var newItem = "<option id='" + $("#ddlOrderTypeCategory").val() + "'  value='" + $("#ddlOrderTypeCategory").val() + "'>" + $("#ddlOrderTypeCategory :selected").text() + "</option>";
        items += newItem;
        $("#ddlOrderCodes").html(items);
        //Set Order Type Code Name
        $("#CodeTypeValue").text("LAB Test");
        //Set Order Type Code Id
        $("#hdMCOrderType").val('7');
    }
    else {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/GetOrderCodesBySubCategory",
            //data: JSON.stringify({ id: $("#hdOrderTypeSubCategoryID").val() }),
            data: jsonData,
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    //Set Order Type Code Name
                    $("#CodeTypeValue").text(data.codeTypeName);
                    //Set Order Type Code Id
                    $("#hdMCOrderType").val(data.codeTypeId);
                    BindDropdownData(data.codeList, "#ddlOrderCodes", '');
                }
            },
            error: function (msg) {
            }
        });
    }
}

function SelectMcContractOrderingCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtOrderCode").val(dataItem.CodeDescription);
    $("#hdMCOrderType").val(dataItem.CodeType);
    var items = '<option value="0">--Select--</option>';
    items += "<option value='" + dataItem.Code + "'>" + dataItem.CodeDescription + "</option>";
    SetValueToOrderingCodesDropdown(dataItem.Code, items);
    BindAllDDLValues();
}

function BindAllDDLValues() {
    if ($("#ddlOrderTypeCategory").val() == "0" || $("#ddlOrderTypeCategory").val() == null) {
        var orderCodesVal = $("#ddlOrderCodes").val();
        var hdOrderType = $('#hdMCOrderType').val();
        if (orderCodesVal != '' || orderCodesVal != '0' || orderCodesVal != null) {
            var jsonData = JSON.stringify({
                code: orderCodesVal,
                Type: hdOrderType
            });
            $.ajax({
                type: "POST",
                url: '/GlobalCode/GetSelectedCodeParent',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonData,
                success: function (data) {
                    if (data != null) {
                        $('#ddlOrderTypeCategory').val(data.GlobalCodeCategoryId);
                        //$('#hdOrderTypeSubCategoryID').val(data.GlobalCodeId);
                        OnChangeCategory('#ddlOrderTypeCategory', '#ddlOrderTypeSubCategory');
                    }
                },
                error: function (msg) {
                }
            });
        }
    }
}

function SetSelectedField() {
    var ordercodesVal = $('#ddlOrderCodes').val();
    if (ordercodesVal != '' || ordercodesVal != '0') {
        $('#txtOrderCode').val($('#ddlOrderCodes :selected').text());
    }
}

function OnMCContractCodeSelection(e) {
    var subCategoryId = $("#ddlOrderTypeSubCategory").val();
    if (subCategoryId == null || subCategoryId == undefined)
        subCategoryId = 0;
    var value = null;
    if (e.filter.filters != null && e.filter.filters.length > 0) {
        value = e.filter.filters[0].value;
    }
    return {
        text: value,
        subCategoryId: subCategoryId
    };
}

//Set the Selected Value to Ordering Codes Dropdown
function SetValueToOrderingCodesDropdown(selectedValue, item) {
    var ddlSubCategory = $("#ddlOrderTypeSubCategory");
    var ddlSelector = $("#ddlOrderCodes");
    if (ddlSelector.length == 0 || ddlSubCategory.val() == '' || ddlSubCategory.val() == null) {
        $(ddlSelector).empty();
        $(ddlSelector).html(item);
    }
    $(ddlSelector).val(selectedValue);
}


//-------------Smart Search feature for "Order Codes" ends here---------------
function ShowOrderPanel(ddlSelector) {
    var selectedValue = $(ddlSelector).val();
    if (selectedValue == 3 || selectedValue == 4) {
        $("#divOrdersInMcContract").show();
        $('.mainClass').removeClass('col-lg-3').addClass('col-lg-2');
        $("#divOrdersInMcContract").removeClass('col-lg-2').addClass('col-lg-3');
    } else {
        $("#divOrdersInMcContract").hide();
        $('.mainClass').removeClass('col-lg-2').addClass('col-lg-3');
    }
}

function OnChangeEncounterLevel(ddlSelector) {
    var selectedValue = $(ddlSelector).val();
    if (selectedValue > 0) {
        if (selectedValue != 1) {
            $("#txtMCPatientFixed").prop("disabled", false);
        } else {
            $("#txtMCPatientFixed").prop("disabled", "disabled");
        }
        if (selectedValue == 2) {
            $("#txtMCInPatientBaseRate").prop("disabled", false);
        }
        else {
            $("#txtMCInPatientBaseRate").prop("disabled", "disabled");
        }
    }
}

var ViewMCRuleSteps = function (mcContractId) {
    var jsonData = JSON.stringify({
        McContractID: mcContractId,
    });
    $.ajax({
        type: "POST",
        url: '/MCRulesTable/BindRuleStepList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $('.MCOrderCodeRates').show();
            $('.ElsePart').show();
            BindList("#RuleStepListDiv", data);
            BindListHeaderDetails(mcContractId);
           
            GetMaxStepNumber();
            ColorCodeRuleSteps();
        },
        error: function (msg) {

        }
    });
};

var BindListHeaderDetails = function (mcContractId) {
    var jsonData = JSON.stringify({
        rmId: mcContractId,
    });
    $.ajax({
        type: "POST",
        url: '/McContract/GetRuleMasterById',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $('.spnRuleMasterDesc').text('(' + data.ModelName + ' )');
            $('.spnRuleCodeDesc').text('(' + data.ModelName + ' )');
            $('#txtMCCode1').val(data.MCCode);
            $("#hdRuleSetNumber").val(data.MCCode);
            $('#txtMCCode1').val(data.MCCode);
        },
        error: function (msg) {

        }
    });
};

var SetSelectedRowColor = function () {
    $('.ViewRulestep').on('click', function () {
        var tr = $(this).parents('tr:first');
        tr.css('background', '#ffff00');
        tr.siblings().css('background', 'none');
    });
};

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

var BindBedChargesDDL = function() {
    var patienttype = $('#ddlMcEncounterType').val();
    if (patienttype != '') {
        if (patienttype != '2') {
            $("#ddlMCOrderType option[value='8']").remove();
        } else {
            $("#ddlMCOrderType").empty();
            $('#ddlMCOrderType').append($('<option>', {
                value: '0',
                text: '--Select--'
            }));
            $('#ddlMCOrderType').append($('<option>', {
                value: '3',
                text: 'CPT'
            }));
            $('#ddlMCOrderType').append($('<option>', {
                value: '5',
                text: 'DRUG'
            }));
            $('#ddlMCOrderType').append($('<option>', {
                value: '8',
                text: 'Bed Charges'
            }));
        }
    }
};

var ViewMCOrderRatediv = function (mcContractId) {
    var jsonData = JSON.stringify({
        McContractID: mcContractId,
    });
    $.ajax({
        type: "POST",
        url: '/MCOrderCodeRates/BindMCOrderCodeList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $('.MCOrderCodeRates').show();
            BindList("#MCOrderCodeRatesListDiv", data);
            setTimeout(function () {
                $("#MCOrderCodeRatesFormDiv").clearForm(true);
                BindListHeaderDetails(mcContractId);
                ColorCodeOrderRateSteps();
                ClearMCOrderCodeRatesForm();
                ViewMCRuleSteps(mcContractId);
            },500);
        },
        error: function (msg) {

        }
    });
};

var GetMCOverView = function(mcCode) {
    var jsonData = JSON.stringify({
        mcCode: mcCode,
    });
    $.ajax({
        type: "POST",
        url: '/McContract/GetMcOverviewString',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function(data) {
            if (data == '') {
                $('#contractViewdiv').html(data);
                $('.hidePopUp').hide();
            } else {
                $('#contractViewdiv').html(data);
                $('.hidePopUp').show();
            }

        },
        error: function(msg) {

        }
    });
};