function BindFavoriteDiagnosisData(data) {
    var favColumns = [{ "targets": 0, "visible": false },
    {
        "targets": 5,
        "mRender": function (data, type, full) {
            var openconfirm = "return OpenConfirmPopup('" + full[0] + "','Delete Favorite','',DeleteFavoriteDiagnosisRecord," + null + "); ";
            var adddia = "AddAsDiagnosis('" + full[5] + "')";
            var deleteicon = '<div style="display:flex;"><a href="#" class="AddAsDiagnosis" title="Add as Diagnosis" onclick="' + adddia + '" style="float: left; margin-right: 7px; margin-left: 5px; width: 15px;"><img class="img-responsive" src="../images/edit_small.png" /></a><a href="#" class="RemoveDiagnosis" title="Remove Favorite" onclick="' + openconfirm + '" style="float: left; margin-right: 7px; margin-left: 5px; width: 15px;"><img class="img-responsive" src= "../images/delete_small.png" /></a></div>';
            return deleteicon;
        }
    }];
    $('#diagnosisfav').dataTable({
        destroy: true,
        aaData: data,
        scrollY: "200px",
        scrollCollapse: true,
        bProcessing: true,
        paging: true,
        aoColumnDefs: favColumns
    });
}

function BindCurrentDiagnosisData(data) {
    var cColumns = [{ "targets": 0, "visible": false }, { "targets": 1, "visible": false },
    {
        "targets": 8,
        "mRender": function (data, type, full) {
            var DiagnosisType = full[8];
            var diagcode = full[3];

            var anchortags = "<div style='display:flex'>";
            var openconfirm = "return OpenConfirmPopup('" + full[0] + "','Delete Current Diagnosis','',DeleteDiagnosis,null); ";

            if (DiagnosisType == 1 || DiagnosisType == 2) {
                var edit = "EditCurrentDiagnosis('" + full[0] + "'); ";
                anchortags += '<a href="#" title="Edit Current Diagnosis" onclick="' + edit + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src= "../images/edit_small.png" /></a>';
            }
            var edit = "EditDiagnosisRecord('" + full[0] + "'); ";

            if (DiagnosisType == 3) {
                var addfav = 'AddAsFavDiagnosisInDiagnosis("' + diagcode + '", "9")';

                anchortags += '<a href="javascript:void(0);" class="hideSummary" title="Edit Current DRG" onclick="' + edit + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src="../images/edit_small.png" /></a>' +
                    '<a href="javascript:void(0);" class="hideSummary favdiag" title="Add As Favorite" onclick="' + addfav + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src="../images/Fav (1).png" /></a>';
            }
            if (DiagnosisType == 4) {
                anchortags += '<a href="javascript:void(0);" class="hideSummary" title="Edit Current CPT" onclick="' + edit + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src="../images/edit_small.png" /></a>';
            }
            if (DiagnosisType != 1) {
                anchortags += '<a href="javascript:void(0);" title="Delete Current Diagnosis" class="hideSummary" onclick="' + openconfirm + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src="../images/delete_small.png" /></a>';

            }
            if (DiagnosisType == 1 || DiagnosisType == 2) {
                var addfav = "AddAsFavDiagnosisInDiagnosis('" + diagcode + "', '16') ";
                anchortags += '<a href="javascript:void(0);" title="Add As Favorite" class="hideSummary favdiag" onclick="' + addfav + '" style="float: left; margin-right: 7px; width: 15px;"><img class="img-responsive" src="../images/Fav (1).png" /></a>';
            }
            return anchortags + "</div>";
        }
    }];
    $('#diagnosisCurrent').dataTable({
        destroy: true,
        aaData: data,
        scrollY: "200px",
        scrollCollapse: true,
        bProcessing: true,
        paging: true,
        aoColumnDefs: cColumns
    });
}