$(function () {
    /*-----------------------------------------------------------*/

    /* Confirm feature deletion */

    $("a.delete").click(function () {
        if (!confirm("Confirm feature deletion")) return false;
    });

    /*-----------------------------------------------------------*/
});