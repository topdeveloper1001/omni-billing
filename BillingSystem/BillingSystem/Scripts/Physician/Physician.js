$(function () {
    $("#PhysicianDiv").validationEngine();
    //$("#divFacultySpeciality").hide();
    BindAllData();

    //$("#ddlUserType").change(function () {
    //    var selected = $(this).val();
    //    if (selected > 0)
    //        BindUserOnUserRoleSelection(selected, 0);
    //});

    $("#ddlUsers").change(function () {
        var selected = $(this).val();
        if (selected > 0) {
            var roleId = $('option:selected', this).attr("roleId");
            var userName = $('option:selected', this).attr("userName");
            var roleName = $('option:selected', this).attr("roleName");

            $("#ddlUserType").val(roleId);
            $("#txtPhysicianName").val(userName);

            LicenseTypes(roleName, '');

            //SetSpecialtyDropdownVisibility(roleName);

        } else {
            $("#ddlUserType").val(0);
            $("#txtPhysicianName").val('');
            $("#ddlPhysicianLicenseType").empty();
        }
    });

    //$("#ddlPrimaryFacility").change(function () {
    //    var selected = $(this).val();
    //    if (selected > 0)
    //        $("#PrimaryFacility").html($('option:selected', this).text());
    //});

    //$("#ddlSecondaryFacility").change(function () {
    //    var selected = $(this).val();
    //    if (selected > 0)
    //        $("#SecondaryFacility").html($('option:selected', this).text());
    //});

    //$("#ddlThirdFacility").change(function () {
    //    var selected = $(this).val();
    //    if (selected > 0)
    //        $("#ThirdFacility").html($('option:selected', this).text());
    //});

    ClearPhysicianForm();
});

function SavePhysician() {
    /// <summary>
    /// Saves the physician.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var isValid = jQuery("#PhysicianDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {

        var selected = '';
        $('#facilityListView input:checked').each(function () {
            selected += '' + $(this)[0].value + ',';

        });
        var appList = selected != '' ? selected.slice(0, -1) : '';

        //if (appList == null || appList == '') {
        //    ShowMessage("At least 1 Facility has to be associated with the Clinical Staff!!", "Warning", "warning", true);
        //    return;
        //}

        var id = $("#hdPhysicianId").val();
        var txtPhysicianEmployeeNumber = $("#txtPhysicianEmployeeNumber").val();
        var txtPhysicianName = $("#txtPhysicianName").val();
        var txtPhysicianLicenseNumber = $("#txtPhysicianLicenseNumber").val();
        var sDate = $("#txtPhysicianLicenseEffectiveStartDate").val();
        var endDate = $("#txtPhysicianLicenseEffectiveEndDate").val();
        //var facility2 = $("#ddlSecondaryFacility").val();
        //var facility3 = $("#ddlThirdFacility").val();


        var physicianlicensetype = $("#ddlPhysicianLicenseType").val();
        var usertype = $("#ddlUserType").val();
        var userId = $("#ddlUsers").val();

        //var ddlFacultySpeciality = usertype == '4' ? $("#ddlFacultySpeciality").val() : '';
        //var ddlFacultyDepartment = usertype == '4' ? $("#ddlFacultyDepartment").val() : '';
        //var txtLunchTimeFrom = usertype == '4' ? $("#txtFacultyLunchTimeFrom").val() : '';
        //var txtLunchTimetill = usertype == '4' ? $("#txtFacultyLunchTimeTill").val() : '';

        //var ddlFacultySpeciality = $("#ddlUserType :selected").text().indexOf("Physician") != -1 ? $("#ddlFacultySpeciality").val() : "";
        var ddlFacultySpeciality = $("#ddlFacultySpeciality").val() != '' ? $("#ddlFacultySpeciality").val() : "0";
        var ddlFacultyDepartment = $("#ddlFacultyDepartment").val();
        var txtLunchTimeFrom = $("#txtFacultyLunchTimeFrom").val();
        var txtLunchTimetill = $("#txtFacultyLunchTimeTill").val();

        var jsonData = JSON.stringify({
            Id: id,
            PhysicianEmployeeNumber: txtPhysicianEmployeeNumber,
            PhysicianName: txtPhysicianName,
            PhysicianLicenseNumber: txtPhysicianLicenseNumber,
            PhysicianLicenseType: physicianlicensetype,
            PhysicianLicenseEffectiveStartDate: sDate,
            PhysicianLicenseEffectiveEndDate: endDate,
            PhysicianPrimaryFacility: $("#ddlPrimaryFacility").val(),
            PhysicianSecondaryFacility: '',         //facility2,
            PhysicianThirdFacility: '',             //facility3,
            IsActive: true,
            UserType: usertype,
            UserId: userId,
            FacultySpeciality: ddlFacultySpeciality,
            FacultyDepartment: ddlFacultyDepartment,
            FacultyLunchTimeFrom: txtLunchTimeFrom,
            FacultyLunchTimeTill: txtLunchTimetill,
            IsDeleted: false,
            IsSchedulingPublic: $("#IsSchedulingPublic")[0].checked,
            AssociatedFacilities: appList
        });
        $.ajax({
            type: "POST",
            url: '/Physician/SavePhysician',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data == "-1") {
                    ShowMessage("Employee Number Already Exist", "Warning", "warning", true);
                } else if (data == "-2") {
                    ShowMessage("Clinical Id Already Exist", "Warning", "warning", true);
                }
                else if (data == "-3") {
                    ShowMessage("The selected User Type and User already exist in the System. Select different ones and try again! ", "Warning", "warning", true);
                }
                else {
                    BindPhysicianGrid();
                    ClearPhysicianForm();
                    var msg = "Records Saved successfully !";
                    if (id > 0)
                        msg = "Records updated successfully";
                    ShowMessage(msg, "Success", "success", true);
                }
            },
            error: function (msg) {
            }
        });
    }

}

function EditPhysician(id) {
    /// <summary>
    /// Edits the physician.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        physicianId: id,
    });
    $.ajax({
        type: "POST",
        url: '/Physician/GetPhysician',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindPhysicianDetailsInEditMode(data);
        },
        error: function (msg) {
        }
    });
}

function DeletePhysician() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var txtFacilityId = $("#hfGlobalConfirmId").val();
        var jsonData = JSON.stringify({
            Id: txtFacilityId,
            ModifiedBy: 1, //Put logged in user id here
            ModifiedDate: new Date(),
            IsDeleted: true,
            DeletedBy: 1, //Put logged in user id here
            DeletedDate: new Date(),
            IsActive: false
        });
        $.ajax({
            type: "POST",
            url: '/Physician/DeletePhysician',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    $('#physicianGrid').empty();
                    BindPhysicianGrid();
                    ShowMessage("Physician Deleted Successfully!", "Success", "success", true);
                } else {
                    return false;
                }
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function BindPhysicianGrid() {
    /// <summary>
    /// Binds the physician grid.
    /// </summary>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Physician/BindPhysicianList",
        dataType: "html",
        async: true,
        success: function (data) {
            $("#physicianGrid").empty();
            $("#physicianGrid").html(data);
            InitializePhysicianDates();
        },
        error: function (msg) {
            //alert(msg);
        }
    });
}

function ClearPhysicianForm() {
    /// <summary>
    /// Clears all.
    /// </summary>
    /// <returns></returns>
    var fId = $("#ddlPrimaryFacility").val();
    $("#PhysicianDiv").clearForm(true);
    $('#collapseOne').removeClass('in');
    $('#collapseTwo').addClass('in');
    $('#btnSavePhysician').val('Save');
    $.validationEngine.closePrompt(".formError", true);
    $(".dis").attr("disabled", false);
    $("#ddlPrimaryFacility").attr("disabled", false);
    //$("#ddlUserType").attr('disabled', false);

    if (fId != null && fId > 0) {
        $("#ddlPrimaryFacility").val(fId);
    }
}

function GetFacilityName(dropdownSelector, labelSelector) {
    /// <summary>
    /// Gets the name of the facility.
    /// </summary>
    /// <param name="dropdownSelector">The dropdown selector.</param>
    /// <param name="labelSelector">The label selector.</param>
    /// <returns></returns>
    var facilityId = $(dropdownSelector).val();
    if (facilityId != "") {
        $.ajax({
            type: "POST",
            url: '/Physician/GetFacilityName',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                Id: facilityId
            }),
            success: function (data) {
                $(labelSelector).html(data);
            },
            error: function (msg) {
            }
        });
    }
    else {
        $("#FacilityName" + lastChar).html("");
    }
}

function BindGenericType(selector, categoryIdval, hidValueSelector) {
    /// <summary>
    /// Binds the type of the generic.
    /// </summary>
    /// <param name="selector">The selector.</param>
    /// <param name="categoryIdval">The category idval.</param>
    /// <param name="hidValueSelector">The hid value selector.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        categoryId: categoryIdval
    });
    $.ajax({
        type: "POST",
        url: "/GlobalCode/GetGlobalCodes",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                if (categoryIdval == 2309) {
                    $.each(data, function (i, globalCode) {
                        items += "<option value='" + globalCode.GlobalCodeValue + "'>" + globalCode.GlobalCodeName + "</option>";
                    });
                } else {
                    $.each(data, function (i, globalCode) {
                        items += "<option value='" + globalCode.GlobalCodeID + "'>" + globalCode.GlobalCodeName + "</option>";
                    });
                }
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

function InitializePhysicianDates() {
    $("#txtPhysicianLicenseEffectiveStartDate").datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12', //yesterday is minimum date(for today use 0 or -1970/01/01)
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });

    $("#txtPhysicianLicenseEffectiveEndDate").datetimepicker({
        format: 'm/d/Y',
        minDate: '1901/12/12', //yesterday is minimum date(for today use 0 or -1970/01/01)
        maxDate: '2025/12/12',
        timepicker: false,
        closeOnDateSelect: true
    });

    $("#txtFacultyLunchTimeFrom").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 30,
        //allowTimes: [
        //    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00'
        //],
        mask: true,
        onChangeDateTime: lunchTimeFunction,
        onShow: lunchTimeFunction
    });

    $("#txtFacultyLunchTimeTill").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 30,
        //allowTimes: [
        //    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00'
        //],
        mask: true,
    });
}

var lunchTimeFunction = function (currentDateTime) {
    $("#txtFacultyLunchTimeTill").val('');
    var minvalue = $("#txtFacultyLunchTimeFrom").val();
    if (minvalue.indexOf(':') > 0) {
        var minvalueInterval = minvalue.split(':')[1];
        minvalue = minvalueInterval == "00" ? minvalue.split(':')[0] + ":30" : (parseInt(minvalue.split(':')[0], 10) + 1) + ":00";
    }
    $("#txtFacultyLunchTimeTill").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 30,
        mask: true,
        minTime: minvalue
    });
};

function BindFacilityDepartments(selector, hidValueSelector) {
    $.ajax({
        type: "POST",
        url: "/Home/GetFacilityDeapartments",
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

function ShowHideSpecility() {
    var text = $("#ddlUserType :selected").text();
    if (text == "Physicians") {
        $("#divFacultySpeciality").show();
    } else {
        $("#divFacultySpeciality").hide();
    }
}

function BindPhysicianDetailsInEditMode(data) {
    $(".dis").attr("disabled", true);
    $("#ddlUserType").val(data.UserType);
    $("#ddlUsers").val(data.UserId);

    $('input[type=checkbox]:checked').each(function () {
        this.checked = false;
    });

    LicenseTypes(data.UserTypeStr, data.PhysicianLicenseType.toString());

    var selectedFacilities = data.AssociatedFacilities;

    if (selectedFacilities != null) {
        if (selectedFacilities.indexOf(',') != -1) {
            var selectAp = selectedFacilities.split(',');
            $("#checkBox_Facilities").find("input[type=checkbox]").each(function () {
                if (selectAp.indexOf($(this).val()) != -1) {
                    $(this).prop("checked", "checked");
                }
            });
        } else {
            $("#checkBox_Facilities").find("input[type=checkbox]").each(function () {
                if (selectedFacilities.indexOf($(this).val()) != -1) {
                    $(this).prop("checked", "checked");
                }
            });
        }
    }


    //SetSpecialtyDropdownVisibility(data.UserTypeStr);
    $("#ddlFacultySpeciality").val(data.FacultySpeciality);
    $("#ddlFacultyDepartment").val(data.FacultyDepartment);
    $("#txtPhysicianEmployeeNumber").val(data.PhysicianEmployeeNumber);
    $("#txtPhysicianName").val(data.PhysicianName);
    $("#txtPhysicianLicenseNumber").val(data.PhysicianLicenseNumber);
    $("#txtFacultyLunchTimeFrom").val(data.FacultyLunchTimeFrom);
    $("#txtFacultyLunchTimeTill").val(data.FacultyLunchTimeTill);
    $("#txtPhysicianLicenseEffectiveStartDate").val(data.PhysicianLicenseEffectiveStartDate);
    $("#txtPhysicianLicenseEffectiveEndDate").val(data.PhysicianLicenseEffectiveEndDate);

    $("#IsSchedulingPublic").prop("checked", data.IsSchedulingPublic);
    $("#ddlPrimaryFacility").val(data.PhysicianPrimaryFacility);
    $("#ddlPrimaryFacility").attr("disabled", true);

    //$("#ddlSecondaryFacility").val(data.PhysicianSecondaryFacility);
    //$("#ddlThirdFacility").val(data.PhysicianThirdFacility);

    $("#hdPhysicianId").val(data.Id);
    $("#btnSavePhysician").val("Update");
    $('#collapseOne').addClass('in');
}

function BindUserOnUserRoleSelection(userRoleId, selectedUser) {
    //ShowHideSpecility();
    $.ajax({
        type: "POST",
        url: "/Physician/BindUserOnUserRoleSelection",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ usertypeId: userRoleId }),
        success: function (data) {
            BindDropdownData(data, "#ddlUsers", "");
            if (selectedUser > 0) {
                $("#ddlUsers").val(selectedUser);
            }
        },
        error: function (msg) {
        }
    });
}

function BindAllData() {
    $.getJSON("/Physician/BindAllFacultyData", {}, function (data) {
        if (data != null) {
            BindDropdownData(data.fList, "#ddlPrimaryFacility", '');
            //BindDropdownData(data.fList, "#ddlSecondaryFacility", '');
            //BindDropdownData(data.fList, "#ddlThirdFacility", '');

            $("#ddlPrimaryFacility").val(data.fId);


            BindDropdownData(data.sList, "#ddlFacultySpeciality", '');

            BindDropdownData(data.dList, "#ddlFacultyDepartment", '');

            BindDropdownData(data.urList, "#ddlUserType", '');

            BindUsersDropdownData(data.uList, "#ddlUsers", "");

            BindList("#facilityListView", data.facilityListView);

            InitializePhysicianDates();
        }
    });
}

function BindUsersDropdownData(data, ddlSelector, hdSelector) {
    /// <summary>
    ///     Binds the dropdown data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="ddlSelector">The DDL selector.</param>
    /// <param name="hdSelector">The hd selector.</param>
    /// <returns></returns>
    $(ddlSelector).empty();
    var items = '<option value="0">--Select--</option>';
    $.each(data,
        function (i, obj) {
            var newItem = '<option value="' + obj.UserID + '" roleId="' + obj.RoleId + '" userName="' + obj.UserName + '" roleName="' + obj.RoleName + '">' + obj.Name + "</option>";
            items += newItem;
        });

    $(ddlSelector).html(items);
    var hdValue = "";
    if (hdSelector.indexOf("#") != -1) {
        hdValue = $(hdSelector).val();
    } else {
        hdValue = hdSelector;
    }
    //
    if (hdValue != null && hdValue != "") {
        $(ddlSelector).val(hdValue);
        if ($(ddlSelector).val() == null || $(ddlSelector).val() == undefined) {
            $(ddlSelector + " option")
                .filter(function (index) { return $(this).text() === "" + hdValue + ""; })
                .attr("selected", "selected");
        }
    } else {
        if ($(ddlSelector).length > 0)
            $(ddlSelector)[0].selectedIndex = 0;
    }
}

function LicenseTypes(roleName, selected) {
    if (roleName != '') {
        $.getJSON("/Physician/BindLicenseTypes", { roleName: roleName }, function (data) {
            if (data != null) {
                BindDropdownData(data, "#ddlPhysicianLicenseType", selected);
            }
        });
    }
}

function SetSpecialtyDropdownVisibility(roleName) {
    if (roleName == "Physicians") {
        $("#divFacultySpeciality").show();
    } else {
        $("#divFacultySpeciality").hide();
    }
    //$("#ddlUserType").attr('disabled', 'disabled');
}
