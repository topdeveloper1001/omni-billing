$(function () {
    $("#MedicalNotesFormDiv").validationEngine();
    $(".collapseTitle").bind("click", function () {
        $.validationEngine.closePrompt(".formError", true);
    });
});

