var BindDatatable = function (data, tablename, cColumns, pageLength) {
    var lengthmenu = [[10, 20, 30, -1], [10, 20, 30, "All"]];
    if (pageLength === undefined || pageLength === "") {
        pageLength = 10;
    } else {
        lengthmenu = [[pageLength, 10, 20, 30, -1], [pageLength, 10, 20, 30, "All"]];
    }
    $(tablename).dataTable({
        destroy: true,
        aaData: data,
        scrollCollapse: true,
        bProcessing: true,
        paging: true,
        aLengthMenu: lengthmenu,

        pageLength: pageLength,
        aoColumnDefs: cColumns
    });
}

var BindDatatablewithFixedHeader = function (data, tablename, cColumns, Height, pageLength) {
    var lengthmenu = [[10, 20, 30, -1], [10, 20, 30, "All"]];
    if (pageLength === undefined || pageLength === "") {
        pageLength = 10;
    } else {
        lengthmenu = [[pageLength, 10, 20, 30, -1], [pageLength, 10, 20, 30, "All"]];
    }
    $(tablename).dataTable({
        destroy: true,
        aaData: data,
        scrollCollapse: true,
        bProcessing: true,
        paging: true,
        aLengthMenu: lengthmenu,
        scrollY: Height + "px",
        pageLength: pageLength,
        aoColumnDefs: cColumns
    });
}


var BindFavouriteTab = function (data, tablename) {
    var cColumns = [{ "targets": 0, "visible": false },
    {
        "targets": 5,
        "mRender": function (data, type, full) {
            var UserDefinedDescriptionId = full[0];
            var addtofav = "AddFavOrderToOrder('" + UserDefinedDescriptionId + "')";
            var anchortags = "<div style='display:flex'>";
            var openconfirm = "return OpenConfirmPopup('" + UserDefinedDescriptionId + "','Delete Fav','',DeleteFav,null); ";
            anchortags += '<a href="javascript:void(0);" title="Add Order" class="AddOrder1" onclick="' + addtofav + '" style="float: left; margin-right: 7px; margin-left: 5px; width: 15px;"><img src= "../images/edit_small.png" /></a>';

            anchortags += '<a href="javascript:void(0);" title="Remove from Fav" class="FavRemove" onclick="" style="float: left; display: none; margin-right: 7px; margin-left: 5px; width: 15px;"><img src="../images/delete_small.png" /></a>';

            return anchortags + "</div>";
        }
    }];
    BindDatatable(data, tablename, cColumns);
}


var BindMostRecentTab = function (data, tablename) {
    var cColumns = [{ "targets": 0, "visible": false },
    {
        "targets": 4,
        "mRender": function (data, type, full) {
            var OpenOrderID = full[0];
            var addtofav = "AddAsFav('" + OpenOrderID + "')";
            var anchortags = "<div style='display:flex'>";
            var addToOrder = "AddToOrder('" + OpenOrderID + "')";

            anchortags += '<a href="javascript:void(0);" title="Add as fav." class="FavAdd" onclick="' + addtofav + '" style="float: left; margin-right: 7px; margin-left: 5px; width: 15px; /*display: none;*/"><img src="../images/Fav (1).png" /></a>';
            anchortags += '<a href="javascript:void(0);" title="Add to Order." class="OrderAdd" onclick="' + addToOrder + '" style="float: left; margin-right: 7px; margin-left: 5px; width: 15px;"><img src="../images/edit_small.png" /></a>';

            return anchortags + "</div>";
        }
    }];
    BindDatatable(data, tablename, cColumns);
}

var BindSearchTab = function (data, tablename) {
    var cColumns = [{ "targets": 0, "visible": false },
    {
        "targets": 11,
        "mRender": function (data, type, full) {
            var OpenOrderID = full[0];
            var addtofav = "AddAsFav('" + OpenOrderID + "')";
            var anchortags = "<div style='display:flex'>";

            anchortags += ' <a href="javascript:void(0);" title="Add as Favorite" onclick="' + addtofav + '" style="float: left; margin-right: 7px; margin-left: 5px; width: 15px;"><img src= "../images/Fav (1).png" /></a>';

            return anchortags + "</div>";
        }
    }];
    BindDatatable(data, tablename, cColumns);
}

var BindPhyPreviousOrders = function (data, tablename) {
    var cColumns = [{ "targets": 0, "visible": false },
    {
        "targets": 4,
        "mRender": function (data, type, full) {
            var OpenOrderID = full[0];
            var addtofav = "AddAsFav('" + OpenOrderID + "')";
            var anchortags = "<div style='display:flex'>";
            var addToOrder = "AddToOrder('" + OpenOrderID + "')";

            anchortags += '<a href="javascript:void(0);" title="Add as fav." class="FavAdd" onclick="' + addtofav + '" style="float: left; margin-right: 7px; margin-left: 5px; width: 15px; /*display: none;*/"><img src="../images/Fav (1).png" /></a>';
            anchortags += '<a href="javascript:void(0);" title="Add to Order." class="OrderAdd" onclick="' + addToOrder + '" style="float: left; margin-right: 7px; margin-left: 5px; width: 15px;"><img src="../images/edit_small.png" /></a>';

            return anchortags + "</div>";
        }
    }];
    BindDatatable(data, tablename, cColumns, 5);
}

var BindOpenOrders = function (data, tablename) {
    var cColumns = [{ "targets": 0, "visible": false }, { "width": "300px", "targets": 3 },
    {
        "targets": 11,
        "mRender": function (data, type, full) {
            var OpenOrderID = full[0];
            var categoryName = full[4];
            var status = full[6];
            var addtofav = "AddAsFav('" + OpenOrderID + "')";
            var anchortags = "<div style='display:flex'>";
            var pharmacyOrder = "ISOrderPhrmacyOrder('" + OpenOrderID + "') ";
            var radImagingOrder = "EditRadImagingOrder('" + OpenOrderID + "') ";
            var approveOrder = "ApproveOrder('" + OpenOrderID + "') ";
            var cancelOrder = "ViewCancelOrderPopup('" + OpenOrderID + "') ";
            //// Add Operation Type Name instead of category Name
            if (categoryName != "Pathology and Laboratory") {
                anchortags += '<a class="editOpenOrder hideSummary" title="Edit Order" onclick="' + pharmacyOrder + '" style="float: left; margin-right: 7px; width: 15px;" href="javascript:void(0);"><img src="../images/edit_small.png" /></a>';
                anchortags += '<a class="editRadImagingOrder hideSummary" title="Add Order Documents" onclick="' + radImagingOrder + '" style="float: left; margin-right: 7px; width: 15px; display: none" href="#collapseFileUploaderAddEdit"><img src="../images/UploadFiles.png" /></a>';
                if (status == "Waiting For Approval") {
                    anchortags += ' <a class="editOpenOrder hideSummary" title="Approve Pharmacy Order" onclick="' + approveOrder + '" style="float: left; margin-right: 7px; width: 15px;" href="javascript:void(0);"><img src="../images/approval.png" /></a>';
                }

            }
            anchortags += '<a class="editOpenOrder hideSummary" title="Cancel Order" onclick="' + cancelOrder + '" style="float: left; margin-right: 7px; width: 15px;" href="javascript:void(0);"><img src="../images/delete_small.png" /></a>';

            return anchortags + "</div>";
        }
    }];
    BindDatatablewithFixedHeader(data, tablename, cColumns, 300);
}

var BindFutureOrders = function (data, tablename) {
    var cColumns = [];
    BindDatatable(data, tablename, cColumns);
}

var BindOrderActivities = function (data, tablename) {
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
            var editOrderActivity = "EditOrderActivity('" + OrderActivityID + "') ";
            var editPharmacyOrderActivity = "EditPharmacyOrderActivity('" + OrderActivityID + "') ";
            var editLabOrderActivity = "EditLabOrderActivity('" + OrderActivityID + "') ";
            var viewCancelOrderActivityPopup = "ViewCancelOrderActivityPopup('" + OrderActivityID + "') ";
            var viewCarePlanAdministerPopup = "ViewCarePlanAdministerPopup(this.id,'" + OrderActivityID + "') ";
            var cancelCarePlanActivity = "CancelCarePlanActivity(this.id,'" + OrderActivityID + "') ";
            var viewCarePlanCancelPopup = "ViewCarePlanCancelPopup(this.id,'" + OrderActivityID + "') ";

            if (status == "Open" || status == "" || status == "Partially Administered") {
                if (showEditAction && OrderTypeName != "Care Task") {
                    var category = OrderCategoryId;
                    if (category != 11080) {
                        anchortags += '<a class="editOpenOrderActivity" href="javascript:void(0);" title="Administer Order Activity" onclick="' + editOrderActivity + '" style="float: left; margin-right: 7px; width: 15px;"><img src="../images/medicate_small.png" /></a>';
                        anchortags += '<a class="editPharmacyActivity" href="javascript:void(0);" title="Edit Pharmacy Order Activity" onclick="' + editPharmacyOrderActivity + '" style="float: left; margin-right: 7px; width: 15px; display: none;"><img src="../images/medicate_small.png" /></a>';
                        anchortags += '<a class="editLabActivity" href="javascript:void(0);" title="Edit Lab Order Activity" onclick="' + editLabOrderActivity + '" style="float: left; margin-right: 7px; width: 15px; display: none;"><img src="../images/medicate_small.png" /></a>';
                    }
                    anchortags += ' <a href="javascript:void(0);" title="Cancel Activity" onclick="' + viewCancelOrderActivityPopup + '" style="float: left; margin-right: 7px; width: 15px;"><img src= "../images/delete_small.png" /></a>';
                }
                else if (showEditAction && OrderTypeName == "Care Task") {
                    anchortags += '<a class="edit" href="javascript:void(0);" id="ankAdministerCarplan_' + OrderActivityID + '" title="Administer Care Plan Activity" onclick="' + viewCarePlanAdministerPopup + '" style="float: left; margin-right: 7px; width: 15px;"><img src="data:image/png;base64,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" /></a>';
                    anchortags += '<a class="edit" href="javascript:void(0);" title="Cancel Care Plan Activity" id="ankCancelCarplan_' + OrderActivityID + '" onclick="' + cancelCarePlanActivity + '" style="float: left; margin-right: 7px; width: 15px;"><img src="../images/delete_small.png" /></a>';
                }
                else if (OrderTypeName == "Care Task") {
                    anchortags += ' <a class="edit" href="javascript:void(0);" title="Cancel Care Plan Activity" id="ankCancelCarplan_' + OrderActivityID + '" onclick="' + viewCarePlanCancelPopup + '" style="float: left; margin-right: 7px; width: 15px;"><img src="../images/delete_small.png" /></a>';
                }
            }
            return anchortags + "</div>";
        }
    }
    ];

    BindDatatablewithFixedHeader(data, tablename, cColumns, 300);

}
