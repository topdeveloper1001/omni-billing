$(function () {
    $("#ProjectDashboardFormDiv").validationEngine();
});

function SaveProjectDashboard(id) {
    var isValid = jQuery("#ProjectDashboardFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
             var txtId = $("#txtId").val();
             var txtProjectID = $("#txtProjectID").val();
             var txtTaskID = $("#txtTaskID").val();
             var txtJanTarget = $("#txtJanTarget").val();
             var txtFebTarget = $("#txtFebTarget").val();
             var txtMarTarget = $("#txtMarTarget").val();
             var txtAprTarget = $("#txtAprTarget").val();
             var txtMayTarget = $("#txtMayTarget").val();
             var txtJunTarget = $("#txtJunTarget").val();
             var txtJulTarget = $("#txtJulTarget").val();
             var txtAugTarget = $("#txtAugTarget").val();
             var txtSepTarget = $("#txtSepTarget").val();
             var txtOctTarget = $("#txtOctTarget").val();
             var txtNovTarget = $("#txtNovTarget").val();
             var txtDecTarget = $("#txtDecTarget").val();
             var txtExternalValue1 = $("#txtExternalValue1").val();
             var txtExternalValue2 = $("#txtExternalValue2").val();
             var txtExternalValue3 = $("#txtExternalValue3").val();
             var txtExternalValue4 = $("#txtExternalValue4").val();
             var txtExternalValue5 = $("#txtExternalValue5").val();
             var txtIsActive = $("#txtIsActive").val();
             var txtCorporateId = $("#txtCorporateId").val();
             var txtFacilityId = $("#txtFacilityId").val();
             var txtCreatedBy = $("#txtCreatedBy").val();
             var dtCreatedDate = $("#dtCreatedDate").val();
             var txtModifiedBy = $("#txtModifiedBy").val();
             var dtModifiedDate = $("#dtModifiedDate").val();
        var jsonData = JSON.stringify({
            Id: txtId,
            ProjectID: txtProjectID,
            TaskID: txtTaskID,
            JanTarget: txtJanTarget,
            FebTarget: txtFebTarget,
            MarTarget: txtMarTarget,
            AprTarget: txtAprTarget,
            MayTarget: txtMayTarget,
            JunTarget: txtJunTarget,
            JulTarget: txtJulTarget,
            AugTarget: txtAugTarget,
            SepTarget: txtSepTarget,
            OctTarget: txtOctTarget,
            NovTarget: txtNovTarget,
            DecTarget: txtDecTarget,
            ExternalValue1: txtExternalValue1,
            ExternalValue2: txtExternalValue2,
            ExternalValue3: txtExternalValue3,
            ExternalValue4: txtExternalValue4,
            ExternalValue5: txtExternalValue5,
            IsActive: txtIsActive,
            CorporateId: txtCorporateId,
            FacilityId: txtFacilityId,
            CreatedBy: txtCreatedBy,
            CreatedDate: dtCreatedDate,
            ModifiedBy: txtModifiedBy,
            ModifiedDate: dtModifiedDate,
            //ProjectDashboardId: id,
            //ProjectDashboardMainPhone: txtProjectDashboardMainPhone,
            //ProjectDashboardFax: txtProjectDashboardFax,
            //ProjectDashboardLicenseNumberExpire: dtProjectDashboardLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - ProjectDashboard
        });
        $.ajax({
            type: "POST",
            url: '/ProjectDashboard/SaveProjectDashboard',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#ProjectDashboardListDiv", data);
                ClearProjectDashboardForm();
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

function EditProjectDashboard(id) {
    var jsonData = JSON.stringify({
             Id: id,
    });
    $.ajax({
        type: "POST",
        url: '/ProjectDashboard/GetProjectDashboard',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindProjectDashboardDetails(data);
        },
        error: function (msg) {

        }
    });
}

function DeleteProjectDashboard(id) {
    if (confirm("Do you want to delete this record? ")) {
        var jsonData = JSON.stringify({
            id: id,
        });
        $.ajax({
            type: "POST",
            url: '/ProjectDashboard/DeleteProjectDashboard',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#ProjectDashboardListDiv", data);
                ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function ClearProjectDashboardForm() {
    $("#ProjectDashboardFormDiv").clearForm(true);
    $('#collapseProjectDashboardAddEdit').removeClass('in');
    $('#collapseProjectDashboardList').addClass('in');
    $("#ProjectDashboardFormDiv").validationEngine();
    $("#btnSave").val("Save");
    $.validationEngine.closePrompt(".formError", true);
}

function BindProjectDashboardDetails(data) {

    $("#btnSave").val("Update");
    $('#collapseProjectDashboardList').removeClass('in');
    $('#collapseProjectDashboardAddEdit').addClass('in');
    $("#ProjectDashboardFormDiv").validationEngine();
}




