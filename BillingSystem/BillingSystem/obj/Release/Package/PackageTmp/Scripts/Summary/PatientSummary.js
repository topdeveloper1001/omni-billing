//1. Patient Summary
//2. Discharge Summary
//3. Orders
//4. Diagnosis
//5. Vitals
//6. Lab
//7. Rad/Images
//8. Surgery
//9. Pharmacy
//10. Physician Tasks
//11. Nurse Tasks
//12. Patient Care Plan
//13. Legal Documents
//14. Allergies/history
//15. Old Paper Med Rec
//16. Personal Fitness
//17. Patient Messaging

var summaryPageUrl = "/Summary/";

$(function () {

    $("#aEvaluation").hide();
    OnClickShowHideActions(true);

    $(".menu")
        .click(function () {
            $(".out").toggleClass("in");
        });

    $("#aEvaluation")
        .click(function () {
            ViewNewEvaluationForm();
        });

    $("#btnSelectDrugAllergy")
        .on("click",
        function () {
            var id = $("#hidOrderIdPopup").val();
            var currentMedicationAllergy = $("#hidCurrentMedicationAllergy").val();
            if (currentMedicationAllergy != "0" && currentMedicationAllergy != "") {
                $("#Allegydiv").hide();
                AddAllergyLogging(false, 1);
            } else {
                //Add the order by uncoomnting the below add order method
                AddOrder(id);
                $(".hidePopUp").hide();
                AddAllergyLogging(false, 1);
            }
            $("#hidDrugAllergy").val("0");
        });

    $("#btnCancelDrugAllergy")
        .click(function () {
            $(".hidePopUp").hide();
            AddAllergyLogging(true, 1);
            ResetOrder();
            return false;
        });

    $("#btnCancelMedicationAllergy")
        .click(function () {
            $(".hidePopUp").hide();
            AddAllergyLogging(true, 2);
            ResetOrder();
            return false;
        });

    $("#btnSelectMedicationAllergy")
        .click(function () {
            var id = $("#hidOrderIdPopup").val();
            var drugAllergy = $("#hidDrugAllergy").val();
            if (drugAllergy != "0" && drugAllergy != "") {
                //----- Hide the lower section 
                AddAllergyLogging(false, 2);
                $("#MedicationDiv").hide();
            } else {
                //Add the order by uncoomnting the below add order method
                AddAllergyLogging(false, 2);
                AddOrder(id);
                $(".hidePopUp").hide();
            }
            $("#hidCurrentMedicationAllergy").val("0");
        });


    //-------------Added for Super Powers functionality-------------///
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();

    $("#GlobalPatientId").val(patientId);
    $("#GlobalEncounterId").val(encounterId);
    BindLinkUrlsForSuperPowers();

    LoadPartialViewFromExternalLink($("#ExternalLinkId").val());

    //-------------Added for Super Powers functionality-------------///
    var keyupFiredCount = 0;

    //ReadBarCode
    function DelayExecution(f, delay) {
        var timer = null;
        return function () {
            var context = this, args = arguments;

            clearTimeout(timer);
            timer = window.setTimeout(function () {
                //f.apply(context, args);

                var barcode = $("#txttest").val();
                $("#txttest").val("");
                var barCodeValue = barcode.split("^^");
            },
                delay || 300);
        };
    }

    $.fn.ConvertToBarcodeTextbox = function () {

        $(this).focus(function () { $(this).select(); });

        $(this)
            .keydown(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);

                if ($(this).val() == "") {
                    keyupFiredCount = 0;
                }
                if (keycode == 13) { //enter key
                    return false;
                    event.stopPropagation();
                }
            });

        $(this)
            .keyup(DelayExecution(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                keyupFiredCount = keyupFiredCount + 1;
            }));
    };


    $("#txttest").ConvertToBarcodeTextbox();

    $("#txtOrderStartDate")
        .datetimepicker({
            format: "Y/m/d",
            onShow: function (ct) {
                this.setOptions({
                    maxDate: $("#txtOrderEndDate").val() ? $("#txtOrderStartDate").val() : false
                });
            },
            timepicker: false
        });
    $("#txtOrderEndDate")
        .datetimepicker({
            format: "Y/m/d",
            onShow: function (ct) {
                this.setOptions({
                    minDate: $("#txtOrderStartDate").val() ? $("#txtOrderStartDate").val() : false
                });
            },
            timepicker: false
        });

    $(".hitme")
        .click(function () {
            $(".summery-tabs").toggleClass("moveRight");
        });

    $(".summery-tabs li a")
        .click(function () {
            $(".summery-tabs").removeClass("moveRight");
            $.validationEngine.closePrompt(".formError", true);
        });

    $(".searchHitMe")
        .click(function () {
            $(".searchSlide").toggleClass("moveLeft");
        });

    $("#btnCancelOrder")
        .click(function () {
            // update the block message 
            //$.blockUI({ message: "<h1>Remote call in progress...</h1>" });
            var orderId = $("#hidCancelOrderId").val();
            CancelOrder(orderId);
        });

    $("#btnCancel")
        .click(function () {
            var orderId = $("#hidCancelOrderId").val();
            HideCancelOrderPopup(orderId);
            return false;
        });

    $("#btnCancelOrderActivity")
        .click(function () {
            // update the block message 
            //$.blockUI({ message: "<h1>Remote call in progress...</h1>" });
            var orderactivityId = $("#hidCancelOrderActivityId").val();
            CancelOrderActivity(orderactivityId);
        });

    $("#btnCancelActivity")
        .click(function () {
            var orderId = $("#hidCancelOrderActivityId").val();
            HideCancelOrderActivityPopup(orderId);
            return false;
        });

    $("#btnTakeSpeciman")
        .click(function () {
            // update the block message 
            //$.blockUI({ message: "<h1>Remote call in progress...</h1>" });
            var specimenOrderactivityId = $("#hidSpecimanOrderActivityId").val();
            UpdateLabOrderActvityStatus(specimenOrderactivityId);
        });

    $("#btnCancelSpeciman")
        .click(function () {
            var orderId = $("#hidSpecimanOrderActivityId").val();
            HideSpecimenPopup(orderId);
            return false;
        });


    $("#btnDeleteNotes")
        .click(function () {
            // update the block message 
            //$.blockUI({ message: "<h1>Remote call in progress...</h1>" });
            var notesid = $("#hidNotesId").val();
            DeleteMedicalNotes(notesid);
        });

    $("#btnCancelDeleteNotes")
        .click(function () {
            $("#hidNotesId").val("");
            $.unblockUI();
            return false;
        });


});

function OpenSignature() {
    $("#divSignature").css({ "display": "block" });
    $("#divDocumentsGrid").css({ "display": "none" });

}

function OpenNurseAssessmentSignatureDiv() {
    clear_canvas_width();
    $("#divSignatureInNurseAssessment").css({ "display": "block" });
    // $("#divDocumentsGrid").css({ "display": "none" });

}

function clear_canvas_width() {
    var s = document.getElementById("scribbler");
    var w = s.width;
    s.width = 10;
    s.width = w;
}

function TemplatesSelection() {
    var ddlValue = $("#ddNurseAssessmentForm").val();
    if (ddlValue == "101") {
        ViewNurseAssessmentForm();
    }
}


function ViewNurseAssessmentForm() {
    var patientId = $("#hdPatientId").val();
    var jsonData = JSON.stringify({
        pId: patientId,
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PDFTemplates/PdfTemplatesData",
        data: jsonData,
        dataType: "html",
        success: function (data) {

            $("#NurseAssessmentDiv").empty();
            $("#NurseAssessmentDiv").html(data);
            $("#divNurseAssessmentGrid").show();
            //BindSignatureData(setId);
            InitializeDateTimePicker();

        },
        error: function (msg) {

        }
    });
}


//function fillAndDownloadPDF() {
//    if ($("#ddNurseAssessmentForm").val() == "0") {
//        ShowMessage("Please select the Nurse Form", "Warning", "warning", true);
//        return false;
//    }
//    var jsonData = JSON.stringify({
//        PatientId: $("#hdPatientId").val(),
//        EncounterId: $("#hdCurrentEncounterId").val(),
//        NurseFormId: $("#ddNurseAssessmentForm").val()
//    });
//    $.ajax({
//        type: "POST",
//        url: summaryPageUrl + "fillAndDownloadPDF?PatientId=" + $("#hdPatientId").val() + "&EncounterId=" + $("#hdCurrentEncounterId").val() + "&NurseFormId=" + $("#ddNurseAssessmentForm").val(),
//        data: jsonData,
//        async: true,
//        dataType: "json",
//        success: function (data) {
//            
//            var splitArray = data.split("`");
//            $("#divDocumentsGrid").show();
//            document.getElementById('DocumentsGridDiv').src = splitArray[0];
//            $("#hfPdfFileName").val(splitArray[1]);
//        },
//        error: function (e) {
//        }

//    });
//}


function GenerateBarCode() {
    var jsonData = JSON.stringify({
        PatientId: $("#hdPatientId").val(),
        EncounterId: $("#hdCurrentEncounterId").val()
    });
    $.ajax({
        type: "POST",
        url: summaryPageUrl + "GenerateBarCodeImages?PatientId=" +
        $("#hdPatientId").val() +
        "&EncounterId=" +
        $("#hdCurrentEncounterId").val(),
        data: jsonData,
        async: true,
        dataType: "html",
        success: function (data) {

            $("#divPrint").html(data);
            $("#aShowBarcode").show();
        },
        error: function (e) {
        }

    });
}

function ShowBarCode() {
    var contents = document.getElementById("divPrint").innerHTML;
    var frame1 = document.createElement("iframe");
    frame1.name = "frame1";
    frame1.style.position = "absolute";
    frame1.style.top = "-1000000px";
    document.body.appendChild(frame1);
    var frameDoc = frame1.contentWindow
        ? frame1.contentWindow
        : frame1.contentDocument.document ? frame1.contentDocument.document : frame1.contentDocument;
    frameDoc.document.open();
    frameDoc.document.write("<html><head><title>DIV Contents</title>");
    frameDoc.document.write("</head><body>");
    frameDoc.document.write(contents);
    frameDoc.document.write("</body></html>");
    frameDoc.document.close();
    setTimeout(function () {
        window.frames["frame1"].focus();
        window.frames["frame1"].print();
        document.body.removeChild(frame1);
    },
        500);
    return false;

}

function PrintGeneratedBarCode() {
    var contents = document.getElementById("printMainDiv").innerHTML;
    var frame1 = document.createElement("iframe");
    frame1.name = "frame1";
    frame1.style.position = "absolute";
    frame1.style.top = "-1000000px";
    document.body.appendChild(frame1);
    var frameDoc = frame1.contentWindow
        ? frame1.contentWindow
        : frame1.contentDocument.document ? frame1.contentDocument.document : frame1.contentDocument;
    frameDoc.document.open();
    frameDoc.document.write("<html><head><title>Generated Barcode</title>");
    frameDoc.document.write("</head><body>");
    frameDoc.document.write(contents);
    frameDoc.document.write("</body></html>");
    frameDoc.document.close();
    setTimeout(function () {
        window.frames["frame1"].focus();
        window.frames["frame1"].print();
        document.body.removeChild(frame1);
    },
        500);
    return false;

}

function ShowGeneratedBarCode(orderActivityId) {
    $.ajax({
        type: "POST",
        url: summaryPageUrl + "ShowGeneratedBarCode?orderActivityId=" + orderActivityId,
        data: {},
        async: true,
        dataType: "json",
        success: function (data) {

            $("#divShowGeneratedBarCode").show();
            $("#ShowGeneratedBarCodeDiv").html(data);
            //insert data in divPrintLabel Div and use this div to print label
            $("#divPrintLabel").html(data);
        },
        error: function (e) {
        }

    });
}

function PrintBarCode() {
    var browser = get_browser_info();
    if (browser.name == "Chrome") {
        //$("#printable1").print();       
        /*var data = $("#divPrint").html();
        var printpage = "divPrint";
        var headstr = "<html><head><title></title></head><body>";
        var footstr = "</body>";
        var newstr = document.all.item(printpage).innerHTML;
        var oldstr = document.body.innerHTML;
        document.body.innerHTML = headstr + newstr + footstr;
        window.print();
        window.close();
        //document.body.innerHTML = oldstr;
        return false;*/
        var data = $("#divPrint").html();
        var printpage = "divPrint";
        var headstr = "<html><head><title></title></head><body>";
        var footstr = "</body>";
        var newstr = document.all.item(printpage).innerHTML;


        printWindow = window.open("", "", "height=400,width=800");
        printWindow.document.write(newstr);
        printWindow.focus();
        // need time to load the barcode before print
        setTimeout(function () { printWindow.print(); }, 200);
        return false;
    } else {
        //$("#divPrint").print();
        var divContents = $("#divPrint").html();
        var printWindow = window.open("", "", "height=400,width=800");
        printWindow.document.write("<html><head><title>DIV Contents</title>");
        printWindow.document.write("</head><body >");
        printWindow.document.write(divContents);
        printWindow.document.write("</body></html>");
        printWindow.document.close();
        printWindow.print();
    }
    return false;
}

function get_browser_info() {
    var ua = navigator.userAgent,
        tem,
        M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
    if (/trident/i.test(M[1])) {
        tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
        return { name: "IE ", version: (tem[1] || "") };
    }
    if (M[1] === "Chrome") {
        tem = ua.match(/\bOPR\/(\d+)/);
        if (tem != null) {
            return { name: "Opera", version: tem[1] };
        }
    }
    M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, "-?"];
    if ((tem = ua.match(/version\/(\d+)/i)) != null) {
        M.splice(1, 1, tem[1]);
    }
    return {
        name: M[0],
        version: M[1]
    };
}

function RowColor() {
    $("#EncounterClosedOrders tbody tr")
        .each(function (i, row) {
            var $actualRow = $(row);
            if ($actualRow.find(".col10").html().indexOf("Open") != -1) {
                $actualRow.removeClass("rowColor3");
                $actualRow.removeClass("rowColor1");
                $actualRow.removeClass("rowColor2");
            } else if ($actualRow.find(".col10").html().indexOf("Open and Administered") != -1) {
                $actualRow.addClass("rowColor1");
            } else if ($actualRow.find(".col10").html().indexOf("Closed") != -1) {
                $actualRow.addClass("rowColor3");
            } else if ($actualRow.find(".col10").html().indexOf("On Bill") != -1) {
                $actualRow.addClass("rowColor2");
            }
        });
}

function CustomDropdown() {
    $("#ddlOrderCodes")
        .selectBoxIt({
            theme: "default",
            defaultText: "Please Select",
            autoWidth: false
        });
    $(".testselectset")
        .change(function () {
            //alert("You selected: " + this.value + " from the Selectboxit plugin");
        });
}

function GetFavoritesOrders() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetFavoritesOrders",
        data: null,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            $("#favOrdersGrid").empty();
            $("#favOrdersGrid").html(data);

            $("#favOrdersGrid1").empty();
            $("#favOrdersGrid1").html(data);
        },
        // ReSharper disable once UnusedParameter
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function GetMostRecentOrders() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetMostRecentOrders",
        data: null,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            $("#MostRecentOrdersGrid").empty();
            $("#MostRecentOrdersGrid").html(data);
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function GetPhysicianAllClosed() {
    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindClosedOrders",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            $("#divEncounterClosedOrders").empty();
            $("#divEncounterClosedOrders").html(data);
            RowColor();
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function AddToFavorites(orderId, favoriteId, markFavorite, favDesc) {
    var ordertypeCategory = $("#ddlOrderTypeCategory :selected").text();
    var orderCode = $("#ddlOrderCodes").val();
    var jsonData = JSON.stringify({
        codeId: orderCode,
        categoryId: ordertypeCategory == "Pharmacy" ? "5" : "3",
        id: favoriteId,
        isFavorite: markFavorite,
        favoriteDesc: favDesc,
        Dtype: "false"
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "AddToFavorites",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            if (data != null) {
                if (data == "1") {
                    ShowMessage("Record already exist.", "Info", "warning", true);
                } else {
                    // ShowMessage("Record added successfully.", "Success", "success", true);
                    $("#favOrdersGrid").empty();
                    $("#favOrdersGrid").html(data);
                    GetPhysicianAllOrders();
                }
            }
        },
        error: function (msg) {

        }
    });
}

function EditFavorite(orderId) {
    var jsonData = JSON.stringify({
        codeId: orderId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetFavoriteByCodeId",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            if (data != null) {
                $("#hdFavoriteId").val(data.id);
                if (data.isFavorite != null && data.isFavorite == true) {
                    $("#chkMarkAsFavorite").prop("checked", "checked");
                    $("#favoriteOrderDescDiv").show();
                    $("#txtFavoriteDescription").val(data.description);
                }
            } else {

            }
        },
        error: function (msg) {

        }
    });
}

function BindOrderGrid(encounterid) {

    var jsonData = JSON.stringify({ EncounterId: encounterid });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindEncounterOrderList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            if ($("#colCurrentOrdersMain").length > 0)
                BindList("#colCurrentOrdersMain", data);

            if ($("#NurseAdminOpenOrdersListDiv").length > 0)
                BindList("#NurseAdminOpenOrdersListDiv", data);

            $("#collapseOpenOrderAddEdit").removeClass("in");
            $("#collapseOpenOrderlist").addClass("in");
        },
        error: function (msg) {
            alert(msg);
        }

    });
}

function EditOrder1(id) {
    var jsonData = JSON.stringify({ orderId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetOrderDetailById",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#OpenOrderDiv").empty();
            $("#OpenOrderDiv").html(data);
            InitializeDateTimePicker();
            EditFavorite(id);
            //BindCategoriesInSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
            BindOrderTypeCategoriesinSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID", "0");

            if ($("#hdOrderCodeId").val() != "") {
                $("#txtOrderCode").val($("#hdOrderCodeId").val());
            }
            //BindGlobalCodesWithValue("#ddlFrequencyList", 1024, "#hdFrequencyCode");
            //BindGlobalCodesWithValue("#ddlOrderStatus", 3102, "#hdOrderStatus");
            //BindGlobalCodesWithValueWithOrder("#ddlQuantityList", 1011, "#hdQuantity");
            BindOrdersRelatedDropdowns(2);

            $("#ddlFrequencyList option:contains(" + $("#hdFrequencyCode").val() + ")").attr("selected", "selected");
            $("#ddlOrderStatus option[value='3']").remove();
            $("#ddlOrderStatus option[value='4']").remove();
            $("#ddlOrderStatus option[value='9']").remove();
            CheckForMultipleActivites($("#hfOpenOrderid").val());
            setTimeout(function () {
                $("#txtOrderCode").val($("#ddlOrderCodes :selected").text());
            },
                1000);
            $("html, body").animate({ scrollTop: $("#collapseOpenOrderAddEdit").offset().top }, "fast");
        },
        error: function (msg) {

        }
    });
}

// changes by ashwani to get by patient ID
function BindDiagnosisType(selector, hidValueSelector) {
    var jsonData = JSON.stringify({
        patientId: $("#hdPatientId").val()
    });
    $.ajax({
        type: "POST",
        url: summaryPageUrl + "GetDiagnosisCodes",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data,
                    function (i, diagnosisCode) {
                        items += "<option value='" +
                            diagnosisCode.DiagnosisCode1 +
                            "'>" +
                            diagnosisCode.DiagnosisFullDescription +
                            "</option>";
                    });
                $(selector).html(items);

                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
                    $(selector).val($(hidValueSelector).val());
            }
        },
        error: function (msg) {
        }
    });
}

// changes by ashwani to get by patient ID
function IsValidOrder(id) {
    debugger;
    var isValid = false;
    if ($("#divPhysicianOrder").html() != null) {
        isValid = jQuery("#divPhysicianOrder").validationEngine({ returnIsValid: true });
    } else {
        isValid = jQuery("#OpenOrderDiv").validationEngine({ returnIsValid: true });
    }
    if ($("#hdCurrentEncounterId").val() != "" && $("#hdCurrentEncounterId").val() > "0") {
        if (id > 0) {
            if ($("#ddlOrderStatus").val() == "3") {
                if (confirm("Do you want to closed the open activities for current order?")) {
                    if (isValid == true) {
                        AddOrder(id);
                    }
                    UpdateOrderActivities(id);
                }
            } else if (isValid == true) {
                AddOrder(id);
            }
        } else {
            if (isValid == true) {
                if ($("#chkOutstandingOrder").prop("checked")) {
                    AddOutstandingOrder(id);
                } else {
                    if ($("#ddlOrderTypeCategory").val().trim() == "11100") {
                        CheckTheDurgAllergy(id);
                    } else {
                        AddOrder(id);
                    }
                }
            }
        }
    } else {
        ShowMessage("Encounter is not started yet!", "Warning", "warning", true);
    }

    //if (isValid == true) {
    //    ResetOrder();
    //}
    return false;
}

function UpdateOrderActivities(orderid) {
    var jsonData = JSON.stringify({
        orderid: orderid
    });
    $.ajax({
        type: "POST",
        url: summaryPageUrl + "UpdateOpenOrderActivities",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {

            } else {
            }
        },
        error: function (msg) {

        }
    });
}

function ResetOrder() {
    $.validationEngine.closePrompt(".formError", true);
    $("#OpenOrderDiv").clearForm();
    //$(".js-example-basic-hide-search").select2('data', { id: null, text: null });
    //$(".js-example-basic-hide-search").select2({
    //    placeholder: "--Select--"
    //});

    ResetAllDropdowns("#OpenOrderDiv");
    $("#collapseOpenOrderAddEdit").removeClass("in");
    $("#collapseOpenOrderlist").addClass("in");
    $("#hdPrimaryDiagnosisId").val("");
    $("#hdOrderTypeCategoryID").val("0");
    $("#hdFrequencyCode").val("0");
    $("#hdOrderTypeSubCategoryID").val("0");
    $("#hdOrderTypeId").val("0");
    $("#hdOrderStatus").val("0");
    $("#hdOrderCodeId").val("0");
    $("#btnAddOrder").attr("onclick", 'return IsValidOrder("0");');
    $("#ddlOrderTypeCategory").val("0");
    $("#ddlOrderTypeSubCategory").empty();
    $("#ddlOrderStatus").val("1");
    $("#ddlFrequencyList").val("10");
    $("#ddlQuantityList").val("1.00");
    $("#ddlOrderCodes").empty();
    $(".DrugDDL").hide();
    //BindCategoriesInSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
    BindOrderTypeCategoriesinSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID", "0");

    var currentdate = new Date();
    var datewithFormat = currentdate.format("mm/dd/yyyy");
    $("#txtOrderStartDate").val(datewithFormat);
    $("#txtOrderEndDate").val(datewithFormat);
}

function EditPhysicanOrderDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.Id;
    EditOrder(id);
}

function ClearPhysicianOrderForm() {
    $("#OpenOrderDiv").clearForm(true);
    $("#ddlOrderStatus").val(OrderStatus.Open);
    $("#collapseOpenOrderAddEdit").removeClass("in");
    $("#collapseOpenOrderlist").addClass("in");
    InitializeDateTimePicker();
    ResetAllDropdowns("#OpenOrderDiv");
    //BindCategoriesInSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
    BindOrderTypeCategoriesinSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID", "11320");

    $("#ddlFrequencyList").val("10");
    var currentdate = new Date();
    var datewithFormat = currentdate.format("mm/dd/yyyy");
    $("#txtOrderStartDate").val(datewithFormat);
    $("#txtOrderEndDate").val(datewithFormat);
    $("#ddlOrderStatus option[value='4']").remove();
    $("#ddlOrderStatus option[value='3']").remove();
    $("#ddlOrderStatus option[value='2']").remove();
    $("#ddlOrderStatus option[value='9']").remove();
    $("#btnAddOrder").attr("onclick", 'return IsValidOrder("0");');
}

function ClearPhysicianOrderAll(encounterid) {
    ClearPhysicianOrderForm();
    $.validationEngine.closePrompt(".formError", true);
}

var medicalNotesid = 1;

function bindNotesData(notestypeid) {
    $("#hdNotesUserType").val(notestypeid);
    $("#aEvaluation").hide();
    $(".ehrtabs").empty();
    if (notestypeid == "1") {
        $("#hfTabValue").val("10");
        $("#aEvaluation").show();
        BindNotesdata(1);
        BindLabOrdersListByPhysician();
    } else if (notestypeid == "2") {
        $("#hfTabValue").val("11");
        BindNotesdata(2);
    }
    $("#divPhyNotes").show();

    medicalNotesid = notestypeid;
    OnClickNurseAdminTab(notestypeid);
}

function BindNotesdata(type) {
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: pid,
        type: type,
        encounterId: eid
    });
    if (pid != "" && pid > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "BindMedicalNotesData",
            data: jsonData,
            dataType: "html",
            success: function (data) {
                if (data != null) {
                    $("#aEvaluation").show();
                    $('#MedicalVitalListDiv').empty();
                    $("#NotesTab").empty();
                    $("#NotesTab").html(data);
                    BindGlobalCodesWithValue("#ddlNotesTypes", 963, "");
                    if (type == 1)
                        BindAddOrdersPaneldata("0");
                    else
                        BindAddOrdersPaneldata("");


                    if (!$('#colClosedActivities').hasClass('in'))
                        $('#colClosedActivities').addClass('in');

                    if (!$('#colClosedOrders').hasClass('in'))
                        $('#colClosedOrders').addClass('in');

                    if ($('#collapseVitalList').hasClass('in')) {
                        $('#PatientCurrentVitals').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                    } else {
                        $('#collapseVitalList').addClass('in');
                        $('#PatientCurrentVitals').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                        $('#collapseVitalList').removeClass('in');
                    }
                    SetGridSorting(SortMedicalVital, "#gridContentVital");
                }
            },
            error: function (msg) {

            }
        });
    }
}

function EditMedicalNotes(id) {
    var jsonData = JSON.stringify({
        medicalNotesId: id
    });
    $.ajax({
        type: "POST",
        url: "/MedicalNotes/GetMedicalNotes",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            if (data) {
                $("#phyMedicalNotesFormDiv").empty();
                $("#phyMedicalNotesFormDiv").html(data);
                $("#collapseMedicalNotesAddEdit").addClass("in").attr("style", "");
                BindGlobalCodesWithValue("#ddlNotesTypes", 963, "#hdMedicalNotesType");
            } else {
            }
        },
        error: function (msg) {

        }
    });
}

function SaveMedicalNotes(id) {
    var isValid = jQuery("#MedicalNotesFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var notesUserType = $("#hdNotesUserType").val();
        var txtPatientId = $("#hdPatientId").val();
        var medicalNotesType = $("#ddlNotesTypes").val();
        var txtEncounterId = $("#hdCurrentEncounterId").val();
        var txtMedicalRecordNumber = $("#hdPatientMRN").val();
        var txtNotes = $("#txtPhyNotes").val();
        var markedComplication = $("#chkMarkedComplication")[0].checked;
        var jsonData = JSON.stringify({
            MedicalNotesID: id,
            NotesUserType: notesUserType,
            PatientID: txtPatientId,
            EncounterID: txtEncounterId,
            MedicalRecordNumber: txtMedicalRecordNumber,
            Notes: txtNotes,
            MedicalNotesType: medicalNotesType,
            MarkedComplication: markedComplication
        });

        $.ajax({
            type: "POST",
            url: "/MedicalNotes/SaveMedicalNotes",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearNotesAll();
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";
                $("#collapseMedicalNotesList").addClass("in").attr("style", "");
                //collapseMedicalNotes
                ShowMessage(msg, "Success", "success", true);
                BindGlobalCodesWithValue("#ddlNotesTypes", 963, "");
                BindMedicalNotesGrid();
            },
            error: function (msg) {

            }
        });
    }
}

function BindMedicalNotesGrid() {
    var jsonData = JSON.stringify({
        patientId: $("#hdPatientId").val(),
        notesUserTypeId: medicalNotesid == 0 ? $("#hdNotesUserType").val() : medicalNotesid
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/MedicalNotes/BindMedicalNotesList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#phyMedicalNotesListDiv").empty();
            $("#phyMedicalNotesListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearNotesForm() {
    $("#phyMedicalNotesFormDiv").clearForm();
    $("#collapseOpenOrderlist").removeClass("in");
}

function ClearNotesAll() {
    ClearNotesForm();
    $.validationEngine.closePrompt(".formError", true);
    $("#ddlNotesTypes").val("0");
    $("#btnUpdateNotes").attr("onclick", "return SaveMedicalNotes('0')");
    $("#btnUpdateNotes").val("Save");
    //$.ajax({
    //    type: "POST",
    //    url: '/MedicalNotes/ResetMedicalNotesForm',
    //    async: false,
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "html",
    //    data: null,
    //    success: function (data) {
    //        if (data) {
    //            $('#phyMedicalNotesFormDiv').empty();
    //            $('#phyMedicalNotesFormDiv').html(data);
    //            BindPatientSummaryNotes();
    //        }
    //    },
    //    error: function (msg) {


    //        return true;
    //    }
    //});

}

function DeleteMedicalNotes() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            medicalNotesId: $("#hfGlobalConfirmId").val(),
        });
        $.ajax({
            type: "POST",
            url: "/MedicalNotes/DeleteMedicalNotes",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindMedicalNotesGrid();
                    ShowMessage("Records Deleted Successfully", "Success", "success", true);
                }
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

//function DeleteMedicalNotes(id) {
//    if (confirm("Do you want to delete this record? ")) {
//        var txtMedicalNotesId = id;
//        var jsonData = JSON.stringify({
//            medicalNotesId: txtMedicalNotesId,
//        });
//        $.ajax({
//            type: "POST",
//            url: '/MedicalNotes/DeleteMedicalNotes',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                if (data) {
//                    BindMedicalNotesGrid();
//                    ShowMessage("Records Deleted Successfully", "Success", "success", true);
//                }
//            },
//            error: function (msg) {
//                return true;
//            }
//        });
//    }
//}

function BindGenericTypeDDL(selector, categoryIdval, hidValueSelector) {
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
        success: function (data) {

            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data,
                    function (i, globalCode) {
                        items += "<option value='" +
                            globalCode.GlobalCodeID +
                            "'>" +
                            globalCode.GlobalCodeName +
                            "</option>";
                    });
                $(selector).html(items);

                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
                    $(selector).val($(hidValueSelector).val());
            } else {
            }
        },
        error: function (msg) {
        }
    });
}

function OpenAddDiagnosis() {
    var txtPatientId = $("#hdPatientId").val();
    var txtEncounterId = $("#hdCurrentEncounterId").val();
    window.location.href = window.location.protocol +
        "//" +
        window.location.host +
        "/Diagnosis/Index?pId=" +
        txtPatientId +
        "&eId=" +
        txtEncounterId;
}

function BindCategoriesInSummary(ddlSelector, hdSelector) {
    /// <summary>
    ///     Binds the categories.
    /// </summary>
    /// <param name="ddlSelector">The DDL selector.</param>
    /// <param name="hdSelector">The hd selector.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        startRange: "11000",
        endRange: "11999"
    });
    $.ajax({
        type: "POST",
        url: summaryPageUrl + "GetOrderTypeCategoriesInSummary",           //GetGlobalCodeCategories
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $(ddlSelector).empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data,
                function (i, gcc) {
                    items += "<option value='" +
                        gcc.GlobalCodeCategoryValue.trim() +
                        "'>" +
                        gcc.GlobalCodeCategoryName +
                        "</option>";
                });
            $(ddlSelector).html(items);

            var hdValue = $(hdSelector).val();
            if (hdValue != null && hdValue != "" && hdValue > 0) {
                $(ddlSelector).val(hdValue);
                OnChangeCategory(ddlSelector, "#ddlOrderTypeSubCategory", "#hdOrderTypeSubCategoryID");
            } else {
                var tabvalue = $("#hfTabValue").val();
                var selectedVal = "0";
                switch (tabvalue) {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "10":
                    case "11":
                        selectedVal = "0";
                        break;
                    case "6":
                        selectedVal = "11080"; // OrderCodeTypes.PathologyandLaboratory;
                        break;
                    case "7":
                        selectedVal = "11070"; //OrderCodeTypes.Radiology;
                        break;
                    case "8":
                        selectedVal = "11010"; //OrderCodeTypes.Surgery;
                        break;
                    case "9":
                        selectedVal = "11100"; //OrderCodeTypes.Pharmacy;
                        break;
                    default:
                        selectedVal = "0";
                }
                $(ddlSelector).val(selectedVal);
                //if (selectedVal != "0") {
                //    $(ddlSelector).attr("disabled", "disabled");
                //}
                OnChangeCategory(ddlSelector, "#ddlOrderTypeSubCategory", "#hdOrderTypeSubCategoryID");
            }
        },
        error: function (msg) {
        }
    });
}

function OnChangeCategory(categorySelector, ddlSelector, hdSubCategorySelector) {
    var categoryId = $(categorySelector).val();
    if (categoryId != "" && categoryId != "0" && categoryId != null) {
        var jsonData = JSON.stringify({
            categoryId: categoryId
        });
        $("#divSpecimanType").hide();
        if ($("#ddlOrderTypeCategory :selected").text() == "Pharmacy") {
            $("#lblSubcategory").html("Generic Drug Name");
            $("#ddlFrequencyList").removeAttr("disabled");
        } else if ($("#ddlOrderTypeCategory :selected").text() == "Medicine") {
            $("#lblSubcategory").html("Order Type Sub-Category");
            $("#ddlFrequencyList").val("10");
            $("#ddlFrequencyList").removeAttr("disabled");
        } else if ($("#ddlOrderTypeCategory").val() == "11080") {
            $("#lblSubcategory").html("Order Type Sub-Category");
            $("#ddlFrequencyList").val("10");
            $("#divSpecimanType").show();
        } else if ($("#ddlOrderTypeCategory").val() == "11009") {
            $("#lblSubcategory").html("Order Type Sub-Category");
            //$('#ddlFrequencyList').val('10');
            $("#ddlFrequencyList").removeAttr("disabled");
        } else {
            $("#lblSubcategory").html("Order Type Sub-Category");
            $("#ddlFrequencyList").val("10");
            $("#ddlFrequencyList").attr("disabled", "disabled");
        }
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "GetOrderTypeSubCategories",
            //data: JSON.stringify({ id: $("#hdOrderTypeSubCategoryID").val() }),
            data: jsonData,
            dataType: "json",
            async: false,
            success: function (data) {
                if (data != null) {
                    BindOrderSubCategoriesWithCustomFields(data, ddlSelector, hdSubCategorySelector);
                    if ($(hdSubCategorySelector).val() > 0) {
                        OnChangeSubCatgory(ddlSelector);
                    }
                }
            },
            error: function (msg) {
            }
        });
    }
}

function SetValue(selector, value) {
    $(selector).val(value);
}

function SearchOrdersList() {
    var txtData = $("#txtSearchData").val();
    if (txtData != "") {
        var jsonData = JSON.stringify({
            text: txtData
        });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "GetSearchedOrders",
            data: jsonData,
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                if (data != null) {
                    $("#SearchedOrdersList").empty();
                    $("#SearchedOrdersList").html(data);
                } else {

                }
            },
            error: function (msg) {

            }
        });
    }
}

function GetSearchOrdersList() {
    var txtData = $("#txtSearchData").val();
    if (txtData != "") {
        var jsonData = JSON.stringify({
            text: txtData
        });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "GetSearchedOrdersList",
            data: jsonData,
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                if (data != null) {
                    BindSearchTab(data.SearchOrders, "#SearchOrders");
                }
            },
            error: function (msg) {

            }
        });
    }
}

function SelectDiagnosisTab() {
    BindGlobalCodesWithValue("#ddlOrderType", 1201, "#hdOrderType");
    $("#Diagnosis").click();
}

// Orders Section Data......
function LoadOrdersTabData1() {
    /// <summary>
    ///     Loads the orders tab data.
    /// </summary>
    /// <returns></returns>
    var encounterId = $("#hdCurrentEncounterId").val();
    encounterId = encounterId == "" ? "0" : encounterId;
    if (encounterId != "") {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "OrdersViewData",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            async: true,
            success: function (data) {
                if (data != null) {
                    $(".ehrtabs").empty();
                    $("#OrderTab").empty();
                    $("#OrderTab").html(data);
                    //SetGridSorting(BindOrdersBySort, "#gridContentOpenOrder");
                    //SetGridSorting(BindPhyFavOrdersBySort, "#PhyFavOrdersGrid");
                    //SetGridSorting(BindPhyMostOrdersBySort, "#phyAllOrdersGrid");
                    SetGridSorting(PhysicianAllSearch, "#gridphyAllOrdersGrid");
                    //SetGridPaging("?", "?encounterId=" + encounterId + "&");
                    BindOrdersRelatedDropdowns(2);

                    //BindCategoriesInSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
                    BindOrderTypeCategoriesinSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID", "0");

                    //RowColor();
                    InitializeDateTimePicker();
                    BindSearchToggle();
                    $("#ddlOrderStatus option[value='4']").remove();
                    $("#ddlOrderStatus option[value='2']").remove();
                    $("#ddlOrderStatus option[value='3']").remove();
                    $("#ddlOrderStatus option[value='9']").remove();
                    $("#ddlQuantityList").val("1.00");
                    $("#ddlOrderStatus").val("1");
                    $("#OpenOrderDiv").validationEngine();
                }
            },
            error: function (msg) {

            }
        });
    }
}

function BindSearchToggle() {
    /// <summary>
    ///     Binds the search toggle.
    /// </summary>
    /// <returns></returns>
    $(".searchHitMe")
        .click(function () {
            $(".searchSlide").toggleClass("moveLeft");
        });
}

function OnClickBindOrdersData() {
    /// <summary>
    ///     Called when [click bind orders data].
    /// </summary>
    /// <returns></returns>
    $("#aEvaluation").hide();
    $("#hfTabValue").val("3");
    LoadOrdersTabData();
    $(".editOpenOrder").show();
    $(".AdministerOrderActivity").hide();
    //var patientId = $("#hdPatientId").val();
    //var encounterId = $("#hdCurrentEncounterId").val();
    //if (encounterId != '' && encounterId > 0) {
    //    $.ajax({
    //        type: "POST",
    //        contentType: "application/json; charset=utf-8",
    //        url: summaryPageUrl + "CheckIfAnyPrimaryDiagnosisExists",
    //        data: JSON.stringify({ encounterId: encounterId }),
    //        dataType: "html",
    //        async: true,
    //        success: function (data) {
    //            if (data != null && data > 0) {
    //                $("#hdPrimaryDiagnosisId").val(data);
    //                //$('#divPhysicianOrder :input').attr('disabled', false);
    //            }
    //            else {
    //                //$('#divPhysicianOrder :input').attr('disabled', true);
    //                //ShowMessage("Add primary diagnosis first and then try again!", "Diagnosis Not Found", "warning", true);
    //            }
    //        },
    //        error: function (msg) {

    //        }
    //    });
    //}
    //else {
    //    //$('#divPhysicianOrder :input').attr('disabled', true);
    //    //ShowMessage("Add primary diagnosis first and then try again!", "Diagnosis Not Found", "warning", true);
    //}
}

function OnChangeMarkAsFavorite() {
    /// <summary>
    ///     Called when [change mark as favorite].
    /// </summary>
    /// <returns></returns>
    var markFavorite = $("#chkMarkAsFavorite").prop("checked");
    if (markFavorite == true) {
        $("#favoriteOrderDescDiv").show();
    } else {
        $("#favoriteOrderDescDiv").hide();
    }
}

function BindOrderCodesBySubCategoryID(data, ddlSelector, hdSelector) {
    /// <summary>
    ///     Binds the order codes by sub category identifier.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="ddlSelector">The DDL selector.</param>
    /// <param name="hdSelector">The hd selector.</param>
    /// <returns></returns>
    BindDropdownData(data, ddlSelector, hdSelector);
    $("#collapseOpenOrderAddEdit").addClass("in");
}

// Orders Section Data......
function LoadRadImagingData() {
    /// <summary>
    ///     Loads the RAD imaging data.
    /// </summary>
    /// <returns></returns>
    $("#hfTabValue").val("7");
    $("#aEvaluation").hide();
    var jsonData = JSON.stringify({
        associatedType: 2,
        pid: $("#hdPatientId").val()
    });
    $("#hdNotesUserType").val("1");
    $("#hfAssociatedType").val("2");
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/FileUploader/GetDocumentsByType",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $(".ehrtabs").empty();
            $(".Documents").empty();
            $("#LabDiv").html(data);
            BindOrdersRelatedDropdowns(1);
            $(".editRadImagingOrder, .RadOrder").show();
            BindAddOrdersPaneldata("11070");
            $("#hdNotesUserType").val("1");
        },
        error: function (msg) {

        }
    });
}

function LoadLegalDocumentsDiv() {
    $("#hfTabValue").val("13");
    $("#aEvaluation").hide();
    var jsonData = JSON.stringify({
        associatedType: 3,
        pid: $("#hdPatientId").val()
    });
    $("#hfAssociatedType").val("3");
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/FileUploader/GetDocumentsByType",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $(".ehrtabs").empty();
            $(".Documents").empty();
            $("#LegalDocDiv").html(data);
            BindGlobalCodesWithValue("#ddlDocumentType", 2306, "#hfDocumentTypeId");
            hideColumnColorRow(2, "PatientDocumentsGrid");
            hideColumnColorRow(6, "PatientDocumentsGrid");
            hideColumnColorRow(7, "PatientDocumentsGrid");
            // SetEncounterNumber();
        },
        error: function (msg) {

        }
    });
}

function LoadOldMedicalRecordsDiv() {
    $("#hfTabValue").val("15");
    $("#aEvaluation").hide();
    var jsonData = JSON.stringify({
        associatedType: 4,
        pid: $("#hdPatientId").val()
    });
    $("#hfAssociatedType").val("4");
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/FileUploader/GetDocumentsByType",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $(".ehrtabs").empty();
            $(".Documents").empty();
            $("#OldMedicalRecordsDiv").html(data);
            BindGlobalCodesWithValue("#ddlDocumentType", 2307, "#hfDocumentTypeId");
            hideColumnColorRow(2, "PatientDocumentsGrid");
            $(".OldMedicalRecord").show();
            //$('#txtReferenceNumber').addClass('validate[required,custom[integerdash]]');
            // SetEncounterNumber();
        },
        error: function (msg) {

        }
    });
}

function SetEncounterNumber() {

    if ($("#hfAssociatedType").val() == "4") {
        // $(".FileUploadEncounterNumber").show();
        BindOldEncounterList("#ddlOldEncounters");
        $("#ddlOldEncounters").addClass("validate[required]");
    } else {
        //$(".FileUploadEncounterNumber").hide();
        $("#ddlOldEncounters").removeClass("validate[required]");
    }
    //var encounterNUmber = $('#hidEncounterNumber').val();
    //if (encounterNUmber == null || encounterNUmber == "") {
    //    $('#txtDocumentEncounterId').removeAttr("readOnly");
    //} else {
    //    $('#txtDocumentEncounterId').val(encounterNUmber);
    //}
}

function BindOldEncounterList(selector) {
    var jsonData = JSON.stringify({
        pid: $("#hdPatientId").val()
    });
    $.ajax({
        type: "POST",
        url: "/Home/GetOldEncounterList",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data,
                    function (i, globalCode) {
                        items += "<option value='" +
                            globalCode.EncounterID +
                            "'>" +
                            globalCode.EncounterNumber +
                            "</option>";
                    });
                $(selector).html(items);
            } else {
            }
        },
        error: function (msg) {
        }
    });
}

function BindGlobalCode(selector, categoryIdval, hidValueSelector) {

    var jsonData = JSON.stringify({
        categoryId: categoryIdval
    });
    $.ajax({
        type: "POST",
        url: "/Home/GetGlobalCodesOrderBy",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data) {
                var items = '<option value="0">--Select--</option>';
                $.each(data,
                    function (i, globalCode) {
                        items += "<option value='" +
                            globalCode.GlobalCodeID +
                            "'>" +
                            globalCode.GlobalCodeName +
                            "</option>";
                    });
                $(selector).html(items);

                if ($(hidValueSelector) != null && $(hidValueSelector).val() > 0)
                    $(selector).val($(hidValueSelector).val());
            } else {
            }
        },
        error: function (msg) {
        }
    });
}

function LoadSurgerySection() {
    var encounterId = $("#hdCurrentEncounterId").val();
    var patientId = $("#hdPatientId").val();
    $("#aEvaluation").hide();
    if (patientId != null && encounterId != null && patientId > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/OperatingRoom/LoadSurgerySection",
            data: JSON.stringify({ patientId: patientId, encounterId: encounterId }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                BindList("#SurgeryTab", data);
                InitializeDateTimePicker();
            },
            error: function (msg) {
                //Console.log(msg);
            }
        });
        //$.post("/OperatingRoom/LoadSurgerySection",
        //    { patientId: patientId, encounterId: encounterId },
        //    function(data) {
        //        if (data != null) {

        //        }
        //    });
    }
}

//------------Not in Use----------------------
function OnChangeFrequency(txtSelector, ddlSelector) {
    var text = $(ddlSelector + " :selected").text();
    if (text == "Other") {
        $(txtSelector).val("NA");
        $(txtSelector).prop("readonly", true);
    } else {
        $(txtSelector).prop("readonly", false);
        $(txtSelector).val();
    }
}

function selectCode(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtOrderCode").val(dataItem.CodeDescription);
    $("#hdOrderCodeId").val(dataItem.Code);
    $("#txtCodeDescription").val(dataItem.Name);
}

function selectCodeDescription(e) {
    var dataItem = this.dataItem(e.item.index());
    $("#txtOrderCode").val(dataItem.CodeDescription);
    $("#hdOrderCodeId").val(dataItem.Code);
    $("#txtCodeDescription").val(dataItem.Name);
}

function OnCodeSelection(e) {
    var categoryId = $("#ddlOrderTypeCategory").val();
    var subCategoryId = $("#ddlOrderTypeSubCategory").val();
    if (subCategoryId == null)
        subCategoryId = 0;
    var value = null;
    if (e.filter.filters != null && e.filter.filters.length > 0) {
        value = e.filter.filters[0].value;
    }
    return {
        text: value,
        subCategoryId: subCategoryId,
        categoryId: categoryId
    };
}

function BindOpenCodes() {
    var jsonData = JSON.stringify({ codetypeid: $("#ddlOrderType").val() });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetOrderCodes",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            $("#ddlOrderCodeType").empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data,
                function (i, orderCode) {
                    items += "<option value='" +
                        orderCode.CodeNumbering +
                        "'>" +
                        orderCode.CodeDescription +
                        "</option>";
                });
            $("#ddlOrderCodeType").html(items);
            if ($("#ddlOrderCodeType") != null && $("#hdOrderCode").val() > 0)
                $("#ddlOrderCodeType").val($("#hdOrderCode").val());
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function BindDateTimePicker() {
    $(".datetimeF")
        .datepicker({
            yearRange: "-100: +15",
            changeMonth: true,
            dateFormat: "dd/mm/yy",
            changeYear: true,
            showSecond: true,
            timeFormat: "hh:mm:ss",
            stepHour: 2,
            stepMinute: 10,
            stepSecond: 10
        });
}

function editDetails(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var id = dataItem.OpenOrderID;
    EditOrder(id);
}

//------------Not in Use----------------------

function OnClickShowHideActions(isHide) {
    if (isHide) {
        $(".editOpenOrder").hide();
        $(".deleteVitalOrder").hide();
        $(".diagnosisActions").hide();
        $(".AdministerOrderActivity").hide();
    } else {
        $(".editOpenOrder").show();
        $(".deleteVitalOrder").show();
    }
}

function DeleteFav() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        $.ajax({
            type: "POST",
            url: "/PhysicianFavorites/DeleteFav",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ Id: $("#hfGlobalConfirmId").val() }),
            success: function (data) {
                if (data != null) {
                    var msg = "Records Deleted successfully !";
                    GetFavoritesOrders();
                    GetPhysicianAllOrders();
                    ShowMessage(msg, "Success", "success", true);
                }
            },
            error: function (msg) {
            }
        });
    }
}


function GetPhysicianAllOrders() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/PhysicianFavorites/GetPhysicianAllOrders",
        data: null,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            $("#MostRecentOrdersGrid").empty();
            $("#MostRecentOrdersGrid").html(data);
            $("#MostRecentOrdersGrid1").empty();
            $("#MostRecentOrdersGrid1").html(data);
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function AddAsFav(id) {
    AddToFavorites(id, 0, true, "");
}

var AddFavOrderToOrder = function (id) {
    var jsonData = JSON.stringify({ favorderId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetFavOrderDetailById",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $(".emptyddl").val("0");
            $(".emptytxt").val("");

            $("#OpenOrderDiv").empty();
            $("#OpenOrderDiv").html(data);
            InitializeDateTimePicker();
            BindOrdersRelatedDropdowns(2);
            EditFavorite(id);
            $(".AddOrder").attr("onclick", "return IsValidOrder('0');");
            //BindCategoriesInSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
            BindOrderTypeCategoriesinSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID", "0");

            if ($("#hdOrderCodeId").val() != "") {
                $("#txtOrderCode").val($("#hdOrderCodeId").val());
            }
            $("#ddlOrderStatus").val(OrderStatus.Open);
            $("#collapseOpenOrderAddEdit").addClass("in");
            $("#collapseOpenOrderAddEdit").removeAttr("style");
            $("#ddlOrderStatus option[value='4']").remove();
            $("#ddlOrderStatus option[value='2']").remove();
            $("#ddlOrderStatus option[value='3']").remove();
            $("#ddlOrderStatus option[value='9']").remove();
            setTimeout(function () { $("#txtOrderCode").val($("#ddlOrderCodes :selected").text()); }, 1000);
            $("#ddlOrderStatus").val("1");
            $("#ddlQuantityList").val("1.00");
            var currentdate = new Date();
            var datewithFormat = currentdate.format("mm/dd/yyyy");
            $("#txtOrderStartDate").val(datewithFormat);
            $("#txtOrderEndDate").val(datewithFormat);
            $(".searchSlide").removeClass("moveLeft");

        },
        error: function (msg) {

        }
    });
};

function AddToOrder(id) {
    var jsonData = JSON.stringify({ orderId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetOrderDetailById",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#OpenOrderDiv").empty();
            $("#OpenOrderDiv").html(data);
            $(".emptyddl").val("0");
            $(".emptytxt").val("");
            InitializeDateTimePicker();
            EditFavorite(id);
            $(".AddOrder").attr("onclick", "return IsValidOrder('0');");
            //BindCategoriesInSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
            BindOrderTypeCategoriesinSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID", "0");

            if ($("#hdOrderCodeId").val() != "") {
                $("#txtOrderCode").val($("#hdOrderCodeId").val());
            }
            $("#ddlFrequencyList option:contains(" + $("#hdFrequencyCode").val() + ")").attr("selected", "selected");
            BindOrdersRelatedDropdowns(2);
            $("#txtOrderCode").val($("#ddlOrderCodes :selected").text());
            $("#collapseOpenOrderAddEdit").addClass("in");
            $("#collapseOpenOrderAddEdit").removeAttr("style");
            $("#ddlOrderStatus option[value='4']").remove();
            $("#ddlOrderStatus option[value='2']").remove();
            $("#ddlOrderStatus option[value='3']").remove();
            $("#ddlOrderStatus option[value='9']").remove();
            setTimeout(function () { $("#txtOrderCode").val($("#ddlOrderCodes :selected").text()); }, 1000);
            $("#ddlOrderStatus").val("1");
            $("#ddlQuantityList").val("1.00");
            var currentdate = new Date();
            var datewithFormat = currentdate.format("mm/dd/yyyy");
            $("#txtOrderStartDate").val(datewithFormat);
            $("#txtOrderEndDate").val(datewithFormat);
            $(".searchSlide").removeClass("moveLeft");
        },
        error: function (msg) {

        }
    });
}

function BindPatientSummaryNotes() {
    var patientId = $("#hdPatientId").val();
    var jsonData = JSON.stringify({ patientId: patientId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindSummaryNotes",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#colCurrentSummaryNotesMain").empty();
            $("#colCurrentSummaryNotesMain").html(data);
        },
        error: function (msg) {

        }
    });
}



function CheckForIsFav() {
    var orderCode = $("#ddlOrderCodes").val();
    if (orderCode != "0") {
        var jsonData = JSON.stringify({ codeid: orderCode });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "CheckCodeForFav",
            dataType: "json",
            async: true,
            data: jsonData,
            success: function (data) {
                if (data == "-1") {
                    $("#chkMarkAsFavorite").prop("checked", false);
                    $("#hdFavoriteId").val();
                    $("#txtFavoriteDescription").val();
                    $("#favoriteOrderDescDiv").hide();
                } else {
                    $("#chkMarkAsFavorite").prop("checked", true);
                    $("#hdFavoriteId").val(data.UserDefinedDescriptionID);
                    $("#txtFavoriteDescription").val(data.UserDefineDescription);
                    $("#favoriteOrderDescDiv").show();
                }

                //Set the Text to Orderding Code Smart TextBox.
                if ($("#txtOrderCode").val() == "" ||
                    $("#txtOrderCode").val().indexOf($("#ddlOrderCodes option:Selected").text() > 0)) {
                    $("#txtOrderCode").val($("#ddlOrderCodes option:Selected").text());
                }
                if ($("#ddlOrderTypeCategory").val() == "11080") {
                    GetSpecimanString();
                }
            },
            error: function (msg) {

            }
        });
    }
}

function BindVitalsdata() {
    $("#hfTabValue").val("5");
    $("#aEvaluation").hide();
    var pid = $("#hdPatientId").val();
    if (pid != "" && pid > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "BindMedicalVitalsTabData",
            data: JSON.stringify({ patientId: pid }),
            dataType: "html",
            beforeSend: function () { },
            success: function (data) {
                if (data != null) {
                    $(".ehrtabs").empty();
                    $("#VitalsTab").empty();
                    $("#VitalsTab").html(data);
                    JsVitalCalls();
                    //SetGridPaging("?", "?patientId=" + pid + "&");
                }
            },
            error: function (msg) {

            }
        });
    }
}

function LoadAllergiesHsitoryDiv() {
    $("#hfTabValue").val("14");
    $("#aEvaluation").hide();
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: pid,
        encounterId: eid
    });
    if (pid != "" && pid > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "BindMedicalHistoryAllergyData",
            data: jsonData,
            dataType: "html",
            success: function (data) {
                if (data != null) {
                    $(".ehrtabs").empty();
                    $("#AllergyHistoryDiv").empty();
                    $("#AllergyHistoryDiv").html(data);
                    $("#MedicalHistoryFormDiv").validationEngine();
                    $("#AlergyFormDiv").validationEngine();
                    GetMedicalRecordByPatientIDEncounterID();
                }
            },
            error: function (msg) {

            }
        });
    }
}

function LoadLabTestData() {
    $("#hfTabValue").val("6");
    $("#aEvaluation").hide();
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: pid,
        encounterId: eid
    });
    if (pid != "" && pid > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "BindLabTestData",
            data: jsonData,
            dataType: "html",
            success: function (data) {
                if (data != null) {
                    $(".ehrtabs").empty();
                    $("#LabTestDiv").empty();
                    $("#LabTestDiv").html(data);
                    $(".AdministerOrderActivity").hide();
                    //$('.editOpenOrder').hide();
                    $(".LabOrderTest").show();
                    //RowColorByID('NurseClosedOrdersGrid');
                    $(".editLabActivity").show();
                    $(".EditOrderActivity").hide();
                    $(".AddOrderActivity").hide();
                    $(".editOpenOrderActivity").hide();
                    BindAddOrdersPaneldata("11080");
                }
            },
            error: function (msg) {

            }
        });
    }
}

function AdminLabTest(openOrderId) {
    var jsonData = JSON.stringify({ orderId: openOrderId });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetOpenOrderDetailByOrderId",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#NurseAdminAddEditFormDiv").empty();
            $("#NurseAdminAddEditFormDiv").html(data);
            BindGlobalCodesWithValue("#ddlActivityStatus", 3103, "#hdActivityStatus");
            $("#ddlActivityStatus").val(1);
            $("#OrderCode").text($("#hdOrderCode").val());
            // toggleCollapseDivs("#CollapseNurseAdminAddEdit");
            var value = $("#hdOrderTypeID").val();
            if (value != "") {
                var jsonD = JSON.stringify({ id: value });
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: summaryPageUrl + "GetCodeValueById",
                    dataType: "json",
                    async: true,
                    data: jsonD,
                    success: function (response) {
                        $("#OrderTypeName").text(response);
                    },
                    error: function (msg) {

                    }
                });
            }
            $(".AddOrderActivity1").hide();
            $(".AddOrderActivity").hide();
            $(".AddLabTest").show();
            $(".AddLabTest").find("input").addClass("validate[required]");
            $("#hdOpenOrderActivityScheduleID").val(openOrderId);
        },
        error: function (msg) {

        }
    });
}

function AddLabTestOrder() {
    var id = $("#hdOpenOrderActivityScheduleID").val();
    var isValid = jQuery("#NurseAdminAddEditFormDiv").validationEngine({ returnIsValid: true });
    if (isValid) {
        var labtestMinValue = $("#txtLabtestMinVal").val();
        var labtestMaxValue = $("#txtLabtestMaxVal").val();
        var labtestUOM = $("#ddlResultUOM").val();
        var jsonData = JSON.stringify({
            OrderActivityID: id,
            OrderType: $("#hdOrderTypeID").val(),
            OrderCode: $("#hdOrderCode").val(),
            OrderCategoryID: $("#hdOrderCategoryID").val(),
            OrderSubCategoryID: $("#hdOrderSubCategoryID").val(),
            OrderActivityStatus: $("#ddlActivityStatus").val(),
            PatientID: $("#hdPatientId").val(),
            EncounterID: $("#hdCurrentEncounterId").val(),
            OrderID: $("#hdOpenOrderID").val(),
            OrderBy: $("#hdOrderBy").val(),
            OrderActivityQuantity: $("#txtActivityQuantity").val(),
            OrderScheduleDate: $("#hdOrderScheduleDate").val(),
            PlannedBy: $("#hdPlannedBy").val(),
            PlannedDate: $("#hdPlannedDate").val(),
            PlannedFor: $("#hdPlannedFor").val(),
            ExecutedQuantity: $("#ddlExecutedQuantity").val(), //$('#txtExecutedQuantity').val(),
            CreatedDate: $("#hdCreatedDate").val(),
            CreatedBy: $("#hdCreatedBy").val(),
            Comments: $("#txtComments").val(),
            IsActive: true,
            ResultValueMin: labtestMinValue,
            ResultValueMax: labtestMaxValue,
            ResultUOM: labtestUOM
        });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "SaveOpenOrderActivitySchedule",
            data: jsonData,
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                var msg = "Records Saved successfully !";
                if (id > 0) {
                    msg = "Records updated successfully";
                }
                ShowMessage(msg, "Success", "success", true);
                ClearLabActivityForm();
                toggleCollapseDivs("#NurseAdminOpenOrdersListDiv");
            },
            error: function (msg) {
            }
        });
    }
    //var id = $("#hdOpenOrderActivityScheduleID").val();
    //var isValid = jQuery("#NurseAdminAddEditFormDiv").validationEngine({ returnIsValid: true });
    //if (isValid) {
    //    var txtPatientId = $("#hdPatientId").val();
    //    var txtEncounterId = $('#hdCurrentEncounterId').val();
    //    var txtMedicalRecordNumber = $("#hdPatientMRN").val();
    //    var jsonData = JSON.stringify({
    //        'MedicalVitalID': 0,
    //        'MedicalVitalType': 4,
    //        'PatientID': txtPatientId,
    //        'EncounterID': txtEncounterId,
    //        'MedicalRecordNumber': txtMedicalRecordNumber,
    //        'GlobalCodeCategoryID': $('#hdOrderCode').val(),
    //        'GlobalCode': $('#hdOrderSubCategoryID').val(), // From Enum//Blood Pressure Id 
    //        'AnswerValueMin': $("#txtLabtestMinVal").val(),
    //        'AnswerValueMax': $("#txtLabtestMaxVal").val(),
    //        'AnswerUOM': '',
    //        'txtComments': $("#txtComments").val()
    //    });
    //    $.ajax({
    //        type: "POST",
    //        url: '/MedicalVital/SaveMedicalVital',
    //        async: false,
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        data: jsonData,
    //        success: function (data) {
    //            var msg = "Records Saved successfully !";
    //            if (id > 0)
    //                msg = "Records updated successfully";
    //            BindLabOrdersTest();
    //            UpdateLabOrderStatus(id);
    //            ShowMessage(msg, "Success", "success", true);
    //        },
    //        error: function (msg) {

    //        }
    //    });
    //}
}

function BindLabOrdersTest() {
    var txtEncounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        PatientID: $("#hdPatientId").val(),
        Encounterid: txtEncounterId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/MedicalVital/BindLabTestList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#LabTestListDiv").empty();
            $("#LabTestListDiv").html(data);
        },
        error: function (msg) {
        }
    });
}

function UpdateLabOrderStatus(id) {
    var orderId = id;
    var jsonData = JSON.stringify({
        Id: orderId,
        orserstatus: $("#ddlActivityStatus").val(),
        comments: $("#txtComments").val()
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "UpdateLabtestOrder",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            LoadLabTestData();
        },
        error: function (msg) {
        }
    });
}

function DeleteLabTest() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            MedicalVitalID: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: "/MedicalVital/DeleteMedicalVital",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    ShowMessage("Records Deleted Successfully", "Success", "success", true);
                    LoadLabTestData();
                }
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function RowColorByID(id) {
    $("#" + id + " tbody tr")
        .each(function (i, row) {
            var $actualRow = $(row);
            if ($actualRow.find(".col10").html().indexOf("Open") != -1) {
                $actualRow.removeClass("rowColor3");
                $actualRow.removeClass("rowColor1");
                $actualRow.removeClass("rowColor2");
            } else if ($actualRow.find(".col10").html().indexOf("Open and Administered") != -1) {
                $actualRow.addClass("rowColor1");
            } else if ($actualRow.find(".col10").html().indexOf("Closed") != -1) {
                $actualRow.addClass("rowColor3");
            } else if ($actualRow.find(".col10").html().indexOf("On Bill") != -1) {
                $actualRow.addClass("rowColor2");
            } else if ($actualRow.find(".col10").html().indexOf("Cancel") != -1) {
                $actualRow.addClass("rowColor3");
            }
        });
}

//Set the Selected Value to Ordering Codes Dropdown
function SetValueToOrderingCodesDropdown(selectedValue, item) {
    var ddlSubCategory = $("#ddlOrderTypeSubCategory");
    var ddlSelector = $("#ddlOrderCodes");
    if (ddlSelector.length == 0 || ddlSubCategory.val() == "" || ddlSubCategory.val() == null) {
        $(ddlSelector).empty();
        $(ddlSelector).html(item);
    } else {
        $(ddlSelector).html(item);
    }
    $(ddlSelector).val(selectedValue);
}

function SelectOrderingCode(e) {
    //;
    var dataItem = this.dataItem(e.item.index());
    $("#txtOrderCode").val(dataItem.CodeDescription);
    $("#hdAutocompleteOrderCodeId").val(dataItem.CodeDescription);
    $("#hdOrderType").val(dataItem.CodeType);
    $("#hdOrderExternalCode").val(dataItem.ExternalCode);
    $("#hidOrderCodeValue").val(dataItem.Code);
    $("#CodeTypeValue").html(dataItem.CodeTypeName);
    var items = "<option value='" + dataItem.Code + "'>" + dataItem.CodeDescription + "</option>";
    SetValueToOrderingCodesDropdown(dataItem.Code, items);
    $("#hdOrderCodeId").val(dataItem.Code);
    BindAllDDLValues();
    $("#ddlOrderCodes").val(dataItem.Code);
    /*setTimeout(function () {
        
        //$("#ddlOrderCodes option:contains(" + $('#txtOrderCode').val() + ")").attr('selected', 'selected');
        $("#ddlOrderCodes").val(dataItem.Code);
        //$("#ddlOrderCodes").val($("#hidOrderCodeValue").val());
        //$("#ddlOrderCodes").val('selected', 'selected');
    }, 5000);*/
}



function RemoveActionFromGrid() {
    hideColumnColorRow(7, "DiagnosisGrid");
    //hideColumnColorRow(10, 'PatientOpenOrders');
    //hideColumnColorRow(6, 'PatientEncountersGrid');
    SetGridSorting(BindDingnosisBySort, "#DiagnosisGrid");
    //SetGridSorting(BindOrdersBySort, "#gridContentOpenOrder");
}

function BindDrugDDLValue() {
    if ($("#ddlOrderTypeCategory :selected").text() == "Pharmacy") {
        $(".DrugDDL").hide();
        //var orderCodesVal = $("#ddlOrderCodes").val();
        //if (orderCodesVal != '') {
        //var jsonData = JSON.stringify({
        //    drugcode: orderCodesVal,
        //});
        //    $.ajax({
        //        type: "POST",
        //        url: '/Home/GetDrugDetailsByDrugCode',
        //        async: false,
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        data: jsonData,
        //        success: function (data) {
        //            //;
        //            if (data != null) {
        //                $("#ddlDosageForm").empty();
        //                var itemsDosageForm = '<option value="0">--Select--</option>';
        //                $.each(data, function (i, item) {
        //                    itemsDosageForm += "<option value='" + item.Id + "'>" + item.DrugDosage + "</option>";
        //                });
        //                $("#ddlDosageForm").html(itemsDosageForm);

        //                if ($("#hdItemDosage") != null && $("#hdItemDosage").val() > 0)
        //                    $("#ddlDosageForm").val($("#hdItemDosage").val());

        //                $("#ddlDosageAmount").empty();
        //                var itemsAmount = '<option value="0">--Select--</option>';
        //                $.each(data, function (i, item) {
        //                    itemsAmount += "<option value='" + item.Id + "'>" + item.DrugStrength + "</option>";
        //                });
        //                $("#ddlDosageAmount").html(itemsAmount);

        //                if ($("#hdItemAmount") != null && $("#hdItemAmount").val() > 0)
        //                    $("#ddlDosageAmount").val($("#hdItemAmount").val());
        //            }
        //        },
        //        error: function (msg) {
        //        }
        //    });
        //}
    } else {
        $(".DrugDDL").hide();
    }
}

function BindAllDDLValues() {
    if ($("#ddlOrderTypeCategory").val() == "0" || $("#ddlOrderTypeCategory").val() == null) {
        var orderCodesVal = $("#ddlOrderCodes").val();
        var hdOrderType = $("#hdOrderType").val();
        if (orderCodesVal != "" || orderCodesVal != "0" || orderCodesVal != null) {
            var jsonData = JSON.stringify({
                code: orderCodesVal,
                Type: hdOrderType
            });
            $.ajax({
                type: "POST",
                url: "/Home/GetSelectedCodeParent",
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonData,
                success: function (data) {

                    if (data != null) {
                        $("#ddlOrderTypeCategory").val(data.GlobalCodeCategoryId);
                        $("#hdOrderTypeSubCategoryID").val(data.GlobalCodeId);
                        OnChangeCategory("#ddlOrderTypeCategory",
                            "#ddlOrderTypeSubCategory",
                            "#hdOrderTypeSubCategoryID");
                    }
                },
                error: function (msg) {
                }
            });
        }
    } else if ($("#hfTabValue").val() == "6" || $("#hfTabValue").val() == "7" || $("#hfTabValue").val() == "9") {
        var orderCodesVal1 = $("#ddlOrderCodes").val();
        var hdOrderType1 = $("#hdOrderType").val();
        if (orderCodesVal1 != "" || hdOrderType1 != "0" || hdOrderType1 != null) {
            var jsonData1 = JSON.stringify({
                code: orderCodesVal1,
                Type: hdOrderType1
            });
            $.ajax({
                type: "POST",
                url: "/Home/GetSelectedCodeParent",
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: jsonData1,
                success: function (data) {
                    //;
                    if (data != null) {
                        //$('#ddlOrderTypeCategory').val(data.GlobalCodeCategoryId);
                        $("#hdOrderTypeSubCategoryID").val(data.GlobalCodeId);
                        OnChangeCategory("#ddlOrderTypeCategory",
                            "#ddlOrderTypeSubCategory",
                            "#hdOrderTypeSubCategoryID");
                    }
                },
                error: function (msg) {
                }
            });
        }
    }
    CheckForIsFav();
}

function ShowHideOthersOnFrequencyCodeChange() {
    $("#divNumberOfTimesInaDay").hide();
    var frequencyval = $("#ddlFrequencyList").val();
    if (frequencyval == "11") {
        $(".OtherFrequnecy").show();
    } else {
        $(".OtherFrequnecy").hide();
    }

    //Get the value of 'Number of times' from Frequency
    if (frequencyval > 0) {
        $.ajax({
            type: "POST",
            url: summaryPageUrl + "GetFrequencyCodeDetail",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ frequencyCodeId: frequencyval }),
            success: function (data) {
                if (data != "") {
                    $("#divNumberOfTimesInaDay").show();
                    $("#lblNumberOfTimes").text(data);
                }
            },
            error: function (msg) {
            }
        });
    }
}



//---------------------For Sorting and Paging-----------------------


function BindPhyFavOrdersBySort(event) {
    var url = summaryPageUrl + "BindPhyFavOrdersBySort";
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#favOrdersGrid1").empty();
            $("#favOrdersGrid1").html(data);

            $("#favOrdersGrid").empty();
            $("#favOrdersGrid").html(data);

            SetGridSorting(BindPhyFavOrdersBySort, "#PhyFavOrdersGrid");
        },
        error: function (msg) {

        }

    });

    return false;
}

function BindPhyMostOrdersBySort(event) {
    var url = summaryPageUrl + "BindPhyMostOrdersBySort";
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#MostRecentOrdersGrid").empty();
            $("#MostRecentOrdersGrid").html(data);

            $("#MostRecentOrdersGrid1").empty();
            $("#MostRecentOrdersGrid1").html(data);

            //SetGridSorting(BindPhyMostOrdersBySort, "#phyAllOrdersGrid");
        },
        error: function (msg) {

        }

    });

    return false;
}

function EditRadImagingOrder(id) {
    var jsonData = JSON.stringify({ orderId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetOrderDetails",
        dataType: "json",
        async: true,
        data: jsonData,
        success: function (data) {
            if (data != "" && data != null) {
                $("#txtOrderType").val(data.OrderCode);
                $("#hdOrderId").val(data.OpenOrderID);
                $("#collapseFileUploaderAddEdit").addClass("in");
            }
        },
        error: function (msg) {

        }
    });
}

function BindPharmacyData() {
    $("#aEvaluation").hide();
    $("#hfTabValue").val("9");
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: pid,
        encounterId: eid
    });
    if (pid != "" && pid > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "BindPharmacyTabData",
            data: jsonData,
            dataType: "html",
            success: function (data) {
                if (data != null) {
                    $(".ehrtabs").empty();
                    $("#PharmacyOrdersDiv").empty();
                    $("#PharmacyOrdersDiv").html(data);
                    // $('.editOpenOrder').hide();
                    $(".AddOrderActivity1").hide();
                    $(".editOpenOrderActivity").hide();
                    $(".PharmacyActivity").show();
                    $(".editPharmacyActivity").show();
                    //setTimeout(function () {
                    //    BillActivtiesRowColor();
                    //    RowNurseClosedColor();
                    //}, 1000);
                    $("#hfTabType").val("1");
                    $(".AdministerOrderActivity").hide();
                    $(".PharmacyOrderActivity").show();
                    $(".editPharmacyActivity").show();
                    BindAddOrdersPaneldata("11100");
                }
            },
            error: function (msg) {

            }
        });
    }
}

var GetRadImagingOrderByActivity = function (id) {
    var jsonData = JSON.stringify({ orderId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetOrderDetailsByActivityId",
        dataType: "json",
        async: true,
        data: jsonData,
        success: function (data) {
            if (data != "" && data != null) {
                $("#txtOrderType").val(data.OrderCode);
                $("#hdOrderId").val(data.OpenOrderID);
                $("#panel-collapse").removeClass("in");
                $("#collapseFileUploaderAddEdit").addClass("in").removeAttr("style");
            }
        },
        error: function (msg) {

        }
    });
};

var GetOrdersTabData = function () {
    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetOrdersTabData",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            BindList("#NurseAdminOpenOrdersListDiv", data.openorderList);
            BindList("#colActivityListDiv", data.openorderActivityList);
            BindList("#ClosedActivitiesDiv", data.closedorderActivityHtml);
            BindList("#divEncounterClosedOrders", data.closedorderHtml);
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
};

var BindAddOrdersPaneldata = function (tabOrdertype) {
    BindOrdersRelatedDropdowns(2);
    //BindCategoriesInSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
    BindOrderTypeCategoriesinSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID", tabOrdertype);
    InitializeDateTimePicker();
    $("#ddlOrderStatus option[value='4']").remove();
    $("#ddlOrderStatus option[value='2']").remove();
    $("#ddlOrderStatus option[value='3']").remove();
    $("#ddlOrderStatus option[value='9']").remove();
    $("#OpenOrderDiv").validationEngine();
    $("#ddlQuantityList").val("1.00");
    $("#ddlExecutedQuantity").val("1.00");
    $("#ddlOrderStatus").val("1");
};

var BindGridsAfterOrder = function () {
    var tabvalue = $("#hfTabValue").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    var selectedVal = "0";
    var showActivity = "0";
    switch (tabvalue) {
        case "1":
        case "2":
        case "3":
        case "4":
        case "5":
            selectedVal = "0";
            showActivity = "0";
            break;
        case "10":
        case "11":
            selectedVal = "0";
            showActivity = "1";
            break;
        case "6":
            showActivity = "0";
            selectedVal = "11080"; // OrderCodeTypes.PathologyandLaboratory;
            break;
        case "7":
            showActivity = "0";
            selectedVal = "11070"; //OrderCodeTypes.Radiology;
            break;
        case "8":
            showActivity = "0";
            selectedVal = "11010"; //OrderCodeTypes.Surgery;
            break;
        case "9":
            showActivity = "0";
            selectedVal = "11100"; //OrderCodeTypes.Pharmacy;
            LoadMarFormList();
            break;
        default:
            showActivity = "0";
            selectedVal = "0";
    }
    BindOrdersGrid(selectedVal);

    /*
    WHY: AJ-Commented the below method due to the reason that it's already being used in the above method referenced i.e. BindOrdersGrid(selectedVal)
    ON: 15-April-2017
    */
    //BindSpecimanGrid();
};

var GetEncounterClosedOrdersByType = function (type) {
    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId,
        type: type
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindEncounterClosedOrdersByType",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            //$("#divEncounterClosedOrders").empty();
            //$("#divEncounterClosedOrders").html(data);
            BindList("#ClosedOrdersDiv", data);
            ShowHideActionButton();
        },
        error: function (msg) {
        }
    });
    return false;
};

var GetEncounterOpenOrdersByType = function (type) {
    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId,
        type: type
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindEncounterOpenOrdersByType",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            if ($("#colCurrentOrdersMain").length > 0)
                BindList("#colCurrentOrdersMain", data);

            if ($("#NurseAdminOpenOrdersListDiv").length > 0)
                BindList("#NurseAdminOpenOrdersListDiv", data);

            ShowHideActionButton();
            $(".AdministerOrderActivity").show();
            $("#collapseOpenOrderAddEdit").removeClass("in");
            $("#collapseOpenOrderlist").addClass("in");
        },
        error: function (msg) {
        }
    });
    return false;
};

var GetEncounterClosedOrderActivitesByType = function (type) {

    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId,
        type: type
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindEncounterClosedActivityList",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            BindList("#ClosedActivitiesDiv", data);
            ShowHideActionButton();
        },
        error: function (msg) {
        }
    });
    return false;
};

var GetEncounterOpenOrderActivitesByType = function (type) {

    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId,
        type: type
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindEncounterOpenActivityList",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            // Changes don by Shashank To fix the Double header which is appear in try order activity grid in Physician task.
            // Changes done on the 14 March 2016
            if (!$("#colActivityList").hasClass("in"))
                $("#colActivityList").addClass("in").attr("style", "height:auto;");
            BindList("#colActivityListDiv", data);
            //$('.openOrderActivity').addClass('in');
            ShowHideActionButton();
        },
        error: function (msg) {
        }
    });
    return false;
};

var CheckForMultipleActivites = function (orderid) {
    var jsonData = JSON.stringify({
        orderid: orderid,
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "CheckMulitpleOpenActivites",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            if (data == true) {
                $("#ddlOrderStatus option[value='2']").remove();
            } else {
                $("#ddlOrderStatus option[value='2']").remove();
                $("#ddlOrderStatus")
                    .append($("<option>",
                        {
                            value: 2,
                            text: "Administered"
                        }));
            }
            $("#ddlOrderStatus").val("1");
        },
        error: function (msg) {
        }
    });
    return false;
};

var ShowHideActionButton = function () {
    var currentdate = new Date();
    var datewithFormat = currentdate.format("mm/dd/yyyy");
    var tabvalue = $("#hfTabValue").val();
    var selectedVal = "0";
    $("#ddlQuantityList").val("1.00");
    switch (tabvalue) {
        case "1":
        case "2":
        case "3":
        case "4":
        case "5":
        case "10":
        case "11":
            selectedVal = "0";
            //$('#NurseAdminAddEditFormDiv').empty();
            $(".AdministerOrderActivity").show();
            $(".editOpenOrder").show();
            $("#txtOrderStartDate").val(datewithFormat);
            $("#txtOrderEndDate").val(datewithFormat);
            break;
        case "6":
            selectedVal = "11080"; // OrderCodeTypes.PathologyandLaboratory;
            //$('#NurseAdminAddEditFormDiv').empty();
            $(".editOpenOrder").show();
            $(".AddOrderActivity1").hide();
            $(".editOpenOrderActivity").hide();
            $(".PharmacyActivity").hide();
            $(".editPharmacyActivity").hide();
            $(".AdministerOrderActivity").hide();
            $(".editLabActivity").show();
            $("#txtOrderStartDate").val(datewithFormat);
            $("#txtOrderEndDate").val(datewithFormat);
            //$('.editRadImagingOrder').show();
            break;
        case "7":
            selectedVal = "11070"; //OrderCodeTypes.Radiology;
            $(".editOpenOrder").show();
            $(".editOpenOrderActivity").show();
            $("#txtOrderStartDate").val(datewithFormat);
            $("#txtOrderEndDate").val(datewithFormat);
            $(".editRadImagingOrder").show();
            break;
        case "8":
            selectedVal = "11010"; //OrderCodeTypes.Surgery;
            break;
        case "9":
            selectedVal = "11100"; //OrderCodeTypes.Pharmacy;
            $(".AddOrderActivity1").hide();
            $(".editOpenOrderActivity").hide();
            $(".editOpenOrder").show();
            $(".PharmacyActivity").show();
            $(".editPharmacyActivity").show();
            $(".AdministerOrderActivity").hide();
            break;
        default:
            //$('#NurseAdminAddEditFormDiv').empty();
            $(".AdministerOrderActivity").show();
            $(".editOpenOrder").show();
            $("#txtOrderStartDate").val(datewithFormat);
            $("#txtOrderEndDate").val(datewithFormat);
    }
};

var BindPatientSummaryData = function () {
    $("#aEvaluation").hide();
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: pid,
        encounterId: eid
    });
    if (pid != "" && pid > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "PatientSummaryTabData",
            data: jsonData,
            dataType: "html",
            success: function (data) {
                if (data != null) {
                    $(".ehrtabs").empty();
                    $("#patientSummaryTab").empty();
                    $("#patientSummaryTab").html(data);
                }
            },
            error: function (msg) {

            }
        });
    }
};

var GetSpecimanString = function () {
    var orderCode = $("#ddlOrderCodes").val();
    if (orderCode != "0") {
        var jsonData = JSON.stringify({ orderCode: orderCode });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "GetSpecimanString",
            dataType: "json",
            async: true,
            data: jsonData,
            success: function (data) {
                $("#TypeOfSpeciman").html(data);
            },
        });
    }
};
var AddLabTestSpecimanOrder = function () {
    var id = $("#hdOpenOrderActivityScheduleIDLabSpeciman").val();
    var isValid = jQuery("#LabSpecimanAddEditFormDiv").validationEngine({ returnIsValid: true });
    if (isValid) {
        var labtestMinValue = $("#txtLabtestMinValLabSpeciman").val();
        var labtestMaxValue = $("#ddlTypeOfSpecimanLabSpeciman").val();
        var labtestUOM = $("#ddlResultUOMLabSpeciman").val();
        var jsonData = JSON.stringify({
            OrderActivityID: id,
            OrderType: $("#hdOrderTypeIDLabSpeciman").val(),
            OrderCode: $("#hdOrderCodeLabSpeciman").val(),
            OrderCategoryID: $("#hdOrderCategoryIDLabSpeciman").val(),
            OrderSubCategoryID: $("#hdOrderSubCategoryIDLabSpeciman").val(),
            OrderActivityStatus: $("#ddlActivityStatusLabSpeciman").val(),
            PatientID: $("#hdPatientIdLabSpeciman").val(),
            EncounterID: $("#hdCurrentEncounterIdLabSpeciman").val(),
            OrderID: $("#hdOpenOrderIDLabSpeciman").val(),
            OrderBy: $("#hdOrderByLabSpeciman").val(),
            OrderActivityQuantity: $("#txtActivityQuantityLabSpeciman").val(),
            OrderScheduleDate: $("#hdOrderScheduleDateLabSpeciman").val(),
            PlannedBy: $("#hdPlannedByLabSpeciman").val(),
            PlannedDate: $("#hdPlannedDateLabSpeciman").val(),
            PlannedFor: $("#hdPlannedForLabSpeciman").val(),
            ExecutedQuantity: $("#ddlExecutedQuantityLabSpeciman").val(), //$('#txtExecutedQuantity').val(),
            CreatedDate: $("#hdCreatedDateLabSpeciman").val(),
            CreatedBy: $("#hdCreatedByLabSpeciman").val(),
            Comments: $("#txtCommentsLabSpeciman").val(),
            IsActive: true,
            ResultValueMin: labtestMinValue,
            ResultValueMax: labtestMaxValue,
            ResultUOM: labtestUOM
        });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "SaveLabSpecimanOrderActivity",
            data: jsonData,
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {
                var msg = "Records Saved successfully !";
                if (id > 0) {
                    msg = "Records updated successfully";
                }
                ShowMessage(msg, "Success", "success", true);
                ClearLabActivityForm();
                CancelLabSpecimanOrder();
                //toggleCollapseDivs("#NurseAdminOpenOrdersListDiv");
            },
            error: function (msg) {
            }
        });
    }
};

////----------Lab Orders List by Current Physician ID---------------------------
function BindLabOrdersListByPhysician() {
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    $.post(summaryPageUrl + "BindLabOrdersListByPhysician",
        { orderType: "11080", orderStatus: 1, encounterId: eid },
        function (data) {
            BindList("#divLabOrdersListByPhysician", data);
        });
}

////----------Lab Orders List by Current Physician ID---------------------------

function CheckTheDurgAllergy(id) {
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var ordercode = $("#ddlOrderCodes").val().trim();
    $.post(summaryPageUrl + "CheckDurgAllergy",
        { ordercode: ordercode, patientId: pid, encounterId: eid },
        function (data) {
            if (data != null && data.isAllergic != null) {
                // code to show popup
                if (data.isAllergic.PatientIsAllergicToDrug || data.isAllergic.PatientCurrentMedicationAllergy) {
                    $("#Allegydiv, #MedicationDiv").hide();
                    $("MedicationDiv .drugPopupText").removeClass("redText orangeText yellowText");
                    $("#spnDrugName").html($("#ddlOrderCodes :selected").text());
                    $("#hidOrderIdPopup").val(id);
                    $("#hidDrugAllergy #hidCurrentMedicationAllergy").val("0");
                    $("#hidCurrentMedicationReactionType #hidCurrentAllergyFromType #hidDrugAllergyName").val("");
                    if (data.isAllergic.PatientIsAllergicToDrug) {
                        $("#hidDrugAllergy").val("1");
                        $("#hidDrugAllergyName").val(data.isAllergic.DrugAllergyName);
                        $("#Allegydiv").show();
                    } else if (data.isAllergic.PatientCurrentMedicationAllergy) {
                        $("#MedicationDiv").show();
                        $("#hidCurrentMedicationAllergy").val("1");
                        $("#pWarningMessage").html(data.isAllergic.WarningText);
                        $("#hidCurrentMedicationReactionType").val(data.isAllergic.ReactionType);
                        $("#hidCurrentAllergyFromType").val(data.isAllergic.AllergyFromType);
                        switch (data.isAllergic.ReactionType) {
                            case "1":
                                $("#MedicationDiv .drugPopupText").addClass("redText");
                                break;
                            case "2":
                                $("#MedicationDiv .drugPopupText").addClass("orangeText");
                                break;
                            case "3":
                                $("#MedicationDiv .drugPopupText").addClass("yellowText");
                                break;
                            default:
                                $("MedicationDiv .drugPopupText").removeClass("redText orangeText yellowText");
                        }

                    }
                    $("#DrugAllergyPopUp").show();
                } else {
                    AddOrder(id);
                }
            } else {
                //Code to create Order
                AddOrder(id);
            }
        });
}

function AddAllergyLogging(status, allergyTypeval) {
    var orderCode = $("#ddlOrderCodes").val().trim();
    var orderCategory = $("#ddlOrderTypeCategory").val().trim();
    var orderSubCategory = $("#ddlOrderTypeSubCategory").val();
    var orderType = $("#hdOrderTypeId").val();
    var orderName = $("#hidCurrentAllergyFromType").val();
    var allergyType = allergyTypeval; // --- 1 for Patient Allergy, 2 for Drug Medication Allergy
    var reactionType = $("#hidCurrentMedicationReactionType").val();
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    var reactionOrderCode = $("#hidReactionCodeType").val();
    var allergyFromName = $("#hidDrugAllergyName").val();
    var isOrderCancel = !status;
    var txtOrderStartDate = ($("#txtOrderStartDate").val());
    var txtOrderEndDate = ($("#txtOrderEndDate").val());

    var jsonData = JSON.stringify({
        OrderCode: orderCode,
        OrderCategory: orderCategory,
        OrderSubCategory: orderSubCategory,
        OrderType: orderType,
        OrderName: orderName,
        AllergyType: allergyType,
        ReactionType: reactionType,
        PatientId: patientId,
        EncounterId: encounterId,
        ReactionOrderCode: reactionOrderCode,
        AllergyFromName: allergyFromName,
        IsOrderCancel: isOrderCancel,
        OrderStartDate: txtOrderStartDate,
        OrderEndDate: txtOrderEndDate
    });

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/DrugAllergyLog/SaveDrugAllergyLogCustom",
        data: jsonData,
        dataType: "json",
        success: function (data) {

        },
        error: function (msg) {

        }
    });
}

/*
    Added for the Super Power links
*/
function LoadPartialViewFromExternalLink(linkId) {
    linkId = parseInt(linkId);
    if (linkId > 0) {
        switch (linkId) {
            case 5: //Vitals
                $("#Vital").click();
                break;
            case 6: //Lab
                $("#linkLabTest").click();
                break;
            case 7: //Radiology
                $("#linkRadiology").click();
                break;
            case 9: //Pharmacy
                $("#linkPharmacyTab").click();
                break;
            case 10: //Physician Tasks
                $("#linkPhysicianTasksTab").click();
                break;
            case 11: //Nurse Tasks
                $("#NurseNote").click();
                break;
            case 12:
                $("#Diagnosis").click();
            default:
        }
    }
}


/*--------------Sort Sort Encounter Report Grid--------By krishna on 27082015---------*/


/*--------------Sort Sort Current Order Grid--------By krishna on 27082015---------*/
function SortCurrentOrderGrid(event) {

    var url = summaryPageUrl + "SortCurrentOrderGrid";
    var encounterId = $("#hdCurrentEncounterId").val();
    var patientId = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#colCurrentOrdersMain").empty();
            $("#colCurrentOrdersMain").html(data);

        },
        error: function (msg) {
        }
    });
}

function SortMedicalNotesInNurseTab(event) {


    var pid = $("#hdPatientId").val();
    var type = 1;
    var eid = $("#hdCurrentEncounterId").val();
    var url = summaryPageUrl + "SortMedicalNotesInNurseTab";

    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + pid + "&currentEncounterId=" + eid + "&" + event.data.msg;
    }


    if (pid != "" && pid > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: url,
            data: "null",
            dataType: "html",
            success: function (data) {
                if (data != null) {

                    $("#collapseMedicalNotesList").empty();
                    $("#collapseMedicalNotesList").html(data);
                    //BindGlobalCodesWithValue("#ddlNotesTypes", 963, '');
                }
            },
            error: function (msg) {

            }
        });
    }
}

function SortOrdersViewData(event) {
    /// <summary>
    ///     Loads the orders tab data.
    /// </summary>
    /// <returns></returns>
    var encounterId = $("#hdCurrentEncounterId").val();
    var url = summaryPageUrl + "SortOrdersViewData";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterId=" + encounterId + "&" + event.data.msg;
    }

    if (encounterId != "") {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: url,
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "html",
            async: true,
            success: function (data) {
                if (data != null) {
                    $(".ehrtabs").empty();
                    $("#OrderTab").empty();
                    $("#OrderTab").html(data);
                }
            },
            error: function (msg) {

            }
        });
    }
}

var SortEandMGrid = function (event) {

    var url = summaryPageUrl + "GetEmList";
    //var encounterId = $("#hdEncounterId").val();
    //var pId = $("#hdPid").val(;

    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        data: null,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {


            BindList("#EvManageGrid", data);

        },
        error: function (msg) {
        }
    });
    return false;
};

function SortNotesdata(event) {
    var tabvalue = $("#hfTabValue").val();
    var selectedVal = "0";
    switch (tabvalue) {
        case "7":
        case "10":
            selectedVal = "1";
            break;
        case "11":
            selectedVal = "2";
            break;
        default:
            selectedVal = "0";
    }
    var pid = $("#hdPatientId").val();
    var url = summaryPageUrl + "SortNoteType";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + pid + "&type=" + selectedVal + "&" + event.data.msg;
    }

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        data: null,
        dataType: "html",
        success: function (data) {
            if (data != null) {
                $("#phyMedicalNotesListDiv").empty();
                $("#phyMedicalNotesListDiv").html(data);
            }
        },
        error: function (msg) {

        }
    });
}



var bindPatientCarePlan = function () {
    $("#aEvaluation").hide();
    $("#hfTabValue").val("12");
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: pid,
        encounterId: eid
    });
    if (pid != "" && pid > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "PatientCarePlanMain",
            data: jsonData,
            dataType: "html",
            success: function (data) {
                $("#PatientCarePlanTab").empty().html(data);
                InitializeDateTimePicker();
                BindCarePlan();
                $("#carePlantaskDiv").hide();

            },
            error: function (msg) {

            }
        });
    }
};

function EditPatientCarePlan(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: summaryPageUrl + "GetPatientCarePlanData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            //$('#PatientCarePlanFormDiv').empty();
            //$('#PatientCarePlanFormDiv').html(data);
            //$('#collapsePatientCarePlanAddEdit').addClass('in');
            //$("#PatientCarePlanFormDiv").validationEngine();
            BindPatientCareData(data);
        },
        error: function (msg) {

        }
    });
}

function DeletePatientCarePlan() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: summaryPageUrl + "DeletePatientCarePlan",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindPatientCarePlanGrid();
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                } else {
                    return false;
                }
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function BindPatientCarePlanGrid() {
    var txtPatientId = $("#hdPatientId").val();
    var txtEncounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: txtPatientId,
        encounterId: txtEncounterId,
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindPatientCarePlanList",
        dataType: "html",
        async: true,
        data: jsonData,
        success: function (data) {
            $("#PatientCarePlanListDiv").empty();
            $("#PatientCarePlanListDiv").html(data);
        },
        error: function (msg) {

        }

    });
}

function ClearPatientCarePlanForm() {
    $("#careTaskDiv").html("");
    $("#selectedCareTaskDiv").html("");
    $(".dropdown_list").hide();
    $("#PatientCarePlanFormDiv").clearForm();
    $("#collapsePatientCarePlanList").addClass("in");
    $("#PatientCarePlanFormDiv").validationEngine();
    $("#btnSavePatientCarePlan").val("Save");
    $("#hdPatientCareId").val("");

}

function BindCarePlan() {
    $.ajax({
        type: "POST",
        url: "/CarePlanTask/BindCarePlanDropdown",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $("#ddlCarePlan").empty();

            var items = '<option value="0">--Select--</option>';

            $.each(data,
                function (i, care) {
                    items += "<option value='" + care.Value + "'>" + care.Text + "</option>";
                });

            $("#ddlCarePlan").html(items);


        },
        error: function (msg) {

        }
    });
}

function BindCarePlanInTask() {
    //$.ajax({
    //    type: "POST",
    //    url: "/CarePlanTask/BindCarePlanDropdown",
    //    async: false,
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    data: null,
    //    success: function (data) {
    //        $("#ddlCarePlanInTask").empty();

    //        var items = '<option value="0">--Select--</option>';

    //        $.each(data, function (i, care) {
    //            items += "<option value='" + care.Value + "'>" + care.Text + "</option>";
    //        });

    //        $("#ddlCarePlanInTask").html(items);


    //    },
    //    error: function (msg) {

    //    }
    //});
}

function BindCarePlanTask() {
    var carePlanId = $("#ddlCarePlan").val();
    if (carePlanId > 0 && carePlanId != null) {
        var rhsSelectList = [];
        $("#selectedCareTaskDiv input:checked")
            .each(function () {
                rhsSelectList.push($(this).attr("value"));
            });
        var jsonData = JSON.stringify({
            careId: carePlanId,
            code: rhsSelectList
        });
        $.ajax({
            type: "POST",
            url: "/PatientCarePlan/BindCarePlanTask",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                $(".dropdown_list").show();
                //$("#ddlCareTask").empty();

                //var items = '<option value="0">--Select--</option>';

                //$.each(data, function (i, care) {
                //    items += "<option value='" + care.TaskNumber + "'>" + care.TaskName + "</option>";
                //});

                //$("#ddlCareTask").html(items);
                var items = "";
                $.each(data,
                    function (i, care) {
                        items += '<li id="li3' +
                            i +
                            '"><input name="chkCareTask" onchange="CheckDuplicateRecords(this);"  id="careTask' +
                            i +
                            '" attr-index="' +
                            i +
                            '"  attr-id="' +
                            0 +
                            '" type="checkbox" value="' +
                            care.Value +
                            '" attr-text="' +
                            care.Text +
                            '"><p>' +
                            care.Text +
                            "</p></li>";
                    });
                $("#careTaskDiv").html(items);

            },
            error: function (msg) {

            }
        });
    } else {

    }

}

function BindPatientCareData(data) {
    $("#hdPatientCareId").val(data.Id);
    $("#ddlCarePlan").val(data.CarePlanId);
    BindCarePlanTask();
    $("#ddlCareTask").val(data.TaskId);
    $("#dtFromDate").val(data.fromdateStr);
    $("#dtTillDate").val(data.tillDateStr);
    $("#btnSavePatientCarePlan").val("Update");
}

function ShowHideNewPlanTask() {
    $("#carePlantaskDiv").show();
    BindPatientTaskDropdown();
}

function BindPatientTaskDropdown() {
    InitializeTaskDates();
    BindActivityTypeDDL();
    BindCorporateFacilityRoles();
    BindOccuranceTypeDDL();
    BindTimeIntervalTypeDDL();
    BindOverDueDays();
    GetMaxCarePlanNumber();
    BindCarePlanInTask();
    $("#chkIsActive").prop("checked", true);
    $("#ddlOccuranceType option[value='2']").remove();
}

function BindActivityTypeDDL() {
    BindGlobalCodesWithValue("#ddlActivityType", 1201, "");
}

function BindOccuranceTypeDDL() {
    BindGlobalCodesWithValueWithOrder("#ddlOccuranceType", 4906, "");
}

function BindTimeIntervalTypeDDL() {
    BindGlobalCodesWithValueWithOrder("#ddlTimeIntervalType", 4907, "");
}

function BindCorporateFacilityRoles() {
    $.ajax({
        type: "POST",
        url: "/CarePlanTask/BindUsersType",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $("#ddlRoles").empty();

            var items = '<option value="0">--Select--</option>';

            $.each(data,
                function (i, role) {
                    items += "<option value='" + role.Value + "'>" + role.Text + "</option>";
                });

            $("#ddlRoles").html(items);

            //if ($("#hdRoleID") != null && $("#hdRoleID") != '')
            //    $("#ddlRoles").val($("#hdRoleID").val());
        },
        error: function (msg) {

        }
    });
}

function BindRecurranceTimeInterval() {
    var i = 0;
    var items = '<option value="0">--Select--</option>';
    var intervalType = $("#ddlTimeIntervalType").val();
    if (intervalType > 0) {
        if (intervalType == 1) {
            for (i = 1; i <= 60; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);

        } else if (intervalType == 2) {
            for (i = 1; i <= 24; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);
        } else if (intervalType == 3) {
            for (i = 1; i <= 31; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);
        } else if (intervalType == 4) {
            for (i = 1; i <= 4; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);
        } else if (intervalType == 5) {
            for (i = 1; i <= 12; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);
        } else {
            for (i = 1; i <= 10; i++) {
                items += "<option value='" + i + "'>" + i + "</option>";
            }
            $("#ddlTimeInterval").html(items);
        }
    } else {
        BindDropDownOnlyWithSelect("#ddlPlanLength");
    }
}

function GetMaxCarePlanNumber() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/CarePlanTask/GetMaxTaskNumber",
        dataType: "Json",
        async: true,
        data: null,
        success: function (data) {
            $("#txtTaskNumber").val(data);
        },
        error: function (msg) {

        }

    });

}

function BindOverDueDays() {
    var i = 0;
    var items = '<option value="0">--Select--</option>';

    for (i = 1; i <= 120; i++) {
        items += "<option value='" + i + "'>" + i + "</option>";
    }
    $("#ddlDuedays").html(items);
}

function SaveCarePlanTask(id) {
    var isValid = jQuery("#CarePlanTaskFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var value = TimeValidation();
        if (!value)
            return false;


        var txtStartDate = $("#dtStartDate").val();
        var txtEndDate = $("#dtEndDate").val();
        var txtPatientId = $("#hdPatientId").val();
        var txtEncounterId = $("#hdCurrentEncounterId").val();
        var txtId = $("#hidCarePlantaskId").val();
        var txtTaskNumber = $("#txtTaskNumber").val();
        var txtTaskDescription = $("#txtTaskDescription").val();
        var txtCarePlanId = "9999"; //$("#ddlCarePlanInTask").val();
        var txtActivityType = $("#ddlActivityType").val();
        var txtResponsibleUserType = $("#ddlRoles").val();
        var txtStartTime = $("#txtStartTime").val();
        var txtEndTime = $("#txtEndTime").val();
        var txtIsRecurring = $("#chkRecurring").is(":checked");
        var txtOccuranceType = $("#ddlOccuranceType").val();
        var txtRecTimeInterval = $("#ddlTimeInterval").val();
        var txtRecTImeIntervalType = $("#ddlTimeIntervalType").val();
        var txtIsActive = $("#chkIsActive").is(":checked");
        var txtTaskName = $("#txtTaskName").val();
        var ddlDuedays = $("#ddlDuedays").val();
        //var ddlCareplanId = $("#ddlCarePlan").val();
        var jsonData = JSON.stringify({
            Id: txtId,
            TaskNumber: txtTaskNumber,
            TaskDescription: txtTaskDescription,
            //CarePlanId: txtCarePlanId,
            ActivityType: txtActivityType,
            ResponsibleUserType: txtResponsibleUserType,
            StartTime: txtStartTime,
            EndTime: txtEndTime,
            IsRecurring: txtIsRecurring,
            RecurranceType: txtOccuranceType,
            RecTimeInterval: txtRecTimeInterval,
            RecTImeIntervalType: txtRecTImeIntervalType,
            IsActive: txtIsActive,
            TaskName: txtTaskName,
            ExtValue1: ddlDuedays,
            PatientId: txtPatientId,
            EncounterId: txtEncounterId,
            StartDate: txtStartDate,
            EndDate: txtEndDate,
            CarePlanId: txtCarePlanId


        });
        $.ajax({
            type: "POST",
            url: summaryPageUrl + "SaveCarePlanTask",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                ClearFormInCarePlanTask();
                BindPatientCarePlanGrid();
                BindTextBoxValidation();
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

function DeleteCarePlanTask() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: "/CarePlanTask/DeleteCarePlanTask",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                } else {
                    return false;
                }
            },
            error: function (msg) {
                return true;
            }
        });
    }
}

function ClearFormInCarePlanTask() {
    InitializeTaskDates();
    $("#carePlantaskDiv").hide();
    $("#CarePlanTaskFormDiv").clearForm();
    $("#collapseCarePlanTaskList").addClass("in");
    $("#chkIsActive").prop("checked", true);
    $("#CarePlanTaskFormDiv").validationEngine();
    $("#btnSaveCarePlantask").val("Save");
    $("#recurrance_event").hide();
    $("#showHideOccurrence").hide();
    $("#ddlTimeIntervalType").val("0").removeAttr("disabled");
    $("#ddlTimeInterval").val("0").removeAttr("disabled");
}

//function EnableDisableNewTaskButton() {
//    var ddlCarePlan = $("#ddlCarePlan").val();
//    if (ddlCarePlan > 0) {
//        $("#btnNewTask").prop('disabled', false);
//    } else {
//        $("#btnNewTask").prop('disabled', true);
//    }
//}

function CheckDuplicateRecords(e) {

    var length;
    var chkText = $(e).attr("attr-text");
    length = $('#selectedCareTaskDiv p:contains("' + chkText + '")').length;
    if (length == "0") {
        BindSelectedTaskList(this);
    } else {
        ShowMessage("Task Already Selected!", "Warning", "warning", true);
        $(e).prop("checked", false);
        return false;
    }
}

function BindSelectedTaskList(e) {
    var htmlTask = "";
    $("#careTaskDiv :CheckBox:checked")
        .each(function () {
            var oy = $(this).parent();
            var outerhrmlobj = oy[0].outerHTML;
            htmlTask += outerhrmlobj;
            $(this).parent().html("");
        });
    //htmlTask = htmlTask.replace('BindSelectedTaskList(this);', 'UnBindSelectedTaskList(this);');
    htmlTask = htmlTask.replace("CheckDuplicateRecords(this);", "UnBindSelectedTaskList(this);");

    $("#selectedCareTaskDiv").append(htmlTask);
    $("#selectedCareTaskDiv")
        .find("input[type=checkbox]")
        .each(function () {
            $(this).prop("checked", "checked");
        });

}

var UnBindSelectedTaskList = function (e1) {

    var htmlTaskList = "";
    var chkText = $(e1).parent().find("p").html();
    var length = $('#careTaskDiv p:contains("' + chkText + '")').length;
    if (length > 0) {
        $(e1).parent().html("");
    } else {
        $("#selectedCareTaskDiv :CheckBox")
            .each(function () {
                if (!$(this).prop("checked")) {
                    //var parentId = $(this).parent().attr('id');
                    var oy = $(this).parent();
                    var outerhrmlobj = oy[0].outerHTML;
                    htmlTaskList += outerhrmlobj;
                    $(this).parent().html("");
                    htmlTaskList = htmlTaskList
                        .replace("UnBindSelectedTaskList(this);", "CheckDuplicateRecords(this);");
                    //$('#careTaskDiv').find('#' + parentId).append(htmlTaskList);
                    $("#careTaskDiv").append(htmlTaskList);
                }
            });
    }
};

//var UnBindSelectedTask = function (e1) {
//    
//    var htmlTaskList = '';
//    $('#selectedCareTaskDiv :CheckBox').each(function () {
//        if (!$(this).prop('checked')) {
//            var parentId = $(this).parent().attr('id');
//            var oy = $(this).parent();
//            var outerhrmlobj = oy[0].outerHTML;
//            htmlTaskList += outerhrmlobj;
//            $(this).parent().html('');
//            htmlTaskList = htmlTaskList.replace('UnBindSelectedTask(this);', 'CheckDuplicateRecords(this);');
//            //$('#careTaskDiv').find('#' + parentId).append(htmlTaskList);
//            $('#careTaskDiv').append(htmlTaskList);
//        }
//    });
//}

function SavePatientCarePlan(id) {
    var isValid = jQuery("#PatientCarePlanFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtId = $("#hdPatientCareId").val();
        //var txtId = id;
        //var test = $("#hdPatientCarePlanId").val();
        var test = "";
        var txtPatientId = $("#hdPatientId").val();
        var txtCarePlanId = $("#ddlCarePlan").val();
        //var txtTaskId = $("#ddlCareTask").val();
        var txtEncounterId = $("#hdCurrentEncounterId").val();
        var dtFromDate = $("#dtFromDate").val();
        var dtTillDate = $("#dtTillDate").val();
        var jsonTaskData = [];
        var selected = [];

        $("#selectedCareTaskDiv input:checked")
            .each(function () {
                selected.push($(this)[0].value);
                test += $(this).attr("attr-id") + ",";
            });

        var patientCarePrimaryId = test.split(",");
        var carePrimaryIdList = patientCarePrimaryId != "" ? patientCarePrimaryId.slice(0, -1) : "";
        if (selected.length <= 0) {
            ShowMessage("Please Select Task First", "Warning", "warning", true);
            return false;
        }

        for (var i = 0; i < selected.length; i++) {
            jsonTaskData[i] = {
                Id: carePrimaryIdList[i],
                PatientId: txtPatientId,
                CarePlanId: txtCarePlanId,
                TaskId: selected[i],
                EncounterId: txtEncounterId,
                FromDate: dtFromDate,
                TillDate: dtTillDate
                //PatientCarePrimaryIdList: carePrimaryIdList
            };
        }

        var jsonData = JSON.stringify(jsonTaskData);
        $.ajax({
            type: "POST",
            url: summaryPageUrl + "SavePatientCarePlan",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                //if (data == "-1") {
                //    ShowMessage("You have been already assign this task", "Warning", "warning", true);
                //    return false;
                //}
                ClearPatientCarePlanForm();
                BindPatientCarePlanGrid();
                //$("#selectedCareTaskDiv"
                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";

                ShowMessage(msg, "Success", "success", true);
            },
            error: function (msg) {

            }
        });
        //for each
    }
}

function EditPatientCarePlanListData(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: summaryPageUrl + "GetPatientCarePlanData",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $("#careTaskDiv").empty();
            if (data.CarePlanId == "9999") {
                var item = "<option value='9999'>Single Task</option>";
                $("#ddlCarePlan").append(item);
                $("#ddlCarePlan").val(9999);
            } else {
                $("#ddlCarePlan").val(data.CarePlanId);
            }
            $("#dtFromDate").val(data.fromdateStr);
            $("#dtTillDate").val(data.tillDateStr);
            $("#hdPatientCareId").val(data.Id);
            $("#btnSavePatientCarePlan").val("Update");
            BindCarePlanTaskInCareOfEdit(data.CarePlanId);
            BindCarePlanTask();
            //BindPatientCareData(data);SavePatientCarePlan
            $("#selectedCareTaskDiv")
                .find("input[type=checkbox]")
                .each(function () {
                    $(this).prop("disabled", "disabled");
                    $(this).attr("title", "Unable to uncheck the task; delete the task to remove from list");
                });
        },
        error: function (msg) {

        }
    });
}

function BindCarePlanTaskInCareOfEdit(careId) {
    //var selectedId = [];
    var txtPatientId = $("#hdPatientId").val();
    var txtEncounterId = $("#hdCurrentEncounterId").val();
    var carePlanId = careId;
    if (carePlanId > 0 && carePlanId != null) {
        var jsonData = JSON.stringify({
            careId: carePlanId,
            patientId: txtPatientId,
            encounterId: txtEncounterId
        });
        $.ajax({
            type: "POST",
            url: summaryPageUrl + "BindCarePlanTaskData",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                $(".dropdown_list").show();
                //$("#selectedCareTaskDiv").empty();
                var items = "";
                $.each(data,
                    function (i, care) {
                        items += '<li id="li3' +
                            i +
                            '"><input name="chkCareTask" onchange="UnBindSelectedTaskList(this);"  id="careTask' +
                            i +
                            '" attr-index="' +
                            i +
                            '" attr-id="' +
                            care.Id +
                            '" type="checkbox" value="' +
                            care.Value +
                            '"><p>' +
                            care.Text +
                            "</p></li>";
                        //selectedId.push(care.Id);
                    });
                //$("#hdPatientCarePlanId").val(selectedId);
                $("#selectedCareTaskDiv").html(items);
                $("#selectedCareTaskDiv")
                    .find("input[type=checkbox]")
                    .each(function () {
                        $(this).prop("checked", "checked");
                    });
            },
            error: function (msg) {

            }
        });
    } else {

    }

}

var GetEncounterClosedOrderActivitesPCByType = function (type) {

    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId,
        type: type
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindEncounterClosedActivityPCList",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            BindList("#ClosedActivitiesDiv", data);
            ShowHideActionButton();
        },
        error: function (msg) {
        }
    });
    return false;
};

var GetEncounterOpenOrderActivitesPCByType = function (type) {

    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId,
        type: type
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindEncounterOpenActivityPCList",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            // Changes don by Shashank To fix the Double header which is appear in try order activity grid in Physician task.
            // Changes done on the 14 March 2016
            if (!$("#colActivityList").hasClass("in"))
                $("#colActivityList").addClass("in").attr("style", "height:auto;");
            BindList("#colActivityListDiv", data);
            ShowHideActionButton();
        },
        error: function (msg) {
        }
    });
    return false;
};

var BindTextBoxValidation = function () {
    $("#txtEndTime").val("");
    var minvalue = $("#txtStartTime").val();
    if (minvalue.indexOf(":") > 0) {
        var minvalueInterval = minvalue.split(":")[1];
        minvalue = minvalueInterval == "00"
            ? minvalue.split(":")[0] + ":30"
            : (parseInt(minvalue.split(":")[0], 10) + 1) + ":00";
    }
    $("#txtEndTime")
        .datetimepicker({
            datepicker: false,
            format: "H:i",
            step: 30,
            mask: true,
            minTime: minvalue
        });
};

function InitializeTaskDates() {
    $("#txtStartTime")
        .datetimepicker({
            datepicker: false,
            format: "H:i",
            step: 30,
            //allowTimes: [
            //    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00'
            //],
            mask: true,
            onChangeDateTime: BindTextBoxValidation,
            onShow: BindTextBoxValidation
        });

    $("#txtEndTime")
        .datetimepicker({
            datepicker: false,
            format: "H:i",
            step: 30,
            //allowTimes: [
            //    '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30', '16:00'
            //],
            mask: true,
        });
}

var AddOutstandingOrder = function (id) {
    var orderId = id;
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    var ddlOrderType = $("#hdOrderTypeId").val(); //$("#ddlOrderType").val();
    var orderCode = $("#ddlOrderCodes").val().trim();
    var hdPrimaryDiagnosisId = $("#hdCurrentDiagnosisID").val();
    var frequency = $("#ddlFrequencyList").val();
    var txtQuantity = $("#ddlQuantityList").val(); //BindGlobalCodesWithValue("#ddlQuantityList", 1011, "#hdQuantity");
    var txtOrderNotes = $("#txtOrderNotes").val();
    var ddlOrderStatus = $("#ddlOrderStatus").val();
    var ddlOrderTypeCategory = $("#ddlOrderTypeCategory").val().trim();
    var ddlOrderTypeSubCategory = $("#ddlOrderTypeSubCategory").val();

    var txtOrderStartDate = ($("#txtOrderStartDate").val());
    var txtOrderEndDate = ($("#txtOrderEndDate").val());
    var hdIsActivitySchecduled = $("#hdIsActivitySchecduled").val();
    var hdActivitySchecduledOn = $("#hdActivitySchecduledOn").val();

    var jsonData = JSON.stringify({
        FutureOpenOrderID: orderId,
        OrderType: ddlOrderType,
        OrderCode: orderCode,
        DiagnosisCode: hdPrimaryDiagnosisId,
        FrequencyCode: frequency,
        Quantity: txtQuantity,
        OrderNotes: txtOrderNotes,
        PeriodDays: 0,
        OrderStatus: ddlOrderStatus,
        EncounterID: encounterId,
        PatientID: patientId,
        IsActive: true,
        IsDeleted: false,
        CategoryId: ddlOrderTypeCategory,
        SubCategoryId: ddlOrderTypeSubCategory,
        StartDate: txtOrderStartDate,
        EndDate: txtOrderEndDate,
        IsActivitySchecduled: hdIsActivitySchecduled,
        ActivitySchecduledOn: hdActivitySchecduledOn
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "AddPhysicianFutureOrder",
        data: jsonData,
        dataType: "json",
        success: function (data) {
            var msg = "Records Saved successfully !";
            var favoriteId = $("#hdFavoriteId").val();
            var markFavorite = $("#chkMarkAsFavorite").prop("checked");
            if (favoriteId == "")
                favoriteId = 0;

            var txtFavDescription = $("#txtFavoriteDescription").val();
            if (markFavorite == true) {
                AddToFavorites(data, favoriteId, markFavorite, txtFavDescription);
                GetFavoritesOrders();
            }
            if (id > 0) {
                msg = "Records updated successfully";
            }
            ShowMessage(msg, "Success", "success", true);
            ClearPhysicianOrderAll(encounterId);
            BindFutureOrdersList();
            $("#chkMarkAsFavorite").attr("checked", false);
        },
        error: function (msg) {

        }
    });
};

var BindFutureOrdersList = function () {
    var patientId = $("#hdPatientId").val();
    var jsonData = JSON.stringify({
        patientId: patientId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetPatientFutureOrder",
        data: jsonData,
        dataType: "html",
        success: function (data) {
            $("#lstFutureEncounterOrder123").empty().html(data);
        },
        error: function (msg) {

        }
    });

};

function BindNurseAssessmentList() {
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: patientId,
        encounterId: encounterId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindNurseAssessmentList",
        data: jsonData,
        dataType: "html",
        success: function (data) {
            $("#phycollapseFormEnteredListDiv").html(data);
        },
        error: function (msg) {

        }
    });
}

var BindRecrrencetype = function () {
    var occurancetype = $("#ddlOccuranceType").val();
    switch (occurancetype) {
        case "1": //Every Day
            $("#ddlTimeIntervalType").val("3").attr("disabled", "disabled");
            BindRecurranceTimeInterval();
            $("#ddlTimeInterval").val("1").attr("disabled", "disabled");
            break;
        case "2": //Every Alternate Day
            $("#ddlTimeIntervalType").val("3").attr("disabled", "disabled");
            BindRecurranceTimeInterval();
            $("#ddlTimeInterval").val("2").attr("disabled", "disabled");
            break;
        case "3": //Every Week
            $("#ddlTimeIntervalType").val("4").attr("disabled", "disabled");
            BindRecurranceTimeInterval();
            $("#ddlTimeInterval").val("1").attr("disabled", "disabled");
            break;
        case "4": //Every Alternate Week
            $("#ddlTimeIntervalType").val("4").attr("disabled", "disabled");
            BindRecurranceTimeInterval();
            $("#ddlTimeInterval").val("2").attr("disabled", "disabled");
            break;
        case "5": //Every Month
            $("#ddlTimeIntervalType").val("5").attr("disabled", "disabled");
            BindRecurranceTimeInterval();
            $("#ddlTimeInterval").val("1").attr("disabled", "disabled");
            break;
        case "6": //Every Alternate Month
            $("#ddlTimeIntervalType").val("5").attr("disabled", "disabled");
            BindRecurranceTimeInterval();
            $("#ddlTimeInterval").val("2").attr("disabled", "disabled");
            break;
        default:
            $("#ddlTimeIntervalType").removeAttr("disabled");
            $("#ddlTimeInterval").removeAttr("disabled");
    }
};

//-----------------------Common Methods----------------------------------------------------
function BindOrdersRelatedDropdowns(tabId) {
    var returnObject = CommonAjaxCalls.GetWithoutParams(summaryPageUrl + "GetDataByCategories", true);
    if (returnObject) {
        BindDropdownData(returnObject.listFrequency, "#ddlFrequencyList", "#hdFrequencyCode");
        BindDropdownData(returnObject.listOrderStatus, "#ddlOrderStatus", "#hdFrequencyCode");
        BindDropdownData(returnObject.listQuantity, "#ddlQuantityList", "#hdQuantity");

        if ($("#ddlExecutedQuantity").length > 0)
            BindDropdownData(returnObject.listQuantity, "#ddlExecutedQuantity", "#hdExecutedQuantity");

        if (tabId == 1) {
            if ($("#ddlDocumentType").length > 0)
                BindDropdownData(returnObject.listDocumentType, "#ddlDocumentType", "#hfDocumentTypeId");

            if ($("#ddlNotesTypes").length > 0)
                BindDropdownData(returnObject.listNoteType, "#ddlNotesTypes", "");
        }
    }
}

//function BindRedImaginData(id) {
//    
//    var returnObject = CommonAjaxCalls.GetWithoutParams(summaryPageUrl + "GetRedImaginDataByCategories?categoryId="+id+" ", true);
//    if (returnObject) {
//        BindDropdownData(returnObject.listDocumentType, "#ddlDocumentType", "#hfDocumentTypeId");
//        BindDropdownData(returnObject.listNoteType, "#ddlNotesTypes", "");

//    }
//}

//-----------------------Common Methods----------------------------------------------------

var ViewCancelOrderPopup = function (orderId) {
    $("#hidCancelOrderId").val(orderId);
    $.blockUI({ message: $("#divCancelOrder"), css: { width: "357px" } });
};
var CancelOrder = function (orderId) {
    var jsonData = JSON.stringify({
        cancelOrderId: orderId
    });
    $.ajax({
        type: "POST",
        url: summaryPageUrl + "CancelOpenOrder",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            if (data) {
                BindGridsAfterOrder();
                //BindOrdersGrid();
                LoadOrdersTabData2();
                ShowMessage("Order Cancelled successfully.", "Success", "success", true);
            } else {
                ShowMessage("Error while Cancelling the Order.", "Success", "success", true);
            }
            $("#hidCancelOrderId").val("");
        },
        error: function (msg) {

        }
    });
};
var HideCancelOrderPopup = function (id) {
    $.unblockUI();
    $("#hidCancelOrderId").val("");
};
var ViewCancelOrderActivityPopup = function (orderactivityId) {
    $("#hidCancelOrderActivityId").val(orderactivityId);
    $.blockUI({ message: $("#divCancelOrderActivity"), css: { width: "357px" } });
};
var HideCancelOrderActivityPopup = function (orderactivityId) {
    $.unblockUI();
    $("#hidCancelOrderActivityId").val("");
};
var HideSpecimenPopup = function (orderactivityId) {
    $.unblockUI();
    $("#hidSpecimanOrderActivityId").val("");
};
var ViewTakeSpecimenPopup = function (id) {
    $("#hidSpecimanOrderActivityId").val(id);
    $.blockUI({ message: $("#divTakeSpeciman"), css: { width: "357px" } });
};
var ISOrderPhrmacyOrder = function (orderid) {
    var jsonData = JSON.stringify({
        orderId: orderid
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "PharamacyOrderActivityAdministered",
        data: jsonData,
        dataType: "json",
        success: function (data) {
            if (!data)
                EditOrder(orderid);
            else
                ShowMessage("Unable to Edit Pharmacy Orders once any of the Order Activities are administered!!",
                    "No Edit",
                    "warning",
                    true);
            //return data;
        },
        error: function (msg) {
            //return false;
        }
    });
};
var ViewDeleteNotesPopup = function (notesId) {
    $("#hidNotesId").val(notesId);
    $.blockUI({ message: $("#divDeleteNotes"), css: { width: "357px" } });
};

function SortDocumentListGrid(event) {
    var tabvalue = $("#hfTabValue").val();

    var selectedVal = "0";
    switch (tabvalue) {
        case "13":
            selectedVal = "3";
            break;
        case "7":
            selectedVal = "2"; //OrderCodeTypes.Radiology;
            break;
        case "15":
            selectedVal = "4"; //OrderCodeTypes.Surgery;
            break;
        default:
            selectedVal = "0";
    }
    var url = "/FileUploader/PatientDocumentsGrid";

    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?associatedType=" + selectedVal + "&pid=" + $("#hdPatientId").val() + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#divDocumentsGrid").empty();
            $("#divDocumentsGrid").html(data);
        },
        error: function (msg) {
        }
    });
}

function BindDischargeSummaryData() {
    //
    $("#aEvaluation").hide();
    $("#hfTabValue").val("2");
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Discharge/DischargePartialView",
        dataType: "json",
        async: false,
        data: JSON.stringify({ patientId: patientId, encounterId: encounterId }),
        success: function (data) {
            $(".ehrtabs").empty();
            BindList("#DischargeSummaryTab", data.partialView);
            BindSelectBox("ddlMedicalProblems", data.MedicalProblems);
            BindSelectBox("ddlPatientInstructions", data.PatientInstructions);
            BindSelectBox("ddlFollowupTypes", data.FollowupTypes);

            $(".hideSummary").hide();
            ClearHiddenFieldsInDischargeSummary();
            $("#txtFollowupDate")
                .datetimepicker({
                    format: "m/d/Y",
                    minDate: "1950/12/12",
                    maxDate: "2025/12/12",
                    timepicker: false,
                    closeOnDateSelect: true
                });

            setTimeout(function () {
                if ($('#colProceduresList').hasClass('in')) {
                    $('#OpenOrderDischarge').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                } else {
                    $('#colProceduresList').addClass('in');
                    $('#OpenOrderDischarge').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                    $('#colProceduresList').removeClass('in');
                }
                SetGridSorting(BindDischargeOrdersBySort, "#gridContentOpenOrderDischarge1");

                if ($('#colDischargeMedications').hasClass('in')) {
                    $('.gridscrollC').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                } else {
                    $('#colDischargeMedications').addClass('in');
                    $('.gridscrollC').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                    $('#colDischargeMedications').removeClass('in');
                }
                SetGridSorting(BindDischargeMedicationBySort, "#gridContentDischargeMedication");
            }, 500);
        },
        error: function (msg) {
        }
    });
}

/*  new way of calling and binding the EHR screens */
var LoadOrdersTabData = function () {
    /// <summary>
    ///     Loads the orders tab data.
    /// </summary>
    /// <returns></returns>
    var encounterId = $("#hdCurrentEncounterId").val();
    encounterId = encounterId == "" ? "0" : encounterId;
    if (encounterId != "") {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "OrdersViewData",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "json",
            async: true,
            success: function (data) {
                if (data != null) {
                    $(".ehrtabs").empty();
                    $("#OrderTab").empty();
                    $("#OrderTab").html(data.partialView);
                    ////SetGridSorting(BindOrdersBySort, "#gridContentOpenOrder");
                    ////SetGridSorting(BindPhyFavOrdersBySort, "#PhyFavOrdersGrid");
                    ////SetGridSorting(BindPhyMostOrdersBySort, "#phyAllOrdersGrid");
                    //SetGridSorting(PhysicianAllSearch, "#gridContentAllOrder");
                    ////SetGridPaging("?", "?encounterId=" + encounterId + "&");
                    //BindSelectBoxWithGC(data.listFrequency, "#ddlFrequencyList", "#hdFrequencyCode");
                    //BindSelectBoxWithGC(data.listOrderStatus, "#ddlOrderStatus", "#hdFrequencyCode");
                    //BindSelectBoxWithGC(data.listQuantity, "#ddlQuantityList", "#hdQuantity");
                    ////BindCategoriesInSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
                    BindOrderTypeCategoriesinSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID", "0");
                    //RowColor();
                    InitializeDateTimePicker();
                    BindSearchToggle();
                    //$("#ddlOrderStatus option[value='4']").remove();
                    //$("#ddlOrderStatus option[value='2']").remove();
                    //$("#ddlOrderStatus option[value='3']").remove();
                    //$("#ddlOrderStatus option[value='9']").remove();
                    //$("#ddlQuantityList").val("1.00");
                    //$("#ddlOrderStatus").val("1");
                    $("#OpenOrderDiv").validationEngine();
                    LoadOrdersTabData2();
                }
            },
            error: function (msg) {

            }
        });
    }
};

var LoadOrdersTabData2 = function () {
    var encounterId = $("#hdCurrentEncounterId").val();
    encounterId = encounterId == "" ? "0" : encounterId;
    if (encounterId != "") {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "GetOrdersViewData",
            data: JSON.stringify({ encounterId: encounterId }),
            dataType: "json",
            async: true,
            success: function (data) {
                if (data != null) {
                    SetGridSorting(PhysicianAllSearch, "#gridContentAllOrder");
                    BindSelectBoxWithGC(data.listFrequency, "#ddlFrequencyList", "#hdFrequencyCode");
                    BindSelectBoxWithGC(data.listOrderStatus, "#ddlOrderStatus", "#hdFrequencyCode");
                    BindSelectBoxWithGC(data.listQuantity, "#ddlQuantityList", "#hdQuantity");
                    $("#ddlOrderStatus option[value='4']").remove();
                    $("#ddlOrderStatus option[value='2']").remove();
                    $("#ddlOrderStatus option[value='3']").remove();
                    $("#ddlOrderStatus option[value='9']").remove();
                    $("#ddlQuantityList").val("1.00");
                    $("#ddlOrderStatus").val("1");

                    BindFavouriteTab(data.FavoriteOrders, "#favOrders");
                    BindFavouriteTab(data.FavoriteOrders, "#favOrdersSearch");
                    BindMostRecentTab(data.MostRecentOrders, "#MostRecentSearch");
                    BindSearchTab(data.SearchOrders, "#SearchOrders");
                    BindPhyPreviousOrders(data.AllPhysicianOrders, "#PhyPreviousOrders");
                    BindOpenOrders(data.OpenOrdersList, "#NurseAdminOpenOrders");
                    BindFutureOrders(data.FutureOpenOrdersList, "#FutureOpenOrders");
                    BindOrderActivities(data.OpenOrderActivityList, "#OrderActivity");

                    //Closed Order Activity
                    var cColumns = [];
                    BindDatatable(data.ClosedOrderActivityList, "#ClosedOrderActivity", cColumns);

                    //Closed Orders
                    var cColumns = [];
                    BindDatatablewithFixedHeader(data.ClosedOrdersList, "#ClosedOrders", cColumns, 300);

                 }
            },
            error: function (msg) {

            }
        });
    }
};
var BindOrdersonOrderAdministeredSuccess = function (data) {
    BindOrderActivities(data.OpenOrderActivityList, "#OrderActivity");
    BindOpenOrders(data.OpenOrdersList, "#NurseAdminOpenOrders");

    var cColumns = [];
    BindDatatablewithFixedHeader(data.ClosedOrdersList, "#ClosedOrders", cColumns, 300);

}
function EditOrder(id) {
    var jsonData = JSON.stringify({ orderId: id });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetOrderDetailJsonById",
        dataType: "json",
        async: true,
        data: jsonData,
        success: function (data) {
            //$('#OpenOrderDiv').empty();
            //$('#OpenOrderDiv').html(data);
            BindEditOrderObj(data);
            EditFavorite(id);
            //BindCategoriesInSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID");
            BindOrderTypeCategoriesinSummary("#ddlOrderTypeCategory", "#hdOrderTypeCategoryID", "0");

            if ($("#hdOrderCodeId").val() != "") {
                $("#txtOrderCode").val($("#hdOrderCodeId").val());
            }

            //$("#ddlFrequencyList option:contains(" + $("#hdFrequencyCode").val() + ")").attr("selected", "selected");

            $("#ddlOrderStatus option[value='3']").remove();
            $("#ddlOrderStatus option[value='4']").remove();
            $("#ddlOrderStatus option[value='9']").remove();
            $("#ddlOrderCodes").val(data.orderDetail.OrderCode);
            CheckForMultipleActivites($("#hfOpenOrderid").val());
            setTimeout(function () {
                $("#txtOrderCode").val($("#ddlOrderCodes :selected").text());
            },
                1000);
            $("#collapseOpenOrderAddEdit").addClass("in").attr("style", "height:auto;");
            $("html, body").animate({ scrollTop: $("#collapseOpenOrderAddEdit").offset().top }, "fast");
        },
        error: function (msg) {

        }
    });
}

var BindEditOrderObj = function (data) {
    $("#hfOpenOrderid").val(data.orderDetail.OpenOrderID);
    $("#hdOrderTypeCategoryID").val(data.orderDetail.CategoryId);
    $("#hdOrderTypeSubCategoryID").val(data.orderDetail.SubCategoryId);
    $("#hdIsActivitySchecduled").val(data.orderDetail.IsActivitySchecduled);
    $("#ddlOrderTypeCategory").val(data.orderDetail.CategoryId);
    $("#hdFrequencyCode").val(data.orderDetail.FrequencyCode);
    $("#ddlFrequencyList").val(data.orderDetail.FrequencyCode);
    $("#ddlQuantityList select.select option[value=" + data.orderDetail.Quantity + "]").prop("selected", true);
    //$("#ddlQuantityList option:contains(" + data.orderDetail.Quantity + ")").attr('selected', 'selected');
    //$('#ddlQuantityList').val(data.orderDetail.Quantity);
    $("#ddlOrderStatus").val(data.orderDetail.OrderStatus);
    $("#txtOrderStartDate").val(data.orderStartDate);
    $("#txtOrderEndDate").val(data.orderEndDate);
    $("#hdOrderCodeId").val(data.orderDetail.OrderCode);
    $("#txtOrderNotes").val(data.orderDetail.OrderNotes);
    $("#btnAddOrder")
        .removeAttr("onclick")
        .attr("onclick", 'return IsValidOrder("' + data.orderDetail.OpenOrderID + '")');
};

function AddOrder(id) {
    var orderId = id;
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    var ddlOrderType = $("#hdOrderTypeId").val(); //$("#ddlOrderType").val();
    var orderCode = $("#ddlOrderCodes").val().trim();
    var hdPrimaryDiagnosisId = $("#hdCurrentDiagnosisID").val();
    var frequency = $("#ddlFrequencyList").val();
    var txtQuantity = $("#ddlQuantityList").val(); //BindGlobalCodesWithValue("#ddlQuantityList", 1011, "#hdQuantity");
    var txtOrderNotes = $("#txtOrderNotes").val();
    var ddlOrderStatus = $("#ddlOrderStatus").val();
    var ddlOrderTypeCategory = $("#ddlOrderTypeCategory").val().trim();
    var ddlOrderTypeSubCategory = $("#ddlOrderTypeSubCategory").val();

    //var txtOrderStartDate = new Date($("#txtOrderStartDate").val());
    //var txtOrderEndDate = new Date($("#txtOrderEndDate ").val());
    var txtOrderStartDate = ($("#txtOrderStartDate").val());
    var txtOrderEndDate = ($("#txtOrderEndDate").val());
    //var txtOrderStartDate =  new Date($("#txtOrderStartDate").val()).format('mm/dd/yyyy');
    //var txtOrderEndDate = new Date($("#txtOrderEndDate ").val()).format('mm/dd/yyyy');

    var hdIsActivitySchecduled = $("#hdIsActivitySchecduled").val();
    var hdActivitySchecduledOn = $("#hdActivitySchecduledOn").val();
    // var ddlDosageForm = '';
    //var ddlDosageAmount = '';
    if ($("#ddlOrderTypeCategory :selected").text() == "Pharmacy") {
        //ddlDosageForm = $("#ddlDosageForm").val();
        //ddlDosageAmount = $("#ddlDosageAmount").val();
    }
    var tabId = getcategoryByValue();
    var jsonData = JSON.stringify({
        OpenOrderID: orderId,
        OrderType: ddlOrderType,
        OrderCode: orderCode,
        DiagnosisCode: hdPrimaryDiagnosisId,
        FrequencyCode: frequency,
        Quantity: txtQuantity,
        OrderNotes: txtOrderNotes,
        PeriodDays: 0,
        OrderStatus: ddlOrderStatus,
        EncounterID: encounterId,
        PatientID: patientId,
        IsActive: true,
        IsDeleted: false,
        CategoryId: ddlOrderTypeCategory,
        SubCategoryId: ddlOrderTypeSubCategory,
        StartDate: txtOrderStartDate,
        EndDate: txtOrderEndDate,
        // ItemDosage: ddlDosageForm,
        //  ItemStrength: ddlDosageAmount,
        IsActivitySchecduled: hdIsActivitySchecduled,
        ActivitySchecduledOn: hdActivitySchecduledOn,
        TabId: tabId
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "AddPhysicianOrderinSummary",
        data: jsonData,
        dataType: "json",
        success: function (data) {
            var msg = "Records Saved successfully !";
            var favoriteId = $("#hdFavoriteId").val();
            var markFavorite = $("#chkMarkAsFavorite").prop("checked");
            if (favoriteId == "")
                favoriteId = 0;

            var txtFavDescription = $("#txtFavoriteDescription").val();
            if (markFavorite == true) {
                AddToFavorites(data.orderid, favoriteId, markFavorite, txtFavDescription);
                //GetFavoritesOrders();
            }
            if (id > 0) {
                msg = "Records updated successfully";
            }
            ClearPhysicianOrderAll(encounterId);
            $("#chkMarkAsFavorite").attr("checked", false);

            //$("#MostRecentOrdersGrid").empty();
            //$("#MostRecentOrdersGrid").html(data.MostRecentOrders);

            //$("#MostRecentOrdersGrid1").empty();
            //$("#MostRecentOrdersGrid1").html(data.MostRecentOrders);

            //$("#NurseAdminOpenOrdersListDiv").empty().html(data.OpenOrdersList);

            //$("#colActivityListDiv").empty().html(data.OpenOrderActivityList);

            //$("#ClosedActivitiesDiv").empty().html(data.ClosedOrderActivityList);

            //$("#ClosedOrdersDiv").empty().html(data.ClosedOrdersList);

            //$("#lstFutureEncounterOrder123").empty().html(data.futureOrdersList);

            //if ($("#MostRecentOrdersGrid").length > 0 && data.MostRecentOrders != null) {
            //    BindList("#MostRecentOrdersGrid", data.MostRecentOrders);
            //}

            //if ($("#MostRecentOrdersGrid1").length > 0 && data.MostRecentOrders != null)
            //    BindList("#MostRecentOrdersGrid1", data.MostRecentOrders);

            //if ($("#NurseAdminOpenOrdersListDiv").length > 0 && data.OpenOrdersList != null)
            //    BindList("#NurseAdminOpenOrdersListDiv", data.OpenOrdersList);

            //if ($("#colActivityListDiv").length > 0 && data.OpenOrderActivityList != null)
            //    BindList("#colActivityListDiv", data.OpenOrderActivityList);

            //if ($("#ClosedActivitiesDiv").length > 0 && data.ClosedOrderActivityList != null)
            //    BindList("#ClosedActivitiesDiv", data.ClosedOrderActivityList);

            //if ($("#ClosedOrdersDiv").length > 0 && data.ClosedOrdersList != null)
            //    BindList("#ClosedOrdersDiv", data.ClosedOrdersList);

            //if ($("#lstFutureEncounterOrder123").length > 0 && data.futureOrdersList != null)
            //    BindList("#lstFutureEncounterOrder123", data.futureOrdersList);

            //if ($("#LabSpecimanAddEditFormDiv").length > 0 && data.labWaitingSpecimenList != null)
            //    BindList("#LabSpecimanAddEditFormDiv", data.labWaitingSpecimenList);


            var tabIdValue = $("#hfTabValue").val();
            if (ddlOrderTypeCategory == 11100 && tabIdValue == "9")
                LoadMarFormList();

            //GetMostRecentOrders();
            //GetPhysicianAllOrders();
            //BindGridsAfterOrder();
            ShowHideActionButton();
            LoadOrdersTabData2();

            ShowMessage(msg, "Success", "success", true);
        },
        error: function (msg) {

        }
    });
}

var BindOrdersGrid = function (type) {
    var encounterId = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        encounterId: encounterId,
        type: type
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "BindOrdersGrid",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {
            BindList("#ClosedOrdersDiv", data.closedorderslist);
            BindList("#ClosedActivitiesDiv", data.closedorderActivityslist);
            BindList("#colActivityListDiv", data.openorderActivityslist);
            BindList("#NurseAdminOpenOrdersListDiv", data.openOrderslist);
            BindList("#LabSpecimanAddEditFormDiv", data.labWaitingSpecimenList);
            ShowHideActionButton();
        },
        error: function (msg) {
        }
    });
    return false;
};

function BindFutureOrdersBySort(event) {
    var url = summaryPageUrl + "GetPatientFutureOrder";
    var patientId = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#lstFutureEncounterOrder123").empty().html(data);
        },
        error: function (msg) {

        }

    });

    return false;
}

function SortLabSpecimanOpenOrderList(event) {
    var url = summaryPageUrl + "SortLabSpecimanOpenOrderList";
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: JSON.stringify({ encounterId: encounterId }),
        success: function (data) {
            BindList("#LabSpecimanAddEditFormDiv", data);
        },
        error: function (msg) {
        }
    });
}

function SortLabClosedOrderActivityList(event) {
    var url = summaryPageUrl + "SortLabClosedOrderActivityList";
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: JSON.stringify({ encounterId: encounterId }),
        success: function (data) {
            BindList("#ClosedActivitiesDiv", data);
        },
        error: function (msg) {
        }
    });
}

function SortLabClosedOrderList1(event) {
    var url = summaryPageUrl + "SortLabClosedOrderList";
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterid=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: JSON.stringify({ encounterId: encounterId }),
        success: function (data) {
            BindList("#ClosedOrdersDiv", data);
        },
        error: function (msg) {
        }
    });
}

function SortOperatingRoomData(event) {
    var url = "/OperatingRoom/SortOperatingRoomData";
    var encounterId = $("#hdCurrentEncounterId").val();
    var patientId = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#OperatingRoom_ListDiv").empty();
            $("#OperatingRoom_ListDiv").html(data);

        },
        error: function (msg) {
        }
    });
}

function SortAnastsiaRoomData(event) {
    var url = "/OperatingRoom/SortAnastsiaRoomData";
    var encounterId = $("#hdCurrentEncounterId").val();
    var patientId = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#AnesthesiaTime_ListDiv").empty();
            $("#AnesthesiaTime_ListDiv").html(data);

        },
        error: function (msg) {
        }
    });
}

function SortLabOrdersListByPhysician(event) {
    var url = summaryPageUrl + "SortLabOrdersListByPhysician";
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?orderType=11080&orderStatus=1&encounterid=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: JSON.stringify({ encounterId: encounterId }),
        success: function (data) {
            BindList("#divLabOrdersListByPhysician", data);
        },
        error: function (msg) {
        }
    });
}

function SortPreEvaluationList(event) {
    var url = summaryPageUrl + "SortPreEvaluationList";
    var encounterId = $("#hdCurrentEncounterId").val();
    var pid = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterId=" + encounterId + "&patientid=" + pid + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: null,
        success: function (data) {
            BindList("#PreEvaluationList", data);
        },
        error: function (msg) {
        }
    });
}

var SortPhysiciansAllorders = function (event) {
    var url = summaryPageUrl + "SortPhysiciansAllorders";
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $("#MostRecentOrdersGrid1").empty().html(data);
        },
        error: function (msg) {

        }
    });
    return false;
}

function PhysicianFav(event) {
    var url = summaryPageUrl + "PhysicianFav";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        data: null,
        dataType: "html",
        success: function (data) {
            $("#favOrdersGrid1").empty();
            $("#favOrdersGrid1").html(data);
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function PhysicianFavSearch(event) {
    var url = summaryPageUrl + "PhysicianFavSearch";
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        data: null,
        dataType: "html",
        success: function (data) {
            $("#favOrdersGrid").empty();
            $("#favOrdersGrid").html(data);
        },
        error: function (msg) {
            //Console.log(msg);
        }
    });
    return false;
}

function BindSortingMethodForGrid() {
    $('.phyAllOrdersGrid').fixedHeaderTable('destroy');
    if ($('#phyAllOrdersGrid tr').length > 0) {
        $('.phyAllOrdersGrid').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
    }
    SetGridSorting(PhysicianAllSearch, "#gridContentAllOrder");
}

function SortDiagnosisEHRTabGrid(event) {
    var url = summaryPageUrl + "SortDiagnosisTabGrid";
    var patientId = $("#hdPatientId").val();
    var encounterId = $("#hdCurrentEncounterId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?patientId=" + patientId + "&encounterId=" + encounterId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: JSON.stringify({ patientId: patientId, encounterId: encounterId }),
        success: function (data) {
            BindList("#CurrentDiagnosisGrid", data);
        },
        error: function (msg) {
        }
    });
}

function SortPhysicianList(event) {
    var url = summaryPageUrl + 'PhysicianFav';
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?=" + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#favOrdersGrid1").empty();
            $("#favOrdersGrid1").html(data);
        },
        error: function (msg) {
        }
    });
}


function TimeValidation() {
    var srtTime = $("#txtStartTime").val();
    var endTime = $("#txtEndTime").val();
    if (srtTime == "__:__" || srtTime == null) {
        ShowMessage("Please Enter Start Time", "Warning", "warning", true);
        return false;
    } else if (endTime == '__:__' || endTime == null) {
        ShowMessage(" Please Enter End Time", "Warning", "warning", true);
        return false;
    } else if (srtTime > endTime) {
        ShowMessage("End time should be grater to the start time", "Warning", "warning", true);
        return false;
    }
    else {
        return true;
    }

}


function SortMedicalHistoryList(event) {
    var url = summaryPageUrl + "SortMedicalHistoryList";
    var encounterId = $("#hdCurrentEncounterId").val();
    var pid = $("#hdPatientId").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != "")) {
        url += "?encounterId=" + encounterId + "&patientid=" + pid + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "html",
        async: false,
        data: null,
        success: function (data) {
            BindList("#MedicalHistoryListDiv", data);
        },
        error: function (msg) {
        }
    });
}








/**^^^^^^^^^^^^^^^^^^^^^^^^^^^^ Physician / Nurse Tab Data ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
function BindPhysicianTabData(type) {
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: pid,
        type: type,
        encounterId: eid
    });
    if (pid != "" && pid > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: summaryPageUrl + "PhysicianOrNurseTabData",
            data: jsonData,
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    /*^^^^^ Bind Tab data ^^^^^*/
                    if (data.pViewResult != null) {
                        $('#MedicalVitalListDiv').empty();
                        $("#NotesTab").empty();
                        $("#NotesTab").html(data.pViewResult);
                    }
                    /*^^^^^ Bind Tab data ^^^^^*/


                    /*^^^^^ Bind all Dropdown data in TAB^^^^^*/
                    if (data.dropdownlistdata != null) {
                        if ($("#ddlNotesTypes").length > 0)
                            FilterDropdownListData("#ddlNotesTypes", "963", "#hdMedicalNotesType", data.dropdownlistdata);


                        FilterDropdownListData("#ddlFrequencyList", "1024", "#hdFrequencyCode", data.dropdownlistdata);
                        FilterDropdownListData("#ddlOrderStatus", "3102", "#hdOrderStatus", data.dropdownlistdata);
                        FilterDropdownListData("#ddlQuantityList", "1011", "#hdQuantity", data.dropdownlistdata);

                        if ($("#ddlExecutedQuantity").length > 0)
                            FilterDropdownListData("#ddlExecutedQuantity", "1011", "#hdExecutedQuantity", data.dropdownlistdata);

                        if ($("#ddlDocumentType").length > 0)
                            FilterDropdownListData("#ddlDocumentType", "2305", "#hfDocumentTypeId", data.dropdownlistdata);


                        if ($("#ddlActivityStatus").length > 0)
                            FilterDropdownListData("#ddlActivityStatus", "3103", "#hdActivityStatus", data.dropdownlistdata);

                        if ($("#ddlOrderTypeCategory").length > 0)
                            BindOrderTypeCategoryData("#ddlOrderTypeCategory", data.orderTypeCategories, "#hdOrderTypeCategoryID")
                    }
                    /*^^^^^ Bind all Dropdown data in TAB^^^^^*/


                    /*^^^^^ Bind Order Type Dropdown data in TAB^^^^^*/
                    if (data.LabOrders != null) {
                        BindList("#divLabOrdersListByPhysician", data.LabOrders);
                    }
                    /*^^^^^ Bind Order Type Dropdown data in TAB^^^^^*/



                    if ($("#NurseAdministeredOrdersDiv").length > 0)
                        $("#NurseAdministeredOrdersDiv").validationEngine();
                    $("#ddlActivityStatus option[value='4']").remove();
                    $("#ddlOrderStatus option[value='4']").remove();
                    $("#ddlOrderStatus option[value='2']").remove();
                    $("#ddlOrderStatus option[value='3']").remove();
                    $("#ddlOrderStatus option[value='9']").remove();
                    if ($("#OpenOrderDiv").length > 0)
                        $("#OpenOrderDiv").validationEngine();
                    $("#ddlQuantityList").val('1.00');
                    $("#ddlExecutedQuantity").val('1.00');
                    $("#ddlOrderStatus").val('1');
                    RemoveActivitesStatus();
                    var tabvalue = $('#hfTabValue').val();
                    if (tabvalue == "11") { //....Nurse tab
                        $('#divAddEditOpenOrders').hide();
                        $('.vitals').show();
                        $(".editOpenOrder").hide();
                    } else {
                        $('.vitals').hide();
                        $(".editOpenOrder").show();
                    }
                    $("#ddlOrderStatus").val('1');
                    InitializeDateTimePicker();
                    $("#aEvaluation").show();
                    $(".AdministerOrderActivity").show();
                    $(".editOpenOrder").hide();
                    $('#hfTabType').val('2');
                    $(".NurseAdministeredOrdersDiv").show();
                    if (type == 1) {
                        $('#divnotesCaptionAddEdit').html('Physician Notes');
                        $('#divnotesCaptionListing').html('Physician Notes List');
                    } else {
                        $('#divnotesCaptionAddEdit').html('Nurse Notes');
                        $('#divnotesCaptionListing').html('Nurse Notes List');
                    }

                    if ($('#collapseVitalList').hasClass('in')) {
                        $('#PatientCurrentVitals').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                    } else {
                        $('#collapseVitalList').addClass('in');
                        $('#PatientCurrentVitals').fixedHeaderTable({ cloneHeadToFoot: true, altClass: 'odd', autoShow: true });
                        $('#collapseVitalList').removeClass('in');
                    }
                    SetGridSorting(SortMedicalVital, "#gridContentVital");
                }
            },
            error: function (msg) {
                console.log(error);
            }
        });
    }
}

function BindOrderTypeCategoryData(ddlSelector, data, hdSelector) {
    $(ddlSelector).empty();
    var items = '<option value="0">--Select--</option>';
    $.each(data,
        function (i, gcc) {
            items += "<option value='" +
                gcc.GlobalCodeCategoryValue.trim() +
                "'>" +
                gcc.GlobalCodeCategoryName +
                "</option>";
        });
    $(ddlSelector).html(items);

    var hdValue = $(hdSelector).val();
    if (hdValue != null && hdValue != "" && hdValue > 0) {
        $(ddlSelector).val(hdValue);
        OnChangeCategory(ddlSelector, "#ddlOrderTypeSubCategory", "#hdOrderTypeSubCategoryID");
    } else {
        var tabvalue = $("#hfTabValue").val();
        var selectedVal = "0";
        switch (tabvalue) {
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "10":
            case "11":
                selectedVal = "0";
                break;
            case "6":
                selectedVal = "11080"; // OrderCodeTypes.PathologyandLaboratory;
                break;
            case "7":
                selectedVal = "11070"; //OrderCodeTypes.Radiology;
                break;
            case "8":
                selectedVal = "11010"; //OrderCodeTypes.Surgery;
                break;
            case "9":
                selectedVal = "11100"; //OrderCodeTypes.Pharmacy;
                break;
            default:
                selectedVal = "0";
        }
        $(ddlSelector).val(selectedVal);
        //if (selectedVal != "0") {
        //    $(ddlSelector).attr("disabled", "disabled");
        //}
        OnChangeCategory(ddlSelector, "#ddlOrderTypeSubCategory", "#hdOrderTypeSubCategoryID");
    }
}
/**^^^^^^^^^^^^^^^^^^^^^^^^^^^^ Physician / Nurse Tab Data ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/



function OnChangeSubCatgory(ddlSelector) {
    //var value = $(ddlSelector).val();
    //var jsonData = JSON.stringify({
    //    subCategoryId: value
    //});
    $("#ddlDosageForm").removeClass("validate[required]");
    $("#ddlDosageAmount").removeClass("validate[required]");
    $(".DrugDDL").hide();


    var selector = $("#ddlOrderTypeSubCategory :selected");
    if (selector.length > 0 && selector.val() > 0) {
        var subCategoryValue = selector.val();
        var gccValue = selector.attr('gcc');
        var oct = selector.attr('oct') == "" ? "5" : selector.attr('oct');
        var startRange = parseInt(selector.attr('sr') == "" ? 0 : selector.attr('sr'));
        var endRange = parseInt(selector.attr('er') == "" ? 0 : selector.attr('er'));

        jsonData = {
            subCategoryValue: subCategoryValue,
            gcc: gccValue,
            orderCodeType: oct,
            startRange: startRange == null ? 0 : startRange,
            endRange: endRange == null ? 0 : endRange,
        };

        var orderCategory = $("#ddlOrderTypeCategory :selected").text();

        if (orderCategory == "LAB Test") {
            var items = '<option value="0">--Select--</option>';
            var newItem = "<option id='" +
                $("#ddlOrderTypeCategory").val() +
                "'  value='" +
                $("#ddlOrderTypeCategory").val() +
                "'>" +
                $("#ddlOrderTypeCategory :selected").text() +
                "</option>";
            items += newItem;
            $("#ddlOrderCodes").html(items);
            //Set Order Type Code Name
            $("#CodeTypeValue").text("LAB Test");
            //Set Order Type Code Id
            $("#hdOrderTypeId").val("11");
        }
        else {
            $.getJSON(summaryPageUrl + "GetCodesBySubCategory", jsonData, function (data) {
                if (data != null) {
                    //Set Order Type Code Name
                    $("#CodeTypeValue").text(data.codeTypeName);
                    //Set Order Type Code Id
                    $("#hdOrderTypeId").val(data.codeTypeId);

                    BindDropdownData(data.codeList, "#ddlOrderCodes", "#hdOrderCodeId");
                    $('#collapseOpenOrderAddEdit').addClass('in');

                    if ($("#ddlOrderCodes").val() != "0") {
                        $("#txtOrderCode").val($("#ddlOrderCodes :selected").text());
                    }
                }
            });
        }
    }



    //if (orderCategory == "Pharmacy") {
    //    $.ajax({
    //        type: "POST",
    //        contentType: "application/json; charset=utf-8",
    //        url: summaryPageUrl + "GetPharmacyOrderCodesBySubCategory",
    //        //data: JSON.stringify({ id: $("#hdOrderTypeSubCategoryID").val() }),
    //        data: jsonData,
    //        dataType: "json",
    //        async: false,
    //        success: function(data) {
    //            if (data != null) {
    //                //Set Order Type Code Name
    //                $("#CodeTypeValue").text(data.codeTypeName);
    //                //Set Order Type Code Id
    //                $("#hdOrderTypeId").val(data.codeTypeId);
    //                BindOrderCodesBySubCategoryID(data.codeList, "#ddlOrderCodes", "#hdOrderCodeId");
    //                $(".DrugDDL").hide();
    //            }
    //        },
    //        error: function(msg) {
    //        }
    //    });
    //} else if (orderCategory == "LAB Test") {
    //    var items = '<option value="0">--Select--</option>';
    //    var newItem = "<option id='" +
    //        $("#ddlOrderTypeCategory").val() +
    //        "'  value='" +
    //        $("#ddlOrderTypeCategory").val() +
    //        "'>" +
    //        $("#ddlOrderTypeCategory :selected").text() +
    //        "</option>";
    //    items += newItem;
    //    $("#ddlOrderCodes").html(items);
    //    //Set Order Type Code Name
    //    $("#CodeTypeValue").text("LAB Test");
    //    //Set Order Type Code Id
    //    $("#hdOrderTypeId").val("11");
    //} else {
    //    $.ajax({
    //        type: "POST",
    //        contentType: "application/json; charset=utf-8",
    //        url: summaryPageUrl + "GetOrderCodesBySubCategory",
    //        //data: JSON.stringify({ id: $("#hdOrderTypeSubCategoryID").val() }),
    //        data: jsonData,
    //        dataType: "json",
    //        success: function(data) {
    //            if (data != null) {
    //                //Set Order Type Code Name
    //                $("#CodeTypeValue").text(data.codeTypeName);
    //                //Set Order Type Code Id
    //                $("#hdOrderTypeId").val(data.codeTypeId);
    //                BindOrderCodesBySubCategoryID(data.codeList, "#ddlOrderCodes", "#hdOrderCodeId");
    //                setTimeout(function() {
    //                    if ($("#ddlOrderCodes").val() != "0") {
    //                        $("#txtOrderCode").val($("#ddlOrderCodes :selected").text());
    //                    }
    //                },
    //                    500);
    //            }
    //        },
    //        error: function(msg) {

    //        }
    //    });
    //}
}



function BindOrderCategoryAndSubCategoryDropdownData(data, ddlSelector, categoryId, subCategoryId) {
    $(ddlSelector).empty();
    var items = '<option value="0">--Select--</option>';
    $.each(data,
        function (i, gcc) {
            items += "<option value='" +
                gcc.GlobalCodeCategoryValue.trim() +
                "'>" +
                gcc.GlobalCodeCategoryName +
                "</option>";
        });
    $(ddlSelector).html(items);

    var hdValue = $(hdSelector).val();
    if (hdValue != null && hdValue != "" && hdValue > 0) {
        $(ddlSelector).val(hdValue);
        OnChangeCategory(ddlSelector, "#ddlOrderTypeSubCategory", "#hdOrderTypeSubCategoryID");
    } else {
        var tabvalue = $("#hfTabValue").val();
        var selectedVal = "0";
        switch (tabvalue) {
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "10":
            case "11":
                selectedVal = "0";
                break;
            case "6":
                selectedVal = "11080"; // OrderCodeTypes.PathologyandLaboratory;
                break;
            case "7":
                selectedVal = "11070"; //OrderCodeTypes.Radiology;
                break;
            case "8":
                selectedVal = "11010"; //OrderCodeTypes.Surgery;
                break;
            case "9":
                selectedVal = "11100"; //OrderCodeTypes.Pharmacy;
                break;
            default:
                selectedVal = categoryId != null && categoryId != "" ? categoryId : "0";
        }

        $("#hdOrderTypeSubCategoryID").val(subCategoryId);

        $(ddlSelector).val(selectedVal);
        //if (selectedVal != "0") {
        //    $(ddlSelector).attr("disabled", "disabled");
        //}
        OnChangeCategory(ddlSelector, "#ddlOrderTypeSubCategory", "#hdOrderTypeSubCategoryID");
    }
}



/************************** Diagnosis Tab Data ****************/


function ShowDiagnosisTabData() {
    $("#aEvaluation").hide();
    $("#hfTabValue").val("4");

    BindDiagnosisTabDataInSummary();
    $(".diagnosisActions").show();
}

function BindDiagnosisTabDataInSummary() {
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: pid,
        encounterId: eid
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetDiagnosisTabData",
        data: jsonData,
        dataType: "html",
        beforeSend: function () { },
        success: function (data) {
            if (data != null) {
                $(".ehrtabs").empty();
                BindList("#diagnosis", data);
                GetDiagnosisTabDataInSummary();
            }
        },
        error: function (msg) {

        }
    });
}

function DiagnosisOnReadyInSummary() {
    $("#diagnosisAddEdit").validationEngine();
    $(".ddlType1").prop("disabled", "disabled");
    $(".AddAsDiagnosis").show();

    var isPrimary = $("#hdIsPrimary").val();
    SetValueInDiagnosisTypeInSummary(isPrimary);
}

function GetDiagnosisTabDataInSummary() {
    var pid = $("#hdPatientId").val();
    var eid = $("#hdCurrentEncounterId").val();
    var jsonData = JSON.stringify({
        patientId: pid,
        encounterId: eid
    });
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: summaryPageUrl + "GetDiagnosisTabDetails",
        data: jsonData,
        dataType: "json",
        beforeSend: function () { },
        success: function (data) {

            /* Bind Previous Diagnosis Data, Starts here  */
            var columns = [{
                "targets": 0,
                "visible": false
            }, {
                "targets": 7,
                "mRender": function (data2, type, full) {
                    return '<a href="#" title="Add Current Diagnosis" onclick="AddPreviuosDiagnosisToCurrent(' + full[0] + ') " style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src= "../images/edit_small.png" /></a>';
                }
            }];
            $('#diagnosisprevious').dataTable({
                destroy: true,
                aaData: data.previousDiagnosisList,
                scrollY: "200px",
                scrollCollapse: true,
                bProcessing: true,
                paging: true,
                aoColumnDefs: columns

            });

            /* Bind Previous Diagnosis Data, Ends here  */

            BindCurrentDiagnosisData(data.DiagnosisList);
            BindFavoriteDiagnosisData(data.FavoriteDiagnosisList);

            var current = data.CurrentDiagnosis;
            if (current != null) {
                $("#hdPatientId").val(current.PatientID);

                $("#hdIsPrimary").val(current.IsPrimary ? 1 : 0);

                $("#hdEncounterId").val(current.EncounterID);
                $("#hdCorporateId").val(current.CorporateID);
                $("#hdfacilityId").val(current.FacilityID);
                //$("#hdMedicalRecordNumber").val();

                $("#hdIsMajorCPTEntered").val(current.IsMajorCPT ? 1 : 0);
                $("#hdIsMajorDRGEntered").val(current.IsMajorDRG ? 1 : 0);

                DiagnosisOnReady();
            }
        },
        error: function (msg) {

        }
    });
}




/************************** Diagnosis Tab Data ****************/
