var BindClaimsByPayerId = function (payerid) {
   
    if (payerid > 0) {
        $.ajax({
            type: "POST",
            url: '/BillHeader/GetClaimsByPayerId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({ payerid: payerid }),
            success: function (data) {
                $('#divFinalBillHEaderListView').empty().html(data);
                $('#BillActivityListDiv').empty();
            },
            error: function (msg) {
            }
        });
    }
}

var ViewPreXMLFile = function (bHeaderId, facilityId) {
    if (bHeaderId > 0) {
        $.ajax({
            type: "POST",
            url: '/BillHeader/GetPreXMLFile',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify({ billHeaderId: bHeaderId, facilityId: facilityId }),
            success: function (data) {
                BindList("#BillActivityListDiv", data);
                $("#CollapseBillActivitiesList").addClass("in");
                $('#billnumberSpn').html("Claim View");
            },
            error: function (msg) {
            }
        });
    }
};

