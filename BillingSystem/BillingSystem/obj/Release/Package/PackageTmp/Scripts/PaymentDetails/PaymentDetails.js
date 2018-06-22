$(function () {
    $("#XPaymentReturnFormDiv").validationEngine();
    BindCountryDataWithCountryCode("#ddlCountries", "#hdCountry", "#lblCountryCode");
    BindPatients();
});

function BindPatients() {
    $.ajax({
        type: "POST",
        url: '/Home/GetPatientList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            BindDropdownData(data, "#ddlPatients", "0");
            var selectedValue = $("#hdPatientId").val();
            if (selectedValue > 0) {
                BindEncounters();
            }
        },
        error: function (msg) {
        }
    });
}

//function BindPatientDetails() {
//    var patientId = $('#ddlPatients').val();
//    if (patientId != '0') {
//        $.ajax({
//            type: "POST",
//            url: '/PatientInfo/GetPatientCustomDetailById',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: JSON.stringify({ PatientID: patientId }),
//            success: function (data) {
//                $('#lblEmiratesID').text(data.PersonEmiratesIDNumber);
//                $('#lblPersonPassportNumber').text(data.PersonPassportNumber);
//                $('#lblCompanyName').text(data.PatientCompanyName);
//                $('#lblClaimsContactPhone').text(data.PatientCompanyClaimPhoneNumber);
//                $('#lblContactMobilePhone').text(data.ContactMobilePhone);
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

function BindGridData() {
    var claimid = $('#ClaimID').val();
    var jsonData = JSON.stringify({
        claimid: claimid
    });
    $.ajax({
        type: "POST",
        url: '/XPaymentReturn/GetRemittanceInfoListByClaimId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $('#XPaymentReturnListDiv').empty();
                $('#XPaymentReturnListDiv').html(data);
            }
        },
        error: function (msg) {
            return true;
        }
    });
}

function GenerateXMLFile() {
    var isValid = jQuery("#collapseXPaymentReturnAddEdit").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        $.ajax({
            type: "POST",
            url: '/XPaymentReturn/GenerateRemittanceXmlFile',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            success: function (data) {
                if (data) {
                    ShowMessage('XML file successfully created.', "Success", "Success", true);
                } else {
                    ShowMessage('Unable to generate XML File.', "Warning", "warning", true);
                }
                setTimeout(function () {
                    window.location.reload(true);
                }, 1000);
            },
            error: function (msg) {
                ShowMessage('Unable to generate XML File.', "Warning", "warning", true);
            }
        });
    }
}

function BindPatients() {
    $.ajax({
        type: "POST",
        url: '/Home/GetPatientList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            BindDropdownData(data, "#ddlPatients", "#hdPatientId");
            var selectedValue = $("#hdPatientId").val();
            if (selectedValue > 0) {
                BindEncounters();
            }
        },
        error: function (msg) {
        }
    });
}

//function BindEncounters() {
//    var patientId = $("#ddlPatients").val();
//    if (patientId > 0) {
//        $('.ddlemty').empty();
//        $('.emptytxt').text('');
//        $('#PaymentHeaderDiv').hide();
//        $('#DivPaymentsDetailsList').empty();
//        $.ajax({
//            type: "POST",
//            url: '/Home/GetEncountersListByPatientId',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: JSON.stringify({ patientId: patientId }),
//            success: function (data) {
                
//                BindDropdownData(data, "#ddlEncounters", "0");
//                var selectedValue = $("#hdEncounterId").val();
//                if (selectedValue > 0) {
//                    GetBillHeaderListByEncounterId();
//                }
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

//function GetBillHeaderListByEncounterId() {
//    var encounterId = $("#ddlEncounters").val();
//    if (encounterId > 0) {
//        $.ajax({
//            type: "POST",
//            url: '/Home/GetBillHeaderListByEncounterId',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: JSON.stringify({ encounterId: encounterId }),
//            success: function (data) {
//                BindDropdownData(data, "#ddlBillHeaders", "0");
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

function BindPaymentDetails() {
    var billId = $('#ddlBillHeaders').val();
    if (billId > 0) {
        $.ajax({
            type: "POST",
            url: '/PaymentDetails/GetPaymentDetails',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ BillHeaderId: billId }),
            success: function (data) {
                $('#PaymentHeaderDiv').show();
                $('#lblPatientSharePayable').text(data.PatientSharePayable);
                $('#lblInsSharePayable').text(data.InsSharePayable);
                $('#lblGrossSharePayable').text(data.GrossSharePayable);
                
                $('#lblPatientSharePaid').text(data.PatientSharePaid);
                $('#lblInsSharePaid').text(data.InsSharePaid);
                $('#lblGrossSharePaid').text(data.GrossSharePaid);

                $('#lblPatientShareBalance').text(data.PatientShareBalance);
                $('#lblInsShareBalance').text(data.InsShareBalance);
                $('#lblGrossShareBalance').text(data.GrossShareBalance);

                $('#lblInsPayment').text(data.InsSharePayable);
                $('#lblInsTotalPaid').text(data.InsTotalPaid);
                $('#lblInsApplied').text(data.InsApplied);
                $('#lblInsUnapplied').text(data.InsUnapplied);

                $('#lblPatientPayment').text(data.PatientSharePayable);
                $('#lblPatientTotalPaid').text(data.PatientTotalPaid);
                $('#lblPatientApplied').text(data.PatientApplied);
                $('#lblPatientUnApplied').text(data.PatientUnApplied);
            },
            error: function (msg) {
            }
        });
        //BindXActvitiesData(billId);
        BindInsurancePayments(billId);
        BindManualPayments(billId);
    }
}

function BindInsurancePayments(billId) {
    $.ajax({
        type: "POST",
        url: '/PaymentDetails/GetPaymentDetailsList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ BillHeaderId: billId }),
        success: function (data) {
            $('#DivPaymentsDetailsList').empty();
            $('#DivPaymentsDetailsList').html(data);
            $('#CollapsePaymentsList').addClass('in');
        },
        error: function (msg) {
        }
    });
}

function BindManualPayments(id) {   
    $.ajax({
        type: "POST",
        url: '/PaymentDetails/GetManualPaymentDetailsList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ BillHeaderId: id }),
        success: function (data) {
            $('#DivManualPaymentsDetailsList').empty();
            $('#DivManualPaymentsDetailsList').html(data);
            $('#CollapseManualPaymentsList').addClass('in');
        },
        error: function (msg) {
        }
    });
}

function BindXActvitiesData(billId) {
    $.ajax({
        type: "POST",
        url: '/PaymentDetails/GetXActivitiesList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: JSON.stringify({ BillHeaderId: billId }),
        success: function (data) {
            $('#DivXActivitesDetailsList').empty();
            $('#DivXActivitesDetailsList').html(data);
            $('#CollapseXActvitiesList').addClass('in');
        },
        error: function (msg) {
        }
    });
}


function SearchPatientInPaymentDetail() {
    /// <summary>
    /// Searches the patient.
    /// </summary>
    /// <returns></returns>
    //var isvalidSearch = ValidSearch();
    //if (isvalidSearch) {
        var contactnumber = $("#txtMobileNumber").val() != '' ? $('#lblCountryCode').html() + '-' + $("#txtMobileNumber").val() : "";
        var jsonData = JSON.stringify({
            PatientID: 0,
            PersonLastName: $("#txtLastName").val(),
            PersonEmiratesIDNumber: $("#txtEmiratesNationalId").val(),
            PersonPassportNumber: $("#txtPassportnumber").val(),
            PersonBirthDate: $("#txtBirthDate").val(),
            ContactMobilePhone: contactnumber
        });
        $.ajax({
            type: "POST",
            url: '/PaymentDetails/GetPatientInfoSearchResult',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                $('#divSearchResultListIndetail').empty();
                $('#divSearchResultListIndetail').html(data);
                $('#collapseOne').addClass('in');
               },
            error: function (msg) {
            }
        });
    }
//}



function GetPatientNameById(pId) {
    var patientId = pId;
    $.ajax({
        type: "POST",
        url: '/Payment/GetPatientName',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ patientId: patientId }),
        success: function (data) {

            if (data != null && data.length > 0) {
                $("#patientName").text(data);
            }

        },
        error: function (msg) {
        }
    });
}

function GetEncounterNumberOfPatient(encounterNumber) {
    $("#lblEncounterNumber").text(encounterNumber);
}
function SetPatientAndEncounterId(patientId, encounterId) {
    $("#hdPatientId").val(patientId);
    $("#hdEncounterId").val(encounterId);
}

function BindPatientDetails(pId) {
    var patientId = pId;
    if (patientId != '0') {
        $.ajax({
            type: "POST",
            url: '/PatientInfo/GetPatientCustomDetailById',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ PatientID: patientId }),
            success: function (data) {
                $('#lblEmiratesID').text(data.PersonEmiratesIDNumber);
                $('#lblPersonPassportNumber').text(data.PersonPassportNumber);
                $('#lblCompanyName').text(data.PatientCompanyName);
                $('#lblClaimsContactPhone').text(data.PatientCompanyClaimPhoneNumber);
                $('#lblContactMobilePhone').text(data.ContactMobilePhone);
            },
            error: function (msg) {
            }
        });
    }
}



function GetBillHeaderListByEncounterId(eId) {
    var encounterId = eId;
    if (encounterId > 0) {
        $.ajax({
            type: "POST",
            url: '/Home/GetBillHeaderListByEncounterId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ encounterId: encounterId }),
            success: function (data) {
                BindDropdownData(data, "#ddlBillHeaders", "0");
            },
            error: function (msg) {
            }
        });
    }

}



function PatientDetailData(patientId, encounterNumber,encounterId) {
    GetEncounterNumberOfPatient(encounterNumber);
    GetPatientNameById(patientId);
    GetBillHeaderListByEncounterId(encounterId);
    SetPatientAndEncounterId(patientId, encounterId);
    BindPatientDetails(patientId);
}