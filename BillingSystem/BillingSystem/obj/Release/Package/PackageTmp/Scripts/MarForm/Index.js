$(function() {
    LoadMarFormList();

    BindFiscalYearDDls('#ddMarYear', "");
    BindMonthsList('#ddMarMonth', "");
});

function LoadMarFormList() {
    var monthValue = $("#ddMarMonth").val();
    var yearValue = $("#ddMarYear").val();
    var fromDate = monthValue + "/01/" + yearValue;
    //var tillDate = monthValue + "/31/" + yearValue;
    var encounterId = $("#hdCurrentEncounterId").val();
    var patientId = $("#hdPatientId").val();
    var jsonData = JSON.stringify({
        PatientId: patientId,
        EncounterId: encounterId,
        DisplayFlag: 1,
        MonthValue: 6,
        FromDate: fromDate
    });

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/MarForm/GetMarView",
        data: jsonData,
        async: false,
        dataType: "html",
        success: function(data) {
            //$(".main_wrapper").html(data);
            $("#MarFormListDiv").html(data);
        },
        error: function(msg) {
        }
    });
}