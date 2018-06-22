$(function () {
    $("#MedicalNecessityFormDiv").validationEngine();
    BindCodeTypeDDL();
});

function SaveMedicalNecessity(id) {
    var isValid = jQuery("#MedicalNecessityFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
             var txtId = $("#Id").val();
             var txtBillingCode = $("#txtBillingCode").val();
             var txtBillingCodeType = $("#ddlBillingCodeType").val();
             var txtICD9Code = $("#txtICD9Code").val();
             var chkIsActive = $("#chkIsActive").is(':checked');
        
            var jsonData = JSON.stringify({
             Id: txtId,
             BillingCode: txtBillingCode,
             BillingCodeType: txtBillingCodeType,
             ICD9Code: txtICD9Code,
             IsActive: chkIsActive,
            });
        $.ajax({
            type: "POST",
            url: '/MedicalNecessity/SaveMedicalNecessity',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearAll();
                BindMedicalNecessityGrid();
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

function EditMedicalNecessity(id) {
    var jsonData = JSON.stringify({
             Id: id
    });
    $.ajax({
        type: "POST",
        url: '/MedicalNecessity/GetMedicalNecesstiyById',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindMedicalNecessityData(data);
            $('#collapseMedicalNecessityAddEdit').addClass('in').attr('style', 'height:auto;');

            //$('#MedicalNecessityFormDiv').empty();
            //$('#MedicalNecessityFormDiv').html(data);
            //$("#MedicalNecessityFormDiv").validationEngine();
        },
        error: function (msg) {

        }
    });
}

function BindMedicalNecessityData(data) {
    $("#Id").val(data.Id);
    $("#txtBillingCode").val(data.BillingCode);
    $("#chkIsActive").prop('checked', data.IsActive);
    $("#ddlBillingCodeType").val(data.BillingCodeType);
    $("#txtICD9Code").val(data.ICD9Code);

}

function DeleteMedicalNecessity(id) {
    if (confirm("Do you want to delete this record? ")) {
        var txtMedicalNecessityId = id;
        var jsonData = JSON.stringify({
             Id: id
        });
        $.ajax({
            type: "POST",
            url: '/MedicalNecessity/DeleteMedicalNecessity',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindMedicalNecessityGrid();
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

function BindMedicalNecessityGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/MedicalNecessity/BindMedicalNecessityList",
        dataType: "html",
        async: false,
        data: null,
        success: function (data) {
            $("#MedicalNecessityListDiv").empty();
            $("#MedicalNecessityListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearForm() {
    
}

function ClearAll() {
    $("#MedicalNecessityFormDiv").clearForm();
    $('#collapseMedicalNecessityAddEdit').removeClass('in');
    $('#collapseMedicalNecessityList').addClass('in');
    $("#MedicalNecessityFormDiv").validationEngine();
    $.validationEngine.closePrompt(".formError", true);
    $.ajax({
        type: "POST",
        url: '/MedicalNecessity/ResetMedicalNecessityForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            if (data) {
                $('#MedicalNecessityFormDiv').empty();
                $('#MedicalNecessityFormDiv').html(data);
                $('#collapseMedicalNecessityList').addClass('in');
                BindMedicalNecessityGrid();
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

function BindCodeTypeDDL() {
    BindGlobalCodesWithValue('#ddlBillingCodeType', 5201, '');
}