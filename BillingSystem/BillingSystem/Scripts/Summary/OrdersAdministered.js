function OpenOrderActivity(openOrderId) {
    /// <summary>
    ///     Opens the order activity.
    /// </summary>
    /// <param name="openOrderId">The open order identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({ orderId: openOrderId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetOpenOrderDetailByOrderId",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $('#NurseAdminAddEditFormDiv').empty();
            $('#NurseAdminAddEditFormDiv').html(data);
            BindGlobalCodesWithValue("#ddlActivityStatus", 3103, "#hdActivityStatus");
            $("#ddlActivityStatus").val(1);
            $("#OrderCode").text($("#hdOrderCode").val());
            //toggleCollapseDivs("#CollapseNurseAdminAddEdit");
            if (!($('#CollapseNurseAdminAddEdit').hasClass('in')))
                $('#CollapseNurseAdminAddEdit').addClass('in');
            var value = $("#hdOrderTypeID").val();
            if (value != '') {
                var jsonD = JSON.stringify({ id: value });
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "/Summary/GetCodeCustomValueById",
                    dataType: "json",
                    async: true,
                    data: jsonD,
                    success: function (data1) {
                        $("#OrderTypeName").text(data1);
                        $("#ddlActivityStatus option[value='4']").remove();
                    },
                    error: function (msg) {

                    }
                });
            }
            RemoveActivitesStatus();
        },
        error: function (msg) {

        }
    });
}

function SaveOrderActivity() {
     /// <summary>
    /// Saves the order activity.
    /// </summary>
    /// <returns></returns>
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
        var jsonData = JSON.stringify({
            OrderActivityID: id,
            OrderType: $('#hdOrderTypeID').val(),
            OrderCode: $('#hdOrderCode').val(),
            OrderCategoryID: $('#hdOrderCategoryID').val(),
            OrderSubCategoryID: $('#hdOrderSubCategoryID').val(),
            OrderActivityStatus: $('#ddlActivityStatus').val(),
            PatientID: $('#hdPatientId').val(),
            EncounterID: $('#hdCurrentEncounterId').val(),
            OrderID: $('#hdOpenOrderID').val(),
            OrderBy: $('#hdOrderBy').val(),
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
                ClearOpenOrderActivityForm();
                BindOrdersonOrderAdministeredSuccess(data);
                toggleCollapseDivs("#NurseAdminOpenOrdersListDiv");
                $('#colActivityList').addClass('in').attr('style', 'height: auto;');

            },
            error: function (msg) {
            }
        });
    }
}

function OnClickNurseAdminTab(selectedTab) {
    /// <summary>
    /// Called when [click nurse admin tab].
    /// </summary>
    /// <param name="selectedTab">The selected tab.</param>
    /// <returns></returns>
    $('#hfTabType').val('2');
    if (selectedTab == 1) {
        $('#divnotesCaptionAddEdit').html('Physician Notes');
        $('#divnotesCaptionListing').html('Physician Notes List');

    } else {
        $('#divnotesCaptionAddEdit').html('Nurse Notes');
        $('#divnotesCaptionListing').html('Nurse Notes List');
    }
    setTimeout(function () {
        $(".NurseAdministeredOrdersDiv").show();
        OrderAdministratorJsCalls();
    }, 2000);

    setTimeout(function () {
        $(".AdministerOrderActivity").show();
        $(".editOpenOrder").hide();
    }, 1000);
    $(".NurseAdministeredOrdersDiv").show();
}

function ClearOpenOrderActivityForm() {
    /// <summary>
    /// Clears the open order activity form.
    /// </summary>
    /// <returns></returns>
    var encounterId = $("#hdCurrentEncounterId").val();
    InitializeDateTimePicker();
    //BindOrderGridForNurseTab(encounterId);
    //BindOrderActivityList(encounterId);
    //BindClosedOrderActivityList(encounterId);
    //BindClosedOrders();
    BindGridsAfterOrder();
    CancelAdministrationOrder();
    //$('#NurseAdminAddEditFormDiv').empty();
    $(".AdministerOrderActivity").show();
    if (!($('#CollapseNurseAdminAddEdit').hasClass('in')))
        $('#CollapseNurseAdminAddEdit').addClass('in');
    $(".editOpenOrder").show();
    var currentdate = new Date();
    var datewithFormat = currentdate.format('mm/dd/yyyy');
    $('#txtOrderStartDate').val(datewithFormat);
    $('#txtOrderEndDate').val(datewithFormat);
    $("#CurrentOrders").trigger("click");
}

function BindOrderGridForNurseTab(encounterid) {
    /// <summary>
    /// Binds the order grid for nurse tab.
    /// </summary>
    /// <param name="encounterid">The encounterid.</param>
    /// <returns></returns>
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

            $('.AdministerOrderActivity').show();
            $('.editOpenOrder').hide();
            SetGridSorting(BindOrdersBySort, "#gridContentOpenOrder");
        },
        error: function (msg) {
            // alert(msg);
        }

    });
}

function toggleCollapseDivs(showDivSelector) {
    /// <summary>
    /// Toggles the collapse divs.
    /// </summary>
    /// <param name="showDivSelector">The show div selector.</param>
    /// <returns></returns>
    $(".openOrderActivity").removeClass("in");
    $(showDivSelector).addClass("in").removeAttr('style');
}

function BindClosedOrders() {
    /// <summary>
    /// Binds the closed orders.
    /// </summary>
    /// <returns></returns>
    var encounterId = $("#hdCurrentEncounterId").val();
    if (encounterId != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindClosedOrders",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                BindList("#ClosedOrdersDiv", data);
                $(".editOpenOrder").hide();
            },
            error: function (msg) {

            }
        });
    }
}

//function OrderAdministratorJsCalls() {
//    /// <summary>
//    /// Orders the administrator js calls.
//    /// </summary>
//    /// <returns></returns>
//    $("#NurseAdministeredOrdersDiv").validationEngine();
//    BindGlobalCodesWithValue("#ddlActivityStatus", 3103, "#hdActivityStatus");
//    $("#ddlActivityStatus option[value='4']").remove();
//    //setTimeout(function() {
//    //    BillActivtiesRowColor();
//    //    RowNurseClosedColor();
//    //}, 1000);
//    BindGlobalCodesWithValue("#ddlFrequencyList", 1024, "#hdFrequencyCode");
//    BindGlobalCodesWithValue("#ddlOrderStatus", 3102, "#hdOrderStatus");
//    BindGlobalCodesWithValueWithOrder("#ddlQuantityList", 1011, "#hdQuantity");
//    BindGlobalCodesWithValueWithOrder("#ddlExecutedQuantity", 1011, "#hdExecutedQuantity");
//    BindCategories("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
//    $("#ddlOrderStatus option[value='4']").remove();
//    $("#ddlOrderStatus option[value='2']").remove();
//    $("#ddlOrderStatus option[value='3']").remove();
//    $("#OpenOrderDiv").validationEngine();
//    InitializeDateTimePicker();
//    $("#ddlQuantityList").val('1.00');
//    $("#ddlExecutedQuantity").val('1.00');
//    $("#ddlOrderStatus").val('1');
//    RemoveActivitesStatus();
//    var tabvalue = $('#hfTabValue').val();
//    if (tabvalue == "11") { //....Nurse tab
//        $('#divAddEditOpenOrders').hide();
//        $('.vitals').show();
//        $(".editOpenOrder").hide();
//    } else {
//        $('.vitals').hide();
//        $(".editOpenOrder").show();
//    }
//    $("#ddlOrderStatus").val('1');
//}

function BindOrderActivityList(encounterId) {
    /// <summary>
    /// Binds the order activity list.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    if (encounterId != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindOpenOrderActivityList",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                BindList("#colActivityListDiv", data);
                BindClosedOrders();
            },
            error: function (msg) {

            }
        });
    }
}

function BindClosedOrderActivityList(encounterId) {
    /// <summary>
    /// Binds the closed order activity list.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    if (encounterId != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindClosedActivityList",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                BindList("#ClosedActivitiesDiv", data);
                //RowNurseClosedColor();
                //BillActivtiesRowColor();
            },
            error: function (msg) {

            }
        });
    }
}

function EditOrderActivity(id) {
    /// <summary>
    /// Edits the order activity.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({ OpenOrderActivityId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/EditOpenOrderActivity",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $('#NurseAdminAddEditFormDiv').empty();
            $('#NurseAdminAddEditFormDiv').html(data);
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
            $('html,body').animate({ scrollTop: $("#CollapseNurseAdminAddEdit").offset().top }, 'fast');
        },
        error: function (msg) {

        }
    });
}

//.....Pharmacy Tab Section Starts.............................

function PharmacyOpenOrderActivity(openOrderId) {
    /// <summary>
    ///     Opens the order activity.
    /// </summary>
    /// <param name="openOrderId">The open order identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({ orderId: openOrderId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetOpenOrderDetailByOrderId",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $('#NurseAdminAddEditFormDiv').empty();
            $('#NurseAdminAddEditFormDiv').html(data);
            BindGlobalCodesWithValue("#ddlActivityStatus", 3103, "#hdActivityStatus");
            $("#ddlActivityStatus").val(1);
            $("#OrderCode").text($("#hdOrderCode").val());
            //toggleCollapseDivs("#CollapseNurseAdminAddEdit");
            if (!($('#CollapseNurseAdminAddEdit').hasClass('in')))
                $('#CollapseNurseAdminAddEdit').addClass('in');
            var value = $("#hdOrderTypeID").val();
            if (value != '') {
                var jsonD = JSON.stringify({ id: value });
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "/Summary/GetCodeCustomValueById",
                    dataType: "json",
                    async: true,
                    data: jsonD,
                    success: function (data1) {
                        $("#OrderTypeName").text(data1);
                        $("#ddlActivityStatus option[value='4']").remove();
                    },
                    error: function (msg) {

                    }
                });
            }
            $('.AddOrderActivity1').hide();
            $('.PharmacyActivity').show();
            RemoveActivitesStatus();
        },
        error: function (msg) {

        }
    });
}

function SavePharmacyOrderActivity() {
    /// <summary>
    ///  Saves the pharmacy order activity.
    /// </summary>
    /// <returns></returns>
    var id = $("#hdOpenOrderActivityScheduleID").val();
    var isValid = jQuery("#NurseAdminAddEditFormDiv").validationEngine({ returnIsValid: true });
    if (isValid) {
        //var createdDate = new Date();
        //if ($("#hdCreatedDate").val() != '') {
        //    createdDate = new Date($("#hdCreatedDate").val());
        //}
        var txtActivityQuantity = $('#txtActivityQuantity').val();
        var selectorExecutedQuantity = $('#ddlExecutedQuantity').val();
        var partiallyExecutedBool = false;
        var partiallyExecutedstatus = 1;
        if (txtActivityQuantity != selectorExecutedQuantity) {
            partiallyExecutedBool = true;
            partiallyExecutedstatus = $('input[name=rbtnExecutedCheck]:checked').val();
        }
        var jsonData = JSON.stringify({
            OrderActivityID: id,
            OrderType: $('#hdOrderTypeID').val(),
            OrderCode: $('#hdOrderCode').val(),
            OrderCategoryID: $('#hdOrderCategoryID').val(),
            OrderSubCategoryID: $('#hdOrderSubCategoryID').val(),
            OrderActivityStatus: $('#ddlActivityStatus').val(),
            PatientID: $('#hdPatientId').val(),
            EncounterID: $('#hdCurrentEncounterId').val(),
            OrderID: $('#hdOpenOrderID').val(),
            OrderBy: $('#hdOrderBy').val(),
            OrderActivityQuantity: $('#txtActivityQuantity').val(),
            OrderScheduleDate: $('#hdOrderScheduleDate').val(),
            PlannedBy: $('#hdPlannedBy').val(),
            PlannedDate: $('#hdPlannedDate').val(),
            PlannedFor: $('#hdPlannedFor').val(),
            ExecutedQuantity: $('#ddlExecutedQuantity').val(),//$('#txtExecutedQuantity').val(),
            CreatedDate: $('#hdCreatedDate').val(),
            CreatedBy: $("#hdCreatedBy").val(),
            Comments: $('#txtComments').val(),
            IsActive: true,
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
                ClearPharmacyActivityForm();
                toggleCollapseDivs("#NurseAdminOpenOrdersListDiv");
                //CancelAdministrationOrder();
                $('#OrderActivityGrid').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
            },
            error: function (msg) {
            }
        });
    }
}

function ClearPharmacyActivityForm() {
    /// <summary>
    ///     Clears the pharmacy activity form.
    /// </summary>
    /// <returns></returns>
    var encounterId = $("#hdCurrentEncounterId").val();
    InitializeDateTimePicker();
    //BindPharmacyGridForNurseTab(encounterId);
    //BindPharmacyOpenActivityList(encounterId);
    //BindPharmacyClosedActivityList(encounterId);
    //BindPharmacyClosedOrders(encounterId);
    BindGridsAfterOrder();
    $('#NurseAdminAddEditFormDiv').empty();
    //$('.editOpenOrder').hide();
    $('.AddOrderActivity1').hide();
    $('.editOpenOrderActivity').hide();
    $('.editOpenOrder').show();
    $('.PharmacyActivity').show();
    $('.editPharmacyActivity').show();
    $('.AdministerOrderActivity').hide();
    var currentdate = new Date();
    var datewithFormat = currentdate.format('mm/dd/yyyy');
    $('#txtOrderStartDate').val(datewithFormat);
    $('#txtOrderEndDate').val(datewithFormat);
    //$('.PharmacyOrderActivity').show();
    //setTimeout(function () {
    //    BillActivtiesRowColor();
    //    RowNurseClosedColor();
    //}, 1000);
}

function BindPharmacyGridForNurseTab(encounterId) {
    /// <summary>
    ///     Binds the pharmacy grid for nurse tab.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({ EncounterId: encounterId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/BindEncounterPharmacyOrderList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#collapseOpenOrderAddEdit").removeClass("in");
            $("#collapseOpenOrderlist").addClass("in");
            BindList("#NurseAdminOpenOrdersListDiv", data);
            $('.AdministerOrderActivity').show();
            $('.editOpenOrder').hide();
            BillActivtiesRowColor();
        },
        error: function (msg) {
        }
    });
}

function BindPharmacyOpenActivityList(encounterId) {
    /// <summary>
    ///     Binds the pharmacy activity list.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    if (encounterId != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindPharmacyOpenActivityList",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                BindList("#colActivityListDiv", data);
                $('.openOrderActivity').addClass('in');
            },
            error: function (msg) {

            }
        });
    }
}

function BindPharmacyClosedActivityList(encounterId) {
    /// <summary>
    ///     Binds the pharmacy activity list.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    if (encounterId != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindPharmacyClosedActivityList",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                BindList("#ClosedActivitiesDiv", data);
            },
            error: function (msg) {

            }
        });
    }
}

function BindPharmacyClosedOrders(encounterid) {
    /// <summary>
    ///     Binds the pharmacy closed orders.
    /// </summary>
    /// <param name="encounterid">The encounterid.</param>
    /// <returns></returns>
    if (encounterid != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindPharmacyClosedOrders",
            data: JSON.stringify({ encounterId: encounterid }),
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                BindList("#ClosedOrdersDiv", data);
                $(".editOpenOrder").hide();
            },
            error: function (msg) {

            }
        });
    }
}

function EditPharmacyOrderActivity(id) {
    /// <summary>
    ///     Edits the pharmacy order activity.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({ OpenOrderActivityId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/EditOpenOrderActivity",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $('#NurseAdminAddEditFormDiv').empty();
            $('#NurseAdminAddEditFormDiv').html(data);
            BindGlobalCodesWithValue("#ddlActivityStatus", 3103, "#hdActivityStatus");
            BindGlobalCodesWithValueWithOrder("#ddlExecutedQuantity", 1011, "#hdExecutedQuantity");
            $("#ddlActivityStatus").val($("#hdActivityStatus").val());
            $("#OrderCode").text($("#hdOrderCode").val());
            //toggleCollapseDivs("#CollapseNurseAdminAddEdit");
            if (!($('#CollapseNurseAdminAddEdit').hasClass('in')))
                $('#CollapseNurseAdminAddEdit').addClass('in');
            $("#ddlActivityStatus option[value='4']").remove();
            var value = $("#hdOrderTypeID").val();
            RemoveActivitesStatus();
            if (value != '') {
                var jsonD = JSON.stringify({ id: value });
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "/Summary/GetCodeCustomValueById",
                    dataType: "json",
                    async: true,
                    data: jsonD,
                    success: function (data1) {
                        $('#CollapseNurseAdminAddEdit').addClass('in').attr('style', 'height: auto;');
                        $("#OrderTypeName").text(data1);
                    },
                    error: function (msg) {

                    }
                });
                $("#hdOpenOrderActivityScheduleID").val(id);
                $('.AddOrderActivity').show();
                $('.AddOrderActivity1').hide();
                $('.PharmacyActivity').show();
            }
            $("#NurseAdministeredOrdersDiv").validationEngine();
            $("#ddlActivityStatus").val('2');
            removeExecutedQuantity();
            //$("#ddlExecutedQuantity").val('1.00');
            CheckExecutedQuantity();
        },
        error: function (msg) {

        }
    });
}

function BillActivtiesRowColor() {
    /// <summary>
    ///     Bills the color of the activties row.
    /// </summary>
    /// <returns></returns>
    $("#OrderActivityGrid tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        if ($actualRow.find('.col10').html().indexOf('Open') != -1) {
            $actualRow.removeClass('rowColor3');
            $actualRow.removeClass('rowColor1');
            $actualRow.removeClass('rowColor2');
        } else if ($actualRow.find('.col10').html().indexOf("Administered") != -1) {
            $actualRow.addClass('rowColor1');
        } else if ($actualRow.find('.col10').html().indexOf('Cancel/Revoked') != -1) {
            $actualRow.addClass('rowColor3');
        } else if ($actualRow.find('.col10').html().indexOf('On Bill') != -1) {
            $actualRow.addClass('rowColor2');
        }
    });
}

//.....Pharmacy Tab Section ends................................

function RemoveActivitesStatus() {
    /// <summary>
    ///     Removes the activites status.
    /// </summary>
    /// <returns></returns>
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

function RowNurseClosedColor() {
    /// <summary>
    ///     Rows the color of the nurse closed.
    /// </summary>
    /// <returns></returns>
    $("#NurseClosedOrdersGrid tbody tr").each(function (i, row) {
        var $actualRow = $(row);
        if ($actualRow.find('.col10').html().indexOf('Open') != -1) {
            $actualRow.removeClass('rowColor3');
            $actualRow.removeClass('rowColor1');
            $actualRow.removeClass('rowColor2');
        } else if ($actualRow.find('.col10').html().indexOf("Administered") != -1) {
            $actualRow.addClass('rowColor1');
        } else if ($actualRow.find('.col10').html().indexOf('Cancel/Revoked') != -1) {
            $actualRow.addClass('rowColor3');
        } else if ($actualRow.find('.col10').html().indexOf('On Bill') != -1) {
            $actualRow.addClass('rowColor2');
        }
    });
}

//function EditLabOrderActivity(id) {
//    var jsonData = JSON.stringify({ OpenOrderActivityId: id });
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        url: "/Summary/EditOpenOrderActivity",
//        dataType: "json",
//        async: true,
//        data: jsonData,
//        success: function (data) {
//            $('#NurseAdminAddEditFormDiv').empty();
//            $('#NurseAdminAddEditFormDiv').html(data);
//            BindGlobalCodesWithValue("#ddlActivityStatus", 3103, "#hdActivityStatus");
//            $("#ddlActivityStatus").val($("#hdActivityStatus").val());
//            $("#OrderCode").text($("#hdOrderCode").val());
//            toggleCollapseDivs("#CollapseNurseAdminAddEdit");
//            $("#ddlActivityStatus option[value='4']").remove();
//            var value = $("#hdOrderTypeID").val();
//            RemoveActivitesStatus();
//            if (value != '') {
//                var jsonD = JSON.stringify({ id: value });
//                $.ajax({
//                    type: "POST",
//                    contentType: "application/json; charset=utf-8",
//                    url: "/Summary/GetCodeCustomValueById",
//                    dataType: "json",
//                    async: true,
//                    data: jsonD,
//                    success: function (data1) {
//                        $("#OrderTypeName").text(data1);
//                    },
//                    error: function (msg) {

//                    }
//                });
//                $("#hdOpenOrderActivityScheduleID").val(id);
//                $('.AddOrderActivity').hide();
//                $('.AddOrderActivity1').hide();
//                $('.editLabActivity').show();
//                //editLabActivity//AddOrderActivity
//            }
//        },
//        error: function (msg) {

//        }
//    });
//}

function ClearLabActivityForm() {
    /// <summary>
    ///     Clears the pharmacy activity form.
    /// </summary>
    /// <returns></returns>
    var encounterId = $("#hdCurrentEncounterId").val();
    InitializeDateTimePicker();
    //BindLabGridForNurseTab(encounterId);
    //BindLabOpenActivityList(encounterId);
    //BindLabClosedActivityList(encounterId);
    //BindLabClosedOrders(encounterId);

    BindGridsAfterOrder();
    //$('#NurseAdminAddEditFormDiv').empty();
    $('.editOpenOrder').show();
    $('.AddOrderActivity1').hide();
    $('.editOpenOrderActivity').hide();

    $('.PharmacyActivity').hide();
    $('.editPharmacyActivity').hide();
    $('.AdministerOrderActivity').hide();
    $('.editLabActivity').show();
    var currentdate = new Date();
    var datewithFormat = currentdate.format('mm/dd/yyyy');
    $('#txtOrderStartDate').val(datewithFormat);
    $('#txtOrderEndDate').val(datewithFormat);

    $('#NurseAdminAddEditFormDiv').clearForm(true);
    //$('.PharmacyOrderActivity').show();
    //setTimeout(function () {
    //    BillActivtiesRowColor();
    //    RowNurseClosedColor();
    //}, 1000);
}

function BindLabGridForNurseTab(encounterId) {
    /// <summary>
    ///     Binds the pharmacy grid for nurse tab.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({ EncounterId: encounterId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/BindEncounterLabOrderList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#collapseOpenOrderAddEdit").removeClass("in");
            $("#collapseOpenOrderlist").addClass("in");
            $('#NurseAdminOpenOrdersListDiv').empty();
            $('#NurseAdminOpenOrdersListDiv').html(data);
            $('.AdministerOrderActivity').show();
            $('.editOpenOrder').hide();
            BillActivtiesRowColor();
        },
        error: function (msg) {
        }
    });
}

function BindLabOpenActivityList(encounterId) {
    /// <summary>
    ///     Binds the pharmacy activity list.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    if (encounterId != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindLabOpenActivityList",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                BindList("#colActivityListDiv", data);
                $('.openOrderActivity').addClass('in');
            },
            error: function (msg) {

            }
        });
    }
}

function BindLabClosedActivityList(encounterId) {
    /// <summary>
    ///     Binds the pharmacy activity list.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    if (encounterId != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindLabClosedActivityList",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                BindList("#ClosedActivitiesDiv", data);
            },
            error: function (msg) {

            }
        });
    }
}

function BindLabClosedOrders(encounterid) {
    /// <summary>
    ///     Binds the pharmacy closed orders.
    /// </summary>
    /// <param name="encounterid">The encounterid.</param>
    /// <returns></returns>
    if (encounterid != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindClosedOrders",
            data: JSON.stringify({ encounterId: encounterid }),
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                BindList("#ClosedOrdersDiv", data);
                $(".editOpenOrder").hide();
            },
            error: function (msg) {

            }
        });
    }
}

function EditLabOrderActivity(id) {
    /// <summary>
    ///     Edits the pharmacy order activity.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({ OpenOrderActivityId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/EditLabOpenOrderActivity",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $('#NurseAdminAddEditFormDiv').empty();
            $('#NurseAdminAddEditFormDiv').html(data);
            BindGlobalCodesWithValue("#ddlActivityStatus", 3103, "#hdActivityStatus");
            BindGlobalCodesWithValueWithOrder("#ddlExecutedQuantity", 1011, "#hdExecutedQuantity");
            $("#ddlActivityStatus").val($("#hdActivityStatus").val());
            setTimeout(function () { $("#OrderCode").text($("#hdOrderCode").val()); }, 100);

            if (!($('#CollapseNurseAdminAddEdit').hasClass('in')))
                $('#CollapseNurseAdminAddEdit').addClass('in');

            if ($("#hdActivityStatus").val() == '4') {
                $("#ddlActivityStatus").val('4');
                $("#ddlActivityStatus").attr('disabled', 'disabled');
            } else {
                $("#ddlActivityStatus").removeAttr('disabled');
                $("#ddlActivityStatus option[value='1']").remove();
                $("#ddlActivityStatus option[value='3']").remove();
                $("#ddlActivityStatus option[value='4']").remove();
                $("#ddlActivityStatus option[value='10']").remove();
                $("#ddlActivityStatus option[value='11']").remove();
                $("#ddlActivityStatus option[value='20']").remove();
                $("#ddlActivityStatus option[value='30']").remove();
                $("#ddlActivityStatus").val('2');
            }

            var value = $("#hdOrderTypeID").val();
            RemoveActivitesStatus();
            if (value != '') {
                var jsonD = JSON.stringify({ id: value });
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "/Summary/GetCodeCustomValueById",
                    dataType: "json",
                    async: true,
                    data: jsonD,
                    success: function (data1) {
                        $("#OrderTypeName").text(data1);
                    },
                    error: function (msg) {

                    }
                });
                $("#hdOpenOrderActivityScheduleID").val(id);
                $('.AddOrderActivity').hide();
                $('.AddOrderActivity1').hide();
                $('.LabActivity').show();
                $('.editLabActivity').show();
                $('.AddLabTest').show();
                BindGlobalCodesWithValue("#ddlResultUOM", 3108, "#hdResultUOM");
                $('#ddlResultUOM').addClass('validate[required]');
                $('#txtLabtestMinVal').addClass('validate[required]');
                $("#NurseAdministeredOrdersDiv").validationEngine();
                $("#ddlActivityStatus").val('2');
                removeExecutedQuantity();
                //$("#ddlExecutedQuantity").val('1.00');
                CheckExecutedQuantity();
            }
        },
        error: function (msg) {

        }
    });
}

function EditLabSpeciman(id) {
    /// <summary>
    ///     Edits the pharmacy order activity.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({ OpenOrderActivityId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/EditLabSpecimanOpenOrderActivity",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $('#LabSpecimanAddEditFormDiv').empty();
            $('#LabSpecimanAddEditFormDiv').html(data);
            $("#OrderCodeLabSpeciman").text($("#hdOrderCodeLabSpeciman").val());

            if (!($('#LabSpecimanAddEditFormDiv').hasClass('in')))
                $('#LabSpecimanAddEditFormDiv').addClass('in');

            $("#ddlActivityStatusLabSpeciman").append($("<option></option>").attr("value", "20").text("Waiting For Specimen"));
            $("#ddlActivityStatusLabSpeciman").append($("<option></option>").attr("value", "30").text("Waiting For Result"));

            $("#ddlActivityStatusLabSpeciman").append("30");

            var value = $("#hdOrderTypeIDLabSpeciman").val();

            if (value != '') {
                var jsonD = JSON.stringify({ id: value });
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "/Summary/GetCodeCustomValueById",
                    dataType: "json",
                    async: true,
                    data: jsonD,
                    success: function (data1) {
                        $("#OrderTypeNameLabSpeciman").text(data1);
                    },
                    error: function (msg) {

                    }
                });
                $("#hdOpenOrderActivityScheduleID").val(id);

                //BindGlobalCodesWithValue("#ddlResultUOM", 3108, "#hdResultUOM");
                BindGlobalCodesWithValue('#ddlTypeOfSpecimanLabSpeciman', 3105, "#hdResultUOMLabSpeciman");
                $('#ddlTypeOfSpecimanLabSpeciman').addClass('validate[required]');
                $("#LabSpecimanAddEditFormDiv").validationEngine();
                $('#CollapseNurseLabSpeciman').addClass('in');
            }
        },
        error: function (msg) {
        }
    });
}

function BindRadGridForNurseTab(encounterId) {
    /// <summary>
    ///     Binds the pharmacy grid for nurse tab.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({ EncounterId: encounterId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/BindEncounterRadOrderList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#collapseOpenOrderAddEdit").removeClass("in");
            $("#collapseOpenOrderlist").addClass("in");
            $('#NurseAdminOpenOrdersListDiv').empty();
            $('#NurseAdminOpenOrdersListDiv').html(data);
            $('.AdministerOrderActivity').show();
            //$('.editOpenOrder').hide();
            BillActivtiesRowColor();
        },
        error: function (msg) {
        }
    });
}

function BindRadOpenActivityList(encounterId) {

    /// <summary>
    ///     Binds the pharmacy activity list.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    if (encounterId != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindRadOpenActivityList",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                BindList("#colActivityListDiv", data);
                $('.openOrderActivity').addClass('in');
            },
            error: function (msg) {

            }
        });
    }
}

function BindRadClosedActivityList(encounterId) {
    /// <summary>
    ///     Binds the pharmacy activity list.
    /// </summary>
    /// <param name="encounterId">The encounter identifier.</param>
    /// <returns></returns>
    if (encounterId != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindRadClosedActivityList",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                BindList("#ClosedActivitiesDiv", data);
            },
            error: function (msg) {

            }
        });
    }
}

function BindRadClosedOrders(encounterid) {
    /// <summary>
    ///     Binds the pharmacy closed orders.
    /// </summary>
    /// <param name="encounterid">The encounterid.</param>
    /// <returns></returns>
    if (encounterid != '') {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/BindClosedOrders",
            data: JSON.stringify({ encounterId: encounterid }),
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                BindList("#ClosedOrdersDiv", data);
                $(".editOpenOrder").hide();
            },
            error: function (msg) {

            }
        });
    }
}

function CancelAdministrationOrder() {
    //$('#NurseAdminAddEditFormDiv').clear(true);
    $("#NurseAdminAddEditFormDiv").clearForm(true);
    $('#OrderCode').html('');
    $('#OrderTypeName').html('');
    $("#ddlActivityStatus").removeAttr('disabled');
    $("#CollapseNurseAdminAddEdit").removeClass('in');
}

function CancelLabSpecimanOrder() {
    $("#LabSpecimanAddEditFormDiv").clearForm(true);
    $('#OrderCodeLabSpeciman').html('');
    $('#OrderTypeNameLabSpeciman').html('');
}

function TakeLabSpeciman(id) {
    //$.blockUI({ message: $('#question'), css: { width: '275px' } });
    if (confirm("Click OK if you taken the Specimen & simultaneously it will generate a bar code. Else Click Cancel.", "Confirm")) {
        UpdateLabOrderActvityStatus(id);
    }
}

function UpdateLabOrderActvityStatus(id) {
    var jsonData = JSON.stringify({
        OrderActivityID: id
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/UpdateLabOrderActvityStatus",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            var msg = "Records Saved successfully !";
            ClearLabActivityForm();
            BindSpecimanGrid();
            ShowMessage(msg, "Success", "success", true);
            $.unblockUI();
            //CancelLabSpecimanOrder();
        },
        error: function (msg) {
        }
    });
}

var BindSpecimanGrid = function () {
    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/GetLabSpecimanOrder",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            BindList("#LabSpecimanAddEditFormDiv", data);
        },
        error: function (msg) {
        }
    });
};

//Done on 12062015
//Added the following method to apply / cancel the validation check according to the selection of Activity Status
function OnChangeActivityStatusInLabTestTab(selector) {
    var id = $(selector).val();
    if (id == 9) {
        $('#ddlResultUOM').removeClass('validate[required]');
        $('#txtLabtestMinVal').removeClass('validate[required]');
    } else {
        $('#ddlResultUOM').addClass('validate[required]');
        $('#txtLabtestMinVal').addClass('validate[required]');
    }
}

$(function () {
    $('#btnYes').click(function () {
        // update the block message 
        $.blockUI({ message: "<h1>Remote call in progress...</h1>" });
        UpdateLabOrderActvityStatus(id);
    });

    $('#btnNo').click(function () {
        $.unblockUI();
        return false;
    });


    $('#btnApprovePharmacyOrder').click(function () {
        // update the block message 
        //$.blockUI({ message: "<h1>Remote call in progress...</h1>" });
        var orderId = $('#hidApprovePharmacyOrderID').val();
        ApprovePharmacyOrder(orderId);
    });

    $('#btnDisapprovePharmacyOrder').click(function () {
        var orderId = $('#hidApprovePharmacyOrderID').val();
        //$.unblockUI();
        CancelPharmacyOrder(orderId);
        return false;
    });


    $('#btnAdministerPatientCareActivity').click(function () {
        var careplanid = $('#hidCareplanActivityId').val();
        var ankId = $('#hidtableTrId').val();
        AdministerCarePlanActivity(ankId, careplanid);
    });

    $('#btnCancelAdministerPatientCareActivity').click(function () {
        $('#hidCareplanActivityId').val('');
        $('#hidtableTrId').val('');
        $.unblockUI();
        return false;
    });

    $('#btnCancelPatientCareActivity').click(function () {
        var careplanid = $('#hidCancelCareplanActivityId').val();
        var ankId = $('#hidCanceltableTrId').val();
        CancelCarePlanActivity(ankId, careplanid);
    });

    $('#btnPatientCareActivity').click(function () {
        $('#hidCancelCareplanActivityId').val('');
        $('#hidCanceltableTrId').val('');
        $.unblockUI();
        return false;
    });

});

var CancelOrderActivity = function (orderactivityId) {
    //if (confirm("Do you want to cancel this Activity? ")) {
    var jsonData = JSON.stringify({ OpenOrderActivityId: orderactivityId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/CancelOrderActivity",
        dataType: "json",
        async: true,
        data: jsonData,
        success: function (data) {
            if (data.Status) {
                BindGridsAfterOrder();
                BindOrderActivities(data.OpenOrderActivityList, "#OrderActivity");
                ShowMessage("Success", "Success", "success", true);
                $('#CollapseNurseAdminAddEdit').removeClass('in');
            }
        },
        error: function (msg) {

        }
    });
    //}
};

var AdministerCarePlanActivity = function (e, id) {
    //if (confirm("Do you want to Administer this Activity? ")) {
    var encounterId = $('#hdCurrentEncounterId').val();
    var jsonData = JSON.stringify({ careplanActivityId: id, encounterid: encounterId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/AdministerCarePlanActivity",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            var trObject = $('#' + e).closest('tr');
            $('#' + e).closest('tr').remove();
            $('#ClosedActivitiesDiv').empty().html(data);
        },
        error: function (msg) {

        }
    });
    //}
}

var CancelCarePlanActivity = function (e, id) {
    //if (confirm("Do you want to Cancel this Activity? ")) {
    var encounterId = $('#hdCurrentEncounterId').val();
    var jsonData = JSON.stringify({ careplanActivityId: id, encounterid: encounterId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/CancelCarePlanActivity",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            var trObject = $('#' + e).closest('tr');
            $('#' + e).closest('tr').remove();
            $('#ClosedActivitiesDiv').empty().html(data);
        },
        error: function (msg) {

        }
    });
    //}
}

var TableHtml = function () {
    var conatiner = '<div id="gridContentOpenOrderClosedActivity">';
    var table = '<table class="table table-grid" id="OrderClosedActivityGrid" data-swhgcallback="" data-swhgcontainer="gridContentOpenOrderClosedActivity" data-swhgajax="true">';
    var tableHead = '<thead>' +
        '<tr class="gridHead">' +
        '<th scope="col">' +
        '<a data-swhglnk="true">Order Type</a>            </th>' +
        '<th scope="col">' +
        '<a data-swhglnk="true">Order Code</a>            </th>' +
        '<th scope="col">' +
        '<a data-swhglnk="true">Order Description</a>            </th>' +
        '<th scope="col">' +
        '<a data-swhglnk="true">Order Category</a>            </th>' +
        '<th scope="col">' +
        '<a data-swhglnk="true">Order Sub-Category</a>            </th>' +
        '<th scope="col">' +
        '<a data-swhglnk="true">Scheduled On</a>            </th>' +
        '<th scope="col">' +
        '<a data-swhglnk="true">Administered On</a>            </th>' +
        '<th scope="col">' +
        '<a data-swhglnk="true">Status</a>            </th>' +
        '<th scope="col">' +
        '<a data-swhglnk="true">Quantity Ordered</a>            </th>' +
        '<th scope="col">' +
        'Quantity Executed            </th>' +
        '<th scope="col">' +
        'Comments            </th>' +
        '</tr>' +
        '</thead>';
    var tableBodyTag = " <tbody> </tbody>";
    var tableClose = "</table></div>";
    var stringtoReturn = conatiner + table + tableHead + tableBodyTag + tableClose;
    return stringtoReturn;
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

var OnChangeOutstandingOrder = function () {

    if ($('#chkOutstandingOrder').prop('checked')) {
        var currentDate = $.datepicker.formatDate('mm/dd/yy', new Date());
        var startDate = $('#txtOrderStartDate').val();
        if (startDate == currentDate) {
            ShowMessage("Future date shoud be graterthan today's date", "Warning", "warning", true);
            $('#txtOrderStartDate').val('');
            $('#chkOutstandingOrder').prop('checked', false);
            return false;
        } else if (startDate == "" || startDate == null) {
            ShowMessage("Enter The Start date", "Warning", "warning", true);
            $('#chkOutstandingOrder').prop('checked', false);
            return false;
        }

        //var todayDate = new Date();
        //var startDateCustom = process(startDate);
        //var customdate = new Date(todayDate.getDate() + '/' + todayDate.getMonth() + 1 + '/' + todayDate.getFullYear());
        //if (startDateCustom > customdate) {

        //} else {
        //    //ShowMessage('Start date should be greater than currentDate, for future orders', "Warning", "warning", true);
        //}
    }
}

function process(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

var OrderDateCheck = function () {

}

var ApproveOrder = function (id) {
    $('#hidApprovePharmacyOrderID').val(id);
    $.blockUI({ message: $('#divApprovePharmacyOrder'), css: { width: '357px' } });
}

var CancelPharmacyOrder = function (id) {
    $.unblockUI();
    $("#txtApprovalReason").val('');
}

//--- 
var ViewCarePlanAdministerPopup = function (e, id) {
    $('#hidCareplanActivityId').val(id);
    $('#hidtableTrId').val(e);
    $.blockUI({ message: $('#divAdministerPatientCareActivity'), css: { width: '357px' } });
}

var HideViewCarePlanAdministerPopup = function (id) {
    $.unblockUI();
    $("#hidCareplanActivityId").val('');
}

var ViewCarePlanCancelPopup = function (e, id) {
    $('#hidCancelCareplanActivityId').val(id);
    $('#hidCanceltableTrId').val(e);
    $.blockUI({ message: $('#divCancelPatientCareActivity'), css: { width: '357px' } });
}


function OrderAdministratorJsCalls() {
    /// <summary>
    /// Orders the administrator js calls.
    /// </summary>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/BindOrderAdministratorOrder",
        data: null,
        dataType: "json",
        async: true,
        success: function (data) {
            if (data != null) {
                BindSelectBoxWithGC(data.listActivityList, "#ddlActivityStatus", "#hdActivityStatus");
                BindSelectBoxWithGC(data.listFrequencyList, "#ddlFrequencyList", "#hdFrequencyCode");
                BindSelectBoxWithGC(data.listOrderStatus, "#ddlOrderStatus", "#hdOrderStatus");
                BindSelectBoxWithGC(data.listQualityList, "#ddlQuantityList", "#hdQuantity");
                BindSelectBoxWithGC(data.listQualityList, "#ddlExecutedQuantity", "#hdExecutedQuantity");
                BindCategoriesInSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");


                $("#NurseAdministeredOrdersDiv").validationEngine();
                $("#ddlActivityStatus option[value='4']").remove();
                $("#ddlOrderStatus option[value='4']").remove();
                $("#ddlOrderStatus option[value='2']").remove();
                $("#ddlOrderStatus option[value='3']").remove();
                $("#OpenOrderDiv").validationEngine();
                InitializeDateTimePicker();
                $("#ddlQuantityList").val('1.00');
                $("#ddlExecutedQuantity").val('1.00');
                $("#ddlOrderStatus").val('1');
                RemoveActivitesStatus();
                var tabvalue = $('#hfTabValue').val();
                if (tabvalue == "11") { //....Nurse tab
                    $('#divAddEditOpenOrders').hide();
                    $('.vitals').show();
                    $(".editOpenOrder").hide();
                } else {
                    $('.vitals').hide();
                    $(".editOpenOrder").show();
                }
                $("#ddlOrderStatus").val('1');

            }
        },
        error: function (msg) {

        }
    });

    //BindGlobalCodesWithValue("#ddlActivityStatus", 3103, "#hdActivityStatus");
    //BindGlobalCodesWithValue("#ddlFrequencyList", 1024, "#hdFrequencyCode");
    //BindGlobalCodesWithValue("#ddlOrderStatus", 3102, "#hdOrderStatus");
    //BindGlobalCodesWithValueWithOrder("#ddlQuantityList", 1011, "#hdQuantity");
    //BindGlobalCodesWithValueWithOrder("#ddlExecutedQuantity", 1011, "#hdExecutedQuantity");
}


var getTabCategoryId = function () {
    var selectedVal = "0";

    if ($("#hfTabValue").length > 0 && $("#hfTabValue").val() != "") {
        var tabvalue = $("#hfTabValue").val();
        switch (tabvalue) {
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
                selectedVal = "0";
                break;
            case "10":
            case "11":
                selectedVal = "0";
                break;
            case "6":
                selectedVal = "11080"; // OrderCodeTypes.PathologyandLaboratory;
                break;
            case "7":
                selectedVal = "11070"; //OrderCodeTypes.Radiology;
                break;
            case "8":
                selectedVal = "11010"; //OrderCodeTypes.Surgery;
                break;
            case "9":
                selectedVal = "11100"; //OrderCodeTypes.Pharmacy;
                LoadMarFormList();
                break;
            default:
                selectedVal = "0";
        }
    }
    return selectedVal;
}



var ApprovePharmacyOrderOld = function (id) {
    if ($('input[name=rbtnApproveOrder]:checked').val() == "1") {
        if ($('#txtApprovalReason').val() == "") {
            $('#txtApprovalReason').val("Approved By Pharmacy");
        }
    } else {
        if ($('#txtApprovalReason').val() == "") {
            ShowMessage("Fill the Comments!", "Warning", 'warning', true);
            return false;
        }
    }
    // Call the method to cancel 
    var jsonData = JSON.stringify({ id: id, type: $('input[name=rbtnApproveOrder]:checked').val(), comment: $('#txtApprovalReason').val() });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Summary/ApprovePharmacyOrder",
        dataType: "json",
        async: true,
        data: jsonData,
        success: function (data) {
            $.unblockUI();
            GetMostRecentOrders();
            GetPhysicianAllOrders();
            BindGridsAfterOrder();
            $("#txtApprovalReason").val('');
        },
        error: function (msg) {
        }
    });
}


function ApprovePharmacyOrder(id) {
    if ($('input[name=rbtnApproveOrder]:checked').val() == "1") {
        if ($('#txtApprovalReason').val() == "") {
            $('#txtApprovalReason').val("Approved By Pharmacy");
        }
    } else {
        if ($('#txtApprovalReason').val() == "") {
            ShowMessage("Fill the Comments!", "Warning", 'warning', true);
            return false;
        }
    }
    var encounterId = $("#hdCurrentEncounterId").length > 0 ? $("#hdCurrentEncounterId").val() : 0;
    var categoryId = getTabCategoryId();
    if (encounterId > 0) {
        // Call the method to cancel 
        var jsonData = JSON.stringify({ id: id, type: $('input[name=rbtnApproveOrder]:checked').val(), comment: $('#txtApprovalReason').val(), encounterId: encounterId, categoryId: categoryId });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Summary/ApprovePharmacyOrder",
            dataType: "json",
            async: true,
            data: jsonData,
            success: function (data) {
                if (data != null && data != "") {
                    $.unblockUI();
                    BindList("#ClosedOrdersDiv", data.closedorderslist);
                    BindList("#ClosedActivitiesDiv", data.closedorderActivityslist);
                    BindList("#colActivityListDiv", data.openorderActivityslist);
                    BindList("#NurseAdminOpenOrdersListDiv", data.openOrderslist);
                    BindList("#LabSpecimanAddEditFormDiv", data.labWaitingSpecimenList);

                    if ($("#MostRecentOrdersGrid").length > 0) {
                        $("#MostRecentOrdersGrid").empty();
                        $("#MostRecentOrdersGrid").html(data.mostRecentOrders);
                    }
                    if ($("#MostRecentOrdersGrid1").length > 0) {
                        $("#MostRecentOrdersGrid1").empty();
                        $("#MostRecentOrdersGrid1").html(data.mostRecentOrders);
                    }
                    ShowHideActionButton();
                    $("#txtApprovalReason").val('');
                }
                else
                    ShowMessage("Request couldn't be completed at the moment. Try again later!", "Ohh", "error", true);
            },
            error: function (msg) {
            }
        });
    }
}