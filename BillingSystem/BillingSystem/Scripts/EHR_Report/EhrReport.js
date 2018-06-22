/*
Owner: Amit Jain
On: 11092014
Purpose: Bind the Codes based on Code Type Id
*/
function BindCodes() {
    var jsonData = JSON.stringify({ id: $('#CPTCodeType').val() });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Admin/GetOrderCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlOrderCodes").empty();
            $("#ddlOrderCodes").append('<option value="0">--Select One--</option>');

            /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
            $.each(data, function (i, code) {
                $("#ddlOrderCodes").append('<option value="' + code.Value + '">' + code.Text + '</option>');
            });
        },
        error: function (msg) {
            Console.log(msg);
        }
    });
    return false;
}
//Owner:Shivani
//Purpose: Save Order to database
function AddOrder() {
    //var orderedDateTime = $("#txtOrderedDateTime").val();
    //var status = $("#txtStatus").val();
    var orderedDateTime = new Date($.now());
    var status = "Open";
    var orderType = $("#CPTCodeType").val();
    var code = $("#ddlOrderCodes").val();
    var unitValue = $("#txtUnits").val();
    var frequency = $("#FrequencyId").val();
    var startDate = $("#txtStartDate").val();
    var endDate = $("#txtEndDate").val();
    var comments = $("#txtComments").val();
    var encounterId = "100220141";//$("#txtOrderedDateTime").val();
    var orderId = 0; //$("#txtOrderedDateTime").val();
    var unitType = $("#ShortType").val();
    var jsonData = JSON.stringify({
        id: orderId, OrderedDateTime: orderedDateTime, Status: status, OrderType: orderType,
        Code: code, UnitValue: unitValue, Frequency: frequency, StartDate: startDate, EndDate: endDate,
        Comments: comments, EncounterId: encounterId, OrderId: orderId, UnitType: unitType
    });
    var message = "Order is saved successfully";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Admin/AddOrder",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            //Append Data to grid
            if (data == "1") {
                alert(message);
                BindOrderGrid();
            }

            else if (data == "2") {
                alert("Order is updated successfully");
                BindOrderGrid();
            }

            else {
                alert("Unable to save! Please try after sometime");
            }
        },
        error: function (msg) {

        }
    });
}
function BindOrderGrid() {

    var encounterId = "100220141";
    var jsonData = JSON.stringify({ EncounterId: encounterId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Admin/BindEncounterOrderList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
          //  $("#lstEncounterOrder").html(data);
        },
        error: function (msg) {
            alert(msg);
        }

    });
}

$(document).ready(function () {
    //BindOrderGrid();
});

function EditOrder(id, orderdatetime) {
    //$("#txtOrderedDateTime").val(orderdatetime);
    $("#hdnOrderId").val(id);
    var jsonData = JSON.stringify({ orderId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Admin/GetOrderDetailById",
        dataType: "json",
        async: true,
        data: jsonData,
        success: function (data) {
            alert(data.orderId); alert(data.Comments);
            $("#txtOrderedDateTime").val(data.OrderedDateTime);
        },
        error: function (msg) {
            alert(msg);
        }

    });
}

/*
    Owner: Vinoth
    On: 17092014
    Purpose: Add CSS style for Search SLider
    */

	$(document).ready(function(){
	    $(".menu").click(function(){
	        $(".out").toggleClass("in");	
	    });	
	});

