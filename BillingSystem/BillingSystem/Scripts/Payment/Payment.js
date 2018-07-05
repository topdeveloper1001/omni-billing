$(LoadData);

function LoadData() {

    BindPatients();
    $("#CollapsePaymentAddEdit").validationEngine();
    BindGlobalCodesWithValue("#ddlPaymentType", 5011, "");
    $("#patmentTypeDiv").hide();
    var startYear = 2016;
    var year = 25;
    BindYearInPayment(year, startYear);
    BindMonthInPayment();
    $("#ViewBill").hide();


    $("#ddlBillHeaders").change(BindPaymentsRelatedData);
    $("#ddlPaymentType").change(ShowHidePaymentDetail);

    $("#BtnSave").click(SaveAndApplyManualPayment);

    $("#btnPaymentCancel").click(ClearPaymentForm);
}

function BindPatients() {
    $.ajax({
        type: "POST",
        url: '/Login/GetPatientList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            BindDropdownData(data, "#ddlPatients", "#hdPatientId");
            var selectedValue = $("#hdPatientId").val();
            if (selectedValue > 0) {
                //BindEncounters();
            }
        },
        error: function (msg) {
        }
    });
}

function ClearPaymentForm() {
    $("#PaymentAddEdit").clearForm();
    $('#CollapsePaymentAddEdit').removeClass('in');
    $('#CollapsePaymentsList').addClass('in');
    $("#patmentTypeDiv").hide();
    $("#BtnSave").val('Save');
}

function EditPayment(paymentId) {
    $.ajax({
        type: "POST",
        url: '/Payment/GetPaymentDetail',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ paymentId: paymentId }),
        success: function (data) {
            BindPaymentDetails(data);
            BindPaymentTypeDetails(data);
        },
        error: function (msg) {
        }
    });
}

function BindPaymentDetails(data) {

    $("#hdPatientId").val(data.PatientFor);
    $("#hdEncounterId").val(data.EncounterId);
    $("#hdBillHeaderId").val(data.BillId);
    $("#ddlPatients").val(data.PatientFor);
    //BindEncounters();
    $("#txtPaymentReference").val(data.PayReference);
    $("#txtPayDate").val(data.PayedDate);
    //$("#txtPayDate").val(new Date(data.PaymentDate));
    $("#txtPayAmount").val(data.PayAmount);
    $("#hdCreatedDate").val(new Date(data.CreatedDate));
    $("#hdCreatedBy").val(data.CreatedBy);
    $("#hdPaymentID").val(data.PaymentID);
    $("#ddlEncounters").val(data.EncounterId);
    $("#ddlBillHeaders").val(data.BillId);
    $('#CollapsePaymentsList').removeClass('in');
    $('#CollapsePaymentAddEdit').addClass('in');
    $("#BtnSave").val('Update');
}


function SearchPatientInPayment() {
    /// <summary>
    /// Searches the patient.
    /// </summary>
    /// <returns></returns>
    var isvalidSearch = ValidSearch();
    if (isvalidSearch) {
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
            url: '/Payment/GetPatientInfoSearchResult',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                $('#divSearchResultList').empty();
                $('#divSearchResultList').html(data);
                $('#collapseOne').addClass('in');
                $('html,body').animate({ scrollTop: $("#divSearchResultList").offset().top }, 'slow');
            },
            error: function (msg) {
            }
        });
    }
}


function ShowHidePaymentDetail() {
    var paymentType = $("#ddlPaymentType").val();
    if (paymentType == 3) {
        $("#patmentTypeDiv").show();
        $("#txtCardNumber").addClass("validate[required]");
        $("#ddlExpiryMonth").addClass("validate[required]");
        $("#ddlExpiryYear").addClass("validate[required]");
        $("#txtCardHolderName").addClass("validate[required]");
        $("#txtSecurityNumber").addClass("validate[required]");

    } else {
        $("#patmentTypeDiv").hide();
        $("#txtCardNumber").removeClass("validate[required]");
        $("#txtSecurityNumber").removeClass("validate[required]");
        $("#ddlExpiryMonth").removeClass("validate[required]");
        $("#ddlExpiryYear").removeClass("validate[required]");
        $("#txtCardHolderName").removeClass("validate[required]");
    }
}


function EditPaymentTypeDetail(paymentId) {
    $.ajax({
        type: "POST",
        url: '/Payment/GetPaymentTypeDetail',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ paymentId: paymentId }),
        success: function (data) {
            BindPaymentTypeDetails(data);
        },
        error: function (msg) {
        }
    });
}


function BindPaymentTypeDetails(data) {
    BindGlobalCodesWithValue("#ddlPaymentType", 5011, "");
    $("#txtSecurityNumber").val(data.ExtValue1);
    $("#txtCardNumber").val(data.CardNumber);
    $("#ddlExpiryMonth").val(data.ExpiryMonth);
    $("#ddlExpiryYear").val(data.ExpiryYear);
    $("#txtCardHolderName").val(data.CardHolderName);
    $("#ddlPaymentType").val(data.PaymentType);
    if (data.PaymentType == 3) {
        $("#patmentTypeDiv").show();
    }
}


function BindYearInPayment(yearLimit, startYear) {
    var i = 0;
    var items = '<option value="0">--Select--</option>';

    for (i = 0; i <= yearLimit; i++) {
        var textYear = startYear + i;
        items += "<option value='" + textYear + "'>" + textYear + "</option>";
    }
    $("#ddlExpiryYear").html(items);
}


function BindMonthInPayment() {
    var i = 0;
    var items = '<option value="0">--Select--</option>';

    for (i = 1; i <= 12; i++) {

        items += "<option value='" + i + "'>" + i + "</option>";
    }
    $("#ddlExpiryMonth").html(items);
}


function GetBillHeaderListByEncounterId(eNumber) {
    $.ajax({
        type: "POST",
        url: '/BillHeader/GetBillHeaderListByEncounterId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ encounterId: eNumber }),
        success: function (data) {
            $('html,body').animate({ scrollTop: $("#CollapsePaymentAddEdit").offset().top }, 'slow');
            BindDropdownData(data, "#ddlBillHeaders", "#hdBillHeaderId");
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
    $('html,body').animate({ scrollTop: $("#CollapsePaymentAddEdit").offset().top }, 'fast');
}



/* $$$$$$$$$$$$$$ New Methods with improvements / big fixes on Billing / Payments $$$$$$$$$$$$$$ */
function SaveAndApplyManualPayment() {
    var isValid = jQuery("#CollapsePaymentAddEdit").validationEngine({ returnIsValid: true });
    if (isValid) {
        var createdBy = 0;
        var createdDate = null;
        var paymentId = $("#hdPaymentID").val();
        if (paymentId > 0) {
            createdBy = $("#hdCreatedBy").val();
            createdDate = new Date($("#hdCreatedDate").val());
        }
        else {
            createdDate = new Date();
        }

        var jsonData = JSON.stringify({
            PaymentID: paymentId,
            PayType: 500,
            PayDate: $("#txtPayDate").val(),
            PayAmount: $("#txtPayAmount").val(),
            PayReference: $("#txtPaymentReference").val(),
            PayBillNumber: $("#ddlBillHeaders option:selected").text(),
            PayFor: $("#hdPatientId").val(),
            PayBy: $("#hdPatientId").val(),
            PayBillID: $("#ddlBillHeaders").val(),
            PayActivityID: null,
            PayEncounterID: $("#hdEncounterId").val(),
            PayXAFileHeaderID: null,
            PayIsActive: true,
            PayCreatedBy: createdBy,
            PaymentTypeId: $("#ddlPaymentType").val(),
            PayCreatedDate: new Date(createdDate),
            PTDCardNumber: $("#txtCardNumber").val(),
            PTDExpiryMonth: $("#ddlExpiryMonth").val(),
            PTDExpiryYear: $("#ddlExpiryYear").val(),
            PTDCardHolderName: $("#txtCardHolderName").val(),
            PTDPaymentType: $("#ddlPaymentType").val(),
            PTDSecurityNumber: $("#txtSecurityNumber").val()
        });
        $.ajax({
            type: "POST",
            url: '/Payment/SaveAndApplyManualPayments',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                
                var msg = "Records Saved successfully !";
                var msgType = 'success';
                var caption = 'Success';
                if (data.Success) {
                    if (paymentId > 0)
                        msg = "Records updated successfully";
                    BindPaymentsData(data.pl, data.pd, data.patients, true, true);
                }
                else {
                    msg = 'Error while Saving or Applying the Manual Payments, Try again in a while!';
                    msgType = 'error';
                    caption = 'Alert';
                }

                ShowMessage(msg, caption, msgType, true);
            },
            error: function (msg) {
                console.log(error);
            }
        });

    }
}


function BindPaymentsRelatedData() {
    if ($("#ddlBillHeaders").val() > 0) {
        var jsonData = JSON.stringify({
            billHeaderId: $("#ddlBillHeaders").val(),
            patientId: $("#hdPatientId").val(),
            eId: $("#hdEncounterId").val(),
        });
        $.ajax({
            type: "POST",
            url: '/Payment/GetPaymentsRelatedData',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                BindPaymentsData(data.pl, data.pd, data.patients, false, false);
            },
            error: function (msg) {
                console.log(msg);
            }
        });
    }
    else {
        $('#PaymentHeaderDiv').hide();
        $("#DivPaymentsList").empty();
    }
}


function BindPaymentsData(paymentsList, paymentHeaderData, patientsData, patientsDataRequired, clearForm) {

    /* Bind Payments List Section*/
    if (paymentsList != null)
        BindList("#DivPaymentsList", paymentsList);


    /* Bind Payment Details Header Section*/
    $('#CollapsePaymentsList').addClass('in');
    if ($("#ddlBillHeaders").val() != '' || $("#ddlBillHeaders").val() != '0') {
        var billid = $("#ddlBillHeaders").val();
        $('#PaymentHeaderDiv').show();
        if (paymentHeaderData != null) {
            $('#lblPatientSharePayable').text(paymentHeaderData.PatientSharePayable);
            $('#lblInsSharePayable').text(paymentHeaderData.InsSharePayable);
            $('#lblGrossSharePayable').text(paymentHeaderData.GrossSharePayable);

            $('#lblPatientSharePaid').text(paymentHeaderData.PatientSharePaid);
            $('#lblInsSharePaid').text(paymentHeaderData.InsSharePaid);
            $('#lblGrossSharePaid').text(paymentHeaderData.GrossSharePaid);

            $('#lblPatientShareBalance').text(paymentHeaderData.PatientShareBalance);
            $('#lblInsShareBalance').text(paymentHeaderData.InsShareBalance);
            $('#lblGrossShareBalance').text(paymentHeaderData.GrossShareBalance);
        }

        $("#ViewBill").show();
        $("#ViewBill").click(function () {
            BillPrintPreview(billid);
        });
    } else {
        $('#PaymentHeaderDiv').hide();
    }
    /* Bind Payment Details Header Section*/



    /* Bind Patients Data to Dropdown */
    if (patientsDataRequired && patientsData != null && $("#ddlPatients").length > 0) {
        BindDropdownData(patientsData, "#ddlPatients", "#hdPatientId");
    }
    /* Bind Patients Data to Dropdown */


    if (clearForm) {
        ClearPaymentForm();
    }
}

/* $$$$$$$$$$$$$$ New Methods with improvements / big fixes on Billing / Payments $$$$$$$$$$$$$$ */











/* ########### Below Methods are not in-use ########### */
function BindPaymentsList() {
    var jsonData = JSON.stringify({
        patientId: $("#hdPatientId").val(), encounterId: $("#hdEncounterId").val(), billHeaderId: $("#ddlBillHeaders").val()
    });
    $.ajax({
        type: "POST",
        url: '/Payment/BindPaymentList',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            BindList("#DivPaymentsList", data);
            $('#CollapsePaymentsList').addClass('in');
            if ($("#ddlBillHeaders").val() != '' || $("#ddlBillHeaders").val() != '0') {
                var billid = $("#ddlBillHeaders").val();
                BindPaymentHeaderTable(billid);
                $("#ViewBill").show();
                $("#ViewBill").click(function () {
                    BillPrintPreview(billid);
                });
            } else {
                $('#PaymentHeaderDiv').hide();
            }

        },
        error: function (msg) {
        }
    });
}

function ApplyManualPayment() {
    $.ajax({
        type: "POST",
        url: '/Payment/ApplyPaymentManualToBill',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data) {
                ShowMessage('Payment successfully added to bill.', "Success", "success", true);
            } else {
                ShowMessage('Unable to add payment to bill.', "Warning", "warning", true);
            }
            setTimeout(function () {
                window.location.reload(true);
            }, 1000);
        },
        error: function (msg) {
        }
    });
}

function BindPaymentHeaderTable(billid) {
    $.ajax({
        type: "POST",
        url: '/PaymentDetails/GetPaymentDetails',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ BillHeaderId: billid }),
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
        },
        error: function (msg) {
        }
    });
}

function BindEncountersDropdown(pId) {

    var patientId = pId;
    if (patientId > 0) {
        $.ajax({
            type: "POST",
            url: '/Home/GetEncountersListByPatientId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ patientId: patientId }),
            success: function (data) {
                //GetPatientNameById(pId);
                BindDropdownData(data, "#ddlEncounters", "#hdEncounterId");

            },
            error: function (msg) {
            }
        });
    }
}

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

function SaveManualPayment() {
    var isValid = jQuery("#CollapsePaymentAddEdit").validationEngine({ returnIsValid: true });
    if (isValid) {
        var createdBy = 0;
        var createdDate = null;
        var paymentId = $("#hdPaymentID").val();
        if (paymentId > 0) {
            createdBy = $("#hdCreatedBy").val();
            createdDate = new Date($("#hdCreatedDate").val());
        }
        else {
            createdDate = new Date();
        }

        var jsonData = JSON.stringify({
            PaymentID: paymentId,
            PayType: 500,
            PayDate: $("#txtPayDate").val(),
            PayAmount: $("#txtPayAmount").val(),
            PayReference: $("#txtPaymentReference").val(),
            PayBillNumber: $("#ddlBillHeaders option:selected").text(),
            PayFor: $("#ddlPatients").val(),
            PayBy: $("#ddlPatients").val(),
            PayBillID: $("#ddlBillHeaders").val(),
            PayActivityID: null,
            PayEncounterID: $("#ddlEncounters").val(),
            PayXAFileHeaderID: null,
            PayIsActive: true,
            PayCreatedBy: createdBy,
            PaymentTypeId: $("#ddlPaymentType").val(),
            PayCreatedDate: new Date(createdDate),
            PTDCardNumber: $("#txtCardNumber").val(),
            PTDExpiryMonth: $("#ddlExpiryMonth").val(),
            PTDExpiryYear: $("#ddlExpiryYear").val(),
            PTDCardHolderName: $("#txtCardHolderName").val(),
            PTDPaymentType: $("#ddlPaymentType").val(),
            PTDSecurityNumber: $("#txtSecurityNumber").val()
        });
        $.ajax({
            type: "POST",
            url: '/Payment/SaveManualPayment',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearPaymentForm();
                BindPaymentsList();
                BindPatients();
                var msg = "Records Saved successfully !";
                if (paymentId > 0)
                    msg = "Records updated successfully";

                ShowMessage(msg, "Success", "success", true);
            },
            error: function (msg) {
            }
        });

    }
}

/* ########### Below Methods are not in-use ########### */

