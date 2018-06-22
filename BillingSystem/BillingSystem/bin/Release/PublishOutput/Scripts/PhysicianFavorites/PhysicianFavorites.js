/*
Owner: Amit Jain
On: 11092014
Purpose: Bind the Codes based on Code Type Id
*/
$(function () {
    JsCallsPhyFav();
    BindPhysicianDropdown();
    BindOrderTypeDDL();
});

function JsCallsPhyFav() {
    $('.FavAdd').show();
    $('.OrderAdd').hide();
    $('.FavRemove').show();
    $('.AddOrder').hide();
}

function BindPhysicianSearchGrid() {
    ajaxStartActive = false;
    var categoryIdval = $('#ddltype').val();
    var jsonData = JSON.stringify({
        searchType: categoryIdval
    });
    $('#loader_event').show();
    $.ajax({
        type: "POST",
        url: "/Home/GetSerachList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                $('#loader_event').hide();
                $("#SearchedOrdersList").empty();
                $("#SearchedOrdersList").html(data);
                $('.unFav').hide();
                $('.AddFav').show();
                if (categoryIdval == "3") {
                    $('#CPTCodesGrid').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                    SetGridSorting(SortCptCodeGrid, "#CPTCodesListDiv");
                    }
                if (categoryIdval == "5") {
                    $('#DrugListgrid').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                    SetGridSorting(SortDrugCodeGrid, "#DrugListDiv");
                    }
                if (categoryIdval == "16") {
                    $('#DiagnosisCodegrid').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                    SetGridSorting(SortDiagnosisCodeList, "#DiagnosisCodeListDiv");
                }
                if (categoryIdval == "4") {
                    $('#HCPCSgrid').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                    SetGridSorting(SortHCPCSCodeGrid, "#gridContent");
                }
              }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

function BindPhysicianDropdown() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Physician/BindPhysicianddlList",
        // data: jsonData,
        dataType: "json",
        success: function (data) {
            $("#ddlPhysicians").empty();
            $("#ddlPhysicians").append('<option value="0">--Select One--</option>');
            $.each(data, function (i, code) {
                $("#ddlPhysicians").append('<option value="' + code.UserId + '">' + code.PhysicianName + '</option>');
            });
        },
        error: function (msg) {
        }
    });
}

function BindOrderTypeDDL() {
    BindGlobalCodesWithValue('#ddltype', 1201, '#hdOrderType');
}

function GetFavoritesOrders() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetFavoritesOrders",
        data: null,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            $("#favOrdersGrid").empty();
            $("#favOrdersGrid").html(data);
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function GetMostRecentOrders() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetMostRecentOrders",
        data: null,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            $("#MostRecentOrdersGrid").empty();
            $("#MostRecentOrdersGrid").html(data);
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function AddTophyFavorites(orderId, favoriteId, markFavorite, favDesc) {
    var physicianId = $('#ddlPhysicians').val();
    var jsonData = JSON.stringify({
        codeId: orderId,
        categoryId: "1",
        id: favoriteId,
        isFavorite: markFavorite,
        favoriteDesc: favDesc,
        UserId: physicianId,
        screentype: '0'
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PhysicianFavorites/AddToPhyFavorites",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            if (data != null) {
                if (data == "1") {
                    ShowMessage("Record already exist.", "Info", "warning", true);
                } else {
                    ShowMessage("Record added successfully.", "Success", "success", true);
                    $('#favOrdersGrid').empty();
                    $('#favOrdersGrid').html(data);
                    GetPhysicianAllOrders();
                    //ShowPhysicianData();
                }
            }
        },
        error: function (msg) {

        }
    });
}

function EditFavorite(orderId) {
    var jsonData = JSON.stringify({
        codeId: orderId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetFavoriteByCodeId",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            if (data != null) {
                $("#hdFavoriteId").val(data.id);
                if (data.isFavorite != null && data.isFavorite == true) {
                    $("#chkMarkAsFavorite").prop("checked", "checked");
                    $("#favoriteOrderDescDiv").show();
                    $("#txtFavoriteDescription").val(data.description);
                }
            }
            else {

            }
        },
        error: function (msg) {

        }
    });
}

// changes by ashwani to get by patient ID
function BindDiagnosisType(selector, hidValueSelector) {
    var jsonData = JSON.stringify({
        patientId: $("#hdPatientId").val()
    });
    $.ajax({
        type: "POST",
        url: "/Summary/GetDiagnosisCodes",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, diagnosisCode) {
                    items += "<option value='" + diagnosisCode.DiagnosisCode1 + "'>" + diagnosisCode.DiagnosisFullDescription + "</option>";
                });
                $(selector).html(items);

                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
                    $(selector).val($(hidValueSelector).val());
            }
        },
        error: function (msg) {
        }
    });
}

function EditPhysicanOrderDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.Id;
    EditOrder(id);
}

function BindCategories(ddlSelector, hdSelector) {
    var jsonData = JSON.stringify({
        startRange: "11000",
        endRange: "11999"
    });
    $.ajax({
        type: "POST",
        url: '/Home/GetGlobalCodeCatByExternalValue',        //GetGlobalCodeCategoriesByExternalValue
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
                $(ddlSelector)[0].selectedIndex = 0;
            }

        },
        error: function (msg) {
        }
    });
}

function OnChangeCategory(categorySelector, ddlSelector, hdSubCategorySelector) {
    var categoryId = $(categorySelector).val().trim();
    if (categoryId != '' && categoryId != "0") {
        var jsonData = JSON.stringify({
            categoryId: categoryId
        });

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/GetOrderTypeSubCategories",
            //data: JSON.stringify({ id: $("#hdOrderTypeSubCategoryID").val() }),
            data: jsonData,
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                if (data != null) {
                    BindDropdownData(data, ddlSelector, hdSubCategorySelector);

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
    }
    return false;
}

function SetValue(selector, value) {
    $(selector).val(value);
}

function SearchCodeList() {
    
    var txtData = $("#txtSearchData").val();
    var searchType = $("#ddltype").val();
    if (searchType != '0') {
        if (txtData != '') {
            var jsonData = JSON.stringify({
                text: txtData,
                searchType: searchType
            });
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/Home/GetFilteredCodesInFav",
                data: jsonData,
                dataType: "html",
                beforeSend: function() {},
                success: function(data) {
                    if (data != null) {
                        $("#SearchedOrdersList").empty();
                        $("#SearchedOrdersList").html(data);
                        $('.unFav').hide();
                        $('.AddFav').show();
                        // HighLightSearchedText();
                    } else {

                    }
                },
                error: function(msg) {

                }
            });
        }
    } else {
        ShowMessage("Please select the Code type!", "Info", "warning", true);
    }
}

function SelectDiagnosisTab() {
    BindGlobalCodesWithValue("#ddlOrderType", 1201, "#hdOrderType");
    $("#Diagnosis").click();
}

function OnClickBindOrdersData() {
    $(".editOpenOrder").show();
    //var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    //BindDiagnosisType("#ddlDiagnosisType", "#hdDiagnosis");
    if (encounterId != '' && encounterId > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/CheckIfAnyPrimaryDiagnosisExists",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                if (data != null && data > 0) {
                    $("#hdPrimaryDiagnosisId").val(data);
                    $('#divPhysicianOrder :input').attr('disabled', false);
                }
                else {
                    $('#divPhysicianOrder :input').attr('disabled', true);
                    ShowMessage("Add primary diagnosis first and then try again!", "Diagnosis Not Found", "warning", true);
                }
            },
            error: function (msg) {

            }
        });
    }
    else {
        $('#divPhysicianOrder :input').attr('disabled', true);
        ShowMessage("Add primary diagnosis first and then try again!", "Diagnosis Not Found", "warning", true);
    }
}

function OnChangeMarkAsFavorite() {
    var markFavorite = $("#chkMarkAsFavorite").prop("checked");
    if (markFavorite == true) {
        $("#favoriteOrderDescDiv").show();
    }
    else {
        $("#favoriteOrderDescDiv").hide();
    }
}

function OnChangeSubCatgory(ddlSelector) {
    var value = $(ddlSelector).val();
    var jsonData = JSON.stringify({
        subCategoryId: value
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetOrderCodesBySubCategory",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            if (data != null) {
                //Set Order Type Code Name
                $("#CodeTypeValue").text(data.codeTypeName);

                //Set Order Type Code Id
                $("#hdOrderTypeId").val(data.codeTypeId);
                BindOrderCodesBySubCategoryID(data.codeList, "#ddlOrderCodes", "#hdOrderCodeId");
            }
        },
        error: function (msg) {

        }
    });
}

function BindOrderCodesBySubCategoryID(data, ddlSelector, hdSelector) {
    BindDropdownData(data, ddlSelector, hdSelector);
}

function OnClickShowHideActions(isHide) {
    if (isHide) {
        $(".editOpenOrder").hide();
        $(".deleteVitalOrder").hide();
    }
    else {
        $(".editOpenOrder").show();
        $(".deleteVitalOrder").show();
    }
}

function OnClickShowHideActions(isHide) {
    if (isHide) {
        $(".editOpenOrder").hide();
        $(".deleteVitalOrder").hide();
    }
    else {
        $(".editOpenOrder").show();
        $(".deleteVitalOrder").show();
    }
}

function DeleteFav() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        $.ajax({
            type: "POST",
            url: '/PhysicianFavorites/DeleteFav',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ Id: $("#hfGlobalConfirmId").val() }),
            success: function (data) {
                if (data != null) {
                    var msg = "Records Deleted successfully !";
                    ShowPhysicianData();
                    JsCallsPhyFav();
                    ShowMessage(msg, "Success", "success", true);
                }
            },
            error: function (msg) {
            }
        });

    }
}


//function DeleteFav(id) {
//    if (confirm("Do you really want to delete this record?")) {
//        $.ajax({
//            type: "POST",
//            url: '/PhysicianFavorites/DeleteFav',
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: JSON.stringify({ Id: id }),
//            success: function (data) {
//                if (data != null) {
//                    var msg = "Records Deleted successfully !";
//                    ShowPhysicianData();
//                    JsCallsPhyFav();
//                    ShowMessage(msg, "Success", "success", true);
//                }
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

function GetPhysicianAllOrders() {
    var physicianId = $('#ddlPhysicians').val();
    var jsonData = JSON.stringify({
        userid: physicianId,
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PhysicianFavorites/GetPhysicianFavorites",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            $("#divPhysicianFavoritesAll").empty();
            $("#divPhysicianFavoritesAll").html(data);
            JsCallsPhyFav();
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function AddAsFav(id) {
    AddTophyFavorites(id, 0, true, "");
}

function ShowPhysicianData() {
    var physicianId = $('#ddlPhysicians').val();
    var jsonData = JSON.stringify({
        userid: physicianId,
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PhysicianFavorites/GetPhysicianFavorites",
        data: jsonData,
        dataType: "html",
        success: function (data) {
            $("#favOrdersGrid").empty();
            $("#favOrdersGrid").html(data);
            JsCallsPhyFav();
            //BindOrderTypeDDL();
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function MarkAsFav(id) {
    var physicianId = $('#ddlPhysicians').val();
    if (physicianId != "0") {
        var codeType = $('#ddltype').val();
        var jsonData = JSON.stringify({
            CodeId: id,
            CategoryId: codeType,
            id: 0,
            isFavorite: true,
            favoriteDesc: "",
            UserId: physicianId,
            screentype: '1'
        });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/PhysicianFavorites/AddToPhyFavorites",
            data: jsonData,
            dataType: "html",
            success: function (data) {
                if (data == "1") {
                    ShowMessage("Record already exist.", "Info", "warning", true);
                } else {
                    ShowPhysicianData(); /* Update by Krishna Date:03072015 */
                    JsCallsPhyFav();
                    ShowMessage("Record added successfully.", "Success", "success", true);
                    //$('#favOrdersGrid').empty();
                    //$('#favOrdersGrid').html(data);
                    //JsCallsPhyFav();
                }
            },
            error: function (msg) {
            }
        });
    } else {
        ShowMessage("Please Select any physician.", "Error", "warning", true);
    }
    return false;
}

var HighLightSearchedText = function() {
    var text = $('#txtSearchData').val();
    var query = new RegExp("(\\b" + text + "\\b)", "gim");
    var e = $('#SearchedOrdersList .col3').html();
    var enew = e.replace(/(<span>|<\/span>)/igm, "");
    $('#SearchedOrdersList .col3').html(enew);
    var newe = enew.replace(query, "<span>$1</span>");
    $('#SearchedOrdersList .col3').html(newe);
    //var e1 = $('#SearchedOrdersList .col4').html();
    //var enew1 = e1.replace(/(<span'>|<\/span>)/igm, "");
    //$('#SearchedOrdersList .col4').html(enew1);
    //var newe1 = enew.replace(query, "<span>$1</span>");
    //$('#SearchedOrdersList .col4').html(newe1);
}



function SortPhysicianFavSearchGrid() {
    
    var categoryIdval = $('#ddltype').val();
    var jsonData = JSON.stringify({
        searchType: categoryIdval
    });
    $.ajax({
        type: "POST",
        url: "/Home/GetSerachList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                //
                $("#SearchedOrdersList").empty();
                $("#SearchedOrdersList").html(data);
                $('.unFav').hide();
                $('.AddFav').show();
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

function SortPhysicianList(event) {
var physicianId = $('#ddlPhysicians').val();
var url = '/PhysicianFavorites/GetPhysicianFavorites';
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?userid=" + physicianId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#favOrdersGrid").empty();
            $("#favOrdersGrid").html(data);
            JsCallsPhyFav();
        },
        error: function (msg) {
        }
    });
}



function SortCptCodeGrid(event) {
    var categoryIdval = $('#ddltype').val();

    var activeInActive = $("#chkShowInActive").is(':checked');
    if (activeInActive) {
        activeInActive = false;
    } else {
        activeInActive = true;
    }
    var url = "";
    var searchText = $("#txtSearchData").val();
    if (searchText != "" && searchText != null) {
        url = "/Home/GetFilteredCodes";
        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
            url += "?text=" + searchText + "&searchType=" + 3 + "&drugStatus=" + 0 + "&" + event.data.msg;
        }
    } else {
        url = "/CPTCodes/GetSerachList";
        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
            url += "?searchType=" + categoryIdval + "&showInActive=" + activeInActive + "&" + event.data.msg;
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
            $("#SearchedOrdersList").empty();
            $("#SearchedOrdersList").html(data);
            $('.unFav').hide();
            $('.AddFav').show();
            SetGridCheckBoxes();
        },
        error: function (msg) {
        }
    });
}