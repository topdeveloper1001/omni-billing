﻿$(function () {


    BindPhysicianDropdownData();
    BindFacilityDropdown();

    BindGlobalCodesWithValue("#ddlTransactionType", 4905, "");
    BindInsuranceCompaniesData("#ddlPersonInsuranceCompany");
    var datenow = new Date();
    var reportingType = $("#hdReportType").val();
    if (reportingType == 6 || reportingType == 7 || reportingType == 8) {
        $('#txtFromDate').val(datenow.format('mm/dd/yyyy'));
    }
    if (reportingType == 19 || reportingType == 20) {
        BindAccountsDropdown();
    }
    InitializeExportPanel();
    InitializeDatesForReporting();
    $("#ReportingGridDetailDiv").hide();
    var userId = $("#UserId");
    if (userId.length > 0) {
        userId.val(0);
        BindUsers();
    }

    var corporateId = $("#CorporateId").val();
    if (reportingType == 1 || reportingType == 2 || reportingType == 3) {
        $("#ShowAllRecords").prop("checked", true);
    }
    else if ((reportingType == 2 || reportingType == 3) && corporateId != 0) {
        $("#divShowAll").hide();
    }

    BindReportingData();
    $('#btnExportExcel').click(function () {
        var item = $(this);
        var hrefString = item.attr("href");
        var controllerAction = hrefString.split('?')[0];
        //var hdReportViewType = $("#hdReportViewType").val();
        var parametersArray = hrefString.split('?')[1].split('&');
        var hrefNew = controllerAction + "?fromDate=" + $('#txtFromDate').val();
        var reportType = $('#hdReportType').val();
        var tilldateval = parametersArray[0].indexOf('tillDate') != 0 ? parametersArray[0].split('=')[1] : ConvertJSDateTimeToServerString(new Date());
        switch (reportType) {
            case "1":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&userId= ' + $('#ddlUsers').val();
                break;
            case "2":
            case "3":
            case "4":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&userId= ' + $('#ddlDepartment').val() + '&viewtype= ' + $('#ddlPersonInsuranceCompany').val();
                break;
            case "5":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val();
                break;
            case "6":
            case "7":
            case "8":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val();
                break;
            case "9":
            case "10":
            case "11":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#hdReportViewType').val();
                break;
            case "13":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#ddlDisplayBy').val();
                break;
            case "14":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#ddlDisplayDenialBy').val();
                break;
            case "15":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=1';
                break;
            case "19":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&userId= ' + $('#ddlDepartment').val() + '&viewtype= ' + $('#ddlPersonInsuranceCompany').val();
                break;
            case "20":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&userId= ' + $('#ddlDepartment').val() + '&viewtype= ' + $('#ddlPersonInsuranceCompany').val();
                break;
            case "21":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&userId= ' + $('#ddlfacility').val() + '&viewtype= ' + $('#ddlPhysician').val();
                break;
            case "25":
            case "26":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#ddlPhysician').val();
                break;
            case "27":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#ddlDepartment').val();
                break;
            default:
                hrefNew += '&tillDate=' + tilldateval + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#hdReportViewType').val();
        }

        //if (hrefString.indexOf("viewtype=Y") != -1)
        //    hrefString=  hrefString.replace("viewtype=Y", "viewtype=" + hdReportViewType);
        //else if (hrefString.indexOf("viewtype=M") != -1)
        //    hrefString = hrefString.replace("viewtype=M", "viewtype=" + hdReportViewType);
        //else if (hrefString.indexOf("viewtype=W") != -1)
        //    hrefString = hrefString.replace("viewtype=W", "viewtype=" + hdReportViewType);

        item.removeAttr('href');
        item.attr('href', hrefNew);
        return true;
    });


    $('#btnExportPDF').click(function () {
        var item = $(this);
        var hrefString = item.attr("href");
        var controllerAction = hrefString.split('?')[0];
        //var hdReportViewType = $("#hdReportViewType").val();
        var parametersArray = hrefString.split('?')[1].split('&');
        var hrefNew = controllerAction + "?fromDate=" + $('#txtFromDate').val();
        var reportType = $('#hdReportType').val();
        var tilldateval = parametersArray[0].indexOf('tillDate') != 0 ? parametersArray[0].split('=')[1] : ConvertJSDateTimeToServerString(new Date());
        switch (reportType) {
            case "1":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&userId= ' + $('#ddlUsers').val();
                break;
            case "2":
            case "3":
            case "4":
            case "5":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val();
                break;
            case "6":
            case "7":
            case "8":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val();
                break;
            case "9":
            case "10":
            case "11":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#hdReportViewType').val();
                break;
            case "13":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#ddlDisplayBy').val();
                break;
            case "14":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#ddlDisplayDenialBy').val();
                break;
            case "15":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=1';
                break;
            case "19":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&userId= ' + $('#ddlDepartment').val() + '&viewtype= ' + $('#ddlPersonInsuranceCompany').val();
                break;
            case "20":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&userId= ' + $('#ddlDepartment').val() + '&viewtype= ' + $('#ddlPersonInsuranceCompany').val();
                break;
            case "21":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&userId= ' + $('#ddlfacility').val() + '&viewtype= ' + $('#ddlPhysician').val();
                break;
            case "22":
            case "23":
            case "24":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#ddlfacility').val();
                break;
            case "25":
            case "26":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#ddlPhysician').val();
                break;
            case "27":
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#ddlDepartment').val();
                break;
            default:
                hrefNew += '&tillDate=' + $('#txtTillDate').val() + '&isAll=' + false + '&reportingId=' + $('#hdReportType').val() + '&viewtype=' + $('#hdReportViewType').val();
        }

        //if (hrefString.indexOf("viewtype=Y") != -1)
        //    hrefString=  hrefString.replace("viewtype=Y", "viewtype=" + hdReportViewType);
        //else if (hrefString.indexOf("viewtype=M") != -1)
        //    hrefString = hrefString.replace("viewtype=M", "viewtype=" + hdReportViewType);
        //else if (hrefString.indexOf("viewtype=W") != -1)
        //    hrefString = hrefString.replace("viewtype=W", "viewtype=" + hdReportViewType);

        item.removeAttr('href');
        item.attr('href', hrefNew);
        return true;
    });
});

//function BindReportingData() {
//    $('#loader_event').show();
//    /// <summary>
//    /// Binds the reporting data.
//    /// </summary>
//    /// <returns></returns>
//    var reportingType = $("#hdReportType").val();
//    var facilityId = $('#ddlfacility').length > 0 && $('#ddlfacility').val() != null ? $('#ddlfacility').val() : 0;

//    if (reportingType == 6 || reportingType == 7 || reportingType == 8) {
//        var jsonDataAgeing = JSON.stringify({
//            reportingTypeId: reportingType,
//            date: $("#txtFromDate").val(),
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/AgeingReport',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonDataAgeing,
//            success: function (data) {
//                BindList("#ReportingGrid", data);
//                RowTotalColor();
//                //SetGridPaging('?', '?reportingTypeId=' + reportingTypeId + '&');
//                //GetListByReportingType
//                //UpdateSearchCriteria();
//                $('#loader_event').hide();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 9 || reportingType == 10 || reportingType == 11) {
//        var jsonDataReConcilation = JSON.stringify({
//            reportingTypeId: reportingType,
//            date: new Date($("#txtFromDate").val()),
//            viewtype: $('#ddlViewType').val()
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/ReconciliationReport',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonDataReConcilation,
//            success: function (data) {
//                BindList("#ReportingGrid", data);
//                //GetListByReportingType
//                //UpdateSearchCriteria();
//                $('#loader_event').hide();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 13) {
//        var jsonDataClaimDetails = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
//            displayby: $("#ddlDisplayBy").val()
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetListByReportingType',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonDataClaimDetails,
//            success: function (data) {
//                $('#loader_event').hide();
//                BindList("#ReportingGrid", data);

//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 14) {
//        var jsonDataDenialDetails1 = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
//            displayby: $("#ddlDisplayDenialBy").val()
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetListByReportingType',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonDataDenialDetails1,
//            success: function (data) {
//                BindList("#ReportingGrid", data);
//                $('#loader_event').hide();
//                var displayBy = $("#ddlDisplayDenialBy").val();
//                if (displayBy == '1') {
//                    hideColumnColorRow('3', 'DenialReportGrid');
//                    hideColumnColorRow('4', 'DenialReportGrid');
//                    hideColumnColorRow('5', 'DenialReportGrid');
//                }
//                else if (displayBy == '2') {
//                    hideColumnColorRow('4', 'DenialReportGrid');
//                    hideColumnColorRow('5', 'DenialReportGrid');
//                }
//                else if (displayBy == '3') {
//                    hideColumnColorRow('1', 'DenialReportGrid');
//                    hideColumnColorRow('4', 'DenialReportGrid');
//                    hideColumnColorRow('5', 'DenialReportGrid');
//                }
//                else if (displayBy == '4') {
//                    hideColumnColorRow('1', 'DenialReportGrid');
//                    hideColumnColorRow('3', 'DenialReportGrid');
//                }
//                else if (displayBy == '5') {
//                    hideColumnColorRow('1', 'DenialReportGrid');
//                }
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 15) {
//        var jsonDataDenialDetails2 = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
//            displayby: 1
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetListByReportingType',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonDataDenialDetails2,
//            success: function (data) {
//                $('#loader_event').hide();
//                BindList("#ReportingGrid", data);
//                $('#ReportingGridLoginActivityDetailDiv').hide();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 16) {
//        var jsonDataDenialDetails3 = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
//            displayby: 2
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetListByReportingType',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonDataDenialDetails3,
//            success: function (data) {
//                $('#loader_event').hide();
//                BindList("#ReportingGrid", data);
//                $('#ReportingGridLoginActivityDetailDiv').hide();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 17) {
//        var jsonDataDenialDetails4 = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
//            displayby: 3
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetListByReportingType',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonDataDenialDetails4,
//            success: function (data) {
//                $('#loader_event').hide();
//                BindList("#ReportingGrid", data);
//                $('#ReportingGridLoginActivityDetailDiv').hide();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 19 || reportingType == 20) {
//        var departmenttype = $('#ddlDepartment').val();
//        var payorId = $('#ddlPersonInsuranceCompany').val();
//        var jsonDataChargesReport = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            departmentNumber: departmenttype != null ? departmenttype : '0',
//            payorId: payorId == null ? 0 : payorId,
//        });

//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetChargesReportsData',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonDataChargesReport,
//            success: function (data) {
//                BindList("#ReportingGrid", data); //GetListByReportingType
//                //$('#ReportingGridLoginActivityDetailDiv').hide();
//                //UpdateSearchCriteria();
//                $('#loader_event').hide();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 21 || reportingType == 26) {
//        var physician = $('#ddlPhysician').val();
//        var jsonPhysicianActivityReport = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            facilityId: facilityId,
//            physicianId: physician == null ? 0 : physician


//        });

//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetPhysicianActivityReport',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonPhysicianActivityReport,
//            success: function (data) {
//                $('#loader_event').hide();
//                
//                BindList("#ReportingGrid", data); //GetListByReportingType
//                //$('#ReportingGridLoginActivityDetailDiv').hide();
//                //UpdateSearchCriteria();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 22) {
//        var jsonReportSrubSummaryData = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            facilityId: facilityId == null ? 0 : facilityId,
//        });

//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetScrubberAndErrorSummaryReport',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonReportSrubSummaryData,
//            success: function (data) {
//                
//                BindList("#ReportingGrid", data); //GetListByReportingType
//                //$('#ReportingGridLoginActivityDetailDiv').hide();
//                //UpdateSearchCriteria();
//                $('#loader_event').hide();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 23) {
//        var jsonReportData = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            facilityId: facilityId,
//        });

//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetScrubberAndErrorSummaryReport',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonReportData,
//            success: function (data) {
//                $('#loader_event').hide();
//                
//                BindList("#ReportingGrid", data); //GetListByReportingType
//                //$('#ReportingGridLoginActivityDetailDiv').hide();
//                //UpdateSearchCriteria();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 24) {

//        var jsonReportData = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            facilityId: facilityId,
//        });

//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetScrubberAndErrorSummaryReport',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonReportData,
//            success: function (data) {
//                $('#loader_event').hide();
//                
//                BindList("#ReportingGrid", data); //GetListByReportingType
//                //$('#ReportingGridLoginActivityDetailDiv').hide();
//                //UpdateSearchCriteria();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 25) {

//        var encounterId = $("#ddlEncounter").val();
//        var transctionId = $("#ddlTransactionType").val();

//        var jsonUserTransctionReportData = JSON.stringify({
//            //reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            facilityId: facilityId,
//            transctionId: transctionId != null ? transctionId : 0,
//            encounterId: encounterId != null ? encounterId : 0
//        });

//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetDetailUserTransctionReport',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonUserTransctionReportData,
//            success: function (data) {
//                $('#loader_event').hide();
//                
//                BindList("#ReportingGrid", data); //GetListByReportingType
//                //$('#ReportingGridLoginActivityDetailDiv').hide();
//                //UpdateSearchCriteria();
//            },
//            error: function (msg) {
//            }
//        });
//    }

//    else if (reportingType == 27) {
//        var departmentId = $('#ddlDepartment').val();

//        var jsonDepartmentUtilizationReport = JSON.stringify({
//            //reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            facilityId: facilityId,
//            departmentId: departmentId == null ? 0 : departmentId
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetDepartmentUtilizationReport',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonDepartmentUtilizationReport,
//            success: function (data) {
//                $('#loader_event').hide();
//                
//                BindList("#ReportingGrid", data); //GetListByReportingType
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 30) {
//        var jsonFutureChargeReport = JSON.stringify({
//            //reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            facilityId: facilityId,
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetFutureChargeReport',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonFutureChargeReport,
//            success: function (data) {
//                $('#loader_event').hide();
//                
//                BindList("#ReportingGrid", data); //GetListByReportingType
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else if (reportingType == 31) {
//        var jsonDataScheduling = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            tillDate: new Date($("#txtTillDate").val()),
//            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
//            displayby: 1
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetListByReportingType',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonDataScheduling,
//            success: function (data) {
//                $('#loader_event').hide();
//                BindList("#ReportingGrid", data);
//                //$('#ReportingGridLoginActivityDetailDiv').hide();
//            },
//            error: function (msg) {
//            }
//        });
//    }
//    else {

//        var corporateId = $("#CorporateId").val();
//        var till = new Date($("#txtTillDate").val());

//        till.setDate(till.getDate() + 1); //Add one day into date

//        var jsonData = JSON.stringify({
//            reportingTypeId: reportingType,
//            fromDate: new Date($("#txtFromDate").val()),
//            //tillDate: new Date($("#txtTillDate").val()),
//            tillDate: till,

//            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
//            userId: $("#ddlUsers").val()
//        });
//        $.ajax({
//            type: "POST",
//            url: '/Reporting/GetListByReportingType',
//            async: false,
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            data: jsonData,
//            success: function (data) {
//                $('#loader_event').hide();
//                BindList("#ReportingGrid", data); //GetListByReportingType
//                $('#ReportingGridLoginActivityDetailDiv').hide();
//                //UpdateSearchCriteria();
//                //SetGridPaging('?', 'reportingTypeId?Ln=' + reportingTypeId + '&fromDate=' + fromDate + '&tillDate=' + tillDate + '&isAll=' + isAll + '&userId=' + userId + '&');
//            },
//            error: function (msg) {
//            }
//        });
//    }
//}

function ExportData(rdSelector, cssClass) {
    /// <summary>
    /// Exports the data.
    /// </summary>
    /// <param name="rdSelector">The rd selector.</param>
    /// <param name="cssClass">The CSS class.</param>
    /// <returns></returns>
    $(cssClass).attr("checked", false);
    $(rdSelector).prop("checked", "checked");
    var fileType = $(rdSelector).val();
}

function UpdateSearchCriteria() {
    /// <summary>
    /// Updates the search criteria.
    /// </summary>
    /// <returns></returns>
    var exportAsExcelLink = $(".exportToFile");
    var href = exportAsExcelLink.prop("href");
    if (href != undefined && href != null) {
        var fromDate = ConvertJSDateTimeToServerString($("#txtFromDate").val());
        var tillDate = ConvertJSDateTimeToServerString($("#txtTillDate").val());
        var isAll = $("#ShowAllRecords")[0].checked;
        if (fromDate != '' && tillDate != '') {
            href = href.replace("fromdate", fromDate);
            href = href.replace("tilldate", tillDate);
            href = href.replace("isall", isAll);
            href = encodeURI(href);
            exportAsExcelLink.prop("href", href);
        }
    }
}

function BindUsers() {
    /// <summary>
    /// Binds the users.
    /// </summary>
    /// <returns></returns>
    var userId = $("#UserId").val();
    if (userId != null) {
        $.ajax({
            type: "POST",
            url: '/Home/GetUsersByCorporateId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            success: function (data) {
                BindDropdownData(data, "#ddlUsers", "#UserId");
            },
            error: function (msg) {
            }
        });
    }
}

function InitializeDatesForReporting() {
    /// <summary>
    /// Initializes the dates for reporting.
    /// </summary>
    /// <returns></returns>
    $("#txtFromDate").datetimepicker({
        minDate: '-1990/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
        maxDate: '+2020/12/12',  //tommorow is maximum date calendar
        format: 'm/d/Y',
        mask: true,
        timepicker: false
    });

    $("#txtTillDate").datetimepicker({
        minDate: '-1990/12/12',//yesterday is minimum date(for today use 0 or -1970/01/01)
        maxDate: '+2020/12/12',  //tommorow is maximum date calendar
        format: 'm/d/Y',
        mask: true,
        timepicker: false
    });
}

function ShowSubReport(reportingId, itemId) {
    /// <summary>
    /// Shows the sub report.
    /// </summary>
    /// <param name="reportingId">The reporting identifier.</param>
    /// <param name="itemId">The item identifier.</param>
    /// <returns></returns>
    var jsonData = JSON.stringify({
        patientId: itemId
    });
    $.ajax({
        type: "POST",
        url: '/Reporting/GetRevenueReportByPatientId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonData,
        success: function (data) {
            $("#ReportingGridDetailDiv").show();
            BindList("#ReportingGridDetail1", data);
        },
        error: function (msg) {
        }
    });
}

function RowTotalColor() {
    /// <summary>
    /// Rows the total color.
    /// </summary>
    /// <returns></returns>
    $("#PayorAgeingReportGrid tbody tr:last").addClass('rowColor9');
    $("#PatientAgeingReportGrid tbody tr:last").addClass('rowColor9');
}

function ShowDetails(userid, logindate) {
    $("#hdUserId").val(userid);
    $("#loginDate").val(logindate);
    var jsonDataLoginActivity = JSON.stringify({
        userid: userid,
        tillDate: new Date(logindate),
    });
    $.ajax({
        type: "POST",
        url: '/Reporting/LoginDetailViewReport',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: jsonDataLoginActivity,
        success: function (data) {
            $('#ReportingGridLoginActivityDetailDiv').show();
            BindList("#ReportingGridLoginActivityDetail", data);
            $('html, body').animate({
                scrollTop: $("#ReportingGridLoginActivityDetailDiv").offset().top
            }, 2000);
        },
        error: function (msg) {
        }
    });
}

/// <var>The show patient aging by payor</var>
var ShowPatientAgingByPayor = function (payorId) {
    if (payorId != '') {
        var jsonData = JSON.stringify({
            payorId: payorId,
            date: $("#txtFromDate").val(),
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/AgeingReportByPayor',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                $('#ReportingGridDetailDiv').show();
                BindList("#ReportingGridDetail1", data);
                $('html, body').animate({
                    scrollTop: $("#ReportingGridDetailDiv").offset().top
                }, 2000);
                $("#PatientAgeingReportGrid tbody tr:last").addClass('rowColor9');

            },
            error: function (msg) {
            }
        });
    } else {
        $('#ReportingGridDetail1').empty();
    }
}

/// <var>bind the accounts dropdown</var>
var BindAccountsDropdown = function () {
    $.post("/FacilityDepartment/BindAccountDropdowns", null, function (data) {
        BindDropdownData(data.reveuneAccount, "#ddlDepartment", "0");
    });
};

/*--------------Sort Password Change Log Grid--------By krishna on 21082015---------*/

function SortPasswordLogGrid(event) {

    //var reportingType = $("#hdReportType").val();
    //var corporateId = $("#CorporateId").val();
    var url = "/Reporting/SortPasswordLogGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    var isAll = $('#ShowAllRecords').prop('checked') ? true : false;

    //var userId = $("#ddlUsers").val();

    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&isAll=" + isAll + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {

            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);
            //ReportingGrid

        },
        error: function (msg) {
        }
    });
}

/*--------------Sort Password Disable Log Grid--------By krishna on 21082015---------*/

function SortPasswordDisableLogGrid(event) {

    //var reportingType = $("#hdReportType").val();
    //var corporateId = $("#CorporateId").val();
    var url = "/Reporting/SortPasswordDisableLogGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    var isAll = $('#ShowAllRecords').prop('checked') ? true : false;

    var userId = $("#ddlUsers").val();

    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&isAll=" + isAll + "&userId=" + userId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {

            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);
            //ReportingGrid

        },
        error: function (msg) {
        }
    });
}


/*--------------Sort User Log Activity Grid--------By krishna on 21082015---------*/

function SortUserLogActivityGrid(event) {

    //var reportingType = $("#hdReportType").val();
    //var corporateId = $("#CorporateId").val();
    var url = "/Reporting/SortUserLogActivityGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    //var isAll = $('#ShowAllRecords').prop('checked') ? true : false;
    var isAll = $('#ShowAllRecords').prop('checked') ? true : false;
    var userId = $("#ddlUsers").val();
    if (userId == "") {
        userId = 0;
    }
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&isAll=" + isAll + "&userId=" + userId + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            ;
            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);
            //ReportingGrid

        },
        error: function (msg) {
        }
    });
}


/*--------------Sort Daily Charge Report Grid--------By krishna on 21082015---------*/

function SortDailyChargeReportGrid(event) {

    //var reportingType = $("#hdReportType").val();
    //var corporateId = $("#CorporateId").val();
    var departmenttype = $('#ddlDepartment').val();
    var url = "/Reporting/SortDailyChargeReportGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    departmentNumber: departmenttype != null ? departmenttype : '0';
    //var isAll = $('#ShowAllRecords').prop('checked') ? true : false;

    //var userId = $("#ddlUsers").val();

    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&departmentNumber=" + departmenttype + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {

            $("#gridContentIPChargesDetailReport").empty();
            $("#gridContentIPChargesDetailReport").html(data);
            //ReportingGrid

        },
        error: function (msg) {
        }
    });
}


/*--------------Sort Collection Report Report Grid--------By krishna on 21082015---------*/

function SortCollectionLogtGrid(event) {

    //var reportingType = $("#hdReportType").val();
    //var corporateId = $("#CorporateId").val();
    var url = "/Reporting/SortCollectionLogtGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    var isAll = $('#ShowAllRecords').prop('checked') ? true : false;

    //var userId = $("#ddlUsers").val();

    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&isAll=" + isAll + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            ;
            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);
            //ReportingGrid

        },
        error: function (msg) {
        }
    });
}

/*--------------Sort Claim Trans Report Grid--------By krishna on 21082015---------*/

function SortClaimTransReportGrid(event) {

    //var reportingType = $("#hdReportType").val();
    //var corporateId = $("#CorporateId").val();
    var url = "/Reporting/SortClaimTransReportGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    //var isAll = $('#ShowAllRecords').prop('checked') ? true : false;
    var displayby = $("#ddlDisplayBy").val();

    //var userId = $("#ddlUsers").val();

    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&displayby=" + displayby + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {

            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);
            //ReportingGrid

        },
        error: function (msg) {
        }
    });
}


/*--------------Sort payor Aging Report Grid--------By krishna on 21082015---------*/
function SortPayorAgingReport(event) {

    var url = "/Reporting/AgeingReport";
    var reportingType = $("#hdReportType").val();
    var reportingTypeId = reportingType;
    var date = $("#txtFromDate").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?reportingTypeId=" + reportingTypeId + "&date=" + date + "&" + event.data.msg;
    }

    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);
        },
        error: function (msg) {
        }
    });
}

/*--------------Sort payor Recancilation Report Grid--------By krishna on 21082015---------*/

function SortRecancilationReportGrid(event) {

    var url = "/Reporting/ReconciliationReport";
    var reportingTypeId = $("#hdReportType").val();
    var date = ($("#txtFromDate").val());
    var viewtype = $('#ddlViewType').val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?reportingTypeId=" + reportingTypeId + "&date=" + date + "&viewtype=" + viewtype + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);

        },
        error: function (msg) {
        }
    });
}


/*--------------Sort Revenu Forcast Report Grid--------By krishna on 21082015---------*/

function SortRevenuForcastReportGrid(event) {

    var url = "/Reporting/SortRevenuForcastReportGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());


    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);

        },
        error: function (msg) {
        }
    });
}


/*--------------Sort Journal Entry Support Report Grid--------By krishna on 27082015---------*/
function JournalEntrySupportReportGrid(event) {
    var url = "/Reporting/JournalEntrySupportReportGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    var displayby = $("#ddlDisplayBy").val();

    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&displayby=" + displayby + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);

        },
        error: function (msg) {
        }
    });
}



function SortDenialCodeReport(event) {
    var url = "/Reporting/SortDenialCodeGrid";
    var reportingType = $("#hdReportType").val();
    var date = $("#txtFromDate").val();
    var ddlDisplayDenialBy = $("#ddlDisplayDenialBy").val();
    var tilldate = $("#txtTillDate").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + date + "&tillDate=" + tilldate + "&displayby=" + ddlDisplayDenialBy + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);
            if (ddlDisplayDenialBy == '1') {
                hideColumnColorRow('3', 'DenialReportGrid');
                hideColumnColorRow('4', 'DenialReportGrid');
                hideColumnColorRow('5', 'DenialReportGrid');
            }
            else if (ddlDisplayDenialBy == '2') {
                hideColumnColorRow('4', 'DenialReportGrid');
                hideColumnColorRow('5', 'DenialReportGrid');
            }
            else if (ddlDisplayDenialBy == '3') {
                hideColumnColorRow('1', 'DenialReportGrid');
                hideColumnColorRow('4', 'DenialReportGrid');
                hideColumnColorRow('5', 'DenialReportGrid');
            }
            else if (ddlDisplayDenialBy == '4') {
                hideColumnColorRow('1', 'DenialReportGrid');
                hideColumnColorRow('3', 'DenialReportGrid');
            }
            else if (ddlDisplayDenialBy == '5') {
                hideColumnColorRow('1', 'DenialReportGrid');
            }
        },
        error: function (msg) {
        }
    });
}

function SortErrorDetailReport(event) {
    var url = "/Reporting/GetScrubberAndErrorSummaryReport";
    var reportingType = $("#hdReportType").val();
    var reportingTypeId = reportingType;
    var date = $("#txtFromDate").val();
    var ddlfacility = $("#ddlfacility").val();
    var tilldate = $("#txtTillDate").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?reportingTypeId=" + reportingTypeId + "&fromDate=" + date + "&tillDate=" + tilldate + "&facilityId=" + ddlfacility + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);
        },
        error: function (msg) {
        }
    });
}

function SortErrorSummaryReport(event) {
    var url = "/Reporting/GetScrubberAndErrorSummaryReport";
    var reportingType = $("#hdReportType").val();
    var reportingTypeId = reportingType;
    var date = $("#txtFromDate").val();
    var ddlfacility = $("#ddlfacility").val();
    var tilldate = $("#txtTillDate").val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?reportingTypeId=" + reportingTypeId + "&fromDate=" + date + "&tillDate=" + tilldate + "&facilityId=" + ddlfacility + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#ReportingGrid").empty();
            $("#ReportingGrid").html(data);
        },
        error: function (msg) {
        }
    });
}

function SortIPChargesReportGrid(event) {
    var url = "/Reporting/SortIPChargesReportGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    var dept = $('#ddlDepartment').val();
    var payor = $('#ddlPersonInsuranceCompany').val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&departmentNumber=" + dept + "&payorId=" + payor + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#InPatientActiveEncounterList").empty();
            $("#InPatientActiveEncounterList").html(data);
        },
        error: function (msg) {
        }
    });
}

function SortOPChargesReportGrid(event) {
    var url = "/Reporting/SortOPChargesReportGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    var dept = $('#ddlDepartment').val();
    var payor = $('#ddlPersonInsuranceCompany').val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&departmentNumber=" + dept + "&payorId=" + payor + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#OpPatientActiveEncounterList").empty();
            $("#OpPatientActiveEncounterList").html(data);
        },
        error: function (msg) {
        }
    });
}

function SortERChargesReportGrid(event) {
    var url = "/Reporting/SortERChargesReportGrid";
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    var dept = $('#ddlDepartment').val();
    var payor = $('#ddlPersonInsuranceCompany').val();
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&departmentNumber=" + dept + "&payorId=" + payor + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#ERPatientActiveEncounterList").empty();
            $("#ERPatientActiveEncounterList").html(data);
        },
        error: function (msg) {
        }
    });
}
/*Bind Payor Dropdown into Daily charge report*/
function BindInsuranceCompaniesData(selector) {
    $.getJSON("/Insurance/GetInsuranceCompaniesDropdownData", {}, function (data) {
        BindInsuranceDropdownData(data, selector, "");
    });
}


function BindFacilityDropdown() {
    if ($("#ddlfacility").length > 0) {
        var cId = $("#hdCorporateID").val();
        if (cId > 0) {
            $.ajax({
                type: "POST",
                url: "/Home/GetFacilitiesbyCorporate",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({ corporateid: cId }),
                success: function (data) {
                    BindDropdownData(data, "#ddlfacility", "");
                    if ($("#ddlfacility").val() == null)
                        $("#ddlfacility")[0].selectedIndex = 0;

                    if ($("#hdfacility").val() > 0) {
                        $("#ddlfacility").val($("#hdfacility").val());
                        $("#hdfacility").val('');
                        BindPhysicianDropdownData();
                        BindUsersDropdownInReporting();
                    }
                },
                error: function (msg) {

                }
            });
        }
        else {
            BindDropdownData('', "#ddlfacility", "");
            $("#ddlfacility")[0].selectedIndex = 0;
        }
        $("#hfCorporateId").val(cId > 0 ? cId : "");
    }
}

function BindPhysicianDropdownData() {
    var facilityId = $('#ddlfacility').val() != null ? $('#ddlfacility').val() : 0;
    if (facilityId > 0 && $("#ddlPhysician").length > 0) {
        var jsonData = JSON.stringify({ facilityId: facilityId });
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Reporting/GetPhysiciansByCorporateAndFacility",
            async: false,
            data: jsonData,
            dataType: "json",
            beforeSend: function () { },
            success: function (data) {

                $("#ddlPhysician").empty();
                $("#ddlPhysician").append('<option value="0">--Select One--</option>');

                /*
            data contains the JSON formatted list of codes 
            passed from the controller
            */
                $.each(data, function (i, code) {
                    $("#ddlPhysician").append('<option value="' + code.Id + '">' + code.PhysicianName + '</option>');
                });



            },
            error: function (msg) {


            }
        });
    }
}

function BindUsersDropdownInReporting() {
    
    /// <summary>
    /// Binds the users dropdown.
    /// </summary>
    /// <returns></returns>
    var facilityId = $('#ddlfacility').val() != null ? $('#ddlfacility').val() : 0;
    if (facilityId > 0 && $("#ddlUser").length > 0) {
        $.ajax({
            type: "POST",
            url: '/Reporting/GetUsersByCorporateIdAndFacilityId',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                facilityId: facilityId
            }),
            success: function (data) {
                BindDropdownData(data, "#ddlUser", "");
            },
            error: function (msg) {
            }
        });
    }
}

function BindEncounterDropdown() {

    /// <summary>
    /// Binds the users dropdown.
    /// </summary>
    /// <returns></returns>

    var userId = $('#ddlUser').val();
    var jsonReportData = JSON.stringify({
        userId: userId
    });
    $.ajax({
        type: "POST",
        url: '/Reporting/GetEncounterByUserId',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonReportData,
        success: function (data) {
            BindDropdownData(data, "#ddlEncounter", "");
        },
        error: function (msg) {
        }
    });
}


function SortPatientActivityReport(event) {

    var url = '/Reporting/GetPhysicianActivityReport';
    var facilityId = $('#ddlfacility').val() != null ? $('#ddlfacility').val() : 0;
    var physician = $('#ddlPhysician').val();
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());


    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?fromDate=" + fromDate + "&tillDate=" + tillDate + "&facilityId=" + facilityId + "&physicianId=" + physician + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {

            BindList("#ReportingGrid", data);
        },
        error: function (msg) {
        }
    });
}



function BindDepartmentDropdownData() {

    /// <summary>
    /// Binds the users dropdown.
    /// </summary>
    /// <returns></returns>


    var facilityId = $('#ddlfacility').val() != null ? $('#ddlfacility').val() : 0;
    var jsonReportData = JSON.stringify({

        facilityId: facilityId
    });
    $.ajax({
        type: "POST",
        url: '/Reporting/GetFacilityDeapartments',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonReportData,
        success: function (data) {
            BindDropdownData(data, "#ddlDepartment", "");
        },
        error: function (msg) {
        }
    });
}


function PhysicianUtilizationReport(event) {
    var url = '/Reporting/GetPhysicianActivityReport';
    var facilityId = $('#ddlfacility').val() != null ? $('#ddlfacility').val() : 0;
    var physician = $('#ddlPhysician').val();
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?reportingTypeId=" + 26 + "&fromDate=" + fromDate + "&tillDate=" + tillDate + "&facilityId=" + facilityId + "&physicianId=" + 0 + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {

            BindList("#ReportingGrid", data);
        },
        error: function (msg) {
        }
    });
}

function DepartmentUtilizationReport(event) {
    var url = '/Reporting/GetDepartmentUtilizationReport';
    var facilityId = $('#ddlfacility').val() != null ? $('#ddlfacility').val() : 0;
    //var departmentId = $('#departmentId').val();
    var fromDate = ($("#txtFromDate").val());
    var tillDate = ($("#txtTillDate").val());
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?reportingTypeId=" + 27 + "&fromDate=" + fromDate + "&tillDate=" + tillDate + "&facilityId=" + facilityId + "&departmentId=" + 0 + "&" + event.data.msg;
    }
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {

            BindList("#ReportingGrid", data);
        },
        error: function (msg) {
        }
    });
}




function SortUserLoginActivityReport(event) {
    var userId = $("#hdUserId").val();
    var tillDate = $("#loginDate").val();
    var url = '/Reporting/LoginDetailViewReport';
    if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
        url += "?userid=" + userId + "&tillDate=" + tillDate + "&" + event.data.msg;
    }

    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $('#ReportingGridLoginActivityDetailDiv').show();
            BindList("#ReportingGridLoginActivityDetail", data);
        },
        error: function (msg) {
        }
    });
}


function BindReportingData() {
    $('#loader_event').show();
    /// <summary>
    /// Binds the reporting data.
    /// </summary>
    /// <returns></returns>
    var reportingType = $("#hdReportType").val();
    var facilityId = $('#ddlfacility').length > 0 && $('#ddlfacility').val() != null ? $('#ddlfacility').val() : 0;
    var fromDate = $("#txtFromDate").val();
    var tillDate = $("#txtTillDate").length > 0 ? $("#txtTillDate").val() : '';

    if (reportingType == 6 || reportingType == 7 || reportingType == 8) {
        var jsonDataAgeing = JSON.stringify({
            reportingTypeId: reportingType,
            date: fromDate,
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/AgeingReport',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonDataAgeing,
            success: function (data) {
                BindList("#ReportingGrid", data);
                RowTotalColor();
                //SetGridPaging('?', '?reportingTypeId=' + reportingTypeId + '&');
                //GetListByReportingType
                //UpdateSearchCriteria();
                $('#loader_event').hide();
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 9 || reportingType == 10 || reportingType == 11) {
        var jsonDataReConcilation = JSON.stringify({
            reportingTypeId: reportingType,
            date: fromDate,
            viewtype: $('#ddlViewType').val()
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/ReconciliationReport',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonDataReConcilation,
            success: function (data) {
                BindList("#ReportingGrid", data);
                //GetListByReportingType
                //UpdateSearchCriteria();
                $('#loader_event').hide();
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 13) {
        var jsonDataClaimDetails = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
            displayby: $("#ddlDisplayBy").val()
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/GetListByReportingType',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonDataClaimDetails,
            success: function (data) {
                $('#loader_event').hide();
                BindList("#ReportingGrid", data);

            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 14) {
        var jsonDataDenialDetails1 = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
            displayby: $("#ddlDisplayDenialBy").val()
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/GetListByReportingType',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonDataDenialDetails1,
            success: function (data) {
                BindList("#ReportingGrid", data);
                $('#loader_event').hide();
                var displayBy = $("#ddlDisplayDenialBy").val();
                if (displayBy == '1') {
                    hideColumnColorRow('3', 'DenialReportGrid');
                    hideColumnColorRow('4', 'DenialReportGrid');
                    hideColumnColorRow('5', 'DenialReportGrid');
                }
                else if (displayBy == '2') {
                    hideColumnColorRow('4', 'DenialReportGrid');
                    hideColumnColorRow('5', 'DenialReportGrid');
                }
                else if (displayBy == '3') {
                    hideColumnColorRow('1', 'DenialReportGrid');
                    hideColumnColorRow('4', 'DenialReportGrid');
                    hideColumnColorRow('5', 'DenialReportGrid');
                }
                else if (displayBy == '4') {
                    hideColumnColorRow('1', 'DenialReportGrid');
                    hideColumnColorRow('3', 'DenialReportGrid');
                }
                else if (displayBy == '5') {
                    hideColumnColorRow('1', 'DenialReportGrid');
                }
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 15) {
        var jsonDataDenialDetails2 = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
            displayby: 1
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/GetListByReportingType',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonDataDenialDetails2,
            success: function (data) {
                $('#loader_event').hide();
                BindList("#ReportingGrid", data);
                $('#ReportingGridLoginActivityDetailDiv').hide();
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 16) {
        var jsonDataDenialDetails3 = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
            displayby: 2
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/GetListByReportingType',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonDataDenialDetails3,
            success: function (data) {
                $('#loader_event').hide();
                BindList("#ReportingGrid", data);
                $('#ReportingGridLoginActivityDetailDiv').hide();
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 17) {
        var jsonDataDenialDetails4 = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
            displayby: 3
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/GetListByReportingType',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonDataDenialDetails4,
            success: function (data) {
                $('#loader_event').hide();
                BindList("#ReportingGrid", data);
                $('#ReportingGridLoginActivityDetailDiv').hide();
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 19 || reportingType == 20) {
        var departmenttype = $('#ddlDepartment').val();
        var payorId = $('#ddlPersonInsuranceCompany').val();
        var jsonDataChargesReport = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            departmentNumber: departmenttype != null ? departmenttype : '0',
            payorId: payorId == null ? 0 : payorId,
        });

        $.ajax({
            type: "POST",
            url: '/Reporting/GetChargesReportsData',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonDataChargesReport,
            success: function (data) {
                BindList("#ReportingGrid", data); //GetListByReportingType
                //$('#ReportingGridLoginActivityDetailDiv').hide();
                //UpdateSearchCriteria();
                $('#loader_event').hide();
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 21 || reportingType == 26) {
        var physician = $('#ddlPhysician').val();
        var jsonPhysicianActivityReport = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            facilityId: facilityId,
            physicianId: physician == null ? 0 : physician


        });

        $.ajax({
            type: "POST",
            url: '/Reporting/GetPhysicianActivityReport',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonPhysicianActivityReport,
            success: function (data) {
                $('#loader_event').hide();
                
                BindList("#ReportingGrid", data); //GetListByReportingType
                //$('#ReportingGridLoginActivityDetailDiv').hide();
                //UpdateSearchCriteria();
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 22) {
        var jsonReportSrubSummaryData = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            facilityId: facilityId == null ? 0 : facilityId,
        });

        $.ajax({
            type: "POST",
            url: '/Reporting/GetScrubberAndErrorSummaryReport',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonReportSrubSummaryData,
            success: function (data) {
                
                BindList("#ReportingGrid", data); //GetListByReportingType
                //$('#ReportingGridLoginActivityDetailDiv').hide();
                //UpdateSearchCriteria();
                $('#loader_event').hide();
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 23) {
        var jsonReportData = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            facilityId: facilityId,
        });

        $.ajax({
            type: "POST",
            url: '/Reporting/GetScrubberAndErrorSummaryReport',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonReportData,
            success: function (data) {
                $('#loader_event').hide();
                
                BindList("#ReportingGrid", data); //GetListByReportingType
                //$('#ReportingGridLoginActivityDetailDiv').hide();
                //UpdateSearchCriteria();
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 24) {

        var jsonReportData1 = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            facilityId: facilityId,
        });

        $.ajax({
            type: "POST",
            url: '/Reporting/GetScrubberAndErrorSummaryReport',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonReportData1,
            success: function (data) {
                $('#loader_event').hide();
                
                BindList("#ReportingGrid", data); //GetListByReportingType
                //$('#ReportingGridLoginActivityDetailDiv').hide();
                //UpdateSearchCriteria();
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 25) {

        var encounterId = $("#ddlEncounter").val();
        var transctionId = $("#ddlTransactionType").val();

        var jsonUserTransctionReportData = JSON.stringify({
            //reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            facilityId: facilityId,
            transctionId: transctionId != null ? transctionId : 0,
            encounterId: encounterId != null ? encounterId : 0
        });

        $.ajax({
            type: "POST",
            url: '/Reporting/GetDetailUserTransctionReport',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonUserTransctionReportData,
            success: function (data) {
                $('#loader_event').hide();
                
                BindList("#ReportingGrid", data); //GetListByReportingType
                //$('#ReportingGridLoginActivityDetailDiv').hide();
                //UpdateSearchCriteria();
            },
            error: function (msg) {
            }
        });
    }

    else if (reportingType == 27) {
        var departmentId = $('#ddlDepartment').val();

        var jsonDepartmentUtilizationReport = JSON.stringify({
            //reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            facilityId: facilityId,
            departmentId: departmentId == null ? 0 : departmentId
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/GetDepartmentUtilizationReport',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonDepartmentUtilizationReport,
            success: function (data) {
                $('#loader_event').hide();
                
                BindList("#ReportingGrid", data); //GetListByReportingType
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 30) {
        var jsonFutureChargeReport = JSON.stringify({
            //reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            facilityId: facilityId,
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/GetFutureChargeReport',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonFutureChargeReport,
            success: function (data) {
                $('#loader_event').hide();
                
                BindList("#ReportingGrid", data); //GetListByReportingType
            },
            error: function (msg) {
            }
        });
    }
    else if (reportingType == 31) {
        var jsonDataScheduling = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
            displayby: 1
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/GetListByReportingType',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonDataScheduling,
            success: function (data) {
                $('#loader_event').hide();
                BindList("#ReportingGrid", data);
                //$('#ReportingGridLoginActivityDetailDiv').hide();
            },
            error: function (msg) {
            }
        });
    }
    else {

        //var till = tillDate;

        //till.setDate(till.getDate() + 1); //Add one day into date

        var jsonData = JSON.stringify({
            reportingTypeId: reportingType,
            fromDate: fromDate,
            tillDate: tillDate,
            //tillDate: till,
            isAll: $('#ShowAllRecords').prop('checked') ? true : false,
            userId: $("#ddlUsers").val()
        });
        $.ajax({
            type: "POST",
            url: '/Reporting/GetListByReportingType',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                $('#loader_event').hide();
                BindList("#ReportingGrid", data); //GetListByReportingType
                $('#ReportingGridLoginActivityDetailDiv').hide();
                //UpdateSearchCriteria();
                //SetGridPaging('?', 'reportingTypeId?Ln=' + reportingTypeId + '&fromDate=' + fromDate + '&tillDate=' + tillDate + '&isAll=' + isAll + '&userId=' + userId + '&');
            },
            error: function (msg) {
            }
        });
    }
}