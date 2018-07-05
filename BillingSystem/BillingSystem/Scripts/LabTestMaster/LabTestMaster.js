$(function () {
    LabTestRangeLoad();
});
function LabTestRangeLoad() {
    $("#LabTestAddEditFormDiv").validationEngine();
    $(".menu").click(function () {
        $(".out").toggleClass("in");
    });

    BindGlobalCodesWithValue("#ddlLabTestTypeCategory", 11080, "#hdGlobalCodeValue");
    BindGlobalCodesWithValue('#ddlLabTestUOM', 3101, '#hdExternalValue3');
}

function CheckDuplicateRecord() {
    var name = $("#txtAllergryName").val().trim();
    var globalCodeId = $("#hdLabTestId").val();
    if (globalCodeId == '')
        globalCodeId = 0;

    var isValid = jQuery("#LabTestAddEditFormDiv").validationEngine({ returnIsValid: true });
    if (isValid) {
        var jsonData = JSON.stringify({
            GlobalCodeName: name,
            GlobalCodeId: '2308',
            GlobalCodeCategoryValue: globalCodeCategory
        });

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/GlobalCode/CheckDuplicateSubCategory",
            data: jsonData,
            dataType: "",
            beforeSend: function () { },
            success: function (data) {
                //Append Data to grid
                if (data) {
                    ShowMessage("LabTest already exists. Try adding with different name!", "Alert", "info", true);
                }
                else {
                    AddLabTest();
                    return true;
                }
            },
            error: function (msg) {
            }
        });
        return false;
    }
    else {
        AddLabTest();
    }
    return false;
}

function AddLabTest(id) {
    var isValid = jQuery("#LabTestAddEditFormDiv").validationEngine({ returnIsValid: true });
    if (isValid) {
        var cptCodeId = id == 0 ? $('#ddlLabTestName').val() : id;// $("#hdLabTestId").val();()
        var description = $("#txtDescription").val();
        //var isActive = false;
        //if ($('#chkIsActive').is(':checked'))
        //    isActive = true;
        var txtExternalValue1 = $("#txtExternalValue1").val();
        var txtExternalValue2 = $("#txtExternalValue2").val();
        var txtExternalValue3 = $("#ddlLabTestUOM").val();

        var hdCodeTableNumber = $("#hdCodeTableNumber").val();
        var hdCodeTableDescription = $("#hdCodeTableDescription").val();
        var hdCodeNumbering = $("#hdCodeNumbering").val();
        var hdCodePrice = $("#hdCodePrice").val();
        var hdCodeAnesthesiaBaseUnit = $("#hdCodeAnesthesiaBaseUnit").val();
        var hdCodeEffectiveDate = $("#hdCodeEffectiveDate").val();
        var hdCodeExpiryDate = $("#hdCodeExpiryDate").val();
        var hdCodeBasicProductApplicationRule = $("#hdCodeBasicProductApplicationRule").val();
        var hdCodeOtherProductsApplicationRule = $("#hdCodeOtherProductsApplicationRule").val();

        var hdCodeServiceMainCategory = $("#hdCodeServiceMainCategory").val();
        var hdCodeServiceCodeSubCategory = $("#hdCodeServiceCodeSubCategory").val();
        var hdCodeUSCLSChapter = $("#hdCodeUSCLSChapter").val();
        var hdCodeCPTMUEValues = $("#hdCodeCPTMUEValues").val();
        var hdCodeGroup = $("#hdCodeGroup").val();
        var hdCreatedBy = $("#hdCreatedBy").val();
        var hdCreatedDate =new Date($("#hdCreatedDate").val());
        var hdDeletedDate =new Date($("#hdDeletedDate").val());
        var hdCTPCodeRangeValue = $("#hdCTPCodeRangeValue").val();
        var hdIsActive = $("#hdIsActive").val();

        var jsonData = JSON.stringify({
            CPTCodesId: cptCodeId,
            CodeTableNumber : hdCodeTableNumber,
            CodeTableDescription: hdCodeTableDescription,
            CodeNumbering: hdCodeNumbering,
            CodePrice: hdCodePrice,
            CodeAnesthesiaBaseUnit: hdCodeAnesthesiaBaseUnit,
            CodeEffectiveDate: hdCodeEffectiveDate,
            CodeExpiryDate: hdCodeExpiryDate,
            CodeBasicProductApplicationRule: hdCodeBasicProductApplicationRule,
            CodeOtherProductsApplicationRule: hdCodeOtherProductsApplicationRule,
            CodeServiceMainCategory: hdCodeServiceMainCategory,
            CodeServiceCodeSubCategory: hdCodeServiceCodeSubCategory,
            CodeUSCLSChapter: hdCodeUSCLSChapter,
            CodeCPTMUEValues: hdCodeCPTMUEValues,
            CodeGroup: hdCodeGroup,
            CreatedBy: hdCreatedBy,
            CreatedDate: hdCreatedDate,
            DeletedDate: hdDeletedDate,
            CTPCodeRangeValue: hdCTPCodeRangeValue,
            CodeDescription : description,
            IsActive: hdIsActive,
            ExternalValue1: txtExternalValue1,
            ExternalValue2: txtExternalValue2,
            ExternalValue3: txtExternalValue3,
        });
        var msg = "";
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/LabTestMaster/AddUpdateGlobalCode",
            data: jsonData,
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                //Append Data to grid
                if (data != null) {
                    ClearLabTestFields();
                    msg = "Record Saved successfully !";
                    if (globalCodeId > 0)
                        msg = "Record updated successfully";
                    ShowMessage(msg, "Success", "success", true);
                    return true;
                }
            },
            error: function (msg) {

            }
        });
    }
    else {
        return false;
    }
}

function EditCurrentLabTest(globalCodeId) {
    if (globalCodeId > 0) {
        var jsonData = JSON.stringify({ id: globalCodeId });
        $.ajax({
            type: "POST",
            url: '/LabTestMaster/GetCurrentLabTest',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data != null) {
                    $("#LabTestAddEditFormDiv").empty();
                    $("#LabTestAddEditFormDiv").html(data);
                    LabTestRangeLoad();
                    bindLabTests();
                    $("#LabTestAddEditFormDiv").validationEngine();
                    $('#collapseLabTestMasterAddEdit').addClass('in');
                }
            },
            error: function (msg) {
            }
        });
    }
}


function DeleteLabTestRecord() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var url = '/LabTestMaster/DeleteLabTest';
        $.ajax({
            type: "POST",
            url: url,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({
                globalCodeId: $("#hfGlobalConfirmId").val()
            }),
            success: function (data) {
                if (data != null) {
                    ShowMessage("Record deleted successfully", "Alert", "info", true);
                    ClearLabTestFields();
                }
                else {
                }
            },
            error: function (msg) {
            }
        });

    }
}

//function DeleteLabTestRecord(id) {
//    if (confirm("Do you want to delete LabTest?")) {
//        var url = '/LabTestMaster/DeleteLabTest';
//        $.ajax({
//            type: "POST",
//            url: url,
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: JSON.stringify({
//                globalCodeId: id
//            }),
//            success: function (data) {
//                if (data != null) {
//                    ShowMessage("Record deleted successfully", "Alert", "info", true);
//                    ClearLabTestFields();
//                }
//                else {
//                }
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else {
//        return false;
//    }
//}

//Bind Dropdown Data of Order Type Categories
function BindLabTestCategories(ddlSelector, hdSelector) {
    var jsonData = JSON.stringify({
        categoryId: "2307",
    });
    $.ajax({
        type: "POST",
        url: '/GlobalCode/GetGlobalCodesOrderBy',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $(ddlSelector).empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data, function (i, gcc) {
                items += "<option value='" + gcc.GlobalCodeCategoryValue + "'>" + gcc.GlobalCodeCategoryName + "</option>";
            });

            $(ddlSelector).html(items);

            var hdValue = $(hdSelector).val();
            if (hdValue != null && hdValue != '') {
                $(ddlSelector).val(hdValue);
            }
        },
        error: function (msg) {
        }
    });
}

function ClearLabTestFields() {
    $("#LabTestAddEditFormDiv").clearForm();
    $('#collapseLabTestMasterAddEdit').removeClass('in');
    $.ajax({
        type: "POST",
        url: "/LabTestMaster/ResetLabTestResultForm",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#LabTestAddEditFormDiv").empty();
            $("#LabTestAddEditFormDiv").html(data);
            LabTestRangeLoad();
            $('#collapseLabTestMasterListDiv').addClass('in');
            $.validationEngine.closePrompt(".formError", true);
            BindLabTestList();
        },
        error: function (msg) {
        }
    });
}

function BindLabTestList() {
    $.ajax({
        type: "POST",
        url: "/LabTestMaster/BindLabTestList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            BindList("#LabTestMasterListDiv", data);
        },
        error: function (msg) {
        }
    });
}

function bindLabTests() {
    var labtestCategoryval = $('#ddlLabTestTypeCategory').val();
    if (labtestCategoryval != '0' || labtestCategoryval != '') {
        var jsonData = JSON.stringify({
            labtrest : labtestCategoryval
        });
        $.ajax({
            type: "POST",
            url: '/GlobalCode/GetCategoryLabtest',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, globalCode) {
                    items += "<option value='" + globalCode.CPTCodesId + "'>" + globalCode.CodeNumbering + ' -  ' + globalCode.CodeDescription + "</option>";
                });
                $('#ddlLabTestName').html(items);

                if ($('#hdCPTCodeId') != null && $('#hdCPTCodeId').val() > 0)
                    $('#ddlLabTestName').val($('#hdCPTCodeId').val());
            },
            error: function (msg) {
            }
        });
    }
}

function SetLabOrderDescription() {
    var selectedval = $('#ddlLabTestName').val();
    if (selectedval != '0' || selectedval != '') {
        var selectedtext = $('#ddlLabTestName :selected').text();
        var splitString = selectedtext.split('-')[1];
        $('#txtDescription').val(splitString);
    } else {
        $('#txtDescription').val('');
    }
}