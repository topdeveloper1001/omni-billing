$(function () {
    $("#AppointmentTypesFormDiv").validationEngine();
    BindCorporateDataInAppointmentTypes();

    //BindGlobalCodesWithValue("#ddlDefaultTime", 4904, '');
    //$("#ddlDefaultTime option[value='1']").remove();
    $("#chkIsActive").prop("checked", true);
    //$("#chkExtValue1").prop("checked", true);
    //$("#ddlDefaultTime")[0].selectedIndex = 3;
    //BindAppointmentTypesGrid();
    BindUserType("#ddlUserType", "");
    SetMaxCategoryNumber();
});

function SaveAppointmentTypes(id) {
    var isValid = jQuery("#AppointmentTypesFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtId = $("#Id").val();
        var txtDescription = $("#txtDescription").val();
        var txtCategoryNumber = $("#txtCategoryNumber").val();
        var txtCptRangeFrom = $("#txtCptRangeFrom").val();
        var txtCptRangeTo = $("#txtCptRangeTo").val();
        var txtDefaultTime = $("#DefaultTime").val();
        var txtCorporateId = $("#ddlCorporate").val();
        var txtFacilityId = $("#ddlFacilityFilter").val();
        var txtIsActive = $("#chkIsActive:checked").length;
        var ddlUserRole = $("#ddlUserType").val();
        var txtName = $("#txtName").val();
        var jsonData = JSON.stringify({
            Id: txtId,
            Description: txtDescription,
            CategoryNumber: txtCategoryNumber,
            CptRangeFrom: txtCptRangeFrom,
            CptRangeTo: txtCptRangeTo,
            DefaultTime: txtDefaultTime,
            CorporateId: txtCorporateId,
            FacilityId: txtFacilityId,
            IsActive: txtIsActive,
            Name: txtName,
            ExtValue1: $("#chkExtValue1")[0].checked ? '1' : '0',
            ExtValue2: ddlUserRole
        });
        $.ajax({
            type: "POST",
            url: '/AppointmentTypes/SaveAppointmentTypes',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data == "-1") {
                    ShowMessage("Appointment Type Aleady Exist", "Warning", "warning", true);
                    return false;
                }
                if (data == "-2") {
                    ShowMessage("Category Number Aleady Exist", "Warning", "warning", true);
                    return false;
                }

                //$("#chkShowInActive").prop("checked", false);
                ClearAppointmentTypeForm();
                BindAppointmentTypesGrid();

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

function BindAppointmentTypesGrid() {
    var showInActive = $("#chkShowInActive").is(':checked');

    if (showInActive) {
        showInActive = false;
    } else {
        showInActive = true;
    }
    var jsonData = JSON.stringify({
        showInActive: showInActive
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/AppointmentTypes/BindAppointmentTypesList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $('#collapseAppointmentTypesList').addClass('in').attr('style', 'height:auto;');
            $("#AppointmentTypesListDiv").empty();
            $("#AppointmentTypesListDiv").html(data);
            $("#chkIsActive").prop("checked", true);
            //BindGlobalCodesWithValue("#ddlDefaultTime", 4904, '');
            //BindCorporateDataInAppointmentTypes();
        },
        error: function (msg) {

        }

    });
}

function ClearAppointmentTypeForm() {
    var cId = $("#ddlCorporate").val();
    var fId = $("#ddlFacilityFilter").val();
    $("#AppointmentTypesFormDiv").clearForm();
    $('#collapseAppointmentTypesList').addClass('in');
    $("#btnSaveAppointment").val("Save");
    $("#chkIsActive").prop("checked", true);
    $("#chkExtValue1").prop("checked", true);
    $("#Id").val(0);
    //$("#ddlDefaultTime")[0].selectedIndex = 3;
    $("#ddlCorporate").val(cId);
    $("#ddlFacilityFilter").val(fId);
    SetMaxCategoryNumber();
}

function EditAppointmentTypes(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/AppointmentTypes/GetAppointmentData',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $('#collapseAppointmentTypesAddEdit').addClass('in').attr('style', 'height:auto;');
            BindAppointmentTypes(data);
        },
        error: function (msg) {

        }
    });
}

function BindAppointmentTypes(data) {
    $("#Id").val(data.Id);
    $("#ddlCorporate").val(data.CorporateId);
    $("#ddlFacilityFilter").val(data.FacilityId);
    $("#txtDescription").val(data.Description);
    $("#txtCptRangeFrom").val(data.CptRangeFrom);
    $("#txtCptRangeTo").val(data.CptRangeTo);
    $("#DefaultTime").val(data.DefaultTime);
    $("#txtCategoryNumber").val(data.CategoryNumber);
    $('#chkIsActive').prop('checked', data.IsActive);
    $("#ddlUserType").val(data.ExtValue2);
    if (data.ExtValue1 == "1") {
        $('#chkExtValue1').prop('checked', true);
    } else {
        $('#chkExtValue1').prop('checked', false);
    }

    $("#txtName").val(data.Name);
    $("#btnSaveAppointment").val("Update");
    $('#collapseOne').addClass('in');

}

function BindCorporateDataInAppointmentTypes() {
    
    //Bind Corporates
    /// <summary>
    /// Binds the corporates.
    /// </summary>
    /// <param name="selector">The selector.</param>
    /// <param name="selectedId">The selected identifier.</param>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        url: "/RoleSelection/GetCorporatesDropdownData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            BindDropdownData(data, "#ddlCorporate", "#hfCorporateId");
            var corporaeIdFilter = $("#ddlCorporate").val();
            if (corporaeIdFilter > 0) {
                BindFacilityDropdownFilterInAppointmentTypes(corporaeIdFilter);
            }
        },
        error: function () {
        }
    });
}

function BindFacilityDropdownFilterInAppointmentTypes(cId) {
    
    if (cId > 0) {
        $.ajax({
            type: "POST",
            url: "/Facility/GetFacilitiesbyCorporate",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ corporateid: cId }),
            success: function (data) {
                BindDropdownData(data, "#ddlFacilityFilter", "#ddlFacilityFilter");
                $("#ddlFacilityFilter")[0].selectedIndex = 0;
                if ($("#hfFacilityId").val() > 0) {
                    $("#ddlFacilityFilter").val($("#hfFacilityId").val());
                }


                $("#ddlItemTypeId").attr("disabled", $("#ddlFacilityFilter").val() == 0);
            },
            error: function (msg) {
                console.log(msg);
            }
        });
    }
    else {
        //BindDropdownData('', "#ddlFacilityFilter", "");
        $("#ddlFacilityFilter")[0].selectedIndex = 0;
    }
    $("#hfCorporateId").val(cId > 0 ? cId : "");
}

function ShowInActiveRecordsInAppointmentTypes(chkSelector) {


    $("#chkActive").prop("checked", false);
    //var categoryId = $("#hfGlobalCodeID").val();

    var active = $(chkSelector)[0].checked;
    var isActive = active == true ? false : true;
    var corporateId = $("#ddlCorporate").val();
    var facilityId = $("#ddlFacilityFilter").val();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/AppointmentTypes/BindAppointmentGridActiveInActive',
        data: JSON.stringify({
            showInActive: isActive,
            corporateId: corporateId,
            facilityId: facilityId


        }),
        dataType: "html",

        success: function (data) {

            if (data != null) {
                $('#collapseAppointmentTypesList').addClass('in').attr('style', 'height:auto;');

                $("#AppointmentTypesListDiv").html(data);
            }
        },
        error: function (msg) {

        }
    });
}

function GetMaxCategoryNumber(cId, fId) {
    var jsonData = JSON.stringify({
        facilityId: fId,
        corporateId: cId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/AppointmentTypes/GetMaxCategoryNumber",
        dataType: "Json",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#txtCategoryNumber").val(data);
        },
        error: function (msg) {

        }

    });

}

function SetMaxCategoryNumber() {
    var cId = $("#ddlCorporate").val();
    var fId = $("#ddlFacilityFilter").val();
    if (cId != '' && fId != '') {
        GetMaxCategoryNumber(cId, fId);
        //GetMaxNumberInAppointment(cId, fId);
    }
}

function GetMaxNumberInAppointment(cId, fId) {
    var jsonData = JSON.stringify({
        facilityId: fId,
        corporateId: cId
    });
    var result = CommonAjaxCalls.PosttWithParams("/AppointmentTypes/GetMaxCategoryNumber", jsonData);
    if (result != null) {
        $("#txtCategoryNumber").val(result);
    }
}


function DeleteAppointmentTypes() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/AppointmentTypes/DeleteAppointmentTypes',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindAppointmentTypesGrid();
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
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

//var result = CommonAjaxCalls.GetHtmlWithParams("/ManualDashboard/RebindGridWithFacility", jsonData);
//if (result != null) {

//}


function SortAppointmentTypesGrid(event) {
    var showInActive = $("#chkShowInActive").is(':checked');
    if (showInActive) {
        showInActive = false;
    } else {
        showInActive = true;
    }

    var url = "/AppointmentTypes/BindAppointmentTypesList";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?showInActive=" + showInActive + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#AppointmentTypesListDiv").empty();
            $("#AppointmentTypesListDiv").html(data);
            $("#chkIsActive").prop("checked", true);
        },
        error: function () {
        }
    });
}

function BindUserType(selector, hidValueSelector) {
    //
    $.ajax({
        type: "POST",
        url: "/Physician/BindUsersType",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, deaprtments) {
                    items += "<option value='" + deaprtments.Value + "'>" + deaprtments.Text + "</option>";
                });
                $(selector).html(items);

                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
                    $(selector).val($(hidValueSelector).val());
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}