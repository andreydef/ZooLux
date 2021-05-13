/* Increment product */
$(function () {

    $("button.incproduct").click(function (e) {
        e.preventDefault();

        var productId = $(this).data("id");
        var url = "/cart/IncrementProduct";

        $.getJSON(url,
            { productId: productId },
            function (data) {
                $("input.qty" + productId).html(data.qty);

                var price = data.qty * data.price;
                var priceHtml = "$" + price.toFixed(2);

                $("td.price" + productId).html(priceHtml);

                var gt = parseFloat($("td.grandtotal span").text());
                var grandtotal = (gt + data.price).toFixed(2);

                $("td.grandtotal span").text(grandtotal);
                /*Урок 26*/
            }).done(function () {
                var url2 = "/cart/PaypalPartial";

                $.get(url2,
                    {},
                    function (data) {
                        $("div.paypaldiv").html(data);
                    });
                window.location.reload();
            });
    });
    /*-----------------------------------------------------------*/

    /* Decriment product */
    $(function () {

        $("button.decproduct").click(function (e) {
            e.preventDefault();

            var $this = $(this);
            var productId = $(this).data("id");
            var url = "/cart/DecrementProduct";

            $.getJSON(url,
                { productId: productId },
                function (data) {

                    if (data.qty == 0) {
                        $this.parent().fadeOut("fast",
                            function () {
                                location.reload();
                            });
                    } else {
                        $("td.qty" + productId).html(data.qty);

                        var price = data.qty * data.price;
                        var priceHtml = "$" + price.toFixed(2);

                        $("td.price" + productId).html(priceHtml);

                        var gt = parseFloat($("td.grandtotal span").text());
                        var grandtotal = (gt - data.price).toFixed(2);

                        $("td.grandtotal span").text(grandtotal);
                    }
                    /*Урок 26*/
                }).done(function () {

                    var url2 = "/cart/PaypalPartial";

                    $.get(url2,
                        {},
                        function (data) {
                            $("div.paypaldiv").html(data);
                        });
                    window.location.reload();
                });
        });
    });

    /*-----------------------------------------------------------*/

    /* Confirm product deletion on cart */

    $("button.removeproduct").click(function () {
        if (!confirm("Confirm product deletion")) return true;
    });

    // Function to capture Enter
    $("button.removeproduct").keyup(function (e) {
        if (e.keyCode == 13) { // keyCode == 13 (key code -  Enter)
            $("button.removeproduct").click();
        }
    });

    /*-----------------------------------------------------------*/

    /* Remove product */
    $(function () {

        $("button.removeproduct").click(function (e) {
            e.preventDefault();

            var productId = $(this).data("id");
            var url = "/cart/RemoveProduct";

            $.get(url,
                { productId: productId },
                function () {
                    location.reload();
                });
            window.location.reload();
        });
    });

    /* Place order */
    $(function () {

        $("a.placeorder").click(function (e) {
            e.preventDefault();

            var url = "/cart/PlaceOrder";

            $.post(url,
                {},
                function () {
                    $("span.message").text("Thank you. You will now be redirected to paypal.");
                    $('form input[name = "submit"]').click();
                });
        });
    });
});