$(function () {

    // Add new categories

    // Declare and initialize variables
    var newCatA = $("a#newcata");            // Add link class
    var newCatTextInput = $("#newcatname");  // Input text-box class
    var ajaxText = $("span.ajax-text");      // Class of picture wich are downloaded
    var table = $("table#pages tbody");      // Class of table output

    // Function to capture Enter
    newCatTextInput.keyup(function (e) {
        if (e.keyCode == 13) { // keyCode == 13 (key code -  Enter)
            newCatA.click();
        }
    });

    // Function Click
    newCatA.click(function (e) {
        e.preventDefault();

        var catName = newCatTextInput.val();

        if (catName.length < 3) {
            alert("Category name must be at least 3 characters long.");
            return false;
        }

        ajaxText.show();

        var url = "/admin/shop/AddNewCategory";

        $.post(url, { catName: catName }, function (data) {
            var response = data.trim();

            if (response == "titletaken") {
                ajaxText.html("<span class='alert alert-danger'>That title is taken!</span>");
                setTimeout(function () {
                    ajaxText.fadeOut("fast", function () {
                        ajaxText.html("<img src='/Content/img/ajax-loader.gif' height='50' />");
                    });
                }, 2000);
                return false;
            }
            else {
                if (!$("table#pages").length) {
                    location.reload();
                }
                else {
                    ajaxText.html("<span class='alert alert-success'>The category has been added!</span>");
                    setTimeout(function () {
                        ajaxText.fadeOut("fast", function () {
                            ajaxText.html("<img src='/Content/img/ajax-loader.gif' height='50' />");
                        });
                    }, 2000);

                    newCatTextInput.val("");

                    var toAppend = $("table#pages tbody tr:last").clone();
                    toAppend.attr("id", "id_" + data);
                    toAppend.find("#item_Name").val(catName);
                    toAppend.find("a.delete").attr("href", "/admin/shop/DeleteCategory/" + data);
                    table.append(toAppend);
                    table.sortable("refresh");
                }
            }
        });
    });

    //-------------------------------------------------------------

    // Confirm category deletion

    $("body ").on("click", "a.delete", function () {
        if (!confirm("Confirm category deletion")) return false;
    });
}); 