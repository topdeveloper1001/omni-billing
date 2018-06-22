function SearchPatientReviewPayments() {
    var isvalidSearch = ValidSearch();
    if (isvalidSearch) {
        var jsonData = JSON.stringify({
            PatientID: 0,
            PersonLastName: $("#txtLastName").val(),
            PersonEmiratesIDNumber: $("#txtEmiratesNationalId").val(),
            PersonPassportNumber: $("#txtPassportnumber").val(),
            PersonBirthDate: $("#txtBirthDate").val(),
            ContactMobilePhone: $("#txtMobileNumber").val()
        });
        $.ajax({
            type: "POST",
            url: '/ReviewExpectedPayments/GetPatientSearchResult',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                $('#divpatientSearchResult').empty();
                $('#divpatientSearchResult').html(data);
                $('#collapseOne').addClass('in');
                SetGridPaging('?', '?PatientID=' + 0 + '&PersonLastName=' + $("#txtLastName").val() + '&PersonEmiratesIDNumber=' + $("#txtEmiratesNationalId").val()
                     + '&PersonPassportNumber=' + $("#txtPassportnumber").val() + '&PersonBirthDate=' + $("#txtBirthDate").val() + '&ContactMobilePhone=' + $("#txtMobileNumber").val() + '&');
                $('#divAccountSummary').empty();
                $('#divAccountSummaryList').hide();
            },
            error: function (msg) {
            }
        });

        BindCountryDataWithCountryCode("#ddlCountries", "#hdCountry", "#lblCountryCode");
    }
}

//function SetGridCustomSorting() {
//    SetGridPaging('GetPatientInfoSearchResult?', 'GetPatientInfoSearchCustomResult?Ln=' + $("#txtLastName").val() + '&EID=' + $("#txtEmiratesNationalId").val()
//                   + '&PassNo=' + $("#txtPassportnumber").val() + '&BD=' + $("#txtBirthDate").val() + '&MobileNo=' + $("#txtMobileNumber").val() + '&');
//}

function ValidSearch() {
    var txtvalue = 0;
    $('#ValidatePatientSearch input[type=text]').each(function () {
        if ($(this).val() != "") {
            txtvalue = txtvalue + 1;
        }
    });
    if (txtvalue < 1) {
        ShowMessage("Confirm at least two pieces of information", "Alert", "warning", true);
        return false;
    }
    else {
        return true;
    }
    return false;
}

function ViewPaymentDetails(id) {
    var jsonData = JSON.stringify({
        PatientID: id,
    });
    $.ajax({
        type: "POST",
        url: '/ReviewExpectedPayments/GetPatientAccountSummary',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $('#divAccountSummaryList').show();
            $('#divAccountSummary').empty();
            $('#divAccountSummary').html(data);
            $('#collapseAccountSummary').addClass('in');
        },
        error: function (msg) {
        }
    });
}

function BindUnMatchedPaymentList() {
    $.ajax({
        type: "POST",
        url: '/ReviewExpectedPayments/GetUnMatchedPaymentList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $('#divUnMatchedPayment').empty();
            $('#divUnMatchedPayment').html(data);
            $('#collapseUnMatchedPayment').addClass('in');
        },
        error: function (msg) {
        }
    });
}

function BindNotReceivedPaymentList() {
    $.ajax({
        type: "POST",
        url: '/ReviewExpectedPayments/GetNotReceivedPaymentList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $('#divNoReceivedPayment').empty();
            $('#divNoReceivedPayment').html(data);
            $('#collapseNoReceivedPayment').addClass('in');
        },
        error: function (msg) {
        }
    });
}

$(function () {
    $(".EmiratesMask").mask("999-99-9999");
    var buttonKeys = { "EnterKey": 13 };
    $(".white-bg").keypress(function (e) {
        if (e.which == buttonKeys.EnterKey) {
            $("#btnSearch").click();
        }
    });
    BindCountryDataWithCountryCode("#ddlCountries", "#hdCountry", "#lblCountryCode");
    BindNotReceivedPaymentList();
    BindUnMatchedPaymentList();
});


function SortExpactedPayment(event) {
    var url = "/ReviewExpectedPayments/SortExpactedPayment";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#colExpectedPaymentInsNotPaid").empty();
            $("#colExpectedPaymentInsNotPaid").html(data);

        },
        error: function (msg) {
        }
    });
}


function SortPatientVarianceReport(event) {
    var url = "/ReviewExpectedPayments/SortPatientVarianceReport";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#colExpectedPaymentPatientVar").empty();
            $("#colExpectedPaymentPatientVar").html(data);

        },
        error: function (msg) {
        }
    });
}