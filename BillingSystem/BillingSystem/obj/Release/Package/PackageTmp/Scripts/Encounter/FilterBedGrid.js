function FilterBedGrid() {
    var floorname = $("#ddlFloor").val();
    var departmentName = $("#ddlDepartment").val();
    var roomName = $("#ddlRoom").val();
    var jsonData = JSON.stringify({
        floorid: floorname,
        departmentid: departmentName,
        roomid: roomName,
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Encounter/FilterBedGridView",
        data: jsonData,
        dataType: "html",
        success: function (data) {
            $('#physicianGrid').empty();
            $('#physicianGrid').html(data);
            ShowCustomGrid();
        },
        error: function (msg) {

        }
    });
}

$(function () {
    BindSearhDDlData();
});

function BindSearhDDlData() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Encounter/GetFacilityStructureDDlData",
        data: null,
        dataType: "json",
        success: function (data) {
            BindSearchDropdownData(data.flList, '#ddlFloor');
            BindSearchDropdownData(data.dtList, '#ddlDepartment');
            BindSearchDropdownData(data.rmList, '#ddlRoom');
        },
        error: function (msg) {

        }
    });
}

function BindSearchDropdownData(data, ddlSelector) {
    /// <summary>
    /// Binds the dropdown data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="ddlSelector">The DDL selector.</param>
    /// <param name="hdSelector">The hd selector.</param>
    /// <returns></returns>
    $(ddlSelector).empty();
    var items = '<option value="0">--Select--</option>';
    $.each(data, function (i, obj) {
        var newItem = "<option id='" + obj.FacilityStructureId + "'  value='" + obj.FacilityStructureId + "'>" + obj.FacilityStructureName + "</option>";
        items += newItem;
    });

    $(ddlSelector).html(items);
    $(ddlSelector)[0].selectedIndex = 0;
}

var BindDepartment = function () {
    var floorid = $('#ddlFloor').val();
    var jsonData = JSON.stringify({
        floorid: floorid,
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Encounter/GetFloorDepartments",
        data: jsonData,
        dataType: "json",
        success: function (data) {
            BindSearchDropdownData(data.dtList, '#ddlDepartment');
        },
        error: function (msg) {

        }
    });
};

var BindRooms = function () {
    var departmentid = $('#ddlDepartment').val();
    var jsonData = JSON.stringify({
        departmentid: departmentid,
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Encounter/GetFloorDepartmentsRooms",
        data: jsonData,
        dataType: "json",
        success: function (data) {
            BindSearchDropdownData(data.rmList, '#ddlRoom');
        },
        error: function (msg) {

        }
    });
};

function ShowCustomGrid() {
    $('.hitTitle').click(function () {
        $('.showGrid').toggleClass('shown');
    });
    var nowdate = new Date();
    $('#txtStartDate').val(nowdate.getDate() + "/" + (nowdate.getMonth() + 1 + "/" + nowdate.getFullYear()));
    var size = $("#GridAvailableBedList > thead > tr >th").size(); // get total column
    $("#GridAvailableBedList > thead > tr >th").last().remove(); // remove last column
    $("#GridAvailableBedList > thead > tr").prepend("<th></th>"); // add one column at first for collapsible column
    $("#GridAvailableBedList > tbody > tr").each(function (i, el) {
        $(this).prepend(
                $("<td></td>")
                .addClass("collapseCustom")
                .addClass("hoverEff")
                .attr('title', "click for show/hide")
            );

        //Now get sub table from last column and add this to the next new added row
        var table = $("table", this).parent().html();
        //add new row with this subtable
        $(this).after("<tr><td></td><td style='padding:5px; margin:0px; background: #eff0f0;' colspan='" + (size - 1) + "'>" + table + "</td></tr>");
        $("table", this).parent().remove();
        // ADD CLICK EVENT FOR MAKE COLLAPSIBLE
        $(".hoverEff", this).on("click", function () {
            $(this).parent().closest("tr").next().slideToggle(100);
            $(this).toggleClass("expandCustom collapseCustom");
        });
    });

    //by default make all subgrid in collapse mode
    $("#GridAvailableBedList > tbody > tr td.collapseCustom").each(function (i, el) {
        $(this).toggleClass("expandCustom collapseCustom");
        $(this).parent().closest("tr").next().slideToggle(100);
    });
}