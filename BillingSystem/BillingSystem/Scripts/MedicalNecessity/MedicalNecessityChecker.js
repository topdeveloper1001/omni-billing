
function BindMedicalNecessityGrid() {
    var searchText = $("#tags").val();
    var jsonData = JSON.stringify({
        text: searchText
});
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/MedicalNecessity/GetSearchData",
        dataType: "json",
        async: false,
        data: jsonData,
        success: function (data) {
           var list = new Array();
            $.each(data, function (i, items) {
                list.push(items.Description);
            });
           $("#tags").autocomplete({
                source: list
            });
        },
        error: function (msg) {

        }

    });
}
