$(function () {
    ShowCoderDiagnosisActions();
});

function ShowCoderDiagnosisActions() {
    $(".diagnosisActions").show();
    toggleRadioButtons("#rdReviewedByCoder");
    $("#divDiagnosisReview").attr("disabled", true);
    $("#DRGCodesInDiagnosisDiv").show();
    SetValueInDiagnosisType(false);
}