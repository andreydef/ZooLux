﻿$(function () {

    /* Select product from specified category(admin) */
<<<<<<< HEAD

=======
>>>>>>> b83509fab7edc5605acb5de78e5ed69d2cdcd23a
    $("#SelectCategory").on("change", function () {
        var url = $(this).val();

        if (url) {
            window.location = "/admin/shop/Products?catId=" + url;
        }
        return false;
    });

    /*-----------------------------------------------------------*/

    /* Confirm product deletion */
<<<<<<< HEAD

    $(".delete a").click(function () {
       if (!confirm("Confirm product deletion")) return false;
    });

    $(function () {

        $("a.delete").click(function (e) {
            e.preventDefault();

            var productId = $(this).data("id");
            var url = "/admin/shop/DeleteProduct";

            $.get(url, { productId: productId }, function (data) {
                location.reload();
            });
        });
    });

    /*-----------------------------------------------------------*/
=======
    $(".delete a").click(function () {
        if (!confirm("Confirm product deletion")) return false;
    });

    $(function () {

        $("a.delete").click(function (e) {
            e.preventDefault();

            var productId = $(this).data("id");
            var url = "/admin/shop/DeleteProduct";

            $.get(url, { productId: productId }, function (data) {
                location.reload();
            });
        });
    });
>>>>>>> b83509fab7edc5605acb5de78e5ed69d2cdcd23a
});