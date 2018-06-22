$(function() {
    
});

var RescrubBillHeader = function(encounterId) {
    if (encounterId > 0) {
        $.ajax({
            type: "POST",
            url: "/MissingData/ScrubXMLBill",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({ encounterId: encounterId }),
            success: function(data) {
                var msg = "Record scrub successfully !";
                ShowMessage(msg, "Success", "success", true);
                BindList("#BillHeaderListDiv", data);
            },
            error: function(msg) {
            }
        });
    }
};

function ViewBillActivities(bHeaderId, urlAction) {
    /// <summary>
    ///     Views the bill activities.
    /// </summary>
    /// <param name="bHeaderId">The b header identifier.</param>
    /// <param name="urlAction">The URL action.</param>
    /// <returns></returns>
    if (bHeaderId > 0) {
        $.ajax({
            type: "POST",
            url: urlAction,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({ billHeaderId: bHeaderId }),
            success: function (data) {
                BindList("#BillActivityListDiv", data);
                $("#CollapseBillActivitiesList").addClass("in");
                GetBillHeaderDetials(bHeaderId);
                var divId = '#CollapseBillActivitiesList';
                $('html, body').animate({
                    scrollTop: $(divId).offset().top
                }, 2000);
            },
            error: function (msg) {
            }
        });
    }
}

var GetBillHeaderDetials = function (billheaderId) {
    if (billheaderId > 0) {
        $.ajax({
            type: "POST",
            url: "/BillHeader/GetBillHeaderDetails",
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ billHeaderId: billheaderId }),
            success: function (data) {
                var str = " ( Bill Number:" + data.BillNumber + " )";
                $('#billnumberSpn').html(str);
            },
            error: function (msg) {
            }
        });
    }
};