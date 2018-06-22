var baseUrl = "/Summary/";

//function SortEncounterGrid(event) {

//    var url = "/Summary/SortEncounterGrid";
//    var patientId = $("#hdPatientId").val();
//    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
//        url += "?patientId=" + patientId + "&" + event.data.msg;
//    }
//    $.ajax({
//        type: "POST",
//        url: url,
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        data: null,
//        success: function (data) {
//            $("#divEncounterListSummary").empty();
//            $("#divEncounterListSummary").html(data);

//        },
//        error: function (msg) {
//        }
//    });
//}

//function SortDiagnosisGrid1(event) {
//    var url = "/Summary/SortDiagnosisGrid";
//    var patientId = $("#hdPatientId").val();
//    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
//        url += "?patientId=" + patientId + "&encId=" + $('#hdCurrentEncounterId').val() + "&" + event.data.msg;
//    }
//    $.ajax({
//        type: "POST",
//        url: url,
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        data: null,
//        success: function (data) {
//            $("#divDiagnosisMainlist").empty();
//            $("#divDiagnosisMainlist").html(data);

//        },
//        error: function (msg) {
//        }
//    });
//}

//function SortPatientOpenOrders(event) {
//    var url = "/Summary/SortPatientOpenOrders";
//    var patientId = $("#hdPatientId").val();
//    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
//        url += "?patientId=" + patientId + "&encId=" + $('#hdCurrentEncounterId').val() + "&" + event.data.msg;
//    }
//    $.ajax({
//        type: "POST",
//        url: url,
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        data: null,
//        success: function (data) {
//            $("#divPatientOpenOrderList_1").empty();
//            $("#divPatientOpenOrderList_1").html(data);

//        },
//        error: function (msg) {
//        }
//    });
//}

//function SortEHRMedicalVital(event) {
//    var url = "/Summary/SortPatientMedicalVital";
//    var patientId = $("#hdPatientId").val();
//    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
//        url += "?pid=" + patientId + "&" + event.data.msg;
//    }
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        url: url,
//        data: null,
//        dataType: "html",
//        success: function (data) {
//            if (data != null) {
//                $("#divPatientMedicalVitals").empty();
//                $("#divPatientMedicalVitals").html(data);
//            }
//        },
//        error: function (msg) {

//        }
//    });

//}

//function SortEHRAllergiesList(event) {
//    var url = "/Summary/SortPatientAllergiesList";
//    var patientId = $("#hdPatientId").val();
//    var encounterId = $("#hdCurrentEncounterId").val();
//    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
//        url += "?patientId=" + patientId + "&" + event.data.msg;
//    }
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        url: url,
//        data: null,
//        dataType: "html",
//        success: function (data) {
//            if (data != null) {
//                $("#divPatientMedicalAllergies").empty();
//                $("#divPatientMedicalAllergies").html(data);
//            }
//        },
//        error: function (msg) {

//        }
//    });

//}


$(function () {
    var pId = $("#hdPatientId").val() == null || $("#hdPatientId").val() == "" ? 0 : $("#hdPatientId").val();
    var eId = $("#hdCurrentEncounterId").val();
    eId = eId > 0 || eId == "" ? ("&eId=" + eId) : "";

    $.ajax({
        type: "GET",
        url: baseUrl + "GetPatientSummaryDataOnLoad?pId=" + pId + eId,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            //Bind Encounters 
            var columns = [{ "data": "EncounterStartTime" }, { "data": "EncounterEndTime" }, { "data": "EncounterNumber" }
                , { "data": "EncounterTypeName" }, { "data": "EncounterPatientTypeName" }
                , { "data": "Charges" }, { "data": "Payment" }];
            BindJQDatatableWithData('dtEncountersInSummary', columns, data.allergies, isPaging = false);

            //Bind Diagnosis
            columns = [{ "data": "DiagnosisTypeName" }, { "data": "DiagnosisCode" }, { "data": "DiagnosisCodeDescription" }
                , { "data": "Notes" }, { "data": "CreatedDate" }, { "data": "EnteredBy" }];
            BindJQDatatableWithData('dtDiagnosisListInSummary', columns, data.allergies, isPaging = false);


            //Bind Current Orders
            var columns = [{ "data": "OrderCode" }, { "data": "OrderDescription" }, { "data": "CategoryName" }
                , { "data": "SubCategoryName" }, { "data": "Status" }, { "data": "Quantity" }, { "data": "FrequencyText" }
                , { "data": "PeriodDays" }];
            BindJQDatatableWithData('dtCurrentOrdersInSummary', columns, data.allergies, isPaging = false);


            //Bind Vitals
            var columns = [{ "data": "MedicalVitalName" }, { "data": "PressureCustom" }, { "data": "UnitOfMeasureName" }
                , { "data": "VitalAddedBy" }, { "data": "VitalAddedOn" }];
            BindJQDatatableWithData('dtVitalsInSummary', columns, data.allergies, isPaging = false);

            //Bind Allergies
            columns = [{ "data": "AlergyType" }, { "data": "AlergyName" }, { "data": "DetailAnswer" }, { "data": "AddedBy" }
                , { "data": "CreatedDate" }];
            BindJQDatatableWithData('dtAllergiesInSummary', columns, data.allergies, isPaging = false);

            //Bind Medical Notes
            var columns = [{ "data": "Notes" }, { "data": "NotesTypeName" }, { "data": "NotesAddedBy" }, { "data": "NotesDate" }];
            BindJQDatatableWithData('dtNotesInSummary', columns, data.allergies, isPaging = false);

            //Bind Medications
            columns = [{ "data": "DrugName", "width": "200px" }, { "data": "DrugDuration" }, { "data": "DrugVolume" }, { "data": "DrugDosage" }
                , { "data": "DrugFrequency" }];
            BindJQDatatableWithData('dtMedicationsInSummary', columns, data.medications, isPaging = false);
        },
        error: function (msg) {
        }
    });
});