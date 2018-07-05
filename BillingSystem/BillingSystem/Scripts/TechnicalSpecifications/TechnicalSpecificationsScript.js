$(function () {
    $("#TechnicalSpecificationFormDiv").validationEngine();
    BindCorporateDataInTechnicalSpecifications();    

});

function SaveTechnicalSpecification(id) {
    var isValid = jQuery("#TechnicalSpecificationFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtId = $("#Id").val();
        var txtEquipment = $("#ddlEquipment").val();
        var txtTechSpec = $("#txtTechSpec").val();
        var txtCorporateId = $("#ddlCorporate").val();
        var txtFacilityId = $("#ddlFacilityFilter").val();
        var jsonData = JSON.stringify({
            Id: txtId,
            ItemID: txtEquipment,
            TechSpec: txtTechSpec,
            CorporateId: txtCorporateId,
            FacilityId: txtFacilityId
        });
        $.ajax({
            type: "POST",
            url: '/TechnicalSpecifications/SaveTechnicalSpecifications',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data == "-1") {
                    ShowMessage("TechnicalSpecification Aleady Exist", "Warning", "warning", true);
                    return false;
                }
                

                //$("#chkShowInActive").prop("checked", false);
                ClearTechnicalSpecificationForm();
                BindTechnicalSpecificationGrid();

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

function BindTechnicalSpecificationGrid() {
    
    //var jsonData = JSON.stringify({
    //    showInActive: showInActive
    //});
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/TechnicalSpecifications/BindTechnicalSpecificationsList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $('#collapseTechnicalSpecificationsList').addClass('in').attr('style', 'height:auto;');
            $("#TechnicalSpecificationsListDiv").empty();
            $("#TechnicalSpecificationsListDiv").html(data);
            
        },
        error: function (msg) {

        }

    });
}

function ClearTechnicalSpecificationForm() {
    var cId = $("#ddlCorporate").val();
    var fId = $("#ddlFacilityFilter").val();
    $("#TechnicalSpecificationFormDiv").clearForm();
    $('#collapseTechnicalSpecificationsList').addClass('in');
    $("#btnSaveTechnicalSpecification").val("Save");
    $("#Id").val(0);
    $("#ddlCorporate").val(cId);
    $("#ddlFacilityFilter").val(fId);
}

function EditTechnicalSpecification(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/TechnicalSpecifications/GetTechnicalSpecificationsData',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $('#collapseTechnicalSpecificationsAddEdit').addClass('in').attr('style', 'height:auto;');
            BindTechnicalSpecifications(data);
        },
        error: function (msg) {

        }
    });
}

function BindTechnicalSpecifications(data) {
    $("#Id").val(data.Id);
    $("#ddlCorporate").val(data.CorporateId);
    $("#ddlFacilityFilter").val(data.FacilityId);
    $("#ddlEquipment").val(data.ItemID);
    $("#txtTechSpec").val(data.TechSpec);
    
    $("#btnSaveTechnicalSpecification").val("Update");
    $('#collapseOne').addClass('in');

}

function BindCorporateDataInTechnicalSpecifications() {

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
                BindFacilityDropdownFilterInTechnicalSpecifications(corporaeIdFilter);
            }
        },
        error: function () {
        }
    });
}

function BindFacilityDropdownFilterInTechnicalSpecifications(cId) {

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

                var fID = $("#ddlFacilityFilter").val();
                if (fID > 0) {
                    BindEquipmentDropdownInTechnicalSpecifications(fID);
                }
                //$("#ddlItemTypeId").attr("disabled", $("#ddlFacilityFilter").val() == 0);
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

function BindEquipmentDropdownInTechnicalSpecifications(fId) {

    if (fId > 0) {
        $.ajax({
            type: "POST",
            url: "/Equipment/BindEquipmentListForDropDown",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ facilityId: fId }),
            success: function (data) {
                BindDropdownData(data, "#ddlEquipment", "#ddlEquipment");
                $("#ddlEquipment")[0].selectedIndex = 0;
                if ($("#hfEquipmentId").val() > 0) {
                    $("#ddlEquipment").val($("#hfEquipmentId").val());
                }

                


                //$("#ddlItemTypeId").attr("disabled", $("#ddlFacilityFilter").val() == 0);
            },
            error: function (msg) {
                console.log(msg);
            }
        });
    }
    else {
        //BindDropdownData('', "#ddlFacilityFilter", "");
        $("#ddlEquipment")[0].selectedIndex = 0;
    }
    $("#hfFacilityId").val(fId > 0 ? fId : "");
}


function DeleteTechnicalSpecification() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/TechnicalSpecifications/DeleteTechnicalSpecification',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindTechnicalSpecificationGrid();
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



function SortTechnicalSpecificationsGrid(event) {
    
    var url = "/TechnicalSpecifications/BindTechnicalSpecificationsList";
    
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#TechnicalSpecificationsListDiv").empty();
            $("#TechnicalSpecificationsListDiv").html(data);
        },
        error: function () {
        }
    });
}
//function BindEquipment(selector, hidValueSelector) {
//    //
//    $.ajax({
//        type: "POST",
//        url: "/Physician/BindUsersType",
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        data: null,
//        success: function (data) {
//            if (data) {
//                var items = '<option value="0">--Select--</option>';
//                $.each(data, function (i, deaprtments) {
//                    items += "<option value='" + deaprtments.Value + "'>" + deaprtments.Text + "</option>";
//                });
//                $(selector).html(items);

//                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
//                    $(selector).val($(hidValueSelector).val());
//            }
//            else {
//            }
//        },
//        error: function (msg) {
//        }
//    });
//}