$(function () {

    // Add new brands

    // Declare and initialize variables
    var newBrandA = $("a#newbra");            // Add link class
    var newBrandTextInput = $("#newbrname");  // Input text-box class
    var ajaxText = $("span.ajax-text");      // Class of picture wich are downloaded
    var table = $("table#pages tbody");      // Class of table output

    // Function to capture Enter
    newBrandTextInput.keyup(function (e) {
        if (e.keyCode == 13) { // keyCode == 13 (key code -  Enter)
            newBrandA.click();
        }
    });

    // Function Click
    newBrandA.click(function (e) {
        e.preventDefault();

        var brName = newBrandTextInput.val();

        if (brName.length < 3) {
            alert("Brand name must be at least 3 characters long.");
            return false;
        }

        ajaxText.show();

        var url = "/admin/shop/AddNewBrand";

        $.post(url, { brName: brName }, function (data) {
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
                    ajaxText.html("<span class='alert alert-success'>The brand has been added!</span>");
                    setTimeout(function () {
                        ajaxText.fadeOut("fast", function () {
                            ajaxText.html("<img src='/Content/img/ajax-loader.gif' height='50' />");
                        });
                    }, 2000);

                    newBrandTextInput.val("");

                    var toAppend = $("table#pages tbody tr:last").clone();
                    toAppend.attr("id", "id_" + data);
                    toAppend.find("#item_Name").val(brName);
                    toAppend.find("a.delete").attr("href", "/admin/shop/DeleteBrand/" + data);
                    table.append(toAppend);
                    table.sortable("refresh");
                }
            }
        });
    });

    //-------------------------------------------------------------

    // Confirm brand deletion

    $("body ").on("click", "a.delete", function () {
        if (!confirm("Confirm brand deletion")) return false;
    });
}); 