$(function () {
    GetUnclosedEncounters();

    $('#btnCancelUCOrder').click(function (e) {
        var orderId = $('#hidCancelOrderId').val();
        if (orderId != undefined && orderId != "") {
            CancelUCOrder(orderId);
        }
    });

    $('#btnCancel').on('click', function () {
        var orderId = $('#hidCancelOrderId').val();
        HideCancelOrderPopup(orderId);
        return false;
    });

    $("#btnCancelActivity")
        .click(function () {
            var orderId = $("#hidCancelOrderActivityId").val();
            HideCancelOrderActivityPopup(orderId);
            return false;
        });

    $("#btnCancelOrderActivityinEncounter").off('click');
    $("#btnCancelOrderActivityinEncounter").on('click', function (e) {
        e.preventDefault();
        var orderactivityId = $("#hidCancelOrderActivityId").val();
        CancelOrderActivityinEncounter(orderactivityId);
    });
});

function SetRowColorToUnclosedEncouters() {
    $("#unclosedEncounters tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        if ($actualRow.find('.colErrorStatus').html().indexOf('O') != -1) {
            $actualRow.removeClass("sorting_1").addClass('rowColor3');
        }
        $('.colErrorStatus').removeClass("sorting_1");

    });
}

function EditOrders(eid, pid) {

    /// <summary>
    /// Edits the orders.
    /// </summary>
    /// <param name="eid">The eid.</param>
    /// <param name="pid">The pid.</param>
    /// <returns></returns>
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
            }
        },
        error: function (msg) {
        }
    });
}
function BindOrders(eid, pid) {

    /// <summary>
    /// Edits the orders.
    /// </summary>
    /// <param name="eid">The eid.</param>
    /// <param name="pid">The pid.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        encounterId: eid,
        patientId: pid
    });
    $.ajax({
        type: "POST",
        url: '/ActiveEncounter/BindOpenOrdersData',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                $("#hdPatientid").val(pid);
                $("#hdEncounterId").val(eid);
                BindOpenOrdersinUnclosedPopUp(data.OpenOrdersList);
                BindOrderActivitiesinUnclosedPopUp(data.orderActivities);
                $("#CurrentOrders").trigger("click");

                $('#divhidepopup').show();
            }
        },
        error: function (msg) {
        }
    });
}

//Set the Selected Value to Ordering Codes Dropdown
function SetValueToOrderingCodesDropdown(selectedValue, item) {
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

function SelectOrderingCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtOrderCode").val(dataItem.CodeDescription);
    $("#hdAutocompleteOrderCodeId").val(dataItem.CodeDescription);
    $("#hdOrderType").val(dataItem.CodeType);
    $("#hdOrderExternalCode").val(dataItem.ExternalCode);
    $('#CodeTypeValue').html(dataItem.CodeTypeName);
    var items = '<option value="0">--Select--</option>';
    items += "<option value='" + dataItem.Code + "'>" + dataItem.CodeDescription + "</option>";
    SetValueToOrderingCodesDropdown(dataItem.Code, items);
    BindAllDDLValues();
    setTimeout(function () {
        $("#ddlOrderCodes option:contains(" + $('#txtOrderCode').val() + ")").attr('selected', 'selected');
    }, 2000);
}

function AddOrder(id) {
    var orderId = id;
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdEncounterId").val();
    var ddlOrderType = $("#hdOrderTypeId").val();                                       //$("#ddlOrderType").val();
    var orderCode = $("#ddlOrderCodes").val().trim();
    var hdPrimaryDiagnosisId = $("#hdCurrentDiagnosisID").val();
    var frequency = $("#ddlFrequencyList").val();
    var txtQuantity = $("#ddlQuantityList").val();//BindGlobalCodesWithValue("#ddlQuantityList", 1011, "#hdQuantity");
    var txtOrderNotes = $("#txtOrderNotes").val();
    var ddlOrderStatus = $("#ddlOrderStatus").val();
    var ddlOrderTypeCategory = $("#ddlOrderTypeCategory").val().trim();
    var ddlOrderTypeSubCategory = $("#ddlOrderTypeSubCategory").val();

    var txtOrderStartDate = new Date($("#txtOrderStartDate").val());
    var txtOrderEndDate = new Date($("#txtOrderEndDate ").val());
    var hdIsActivitySchecduled = $('#hdIsActivitySchecduled').val();
    var hdActivitySchecduledOn = $('#hdActivitySchecduledOn').val();
    // var ddlDosageForm = '';
    //var ddlDosageAmount = '';
    if ($("#ddlOrderTypeCategory :selected").text() == "Pharmacy") {
        //ddlDosageForm = $("#ddlDosageForm").val();
        //ddlDosageAmount = $("#ddlDosageAmount").val();
    }
    var jsonData = JSON.stringify({
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
        StartDate: txtOrderStartDate,
        EndDate: txtOrderEndDate,
        // ItemDosage: ddlDosageForm,
        //  ItemStrength: ddlDosageAmount,
        IsActivitySchecduled: hdIsActivitySchecduled,
        ActivitySchecduledOn: hdActivitySchecduledOn
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/AddPhysicianOrder",
        data: jsonData,
        dataType: "json",
        success: function (data) {
            var msg = "Records Saved successfully !";
            var favoriteId = $("#hdFavoriteId").val();
            var markFavorite = $("#chkMarkAsFavorite").prop("checked");
            if (favoriteId == '')
                favoriteId = 0;

            var txtFavDescription = $("#txtFavoriteDescription").val();
            if (markFavorite == true) {
                AddToFavorites(data, favoriteId, markFavorite, txtFavDescription);
                GetFavoritesOrders();
            }
            if (id > 0) {
                msg = "Records updated successfully";
            }
            ShowMessage(msg, "Success", "success", true);
            ClearPhysicianOrderAll(encounterId);
            $("#chkMarkAsFavorite").attr("checked", false);
            //GetOrdersTabData();
            ///...Need to fix this in one single json call
            GetMostRecentOrders();
            GetPhysicianAllOrders();
            //GetPhysicianAllClosed();
            //BindOrderActivityList(encounterId);
            //BindClosedOrderActivityList(encounterId);
            BindGridsAfterOrder();
        },
        error: function (msg) {

        }
    });
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

            $("#favOrdersGrid1").empty();
            $("#favOrdersGrid1").html(data);
        },
        // ReSharper disable once UnusedParameter
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

function GetPhysicianAllClosed() {
    var encounterId = $("#hdEncounterId").val();
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
            RowColor();
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function AddToFavorites(orderId, favoriteId, markFavorite, favDesc) {
    var ordertypeCategory = $('#ddlOrderTypeCategory :selected').text();
    var orderCode = $('#ddlOrderCodes').val();
    var jsonData = JSON.stringify({
        codeId: orderCode,
        categoryId: ordertypeCategory == "Pharmacy" ? "5" : "3",
        id: favoriteId,
        isFavorite: markFavorite,
        favoriteDesc: favDesc,
        Dtype: 'false'
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/AddToFavorites",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            if (data != null) {
                if (data == "1") {
                    ShowMessage("Record already exist.", "Info", "warning", true);
                } else {
                    // ShowMessage("Record added successfully.", "Success", "success", true);
                    $('#favOrdersGrid').empty();
                    $('#favOrdersGrid').html(data);
                    GetPhysicianAllOrders();
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

function BindOrderGrid(encounterid) {
    var jsonData = JSON.stringify({ EncounterId: encounterid });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/BindEncounterOrderList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#NurseAdminOpenOrdersListDiv").empty();
            $("#NurseAdminOpenOrdersListDiv").html(data);

            $('#colCurrentOrdersMain').empty();
            $('#colCurrentOrdersMain').html(data);

            $('#NurseAdminOpenOrdersListDiv').empty();
            $('#NurseAdminOpenOrdersListDiv').html(data);

            $("#collapseOpenOrderAddEdit").removeClass("in");
            $("#collapseOpenOrderlist").addClass("in");
            SetGridSorting(BindOrdersBySort, "#gridContentOpenOrder");
        },
        error: function (msg) {
            alert(msg);
        }

    });
}

function EditOrder(id) {
    var jsonData = JSON.stringify({ orderId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetOrderDetailById",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $('#OpenOrderDiv').empty();
            $('#OpenOrderDiv').html(data);
            InitializeDateTimePicker();
            EditFavorite(id);
            BindCategories("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
            if ($("#hdOrderCodeId").val() != '') {
                $("#txtOrderCode").val($("#hdOrderCodeId").val());
            }
            BindGlobalCodesWithValue("#ddlFrequencyList", 1024, "#hdFrequencyCode");
            BindGlobalCodesWithValue("#ddlOrderStatus", 3102, "#hdOrderStatus");
            BindGlobalCodesWithValue("#ddlQuantityList", 1011, "#hdQuantity");
            // $('#ddlOrderStatus').val(OrderStatus.Open);

            //BindDiagnosisType("#ddlDiagnosisType", "#hdDiagnosis");
            //BindDateTimePicker();
            //BindOpenCodes();
            $("#ddlFrequencyList option:contains(" + $('#hdFrequencyCode').val() + ")").attr('selected', 'selected');
            $("#ddlOrderStatus option[value='4']").remove();
            $("#ddlOrderStatus option[value='3']").remove();
            //$("#ddlOrderStatus option[value='2']").remove();
            CheckForMultipleActivites($("#hfOpenOrderid").val());
            setTimeout(function () {
                $('#txtOrderCode').val($('#ddlOrderCodes :selected').text());
            }, 1000);
        },
        error: function (msg) {

        }
    });
}

function IsValidOrder(id) {
    var isValid = false;
    if ($("#divPhysicianOrder").html() != null) {
        isValid = jQuery("#divPhysicianOrder").validationEngine({ returnIsValid: true });
    } else {
        isValid = jQuery("#OpenOrderDiv").validationEngine({ returnIsValid: true });
    }
    if (id > 0) {
        if ($('#ddlOrderStatus').val() == '3') {
            if (confirm("Do you want to closed the open activities for current order?")) {
                if (isValid == true) {
                    AddOrder(id);
                }
                UpdateOrderActivities(id);
            }
        }
        else if (isValid == true) {
            AddOrder(id);
        }
    } else {
        if (isValid == true) {
            AddOrder(id);
        }
    }
    return false;
}

function UpdateOrderActivities(orderid) {
    var jsonData = JSON.stringify({
        orderid: orderid
    });
    $.ajax({
        type: "POST",
        url: '/Summary/UpdateOpenOrderActivities',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {

            }
            else {
            }
        },
        error: function (msg) {

        }
    });
}

function ResetOrder() {
    $.validationEngine.closePrompt(".formError", true);
    $("#OpenOrderDiv").clearForm();
    ResetAllDropdowns("#OpenOrderDiv");
    $('#collapseOpenOrderAddEdit').removeClass('in');
    $('#collapseOpenOrderlist').addClass('in');
    $('#hdPrimaryDiagnosisId').val('');
    $('#hdOrderTypeCategoryID').val('0');
    $('#hdFrequencyCode').val('0');
    $('#hdOrderTypeSubCategoryID').val('0');
    $('#hdOrderTypeId').val('0');
    $('#hdOrderStatus').val('0');
    $('#hdOrderCodeId').val('0');
    $('#btnAddOrder').attr('onclick', 'return IsValidOrder("0");');
    $('#ddlOrderTypeCategory').val('0');
    $('#ddlOrderTypeSubCategory').empty();
    $('#ddlOrderStatus').val('1');
    $('#ddlFrequencyList').val('11');
    $('#ddlQuantityList').val('1.00');
    $('#ddlOrderCodes').empty();
    $('.DrugDDL').hide();
    BindCategories("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
    var currentdate = new Date();
    var datewithFormat = currentdate.format('mm/dd/yyyy');
    $('#txtOrderStartDate').val(datewithFormat);
    $('#txtOrderEndDate').val(datewithFormat);
}

function EditPhysicanOrderDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.Id;
    EditOrder(id);
}

function ClearPhysicianOrderForm() {
    $("#OpenOrderDiv").clearForm(true);
    $('#ddlOrderStatus').val(OrderStatus.Open);
    $('#collapseOpenOrderAddEdit').removeClass('in');
    $('#collapseOpenOrderlist').addClass('in');
    InitializeDateTimePicker();
    ResetAllDropdowns('#OpenOrderDiv');
    BindCategories("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
    $("#ddlFrequencyList").val('10');
    var currentdate = new Date();
    var datewithFormat = currentdate.format('mm/dd/yyyy');
    $('#txtOrderStartDate').val(datewithFormat);
    $('#txtOrderEndDate').val(datewithFormat);
    $("#ddlOrderStatus option[value='4']").remove();
    $("#ddlOrderStatus option[value='3']").remove();
    $("#ddlOrderStatus option[value='2']").remove();
    $('#btnAddOrder').attr('onclick', 'return IsValidOrder("0");');
}

function ClearPhysicianOrderAll(encounterid) {
    ClearPhysicianOrderForm();
    $.validationEngine.closePrompt(".formError", true);
    //var jsonData = JSON.stringify({
    //    Id: 0,
    //});
    //$.ajax({
    //    type: "POST",
    //    url: '/Summary/ResetPhysicianOrderForm',
    //    async: false,
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "html",
    //    data: jsonData,
    //    success: function (data) {
    //        if (data) {
    //            $('#OpenOrderDiv').empty();
    //            $('#OpenOrderDiv').html(data);

    //            //BindOrderGrid(encounterid);
    //            BindCategories("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
    //            BindGlobalCodesWithValue("#ddlFrequencyList", 1024, "#hdFrequencyCode");
    //            //BindDiagnosisType("#ddlDiagnosisType", "#hdDiagnosis");
    //            //BindDateTimePicker();
    //            //BindOpenCodes();
    //            BindGlobalCodesWithValue("#ddlOrderStatus", 3102, "#hdOrderStatus");
    //            BindGlobalCodesWithValue("#ddlQuantityList", 1011, "#hdQuantity");
    //            //$('#ddlOrderStatus').val(OrderStatus.Open);
    //            $('#collapseOpenOrderAddEdit').removeClass('in');
    //            $('#collapseOpenOrderlist').addClass('in');
    //            $("#ddlOrderStatus option[value='4']").remove();
    //            $("#ddlOrderStatus option[value='3']").remove();
    //            $("#ddlOrderStatus option[value='2']").remove();
    //            InitializeDateTimePicker();
    //        }
    //    },
    //    error: function (msg) {


    //        return true;
    //    }
    //});
}

function BindCategories(ddlSelector, hdSelector) {
    /// <summary>
    /// Binds the categories.
    /// </summary>
    /// <param name="ddlSelector">The DDL selector.</param>
    /// <param name="hdSelector">The hd selector.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        startRange: "11000",
        endRange: "11999"
    });
    $.ajax({
        type: "POST",
        url: '/Home/GetGlobalCodeCatByExternalValue',
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
            } else {
                var tabvalue = $('#hfTabValue').val();
                var selectedVal = "0";
                switch (tabvalue) {
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '10':
                    case '11':
                        selectedVal = "0";
                        break;
                    case '6':
                        selectedVal = "11080";// OrderCodeTypes.PathologyandLaboratory;
                        break;
                    case '7':
                        selectedVal = "11070";//OrderCodeTypes.Radiology;
                        break;
                    case '8':
                        selectedVal = "11010";//OrderCodeTypes.Surgery;
                        break;
                    case '9':
                        selectedVal = "11100";//OrderCodeTypes.Pharmacy;
                        break;
                    default:
                        selectedVal = "0";
                }
                $(ddlSelector).val(selectedVal);
                if (selectedVal != "0") {
                    $(ddlSelector).attr('disabled', 'disabled');
                }
                OnChangeCategory(ddlSelector, "#ddlOrderTypeSubCategory", "#hdOrderTypeSubCategoryID");
            }

        },
        error: function (msg) {
        }
    });
}

function OnChangeCategory(categorySelector, ddlSelector, hdSubCategorySelector) {
    var categoryId = $(categorySelector).val();
    if (categoryId != '' && categoryId != "0" && categoryId != null) {
        var jsonData = JSON.stringify({
            categoryId: categoryId
        });
        if ($("#ddlOrderTypeCategory :selected").text() == "Pharmacy") {
            $("#lblSubcategory").html("Generic Drug Name");
            $('#ddlFrequencyList').removeAttr('disabled');
        } else if ($("#ddlOrderTypeCategory :selected").text() == "Medicine") {
            $("#lblSubcategory").html("Order Type Sub-Category");
            $('#ddlFrequencyList').val('10');
            $('#ddlFrequencyList').removeAttr('disabled');
        } else {
            $("#lblSubcategory").html("Order Type Sub-Category");
            $('#ddlFrequencyList').val('10');
            $('#ddlFrequencyList').attr('disabled', 'disabled');
        }
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/GetOrderTypeSubCategories",
            //data: JSON.stringify({ id: $("#hdOrderTypeSubCategoryID").val() }),
            data: jsonData,
            dataType: "json",
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
}

function OnChangeFrequency(txtSelector, ddlSelector) {
    var text = $(ddlSelector + " :selected").text();
    if (text == 'Other') {
        $(txtSelector).val("NA");
        $(txtSelector).prop('readonly', true);
    }
    else {
        $(txtSelector).prop('readonly', false);
        $(txtSelector).val();
    }
}

function selectCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtOrderCode").val(dataItem.CodeDescription);
    $("#hdOrderCodeId").val(dataItem.Code);
    $("#txtCodeDescription").val(dataItem.Name);
}

function selectCodeDescription(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtOrderCode").val(dataItem.CodeDescription);
    $("#hdOrderCodeId").val(dataItem.Code);
    $("#txtCodeDescription").val(dataItem.Name);
}

function OnCodeSelection(e) {
    var subCategoryId = $("#ddlOrderTypeSubCategory").val();
    if (subCategoryId == null)
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

function BindOpenCodes() {
    var jsonData = JSON.stringify({ codetypeid: $('#ddlOrderType').val() });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetOrderCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlOrderCodeType").empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data, function (i, orderCode) {
                items += "<option value='" + orderCode.CodeNumbering + "'>" + orderCode.CodeDescription + "</option>";
            });
            $("#ddlOrderCodeType").html(items);
            if ($("#ddlOrderCodeType") != null && $("#hdOrderCode").val() > 0)
                $("#ddlOrderCodeType").val($("#hdOrderCode").val());
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function BindDateTimePicker() {
    $(".datetimeF").datepicker({
        yearRange: "-100: +15",
        changeMonth: true,
        dateFormat: 'dd/mm/yy',
        changeYear: true,
        showSecond: true,
        timeFormat: 'hh:mm:ss',
        stepHour: 2,
        stepMinute: 10,
        stepSecond: 10
    });
}

function editDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.OpenOrderID;
    EditOrder(id);
}

function OnChangeSubCatgory(ddlSelector) {
    /// <summary>
    /// Called when [change sub catgory].
    /// </summary>
    /// <param name="ddlSelector">The DDL selector.</param>
    /// <returns></returns>
    var value = $(ddlSelector).val();
    var jsonData = JSON.stringify({
        subCategoryId: value
    });
    $('#ddlDosageForm').removeClass('validate[required]');
    $('#ddlDosageAmount').removeClass('validate[required]');
    $('.DrugDDL').hide();
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
                    $("#hdOrderTypeId").val(data.codeTypeId);
                    BindOrderCodesBySubCategoryID(data.codeList, "#ddlOrderCodes", "#hdOrderCodeId");
                    $('.DrugDDL').hide();
                    //$('#ddlDosageForm').addClass('validate[required]');
                    //$('#ddlDosageAmount').addClass('validate[required]');
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
        $("#hdOrderTypeId").val('11');
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
                    $("#hdOrderTypeId").val(data.codeTypeId);
                    BindOrderCodesBySubCategoryID(data.codeList, "#ddlOrderCodes", "#hdOrderCodeId");
                    setTimeout(function () {
                        if ($("#ddlOrderCodes").val() != '0') {
                            $('#txtOrderCode').val($("#ddlOrderCodes :selected").text());
                        }
                    }, 1000);
                }
            },
            error: function (msg) {

            }
        });
    }
}

function BindOrderCodesBySubCategoryID(data, ddlSelector, hdSelector) {
    /// <summary>
    /// Binds the order codes by sub category identifier.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="ddlSelector">The DDL selector.</param>
    /// <param name="hdSelector">The hd selector.</param>
    /// <returns></returns>
    BindDropdownData(data, ddlSelector, hdSelector);
    $('#collapseOpenOrderAddEdit').addClass('in');
}

function ClosePopup() {
    /// <summary>
    /// Closes the popup.
    /// </summary>
    /// <returns></returns>
    $('#divhidepopup').hide();
    $.validationEngine.closePrompt('.formError', true);
    window.location.reload(true);
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
                url: '/GlobalCode/GetSelectedCodeParent',
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
    } else if ($("#hfTabValue").val() == "6" || $("#hfTabValue").val() == "7" || $("#hfTabValue").val() == "9") {
        var orderCodesVal1 = $("#ddlOrderCodes").val();
        var hdOrderType1 = $('#hdOrderType').val();
        if (orderCodesVal1 != '' || hdOrderType1 != '0' || hdOrderType1 != null) {
            var jsonData1 = JSON.stringify({
                code: orderCodesVal1,
                Type: hdOrderType1
            });
            $.ajax({
                type: "POST",
                url: '/GlobalCode/GetSelectedCodeParent',
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonData1,
                success: function (data) {
                    if (data != null) {
                        //$('#ddlOrderTypeCategory').val(data.GlobalCodeCategoryId);
                        $('#hdOrderTypeSubCategoryID').val(data.GlobalCodeId);
                        OnChangeCategory('#ddlOrderTypeCategory', '#ddlOrderTypeSubCategory', '#hdOrderTypeSubCategoryID');
                    }
                },
                error: function (msg) {
                }
            });
        }
    }
    CheckForIsFav();
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
                if ($("#txtOrderCode").val() == "") {
                    $("#txtOrderCode").val($('#ddlOrderCodes option:Selected').text());
                }
            },
            error: function (msg) {

            }
        });
    }
}

var CheckForMultipleActivites = function (orderid) {
    var jsonData = JSON.stringify({
        orderid: orderid,
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/CheckMulitpleOpenActivites",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            if (data == true) {
                $("#ddlOrderStatus option[value='2']").remove();
            } else {
                $("#ddlOrderStatus option[value='2']").remove();
                $('#ddlOrderStatus').append($('<option>', {
                    value: 2,
                    text: 'Administered'
                }));
            }
        },
        error: function (msg) {
        }
    });
    return false;
};

function GetPhysicianAllOrders() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PhysicianFavorites/GetPhysicianAllOrders",
        data: null,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            $("#MostRecentOrdersGrid").empty();
            $("#MostRecentOrdersGrid").html(data);
            $("#MostRecentOrdersGrid1").empty();
            $("#MostRecentOrdersGrid1").html(data);
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function ViewAuthInUnClosedEncounters(encounterid) {
    /// <summary>
    /// Views the authentication.
    /// </summary>
    /// <param name="patientId">The patient identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        encounterId: encounterid
    });
    $.ajax({
        type: "POST",
        url: '/PatientInfo/GetAuthorizationPopup',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $("#authorizationdiv").empty();
                $("#authorizationdiv").html(data);
                $(".hidePopUp").show();
                $("#hfMissAuthEncounterId").val(encounterid);
            }
        },
        error: function (msg) {
        }
    });
}

var CancelUCOrder = function (orderId) {
    var jsonData = JSON.stringify({
        cancelOrderId: orderId
    });
    $.ajax({
        type: "POST",
        url: '/Summary/CancelOpenOrder',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data) {
                ShowMessage("Order Cancelled successfully.", "Success", "success", true);
            } else {
                ShowMessage("Error while Cancelling the Order.", "Success", "success", true);
            }
            BindOrders($("#hdEncounterId").val(), $("#hdPatientid").val());
            $("#hidCancelOrderId").val('');
            $('#' + $('#hidTrId').val()).closest('tr').remove();
            $('#hidTrId').val('');
            $.unblockUI();
            if ($('#gridContentOpenOrder tbody tr').length == 0) {
                //ShowNoOrderPending();
                ShowMessage("All pending orders had been cancelled.", "Success", "success", true);
                setTimeout(window.location.reload(true), 800);
            }
        },
        error: function (msg) {

        }
    });
}

var CheckEncounterEndStatus = function (encid) {
    var jsonData = JSON.stringify({
        encounterid: encid
    });
    $.ajax({
        type: "POST",
        url: '/ActiveEncounter/CheckEncounterEndStatus',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data == 'Success') {
                ShowMessage("Encounter scrub scuuessfully.", "Success", "success", true);
                window.location.reload(true);
            } else {
                ShowMessage("Error while scrubing Encounter.", "Success", "success", true);
            }

        },
        error: function (msg) {

        }
    });
}

var CloseOpenorderPopup = function () {
    $.unblockUI();
    ClosePopup();
    window.location.reload(true);
}

var ShowNoOrderPending = function () {
    $.blockUI({ message: $('#divNoOrderPending'), css: { width: '357px' } });
}

var ViewCancelOrderPopup = function (orderId, id) {
    $('#hidCancelOrderId').val(orderId);
    $('#hidTrId').val(id);
    $.blockUI({ message: $('#divUCCancelOrder'), css: { width: '357px' } });
}

var HideCancelOrderPopup = function (id) {
    $.unblockUI();
    $('#hidTrId').val('');
    $("#hidCancelOrderId").val('');
}


function GetUnclosedEncounters() {
    $.ajax({
        type: "POST",
        url: '/ActiveEncounter/GetUnclosedEncounters',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {},
        success: function (data) {
            BindUnclosedEncountersData(data.unclosedEncounters);
            SetRowColorToUnclosedEncouters();

        },
        error: function (msg) {
        }
    });
}

function BindUnclosedEncountersData(data) {
    var cColumns = [{ "targets": 0, "visible": false },
    {
        "targets": 10,
        "class": "colErrorStatus",
        "mRender": function (data, type, full) {

            var errorStatus = full[10];
            var patientId = full[11];

            var anchortags = "";
            if (errorStatus == "E") {
                anchortags += "<p>Encounter Auto Closed</p>";
            }
            if (errorStatus == "A") {
                anchortags += "<p>Authorization Missing</p>";
            }
            if (errorStatus == "O") {
                anchortags += "<p>Orders Pending</p>";
            }
            return anchortags;
        }
    },
    {
        "targets": 11,
        "mRender": function (data, type, full) {
            var errorStatus = full[10];
            var patientId = full[11];
            var encounterId = full[0];

            var anchortags = "<div style='display:flex'>";
            var editOrder = "BindOrders('" + encounterId + "', '" + patientId + "')";
            var authorization = "return ViewAuthInUnClosedEncounters('" + encounterId + "'); ";
            if (errorStatus == "A") {
                anchortags += '<a href="javascript: void (0); " title="Edit Authorization" onclick="' + authorization + '" style="float: left; margin - right: 7px; width: 15px; "><img src= "../images/Authorization_red.png" /></a>';
            }
            if (errorStatus == "O") {
                anchortags += '<a href="javascript: void (0)" class="" onclick="' + editOrder + '" title="Close Open Orders"><img src= "../images/edit.png" /></a>';
            }
            return anchortags + "</div>";
        }
    }];
    $('#unclosedEncounters').dataTable({
        destroy: true,
        aaData: data,
        bProcessing: true,
        scrollY: "200px",
        scrollCollapse: true,
        paging: false,
        aoColumnDefs: cColumns,
        order: [[10, "asc"]]

    });
}

function BindOpenOrdersinUnclosedPopUp(data) {
    var cColumns = [{ "targets": 0, "visible": false }, {
        "targets": 10,
        "mRender": function (data, type, full) {
            var openOrderId = full[0];

            var CancelOrder = "ViewCancelOrderPopup('" + openOrderId + "',this.id)";
            var editOrder = "GetOrderActivitiesbyOpenOrder('" + openOrderId + "')";
            var anchortags = '<div style="display: flex"><a class="editOpenOrder hideSummary" title="View Order Activities" onclick="' + editOrder + '" id="trOpenOrderId_' + openOrderId + '" style="float: left; margin-right: 7px; width: 15px;" href="javascript:void(0);"><img src= "../images/details-icon.png" /></a><a class="editOpenOrder hideSummary" title="Cancel Order" onclick="' + CancelOrder + '" id="trOpenOrderId_' + openOrderId + '" style="float: left; margin-right: 7px; width: 15px;" href="javascript:void(0);"><img src= "../images/delete_small.png" /></a></div>';
            return anchortags;
        }
    }];
    $('#unclosedOpenOrders').dataTable({
        destroy: true,
        aaData: data,
        bProcessing: true,
        paging: true,
        aoColumnDefs: cColumns

    });
}

var viewCancelOrderActivityPopupinEncounter = function (orderactivityId) {
    $("#hidCancelOrderActivityId").val(orderactivityId);
    $.blockUI({ message: $("#divCancelOrderActivityinEncounter"), css: { width: "357px" } });
};

function GetOrderActivitiesbyOpenOrder(OpenOrderId) {
    var jsonData = JSON.stringify({
        openOrderId: OpenOrderId
    });

    $.ajax({
        type: "POST",
        url: '/ActiveEncounter/GetOrderActivitiesData',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {

            BindOrderActivitiesinUnclosedPopUp(data.orderActivities);
            $("#hdOpenOrderId").val(OpenOrderId);
            $("#OrderActivities").attr("data-toggle", "tab");
            $("#OrderActivities").trigger("click");

        },
        error: function (msg) {
        }
    });
}

function BindOrderActivitiesinUnclosedPopUp(data) {
    var cColumns = [{ "targets": 0, "visible": false },
    { "targets": 1, "visible": false },
    { "targets": 2, "visible": false },
    { "targets": 3, "visible": false },
    {
        "targets": 9,
        "mRender": function (data, type, full) {
            var OrderActivityID = full[2];
            var showEditAction = full[1];
            var status = full[0];

            var anchortags = '<div style="display: flex">';
            if (showEditAction && status != "Closed" && status != "On Bill" && status != "Administered" && status != "Cancel/Revoked") {
                anchortags += '<input type="hidden" id="hdShowEditAction" value="' + showEditAction + '" />';
            }
            return anchortags + "</div>";
        }
    },
    {
        "targets": 10,
        "mRender": function (data, type, full) {
            var status = full[0];
            var showEditAction = full[1];
            var OrderActivityID = full[2];
            var OrderCategoryId = full[3];
            var OrderTypeName = full[4];
            var anchortags = '<div style="display: flex">';
            var editOrderActivity = "EditOrderActivityinEncounter('" + OrderActivityID + "') ";
            var editPharmacyOrderActivity = "EditPharmacyOrderActivity('" + OrderActivityID + "') ";
            var editLabOrderActivity = "EditLabOrderActivity('" + OrderActivityID + "') ";
            var viewCancelOrderActivityPopupinEncounter = "viewCancelOrderActivityPopupinEncounter('" + OrderActivityID + "') ";
            var viewCarePlanAdministerPopup = "ViewCarePlanAdministerPopup(this.id,'" + OrderActivityID + "') ";
            var cancelCarePlanActivity = "CancelCarePlanActivity(this.id,'" + OrderActivityID + "') ";
            var viewCarePlanCancelPopup = "ViewCarePlanCancelPopup(this.id,'" + OrderActivityID + "') ";

            if (status == "Open" || status == "" || status == "Partially Administered") {

                //if (OrderTypeName != "Care Task") {
                var category = OrderCategoryId;
                if (category != 11080) {
                    anchortags += '<a class="editOpenOrderActivity" href="javascript:void(0);" title="Administer Order Activity" onclick="' + editOrderActivity + '" style="float: left; margin-right: 7px; width: 15px;"><img src="../images/medicate_small.png" /></a>';
                }

                anchortags += '<a href="javascript:void(0);" title="Cancel Activity" onclick="' + viewCancelOrderActivityPopupinEncounter + '" style="float: left; margin-right: 7px; width: 15px;"><img src="../images/delete_small.png" /></a>';

                //}
                //else //if (showEditAction && OrderTypeName == "Care Task")
                //{
                //    anchortags += '<a class="edit" href="javascript:void(0);" id="ankAdministerCarplan_' + OrderActivityID + '" title="Administer Care Plan Activity" onclick="' + viewCarePlanAdministerPopup + '" style="float: left; margin-right: 7px; width: 15px;"><img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAG7UlEQVRYR+2Wa2wcZxWGn5nZ2bsd767tjWNn7TSJ47qxvbnaaUF1UqlCSCBBBSlSG1GhghpSNS0lqKJNSGmIqIoAEdH+ACk/kEgUECJSEUVtsiChglMrcW0nceLESbDj2N71etf23mbmG/SN7ShOLyCkun+Y1dH3anb3nPe873dT+JQf5VOuz/8JOAqcOnXqQVVVXwW2LpElXUKIfTt27PirQyCRSIzGN2xYvqy8fEnqZ7JZzp09e6uzs7NGElASiYRoa2tjcnJySQiEw2F6enro7OxUJQEtkUiYLS0tzOZyqIqC2+3G6/U64XK5UJT/ba7ato1pmhQKBSdKpRLCtvH7fPT19UkCLpnZlUgkjC1btlAsldj3vRcYHh7BpetO8bnQ0eexprnQXBpyVFXVIScLCSGwLBPLtJxRFjZMORoONg2T2toaDv/oFSdXd3e3JKDfJrBt2zbnD488spM9e3ajaRrllT7yGdMpUhb2UZi1UFAILvPMYUXBF3SRnzURlo0vqDGTLSEsC1+Zi5l0AdOyCFToZFN5jhx5nePHfoOmqnR1dS0m0NHRgWEY7Pza49x/fweDV4bwrZ5FG6tG09y466dwp2pRZec1STyZWlQ0rMgYrnQU07QohUawxyoxiwZG1U2Mf4UoFQvYNRN0rvsCJ078gV+98Qa6rnPmzJnFBNrb2x0LHnv8CR78UitXRi7gjxYhW4GqunBXzqDOVqIqGlpFFi1XiYKGXZ5Cm4lgWRZWIInIlDs2iLIkRiroWNDYvJLmunZ++ePf8ouf/Ryf1/tBCzbLOVAs8vUnnqS9o50rg1cI3JPHnqhG11x46jJoUzVoqo4WTaJnalBQscPjqFPVmJbAqhhFTFRilAxEeJTSzQruWVtHtFGjkFV489jf+OlPXsPv98tluFiBjRs3ks/n+caTT7F162YuXb5KsGEWktICDU/dNGo6iiYVqEmjpaMotkpojUltWSP9PVcxJIGxEIZhYkfGWR3ZwLJqnevZc6zwNfOn43/ntVcPEwwG6e3tXUxA7gNyGX7zW7t54PPNXLrRj3d5EabKUVUNd3UOJRtyutYi0yjTIRRboa29EVFSCHmX0z10isyIjWWa3BuPEXCHGRodQA3mqfW18Oav3+PwoUPIDe/8+fOLCaxvaWF6epqndj/Npk2buDgwgL8+jz0ednz3xWaxkxEH6yumIFlFpCpENG4yNWrg9XhZ3djAxXdv4fXqLFtVYvDiTWdJqqFZVnrX8+ffvcsrL/+AUCjEpYGBxQTubW4mk8mw5+m9SDvOXzhPoL7gSKqoGv66WUQq7Cjgrs0iJiJs+UwzY0ovIhPEFgJXKEcs0Ea+kGMk108x5UWYFmo4R8zfwl9+/w8OHniJSCTClcHBxQTWNTWRTqd5Zu9zdDzcRP9gD94VBnYqwH3r72N1Uy3dZy4ycWscPZqjggZqGsrJqqOIGa9DQC0rYmU9WJZAKy9gTOkIW6CWF2nwtfHW0W5e+v5+qqqqGLp6dTGBtY2Nzlmw99nnibe18n5fP8FYkahnLU2b65hSh1lV1oZV1Oi/cYaVoXUMpy+jhQpYaZ+zEbkiOcyUd45AJI8x7sYyBXplgXp/K++c/Cf7X3yR6upqrl+7dheBtWtJTU7yzN7vEG9tpae3lxXrfXRsfIC+wffxREuIlI9AoIymlgaGBkZIZzJolQXnvdyKJbYmPA52VRYojbsRpsBVVWRVoJW3T77Hwf0fQWD1mjUkk0mefe55tjy0jsvX+vncVz9L38ULCEvgDpuYWbdzWuohEyszh13LSpgS2zZahYElZRe2895I61hCoIcMVgXivHX0LC8fOPDhCtQ3NDA+Ps53971APN5KVayMlHaZ7IhwCnmWm1hJDzbgrjbmsG2jV5cwZdcSV5UwJmTXNnq0SGlMx5bkoyUaAnHeOdnlrIIPteBOAo8+9mWEO0fJncWccs11HbYwp3QkA4mtjI4tJL6j65CJmXZh23JFzGGpnvx9aVLl7T92ceiHBz+ewLHjJzh3rsc5auUjT7zbIY9f+XHeLdxdJJC6OC44qjgfIebwfMh5EY+38ejOr3w8gfpYzNkqP4lnZmaG6zdufICAnkgkSgsWfBKF7855xxxwS/08p0+fHopGozU+v/+23NKC2yH1ntf8P13O5syY80NOTCn9QizYkc/lGBsbG92+ffsqmc979OjRL8ZisSOqqlYthQJCiInh4eFv79q166RzJZMkpBLzWPX5fC7btuUs1OZHiRXbtp1b9B1xu9+FvmXviqJIIWQIRVHkOrYURXEil8tZgAkUZSwkkyRkaHcVWPjeWRDzRf5bFxZI3D3KNAskzDuTOl0uhQUL6jhqLVHBjyzzb6dri67IsNb1AAAAAElFTkSuQmCC" /></a>';
                //    anchortags += '<a class="edit" href="javascript:void(0);" title="Cancel Care Plan Activity" id="ankCancelCarplan_' + OrderActivityID + '" onclick="' + cancelCarePlanActivity + '" style="float: left; margin-right: 7px; width: 15px;"><img src= "../images/delete_small.png" /></a>';
                //}
                //else if (OrderTypeName == "Care Task") {
                //    anchortags += '<a class="edit" href="javascript:void(0);" title="Cancel Care Plan Activity" id="ankCancelCarplan_' + OrderActivityID + '" onclick="' + viewCarePlanCancelPopup + '" style="float: left; margin-right: 7px; width: 15px;"><img src="../images/delete_small.png" /></a>';
                //}
            }
            return anchortags + "</div>";
        }
    }
    ];
    $('#unclosedOrderActivity').dataTable({
        destroy: true,
        aaData: data,
        bProcessing: true,
        paging: true,
        aoColumnDefs: cColumns

    });
}
var BindOrderAdministeredData = function (data) {

}
function EditOrderActivityinEncounter(id) {
    /// <summary>
    /// Edits the order activity.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({ OpenOrderActivityId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/ActiveEncounter/EditOpenOrderActivity",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {

            $('#NurseAdminAddEditFormDiv').empty();
            $('#NurseAdminAddEditFormDiv').html(data);
            BindOrderAdministeredData(data);
            BindGlobalCodesWithValue("#ddlActivityStatus", 3103, "#hdActivityStatus");
            BindGlobalCodesWithValueWithOrder("#ddlExecutedQuantity", 1011, "#hdExecutedQuantity");
            $("#ddlActivityStatus").val($("#hdActivityStatus").val());
            $("#OrderCode").text($("#hdOrderCode").val());
            //toggleCollapseDivs("#CollapseNurseAdminAddEdit");
            if (!($('#CollapseNurseAdminAddEdit').hasClass('in')))
                $('#CollapseNurseAdminAddEdit').addClass('in');
            $("#ddlActivityStatus option[value='4']").remove();
            var value = $("#hdOrderTypeID").val();
            $("#NurseAdministeredOrdersDiv").validationEngine();
            if (value != '') {
                var jsonD = JSON.stringify({ id: value });
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "/Summary/GetCodeCustomValueById",
                    dataType: "json",
                    async: true,
                    data: jsonD,
                    success: function (data) {
                        $("#OrderTypeName").text(data);
                    },
                    error: function (msg) {

                    }
                });
                $("#hdOpenOrderActivityScheduleID").val(id);
            }
            RemoveActivitesStatus();
            $("#ddlActivityStatus").val('2');
            removeExecutedQuantity();
            //$("#ddlExecutedQuantity").val('1.00');
            CheckExecutedQuantity();
            $("#AdministerOrderActivities").attr("data-toggle", "tab");
            $("#AdministerOrderActivities").trigger("click");
            $("#btnAddOrder").attr("onclick", "SaveOrderActivityinAutoCloseEncounter()");
            $("#btnCancelAdminOrder").attr("onclick", "CancelOrderAdministerinEncounter()");
        },
        error: function (msg) {

        }
    });
}

function SaveOrderActivityinAutoCloseEncounter() {
    var id = $("#hdOpenOrderActivityScheduleID").val();
    var isValid = jQuery("#NurseAdminAddEditFormDiv").validationEngine({ returnIsValid: true });
    if (isValid) {
        var createdDate = new Date();

        if ($("#hdCreatedDate").val() != '') {
            createdDate = new Date($("#hdCreatedDate").val());
        }

        var labtestMinValue = $('#txtLabtestMinVal').val();
        var labtestMaxValue = $('#txtLabtestMaxVal').val();
        var txtActivityQuantity = $('#txtActivityQuantity').val();
        var selectorExecutedQuantity = $('#ddlExecutedQuantity').val();
        var partiallyExecutedBool = false;
        var partiallyExecutedstatus = 1;
        if (txtActivityQuantity != selectorExecutedQuantity) {
            partiallyExecutedBool = true;
            partiallyExecutedstatus = $('input[name=rbtnExecutedCheck]:checked').val();
        }
        var executedate = $("#txtExecutedDate").val();

        var newCustomDate =new Date( executedate.toString().split('00:00:00')[0] + " " + $('#txtExecuteHour').val() + ":" + $('#txtExecuteMin').val());

        var startdate = new Date($("#txtEncounterStartDate").val());
        var enddate = new Date($("#txtEncounterEndDate").val());
        debugger;
        if (newCustomDate < startdate && newCustomDate < enddate) {
            ShowErrorMessage("Execute Date should be between Encounter Start Date and End Date!", true);
        } else {


            var jsonData = JSON.stringify({
                OrderActivityID: id,
                OrderType: $('#hdOrderTypeID').val(),
                OrderCode: $('#hdOrderCode').val(),
                OrderCategoryID: $('#hdOrderCategoryID').val(),
                OrderSubCategoryID: $('#hdOrderSubCategoryID').val(),
                OrderActivityStatus: $('#ddlActivityStatus').val(),
                PatientID: $('#hdPatientId').val(),
                EncounterID: $('#hdEncounterId').val(),
                OrderID: $('#hdOpenOrderID').val(),
                OrderBy: $('#hdOrderBy').val(),
                ExecutedDate: newCustomDate,
                OrderActivityQuantity: $('#txtActivityQuantity').val(),
                OrderScheduleDate: $('#hdOrderScheduleDate').val(),
                PlannedBy: $('#hdPlannedBy').val(),
                PlannedDate: $('#hdPlannedDate').val(),
                PlannedFor: $('#hdPlannedFor').val(),
                ExecutedQuantity: $('#ddlExecutedQuantity').val(),//ddlQuantityList $('#txtExecutedQuantity').val(),
                CreatedDate: createdDate,
                CreatedBy: $("#hdCreatedBy").val(),
                Comments: $('#txtComments').val(),
                IsActive: true,
                ResultValueMin: labtestMinValue,
                ResultValueMax: labtestMaxValue,
                PartiallyExecutedBool: partiallyExecutedBool,
                PartiallyExecutedstatus: partiallyExecutedstatus
            });

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/Summary/SaveOpenOrderActivitySchedule",
                data: jsonData,
                dataType: "json",
                beforeSend: function () { },
                success: function (data) {
                    var msg = "Records Saved successfully !";
                    if (id > 0) {
                        msg = "Records updated successfully";
                    }
                    ShowMessage(msg, "Success", "success", true);
                    $("#NurseAdminAddEditFormDiv").clearForm(true);
                    $('#OrderCode').html('');
                    $('#OrderTypeName').html('');
                    $("#ddlActivityStatus").removeAttr('disabled');
                    //BindOrders($("#hdEncounterId").val(), $("#hdPatientid").val());
                    //BindOrderActivitiesinUnclosedPopUp();
                    GetOrderActivitiesbyOpenOrder($("#hdOpenOrderId").val());
                    //$("#CurrentOrders").trigger("click");

                    $("#OrderActivities").removeAttr("data-toggle");
                    $("#AdministerOrderActivities").removeAttr("data-toggle");

                    if ($('#unclosedOrderActivity tbody tr').length == 1 && $('#unclosedOpenOrders tbody tr').length == 1) {
                        GetUnclosedEncounters();
                        $('#divhidepopup').hide();
                    } else {
                        BindOrders($("#hdEncounterId").val(), $("#hdPatientid").val());
                        BindOrderActivitiesinUnclosedPopUp();
                        $("#CurrentOrders").trigger("click");
                    }
                },
                error: function (msg) {
                }
            });
        }
    }
}

function RemoveActivitesStatus() {
    var showtype = $('#hfTabType').val();
    if (showtype == '2') {
        $("#ddlActivityStatus option[value='3']").remove();
        $("#ddlActivityStatus option[value='10']").remove();
        $("#ddlActivityStatus option[value='11']").remove();
    } else if (showtype == '1') {
        $("#ddlActivityStatus option[value='3']").remove();
    }
    $("#ddlActivityStatus option[value='1']").remove();
    $("#ddlActivityStatus option[value='20']").remove();
    $("#ddlActivityStatus option[value='30']").remove();
    $("#ddlActivityStatus option[value='40']").remove();
}

var removeExecutedQuantity = function () {
    var txtActivityQuantity = $('#txtActivityQuantity').val();
    var selector = $('#ddlExecutedQuantity');
    $(selector).empty();
    $(selector).append(
        $("<option></option>")
            .attr("value", '0')
            .text("--Select--")
    );
    var parsedData = parseFloat(txtActivityQuantity);
    if (parsedData != NaN) {
        for (var i = 1.00; i <= parsedData; i++) {
            $(selector).append(
                $("<option></option>")
                    .attr("value", i)
                    .text(i)
            );
        }
        $(selector).val(parsedData);
    }
}
var CheckExecutedQuantity = function () {
    var txtActivityQuantity = $('#txtActivityQuantity').val();
    var selector = $('#ddlExecutedQuantity');
    var parsedData = parseFloat(txtActivityQuantity);
    if (parsedData != NaN) {
        if (selector.val() != "0") {
            if (parsedData != parseFloat(selector.val())) {
                $('#divExecutedQuantityCheck').show();
                $('#rbtnAdminister1').prop('checked', 'checked');
            } else {
                $('#divExecutedQuantityCheck').hide();
            }
        }
    }
}

var CancelOrderActivityinEncounter = function (orderactivityId) {
    var jsonData = JSON.stringify({ OpenOrderActivityId: orderactivityId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/CancelOrderActivity",
        dataType: "json",
        async: true,
        data: jsonData,
        success: function (data) {
            if (data) {
                $("#OrderActivities").removeAttr("data-toggle");
                $("#AdministerOrderActivities").removeAttr("data-toggle");
                GetOrderActivitiesbyOpenOrder($("#hdOpenOrderId").val());
                var v = $('#unclosedOrderActivity tbody tr').length;
                var c = $('#unclosedOpenOrders tbody tr').length;
                if ($('#unclosedOrderActivity tbody tr').length == 1 && $('#unclosedOpenOrders tbody tr').length == 1) {
                    GetUnclosedEncounters();
                    $('#divhidepopup').hide();
                } else {
                    BindOrders($("#hdEncounterId").val(), $("#hdPatientid").val());
                    BindOrderActivitiesinUnclosedPopUp();
                    $("#CurrentOrders").trigger("click");
                }
            }
        },
        error: function (msg) {

        }
    });
};


var HideCancelOrderActivityPopup = function (orderactivityId) {
    $.unblockUI();
    $("#hidCancelOrderActivityId").val("");
};

var CancelOrderAdministerinEncounter = function () {
    $("#NurseAdminAddEditFormDiv").clearForm(true);
    $('#OrderCode').html('');
    $('#OrderTypeName').html('');
    $("#ddlActivityStatus").removeAttr('disabled');
    $("#AdministerOrderActivities").removeAttr("data-toggle");

    $("#OrderActivities").trigger("click");
};
