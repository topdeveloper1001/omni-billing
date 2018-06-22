$(function () {
    //BindFaciltyGrid();

    $("#OpenOrderActivityScheduleDiv").validationEngine();
});

function SaveOpenOrderActivitySchedule(id) {
    var isValid = jQuery("#OpenOrderActivityScheduleDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        $.ajax({
            type: "POST",
            url: '/OpenOrderActivitySchedule/SaveOpenOrderActivitySchedule',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindFaciltyGrid();
                    ClearAll();
                    var msg = "Records Saved successfully !";
                    if (id > 0)
                        msg = "Records updated successfully";

                    ShowMessage(msg, "Success", "success", true);
                }
                else {
                }
            },
            error: function (msg) {
            }
        });
    }
    return false;
}
function editDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.OpenOrderActivityScheduleId;
    EditOpenOrderActivitySchedule(id);

}
function deleteDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.OpenOrderActivityScheduleId;
    DeleteOpenOrderActivitySchedule(id);
}

function EditOpenOrderActivitySchedule(id) {
    var txtOpenOrderActivityScheduleId = id;
    var jsonData = JSON.stringify({
        Id: txtOpenOrderActivityScheduleId,
        ViewOnly: ''
    });
    $.ajax({
        type: "POST",
        url: '/OpenOrderActivitySchedule/GetOpenOrderActivitySchedule',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data != null) {
                $('#OpenOrderActivityScheduleDiv').empty();
                $('#OpenOrderActivityScheduleDiv').html(data);

                $('#collapseOne').addClass('in');
            }
        },
        error: function (msg) {
        }
    });
}

function ViewOpenOrderActivitySchedule(id) {
    var txtOpenOrderActivityScheduleId = id;
    var jsonData = JSON.stringify({
        Id: txtOpenOrderActivityScheduleId,
        ViewOnly: 'true'
    });
    $.ajax({
        type: "POST",
        url: '/OpenOrderActivitySchedule/GetOpenOrderActivitySchedule',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#OpenOrderActivityScheduleDiv').empty();
                $('#OpenOrderActivityScheduleDiv').html(data);
                $('#collapseOne').addClass('in');
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

function DeleteOpenOrderActivitySchedule(id) {
    if (confirm("Do you want to delete this record? ")) {
        var txtOpenOrderActivityScheduleId = id;
        var jsonData = JSON.stringify({
            Id: txtOpenOrderActivityScheduleId,
            IsDeleted: true,
            DeletedBy: 1,//Put logged in user id here
            DeletedDate: new Date(),
            IsActive: false
        });
        $.ajax({
            type: "POST",
            url: '/OpenOrderActivitySchedule/DeleteOpenOrderActivitySchedule',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    $('#OpenOrderActivityScheduleGrid').empty();
                    $('#OpenOrderActivityScheduleGrid').html(data);
                    BindFaciltyGrid();
                    ShowMessage("OpenOrderActivitySchedule Deleted Successfully!", "Success", "info", true);
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
    return false;
}

function BindFaciltyGrid() {
    var encounterId = "100220141";
    // var jsonData = JSON.stringify({ EncounterId: encounterId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/OpenOrderActivitySchedule/BindFaciltyList",
        dataType: "html",
        async: true,
        // data: jsonData,
        success: function (data) {
            $("#OpenOrderActivityScheduleGrid").empty();
            $("#OpenOrderActivityScheduleGrid").html(data);
        },
        error: function (msg) {
            alert(msg);
        }

    });
}

function ClearForm() {
    $("#OpenOrderActivityScheduleDiv").clearForm();
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
        url: '/OpenOrderActivitySchedule/ResetOpenOrderActivityScheduleForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                BindFaciltyGrid();
                $('#OpenOrderActivityScheduleDiv').empty();
                $('#OpenOrderActivityScheduleDiv').html(data);
                $('#collapseTwo').addClass('in');
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

