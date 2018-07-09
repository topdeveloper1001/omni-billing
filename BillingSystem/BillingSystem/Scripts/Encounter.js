function checkInt(id) {
    total = parseInt($("#" + id.id).val());
    if (isNaN(total) == true) {

        $("#" + id.id).css("border", "1px solid red");
    }
}
