$(function () {
    JsCalls();
    BindGlobalCodesDropdownData(); //Bind Global Code Dropdown (Denial status and Type) 
});

var blockNumber = 2;        //Infinate Scroll starts from second block
var noMoreData = false;
var inProgress = false;

function JsCalls() {
    $("#DenialFormDiv").validationEngine();

    //$("#dtDenialSetStartDate").datepicker({
    //    yearRange: "-130: +0",
    //    changeMonth: true,
    //    dateFormat: 'dd/mm/yy',
    //    changeYear: true
    //});
    //$("#dtDenialSetEndDate").datepicker({
    //    yearRange: "-130: +0",
    //    changeMonth: true,
    //    dateFormat: 'dd/mm/yy',
    //    changeYear: true
    //});
    //$("#dtDenialStartDate").datepicker({
    //    yearRange: "-130: +0",
    //    changeMonth: true,
    //    dateFormat: 'dd/mm/yy',
    //    changeYear: true
    //});
    //$("#dtDenialEndDate").datepicker({
    //    yearRange: "-130: +0",
    //    changeMonth: true,
    //    dateFormat: 'dd/mm/yy',
    //    changeYear: true
    //});

    $(".collapseTitle").bind("click", function () {
        $.validationEngine.closePrompt(".formError", true);
    });



}

function editDetails(e) {

    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.DenialSetNumber;
    EditDenial(id);
}
function deleteDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.DenialSetNumber;
    DeleteDenial(id);
}

function SaveDenial(id) {
    var isValid = jQuery("#DenialFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        id = $("#hdDenialId").val();
        var txtDenialSetDescription = $("#txtDenialSetDescription").val();
        var dtDenialSetStartDate = $("#dtDenialSetStartDate").val();
        var dtDenialSetEndDate = $("#dtDenialSetEndDate").val();
        var txtDenialCode = $("#txtDenialCode").val();
        var txtDenialDescription = $("#txtDenialDescription").val();
        var txtDenialExplain = $("#txtDenialExplain").val();
        var ddlDenialStatus = $("#ddlDenialStatus").val();
        var ddlDenialType = $("#ddlDenialType").val();
        var dtDenialStartDate = $("#dtDenialStartDate").val();//$("#txtOrderedDateTime").val();
        var dtDenialEndDate = $("#dtDenialEndDate").val();

        
        var jsonData = JSON.stringify({
            DenialSetNumber: id,
            DenialSetDescription: txtDenialSetDescription,
            DenialSetStartDate: dtDenialSetStartDate,
            DenialSetEndDate: dtDenialSetEndDate,
            DenialCode: txtDenialCode,
            DenialDescription: txtDenialDescription,
            DenialExplain: txtDenialExplain,
            DenialStatus: ddlDenialStatus,
            DenialType: ddlDenialType,
            DenialStartDate: dtDenialStartDate,
            DenialEndDate: dtDenialEndDate,
            IsActive: "1",
            Isdeleted:0,
        });
        $.ajax({
            type: "POST",
            url: '/Denial/SaveDenial',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearAll();
                BindDenialGrid(blockNumber);
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

function EditDenial(id)  {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/Denial/GetDenialCodesData',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindDenialCodeData(data);
            $('#collapseOne').addClass('in');
            InitializeDateTimePicker();//initialize the datepicker by ashwani
        },
        error: function (msg) {
        }
    });
}


function BindDenialCodeData(data) {
    $("#hdDenialId").val(data.DenialSetNumber);
    $("#txtDenialSetDescription").val(data.DenialSetDescription);
    $("#dtDenialSetStartDate").val(data.DenialSetStartDate);
    $("#dtDenialSetEndDate").val(data.DenialSetEndDate);
    $("#txtDenialCode").val(data.DenialCode);
    $("#txtDenialDescription").val(data.DenialDescription);
    $("#txtDenialExplain").val(data.DenialExplain);
    $("#ddlDenialStatus").val(data.DenialStatus);
    $("#ddlDenialType").val(data.DenialType);
    $("#dtDenialStartDate").val(data.DenialStartDate);
    $("#dtDenialEndDate").val(data.DenialEndDate);
}

function ViewDenial(id) {
    
    var txtDenialId = id;
    var jsonData = JSON.stringify({
        Id: txtDenialId,
        ViewOnly: 'true'
    });
    $.ajax({
        type: "POST",
        url: '/Denial/GetDenial',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            
            if (data) {
                $('#DenialDiv').empty();
                $('#DenialDiv').html(data);
                $('#collapseOne').addClass('in');
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

function DeleteDenial() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/Denial/DeleteDenial',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindDenialGrid();
                    ShowMessage("Records Deleted Successfully", "Success", "success", true);
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
}


function BindDenialGrid(bn) {
    var jsonData = JSON.stringify({
        blockNumber: bn
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Denial/BindDenialList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#DenialListDiv").empty();
            $("#DenialListDiv").html(data);
        },
        error: function (msg) {
            alert(msg);
        }

    });
}

function ClearForm() {
    $("#DenialFormDiv").clearForm();
    $('#collapseOne').removeClass('in');
    $('#collapseTwo').addClass('in');
}

function ClearAll() {

    ClearForm();
    $.validationEngine.closePrompt(".formError", true);
    var jsonData = JSON.stringify({
        Id: 0,
    });
    $.ajax({
        type: "POST",
        url: '/Denial/ResetDenialForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {

                $('#DenialFormDiv').empty();
                $('#DenialFormDiv').html(data);
                $('#collapseTwo').addClass('in');
                
                InitializeDateTimePicker();//initialize the datepicker by ashwani
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



//function ExportToExcel1() {
    
//    var searchText = $("#SearchCodeOrDesc").val();
//    var jsonData = JSON.stringify({
//        searchText: searchText
//    });
//    $.ajax({
//        type: "POST",
//        url: '/Denial/ExportDenialToExcel',
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        data: jsonData,
//        });
//}


//function ExportToExcel() {
//    
//    var searchText = 't';//$("#SearchCodeOrDesc").val();
   

//        $('#btnExportExcel').click(function() {

//            window.location.href = '/Denial/ExportDenialToExcel?searchText=' + searchText;

//        });
   
//}

var ExportToExcel1 = function () {
    var item = $("#btnExportExcel");
    var hrefString = item.attr("href");
    var controllerAction = hrefString;
   
    var searchText = $("#SearchCodeOrDesc").val();
    var hrefNew = controllerAction + "?searchText=" + searchText ;
    item.removeAttr('href');
    item.attr('href', hrefNew);
    return true;
}



function BindGlobalCodesDropdownData() {
    /// <summary>
    /// Binds the global codes dropdown data.
    /// </summary>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        url: "/Denial/BindGlobalCodesDropdownData",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data != null) {
                BindDropdownData(data.listDenialStatus, '#ddlDenialStatus', '');
                BindDropdownData(data.listDenialType, "#ddlDenialType", "");
                }
        },
        error: function (msg) {
        }
    });
}




function AppendDataToDenialCodeGrid(data, userId) {
    
    var html = "";
    if (data.length > 0) {
        $.each(data, function (i, obj) {
            html += "<tr class=\"gridRow\">";
            //if (userId == 1) {
            //html += "<td><input type=\"checkbox\" value=\"" + obj.DiagnosisTableNumberId + "\" name=\"assignChkBx\" id=\"assignChkBx\" class=\"check-box\"></td>";
            //}
            html += "<td class=\"col1\">" + obj.DenialSetDescription + "</td>";
            html += "<td class=\"col2\">" + obj.DenialCode + "</td>";
            html += "<td class=\"col3\">" + obj.DenialDescription + "</td>";
            html += "<td class=\"col4\">" + obj.DenialExplain + "</td>";
            html += "<td class=\"col5\">" + obj.DenialStatusStr + "</td>";
            html += "<td class=\"col6\">" + obj.DenialTypeStr + "</td>";
            html += "<td class=\"col11\">";
            html += "<a class=\"unFav\" style=\"float: left; margin-right: 7px; width: 15px;\" onclick=\"EditDenial('" + obj.DenialSetNumber + "') \" href=\"javascript:;\">";
            html += "<img src=\"/images/edit.png\">";
            html += "</a>";
            //html += "<a class=\"unFav\" style=\"float: left; margin-right: 7px; width: 15px;\" onclick=\"return DeleteDiagnosisCode('" + obj.DiagnosisTableNumberId + "'); \" title=\"Remove\" href=\"javascript:;\">";
            html += "<a class=\"unFav\" style=\"float: left; margin-right: 7px; width: 15px;\" onclick=\"return OpenConfirmPopup('" + obj.DenialSetNumber + "','Delete Denial Code','',DeleteDenial,null); \" title=\"Remove\" href=\"javascript:;\">";

            html += "<img src=\"/images/delete.png\">";
            html += "</a>";
            html += "</td>";
            html += "</tr>";
        });
    }
    return html;
}


function SortDenialCodeGrid(event) {
    var url = "";
    var searchText = $("#SearchCodeOrDesc").val();
    if (searchText != "" && searchText != null) {
        url = "/Home/GetFilteredCodes";
        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
            url += "?text=" + searchText + "&searchType=" + 19 + "&drugStatus=" + 0 + "&" + event.data.msg;
        }
    } else {
         url = "/Denial/BindDenialList";
        if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
            url += "?blockNumber=" + blockNumber + "&" + event.data.msg;
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
           $("#DenialListDiv").empty();
            $("#DenialListDiv").html(data);
        },
        error: function () {
        }
    });
}