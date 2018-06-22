$(function () {
    JsCalls();
});

function JsCalls() {

    $("#MedicalRecordFormDiv").validationEngine();

    $(".collapseTitle").bind("click", function () {
        $.validationEngine.closePrompt(".formError", true);
    });

    $("#dtMedicalRecordLicenseNumberExpire").datepicker({
        yearRange: "-130: +0",
        changeMonth: true,
        dateFormat: 'dd/mm/yy',
        changeYear: true
    });

}

function SaveMedicalRecord(id) {
    var isValid = jQuery("#MedicalRecordFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtMedicalRecordID = $("#txtMedicalRecordID").val();
        var txtMedicalRecordType = $("#txtMedicalRecordType").val();
        var txtCorporateID = $("#txtCorporateID").val();
        var txtFacilityID = $("#txtFacilityID").val();
        var txtPatientID = $("#txtPatientID").val();
        var txtEncounterID = $("#txtEncounterID").val();
        var txtMedicalRecordNumber = $("#txtMedicalRecordNumber").val();
        var txtGlobalCodeCategoryID = $("#txtGlobalCodeCategoryID").val();
        var txtGlobalCode = $("#txtGlobalCode").val();
        var txtShortAnswer = $("#txtShortAnswer").val();
        var txtDetailAnswer = $("#txtDetailAnswer").val();
        var txtAnswerValueMin = $("#txtAnswerValueMin").val();
        var txtAnswerValueMax = $("#txtAnswerValueMax").val();
        var txtAnswerUOM = $("#txtAnswerUOM").val();
        var txtComments = $("#txtComments").val();
        var txtCommentBy = $("#txtCommentBy").val();
        var dtCommentDate = $("#dtCommentDate").val();
        var txtModifiedBy = $("#txtModifiedBy").val();
        var dtModifiedDate = $("#dtModifiedDate").val();
        var txtIsDeleted = $("#txtIsDeleted").val();
        var txtDeletedBy = $("#txtDeletedBy").val();
        var dtDeletedDate = $("#dtDeletedDate").val();
        //var txtMedicalRecordFax = $("#txtMedicalRecordFax").val();
        //var txtMedicalRecordMainPhone = $("#txtMedicalRecordMainPhone").val();
        

        //if (txtMedicalRecordFax != '') {
        //    var countryCodeFax = $('#ddlCompanyFaxCode').val();
        //    txtMedicalRecordFax = countryCodeFax + "-" + txtMedicalRecordFax;
        //	}
        

        //var txtMedicalRecordId = $("#hdMedicalRecordId").val();
        // var dtMedicalRecordLicenseNumberExpire = $("#dtMedicalRecordLicenseNumberExpire").val();
        // 1MAPCOLUMNSHERE - MedicalRecord


        var jsonData = JSON.stringify({
            MedicalRecordID: txtMedicalRecordID
            MedicalRecordType: txtMedicalRecordType
            CorporateID: txtCorporateID
            FacilityID: txtFacilityID
            PatientID: txtPatientID
            EncounterID: txtEncounterID
            MedicalRecordNumber: txtMedicalRecordNumber
            GlobalCodeCategoryID: txtGlobalCodeCategoryID
            GlobalCode: txtGlobalCode
            ShortAnswer: txtShortAnswer
            DetailAnswer: txtDetailAnswer
            AnswerValueMin: txtAnswerValueMin
            AnswerValueMax: txtAnswerValueMax
            AnswerUOM: txtAnswerUOM
            Comments: txtComments
            CommentBy: txtCommentBy
            CommentDate: dtCommentDate
            ModifiedBy: txtModifiedBy
            ModifiedDate: dtModifiedDate
            IsDeleted: txtIsDeleted
            DeletedBy: txtDeletedBy
            DeletedDate: dtDeletedDate
            //MedicalRecordId: id,
            //MedicalRecordMainPhone: txtMedicalRecordMainPhone,
            //MedicalRecordFax: txtMedicalRecordFax,
            //MedicalRecordLicenseNumberExpire: dtMedicalRecordLicenseNumberExpire,
            // 2MAPCOLUMNSHERE - MedicalRecord


        });
    $.ajax({
        type: "POST",
        url: '/MedicalRecord/SaveMedicalRecord',
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
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.MedicalRecordId;
    EditMedicalRecord(id);

}

function deleteDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.MedicalRecordId;
    DeleteMedicalRecord(id);
}

function EditMedicalRecord(id) {
    var txtMedicalRecordId = id;
    var jsonData = JSON.stringify({
        MedicalRecordID: txtMedicalRecordID
        MedicalRecordType: txtMedicalRecordType
        CorporateID: txtCorporateID
        FacilityID: txtFacilityID
        PatientID: txtPatientID
        EncounterID: txtEncounterID
        MedicalRecordNumber: txtMedicalRecordNumber
        GlobalCodeCategoryID: txtGlobalCodeCategoryID
        GlobalCode: txtGlobalCode
        ShortAnswer: txtShortAnswer
        DetailAnswer: txtDetailAnswer
        AnswerValueMin: txtAnswerValueMin
        AnswerValueMax: txtAnswerValueMax
        AnswerUOM: txtAnswerUOM
        Comments: txtComments
        CommentBy: txtCommentBy
        CommentDate: dtCommentDate
        ModifiedBy: txtModifiedBy
        ModifiedDate: dtModifiedDate
        IsDeleted: txtIsDeleted
        DeletedBy: txtDeletedBy
        DeletedDate: dtDeletedDate
        Id: txtMedicalRecordId
    });
$.ajax({
    type: "POST",
    url: '/MedicalRecord/GetMedicalRecord',
    async: false,
    contentType: "application/json; charset=utf-8",
    dataType: "html",
    data: jsonData,
    success: function (data) {
        if (data) {
            $('#MedicalRecordFormDiv').empty();
            $('#MedicalRecordFormDiv').html(data);
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

function ViewMedicalRecord(id) {

    var txtServiceCodeId = id;
    var jsonData = JSON.stringify({
        MedicalRecordID: txtMedicalRecordID
        MedicalRecordType: txtMedicalRecordType
        CorporateID: txtCorporateID
        FacilityID: txtFacilityID
        PatientID: txtPatientID
        EncounterID: txtEncounterID
        MedicalRecordNumber: txtMedicalRecordNumber
        GlobalCodeCategoryID: txtGlobalCodeCategoryID
        GlobalCode: txtGlobalCode
        ShortAnswer: txtShortAnswer
        DetailAnswer: txtDetailAnswer
        AnswerValueMin: txtAnswerValueMin
        AnswerValueMax: txtAnswerValueMax
        AnswerUOM: txtAnswerUOM
        Comments: txtComments
        CommentBy: txtCommentBy
        CommentDate: dtCommentDate
        ModifiedBy: txtModifiedBy
        ModifiedDate: dtModifiedDate
        IsDeleted: txtIsDeleted
        DeletedBy: txtDeletedBy
        DeletedDate: dtDeletedDate
        Id: txtServiceCodeId,
        ViewOnly: 'true'
});
$.ajax({
    type: "POST",
    url: '/MedicalRecord/GetMedicalRecord',
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

function DeleteMedicalRecord(id) {
    if (confirm("Do you want to delete this record? ")) {
        var txtMedicalRecordId = id;
        var jsonData = JSON.stringify({
            MedicalRecordID: txtMedicalRecordID,
            MedicalRecordType: txtMedicalRecordType,
            CorporateID: txtCorporateID,
            FacilityID: txtFacilityID,
            PatientID: txtPatientID,
            EncounterID: txtEncounterID,
            MedicalRecordNumber: txtMedicalRecordNumber,
            GlobalCodeCategoryID: txtGlobalCodeCategoryID,
            GlobalCode: txtGlobalCode,
            ShortAnswer: txtShortAnswer,
            DetailAnswer: txtDetailAnswer,
            AnswerValueMin: txtAnswerValueMin,
            AnswerValueMax: txtAnswerValueMax,
            AnswerUOM: txtAnswerUOM,
            Comments: txtComments,
            CommentBy: txtCommentBy,
            CommentDate: dtCommentDate,
            ModifiedBy: txtModifiedBy,
            ModifiedDate: dtModifiedDate,
            IsDeleted: txtIsDeleted,
            DeletedBy: txtDeletedBy,
            DeletedDate: dtDeletedDate,
            Id: txtMedicalRecordId,
    });
    $.ajax({
        type: "POST",
        url: '/MedicalRecord/DeleteMedicalRecord',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                BindMedicalRecordGrid();
                ShowMessage("Records Deleted Successfully", "Success", "success", true);
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

function BindMedicalRecordGrid() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/MedicalRecord/BindMedicalRecordList",
        dataType: "html",
        async: true,
        // data: jsonData,
        success: function (data) {

            $("#MedicalRecordListDiv").empty();
            $("#MedicalRecordListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearForm() {
    $("#MedicalRecordFormDiv").clearForm();
    $('#collapseOne').removeClass('in');
    $('#collapseTwo').addClass('in');
}

function ClearAll() {
    ClearForm();
    $.validationEngine.closePrompt(".formError", true);
    var jsonData = JSON.stringify({
        MedicalRecordID: txtMedicalRecordID,
        MedicalRecordType: txtMedicalRecordType,
        CorporateID: txtCorporateID,
        FacilityID: txtFacilityID,
        PatientID: txtPatientID,
        EncounterID: txtEncounterID,
        MedicalRecordNumber: txtMedicalRecordNumber,
        GlobalCodeCategoryID: txtGlobalCodeCategoryID,
        GlobalCode: txtGlobalCode,
        ShortAnswer: txtShortAnswer,
        DetailAnswer: txtDetailAnswer,
        AnswerValueMin: txtAnswerValueMin,
        AnswerValueMax: txtAnswerValueMax,
        AnswerUOM: txtAnswerUOM,
        Comments: txtComments,
        CommentBy: txtCommentBy,
        CommentDate: dtCommentDate,
        ModifiedBy: txtModifiedBy,
        ModifiedDate: dtModifiedDate,
        IsDeleted: txtIsDeleted,
        DeletedBy: txtDeletedBy,
        DeletedDate: dtDeletedDate,
        Id: 0,
    });
$.ajax({
    type: "POST",
    url: '/MedicalRecord/ResetMedicalRecordForm',
    async: false,
    contentType: "application/json; charset=utf-8",
    dataType: "html",
    data: jsonData,
    success: function (data) {
        if (data) {

            $('#MedicalRecordFormDiv').empty();
            $('#MedicalRecordFormDiv').html(data);
            $('#collapseTwo').addClass('in');
            BindMedicalRecordGrid();
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




