var summaryPageUrl = "/Summary/";
var uploadCharges = "/UploadCharges/";

$(function () {

    BindOrders();
    PrimaryDiagnosisRowColor();
    $(".EmiratesMask").mask("999-99-9999");
    var ButtonKeys = { "EnterKey": 13 };
    $(".white-bg").keypress(function (e) {
        if (e.which == ButtonKeys.EnterKey) {
            $("#btnSearch").click();
        }
    });
    BindCountryDataWithCountryCode("#ddlCountries", "#hdCountry", "#lblCountryCode");

    if ($("#CorrectionsViewHfPatientId").length > 0 && $("#CorrectionsViewHfPatientId").val() > 0) {
        var patientId = $("#CorrectionsViewHfPatientId").val();
        var eId = $("#CorrectionsViewHfEncounterId").val();
        var billHeaderId = $("#CorrectionsViewHfBillHeaderId").val();
        GetPatientResultsFromPatientId(patientId, eId, billHeaderId);
        $("#CorrectionsViewHfPatientId").val('');
        $("#CorrectionsViewHfEncounterId").val('');
        $("#CorrectionsViewHfBillHeaderId").val('');
    }
});

function BindOrders() {
    BindGlobalCodesWithValue("#ddlFrequencyList", 1025, "#hdFrequencyCode");
    BindGlobalCodesWithValue("#ddlOrderStatus", 3102, "#hdOrderStatus");
    BindGlobalCodesWithValueWithOrder("#ddlQuantityList", 1011, "#hdQuantity");
    BindCategories("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
    $("#ddlOrderStatus").val('3');
    $("#ddlOrderStatus").attr('disabled', 'disabled');
    $("#OpenOrderDiv").validationEngine();
}

function EditOrders(eid, pid) {
    var jsonData = JSON.stringify({
        encounterId: eid,
        patientId: pid
    });
    $.ajax({
        type: "POST",
        url: '/ActiveEncounter/GetOpenOrders',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                $("#EditOpenOrdersDiv").empty();
                $("#EditOpenOrdersDiv").html(data);
                $('#divhidepopup').show();
                BindOrders();
            }
        },
        error: function (msg) {
        }
    });
}

function GetPhysicianAllClosed() {
    var encounterId = $("#hdCurrentEncounterId1").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/BindClosedOrders",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            $("#divEncounterClosedOrders").empty();
            $("#divEncounterClosedOrders").html(data);
            //RowColor();
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function ResetOrder() {

    $("#divQuantityMultiple").hide();
    $.validationEngine.closePrompt(".formError", true);
    $("#divAddEditOrder").clearForm();
    ResetAllDropdowns("#OpenOrderDiv");
    $('#collapseOpenOrderAddEdit').removeClass('in');
    $('#collapseOpenOrderlist').addClass('in');
    $('#divQuantityMultiple').empty();
    SetDateRange();
    $('#ddlOrderStatus').val('3');
    $('#ddlOrderCodes').empty();
}

function BindCategories(ddlSelector, hdSelector) {
    var jsonData = JSON.stringify({
        startRange: "11000",
        endRange: "11999"
    });
    $.ajax({
        type: "POST",
        url: summaryPageUrl + "GetOrderTypeCategoriesInSummary",           //GetGlobalCodeCategories
        //url: '/Home/GetGlobalCodeCatByExternalValue',        //GetGlobalCodeCategories
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
                OnChangeCategory(ddlSelector, "#ddlOrderTypeSubCategory", "#hdOrderTypeSubCategoryID");
            }
            else {
                // if ($(ddlSelector) != null){ $(ddlSelector)[0].selectedIndex = 0;}
            }
        },
        error: function (msg) {
        }
    });
}

function SelectOrderingCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtOrderCode").val(dataItem.CodeDescription);
    $("#hdAutocompleteOrderCodeId").val(dataItem.CodeDescription);
    $("#hdOrderType").val(dataItem.CodeType);
    $("#hdOrderExternalCode").val(dataItem.ExternalCode);
    $("#hidOrderCodeValue").val(dataItem.Code);
    $('#CodeTypeValue').html(dataItem.CodeTypeName);
    var items = "<option value='" + dataItem.Code + "'>" + dataItem.CodeDescription + "</option>";
    SetValueToOrderingCodesDropdown(dataItem.Code, items);
    $('#hdOrderCodeId').val(dataItem.Code);
    BindAllDDLValues();
    setTimeout(function () {
        //$("#ddlOrderCodes option:contains(" + $('#txtOrderCode').val() + ")").attr('selected', 'selected');
        $("#ddlOrderCodes").val(dataItem.Code);
        //$("#ddlOrderCodes").val($("#hidOrderCodeValue").val());
        //$("#ddlOrderCodes").val('selected', 'selected');
    }, 2000);
}

function OnChangeCategory(categorySelector, ddlSelector, hdSubCategorySelector) {
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
            //url: "/Summary/GetOrderTypeSubCategories",
            url: summaryPageUrl + "GetOrderTypeSubCategories",
            data: jsonData,
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    BindOrderSubCategoriesWithCustomFields(data, ddlSelector, hdSubCategorySelector);

                    if ($(hdSubCategorySelector).val() > 0) {
                        OnChangeSubCatgory(ddlSelector);
                    }
                    //$("#txtOrderCode").attr("disabled", false);
                    //$("#txtCodeDescription").attr("disabled", false);
                }
            },
            error: function (msg) {
            }
        });
        return false;
    }

}

//function OnChangeSubCatgory(ddlSelector) {
//    var value = $(ddlSelector).val();
//    var jsonData = JSON.stringify({
//        subCategoryId: value
//    });
//    //$('#ddlDosageForm').removeClass('validate[required]');
//    //$('#ddlDosageAmount').removeClass('validate[required]');
//    $('.DrugDDL').hide();
//    var orderCategory = $("#ddlOrderTypeCategory :selected").text();
//    if (orderCategory == "Pharmacy") {
//        $.ajax({
//            type: "POST",
//            contentType: "application/json; charset=utf-8",
//            url: "/Summary/GetPharmacyOrderCodesBySubCategory",
//            //data: JSON.stringify({ id: $("#hdOrderTypeSubCategoryID").val() }),
//            data: jsonData,
//            dataType: "json",
//            success: function (data) {
//                if (data != null) {
//                    //Set Order Type Code Name
//                    $("#CodeTypeValue").text(data.codeTypeName);
//                    //Set Order Type Code Id
//                    $("#hdOrderTypeId").val(data.codeTypeId);
//                    BindOrderCodesBySubCategoryID(data.codeList, "#ddlOrderCodes", "#hdOrderCodeId");
//                    $('.DrugDDL').hide();
//                    //$('#ddlDosageForm').addClass('validate[required]');
//                    // $('#ddlDosageAmount').addClass('validate[required]');
//                }
//            },
//            error: function (msg) {
//            }
//        });
//    } else if (orderCategory == "Lab Test") {
//        var items = '<option value="0">--Select--</option>';
//        var newItem = "<option id='" + $("#ddlOrderTypeCategory").val() + "'  value='" + $("#ddlOrderTypeCategory").val() + "'>" + $("#ddlOrderTypeCategory :selected").text() + "</option>";
//        items += newItem;
//        $("#ddlOrderCodes").html(items);
//        //Set Order Type Code Name
//        $("#CodeTypeValue").text("LAB Test");
//        //Set Order Type Code Id
//        $("#hdOrderTypeId").val('11');
//    }
//    else {
//        $.ajax({
//            type: "POST",
//            contentType: "application/json; charset=utf-8",
//            url: "/Summary/GetOrderCodesBySubCategory",
//            //data: JSON.stringify({ id: $("#hdOrderTypeSubCategoryID").val() }),
//            data: jsonData,
//            dataType: "json",
//            success: function (data) {
//                if (data != null) {
//                    //Set Order Type Code Name
//                    $("#CodeTypeValue").text(data.codeTypeName);
//                    //Set Order Type Code Id
//                    $("#hdOrderTypeId").val(data.codeTypeId);
//                    BindOrderCodesBySubCategoryID(data.codeList, "#ddlOrderCodes", "#hdOrderCodeId");
//                }
//            },
//            error: function (msg) {

//            }
//        });
//    }
//}

function BindOrderCodesBySubCategoryID(data, ddlSelector, hdSelector) {
    BindDropdownData(data, ddlSelector, hdSelector);
    $('#collapseOpenOrderAddEdit').addClass('in');
}

function BindDrugDDLValue() {
    if ($("#ddlOrderTypeCategory :selected").text() == "Pharmacy") {
        $('.DrugDDL').hide();
        var orderCodesVal = $("#ddlOrderCodes").val();
        if (orderCodesVal != '') {
            var jsonData = JSON.stringify({
                drugcode: orderCodesVal,
            });
            $.ajax({
                type: "POST",
                url: '/Home/GetDrugDetailsByDrugCode',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonData,
                success: function (data) {
                    if (data != null) {
                        $("#ddlDosageForm").empty();
                        var itemsDosageForm = '<option value="0">--Select--</option>';
                        $.each(data, function (i, item) {
                            itemsDosageForm += "<option value='" + item.Id + "'>" + item.DrugDosage + "</option>";
                        });
                        $("#ddlDosageForm").html(itemsDosageForm);

                        if ($("#hdItemDosage") != null && $("#hdItemDosage").val() > 0)
                            $("#ddlDosageForm").val($("#hdItemDosage").val());

                        $("#ddlDosageAmount").empty();
                        var itemsAmount = '<option value="0">--Select--</option>';
                        $.each(data, function (i, item) {
                            itemsAmount += "<option value='" + item.Id + "'>" + item.DrugStrength + "</option>";
                        });
                        $("#ddlDosageAmount").html(itemsAmount);

                        if ($("#hdItemAmount") != null && $("#hdItemAmount").val() > 0)
                            $("#ddlDosageAmount").val($("#hdItemAmount").val());
                    }
                },
                error: function (msg) {
                }
            });
        }
    } else {
        $('.DrugDDL').hide();
    }
}

function BindAllDDLValues() {
    if ($("#ddlOrderTypeCategory").val() == "0" || $("#ddlOrderTypeCategory").val() == null) {
        var orderCodesVal = $("#ddlOrderCodes").val();
        var hdOrderType = $('#hdOrderType').val();
        if (orderCodesVal != '' || orderCodesVal != '0' || orderCodesVal != null) {
            var jsonData = JSON.stringify({
                code: orderCodesVal,
                Type: hdOrderType
            });
            $.ajax({
                type: "POST",
                url: '/Home/GetSelectedCodeParent',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonData,
                success: function (data) {
                    if (data != null) {
                        $('#ddlOrderTypeCategory').val(data.GlobalCodeCategoryId);
                        $('#hdOrderTypeSubCategoryID').val(data.GlobalCodeId);
                        OnChangeCategory('#ddlOrderTypeCategory', '#ddlOrderTypeSubCategory', '#hdOrderTypeSubCategoryID');
                    }
                },
                error: function (msg) {
                }
            });
        }
    }
}

function OnCodeSelection(e) {
    var categoryId = $("#ddlOrderTypeCategory").val();
    var subCategoryId = $("#ddlOrderTypeSubCategory").val();
    if (subCategoryId == null)
        subCategoryId = 0;
    var value = null;
    if (e.filter.filters != null && e.filter.filters.length > 0) {
        value = e.filter.filters[0].value;
    }
    return {
        text: value,
        subCategoryId: subCategoryId,
        categoryId: categoryId
    };
}

function CheckForIsFav() {
    var orderCode = $('#ddlOrderCodes').val();
    if (orderCode != "0") {
        var jsonData = JSON.stringify({ codeid: orderCode });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/CheckCodeForFav",
            dataType: "json",
            async: true,
            data: jsonData,
            success: function (data) {
                if (data == "-1") {
                    $('#chkMarkAsFavorite').prop('checked', false);
                    $('#hdFavoriteId').val();
                    $('#txtFavoriteDescription').val();
                    $('#favoriteOrderDescDiv').hide();
                } else {
                    $('#chkMarkAsFavorite').prop('checked', true);
                    $('#hdFavoriteId').val(data.UserDefinedDescriptionID);
                    $('#txtFavoriteDescription').val(data.UserDefineDescription);
                    $('#favoriteOrderDescDiv').show();
                }

                //Set the Text to Orderding Code Smart TextBox.
                $("#txtOrderCode").val($('#ddlOrderCodes option:Selected').text());
            },
            error: function (msg) {

            }
        });
    }
}

function SaveDiagnosisData(id) {

    //var diagnosisStart = $("#txtDRGStartDate").val().split(' ')[0].split('/');
    //var diagnosisStartDate = diagnosisStart[1] + "/" + diagnosisStart[0] + "/" + diagnosisStart[2];
    //var txtDiagnosisStartDate = diagnosisStartDate + " " + $('#txtDRGStartTimeHrs').val() + ":" + $('#txtDRGStartTimeMins').val();


    var txtDiagnosisStartDate = $("#txtDRGStartDate").val() + " " + $('#txtDRGStartTimeHrs').val() + ":" + $('#txtDRGStartTimeMins').val();

    var encounterId = $("#hdCurrentEncounterId1").val();
    var jsonData = JSON.stringify({ encounterId: encounterId, orderStartDate: txtDiagnosisStartDate });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/UploadCharges/CheckDiagnosisDateRange",
        dataType: "json",
        async: true,
        data: jsonData,
        success: function (data) {
            if (data == false) {
                ShowMessage("Diagnosis date range should be in the range of the Encounter Start and end Date .", "Warning", "warning", true);
                return false;
            } else if (data == true) {
                AddDiagnosisData(id);
                return true;
            } else {
                ShowMessage("Diagnosis date range should be in the range of the Encounter Start and end Date .", "Warning", "warning", true);
                return false;
            }
        },
        error: function (msg) {
        }
    });


};

function IsValidOrderInUploadCharges(id) {
    var isValid = false;
    if ($("#divPhysicianOrder").html() != null) {
        isValid = jQuery("#divPhysicianOrder").validationEngine({ returnIsValid: true });
    } else {
        isValid = jQuery("#OpenOrderDiv").validationEngine({ returnIsValid: true });
    }
    if ($('#txtOrderStartDate').val() > $('#txtOrderEndDate').val()) {
        ShowMessage("End Date Should Be Greater than Start Date!", "Warning", "warning", true);
        return false;
    }
    if (isValid == true) {
        CheckOrderDateRange(id);
    }
    return false;
}

var CheckOrderDateRange = function (id) {

    var orderStartDateParts = $("#txtOrderStartDate").val().split(' ')[0].split('/');
    var orderEndDateParts = $("#txtOrderEndDate ").val().split(' ')[0].split('/');


    //var orderStartDate = new Date(orderStartDateParts[2], orderStartDateParts[1] - 1, orderStartDateParts[0]);
    //var orderEndDate = new Date(orderEndDateParts[2], orderEndDateParts[1] - 1, orderEndDateParts[0]);
    //var orderStartDate = orderStartDateParts[1] + "/" + orderStartDateParts[0] + "/" + orderStartDateParts[2];
    var orderStartDate = orderStartDateParts[0] + "/" + orderStartDateParts[1] + "/" + orderStartDateParts[2];
    var orderEndDate = orderEndDateParts[0] + "/" + orderEndDateParts[1] + "/" + orderEndDateParts[2];
    //var txtOrderEndDate = new Date(orderEndDate);
    //var txtOrderStartDate = new Date(orderStartDate);
    var txtOrderEndDate = orderEndDate;
    var txtOrderStartDate = orderStartDate;

    //var newCustomStartDate = txtOrderStartDate.toString().split('00:00:00')[0] + " " + $('#txtOrderStartTimeH').val() +":"+$('#txtOrderStartTimeM').val();
    //var newCustomEndDate = txtOrderEndDate.toString().split('00:00:00')[0] + " " + $('#txtOrderEndTimeH').val() + ":" + $('#txtOrderEndTimeM').val();
    var newCustomStartDate = txtOrderStartDate + " " + $('#txtOrderStartTimeH').val() + ":" + $('#txtOrderStartTimeM').val();
    var newCustomEndDate = txtOrderEndDate + " " + $('#txtOrderEndTimeH').val() + ":" + $('#txtOrderEndTimeM').val();

    var encounterId = $("#hdCurrentEncounterId1").val();
    var jsonData = JSON.stringify({ encounterId: encounterId, orderStartDate: newCustomStartDate, orderEndDate: newCustomEndDate });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/UploadCharges/CheckOrderDateRange",
        dataType: "json",
        async: true,
        data: jsonData,
        success: function (data) {
            if (data == false) {
                ShowMessage("Order date range should be in the range of the Encounter Start and end Date .", "Warning", "warning", true);
                return false;
            } else if (data == true) {
                AddOrderInUploadCharges(id);
                return true;
            } else {
                ShowMessage("Order date range should be in the range of the Encounter Start and end Date .", "Warning", "warning", true);
                return false;
            }
        },
        error: function (msg) {
        }
    });
};

function ClearPhysicianOrderAllInUploadCharges(encounterid) {
    ClearPhysicianOrderForm();
    $.validationEngine.closePrompt(".formError", true);
    //var jsonData = JSON.stringify({
    //    EncId: encounterid,
    //});
    //$.ajax({
    //    type: "POST",
    //    url: '/UploadCharges/ResetPhysicianOrderForm',
    //    async: false,
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "html",
    //    data: jsonData,
    //    success: function (data) {
    //        if (data) {

    //        }
    //        else {
    //            return false;
    //        }
    //    },
    //    error: function (msg) {
    //        return true;
    //    }
    //});
    //$('#OpenOrderDiv').empty();
    //$('#OpenOrderDiv').html(data);
    BindOrderGrid(encounterid);
    //InitializeDateTimePicker();
    $('#divQuantityMultiple').empty();
    $("#OpenOrderDiv").validationEngine();
    $('.disabledTxt').removeAttr("disabled");
    SetDateRange();
}

function AddCharges(encounterId, patientId) {
    $("#hdPatientID2").val(patientId);
    $("#hdCurrentEncounterId1").val(encounterId);
    var jsonData = JSON.stringify({
        EncounterID: encounterId,
    });
    $('#divOrdersList').show();
    $('#divAddEditOrder').show();
    $('.diagnosisview').hide();
    $('#RoomChargesDiv').hide();
    $.ajax({
        type: "POST",
        url: '/UploadCharges/GetOrdersByEncounterId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#OpenOrderListDiv').empty();
                $('#OpenOrderListDiv').html(data);
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

function PrimaryDiagnosisRowColor() {
    $("#EncountersGridContent tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        if ($actualRow.find('.colPrimaryDiagnosis').html().indexOf('No') != -1) {
            $actualRow.addClass('rowColor3');
        }
    });
}

function ClearPhysicianOrderForm() {
    $("#OpenOrderDiv").clearForm();
    $('.emptytxt').val('');
    $('.emptyddl').val('0');
    $('#ddlOrderStatus').val('3');
    $('#txtOrderEndDate').removeAttr('disabled');
    $('#ddlQuantityList').val('1');
    $('#divQuantitySingle').show();
    $('#divQuantityMultiple').hide();
}

function UpdateOrderActivities(orderid) {
    var jsonData = JSON.stringify({
        orderId: orderid,
    });
    $.ajax({
        type: "POST",
        url: '/UploadCharges/UpdateOrderActivities',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data) {

            }
        },
        error: function (msg) {
            return true;
        }
    });
}

function BindOrderGrid(encounterid) {
    var jsonData = JSON.stringify({ EncounterId: encounterid });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/UploadCharges/BindEncounterOrderList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#OpenOrderListDiv").empty();
            $("#OpenOrderListDiv").html(data);
        },
        error: function (msg) {
            alert(msg);
        }
    });
}

function GetPhysicianAllOrders() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PhysicianFavorites/GetPhysicianAllOrders",
        data: null,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            $("#OpenOrderListDiv").empty();
            $("#OpenOrderListDiv").html(data);
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function SearchClaimPatient() {
    var isvalidSearch = ValidClaimSearch();
    if (isvalidSearch) {
        var jsonData = JSON.stringify({
            PatientID: 0,
            PersonLastName: $("#txtLastName").val(),
            PersonEmiratesIDNumber: $("#txtEmiratesNationalId").val(),
            PersonPassportNumber: $("#txtPassportnumber").val(),
            PersonBirthDate: $("#txtBirthDate").val(),
            ContactMobilePhone: $("#txtMobileNumber").val()
        });
        $.ajax({
            type: "POST",
            url: '/UploadCharges/GetPatientDenailSearchResult',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                $('#divPatientSearch').show();
                $('#PatientSearchResultDiv').empty();
                $('#PatientSearchResultDiv').html(data);
            },
            error: function (msg) {
            }
        });
        BindCountryDataWithCountryCode("#ddlCountries", "#hdCountry", "#lblCountryCode");
    }
}

function ValidClaimSearch() {
    var txtvalue = 0;
    $('#ValidatePatientSearch input[type=text]').each(function () {
        if ($(this).val() != "") {
            txtvalue = txtvalue + 1;
        }
    });
    if (txtvalue < 1) {
        ShowMessage("Confirm at least one piece of information", "Alert", "warning", true);
        return false;
    }
    else {
        return true;
    }
    return false;
}

//$('html,body').animate({ scrollTop: $("#collapseDiagnosisAddEdit").offset().top }, 'fast');
function AddManualCharges(patientId, encounterId, claimId, aActivityId, startDate, endDate) {
    $('#divQuantityMultiple').empty();
    $('#divQuantityMultiple').hide();
    $("#OpenOrderDiv").clearForm();
    $('#divBillActivitesList').hide();
    AddCharges(encounterId, patientId);
    $("#hdOrderActivityID").val(aActivityId);
    $("#hdOrderClaimID").val(claimId);
    $('#hdEncounterEndDate').val(endDate);
    $('#hdEncounterStartDate').val(startDate);
    $("#hdPatientID2").val(patientId);
    $('#ddlOrderStatus').val('3');
    $('#ddlQuantityList').val('1.00');
    $('html,body').animate({ scrollTop: $("#collapseDiagnosisAddEdit").offset().top }, 'fast');
    SetDateRange();
}

function SetDateRange() {
    //setTimeout(function () {
    //    $('.disabledTxt').removeAttr("disabled");
    //    var startDateObj = $('#hdEncounterStartDate').val();
    //    var endDateObj = $('#hdEncounterEndDate').val();
    //    var enstartDate = new Date(startDateObj).format('mm/dd/yyyy');
    //    //var enendDate = endDateObj != '' ? new Date(endDateObj).format('mm/dd/yyyy') : new Date().format('mm/dd/yyyy');
    //    var enendDate = endDateObj != '' ? new Date(endDateObj).format('mm/dd/yyyy') : new Date().format('mm/dd/yyyy');
    //    var enstartTimeHours = new Date(startDateObj).format('HH');
    //    var enstartTimeMins = new Date(startDateObj).format('MM');
    //    var enStartMinsIncreaded = parseInt(enstartTimeMins) + 1;
    //    var enendTimeHours = endDateObj != '' ? new Date(endDateObj).format('HH') : new Date().format('HH');
    //    var enendTimeMins = endDateObj != '' ? new Date(endDateObj).format('MM') : new Date().format('MM');

    //    $('#txtOrderEndDate').datetimepicker({
    //        format: 'm/d/Y',
    //        minDate: '1901/12/12',
    //        maxDate: '2050/12/12',
    //        timepicker: false,
    //        closeOnDateSelect: true
    //    });
    //    $('#txtOrderStartDate').datetimepicker({
    //        format: 'm/d/Y',
    //        minDate: '1901/12/12',
    //        maxDate: '2050/12/12',
    //        timepicker: false,
    //        closeOnDateSelect: true
    //    });
    //    $('#txtOrderStartDate').val(enstartDate);
    //    $('#txtOrderEndDate').val(enendDate);
    //    $('#txtOrderStartTimeH').val(enstartTimeHours);
    //    $('#txtOrderStartTimeM').val(enStartMinsIncreaded);
    //    $('#txtOrderEndTimeH').val(enendTimeHours);
    //    $('#txtOrderEndTimeM').val(enendTimeMins);
    //}, 500);

    $('.disabledTxt').removeAttr("disabled");

    $('#txtOrderEndDate').datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12',
        maxDate: '2050/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });
    $('#txtOrderStartDate').datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12',
        maxDate: '2050/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });

    var startDate = $('#hdEncounterStartDate').val();
    var endDate = $('#hdEncounterEndDate').val();

    if (startDate != null && startDate != "") {
        var sDatePart = startDate.split(' ')[0];
        var sTimePart = startDate.split(' ')[1];
        $('#txtOrderStartDate').val(sDatePart);
        if (sTimePart != '' && sTimePart.indexOf(':') != -1) {
            var sHours = sTimePart.split(':')[0];
            var sMins = sTimePart.split(':')[1];
            $('#txtOrderStartTimeH').val(sHours);
            $('#txtOrderStartTimeM').val(sMins);
        }
    }

    if (endDate != null && endDate != '') {
        var eDatePart = endDate.split(' ')[0];
        var eTimePart = endDate.split(' ')[1];
        $('#txtOrderEndDate').val(eDatePart);

        if (eTimePart != '' && eTimePart.indexOf(':') != -1) {
            var eHours = eTimePart.split(':')[0];
            var eMins = eTimePart.split(':')[1];
            $('#txtOrderEndTimeH').val(eHours);
            $('#txtOrderEndTimeM').val(eMins);
        }
    }
}

function AddDynamicControls() {
    var frequencyval = $('#ddlFrequencyList').val();
    var orderStartDate = $('#txtOrderStartDate').val();
    var orderEndDate = $('#txtOrderEndDate').val();
    $('#divQuantityMultiple').empty();
    $('#divQuantityMultiple').hide();
    $('#divQuantitySingle').show();
    $('.disabledTxt').removeAttr("disabled");
    if (frequencyval == '2') {
        if (orderEndDate != '') {
            var orderStartDateParts = $("#txtOrderStartDate").val().split(' ')[0].split('/');
            var orderEndDateParts = $("#txtOrderEndDate").val().split(' ')[0].split('/');
            var date1 = new Date(orderStartDateParts[2], orderStartDateParts[1] - 1, orderStartDateParts[0]);
            var date2 = new Date(orderEndDateParts[2], orderEndDateParts[1] - 1, orderEndDateParts[0]);
            var timeDiff = Math.abs(date2.getTime() - date1.getTime());
            var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
            var newHtmlString = "<div class='col-lg-4 labelBold'>";
            if (diffDays > 0) {
                for (var i = 0; i <= diffDays; i++) {
                    var d = new Date(orderStartDateParts[2], orderStartDateParts[1] - 1, orderStartDateParts[0]);
                    var newDate = new Date(d.setDate(d.getDate() + i));
                    newHtmlString += "<div class='col-lg-12 labelBold'>";
                    newHtmlString += "<div class='col-sm-6'>";
                    newHtmlString += "<div class='rowTable'>";
                    newHtmlString += "<label class='labelNormal'></span>" + newDate.format("dd/mm/yyyy") + "</label>";
                    newHtmlString += "</div>";
                    newHtmlString += "</div>";
                    newHtmlString += "<div class='col-sm-6'>";
                    newHtmlString += "<div class='rowTable'>";
                    newHtmlString += "<input type='text' value='' placeholder='Quantity' name='Quantity' maxlength='5' id='txtQuantity" + i + "' class='validate[optional[integer]] emptytxt form-control'>";
                    newHtmlString += "</div>";
                    newHtmlString += "</div>";
                    newHtmlString += "</div>";
                }
                newHtmlString += "</div>";
                $('#divQuantitySingle').hide();
                $('#divQuantityMultiple').show();
                $('#divQuantityMultiple').html(newHtmlString);
                $('#ddlQuantityList').removeClass('validate[required]');
                $("#OpenOrderDiv").validationEngine();
            } else if (diffDays == 0) {
                var d1 = new Date(orderStartDateParts[2], orderStartDateParts[1] - 1, orderStartDateParts[0]);
                var currentDate = new Date(d1.setDate(d1.getDate()));
                newHtmlString += "<div class='col-lg-12 labelBold'>";
                newHtmlString += "<div class='col-sm-6'>";
                newHtmlString += "<div class='rowTable'>";
                newHtmlString += "<label class='labelNormal'></span>" + currentDate.format("dd/mm/yyyy") + "</label>";
                newHtmlString += "</div>";
                newHtmlString += "</div>";
                newHtmlString += "<div class='col-sm-6'>";
                newHtmlString += "<div class='rowTable'>";
                newHtmlString += "<input type='text' value='' placeholder='Quantity' name='Quantity' maxlength='5' id='txtQuantity1' class='validate[optional[integer]] emptytxt form-control'>";
                newHtmlString += "</div>";
                newHtmlString += "</div>";
                newHtmlString += "</div>";
                newHtmlString += "</div>";
                $('#divQuantitySingle').hide();
                $('#divQuantityMultiple').show();
                $('#divQuantityMultiple').html(newHtmlString);
                $('#ddlQuantityList').removeClass('validate[required]');
                $("#OpenOrderDiv").validationEngine();
            } else {
                ShowMessage("Start date should be less than end date.", "Warning", "warning", true);
            }
        }
    }
    else if (frequencyval == '1') {
        $('#txtOrderEndDate').val($('#txtOrderStartDate').val());
        $('#txtOrderEndTimeH').val($('#txtOrderStartTimeH').val());
        $('#txtOrderEndTimeM').val($('#txtOrderStartTimeM').val());
        $('#ddlQuantityList').addClass('validate[required]');
        $('.disabledTxt').attr("disabled", "disabled");
        $("#OpenOrderDiv").validationEngine();
    }
    else if (frequencyval == '3') {

    }
}

function SetValueToOrderingCodesDropdown(selectedValue, item) {
    /// <summary>
    /// Sets the value to ordering codes dropdown.
    /// </summary>
    /// <param name="selectedValue">The selected value.</param>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    var ddlSubCategory = $("#ddlOrderTypeSubCategory");
    var ddlSelector = $("#ddlOrderCodes");
    if (ddlSelector.length == 0 || ddlSubCategory.val() == '' || ddlSubCategory.val() == null) {
        $(ddlSelector).empty();
        $(ddlSelector).html(item);
    } else {
        $(ddlSelector).html(item);
    }
    $(ddlSelector).val(selectedValue);
}


function SetDiagnosisDateRange() {

    setTimeout(function () {
        var startDateObj = $('#txtDRGStartDate').val();
        var enstartDate = new Date(startDateObj).format('mm/dd/yyyy');
        var enstartTimeHours = new Date(startDateObj).format('HH');
        var enstartTimeMins = new Date(startDateObj).format('MM');

        $('#txtDRGStartDate').val(enstartDate);
        $('#txtDRGStartDate').datetimepicker({
            format: 'm/d/Y',
            minDate: '1901/12/12',
            maxDate: '2050/12/12',
            timepicker: false,
            closeOnDateSelect: true
        });

        $('#txtDRGStartTimeHrs').val(enstartTimeHours);
        $('#txtDRGStartTimeMins').val(enstartTimeMins);

    }, 500);
}

/// <var>The add update diagnosis</var>
function AddUpdateDiagnosis(encounterId, patientId, startTime) {
    //$("#txtDRGStartDate").val(startTime);
    //var startDateObj = $('#txtDRGStartDate').val();
    //var enstartDate = new Date(startDateObj).format('mm/dd/yyyy');
    //$('#txtDRGStartDate').val(enstartDate);
    //var enstartTimeHours = new Date(startDateObj).format('HH');
    //var enstartTimeMins = new Date(startDateObj).format('MM');



    $("#hdPatientID2").val(patientId);
    $("#hdCurrentEncounterId1").val(encounterId);
    var jsonData = JSON.stringify({
        EncounterID: encounterId,
        PatientID: patientId
    });
    $('#divOrdersList').hide();
    $('#divAddEditOrder').hide();
    $('#divBillActivitesList').hide();
    $('#RoomChargesDiv').empty();
    $.ajax({
        type: "POST",
        url: '/UploadCharges/DiagnosisCheck',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {


                if (startTime != '') {
                    var datePart = startTime.split(' ')[0];
                    var timePart = startTime.split(' ')[1];

                    $('#txtDRGStartDate').val(datePart);
                    if (timePart != null && timePart != '' && timePart.indexOf(':') != -1) {
                        $('#txtDRGStartTimeHrs').val(timePart.split[0]);
                        $('#txtDRGStartTimeMins').val(timePart.split[1]);
                    }
                }
                $('#diagnosisDiv').empty();
                $('#diagnosisDiv').show();
                $('#diagnosisDiv').html(data);
                $('.diagnosisview').show();
                $('.favdiag').hide();
                //$('#txtDRGStartDate').val(enstartDate);
                //$('#txtDRGStartTimeHrs').val(enstartTimeHours);
                //$('#txtDRGStartTimeMins').val(enstartTimeMins);
                DiagnosisOnReady();
                $('html,body').animate({ scrollTop: $("#collapseDiagnosisAddEdit").offset().top }, 'fast');
            }
            else {
                return false;
            }
        },
        error: function (msg) {
            return true;
        }
    });
};

var customCheck = 0;
/// <var>The view bill activites</var>
var ViewBillActivites = function (billheaderId) {
    var jsonData = JSON.stringify({
        billheaderId: billheaderId,
    });
    customCheck += 1;
    $('#divOrdersList').hide();
    $('#divAddEditOrder').hide();
    $('#diagnosisDiv').hide();
    $('#RoomChargesDiv').hide();
    $.ajax({
        type: "POST",
        url: '/UploadCharges/GetBillDetailsByBillHeaderId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#collapseBillActivitesTwo').addClass('in').attr('style', 'height: auto;');
                $('#divBillActivitesList').show();
                $('#BillActivitesListDiv').empty();
                $('#BillActivitesListDiv').html(data);
                if (customCheck == 1) {
                    //.....This is to check the Grid alignment. Worked so keep it here
                    setTimeout(ViewBillActivites(billheaderId), 500);
                }
            } else {
                return false;
            }
        },
        error: function (msg) {
            return true;
        }
    });
};

var AddManualRoomCharges = function (patientId, encounterId, claimId, aActivityId, startDate, endDate) {
    AddRoomCharges(encounterId, patientId, claimId);
    $("#hdOrderActivityID").val(aActivityId);
    $("#hdOrderClaimID").val(claimId);
    $('#hdEncounterEndDate1').val(startDate);
    $('#hdEncounterStartDate1').val(startDate);
    $("#hdPatientID2").val(patientId);
    $('#ddlOrderStatus').val('3');
    $('#ddlQuantityList').val('1.00');
    $("#ddlFrequencyList1 option[value='2']").remove();
    //$('html,body').animate({ scrollTop: $("#collapseRoomChargesAddEdit").offset().top }, 'slow');
    SetDateRange1();
}

function SetDateRange1() {
    //setTimeout(function () {
    //    $('.disabledTxt').attr("disabled", "disabled");

    //    $('#txtOrderStartDate1').datetimepicker({
    //        format: 'm/d/Y',
    //        minDate: '1901/12/12',
    //        maxDate: '2050/12/12',
    //        timepicker: false,
    //        closeOnDateSelect: true
    //    });
    //    $('#txtOrderEndDate1').datetimepicker({
    //        format: 'm/d/Y',
    //        minDate: '1901/12/12',
    //        maxDate: '2050/12/12',
    //        timepicker: false,
    //        closeOnDateSelect: true
    //    });
    //    $('#txtDRGStartDate').datetimepicker({
    //        format: 'm/d/Y',
    //        minDate: '1901/12/12',
    //        maxDate: '2050/12/12',
    //        timepicker: false,
    //        closeOnDateSelect: true
    //    });

    //    var startDateObj = $('#hdEncounterStartDate1').val();
    //    var endDateObj = $('#hdEncounterEndDate1').val();
    //    var enstartDate = new Date(startDateObj).format('mm/dd/yyyy');
    //    var enendDate = endDateObj != '' ? new Date(endDateObj).format('mm/dd/yyyy') : new Date().format('mm/dd/yyyy');
    //    var enstartTimeHours = new Date(startDateObj).format('HH');
    //    var enstartTimeMins = new Date(startDateObj).format('MM');
    //    var enStartMinsIncreaded = parseInt(enstartTimeMins) + 1;
    //    var enendTimeHours = endDateObj != '' ? new Date(endDateObj).format('HH') : new Date().format('HH');
    //    var enendTimeMins = endDateObj != '' ? new Date(endDateObj).format('MM') : new Date().format('MM');
    //    $('#txtOrderStartDate1').val(enstartDate);
    //    $('#txtOrderEndDate1').val(enendDate);
    //    $('#txtOrderStartTimeH1').val(enstartTimeHours);
    //    $('#txtOrderStartTimeM1').val(enStartMinsIncreaded);
    //    $('#txtOrderEndTimeH1').val(enendTimeHours);
    //    $('#txtOrderEndTimeM1').val(enendTimeMins);
    //}, 500);

    $('.disabledTxt').attr("disabled", "disabled");

    $('#txtOrderStartDate1').datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12',
        maxDate: '2050/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });
    $('#txtOrderEndDate1').datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12',
        maxDate: '2050/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });
    $('#txtDRGStartDate').datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12',
        maxDate: '2050/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });

    var startDate = $('#hdEncounterStartDate1').val();
    var endDate = $('#hdEncounterEndDate1').val();

    if (startDate != null && startDate != "") {
        var sDatePart = startDate.split(' ')[0];
        var sTimePart = startDate.split(' ')[1];
        $('#txtOrderStartDate1').val(sDatePart);
        if (sTimePart != '' && sTimePart.indexOf(':') != -1) {
            var sHours = sTimePart.split(':')[0];
            var sMins = sTimePart.split(':')[1];
            $('#txtOrderStartTimeH1').val(sHours);
            $('#txtOrderStartTimeM1').val(sMins);
        }
    }

    if (endDate != null && endDate != '') {
        var eDatePart = endDate.split(' ')[0];
        var eTimePart = endDate.split(' ')[1];
        $('#txtOrderEndDate1').val(eDatePart);

        if (eTimePart != '' && eTimePart.indexOf(':') != -1) {
            var eHours = eTimePart.split(':')[0];
            var eMins = eTimePart.split(':')[1];
            $('#txtOrderEndTimeH1').val(eHours);
            $('#txtOrderEndTimeM1').val(eMins);
        }
    }
    $('html,body').animate({ scrollTop: $("#collapseDiagnosisAddEdit").offset().top }, 'fast');

}

function AddRoomCharges(encounterId, patientId, claimId) {
    $("#hdPatientID2").val(patientId);
    $("#hdCurrentEncounterId1").val(encounterId);
    var jsonData = JSON.stringify({
        EncounterID: encounterId,
        claimId: claimId
    });
    $('#divOrdersList').hide();
    $('#divAddEditOrder').hide();
    $('.diagnosisview').hide();
    $.ajax({
        type: "POST",
        url: '/UploadCharges/GetRoomChargesByEncounterId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#RoomChargesDiv').empty();
                $('#RoomChargesDiv').show();
                $('#RoomChargesDiv').html(data);
                BindGlobalCodesWithValueWithOrder("#ddlQuantityList1", 1011, "#hdQuantity1");
                BindGlobalCodesWithValue("#ddlFrequencyList1", 1025, "#hdFrequencyCode1");
                BindServiceCodes('#ddlServiceCodes');
                $('#ddlFrequencyList1').val('1');
                // $('#ddlFrequencyList1').attr('disabled', 'disabled');
                AddDynamicControls();
                //$('html,body').animate({ scrollTop: $("#collapseDiagnosisAddEdit").offset().top }, 'fast');
                $('html,body').animate({ scrollTop: $("#collapseRoomChargesAddEdit").offset().top }, 'slow');
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

//---- Diagnosis Section methods Starts
function DiagnosisOnReady() {
    debugger;
    $("#diagnosisAddEdit").validationEngine();
    var isPrimary = $("#hdIsPrimary").val();
    SetValueInDiagnosisType(isPrimary);
    $(".ddlType1").prop('disabled', 'disabled');
    $('.AddAsDiagnosis').show();
    $('.RemoveDiagnosis').hide();

    var isMajorCPTEntered = $("#hdIsMajorCPTEntered").val();
    DisableCPTPanelInDiagnosis(isMajorCPTEntered);
    if ($('#DiagnosisGrid tr').length > 4) {
        $('#DiagnosisGrid').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
    }
}

function SelectDiagnosisCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#primaryDiagnosisCode").val(dataItem.Menu_Title);
    $("#hdprimaryCodeId").val(dataItem.ID);
    $("#hdprimaryCodeValue").val(dataItem.Name);
}

function ClearAll() {
    $("#diagnosisAddEdit").clearForm();
    $.validationEngine.closePrompt(".formError", true);
    var hdPatientId = $("#hdPatientId").val();
    BindDiagnosisList(hdPatientId);
    $('.btnSave').val('Save');
    $("#hdDiagnosisID").val('');
    $("#rdReviewedByPhysician").prop("checked", "checked");
    $("#rdIntiallyEnteredBy").prop("checked", "checked");
    $('#collapseDiagnosisAddEdit').removeClass('in');
    $('#collapseDiagnosisList').addClass('in');
    var isPrimary = $("#hdIsPrimary").val();
    SetValueInDiagnosisType(isPrimary);

    var isMajorCPTEntered = $("#hdIsMajorCPTEntered").val();
    DisableCPTPanelInDiagnosis(isMajorCPTEntered);

    var isMajorDRGEntered = $("#hdIsMajorDRGEntered").val();
    DisableCPTPanelInDiagnosis(isMajorDRGEntered);
    $(".favdiag").hide();
}

function BindDiagnosisList(patientId) {
    var jsonData = JSON.stringify({
        patientId: patientId,
        encounterId: $("#hdEncounterId").val()
    });
    $.ajax({
        type: "POST",
        url: '/Diagnosis/GetUploadChargesCurrentDiagnosisList',
        async: false,
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
                SetGridPaging('?', '?patientId=' + patientId + '&');
            }
        },
        error: function (msg) {
        }
    });
}

function SetValueInDiagnosisType(isPrimary) {
    if (isPrimary == 1) {
        $(".ddlType1").val(2);
        $(".ddlType1").prop('disabled', 'disabled');
        $("#DRGCodesInDiagnosisDiv").prop('disabled', false);      //Enable DRG Div
    }
    else {
        $(".ddlType1").val(1);
        $(".ddlType1").attr('disabled', false);
        $("#DRGCodesInDiagnosisDiv").prop('disabled', 'disabled');      //Disable DRG Div
    }
    var isMajorCPTEntered = $("#hdIsMajorCPTEntered").val();
    DisableCPTPanelInDiagnosis(isMajorCPTEntered);

    var isMajorDRGEntered = $("#hdIsMajorDRGEntered").val();
    DisableDRGPanelInDiagnosis(isMajorDRGEntered);
}


function AddDiagnosisData(id) {
    var isValid = $("#diagnosisAddEdit").validationEngine({ returnIsValid: true });
    var hdEncounterId = $("#hdEncounterId").val() == "" ? $("#hdCurrentEncounterId").val() : $("#hdEncounterId").val();
    if (hdEncounterId == '' || hdEncounterId == null) {
        ShowMessage("Encounter not started yet, Unable to add diagnosis!", "Warning", "warning", true);
        return false;
    }
    var DRGCodeId = $("#hdDrgCodeID").val();
    var CPTCodeID = $("#hdCPTCodeValue").val();
    if (DRGCodeId > 0 || CPTCodeID != '') {
        isValid = true;
    }
    if (isValid == true) {
        id = $("#hdDiagnosisID").val();
        var txtNotes = $("#txtNotes").val();
        var hdCodeId = $("#hdprimaryCodeId").val();
        var primaryDiagnosisCode = $("#hdprimaryCodeId").val();
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
        //var DRGCodeId = $("#hdDrgCodeID").val();
        var DrgCodeValue = $("#hdDrgCodeValue").val();
        if (DRGCodeId > 0) {
            rdIntiallyEnteredBy = 0;
        }
        var dateArray = $("#txtDRGStartDate").val().split('/');
        var newCreatedDate = dateArray[0] + "/" + dateArray[1] + "/" + dateArray[2] + " " + $("#txtDRGStartTimeHrs").val() + ":" + $("#txtDRGStartTimeMins").val();
        if (hdEncounterId != '' && hdEncounterId > 0) {
            //if (txtNotes != '' || primaryDiagnosisCode != 'Enter Diagnosis Code...') {
            var jsonData = JSON.stringify({
                DiagnosisID: id,
                DiagnosisType: diagnosisType,
                CorporateID: corporateId,
                FacilityID: facilityId,
                PatientID: hdPatientId,
                EncounterID: hdEncounterId,
                DiagnosisCodeId: hdCodeId,
                DiagnosisCode: hdCodeId, //primaryDiagnosisCode,
                DiagnosisCodeDescription: txtPrimaryDiagnosisDescription,
                Notes: txtNotes,
                InitiallyEnteredByPhysicianId: rdIntiallyEnteredBy,
                ReviewedByCoderID: rdReviewedByCoder,
                ReviewedByPhysicianID: rdReviewedByPhysician,
                MedicalRecordNumber: hdMedicalRecordNumber,
                CreatedBy: hdCreatedBy,
                CreatedDate: newCreatedDate,//new Date(hdCreatedDate),
                IsDeleted: false,
                DRGCodeID: DRGCodeId,
                MajorCPTCodeId: CPTCodeID
            });
            $.ajax({
                type: "POST",
                url: '/Diagnosis/SaveDiagnosisCustomCode',
                //async: false,
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

                            AddUpdateMaunalChargesAuditLog(jsonData);

                            ClearAll(hdPatientId);
                            $("#hdDiagnosisID").val('');
                            var msg = "Records Saved successfully !";
                            if (id > 0)
                                msg = "Records updated successfully";

                            $(".ddlType1").prop('disabled', 'disabled');
                            ShowMessage(msg, "Success", "success", true);
                            $('.favdiag').hide();
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
    //}
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
                BindDiagnosisDetailsInUploadCharges(data);
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
                BindDiagnosisDetailsInUploadCharges(data);
            }
        },
        error: function (msg) {
        }
    });
}

function BindDiagnosisDetailsInUploadCharges(data) {
    //var txtNotes = $("#txtNotes").val(data.Notes);
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
    $("#hdEncounterId").val(data.eId);
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
        DisableDRGPanelInDiagnosis(1);
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
        DisableCPTPanelInDiagnosis("True");
        //$("#CPTCodesInDiagnosisDiv").prop('disabled', false);
    }


    if ($("#hdDiagnosisID").val() === '' || $("#hdDiagnosisID").val() === '0') {
        $('.btnSave').val('Save');
    } else {
        $('.btnSave').val('Update');
    }

    //var diagnosisStart = $("#txtDRGStartDate").val().split(' ')[0].split('/');
    //var diagnosisStartDate = diagnosisStart[0] + "/" + diagnosisStart[1] + "/" + diagnosisStart[2];
    //var txtDiagnosisStartDate = diagnosisStartDate;



    //$('#collapseDiagnosisList').removeClass('in');
    $('#collapseDiagnosisAddEdit').addClass('in');
    $(".ddlType1").prop('disabled', 'disabled');
    var createdDate = data.CreatedDate.split(' ');
    /*Change 12 hr format to 24 hr format*/

    //var time = Change12HrTo24HrFormat(createdDate[1] + " " + createdDate[2]);
    //var diagnosisStart = createdDate[0].split('/');
    //var customMonth = diagnosisStart[0].length == 1 ? "0" + diagnosisStart[0] : diagnosisStart[0];
    //var txtDiagnosisStartDate = diagnosisStart[1] + "/" + customMonth + "/" + diagnosisStart[2];
    //$("#txtDRGStartDate").val(txtDiagnosisStartDate);
    //$("#txtDRGStartTimeHrs").val(time.split(':')[0]);
    //$("#txtDRGStartTimeMins").val(time.split(':')[1]);

    var datePart = createdDate[0];
    var timePart = createdDate[1].split(':');
    var hours = timePart[0];
    var mins = timePart[1];
    $("#txtDRGStartDate").val(datePart);
    $("#txtDRGStartTimeHrs").val(hours);
    $("#txtDRGStartTimeMins").val(mins);

    InitializeDateTimePicker();
    //$('#txtDRGStartDate').datetimepicker({
    //    format: 'd/m/Y',
    //    minDate: '1901/12/12',
    //    maxDate: '2050/12/12',
    //    timepicker: false,
    //    closeOnDateSelect: true
    //});
}

function ToggleRadioButtonsInDiagnosis(selector) {
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
                BindDiagnosisDetailsInUploadCharges(data);
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

function DisableCPTPanelInDiagnosis(isMajorCpt) {
    if (isMajorCpt == 0) {
        $("#lblMajorCPTInstruction").text("* You have already entered the Major CPT");
        $("#txtCPTCode").prop("disabled", "disabled");
    }
    else {
        $("#lblMajorCPTInstruction").text("* You can search the Major CPT for the code or description.");
        $("#txtCPTCode").removeAttr("disabled");
    }
}

function DisableDRGPanelInDiagnosis(isMajorDrg) {
    if (isMajorDrg == 0) {
        $("#lblMajorDRGInstruction").text("* You have already entered the DRG.");
        $("#txtDRGCode").prop("disabled", "disabled");
    }
    else {
        $("#lblMajorDRGInstruction").text("* You can search the DRG for the code or description.");
        $("#txtDRGCode").removeAttr("disabled");
    }
}

//-----------major CPT-------------

function ShowDiagnosisActions() {
    BindDiagnosisTabData();
    $(".diagnosisActions").show();
    ToggleRadioButtonsInDiagnosis("#rdReviewedByPhysician");
    $("#divDiagnosisReview").attr("disabled", false);
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
//---- Diagnosis Section methods Ends

var DeleteBillActivity = function () {
    if ($("#hfGlobalConfirmFirstId").val() > 0) {
        var jsonData = JSON.stringify({
            billActivityId: $("#hfGlobalConfirmFirstId").val(),
            billheaderId: $("#hfGlobalConfirmedSecondId").val()
        });
        $.ajax({
            type: "POST",
            url: '/UploadCharges/DeleteBillActivity',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    $('#BillActivitesListDiv').empty();
                    $('#BillActivitesListDiv').html(data);
                    ShowMessage("Record Deleted Successfully!", "Success", "success", true);
                }
            },
            error: function (msg) {
            }
        });
    }
}



var AddUpdateMaunalChargesAuditLog = function (jsonData) {
    $.ajax({
        type: "POST",
        url: '/UploadCharges/AddUpdateMaunalChargesAuditLog',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {

            }
        },
        error: function (msg) {
        }
    });
};

/// <var>
/// The maunal charges addition audit log deletion
/// </var>
var MaunalChargesAdditionAuditLogDeletion = function (id, diagnosisType) {
    $.ajax({
        type: "POST",
        url: '/UploadCharges/MaunalChargesAdditionAuditLogDeletion',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ id: id, diagnosisType: diagnosisType }),
        success: function (data) {
            if (data) {

            }
        },
        error: function (msg) {
        }
    });
}

/// <var>The bind service codes</var>
var BindServiceCodes = function (id) {
    $.ajax({
        type: "POST",
        url: "/Home/GetServiceCodesList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, serviceCode) {
                    items += "<option value='" + serviceCode.ServiceCodeValue + "'> ( " + serviceCode.ServiceCodeValue + " ) " + serviceCode.ServiceCodeDescription + "</option>";
                });
                $(id).html(items);
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

var SelectServiceCode = function (e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtServiceCode").val(dataItem.CodeDescription);
    $("#hdAutocompleteOrderCodeId").val(dataItem.CodeDescription);
    $("#hdOrderType").val(dataItem.CodeType);
    $('#CodeTypeValue').html(dataItem.CodeTypeName);
    var items = '<option value="0">--Select--</option>';
    items += "<option value='" + dataItem.Code + "'>" + dataItem.CodeDescription + "</option>";
    SetValueToServiceCodesDropdown(dataItem.Code, items);
    setTimeout(function () {
        $("#ddlService option:contains(" + $('#txtService').val() + ")").attr('selected', 'selected');
    }, 2000);
};

var SetValueToServiceCodesDropdown = function (selectedValue, item) {
    var ddlSelector = $("#ddlServiceCodes");
    if (ddlSelector.length == 0) {
        $(ddlSelector).empty();
        $(ddlSelector).html(item);
    }
    $(ddlSelector).val(selectedValue);
};

function SaveRoomCharges() {

    var patientId = $("#hdPatientID2").val();
    var encounterId = $("#hdCurrentEncounterId1").val();
    //var ddlOrderType = $("#hdOrderTypeId").val();
    var orderCode = $("#ddlServiceCodes").val().trim();
    var hdPrimaryDiagnosisId = $("#hdCurrentDiagnosisID").val();

    var frequency = $("#ddlFrequencyList1").val();
    var txtQuantity = $("#ddlQuantityList1").val();
    var txtOrderNotes = $("#txtOrderNotes1").val();
    //var ddlOrderStatus = $("#ddlOrderStatus").val();

    var orderStartDateParts = $("#txtOrderStartDate1").val().split(' ')[0].split('/');
    var orderStartDate = orderStartDateParts[0] + "/" + orderStartDateParts[1] + "/" + orderStartDateParts[2];
    var txtOrderStartDate = orderStartDate;
    //var orderEndDateParts = $("#txtOrderEndDate1").val().split(' ')[0].split('/');
    //var txtOrderEndDate = new Date(orderEndDateParts[1], orderEndDateParts[2] - 1, orderEndDateParts[0]);

    var txtOrderEndDate = $("#txtOrderEndDate1").val().split(' ')[0];
    var newCustomStartDate = txtOrderStartDate.toString().split('00:00:00')[0] + " " + $('#txtOrderStartTimeH1').val() + ":" + $('#txtOrderStartTimeM1').val();
    var newCustomEndDate = txtOrderEndDate.toString().split('00:00:00')[0] + " " + $('#txtOrderEndTimeH1').val() + ":" + $('#txtOrderEndTimeM1').val();

    var activityId = $("#hdOrderActivityID").val();
    var claimId = $("#hdOrderClaimID").val();
    var jsonData = [];
    if (frequency == '2' || frequency == '3') {

        if (txtOrderEndDate != '') {
            var date1 = new Date(txtOrderStartDate);
            var date2 = new Date(txtOrderEndDate);
            var timeDiff = Math.abs(date2.getTime() - date1.getTime());
            var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
            if (diffDays > 0) {
                for (var i = 0; i <= diffDays; i++) {
                    var d = new Date(txtOrderStartDate);
                    var newDate = new Date(d.setDate(d.getDate() + i));
                    jsonData[i] = {
                        OpenOrderID: 0,
                        OrderType: '8',
                        OrderCode: orderCode,
                        DiagnosisCode: hdPrimaryDiagnosisId,
                        FrequencyCode: frequency,
                        Quantity: txtQuantity,
                        OrderNotes: txtOrderNotes,
                        PeriodDays: 0,
                        OrderStatus: '0',
                        EncounterID: encounterId,
                        PatientID: patientId,
                        IsActive: true,
                        IsDeleted: false,
                        CategoryId: 0,
                        SubCategoryId: 0,
                        StartDate: newDate,
                        EndDate: newDate,
                        ClaimId: claimId,
                        ActivityId: activityId,
                    };
                }
            } else {
                var d1 = new Date(txtOrderStartDate);
                var newDate1 = new Date(d1.setDate(d1.getDate()));
                jsonData[0] = {
                    OpenOrderID: 0,
                    OrderType: '8',
                    OrderCode: orderCode,
                    DiagnosisCode: hdPrimaryDiagnosisId,
                    FrequencyCode: frequency,
                    Quantity: txtQuantity,
                    OrderNotes: txtOrderNotes,
                    PeriodDays: 0,
                    OrderStatus: '0',
                    EncounterID: encounterId,
                    PatientID: patientId,
                    IsActive: true,
                    IsDeleted: false,
                    CategoryId: 0,
                    SubCategoryId: 0,
                    StartDate: newDate1,
                    EndDate: newDate1,
                    ClaimId: claimId,
                    ActivityId: activityId,
                };
            }
        }
    } else {
        jsonData[0] = {
            OpenOrderID: 0,
            OrderType: '8',
            OrderCode: orderCode,
            DiagnosisCode: hdPrimaryDiagnosisId,
            FrequencyCode: frequency,
            Quantity: txtQuantity,
            OrderNotes: txtOrderNotes,
            PeriodDays: 0,
            OrderStatus: '0',
            EncounterID: encounterId,
            PatientID: patientId,
            IsActive: true,
            IsDeleted: false,
            CategoryId: 0,
            SubCategoryId: 0,
            StartDate: newCustomStartDate,
            EndDate: newCustomEndDate,
            ClaimId: claimId,
            ActivityId: activityId,
        };
    }
    var jsonD = JSON.stringify(jsonData);
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/UploadCharges/AddManualRoomCharges",
        data: jsonD,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            if (data == true) {
                var msg = "Records Saved successfully !";
                ShowMessage(msg, "Success", "success", true);
                getRoomCharges(encounterId);
                ResetRoomCharges();
            } else {
                ShowMessage("Unable to add manual charges! Same day charges already exist in the system.", "Warning", "warning", true);
            }
        },
        error: function (msg) {
            ShowMessage("Unable to add manual charges", "Warning", "warning", true);
        }
    });
}

var getRoomCharges = function (encounterid) {
    var claimId = $("#hdOrderClaimID").val();
    var jsonData = JSON.stringify({
        encounterid: encounterid,
        claimId: claimId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/UploadCharges/GetRoomCharges",
        data: jsonData,
        dataType: "html",
        success: function (data) {
            $('#CurrentRoomChargesGrid').empty();
            $('#CurrentRoomChargesGrid').html(data);
        },
        error: function (msg) {
        }
    });
};

var SetTextFieldValue = function () {
    var selectedval = $("#ddlServiceCodes").val();
    var selectedtext = $("#ddlServiceCodes :selected").text();
    $("#txtServiceCode").val(selectedtext);
    var jsonData = JSON.stringify({
        serviceCode: selectedval,
        effectiveDate: $("#txtOrderStartDate1").val()
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/UploadCharges/GetRoomChargesByServiceCode",
        data: jsonData,
        dataType: "json",
        success: function (data) {
            $('#lblServiceCodeCharges').html(data);
        },
        error: function (msg) {
        }
    });
};

var ResetRoomCharges = function () {
    $.validationEngine.closePrompt(".formError", true);
    $('#ddlServiceCodes').val('0');
    $('#txtServiceCode').val('');
    $('#txtOrderNotes1').val('');
    SetDateRange1();
    $('#ddlOrderStatus').val('3');
    $('html,body').animate({ scrollTop: $("#collapsepatientSerach").offset().top }, 'fast');
};

function AddDynamicControls1() {
    var frequencyval = $('#ddlFrequencyList1').val();
    var orderStartDate = $('#txtOrderStartDate1').val();
    var orderEndDate = $('#txtOrderEndDate1').val();
    $('#divQuantitySingle').show();
    $('.disabledTxt').removeAttr("disabled");
    if (frequencyval == '2') {

    }
    else if (frequencyval == '1') {
        $('#txtOrderEndDate1').val($('#txtOrderStartDate1').val());
        $('#txtOrderEndTimeH1').val($('#txtOrderStartTimeH1').val());
        $('#txtOrderEndTimeM1').val($('#txtOrderStartTimeM1').val());
        $('#ddlQuantityList1').addClass('validate[required]');
        $('.disabledTxt').attr("disabled", "disabled");
    }
}

var CheckRoomChargesDateRange = function () {
    var orderStartDateParts = $("#txtOrderStartDate1").val().split(' ')[0].split('/');
    var orderEndDateParts = $("#txtOrderEndDate1").val().split(' ')[0].split('/');

    var orderStartDate = new Date(orderStartDateParts[2], orderStartDateParts[1] - 1, orderStartDateParts[0]);
    var orderEndDate = new Date(orderEndDateParts[2], orderEndDateParts[1] - 1, orderEndDateParts[0]);

    var txtOrderEndDate = new Date(orderEndDate);
    var txtOrderStartDate = new Date(orderStartDate);

    var newCustomStartDate = txtOrderStartDate.toString().split('00:00:00')[0] + " " + $('#txtOrderStartTimeH').val() + ":" + $('#txtOrderStartTimeM').val();
    var newCustomEndDate = txtOrderEndDate.toString().split('00:00:00')[0] + " " + $('#txtOrderEndTimeH').val() + ":" + $('#txtOrderEndTimeM').val();

    var encounterId = $("#hdCurrentEncounterId1").val();
    var jsonData = JSON.stringify({ encounterId: encounterId, orderStartDate: newCustomStartDate, orderEndDate: newCustomEndDate });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/UploadCharges/CheckRoomChargesDateRange",
        dataType: "json",
        async: true,
        data: jsonData,
        success: function (data) {
            if (data == false) {
                ShowMessage("Room charges date range should be in the range of the Encounter Start and end Date .", "Warning", "warning", true);
                return false;
            } else if (data == true) {
                SaveRoomCharges();
                return true;
            } else {
                ShowMessage("Room charges range should be in the range of the Encounter Start and end Date .", "Warning", "warning", true);
                return false;
            }
        },
        error: function (msg) {
        }
    });
};


var DeleteRoomChargesBillActivity = function () {
    if ($("#hfGlobalConfirmFirstId").val() > 0) {
        var jsonData = JSON.stringify({
            billActivityId: $("#hfGlobalConfirmFirstId").val(),
            billheaderId: $("#hfGlobalConfirmedSecondId").val()
        });
        $.ajax({
            type: "POST",
            url: '/UploadCharges/DeleteRoomChargesBillActivity',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    $('#CurrentRoomChargesGrid').empty();
                    $('#CurrentRoomChargesGrid').html(data);
                    ShowMessage("Record Deleted Successfully!", "Success", "success", true);
                }
            },
            error: function (msg) {
            }
        });
    }
}

function DisableEnableQqantity() {
    var frequency = $("#ddlFrequencyList1").val();
    if (frequency == '3') {
        //$("#mydropdownlist").attr('selectedIndex', 2);
        $("#ddlQuantityList1").val("1.00");
        $("#ddlQuantityList1").prop("disabled", true);
    } else {
        $("#ddlQuantityList1").prop("disabled", false);

    }
}


function AddOrderInUploadChargesNotInUse(id) {
    var orderId = id;
    var patientId = $("#hdPatientID2").val();
    var encounterId = $("#hdCurrentEncounterId1").val();
    var ddlOrderType = $("#hdOrderTypeId").val();
    var orderCode = $("#ddlOrderCodes").val().trim();
    var hdPrimaryDiagnosisId = $("#hdCurrentDiagnosisID").length > 0 ? $("#hdCurrentDiagnosisID").val() : "";
    var frequency = $("#ddlFrequencyList").val();
    var txtQuantity = $("#ddlQuantityList").val();
    var txtOrderNotes = $("#txtOrderNotes").val();
    var ddlOrderStatus = $("#ddlOrderStatus").val();
    var ddlOrderTypeCategory = $("#ddlOrderTypeCategory").val().trim();
    var ddlOrderTypeSubCategory = $("#ddlOrderTypeSubCategory").val();
    var orderStartDateParts = $("#txtOrderStartDate").val().split(' ')[0].split('/');
    var orderEndDateParts = $("#txtOrderEndDate ").val().split(' ')[0].split('/');
    var orderStartDate = new Date(orderStartDateParts[2], orderStartDateParts[1] - 1, orderStartDateParts[0]);
    var orderEndDate = new Date(orderEndDateParts[2], orderEndDateParts[1] - 1, orderEndDateParts[0]);
    var txtOrderEndDate = new Date(orderEndDate);
    var txtOrderStartDate = new Date(orderStartDate);

    var newCustomStartDate = txtOrderStartDate.toString().split('00:00:00')[0] + " " + $('#txtOrderStartTimeH').val() + ":" + $('#txtOrderStartTimeM').val();
    var newCustomEndDate = txtOrderEndDate.toString().split('00:00:00')[0] + " " + $('#txtOrderEndTimeH').val() + ":" + $('#txtOrderEndTimeM').val();
    var activityId = $("#hdOrderActivityID").val();
    var claimId = $("#hdOrderClaimID").val();

    var jsonData = [];
    if (frequency != '0') {
        var date1 = new Date(txtOrderStartDate);
        var date2 = new Date(txtOrderEndDate);
        if (frequency == '2') {
            if (txtOrderEndDate != '') {
                var timeDiff = Math.abs(date2.getTime() - date1.getTime());
                var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
                if (diffDays > 0) {
                    for (var i = 0; i <= diffDays; i++) {
                        var d = new Date(txtOrderStartDate);
                        var qunatitySelected = $('#txtQuantity' + i).val();
                        var newDate = new Date(d.setDate(d.getDate() + i));
                        if (qunatitySelected != '') {
                            jsonData[i] = {
                                OpenOrderID: orderId,
                                OrderType: ddlOrderType,
                                OrderCode: orderCode,
                                DiagnosisCode: hdPrimaryDiagnosisId,
                                FrequencyCode: frequency,
                                Quantity: qunatitySelected,
                                OrderNotes: txtOrderNotes,
                                PeriodDays: 0,
                                OrderStatus: ddlOrderStatus,
                                EncounterID: encounterId,
                                PatientID: patientId,
                                IsActive: true,
                                IsDeleted: false,
                                CategoryId: ddlOrderTypeCategory,
                                SubCategoryId: ddlOrderTypeSubCategory,
                                StartDate: newDate,
                                EndDate: newDate,
                                ClaimId: claimId,
                                ActivityId: activityId,
                            };
                        }
                    }
                } else {
                    var d1 = new Date(txtOrderStartDate);
                    var qunatitySelected1 = $('#txtQuantity1').val();
                    var newDate1 = new Date(d1.setDate(d1.getDate()));
                    jsonData[0] = {
                        OpenOrderID: orderId,
                        OrderType: ddlOrderType,
                        OrderCode: orderCode,
                        DiagnosisCode: hdPrimaryDiagnosisId,
                        FrequencyCode: frequency,
                        Quantity: qunatitySelected1,
                        OrderNotes: txtOrderNotes,
                        PeriodDays: 0,
                        OrderStatus: ddlOrderStatus,
                        EncounterID: encounterId,
                        PatientID: patientId,
                        IsActive: true,
                        IsDeleted: false,
                        CategoryId: ddlOrderTypeCategory,
                        SubCategoryId: ddlOrderTypeSubCategory,
                        StartDate: newDate1,
                        EndDate: newDate1,
                        ClaimId: claimId,
                        ActivityId: activityId,
                    };
                }
            }
        } else if (frequency == '3') {
            var timeDiff1 = Math.abs(date2.getTime() - date1.getTime());
            var diffDays1 = Math.ceil(timeDiff1 / (1000 * 3600 * 24));
            var d3 = txtOrderStartDate;
            if (diffDays1 > 0) {
                for (var k = 0; k <= diffDays1; k++) {
                    var qunatitySelected3 = txtQuantity;
                    var newDate3 = new Date(d3.setDate(d3.getDate() + k));
                    if (qunatitySelected != '') {
                        jsonData[k] = {
                            OpenOrderID: orderId,
                            OrderType: ddlOrderType,
                            OrderCode: orderCode,
                            DiagnosisCode: hdPrimaryDiagnosisId,
                            FrequencyCode: frequency,
                            Quantity: qunatitySelected3,
                            OrderNotes: txtOrderNotes,
                            PeriodDays: 0,
                            OrderStatus: ddlOrderStatus,
                            EncounterID: encounterId,
                            PatientID: patientId,
                            IsActive: true,
                            IsDeleted: false,
                            CategoryId: ddlOrderTypeCategory,
                            SubCategoryId: ddlOrderTypeSubCategory,
                            StartDate: newDate3,
                            EndDate: newDate3,
                            ClaimId: claimId,
                            ActivityId: activityId,
                        };
                    }
                }
            } else {
                var qunatitySelected4 = txtQuantity;
                var newDate4 = new Date(txtOrderStartDate.setDate(txtOrderStartDate.getDate()));
                jsonData[0] = {
                    OpenOrderID: orderId,
                    OrderType: ddlOrderType,
                    OrderCode: orderCode,
                    DiagnosisCode: hdPrimaryDiagnosisId,
                    FrequencyCode: frequency,
                    Quantity: qunatitySelected4,
                    OrderNotes: txtOrderNotes,
                    PeriodDays: 0,
                    OrderStatus: ddlOrderStatus,
                    EncounterID: encounterId,
                    PatientID: patientId,
                    IsActive: true,
                    IsDeleted: false,
                    CategoryId: ddlOrderTypeCategory,
                    SubCategoryId: ddlOrderTypeSubCategory,
                    StartDate: newDate4,
                    EndDate: newDate4,
                    ClaimId: claimId,
                    ActivityId: activityId,
                };
            }
        } else {
            jsonData[0] = {
                OpenOrderID: orderId,
                OrderType: ddlOrderType,
                OrderCode: orderCode,
                DiagnosisCode: hdPrimaryDiagnosisId,
                FrequencyCode: frequency,
                Quantity: txtQuantity,
                OrderNotes: txtOrderNotes,
                PeriodDays: 0,
                OrderStatus: ddlOrderStatus,
                EncounterID: encounterId,
                PatientID: patientId,
                IsActive: true,
                IsDeleted: false,
                CategoryId: ddlOrderTypeCategory,
                SubCategoryId: ddlOrderTypeSubCategory,
                StartDate: newCustomStartDate,
                EndDate: newCustomEndDate,
                ClaimId: claimId,
                ActivityId: activityId,
            };
        }
        var jsonD = JSON.stringify(jsonData);
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/UploadCharges/AddManualOrders",
            data: jsonD,
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                if (data == true) {
                    var msg = "Records Saved successfully !";
                    if (id > 0) {
                        msg = "Records updated successfully";
                    }
                    ShowMessage(msg, "Success", "success", true);
                } else {
                    ShowMessage("Unable to add manual charges", "Warning", "warning", true);
                }
                ClearPhysicianOrderAllInUploadCharges(encounterId);
            },
            error: function (msg) {
                ShowMessage("Unable to add manual charges", "Warning", "warning", true);
            }
        });
    }
}

function AddOrderInUploadCharges(id) {
    var orderId = id;
    var patientId = $("#hdPatientID2").val();
    var encounterId = $("#hdCurrentEncounterId1").val();
    var ddlOrderType = $("#hdOrderTypeId").val();
    var orderCode = $("#ddlOrderCodes").val().trim();
    var hdPrimaryDiagnosisId = $("#hdCurrentDiagnosisID").length > 0 ? $("#hdCurrentDiagnosisID").val() : "";
    var frequency = parseInt($("#ddlFrequencyList").val());
    var txtQuantity = $("#ddlQuantityList").val();
    var txtOrderNotes = $("#txtOrderNotes").val();
    var ddlOrderStatus = $("#ddlOrderStatus").val();
    var ddlOrderTypeCategory = $("#ddlOrderTypeCategory").val().trim();
    var ddlOrderTypeSubCategory = $("#ddlOrderTypeSubCategory").val();
    var startDate = $("#txtOrderStartDate").val() + " " + $('#txtOrderStartTimeH').val() + ":" + $('#txtOrderStartTimeM').val();
    var endDate = $("#txtOrderEndDate").val() + " " + $('#txtOrderEndTimeH').val() + ":" + $('#txtOrderEndTimeM').val();
    var activityId = $("#hdOrderActivityID").val();
    var claimId = $("#hdOrderClaimID").val();

    var jsonData = [];
    if (frequency > 0) {
        var date1 = new Date(startDate);
        var date2 = new Date(endDate);
        var timeDiff = Math.abs(date2.getTime() - date1.getTime());
        var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
        if (diffDays > 0) {
            for (var i = 0; i <= diffDays; i++) {
                var qunatitySelected = txtQuantity > 0 && frequency == 2 ? $('#txtQuantity' + i).val() : txtQuantity;
                //var d = new Date(startDate);
                //var newDate = new Date(d.setDate(d.getDate() + i));
                var newDate = DateFromString(startDate, i);
                if (qunatitySelected != '') {
                    jsonData[i] = {
                        OpenOrderID: orderId,
                        OrderType: ddlOrderType,
                        OrderCode: orderCode,
                        DiagnosisCode: hdPrimaryDiagnosisId,
                        FrequencyCode: 10,          //10 Frequency means Immediate. 
                        Quantity: qunatitySelected,
                        OrderNotes: txtOrderNotes,
                        PeriodDays: 0,
                        OrderStatus: ddlOrderStatus,
                        EncounterID: encounterId,
                        PatientID: patientId,
                        IsActive: true,
                        IsDeleted: false,
                        CategoryId: ddlOrderTypeCategory,
                        SubCategoryId: ddlOrderTypeSubCategory,
                        StartDate: newDate,
                        EndDate: newDate,
                        ClaimId: claimId,
                        ActivityId: activityId,
                        IsApproved: true,
                    };
                }
            }
        }
        else {
            //var d1 = new Date(startDate);
            var singleQuantity = txtQuantity > 0 && frequency == 2 ? $('#txtQuantity1').val() : txtQuantity;
            //var newDate1 = new Date(d1.setDate(d1.getDate()));
            jsonData[0] = {
                OpenOrderID: orderId,
                OrderType: ddlOrderType,
                OrderCode: orderCode,
                DiagnosisCode: hdPrimaryDiagnosisId,
                FrequencyCode: 10,
                Quantity: singleQuantity,
                OrderNotes: txtOrderNotes,
                PeriodDays: 0,
                OrderStatus: ddlOrderStatus,
                EncounterID: encounterId,
                PatientID: patientId,
                IsActive: true,
                IsDeleted: false,
                CategoryId: ddlOrderTypeCategory,
                SubCategoryId: ddlOrderTypeSubCategory,
                //StartDate: newDate1,
                //EndDate: newDate1,
                StartDate: startDate,
                EndDate: startDate,
                ClaimId: claimId,
                ActivityId: activityId,
                IsApproved: true,
            };
        }

        var jsonD = JSON.stringify(jsonData);
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/UploadCharges/AddManualOrders",
            data: jsonD,
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                if (data == true) {
                    var msg = "Records Saved successfully !";
                    if (id > 0) {
                        msg = "Records updated successfully";
                    }
                    ShowMessage(msg, "Success", "success", true);
                } else {
                    ShowMessage("Unable to add manual charges", "Warning", "warning", true);
                }
                ClearPhysicianOrderAllInUploadCharges(encounterId);
            },
            error: function (msg) {
                ShowMessage("Unable to add manual charges", "Warning", "warning", true);
            }
        });
    }
}

function OnChangeFrequency() {
    var frequencyval = $('#ddlFrequencyList').val();
    var startDate = $("#txtOrderStartDate").val() + " " + $('#txtOrderStartTimeH').val() + ":" + $('#txtOrderStartTimeM').val();
    var endDate = $("#txtOrderEndDate").val() + " " + $('#txtOrderEndTimeH').val() + ":" + $('#txtOrderEndTimeM').val();
    $('#divQuantityMultiple').empty();
    $('#divQuantityMultiple').hide();
    $('#divQuantitySingle').show();
    $('.disabledTxt').removeAttr("disabled");
    if (frequencyval == '2' && endDate != '') {
        var date1 = new Date(startDate);
        var date2 = new Date(endDate);
        var timeDiff = Math.abs(date2.getTime() - date1.getTime());
        var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
        var newHtmlString = "<div class='col-lg-4 labelBold'>";
        if (diffDays > 0) {
            for (var i = 0; i <= diffDays; i++) {
                var newDate = new Date(startDate);
                newDate.setDate(newDate.getDate() + i);
                newHtmlString += "<div class='col-lg-12 labelBold'>";
                newHtmlString += "<div class='col-sm-6'>";
                newHtmlString += "<div class='rowTable'>";
                newHtmlString += "<label class='labelNormal'></span>" + newDate.format("mm/dd/yyyy") + "</label>";
                newHtmlString += "</div>";
                newHtmlString += "</div>";
                newHtmlString += "<div class='col-sm-6'>";
                newHtmlString += "<div class='rowTable'>";
                newHtmlString += "<input type='text' value='' placeholder='Quantity' name='Quantity' maxlength='5' id='txtQuantity" + i + "' class='validate[optional[integer]] emptytxt form-control'>";
                newHtmlString += "</div>";
                newHtmlString += "</div>";
                newHtmlString += "</div>";
            }
            newHtmlString += "</div>";
            $('#divQuantitySingle').hide();
            $('#divQuantityMultiple').show();
            $('#divQuantityMultiple').html(newHtmlString);
            $('#ddlQuantityList').removeClass('validate[required]');
            $("#OpenOrderDiv").validationEngine();
        } else if (diffDays == 0) {
            var newDate1 = new Date(startDate);
            newHtmlString += "<div class='col-lg-12 labelBold'>";
            newHtmlString += "<div class='col-sm-6'>";
            newHtmlString += "<div class='rowTable'>";
            newHtmlString += "<label class='labelNormal'></span>" + newDate1.format("mm/dd/yyyy") + "</label>";
            newHtmlString += "</div>";
            newHtmlString += "</div>";
            newHtmlString += "<div class='col-sm-6'>";
            newHtmlString += "<div class='rowTable'>";
            newHtmlString += "<input type='text' value='' placeholder='Quantity' name='Quantity' maxlength='5' id='txtQuantity1' class='validate[optional[integer]] emptytxt form-control'>";
            newHtmlString += "</div>";
            newHtmlString += "</div>";
            newHtmlString += "</div>";
            newHtmlString += "</div>";
            $('#divQuantitySingle').hide();
            $('#divQuantityMultiple').show();
            $('#divQuantityMultiple').html(newHtmlString);
            $('#ddlQuantityList').removeClass('validate[required]');
            $("#OpenOrderDiv").validationEngine();
        } else {
            ShowMessage("Start date should be less than end date.", "Warning", "warning", true);
        }
    }
    else {
        if (frequencyval == 1) {
            $('#txtOrderEndDate').val($('#txtOrderStartDate').val());
            $('#txtOrderEndTimeH').val($('#txtOrderStartTimeH').val());
            $('#txtOrderEndTimeM').val($('#txtOrderStartTimeM').val());
        }
        $('#ddlQuantityList').addClass('validate[required]');
        $('.disabledTxt').attr("disabled", "disabled");
        $("#OpenOrderDiv").validationEngine();
    }
}


function GetPatientResultsFromPatientId(patientId, eId, bhId) {
    $.ajax({
        type: "POST",
        url: '/UploadCharges/GetPatientResultByPatientId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ patientId: patientId, encounterId: eId, billHeaderId: bhId }),
        success: function (data) {
            $('#divPatientSearch').show();
            $('#PatientSearchResultDiv').empty();
            $('#PatientSearchResultDiv').html(data);
            $("#collapsePatientSearch").removeClass("in");
        },
        error: function (msg) {
        }
    });
}


function DateFromString(completedDate, i) {
    var datePart = completedDate.split(' ')[0];
    var str = datePart.split(/\D+/);
    str = new Date(str[2], str[0] - 1, (parseInt(str[1]) + i));
    var datePartUpdated = GetDateWithAddedDays(str);
    return datePartUpdated + " " + completedDate.split(' ')[1];
}


function GetDateWithAddedDays(str) {
    var ndateArr = str.toString().split(' ');
    var Months = 'Jan Feb Mar Apr May Jun Jul Aug Sep Oct Nov Dec';
    var mValueInt = (Months.indexOf(ndateArr[1]) / 4) + 1;
    var mValueStr = mValueInt > 9 ? mValueInt.toString() : "0" + mValueInt.toString();
    return mValueStr + '/' + ndateArr[2] + '/' + ndateArr[3];
}

var ViewVirtualDischargePopup = function (patientId, eId, bhId) {
    $.ajax({
        type: "POST",
        url: '/UploadCharges/GetBillVirtualDischargeDetails',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ patientId: patientId, encounterId: eId, billHeaderId: bhId }),
        success: function (data) {

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


function OnChangeSubCatgory(ddlSelector) {
    $("#ddlDosageForm").removeClass("validate[required]");
    $("#ddlDosageAmount").removeClass("validate[required]");
    $(".DrugDDL").hide();


    var selector = $("#ddlOrderTypeSubCategory :selected");
    if (selector.length > 0 && selector.val() > 0) {
        var subCategoryValue = selector.val();
        var gccValue = selector.attr('gcc');
        var oct = selector.attr('oct') == "" ? "5" : selector.attr('oct');
        var startRange = parseInt(selector.attr('sr') == "" ? 0 : selector.attr('sr'));
        var endRange = parseInt(selector.attr('er') == "" ? 0 : selector.attr('er'));

        jsonData = {
            subCategoryValue: subCategoryValue,
            gcc: gccValue,
            orderCodeType: oct,
            startRange: startRange == null ? 0 : startRange,
            endRange: endRange == null ? 0 : endRange,
        };

        var orderCategory = $("#ddlOrderTypeCategory :selected").text();

        if (orderCategory == "LAB Test") {
            var items = '<option value="0">--Select--</option>';
            var newItem = "<option id='" +
                $("#ddlOrderTypeCategory").val() +
                "'  value='" +
                $("#ddlOrderTypeCategory").val() +
                "'>" +
                $("#ddlOrderTypeCategory :selected").text() +
                "</option>";
            items += newItem;
            $("#ddlOrderCodes").html(items);
            //Set Order Type Code Name
            $("#CodeTypeValue").text("LAB Test");
            //Set Order Type Code Id
            $("#hdOrderTypeId").val("11");
        }
        else {
            $.getJSON(summaryPageUrl + "GetCodesBySubCategory", jsonData, function (data) {
                if (data != null) {
                    //Set Order Type Code Name
                    $("#CodeTypeValue").text(data.codeTypeName);
                    //Set Order Type Code Id
                    $("#hdOrderTypeId").val(data.codeTypeId);

                    BindDropdownData(data.codeList, "#ddlOrderCodes", "#hdOrderCodeId");
                    $('#collapseOpenOrderAddEdit').addClass('in');

                    if ($("#ddlOrderCodes").val() != "0") {
                        $("#txtOrderCode").val($("#ddlOrderCodes :selected").text());
                    }
                }
            });
        }
    }
}