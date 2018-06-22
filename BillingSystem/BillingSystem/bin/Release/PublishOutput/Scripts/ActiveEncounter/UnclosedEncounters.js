$(function () {
    SetRowColorToUnclosedEncouters();
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
});

function SetRowColorToUnclosedEncouters() {
    /// <summary>
    /// Sets the row color to unclosed encouters.
    /// </summary>
    /// <returns></returns>
    $("#UnclosedEncountersList tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        if ($actualRow.find('.colErrorStatus').html().indexOf('O') != -1) {
            $actualRow.addClass('rowColor3');
        }
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
    var encounterId = $("#hdCurrentEncounterId").val();
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
    var encounterId = $("#hdCurrentEncounterId").val();
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
                url: '/Home/GetSelectedCodeParent',
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

var CloseOpenorderPopup=function() {
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

