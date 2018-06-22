function SaveBedMaster(id) {
    /// <summary>
    ///     Saves the bed master.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var ddlBedTypes = $("#ddlBedType").val();
    var ddlFacilityId = $("#ddlFacilityId").val();
    var txtRate = $("#txtRate").val();
    var txtStartDate = new Date($("#txtStartDate").val());
    var chkIsActive = $("#chkIsActive").prop("checked");
    var ddlBedName = $("#ddlBedName").val();
    var jsonData = JSON.stringify({
        BedId: id,
        FacilityId: ddlFacilityId,
        BedType: ddlBedTypes,
        Rate: txtRate,
        StartDate: txtStartDate,
        IsActive: chkIsActive,
        IsOccupied: true,
        FacilityStructureId: ddlBedName
    });

    $.ajax({
        type: "POST",
        url: '/BedMaster/SaveBedMaster',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function(data) {
            if (data) {
                BindBedMasterGrid();
                ClearAll();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";
                ShowMessage(msg, "Success", "success", true);
            } else {
            }
        },
        error: function(msg) {
        }
    });

}

function EditBedMaster(id) {
    /// <summary>
    /// Edits the bed master.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var txtBedMasterId = id;
    var jsonData = JSON.stringify({
        id: txtBedMasterId,
        viewOnly: ''
    });
    $.ajax({
        type: "POST",
        url: '/BedMaster/GetBedMaster',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function(data) {

            if (data) {
                $('#BedMasterDiv').empty();
                $('#BedMasterDiv').html(data);
                $('#collapseOne').addClass('in');
                BindGlobalCodesWithValue("#ddlBedType", 1001, "#hdBedType");
                FacilityList("#ddlFacilityId", "#hdFacility");
                ServicesList("#ddlServices");
                GetBedServices(id);
            } else {
            }
        },
        error: function(msg) {
        }
    });
}

function ViewBedMaster(id) {

    /// <summary>
    /// Views the bed master.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var txtBedMasterId = id;
    var jsonData = JSON.stringify({
        Id: txtBedMasterId,
        ViewOnly: 'true'
    });
    $.ajax({
        type: "POST",
        url: '/BedMaster/GetBedMaster',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function(data) {

            if (data) {
                $('#BedMasterDiv').empty();
                $('#BedMasterDiv').html(data);
                $('#collapseOne').addClass('in');
            } else {
            }
        },
        error: function(msg) {
        }
    });
}

function DeleteBedMaster(id) {
    /// <summary>
    /// Deletes the bed master.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var txtBedMasterId = id;
    var jsonData = JSON.stringify({
        id: txtBedMasterId,
        active: false
    });
    $.ajax({
        type: "POST",
        url: '/BedMaster/DeleteBedMaster',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function(data) {
            if (data) {
                $('#divBedMaster').empty();
                $('#divBedMaster').html(data);
            } else {
                return false;
            }
        },
        error: function(msg) {
            return true;
        }
    });
}

$(function() {
    $(".DateTimeC").datepicker({
        yearRange: "-70: +15",
        changeMonth: true,
        dateFormat: 'dd/mm/yy',
        changeYear: true
    });
    BindGlobalCodesWithValue("#ddlBedType", 1001, "#hdBedType");
    FacilityList("#ddlFacilityId", "#hdFacility");
    ServicesList("#ddlServices");
    BindBedNameList("#ddlBedName", "#hdFacilityStructureId");
});

//ddlServices
function ServicesList(selector, hidValueSelector) {
    /// <summary>
    /// Serviceses the list.
    /// </summary>
    /// <param name="selector">The selector.</param>
    /// <param name="hidValueSelector">The hid value selector.</param>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        url: "/BedMaster/GetServicesList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //data: jsonData,
        success: function(data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function(i, ServiceCode) {
                    items += "<option value='" + ServiceCode.ServiceCodeId + "'>" + ServiceCode.ServiceCodeDescription + "</option>";
                });
                $(selector).html(items);

                //$.ajax({
                //    type: "POST",
                //    url: "/BedMaster/GetSelectedServicesList",
                //    async: false,
                //    contentType: "application/json; charset=utf-8",
                //    dataType: "json",
                //    //data: jsonData,
                //    success: function (data) {
                //        
                //        if (data) {
                //            $("#ddlServices").val(data);
                //        }
                //        else {
                //        }
                //    },
                //    error: function (msg) {
                //    }
                //});
            } else {
            }
        },
        error: function(msg) {
        }
    });
}

function FacilityList(selector, hidValueSelector) {
    /// <summary>
    /// Facilities the list.
    /// </summary>
    /// <param name="selector">The selector.</param>
    /// <param name="hidValueSelector">The hid value selector.</param>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        url: "/BedMaster/GetFacilityList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //data: jsonData,
        success: function(data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function(i, facility) {
                    items += "<option value='" + facility.FacilityId + "'>" + facility.FacilityName + "</option>";
                });
                $(selector).html(items);

                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
                    $(selector).val($(hidValueSelector).val());
            } else {
            }
        },
        error: function(msg) {
        }
    });
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
        url: "/Home/GetGlobalCodes",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function(data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function(i, globalCode) {
                    items += "<option value='" + globalCode.GlobalCodeValue + "'>" + globalCode.GlobalCodeName + "</option>";
                });
                $(selector).html(items);

                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
                    $(selector).val($(hidValueSelector).val());
            } else {
            }
        },
        error: function(msg) {
        }
    });
}

function GetBedServices(id) {
    /// <summary>
    /// Gets the bed services.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        bedId: id
    });
    $.ajax({
        type: "POST",
        url: "/BedMaster/GetSelectedServicesList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function(data) {

            if (data) {
                $("#ddlServices").val(data);
            } else {
            }
        },
        error: function(msg) {
        }
    });
}

function BindBedMasterGrid() {
    /// <summary>
    /// Binds the bed master grid.
    /// </summary>
    /// <returns></returns>
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/BedMaster/BindBedMasterList",
        dataType: "html",
        async: true,
        // data: jsonData,
        success: function(data) {
            $("#BedMasterGrid").empty();
            $("#BedMasterGrid").html(data);
        },
        error: function(msg) {
        }
    });
}

function ClearForm() {
    /// <summary>
    /// Clears the form.
    /// </summary>
    /// <returns></returns>
    $('#BedMasterDiv').clearForm();
}

function ClearAll() {
    /// <summary>
    /// Clears all.
    /// </summary>
    /// <returns></returns>
    ClearForm();
    $.validationEngine.closePrompt(".formError", true);
    var jsonData = JSON.stringify({
        Id: 0,
    });
    $.ajax({
        type: "POST",
        url: '/BedMaster/ResetBedMasterForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function(data) {
            if (data) {

                BindBedMasterGrid();
                $('#BedMasterDiv').empty();
                $('#BedMasterDiv').html(data);
                $('#collapseTwo').addClass('in');
            } else {
                return false;
            }
        },
        error: function(msg) {


            return true;
        }
    });
}

function IsBedMasterValid(id) {
    /// <summary>
    /// Determines whether [is bed master valid] [the specified identifier].
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    var isValid = jQuery("#BedMasterDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        SaveBedMaster(id);
    }
    return false;
}

function BindBeds() {
    /// <summary>
    /// Binds the beds.
    /// </summary>
    /// <returns></returns>
    BindBedNameList("#ddlBedName", "#hdFacilityStructureId");
    BindBedMasterGridByfacility();
}

function BindBedNameList(selector, hidValueSelector) {
    /// <summary>
    /// Binds the bed name list.
    /// </summary>
    /// <param name="selector">The selector.</param>
    /// <param name="hidValueSelector">The hid value selector.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        facilityId: $("#ddlFacilityId").val(),
    });
    $.ajax({
        type: "POST",
        url: "/BedMaster/GetBedNameList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function(data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data, function(i, facility) {
                    items += "<option value='" + facility.FacilityStructureId + "'>" + facility.FacilityStructureName + "</option>";
                });
                $(selector).html(items);

                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
                    $(selector).val($(hidValueSelector).val());
            } else {
            }
        },
        error: function(msg) {
        }
    });
}

function BindBedMasterGridByfacility() {
    /// <summary>
    /// Binds the bed master grid byfacility.
    /// </summary>
    /// <returns></returns>
    var facilityId = $("#ddlFacilityId").val();
    var jsonData = JSON.stringify({
        _facilityId: facilityId,
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/BedMaster/BindBedMasterListByfacility",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function(data) {
            $("#BedMasterGrid").empty();
            $("#BedMasterGrid").html(data);
        },
        error: function(msg) {
        }
    });
}