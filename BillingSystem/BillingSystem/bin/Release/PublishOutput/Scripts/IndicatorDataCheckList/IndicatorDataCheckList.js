$(function () {
    $("#IndicatorDataCheckListFormDiv").validationEngine();
    var formData = JSON.stringify({
        BudgetType: $("#ddlBudget").val(),
        Year: $("#ddlYear").val()
        //,Month:$("#ddlMonth").val()
    });
    $("#ddlCorporate").change(function () {
        $.ajax({
            type: "POST",
            url: '/IndicatorDataCheckList/GetDataFromIndicatorDataCheckList',
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: formData,
            success: function (data) {
                BindList("#divIndicatorDataCheckListAddEdit", data);
                BindAllCheckBox();
                BindMonthChkBoxInList();
                if ($("#IndicatorDataCheckListList_0__Month").val() != "") {
                    $("#ddlMonth").val($("#IndicatorDataCheckListList_0__Month").val());
                } else {
                    $("#ddlMonth").val("0");
                }
            },
            error: function (msg) {

            }
        });
    });
    BindCorporates("#ddlCorporate", "");
    BindGlobalCodesWithValue("#ddlBudget", 3112, '');
    BindFiscalYearWithoutSelectDDls('#ddlYear', '');
    BindGlobalCodesWithValueForMonth("#ddlMonth", 903, '');
    
    $("#ddlBudget").change(function () {
        var value = $(this).val();
        $("#IndicatorDataCheckListList_0__BudgetType").val(value);
        var formData = JSON.stringify({
            BudgetType: value,
            Year: $("#ddlYear").val()
        });
        $.ajax({
            type: "POST",
            url: '/IndicatorDataCheckList/GetDataFromIndicatorDataCheckList',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data:formData,
            success: function (data) {
                BindList("#divIndicatorDataCheckListAddEdit", data);
                BindAllCheckBox();
                BindMonthChkBoxInList();
            },
            error: function (msg) {

            }
        });
    });
    $("#ddlYear").change(function () {
        var value = $(this).val();
        $("#IndicatorDataCheckListList_0__Year").val(value);
        var formData = JSON.stringify({
            BudgetType: $("#ddlBudget").val(),
            Year: value
            //,Month: $("#ddlMonth").val()
        });
        $.ajax({
            type: "POST",
            url: '/IndicatorDataCheckList/GetDataFromIndicatorDataCheckList',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: formData,
            success: function (data) {
                BindList("#divIndicatorDataCheckListAddEdit", data);
                BindAllCheckBox();
                BindMonthChkBoxInList();
                if ($("#IndicatorDataCheckListList_0__Month").val() != "") {
                    $("#ddlMonth").val($("#IndicatorDataCheckListList_0__Month").val());
                } else {
                    $("#ddlMonth").val("0");
                }
            },
            error: function (msg) {

            }
        });
    });
    $("#ddlMonth").change(function() {
        var value = $(this).val();
        $("#IndicatorDataCheckListList_0__Month").val(value);
        /*var formData = JSON.stringify({
            BudgetType: $("#ddlBudget").val(),
            Year: $("#ddlYear").val()
            //,Month: $("#ddlMonth").val()
        });
        $.ajax({
            type: "POST",
            url: '/IndicatorDataCheckList/GetDataFromIndicatorDataCheckList',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: formData,
            success: function (data) {
                BindList("#divIndicatorDataCheckListAddEdit", data);
                BindAllCheckBox();
                BindMonthChkBoxInList();
            },
            error: function (msg) {

            }
        });*/
    });
    $("#ddlBudget option[value='3']").remove();//code to remove project budget type from the drop down
});
function BindAllCheckBox() {
    var fcount = $("#hfTotalCount").val();
    //var allCheckBox = $("[id^='__CusM1']");
    //var count_checked = allCheckBox.filter(":checked").length;
    for (var i = 1; i <= 12; i++) {
        var checkedCount = $('.CusM' + i + ':checked').length;
        var classCount = $('.CusM' + i).length;
        if (checkedCount == classCount) {
            $("#__CusM" + i).prop("checked", true);
        } else {
            $("#__CusM" + i).prop("checked", false);
        }
    }
}
function BindMonthCheckBox() {
    if ($("#ddlMonth").val() == "0") {
        alert("please select month");
        $(".CusMonth").prop("checked", false);
        return false;
    }
    var fcount = $("#hfTotalCount").val();
    //var allCheckBox = $("[id^='__CusM1']");
    //var count_checked = allCheckBox.filter(":checked").length;
    for (var i = 1; i <= fcount; i++) {
        var checkedCount = $('.CusMonth:checked').length;
        var classCount = $('.CusMonth').length;
        if (checkedCount == classCount) {
            $("#__CusMonth").prop("checked", true);
        } else {
            $("#__CusMonth").prop("checked", false);
        }
    }
}
function BindMonthChkBoxInList() {
    var fcount = $("#hfTotalCount").val();
    //var allCheckBox = $("[id^='__CusM1']");
    //var count_checked = allCheckBox.filter(":checked").length;
    for (var i = 1; i <= fcount; i++) {
        var checkedCount = $('.CusMonth:checked').length;
        var classCount = $('.CusMonth').length;
        if (checkedCount == classCount) {
            $("#__CusMonth").prop("checked", true);
        } else {
            $("#__CusMonth").prop("checked", false);
        }
    }
}
function SelectAllMonth(obj) {
    var fcount = $("#hfTotalCount").val();
    var id = obj.id;
    if (id == "__CusMonth" && $("#ddlMonth").val() == "0") {
        alert("please select month");
        for (var i = 0; i < fcount; i++) {
            var selectedId = "IndicatorDataCheckListList_" + i + id;
            $("#" + selectedId).prop('checked', false);
            $("#" + id).prop('checked', false);
        }
        return false;
    }
    for (var i = 0; i < fcount; i++) {
        var selectedId = "IndicatorDataCheckListList_" + i + id;
        $("#" + selectedId).prop('checked', obj.checked);
    }
}
function SaveIndicatorDataCheckList(id) {
    $("#IndicatorDataCheckListList_0__Year").val($("#ddlYear").val());
    $("#IndicatorDataCheckListList_0__BudgetType").val($("#ddlBudget").val());
    $("#IndicatorDataCheckListList_0__Month").val($("#ddlMonth").val());
    var isValid = jQuery("#IndicatorDataCheckListFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var formData = $('#form1').serializeArray();
        $.post("/IndicatorDataCheckList/SaveIndicatorDataCheckListInDB", formData, function (data) {
            if (data.length > 0) {
                var msg = "Records Saved successfully !";
                ShowMessage(msg, "Success", "success", true);
            }
        });
        //$.ajax({
        //    type: "POST",
        //    url: '/IndicatorDataCheckList/SaveIndicatorDataCheckListInDB',
        //    async: false,
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "html",
        //    data: formData,
        //    success: function (data) {
        //        BindList("#IndicatorDataCheckListListDiv", data);
        //        ClearIndicatorDataCheckListForm();
        //        var msg = "Records Saved successfully !";
        //        if (id > 0)
        //            msg = "Records updated successfully";
        //        ShowMessage(msg, "Success", "success", true);
        //    },
        //    error: function (msg) {

        //    }
        //});
    }
}

function EditIndicatorDataCheckList(id) {
    var jsonData = JSON.stringify({
        Id: id,
    });
    $.ajax({
        type: "POST",
        url: '/IndicatorDataCheckList/GetIndicatorDataCheckList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindIndicatorDataCheckListDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteIndicatorDataCheckList() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val(),
        });
        $.ajax({
            type: "POST",
            url: '/IndicatorDataCheckList/DeleteIndicatorDataCheckList',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#IndicatorDataCheckListListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

//function DeleteIndicatorDataCheckList(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var jsonData = JSON.stringify({
//            id: id,
//        });
//        $.ajax({
//            type: "POST",
//            url: '/IndicatorDataCheckList/DeleteIndicatorDataCheckList',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                BindList("#IndicatorDataCheckListListDiv", data);
//                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function ClearIndicatorDataCheckListForm() {
    $("#IndicatorDataCheckListFormDiv").clearForm(true);
    $('#collapseIndicatorDataCheckListAddEdit').addClass('in');
    $('#collapseIndicatorDataCheckListList').addClass('in');
    $("#IndicatorDataCheckListFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
}

function BindIndicatorDataCheckListDetails(data) {

    $("#btnSave").val("Update");
    $('#collapseIndicatorDataCheckListList').addClass('in');
    $('#collapseIndicatorDataCheckListAddEdit').addClass('in');
    $("#IndicatorDataCheckListFormDiv").validationEngine();
}