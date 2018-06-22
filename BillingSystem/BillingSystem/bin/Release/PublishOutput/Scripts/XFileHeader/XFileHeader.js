function ViewFile(id) {
    var jsonData = JSON.stringify({
        id: id
        //fileName: 'abc'
    });
    $.ajax({
        type: "POST",
        url: '/XMLBilling/ViewFile',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "text",
        data: jsonData,
        success: function (data) {
            //var url = ".." + data;
            //window.open(url, "_blank", "toolbar=yes, scrollbars=yes, resizable=yes");
            //var xmlHeader = '<?xml version="1.0" encoding="UTF-8"?>';
            //xmlHeader += data;
            //var parser = new DOMParser();
            //// XML DOM object
            //var xmlObject = parser.parseFromString(data, "text/xml");
            //var win = window.open('about:blank');

            if (data != '') {
                $('#txtXmlBillingView').val('');
                $('#txtXmlBillingView').val(data);
                $('#divhidepopup').show();
            } else {
                ShowMessage('Something went wrong in the xml. Please try again later!', "", "warning", true);
            }
        },
        error: function (response) {
        }
    });
}


function ExportToXml(id) {
    window.location = "/XMLBilling/ExportToXml?id=" + id;
    //if (id > 0) {
    //    $.ajax({
    //        type: "POST",
    //        url: '/XMLBilling/ExportToXml',
    //        async: false,
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "text",
    //        data: JSON.stringify({ id: id }),
    //        success: function (data) {
    //            
    //        },
    //        error: function (response) {
    //            
    //        }
    //    });
    //}
}

if (typeof window.DOMParser === "undefined") {
    window.DOMParser = function () { };

    window.DOMParser.prototype.parseFromString = function (str, contentType) {
        if (typeof ActiveXObject !== 'undefined') {
            var xmldata = new ActiveXObject('MSXML.DomDocument');
            xmldata.async = false;
            xmldata.loadXML(str);
            return xmldata;
        } else if (typeof XMLHttpRequest !== 'undefined') {
            var xmldata = new XMLHttpRequest;

            if (!contentType) {
                contentType = 'application/xml';
            }

            xmldata.open('GET', 'data:' + contentType + ';charset=utf-8,' + encodeURIComponent(str), false);

            if (xmldata.overrideMimeType) {
                xmldata.overrideMimeType(contentType);
            }

            xmldata.send(null);
            return xmldata.responseXML;
        }
    };
}