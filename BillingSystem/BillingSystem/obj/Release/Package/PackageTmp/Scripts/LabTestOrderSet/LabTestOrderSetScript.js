$(function () {
    $("#LabTestOrderSetFormDiv").validationEngine();
    BindGlobalCodesCustomWithValue(3113);
    $(".blueBox1").prop("disabled", "disabled");
    $("#divCodeValue1").prop("disabled", false);
});

function BindGlobalCodesCustomWithValue(categoryIdval) {
    var jsonData = JSON.stringify({
        categoryId: categoryIdval
    });
    $.ajax({
        type: "POST",
        url: "/Home/GetGlobalCodesOrderBy",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, globalCode) {
                    items += "<option value='" + globalCode.GlobalCodeValue + "'>" + globalCode.GlobalCodeName + "</option>";
                });
                $('.ddlVal').html(items);
            }
        },
        error: function (msg) {
        }
    });
}

function SaveLabTestOrderSet(id) {
    var isValid = jQuery("#LabTestOrderSetFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {

        var jsonData = JSON.stringify({
            Id: $("#hfId").val(),
            OrderSetTableNumber: $("#txtOrderSetTableNumber").val(),
            OrderSetTableName: $("#txtOrderSetTableName").val(),
            OrderSetValue: $("#txtOrderSetValue").val(),
            Description: $("#txtDescription").val(),
            CodeValue1: $("#txtCodeValue1").val(),
            CodeTime1: $("#ddlOrderedType1").val(),
            CodeValue2: $("#txtCodeValue2").val(),
            CodeTime2: $("#ddlOrderedType2").val(),
            CodeValue3: $("#txtCodeValue3").val(),
            CodeTime3: $("#ddlOrderedType3").val(),
            CodeValue4: $("#txtCodeValue4").val(),
            CodeTime4: $("#ddlOrderedType4").val(),
            CodeValue5: $("#txtCodeValue5").val(),
            CodeTime5: $("#ddlOrderedType5").val(),
            CodeValue6: $("#txtCodeValue6").val(),
            CodeTime6: $("#ddlOrderedType6").val(),
            CodeValue7: $("#txtCodeValue7").val(),
            CodeTime7: $("#ddlOrderedType7").val(),
            CodeValue8: $("#txtCodeValue8").val(),
            CodeTime8: $("#ddlOrderedType8").val(),
            CodeValue9: $("#txtCodeValue9").val(),
            CodeTime9: $("#ddlOrderedType9").val(),
            CodeValue10: $("#txtCodeValue10").val(),
            CodeTime10: $("#ddlOrderedType10").val(),
            CodeValue11: $("#txtCodeValue11").val(),
            CodeTime11: $("#ddlOrderedType11").val(),
            CodeValue12: $("#txtCodeValue12").val(),
            CodeTime12: $("#ddlOrderedType12").val(),
            CodeValue13: $("#txtCodeValue13").val(),
            CodeTime13: $("#ddlOrderedType13").val(),
            CodeValue14: $("#txtCodeValue14").val(),
            CodeTime14: $("#ddlOrderedType14").val(),
            CodeValue15: $("#txtCodeValue15").val(),
            CodeTime15: $("#ddlOrderedType15").val(),
            IsDeleted: false
        });
        $.ajax({
            type: "POST",
            url: '/LabTestOrderSet/SaveLabTestOrderSet',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                ClearLabTestOrderSetForm();
                BindList("#LabTestOrderSetListDiv", data);
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

function EditCurrentLabTestOrderSet(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/LabTestOrderSet/GetLabTestOrderSet',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindLabTestDetails(data);
        },
        error: function (msg) {

        }
    });
}

function BindLabTestDetails(data) {
    $("#hfId").val(data.Id);

    $("#txtOrderSetTableNumber").val(data.OrderSetTableNumber);
    $("#txtOrderSetTableName").val(data.OrderSetTableName);
    $("#txtOrderSetValue").val(data.OrderSetValue);
    $("#txtDescription").val(data.Description);
    $("#txtCodeValue1").val(data.CodeValue1);
    $("#ddlOrderedType1").val(data.CodeTime1);
    if (data.CodeTime1 != '0') {
        $("#divCodeValue1").prop("disabled", false);
    }

    $("#txtCodeValue2").val(data.CodeValue2);
    $("#ddlOrderedType2").val(data.CodeTime2);
    if (data.CodeTime2 != '0') {
        $("#divCodeValue2").prop("disabled", false);
    }
    $("#txtCodeValue3").val(data.CodeValue3);
    $("#ddlOrderedType3").val(data.CodeTime3);
    if (data.CodeTime3 != '0') {
        $("#divCodeValue3").prop("disabled", false);
    }
    $("#txtCodeValue4").val(data.CodeValue4);
    $("#ddlOrderedType4").val(data.CodeTime4);
    if (data.CodeTime1 != '0') {
        $("#divCodeValue1").prop("disabled", false);
    }
    $("#txtCodeValue5").val(data.CodeValue5);
    $("#ddlOrderedType5").val(data.CodeTime5);
    if (data.CodeTime5 != '0') {
        $("#divCodeValue5").prop("disabled", false);
    }
    $("#txtCodeValue6").val(data.CodeValue6);
    $("#ddlOrderedType6").val(data.CodeTime6);
    if (data.CodeTime6 != '0') {
        $("#divCodeValue6").prop("disabled", false);
    }
    $("#txtCodeValue7").val(data.CodeValue7);
    $("#ddlOrderedType7").val(data.CodeTime7);
    if (data.CodeTime7 != '0') {
        $("#divCodeValue7").prop("disabled", false);
    }
    $("#txtCodeValue8").val(data.CodeValue8);
    $("#ddlOrderedType8").val(data.CodeTime8);
    if (data.CodeTime8 != '0') {
        $("#divCodeValue8").prop("disabled", false);
    }
    $("#txtCodeValue9").val(data.CodeValue9);
    $("#ddlOrderedType9").val(data.CodeTime9);
    if (data.CodeTime9 != '0') {
        $("#divCodeValue9").prop("disabled", false);
    }
    $("#txtCodeValue10").val(data.CodeValue10);
    $("#ddlOrderedType10").val(data.CodeTime10);
    if (data.CodeTime10 != '0') {
        $("#divCodeValue10").prop("disabled", false);
    }
    $("#txtCodeValue11").val(data.CodeValue11);
    $("#ddlOrderedType11").val(data.CodeTime11);
    if (data.CodeTime11 != '0') {
        $("#divCodeValue11").prop("disabled", false);
    }
    $("#txtCodeValue12").val(data.CodeValue12);
    $("#ddlOrderedType12").val(data.CodeTime12);
    if (data.CodeTime12 != '0') {
        $("#divCodeValue12").prop("disabled", false);
    }
    $("#txtCodeValue13").val(data.CodeValue13);
    $("#ddlOrderedType13").val(data.CodeTime13);
    if (data.CodeTime13 != '0') {
        $("#divCodeValue13").prop("disabled", false);
    }
    $("#txtCodeValue14").val(data.CodeValue14);
    $("#ddlOrderedType14").val(data.CodeTime14);
    if (data.CodeTime14 != '0') {
        $("#divCodeValue14").prop("disabled", false);
    }
    $("#txtCodeValue15").val(data.CodeValue15);
    $("#ddlOrderedType15").val(data.CodeTime15);

    if (data.CodeTime15 != '0') {
        $("#divCodeValue15").prop("disabled", false);
    }

    $('#btnSave').val('Update');
    $('#collapseLabTestOrderSetList').removeClass('in');
    $('#collapseLabTestOrderSetAddEdit').addClass('in');
}

function DeleteCurrentLabTestOrderSet() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/LabTestOrderSet/DeleteLabTestOrderSet',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#LabTestOrderSetListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

//function DeleteCurrentLabTestOrderSet(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var jsonData = JSON.stringify({
//            Id: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/LabTestOrderSet/DeleteLabTestOrderSet',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                BindList("#LabTestOrderSetListDiv", data);
//                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function ClearLabTestOrderSetForm() {
    $("#LabTestOrderSetFormDiv").clearForm(true);
    $('#collapseLabTestOrderSetAddEdit').removeClass('in');
    $('#collapseLabTestOrderSetList').addClass('in');
    $("#LabTestOrderSetFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $(".blueBox1").prop("disabled", "disabled");
    $("#divCodeValue1").prop("disabled", false);
}

function CheckIfCptCodeExistsInRange(txtSelector) {
    var txtValue = $(txtSelector).val();
    if (txtValue != '' && TryParseInt(txtValue, null) != null) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/GlobalCodeCategory/CheckIfCptCodeExistsInRange",
            data: JSON.stringify({ value: txtValue }),
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                if (!data) {
                    ShowMessage("Code Value not in the CPT Codes range!", "Not in the list", "warning", true);
                    $(txtSelector).val('');
                    $(txtSelector).focus();
                    return false;
                }
                return data;
            },
            error: function (msg) {

            }
        });
    }

    if (txtSelector != '#txtOrderSetValue') {
        var txtCurrentNumber = parseInt(txtSelector.substring(txtSelector.indexOf("ue") + 2, txtSelector.length), 10);
        var txtNext = parseInt(txtCurrentNumber + 1, 10);
        var txtSetDisable = parseInt(txtCurrentNumber + 2, 10);
        $("#divCodeValue" + txtNext).prop("disabled", false);
        if (txtSetDisable <= 15) {
            for (var i = txtSetDisable; i <= 15; i++) {
                var divSelectorNext = "#divCodeValue" + txtSetDisable;
                var txtSelectorNext = "#txtCodeValue" + txtSetDisable;
                var ddlSelectorNext = "#ddlOrderedType" + txtSetDisable;
                $(txtSelectorNext).val('');
                $(ddlSelectorNext).val('0');
                $(divSelectorNext).prop("disabled", "disabled");
            }
        }
        var txtSelectorNextToCurrent = "#txtCodeValue" + txtNext;
        var ddlSelectorNextToCurrent = "#ddlOrderedType" + txtNext;
        $(txtSelectorNextToCurrent).val('');
        $(ddlSelectorNextToCurrent).val('0');
    }
    return true;
}


//-------------Smart Search of Global Code Categories starts here-------------------

function OnGCodeCategorySelection(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtDescription").val(dataItem.Name);
    $('#txtOrderSetValue').val(dataItem.CodeValue);
    $('#hfCodeValue').val(dataItem.CodeValue);
}

function SetCodeValue(hiddenSelector) {
    
    //var text = $('#txtOrderSetValue').val();
    if ($('#txtOrderSetValue').val() != '') {
        $('#txtOrderSetValue').val($('#hfCodeValue').val());
    } else {
        $("#txtDescription").val('');
    }
}

function SelectGCodeCategory(e) {
    var value = null;
    if (e.filter.filters != null && e.filter.filters.length > 0) {
        value = e.filter.filters[0].value;
    }
    return {
        typeId: "4000",
        text: value
    };
}

//-------------Smart Search of Global Code Categories ends here-------------------



function SortLabTestOrderList(event) {
    
    var url = "/LabTestOrderSet/SortLabTestOrderList";
 if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
     url += "?"  + "&" + event.data.msg;
 }

    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            
            $("#collapseLabTestOrderSetList").empty();
            $("#collapseLabTestOrderSetList").html(data);
            //$('#LabTest').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
            //BindLabTestDetails(data);
            },
        error: function (msg) {
        }
    });
}

