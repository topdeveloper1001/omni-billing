$(function () {
    JsCalls();
});

function JsCalls() {
    $("#ManagedCareFormDiv").validationEngine();
    $(".collapseTitle").bind("click", function () {
        $.validationEngine.closePrompt(".formError", true);
    });

    BindInsuranceCompanyDropdown("#ddlManagedCareInsurance", "#hdManagedCareInsuranceID");
}

function SaveManagedCare(id) {
    var isValid = jQuery("#ManagedCareFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtManagedCareInsuranceID = $("#ddlManagedCareInsurance").val();
        var txtManagedCarePlanID = $("#ddlManagedCarePlans").val();
        var txtManagedCarePolicyID = $("#ddlManagedCarePolicy").val();
        var txtManagedCareMultiplier = $("#txtManagedCareMultiplier").val();
        var txtManagedCareInpatientDeduct = $("#txtManagedCareInpatientDeduct").val();
        var txtManagedCareOutpatientDeduct = $("#txtManagedCareOutpatientDeduct").val();
        var txtManagedCarePerDiems = $("#txtManagedCarePerDiems").val();
        var txtCorporateID = $("#txtCorporateID").val();
        var txtFacilityID = $("#txtFacilityID").val();
        var isActive = $("#chkIsActive")[0].checked;

        var jsonData = JSON.stringify({
            ManagedCareID: id,
            ManagedCareInsuranceID: txtManagedCareInsuranceID,
            ManagedCarePlanID: txtManagedCarePlanID,
            ManagedCarePolicyID: txtManagedCarePolicyID,
            ManagedCareMultiplier: txtManagedCareMultiplier,
            ManagedCareInpatientDeduct: txtManagedCareInpatientDeduct,
            ManagedCareOutpatientDeduct: txtManagedCareOutpatientDeduct,
            ManagedCarePerDiems: txtManagedCarePerDiems,
            IsActive: isActive
        });

        $.ajax({
            type: "POST",
            url: '/ManagedCare/SaveManagedCare',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearAll();
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

function EditManagedCare(id) {
    var jsonData = JSON.stringify({
        ManagedCareID: id
    });
    $.ajax({
        type: "POST",
        url: '/ManagedCare/GetManagedCare',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#ManagedCareFormDiv').empty();
                $('#ManagedCareFormDiv').html(data);
                $('#collapseManagedCareAddEditForm').addClass('in');
                JsCalls();
            }
            else {
            }
        },
        error: function (msg) {

        }
    });
}

function DeleteManagedCare() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            ManagedCareID: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/ManagedCare/DeleteManagedCare',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindManagedCareGrid();
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

//function DeleteManagedCare(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtManagedCareId = id;
//        var jsonData = JSON.stringify({
//            ManagedCareID: id
//        });
//        $.ajax({
//            type: "POST",
//            url: '/ManagedCare/DeleteManagedCare',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindManagedCareGrid();
//                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
//                }
//                else {
//                    return false;
//                }
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function BindManagedCareGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/ManagedCare/BindManagedCareList",
        dataType: "html",
        async: true,
        // data: jsonData,
        success: function (data) {

            $("#ManagedCareListDiv").empty();
            $("#ManagedCareListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearForm() {
    $("#ManagedCareFormDiv").clearForm();
    $('#collapseManagedCareAddEditForm').removeClass('in');
    $('#collapseManagedCareListForm').addClass('in');
}

function ClearAll() {
    ClearForm();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/ManagedCare/ResetManagedCareForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $('#ManagedCareFormDiv').empty();
            $('#ManagedCareFormDiv').html(data);
            $('#collapseManagedCareListForm').addClass('in');
            JsCalls();
            BindManagedCareGrid();
        },
        error: function (msg) {


            return true;
        }
    });

}

function BindInsuranceCompanyDropdown(ddlSelector, hdSelector) {
    $.ajax({
        type: "POST",
        url: '/ManagedCare/BindInsuranceCompanies',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $(ddlSelector).empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data, function (i, pol) {
                items += "<option value='" + pol.InsuranceCompanyId + "'>" + pol.InsuranceCompanyName + "</option>";
            });

            $(ddlSelector).html(items);

            var selectedValue = $(hdSelector).val();
            if (selectedValue > 0)
                $(ddlSelector).val(selectedValue);
            else
                $(ddlSelector)[0].selectedIndex = 0;

            if (selectedValue != '' && selectedValue > 0) {
                OnChangeInsuranceCompany(ddlSelector, "#ddlManagedCarePlans");
            }
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

function OnChangeInsuranceCompany(ddlInsuranceSelector, ddlPlanSelector) {
    var insSelectedValue = $(ddlInsuranceSelector).val();
    if (insSelectedValue != '' && insSelectedValue > 0) {
        $.ajax({
            type: "POST",
            url: '/ManagedCare/BindInsurancePlansByCompanyId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ companyId: insSelectedValue }),
            success: function (data) {
                $(ddlPlanSelector).empty();
                var items = '<option value="0">--Select--</option>';
                $.each(data, function (i, plan) {
                    items += "<option value='" + plan.InsurancePlanId + "'>" + plan.PlanName + "</option>";
                });

                $(ddlPlanSelector).html(items);

                var hdPlanSelector = "#hdManagedCarePlanID";
                var selectedValue = $(hdPlanSelector).val();
                if (selectedValue > 0) {
                    $(ddlPlanSelector).val(selectedValue);
                    OnChangeInsurancePlan(ddlPlanSelector, "#ddlManagedCarePolicy");
                }
                else
                    $(ddlPlanSelector)[0].selectedIndex = 0;
            },
            error: function (msg) {
                console.log(msg);
            }
        });
    }
}

function OnChangeInsurancePlan(ddlPlanSelector, ddlPolicySelector) {
    var planId = $(ddlPlanSelector).val();
    var jsonData = JSON.stringify({
        planId: planId
    });

    $.ajax({
        type: "POST",
        url: '/PatientInfo/GetInsurancePolicesByPlanId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            //$(ddlPolicySelector).empty();
            //var items = '<option value="0">--Select--</option>';
            //$.each(data, function (i, policy) {
            //    items += "<option value='" + policy.InsurancePolicyId + "'>" + policy.PolicyName + "</option>";
            //});

            //$(ddlPolicySelector).html(items);

            //var hdPolicySelector = "#hdManagedCarePolicyID";
            //var selectedValue = $(hdPolicySelector).val();
            //if (selectedValue > 0)
            //    $(ddlPolicySelector).val(selectedValue);
            //else
            //    $(ddlPolicySelector)[0].selectedIndex = 0;
            BindDropdownData(data, ddlPolicySelector, "#hdManagedCarePolicyID");
        },
        error: function (msg) {
        }
    });
}
