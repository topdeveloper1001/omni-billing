function ValidateFile() {
    var value = $("#ImportXMLfile").val();
    if (value == '') {
        ShowMessage('Select any XML file!', "Alert", "warning", true);
        return false;
    }
    if (value != '') {
        if (!value.match(/\.xml|XML$/)) {
            ShowMessage('Please select only XML file!', "Alert", "warning", true);
            return false;
        }
    }
    var isValid = jQuery(".validateUploadExcel").validationEngine({ returnIsValid: true });
    if (isValid == true) {
    }
    else {
        return true;
    }
}

function FetchXMlParsedData(fileHeaderId) {
    if (fileHeaderId > 0) {
        var jsonData = JSON.stringify({ fileId: fileHeaderId });
        $.ajax({
            type: "POST",
            url: "/ImportBills/BindTpXMLParsedData",
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                BindList("#divXmlParsedData", data);
                $("#collapseXmlParsedData").addClass("in");
                return true;
            },
            error: function (msg) {
            }
        });
    }
    else {
        return false;
    }
}