$(function () {

    /* Select product from specified category */

    $("#SelectCategory").on("change", function () {
        var url = $(this).val();

        if (url) {
            window.location = "/admin/shop/AddCategory?catId=" + url;
        }
        return false;
    });

    /*-----------------------------------------------------------*/

    /* Confirm product deletion */

    $("a.delete").click(function () {
        if (!confirm("Confirm category deletion")) return false;
    });

    /*-----------------------------------------------------------*/
});