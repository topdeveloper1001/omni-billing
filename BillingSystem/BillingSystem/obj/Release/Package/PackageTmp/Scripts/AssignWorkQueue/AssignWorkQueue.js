$(function () {
    BindRuleEditorUsersDropdown();
});

function BindRuleEditorUsersDropdown() {
    /// <summary>
    /// Binds the rule editor users dropdown.
    /// </summary>
    /// <returns></returns>
    var selector = $(".ddlUsers");
    $.ajax({
        type: "POST",
        url: '/Home/GetRuleEditorUsers',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: null,
        success: function (data) {
            $(selector).empty();
            var items = '<option value="0">--Select--</option>';
            $.each(data, function (i, obj) {
                var newItem = "<option id='" + obj.Value + "'  value='" + obj.Value + "'>" + obj.Text + "</option>";
                items += newItem;
            });
            $(selector).html(items);
        },
        error: function (msg) {
        }
    });
}
