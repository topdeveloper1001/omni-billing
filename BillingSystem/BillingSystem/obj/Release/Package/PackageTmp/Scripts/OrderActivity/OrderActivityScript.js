$(function () {
    JsCalls();
 });

function JsCalls() {

    $("#OrderActivityFormDiv").validationEngine();

    $(".collapseTitle").bind("click", function () {
        $.validationEngine.closePrompt(".formError", true);
    });

    $("#dtOrderActivityLicenseNumberExpire").datepicker({
        yearRange: "-130: +0",
        changeMonth: true,
        dateFormat: 'dd/mm/yy',
        changeYear: true
});

    //Filling all DropDown in page.
    //BindCountryData("#ddlOrderActivityCountry");
    //BindCountryDataWithCountryCode("#ddlClaimsContactPhoneCode", "#hdClaimsContactPhoneCode");
    
}

function SaveOrderActivity(id) {
    var isValid = jQuery("#OrderActivityFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
             var txtOrderActivityID = $("#txtOrderActivityID").val();
             var txtOrderType = $("#txtOrderType").val();
             var txtOrderCode = $("#txtOrderCode").val();
             var txtOrderCategoryID = $("#txtOrderCategoryID").val();
             var txtOrderSubCategoryID = $("#txtOrderSubCategoryID").val();
             var txtOrderActivityStatus = $("#txtOrderActivityStatus").val();
             var txtCorporateID = $("#txtCorporateID").val();
             var txtFacilityID = $("#txtFacilityID").val();
             var txtPatientID = $("#txtPatientID").val();
             var txtEncounterID = $("#txtEncounterID").val();
             var txtMedicalRecordNumber = $("#txtMedicalRecordNumber").val();
             var txtOrderID = $("#txtOrderID").val();
             var txtOrderBy = $("#txtOrderBy").val();
             var txtOrderActivityQuantity = $("#txtOrderActivityQuantity").val();
             var dtOrderScheduleDate = $("#dtOrderScheduleDate").val();
             var txtPlannedBy = $("#txtPlannedBy").val();
             var dtPlannedDate = $("#dtPlannedDate").val();
             var txtPlannedFor = $("#txtPlannedFor").val();
             var txtExecutedBy = $("#txtExecutedBy").val();
             var dtExecutedDate = $("#dtExecutedDate").val();
             var txtExecutedQuantity = $("#txtExecutedQuantity").val();
             var txtResultValueMin = $("#txtResultValueMin").val();
             var txtResultValueMax = $("#txtResultValueMax").val();
             var txtResultUOM = $("#txtResultUOM").val();
             var txtComments = $("#txtComments").val();
             var txtIsActive = $("#txtIsActive").val();
             var txtModifiedBy = $("#txtModifiedBy").val();
             var dtModifiedDate = $("#dtModifiedDate").val();
             var txtCreatedBy = $("#txtCreatedBy").val();
             var dtCreatedDate = $("#dtCreatedDate").val();
        //var txtOrderActivityFax = $("#txtOrderActivityFax").val();
        //var txtOrderActivityMainPhone = $("#txtOrderActivityMainPhone").val();
        

        //if (txtOrderActivityFax != '') {
        //    var countryCodeFax = $('#ddlCompanyFaxCode').val();
        //    txtOrderActivityFax = countryCodeFax + "-" + txtOrderActivityFax;
        //	}
        

        //var txtOrderActivityId = $("#hdOrderActivityId").val();
	// var dtOrderActivityLicenseNumberExpire = $("#dtOrderActivityLicenseNumberExpire").val();
	// 1MAPCOLUMNSHERE - OrderActivity


        var jsonData = JSON.stringify({
             OrderActivityID: txtOrderActivityID
             OrderType: txtOrderType
             OrderCode: txtOrderCode
             OrderCategoryID: txtOrderCategoryID
             OrderSubCategoryID: txtOrderSubCategoryID
             OrderActivityStatus: txtOrderActivityStatus
             CorporateID: txtCorporateID
             FacilityID: txtFacilityID
             PatientID: txtPatientID
             EncounterID: txtEncounterID
             MedicalRecordNumber: txtMedicalRecordNumber
             OrderID: txtOrderID
             OrderBy: txtOrderBy
             OrderActivityQuantity: txtOrderActivityQuantity
             OrderScheduleDate: dtOrderScheduleDate
             PlannedBy: txtPlannedBy
             PlannedDate: dtPlannedDate
             PlannedFor: txtPlannedFor
             ExecutedBy: txtExecutedBy
             ExecutedDate: dtExecutedDate
             ExecutedQuantity: txtExecutedQuantity
             ResultValueMin: txtResultValueMin
             ResultValueMax: txtResultValueMax
             ResultUOM: txtResultUOM
             Comments: txtComments
             IsActive: txtIsActive
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
            //OrderActivityId: id,
            //OrderActivityMainPhone: txtOrderActivityMainPhone,
            //OrderActivityFax: txtOrderActivityFax,
            //OrderActivityLicenseNumberExpire: dtOrderActivityLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - OrderActivity


        });
        $.ajax({
            type: "POST",
            url: '/OrderActivity/SaveOrderActivity',
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

function editDetails(e) {
    //
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.OrderActivityId;
    EditOrderActivity(id);

}

function deleteDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.OrderActivityId;
    DeleteOrderActivity(id);
}

function EditOrderActivity(id) {
    var txtOrderActivityId = id;
    var jsonData = JSON.stringify({
             OrderActivityID: txtOrderActivityID
             OrderType: txtOrderType
             OrderCode: txtOrderCode
             OrderCategoryID: txtOrderCategoryID
             OrderSubCategoryID: txtOrderSubCategoryID
             OrderActivityStatus: txtOrderActivityStatus
             CorporateID: txtCorporateID
             FacilityID: txtFacilityID
             PatientID: txtPatientID
             EncounterID: txtEncounterID
             MedicalRecordNumber: txtMedicalRecordNumber
             OrderID: txtOrderID
             OrderBy: txtOrderBy
             OrderActivityQuantity: txtOrderActivityQuantity
             OrderScheduleDate: dtOrderScheduleDate
             PlannedBy: txtPlannedBy
             PlannedDate: dtPlannedDate
             PlannedFor: txtPlannedFor
             ExecutedBy: txtExecutedBy
             ExecutedDate: dtExecutedDate
             ExecutedQuantity: txtExecutedQuantity
             ResultValueMin: txtResultValueMin
             ResultValueMax: txtResultValueMax
             ResultUOM: txtResultUOM
             Comments: txtComments
             IsActive: txtIsActive
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
        Id: txtOrderActivityId
    });
    $.ajax({
        type: "POST",
        url: '/OrderActivity/GetOrderActivity',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#OrderActivityFormDiv').empty();
                $('#OrderActivityFormDiv').html(data);
                $('#collapseOne').addClass('in');
                JsCalls();
              
            }
            else {
            }
        },
        error: function (msg) {

        }
    });
}

function ViewOrderActivity(id) {

    var txtServiceCodeId = id;
    var jsonData = JSON.stringify({
             OrderActivityID: txtOrderActivityID
             OrderType: txtOrderType
             OrderCode: txtOrderCode
             OrderCategoryID: txtOrderCategoryID
             OrderSubCategoryID: txtOrderSubCategoryID
             OrderActivityStatus: txtOrderActivityStatus
             CorporateID: txtCorporateID
             FacilityID: txtFacilityID
             PatientID: txtPatientID
             EncounterID: txtEncounterID
             MedicalRecordNumber: txtMedicalRecordNumber
             OrderID: txtOrderID
             OrderBy: txtOrderBy
             OrderActivityQuantity: txtOrderActivityQuantity
             OrderScheduleDate: dtOrderScheduleDate
             PlannedBy: txtPlannedBy
             PlannedDate: dtPlannedDate
             PlannedFor: txtPlannedFor
             ExecutedBy: txtExecutedBy
             ExecutedDate: dtExecutedDate
             ExecutedQuantity: txtExecutedQuantity
             ResultValueMin: txtResultValueMin
             ResultValueMax: txtResultValueMax
             ResultUOM: txtResultUOM
             Comments: txtComments
             IsActive: txtIsActive
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
        Id: txtServiceCodeId,
        ViewOnly: 'true'
    });
    $.ajax({
        type: "POST",
        url: '/OrderActivity/GetOrderActivity',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {

            if (data) {
                $('#serviceCodeDiv').empty();
                $('#serviceCodeDiv').html(data);
                $('#collapseOne').addClass('in');
            }
            else {
            }
        },
        error: function (msg) {
        }
    });
}

function DeleteOrderActivity(id) {
    if (confirm("Do you want to delete this record? ")) {
        var txtOrderActivityId = id;
        var jsonData = JSON.stringify({
             OrderActivityID: txtOrderActivityID
             OrderType: txtOrderType
             OrderCode: txtOrderCode
             OrderCategoryID: txtOrderCategoryID
             OrderSubCategoryID: txtOrderSubCategoryID
             OrderActivityStatus: txtOrderActivityStatus
             CorporateID: txtCorporateID
             FacilityID: txtFacilityID
             PatientID: txtPatientID
             EncounterID: txtEncounterID
             MedicalRecordNumber: txtMedicalRecordNumber
             OrderID: txtOrderID
             OrderBy: txtOrderBy
             OrderActivityQuantity: txtOrderActivityQuantity
             OrderScheduleDate: dtOrderScheduleDate
             PlannedBy: txtPlannedBy
             PlannedDate: dtPlannedDate
             PlannedFor: txtPlannedFor
             ExecutedBy: txtExecutedBy
             ExecutedDate: dtExecutedDate
             ExecutedQuantity: txtExecutedQuantity
             ResultValueMin: txtResultValueMin
             ResultValueMax: txtResultValueMax
             ResultUOM: txtResultUOM
             Comments: txtComments
             IsActive: txtIsActive
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
            Id: txtOrderActivityId,
            ModifiedBy: 1,//Put logged in user id here
            ModifiedDate: new Date(),
            IsDeleted: true,
            DeletedBy: 1,//Put logged in user id here
            DeletedDate: new Date(),
            IsActive: false
        });
        $.ajax({
            type: "POST",
            url: '/OrderActivity/DeleteOrderActivity',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindOrderActivityGrid();
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

function BindOrderActivityGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/OrderActivity/BindOrderActivityList",
        dataType: "html",
        async: true,
        // data: jsonData,
        success: function (data) {

            $("#OrderActivityListDiv").empty();
            $("#OrderActivityListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearForm() {
    $("#OrderActivityFormDiv").clearForm();
    $('#collapseOne').removeClass('in');
    $('#collapseTwo').addClass('in');
}

function ClearAll() {
    ClearForm();
    $.validationEngine.closePrompt(".formError", true);
    var jsonData = JSON.stringify({
             OrderActivityID: txtOrderActivityID
             OrderType: txtOrderType
             OrderCode: txtOrderCode
             OrderCategoryID: txtOrderCategoryID
             OrderSubCategoryID: txtOrderSubCategoryID
             OrderActivityStatus: txtOrderActivityStatus
             CorporateID: txtCorporateID
             FacilityID: txtFacilityID
             PatientID: txtPatientID
             EncounterID: txtEncounterID
             MedicalRecordNumber: txtMedicalRecordNumber
             OrderID: txtOrderID
             OrderBy: txtOrderBy
             OrderActivityQuantity: txtOrderActivityQuantity
             OrderScheduleDate: dtOrderScheduleDate
             PlannedBy: txtPlannedBy
             PlannedDate: dtPlannedDate
             PlannedFor: txtPlannedFor
             ExecutedBy: txtExecutedBy
             ExecutedDate: dtExecutedDate
             ExecutedQuantity: txtExecutedQuantity
             ResultValueMin: txtResultValueMin
             ResultValueMax: txtResultValueMax
             ResultUOM: txtResultUOM
             Comments: txtComments
             IsActive: txtIsActive
             ModifiedBy: txtModifiedBy
             ModifiedDate: dtModifiedDate
             CreatedBy: txtCreatedBy
             CreatedDate: dtCreatedDate
        Id: 0,
    });
    $.ajax({
        type: "POST",
        url: '/OrderActivity/ResetOrderActivityForm',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {

                $('#OrderActivityFormDiv').empty();
                $('#OrderActivityFormDiv').html(data);
                $('#collapseTwo').addClass('in');
                BindOrderActivityGrid();
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




