$(function () {
    BindDepartmentsDropdown();
    BindGlobalCodesWithValueCustom("#ddlFacilityType", 4242, "");
    BindGlobalCodesWithValueCustom("#ddlRegionType", 4141, "");
    BindFacilitiesWithoutCorporate('#ddlFacility', $('#hdFacilityId').val());
    BindAndSetDefaultMonth(903, $('#hdFacilityId').val(), "", "#ddlMonth");

    setTimeout(function () {
        BindBillScrubberGraphsUpdated();
        $('#ddlFacility option[value="0"]').text('---All---');
    }, 500);

    $('#btnReBindGraphs').on('click', function () {
        BindBillScrubberGraphsUpdated();
    });
});

/// <var>The enableddls</var>
var Enableddls = function () {
    var facilityId = $('#ddlFacility').val();
    if (facilityId === "0") {
        $('.facDisabled').removeAttr('disabled');
    } else {
        $('.facDisabled').val('0');
        $('.facDisabled').attr('disabled', 'disabled');
    }

}

/// <var>bind the accounts dropdown</var>
var BindDepartmentsDropdown = function () {
    $.post("/FacilityDepartment/BindAccountDropdowns", null, function (data) {
        $("#ddlDepartment").empty();
        var items = '<option value="0">--All--</option>';
        $.each(data.reveuneAccount, function (i, obj) {
            var newItem = "<option id='" + obj.Value + "'  value='" + obj.Value + "'>" + obj.Text + "</option>";
            items += newItem;
        });
        $("#ddlDepartment").html(items);
        $("#ddlDepartment").val('0');
    });
};

function BindRemarksList(id) {
    var facilityId = $('#ddlFacility').val();
    var month = $('#ddlMonth').val();
    var facilityType = $('#ddlFacilityType').val();
    var regionType = $('#ddlRegionType').val();
    var department = $('#ddlDepartment').val();
    var dashBoardType = $('#hdDashboardType').val();
    var typeSection = id;
    var viewAll = $('#chkShowAllSection' + id).prop('checked');
    var jsonData = JSON.stringify({
        facilityId: facilityId,
        month: month,
        facilityType: facilityType,
        segment: regionType,
        Department: department != null ? department : 0,
        type: dashBoardType,
        viewAll: viewAll,
        sectionType: typeSection
    });

    $.ajax({
        cache: false,
        type: "POST",
        url: '/ExternalDashboard/ViewRemarkdsListByDashboardSection',
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            BindList("#divSection" + id, data);
        },
        error: function (msg) {
        }
    });
};

/*Created with new changes*/
function BindBillScrubberGraphsUpdated() {
    var departmentNumber = $('#ddlDepartment').val() != null ? $('#ddlDepartment').val() : 0;
    var d = new Date();
    var monthVal = d.getMonth() + 1;
    var jsonData = JSON.stringify({
        facilityId: $('#ddlFacility').val() != null ? $('#ddlFacility').val() : 0, // $('#ddlFacility').val(),
        month: $('#ddlMonth').val() != null ? $('#ddlMonth').val() : monthVal,
        facilityType: $('#ddlFacilityType').val(),
        segment: $('#ddlRegionType').val(),
        department: departmentNumber,
    });
    $.ajax({
        cache: false,
        type: "POST",
        url: '/XMLDashboard/BillScrubberGraphsData',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            BindBillScrubberGraphs(data);
        },
        error: function (msg) {
        }
    });
}


var BindBillScrubberGraphs = function (data) {
    
    if (data != null && data != "") {
        //--------------Section 1 Graphs Start here------------------------------------------/////
        /*Indicator Number - 2000*/
        if (data.TotalDollarBilledClaims != null && data.TotalDollarBilledClaims.length > 0) {
            BindMonthActualAndYtdGraphBillScrubber(data.TotalDollarBilledClaims, 'myDashboardTotalDollarBilledClaims', "column", "Total Dollar Billed Claims", 3, 'By Month');
        } else {
            EmptyGraphsBuilderWithoutPercentageSubtitleTwoBarBillScrubber('myDashboardTotalDollarBilledClaims', "column", "Total Dollar Billed Claims ", 3, 'By Month');
        }
        /*Indicator Number - 2001*/
        if (data.TotalDollarDeniedClaims != null && data.TotalDollarDeniedClaims.length > 0) {
            BindMonthActualAndYtdGraphBillScrubber(data.TotalDollarDeniedClaims, 'myDashboardTotalDollarDeniedClaims', "column", "Total Dollar Denied Claims", 3, 'By Month');
        } else {
            EmptyGraphsBuilderWithoutPercentageSubtitleTwoBarBillScrubber('myDashboardTotalDollarDeniedClaims', "column", "Total Dollar Denied Claims", 3, 'By Month');
        }
        //--------------Section 1 Graphs end here------------------------------------------/////


        //--------------Section 2 Graphs Start here------------------------------------------/////
        /*Indicator Number - 2002*/
        if (data.GrossDenialRate != null && data.GrossDenialRate.length > 0) {
            BindMonthActualAndYtdGraphBillScrubberWithOneDecimalPercent(data.GrossDenialRate, 'myDashboardGrossDenialRate', "column", "Gross Denial Rate", 3, 'By Month');
        } else {
            EmptyGraphsBuilderWithoutPercentageSubtitleTwoBarBillScrubber('myDashboardGrossDenialRate', "column", "Gross Denial Rate", 3, 'By Month');
        }
        /*Indicator Number - 2003*/
        if (data.DollarAmountTechnicalEdits != null && data.DollarAmountTechnicalEdits.length > 0) {
            BindMonthActualAndYtdGraphBillScrubberDollarAmtTechEdits(data.DollarAmountTechnicalEdits, 'myDashboardDollarAmountTechnicalEdits', "column", "Dollar Amount Technical Edits", 3, 'By Month - 000s');
        } else {
            EmptyGraphsBuilderWithoutPercentageSubtitleTwoBarBillScrubber('myDashboardDollarAmountTechnicalEdits', "column", "Dollar Amount Technical Edits", 3, 'By Month - 000s');
        }
        //--------------Section 2 Graphs End here------------------------------------------/////


        //--------------Section 3 and 4 Graphs Start here------------------------------------------/////
        /*Indicator Number - 2004*/
        if (data.DenialsbyReasonforDenial != null && data.DenialsbyReasonforDenial.length > 0) {
            BuildCustomGraphWithoutDecimalsBillScrubberDenials(data.DenialsbyReasonforDenial, 'myDashboardDenialsbyReasonforDenial', "column", "Denials by Reason for Denial", 3, 'Current Month - 000s', 0);
            BuildCustomGraphWithoutDecimalsBillScrubberDenialsYearToDate(data.DenialsbyReasonforDenial, 'myDashboardDenialsbyReasonforDenialYearToDate', "column", "Denials by Reason for Denial", 3, 'Year To Date - 000s', 1);
        } else {
            EmptyGraphMonthlyBillScrubber(data.DenialsbyReasonforDenial, 'myDashboardDenialsbyReasonforDenial', "column", "Denials by Reason for Denial", 3, 'Current Month - 000s');
            EmptyGraphYearToDateBillScrubber(data.DenialsbyReasonforDenial, 'myDashboardDenialsbyReasonforDenialYearToDate', "column", "Denials by Reason for Denial", 3, 'Year To Date - 000s');
            //EmptyGraphYearToDateBillScrubber(data.DenialsbyReasonforDenial, 'myDashboardDenialsbyReasonforDenialYearToDate', "column", "Denials by Reason for Denial", 3, 'Year To Date - 000s');

        }
        //--------------Section 3 and 4 Graphs End here------------------------------------------/////



        //--------------Section 5 Graphs Start here------------------------------------------/////
        /*Indicator Number - 2005*/
        if (data.DollarClaimsResubmittedbyDenialByMonth != null && data.DollarClaimsResubmittedbyDenialByMonth.length > 0) {
            BuildCustomGraphWithoutDecimalsBillScrubberDollarClaims(data.DollarClaimsResubmittedbyDenialByMonth, 'myDashboardDollarClaimsResubmittedbyDenialByMonth', "column", "Dollar Claims Resubmitted by Denial", 3, 'Current Month', 0);
            BuildCustomGraphWithoutDecimalsBillScrubberDollarClaims(data.DollarClaimsResubmittedbyDenialByMonth, 'myDashboardClaimsResubmittedbyDenialByYTD', "column", "Dollar Claims Resubmitted by Denial", 3, 'Year To Date', 'Year To Date', 1);
        } else {
            EmptyGraphMonthlyBillScrubber(data.DollarClaimsResubmittedbyDenialByMonth, 'myDashboardDollarClaimsResubmittedbyDenialByMonth', "column", "Dollar Claims Resubmitted by Denial", 3, 'Current Month - 000s');
            EmptyGraphYearToDateBillScrubber(data.DollarClaimsResubmittedbyDenialByMonth, 'myDashboardDenialsbyReasonforDenialYearToDate', "column", "Dollar Claims Resubmitted by Denial", 3, 'Year To Date - 000s');
            //EmptyGraphYearToDateBillScrubber(data.DenialsbyReasonforDenial, 'myDashboardDenialsbyReasonforDenialYearToDate', "column", "Dollar Claims Resubmitted by Denial", 3, 'Year To Date - 000s');

        }
        //--------------Section 5 Graphs End here------------------------------------------/////



        //--------------Section 6 Graphs Start here------------------------------------------/////
        /*Indicator Number - 2006*/
        if (data.TotalDollarClaimsResubmitted != null && data.TotalDollarClaimsResubmitted.length > 0) {
            GraphsBuilderWithoutPercentageBillScrubber45Angle(data.TotalDollarClaimsResubmitted, 'myDashboardTotalDollarClaimsResubmitted', "line", "Total Dollar Claims Resubmitted", 3);
        } else {
            EmptyGraphsBuilderWithoutPercentageBillScrubber('myDashboardTotalDollarClaimsResubmitted', "line", "Total Dollar Claims Resubmitted", 3);
        }
        /*Indicator Number - 2007 (SubCategory - 27)*/
        if (data.TotalResubmissionDollarsCollected != null && data.TotalResubmissionDollarsCollected.length > 0) {
            GraphsBuilderWithoutPercentageBillScrubber(data.TotalResubmissionDollarsCollected, 'myDashboardTotalResubmissionDollarsCollected', "column", "Total Resubmission Dollars Collected", 3);
        } else {
            EmptyGraphsBuilderWithoutPercentageBillScrubber('myDashboardTotalResubmissionDollarsCollected', "column", "Total Resubmission Dollars Collected ", 3);
        }
        //--------------Section 6 Graphs End here------------------------------------------/////


        //--------------Section 7 Graphs Start here------------------------------------------/////
        /*Indicator Number - 2007 (SubCategory - 142)*/
        if (data.TotalResubmissionInpatientDollarsCollected != null && data.TotalResubmissionInpatientDollarsCollected.length > 0) {
            GraphsBuilderCustomSubTitleWithoutPercentageBillScrubber(data.TotalResubmissionInpatientDollarsCollected, 'myDashboardTotalResubmissionInpatientDollarsCollected', "column", "Total Resubmission Inpatient Dollars Collected", 3, "By Month - 000s");
        } else {
            EmptyGraphsBuilderWithoutPercentageSubtitleBillScrubber('myDashboardTotalResubmissionInpatientDollarsCollected', "column", "Total Resubmission Inpatient Dollars Collected ", 3, "By Month - 000s)");
        }


        /*Indicator Number - 2007 (SubCategory - 143) */
        if (data.TotalResubmissionOutpatientDollarsCollected != null && data.TotalResubmissionOutpatientDollarsCollected.length > 0) {
            GraphsBuilderCustomSubTitleWithoutPercentageBillScrubber(data.TotalResubmissionOutpatientDollarsCollected, 'myDashboardTotalResubmissionOutpatientDollarsCollected', "column", "Total Resubmission Outpatient Dollars Collected", 3, "By Month - 000s");
        } else {
            EmptyGraphsBuilderWithoutPercentageBillScrubber('myDashboardTotalResubmissionOutpatientDollarsCollected', "column", "Total Resubmission Outpatient Dollars Collected", 3);
        }
        //--------------Section 7 Graphs End here------------------------------------------/////



        //--------------Section 8 Graphs Start here------------------------------------------/////
        /*Indicator Number - 2008*/
        if (data.DollarPercentofDenialsResubmitted != null && data.DollarPercentofDenialsResubmitted.length > 2) {
            BilliableLineGraphBillScrubber(data.DollarPercentofDenialsResubmitted, 'myDashboardDollarPercentofDenialsResubmitted', "line", "Dollar Percent of Denials Resubmitted", 2);
        } else {
            EmptyGraphsBuilderWithoutPercentageBillScrubber('myDashboardDollarPercentofDenialsResubmitted', "line", "Dollar Percent of Denials Resubmitted", 2);
        }
        /*Indicator Number - 2009*/
        if (data.PercentofResubmissionsCollected != null && data.PercentofResubmissionsCollected.length > 2) {
            BilliableLineGraphBillScrubber(data.PercentofResubmissionsCollected, 'myDashboardPercentofResubmissionsCollected', "line", "Percent of Resubmissions Collected", 2);
        } else {
            EmptyGraphsBuilderWithoutPercentageBillScrubber('myDashboardPercentofResubmissionsCollected', "line", "Percent of Resubmissions Collected", 2);
        }
        //--------------Section 8 Graphs End here------------------------------------------/////

        //--------------Section 9 Graphs Start here------------------------------------------/////
        /*Indicator Number - 2010*/
        if (data.TotalCashCollected != null && data.TotalCashCollected.length > 2) {
            GraphsBuilderCustomSubTitleWithoutPercentageBillScrubber(data.TotalCashCollected, 'myDashboardTotalCashCollected', "column", "Total Cash Collected", 3, 'By Month');
        } else {
            EmptyGraphsBuilderWithoutPercentageSubtitleBillScrubber('myDashboardTotalCashCollected', "column", "Total Cash Collected", 3, 'By Month');
        }
        /*Indicator Number - 2011*/
        if (data.AverageAmountCollectedPerResubmission != null && data.AverageAmountCollectedPerResubmission.length > 0) {
            GraphsBuilderCustomSubTitleWithoutPercentageBillScrubber(data.AverageAmountCollectedPerResubmission, 'myDashboardAverageAmountCollectedPerResubmission', "column", "Average Amount Collected Per Resubmission", 3, 'By Month', '');
        } else {
            EmptyGraphsBuilderWithoutPercentageSubtitleBillScrubber('myDashboardAverageAmountCollectedPerResubmission', "column", "Average Amount Collected Per Resubmission", 3, "By Month");
        }
        //--------------Section 9 Graphs End here------------------------------------------/////


        //--------------Section 10 Graphs Start here------------------------------------------/////
        /*Indicator Number - 2012*/
        if (data.RevenueCollectedByDenialCodeYearToDate != null && data.RevenueCollectedByDenialCodeYearToDate.length > 2) {
            BuildBarGraphsRevenueByCategoryBillScrubber(data.RevenueCollectedByDenialCodeYearToDate, 'divRevenueCollectedbyDenialCode', "column", "Revenue Collected by Denial Code", 2, 'Year To Date - 000s');
        } else {
            EmptyGraphsBuilderWithoutPercentageSubtitleBillScrubber('divRevenueCollectedbyDenialCode', "column", "Revenue Collected by Denial Code", 2, 'Year To Date - 000s');
        }
        /*Indicator Number - 2013*/
        if (data.PercentRevenueCollectedByDenialCodeYTD != null && data.PercentRevenueCollectedByDenialCodeYTD.length > 2) {
            SubCategoryPieChartBuilderYearToDateBillScrubber(data.PercentRevenueCollectedByDenialCodeYTD, 'divPercentageRevenueCollectedByDenialCode', "column", "% Revenue Collected By Denial Code", 2);
        } else {
            EmptyGraphsBuilderWithoutPercentageSubtitleBillScrubber('divPercentageRevenueCollectedByDenialCode', "column", "% Revenue Collected By Denial Code", 2, 'Year To Date');
        }
        //--------------Section 10 Graphs End here------------------------------------------/////
    }
}

/*new methods - start*/
function BindMonthActualAndYtdGraphBillScrubber(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle) {
    var month = $('#ddlMonth').val();
    var chartData = new Array();
    var categories = new Array();
    switch (parseInt(month)) {
        case 1:
            categories = ['January'];
            break;
        case 2:
            categories = ['February'];
            break;
        case 3:
            categories = ['March'];
            break;
        case 4:
            categories = ['April'];
            break;
        case 5:
            categories = ['May'];
            break;
        case 6:
            categories = ['June'];
            break;
        case 7:
            categories = ['July'];
            break;
        case 8:
            categories = ['August'];
            break;
        case 9:
            categories = ['September'];
            break;
        case 10:
            categories = ['October'];
            break;
        case 11:
            categories = ['November'];
            break;
        default:
            categories = ['December'];
            break;
    }
    if (dashboardData != null) {
        for (var i = 0; i < dashboardData.length; i++) {
            chartData.push({ 'name': "Current Month", 'data': [parseInt(dashboardData[i].CMA)] });
            chartData.push({ 'name': "Year To Date", 'data': [parseInt(dashboardData[i].CYTA)] });
        }
        BuildTwoSeriseGraphWithLegendsTooltipBillScrubber(containerid, chartData, charttype, chartName, subtitle, categories);
    }
}
function BindMonthActualAndYtdGraphBillScrubberDollarAmtTechEdits(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle) {
    var month = $('#ddlMonth').val();
    var chartData = new Array();
    var categories = new Array();
    switch (parseInt(month)) {
        case 1:
            categories = ['January'];
            break;
        case 2:
            categories = ['February'];
            break;
        case 3:
            categories = ['March'];
            break;
        case 4:
            categories = ['April'];
            break;
        case 5:
            categories = ['May'];
            break;
        case 6:
            categories = ['June'];
            break;
        case 7:
            categories = ['July'];
            break;
        case 8:
            categories = ['August'];
            break;
        case 9:
            categories = ['September'];
            break;
        case 10:
            categories = ['October'];
            break;
        case 11:
            categories = ['November'];
            break;
        default:
            categories = ['December'];
            break;
    }
    if (dashboardData != null) {
        for (var i = 0; i < dashboardData.length; i++) {
            chartData.push({ 'name': "Current Month", 'data': [parseInt(dashboardData[i].CMA)] });
            chartData.push({ 'name': "Year To Date", 'data': [parseInt(dashboardData[i].CYTA) / $("#ddlMonth").val()] });// YTD number needs to be an average of the months for now
        }
        BuildTwoSeriseGraphWithLegendsTooltipBillScrubber(containerid, chartData, charttype, chartName, subtitle, categories);
    }
}
function BindMonthActualAndYtdGraphBillScrubberWithOneDecimalPercent(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle) {
    var month = $('#ddlMonth').val();
    var chartData = new Array();
    var categories = new Array();
    switch (parseInt(month)) {
        case 1:
            categories = ['January'];
            break;
        case 2:
            categories = ['February'];
            break;
        case 3:
            categories = ['March'];
            break;
        case 4:
            categories = ['April'];
            break;
        case 5:
            categories = ['May'];
            break;
        case 6:
            categories = ['June'];
            break;
        case 7:
            categories = ['July'];
            break;
        case 8:
            categories = ['August'];
            break;
        case 9:
            categories = ['September'];
            break;
        case 10:
            categories = ['October'];
            break;
        case 11:
            categories = ['November'];
            break;
        default:
            categories = ['December'];
            break;
    }
    if (dashboardData != null) {
        for (var i = 0; i < dashboardData.length; i++) {
            chartData.push({ 'name': "Current Month", 'data': [parseFloat(dashboardData[i].CMA) * 100] });
            chartData.push({ 'name': "Year To Date", 'data': [parseFloat(dashboardData[i].CYTA) / $("#ddlMonth").val() * 100] });//Divided by month number to show YTD number needs to be an average of the months for now
        }
        BuildTwoSeriseGraphWithLegendsTooltipBillScrubberWithOneDecimalPercent(containerid, chartData, charttype, chartName, subtitle, categories);
    }
}
function BuildTwoSeriseGraphWithLegendsTooltipBillScrubberWithOneDecimalPercent(container, twoDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {
    /// <summary>
    /// Shows the two bars chart with labels on bars.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="twoDimDataArray">The two dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            floating: true,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            min: 0,
            title: {
                text: ''
            }
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: <b>{point.y:.1f}%</b> '
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800,
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    align: 'top',
                    format: '{point.y:.1f}%',
                    enabled: true,
                    floating: true,
                    crop: false,
                    overflow: 'none',
                    rotation: 360,
                    x: -12,
                    y: -3,
                    style: {
                        fontWeight: 'normal',
                        y: -5
                    }
                }
            }
        },
        series: [{
            name: twoDimDataArray[0].name,
            data: twoDimDataArray[0].data,//[34.7, 34.8, 34.6,36.4,40.0],//twoDimDataArray[0].data,
            color: '#48CCCD'
        },
        {
            name: twoDimDataArray[1].name,
            data: twoDimDataArray[1].data,//[10.7, 10.8, 10.6, 10.4, 10.0],//twoDimDataArray[1].data,
            color: '#7FE817'
        }]
    });
}
function BuildTwoSeriseGraphWithLegendsTooltipBillScrubber(container, twoDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {
    /// <summary>
    /// Shows the two bars chart with labels on bars.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="twoDimDataArray">The two dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            floating: true,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            min: 0,
            title: {
                text: ''
            }
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y} '
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800,
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    align: 'top',
                    enabled: true,
                    floating: true,
                    crop: false,
                    overflow: 'none',
                    rotation: 360,
                    x: -12,
                    y: -3,
                    style: {
                        fontWeight: 'normal',
                        y: -5
                    }
                }
            }
        },
        series: [{
            name: twoDimDataArray[0].name,
            data: twoDimDataArray[0].data,//[34.7, 34.8, 34.6,36.4,40.0],//twoDimDataArray[0].data,
            color: '#48CCCD'
        },
        {
            name: twoDimDataArray[1].name,
            data: twoDimDataArray[1].data,//[10.7, 10.8, 10.6, 10.4, 10.0],//twoDimDataArray[1].data,
            color: '#7FE817'
        }]
    });
}
function EmptyGraphsBuilderWithoutPercentageSubtitleTwoBarBillScrubber(containerid, charttype, chartName, chartLegendPosition, subtitle) {
    var chartData = new Array();
    var categories = new Array();
    categories = ["Current Month", "Year To Date"];
    chartData.push({ 'name': 'Current Month', 'data': parseFloat(0) });
    chartData.push({ 'name': "Year To Date", 'data': parseFloat(0) });
    BuildTwoSeriseGraphWithLegendsTooltipBillScrubber(containerid, chartData, charttype, chartName, subtitle, categories);
}
function EmptyGraphMonthlyBillScrubber(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle) {
    
    var chartData = new Array();
    var categories = new Array();
    if (dashboardData.length == 8) {
        for (var i = 0; i < dashboardData.length; i++) {
            chartData.push({ 'name': dashboardData[i].DashBoard, 'data': [parseInt(0)] });
            categories.push(dashboardData[i].DashBoard);
        }
        BuildEightSeriseBarGraphWithOutDecimalsBillScrubber(containerid, chartData, charttype, chartName, subtitle, categories);
    }
    else
        if (dashboardData.length == 4) {
            for (var j = 0; j < dashboardData.length; j++) {
                chartData.push({ 'name': dashboardData[j].DashBoard, 'data': [parseFloat(0)] });
                categories.push(dashboardData[j].DashBoard);
            }
            BuildFourSeriseBarGraphWithDecimalsBillScrubber(containerid, chartData, charttype, chartName, subtitle, categories, categories);
        }
}
function BuildEightSeriseBarGraphWithOutDecimalsBillScrubber(container, dataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {

    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            /*categories: categoriesArray,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            },*/ //['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
            labels: {
                enabled: false
            },
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            min: 0,
            title: {
                text: ''
            }
        },
        exporting: {
            filename: widgetTitle,
            sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            formatter: function () {
                var toolTip = '<span style="color:' + this.series.color + '">\u25CF</span> ' + this.series.name + ':' + this.point.y + '';
                return toolTip;
            }
            //headerFormat: '<b>{point.key}</b><br>',
            //pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y}'
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    enabled: true,
                    format: '{point.y}',
                    rotation: -90,
                    color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
        },
        series: [{
            name: dataArray[0].name,
            data: dataArray[0].data,
        },
        {
            name: dataArray[1].name,
            data: dataArray[1].data,
        },
        {
            name: dataArray[2].name,
            data: dataArray[2].data,
        },
        {
            name: dataArray[3].name,
            data: dataArray[3].data,
        },
        {
            name: dataArray[4].name,
            data: dataArray[4].data,
        },
        {
            name: dataArray[5].name,
            data: dataArray[5].data,
        },
        {
            name: dataArray[6].name,
            data: dataArray[6].data,
        },
        {
            name: dataArray[7].name,
            data: dataArray[7].data,
        }
        ]
    });
}
function BuildFourSeriseBarGraphWithDecimalsBillScrubber(container, dataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {

    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            } //['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            min: 0,
            title: {
                text: ''
            }
        },
        exporting: {
            filename: widgetTitle,
            sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            //headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y:,.2f}'
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    enabled: true,
                    format: '{point.y:,.2f}',
                    rotation: -90,
                    color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
        },
        series: [{
            name: dataArray[0].name,
            data: dataArray[0].data,
        },
        {
            name: dataArray[1].name,
            data: dataArray[1].data,
        },
        {
            name: dataArray[2].name,
            data: dataArray[2].data,
        },
        {
            name: dataArray[3].name,
            data: dataArray[3].data,
        }
        ]
    });
}
function EmptyGraphYearToDateBillScrubber(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle) {

    var chartData = new Array();
    if (dashboardData.length == 8) {
        for (var i = 0; i < dashboardData.length; i++) {
            chartData.push({ 'name': dashboardData[i].DashBoard, 'data': [parseInt(0)] });
            categories.push(dashboardData[i].DashBoard);
        }
        BuildEightSeriseBarGraphWithOutDecimalsBillScrubber(containerid, chartData, charttype, chartName, subtitle, categories);
    }
    else
        if (dashboardData.length == 4) {
            for (var j = 0; j < dashboardData.length; j++) {
                chartData.push({ 'name': dashboardData[j].DashBoard, 'data': [parseFloat(0)] });
                categories.push(dashboardData[j].DashBoard);
            }
            BuildFourSeriseBarGraphWithDecimalsBillScrubber(containerid, chartData, charttype, chartName, subtitle, categories, categories);
        }
}
function EmptyGraphsBuilderWithoutPercentageBillScrubber(containerid, charttype, chartName, chartLegendPosition) {
    var month = $('#ddlMonth').val();
    var dataArray = new Array();
    var categories = new Array();

    var monthsArray = new Array();
    for (var i = 0; i < 3; i++) {
        monthsArray = new Array();
        switch (parseInt(month)) {
            case 1:
                monthsArray.push(parseFloat(0));
                categories = ['Jan'];
                break;
            case 2:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb'];
                break;
            case 3:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar'];
                break;
            case 4:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr'];
                break;
            case 5:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May'];
                break;
            case 6:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
                break;
            case 7:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'];
                break;
            case 8:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug'];
                break;
            case 9:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'];
                break;
            case 10:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'];

                break;
            case 11:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov'];

                break;
            default:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                break;
        }
        var dashbaordName = i == 0 ? "Prior Year" : i == 1 ? "Actual" : "Target";
        dataArray.push({ 'name': dashbaordName, 'data': monthsArray });
    }
    if (chartLegendPosition == 1)
        BuildThreeSeriseBarGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 7)
        BuildThreeSeriseBarGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, "By Month (000s)", categories);
    else
        BuildThreeSeriseGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, "Current Month", categories);
}
function BuildThreeSeriseBarGraphWithLegendsTooltipBillScrubber(container, threeDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray, yaxisTitle) {
    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            allowDecimals: false,
            //min: 0,
            title: {
                text: yaxisTitle
            }
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            //headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y:.1f} '
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    /*enabled: true,
                    format: '{point.y:.1f}',
                    rotation: -90,
                    color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }*/
                    align: 'top',
                    enabled: true,
                    floating: true,
                    crop: false,
                    format: '{point.y:.1f}',
                    overflow: 'none',
                    rotation: 360,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        y: -5
                    }
                }
            }
        },
        series: [{
            name: threeDimDataArray[0].name,
            data: threeDimDataArray[0].data,
            color: '#FFFF00',
        },
        {
            name: threeDimDataArray[2].name,
            data: threeDimDataArray[2].data,
            color: '#48CCCD',
        },
        {
            name: threeDimDataArray[1].name,
            data: threeDimDataArray[1].data,
            color: '#7FE817'
        }]
    });
}
function BuildThreeSeriseGraphWithLegendsTooltipBillScrubber(container, threeDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray, yaxisTitle) {
    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            allowDecimals: false,
            //min: 0,
            title: {
                text: yaxisTitle
            }
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            //headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y} '
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    align: 'top',
                    enabled: true,
                    floating: true,
                    crop: false,
                    format: '{point.y}',
                    overflow: 'none',
                    rotation: 360,
                    style: {
                        fontWeight: 'normal',
                        y: -5
                    }
                }
            }
        },
        series: [{
            name: threeDimDataArray[0].name,
            data: threeDimDataArray[0].data,
            color: '#FFFF00',
        },
        {
            name: threeDimDataArray[2].name,
            data: threeDimDataArray[2].data,
            color: '#48CCCD',
        },
        {
            name: threeDimDataArray[1].name,
            data: threeDimDataArray[1].data,
            color: '#7FE817'
        }]
    });
}
function GraphsBuilderWithoutPercentageBillScrubber(dashboardData, containerid, charttype, chartName, chartLegendPosition) {
    var month = $('#ddlMonth').val();
    var dataArray = new Array();
    var categories = new Array();

    var monthsArray = new Array();
    for (var i = 0; i < 3; i++) {
        monthsArray = new Array();
        switch (parseInt(month)) {
            case 1:
                monthsArray.push(parseInt(dashboardData[i].M1));
                categories = ['Jan'];
                break;
            case 2:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                categories = ['Jan', 'Feb'];
                break;
            case 3:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                categories = ['Jan', 'Feb', 'Mar'];
                break;
            case 4:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                categories = ['Jan', 'Feb', 'Mar', 'Apr'];
                break;
            case 5:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May'];
                break;
            case 6:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
                break;
            case 7:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'];
                break;
            case 8:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                monthsArray.push(parseInt(dashboardData[i].M8));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug'];
                break;
            case 9:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                monthsArray.push(parseInt(dashboardData[i].M8));
                monthsArray.push(parseInt(dashboardData[i].M9));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'];
                break;
            case 10:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                monthsArray.push(parseInt(dashboardData[i].M8));
                monthsArray.push(parseInt(dashboardData[i].M9));
                monthsArray.push(parseInt(dashboardData[i].M10));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'];

                break;
            case 11:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                monthsArray.push(parseInt(dashboardData[i].M8));
                monthsArray.push(parseInt(dashboardData[i].M9));
                monthsArray.push(parseInt(dashboardData[i].M10));
                monthsArray.push(parseInt(dashboardData[i].M11));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov'];

                break;
            default:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                monthsArray.push(parseInt(dashboardData[i].M8));
                monthsArray.push(parseInt(dashboardData[i].M9));
                monthsArray.push(parseInt(dashboardData[i].M10));
                monthsArray.push(parseInt(dashboardData[i].M11));
                monthsArray.push(parseInt(dashboardData[i].M12));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                break;
        }
        //var dashbaordName = i == 0 ? "Prior Year" : i == 1 ? "Actual" : "Budget";
        //dataArray.push({ 'name': dashbaordName, 'data': monthsArray });

        var currentYear = new Date().getFullYear();
        var dashboardName = dashboardData[i].BudgetType == 2 && dashboardData[i].Year == currentYear - 1 ? "Prior Year" : (dashboardData[i].BudgetType == 2 ? "Actual" : "Target");
        dataArray.push({ 'name': dashboardName, 'data': monthsArray });
    }
    if (chartLegendPosition == 1)
        BuildThreeSeriseBarGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 2)
        BuildThreeSeriseBarGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 3)
        BuildThreeSeriseBarGraphWithOutDecimalsBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 4)
        BuildThreeSeriseGraphWithLegendsTooltipPercentageBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 5)
        BuildThreeSeriseGraphWithOutDecimalLabelBillscrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 6)
        BuildThreeSeriseGraphWithLevelBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 7)
        BuildThreeSeriseBarGraphWithOutDecimalsBillScrubber(containerid, dataArray, charttype, chartName, "By Month (000s)", categories);
}
function BuildThreeSeriseBarGraphWithOutDecimalsBillScrubber(container, threeDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray, yaxisTitle) {
    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            allowDecimals: false,
            //min: 0,
            title: {
                text: yaxisTitle
            }
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            //headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y:,.0f}'
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    enabled: true,
                    format: '{point.y:,.0f}',
                    rotation: -90,
                    //color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
        },
        series: [{
            name: threeDimDataArray[0].name,
            data: threeDimDataArray[0].data,
            color: '#FFFF00',
        },
        {
            name: threeDimDataArray[2].name,
            data: threeDimDataArray[2].data,
            color: '#48CCCD',
        },
        {
            name: threeDimDataArray[1].name,
            data: threeDimDataArray[1].data,
            color: '#7FE817'
        }]
    });
}
function BuildThreeSeriseBarGraphWithOutDecimalsBillScrubber45Angle(container, threeDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray, yaxisTitle) {
    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            allowDecimals: false,
            //min: 0,
            title: {
                text: yaxisTitle
            }
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            //headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y:,.0f}'
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    enabled: true,
                    format: '{point.y:,.0f}',
                    rotation: 45,
                    //color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
        },
        series: [{
            name: threeDimDataArray[0].name,
            data: threeDimDataArray[0].data,
            color: '#FFFF00',
        },
        {
            name: threeDimDataArray[2].name,
            data: threeDimDataArray[2].data,
            color: '#48CCCD',
        },
        {
            name: threeDimDataArray[1].name,
            data: threeDimDataArray[1].data,
            color: '#7FE817'
        }]
    });
}
function BuildThreeSeriseGraphWithLegendsTooltipPercentageBillScrubber(container, twoDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {
    /// <summary>
    /// Shows the two bars chart with labels on bars.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="twoDimDataArray">The two dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            floating: true,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            //min: 0,
            title: {
                text: ''
            }
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: <b>{point.y:.1f}%</b> '//'{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800,
        },
        plotOptions: {
            column: {
                depth: 25
            },
            series: {
                dataLabels: {
                    enabled: true,
                    format: '{point.y:.1f} %',
                    rotation: -90,
                    //color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
            //series: {
            //    dataLabels: {
            //        format: '{point.y:.1f} %',
            //        align: 'top',
            //        enabled: true,
            //        floating: true,
            //        crop: false,
            //        overflow: 'none',
            //        rotation: 360,
            //        x: -12,
            //        y: -3,
            //        style: {
            //            fontWeight: 'normal',
            //            y: -5
            //        }
            //    }
            //}
        },
        series: [{
            name: twoDimDataArray[0].name,
            data: twoDimDataArray[0].data,//[34.7, 34.8, 34.6,36.4,40.0],//twoDimDataArray[0].data,
            color: '#FFFF00'
        },
        {
            name: twoDimDataArray[2].name,
            data: twoDimDataArray[2].data,//[34.7, 34.8, 34.6,36.4,40.0],//twoDimDataArray[0].data,
            color: '#48CCCD'
        },
        {
            name: twoDimDataArray[1].name,
            data: twoDimDataArray[1].data,//[10.7, 10.8, 10.6, 10.4, 10.0],//twoDimDataArray[1].data,
            color: '#7FE817'
        }]
    });
}
function BuildThreeSeriseGraphWithOutDecimalLabelBillscrubber(container, twoDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {
    /// <summary>
    /// Shows the two bars chart with labels on bars.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="twoDimDataArray">The two dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            floating: true,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            //min: 0,
            title: {
                text: ''
            }
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: <b>{point.y}%</b> '//'{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800,
        },
        plotOptions: {
            column: {
                depth: 25
            },
            series: {
                dataLabels: {
                    /*enabled: true,
                    format: '{point.y} %',
                    rotation: -90,
                    color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }*/
                    align: 'top',
                    enabled: true,
                    floating: true,
                    crop: false,
                    format: '{point.y}%',
                    overflow: 'none',
                    rotation: 360,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
            //series: {
            //    dataLabels: {
            //        format: '{point.y:.1f} %',
            //        align: 'top',
            //        enabled: true,
            //        floating: true,
            //        crop: false,
            //        overflow: 'none',
            //        rotation: 360,
            //        x: -12,
            //        y: -3,
            //        style: {
            //            fontWeight: 'normal',
            //            y: -5
            //        }
            //    }
            //}
        },
        series: [{
            name: twoDimDataArray[0].name,
            data: twoDimDataArray[0].data,//[34.7, 34.8, 34.6,36.4,40.0],//twoDimDataArray[0].data,
            color: '#FFFF00'
        },
        {
            name: twoDimDataArray[2].name,
            data: twoDimDataArray[2].data,//[34.7, 34.8, 34.6,36.4,40.0],//twoDimDataArray[0].data,
            color: '#48CCCD'
        },
        {
            name: twoDimDataArray[1].name,
            data: twoDimDataArray[1].data,//[10.7, 10.8, 10.6, 10.4, 10.0],//twoDimDataArray[1].data,
            color: '#7FE817'
        }]
    });
}
function BuildThreeSeriseGraphWithLevelBillScrubber(container, threeDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray, yaxisTitle) {
    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            allowDecimals: false,
            //min: 0,
            title: {
                text: yaxisTitle
            }
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            //headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y} '
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    align: 'top',
                    enabled: true,
                    floating: true,
                    crop: false,
                    //overflow: 'none',
                    rotation: 360,
                    style: {
                        fontWeight: 'normal',
                        y: -5
                    }
                }
            }
        },
        series: [{
            name: threeDimDataArray[0].name,
            data: threeDimDataArray[0].data,
            color: '#FFFF00',
        },
        {
            name: threeDimDataArray[1].name,
            data: threeDimDataArray[1].data,
            color: '#48CCCD',
        },
        {
            name: threeDimDataArray[2].name,
            data: threeDimDataArray[2].data,
            color: '#7FE817'
        }]
    });
}
function GraphsBuilderCustomSubTitleWithoutPercentageBillScrubber(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle) {
    var month = $('#ddlMonth').val();
    var dataArray = new Array();
    var categories = new Array();

    var monthsArray = new Array();
    for (var i = 0; i < 3; i++) {
        monthsArray = new Array();
        switch (parseInt(month)) {
            case 1:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                categories = ['Jan'];
                break;
            case 2:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                categories = ['Jan', 'Feb'];
                break;
            case 3:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                monthsArray.push(parseFloat(dashboardData[i].M3));
                categories = ['Jan', 'Feb', 'Mar'];
                break;
            case 4:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                monthsArray.push(parseFloat(dashboardData[i].M3));
                monthsArray.push(parseFloat(dashboardData[i].M4));
                categories = ['Jan', 'Feb', 'Mar', 'Apr'];
                break;
            case 5:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                monthsArray.push(parseFloat(dashboardData[i].M3));
                monthsArray.push(parseFloat(dashboardData[i].M4));
                monthsArray.push(parseFloat(dashboardData[i].M5));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May'];
                break;
            case 6:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                monthsArray.push(parseFloat(dashboardData[i].M3));
                monthsArray.push(parseFloat(dashboardData[i].M4));
                monthsArray.push(parseFloat(dashboardData[i].M5));
                monthsArray.push(parseFloat(dashboardData[i].M6));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
                break;
            case 7:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                monthsArray.push(parseFloat(dashboardData[i].M3));
                monthsArray.push(parseFloat(dashboardData[i].M4));
                monthsArray.push(parseFloat(dashboardData[i].M5));
                monthsArray.push(parseFloat(dashboardData[i].M6));
                monthsArray.push(parseFloat(dashboardData[i].M7));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'];
                break;
            case 8:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                monthsArray.push(parseFloat(dashboardData[i].M3));
                monthsArray.push(parseFloat(dashboardData[i].M4));
                monthsArray.push(parseFloat(dashboardData[i].M5));
                monthsArray.push(parseFloat(dashboardData[i].M6));
                monthsArray.push(parseFloat(dashboardData[i].M7));
                monthsArray.push(parseFloat(dashboardData[i].M8));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug'];
                break;
            case 9:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                monthsArray.push(parseFloat(dashboardData[i].M3));
                monthsArray.push(parseFloat(dashboardData[i].M4));
                monthsArray.push(parseFloat(dashboardData[i].M5));
                monthsArray.push(parseFloat(dashboardData[i].M6));
                monthsArray.push(parseFloat(dashboardData[i].M7));
                monthsArray.push(parseFloat(dashboardData[i].M8));
                monthsArray.push(parseFloat(dashboardData[i].M9));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'];
                break;
            case 10:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                monthsArray.push(parseFloat(dashboardData[i].M3));
                monthsArray.push(parseFloat(dashboardData[i].M4));
                monthsArray.push(parseFloat(dashboardData[i].M5));
                monthsArray.push(parseFloat(dashboardData[i].M6));
                monthsArray.push(parseFloat(dashboardData[i].M7));
                monthsArray.push(parseFloat(dashboardData[i].M8));
                monthsArray.push(parseFloat(dashboardData[i].M9));
                monthsArray.push(parseFloat(dashboardData[i].M10));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'];

                break;
            case 11:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                monthsArray.push(parseFloat(dashboardData[i].M3));
                monthsArray.push(parseFloat(dashboardData[i].M4));
                monthsArray.push(parseFloat(dashboardData[i].M5));
                monthsArray.push(parseFloat(dashboardData[i].M6));
                monthsArray.push(parseFloat(dashboardData[i].M7));
                monthsArray.push(parseFloat(dashboardData[i].M8));
                monthsArray.push(parseFloat(dashboardData[i].M9));
                monthsArray.push(parseFloat(dashboardData[i].M10));
                monthsArray.push(parseFloat(dashboardData[i].M11));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov'];

                break;
            default:
                monthsArray.push(parseFloat(dashboardData[i].M1));
                monthsArray.push(parseFloat(dashboardData[i].M2));
                monthsArray.push(parseFloat(dashboardData[i].M3));
                monthsArray.push(parseFloat(dashboardData[i].M4));
                monthsArray.push(parseFloat(dashboardData[i].M5));
                monthsArray.push(parseFloat(dashboardData[i].M6));
                monthsArray.push(parseFloat(dashboardData[i].M7));
                monthsArray.push(parseFloat(dashboardData[i].M8));
                monthsArray.push(parseFloat(dashboardData[i].M9));
                monthsArray.push(parseFloat(dashboardData[i].M10));
                monthsArray.push(parseFloat(dashboardData[i].M11));
                monthsArray.push(parseFloat(dashboardData[i].M12));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                break;
        }
        //var dashbaordName = i == 0 ? "Prior Year" : i == 1 ? "Actual" : "Target";
        //dataArray.push({ 'name': dashbaordName, 'data': monthsArray });

        var currentYear = new Date().getFullYear();
        var dashboardName = dashboardData[i].BudgetType == 2 && dashboardData[i].Year == currentYear - 1 ? "Prior Year" : (dashboardData[i].BudgetType == 2 ? "Actual" : "Target");
        dataArray.push({ 'name': dashboardName, 'data': monthsArray });
    }
    if (chartLegendPosition == 1)
        BuildThreeSeriseBarGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, subtitle, categories);
    else if (chartLegendPosition == 2)
        BuildThreeSeriseGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, subtitle, categories);
    else if (chartLegendPosition == 3)
        BuildThreeSeriseBarGraphWithOutDecimalsBillScrubber(containerid, dataArray, charttype, chartName, subtitle, categories);
}
function EmptyGraphsBuilderWithoutPercentageSubtitleBillScrubber(containerid, charttype, chartName, chartLegendPosition, subtitle) {
    var month = $('#ddlMonth').val();
    var dataArray = new Array();
    var categories = new Array();

    var monthsArray = new Array();
    for (var i = 0; i < 3; i++) {
        monthsArray = new Array();
        switch (parseInt(month)) {
            case 1:
                monthsArray.push(parseFloat(0));
                categories = ['Jan'];
                break;
            case 2:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb'];
                break;
            case 3:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar'];
                break;
            case 4:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr'];
                break;
            case 5:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May'];
                break;
            case 6:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
                break;
            case 7:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'];
                break;
            case 8:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug'];
                break;
            case 9:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'];
                break;
            case 10:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'];

                break;
            case 11:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov'];

                break;
            default:
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                monthsArray.push(parseFloat(0));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                break;
        }
        var dashbaordName = i == 0 ? "Prior Year" : i == 1 ? "Actual" : "Target";
        dataArray.push({ 'name': dashbaordName, 'data': monthsArray });
    }
    if (chartLegendPosition == 1)
        BuildThreeSeriseBarGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, subtitle, categories);
    else
        BuildThreeSeriseGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, subtitle, categories);
}
function GraphsBuilderWith100TargetBillScrubber(dashboardData, containerid, charttype, chartName, chartLegendPosition) {
    var month = $('#ddlMonth').val();
    var dataArray = new Array();
    var categories = new Array();
    var monthsArray = new Array();
    for (var i = 0; i < 3; i++) {
        monthsArray = new Array();
        switch (parseInt(month)) {
            case 1:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                //monthsArray.push(parseFloat(Math.round(dashboardData[i].M1 * 100) / 100) * 100);
                categories = ['Jan'];
                break;
            case 2:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                categories = ['Jan', 'Feb'];
                break;
            case 3:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M3) * 100);
                categories = ['Jan', 'Feb', 'Mar'];
                break;
            case 4:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M3) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M4) * 100);
                categories = ['Jan', 'Feb', 'Mar', 'Apr'];
                break;
            case 5:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M3) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M4) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M5) * 100);
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May'];
                break;
            case 6:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M3) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M4) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M5) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M6) * 100);
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
                break;
            case 7:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M3) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M4) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M5) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M6) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M7) * 100);
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'];
                break;
            case 8:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M3) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M4) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M5) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M6) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M7) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M8) * 100);
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug'];
                break;
            case 9:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M3) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M4) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M5) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M6) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M7) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M8) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M9) * 100);
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'];
                break;
            case 10:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M3) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M4) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M5) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M6) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M7) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M8) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M9) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M10) * 100);
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'];

                break;
            case 11:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M3) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M4) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M5) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M6) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M7) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M8) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M9) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M10) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M11) * 100);
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov'];

                break;
            default:
                monthsArray.push(parseFloat(dashboardData[i].M1) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M2) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M3) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M4) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M5) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M6) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M7) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M8) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M9) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M10) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M11) * 100);
                monthsArray.push(parseFloat(dashboardData[i].M12) * 100);
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                break;
        }
        //var dashbaordName = i == 0 ? "Prior Year" : i == 1 ? "Actual" : "Target";
        //dataArray.push({ 'name': dashbaordName, 'data': monthsArray });

        var currentYear = new Date().getFullYear();
        var dashboardName = dashboardData[i].BudgetType == 2 && dashboardData[i].Year == currentYear - 1 ? "Prior Year" : (dashboardData[i].BudgetType == 2 ? "Actual" : "Target");
        dataArray.push({ 'name': dashboardName, 'data': monthsArray });
    }
    if (chartLegendPosition == 1)
        BuildThreeSeriseGraphWithLegendsTooltipPercentage(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 2)
        BuildThreeSeriesBarGraphWithLegendsTooltipPercentageCustom(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 3)
        BuildThreeSeriseGraphCustomTargetColorBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 4)
        BuildThreeSeriseGraphCustomTargetColorLabel(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 5)
        BuildThreeSeriseGraphWithOutDecimalLabel(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 6)
        BuildThreeSeriseGraphWithLevel(containerid, dataArray, charttype, chartName, "Total Emiratis", categories);
}
function BuildThreeSeriseGraphCustomTargetColorBillScrubber(container, threeDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray, yaxisTitle) {
    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            allowDecimals: false,
            //min: 0,
            title: {
                text: yaxisTitle
            }
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: <b>{point.y:.1f}%</b> '//'{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    enabled: true,
                    format: '{point.y:.1f}%',
                    rotation: -90,
                    //color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
        },
        series: [{
            name: threeDimDataArray[0].name,
            data: threeDimDataArray[0].data,
            color: '#FFFF00',
        },
        {
            name: threeDimDataArray[2].name,
            data: threeDimDataArray[2].data,
            color: '#48CCCD',
        },
        {
            name: threeDimDataArray[1].name,
            data: threeDimDataArray[1].data,
            color: '#7FE817'
        }]
    });
}
function GraphsBuilderWithoutPercentageBillScrubber45Angle(dashboardData, containerid, charttype, chartName, chartLegendPosition) {
    var month = $('#ddlMonth').val();
    var dataArray = new Array();
    var categories = new Array();

    var monthsArray = new Array();
    for (var i = 0; i < 3; i++) {
        monthsArray = new Array();
        switch (parseInt(month)) {
            case 1:
                monthsArray.push(parseInt(dashboardData[i].M1));
                categories = ['Jan'];
                break;
            case 2:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                categories = ['Jan', 'Feb'];
                break;
            case 3:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                categories = ['Jan', 'Feb', 'Mar'];
                break;
            case 4:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                categories = ['Jan', 'Feb', 'Mar', 'Apr'];
                break;
            case 5:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May'];
                break;
            case 6:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
                break;
            case 7:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'];
                break;
            case 8:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                monthsArray.push(parseInt(dashboardData[i].M8));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug'];
                break;
            case 9:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                monthsArray.push(parseInt(dashboardData[i].M8));
                monthsArray.push(parseInt(dashboardData[i].M9));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'];
                break;
            case 10:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                monthsArray.push(parseInt(dashboardData[i].M8));
                monthsArray.push(parseInt(dashboardData[i].M9));
                monthsArray.push(parseInt(dashboardData[i].M10));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'];

                break;
            case 11:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                monthsArray.push(parseInt(dashboardData[i].M8));
                monthsArray.push(parseInt(dashboardData[i].M9));
                monthsArray.push(parseInt(dashboardData[i].M10));
                monthsArray.push(parseInt(dashboardData[i].M11));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov'];

                break;
            default:
                monthsArray.push(parseInt(dashboardData[i].M1));
                monthsArray.push(parseInt(dashboardData[i].M2));
                monthsArray.push(parseInt(dashboardData[i].M3));
                monthsArray.push(parseInt(dashboardData[i].M4));
                monthsArray.push(parseInt(dashboardData[i].M5));
                monthsArray.push(parseInt(dashboardData[i].M6));
                monthsArray.push(parseInt(dashboardData[i].M7));
                monthsArray.push(parseInt(dashboardData[i].M8));
                monthsArray.push(parseInt(dashboardData[i].M9));
                monthsArray.push(parseInt(dashboardData[i].M10));
                monthsArray.push(parseInt(dashboardData[i].M11));
                monthsArray.push(parseInt(dashboardData[i].M12));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                break;
        }
        //var dashbaordName = i == 0 ? "Prior Year" : i == 1 ? "Actual" : "Budget";
        //dataArray.push({ 'name': dashbaordName, 'data': monthsArray });

        var currentYear = new Date().getFullYear();
        var dashboardName = dashboardData[i].BudgetType == 2 && dashboardData[i].Year == currentYear - 1 ? "Prior Year" : (dashboardData[i].BudgetType == 2 ? "Actual" : "Target");
        dataArray.push({ 'name': dashboardName, 'data': monthsArray });
    }
    if (chartLegendPosition == 1)
        BuildThreeSeriseBarGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 2)
        BuildThreeSeriseBarGraphWithLegendsTooltipBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 3)
        BuildThreeSeriseBarGraphWithOutDecimalsBillScrubber45Angle(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 4)
        BuildThreeSeriseGraphWithLegendsTooltipPercentageBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 5)
        BuildThreeSeriseGraphWithOutDecimalLabelBillscrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 6)
        BuildThreeSeriseGraphWithLevelBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
    else if (chartLegendPosition == 7)
        BuildThreeSeriseBarGraphWithOutDecimalsBillScrubber(containerid, dataArray, charttype, chartName, "By Month (000s)", categories);
}
/*new methods - end*/
function SubCategoryPieChartBuilderYearToDateBillScrubber(dashboardData, containerid, charttype, chartName, chartFormattype) {
    var swbChartDataMonthly = new Array();
    if (dashboardData != null) {
        for (var i = 0; i < dashboardData.length; i++) {
            var value = parseFloat(dashboardData[i].CYTA * 100);
            if (value > 0) {
                var name = dashboardData[i].DashBoard.replace('% Revenue Collected By Denial Code', '');
                swbChartDataMonthly.push({ 'name': name, 'y': value });
            }
        }
        ShowOnePieChartWithColorsWithPercentageOneDecimalBillScrubberYtd(containerid, swbChartDataMonthly, chartName, 'Year To Date');
    }
}

function BuildBarGraphsRevenueByCategoryBillScrubber(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle) {
    var dataArray = new Array();
    var categories = new Array();
    var budgets = new Array();
    var actuals = new Array();

    for (var i = 0; i < dashboardData.length; i++) {
        budgets.push(dashboardData[i].CYTB);
        actuals.push(dashboardData[i].CYTA);
        var name = dashboardData[i].DashBoard.replace("Revenue Collected by Denial Code", "");
        categories.push(name);
    }
    dataArray.push({ 'name': "Actual", 'data': actuals });
    dataArray.push({ 'name': "Budget", 'data': budgets });

    BuildTwoBarsChartWithLabelsOnBarsBillScrubber(containerid, dataArray, charttype, chartName, subtitle, categories);
}

function BuildTwoBarsChartWithLabelsOnBarsBillScrubber(container, twoDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {
    /// <summary>
    /// Shows the two bars chart with labels on bars.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="twoDimDataArray">The two dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            floating: true,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            } //['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            min: 0,
            title: {
                text: ''
            }
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y}'
        },
        exporting: {
            filename: widgetTitle,
            sourceWidth: 1000,
            sourceHeight: 800,
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    rotation: 270,
                    //format: '{y}',
                    format: '{point.y:,.0f}',
                    align: 'top',
                    enabled: true,
                    floating: true,
                    style: {
                        fontWeight: 'normal',
                        y: -5
                    }
                }
            }
        },
        series: [
            {
                name: twoDimDataArray[1].name,
                data: twoDimDataArray[1].data, //[10.7, 10.8, 10.6, 10.4, 10.0],//twoDimDataArray[1].data,
                color: '#48CCCD'
            }, {
                name: twoDimDataArray[0].name,
                data: twoDimDataArray[0].data, //[34.7, 34.8, 34.6,36.4,40.0],//twoDimDataArray[0].data,
                color: '#7FE817'
            }
        ]
    });
}

function BuildCustomGraphWithoutDecimalsBillScrubber(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle, ytd) {
    var chartData = new Array();
    var categories = new Array();
    for (var i = 0; i < dashboardData.length; i++) {
        var name = dashboardData[i].DashBoard.replace('Denials by Reason for Denial', '');
        if (ytd == 0) {
            chartData.push({ 'name': name, 'data': [parseFloat(dashboardData[i].CMA)] });
        } else {
            chartData.push({ 'name': name, 'data': [parseFloat(dashboardData[i].CYTA)] });
        }
        categories.push(name);
    }
    BuildBarGraphsWithoutDecimalsBillScrubber(containerid, chartData, charttype, chartName, subtitle, categories);
}

function BuildBarGraphsWithoutDecimalsBillScrubber(container, dataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {

    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            labels: {
                enabled: false
            }
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            min: 0,
            title: {
                text: ''
            }
        },
        exporting: {
            filename: widgetTitle,
            sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            //headerFormat: '<b>{point.key}</b><br>',
            //pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y:,.2f}'
            formatter: function () {
                var toolTip = '<span style="color:' + this.series.color + '">\u25CF</span> ' + this.series.name + ':' + this.point.y + '';
                return toolTip;
            }
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    enabled: true,
                    format: '{point.y}',
                    rotation: -90,
                    //color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
        },
        series: dataArray
    });
}
function BuildBarGraphsWithoutDecimalsBillScrubberYearToDate(container, dataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {

    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            labels: {
                enabled: false
            }
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            min: 0,
            title: {
                text: ''
            }
        },
        exporting: {
            filename: widgetTitle,
            sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            //headerFormat: '<b>{point.key}</b><br>',
            //pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y:,.2f}'
            formatter: function () {
                var toolTip = '<span style="color:' + this.series.color + '">\u25CF</span> ' + this.series.name + ':' + Highcharts.numberFormat(this.point.y, 0, '.', ',') + '';
                return toolTip;
            }
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    enabled: true,
                    format: '{point.y:,.0f}',
                    rotation: -90,
                    //color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
        },
        series: dataArray
    });
}
function BilliableLineGraphBillScrubber(dashboardData, containerid, charttype, chartName) {
    var month = $('#ddlMonth').val();
    var dataArray = new Array();
    var categories = new Array();
    var monthsArray = new Array();
    for (var i = 0; i < 3; i++) {
        monthsArray = new Array();
        switch (parseInt(month)) {
            case 1:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                categories = ['Jan'];
                break;
            case 2:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                categories = ['Jan', 'Feb'];
                break;
            case 3:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M3 * 100));
                categories = ['Jan', 'Feb', 'Mar'];
                break;
            case 4:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M3 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M4 * 100));
                categories = ['Jan', 'Feb', 'Mar', 'Apr'];
                break;
            case 5:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M3 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M4 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M5 * 100));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May'];
                break;
            case 6:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M3 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M4 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M5 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M6 * 100));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
                break;
            case 7:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M3 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M4 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M5 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M6 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M7 * 100));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'];
                break;
            case 8:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M3 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M4 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M5 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M6 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M7 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M8 * 100));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug'];
                break;
            case 9:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M3 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M4 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M5 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M6 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M7 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M8 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M9 * 100));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'];
                break;
            case 10:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M3 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M4 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M5 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M6 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M7 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M8 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M9 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M10 * 100));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'];
                break;
            case 11:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M3 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M4 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M5 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M6 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M7 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M8 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M9 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M10 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M11 * 100));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov'];

                break;
            default:
                monthsArray.push(parseFloat(dashboardData[i].M1 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M2 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M3 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M4 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M5 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M6 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M7 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M8 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M9 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M10 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M11 * 100));
                monthsArray.push(parseFloat(dashboardData[i].M12 * 100));
                categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                break;
        }
        //var dashbaordName = i == 0 ? "Prior Year" : i == 1 ? "Actual" : "Target";
        //dataArray.push({ 'name': dashbaordName, 'data': monthsArray });

        var currentYear = new Date().getFullYear();
        var dashboardName = dashboardData[i].BudgetType == 2 && dashboardData[i].Year == currentYear - 1 ? "Prior Year" : (dashboardData[i].BudgetType == 2 ? "Actual" : "Target");
        dataArray.push({ 'name': dashboardName, 'data': monthsArray });
    }
    BuildBillableLineGraphBillScrubber(containerid, dataArray, charttype, chartName, "By Month", categories);
}

function BuildBillableLineGraphBillScrubber(container, twoDimDataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {
    /// <summary>
    /// Shows the two bars chart with labels on bars.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="twoDimDataArray">The two dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            categories: categoriesArray,
            floating: true,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif'
            }//['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            //min: 0,
            title: {
                text: this.yAxisTitle,
                align: 'low'
            }
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: <b>{point.y:,.1f}%</b> '//'{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        exporting: {
            filename: widgetTitle, sourceWidth: 1000,
            sourceHeight: 800,
        },
        plotOptions: {
            column: {
                depth: 25
            },
            series: {
                dataLabels: {
                    enabled: true,
                    format: '{point.y:,.1f} %',
                    rotation: -45,
                    //color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
            //series: {
            //    dataLabels: {
            //        format: '{point.y:.1f} %',
            //        align: 'top',
            //        enabled: true,
            //        floating: true,
            //        crop: false,
            //        overflow: 'none',
            //        rotation: 360,
            //        x: -12,
            //        y: -3,
            //        style: {
            //            fontWeight: 'normal',
            //            y: -5
            //        }
            //    }
            //}
        },
        series: [{
            name: twoDimDataArray[0].name,
            data: twoDimDataArray[0].data,//[34.7, 34.8, 34.6,36.4,40.0],//twoDimDataArray[0].data,
            color: '#FFFF00'/*,
            dataLabels: {
                enabled: true,
                rotation: -45,
                color: '#00000',
                align: 'right',
                y: 10, // 10 pixels down from the top
                x: 10,
                style: {
                    fontSize: '10px',
                    fontFamily: 'Verdana, sans-serif'
                }
            }*/
        },
        {
            name: twoDimDataArray[2].name,
            data: twoDimDataArray[2].data,//[34.7, 34.8, 34.6,36.4,40.0],//twoDimDataArray[0].data,
            color: '#48CCCD'/*,
            dataLabels: {
                enabled: true,
                rotation: -45,
                color: '#00000',
                align: 'right',
                y: -15, // 10 pixels down from the top
                x: -15,
                style: {
                    fontSize: '10px',
                    fontFamily: 'Verdana, sans-serif'
                }
            }*/
        },
        {
            name: twoDimDataArray[1].name,
            data: twoDimDataArray[1].data,//[10.7, 10.8, 10.6, 10.4, 10.0],//twoDimDataArray[1].data,
            color: '#7FE817'/*,
            dataLabels: {
                enabled: true,
                rotation: -45,
                color: '#00000',
                align: 'right',
                y: 10, // 10 pixels down from the top
                x: 10,
                style: {
                    fontSize: '9px',
                    fontWeight: 'normal',
                    fontFamily: 'Verdana, sans-serif'
                }
            }*/
        }]
    });
}


function BuildCustomGraphWithoutDecimalsBillScrubberDenials(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle, ytd) {
    var chartData = new Array();
    var categories = new Array();
    for (var i = 0; i < dashboardData.length; i++) {
        var name = dashboardData[i].DashBoard.replace('Denials by Reason for Denial', '');
        if (ytd == 0) {
            chartData.push({ 'name': name, 'data': [parseFloat(dashboardData[i].CMA)] });
        } else {
            chartData.push({ 'name': name, 'data': [parseFloat(dashboardData[i].CYTA)] });
        }
        categories.push(name);
    }
    BuildBarGraphsWithoutDecimalsBillScrubber(containerid, chartData, charttype, chartName, subtitle, categories);
}
function BuildCustomGraphWithoutDecimalsBillScrubberDenialsYearToDate(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle, ytd) {
    var chartData = new Array();
    var categories = new Array();
    for (var i = 0; i < dashboardData.length; i++) {
        var name = dashboardData[i].DashBoard.replace('Denials by Reason for Denial', '');
        if (ytd == 0) {
            chartData.push({ 'name': name, 'data': [parseFloat(dashboardData[i].CMA)] });
        } else {
            chartData.push({ 'name': name, 'data': [parseFloat(dashboardData[i].CYTA)] });
        }
        categories.push(name);
    }
    BuildBarGraphsWithoutDecimalsBillScrubberYearToDate(containerid, chartData, charttype, chartName, subtitle, categories);
}

function BuildCustomGraphWithoutDecimalsBillScrubberDollarClaims(dashboardData, containerid, charttype, chartName, chartLegendPosition, subtitle, ytd) {
    var chartData = new Array();
    var categories = new Array();
    for (var i = 0; i < dashboardData.length; i++) {
        var name = dashboardData[i].DashBoard.replace('Dollar Claims Resubmitted by Denial', '');
        if (ytd == 0) {
            chartData.push({ 'name': name, 'data': [parseFloat(dashboardData[i].CMA)] });
        } else {
            chartData.push({ 'name': name, 'data': [parseFloat(dashboardData[i].CYTA)] });
        }
        categories.push(name);
    }
    BuildBarGraphsWithoutDecimalsBillScrubberDollarClaims(containerid, chartData, charttype, chartName, subtitle, categories);
}
function BuildBarGraphsWithoutDecimalsBillScrubberDollarClaims(container, dataArray, widgetType, widgetTitle, widgetSubTitle, categoriesArray) {

    /// <summary>
    /// Shows the three bars chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="threeDimDataArray">The three dim data array.</param>
    /// <param name="widgetType">Type of the widget.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <param name="widgetSubTitle">The widget sub title.</param>
    /// <param name="categoriesArray">The categories array.</param>
    /// <param name="yaxisTitle">The yaxis title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: widgetType,
            options3d: {
                enabled: true,
                alpha: 10,
                beta: 10,
                viewDistance: 25,
                depth: 40
            },
            marginTop: 80,
            marginRight: 40
        },

        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: widgetSubTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        xAxis: {
            labels: {
                enabled: false
            }
        },
        yAxis: {
            floating: true,
            allowDecimals: false,
            min: 0,
            title: {
                text: ''
            }
        },
        exporting: {
            filename: widgetTitle,
            sourceWidth: 1000,
            sourceHeight: 800
        },
        legend: {
            align: 'left',
            verticalAlign: 'bottom',
            itemWidth: 250,
            padding: 3,
            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            }
        },
        tooltip: {
            //headerFormat: '<b>{ }</b><br>',
            //pointFormat: '<span style="color:{series.color}">\u25CF</span> {series.name}: {point.y:,.0f}'
            formatter: function () {
                var toolTip = '<span style="color:' + this.series.color + '">\u25CF</span> ' + this.series.name + ':' + Highcharts.numberFormat(this.point.y, 0, '.', ',') + '';
                return toolTip;
            }
        },
        plotOptions: {
            column: {
                depth: 40
            },
            series: {
                dataLabels: {
                    enabled: true,
                    format: '{point.y:,.0f}',
                    rotation: -90,
                    //color: '#00000',
                    align: 'top',
                    inside: false,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'normal',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }
            }
        },
        series: dataArray
    });
}




function ShowOnePieChartWithColorsWithPercentageOneDecimalBillScrubberYtd(container, data, widgetTitle, subtitleval) {
    /// <summary>
    /// Shows the one pie chart with colors.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="data">The data.</param>
    /// <param name="widgetTitle">The widget title.</param>
    /// <returns></returns>
    $("#" + container).highcharts({
        chart: {
            type: 'pie',
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            }
        },
        title: {
            text: widgetTitle,
            style: {
                fontSize: '13px',
                fontFamily: 'Verdana, sans-serif',
                fontWeight: 'normal'
            }
        },
        subtitle: {
            text: subtitleval
        },
        tooltip: {
            //pointFormat: '<b>{point.name}</b>: {point.y:.1f} %.'//'{series.name}: <b>{point.name:.1f}</b>'//'<b>{point.name}</b>: {point.percentage:.1f} %','<b>{point.name}</b>: {point.y:.1f} Rs.',
            formatter: function () {
                var toolTip = '<span style="color:' + this.series.color + '">\u25CF</span> ' + this.point.name + ':' + this.point.y + '';
                return toolTip;
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                depth: 35,
                dataLabels: {
                    //enabled: true,
                    //format: '<b>{point.name}</b>: {point.y:.1f}  %',
                    //style: {
                    //    color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    //}
                    useHTML: true,
                    formatter: function () {
                        var point = this.point,
                            width = 50,
                            height = 50,
                            series = point.series,
                            center = series.center,               //center of the pie
                            startX = point.labelPos[4],           //connector X starts here
                            endX = point.labelPos[0] + 5,         //connector X ends here
                            left = -(endX - startX + width / 2), //center label over right edge
                            startY = point.labelPos[5],           //connector Y starts here
                            endY = point.labelPos[1] - 5,         //connector Y ends here
                            top = -(endY - startY + height / 2); //center label over right edge


                        // now move inside the slice:
                        left += (center[0] - startX) / 2;
                        top += (center[1] - startY) / 2;

                        return point.name + ': ' + '<b>' + point.y + ' %' + '<b>';
                    }
                }
            }
        },
        exporting: {
            filename: widgetTitle,
            sourceWidth: 1000,
            sourceHeight: 800
        },
        series: [{
            type: 'pie',
            name: 'Percentage',
            data: data,
        }]
    });
}
