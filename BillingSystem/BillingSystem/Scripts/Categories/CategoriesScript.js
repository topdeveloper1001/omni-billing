$(function () {
    $("#CategoryFormDiv").validationEngine();
    

});

function SaveCategory(id) {
    var isValid = jQuery("#CategoryFormDiv").validationEngine({ returnIsValid: true });
    if (isValid == true) {
        var txtId = $("#Id").val();
        var txtProdCatNumber = $("#txtProdCatNumber").val();
        var txtProdCat = $("#txtProdCat").val();
        var txtProdSubcat = $("#txtProdSubcat").val();
        var txtProdSubcat2 = $("#txtProdSubcat2").val();
        var txtProdSubcat3 = $("#txtProdSubcat3").val();
        
        var jsonData = JSON.stringify({
            Id: txtId,
            ProdCatNumber: txtProdCatNumber,
            ProdCat: txtProdCat,
            ProdSubcat: txtProdSubcat,
            ProdSubcat2: txtProdSubcat2,
            ProdSubcat3: txtProdSubcat3
            
        });
        $.ajax({
            type: "POST",
            url: '/Categories/SaveCategories',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData,
            success: function (data) {
                if (data == "-1") {
                    ShowMessage("Category Aleady Exist", "Warning", "warning", true);
                    return false;
                }
                

                //$("#chkShowInActive").prop("checked", false);
                ClearCategoryForm();
                BindCategoryGrid();

                var msg = "Records Saved successfully !";
                if (id > 0)
                    msg = "Records updated successfully";

                ShowMessage(msg, "Success", "success", true);
            },
            error: function (msg) {

            }
        });
    }
}

function BindCategoryGrid() {
    
    //var jsonData = JSON.stringify({
    //    showInActive: showInActive
    //});
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Categories/BindCategoriesList",
        dataType: "html",
        async: true,
        data: null,
        success: function (data) {
            $('#collapseCategoriesList').addClass('in').attr('style', 'height:auto;');
            $("#CategoriesListDiv").empty();
            $("#CategoriesListDiv").html(data);
            
        },
        error: function (msg) {

        }

    });
}

function ClearCategoryForm() {
    
    $("#CategoryFormDiv").clearForm();
    $('#collapseCategoriesList').addClass('in');
    $("#btnSaveCategory").val("Save");
    $("#Id").val(0);
    
}

function EditCategory(id) {
    var jsonData = JSON.stringify({
        Id: id
    });
    $.ajax({
        type: "POST",
        url: '/Categories/GetCategoriesData',
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            $('#collapseCategoriesAddEdit').addClass('in').attr('style', 'height:auto;');
            BindCategories(data);
        },
        error: function (msg) {

        }
    });
}

function BindCategories(data) {
    $("#Id").val(data.Id);
    $("#txtProdCatNumber").val(data.ProdCatNumber);
    $("#txtProdCat").val(data.ProdCat);
    $("#txtProdSubcat").val(data.ProdSubcat);
    $("#txtProdSubcat2").val(data.ProdSubcat2);
    $("#txtProdSubcat3").val(data.ProdSubcat3);
    
    $("#btnSaveCategory").val("Update");
    $('#collapseOne').addClass('in');

}



function DeleteCategory() {
    if ($("#hfGlobalConfirmId").val() > 0) {
        var jsonData = JSON.stringify({
            Id: $("#hfGlobalConfirmId").val()
        });
        $.ajax({
            type: "POST",
            url: '/Categories/DeleteCategory',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: jsonData,
            success: function (data) {
                if (data) {
                    BindCategoryGrid();
                    ShowMessage("Records Deleted Successfully", "Sucess", "success", true);
                }
                else {
                    return false;
                }
            },
            error: function (msg) {
                return true;
            }
        });
    }
}



function SortCategoriesGrid(event) {
    
    var url = "/Categories/BindCategoriesList";
    //if (event.data != null && (event.data.msg != null || event.data.msg != undefined || event.data.msg != '')) {
    //    url += "?showInActive=" + showInActive + "&" + event.data.msg;
    //}
    $.ajax({
        type: "POST",
        url: url,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        data: null,
        success: function (data) {
            $("#CategoriesListDiv").empty();
            $("#CategoriesListDiv").html(data);
        },
        error: function () {
        }
    });
}
